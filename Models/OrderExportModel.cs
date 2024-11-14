using System;

namespace SalesOrder.Models;

public class OrderExportModel
{
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Double TotalPrice { get; set; }
    public int TotalItem { get; set; }
}
