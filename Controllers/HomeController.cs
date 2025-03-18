using Microsoft.AspNetCore.Mvc;
using Proyecto_Gestion_Ventas.Models;
using System.Diagnostics;

namespace Proyecto_Gestion_Ventas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AccesoDatos _accesoDatos;

        public HomeController(ILogger<HomeController> logger, AccesoDatos accesoDatos)
        {
            _logger = logger;
            _accesoDatos = accesoDatos;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Home/AgregarCliente
        public IActionResult AgregarCliente()
        {
            return View();
        }

        // POST: Home/AgregarCliente
        [HttpPost]
        public IActionResult AgregarCliente(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Si el usuario no está autenticado, asignar un valor predeterminado
                    if (string.IsNullOrEmpty(cliente.AdicionadoPor))
                    {
                        cliente.AdicionadoPor = "Sistema";
                    }

                    // Usar la clase AccesoDatos para agregar el cliente
                    int idCliente = _accesoDatos.AgregarCliente(cliente);
                    TempData["SuccessMessage"] = "Cliente registrado con éxito. ID: " + idCliente;
                    return RedirectToAction("AgregarCliente");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(cliente);
        }
     

        //Este es el metodo de obtener los clientes

        public IActionResult ObtenerTodosClientes()
        {
            try
            {
                // Obtener todos los clientes
                List<Cliente> clientes = _accesoDatos.ObtenerTodosClientes();

                // Pasar la lista de clientes a la vista
                return View(clientes);
            }
            catch (Exception ex)
            {
                // Manejar el error
                ViewBag.Error = "Error al cargar los clientes: " + ex.Message;
                return View(new List<Cliente>());
            }
        }

        //Este es el metodo de Actualizar los clientes

        // GET: Home/EditarCliente/{id}
        public IActionResult EditarCliente(int id)
        {
            try
            {
                // Obtener el cliente por ID
                var clientes = _accesoDatos.ObtenerTodosClientes();
                var cliente = clientes.FirstOrDefault(c => c.IdCliente == id);

                if (cliente == null)
                {
                    return NotFound();
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al obtener el cliente: " + ex.Message;
                return View("Error");
            }
        }

        // POST: Home/EditarCliente
        [HttpPost]
        public IActionResult EditarCliente(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Configurar propiedades de auditoría
                    cliente.FechaModificacion = DateTime.Now;
                    cliente.ModificadoPor = User.Identity?.Name ?? "Sistema";

                    // Actualizar el cliente
                    bool resultado = _accesoDatos.ActualizarCliente(cliente);

                    if (resultado)
                    {
                        TempData["SuccessMessage"] = "Cliente actualizado correctamente.";
                        return RedirectToAction("ObtenerTodosClientes");
                    }
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar el cliente: " + ex.Message;
                return View(cliente);
            }
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