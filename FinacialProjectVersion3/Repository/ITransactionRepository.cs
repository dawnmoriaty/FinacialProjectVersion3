using FinacialProjectVersion3.Models.Entity;
using System.Threading.Tasks;

namespace FinacialProjectVersion3.Repository
{
    public interface ITransactionRepository
    {
        Task<int> Create(Transaction transaction);
        Task<Transaction?> GetByIdAndUserId(int id, int userId);
        Task<bool> Update(Transaction transaction);
        Task<bool> Delete(int id, int userId);
        Task<List<Transaction>> GetByUserId(int userId, int page = 1, int pageSize = 10);
        Task<List<Transaction>> GetByUserIdWithFilter(int userId, string? type = null, int? categoryId = null,
            DateTime? fromDate = null, DateTime? toDate = null, string? searchTerm = null,
            int page = 1, int pageSize = 10);
        Task<int> GetTotalCountByUserId(int userId);
        Task<int> GetTotalCountWithFilter(int userId, string? type = null, int? categoryId = null,
            DateTime? fromDate = null, DateTime? toDate = null, string? searchTerm = null);
        Task<decimal> GetTotalIncomeByUserId(int userId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<decimal> GetTotalExpenseByUserId(int userId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<Transaction>> GetRecentTransactions(int userId, int count = 5);
        Task<bool> UpdateByFields(int id, string description, decimal amount, int categoryId, DateTime transactionDate, int userId);  
    }
    
   }


