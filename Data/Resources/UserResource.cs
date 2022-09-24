using MiEstudio.Data.Models;
using Newtonsoft.Json;

namespace MiEstudio.Data.Resources
{
    public class UserResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public UserType Type { get; set; }
    }

    public static class UserResourceExtension
    {
        public static UserResource ToResource(this UserModel model)
        {
            return JsonConvert.DeserializeObject<UserResource>(JsonConvert.SerializeObject(model));
        }
    }
}