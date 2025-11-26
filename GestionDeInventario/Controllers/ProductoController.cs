using GestionDeInventario.DTOs.ProductoDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Implementations;
using GestionDeInventario.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Controllers
{
    // Hereda de Controller (para vistas Razor)
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;

        // Inyección del Servicio
        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // =========================================================
        // 1. READ: Listado de Productos (Index)
        // =========================================================

        public async Task<IActionResult> Index(string nombre, int registros = 5)
        {
            
            IQueryable<ProductoResponseDTO> query = _productoService.GetQueryable();

            // 2. APLICAR FILTRO POR NOMBRE
            if (!string.IsNullOrEmpty(nombre))
            {
                // Usar ToLower() en ambos lados es buena práctica para búsquedas case-insensitive (si el proveedor DB lo soporta)
                query = query.Where(c => c.nombre.ToLower().Contains(nombre.ToLower()));
            }

            // 3. APLICAR LÍMITE DE REGISTROS (PAGINACIÓN SIMPLE)
            if (registros > 0)
            {
                // El método Take añade una cláusula TOP/LIMIT a la sentencia SQL.
                query = query.Take(registros);
            }

            // 4. EJECUTAR LA CONSULTA FINAL DENTRO DEL BLOQUE TRY
            try
            {
                // SOLO AQUÍ se ejecuta la consulta optimizada.
                List<ProductoResponseDTO> listaFiltrada = await query.ToListAsync();

                // 5. MANTENER LOS VALORES EN LA VISTA
                ViewData["CurrentFilter"] = nombre;
                ViewData["CurrentRecords"] = registros;

                // 6. DEVOLVER LA VISTA CON LA LISTA FILTRADA Y LIMITADA
                return View(listaFiltrada);
            }
            catch (Exception ex)
            {
                // Captura cualquier error de la base de datos o mapeo
                TempData["MensajeError"] = "Ocurrió un error al cargar la lista de productos: " + ex.Message;
                // Se recomienda Logging aquí

                // Devolver la vista con una lista vacía para evitar errores en la View
                return View(new List<ProductoResponseDTO>());
            }
        }
        // =========================================================
        // 2. CREATE: GET (Mostrar formulario)
        // =========================================================

        public IActionResult Create()
        {
            // Pasa un DTO vacío a la vista para el formulario
            return View(new ProductoCreateDTO());
        }

        // =========================================================
        // 2. CREATE: POST (Procesar formulario)
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoCreateDTO dto)
        {
            // 💡 1. Validación de Sintaxis (ModelState) antes de llamar al Servicio
            if (!ModelState.IsValid)
            {
                // Si falla la validación del DTO (ej. campos requeridos vacíos)
                return View(dto);
            }

            try
            {
                // El Controller llama al Servicio para ejecutar la lógica de negocio
                await _productoService.AddAsync(dto);

                TempData["SuccessMessage"] = "Producto creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            // 🚨 Captura de Excepciones de Dominio
            catch (BusinessRuleException ex)
            {
                // Si el Servicio falla por una regla de negocio (ej. precio <= 0)
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                // Error inesperado
                TempData["ErrorMessage"] = "Error al crear el producto. Inténtelo de nuevo.";
                // Logger.LogError(ex, "Error creating product");
                return View(dto);
            }
        }

        // =========================================================
        // 3. UPDATE: GET (Cargar producto para edición)
        // =========================================================

        //public async Task<IActionResult> Edit(int id)
        //{
        //    try
        //    {
        //        var productoDto = await _productoService.GetByIdAsync(id);

        //        if (productoDto == null)
        //        {
        //            // 🚨 Si el servicio devuelve null, lanzamos una excepción o redirigimos con error
        //            throw new NotFoundException($"Producto con ID {id} no encontrado.");
        //        }

        //        // 💡 Mapeo manual simple de ResponseDTO a UpdateDTO para llenar el formulario
        //        var updateDto = new ProductoUpdateDTO
        //        {
        //            nombre = productoDto.nombre,
        //            descripcion = productoDto.descripcion,
        //            cantidadStock = productoDto.cantidadStock,
        //            unidadMedida = productoDto.unidadMedida,
        //            precio = productoDto.precio,
        //            estado = productoDto.estado
        //        };

        //        ViewBag.IdProducto = id; // Pasar el ID que se necesita en el POST
        //        return View(updateDto);
        //    }
        //    // 🚨 Captura de Excepciones de Dominio (NotFoundException)
        //    catch (NotFoundException ex)
        //    {
        //        TempData["ErrorMessage"] = ex.Message;
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception)
        //    {
        //        TempData["ErrorMessage"] = "No se pudo cargar el producto para edición.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //}
        // --- DESPUÉS (Se confía en la excepción del Servicio):
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Si no existe, GetByIdAsync ahora lanza NotFoundException
                var productoDto = await _productoService.GetByIdAsync(id);

                // 💡 Mapeo manual simple de ResponseDTO a UpdateDTO para llenar el formulario
                var updateDto = new ProductoUpdateDTO
                {
                    nombre = productoDto.nombre,
                    descripcion = productoDto.descripcion,
                    cantidadStock = productoDto.cantidadStock,
                    unidadMedida = productoDto.unidadMedida,
                    precio = productoDto.precio,
                    estado = productoDto.estado
                };

                ViewBag.IdProducto = id;
                return View(updateDto);
            }
            // El 'catch' existente maneja la excepción perfectamente.
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "No se pudo cargar el producto para edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        // =========================================================
        // 3. UPDATE: POST (Guardar cambios)
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IdProducto = id;
                return View(dto);
            }

            try
            {
                await _productoService.UpdateAsync(id, dto);

                TempData["SuccessMessage"] = "Producto actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            // 🚨 Captura de Excepciones de Dominio (NotFound y BusinessRule)
            catch (NotFoundException ex)
            {
                // El producto ya no existe: redirigir al listado con mensaje de error
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                // Falla la validación de negocio (ej. stock negativo): volver a la vista
                ModelState.AddModelError("", ex.Message);
                ViewBag.IdProducto = id;
                return View(dto);
            }
        }

        // GET: /Empleado/Details/5 (Muestra los detalles de un empleado por ID)
        //public async Task<IActionResult> Details(int id)
        //{
        //    // 1. Llama al servicio para obtener la categoría por ID
        //    ProductoResponseDTO? producto = await _productoService.GetByIdAsync(id);

        //    // 2. Verifica si la categoría existe
        //    if (producto == null)
        //    {
        //        return NotFound(); // Retorna error 404 si no se encuentra
        //    }
        //    // 3. Envía el objeto a la vista "Details.cshtml"
        //    return View(producto);
        //}
        // --- DESPUÉS: (Lanza NotFoundException desde el servicio, capturada aquí)
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // 1. Llama al servicio (ahora lanza NotFoundException si no existe)
                ProductoResponseDTO producto = await _productoService.GetByIdAsync(id);

                // 2. Envía el objeto a la vista
                return View(producto);
            }
            catch (NotFoundException)
            {
                // 🚨 Captura la excepción y retorna 404
                return NotFound();
            }
        }

        // =========================================================
        // 4. DELETE: POST (Eliminar)
        // =========================================================
        // Nota: Las eliminaciones a menudo se hacen con POST por seguridad
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var empleado = await _productoService.GetByIdAsync(id.Value);

            if (empleado == null)
            {
                TempData["MensajeError"] = "Error: El empleado solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
            return View(empleado);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productoService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Producto eliminado exitosamente.";
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al intentar eliminar el producto.";
                // Logger.LogError(ex, "Error deleting product");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
