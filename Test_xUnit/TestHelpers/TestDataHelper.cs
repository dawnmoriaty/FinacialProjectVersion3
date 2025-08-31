namespace Test_xUnit.TestHelper;
using FinacialProjectVersion3.Data;
using FinacialProjectVersion3.Models.Entity;

public static class TestDataHelper
{
    public static void SeedTestData(ApplicationDbContext context)
    {
        // Tạo người dùng test
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = "test_hash",
            CreatedAt = DateTime.Now
        };

        context.Users.Add(user);

        // Tạo danh mục
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Lương", Type = "income", UserId = 1 },
            new Category { Id = 2, Name = "Ăn uống", Type = "expense", UserId = 1 },
            new Category { Id = 3, Name = "Mua sắm", Type = "expense", UserId = 1 },
            new Category { Id = 4, Name = "Tiện ích", Type = "expense", UserId = 1 }
        };

        context.Categories.AddRange(categories);

        // Tạo giao dịch
        var currentDate = DateTime.Today;
        var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

        var transactions = new List<Transaction>
        {
            // Tháng hiện tại
            new Transaction
            {
                Id = 1,
                Description = "Lương tháng",
                Amount = 10000000,
                TransactionDate = currentDate.AddDays(-5),
                CategoryId = 1,
                UserId = 1,
                CreatedAt = DateTime.Now.AddDays(-5),
                Category = categories[0]
            },
            new Transaction
            {
                Id = 2,
                Description = "Ăn trưa",
                Amount = 100000,
                TransactionDate = currentDate.AddDays(-4),
                CategoryId = 2,
                UserId = 1,
                CreatedAt = DateTime.Now.AddDays(-4),
                Category = categories[1]
            },
            new Transaction
            {
                Id = 3,
                Description = "Mua quần áo",
                Amount = 500000,
                TransactionDate = currentDate.AddDays(-3),
                CategoryId = 3,
                UserId = 1,
                CreatedAt = DateTime.Now.AddDays(-3),
                Category = categories[2]
            },
            new Transaction
            {
                Id = 4,
                Description = "Tiền điện",
                Amount = 300000,
                TransactionDate = currentDate.AddDays(-2),
                CategoryId = 4,
                UserId = 1,
                CreatedAt = DateTime.Now.AddDays(-2),
                Category = categories[3]
            },

            // Tháng trước
            new Transaction
            {
                Id = 5,
                Description = "Lương tháng trước",
                Amount = 10000000,
                TransactionDate = currentDate.AddMonths(-1),
                CategoryId = 1,
                UserId = 1,
                CreatedAt = DateTime.Now.AddMonths(-1),
                Category = categories[0]
            },
            new Transaction
            {
                Id = 6,
                Description = "Chi tiêu tháng trước",
                Amount = 800000,
                TransactionDate = currentDate.AddMonths(-1),
                CategoryId = 2,
                UserId = 1,
                CreatedAt = DateTime.Now.AddMonths(-1),
                Category = categories[1]
            }
        };

        context.Transactions.AddRange(transactions);
        context.SaveChanges();
    }
}
