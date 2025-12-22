using GestionDeInventario.DTOs.EmpleadoDTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestionDeInventario.Views.Empleado
{
    public class RPEmpleado : IDocument
    {
        public EmpleadoResponseDTO Model { get; }
        public RPEmpleado(EmpleadoResponseDTO model) => Model = model;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Black));

                // --- 1. ENCABEZADO ---
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("SISTEMA DE GESTIÓN").FontSize(10).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text("FICHA TÉCNICA DE EMPLEADO").FontSize(22).Black();
                        col.Item().PaddingTop(2).Height(2).Background(Colors.Blue.Medium); // Línea decorativa
                    });

                    row.ConstantItem(100).AlignRight().Column(col =>
                    {
                        col.Item().Text($"ID: #{Model.idEmpleado:D5}").FontSize(12).SemiBold();
                        col.Item().Text($"{DateTime.Now:dd/MM/yyyy}").FontSize(9);
                    });
                });

                // --- 2. CONTENIDO ---
                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Spacing(15);

                    // Bloque Principal: Nombre y Estado
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(innerCol =>
                        {
                            innerCol.Item().Text("NOMBRE COMPLETO").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                            innerCol.Item().Text($"{Model.nombre} {Model.apellido}").FontSize(16).SemiBold().FontColor(Colors.Blue.Darken4);
                        });

                        row.ConstantItem(120).Background(Model.estado == "Activo" ? Colors.Green.Lighten5 : Colors.Red.Lighten5)
                           .Border(1).BorderColor(Model.estado == "Activo" ? Colors.Green.Medium : Colors.Red.Medium)
                           .AlignMiddle().AlignCenter()
                           .Text(Model.estado.ToUpper()).FontSize(12).SemiBold().FontColor(Model.estado == "Activo" ? Colors.Green.Darken3 : Colors.Red.Darken3);
                    });

                    // Sección: Detalles Personales (Grid de 2 columnas)
                    col.Item().PaddingTop(10).Text("INFORMACIÓN PERSONAL").FontSize(10).SemiBold().FontColor(Colors.Blue.Medium);
                    col.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(5).Grid(grid =>
                    {
                        grid.Columns(2);
                        grid.Spacing(5);

                        grid.Item().Element(DatoEstilo).Text(t => { t.Span("Edad: ").Bold(); t.Span($"{Model.edad} años"); });
                        grid.Item().Element(DatoEstilo).Text(t => { t.Span("Género: ").Bold(); t.Span(Model.genero); });
                        grid.Item().Element(DatoEstilo).Text(t => { t.Span("Teléfono: ").Bold(); t.Span(Model.telefono); });
                        grid.Item().Element(DatoEstilo).Text(t => { t.Span("Departamento: ").Bold(); t.Span(Model.departamento?.nombre ?? "N/A"); });
                    });

                    // Sección: Ubicación (Ancho completo)
                    col.Item().PaddingTop(10).Text("UBICACIÓN Y DOMICILIO").FontSize(10).SemiBold().FontColor(Colors.Blue.Medium);
                    col.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(5).Element(DatoEstilo)
                       .Text(t => { t.Span("Dirección: ").Bold(); t.Span(Model.direccion); });

                    // Espacio para Firma o Sello
                    col.Item().PaddingTop(50).AlignRight().Column(c => {
                        c.Item().Width(200).BorderTop(1).PaddingTop(5).AlignCenter().Text("Firma Autorizada").FontSize(10);
                        c.Item().Width(200).AlignCenter().Text("Recursos Humanos").FontSize(8).Italic();
                    });
                });

                // --- 3. PIE DE PÁGINA ---
                page.Footer().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(10).Row(row =>
                {
                    row.RelativeItem().Text("Este documento es confidencial y para uso interno.").FontSize(8).Italic();
                    row.RelativeItem().AlignRight().Text(t =>
                    {
                        t.Span("Página ");
                        t.CurrentPageNumber();
                        t.Span(" de ");
                        t.TotalPages();
                    });
                });
            });
        }

        // Estilo auxiliar para las celdas de datos
        static IContainer DatoEstilo(IContainer container) => container.PaddingVertical(5);
    }
}
