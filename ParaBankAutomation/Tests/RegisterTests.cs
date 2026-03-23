using ParaBankAutomation.Pages;

namespace ParaBankAutomation.Tests
{
    /// <summary>
    /// Register Tests — kiểm tra chức năng đăng ký tài khoản
    /// </summary>
    [TestFixture]
    [Category("Functional")]
    public class RegisterTests : BaseTest
    {
        /// <summary>
        /// Test 4: Kiểm tra đăng ký tài khoản mới thành công
        /// - Tạo username riêng (khác với user đã đăng ký ở BaseTest)
        /// - Điền đầy đủ form và submit
        /// - Kiểm tra trang kết quả hiện thông báo thành công
        /// </summary>
        [Test]
        [Description("Kiểm tra đăng ký tài khoản mới với đầy đủ thông tin hợp lệ")]
        public void Register_WithValidData_ShouldSucceed()
        {
            // Tạo LoginPage và click link Register để chuyển sang trang đăng ký
            var loginPage = new LoginPage(driver);
            var registerPage = loginPage.ClickRegisterLink();

            // Tạo username duy nhất cho test này (thêm random để tránh trùng)
            var rand = new Random().Next(1000, 9999);
            var uniqueUsername = $"rg_{DateTime.Now:MMddHHmmss}{rand}";
            var password = "Test@12345";

            // Điền form đăng ký với đầy đủ thông tin
            registerPage.Register(
                firstName: "Selenium",
                lastName: "Tester",
                address: "123 Automation Street",
                city: "Test City",
                state: "TC",
                zipCode: "12345",
                phone: "0901234567",
                ssn: "123-45-6789",
                username: uniqueUsername,
                password: password,
                confirmPassword: password
            );

            // Kiểm tra tiêu đề trang — ParaBank hiện "Welcome <username>" khi đăng ký thành công
            var successTitle = registerPage.GetSuccessTitle();
            Assert.That(successTitle, Does.Contain("Welcome").Or.Contain(uniqueUsername),
                "Trang kết quả phải hiển thị tiêu đề chào mừng với username");

            // Kiểm tra thông báo thành công
            var successMessage = registerPage.GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("created successfully").Or.Contain("logged in"),
                "Phải hiển thị thông báo tài khoản đã được tạo thành công");

            // Kiểm tra đã tự động login (Welcome message trong left panel)
            Assert.That(registerPage.IsWelcomeMessageDisplayed(), Is.True,
                "Sau khi đăng ký thành công, phải tự động đăng nhập (hiện Welcome)");
        }
    }
}
