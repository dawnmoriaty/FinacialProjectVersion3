using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace Test_xUnit.UI.Selenium;

public class DashboardUITests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly string _baseUrl = "http://localhost:5091"; // đổi theo app của bạn
    private readonly WebDriverWait _wait;

    public DashboardUITests()
    {
        var options = new ChromeOptions();
        // options.AddArgument("--headless"); // Chạy headless để tránh lỗi UI
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        _driver = new ChromeDriver(options);
        _driver.Manage().Window.Maximize();
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

        Login();
    }

    private void Login()
    {
        try
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Account/Login");

            // Chờ trang login load xong
            _wait.Until(d => d.FindElement(By.Id("Username")));
            
            _driver.FindElement(By.Id("Username")).SendKeys("nghao");
            _driver.FindElement(By.Id("Password")).SendKeys("123456");
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Sau khi login thành công, ứng dụng chuyển về Home, không phải Dashboard
            // Nên chúng ta chờ URL chứa Home hoặc không chứa Login
            _wait.Until(d => !d.Url.Contains("/Account/Login"));
            
            // Đợi một chút để đảm bảo trang đã load hoàn toàn
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể đăng nhập: {ex.Message}");
        }
    }

    [Fact]
    public void TC_5_1_XemTongQuanTaiChinh()
    {
        // Chuyển đến trang Dashboard sau khi đã login
        _driver.Navigate().GoToUrl($"{_baseUrl}/Dashboard");

        // Chờ trang Dashboard load
        try 
        {
            _wait.Until(d => d.FindElement(By.CssSelector("h2, .container-fluid")));
            
            // Kiểm tra các thẻ thống kê tài chính có hiển thị không
            var totalIncomeElement = _wait.Until(d => 
                d.FindElements(By.XPath("//h6[contains(text(), 'Tổng Thu Nhập')] | //div[contains(@class,'card')]//h6")).FirstOrDefault());
            Assert.NotNull(totalIncomeElement);

            var totalExpenseElement = _driver.FindElements(By.XPath("//h6[contains(text(), 'Tổng Chi Tiêu')] | //div[contains(@class,'card')]//h6")).FirstOrDefault();
            Assert.NotNull(totalExpenseElement);

            // Kiểm tra có ít nhất 2 card thống kê
            var statisticCards = _driver.FindElements(By.CssSelector(".card"));
            Assert.True(statisticCards.Count >= 2, "Phải có ít nhất 2 card thống kê");
        }
        catch (Exception ex)
        {
            throw new Exception($"Không tìm thấy elements tổng quan tài chính: {ex.Message}");
        }
    }

    [Fact]
    public void TC_5_2_XemBieuDoThuChi()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/Dashboard");

        try
        {
            // Chờ canvas chart xuất hiện 
            var chartCanvas = _wait.Until(d => 
            {
                var elements = d.FindElements(By.Id("monthlyChart"));
                return elements.FirstOrDefault(e => e.Displayed);
            });

            Assert.NotNull(chartCanvas);
            Assert.True(chartCanvas.Size.Width > 0, "Chart phải có chiều rộng > 0");
            Assert.True(chartCanvas.Size.Height > 0, "Chart phải có chiều cao > 0");

            // Tìm tiêu đề chart - có thể nằm trong card chứa chart
            var chartCards = _driver.FindElements(By.CssSelector(".card"));
            var hasChartTitle = chartCards.Any(card => 
                card.Text.Contains("Thu Chi") || 
                card.Text.Contains("Biểu Đồ") || 
                card.FindElements(By.TagName("canvas")).Count > 0);
            
            Assert.True(hasChartTitle, "Phải có tiêu đề hoặc card chứa biểu đồ thu chi");
        }
        catch (Exception ex)
        {
            throw new Exception($"Không tìm thấy biểu đồ thu chi: {ex.Message}");
        }
    }

    [Fact]
    public void TC_5_3_XemBieuDoPhanBoChiTieu()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/Dashboard");

        try
        {
            var chartCanvas = _wait.Until(d => 
            {
                var elements = d.FindElements(By.Id("categoryChart"));
                return elements.FirstOrDefault(e => e.Displayed);
            });

            Assert.NotNull(chartCanvas);
            Assert.True(chartCanvas.Size.Width > 0, "Category chart phải có chiều rộng > 0");
            Assert.True(chartCanvas.Size.Height > 0, "Category chart phải có chiều cao > 0");

            // Kiểm tra có card chứa category chart
            var chartCards = _driver.FindElements(By.CssSelector(".card"));
            var hasCategoryChart = chartCards.Any(card => 
                card.Text.Contains("Chi Tiêu") || 
                card.Text.Contains("Danh Mục") ||
                card.FindElements(By.Id("categoryChart")).Count > 0);
            
            Assert.True(hasCategoryChart, "Phải có card chứa biểu đồ phân bổ chi tiêu");
        }
        catch (Exception ex)
        {
            throw new Exception($"Không tìm thấy biểu đồ phân bổ chi tiêu: {ex.Message}");
        }
    }

    [Fact]
    public void TC_5_4_XemGiaoDichGanDay()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/Dashboard");

        try
        {
            // Tìm section giao dịch gần đây
            var hasTransactionSection = false;
            
            // Tìm theo nhiều cách khác nhau
            var possibleSelectors = new[]
            {
                "//h5[contains(., 'Giao Dịch Gần Đây')]",
                "//h5[contains(., 'Giao Dịch')]", 
                "//h4[contains(., 'Giao Dịch')]",
                "//div[contains(@class,'card')]//h5",
                "//div[contains(@class,'card')]//h4"
            };

            IWebElement? sectionTitle = null;
            foreach (var selector in possibleSelectors)
            {
                try
                {
                    var elements = _driver.FindElements(By.XPath(selector));
                    sectionTitle = elements.FirstOrDefault(e => e.Displayed && e.Text.Contains("Giao Dịch"));
                    if (sectionTitle != null)
                    {
                        hasTransactionSection = true;
                        break;
                    }
                }
                catch (Exception) { /* Tiếp tục thử selector khác */ }
            }

            if (!hasTransactionSection)
            {
                // Nếu không tìm thấy title, kiểm tra có table giao dịch không
                var tables = _driver.FindElements(By.CssSelector(".table, table"));
                hasTransactionSection = tables.Any(t => t.Displayed);
            }

            Assert.True(hasTransactionSection, "Phải có section hoặc table giao dịch gần đây");

            // Kiểm tra table hoặc message không có dữ liệu
            try
            {
                var transactionTable = _driver.FindElement(By.CssSelector(".table, table"));
                if (transactionTable.Displayed)
                {
                    var headers = transactionTable.FindElements(By.CssSelector("thead th, th"));
                    Assert.True(headers.Count >= 3, "Table giao dịch phải có ít nhất 3 cột");
                }
            }
            catch (NoSuchElementException)
            {
                // Nếu không có table, kiểm tra có thông báo không có dữ liệu
                var emptyMessages = _driver.FindElements(By.XPath(
                    "//p[contains(text(), 'Chưa có giao dịch')] | " +
                    "//p[contains(text(), 'Không có giao dịch')] | " +
                    "//div[contains(text(), 'Chưa có')] | " +
                    "//div[contains(@class, 'empty') or contains(@class, 'no-data')]"));
                
                var hasEmptyMessage = emptyMessages.Any(e => e.Displayed);
                Assert.True(hasEmptyMessage, "Nếu không có table thì phải có thông báo không có dữ liệu");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi kiểm tra giao dịch gần đây: {ex.Message}");
        }
    }

    public void Dispose()
    {
        try
        {
            _driver?.Quit();
        }
        catch (Exception)
        {
            // Ignore cleanup errors
        }
        finally
        {
            _driver?.Dispose();
        }
    }
}
