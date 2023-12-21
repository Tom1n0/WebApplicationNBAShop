using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using WebApplicationNBAShop.Data;
using WebApplicationNBAShop.Models;

namespace WebApplicationNBAShop.Controllers
{
    [Authorize(policy: "RequireAdminOrStaff")]
    public class UsuariosController : BaseController
    {


        public UsuariosController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Usuarios.Include(u => u.IdRolNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["IdRol"] = new SelectList(_context.Rols, "IdRol", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,Nombre,Telefono,NombreUsuario,Contraseña,Correo,Direccion,Ciudad,Provincia,CodigoPostal,IdRol")] Usuario usuario)
        {
            // Asigno a una variable global llamada rol, el valor de un rol que se encuentre en base al IdRol que el usuario seleccionó.
            var rol = await _context.Rols
                .Where(d => d.IdRol == usuario.IdRol)
                .FirstOrDefaultAsync();

            if (rol != null)
            { 
                // En caso de que sea distinto a null, lo asigno con el rol encontrado.
                usuario.IdRolNavigation =rol;

                // Asigno la propiedad Direcciones, con una nueva direccion.
                usuario.Direcciones = new List<Direccion>
                {
                    new Direccion
                    {
                        Adress=usuario.Direccion,
                        Ciudad=usuario.Ciudad,
                        Provincia=usuario.Provincia,
                        CodigoPostal=usuario.CodigoPostal
                    }
                };    


                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRol"] = new SelectList(_context.Rols, "IdRol", "Nombre", usuario.IdRol);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["IdRol"] = new SelectList(_context.Rols, "IdRol", "Nombre", usuario.IdRol);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,Nombre,Telefono,NombreUsuario,Contraseña,Correo,Direccion,Ciudad,Provincia,CodigoPostal,IdRol")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            var rol = await _context.Rols
               .Where(d => d.IdRol == usuario.IdRol)
               .FirstOrDefaultAsync();

            if (rol != null)
            {

                var usuarioExistente = await _context.Usuarios
                    .Include(u=> u.Direcciones)
                    .FirstOrDefaultAsync(u=>u.IdUsuario==id);

                if (usuarioExistente != null)
                {
                    // Si las direcciones del usuarioExistente son mayores a 0
                    if (usuarioExistente.Direcciones.Count > 0)
                    {
                        // Se crea una nueva direccion y asgino los valores
                        var direccion = usuarioExistente.Direcciones.First();
                        direccion.Adress = usuario.Direccion;
                        direccion.Ciudad = usuario.Ciudad;
                        direccion.Provincia = usuario.Provincia;
                        direccion.CodigoPostal = usuario.CodigoPostal;
                    }
                    else
                    {
                        // Crear una nueva direcciones y asociarla al usuario
                        usuarioExistente.Direcciones = new List<Direccion>
                        {
                            // Asignar los parámetros a una nueva direccion
                            new Direccion
                            {
                                Adress= usuario.Direccion,
                                Ciudad= usuario.Ciudad,
                                Provincia= usuario.Provincia,
                                CodigoPostal= usuario.CodigoPostal
                            }
                        };
                    }
                    usuarioExistente.IdRolNavigation = rol;
                    usuarioExistente.IdRol =usuario.IdRol;
                    usuarioExistente.Nombre = usuario.Nombre;
                    usuarioExistente.Telefono = usuario.Telefono;
                    usuarioExistente.NombreUsuario = usuario.NombreUsuario;
                    usuarioExistente.Contraseña = usuario.Contraseña;
                    usuarioExistente.Correo = usuario.Correo;


                    try
                    {
                        _context.Update(usuarioExistente);
                        await _context.SaveChangesAsync();

                    }
                    // En caso de que el try no se guarde:
                    catch(DbUpdateConcurrencyException)
                    {
                        // Valido si el usuario existe por medio de us Id
                        if(!UsuarioExists(usuario.IdUsuario))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }

            }
            ViewData["IdRol"] = new SelectList(_context.Rols, "IdRol", "Nombre", usuario.IdRol);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
          return (_context.Usuarios?.Any(e => e.IdUsuario == id)).GetValueOrDefault();
        }
    }
}
