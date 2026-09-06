# 🌿 Spa Volta Vida — Backend Web API (.NET 10)

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2B%20CQRS-blue?style=for-the-badge)](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
[![Database](https://img.shields.io/badge/Database-MySQL-4479A1?style=for-the-badge&logo=mysql)](https://www.mysql.com/)
[![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?style=for-the-badge&logo=docker)](https://www.docker.com/)

RESTful API backend desarrollada en **.NET 10 C#** para la gestión integral de reservaciones, cotización de servicios, procesamiento automatizado de pagos y envío de notificaciones en tiempo real para el centro de bienestar **Spa Volta Vida**.

---

## 📐 Arquitectura y Funcionamiento del Sistema

El proyecto está diseñado bajo los principios de **Clean Architecture** y el patrón **CQRS (Command Query Responsibility Segregation)** con **MediatR**.

```text
[ Cliente / Web Frontend ]
          │
          ▼
   [ Controller API ]
          │
          ▼
 [ MediatR Pipeline ] ──► (Global Exception Handling Middleware & Rate Limiter)
          │
    ┌─────┴─────────────────────────────┐
    ▼                                   ▼
[ Commands / Queries ]        [ Background Services ]
  - GetCreateCita               - CitasPendientesJob (Cron 15m)
  - ProcesarWebhook (MP)        - NotificacionesFallidasJob (Cron 1h, Max Retries)
  - CancelarCita (Auth Phone)
  - GetEstaDisponible
    │
    ├──────────────┬────────────────────┬───────────────────┐
    ▼              ▼                    ▼                   ▼
[ EF Core ]   [ Mercado Pago ]    [ Twilio WhatsApp ]   [ SendGrid / SMTP ]
 (MySQL)        (Pasarela)          (Lookup + Msg)        (Email Backup)
```

### 🔑 Aspectos Clave de Implementación

1. **Control de Concurrencia y Reserva Crítica:**
   * Al crear una cita, se inicia una transacción en MySQL utilizando bloqueo pesimista (`FOR UPDATE`) para evitar reservas duplicadas en el mismo rango horario en peticiones simultáneas.
2. **Pasarela de Pagos (Mercado Pago API):**
   * Creación automática de `Preferences` de pago para cobrar únicamente el anticipo configurado (50%).
   * Reescritura idempotente del estado del pago a través del Webhook (`api/pagos/webhook`).
   * Manejo automatizado de reembolsos en caso de detectar pagos duplicados.
3. **Notificaciones Multicanal (WhatsApp & Email Backup):**
   * Envío de confirmaciones inmediatas vía Twilio WhatsApp y respaldo mediante correo electrónico con el ID de la cita para garantizar la entrega en caso de fallas de red o errores de tipeo.
4. **Tareas en Segundo Plano y Resiliencia en Mensajería:**
   * **`CitasPendientesJob`**: Cancela automáticamente reservas en estado `Pendiente` que hayan superado 1 hora sin registrar pago de anticipo.
   * **`NotificacionesFallidasJob`**: Mecanismo de reintento con límite máximo de reintentos (*Max Retries*) y *Exponential Backoff* para evitar la saturación de APIs externas.
5. **Resiliencia & Rate Limiting:**
   * Políticas de reintento HTTP exponenciales con **Polly** para integración de pagos y mensajería.
   * Rate Limiter integrado por ventana fija (`citas-policy` y `servicios-policy`).

---

## 🚧 Deuda Técnica & Próximas Mejoras (Roadmap)

Se han identificado e incorporado en el backlog los siguientes puntos de refactorización y robustecimiento técnico:

1. **🔒 Autenticación para Cancelación de Citas:**
   * Implementación de verificación de identidad (mediante número telefónico registrado, token único o PIN de confirmación) al solicitar un `DELETE` en `/api/citas/cancelarcita/{id}`, evitando cancelaciones no autorizadas.
2. **📱 Validación Avanzada de Número Telefónico:**
   * Integración de validación previa mediante **Twilio Lookup API** / formato E.164 estricto al registrar la cita, previniendo el almacenamiento de números inexistentes o con formato erróneo.
3. **📧 Canal de Respaldo por Correo Electrónico (Email Notification):**
   * Implementación de envío de email (vía SendGrid/SMTP) incluyendo el **ID de Cita** y los detalles de la reserva. Esto asegura que el cliente tenga acceso a su comprobante incluso si el número de WhatsApp fue escrito con errores o no es accesible.
4. **🔄 Control y Límite de Reintentos de Notificación (Max Retries):**
   * Configuración de un número máximo de reintentos (*Threshold Max Retries = 3*) en `NotificacionesFallidasJob` y alertas tras sobrepasar el límite, evitando bucles infinitos y cuotas agotadas en proveedores de SMS/WhatsApp.

---

## 🛠️ Tech Stack

* **Lenguaje & Framework:** C# / .NET 10.0 Web API
* **Patrón de Mediador:** MediatR v12
* **ORM:** Entity Framework Core 9.0 (Pomelo MySQL)
* **Base de Datos:** MySQL / MariaDB
* **Integraciones:** Mercado Pago SDK REST, Twilio API (WhatsApp + Lookup), SendGrid / SMTP
* **Resiliencia HTTP:** Polly
* **Contenedor:** Docker / Multi-stage builds

---

## 📂 Estructura del Proyecto

```text
BackendSpa/
├── Application/                # Capa de Aplicación (CQRS, DTOs, Handlers, Interfaces)
│   ├── Common/                 # Pipeline Behaviors y modelos genéricos de respuesta
│   ├── Features/               # Módulos de negocio (Citas, Clientes, Pagos, Servicios)
│   └── Interfaces/             # Contratos para DbContext, Pagos, Email y Notificaciones
├── Controllers/                # Endpoints HTTP (CitasController, PagosController, etc.)
├── Domain/                     # Entidades del Dominio, Enums e Interfaces puras
├── Infrastructure/             # Implementación de persistencia, Twilio, Email, MercadoPago y Jobs
│   ├── BackgroundServices/     # Tareas programadas (.NET HostedServices con Max Retries)
│   ├── Persistance/            # DbContext y mapeos de EF Core
│   └── Services/               # Servicios de terceros (Twilio, EmailService, MercadoPago)
├── Middlewares/                # Middleware global de manejo de excepciones
├── SQL/                        # Scripts de estructura e inserción de la BD MySQL
├── Dockerfile                  # Construcción multi-etapa optimizada
├── appsettings.json            # Configuración base de la aplicación
└── Program.cs                  # Punto de entrada y composición de servicios
```

---

## ⚙️ Configuración (`appsettings.json`)

Antes de ejecutar la aplicación, debes reemplazar las credenciales y URLs dentro de `appsettings.json` (o mediante variables de entorno):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MercadoPago": {
    "AccessToken": "TU_ACCESS_TOKEN_MERCADO_PAGO",
    "WebhookUrl": "https://tu-dominio.com/api/pagos/webhook",
    "UrlBase": "https://tu-dominio.com"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=spa_volta_vida;Uid=root;Pwd=tu_password;"
  },
  "NumberPhone": {
    "PhoneNumber": "+523300000000"
  },
  "Twilio": {
    "AccountSid": "TU_TWILIO_ACCOUNT_SID",
    "AuthToken": "TU_TWILIO_AUTH_TOKEN",
    "PhoneNumber": "+14155238886",
    "MaxNotificationRetries": 3
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "notificaciones@spavoltavida.com",
    "SenderPassword": "TU_APP_PASSWORD"
  }
}
```

---

## 🗄️ Base de Datos MySQL

1. Crea la base de datos `spa_volta_vida` en tu servidor MySQL/MariaDB.
2. Ejecuta el script SQL incluido en el repositorio:
   ```bash
   mysql -u root -p spa_volta_vida < SQL/spa_volta_vida.sql
   ```

---

## 🐳 Docker: Compilación y Ejecución Local

El proyecto incluye un `Dockerfile` optimizado en múltiples etapas (*multi-stage build*) para mantener la imagen de producción ligera utilizando la imagen runtime oficial de .NET 10.

### 1. Compilar la Imagen Docker Localmente

Asegúrate de estar en la raíz del proyecto (donde se ubica el archivo `Dockerfile`) y ejecuta:

```bash
docker build -t backend-spa:latest -f Dockerfile .
```

### 2. Ejecutar el Contenedor Localmente

> **💡 Recomendación de configuración:** Para mayor comodidad, define previamente las credenciales, cadenas de conexión (`DefaultConnection`) y claves API directamente en el archivo `appsettings.json` antes de construir la imagen. De esta forma, evitas pasar banderas largas de variables de entorno (`-e`) en la línea de comandos.

Una vez configurado el archivo JSON, ejecuta el contenedor de manera limpia exponiendo el puerto `8080`:

```bash
docker run -d -p 8080:8080 --name backend-spa-app backend-spa:latest
```

---

## 📌 Principales Endpoints API

### 📅 Citas (`/api/citas`)
* **`POST /api/citas`**: Registra una nueva cita, valida el teléfono/email, bloquea el horario en BD y genera la URL de checkout de Mercado Pago para el anticipo.
* **`GET /api/citas/disponibilidad`**: Consulta si un rango de fecha y hora se encuentra libre antes de proceder a la reserva.
* **`DELETE /api/citas/cancelarcita/{id}`**: Cancela manualmente una cita validando la identidad del cliente (teléfono / token de confirmación) y notifica vía WhatsApp/Email.

### 💳 Pagos (`/api/pagos`)
* **`POST /api/pagos/webhook`**: Endpoint expuesto para recibir las notificaciones asíncronas IPN/Webhook de Mercado Pago, confirmar la cita y disparar las notificaciones multicanal (WhatsApp + Email).

### 💆 Servicios (`/api/servicios`)
* **`GET /api/servicios`**: Lista todos los servicios activos del Spa agrupados por categoría.
* **`GET /api/servicios/{id}`**: Consulta el detalle y precio de un servicio específico.

---

## 📄 Licencia

Este proyecto se distribuye bajo la licencia MIT. Consulta el archivo `LICENSE` para obtener más información.