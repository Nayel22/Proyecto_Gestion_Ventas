namespace Proyecto_Gestion_Ventas.Models
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public double Total { get; set; }
        public string AdicionadoPor { get; set; }
        public int IdCliente { get; set; }
        // Propiedad adicional para mostrar el nombre del cliente
        public string NombreCliente { get; set; }
    }
}
