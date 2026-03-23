using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace ParaBankAutomation.Utilities
{
    /// <summary>
    /// Factory tạo WebDriver cho Firefox
    /// GeckoDriver đã cài system-wide nên không cần chỉ đường dẫn
    /// </summary>
    public static class DriverFactory
    {
        /// <summary>
        /// Tạo và cấu hình FirefoxDriver
        /// </summary>
        /// <returns>IWebDriver đã cấu hình sẵn</returns>
        public static IWebDriver CreateDriver()
        {
            // Tạo options cho Firefox
            var options = new FirefoxOptions();

            // Nếu config bật headless → chạy không hiện giao diện
            if (ConfigReader.Headless)
            {
                options.AddArgument("--headless");
            }

            // Tạo FirefoxDriver (geckodriver đã có trong PATH, không cần chỉ path)
            var driver = new FirefoxDriver(options);

            // Cấu hình implicit wait — tự động chờ element xuất hiện trong khoảng thời gian này
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(ConfigReader.ImplicitWaitSeconds);

            // Đặt kích thước cửa sổ lớn để tránh element bị che
            driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);

            return driver;
        }
    }
}
