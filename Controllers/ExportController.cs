using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesOrder.Data;
using SalesOrder.Models;


namespace SalesOrder.Controllers
{
    public class ExportController : Controller
    {
        private readonly AppDbContext _context;

        public ExportController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ExportToExcel(string keyword, DateTime? orderDate)
        {
            try
            {
                var query = _context.SoOrders
                    .Include(o => o.Customer)   
                    .AsQueryable();

                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(o => o.OrderNo.Contains(keyword) || o.Customer.CustomerName.Contains(keyword));
                }

                if (orderDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate.Date == orderDate.Value.Date);
                }

                //var totalItems = await query.CountAsync();
                //var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                //page = Math.Min(Math.Max(1, page), totalPages);

                var orders = await query
                    //.Skip((page - 1) * pageSize)
                    //.Take(pageSize)
                    .Select(o => new OrderExportModel
                    {  
                        Order_Number = o.OrderNo,
                        Order_Date = o.OrderDate,
                        Customer_Name = o.Customer.CustomerName ?? string.Empty,
                        Total_Price = o.Items.Sum(i=>i.Quantity * i.Price),
                        Total_Item = o.Items.Sum(i=>i.Quantity)
                    })
                    .ToListAsync();

                var generationTime = DateTime.Now;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sales Orders");
                    
                    var lastTitleRow = 1;
                    worksheet.Range(lastTitleRow, 1, lastTitleRow, 6).Merge()
                        .SetValue("Sales Orders")
                        .Style.Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.CenterContinuous);
                    lastTitleRow++;
                    worksheet.Range(lastTitleRow, 1, lastTitleRow, 6).Merge()
                        .SetValue($"Generated at {generationTime:dd/M/yyyy HH:mm}")
                        .Style.Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    if (orderDate.HasValue)
                    {
                        lastTitleRow=3;
                        worksheet.Range(lastTitleRow, 4, lastTitleRow, 6).Merge()
                        .SetValue($"Date Filter: {orderDate:dd/M/yyyy}")
                        .Style.Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    }

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        lastTitleRow=3;
                        worksheet.Range(lastTitleRow, 1, lastTitleRow, 3).Merge()
                        .SetValue($"Keyword: {keyword}")
                        .Style.Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    }

                    var headerRow = lastTitleRow + 2;

                    var table = worksheet.Cell(headerRow, 1).InsertTable(
                        orders
                        , "SalesOrder");

                    table.ShowTotalsRow = true;
                    table.Field(nameof(OrderExportModel.Customer_Name)).TotalsRowLabel = "Total";

                    var fieldTotalItem = table.Field(nameof(OrderExportModel.Total_Item));

                    fieldTotalItem.TotalsRowFunction = XLTotalsRowFunction.Sum;

                    var fieldTotalPrice = table.Field(nameof(OrderExportModel.Total_Price));
                    fieldTotalPrice.TotalsRowFunction = XLTotalsRowFunction.Sum;
                    int rowNumber = 1;
                    foreach (var item in table.Field(nameof(OrderExportModel.No)).DataCells)
                    {
                        item.Value = rowNumber++;
                    }

                    table.Field(nameof(OrderExportModel.Order_Date)).DataCells.Style.DateFormat.Format = "dd/M/yyyy";
                    fieldTotalPrice.Column.Style.NumberFormat.Format = "#,###";
                    
                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"SalesOrders_Export_{generationTime:yyyyMMddHHmmss}" +
                            (string.IsNullOrEmpty(keyword)?"":"_"+keyword) +
                            (orderDate.HasValue?"_"+orderDate.Value.ToString("yyyyMMdd"):"")+
                            $".xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "An error occurred while exporting to Excel.");
            }
        }
    }
}
