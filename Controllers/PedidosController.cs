using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationNBAShop.Data;
using WebApplicationNBAShop.Models;

namespace WebApplicationNBAShop.Controllers
{
    [Authorize(policy: "RequireAdminOrStaff")]
    public class PedidosController : BaseController
    {

        public PedidosController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: Pedidoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Pedidos.Include(p => p.IdDireccionSeleccionadaNavigation).Include(p => p.IdUsuarioNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Pedidoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.IdDireccionSeleccionadaNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .Include(p=>p.DetallePedidos)
                .ThenInclude(dp=>dp.Producto)
                .FirstOrDefaultAsync(m => m.IdPedido == id);
            if (pedido == null)
            {
                return NotFound();
            }
            pedido.Direccion = await _context.Direcciones
                .FirstOrDefaultAsync(d => d.IdDireccion == pedido.IdDireccionSeleccionada)
                ?? new Direccion();

            return View(pedido);
        }

        // GET: Pedidoes/Create
        public IActionResult Create()
        {
            ViewData["IdDireccionSeleccionada"] = new SelectList(_context.Direcciones, "IdDireccion", "Adress");
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Ciudad");
            return View();
        }

        // POST: Pedidoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPedido,IdUsuario,Fecha,Estado,IdDireccionSeleccionada,Total")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdDireccionSeleccionada"] = new SelectList(_context.Direcciones, "IdDireccion", "Adress", pedido.IdDireccionSeleccionada);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Ciudad", pedido.IdUsuario);
            return View(pedido);
        }

        // GET: Pedidoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            ViewData["IdDireccionSeleccionada"] = new SelectList(_context.Direcciones, "IdDireccion", "Adress", pedido.IdDireccionSeleccionada);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Ciudad", pedido.IdUsuario);
            return View(pedido);
        }

        // POST: Pedidoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPedido,IdUsuario,Fecha,Estado,IdDireccionSeleccionada,Total")] Pedido pedido)
        {
            if (id != pedido.IdPedido)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.IdPedido))
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
            ViewData["IdDireccionSeleccionada"] = new SelectList(_context.Direcciones, "IdDireccion", "Adress", pedido.IdDireccionSeleccionada);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Ciudad", pedido.IdUsuario);
            return View(pedido);
        }

        // GET: Pedidoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.IdDireccionSeleccionadaNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdPedido == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pedidos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Pedidos'  is null.");
            }
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
          return (_context.Pedidos?.Any(e => e.IdPedido == id)).GetValueOrDefault();
        }
    }
}
