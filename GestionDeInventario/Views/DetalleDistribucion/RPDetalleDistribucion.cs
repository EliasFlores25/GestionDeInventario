using GestionDeInventario.DTOs.DetalleDistribucionDTOs;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using QuestPDF.Fluent;

namespace GestionDeInventario.Views.DetalleDistribucion
{
    public class RPDetalleDistribucion : IDocument
    {
        public DetalleDistribucionResponseDTO Model { get; }

        public RPDetalleDistribucion(DetalleDistribucionResponseDTO model)
        {
            Model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Portrait());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                // ENCABEZADO
                page.Header()
                    .Column(column =>
                    {
                        // Título y logo
                        column.Item()
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Column(col =>
                                    {
                                        col.Item()
                                            .Text("SISTEMA DE INVENTARIO")
                                            .FontSize(12)
                                            .Bold()
                                            .FontColor(Colors.Blue.Darken3);

                                        col.Item()
                                            .Text("REPORTE DE DISTRIBUCIÓN")
                                            .FontSize(16)
                                            .Bold()
                                            .FontColor(Colors.Black);
                                    });

                                row.ConstantItem(80)
                                    .AlignRight()
                                    .Text(text =>
                                    {
                                        text.Span("Distribución:\n").Bold();
                                        text.Span(Model.NumeroDistribucion).FontSize(9);
                                    });
                            });

                        column.Item()
                            .PaddingVertical(10)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Medium);

                        // Información de distribución
                        column.Item()
                            .PaddingBottom(5)
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Text(text =>
                                    {
                                        text.Span("Fecha Salida: ").Bold();
                                        text.Span($"{Model.FechaSalida:dd/MM/yyyy}");
                                    });

