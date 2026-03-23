# ParaBank Automation Testing (AI GEN)

Dự án kiểm thử tự động website ParaBank sử dụng Selenium WebDriver, NUnit, và C# (.NET 10.0).

## Yêu cầu hệ thống

- **OS**: Linux (đã test trên Arch Linux)
- **.NET SDK**: 10.0 trở lên
- **Firefox**: phiên bản mới nhất
- **GeckoDriver**: 0.36.0+ (cài system-wide)

## Cài đặt

### 1. Cài GeckoDriver (Arch Linux)

```bash
yay -S --noconfirm geckodriver
```

### 2. Verify môi trường

```bash
dotnet --version    # >= 10.0
firefox --version   # Firefox mới nhất
geckodriver --version  # >= 0.36.0
```

### 3. Restore dependencies

```bash
cd ParaBankAutomation
dotnet restore
```

## Chạy test

### Chạy tất cả test (5 tests)

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Chạy theo category

```bash
# Smoke tests (2 tests)
dotnet test --filter "Category=Smoke" --logger "console;verbosity=detailed"

# GUI tests (1 test)
dotnet test --filter "Category=GUI" --logger "console;verbosity=detailed"

# Functional tests (2 tests)
dotnet test --filter "Category=Functional" --logger "console;verbosity=detailed"
```

### Chạy 1 test cụ thể

```bash
dotnet test --filter "FullyQualifiedName~HomePage_ShouldLoadSuccessfully"
```

## Cấu trúc thư mục

```
ParaBankAutomation/
├── Pages/                    # Page Object Model classes
│   ├── BasePage.cs           # Base class — các methods dùng chung
│   ├── LoginPage.cs          # Trang đăng nhập
│   └── RegisterPage.cs       # Trang đăng ký
├── Tests/                    # Test classes
│   ├── BaseTest.cs           # Setup/TearDown chung cho tất cả test
│   ├── SmokeTests.cs         # Smoke tests (2 tests)
│   ├── GUITests.cs           # GUI tests (1 test)
│   ├── LoginTests.cs         # Login tests (1 test)
│   └── RegisterTests.cs      # Register tests (1 test)
├── Utilities/                # Các lớp tiện ích
│   ├── ConfigReader.cs       # Đọc cấu hình từ appsettings.json
│   ├── DriverFactory.cs      # Tạo FirefoxDriver
│   └── ScreenshotHelper.cs   # Chụp ảnh khi test fail
├── TestData/                 # Dữ liệu test
│   └── users.json            # Thông tin user test
├── Screenshots/              # Screenshot tự động khi test fail
├── appsettings.json          # Cấu hình (URL, timeout, headless...)
├── TestPlan.md               # Kế hoạch kiểm thử
├── TestScenarios.md          # 30 test scenarios chi tiết
└── README.md                 # File này
```

## Cấu hình

Chỉnh sửa `appsettings.json`:

| Key | Mô tả | Mặc định |
|-----|--------|----------|
| BaseUrl | URL trang ParaBank | https://parabank.parasoft.com/parabank/index.htm |
| Headless | Chạy không hiện trình duyệt | false |
| ImplicitWaitSeconds | Thời gian chờ ngầm định | 10 |
| ExplicitWaitSeconds | Thời gian chờ tường minh | 15 |

## Xem kết quả

- **Console output**: Kết quả chi tiết hiện trên terminal
- **Screenshots**: Khi test fail, ảnh tự động lưu trong `Screenshots/` với format `{TestName}_{timestamp}.png`

## 5 Automated Tests

| # | Test | Category | Mô tả |
|---|------|----------|-------|
| 1 | HomePage_ShouldLoadSuccessfully | Smoke | Trang chủ load đúng |
| 2 | Login_WithValidCredentials_ShouldSucceed | Smoke | Login thành công |
| 3 | LoginButton_ShouldBeDisplayedAndClickable | GUI | Nút Login hiển thị |
| 4 | Register_WithValidData_ShouldSucceed | Functional | Đăng ký thành công |
| 5 | Login_WithEmptyFields_ShouldShowError | Functional | Login trống → lỗi |

## Công nghệ

- **Design Pattern**: Page Object Model (POM)
- **Wait Strategy**: WebDriverWait + ExpectedConditions (KHÔNG dùng Thread.Sleep)
- **Test Framework**: NUnit 4.x với Assert.That() syntax
- **Screenshot**: Tự động chụp khi test fail
