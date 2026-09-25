using System;
using System.ComponentModel.DataAnnotations;

namespace TiendaRopa.Domain
{
    public class ItemCarrito
    {
        [Key]
        public int IdItemCarrito { get; set; }

        public int IdCarrito { get; set; }
        public Carrito? Carrito { get; set; }

        public int IdVariante { get; set; }
        public ProductoVariante? Variante { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }
    }
}