                                row.RelativeItem()
                                    .AlignRight()
                                    .Text(text =>
                                    {
                                        text.Span("Motivo: ").Bold();
                                        text.Span(Model.Motivo ?? "Sin especificar");
                                    });
                            });
                    });

                // CONTENIDO PRINCIPAL
                page.Content()
                    .PaddingVertical(10)
                    .Column(column =>
                    {
                        // SECCIÓN 1: Información de partes
                        column.Item()
                            .PaddingBottom(15)
                            .Row(row =>
                            {
                                // Empleado asignado
                                row.RelativeItem()
                                    .Background(Colors.Grey.Lighten5)
                                    .Padding(10)
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Column(col =>
                                    {
                                        col.Item()
                                            .Text("EMPLEADO ASIGNADO")
                                            .FontSize(11)
                                            .Bold()
                                            .FontColor(Colors.Blue.Darken3);

                                        col.Item().PaddingTop(3);

                                        if (Model.Empleado != null)
                                        {
                                            col.Item()
                                                .Text($"{Model.Empleado.nombre} {Model.Empleado.apellido}")
                                                .Bold()
                                                .FontSize(11);

                                            col.Item()
                                                .Text(text =>
                                                {
                                                    text.Span("Departamento: ");
                                                    text.Span(Model.Empleado.departamento?.nombre ?? "N/A");
                                                });

                                            col.Item()
                                                .Text(text =>
                                                {
                                                    text.Span("Teléfono: ");
                                                    text.Span(Model.Empleado.telefono ?? "N/A");
                                                });
                                        }
                                        else
                                        {
                                            col.Item().Text("N/A").Italic();
                                        }
                                    });

                                // Usuario que registra
                                row.RelativeItem()
                                    .PaddingLeft(10)
                                    .Background(Colors.Grey.Lighten5)
                                    .Padding(10)
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Column(col =>
                                    {
                                        col.Item()
                                            .Text("REGISTRADO POR")
                                            .FontSize(11)
                                            .Bold()
                                            .FontColor(Colors.Blue.Darken3);

                                        col.Item().PaddingTop(3);

                                        if (Model.Usuario != null)
                                        {
                                            col.Item()
                                                .Text(Model.Usuario.nombre)
                                                .Bold()
                                                .FontSize(11);
                                        }
                                        else
                                        {
                                            col.Item().Text("N/A").Italic();
                                        }
                                    });
                            });

                        // SECCIÓN 2: Detalles del producto
                        column.Item()
                            .PaddingBottom(10)
                            .Background(Colors.Grey.Lighten5)
                            .Padding(10)
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Column(col =>
                            {
                                col.Item()
                                    .Text("DETALLES DEL PRODUCTO")
                                    .FontSize(11)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken3);

                                col.Item().PaddingTop(5);

                                if (Model.Producto != null)
                                {
                                    col.Item()
                                        .Row(row =>
                                        {
                                            row.RelativeItem()
                                                .Text(text =>
                                                {
                                                    text.Span("ID Producto: ").Bold();
                                                    text.Span(Model.ProductoId.ToString());
                                                });

                                            row.RelativeItem()
                                                .Text(text =>
                                                {
                                                    text.Span("Producto: ").Bold();
                                                    text.Span(Model.Producto.nombre);
                                                });

                                            row.RelativeItem()
                                                .Text(text =>
                                                {
                                                    text.Span("Unidad: ").Bold();
                                                    text.Span(Model.Producto.unidadMedida ?? "N/A");
                                                });
                                        });
                                }
                                else
                                {
                                    col.Item().Text("Producto no disponible").Italic();
                                }
                            });

                        // SECCIÓN 3: Tabla de distribución
                        column.Item()
                            .PaddingBottom(15)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);  // #
                                    columns.RelativeColumn(3);   // Descripción
                                    columns.ConstantColumn(70);  // Cantidad
                                    columns.ConstantColumn(90);  // Precio Costo
                                    columns.ConstantColumn(90);  // Valor Total
                                });

                                // Encabezado
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellHeader).Text("#").AlignCenter();
                                    header.Cell().Element(CellHeader).Text("DESCRIPCIÓN");
                                    header.Cell().Element(CellHeader).Text("CANTIDAD").Light();
                                    header.Cell().Element(CellHeader).Text("PRECIO COSTO").Light();
                                    header.Cell().Element(CellHeader).Text("VALOR TOTAL").Light();
                                });

                                // Datos
                                table.Cell().Element(CellData).Text("1").AlignCenter();
                                table.Cell().Element(CellData).Text(Model.Producto?.nombre ?? "Producto");
                                table.Cell().Element(CellData).Text(Model.Cantidad.ToString("N0")).Light();
                                table.Cell().Element(CellData).Text(FormatCurrency(Model.PrecioCostoUnitario)).Light();
                                table.Cell().Element(CellData).Text(FormatCurrency(Model.MontoTotal)).Light();

                                // Resumen
                                table.Cell().ColumnSpan(5).Element(CellSummary)
                                    .PaddingTop(10)
                                    .Row(row =>
                                    {
                                        row.RelativeItem();
                                        row.ConstantItem(250)
                                            .Column(col =>
                                            {
                                                // Cantidad total
                                                col.Item()
                                                    .Row(sumRow =>
                                                    {
                                                        sumRow.RelativeItem()
                                                            .Text("Cantidad Total:")
                                                            .FontSize(10);

                                                        sumRow.ConstantItem(80)
                                                            .AlignRight()
                                                            .Text($"{Model.Cantidad:N0} unidades")
                                                            .FontSize(10);
                                                    });

                                                // Valor total - ETIQUETA EN NEGRO
                                                col.Item()
                                                    .PaddingTop(5)
                                                    .BorderTop(1)
                                                    .BorderColor(Colors.Grey.Medium)
                                                    .PaddingTop(5)
                                                    .Row(totalRow =>
                                                    {
                                                        totalRow.RelativeItem()
                                                            .Text("VALOR TOTAL:")
                                                            .Bold()
                                                            .FontSize(12)
                                                            .FontColor(Colors.Black);  // NEGRO

                                                        totalRow.ConstantItem(90)
                                                            .AlignRight()
                                                            .Text(FormatCurrency(Model.MontoTotal))
                                                            .Bold()
                                                            .FontSize(12)
                                                            .FontColor(Colors.Blue.Darken3);  // VALOR EN AZUL
                                                    });
                                            });
                                    });
                            });

                    });

                // PIE DE PÁGINA
                page.Footer()
                    .AlignCenter()
                    .PaddingTop(10)
                    .Column(col =>
                    {
                        col.Item()
                            .LineHorizontal((float)0.5)
                            .LineColor(Colors.Grey.Lighten1);

                        col.Item()
                            .PaddingTop(5)
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .AlignLeft()
                                    .Text("Control de Inventario")
                                    .FontSize(8)
                                    .FontColor(Colors.Grey.Medium);

                                row.RelativeItem()
                                    .AlignCenter()
                                    .Text(text =>
                                    {
                                        text.Span("Página ");
                                        text.CurrentPageNumber();
                                        text.Span(" de ");
                                        text.TotalPages();
                                    });

                                row.RelativeItem()
                                    .AlignRight()
                                    .Text($"{DateTime.Now:dd/MM/yyyy HH:mm}")
                                    .FontSize(8)
                                    .FontColor(Colors.Grey.Medium);
                            });
                    });
            });
        }

        private string FormatCurrency(decimal amount)
        {
            return amount.ToString("C", CultureInfo.GetCultureInfo("en-US"));
        }

        private static IContainer CellHeader(IContainer container)
        {
            return container
                .Background(Colors.Blue.Lighten5)
                .PaddingVertical(7)
                .PaddingHorizontal(5)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten1);
        }

        private static IContainer CellData(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(7)
                .PaddingHorizontal(5);
        }

        private static IContainer CellSummary(IContainer container)
        {
            return container
                .BorderTop(2)
                .BorderColor(Colors.Grey.Medium)
                .PaddingTop(5);
        }
    }
}