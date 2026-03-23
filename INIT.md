# SELENIUM AUTOMATION TESTING — PARABANK

# Prompt cho Claude Code

## BỐI CẢNH

Đây là bài tập nhóm môn Testing: xây dựng automation test cho website ParaBank.

- Tôi làm **một mình** (tính như nhóm 3 → tối thiểu 15 test scenarios, mỗi SV 5 automation tests)
- Tôi **mới học C#/Selenium**, chưa biết gì → code cần có **comment tiếng Việt** giải thích logic
- Mục tiêu: **9-10 điểm** → cần vượt yêu cầu tối thiểu, code chất lượng cao, coverage rộng
- Tôi sẽ chỉ chạy, không tự viết thêm → **tạo toàn bộ project hoàn chỉnh, chạy được ngay**

## MÔI TRƯỜNG — QUAN TRỌNG, ĐỌC KỸ

- **OS**: Arch Linux
- **.NET SDK**: 10.0.104 (đã cài sẵn)
- **Browser**: Firefox (KHÔNG dùng Chrome/Chromium)
- **WebDriver**: GeckoDriver — **CHƯA CÀI**, cần cài
- **AUR helper**: yay
- **Thư mục**: Tôi đã mở Claude Code trong thư mục project rỗng, tạo mọi thứ tại `.` (thư mục hiện tại)

## BƯỚC 0 — SETUP MÔI TRƯỜNG (chạy trước khi tạo code)

```bash
# 1. Detect Firefox version
firefox --version

# 2. Cài geckodriver từ AUR
yay -S --noconfirm geckodriver

# 3. Verify
geckodriver --version
dotnet --version
```

Nếu geckodriver không cài được qua yay, tải binary từ GitHub releases:

```bash
# Lấy version mới nhất từ https://github.com/mozilla/geckodriver/releases
# wget, giải nén, chmod +x, move vào /usr/local/bin/
```

## BƯỚC 1 — TẠO PROJECT

```bash
dotnet new nunit -n ParaBankAutomation --framework net9.0
cd ParaBankAutomation
```

> **QUAN TRỌNG**: Dùng `net9.0` làm target framework (ổn định hơn net10.0 cho NuGet packages). SDK 10.0 vẫn build được net9.0.

Cài NuGet packages:

```bash
dotnet add package Selenium.WebDriver
dotnet add package Selenium.Support
dotnet add package NUnit
dotnet add package NUnit3TestAdapter
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package Newtonsoft.Json
```

> **KHÔNG** cài `Selenium.WebDriver.ChromeDriver`. Dùng **Firefox + GeckoDriver** đã cài system-wide.

## BƯỚC 2 — CẤU TRÚC THƯ MỤC

```
ParaBankAutomation/
├── ParaBankAutomation.csproj
├── Pages/
│   ├── BasePage.cs              # Base class cho tất cả pages
│   ├── LoginPage.cs             # Trang đăng nhập
│   ├── RegisterPage.cs          # Trang đăng ký
│   ├── AccountOverviewPage.cs   # Tổng quan tài khoản
│   ├── OpenNewAccountPage.cs    # Mở tài khoản mới
│   ├── TransferFundsPage.cs     # Chuyển tiền
│   ├── BillPayPage.cs           # Thanh toán hóa đơn
│   ├── FindTransactionsPage.cs  # Tìm giao dịch
│   ├── UpdateContactInfoPage.cs # Cập nhật thông tin
│   └── RequestLoanPage.cs       # Yêu cầu vay
├── Tests/
│   ├── BaseTest.cs              # SetUp/TearDown chung
│   ├── SmokeTests.cs            # Smoke tests
│   ├── GUITests.cs              # GUI tests
│   ├── LoginTests.cs            # Test đăng nhập
│   ├── RegisterTests.cs         # Test đăng ký
│   ├── AccountOverviewTests.cs  # Test tổng quan TK
│   ├── OpenNewAccountTests.cs   # Test mở TK mới
│   ├── TransferFundsTests.cs    # Test chuyển tiền
│   ├── BillPayTests.cs          # Test thanh toán
│   ├── FindTransactionsTests.cs # Test tìm giao dịch
│   ├── UpdateContactInfoTests.cs# Test cập nhật info
│   └── RequestLoanTests.cs      # Test yêu cầu vay
├── Utilities/
│   ├── DriverFactory.cs         # Tạo FirefoxDriver
│   ├── ConfigReader.cs          # Đọc config
│   └── ScreenshotHelper.cs      # Chụp ảnh khi fail
├── TestData/
│   ├── users.json               # Data user
│   └── testdata.json            # Data test khác
├── Screenshots/                  # Thư mục lưu screenshot
├── appsettings.json             # Config (URL, timeout...)
├── TestPlan.md                  # Kế hoạch test
├── TestScenarios.md             # Danh sách scenarios
└── README.md                    # Hướng dẫn chạy
```

