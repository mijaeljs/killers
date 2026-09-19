using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TiendaRopa.Domain;

public class Color
{
    [Key]
    public int IdColor { get; set; }

    public string Nombre { get; set; } = ""; // Rojo, Azul, Negro

    public ICollection<ProductoVariante> Variantes { get; set; } = new List<ProductoVariante>();
}