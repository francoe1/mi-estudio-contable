namespace MiEstudio.Server.Data.Models
{
    public class ClientExpenseModel
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }
}