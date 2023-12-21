using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationNBAShop.Models;

public partial class Categoria
{
    [Key]
    public int IdCategoria { get; set; } 

    [Required(ErrorMessage = "El campo nombre es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Required]
    [StringLength(1000)]
    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
