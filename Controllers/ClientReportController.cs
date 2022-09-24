using Microsoft.AspNetCore.Mvc;
using MiEstudio.Data.Contexts;
using MiEstudio.Data.Core;
using MiEstudio.Data.Queries;
using MiEstudio.Data.Resources;
using MiEstudio.Reports;

namespace MiEstudio.Controllers
{
    [Route("api/Client/")]
    [ApiController]
    public class ClientReportController : Controller
    {
        private readonly DataContext _context;

        public ClientReportController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("Report")]
        public async Task<IActionResult> Client()
        {
            ClientQuery query = new ClientQuery(GetQueryContext(_context, new Paginate
            {
                PerPage = _context.Clients.Count()
            }));

            query.Where(x => x.LastMovementModel.Balance < 0);

            FileCSV csv = new FileCSV();
            csv.AddHeader("Nombre", "CUIT", "Tipo", "Abono", "Balance");

            foreach (ClientResource client in (await query.ToResourceList()).Data)
            {
                csv.AddRow(client.Name, client.CUIT, client.Type, client.Expense, client.Balance);
            }

            return File(csv.ToBytes(), "text/csv", "client-report.csv", true);
        }

        [HttpGet("{clientId}/Report/Movement")]
        public async Task<IActionResult> Movements([FromRoute] Guid clientId)
        {
            MovementQuery query = new MovementQuery(GetQueryContext(_context, new Paginate
            {
                PerPage = 1000
            }));
            query.Where(x => x.MovementModel.ClientId == clientId);

            FileCSV csv = new FileCSV();
            csv.AddHeader("Fecha", "Concepto", "Tipo", "Valor", "Balance");

            foreach (MovementResource movement in (await query.ToResourceList()).Data)
            {
                csv.AddRow(movement.Date, movement.Concept, movement.Type, movement.Value, movement.Balance);
            }

            return File(csv.ToBytes(), "text/csv", "movement-report.csv");
        }
    }
}