using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiEstudio.Server.Data.Contexts;
using MiEstudio.Server.Data.Models;
using MiEstudio.Server.Data.Queries;
using MiEstudio.Shared.Data.Filters;
using MiEstudio.Shared.Data.Resources;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MiEstudio.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : Controller
    {
        private readonly DataContext _context;

        public ClientController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ClientFilter filter)
        {
            ClientQuery query = new ClientQuery(GetQueryContext(_context, filter));
            return Ok(await query.ToResourceList());
        }

        [HttpGet("{clientId}")]
        public async Task<IActionResult> Index([FromRoute] Guid clientId)
        {
            ClientQuery query = new ClientQuery(GetQueryContext(_context));
            query.Where(x => x.ClientModel.Id == clientId);
            return Ok(await query.FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> Store([FromBody] ClientStoreInput input)
        {
            ClientModel client = new ClientModel
            {
                Id = Guid.NewGuid(),
                Name = input.Name,
                Address = input.Address,
                City = input.City,
                Phone = input.Phone,
                CUIT = input.CUIT,
                Note = input.Note,
                Type = input.Type
            };

            client = _context.Clients.Add(client).Entity;
            await _context.SaveChangesAsync();

            ClientQuery query = new ClientQuery(GetQueryContext(_context));
            query.Where(x => x.ClientModel.Id == client.Id);
            return Ok(await query.FirstOrDefault());
        }

        [HttpPut("{clientId}")]
        public async Task<IActionResult> Update([FromRoute] Guid clientId, [FromBody] ClientStoreInput input)
        {
            ClientModel client = await (from c in _context.Clients where c.Id == clientId select c).FirstOrDefaultAsync();
            if (client == null) return BadRequest($"Client [{clientId}] not exist");

            client.Name = input.Name;
            client.Address = input.Address;
            client.City = input.City;
            client.Phone = input.Phone;
            client.CUIT = input.CUIT;
            client.Note = input.Note;
            client.Type = input.Type;

            client = _context.Clients.Update(client).Entity;
            await _context.SaveChangesAsync();

            ClientQuery query = new ClientQuery(GetQueryContext(_context));
            query.Where(x => x.ClientModel.Id == clientId);
            return Ok(await query.FirstOrDefault());
        }

        [HttpDelete("{clientId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid clientId)
        {
            ClientModel client = await (from c in _context.Clients where c.Id == clientId select c).FirstOrDefaultAsync();
            if (client == null) return BadRequest($"Client [{clientId}] not exist");

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("ApplyExpense")]
        public async Task<IActionResult> ApplyExpense()
        {
            ClientQuery query = new ClientQuery(GetQueryContext(_context, new ClientFilter
            {
                PerPage = int.MaxValue
            }));

            query.Where(x => x.ClientExpenseModel != null && x.ClientExpenseModel.Value > 0);

            ClientMovementController clientMovement = new ClientMovementController(_context);

            Task<IActionResult>[] tasks = (await query.ToList()).Select(resource => clientMovement.Store(resource.Id, new ClientMovementController.MovementStoreInput
            {
                ExpireDate = DateTime.Now,
                Type = MovementType.Credito,
                Value = resource.Expense,
                Concept = $"Abono automatico {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month)}",
            })).ToArray();

            IActionResult[] results = await Task.WhenAll(tasks);

            return Ok(new
            {
                RequireUpdate = tasks.Length,
                Update = results.Count(x => x is OkObjectResult)
            });
        }

        public struct ClientStoreInput
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Phone { get; set; }
            public string CUIT { get; set; }
            public string Note { get; set; }
            public ClientType Type { get; set; }
        }
    }
}