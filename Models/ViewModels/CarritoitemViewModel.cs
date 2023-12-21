namespace WebApplicationNBAShop.Models.ViewModels
{
    public class CarritoitemViewModel
    {
        public int IdProducto { get; set; }
        public Producto Producto { get; set; } = null!;
        public string Nombre { get; set;} = null!;
        public decimal Precio { get; set;}
        public int Cantidad { get; set; }
        public decimal SubTotal => Precio * Cantidad;
    }
}
