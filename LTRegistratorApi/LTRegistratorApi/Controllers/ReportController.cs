using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize(Policy = "IsAdminOrCurrentManager")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("monthly/{date}/manager/{managerId}")]
        public async Task<IActionResult> GetMonthlyReport(int managerId, DateTime date)
        {
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
                    range.Style.Font.Bold = true;
                    using (ExcelRange headerRange = worksheet.Cells[1, 3, 1, header.Count])
                    {
                        headerRange.Style.TextRotation = 90;
                    }
                }

                for (int i = 0; i < report.Events.Count(); i++)
                {
                    var item = report.Events.ElementAt(i);
                    var cell = worksheet.Cells[1, header.Count + 1 + i];
                    cell.Value = item.Name;
                    cell.Style.TextRotation = 90;
                    cell.Style.Font.Bold = true;
                    Color color;
                    switch (item.EventType)
                    {
                        case EventType.ManagerProject:
                            color = Color.LightGreen;
                            break;
                        case EventType.NotManagerProject:
                            color = Color.FromArgb(255, 255, 204);
                            break;
                        case EventType.Vacation:
                            color = Color.FromArgb(255, 204, 0);
                            break;
                        case EventType.AnotherLeave:
                            color = Color.FromArgb(204, 153, 255);
                            break;
                        default:
                            color = Color.White;
                            break;
                    }
                    AddColorForRangeCells(cell, color);
                }

                //Header Two
                worksheet.Cells[2, 2].Value = "Итого";
                worksheet.Cells[2, 2].Style.Font.Bold = true;
                using (ExcelRange range = worksheet.Cells[2, header.Count, 2, header.Count + report.Events.Count()])
                {
                    for (int column = 1; column <= range.Columns; column++)
                    {
                        var cell = worksheet.Cells[range.Start.Row, column + header.Count - 1];
                        var startCell = cell.Start;
                        cell.Formula = $"SUM({worksheet.Cells[startCell.Row + 1, startCell.Column].Address}:{worksheet.Cells[startCell.Row + report.Users.Count(), startCell.Column].Address})";
                    }

                    range.Style.Font.Bold = true;
                }
                AddColorForRangeCells(worksheet, 2, 1, 2, header.Count + report.Events.Count(), Color.FromArgb(153, 204, 255));

                //Data
                var userIndex = 1;
                foreach (var user in report.Users)
                {
                    using (ExcelRange range = worksheet.Cells[2 + userIndex, 1, 3 + userIndex, header.Count + report.Events.Count()])
                    {
                        var row = new List<string>
                        {
                            userIndex.ToString(), $"{user.FirstName} {user.Surname}", user.Rate.ToString(CultureInfo.InvariantCulture), user.NormHours.ToString(CultureInfo.InvariantCulture)
                        };
                        range.LoadFromArrays(new List<string[]>(new[] { row.ToArray() }));
                        var currentCell = worksheet.Cells[2 + userIndex, 5];
                        currentCell.Formula = $"SUM({worksheet.Cells[2 + userIndex, currentCell.Start.Column + 1].Address}:{worksheet.Cells[1 + userIndex, range.End.Column].Address})";
                        currentCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        currentCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(204, 204, 255));

                        using (ExcelRange headerProjectRange = worksheet.Cells[1, header.Count + 1, 1, header.Count + report.Events.Count()])
                        {
                            foreach (var project in user.Projects)
                            {
                                var eventCell = headerProjectRange.FirstOrDefault(h => (string)h.Value == project.ProjectName);
                                if (eventCell == null) throw new Exception();

                                if (project.Hours > 0)
                                    worksheet.Cells[2 + userIndex, eventCell.Start.Column].Value = project.Hours;
                            }
                        }

                        using (ExcelRange headerProjectRange = worksheet.Cells[1, header.Count + 1, 1, header.Count + report.Events.Count()])
                        {
                            foreach (var leave in user.Leaves)
                            {
                                var eventCell = headerProjectRange.FirstOrDefault(h => (string)h.Value == leave.Name);
                                if (eventCell == null) throw new Exception();

                                worksheet.Cells[2 + userIndex, eventCell.Start.Column].Value = leave.Hours;
                            }
                        }

                        userIndex++;
                    }
                }

                using (ExcelRange range = worksheet.Cells[3, header.Count + 1, 3 + report.Users.Count() - 1, header.Count + report.Events.Count()])
                {
                    AddColorForRangeCells(range, Color.FromArgb(204, 255, 204));
                }

                using (ExcelRange range = worksheet.Cells[1, 1, report.Users.Count() + 2, report.Events.Count() + 5])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Font.Size = 10;
                }

                //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

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
