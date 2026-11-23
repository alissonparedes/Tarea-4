using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaScaffolding.Data;
using TiendaScaffolding.Models;

namespace TiendaScaffolding.Controllers;

public class PedidosController : Controller
{
    private readonly TiendaDbContext _context;

    public PedidosController(TiendaDbContext context) => _context = context;

    // READ - listado de los cinco atributos de la tabla.
    public async Task<IActionResult> Index()
    {
        var registros = await _context.Pedidos.AsNoTracking().Include(x => x.Cliente)
            .OrderBy(x => x.PedidoId).ToListAsync();
        return View(registros);
    }

    // READ - detalles.
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Pedidos.AsNoTracking().Include(x => x.Cliente)
            .FirstOrDefaultAsync(x => x.PedidoId == id);
        return modelo is null ? NotFound() : View(modelo);
    }

    // CREATE - formulario.
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var modelo = new Pedido { Fecha = DateTime.Now, Estado = "Pendiente" };
        await CargarListas(modelo.ClienteId);
        return View(modelo);
    }

    // CREATE - guardar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PedidoId,Fecha,Estado,Observaciones,ClienteId")] Pedido pedido)
    {
        ModelState.Remove(nameof(Pedido.Cliente));
        if (pedido.Fecha == default) ModelState.AddModelError(nameof(Pedido.Fecha), "Indica una fecha válida.");
        if (pedido.ClienteId <= 0 || !await _context.Clientes.AnyAsync(c => c.ClienteId == pedido.ClienteId))
            ModelState.AddModelError(nameof(Pedido.ClienteId), "Selecciona un cliente existente.");
        if (ModelState.IsValid)
        {
            try
            {
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Registro creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "No se pudo guardar. Comprueba los datos y las relaciones.");
            }
        }
        await CargarListas(pedido.ClienteId);
        return View(pedido);
    }

    // UPDATE - formulario.
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Pedidos.FindAsync(id);
        if (modelo is null) return NotFound();
        await CargarListas(modelo.ClienteId);
        return View(modelo);
    }

    // UPDATE - guardar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("PedidoId,Fecha,Estado,Observaciones,ClienteId")] Pedido pedido)
    {
        if (id != pedido.PedidoId) return BadRequest();
        ModelState.Remove(nameof(Pedido.Cliente));
        if (pedido.Fecha == default) ModelState.AddModelError(nameof(Pedido.Fecha), "Indica una fecha válida.");
        if (pedido.ClienteId <= 0 || !await _context.Clientes.AnyAsync(c => c.ClienteId == pedido.ClienteId))
            ModelState.AddModelError(nameof(Pedido.ClienteId), "Selecciona un cliente existente.");
        if (ModelState.IsValid)
        {
            var actual = await _context.Pedidos.FindAsync(id);
            if (actual is null) return NotFound();
            actual.Fecha = pedido.Fecha;
            actual.Estado = pedido.Estado;
            actual.Observaciones = pedido.Observaciones;
            actual.ClienteId = pedido.ClienteId;
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
        await CargarListas(pedido.ClienteId);
        return View(pedido);
    }

    // DELETE - confirmación.
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Pedidos.AsNoTracking().Include(x => x.Cliente)
            .FirstOrDefaultAsync(x => x.PedidoId == id);
        return modelo is null ? NotFound() : View(modelo);
    }

    // DELETE - eliminación con protección de claves foráneas.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var modelo = await _context.Pedidos.FindAsync(id);
        if (modelo is null) return NotFound();
        try
        {
            _context.Pedidos.Remove(modelo);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            _context.Entry(modelo).State = EntityState.Unchanged;
            var completo = await _context.Pedidos.AsNoTracking().Include(x => x.Cliente)
                .FirstOrDefaultAsync(x => x.PedidoId == id);
            ViewBag.ErrorEliminacion = "No se puede eliminar este registro porque tiene datos relacionados. Elimina primero sus dependencias.";
            return completo is null ? NotFound() : View("Delete", completo);
        }
    }

    private async Task CargarListas(int? clienteId = null)
    {
        var clientes = await _context.Clientes.AsNoTracking().OrderBy(x => x.Apellidos).ToListAsync();
        var opciones = clientes.Select(x => new { x.ClienteId, NombreCompleto = x.Nombres + " " + x.Apellidos }).ToList();
        ViewBag.ClienteId = new SelectList(opciones, "ClienteId", "NombreCompleto", clienteId);
    }

}
