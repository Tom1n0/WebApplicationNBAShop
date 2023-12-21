using WebApplicationNBAShop.Models;
using WebApplicationNBAShop.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationNBAShop.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ApplicationDbContext _context;
        public CategoriaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> GetCategorias()
        {
            return await _context.Categoria.ToListAsync();
        }
    }
}
