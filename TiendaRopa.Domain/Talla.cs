using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TiendaRopa.Domain;

public class Talla
{
    [Key]
    public int IdTalla { get; set; }

    public string Nombre { get; set; } = ""; // S, M, L, XL

    public ICollection<ProductoVariante> Variantes { get; set; } = new List<ProductoVariante>();
}