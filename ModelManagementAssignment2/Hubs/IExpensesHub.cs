using ModelManagementAssignment2.Models;
using System.Threading.Tasks;

namespace ModelManagementAssignment2.Hubs
{
    public interface IExpensesHub
    {
        Task NewExpenseCreated(string newExpense);
    }
}
