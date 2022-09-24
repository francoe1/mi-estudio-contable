using MiEstudio.Shared.Data.Resources;

namespace MiEstudio.Server.Data.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public UserType Type { get; set; }
    }
}