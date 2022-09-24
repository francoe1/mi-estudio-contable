using MiEstudio.Server.Data.Models;
using MiEstudio.Shared.Data.Resources;
using Newtonsoft.Json;

namespace MiEstudio.Server.Data.Extensions
{
    public static class ClientExpenseResourceExtension
    {
        public static ClientExpenseResource ToResource(this ClientExpenseModel model)
        {
            return JsonConvert.DeserializeObject<ClientExpenseResource>(JsonConvert.SerializeObject(model));
        }
    }
}