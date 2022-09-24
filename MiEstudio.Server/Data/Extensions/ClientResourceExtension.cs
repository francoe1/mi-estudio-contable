using MiEstudio.Server.Data.Models;
using MiEstudio.Shared.Data.Resources;
using Newtonsoft.Json;

namespace MiEstudio.Server.Data.Resources
{
    public static class ClientResourceExtension
    {
        public static ClientResource ToResource(this ClientModel model)
        {
            return JsonConvert.DeserializeObject<ClientResource>(JsonConvert.SerializeObject(model));
        }
    }
}