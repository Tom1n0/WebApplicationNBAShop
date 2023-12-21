using WebApplicationNBAShop.Models;

namespace WebApplicationNBAShop.Services
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> GetCategorias();
    }
}
