using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace TiendaScaffolding.Models;

public partial class Detallespedido
{
    public int DetallePedidoId { get; set; }

    public int PedidoId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    [ValidateNever]
    public virtual Pedido Pedido { get; set; } = null!;

    [ValidateNever]
    public virtual Producto Producto { get; set; } = null!;
}
