using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ParaBankAutomation.Pages
{
    /// <summary>
    /// Page Object cho trang đăng ký tài khoản ParaBank
    /// Chứa các locator cho form đăng ký và phương thức điền form
    /// </summary>
    public class RegisterPage : BasePage
    {
        // === LOCATORS — dùng By.Id vì ParaBank cung cấp id cho mỗi field ===

        private readonly By _firstName = By.Id("customer.firstName");
        private readonly By _lastName = By.Id("customer.lastName");
        private readonly By _address = By.Id("customer.address.street");
        private readonly By _city = By.Id("customer.address.city");
        private readonly By _state = By.Id("customer.address.state");
        private readonly By _zipCode = By.Id("customer.address.zipCode");
        private readonly By _phone = By.Id("customer.phoneNumber");
        private readonly By _ssn = By.Id("customer.ssn");
        private readonly By _username = By.Id("customer.username");
        private readonly By _password = By.Id("customer.password");
        private readonly By _confirmPassword = By.Id("repeatedPassword");

        // Nút đăng ký
        private readonly By _registerButton = By.CssSelector("input[value='Register']");

        // Tiêu đề trang sau khi đăng ký thành công
        private readonly By _successTitle = By.CssSelector("#rightPanel h1.title");

        // Thông báo thành công
        private readonly By _successMessage = By.CssSelector("#rightPanel p");

        // Dòng chào mừng (xuất hiện sau khi đăng ký thành công — tự động login)
        private readonly By _welcomeText = By.CssSelector("#leftPanel p.smallText");

        /// <summary>
        /// Constructor — truyền driver cho BasePage
        /// </summary>
        public RegisterPage(IWebDriver driver) : base(driver) { }

        // === CÁC PHƯƠNG THỨC TƯƠNG TÁC ===

        /// <summary>
        /// Điền đầy đủ form đăng ký với tất cả các trường
        /// </summary>
        public void FillRegistrationForm(
            string firstName, string lastName,
            string address, string city, string state, string zipCode,
            string phone, string ssn,
            string username, string password, string confirmPassword)
        {
            // Điền thông tin cá nhân
            SendKeys(_firstName, firstName);
            SendKeys(_lastName, lastName);

            // Điền địa chỉ
            SendKeys(_address, address);
            SendKeys(_city, city);
            SendKeys(_state, state);
            SendKeys(_zipCode, zipCode);

            // Điền số điện thoại và SSN
            SendKeys(_phone, phone);
            SendKeys(_ssn, ssn);

            // Điền thông tin tài khoản
            SendKeys(_username, username);
            SendKeys(_password, password);
            SendKeys(_confirmPassword, confirmPassword);
        }

        /// <summary>
        /// Click nút Register để gửi form đăng ký
        /// Sau khi click, chờ trang kết quả load xong (title thay đổi)
        /// </summary>
        public void ClickRegister()
        {
            Click(_registerButton);

            // Chờ trang kết quả — title phải thay đổi khỏi trang đăng ký
            _wait.Until(d =>
                d.Title.Contains("Customer Created") ||
                d.Title.Contains("Accounts Overview") ||
                d.Title.Contains("Error"));
        }

        /// <summary>
        /// Thực hiện đăng ký đầy đủ: điền form + click Register
        /// </summary>
        public void Register(
            string firstName, string lastName,
            string address, string city, string state, string zipCode,
            string phone, string ssn,
            string username, string password, string confirmPassword)
        {
            FillRegistrationForm(firstName, lastName, address, city, state,
                zipCode, phone, ssn, username, password, confirmPassword);
            ClickRegister();
        }

        // === CÁC PHƯƠNG THỨC KIỂM TRA ===

        /// <summary>
        /// Lấy tiêu đề trang kết quả đăng ký (VD: "Welcome sel_user_xxx")
        /// </summary>
        public string GetSuccessTitle()
        {
            return GetText(_successTitle);
        }

        /// <summary>
        /// Lấy thông báo thành công sau khi đăng ký
        /// </summary>
        public string GetSuccessMessage()
        {
            return GetText(_successMessage);
        }

        /// <summary>
        /// Kiểm tra dòng chào mừng có hiển thị không (nghĩa là đã tự động login)
        /// </summary>
        public bool IsWelcomeMessageDisplayed()
        {
            return IsElementDisplayed(_welcomeText);
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
