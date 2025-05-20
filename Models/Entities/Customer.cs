namespace MyApiRestDapperOracle.Models.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }//PK
        public string EmailAdress { get; set; }

        public string FullName { get; set; }
    }
}