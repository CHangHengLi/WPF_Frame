using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Frame.Pages
{
    /// <summary>
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            
            // 页面加载时检查全局主题设置
            ApplyGlobalThemeSettings();
        }
        
        // 应用全局主题设置
        private void ApplyGlobalThemeSettings()
        {
            // 应用字体大小
            this.FontSize = MainWindow.GlobalFontSize;
            
            // 应用深色/浅色模式
            if (MainWindow.IsDarkMode)
            {
                ApplyDarkMode();
            }
            else
            {
                ApplyLightMode();
            }
        }
        
        // 应用深色模式
        private void ApplyDarkMode()
        {
            // 这里手动设置深色模式样式，确保视觉一致性
            this.Background = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(70, 70, 70));
            this.Foreground = System.Windows.Media.Brushes.White;
        }
        
        // 应用浅色模式
        private void ApplyLightMode()
        {
            // 恢复默认样式
            this.Background = System.Windows.Media.Brushes.White;
            this.Foreground = System.Windows.Media.Brushes.Black;
        }
        
        private void ViewSourceCode_Click(object sender, RoutedEventArgs e)
        {
            // 在实际应用中，这将打开源代码仓库网站
            // 这里仅显示一个消息框作为演示
            MessageBox.Show(
                "源代码可在GitHub上查看。\n\n本演示程序展示了WPF Frame控件的基本用法，包括页面导航、导航历史管理和页面间参数传递等功能。",
                "源代码信息",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
} 