## BƯỚC 3 — THIẾT KẾ CHI TIẾT

### 3.1 appsettings.json

```json
{
  "BaseUrl": "https://parabank.parasoft.com/parabank/index.htm",
  "Browser": "Firefox",
  "Headless": false,
  "ImplicitWaitSeconds": 10,
  "ExplicitWaitSeconds": 15,
  "ScreenshotPath": "Screenshots/"
}
```

### 3.2 DriverFactory.cs

- Tạo **FirefoxDriver** (KHÔNG phải ChromeDriver)
- GeckoDriver đã cài system-wide, KHÔNG cần chỉ path
- Cấu hình: `new FirefoxOptions()` với implicit wait, maximize window
- Hỗ trợ headless mode qua `options.AddArgument("--headless")` khi config bật
- Return IWebDriver

### 3.3 BasePage.cs

Chứa methods dùng chung (tất cả dùng **WebDriverWait**, KHÔNG dùng Thread.Sleep):

- `WaitForElement(By locator)` — đợi element visible
- `Click(By locator)` — đợi + click
- `SendKeys(By locator, string text)` — đợi + clear + nhập text
- `GetText(By locator)` — đợi + lấy text
- `IsElementDisplayed(By locator)` — kiểm tra hiển thị (try-catch, return bool)
- `WaitForUrl(string partialUrl)` — đợi URL chứa chuỗi
- `GetPageTitle()` — lấy title trang
- Constructor nhận IWebDriver

### 3.4 BaseTest.cs

- `[OneTimeSetUp]`: Đăng ký user mới 1 lần cho toàn bộ test suite (gọi API hoặc Register qua UI), lưu username/password vào biến static
- `[SetUp]`: Tạo driver qua DriverFactory, navigate đến BaseUrl
- `[TearDown]`: Nếu test FAIL → chụp screenshot lưu vào `Screenshots/{TestName}_{timestamp}.png`. Sau đó quit driver
- Dùng `TestContext.CurrentContext.Result.Outcome.Status` để check fail

### 3.5 Page Object Model — QUY TẮC BẮT BUỘC

- **Mỗi page = 1 class**, kế thừa BasePage
- **Locator** (kiểu `By`) khai báo là `private` hoặc `private readonly` trong page class
- **Ưu tiên locator**: `By.Id` > `By.Name` > `By.CssSelector` > `By.XPath`
- **Methods** trong page thực hiện action (VD: `Login(user, pass)`, `FillRegistrationForm(data)`)
- **Methods trả về page object** khi navigate sang trang khác (fluent pattern)
- **Test class KHÔNG chứa By locator hay FindElement** — chỉ gọi methods từ Page

### 3.6 Test Data

**users.json**:

```json
{
  "validUser": {
    "username": "auto_test_user",
    "password": "Auto@12345"
  },
  "invalidUser": {
    "username": "wrong_user_xxx",
    "password": "wrong_pass_xxx"
  },
  "registerUser": {
    "firstName": "Selenium",
    "lastName": "Tester",
    "address": "123 Automation Street",
    "city": "Test City",
    "state": "TC",
    "zipCode": "12345",
    "phone": "0901234567",
    "ssn": "123-45-6789",
    "username": "sel_user_{timestamp}",
    "password": "Auto@12345",
    "confirmPassword": "Auto@12345"
  }
}
```

