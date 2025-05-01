using System.Windows;
using System.Windows.Controls;

namespace WPF_Frame.Pages
{
    /// <summary>
    /// ProductDetailPage.xaml 的交互逻辑
    /// </summary>
    public partial class ProductDetailPage : Page
    {
        private Product _product;
        
        public ProductDetailPage(Product product)
        {
            InitializeComponent();
            
            _product = product;
            
            // 显示产品信息
            DisplayProductInfo();
            
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
        
        private void DisplayProductInfo()
        {
            // 设置产品详情信息
            productName.Text = _product.Name;
            productPrice.Text = $"¥{_product.Price:N2}";
            productDescription.Text = _product.Description;
            
            // 设置产品ID
            productId.Text = _product.Id.ToString();
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // 返回到上一页
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        
        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"已将 {_product.Name} 添加到购物车！", "操作成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
} 