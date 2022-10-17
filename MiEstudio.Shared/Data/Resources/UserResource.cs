using System;

namespace MiEstudio.Shared.Data.Resources
{
    public class UserResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public UserType Type { get; set; }
    }
}