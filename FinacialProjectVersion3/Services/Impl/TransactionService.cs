using FinacialProjectVersion3.Models.Entity;
using FinacialProjectVersion3.Repository;
using FinacialProjectVersion3.Utils;
using FinacialProjectVersion3.ViewModels.Transaction;

namespace FinacialProjectVersion3.Services.Impl
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TransactionService(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
        {
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResult<Transaction>> CreateTransaction(TransactionCreateViewModel model, int userId)
        {
            try
            {
                // Validate category belongs to user
                var category = await _categoryRepository.GetByIdAndUserId(model.CategoryId, userId);
                if (category == null)
                {
                    return ServiceResult<Transaction>.Failed("Danh mục không tồn tại hoặc không thuộc về bạn");
                }

                var transaction = new Transaction
                {
                    Description = model.Description,
                    Amount = model.Amount,
                    CategoryId = model.CategoryId,
                    TransactionDate = model.TransactionDate,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var transactionId = await _transactionRepository.Create(transaction);
                transaction.Id = transactionId;

                return ServiceResult<Transaction>.Succeeded(transaction, "Tạo giao dịch thành công!");
            }
            catch (Exception ex)
            {
                return ServiceResult<Transaction>.Failed($"Lỗi khi tạo giao dịch: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Transaction>> GetTransactionById(int id, int userId)
        {
            try
            {
                var transaction = await _transactionRepository.GetByIdAndUserId(id, userId);
                if (transaction == null)
                {
                    return ServiceResult<Transaction>.Failed("Giao dịch không tồn tại");
                }

                return ServiceResult<Transaction>.Succeeded(transaction);
            }
            catch (Exception ex)
            {
                return ServiceResult<Transaction>.Failed($"Lỗi khi lấy thông tin giao dịch: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateTransaction(TransactionEditViewModel model, int userId)
        {
            try
            {
                var transaction = await _transactionRepository.GetByIdAndUserId(model.Id, userId);
                if (transaction == null)
                {
                    return ServiceResult.Failed("Giao dịch không tồn tại");
                }

                // Validate category belongs to user
                var category = await _categoryRepository.GetByIdAndUserId(model.CategoryId, userId);
                if (category == null)
                {
                    return ServiceResult.Failed("Danh mục không tồn tại hoặc không thuộc về bạn");
                }

                transaction.Description = model.Description;
                transaction.Amount = model.Amount;
                transaction.CategoryId = model.CategoryId;
                transaction.TransactionDate = model.TransactionDate;
                transaction.UpdatedAt = DateTime.Now;

                var result = await _transactionRepository.Update(transaction);
                if (result)
                {
                    return ServiceResult.Succeeded("Cập nhật giao dịch thành công!");
                }

                return ServiceResult.Failed("Không thể cập nhật giao dịch");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failed($"Lỗi khi cập nhật giao dịch: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteTransaction(int id, int userId)
        {
            try
            {
                var result = await _transactionRepository.Delete(id, userId);
                if (result)
                {
                    return ServiceResult.Succeeded("Xóa giao dịch thành công!");
                }

                return ServiceResult.Failed("Giao dịch không tồn tại hoặc không thể xóa");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failed($"Lỗi khi xóa giao dịch: {ex.Message}");
            }
        }

        public async Task<ServiceResult<TransactionListViewModel>> GetTransactions(TransactionFilterViewModel filter, int userId)
        {
            try
            {
                var transactions = await _transactionRepository.GetByUserIdWithFilter(
                    userId, filter.Type, filter.CategoryId, filter.FromDate, filter.ToDate,
                    filter.SearchTerm, filter.Page, filter.PageSize);

                var totalItems = await _transactionRepository.GetTotalCountWithFilter(
                    userId, filter.Type, filter.CategoryId, filter.FromDate, filter.ToDate, filter.SearchTerm);

                var totalIncome = await _transactionRepository.GetTotalIncomeByUserId(userId, filter.FromDate, filter.ToDate);
                var totalExpense = await _transactionRepository.GetTotalExpenseByUserId(userId, filter.FromDate, filter.ToDate);

                var result = new TransactionListViewModel
                {
                    Transactions = transactions,
                    Filter = filter,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling((double)totalItems / filter.PageSize),
                    CurrentPage = filter.Page,
                    PageSize = filter.PageSize,
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    Balance = totalIncome - totalExpense
                };

                return ServiceResult<TransactionListViewModel>.Succeeded(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<TransactionListViewModel>.Failed($"Lỗi khi lấy danh sách giao dịch: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<Transaction>>> GetRecentTransactions(int userId, int count = 5)
        {
            try
            {
                var transactions = await _transactionRepository.GetRecentTransactions(userId, count);
                return ServiceResult<List<Transaction>>.Succeeded(transactions);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Transaction>>.Failed($"Lỗi khi lấy giao dịch gần đây: {ex.Message}");
            }
        }

        public async Task<ServiceResult<decimal>> GetBalance(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var totalIncome = await _transactionRepository.GetTotalIncomeByUserId(userId, fromDate, toDate);
                var totalExpense = await _transactionRepository.GetTotalExpenseByUserId(userId, fromDate, toDate);
                var balance = totalIncome - totalExpense;

                return ServiceResult<decimal>.Succeeded(balance);
            }
            catch (Exception ex)
            {
                return ServiceResult<decimal>.Failed($"Lỗi khi tính số dư: {ex.Message}");
            }
        }
    }
}
