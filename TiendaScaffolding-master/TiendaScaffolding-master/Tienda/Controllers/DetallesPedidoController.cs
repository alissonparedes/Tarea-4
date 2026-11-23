using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaScaffolding.Data;
using TiendaScaffolding.Models;

namespace TiendaScaffolding.Controllers;

public class DetallesPedidoController : Controller
{
    private readonly TiendaDbContext _context;

    public DetallesPedidoController(TiendaDbContext context) => _context = context;

    // READ - listado de los cinco atributos de la tabla.
    public async Task<IActionResult> Index()
    {
        var registros = await _context.Detallespedidos.AsNoTracking().Include(x => x.Pedido).Include(x => x.Producto)
            .OrderBy(x => x.DetallePedidoId).ToListAsync();
        return View(registros);
    }

    // READ - detalles.
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Detallespedidos.AsNoTracking().Include(x => x.Pedido).Include(x => x.Producto)
            .FirstOrDefaultAsync(x => x.DetallePedidoId == id);
        return modelo is null ? NotFound() : View(modelo);
    }

    // CREATE - formulario.
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var modelo = new Detallespedido { Cantidad = 1 };
        await CargarListas(modelo.PedidoId, modelo.ProductoId);
        return View(modelo);
    }

    // CREATE - guardar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("DetallePedidoId,PedidoId,ProductoId,Cantidad,PrecioUnitario")] Detallespedido detallespedido)
    {
        ModelState.Remove(nameof(Detallespedido.Pedido));
        ModelState.Remove(nameof(Detallespedido.Producto));
        if (detallespedido.Cantidad <= 0) ModelState.AddModelError(nameof(Detallespedido.Cantidad), "La cantidad debe ser mayor que cero.");
        if (detallespedido.PrecioUnitario < 0) ModelState.AddModelError(nameof(Detallespedido.PrecioUnitario), "El precio no puede ser negativo.");
        if (detallespedido.PedidoId <= 0 || !await _context.Pedidos.AnyAsync(p => p.PedidoId == detallespedido.PedidoId))
            ModelState.AddModelError(nameof(Detallespedido.PedidoId), "Selecciona un pedido existente.");
        if (detallespedido.ProductoId <= 0 || !await _context.Productos.AnyAsync(p => p.ProductoId == detallespedido.ProductoId))
            ModelState.AddModelError(nameof(Detallespedido.ProductoId), "Selecciona un producto existente.");
        if (ModelState.IsValid)
        {
            try
            {
                _context.Detallespedidos.Add(detallespedido);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Registro creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "No se pudo guardar. Comprueba los datos y las relaciones.");
            }
        }
        await CargarListas(detallespedido.PedidoId, detallespedido.ProductoId);
        return View(detallespedido);
    }

    // UPDATE - formulario.
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Detallespedidos.FindAsync(id);
        if (modelo is null) return NotFound();
        await CargarListas(modelo.PedidoId, modelo.ProductoId);
        return View(modelo);
    }

    // UPDATE - guardar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("DetallePedidoId,PedidoId,ProductoId,Cantidad,PrecioUnitario")] Detallespedido detallespedido)
    {
        if (id != detallespedido.DetallePedidoId) return BadRequest();
        ModelState.Remove(nameof(Detallespedido.Pedido));
        ModelState.Remove(nameof(Detallespedido.Producto));
        if (detallespedido.Cantidad <= 0) ModelState.AddModelError(nameof(Detallespedido.Cantidad), "La cantidad debe ser mayor que cero.");
        if (detallespedido.PrecioUnitario < 0) ModelState.AddModelError(nameof(Detallespedido.PrecioUnitario), "El precio no puede ser negativo.");
        if (detallespedido.PedidoId <= 0 || !await _context.Pedidos.AnyAsync(p => p.PedidoId == detallespedido.PedidoId))
            ModelState.AddModelError(nameof(Detallespedido.PedidoId), "Selecciona un pedido existente.");
        if (detallespedido.ProductoId <= 0 || !await _context.Productos.AnyAsync(p => p.ProductoId == detallespedido.ProductoId))
            ModelState.AddModelError(nameof(Detallespedido.ProductoId), "Selecciona un producto existente.");
        if (ModelState.IsValid)
        {
            var actual = await _context.Detallespedidos.FindAsync(id);
            if (actual is null) return NotFound();
            actual.PedidoId = detallespedido.PedidoId;
            actual.ProductoId = detallespedido.ProductoId;
            actual.Cantidad = detallespedido.Cantidad;
            actual.PrecioUnitario = detallespedido.PrecioUnitario;
            try
            {
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Registro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "No se pudo actualizar. Comprueba las relaciones de este registro.");
            }
        }
        await CargarListas(detallespedido.PedidoId, detallespedido.ProductoId);
        return View(detallespedido);
    }

    // DELETE - confirmación.
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Detallespedidos.AsNoTracking().Include(x => x.Pedido).Include(x => x.Producto)
            .FirstOrDefaultAsync(x => x.DetallePedidoId == id);
        return modelo is null ? NotFound() : View(modelo);
    }

    // DELETE - eliminación con protección de claves foráneas.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var modelo = await _context.Detallespedidos.FindAsync(id);
        if (modelo is null) return NotFound();
        try
        {
            _context.Detallespedidos.Remove(modelo);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            _context.Entry(modelo).State = EntityState.Unchanged;
            var completo = await _context.Detallespedidos.AsNoTracking().Include(x => x.Pedido).Include(x => x.Producto)
                .FirstOrDefaultAsync(x => x.DetallePedidoId == id);
            ViewBag.ErrorEliminacion = "No se puede eliminar este registro porque tiene datos relacionados. Elimina primero sus dependencias.";
            return completo is null ? NotFound() : View("Delete", completo);
        }
    }

    private async Task CargarListas(int? pedidoId = null, int? productoId = null)
    {
        var pedidos = await _context.Pedidos.Include(x => x.Cliente).AsNoTracking().OrderBy(x => x.PedidoId).ToListAsync();
        var opcionesPedido = pedidos.Select(x => new { x.PedidoId, Descripcion = $"Pedido #{x.PedidoId} - {x.Cliente.Nombres} {x.Cliente.Apellidos}" }).ToList();
        ViewBag.PedidoId = new SelectList(opcionesPedido, "PedidoId", "Descripcion", pedidoId);
        var productos = await _context.Productos.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync();
        ViewBag.ProductoId = new SelectList(productos, "ProductoId", "Nombre", productoId);
    }

}
