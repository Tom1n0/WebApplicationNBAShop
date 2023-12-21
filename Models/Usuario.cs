using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationNBAShop.Models;

public partial class Usuario
{
    [Key]
    public int IdUsuario { get; set; }
    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;
    [Required]
    [StringLength(15)]
    public string Telefono { get; set; } = null!;
    [Required]
    [StringLength(50)]
    public string NombreUsuario { get; set; } = null!;
    [Required]
    [StringLength(255)]
    public string Contraseña { get; set; } = null!;
    [Required]
    [StringLength(255)]
    public string Correo { get; set; } = null!;
    [Required]
    [StringLength(100)]
    public string Direccion { get; set; } = null!;
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
    public int IdRol { get; set; }
    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
    public virtual Rol? IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
