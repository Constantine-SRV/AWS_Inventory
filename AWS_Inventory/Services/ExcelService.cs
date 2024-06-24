using Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Services
{
    public class ExcelService
    {
        public async Task SaveToExcelAsync<T>(List<T> data, string filePath) where T : class
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(typeof(T).Name);

            var properties = typeof(T).GetProperties().Where(p => p.Name != "Tags").ToArray();
            var headers = properties.Select(p => p.Name).ToList();

            var uniqueTags = new HashSet<string>();
            foreach (var item in data)
            {
                var tagsProperty = item.GetType().GetProperty("Tags");
                if (tagsProperty != null)
                {
                    var tags = (Dictionary<string, string>)tagsProperty.GetValue(item);
                    foreach (var tag in tags.Keys)
                    {
                        uniqueTags.Add(tag);
                    }
                }
            }

            headers.AddRange(uniqueTags.Select(tag => "tag_" + tag));

            for (var i = 0; i < headers.Count; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                for (var j = 0; j < properties.Length; j++)
                {
                    var value = properties[j].GetValue(item);
                    worksheet.Cells[i + 2, j + 1].Value = value is DateTime date ? date.ToString("yyyy-MM-dd HH:mm:ss") : value;
                }

                var columnIndex = properties.Length + 1;
                var tagsProperty = item.GetType().GetProperty("Tags");
                if (tagsProperty != null)
                {
                    var tags = (Dictionary<string, string>)tagsProperty.GetValue(item);
                    foreach (var tagKey in uniqueTags)
                    {
                        if (!tags.ContainsKey(tagKey))
                        {
                            worksheet.Cells[i + 2, columnIndex].Value = "Null";
                            //worksheet.Cells[i + 2, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            //worksheet.Cells[i + 2, columnIndex].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSalmon);
                            worksheet.Cells[i + 2, columnIndex].Style.Font.Color.SetColor(System.Drawing.Color.LightSalmon);
                        }
                        else
                        {
                            worksheet.Cells[i + 2, columnIndex].Value = tags[tagKey];
                        }

                        columnIndex++;
                    }
                }
            }

            worksheet.Cells.AutoFitColumns(0);
            worksheet.Cells[1, 1, 1, headers.Count].AutoFilter = true;

            var fileInfo = new FileInfo(filePath);
            await package.SaveAsAsync(fileInfo);
        }
    }
}
