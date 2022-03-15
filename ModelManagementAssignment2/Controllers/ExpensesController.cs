#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ModelManagementAssignment2.Data;
using ModelManagementAssignment2.Hubs;
using ModelManagementAssignment2.Models;

namespace ModelManagementAssignment2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly ModelManagementDb _context;
        private readonly IHubContext<ExpenseHub,IExpensesHub> _hubContext;

        public ExpensesController(ModelManagementDb context, IHubContext<ExpenseHub, IExpensesHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Expenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
            return await _context.Expenses.ToListAsync();
        }

        // GET: api/Expenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(long id)
        {
            var expense = await _context.Expenses.Where(x => x.ExpenseId == id).SingleOrDefaultAsync();

            if (expense == null)
            {
                return NotFound();
            }

            return expense;
        }

        // PUT: api/Expenses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpense(long id, Expense expense)
        {
            if (id != expense.ExpenseId)
            {
                return BadRequest();
            }

            _context.Entry(expense).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Expenses
        //Krav: Oprette en ny expense. Bemærk at en expense både er tilknyttet en model og et job.
        [HttpPost("{modelid}/{jobid}")]
        public async Task<ActionResult<Expense>> PostExpense(Expense expense, long modelid, long jobid)
        {
            var model = await _context.Models.Where(m => m.ModelId == modelid).Include(e => e.Expenses).SingleOrDefaultAsync();
            var job = await _context.Jobs.Where(j => j.JobId == jobid).Include(e => e.Expenses).Include(m => m.Models).SingleOrDefaultAsync();

            if (job.Models.Contains(model))
            { 
                _context.Expenses.Add(expense);

                _context.Entry(expense).State = EntityState.Modified;
                _context.Entry(model).State = EntityState.Modified;
                _context.Entry(job).State = EntityState.Modified;

                await _context.SaveChangesAsync();


                await _hubContext.Clients.All.NewExpenseCreated(expense.Text);
            }
            else
            {
                BadRequest();
            }

            return CreatedAtAction("GetExpense", new { id = expense.ExpenseId }, expense);
        }

        // DELETE: api/Expenses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(long id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExpenseExists(long id)
        {
            return _context.Expenses.Any(e => e.ExpenseId == id);
        }
    }
}
