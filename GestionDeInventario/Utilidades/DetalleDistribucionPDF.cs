using GestionDeInventario.DTOs.DetalleDistribucionDTOs;
using GestionDeInventario.Views.DetalleDistribucion;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace GestionDeInventario.Utilidades
{
    public class DetalleDistribucionPDF
    {
        public byte[] GenerarArchivoFicha(DetalleDistribucionResponseDTO dto)
        {
            // Configurar licencia
            QuestPDF.Settings.License = LicenseType.Community;

            var documento = new RPDetalleDistribucion(dto);
            return documento.GeneratePdf();
        }
    }
}
