namespace FinacialProjectVersion3.ViewModels.Transaction
{
    public class TransactionListViewModel
    {
        public List<Models.Entity.Transaction> Transactions { get; set; } = new List<Models.Entity.Transaction>();
        public TransactionFilterViewModel Filter { get; set; } = new TransactionFilterViewModel();

        // Pagination
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        // Summary
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
    }
}