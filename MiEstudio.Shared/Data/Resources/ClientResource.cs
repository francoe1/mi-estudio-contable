using System;

namespace MiEstudio.Shared.Data.Resources
{
    public class ClientResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string CUIT { get; set; }
        public string Note { get; set; }
        public ClientType Type { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance { get; set; }
    }
}