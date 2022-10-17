using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiEstudio.Server.Data.Contexts;
using MiEstudio.Server.Data.Models;
using MiEstudio.Server.Data.Queries;
using MiEstudio.Server.Data.Resources;
using MiEstudio.Shared.Data.Filters;
using MiEstudio.Shared.Data.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MiEstudio.Server.Controllers
{
    [Route("api/Client/{clientId}/Movement/")]
    public class ClientMovementController : Controller
    {
        private readonly DataContext _context;

        public ClientMovementController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] Guid clientId, MovementFilter filter)
        {
            MovementQuery query = new MovementQuery(GetQueryContext(_context, filter));
            query.Where(x => x.MovementModel.ClientId == clientId);
            return Ok(await query.ToResourceList());
        }

        [HttpPost]
        public async Task<IActionResult> Store([FromRoute] Guid clientId, [FromBody] MovementStoreInput input)
        {
            if (input.Value <= decimal.Zero) return BadRequest("Can not negative value");
            if (string.IsNullOrEmpty(input.Concept)) return BadRequest("The concept is required");

            decimal balance = await _context.Movements.Where(m => m.ClientId == clientId)
                .OrderByDescending(m => m.Date)
                .Select(m => m.Balance)
                .FirstOrDefaultAsync();

            switch (input.Type)
            {
                case MovementType.Debito:
                    balance += input.Value;
                    break;

                case MovementType.Credito:
                    balance -= input.Value;
                    break;
            }

            MovementModel movement = new MovementModel
            {
                Id = Guid.NewGuid(),
                ClientId = clientId,
                Value = input.Value,
                Date = DateTime.Now,
                ExpireDate = input.ExpireDate,
                Type = input.Type,
                Balance = balance,
                Concept = input.Concept,
            };

            movement = _context.Movements.Add(movement).Entity;
            await _context.SaveChangesAsync();

            return Ok(movement.ToResource());
        }

        public class MovementStoreInput
        {
            public DateTime ExpireDate { get; set; } = DateTime.Now.AddDays(30);
            public MovementType Type { get; set; }
            public decimal Value { get; set; }
            public string Concept { get; set; }
        }
    }
}