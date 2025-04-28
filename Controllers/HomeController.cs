using Microsoft.AspNetCore.Mvc;
using Proyecto_Gestion_Ventas.Models;
using System.Diagnostics;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
                    // Si el usuario no est� autenticado, asignar un valor predeterminado
                    if (string.IsNullOrEmpty(cliente.AdicionadoPor))
                    {
                        cliente.AdicionadoPor = "Sistema";
                    }

                    // Usar la clase AccesoDatos para agregar el cliente
                    int idCliente = _accesoDatos.AgregarCliente(cliente);
                    TempData["SuccessMessage"] = "Cliente registrado con �xito. ID: " + idCliente;
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

        // M�todo para mostrar el formulario de actualizaci�n
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

        // M�todo para procesar la actualizaci�n
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

        // Acci�n para eliminar un cliente
        public IActionResult EliminarCliente(int id)
        {
            try
            {
                // Llamar al m�todo EliminarCliente de AccesoDatos
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

                    // Llamar al m�todo de la capa de acceso a datos
                    int idProducto = _accesoDatos.AgregarProducto(producto);

                    // Mostrar mensaje de �xito
                    TempData["SuccessMessage"] = "Producto agregado correctamente con ID: " + idProducto;

                    // Redirect a la misma p�gina para limpiar el formulario
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

                // ? Aqu� se imprime en consola

                Console.WriteLine("Resultado SP: " + resultado);


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

        // Acci�n para eliminar un producto
        [HttpPost]
        public IActionResult EliminarProducto(int id)
        {
            try
            {
                // Llamar al m�todo EliminarProducto de AccesoDatos
                int resultado = _accesoDatos.EliminarProducto(id);

                if (resultado == 1)
                {
                    TempData["Mensaje"] = "Producto eliminado correctamente";
                }
                else
                {
                    TempData["Error"] = "No se encontr� el producto a eliminar";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el producto: " + ex.Message;
            }

            // Redirigir a la lista de productos
            return RedirectToAction("ObtenerTodosProductos");

        }

        ///////////////Procedimientos almacenados de Venta/////////////////////////////////////////

        // GET: /Home/CrearVenta
        public IActionResult CrearVenta()
        {
            // Cargar la lista de clientes para el dropdown
            try
            {
                ViewBag.Clientes = _accesoDatos.ObtenerTodosClientes();
                ViewBag.Productos = _accesoDatos.ObtenerTodosProductos();
                return View(new Venta());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: /Home/CrearVenta
        [HttpPost]
        public IActionResult CrearVenta(Venta venta)
        {
            try
            {
                // Asignar el usuario que est� creando la venta
                venta.AdicionadoPor = User.Identity?.Name ?? "Sistema";

                // Insertar la venta en la base de datos
                var ventaInsertada = _accesoDatos.InsertarVenta(venta);

                if (ventaInsertada != null)
                {
                    TempData["Mensaje"] = "Venta registrada correctamente.";
                    // Podr�amos redirigir a una p�gina de detalles de la venta o a otra acci�n
                    return RedirectToAction("DetalleVenta", new { id = ventaInsertada.IdVenta });
                }
                else
                {
                    ViewBag.Error = "No se pudo registrar la venta.";
                    ViewBag.Clientes = _accesoDatos.ObtenerTodosClientes();
                    ViewBag.Productos = _accesoDatos.ObtenerTodosProductos();
                    return View(venta);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Clientes = _accesoDatos.ObtenerTodosClientes();
                ViewBag.Productos = _accesoDatos.ObtenerTodosProductos();
                return View(venta);
            }
        }

        // GET: /Home/DetalleVenta/5

        // M�todo para mostrar detalle de venta con factura
        // Acci�n para ver los detalles de una venta espec�fica
        public IActionResult DetalleVenta(int id)
        {
            try
            {
                var venta = _accesoDatos.ObtenerVentaPorId(id);
                if (venta == null)
                {
                    return NotFound();
                }

                var facturas = _accesoDatos.ObtenerFacturasPorVentaId(id);

                if (facturas != null && facturas.Count > 0)
                {
                    foreach (var factura in facturas)
                    {
                        var producto = _accesoDatos.ObtenerProductoPorId(factura.IdProducto);
                        factura.NombreProducto = producto != null ? producto.Nombre : "Producto no encontrado";
                    }
                }

                ViewBag.DetallesVenta = facturas;

                double subtotal = Math.Round(venta.Total / 1.16, 2);
                double impuesto = Math.Round(venta.Total - subtotal, 2);

                ViewBag.Subtotal = subtotal;
                ViewBag.PorcentajeIVA = 16;
                ViewBag.Impuesto = impuesto;
                ViewBag.Total = venta.Total;
                ViewBag.NumeroFactura = $"F-{venta.IdVenta:D6}";

                var cliente = _accesoDatos.ObtenerClientePorId(venta.IdCliente);
                if (cliente != null)
                {
                    ViewBag.EmpresaNombre = cliente.Nombre;
                    ViewBag.EmpresaRUC = $"ID Cliente: {cliente.IdCliente}";
                    ViewBag.EmpresaDireccion = cliente.Direccion;
                    ViewBag.EmpresaTelefono = cliente.Telefono;
                    ViewBag.ClienteDireccion = cliente.Direccion;
                    ViewBag.ClienteTelefono = cliente.Telefono;
                    ViewBag.FechaRegistroCliente = cliente.FechaRegistro.ToString("dd/MM/yyyy");
                }
                else
                {
                    ViewBag.EmpresaNombre = "Cliente no encontrado";
                    ViewBag.EmpresaRUC = "N/A";
                    ViewBag.EmpresaDireccion = "N/A";
                    ViewBag.EmpresaTelefono = "N/A";
                }

                ViewBag.FormaPago = "Efectivo";
                ViewBag.NotasAdicionales = "Gracias por su compra.";

                return View(venta);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar los detalles de la venta: " + ex.Message;
                return RedirectToAction("ListaVentas");
            }
        }




        // GET: /Home/ListaVentas
        public IActionResult ListaVentas()
        {
            try
            {
                var ventas = _accesoDatos.ObtenerTodasVentas();
                return View(ventas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // M�todo para eliminar una venta
        public IActionResult EliminarVenta(int id)
        {
            try
            {
                // Intentar eliminar la venta
                bool resultado = _accesoDatos.EliminarVenta(id);

                if (resultado)
                {
                    TempData["Mensaje"] = "Venta eliminada correctamente.";
                }
                else
                {
                    TempData["Error"] = "No fue posible eliminar la venta.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la venta: " + ex.Message;
            }

            // Redirigir a la lista de ventas
            return RedirectToAction("ListaVentas");
        }




        ///////////////////Procedimiento almacenado de de Factura///////////////////////////////



        // M�todo auxiliar para obtener los detalles de factura por ID de venta
        private List<Factura> ObtenerDetallesFacturasPorVenta(int idVenta)
        {
            try
            {
                // Suponemos que podemos filtrar la lista de facturas por el ID de venta
                var todasLasFacturas = _accesoDatos.ListarFacturas();
                var detallesVenta = todasLasFacturas.Where(f => f.IdVenta == idVenta).ToList();

                return detallesVenta;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los detalles de la factura: " + ex.Message);
            }
        }


        // M�todo para generar PDF de factura (opcional)
        public IActionResult DescargarFacturaPDF(int id)
        {
            try
            {
                // Aqu� se implementar�a la generaci�n del PDF de la factura
                // Este m�todo es opcional y requerir�a una biblioteca para generar PDFs

                // Por ahora, simplemente redireccionamos al detalle
                return RedirectToAction("DetalleVenta", new { id = id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al generar la factura: " + ex.Message;
                return RedirectToAction("DetalleVenta", new { id = id });
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