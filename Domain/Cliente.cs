using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendSpa.Domain
{

    [Table("clientes")]
    public class Cliente
    {
        [Key]
        [Column("id_cliente")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCliente { get; set; }

        [Column("nombre")]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Column("email")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Column("telefono")]
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
