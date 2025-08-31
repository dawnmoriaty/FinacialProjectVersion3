using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Test_xUnit.UI.Selenium;

public class DashboardUITests : IDisposable
{
    private readonly IWebDriver _driver;
        private readonly string _baseUrl = "https://localhost:7133"; // Thay đổi theo URL của ứng dụng
        private readonly WebDriverWait _wait;

        public DashboardUITests()
        {
            var options = new ChromeOptions();
            // options.AddArgument("--headless"); // Chạy ẩn (bỏ comment nếu muốn chạy không giao diện)
            
            _driver = new ChromeDriver(options);
            _driver.Manage().Window.Maximize();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            
            // Đăng nhập trước khi chạy test
            Login();
        }

        private void Login()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Account/Login");
            
            _driver.FindElement(By.Id("Email")).SendKeys("test@example.com");
            _driver.FindElement(By.Id("Password")).SendKeys("Test123!");
            
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            
            // Đợi đến khi đăng nhập thành công
            _wait.Until(d => d.Url.Contains("/Dashboard") || d.Url.Contains("/Home"));
        }

        [Fact]
        public void TC_5_1_XemTongQuanTaiChinh()
        {
            // Navigate to Dashboard
            _driver.Navigate().GoToUrl($"{_baseUrl}/Dashboard");
            
            // Đợi trang tải xong
            _wait.Until(d => d.FindElement(By.CssSelector(".container-fluid h2")).Displayed);
            
            // Kiểm tra tiêu đề trang
            var pageTitle = _driver.FindElement(By.CssSelector(".container-fluid h2"));
            Assert.Contains("Dashboard", pageTitle.Text);
            
            // Kiểm tra hiển thị các thẻ tổng quan
            Assert.True(_driver.FindElement(By.XPath("//h6[contains(text(), 'Tổng Thu Nhập')]")).Displayed);
            Assert.True(_driver.FindElement(By.XPath("//h6[contains(text(), 'Tổng Chi Tiêu')]")).Displayed);
            Assert.True(_driver.FindElement(By.XPath("//h6[contains(text(), 'Số Dư')]")).Displayed);
            Assert.True(_driver.FindElement(By.XPath("//h6[contains(text(), 'Thu Nhập Tháng')]")).Displayed);
        }

        [Fact]
        public void TC_5_2_XemBieuDoThuChi()
        {
            // Navigate to Dashboard
            _driver.Navigate().GoToUrl($"{_baseUrl}/Dashboard");
            
            // Đợi trang tải xong
            _wait.Until(d => d.FindElement(By.Id("monthlyChart")).Displayed);
            
            // Kiểm tra tiêu đề biểu đồ
            var chartTitle = _driver.FindElement(By.XPath("//h5[contains(text(), 'Thu Chi 6 Tháng Gần Nhất')]"));
            Assert.True(chartTitle.Displayed);
            
            // Kiểm tra canvas biểu đồ đã được tạo
            var chartCanvas = _driver.FindElement(By.Id("monthlyChart"));
            Assert.True(chartCanvas.Displayed);
            Assert.True(chartCanvas.Size.Width > 0);
            Assert.True(chartCanvas.Size.Height > 0);
        }

        [Fact]
        public void TC_5_3_XemBieuDoPhanBoChuTieu()
        {
            // Navigate to Dashboard
            _driver.Navigate().GoToUrl($"{_baseUrl}/Dashboard");
            
            // Đợi trang tải xong
            _wait.Until(d => d.FindElement(By.Id("categoryChart")).Displayed);
            
            // Kiểm tra tiêu đề biểu đồ
            var chartTitle = _driver.FindElement(By.XPath("//h5[contains(text(), 'Chi Tiêu Tháng Này')]"));
            Assert.True(chartTitle.Displayed);
            
            // Kiểm tra canvas biểu đồ đã được tạo
            var chartCanvas = _driver.FindElement(By.Id("categoryChart"));
            Assert.True(chartCanvas.Displayed);
            Assert.True(chartCanvas.Size.Width > 0);
            Assert.True(chartCanvas.Size.Height > 0);
        }

        [Fact]
        public void TC_5_4_XemGiaoDichGanDay()
        {
            // Navigate to Dashboard
            _driver.Navigate().GoToUrl($"{_baseUrl}/Dashboard");
            
            // Đợi trang tải xong
            _wait.Until(d => d.FindElement(By.XPath("//h5[contains(text(), 'Giao Dịch Gần Đây')]")).Displayed);
            
            // Kiểm tra tiêu đề phần giao dịch gần đây
            var sectionTitle = _driver.FindElement(By.XPath("//h5[contains(text(), 'Giao Dịch Gần Đây')]"));
            Assert.True(sectionTitle.Displayed);
            
            try
            {
                // Kiểm tra bảng giao dịch
                var transactionTable = _driver.FindElement(By.CssSelector(".table"));
                Assert.True(transactionTable.Displayed);
                
                // Kiểm tra các cột trong bảng
                var headers = transactionTable.FindElements(By.CssSelector("thead th"));
                Assert.True(headers.Count >= 4); // Có ít nhất 4 cột
            }
            catch (NoSuchElementException)
            {
                // Trường hợp không có giao dịch nào
                var emptyMessage = _driver.FindElement(By.XPath("//p[contains(text(), 'Chưa có giao dịch nào')]"));
                Assert.True(emptyMessage.Displayed);
            }
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
