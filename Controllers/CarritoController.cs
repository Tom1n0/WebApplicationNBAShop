using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Security.Claims;
using WebApplicationNBAShop.Data;
using WebApplicationNBAShop.Models;
using WebApplicationNBAShop.Models.ViewModels;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Core;
using Microsoft.Build.Evaluation;
using PayPalHttp;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Logging;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationNBAShop.Controllers
{
    public class CarritoController : BaseController
    {
        public CarritoController(ApplicationDbContext context) : base(context)
        {
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var carritoViewModel= await GetCarritoViewModelAsync();

            foreach(var item in carritoViewModel.Items)
            {
                var producto = await _context.Productos.FindAsync(item.IdProducto);
                if (producto != null)
                {
                    item.Producto = producto;

                    if (!producto.Activo)
                        carritoViewModel.Items.Remove(item);
                    else
                        item.Cantidad = Math.Min((byte)item.Cantidad, (byte)producto.Stock);                   
                }
                else
                    item.Cantidad = 0;
            }
            carritoViewModel.Total = carritoViewModel.Items.Sum(item => item.SubTotal);
            // Validar que el usuario tenga una direccion de envio válida. Si el usuario es true (IsAuthenticated == true) asigna a la variable usuarioId ese identificador, en caso contrario se asigna un 0.
            var usuarioId = User.Identity?.IsAuthenticated == true ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : 0;

            // Si es true, se agarra el contexto.Direcciones y con un where para encontrar con una expresion lambda que el IdUsuario coincida con el usuarioId, en caso contrario se inicializa una nueva lista de direcciones.
            var direcciones = User.Identity?.IsAuthenticated == true ? _context.Direcciones.Where(d => d.IdUsuario == usuarioId).ToList() : new List<Direccion>();

            var prodecerConCompraViewModel = new ProcederConCompraViewModel
            {
                Carrito = carritoViewModel,
                Direcciones = direcciones
            };

            return View(prodecerConCompraViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarCantidad(int id, int cantidad)
        {
            var carritoViewModel = await GetCarritoViewModelAsync();
            var carritoItem = carritoViewModel.Items.FirstOrDefault(i=>i.IdProducto==id);

            if(carritoItem!=null)
            {
                carritoItem.Cantidad=cantidad;
                var producto = await _context.Productos.FindAsync(id);
                if(producto!=null && producto.Activo && producto.Stock>0)
                    carritoItem.Cantidad = Math.Min((byte)cantidad, (byte)producto.Stock);


                await UpdateCarritoViewModelAsync(carritoViewModel);
            }

            return RedirectToAction("Index", "Carrito");
        }


        [HttpPost]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var carritoViewModel = await GetCarritoViewModelAsync();
            var carritoItem = carritoViewModel.Items.FirstOrDefault(i => i.IdProducto == id);

            if (carritoItem != null)
            {
                carritoViewModel.Items.Remove(carritoItem);
                await UpdateCarritoViewModelAsync(carritoViewModel);
            }

            return RedirectToAction("Index", "Carrito");
        }

        [HttpPost]
        public async Task<IActionResult> VaciarCarrito()
        {
            await RemoveCarritoViewModelAsync();
            return RedirectToAction("Index");
        }
        private async Task RemoveCarritoViewModelAsync()
        {
            await Task.Run(() => Response.Cookies.Delete("carrito"));
        }

        // Informacion de cuenta de paypal
        private readonly string clientId = "AejeP8ZODtvc-5DRMP-CfhhhNYRM5KODOQFvY2wgSIvGUFy2Gs_fBesIpE9zvDlrq9FMHHKkXkwakj61";
        private readonly string clientSecret = "EIL2avIWS2L103Y8CmyujS0RAZPAvnKY-1x7SNpREWdrA6AN65-Bjr54KZvcU6clI_CDrIUYlyJiS5D_";
        public IActionResult ProcederConCompra(decimal montoTotal, int IdDireccionSeleccionada)
        {
            if (IdDireccionSeleccionada > 0)
            {
                Response.Cookies.Append(
                    "direccionseleccionada", IdDireccionSeleccionada.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
            }
            else
            {
                return View("Index");
            }

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody(montoTotal));

            var enviroment = new SandboxEnvironment(clientId, clientSecret);
            var client = new PayPalHttpClient(enviroment);

            try
            {
                var response = client.Execute(request).Result;
                var statusCode = response.StatusCode;
                var responseBody = response.Result<Order>();

                var approveLink = responseBody.Links.FirstOrDefault(x => x.Rel == "approve");
                if (approveLink != null)
                    return Redirect(approveLink.Href);
                else
                    return RedirectToAction("Error");
            }
            catch(HttpException e)
            {
                return (IActionResult)e;
            }

        }

        private OrderRequest BuildRequestBody(decimal montoTotal)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var request= new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode="USD",
                            Value=montoTotal.ToString("F2").Replace(",", ".")
                        }
                    }
                },
                ApplicationContext = new ApplicationContext()
                {
                    ReturnUrl = $"{baseUrl}/Carrito/PagoCompletado",
                    CancelUrl = $"{baseUrl}/Carrito/Index"
                }
            };
            return request;
        }

        public IActionResult PagoCompletado()
        {
            try
            {
                var carritoJson = Request.Cookies["carrito"];

                int direccionId = 0;

                // Obtener el valor de la cookie "direccionseleccionada", la retorno en una variable string cookieValue y hacer el parse de esa cookieValue a entero, si eso se logra se retorna en un parámetro parseValue.
                if (Request.Cookies.TryGetValue("direccionseleccionada", out string? cookieValue) && int.TryParse(cookieValue, out int parseValue))
                {
                    // Y acá se asigna
                    direccionId = parseValue;
                }
                List<ProductoIdAndCantidad>? productoIdAndCantidads = !string.IsNullOrEmpty(carritoJson) ? JsonConvert.DeserializeObject<List<ProductoIdAndCantidad>>(carritoJson) : null;

                CarritoViewModel carritoViewModel = new();

                if (productoIdAndCantidads != null)
                {
                    foreach(var item in productoIdAndCantidads)
                    {
                        var producto = _context.Productos.Find(item.ProductoId);
                        if (producto != null)
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

                var usuarioId = User.Identity?.IsAuthenticated == true ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : 0;

                carritoViewModel.Total = carritoViewModel.Items.Sum(i => i.SubTotal);
                var pedido = new Pedido()
                {
                    IdUsuario = usuarioId,
                    Fecha = DateTime.UtcNow,
                    Estado = "Vendido",
                    IdDireccionSeleccionada = direccionId,
                    Total = carritoViewModel.Total
                };

                _context.Pedidos.Add(pedido);
                _context.SaveChanges();

                foreach(var item in carritoViewModel.Items)
                {
                    var pedidoDetalle = new DetallePedido
                    {
                        IdPedido = pedido.IdPedido,
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad,
                        Precio = item.Precio
                    };

                    _context.DetallePedidos.Add(pedidoDetalle);

                    var producto = _context.Productos.FirstOrDefault(p => p.IdProducto == item.IdProducto);

                    if (producto != null)
                        producto.Stock -= item.Cantidad;
                }
                _context.SaveChanges();

                Response.Cookies.Delete("carrito");
                Response.Cookies.Delete("direccionseleccionada");

                ViewBag.DetallePedidos = _context.DetallePedidos
                    .Where(dp => dp.IdPedido == pedido.IdPedido)
                    .Include(dp => dp.Producto)
                    .ToList();

                return View("PagoCompletado", pedido);

            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }
    }
}
