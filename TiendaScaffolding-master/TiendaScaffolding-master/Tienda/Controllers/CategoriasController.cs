using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaScaffolding.Data;
using TiendaScaffolding.Models;

namespace TiendaScaffolding.Controllers;

public class CategoriasController : Controller
{
    private readonly TiendaDbContext _context;

    public CategoriasController(TiendaDbContext context) => _context = context;

    // READ - listado de los cinco atributos de la tabla.
    public async Task<IActionResult> Index()
    {
        var registros = await _context.Categorias.AsNoTracking()
            .OrderBy(x => x.CategoriaId).ToListAsync();
        return View(registros);
    }

    // READ - detalles.
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Categorias.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CategoriaId == id);
        return modelo is null ? NotFound() : View(modelo);
    }

    // CREATE - formulario.
    [HttpGet]
    public IActionResult Create()
    {
        var modelo = new Categoria { Activa = true, FechaRegistro = DateOnly.FromDateTime(DateTime.Today) };
        return View(modelo);
    }

    // CREATE - guardar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CategoriaId,Nombre,Descripcion,Activa,FechaRegistro")] Categoria categoria)
    {

        if (categoria.FechaRegistro == default) ModelState.AddModelError(nameof(Categoria.FechaRegistro), "Indica una fecha válida.");
        if (ModelState.IsValid)
        {
            try
            {
                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Registro creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "No se pudo guardar. Comprueba los datos y las relaciones.");
            }
        }
        return View(categoria);
    }

    // UPDATE - formulario.
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Categorias.FindAsync(id);
        if (modelo is null) return NotFound();
        return View(modelo);
    }

    // UPDATE - guardar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("CategoriaId,Nombre,Descripcion,Activa,FechaRegistro")] Categoria categoria)
    {
        if (id != categoria.CategoriaId) return BadRequest();

        if (categoria.FechaRegistro == default) ModelState.AddModelError(nameof(Categoria.FechaRegistro), "Indica una fecha válida.");
        if (ModelState.IsValid)
        {
            var actual = await _context.Categorias.FindAsync(id);
            if (actual is null) return NotFound();
            actual.Nombre = categoria.Nombre;
            actual.Descripcion = categoria.Descripcion;
            actual.Activa = categoria.Activa;
            actual.FechaRegistro = categoria.FechaRegistro;
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
        return View(categoria);
    }

    // DELETE - confirmación.
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Categorias.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CategoriaId == id);
        return modelo is null ? NotFound() : View(modelo);
    }

    // DELETE - eliminación con protección de claves foráneas.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var modelo = await _context.Categorias.FindAsync(id);
        if (modelo is null) return NotFound();
        try
        {
            _context.Categorias.Remove(modelo);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            _context.Entry(modelo).State = EntityState.Unchanged;
            var completo = await _context.Categorias.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CategoriaId == id);
            ViewBag.ErrorEliminacion = "No se puede eliminar este registro porque tiene datos relacionados. Elimina primero sus dependencias.";
            return completo is null ? NotFound() : View("Delete", completo);
        }
    }
}
