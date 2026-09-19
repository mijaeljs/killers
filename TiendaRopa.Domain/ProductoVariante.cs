using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TiendaRopa.Domain;

public class ProductoVariante
{
    [Key]
    public int IdVariante { get; set; }

    public int IdProducto { get; set; }
    public Producto? Producto { get; set; }

    public int IdTalla { get; set; }
    public Talla? Talla { get; set; }

    public int IdColor { get; set; }
    public Color? Color { get; set; }

    public int Stock { get; set; }
}