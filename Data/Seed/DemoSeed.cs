using MiEstudio.Data.Contexts;
using MiEstudio.Data.Models;

namespace MiEstudio.Data.Seed
{
    public class DemoSeed
    {
        private readonly DataContext _context;

        public DemoSeed(DataContext context)
        {
            _context = context;
            _context.Clients.AddRange(ClientSeed().Take(500));
            _context.SaveChanges();
        }

        private IEnumerable<ClientModel> ClientSeed()
        {
            while (true)
            {
                yield return new ClientModel
                {
                    Id = Guid.NewGuid(),
                    Name = Faker.Name.FullName(),
                    Note = Faker.Lorem.Paragraph(),
                    Phone = Faker.Phone.Number(),
                    Type = Faker.Enum.Random<ClientType>(),
                    Address = Faker.Address.StreetAddress(),
                    City = Faker.Address.City(),
                    CUIT = Faker.Identification.UkNationalInsuranceNumber(),
                };
            }
        }
    }
}