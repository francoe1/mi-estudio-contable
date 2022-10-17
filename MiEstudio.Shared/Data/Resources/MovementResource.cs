using System;

namespace MiEstudio.Shared.Data.Resources
{
    public class MovementResource
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public DateTime ExpireDate { get; set; } = DateTime.Now.AddDays(30);
        public MovementType Type { get; set; }
        public string Concept { get; set; }
        public decimal Value { get; set; }
        public decimal Balance { get; set; }
    }
}