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
                                            .Text("REPORTE DE COMPRA")
                                            .FontSize(16)
                                            .Bold()
                                            .FontColor(Colors.Black);
                                    });
                            });

                        column.Item()
                            .PaddingVertical(10)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Medium);

                        // Información de compra
                        column.Item()
                            .PaddingBottom(5)
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Text(text =>
                                    {
                                        text.Span("Fecha: ").Bold();
                                        text.Span($"{DateTime.Now:dd/MM/yyyy}");
                                    });

                                row.RelativeItem()
                                    .AlignRight()
                                    .Text(text =>
                                    {
                                        text.Span("Detalle ID: ").Bold();
                                        text.Span(Model.IdDetalleCompra.ToString());
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
                                // Proveedor (si está disponible en Compra)
                                row.RelativeItem()
                                    .Background(Colors.Grey.Lighten5)
                                    .Padding(10)
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Column(col =>
                                    {
                                        col.Item()
                                            .Text("INFORMACIÓN DE COMPRA")
                                            .FontSize(11)
                                            .Bold()
                                            .FontColor(Colors.Blue.Darken3);

                                        col.Item().PaddingTop(3);

                                        if (Model.Compra != null)
                                        {
                                            col.Item()
                                                .Text(text =>
                                                {
                                                    text.Span("Número de Compra: ").Bold();
                                                    text.Span(Model.Compra.IdCompra.ToString());
                                                });

                                            // Mostrar información del proveedor si está disponible
                                            if (Model.Compra.Proveedor != null)
                                            {
                                                col.Item()
                                                    .Text(text =>
                                                    {
                                                        text.Span("Proveedor: ").Bold();
                                                        text.Span(Model.Compra.Proveedor.nombreEmpresa ?? "N/A");
                                                    });
                                            }
                                        }
                                        else
                                        {
                                            col.Item().Text("Información de compra no disponible").Italic();
                                        }
                                    });

                                // Producto
                                row.RelativeItem()
                                    .PaddingLeft(10)
                                    .Background(Colors.Grey.Lighten5)
                                    .Padding(10)
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Column(col =>
                                    {
                                        col.Item()
                                            .Text("PRODUCTO")
                                            .FontSize(11)
                                            .Bold()
                                            .FontColor(Colors.Blue.Darken3);

                                        col.Item().PaddingTop(3);

                                        if (Model.Producto != null)
                                        {
                                            col.Item()
                                                .Text(Model.Producto.nombre ?? "Producto")
                                                .Bold()
                                                .FontSize(11);

                                            col.Item()
     .Text(text =>
     {
         text.Span("ID Producto: ");
         text.Span(Model.Producto.idProducto.ToString());
     });
                                        }
                                        else
                                        {
                                            col.Item().Text("Producto no disponible").Italic();
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
                                                    text.Span(Model.Producto.nombre ?? "N/A");
                                                });
                                            row.RelativeItem()
                                                .Text(text =>
                                                {
                                                    text.Span("Producto: ").Bold();
                                                    text.Span(Model.Producto.unidadMedida ?? "N/A");
                                                });

                                        });
                                }
                                else
                                {
                                    col.Item().Text("Producto no disponible").Italic();
                                }
                            });

                        // SECCIÓN 3: Tabla de compra
                        column.Item()
                            .PaddingBottom(15)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);  // #
                                    columns.RelativeColumn(3);   // Descripción
                                    columns.ConstantColumn(70);  // Cantidad
                                    columns.ConstantColumn(90);  // Precio Unit.
                                    columns.ConstantColumn(90);  // Subtotal
                                });

                                // Encabezado
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellHeader).Text("#").AlignCenter();
                                    header.Cell().Element(CellHeader).Text("DESCRIPCIÓN");
                                    header.Cell().Element(CellHeader).Text("CANTIDAD").Light();
                                    header.Cell().Element(CellHeader).Text("PRECIO UNIT.").Light();
                                    header.Cell().Element(CellHeader).Text("SUBTOTAL").Light();
                                });

                                // Datos - usando el Subtotal del DTO en lugar de calcularlo
                                table.Cell().Element(CellData).Text("1").AlignCenter();
                                table.Cell().Element(CellData).Text(Model.Producto?.nombre ?? "Producto");
                                table.Cell().Element(CellData).Text(Model.Cantidad.ToString("N0")).Light();
                                table.Cell().Element(CellData).Text(FormatUSD(Model.PrecioUnitarioCosto)).Light();
                                table.Cell().Element(CellData).Text(FormatUSD(Model.Subtotal)).Light();

                                // Total
                                table.Cell().ColumnSpan(5).Element(CellTotal)
                                    .PaddingTop(10)
                                    .Row(row =>
                                    {
                                        row.RelativeItem();
                                        row.ConstantItem(250)
                                            .Row(totalRow =>
                                            {
                                                totalRow.RelativeItem()
                                                    .Text("TOTAL:")
                                                    .Bold()
                                                    .FontSize(12);

                                                totalRow.ConstantItem(90)
                                                    .AlignRight()
                                                    .Text(FormatUSD(Model.Subtotal))
                                                    .Bold()
                                                    .FontSize(12)
                                                    .FontColor(Colors.Blue.Darken3);
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

        private string FormatUSD(decimal amount)
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

        private static IContainer CellTotal(IContainer container)
        {
            return container
                .BorderTop(2)
                .BorderColor(Colors.Grey.Medium)
                .PaddingTop(5);
        }
    }
}