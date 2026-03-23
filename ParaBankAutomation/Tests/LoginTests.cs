using ParaBankAutomation.Pages;

namespace ParaBankAutomation.Tests
{
    /// <summary>
    /// Login Tests — kiểm tra chức năng đăng nhập (negative test cases)
    /// </summary>
    [TestFixture]
    [Category("Functional")]
    public class LoginTests : BaseTest
    {
        /// <summary>
        /// Test 5: Kiểm tra đăng nhập với trường rỗng hiển thị lỗi
        /// - Không nhập username và password
        /// - Click Login → phải hiện thông báo lỗi
        /// </summary>
        [Test]
        [Description("Kiểm tra đăng nhập với trường rỗng — phải hiển thị thông báo lỗi yêu cầu nhập thông tin")]
        public void Login_WithEmptyFields_ShouldShowError()
        {
            // Tạo LoginPage
            var loginPage = new LoginPage(driver);

            // Click nút Login mà không nhập gì
            loginPage.ClickLogin();

            // Kiểm tra thông báo lỗi hiển thị
            var errorMessage = loginPage.GetErrorMessage();
            Assert.That(errorMessage, Does.Contain("enter a username and password"),
                "Phải hiển thị thông báo lỗi yêu cầu nhập username và password");

            // Kiểm tra title trang — ParaBank chuyển sang trang Error
            var title = loginPage.GetTitle();
            Assert.That(title, Does.Contain("Error"),
                "Title trang phải chứa 'Error' khi đăng nhập thất bại");
        }
    }
}
