using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using WebApplicationNBAShop.Data;
using WebApplicationNBAShop.Models;

namespace WebApplicationNBAShop.Controllers
{
    [Authorize(policy: "RequireAdminOrStaff")]
    public class ProductosController : BaseController
    {

        public ProductosController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: Productos
        public async Task<IActionResult> Index(string buscar)
        {
            var productos= from producto in _context.Productos select producto;

            if (!string.IsNullOrEmpty(buscar))
            {
                productos= productos.Where(p=>p.Nombre!.Contains(buscar));
            }
            return View(await productos.ToListAsync());

            //var applicationDbContext = _context.Productos.Include(p => p.IdCategoriaNavigation);
            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productoes/Create
        public IActionResult Create()
        {
            ViewData["IdCategoria"] = new SelectList(_context.Categoria, "IdCategoria", "Descripcion");
            return View();
        }

        // POST: Productoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProducto,Codigo,Nombre,Modelo,Descripcion,Precio,Imagen,IdCategoria,Stock,Activo,Marca")] Producto producto)
        {
            var cat = await _context.Categoria
                .Where(c => c.IdCategoria == producto.IdCategoria)
                .FirstOrDefaultAsync();
            if (cat != null)
            {
                producto.IdCategoriaNavigation = cat;
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            //if (ModelState.IsValid)
            //{
            //    _context.Add(producto);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));

            ViewData["IdCategoria"] = new SelectList(_context.Categoria, "IdCategoria", "Descripcion", producto.IdCategoria);
            return View(producto);
        }


        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["IdCategoria"] = new SelectList(_context.Categoria, "IdCategoria", "Descripcion", producto.IdCategoria);
            return View(producto);
        }

        // POST: Productoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProducto,Codigo,Nombre,Modelo,Descripcion,Precio,Imagen,IdCategoria,Stock,Activo,Marca")] Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }
                var cat = await _context.Categoria
                        .Where(c => c.IdCategoria == producto.IdCategoria)
                        .FirstOrDefaultAsync();
            if (cat != null)
                {
                    producto.IdCategoriaNavigation = cat;
                    try
                    {
                        _context.Update(producto);
                        await _context.SaveChangesAsync();
                    }
                    catch(DbUpdateConcurrencyException)
                    {
                        if (!ProductoExists(producto.IdProducto))
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

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(producto);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!ProductoExists(producto.IdProducto))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
                //}
                /*return RedirectToAction(nameof(Index));*/
            //}
            ViewData["IdCategoria"] = new SelectList(_context.Categoria, "IdCategoria", "Descripcion", producto.IdCategoria);
            return View(producto);
        }

        // GET: Productoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Productos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Productos'  is null.");
            }
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
          return (_context.Productos?.Any(e => e.IdProducto == id)).GetValueOrDefault();
        }
    }
}
