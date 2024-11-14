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
                        OrderNumber = o.OrderNo,
                        OrderDate = o.OrderDate,
                        CustomerName = o.Customer.CustomerName ?? string.Empty,
                        TotalPrice = o.Items.Sum(i=>i.Quantity * i.Price),
                        TotalItem = o.Items.Sum(i=>i.Quantity)
                    })
                    .ToListAsync();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sales Orders");

                    
                    worksheet.Cell(1, 1).Value = "No";
                    worksheet.Cell(1, 2).Value = "Sales Order";
                    worksheet.Cell(1, 3).Value = "Order Date";
                    worksheet.Cell(1, 4).Value = "Customer";
                    worksheet.Cell(1, 5).Value = "Total Price";
                    worksheet.Cell(1, 6).Value = "Total Item";

                    var rangeHeader = worksheet.Range(1, 1, 1, 6);

                    rangeHeader.Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                    rangeHeader.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    
                    int row = 2;
                    foreach (var order in orders)
                    {
                        worksheet.Cell(row, 1).Value = row - 1;
                        worksheet.Cell(row, 2).Value = order.OrderNumber;
                        worksheet.Cell(row, 3).Value = order.OrderDate;
                        worksheet.Cell(row, 4).Value = order.CustomerName;
                        worksheet.Cell(row, 5).Value = order.TotalPrice;
                        worksheet.Cell(row, 6).Value = order.TotalItem;
                        if (row%2==0)
                        {
                            worksheet.Range(row, 1, row, 6).Style.Fill.SetBackgroundColor(XLColor.AshGrey);
                        }
                        
                        row++;
                    }
                    row--;
                    worksheet.Range(2, 3, row, 3).Style.NumberFormat.Format = "dd/M/yyyy";
                    var totalPriceRange = worksheet.Range(2, 5, row, 5);
                    totalPriceRange.Style.NumberFormat.Format = "_-* #.##0,00_-;-* #.##0,00_-;_-* \"-\"??_-;_-@_-";
                    totalPriceRange.AddConditionalFormat().DataBar(XLColor.LightSkyBlue);
                    worksheet.Range(2, 6, row, 6).AddConditionalFormat().DataBar(XLColor.CadetBlue);
                    var rangeRows = worksheet.Range(2, 1, row, 6);
                    rangeRows.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    rangeRows.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    worksheet.Range(1, 1, row, 6).SetAutoFilter();
                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"SalesOrders_Export_{DateTime.Now:yyyyMMddHHmmss}" +
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
