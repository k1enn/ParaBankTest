using ParaBankAutomation.Pages;

namespace ParaBankAutomation.Tests
{
    /// <summary>
    /// GUI Tests — kiểm tra giao diện người dùng
    /// Đảm bảo các element hiển thị đúng, có thể tương tác
    /// </summary>
    [TestFixture]
    [Category("GUI")]
    public class GUITests : BaseTest
    {
        /// <summary>
        /// Test 3: Kiểm tra nút Login hiển thị và có thể click
        /// - Nút phải visible
        /// - Nút phải enabled (có thể click)
        /// </summary>
        [Test]
        [Description("Kiểm tra nút Login hiển thị trên trang và ở trạng thái enabled")]
        public void LoginButton_ShouldBeDisplayedAndClickable()
        {
            // Tạo LoginPage để kiểm tra các element
            var loginPage = new LoginPage(driver);

            // Kiểm tra nút Login có hiển thị không
            Assert.That(loginPage.IsLoginButtonDisplayed(), Is.True,
                "Nút Login phải hiển thị trên trang đăng nhập");

            // Kiểm tra nút Login có enabled (clickable) không
            Assert.That(loginPage.IsLoginButtonEnabled(), Is.True,
                "Nút Login phải ở trạng thái enabled (có thể click)");
        }
    }
}
