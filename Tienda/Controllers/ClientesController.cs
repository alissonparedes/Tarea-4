using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaScaffolding.Data;
using TiendaScaffolding.Models;

namespace TiendaScaffolding.Controllers;

public class ClientesController : Controller
{
    private readonly TiendaDbContext _context;

    public ClientesController(TiendaDbContext context) => _context = context;

    // READ - listado de los cinco atributos de la tabla.
    public async Task<IActionResult> Index()
    {
        var registros = await _context.Clientes.AsNoTracking()
            .OrderBy(x => x.ClienteId).ToListAsync();
        return View(registros);
    }

    // READ - detalles.
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ClienteId == id);
        return modelo is null ? NotFound() : View(modelo);
    }

    // CREATE - formulario.
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Cliente());
    }

    // CREATE - guardar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ClienteId,Nombres,Apellidos,Correo,Telefono")] Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nombres))
            ModelState.AddModelError(nameof(Cliente.Nombres), "Ingresa los nombres.");
        if (string.IsNullOrWhiteSpace(cliente.Apellidos))
            ModelState.AddModelError(nameof(Cliente.Apellidos), "Ingresa los apellidos.");
        if (!string.IsNullOrWhiteSpace(cliente.Correo) &&
            !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(cliente.Correo))
            ModelState.AddModelError(nameof(Cliente.Correo), "Ingresa un correo electrónico válido.");
        if (ModelState.IsValid)
        {
            try
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Registro creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "No se pudo guardar. Comprueba los datos y las relaciones.");
            }
        }
        return View(cliente);
    }

    // UPDATE - formulario.
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Clientes.FindAsync(id);
        if (modelo is null) return NotFound();
        return View(modelo);
    }

    // UPDATE - guardar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ClienteId,Nombres,Apellidos,Correo,Telefono")] Cliente cliente)
    {
        if (id != cliente.ClienteId) return BadRequest();
        if (string.IsNullOrWhiteSpace(cliente.Nombres))
            ModelState.AddModelError(nameof(Cliente.Nombres), "Ingresa los nombres.");
        if (string.IsNullOrWhiteSpace(cliente.Apellidos))
            ModelState.AddModelError(nameof(Cliente.Apellidos), "Ingresa los apellidos.");
        if (!string.IsNullOrWhiteSpace(cliente.Correo) &&
            !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(cliente.Correo))
            ModelState.AddModelError(nameof(Cliente.Correo), "Ingresa un correo electrónico válido.");
        if (ModelState.IsValid)
        {
            var actual = await _context.Clientes.FindAsync(id);
            if (actual is null) return NotFound();
            actual.Nombres = cliente.Nombres;
            actual.Apellidos = cliente.Apellidos;
            actual.Correo = cliente.Correo;
            actual.Telefono = cliente.Telefono;
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
        return View(cliente);
    }

    // DELETE - confirmación.
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var modelo = await _context.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ClienteId == id);
        return modelo is null ? NotFound() : View(modelo);
    }

    // DELETE - eliminación con protección de claves foráneas.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var modelo = await _context.Clientes.FindAsync(id);
        if (modelo is null) return NotFound();
        try
        {
            _context.Clientes.Remove(modelo);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            _context.Entry(modelo).State = EntityState.Unchanged;
            var completo = await _context.Clientes.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ClienteId == id);
            ViewBag.ErrorEliminacion = "No se puede eliminar este registro porque tiene datos relacionados. Elimina primero sus dependencias.";
            return completo is null ? NotFound() : View("Delete", completo);
        }
    }
}
