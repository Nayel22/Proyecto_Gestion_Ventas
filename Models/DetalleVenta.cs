namespace Proyecto_Gestion_Ventas.Models
{
    public class DetalleVenta
    {
        private int IdDetalleVenta { get; set; }
        private int IdVenta { get; set; }
        private int IdProducto { get; set; }
        private int Cantidad { get; set; }
        private double SubTotal { get; set; }
    }
}
