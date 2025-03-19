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
        // GET: Home/ActualizarCliente/5
        public IActionResult ActualizarCliente(int id)
        {
            try
            {
                // Obtener el cliente por ID usando el procedimiento almacenado
                Cliente cliente = _accesoDatos.ObtenerClientePorId(id);

                if (cliente == null)
                {
                    return NotFound();
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar el cliente: " + ex.Message;
                return RedirectToAction("ObtenerTodosClientes");
            }
        }

        // POST: Home/ActualizarCliente
        [HttpPost]
        public IActionResult ActualizarCliente(Cliente cliente)
        {
            try
            {
                // Establecer quién modifica el registro
                cliente.ModificadoPor = "Usuario Actual"; // Idealmente, esto vendría de tu sistema de autenticación

                // Actualizar el cliente
                bool resultado = _accesoDatos.ActualizarCliente(cliente);

                if (resultado)
                {
                    // Redirigir a la lista de clientes después de actualizar
                    return RedirectToAction("ObtenerTodosClientes");
                }
                else
                {
                    ViewBag.Error = "No se pudo actualizar el cliente";
                    return View(cliente);
                }
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