**testdata.json**:

```json
{
  "transfer": {
    "amount": "100.00"
  },
  "billPay": {
    "payeeName": "Electric Company",
    "address": "456 Utility Ave",
    "city": "Service City",
    "state": "SC",
    "zipCode": "67890",
    "phone": "0987654321",
    "accountNumber": "12345",
    "amount": "250.00"
  },
  "loan": {
    "loanAmount": "5000",
    "downPayment": "500"
  },
  "findTransaction": {
    "amount": "100.00"
  }
}
```

> Username đăng ký mới phải thêm **timestamp** (VD: `sel_user_20260323_143022`) để không trùng.

## BƯỚC 4 — 25 TEST SCENARIOS

### A. SMOKE TESTS [Category("Smoke")] — 4 tests

| #   | Test Method                                 | Mô tả                                       |
| --- | ------------------------------------------- | ------------------------------------------- |
| 1   | `HomePage_ShouldLoadSuccessfully`           | Trang chủ ParaBank load được, title đúng    |
| 2   | `Login_WithValidCredentials_ShouldSucceed`  | Login thành công, hiện "Welcome" + username |
| 3   | `Logout_AfterLogin_ShouldReturnToLoginPage` | Logout về trang login                       |
| 4   | `TransferFunds_WithValidData_ShouldSucceed` | Chuyển tiền thành công                      |

### B. GUI TESTS [Category("GUI")] — 6 tests

| #   | Test Method                                       | Mô tả                             |
| --- | ------------------------------------------------- | --------------------------------- |
| 5   | `LoginButton_ShouldBeDisplayedAndClickable`       | Button Login hiển thị, enabled    |
| 6   | `UsernameAndPasswordFields_ShouldAcceptInput`     | Textbox nhập được text            |
| 7   | `RegisterLink_ShouldNavigateToRegistrationPage`   | Link "Register" chuyển đúng trang |
| 8   | `ErrorMessage_ShouldDisplay_WhenLoginFails`       | Thông báo lỗi khi login sai       |
| 9   | `NavigationMenu_ShouldDisplayAllLinks_AfterLogin` | Menu hiện đủ links sau login      |
| 10  | `Logo_ShouldBeVisible_OnAllPages`                 | Logo ParaBank hiển thị            |

### C. FUNCTIONAL TESTS [Category("Functional")] — 15 tests

**Register (3 tests):**

| # | Test Method | Mô tả |
|---|-----------|-------|
| 11 | `Register_WithValidData_ShouldSucceed` | Đăng ký thành công |
| 12 | `Register_WithEmptyFields_ShouldShowErrors` | Để trống → hiện lỗi |
| 13 | `Register_WithMismatchPassword_ShouldShowError` | Password không khớp → lỗi |

**Login (3 tests):**

| # | Test Method | Mô tả |
|---|-----------|-------|
| 14 | `Login_WithInvalidUsername_ShouldFail` | Username sai → lỗi |
| 15 | `Login_WithInvalidPassword_ShouldFail` | Password sai → lỗi |
| 16 | `Login_WithEmptyFields_ShouldShowError` | Để trống → lỗi |

**Open New Account (2 tests):**

| # | Test Method | Mô tả |
|---|-----------|-------|
| 17 | `OpenAccount_Checking_ShouldSucceed` | Mở TK Checking thành công |
| 18 | `OpenAccount_Savings_ShouldSucceed` | Mở TK Savings thành công |

**Transfer Funds (2 tests):**

| # | Test Method | Mô tả |
|---|-----------|-------|
| 19 | `TransferFunds_ValidAmount_ShouldShowConfirmation` | Chuyển tiền → xác nhận |
| 20 | `TransferFunds_BetweenAccounts_ShouldUpdateBalance` | Kiểm tra số dư thay đổi |

**Bill Pay (2 tests):**

| # | Test Method | Mô tả |
|---|-----------|-------|
| 21 | `BillPay_WithValidInfo_ShouldSucceed` | Thanh toán thành công |
| 22 | `BillPay_WithEmptyFields_ShouldShowErrors` | Để trống → lỗi |

**Find Transactions (1 test):**

