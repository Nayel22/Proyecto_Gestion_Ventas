using Microsoft.AspNetCore.Mvc;
using Proyecto_Gestion_Ventas.Models;
using System.Diagnostics;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Borders;

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

                // ? Aquí se imprime en consola

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

        // Acción para eliminar un producto
        [HttpPost]
        public IActionResult EliminarProducto(int id)
        {
            try
            {
                // Llamar al método EliminarProducto de AccesoDatos
                int resultado = _accesoDatos.EliminarProducto(id);

                if (resultado == 1)
                {
                    TempData["Mensaje"] = "Producto eliminado correctamente";
                }
                else
                {
                    TempData["Error"] = "No se encontró el producto a eliminar";
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
                // Asignar el usuario que está creando la venta
                venta.AdicionadoPor = User.Identity?.Name ?? "Sistema";

                // Insertar la venta en la base de datos
                var ventaInsertada = _accesoDatos.InsertarVenta(venta);

                if (ventaInsertada != null)
                {
                    TempData["Mensaje"] = "Venta registrada correctamente.";
                    // Podríamos redirigir a una página de detalles de la venta o a otra acción
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

        // Método para mostrar detalle de venta con factura
        // Acción para ver los detalles de una venta específica
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

        // Método para eliminar una venta
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



        // Método auxiliar para obtener los detalles de factura por ID de venta
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


        // Método para generar PDF de factura (opcional)

public IActionResult DescargarFacturaPDF(int id)
    {
        _logger.LogInformation($"Iniciando descarga de factura para venta ID: {id}");

        var venta = _accesoDatos.ObtenerVentaPorId(id);
        if (venta == null)
        {
            _logger.LogWarning($"Venta no encontrada: {id}");
            return NotFound("La venta especificada no existe");
        }

        var detallesFactura = _accesoDatos.ObtenerFacturasPorVentaId(id) ?? new List<Factura>();

        try
        {
            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Encabezado de la factura
                var titulo = new Paragraph($"Factura de Venta #{venta.IdVenta}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18); // Solo tamaño, sin bold

                document.Add(titulo);
                document.Add(new Paragraph($"Cliente: {venta.NombreCliente ?? "N/A"}"));
                document.Add(new Paragraph($"Fecha: {venta.Fecha.ToString("dd/MM/yyyy HH:mm")}"));
                document.Add(new Paragraph($"Total: {venta.Total:C}"));

                document.Add(new Paragraph("\nDetalles de Productos:"));

                foreach (var detalle in detallesFactura)
                {
                    var idProducto = detalle?.IdProducto != null ? detalle.IdProducto.ToString() : "N/A";
                    var cantidad = detalle?.Cantidad ?? 0;
                    var subtotal = detalle?.SubTotal != null ? Convert.ToDecimal(detalle.SubTotal) : 0m;

                    document.Add(new Paragraph($"- Producto: {idProducto}, Cantidad: {cantidad}, Subtotal: {subtotal:C}"));
                }

                document.Close();
                pdf.Close();
                writer.Close();

                var fileBytes = ms.ToArray();
                return File(fileBytes, "application/pdf", $"Factura_{venta.IdVenta}.pdf");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando PDF");
            return Content($"Error al generar el PDF: {ex.Message}");
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