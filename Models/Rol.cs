using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationNBAShop.Models;

public partial class Rol
{
    [Key]
    public int IdRol { get; set; }
    [Required(ErrorMessage = "El campo nombre es obligatorio")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
