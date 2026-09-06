using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendSpa.Domain
{
    [Table("categorias")]
    public class Categoria
    {
        [Key]
        [Column("id_categoria")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCategoria { get; set; }

        [Column("nombre")]
        [MaxLength(100)]
        [MinLength(2)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
    }
}
