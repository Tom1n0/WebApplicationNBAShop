using WebApplicationNBAShop.Models;
using WebApplicationNBAShop.Models.ViewModels;

namespace WebApplicationNBAShop.Services
{
    public interface IProductoService
    {
        Producto GetProducto(int id);

        Task<List<Producto>> GetProductoDestacados();

        Task<ProductosPaginadosViewModel> GetProductoPaginados(int? categoriaId, string? busqueda, int pagina, int productosPorPagina);
    }
}
