using GestionDeInventario.DTOs.DetalleCompraDTOs;
using GestionDeInventario.Views.DetalleCompra;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace GestionDeInventario.Utilidades
{
    public class DetalleCompraPDF
    {
        public byte[] GenerarArchivoFicha(DetalleCompraResponseDTO dto)
        {
            // Configurar licencia obligatoria en versiones recientes
            QuestPDF.Settings.License = LicenseType.Community;

            var documento = new RPDetalleCompra(dto);
            return documento.GeneratePdf();
        }
    }
}
