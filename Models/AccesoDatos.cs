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



    }
}
