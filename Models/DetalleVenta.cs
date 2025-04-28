

namespace Proyecto_Gestion_Ventas.Models
{
    public class DetalleVenta
    {
        public int id_detalle { get; set; }

        public int id_venta { get; set; }

        public string nombre_producto { get; set; }

        public int cantidad { get; set; }

        public decimal subtotal { get; set; }

        // Propiedad de navegación para la relación con Venta
        public virtual Venta Venta { get; set; }


    }
}
