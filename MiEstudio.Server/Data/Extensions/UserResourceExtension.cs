using MiEstudio.Server.Data.Models;
using MiEstudio.Shared.Data.Resources;
using Newtonsoft.Json;

namespace MiEstudio.Server.Data.Resources
{
    public static class UserResourceExtension
    {
        public static UserResource ToResource(this UserModel model)
        {
            return JsonConvert.DeserializeObject<UserResource>(JsonConvert.SerializeObject(model));
        }
    }
}