using GestionDeInventario.DTOs.DetalleCompraDTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace GestionDeInventario.Views.DetalleCompra
{
    public class RPDetalleCompra : IDocument
    {
        public DetalleCompraResponseDTO Model { get; }

        public RPDetalleCompra(DetalleCompraResponseDTO model)
        {
            Model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // Configurar tamaño y márgenes
                page.Size(PageSizes.A4.Portrait());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                // ENCABEZADO
                page.Header()
                    .Column(column =>
                    {
                        // Título principal
                        column.Item()
                            .AlignCenter()
                            .Text("DETALLE DE COMPRA")
                            .FontSize(16)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        // Línea divisoria
                        column.Item()
                            .PaddingVertical(5)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten1);

                        // Información básica
                        column.Item()
                            .PaddingTop(5)
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Text($"Factura: {Model.numeroFactura}")
                                    .Bold();

                                row.RelativeItem()
                                    .AlignRight()
                                    .Text($"Fecha: {Model.fechaCompra:dd/MM/yyyy HH:mm}")
                                    .FontColor(Colors.Grey.Medium);
                            });
                    });

                // CONTENIDO PRINCIPAL
                page.Content()
                    .PaddingVertical(15)
                    .Column(column =>
                    {
                        // SECCIÓN 1: Información de la compra
                        column.Item()
                            .Background(Colors.Grey.Lighten5)
                            .Padding(10)
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Column(section =>
                            {
                                section.Item()
                                    .Text("INFORMACIÓN DE LA COMPRA")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken3);

                                section.Item().PaddingTop(5);

                                // Datos del proveedor
                                section.Item()
                                    .Text(text =>
                                    {
                                        text.Span("Proveedor: ").Bold();
                                        text.Span($"{Model.proveedor?.nombreEmpresa ?? "N/A"}");
                                    });

                                // Datos del producto
                                section.Item()
                                    .Text(text =>
                                    {
                                        text.Span("Producto: ").Bold();
                                        text.Span($"{Model.producto?.nombre ?? "N/A"}");
                                    });

                                // Datos del usuario
                                section.Item()
                                    .Text(text =>
                                    {
                                        text.Span("Registrado por: ").Bold();
                                        text.Span($"{Model.usuario?.nombre ?? "N/A"}");
                                    });
                            });

                        column.Item().PaddingTop(20);

                        // SECCIÓN 2: Detalles de la compra (Tabla)
                        column.Item()
                            .Text("DETALLES DE LA COMPRA")
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        column.Item().PaddingTop(5);

                        // Tabla de detalles
                        column.Item()
                            .Table(table =>
                            {
                                // Definir columnas
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(40); // Ítem
                                    columns.RelativeColumn(3);  // Descripción
                                    columns.ConstantColumn(80); // Cantidad
                                    columns.ConstantColumn(100); // Precio Unit.
                                    columns.ConstantColumn(100); // Total
                                });

                                // Encabezado de la tabla
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("#").Bold();
                                    header.Cell().Element(CellStyle).Text("DESCRIPCIÓN").Bold();
                                    header.Cell().Element(CellStyle).AlignRight().Text("CANTIDAD").Bold();
                                    header.Cell().Element(CellStyle).AlignRight().Text("PRECIO UNIT.").Bold();
                                    header.Cell().Element(CellStyle).AlignRight().Text("TOTAL").Bold();

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container
                                            .Background(Colors.Blue.Lighten5)
                                            .PaddingVertical(8)
                                            .PaddingHorizontal(5)
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Grey.Lighten1);
                                    }
                                });

                                // Fila de datos
                                table.Cell().Element(CellStyle).Text("1");
                                table.Cell().Element(CellStyle).Text(Model.producto?.nombre ?? "Producto");
                                table.Cell().Element(CellStyle).AlignRight().Text(Model.cantidad.ToString("N0"));
                                table.Cell().Element(CellStyle).AlignRight().Text(FormatCurrency(Model.precioUnitarioCosto));
                                table.Cell().Element(CellStyle).AlignRight().Text(FormatCurrency(Model.montoTotal));

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .PaddingVertical(8)
                                        .PaddingHorizontal(5);
                                }
                            });

                        column.Item().PaddingTop(30);

                        // SECCIÓN 3: Resumen total
                        column.Item()
                            .AlignRight()
                            .Width(200)
                            .Background(Colors.Grey.Lighten5)
                            .Padding(15)
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Column(summary =>
                            {
                                summary.Item()
                                    .Row(row =>
                                    {
                                        row.RelativeItem()
                                            .Text("Subtotal:")
                                            .FontSize(11);

                                        row.ConstantItem(80)
                                            .AlignRight()
                                            .Text(FormatCurrency(Model.precioUnitarioCosto))
                                            .FontSize(11);
                                    });

                                summary.Item()
                                    .Row(row =>
                                    {
                                        row.RelativeItem()
                                            .Text("Cantidad:")
                                            .FontSize(11);

                                        row.ConstantItem(80)
                                            .AlignRight()
                                            .Text($"x {Model.cantidad}")
                                            .FontSize(11);
                                    });

                                summary.Item()
                                    .PaddingTop(5)
                                    .BorderTop(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Row(row =>
                                    {
                                        row.RelativeItem()
                                            .Text("TOTAL:")
                                            .Bold()
                                            .FontSize(12);

                                        row.ConstantItem(80)
                                            .AlignRight()
                                            .Text(FormatCurrency(Model.montoTotal))
                                            .Bold()
                                            .FontSize(12)
                                            .FontColor(Colors.Blue.Darken3);
                                    });
                            });
                    });

                // PIE DE PÁGINA
                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                        text.Span(" | Sistema de Gestión de Inventario");
                    });
            });
        }

        private string FormatCurrency(decimal amount)
        {
            return amount.ToString("C", new CultureInfo("es-HN"));
        }
    }
}