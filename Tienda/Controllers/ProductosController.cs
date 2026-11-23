using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaScaffolding.Data;
using TiendaScaffolding.Models;

namespace TiendaScaffolding.Controllers;

public class ProductosController : Controller
{
    private readonly TiendaDbContext _context;

    public ProductosController(TiendaDbContext context) => _context = context;

    // READ - listado de los cinco atributos de la tabla.
    public async Task<IActionResult> Index()
    {
        var registros = await _context.Productos.AsNoTracking().Include(x => x.Categoria)
            .OrderBy(x => x.ProductoId).ToListAsync();
        return View(registros);
    }

    // READ - detalles.
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Productos.AsNoTracking().Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.ProductoId == id);
        return modelo is null ? NotFound() : View(modelo);
    }

    // CREATE - formulario.
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var modelo = new Producto();
        await CargarListas(modelo.CategoriaId);
        return View(modelo);
    }

    // CREATE - guardar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProductoId,Nombre,Precio,Stock,CategoriaId")] Producto producto)
    {
        ModelState.Remove(nameof(Producto.Categoria));
        if (producto.Precio < 0) ModelState.AddModelError(nameof(Producto.Precio), "El precio no puede ser negativo.");
        if (producto.Stock < 0) ModelState.AddModelError(nameof(Producto.Stock), "El stock no puede ser negativo.");
        if (producto.CategoriaId <= 0 || !await _context.Categorias.AnyAsync(c => c.CategoriaId == producto.CategoriaId))
            ModelState.AddModelError(nameof(Producto.CategoriaId), "Selecciona una categoría existente.");
        if (ModelState.IsValid)
        {
            try
            {
                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Registro creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "No se pudo guardar. Comprueba los datos y las relaciones.");
            }
        }
        await CargarListas(producto.CategoriaId);
        return View(producto);
    }

    // UPDATE - formulario.
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Productos.FindAsync(id);
        if (modelo is null) return NotFound();
        await CargarListas(modelo.CategoriaId);
        return View(modelo);
    }

    // UPDATE - guardar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ProductoId,Nombre,Precio,Stock,CategoriaId")] Producto producto)
    {
        if (id != producto.ProductoId) return BadRequest();
        ModelState.Remove(nameof(Producto.Categoria));
        if (producto.Precio < 0) ModelState.AddModelError(nameof(Producto.Precio), "El precio no puede ser negativo.");
        if (producto.Stock < 0) ModelState.AddModelError(nameof(Producto.Stock), "El stock no puede ser negativo.");
        if (producto.CategoriaId <= 0 || !await _context.Categorias.AnyAsync(c => c.CategoriaId == producto.CategoriaId))
            ModelState.AddModelError(nameof(Producto.CategoriaId), "Selecciona una categoría existente.");
        if (ModelState.IsValid)
        {
            var actual = await _context.Productos.FindAsync(id);
            if (actual is null) return NotFound();
            actual.Nombre = producto.Nombre;
            actual.Precio = producto.Precio;
            actual.Stock = producto.Stock;
            actual.CategoriaId = producto.CategoriaId;
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
        await CargarListas(producto.CategoriaId);
        return View(producto);
    }

    // DELETE - confirmación.
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Productos.AsNoTracking().Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.ProductoId == id);
        return modelo is null ? NotFound() : View(modelo);
    }

    // DELETE - eliminación con protección de claves foráneas.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var modelo = await _context.Productos.FindAsync(id);
        if (modelo is null) return NotFound();
        try
        {
            _context.Productos.Remove(modelo);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            _context.Entry(modelo).State = EntityState.Unchanged;
            var completo = await _context.Productos.AsNoTracking().Include(x => x.Categoria)
                .FirstOrDefaultAsync(x => x.ProductoId == id);
            ViewBag.ErrorEliminacion = "No se puede eliminar este registro porque tiene datos relacionados. Elimina primero sus dependencias.";
            return completo is null ? NotFound() : View("Delete", completo);
        }
    }

    private async Task CargarListas(int? categoriaId = null)
    {
        var categorias = await _context.Categorias.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync();
        ViewBag.CategoriaId = new SelectList(categorias, "CategoriaId", "Nombre", categoriaId);
    }

}
