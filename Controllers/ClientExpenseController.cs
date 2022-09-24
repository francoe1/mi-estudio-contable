﻿using Microsoft.AspNetCore.Mvc;
using MiEstudio.Data.Contexts;
using MiEstudio.Data.Models;
using MiEstudio.Data.Resources;

namespace MiEstudio.Controllers
{
    [Route("api/Client/{clientId}/Expense/")]
    public class ClientExpenseController : Controller
    {
        private readonly DataContext _context;

        public ClientExpenseController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Store([FromRoute] Guid clientId, [FromBody] ExpenseStoreInput input)
        {
            if (input.Value < decimal.Zero) return BadRequest("Can not negative value");

            ClientExpenseModel expense = new ClientExpenseModel
            {
                Id = Guid.NewGuid(),
                ClientId = clientId,
                Date = DateTime.Now,
                Value = input.Value,
            };

            expense = _context.ClientsExpense.Add(expense).Entity;
            await _context.SaveChangesAsync();

            return Ok(expense.ToResource());
        }

        public record ExpenseStoreInput
        {
            public decimal Value { get; set; }
        }
    }
}