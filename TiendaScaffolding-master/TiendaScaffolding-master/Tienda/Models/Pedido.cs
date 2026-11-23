using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace TiendaScaffolding.Models;

public partial class Pedido
{
    public int PedidoId { get; set; }

    public DateTime Fecha { get; set; }

    public string Estado { get; set; } = null!;

    public string? Observaciones { get; set; }

    public int ClienteId { get; set; }

    [ValidateNever]
    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<Detallespedido> Detallespedidos { get; set; } = new List<Detallespedido>();
}
