using OpenQA.Selenium;

namespace ParaBankAutomation.Utilities
{
    /// <summary>
    /// Lớp tiện ích chụp screenshot và hiển thị popup lỗi khi test fail
    /// Popup lỗi được inject bằng JavaScript vào browser trước khi chụp screenshot
    /// </summary>
    public static class ScreenshotHelper
    {
        /// <summary>
        /// Hiển thị popup thông báo lỗi trên browser bằng JavaScript
        /// Popup màu đỏ nổi bật, hiện tên test và nội dung lỗi
        /// </summary>
        /// <param name="driver">WebDriver đang chạy</param>
        /// <param name="testName">Tên test case bị fail</param>
        /// <param name="errorMessage">Nội dung lỗi từ assertion</param>
        public static void ShowErrorPopup(IWebDriver driver, string testName, string errorMessage)
        {
            try
            {
                // Escape ký tự đặc biệt trong message để tránh lỗi JavaScript
                var safeTestName = testName.Replace("'", "\\'").Replace("\n", "\\n");
                var safeError = errorMessage.Replace("'", "\\'").Replace("\n", "\\n");

                // Inject popup HTML/CSS vào trang bằng JavaScript
                var js = (IJavaScriptExecutor)driver;
                js.ExecuteScript(
                    $@"
                    // Tạo overlay mờ phía sau popup
                    var overlay = document.createElement('div');
                    overlay.style.cssText = 'position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(0,0,0,0.5); z-index:99998;';
                    document.body.appendChild(overlay);

                    // Tạo popup thông báo lỗi
                    var popup = document.createElement('div');
                    popup.style.cssText = 'position:fixed; top:50%; left:50%; transform:translate(-50%,-50%); background:#fff; border:3px solid #dc3545; border-radius:12px; padding:30px 40px; z-index:99999; min-width:500px; max-width:700px; box-shadow:0 8px 32px rgba(0,0,0,0.3); font-family:Arial,sans-serif;';

                    // Header đỏ với icon X
                    popup.innerHTML = '<div style=""background:#dc3545; color:#fff; padding:12px 20px; margin:-30px -40px 20px -40px; border-radius:9px 9px 0 0; font-size:18px; font-weight:bold;"">'
                        + '&#10060; TEST FAILED'
                        + '</div>'
                        + '<div style=""margin-bottom:12px;"">'
                        + '<span style=""color:#666; font-size:13px;"">Test Case:</span><br>'
                        + '<span style=""color:#333; font-size:16px; font-weight:bold;"">{safeTestName}</span>'
                        + '</div>'
                        + '<div style=""background:#fff3f3; border-left:4px solid #dc3545; padding:12px 16px; border-radius:4px; margin-top:10px;"">'
                        + '<span style=""color:#666; font-size:13px;"">Error:</span><br>'
                        + '<span style=""color:#dc3545; font-size:14px;"">{safeError}</span>'
                        + '</div>'
                        + '<div style=""text-align:center; margin-top:18px; color:#999; font-size:12px;"">'
                        + 'Screenshot sẽ được lưu tự động'
                        + '</div>';

                    document.body.appendChild(popup);
                "
                );
            }
            catch (Exception ex)
            {
                // Nếu inject popup lỗi -> ghi log, không ảnh hưởng test
                TestContext.Out.WriteLine($"Không thể hiển thị popup lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Chụp screenshot và lưu vào file
        /// </summary>
        /// <param name="driver">WebDriver đang chạy</param>
        /// <param name="testName">Tên test case (dùng làm tên file)</param>
        public static void CaptureScreenshot(IWebDriver driver, string testName)
        {
            try
            {
                // Lấy đường dẫn thư mục lưu screenshot
                var screenshotPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    ConfigReader.ScreenshotPath
                );

                // Tạo thư mục nếu chưa tồn tại
                Directory.CreateDirectory(screenshotPath);

                // Tạo tên file với timestamp để không bị trùng
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"{testName}_{timestamp}.png";
                var filePath = Path.Combine(screenshotPath, fileName);

                // Chụp screenshot — cast driver sang ITakesScreenshot
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                // Lưu file ảnh PNG
                screenshot.SaveAsFile(filePath);

                // Ghi log đường dẫn file screenshot vào output của test
                TestContext.Out.WriteLine($"Screenshot đã lưu tại: {filePath}");
            }
            catch (Exception ex)
            {
                // Nếu chụp screenshot lỗi -> ghi log nhưng KHÔNG throw exception
                // (tránh che mất lỗi gốc của test)
                TestContext.Out.WriteLine($"Không thể chụp screenshot: {ex.Message}");
            }
        }
    }
}
