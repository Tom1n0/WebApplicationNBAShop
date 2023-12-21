using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationNBAShop.Data;
using WebApplicationNBAShop.Models;

namespace WebApplicationNBAShop.Controllers
{
    public class PerfilController : BaseController
    {
        public PerfilController(ApplicationDbContext context): base (context)
        {
              
        }

        public async Task<IActionResult> Details(int id)
        {
            if(id== 0)
                return NotFound();

            var usuario= await _context.Usuarios
                .Include(u=>u.Direcciones)
                .FirstOrDefaultAsync(u=>u.IdUsuario== id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        public IActionResult AgregarDireccion(int id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarDireccion(Direccion direccion, int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
                if (usuario != null)
                {
                    direccion.IdUsuarioNavigation = usuario;
                }

                _context.Add(direccion);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id });
            }
            catch (SystemException)
            {
                return View(direccion);
            }


            
        }
    }
}
