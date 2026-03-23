using ParaBankAutomation.Pages;

namespace ParaBankAutomation.Tests
{
    /// <summary>
    /// Smoke Tests — kiểm tra các chức năng cơ bản nhất của hệ thống
    /// Nếu smoke test fail → hệ thống có vấn đề nghiêm trọng
    /// </summary>
    [TestFixture]
    [Category("Smoke")]
    public class SmokeTests : BaseTest
    {
        /// <summary>
        /// Test 1: Kiểm tra trang chủ ParaBank load thành công
        /// - Title phải đúng
        /// - Logo phải hiển thị
        /// - Panel đăng nhập phải hiển thị
        /// </summary>
        [Test]
        [Description(
            "Kiểm tra trang chủ ParaBank load thành công, title đúng, logo và form login hiển thị"
        )]
        public void HomePage_ShouldLoadSuccessfully()
        {
            // Tạo LoginPage object để kiểm tra các element trên trang chủ
            var loginPage = new LoginPage(driver);

            // Kiểm tra title trang chủ
            var title = loginPage.GetTitle();
            Assert.That(title, Does.Contain("WRONG_TITLE"), "Title trang chủ phải chứa 'ParaBank'");

            // Kiểm tra logo hiển thị
            Assert.That(
                loginPage.IsLogoDisplayed(),
                Is.True,
                "Logo ParaBank phải hiển thị trên trang chủ"
            );

            // Kiểm tra panel đăng nhập hiển thị
            Assert.That(
                loginPage.IsLoginPanelDisplayed(),
                Is.True,
                "Panel đăng nhập phải hiển thị trên trang chủ"
            );
        }

        /// <summary>
        /// Test 2: Kiểm tra đăng nhập với thông tin hợp lệ
        /// - Dùng user đã đăng ký ở OneTimeSetUp
        /// - Sau login: title chứa "Accounts Overview", hiện Welcome, có nút Logout
        /// </summary>
        [Test]
        [Description("Kiểm tra đăng nhập với thông tin hợp lệ — hiện Welcome và Accounts Overview")]
        public void Login_WithValidCredentials_ShouldSucceed()
        {
            // Đảm bảo đã có user đăng ký (nếu chưa → Inconclusive)
            EnsureUserRegistered();

            // Tạo LoginPage và thực hiện đăng nhập
            var loginPage = new LoginPage(driver);
            loginPage.Login(RegisteredUsername, RegisteredPassword);

            // Kiểm tra title trang sau khi login — phải chứa "Accounts Overview"
            var title = loginPage.GetTitle();
            Assert.That(
                title,
                Does.Contain("Accounts Overview"),
                "Sau khi login, title phải chứa 'Accounts Overview'"
            );

            // Kiểm tra dòng chào mừng hiển thị (Welcome ...)
            var welcomeText = loginPage.GetWelcomeText();
            Assert.That(
                welcomeText,
                Does.Contain("Welcome"),
                "Phải hiển thị dòng chào mừng 'Welcome' sau khi login"
            );

            // Kiểm tra có nút Logout (chứng tỏ đã login thành công)
            Assert.That(
                loginPage.IsLogoutLinkDisplayed(),
                Is.True,
                "Nút Logout phải hiển thị sau khi login thành công"
            );
        }
    }
}
