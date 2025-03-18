namespace Proyecto_Gestion_Ventas.Models
{
    public class Venta
    {
        private int IdVenta { get; set; }
        private DateTime Fecha { get; set; }
        private double Total { get; set; }
        private string AdicionadoPor { get; set; }
        private int IdCliente { get; set; }
    }
}
