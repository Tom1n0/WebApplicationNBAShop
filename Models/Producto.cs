using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationNBAShop.Models;

public partial class Producto
{
    [Key]
    public int IdProducto { get; set; }
    [Required]
    [StringLength(50)]
    public string? Codigo { get; set; } = null!;
    [Required]
    [StringLength(255)]
    public string Nombre { get; set; } = null!; 
    [Required]
    [StringLength(255)]
    public string? Modelo { get; set; } = null!;
    [Required]
    [StringLength(1000)]
    public string Descripcion { get; set; } = null!;
    [Required]
    public decimal Precio { get; set; }
    [Required]
    [StringLength(255)]
    public string? Imagen { get; set; } = null!;
    [Required]
    public int? IdCategoria { get; set; } = null!;
    [Required]
    public int? Stock { get; set; } = null!;
    [Required]
    public bool Activo { get; set; }
    [Required]   
    public string Marca { get; set; } = null!;
    [Required]
    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
    
    //public Categoria Categoria { get; set; } = null!;
    public virtual Categoria IdCategoriaNavigation { get; set; }
    //public Categoria Categoria { get; set; }

}
