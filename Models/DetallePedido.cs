using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationNBAShop.Models;

public partial class DetallePedido
{
    [Key]
    public int IdDetallePedido { get; set; }
    [Required]
    public int IdPedido { get; set; }
    [Required]
    public int IdProducto { get; set; }
    [Required]
    public int Cantidad { get; set; }
    [Required]
    public decimal Precio { get; set; } 
    [Required]
    [ForeignKey("IdPedido")]
    public Pedido Pedido { get; set; } = null!;
    public virtual Pedido? IdPedidoNavigation { get; set; } = null!;
    [Required]
    [ForeignKey("IdProducto")]
    public Producto Producto { get; set; } = null!;
    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
