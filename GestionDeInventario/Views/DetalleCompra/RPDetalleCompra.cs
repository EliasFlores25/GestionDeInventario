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

                                row.ConstantItem(80)
                                    .AlignRight()
                                    .Text(text =>
                                    {
                                        text.Span("Factura:\n").Bold();
                                        text.Span(Model.numeroFactura).FontSize(9);
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
                                        text.Span($"{Model.fechaCompra:dd/MM/yyyy}");
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
                                // Proveedor
                                row.RelativeItem()
                                    .Background(Colors.Grey.Lighten5)
                                    .Padding(10)
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Column(col =>
                                    {
                                        col.Item()
                                            .Text("PROVEEDOR")
                                            .FontSize(11)
                                            .Bold()
                                            .FontColor(Colors.Blue.Darken3);

                                        col.Item().PaddingTop(3);

                                        if (Model.proveedor != null)
                                        {
                                            col.Item()
                                                .Text(Model.proveedor.nombreEmpresa)
                                                .Bold()
                                                .FontSize(11);

                                            col.Item()
                                                .Text(text =>
                                                {
                                                    text.Span("Contacto: ");
                                                    text.Span(Model.proveedor.telefono ?? "N/A");
                                                });

                                            col.Item()
                                                .Text(text =>
                                                {
                                                    text.Span("Dirección: ");
                                                    text.Span(Model.proveedor.direccion ?? "N/A");
                                                });
                                        }
                                        else
                                        {
                                            col.Item().Text("N/A").Italic();
                                        }
                                    });

                                // Usuario
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

                                        if (Model.usuario != null)
                                        {
                                            col.Item()
                                                .Text(Model.usuario.nombre)
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

                                if (Model.producto != null)
                                {
                                    col.Item()
                                        .Row(row =>
                                        {

                                            row.RelativeItem()
                                        .Text(text =>
     {
         text.Span("ID Producto: ").Bold();
         text.Span(Model.productoId.ToString());
     });

                                            row.RelativeItem()
                                                .Text(text =>
                                                {
                                                    text.Span("Producto: ").Bold();
                                                    text.Span(Model.producto.nombre);
                                                });

                                            row.RelativeItem()
                                                .Text(text =>
                                                {
                                                    text.Span("Unidad: ").Bold();
                                                    text.Span(Model.producto.unidadMedida ?? "N/A");
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

                                // Datos
                                table.Cell().Element(CellData).Text("1").AlignCenter();
                                table.Cell().Element(CellData).Text(Model.producto?.nombre ?? "Producto");
                                table.Cell().Element(CellData).Text(Model.cantidad.ToString("N0")).Light();
                                table.Cell().Element(CellData).Text(FormatUSD(Model.precioUnitarioCosto)).Light();
                                table.Cell().Element(CellData).Text(FormatUSD(Model.precioUnitarioCosto * Model.cantidad)).Light();

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
                                                    .Text(FormatUSD(Model.montoTotal))
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