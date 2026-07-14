namespace Fgc.Catalog.Application.DTOS.Purchases
{
    public class PurchaseResponse
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal Price { get; set; }
    }
}