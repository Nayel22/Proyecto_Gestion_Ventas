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
    }
}
