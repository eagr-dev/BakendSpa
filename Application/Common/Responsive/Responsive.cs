namespace BackendSpa.Application.Common.Responsive
{
    public record Responsive<T>(bool Success, string Mensaje, T? Data = default);
}
