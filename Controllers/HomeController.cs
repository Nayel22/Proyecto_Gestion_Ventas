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

        // Método para mostrar el formulario de actualización
        public IActionResult ActualizarCliente(int id)
        {
            try
            {
                var cliente = _accesoDatos.ObtenerClientePorId(id);
                if (cliente == null)
                {
                    ViewBag.Error = "Cliente no encontrado";
                    return RedirectToAction("ObtenerTodosClientes");
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("ObtenerTodosClientes");
            }
        }

        // Método para procesar la actualización
        [HttpPost]
        public IActionResult GuardarCliente(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _accesoDatos.ActualizarCliente(cliente);
                    return RedirectToAction("ObtenerTodosClientes");
                }
                return View("ActualizarCliente", cliente);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("ActualizarCliente", cliente);
            }
        }

        // Acción para eliminar un cliente
        public IActionResult EliminarCliente(int id)
        {
            try
            {
                // Llamar al método EliminarCliente de AccesoDatos
                bool resultado = _accesoDatos.EliminarCliente(id);

                if (resultado)
                {
                    TempData["Mensaje"] = "Cliente eliminado correctamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el cliente";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el cliente: " + ex.Message;
            }

            // Redirigir a la lista de clientes
            return RedirectToAction("ObtenerTodosClientes");
        }

        ////////////////Vamos ha empezar con los procedimientos almacenados de PRODUCTO//////////////////////////


        // GET: Home/AgregarProducto
        public IActionResult AgregarProducto()
        {
            return View();
        }

        // POST: Home/AgregarProducto
        [HttpPost]
        public IActionResult AgregarProducto(Producto producto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Asignar la fecha de registro
                    producto.FechaRegistro = DateTime.Now;

                    // Llamar al método de la capa de acceso a datos
                    int idProducto = _accesoDatos.AgregarProducto(producto);

                    // Mostrar mensaje de éxito
                    TempData["SuccessMessage"] = "Producto agregado correctamente con ID: " + idProducto;

                    // Redirect a la misma página para limpiar el formulario
                    return RedirectToAction(nameof(AgregarProducto));
                }
                return View(producto);
            }
            catch (Exception ex)
            {
                // Mostrar mensaje de error
                ViewBag.Error = ex.Message;
                return View(producto);
            }
        }

        // GET: Home/ObtenerTodosProductos
        public IActionResult ObtenerTodosProductos()
        {
            try
            {
                List<Producto> productos = _accesoDatos.ObtenerTodosProductos();
                return View(productos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<Producto>());
            }
        }

        // GET: Home/ActualizarProducto/5
        public IActionResult ActualizarProducto(int id)
        {
            try
            {
                // Obtener el producto por ID
                Producto producto = _accesoDatos.ObtenerProductoPorId(id);

                if (producto == null)
                {
                    TempData["Error"] = "Producto no encontrado.";
                    return RedirectToAction("ObtenerTodosProductos");
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: Home/GuardarProducto
        [HttpPost]
        public IActionResult GuardarProducto(Producto producto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("ActualizarProducto", producto);
                }

                int resultado = _accesoDatos.ActualizarProducto(producto);

                if (resultado == 1)
                {
                    TempData["Mensaje"] = "Producto actualizado correctamente.";
                    return RedirectToAction("ObtenerTodosProductos");
                }
                else
                {
                    ViewBag.Error = "No se pudo actualizar el producto. Verifique que exista.";
                    return View("ActualizarProducto", producto);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("ActualizarProducto", producto);
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