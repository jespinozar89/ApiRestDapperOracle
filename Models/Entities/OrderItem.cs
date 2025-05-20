namespace MyApiRestDapperOracle.Models.Entities
{
    public class OrderItem
    {
        public int OrderId { get; set; } //FK
        public int LineItemId { get; set; } //PK
        public int ProductId { get; set; } //FK
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int? ShipmentId { get; set; } //FK
    }
}