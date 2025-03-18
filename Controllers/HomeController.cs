using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Gestion_Ventas.Models;

namespace Proyecto_Gestion_Ventas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        private AccesoDatos _acceso;

        public HomeController(AccesoDatos acceso)
        {
            _acceso = acceso;
        }

        //Metodos de Cliente
        //Crear cliente




        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }








        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
