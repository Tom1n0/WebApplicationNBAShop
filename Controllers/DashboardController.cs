using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationNBAShop.Data;

namespace WebApplicationNBAShop.Controllers
{
    [Authorize(policy: "RequireAdminOrStaff")]
    public class DashboardController : BaseController
    {
        public DashboardController(ApplicationDbContext context) : base(context)
        {
            
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
