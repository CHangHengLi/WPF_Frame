using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Linq;  // 添加命名空间以使用Last()方法
using System.Diagnostics; // 添加Debug输出支持

namespace WPF_Frame;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // 公开菜单按钮，使设置页面可以访问并修改它们
    public Button BtnHome => btnHome;
    public Button BtnProducts => btnProducts;
    public Button BtnSettings => btnSettings;
    public Button BtnAbout => btnAbout;
    public Button BtnBack => btnBack;
    public Button BtnForward => btnForward;
    
    // 不需要重新定义MainFrame，因为XAML中已经定义了，且默认可访问
    // public Frame MainFrame => MainFrame; // 这会导致循环引用
    
    // 添加全局主题状态
    private static bool _isDarkMode = false;
    private static int _themeColorIndex = 0; // 默认蓝色
    private static double _globalFontSize = 12;
    
    public static bool IsDarkMode 
    { 
        get { return _isDarkMode; }
        set 
        { 
            if (_isDarkMode != value)
                Debug.WriteLine($"[全局] 深色模式: {_isDarkMode} -> {value}");
            _isDarkMode = value; 
        }
    }
    
    public static int ThemeColorIndex 
    { 
        get { return _themeColorIndex; }
        set 
        { 
            if (_themeColorIndex != value)
                Debug.WriteLine($"[全局] 主题颜色: {_themeColorIndex} -> {value}");
            _themeColorIndex = value; 
        }
    }
    
    public static double GlobalFontSize 
    { 
        get { return _globalFontSize; }
        set 
        { 
            if (_globalFontSize != value)
                Debug.WriteLine($"[全局] 字体大小: {_globalFontSize} -> {value}");
            _globalFontSize = value; 
        }
    }
    
    public MainWindow()
    {
        InitializeComponent();
        
        // 初始导航到首页
        MainFrame.Navigate(new Pages.HomePage());
        
        // 设置当前选中的按钮
        UpdateMenuSelection(btnHome);
        
        // 如果已经设置了深色模式，应用到主窗口
        if (IsDarkMode)
        {
            ApplyDarkModeToMainWindow();
        }
        
        // 启动时应用全局设置
        ApplyGlobalSettings(true);
        
        // 添加主窗口加载完成事件处理，确保所有设置正确应用
        this.Loaded += (s, e) => ApplyGlobalSettings(true);
    }
    
    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string tag)
        {
            switch (tag)
            {
                case "Home":
                    MainFrame.Navigate(new Pages.HomePage());
                    break;
                case "Products":
                    MainFrame.Navigate(new Pages.ProductsPage());
                    break;
                case "Settings":
                    // 导航到设置页面时，确保不会重置设置
                    MainFrame.Navigate(new Pages.SettingsPage());
                    break;
                case "About":
                    MainFrame.Navigate(new Pages.AboutPage());
                    break;
            }
            
            // 更新选中的菜单按钮
            UpdateMenuSelection(button);
        }
    }
    
    private void MainFrame_Navigated(object sender, NavigationEventArgs e)
    {
        // 更新导航按钮状态
        UpdateNavigationButtons();
        
        // 更新选中的菜单按钮
        if (e.Content is Page page)
        {
            // 按页面类型选择对应按钮
            if (page is Pages.HomePage) UpdateMenuSelection(btnHome);
            else if (page is Pages.ProductsPage) UpdateMenuSelection(btnProducts);
            else if (page is Pages.SettingsPage) UpdateMenuSelection(btnSettings);
            else if (page is Pages.AboutPage) UpdateMenuSelection(btnAbout);
            
            // 应用字体大小
            page.FontSize = GlobalFontSize;
            
            // 如果当前页面是设置页面，不执行额外操作
            // 否则，应用适当的样式但不重置设置
            if (!(page is Pages.SettingsPage))
            {
                // 对于非设置页面，应用当前设置的样式，但不重置值
                
                // 应用深色模式到页面
                if (IsDarkMode)
                {
                    ApplyDarkModeToPage(page);
                }
                else
                {
                    ApplyLightModeToPage(page);
                }
                
                // 应用主题颜色到选中按钮
                ApplyThemeColorToSelectedButton();
            }
        }
    }
    
    // 添加一个专门为主窗口应用深色模式的方法
    public void ApplyDarkModeToMainWindow()
    {
        var darkBackground = new SolidColorBrush(Color.FromRgb(50, 50, 50));
        var darkBackgroundLighter = new SolidColorBrush(Color.FromRgb(70, 70, 70));
        
        // 主窗口背景
        this.Background = darkBackground;
        this.Foreground = Brushes.White;
        
        // 左侧导航区域背景 - 直接设置XAML中定义的控件
        StackPanel leftPanel = (StackPanel)this.FindName("StackPanel");
        if (leftPanel != null)
        {
            leftPanel.Background = darkBackgroundLighter;
        }
        else
        {
            // 如果无法直接通过名称找到，则尝试通过Grid.Column属性查找
            foreach (var stackPanel in FindVisualChildren<StackPanel>(this))
            {
                if (Grid.GetColumn(stackPanel) == 0)
                {
                    stackPanel.Background = darkBackgroundLighter;
                }
            }
        }
        
        // 更新所有按钮的样式
        Button selectedButton = GetSelectedButton();
        foreach (var btn in new[] { btnHome, btnProducts, btnSettings, btnAbout })
        {
            if (btn != selectedButton) btn.Background = darkBackground;
            btn.Foreground = Brushes.White;
        }
        
        // 导航控制按钮
        btnBack.Background = darkBackground;
        btnForward.Background = darkBackground;
        btnBack.Foreground = Brushes.White;
        btnForward.Foreground = Brushes.White;
        
        // 文本块
        foreach (var textBlock in FindVisualChildren<TextBlock>(this))
        {
            if (Grid.GetColumn(textBlock) == 0 || Grid.GetColumn(textBlock) == -1)
            {
                textBlock.Foreground = Brushes.White;
            }
        }
        
        // 分隔符
        foreach (var separator in FindVisualChildren<Separator>(this))
        {
            separator.Background = Brushes.Gray;
        }
        
        // Frame背景
        MainFrame.Background = darkBackgroundLighter;
        
        // 立即应用到当前页面
        if (MainFrame.Content is Page currentPage)
        {
            ApplyDarkModeToPage(currentPage);
        }
    }
    
    // 添加一个专门为主窗口应用浅色模式的方法
    public void ApplyLightModeToMainWindow()
    {
        // 主窗口背景
        this.Background = Brushes.White;
        this.Foreground = Brushes.Black;
        
        // 左侧导航区域背景
        foreach (var stackPanel in FindVisualChildren<StackPanel>(this))
        {
            // 找到左侧导航区域的StackPanel
            if (Grid.GetColumn(stackPanel) == 0)
            {
                stackPanel.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)); // #f0f0f0
            }
        }
        
        // 更新按钮样式
        Button selectedButton = GetSelectedButton();
        foreach (var btn in new[] { btnHome, btnProducts, btnSettings, btnAbout })
        {
            if (btn != selectedButton) btn.Background = Brushes.Transparent;
            btn.Foreground = Brushes.Black;
        }
        
        // 导航控制按钮
        btnBack.Background = Brushes.White;
        btnForward.Background = Brushes.White;
        btnBack.Foreground = Brushes.Black;
        btnForward.Foreground = Brushes.Black;
        
        // 文本块
        foreach (var textBlock in FindVisualChildren<TextBlock>(this))
        {
            if (Grid.GetColumn(textBlock) == 0 || Grid.GetColumn(textBlock) == -1)
            {
                textBlock.Foreground = Brushes.Black;
            }
        }
        
        // 分隔符
        foreach (var separator in FindVisualChildren<Separator>(this))
        {
            separator.Background = Brushes.LightGray;
        }
        
        // Frame背景
        MainFrame.Background = Brushes.White;
        
        // 立即应用到当前页面
        if (MainFrame.Content is Page currentPage)
        {
            ApplyLightModeToPage(currentPage);
        }
    }
    
    // 将深色模式应用到页面
    private void ApplyDarkModeToPage(Page page)
    {
        var darkBackground = new SolidColorBrush(Color.FromRgb(50, 50, 50));
        var darkBackgroundLighter = new SolidColorBrush(Color.FromRgb(70, 70, 70));
        
        // 设置页面本身的背景和前景
        page.Background = darkBackgroundLighter;
        page.Foreground = Brushes.White;
        
        // 确保更新MainFrame的背景色
        MainFrame.Background = darkBackgroundLighter;
        
        // 设置根元素为深色背景（如果存在）
        if (page.Content is FrameworkElement rootElement)
        {
            // 对直接子元素应用深色背景
            if (rootElement is Panel panel)
            {
                panel.Background = darkBackgroundLighter;
            }
            else if (rootElement is ScrollViewer scrollViewer)
            {
                scrollViewer.Background = darkBackgroundLighter;
                
                // 如果ScrollViewer的内容是Panel，也应用深色背景
                if (scrollViewer.Content is Panel scrollContent)
                {
                    scrollContent.Background = darkBackgroundLighter;
                }
            }
        }
        
        ApplyBrushesToControls(page, true, darkBackgroundLighter, darkBackground);
    }
    
    // 将浅色模式应用到页面
    private void ApplyLightModeToPage(Page page)
    {
        // 设置页面本身的背景和前景
        page.Background = Brushes.White;
        page.Foreground = Brushes.Black;
        
        // 确保更新MainFrame的背景色
        MainFrame.Background = Brushes.White;
        
        // 设置根元素为浅色背景（如果存在）
        if (page.Content is FrameworkElement rootElement)
        {
            // 对直接子元素应用浅色背景
            if (rootElement is Panel panel)
            {
                panel.Background = Brushes.White;
            }
            else if (rootElement is ScrollViewer scrollViewer)
            {
                scrollViewer.Background = Brushes.White;
                
                // 如果ScrollViewer的内容是Panel，也应用浅色背景
                if (scrollViewer.Content is Panel scrollContent)
                {
                    scrollContent.Background = Brushes.White;
                }
            }
        }
        
        ApplyBrushesToControls(page, false, Brushes.White, Brushes.Transparent);
    }
    
    // 统一处理控件样式应用
    private void ApplyBrushesToControls(DependencyObject container, bool isDarkMode, Brush mainBackground, Brush secondaryBackground)
    {
        var foreground = isDarkMode ? Brushes.White : Brushes.Black;
        var borderBrush = new SolidColorBrush(isDarkMode ? Color.FromRgb(100, 100, 100) : Color.FromRgb(200, 200, 200));
        
        // 递归处理所有视觉元素
        foreach (var grid in FindVisualChildren<Grid>(container))
        {
            grid.Background = mainBackground;
        }
        
        foreach (var scrollViewer in FindVisualChildren<ScrollViewer>(container))
        {
            scrollViewer.Background = mainBackground;
        }
        
        foreach (var stackPanel in FindVisualChildren<StackPanel>(container))
        {
            stackPanel.Background = isDarkMode ? mainBackground : Brushes.Transparent;
        }
        
        foreach (var border in FindVisualChildren<Border>(container))
        {
            border.Background = secondaryBackground;
        }
        
        foreach (var groupBox in FindVisualChildren<GroupBox>(container))
        {
            groupBox.Background = secondaryBackground;
            groupBox.Foreground = foreground;
        }
        
        foreach (var textBlock in FindVisualChildren<TextBlock>(container))
        {
            textBlock.Foreground = foreground;
        }
        
        foreach (var checkBox in FindVisualChildren<CheckBox>(container))
        {
            checkBox.Foreground = foreground;
        }
        
        foreach (var radioButton in FindVisualChildren<RadioButton>(container))
        {
            radioButton.Foreground = foreground;
        }
        
        foreach (var comboBox in FindVisualChildren<ComboBox>(container))
        {
            comboBox.Background = secondaryBackground;
            comboBox.Foreground = foreground;
            comboBox.BorderBrush = borderBrush;
        }
        
        foreach (var slider in FindVisualChildren<Slider>(container))
        {
            slider.Background = secondaryBackground;
            slider.Foreground = foreground;
        }
        
        foreach (var button in FindVisualChildren<Button>(container))
        {
            // 不处理已有特殊颜色的按钮
            if (button.Background == Brushes.Transparent || 
                button.Background == Brushes.LightBlue ||
                (button.Background is SolidColorBrush brush && 
                 (brush.Color == Color.FromRgb(33, 150, 243) || // 蓝色
                  brush.Color == Color.FromRgb(76, 175, 80) || // 绿色
                  brush.Color == Color.FromRgb(156, 39, 176) || // 紫色
                  brush.Color == Color.FromRgb(255, 152, 0) || // 橙色
                  brush.Color == Color.FromRgb(0, 120, 215)))) // 亮蓝色 (#0078D7)
            {
                continue;
            }
            
            button.Background = secondaryBackground;
            button.Foreground = foreground;
            button.BorderBrush = borderBrush;
        }
    }
    
    // 查找视觉树中的所有指定类型控件
    private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) 
        where T : DependencyObject
    {
        if (depObj != null)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }

                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }
    
    private void UpdateNavigationButtons()
    {
        // 更新前进/后退按钮状态
        btnBack.IsEnabled = MainFrame.CanGoBack;
        btnForward.IsEnabled = MainFrame.CanGoForward;
    }
    
    private void UpdateMenuSelection(Button selectedButton)
    {
        // 重置所有按钮样式
        SolidColorBrush defaultBackground = IsDarkMode ? 
            new SolidColorBrush(Color.FromRgb(50, 50, 50)) : 
            Brushes.Transparent;
            
        btnHome.Background = defaultBackground;
        btnProducts.Background = defaultBackground;
        btnSettings.Background = defaultBackground;
        btnAbout.Background = defaultBackground;
        
        // 设置选中按钮样式
        if (selectedButton != null)
        {
            selectedButton.Background = Brushes.LightBlue;
        }
    }
    
    // 用于更新选中按钮的主题颜色
    public void UpdateSelectedButtonWithThemeColor(Brush accentBrush)
    {
        Button selectedButton = GetSelectedButton();
        if (selectedButton != null)
        {
            // 清除所有按钮的背景色
            SolidColorBrush defaultBackground = IsDarkMode ? 
                new SolidColorBrush(Color.FromRgb(50, 50, 50)) : 
                Brushes.Transparent;
            
            // 设置未选中按钮的背景
            foreach (var btn in new[] { btnHome, btnProducts, btnSettings, btnAbout })
            {
                if (btn != selectedButton) btn.Background = defaultBackground;
            }
            
            // 设置选中按钮的特殊样式
            selectedButton.Background = accentBrush;
            selectedButton.Foreground = Brushes.White;
        }
    }
    
    // 辅助方法，获取当前选中的按钮
    private Button GetSelectedButton()
    {
        if (MainFrame.Content is Pages.HomePage)
            return btnHome;
        else if (MainFrame.Content is Pages.ProductsPage)
            return btnProducts;
        else if (MainFrame.Content is Pages.SettingsPage)
            return btnSettings;
        else if (MainFrame.Content is Pages.AboutPage)
            return btnAbout;
        
        return null;
    }
    
    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
        if (MainFrame.CanGoBack)
        {
            MainFrame.GoBack();
        }
    }
    
    private void btnForward_Click(object sender, RoutedEventArgs e)
    {
        if (MainFrame.CanGoForward)
        {
            MainFrame.GoForward();
        }
    }
    
    // 添加应用主题颜色到选中按钮的方法
    private void ApplyThemeColorToSelectedButton()
    {
        Button selectedButton = GetSelectedButton();
        if (selectedButton != null)
        {
            // 根据当前主题颜色索引获取正确的颜色
            Brush accentBrush;
            switch (ThemeColorIndex)
            {
                case 1: // 绿色
                    accentBrush = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                    break;
                case 2: // 紫色
                    accentBrush = new SolidColorBrush(Color.FromRgb(156, 39, 176));
                    break;
                case 3: // 橙色
                    accentBrush = new SolidColorBrush(Color.FromRgb(255, 152, 0));
                    break;
                default: // 蓝色
                    accentBrush = new SolidColorBrush(Color.FromRgb(33, 150, 243));
                    break;
            }
            
            selectedButton.Background = accentBrush;
            selectedButton.Foreground = Brushes.White;
        }
    }
    
    // 添加应用全局设置的方法
    private void ApplyGlobalSettings(bool resetThemeSettings = true)
    {
        // 应用深色模式设置
        if (IsDarkMode)
        {
            ApplyDarkModeToMainWindow();
        }
        else
        {
            ApplyLightModeToMainWindow();
        }
        
        // 应用字体大小
        this.FontSize = GlobalFontSize;
        
        // 应用主题颜色到相关UI元素
        ApplyThemeColorToSelectedButton();
        
        // 如果当前页面已经打开，重新应用主题设置
        if (MainFrame.Content is Page currentPage)
        {
            if (IsDarkMode)
            {
                ApplyDarkModeToPage(currentPage);
            }
            else
            {
                ApplyLightModeToPage(currentPage);
            }
            
            // 应用字体大小
            currentPage.FontSize = GlobalFontSize;
        }
    }
}