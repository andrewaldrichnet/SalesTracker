namespace SalesTracker.Shared.Models;

public class Order
{
    public int OrderID { get; set; }
    public string CustomerName { get; set; }
    public int ItemID { get; set; }
    public DateTime SellDate { get; set; }
    public decimal Price { get; set; }
    public int Qty { get; set; }
    public bool HasReceivedPayment { get; set; }
    public bool HasDelivered { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
