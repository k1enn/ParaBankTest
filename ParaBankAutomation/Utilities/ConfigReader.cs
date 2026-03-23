using Newtonsoft.Json.Linq;

namespace ParaBankAutomation.Utilities
{
    /// <summary>
    /// Lớp đọc cấu hình từ file appsettings.json
    /// Sử dụng Newtonsoft.Json để parse JSON và cung cấp các property static
    /// </summary>
    public static class ConfigReader
    {
        // Biến lưu trữ config đã đọc (chỉ đọc file 1 lần)
        private static JObject? _config;

        /// <summary>
        /// Đọc file appsettings.json và cache lại kết quả
        /// </summary>
        private static JObject Config
        {
            get
            {
                if (_config == null)
                {
                    // Lấy đường dẫn thư mục chứa assembly đang chạy
                    var basePath = AppDomain.CurrentDomain.BaseDirectory;
                    var configPath = Path.Combine(basePath, "appsettings.json");

                    // Đọc file JSON và parse thành JObject
                    var jsonContent = File.ReadAllText(configPath);
                    _config = JObject.Parse(jsonContent);
                }
                return _config;
            }
        }

        /// <summary>
        /// URL gốc của trang ParaBank
        /// </summary>
        public static string BaseUrl => Config["BaseUrl"]?.ToString()
            ?? "https://parabank.parasoft.com/parabank/index.htm";

        /// <summary>
        /// Loại trình duyệt sử dụng (Firefox)
        /// </summary>
        public static string Browser => Config["Browser"]?.ToString() ?? "Firefox";

        /// <summary>
        /// Chế độ headless (không hiện giao diện trình duyệt)
        /// </summary>
        public static bool Headless => Config["Headless"]?.Value<bool>() ?? false;

        /// <summary>
        /// Thời gian chờ ngầm định (giây) cho mỗi lần tìm element
        /// </summary>
        public static int ImplicitWaitSeconds => Config["ImplicitWaitSeconds"]?.Value<int>() ?? 10;

        /// <summary>
        /// Thời gian chờ tường minh (giây) cho WebDriverWait
        /// </summary>
        public static int ExplicitWaitSeconds => Config["ExplicitWaitSeconds"]?.Value<int>() ?? 15;

        /// <summary>
        /// Đường dẫn thư mục lưu screenshot khi test fail
        /// </summary>
        public static string ScreenshotPath => Config["ScreenshotPath"]?.ToString() ?? "Screenshots/";
    }
}
