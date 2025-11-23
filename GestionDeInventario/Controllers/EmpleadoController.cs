using GestionDeInventario.Models;
using GestionDeInventario.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly IEmpleadoService _empleadoService;
        public EmpleadoController(IEmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }
        // GET: /Empleado/ (Lista de empleados con búsqueda y paginación)
        public async Task<IActionResult> Index(string nombre, int registros = 10)
        {
            // 1. OBTENER LA CONSULTA BASE (IQueryable)
            // Esto NO ejecuta una consulta SQL. Simplemente prepara la base.
            IQueryable<Empleado> query = _empleadoService.GetQueryable();

            // 2. APLICAR FILTRO POR NOMBRE
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(c => c.nombre.Contains(nombre));
            }
            // 3. APLICAR LÍMITE DE REGISTROS (PAGINACIÓN SIMPLE)
            if (registros > 0)
            {
                // El método Take añade una cláusula TOP/LIMIT a la sentencia SQL.
                query = query.Take(registros);
            }
            // 4. EJECUTAR LA CONSULTA FINAL
            // SOLO AQUÍ se contacta a la base de datos con una ÚNICA y OPTIMIZADA consulta SQL.
            List<Empleado> listaFiltrada = await query.ToListAsync();

            // 5. MANTENER LOS VALORES EN LA VISTA
            // Esto es para que el usuario vea los filtros que aplicó.
            ViewData["CurrentFilter"] = nombre;
            ViewData["CurrentRecords"] = registros;

            // 6. DEVOLVER LA VISTA
            return View(listaFiltrada);
        }


        // GET: /Empleado/Details/5 (Muestra los detalles de un empleado por ID)
        public async Task<IActionResult> Details(int id)
        {
            // 1. Llama al servicio para obtener la categoría por ID
            Empleado? empleado = await _empleadoService.GetByIdAsync(id);

            // 2. Verifica si la categoría existe
            if (empleado == null)
            {
                return NotFound(); // Retorna error 404 si no se encuentra
            }
            // 3. Envía el objeto a la vista "Details.cshtml"
            return View(empleado);
        }

        // GET: /Empleado/Crear (Muestra el formulario vacío)
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Empleado/Crear (Recibe los datos del formulario)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Empleado empleado)
        {
            Empleado empleadoCreado = await _empleadoService.AddAsync(empleado);

            if (empleadoCreado != null && empleadoCreado.idEmpleado > 0)
            {
                TempData["MensajeExito"] = "OK: El empleado se ha creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Error: No se pudo crear el empleado. Verifica los datos.");
            return View(empleado);
        }

        // GET: /Empleado/Editar/5 (Muestra el formulario con datos existentes)
        public async Task<IActionResult> Edit(int id)
        {
            Empleado? empleado = await _empleadoService.GetByIdAsync(id);

            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }

        // POST: /Empleado/Editar/5 (Recibe los datos actualizados del formulario)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Empleado empleado)
        {
            bool exito = await _empleadoService.UpdateAsync(empleado.idEmpleado, empleado);

            if (exito)
            {
                TempData["MensajeExito"] = "OK: El empleado se ha actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Error: No se pudo actualizar el empleado. Verifique la existencia y los datos.");
            return View(empleado);
        }


        // 1. MÉTODO HTTP GET: Muestra la vista de confirmación.
        // Es el que necesitas para que la URL /Categoria/Delete/1 funcione.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var empleado = await _empleadoService.GetByIdAsync(id.Value);

            if (empleado == null)
            {
                TempData["MensajeError"] = "Error: El empleado solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
            return View(empleado);
        }
       
        //
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            bool exito = await _empleadoService.DeleteAsync(id);

            if (exito)
            {
                TempData["MensajeExito"] = "OK: El empleado se ha eliminado exitosamente.";
            }
            else
            {
                TempData["MensajeError"] = "Error: No se pudo eliminar el empleado. Es posible que no exista.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
