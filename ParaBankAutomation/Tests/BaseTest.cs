using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using ParaBankAutomation.Pages;
using ParaBankAutomation.Utilities;

namespace ParaBankAutomation.Tests
{
    /// <summary>
    /// Lớp cơ sở cho tất cả test class
    /// Xử lý: tạo driver, đăng ký user 1 lần, chụp screenshot khi fail, đóng driver
    /// </summary>
    [TestFixture]
    public class BaseTest
    {
        // WebDriver — mỗi test tạo mới để đảm bảo độc lập
#pragma warning disable NUnit1032 // Dispose trong TearDown bằng Quit()
        protected IWebDriver driver = null!;
#pragma warning restore NUnit1032

        // Thông tin user đã đăng ký — dùng chung cho các test cần login
        // static để chia sẻ giữa tất cả test fixture
        protected static string RegisteredUsername = string.Empty;
        protected static string RegisteredPassword = "Auto@12345";

        // Cờ đánh dấu đã đăng ký user chưa (chỉ đăng ký 1 lần)
        private static bool _isUserRegistered = false;

        // Lock object để đảm bảo thread-safe khi đăng ký
        private static readonly object _registerLock = new object();

        /// <summary>
        /// Chạy 1 lần trước tất cả test trong fixture
        /// Đăng ký user mới nếu chưa đăng ký
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            lock (_registerLock)
            {
                // Nếu đã đăng ký rồi thì bỏ qua
                if (_isUserRegistered)
                    return;

                IWebDriver? tempDriver = null;
                try
                {
                    // Tạo driver tạm để đăng ký user
                    tempDriver = DriverFactory.CreateDriver();
                    tempDriver.Navigate().GoToUrl(ConfigReader.BaseUrl);

                    // Tạo username duy nhất bằng timestamp + random để tránh trùng
                    var rand = new Random().Next(1000, 9999);
                    RegisteredUsername = $"au_{DateTime.Now:MMddHHmmss}{rand}";

                    // Mở trang đăng ký trực tiếp bằng URL (tránh click bị intercept)
                    tempDriver
                        .Navigate()
                        .GoToUrl(ConfigReader.BaseUrl.Replace("index.htm", "register.htm"));
                    var registerPage = new RegisterPage(tempDriver);

                    // Điền form đăng ký và submit
                    registerPage.Register(
                        firstName: "Auto",
                        lastName: "Tester",
                        address: "123 Test Street",
                        city: "Test City",
                        state: "TC",
                        zipCode: "12345",
                        phone: "0901234567",
                        ssn: "123-45-6789",
                        username: RegisteredUsername,
                        password: RegisteredPassword,
                        confirmPassword: RegisteredPassword
                    );

                    // ClickRegister() đã tự chờ trang kết quả load xong

                    _isUserRegistered = true;
                    TestContext.Progress.WriteLine(
                        $"[BaseTest] Đăng ký user thành công: {RegisteredUsername}"
                    );
                }
                catch (Exception ex)
                {
                    // Nếu đăng ký thất bại — ghi log cảnh báo
                    // Các test cần login sẽ bị đánh dấu Inconclusive
                    TestContext.Progress.WriteLine(
                        $"[BaseTest] CẢNH BÁO: Không thể đăng ký user: {ex.Message}"
                    );
                }
                finally
                {
                    // Đóng driver tạm
                    tempDriver?.Quit();
                }
            }
        }

        /// <summary>
        /// Chạy trước MỖI test — tạo driver mới và mở trang chủ
        /// Đảm bảo mỗi test bắt đầu từ trạng thái sạch
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            driver = DriverFactory.CreateDriver();
            driver.Navigate().GoToUrl(ConfigReader.BaseUrl);
        }

        /// <summary>
        /// Chạy sau MỖI test — chụp screenshot nếu fail, rồi đóng driver
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            try
            {
                // Kiểm tra test có fail không
                if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                {
                    // Lấy tên test và nội dung lỗi
                    var testName = TestContext.CurrentContext.Test.Name;
                    var errorMessage = TestContext.CurrentContext.Result.Message ?? "Không rõ lỗi";

                    // Hiển thị popup thông báo lỗi trên browser (màu đỏ, nổi bật)
                    ScreenshotHelper.ShowErrorPopup(driver, testName, errorMessage);

                    // Chụp screenshot (có luôn popup lỗi trong ảnh)
                    ScreenshotHelper.CaptureScreenshot(driver, testName);
                }
            }
            finally
            {
                // Luôn đóng driver dù test pass hay fail
                driver?.Quit();
            }
        }

        /// <summary>
        /// Helper: kiểm tra user đã đăng ký chưa
        /// Nếu chưa, đánh dấu test là Inconclusive (không phải fail)
        /// </summary>
        protected void EnsureUserRegistered()
        {
            if (!_isUserRegistered)
            {
                Assert.Inconclusive(
                    "Không thể chạy test này vì chưa đăng ký được user. "
                        + "ParaBank server có thể đang bị lỗi."
                );
            }
        }
    }
}
