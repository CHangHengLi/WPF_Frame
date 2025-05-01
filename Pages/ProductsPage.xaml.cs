using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Frame.Pages
{
    /// <summary>
    /// ProductsPage.xaml 的交互逻辑
    /// </summary>
    public partial class ProductsPage : Page
    {
        private List<Product> _products;
        
        public ProductsPage()
        {
            InitializeComponent();
            
            // 初始化产品数据
            InitializeProducts();
            
            // 绑定数据到ListView
            productsListView.ItemsSource = _products;
            
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
            
            // 设置ListView的背景色
            if (productsListView != null)
            {
                productsListView.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(50, 50, 50));
                productsListView.Foreground = System.Windows.Media.Brushes.White;
            }
        }
        
        // 应用浅色模式
        private void ApplyLightMode()
        {
            // 恢复默认样式
            this.Background = System.Windows.Media.Brushes.White;
            this.Foreground = System.Windows.Media.Brushes.Black;
            
            // 恢复ListView的默认样式
            if (productsListView != null)
            {
                productsListView.Background = System.Windows.Media.Brushes.White;
                productsListView.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
        
        // 初始化产品数据
        private void InitializeProducts()
        {
            _products = new List<Product>
            {
                new Product { Id = 1, Name = "基础版WPF框架", Price = 99.99M, Description = "适合初学者的基础WPF开发框架" },
                new Product { Id = 2, Name = "专业版WPF框架", Price = 199.99M, Description = "提供更多高级功能的专业WPF框架" },
                new Product { Id = 3, Name = "企业版WPF框架", Price = 299.99M, Description = "面向企业级应用的WPF开发框架" },
                new Product { Id = 4, Name = "WPF控件库", Price = 149.99M, Description = "包含多种自定义控件的WPF控件库" },
                new Product { Id = 5, Name = "WPF主题包", Price = 79.99M, Description = "多种精美主题的WPF主题包" }
            };
        }
        
        // 处理产品点击事件
        private void productsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsListView != null && productsListView.SelectedItem != null)
            {
                Product selectedProduct = (Product)productsListView.SelectedItem;
                
                // 导航到产品详情页面，并传递产品参数
                NavigationService.Navigate(new ProductDetailPage(selectedProduct));
            }
        }
    }
    
    // 产品类
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
} 