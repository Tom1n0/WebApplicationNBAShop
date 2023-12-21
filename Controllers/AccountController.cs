using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Security.Claims;
using WebApplicationNBAShop.Data;
using WebApplicationNBAShop.Models;

namespace WebApplicationNBAShop.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(ApplicationDbContext context) : base(context)
        {
        }

        [AllowAnonymous] // Acceso a cualquier usuario.
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost] //Metodo post.
        // Recibe un modelo de tipo usuario, y debe de contener todos los datos que necesitamos para ejecutar un registro.
        public async Task<IActionResult> Register(Usuario usuario)
        {
            try
            {
                if (usuario != null)
                {
                    if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario))
                    {
                        ModelState.AddModelError(nameof(usuario.NombreUsuario), "El nombre de usuario ya esta en uso.");
                        return View(usuario);
                    }
                    // Asignar el rol de cliente al usuario que estamos intentado registrar.
                    var clienteRol = await _context.Rols.FirstOrDefaultAsync(r => r.Nombre == "Cliente");

                    if (clienteRol != null)
                    {
                        usuario.IdRol = clienteRol.IdRol;
                    }
                    // Asignar una nueva direccion al usuario registrado.
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

                    _context.Usuarios.Add(usuario);
                    await _context.SaveChangesAsync();
                    // Autenticar al usuario.
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    // Establecer valores.
                    identity.AddClaim(new Claim(ClaimTypes.Name, usuario.NombreUsuario));
                    identity.AddClaim(new Claim(ClaimTypes.Role, "Cliente"));

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                    return RedirectToAction("Index", "Home");
                }
                return View(usuario);
            }
            // Usar DbException para retornar uno de los metodos de retorno de error programado.
            catch (DbException DbException)
            {
                return HandleDbError(DbException);
            }
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Implementar la logica para autenticar al usuario.
            try
            {
                var user= await _context.Usuarios.FirstOrDefaultAsync(u=>u.NombreUsuario==username && u.Contraseña==password); 
                if(user != null)
                {
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    // Establecer valores.
                    identity.AddClaim(new Claim(ClaimTypes.Name, username));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()));

                    var rol = await _context.Rols.FirstOrDefaultAsync(r => r.IdRol == user.IdRol);
                    if (rol != null)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, rol.Nombre));
                    }
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                    if (rol != null)
                    {
                        if (rol.Nombre == "Administrador" || rol.Nombre == "Staff")
                            return RedirectToAction("Index", "Dashboard");
                        else
                            return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Credenciales inválidas.");
                return View();

            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View(); 
        }

    }
}
