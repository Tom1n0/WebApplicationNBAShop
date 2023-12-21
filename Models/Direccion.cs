using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationNBAShop.Models;

public partial class Direccion
{
    [Key]
    public int IdDireccion { get; set; }
    [Required]
    [StringLength(50)]
    public string Adress { get; set; } = null!;
    [Required]
    [StringLength(20)]
    public string Ciudad { get; set; } = null!;
    [Required]
    [StringLength(20)]
    public string Provincia { get; set; } = null!;
    [Required]
    [StringLength(10)]
    public string CodigoPostal { get; set; } = null!;
    [Required]
    [ForeignKey("IdUsuario")]
    public int IdUsuario { get; set; }
    [InverseProperty("Direcciones")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
