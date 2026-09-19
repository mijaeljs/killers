using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TiendaRopa.Domain;

public class Producto
{
    [Key]
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = "";

    public string Descripcion { get; set; } = "";

    public decimal Precio { get; set; }

    public int IdCategoria { get; set; }

    public Categoria? Categoria { get; set; }

    public ICollection<ProductoVariante> Variantes { get; set; } = new List<ProductoVariante>();
}