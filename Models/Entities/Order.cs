namespace MyApiRestDapperOracle.Models.Entities
{
    public class Order
    {
        public int OrderId { get; set; } //PK
        public DateTime OrderTms { get; set; }
        public int CustomerId { get; set; } //FK
        public string OrderStatus { get; set; }
        public int StoreId { get; set; } //FK
    }
}