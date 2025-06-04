using FinacialProjectVersion3.Models.Entity;
using FinacialProjectVersion3.Utils;
using FinacialProjectVersion3.ViewModels.Transaction;

namespace FinacialProjectVersion3.Services
{
    public interface ITransactionService
    {
        Task<ServiceResult<Transaction>> CreateTransaction(TransactionCreateViewModel model, int userId);
        Task<ServiceResult<Transaction>> GetTransactionById(int id, int userId);
        Task<ServiceResult> UpdateTransaction(TransactionEditViewModel model, int userId);
        Task<ServiceResult> DeleteTransaction(int id, int userId);
        Task<ServiceResult<TransactionListViewModel>> GetTransactions(TransactionFilterViewModel filter, int userId);
        Task<ServiceResult<List<Transaction>>> GetRecentTransactions(int userId, int count = 5);
        Task<ServiceResult<decimal>> GetBalance(int userId, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
