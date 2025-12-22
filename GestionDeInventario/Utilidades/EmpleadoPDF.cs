using GestionDeInventario.DTOs.EmpleadoDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Views.Empleado;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace GestionDeInventario.Utilidades
{
    public class EmpleadoPDF
    {
        public byte[] GenerarArchivoFicha(EmpleadoResponseDTO dto)
        {
            // Configurar licencia obligatoria en versiones recientes
            QuestPDF.Settings.License = LicenseType.Community;

            var documento = new RPEmpleado(dto);
            return documento.GeneratePdf();
        }
    }
}
