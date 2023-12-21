using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using PayPalCheckoutSdk.Orders;
using System.Data.Common;
using System.Diagnostics;
using WebApplicationNBAShop.Data;
using WebApplicationNBAShop.Models;
using WebApplicationNBAShop.Models.ViewModels;

namespace WebApplicationNBAShop.Controllers
{
    public class BaseController : Controller
    {
        public readonly ApplicationDbContext _context;
        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public override ViewResult View(string? viewName, object? model)
        {
            ViewBag.NumeroProductos = GetCarritoCount();
            return base.View(viewName, model);
        }

        protected int GetCarritoCount()
        {
            // Inicializa la variable 'count' con un valor de 0.
            int count = 0;

            // Obtiene el valor de la cookie llamada "carrito" de la solicitud actual.
            string? carritoJson = Request.Cookies["carrito"];

            // Verifica si la cadena obtenida de la cookie no es nula ni vacía.
            if (!string.IsNullOrEmpty(carritoJson))
            {
                // Deserializa la cadena JSON en una lista de objetos 'ProductoIdAndCantidad'.
                var carrito = JsonConvert.DeserializeObject<List<ProductoIdAndCantidad>>(carritoJson);
                // Verifica si la lista deserializada no es nula.
                if (carrito != null)
                {
                    // Actualiza la variable 'count' con la cantidad de elementos en la lista 'carrito'.
                    count = carrito.Count;
                }
            }
            // Devuelve el valor final de 'count'.
            return count;
        }

        public async Task<CarritoViewModel> AgregarProductoAlCarrito(int productoId, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(productoId);

            if (producto != null)
            {
                var carritoViewModel = await GetCarritoViewModelAsync();

                var carritoItem = carritoViewModel.Items.FirstOrDefault(item => item.IdProducto == productoId);

                if (carritoItem != null)
                {
                    carritoItem.Cantidad += cantidad;
                }
                else
                {
                    carritoViewModel.Items.Add(new CarritoitemViewModel
                    {
                        IdProducto = producto.IdProducto,
                        Nombre = producto.Nombre,
                        Precio = producto.Precio,
                        Cantidad = cantidad
                    });
                }


                carritoViewModel.Total = carritoViewModel.Items.Sum(
                    item => item.Cantidad * item.Precio);

                await UpdateCarritoViewModelAsync(carritoViewModel);
                return carritoViewModel;
            }

            return new CarritoViewModel();
        }

        public async Task UpdateCarritoViewModelAsync(CarritoViewModel carritoViewModel)
        {
            var productoIds = carritoViewModel.Items.Select(
                item=> new ProductoIdAndCantidad
                {
                    ProductoId = item.IdProducto,
                    Cantidad=item.Cantidad
                })
                .ToList();

            var carritoJson = await Task.Run(() => JsonConvert.SerializeObject(productoIds));
            Response.Cookies.Append(
                "carrito",
                carritoJson,
                new CookieOptions { Expires=DateTimeOffset.Now.AddDays(7) });
        }

        public async Task<CarritoViewModel> GetCarritoViewModelAsync()
        {
            var carritoJson = Request.Cookies["carrito"];

            if (string.IsNullOrEmpty(carritoJson))
                return new CarritoViewModel();

            var productoIdsAndCantidades= JsonConvert.DeserializeObject<List<ProductoIdAndCantidad>>(carritoJson);

            var carritoViewModel = new CarritoViewModel();

            if (productoIdsAndCantidades != null)
            {
                foreach(var item in productoIdsAndCantidades)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if(producto  != null)
                    {
                        carritoViewModel.Items.Add(
                            new CarritoitemViewModel
                            {
                                IdProducto = producto.IdProducto,
                                Nombre = producto.Nombre,
                                Precio = producto.Precio,
                                Cantidad = item.Cantidad
                            });
                    }
                }
            }
            carritoViewModel.Total = carritoViewModel.Items.Sum(item => item.SubTotal);
            return carritoViewModel;
        }

        protected IActionResult HandleError(Exception e)
        {
            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        protected IActionResult HandleDbError(DbException dbException)
        {
            var viewModel = new DbErrorViewModel
            {
                ErrorMessage = "Error de base de datos",
                Details = dbException.Message
            };
            return View("DbError", viewModel);
        }

        protected IActionResult HandleDbUpdateError(DbUpdateException dbUpdateException)
        {
            var viewModel = new DbErrorViewModel
            {
                ErrorMessage = "Error de actualización de base de datos",
                Details = dbUpdateException.Message
            };
            return View("DbError", viewModel);
        }
    }
}
