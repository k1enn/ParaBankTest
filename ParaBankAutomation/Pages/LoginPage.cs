using OpenQA.Selenium;

namespace ParaBankAutomation.Pages
{
    /// <summary>
    /// Page Object cho trang đăng nhập ParaBank
    /// Chứa các locator và phương thức tương tác với form login
    /// </summary>
    public class LoginPage : BasePage
    {
        // === LOCATORS — ưu tiên By.Name và By.CssSelector, hạn chế XPath ===

        // Ô nhập username (dùng attribute name)
        private readonly By _usernameInput = By.Name("username");

        // Ô nhập password (dùng attribute name)
        private readonly By _passwordInput = By.Name("password");

        // Nút đăng nhập (dùng CSS selector theo value)
        private readonly By _loginButton = By.CssSelector("input[value='Log In']");

        // Link đăng ký tài khoản mới
        private readonly By _registerLink = By.LinkText("Register");

        // Thông báo lỗi khi đăng nhập thất bại
        private readonly By _errorMessage = By.CssSelector("p.error");

        // Dòng chào mừng sau khi đăng nhập thành công ("Welcome ...")
        private readonly By _welcomeText = By.CssSelector("#leftPanel p.smallText");

        // Panel chứa form đăng nhập
        private readonly By _loginPanel = By.Id("loginPanel");

        // Logo ParaBank
        private readonly By _logo = By.CssSelector("img.logo");

        // Link đăng xuất (hiện sau khi login thành công)
        private readonly By _logoutLink = By.LinkText("Log Out");

        /// <summary>
        /// Constructor — truyền driver cho BasePage
        /// </summary>
        public LoginPage(IWebDriver driver) : base(driver) { }

        // === CÁC PHƯƠNG THỨC TƯƠNG TÁC ===

        /// <summary>
        /// Nhập username vào ô input
        /// </summary>
        public void EnterUsername(string username)
        {
            SendKeys(_usernameInput, username);
        }

        /// <summary>
        /// Nhập password vào ô input
        /// </summary>
        public void EnterPassword(string password)
        {
            SendKeys(_passwordInput, password);
        }

        /// <summary>
        /// Click nút Log In
        /// </summary>
        public void ClickLogin()
        {
            Click(_loginButton);
        }

        /// <summary>
        /// Thực hiện đăng nhập đầy đủ: nhập username, password, click Login
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        public void Login(string username, string password)
        {
            EnterUsername(username);
            EnterPassword(password);
            ClickLogin();
        }

        /// <summary>
        /// Click link "Register" để chuyển sang trang đăng ký
        /// Trả về RegisterPage (fluent pattern — cho phép gọi tiếp methods của trang mới)
        /// </summary>
        public RegisterPage ClickRegisterLink()
        {
            Click(_registerLink);
            return new RegisterPage(_driver);
        }

        // === CÁC PHƯƠNG THỨC KIỂM TRA ===

        /// <summary>
        /// Lấy nội dung thông báo lỗi khi đăng nhập thất bại
        /// </summary>
        public string GetErrorMessage()
        {
            return GetText(_errorMessage);
        }

        /// <summary>
        /// Lấy dòng chào mừng sau khi đăng nhập thành công
        /// </summary>
        public string GetWelcomeText()
        {
            return GetText(_welcomeText);
        }

        /// <summary>
        /// Kiểm tra nút Login có hiển thị không
        /// </summary>
        public bool IsLoginButtonDisplayed()
        {
            return IsElementDisplayed(_loginButton);
        }

        /// <summary>
        /// Kiểm tra nút Login có enabled (có thể click) không
        /// </summary>
        public bool IsLoginButtonEnabled()
        {
            try
            {
                var element = WaitForElement(_loginButton);
                return element.Enabled;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra panel đăng nhập có hiển thị không
        /// </summary>
        public bool IsLoginPanelDisplayed()
        {
            return IsElementDisplayed(_loginPanel);
        }

        /// <summary>
        /// Kiểm tra logo ParaBank có hiển thị không
        /// </summary>
        public bool IsLogoDisplayed()
        {
            return IsElementDisplayed(_logo);
        }

        /// <summary>
        /// Kiểm tra link Logout có hiển thị không (chỉ hiện sau khi login)
        /// </summary>
        public bool IsLogoutLinkDisplayed()
        {
            return IsElementDisplayed(_logoutLink);
        }

        /// <summary>
        /// Lấy title của trang hiện tại
        /// </summary>
        public string GetTitle()
        {
            return GetPageTitle();
        }
    }
}
