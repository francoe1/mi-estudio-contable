using MiEstudio.Data.Models;
using Newtonsoft.Json;

namespace MiEstudio.Data.Resources
{
    public class ClientExpenseResource
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }

    public static class ClientExpenseResourceExtension
    {
        public static ClientExpenseResource ToResource(this ClientExpenseModel model)
        {
            return JsonConvert.DeserializeObject<ClientExpenseResource>(JsonConvert.SerializeObject(model));
        }
    }
}