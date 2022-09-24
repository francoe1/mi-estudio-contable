namespace MiEstudio.Data.Models
{
    public class ClientModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string CUIT { get; set; }
        public string Note { get; set; }
        public ClientType Type { get; set; }
    }
}