| # | Test Method | Mô tả |
|---|-----------|-------|
| 23 | `FindTransaction_ByAmount_ShouldReturnResults` | Tìm theo amount |

**Update Contact Info (1 test):**

| # | Test Method | Mô tả |
|---|-----------|-------|
| 24 | `UpdateContactInfo_ShouldSaveSuccessfully` | Cập nhật info thành công |

**Request Loan (1 test):**

| # | Test Method | Mô tả |
|---|-----------|-------|
| 25 | `RequestLoan_WithValidData_ShouldShowResult` | Yêu cầu vay → hiện kết quả |

## BƯỚC 5 — QUY TẮC CODE (BẮT BUỘC)

1. **KHÔNG dùng `Thread.Sleep()`** — chỉ dùng `WebDriverWait` với `ExpectedConditions`
2. **Mỗi test độc lập** — không phụ thuộc thứ tự chạy hay test khác
3. **Tên method rõ ràng**: `Module_Condition_ExpectedResult` (VD: `Login_WithEmptyFields_ShouldShowError`)
4. **NUnit attributes đầy đủ**: `[Test]`, `[Category("Smoke"/"GUI"/"Functional")]`, `[Description("...")]`
5. **Assert rõ ràng**: dùng `Assert.That()` syntax mới của NUnit, message mô tả khi fail
6. **Comment tiếng Việt** giải thích logic từng block code (vì tôi mới học)
7. **Screenshot tự động** khi test fail, lưu vào `Screenshots/`
8. **Try-catch** cho các thao tác có thể fail (element not found, timeout)
9. **Locator priority**: Id > Name > CssSelector > XPath (tuyệt đối hạn chế XPath)
10. **Mỗi test bắt đầu = trạng thái sạch**: login fresh, không reuse session

## BƯỚC 6 — TẠO TÀI LIỆU

### TestPlan.md (1-2 trang)

Gồm:

- Phạm vi test (modules nào)
- Chiến lược: Smoke → GUI → Functional
- Tools: .NET 9, NUnit, Selenium WebDriver, Firefox + GeckoDriver
- Môi trường: Arch Linux, ParaBank URL
- Tiêu chí pass/fail
- Rủi ro: ParaBank server chậm, data reset

### TestScenarios.md

Bảng đầy đủ 25 scenarios ở trên, format:
| # | Module | Test Scenario | Priority | Category | Pre-condition | Steps | Expected Result |

### README.md

Hướng dẫn:

1. Yêu cầu hệ thống (Arch Linux, .NET SDK, Firefox, GeckoDriver)
2. Cách cài dependencies
3. Cách chạy test: `dotnet test`, chạy theo category, chạy 1 test cụ thể
4. Cấu trúc thư mục giải thích
5. Xem kết quả + screenshots

## BƯỚC 7 — VERIFY

Sau khi tạo xong tất cả, chạy:

```bash
# Build project
dotnet build

# Chạy smoke test trước
dotnet test --filter "Category=Smoke" --logger "console;verbosity=detailed"

# Chạy tất cả test
dotnet test --logger "console;verbosity=detailed"
```

Nếu có lỗi build hoặc test fail do bug code → **tự fix cho đến khi build thành công**.
Test fail do ParaBank behavior (server chậm, data không có) → ghi chú trong code, tăng timeout.

## TÓM TẮT THỰC HIỆN

1. ✅ Cài geckodriver (`yay -S geckodriver`)
2. ✅ Tạo dotnet project với NuGet packages
3. ✅ Tạo cấu trúc thư mục đầy đủ
4. ✅ Viết Utilities (DriverFactory, ConfigReader, ScreenshotHelper)
5. ✅ Viết BasePage + 9 Page classes (POM)
6. ✅ Viết BaseTest + 11 Test classes (25 tests)
7. ✅ Tạo TestData JSON files
8. ✅ Tạo TestPlan.md, TestScenarios.md, README.md
9. ✅ Build thành công
10. ✅ Chạy test, fix lỗi nếu có

**BẮT ĐẦU NGAY. Không hỏi lại. Tạo toàn bộ project hoàn chỉnh.**
