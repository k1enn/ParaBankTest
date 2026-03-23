using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using ParaBankAutomation.Utilities;

namespace ParaBankAutomation.Pages
{
    /// <summary>
    /// Lớp cơ sở cho tất cả Page Object
    /// Chứa các phương thức dùng chung: chờ element, click, nhập text, lấy text...
    /// KHÔNG dùng Thread.Sleep — chỉ dùng WebDriverWait
    /// </summary>
    public class BasePage
    {
        // WebDriver dùng để tương tác với trình duyệt
        protected readonly IWebDriver _driver;

        // WebDriverWait dùng để chờ element xuất hiện/sẵn sàng
        protected readonly WebDriverWait _wait;

        /// <summary>
        /// Constructor — nhận driver và tạo WebDriverWait với timeout từ config
        /// </summary>
        public BasePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(ConfigReader.ExplicitWaitSeconds));
        }

        /// <summary>
        /// Chờ element hiển thị trên trang và trả về element đó
        /// </summary>
        /// <param name="locator">Cách tìm element (By.Id, By.Name, ...)</param>
        /// <returns>Element đã hiển thị</returns>
        protected IWebElement WaitForElement(By locator)
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        /// <summary>
        /// Chờ element có thể click được, rồi click vào nó
        /// Nếu bị element khác che → dùng JavaScript click thay thế
        /// </summary>
        /// <param name="locator">Cách tìm element</param>
        protected void Click(By locator)
        {
            // Chờ element clickable (hiển thị + enabled)
            var element = _wait.Until(ExpectedConditions.ElementToBeClickable(locator));

            try
            {
                // Cuộn đến element trước khi click
                ((IJavaScriptExecutor)_driver).ExecuteScript(
                    "arguments[0].scrollIntoView({block: 'center'});", element);
                element.Click();
            }
            catch (OpenQA.Selenium.ElementClickInterceptedException)
            {
                // Nếu element bị che bởi element khác → dùng JavaScript click
                ((IJavaScriptExecutor)_driver).ExecuteScript(
                    "arguments[0].click();", element);
            }
        }

        /// <summary>
        /// Chờ element hiển thị, xóa nội dung cũ, rồi nhập text mới
        /// </summary>
        /// <param name="locator">Cách tìm element</param>
        /// <param name="text">Nội dung cần nhập</param>
        protected void SendKeys(By locator, string text)
        {
            var element = WaitForElement(locator);
            element.Clear();       // Xóa nội dung cũ trong ô input
            element.SendKeys(text); // Nhập text mới
        }

        /// <summary>
        /// Chờ element hiển thị và lấy nội dung text của nó
        /// </summary>
        /// <param name="locator">Cách tìm element</param>
        /// <returns>Nội dung text của element</returns>
        protected string GetText(By locator)
        {
            var element = WaitForElement(locator);
            return element.Text;
        }

        /// <summary>
        /// Kiểm tra element có đang hiển thị trên trang không
        /// Dùng try-catch vì element có thể không tồn tại
        /// </summary>
        /// <param name="locator">Cách tìm element</param>
        /// <returns>true nếu element hiển thị, false nếu không</returns>
        protected bool IsElementDisplayed(By locator)
        {
            try
            {
                // Tạo wait ngắn hơn (3 giây) để không chờ quá lâu
                var shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
                var element = shortWait.Until(ExpectedConditions.ElementIsVisible(locator));
                return element.Displayed;
            }
            catch
            {
                // Element không tìm thấy hoặc không hiển thị
                return false;
            }
        }

        /// <summary>
        /// Chờ cho đến khi URL chứa chuỗi chỉ định
        /// </summary>
        /// <param name="partialUrl">Phần URL cần kiểm tra</param>
        protected void WaitForUrl(string partialUrl)
        {
            _wait.Until(ExpectedConditions.UrlContains(partialUrl));
        }

        /// <summary>
        /// Lấy title (tiêu đề) của trang hiện tại
        /// </summary>
        /// <returns>Title của trang</returns>
        protected string GetPageTitle()
        {
            return _driver.Title;
        }
    }
}
