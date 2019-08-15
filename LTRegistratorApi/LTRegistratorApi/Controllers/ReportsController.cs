using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    //[ApiController, Authorize]
    [ApiController]
    public class ReportsController : BaseApiController
    {
        private readonly IReportService _reportService;

        public ReportsController(IMapper mapper, IReportService reportService) : base(mapper)
        {
            _reportService = reportService;
        }

        [HttpGet("monthly/{date}")]
        public async Task<IActionResult> Test(DateTime date)
        {
            //var managerId = Convert.ToInt32(User.Identity.GetUserId());
            var managerId = 2;
            var report = await _reportService.GetMonthlyReportAsync(managerId, date);

            byte[] fileContents;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Часы");

                //Header
                var header = new List<string>()
                {
                    "№", "ФИО","Ставка","Норма часов", "Итого"
                };

                using (ExcelRange range = worksheet.Cells[1, 1, 1, header.Count])
                {
                    range.LoadFromArrays(new List<string[]>(new[] { header.ToArray() }));
                    AddColorForRangeCells(range, Color.FromArgb(204, 204, 255));
                    using (ExcelRange headerRange = worksheet.Cells[1, 3, 1, header.Count])
                    {
                        headerRange.Style.TextRotation = 90;
                    }
                }

                using (ExcelRange range = worksheet.Cells[1, header.Count + 1, 1, header.Count + report.Projects.Count])
                {
                    range.LoadFromArrays(new List<string[]>(new[] { report.Projects.Select(p => p.ProjectName).ToArray() }));
                    AddColorForRangeCells(range, Color.LightGreen);
                    range.Style.TextRotation = 90;
                    range.Style.Font.Bold = true;
                }

                using (ExcelRange range = worksheet.Cells[1, 1, 1, header.Count + report.Projects.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                //Header Two
                worksheet.Cells[2, 2].Value = "Итого";
                using (ExcelRange range = worksheet.Cells[2, header.Count, 2, header.Count + report.Projects.Count])
                {
                    for (int column = 1; column <= range.Columns; column++)
                    {
                        var cell = worksheet.Cells[range.Start.Row, column + header.Count - 1];
                        var startCell = cell.Start;
                        cell.Formula = $"SUM({worksheet.Cells[startCell.Row + 1, startCell.Column].Address}:{worksheet.Cells[startCell.Row + 1 + report.Users.Count, startCell.Column].Address})";
                    }

                    range.Style.Font.Bold = true;
                }
                AddColorForRangeCells(worksheet, 2, 1, 2, header.Count + report.Projects.Count, Color.FromArgb(153, 204, 255));

                //Data
                var userIndex = 1;
                foreach (var user in report.Users)
                {
                    using (ExcelRange range = worksheet.Cells[2 + userIndex, 1, 3 + userIndex, header.Count + report.Projects.Count])
                    {
                        var row = new List<string>
                        {
                            userIndex.ToString(), $"{user.FirstName} {user.Surname}", user.Rate.ToString(CultureInfo.InvariantCulture), user.NormHours.ToString(CultureInfo.InvariantCulture)
                        };
                        range.LoadFromArrays(new List<string[]>(new[] { row.ToArray() }));
                        var currentCell = worksheet.Cells[2 + userIndex, 5];
                        currentCell.Formula = $"SUM({worksheet.Cells[2 + userIndex, currentCell.Start.Column + 1].Address}:{worksheet.Cells[2 + userIndex, range.End.Column].Address})";
                        currentCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        currentCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(204, 204, 255));

                        using (ExcelRange headerProjectRange = worksheet.Cells[1, header.Count + 1, 1, header.Count + report.Projects.Count])
                        {
                            foreach (var project in user.Projects)
                            {
                                var projectCell = headerProjectRange.FirstOrDefault(h => (string)h.Value == project.ProjectName);
                                if (projectCell == null) throw new Exception();

                                worksheet.Cells[2 + userIndex, projectCell.Start.Column].Value = project.Hours;
                            }
                        }

                        userIndex++;
                    }
                }

                using (ExcelRange range = worksheet.Cells[3, header.Count + 1, 3 + report.Users.Count - 1, header.Count + report.Projects.Count])
                {
                    AddColorForRangeCells(range, Color.FromArgb(204, 255, 204));
                }

                using (ExcelRange range = worksheet.Cells[1, 1, report.Users.Count + 2, report.Projects.Count + 5])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Font.Size = 10;
                }

                worksheet.Cells.AutoFitColumns();

                fileContents = package.GetAsByteArray();
            }

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"monthly_report_{date.Year}_{date.Month}.xlsx");
        }

        private void AddColorForRangeCells(ExcelWorksheet worksheet, int startRow, int startColumn, int endRow, int endColumn, Color color)
        {
            using (var range = worksheet.Cells[startRow, startColumn, endRow, endColumn])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(color);
            }
        }

        private void AddColorForRangeCells(ExcelRange range, Color color)
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(color);
        }
    }
}
