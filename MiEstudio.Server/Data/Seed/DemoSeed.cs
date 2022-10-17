using MiEstudio.Server.Data.Contexts;
using MiEstudio.Server.Data.Models;
using MiEstudio.Shared.Data.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiEstudio.Server.Data.Seed
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