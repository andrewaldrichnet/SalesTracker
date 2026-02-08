namespace SalesTracker.Shared.Models;

public class ItemImage
{
    public int ImageID { get; set; }
    public int ItemID { get; set; }
    public required string ImagePath { get; set; }
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}
