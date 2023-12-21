namespace WebApplicationNBAShop.Models.ViewModels
{
    public class CarritoViewModel
    {
        public List<CarritoitemViewModel> Items { get; set; } = new List<CarritoitemViewModel>();
        public decimal Total { get; set; }
    }
}
