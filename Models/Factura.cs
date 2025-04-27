namespace Proyecto_Gestion_Ventas.Models
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public int IdVenta { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public double SubTotal { get; set; }

        // Propiedades adicionales para mostrar información relacionada
        public string NombreProducto { get; set; }
        public string NombreCliente { get; set; }
        public DateTime FechaVenta { get; set; }
    }
}
