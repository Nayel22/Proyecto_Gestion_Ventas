using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Proyecto_Gestion_Ventas.Models
{
    public class AccesoDatos
    {
        // Almacenar la cadena de conexión a la base de datos
        private readonly string _conexion;

        public AccesoDatos(IConfiguration configuracion)
        {
            _conexion = configuracion.GetConnectionString("Conexion");
        }

        // Método para agregar un cliente
        public int AgregarCliente(Cliente nuevoCliente)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_CrearCliente @nombre, @direccion, @telefono, @adicionado_por";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Asignar los valores de los parámetros
                        cmd.Parameters.AddWithValue("@nombre", nuevoCliente.Nombre);
                        cmd.Parameters.AddWithValue("@direccion", nuevoCliente.Direccion);
                        cmd.Parameters.AddWithValue("@telefono", nuevoCliente.Telefono);
                        cmd.Parameters.AddWithValue("@adicionado_por", nuevoCliente.AdicionadoPor);

                        // Abrir la conexión
                        con.Open();

                        // Ejecutar el procedimiento almacenado y obtener el ID del cliente creado
                        int idCliente = Convert.ToInt32(cmd.ExecuteScalar());

                        // Retornar el ID del cliente
                        return idCliente;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar el cliente: " + ex.Message);
                }
            }
        }

        // Método para obtener todos los clientes
        public List<Cliente> ObtenerTodosClientes()
        {
            List<Cliente> listaClientes = new List<Cliente>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_ObtenerTodosClientes";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Cliente cliente = new Cliente
                                {
                                    IdCliente = Convert.ToInt32(reader["IdCliente"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Direccion = reader["Direccion"].ToString(),
                                    Telefono = reader["Telefono"].ToString(),
                                    FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"]),
                                    AdicionadoPor = reader["AdicionadoPor"].ToString()
                                };

                                // Verificar si los campos opcionales tienen valor
                                if (reader["FechaModificacion"] != DBNull.Value)
                                    cliente.FechaModificacion = Convert.ToDateTime(reader["FechaModificacion"]);

                                if (reader["ModificadoPor"] != DBNull.Value)
                                    cliente.ModificadoPor = reader["ModificadoPor"].ToString();

                                listaClientes.Add(cliente);
                            }
                        }
                    }

                    return listaClientes;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener los clientes: " + ex.Message);
                }
            }
        }
        //Metodo para ObternerTodosClientePorId//////////////

        // Método para obtener un cliente por ID
        public Cliente ObtenerClientePorId(int id)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_ObtenerClientePorId @IdCliente";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@IdCliente", id);
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Cliente cliente = new Cliente
                                {
                                    IdCliente = Convert.ToInt32(reader["IdCliente"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Direccion = reader["Direccion"].ToString(),
                                    Telefono = reader["Telefono"].ToString(),
                                    FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"]),
                                    AdicionadoPor = reader["AdicionadoPor"].ToString()
                                };

                                // Verificar si los campos opcionales tienen valor
                                if (reader["FechaModificacion"] != DBNull.Value)
                                    cliente.FechaModificacion = Convert.ToDateTime(reader["FechaModificacion"]);

                                if (reader["ModificadoPor"] != DBNull.Value)
                                    cliente.ModificadoPor = reader["ModificadoPor"].ToString();

                                return cliente;
                            }
                        }

                        return null; // Si no se encuentra el cliente
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el cliente: " + ex.Message);
                }
            }
        }

        //ActualizarCliente

        // Método para actualizar un cliente
        public bool ActualizarCliente(Cliente cliente)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_ActualizarCliente @IdCliente, @nombre, @direccion, @telefono, @FechaModificacion, @ModificadoPor";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Asignar los valores de los parámetros
                        cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                        cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
                        cmd.Parameters.AddWithValue("@direccion", cliente.Direccion);
                        cmd.Parameters.AddWithValue("@telefono", cliente.Telefono);
                        cmd.Parameters.AddWithValue("@FechaModificacion", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ModificadoPor", cliente.ModificadoPor);

                        // Abrir la conexión
                        con.Open();

                        // Ejecutar el procedimiento almacenado
                        cmd.ExecuteNonQuery();

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al actualizar el cliente: " + ex.Message);
                }
            }
        }

        //Eliminar Cliente

        //Metodos para poder elimanr cliente

        // Método para eliminar un cliente
        public bool EliminarCliente(int idCliente)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_EliminarCliente @IdCliente";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Asignar el parámetro
                        cmd.Parameters.AddWithValue("@IdCliente", idCliente);

                        // Abrir la conexión
                        con.Open();

                        // Ejecutar el procedimiento almacenado
                        cmd.ExecuteNonQuery();

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar el cliente: " + ex.Message);
                }
            }
        }


        ////////////////Vamos ha empezar con los procedimientos almacenados de PRODUCTO//////////////////////////

        // Método para agregar un producto
        public int AgregarProducto(Producto nuevoProducto)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_InsertarProducto @nombre, @precio, @stock";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Asignar los valores de los parámetros
                        cmd.Parameters.AddWithValue("@nombre", nuevoProducto.Nombre);
                        cmd.Parameters.AddWithValue("@precio", nuevoProducto.Precio);
                        cmd.Parameters.AddWithValue("@stock", nuevoProducto.Stock);

                        // Abrir la conexión
                        con.Open();

                        // Ejecutar el procedimiento almacenado y obtener el ID del producto creado
                        int idProducto = Convert.ToInt32(cmd.ExecuteScalar());

                        // Retornar el ID del producto
                        return idProducto;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar el producto: " + ex.Message);
                }
            }
        }

        // Método para obtener todos los productos
        public List<Producto> ObtenerTodosProductos()
        {
            List<Producto> listaProductos = new List<Producto>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_ObtenerProductos";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Producto producto = new Producto
                                {
                                    IdProducto = Convert.ToInt32(reader["id_producto"]),
                                    Nombre = reader["nombre"].ToString(),
                                    Precio = Convert.ToDouble(reader["precio"]),
                                    Stock = Convert.ToInt32(reader["stock"]),
                                    FechaRegistro = Convert.ToDateTime(reader["fecha_registro"])
                                };

                                listaProductos.Add(producto);
                            }
                        }
                    }

                    return listaProductos;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener los productos: " + ex.Message);
                }
            }
        }

        // Método para obtener un producto por ID
        public Producto ObtenerProductoPorId(int id)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_ObtenerProductoPorId @id_producto";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id_producto", id);
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Producto producto = new Producto
                                {
                                    IdProducto = Convert.ToInt32(reader["id_producto"]),
                                    Nombre = reader["nombre"].ToString(),
                                    Precio = Convert.ToDouble(reader["precio"]),
                                    Stock = Convert.ToInt32(reader["stock"]),
                                    FechaRegistro = Convert.ToDateTime(reader["fecha_registro"])
                                };

                                return producto;
                            }
                        }

                        return null; // Si no se encuentra el producto
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el producto: " + ex.Message);
                }
            }
        }

        // Método para actualizar un producto
        public int ActualizarProducto(Producto producto)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    // Crear el comando
                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarProducto", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parámetros
                        cmd.Parameters.AddWithValue("@id_producto", producto.IdProducto);
                        cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                        cmd.Parameters.AddWithValue("@precio", producto.Precio);
                        cmd.Parameters.AddWithValue("@stock", producto.Stock);

                        // Parámetro de retorno
                        SqlParameter returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        // Abrir conexión y ejecutar
                        con.Open();
                        cmd.ExecuteNonQuery();

                        // Obtener el valor de retorno
                        int resultado = (int)returnParameter.Value;
                        return resultado;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al actualizar el producto: " + ex.Message);
                }
            }
        }

        // Método para eliminar un producto
        public int EliminarProducto(int idProducto)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    // Crear el comando para el procedimiento almacenado
                    using (SqlCommand cmd = new SqlCommand("sp_EliminarProducto", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parámetro de entrada
                        cmd.Parameters.AddWithValue("@id_producto", idProducto);

                        // Parámetro de retorno
                        SqlParameter returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        // Abrir conexión y ejecutar
                        con.Open();
                        cmd.ExecuteNonQuery();

                        // Obtener el valor de retorno
                        int resultado = (int)returnParameter.Value;
                        return resultado;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar el producto: " + ex.Message);
                }
            }
        }


        ///////////////////Procedimientos Almacenados de Venta////////////////////////////////////////

        // Método para insertar una venta
        public Venta InsertarVenta(Venta nuevaVenta)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_InsertarVenta @total, @adicionado_por, @id_cliente";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Asignar los valores de los parámetros
                        cmd.Parameters.AddWithValue("@total", nuevaVenta.Total);
                        cmd.Parameters.AddWithValue("@adicionado_por", nuevaVenta.AdicionadoPor);
                        cmd.Parameters.AddWithValue("@id_cliente", nuevaVenta.IdCliente);

                        // Abrir la conexión
                        con.Open();

                        // Ejecutar el procedimiento y obtener los resultados
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Venta ventaInsertada = new Venta
                                {
                                    IdVenta = Convert.ToInt32(reader["id_Venta"]),
                                    Fecha = Convert.ToDateTime(reader["fecha"]),
                                    Total = Convert.ToDouble(reader["total"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    IdCliente = Convert.ToInt32(reader["id_cliente"]),
                                    NombreCliente = reader["nombre_cliente"].ToString()
                                };

                                return ventaInsertada;
                            }
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar la venta: " + ex.Message);
                }
            }
        }


        // Método para obtener una venta por ID
        public Venta ObtenerVentaPorId(int id)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = @"SELECT v.id_Venta, v.fecha, v.total, v.adicionado_por, v.id_cliente, 
                           c.nombre AS nombre_cliente
                           FROM Venta v
                           INNER JOIN Cliente c ON v.id_cliente = c.id_cliente
                           WHERE v.id_Venta = @id_venta";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id_venta", id);
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Venta venta = new Venta
                                {
                                    IdVenta = Convert.ToInt32(reader["id_Venta"]),
                                    Fecha = Convert.ToDateTime(reader["fecha"]),
                                    Total = Convert.ToDouble(reader["total"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    IdCliente = Convert.ToInt32(reader["id_cliente"]),
                                    NombreCliente = reader["nombre_cliente"].ToString()
                                };

                                return venta;
                            }
                        }

                        return null; // Si no se encuentra la venta
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la venta: " + ex.Message);
                }
            }
        }

        // Método para obtener todas las ventas
        public List<Venta> ObtenerTodasVentas()
        {
            List<Venta> listaVentas = new List<Venta>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = @"SELECT v.id_Venta, v.fecha, v.total, v.adicionado_por, v.id_cliente, 
                           c.nombre AS nombre_cliente
                           FROM Venta v
                           INNER JOIN Cliente c ON v.id_cliente = c.id_cliente
                           ORDER BY v.fecha DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Venta venta = new Venta
                                {
                                    IdVenta = Convert.ToInt32(reader["id_Venta"]),
                                    Fecha = Convert.ToDateTime(reader["fecha"]),
                                    Total = Convert.ToDouble(reader["total"]),
                                    AdicionadoPor = reader["adicionado_por"].ToString(),
                                    IdCliente = Convert.ToInt32(reader["id_cliente"]),
                                    NombreCliente = reader["nombre_cliente"].ToString()
                                };

                                listaVentas.Add(venta);
                            }
                        }
                    }

                    return listaVentas;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener las ventas: " + ex.Message);
                }
            }
        }


        ///////////////////Procedimiento almacenado de de Factura///////////////////////////////

        // Método para insertar una factura
        public int InsertarFactura(Factura nuevaFactura)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_InsertarFactura @id_Venta, @id_producto, @cantidad, @subTotal";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Asignar los valores de los parámetros
                        cmd.Parameters.AddWithValue("@id_Venta", nuevaFactura.IdVenta);
                        cmd.Parameters.AddWithValue("@id_producto", nuevaFactura.IdProducto);
                        cmd.Parameters.AddWithValue("@cantidad", nuevaFactura.Cantidad);
                        cmd.Parameters.AddWithValue("@subTotal", nuevaFactura.SubTotal);

                        // Abrir la conexión
                        con.Open();

                        // Ejecutar el procedimiento almacenado
                        cmd.ExecuteNonQuery();

                        // Obtener el ID de la factura insertada (puede necesitar ajustes según cómo retorne el ID tu procedimiento)
                        string queryId = "SELECT SCOPE_IDENTITY()";
                        cmd.CommandText = queryId;
                        int idFactura = Convert.ToInt32(cmd.ExecuteScalar());

                        return idFactura;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar la factura: " + ex.Message);
                }
            }
        }

        // Método para actualizar una factura
        public bool ActualizarFactura(Factura factura)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_ActualizarFactura @id_factura, @id_Venta, @id_producto, @cantidad, @subTotal";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Asignar los valores de los parámetros
                        cmd.Parameters.AddWithValue("@id_factura", factura.IdFactura);
                        cmd.Parameters.AddWithValue("@id_Venta", factura.IdVenta);
                        cmd.Parameters.AddWithValue("@id_producto", factura.IdProducto);
                        cmd.Parameters.AddWithValue("@cantidad", factura.Cantidad);
                        cmd.Parameters.AddWithValue("@subTotal", factura.SubTotal);

                        // Abrir la conexión
                        con.Open();

                        // Ejecutar el procedimiento almacenado
                        cmd.ExecuteNonQuery();

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al actualizar la factura: " + ex.Message);
                }
            }
        }

        // Método para eliminar una factura
        public bool EliminarFactura(int idFactura)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_EliminarFactura @id_factura";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Asignar el parámetro
                        cmd.Parameters.AddWithValue("@id_factura", idFactura);

                        // Abrir la conexión
                        con.Open();

                        // Ejecutar el procedimiento almacenado
                        cmd.ExecuteNonQuery();

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar la factura: " + ex.Message);
                }
            }
        }

        // Método para listar todas las facturas
        public List<Factura> ListarFacturas()
        {
            List<Factura> listaFacturas = new List<Factura>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_ListarFacturas";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Factura factura = new Factura
                                {
                                    IdFactura = Convert.ToInt32(reader["id_factura"]),
                                    FechaVenta = Convert.ToDateTime(reader["fecha_venta"]),
                                    NombreCliente = reader["cliente"].ToString(),
                                    NombreProducto = reader["producto"].ToString(),
                                    Cantidad = Convert.ToInt32(reader["cantidad"]),
                                    SubTotal = Convert.ToDouble(reader["subTotal"])
                                };

                                listaFacturas.Add(factura);
                            }
                        }
                    }

                    return listaFacturas;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener las facturas: " + ex.Message);
                }
            }
        }

        // Método para obtener una factura por ID
        public Factura ObtenerFacturaPorId(int id)
        {
            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = "Exec sp_ObtenerFacturaPorId @id_factura";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id_factura", id);
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Factura factura = new Factura
                                {
                                    IdFactura = Convert.ToInt32(reader["id_factura"]),
                                    FechaVenta = Convert.ToDateTime(reader["fecha_venta"]),
                                    NombreCliente = reader["cliente"].ToString(),
                                    NombreProducto = reader["producto"].ToString(),
                                    Cantidad = Convert.ToInt32(reader["cantidad"]),
                                    SubTotal = Convert.ToDouble(reader["subTotal"])
                                };

                                return factura;
                            }
                        }

                        return null; // Si no se encuentra la factura
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la factura: " + ex.Message);
                }
            }
        }

        // Método para obtener facturas por ID de venta
        public List<Factura> ObtenerFacturasPorVentaId(int idVenta)
        {
            List<Factura> listaFacturas = new List<Factura>();

            using (SqlConnection con = new SqlConnection(_conexion))
            {
                try
                {
                    string query = @"SELECT f.id_factura, f.id_Venta, f.id_producto, f.cantidad, f.subTotal,
                              p.nombre AS producto, v.fecha AS fecha_venta, c.nombre AS cliente
                           FROM Factura f
                           INNER JOIN Producto p ON f.id_producto = p.id_producto
                           INNER JOIN Venta v ON f.id_Venta = v.id_Venta
                           INNER JOIN Cliente c ON v.id_cliente = c.id_cliente
                           WHERE f.id_Venta = @id_Venta
                           ORDER BY f.id_factura";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id_Venta", idVenta);
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Factura factura = new Factura
                                {
                                    IdFactura = Convert.ToInt32(reader["id_factura"]),
                                    IdVenta = Convert.ToInt32(reader["id_Venta"]),
                                    IdProducto = Convert.ToInt32(reader["id_producto"]),
                                    Cantidad = Convert.ToInt32(reader["cantidad"]),
                                    SubTotal = Convert.ToDouble(reader["subTotal"]),
                                    NombreProducto = reader["producto"].ToString(),
                                    FechaVenta = Convert.ToDateTime(reader["fecha_venta"]),
                                    NombreCliente = reader["cliente"].ToString()
                                };

                                listaFacturas.Add(factura);
                            }
                        }
                    }

                    return listaFacturas;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener las facturas de la venta: " + ex.Message);
                }
            }
        }






    }
}