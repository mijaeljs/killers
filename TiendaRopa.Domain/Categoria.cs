using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TiendaRopa.Domain;

public class Categoria
{
    [Key]
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = "";

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
