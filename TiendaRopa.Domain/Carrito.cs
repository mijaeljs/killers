using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaRopa.Domain
{
    public class Carrito
    {
        [Key]
        public int IdCarrito { get; set; }

        public int IdCliente { get; set; }
        public Cliente? Cliente { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public ICollection<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();
    }
}