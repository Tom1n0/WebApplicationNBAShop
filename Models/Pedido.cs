using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationNBAShop.Models;

public partial class Pedido
{
    [Key]
    public int IdPedido { get; set; }
    [Required]
    public int IdUsuario { get; set; }
    [Required]
    public DateTime Fecha { get; set; }
    [Required]
    public string? Estado { get; set; }
    [Required]
    public int IdDireccionSeleccionada { get; set; }
    [Required]
    public Direccion Direccion { get; set; } = null!; 
    public decimal Total { get; set; }
    [Required]
    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
    [Required]
    public virtual Direccion? IdDireccionSeleccionadaNavigation { get; set; } 
    [Required]
    [ForeignKey("IdUsuario")]
    public Usuario Usuario { get; set; } = null!;
    public virtual Usuario? IdUsuarioNavigation { get; set; } = null!;
}
