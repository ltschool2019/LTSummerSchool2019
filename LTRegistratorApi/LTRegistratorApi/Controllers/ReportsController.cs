using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : BaseApiController
    {
        private readonly IReportService _reportService;

        public ReportsController(IMapper mapper, IReportService reportService) : base(mapper)
        {
            _reportService = reportService;
        }

        [HttpGet("{managerId}/test")]
        public async Task<IActionResult> Test(int managerId)
        {
            var report = await _reportService.GetHourReportAsync(managerId);


            byte[] fileContents;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Часы");
                //Header
                worksheet.Cells[1, 1].Value = "№";
                worksheet.Cells[1, 2].Value = "ФИО";
                worksheet.Cells[1, 3].Value = "Ставка";
                worksheet.Cells[1, 4].Value = "Норма часов";
                worksheet.Cells[1, 5].Value = "Итого";
                AddColorForRangeCells(worksheet, 1, 1, 1, 5, Color.FromArgb(204, 204, 255));

                var i = 6;
                foreach (var project in report.Projects)
                {
                    worksheet.Cells[1, i].Value = project.ProjectName;
                    i++;
                }
                AddColorForRangeCells(worksheet, 1, 6, 1, i - 1, Color.Green);

                //Total
                worksheet.Cells[2, 2].Value = "Итого";
                for (i = 5; i <= 5 + report.Projects.Count; i++)
                {
                    worksheet.Cells[2, i].Formula = $"SUM({worksheet.Cells[3, i].Address}:{worksheet.Cells[3 + report.Users.Count - 1, i].Address})";
                }
                AddColorForRangeCells(worksheet, 2, 1, 2, 5 + report.Projects.Count, Color.FromArgb(153, 204, 255));
                //Data1
                int row = 3;
                i = 1;
                foreach (var user in report.Users)
                {
                    worksheet.Cells[row, 1].Value = i;
                    worksheet.Cells[row, 2].Value = $"{user.FirstName} {user.Surname} {user.LastName}";
                    worksheet.Cells[row, 3].Value = user.Rate;
                    worksheet.Cells[row, 4].Value = user.NormHours;
                    worksheet.Cells[row, 5].Formula = $"SUM({worksheet.Cells[row, 6].Address}:{worksheet.Cells[row, 5 + report.Projects.Count].Address})";
                    worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(204, 204, 255));

                    foreach (var project in user.Projects)
                    {
                        for (int j = 6; j < report.Projects.Count + 6; j++)
                        {
                            if ((string)worksheet.Cells[1, j].Value == project.ProjectName)
                            {
                                worksheet.Cells[row, j].Value = project.Hours;
                                break;
                            }
                        }
                    }

                    row++;
                    i++;
                }
                worksheet.Cells[1, 1, report.Users.Count + 2, report.Projects.Count + 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1, report.Users.Count + 2, report.Projects.Count + 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1, report.Users.Count + 2, report.Projects.Count + 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1, report.Users.Count + 2, report.Projects.Count + 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                AddColorForRangeCells(worksheet, 3, 6, 3 + report.Users.Count - 1, 6 + report.Projects.Count - 1, Color.FromArgb(204, 255, 204));
                fileContents = package.GetAsByteArray();
            }

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "test.xlsx"
            );
        }

        private void AddColorForRangeCells(ExcelWorksheet worksheet, int startRow, int startColumn, int endRow, int endColumn, Color color)
        {
            using (var range = worksheet.Cells[startRow, startColumn, endRow, endColumn])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(color);
            }
        }


    }
}
