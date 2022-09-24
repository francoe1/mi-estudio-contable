namespace MiEstudio.Shared.Data.Resources
{
    public class ClientExpenseResource
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }
}