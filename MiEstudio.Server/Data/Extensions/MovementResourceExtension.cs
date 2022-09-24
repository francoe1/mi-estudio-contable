using MiEstudio.Server.Data.Models;
using MiEstudio.Shared.Data.Resources;
using Newtonsoft.Json;

namespace MiEstudio.Server.Data.Resources
{
    public static class MovementResourceExtension
    {
        public static MovementResource ToResource(this MovementModel model)
        {
            return JsonConvert.DeserializeObject<MovementResource>(JsonConvert.SerializeObject(model));
        }
    }
}