using System;
using System.ComponentModel.DataAnnotations;

namespace TiendaRopa.Domain;

public class Administrador
{
    [Key]
    public int IdAdministrador { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El usuario es obligatorio")]
    [MaxLength(50)]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
    [MaxLength(150)]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Rol { get; set; } = "Administrador";

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
