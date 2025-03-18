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

        [HttpPost]
        public IActionResult AgregarCliente(Cliente modelo)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", modelo);
            }
            try
            {
                _acceso.AgregarCliente(modelo);

                //si al agregar el usuario es exitoso
                TempData["SuccessMessage"] = "Tu cliente se guardó con éxito.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["SuccessMessage"] = "Tu cliente no se guardó." + ex.Message;
                return View("Index", modelo);
            }
        }



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
