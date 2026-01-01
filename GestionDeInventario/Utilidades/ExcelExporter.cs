using GestionDeInventario.DTOs.DetalleCompraDTOs;
using GestionDeInventario.DTOs.DetalleDistribucionDTOs;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace GestionDeInventario.Utilidades
{
    public class ExcelExporter
    {
        public byte[] ExportarDetalleDistribucion(
            List<DetalleDistribucionExcelDTO> datos,
            string nombreHoja = "Distribuciones")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(nombreHoja);

                // ENCABEZADOS usando Display Name del DTO
                var properties = typeof(DetalleDistribucionExcelDTO).GetProperties();

                for (int i = 0; i < properties.Length; i++)
                {
                    var displayAttr = properties[i].GetCustomAttributes(typeof(DisplayAttribute), false)
                        .FirstOrDefault() as DisplayAttribute;

                    worksheet.Cells[1, i + 1].Value = displayAttr?.Name ?? properties[i].Name;
                }

                // ESTILO ENCABEZADOS
                using (var range = worksheet.Cells[1, 1, 1, properties.Length])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 99, 235)); // Azul #2563eb
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // DATOS
                int row = 2;
                foreach (var item in datos)
                {
                    worksheet.Cells[row, 1].Value = item.IdDetalleDistribucion;
                    worksheet.Cells[row, 2].Value = item.NumeroDistribucion;
                    worksheet.Cells[row, 3].Value = item.FechaSalida.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4].Value = item.NombreEmpleado;
                    worksheet.Cells[row, 5].Value = item.NombreProducto;
                    worksheet.Cells[row, 6].Value = item.Cantidad;
                    worksheet.Cells[row, 7].Value = item.Motivo;
                    worksheet.Cells[row, 8].Value = item.PrecioCostoUnitario;
                    worksheet.Cells[row, 9].Value = item.MontoTotal;
                    worksheet.Cells[row, 10].Value = item.UsuarioRegistro;
                    row++;
                }

                // ESTILO DATOS
                if (datos.Count > 0)
                {
                    using (var range = worksheet.Cells[2, 1, row - 1, properties.Length])
                    {
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    // Formato moneda para columnas de precios
                    using (var range = worksheet.Cells[2, 8, row - 1, 9])
                    {
                        range.Style.Numberformat.Format = "#,##0.00";
                    }
                }

                // AJUSTAR ANCHO DE COLUMNAS
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // TOTALES (si hay datos)
                if (datos.Count > 0)
                {
                    // Fila de totales
                    worksheet.Cells[row, 1].Value = "TOTALES";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                    // Suma de cantidad
                    worksheet.Cells[row, 6].Formula = $"SUM(F2:F{row - 1})";
                    worksheet.Cells[row, 6].Style.Font.Bold = true;

                    // Suma de valor total
                    worksheet.Cells[row, 9].Formula = $"SUM(I2:I{row - 1})";
                    worksheet.Cells[row, 9].Style.Font.Bold = true;
                    worksheet.Cells[row, 9].Style.Numberformat.Format = "#,##0.00";
                }

                // INFORMACIÓN DEL REPORTE
                int infoRow = row + 2;
                worksheet.Cells[infoRow, 1].Value = "Información del Reporte";
                worksheet.Cells[infoRow, 1].Style.Font.Bold = true;
                worksheet.Cells[infoRow, 1].Style.Font.Size = 11;

                worksheet.Cells[infoRow + 1, 1].Value = $"Total de registros: {datos.Count}";
                worksheet.Cells[infoRow + 2, 1].Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}";
                worksheet.Cells[infoRow + 3, 1].Value = "Sistema de Gestión de Inventario";

                return package.GetAsByteArray();
            }
        }

        // Método genérico reutilizable para otras entidades
        public byte[] ExportarLista<T>(List<T> datos, string nombreHoja = "Reporte")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(nombreHoja);
                var properties = typeof(T).GetProperties();

                // Encabezados
                for (int i = 0; i < properties.Length; i++)
                {
                    var displayAttr = properties[i].GetCustomAttributes(typeof(DisplayAttribute), false)
                        .FirstOrDefault() as DisplayAttribute;

                    worksheet.Cells[1, i + 1].Value = displayAttr?.Name ?? properties[i].Name;
                }

                // Datos usando reflexión
                int row = 2;
                foreach (var item in datos)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var value = properties[i].GetValue(item);
                        worksheet.Cells[row, i + 1].Value = value?.ToString() ?? "";
                    }
                    row++;
                }

                // Estilos básicos
                if (properties.Length > 0)
                {
                    // Encabezados
                    using (var range = worksheet.Cells[1, 1, 1, properties.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 99, 235));
                        range.Style.Font.Color.SetColor(Color.White);
                    }

                    // Ajustar columnas
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }

                return package.GetAsByteArray();
            }
        }


        public byte[] ExportarDetalleCompra(
            List<DetalleCompraExcelDTO> datos,
            string nombreHoja = "Compras")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(nombreHoja);

                // ENCABEZADOS
                string[] headers = {
                    "ID", "N° Factura", "Fecha Compra", "Proveedor",
                    "Producto", "Cantidad", "Precio Unitario", "Monto Total", "Usuario"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // ESTILO ENCABEZADOS
                using (var range = worksheet.Cells[1, 1, 1, headers.Length])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 99, 235));
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // DATOS
                int row = 2;
                foreach (var item in datos)
                {
                    worksheet.Cells[row, 1].Value = item.IdDetalleCompra;
                    worksheet.Cells[row, 2].Value = item.NumeroFactura;
                    worksheet.Cells[row, 3].Value = item.FechaCompra.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4].Value = item.NombreProveedor;
                    worksheet.Cells[row, 5].Value = item.NombreProducto;
                    worksheet.Cells[row, 6].Value = item.Cantidad;
                    worksheet.Cells[row, 7].Value = item.PrecioUnitarioCosto;
                    worksheet.Cells[row, 8].Value = item.MontoTotal;
                    worksheet.Cells[row, 9].Value = item.UsuarioRegistro;
                    row++;
                }

                // ESTILO DATOS
                if (datos.Count > 0)
                {
                    using (var range = worksheet.Cells[2, 1, row - 1, headers.Length])
                    {
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    // Formato moneda
                    using (var range = worksheet.Cells[2, 7, row - 1, 8])
                    {
                        range.Style.Numberformat.Format = "#,##0.00";
                    }
                }

                // AJUSTAR ANCHO
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // TOTALES
                if (datos.Count > 0)
                {
                    worksheet.Cells[row, 1].Value = "TOTALES";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                    // Suma de cantidad
                    worksheet.Cells[row, 6].Formula = $"SUM(F2:F{row - 1})";
                    worksheet.Cells[row, 6].Style.Font.Bold = true;

                    // Suma de monto total
                    worksheet.Cells[row, 8].Formula = $"SUM(H2:H{row - 1})";
                    worksheet.Cells[row, 8].Style.Font.Bold = true;
                    worksheet.Cells[row, 8].Style.Numberformat.Format = "#,##0.00";
                }

                // INFORMACIÓN DEL REPORTE
                int infoRow = row + 2;
                worksheet.Cells[infoRow, 1].Value = "Información del Reporte";
                worksheet.Cells[infoRow, 1].Style.Font.Bold = true;
                worksheet.Cells[infoRow, 1].Style.Font.Size = 11;

                worksheet.Cells[infoRow + 1, 1].Value = $"Total de compras: {datos.Count}";
                worksheet.Cells[infoRow + 2, 1].Value = $"Valor total: {datos.Sum(x => x.MontoTotal).ToString("C")}";
                worksheet.Cells[infoRow + 3, 1].Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}";
                worksheet.Cells[infoRow + 4, 1].Value = "Sistema de Gestión de Inventario";

                return package.GetAsByteArray();
            }
        }
    }
}