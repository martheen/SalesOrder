using System;

namespace SalesOrder.Models;

public class OrderExportModel
{
    public int No { get; set; }
    public string Order_Number { get; set; }
    public DateTime Order_Date { get; set; }
    public string Customer_Name { get; set; } = string.Empty;
    public Double Total_Price { get; set; }
    public int Total_Item { get; set; }
}
