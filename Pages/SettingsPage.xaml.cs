using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.Linq;

namespace WPF_Frame.Pages
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : Page
    {
        // 默认设置
        private readonly bool _defaultDarkMode = false;
        private readonly int _defaultThemeColorIndex = 0; // 蓝色
        private readonly double _defaultFontSize = 12;
        private readonly bool _defaultAutoUpdate = true;
        private readonly bool _defaultEnableNotifications = true;
        private readonly bool _defaultRememberLogin = false;
        private readonly int _defaultCacheSizeIndex = 1; // 500MB
        private readonly int _defaultCacheStrategy = 0; // 缓存所有页面

        // 主窗口引用
        private MainWindow _mainWindow;
        
        // 添加初始化标志，防止在加载过程中触发设置保存
        private bool _isInitializing = false;
        
        // 静态设置存储，保证在页面重新加载时保持设置
        private static class AppSettings
        {
            // 初始值不重要，会在每次打开设置页面时从MainWindow更新
            private static bool _darkMode = false;
            private static int _themeColorIndex = 0;
            private static double _fontSize = 12;
            
            public static bool DarkMode 
            { 
                get { return _darkMode; }
                set 
                { 
                    if (_darkMode != value)
                        Debug.WriteLine($"[设置] AppSettings.DarkMode: {_darkMode} -> {value}");
                    _darkMode = value; 
                }
            }
            
            public static int ThemeColorIndex 
            { 
                get { return _themeColorIndex; }
                set 
                { 
                    if (_themeColorIndex != value)
                        Debug.WriteLine($"[设置] AppSettings.ThemeColorIndex: {_themeColorIndex} -> {value}");
                    _themeColorIndex = value; 
                }
            }
            
            public static double FontSize 
            { 
                get { return _fontSize; }
                set 
                { 
                    if (_fontSize != value)
                        Debug.WriteLine($"[设置] AppSettings.FontSize: {_fontSize} -> {value}");
                    _fontSize = value; 
                }
            }
            
            public static bool AutoUpdate { get; set; } = true;
            public static bool EnableNotifications { get; set; } = true;
            public static bool RememberLogin { get; set; } = false;
            public static int CacheSizeIndex { get; set; } = 1;
            public static int CacheStrategy { get; set; } = 0;
            public static bool IsInitialized { get; set; } = false;
        }
        
        public SettingsPage()
        {
            InitializeComponent();
            
            // 获取主窗口引用
            _mainWindow = Application.Current.MainWindow as MainWindow;
            
            Debug.WriteLine($"[设置] 构造函数: 当前全局设置 - 深色模式={MainWindow.IsDarkMode}, 主题颜色={MainWindow.ThemeColorIndex}, 字体大小={MainWindow.GlobalFontSize}");
            
            // 设置控件事件处理
            SetupEventHandlers();
            
            // 加载保存的设置
            LoadSettings();
        }
        
        private void SetupEventHandlers()
        {
            // 为设置项添加变更事件处理，以便实时应用更改
            chkDarkMode.Checked += (s, e) => 
            {
                if (_isInitializing) return;
                SaveCurrentSettingsToStatic();
                ApplyDarkMode(true);
            };
            
            chkDarkMode.Unchecked += (s, e) => 
            {
                if (_isInitializing) return;
                SaveCurrentSettingsToStatic();
                ApplyDarkMode(false);
            };
            
            cmbThemeColor.SelectionChanged += (s, e) => 
            {
                if (cmbThemeColor.SelectedIndex >= 0 && !_isInitializing)
                {
                    SaveCurrentSettingsToStatic();
                    ApplyThemeColor();
                }
            };
            
            sldFontSize.ValueChanged += (s, e) => 
            {
                if (_isInitializing) return;
                SaveCurrentSettingsToStatic();
                ApplyFontSize();
            };
            
            // 为其他设置项添加变更事件，以便立即保存
            // 使用统一的事件处理方法
            RoutedEventHandler simpleSettingChanged = (s, e) => 
            { 
                if (!_isInitializing) SaveCurrentSettingsToStatic(); 
            };
            
            chkAutoUpdate.Checked += simpleSettingChanged;
            chkAutoUpdate.Unchecked += simpleSettingChanged;
            
            chkEnableNotifications.Checked += simpleSettingChanged;
            chkEnableNotifications.Unchecked += simpleSettingChanged;
            
            chkRememberLogin.Checked += simpleSettingChanged;
            chkRememberLogin.Unchecked += simpleSettingChanged;
            
            cmbCacheSize.SelectionChanged += (s, e) => simpleSettingChanged(s, e);
            
            rbCacheAll.Checked += simpleSettingChanged;
            rbCacheCommon.Checked += simpleSettingChanged;
            rbCacheNone.Checked += simpleSettingChanged;
        }
        
        private void LoadSettings()
        {
            // 设置初始化标志，防止在控件设置值时触发不必要的保存操作
            _isInitializing = true;
            
            Debug.WriteLine($"[设置] 加载设置: 从MainWindow同步设置");
            
            // 每次打开设置页面时，始终从MainWindow获取当前的全局设置值
            // 这确保了无论何时打开设置页面，都会显示当前系统的实际设置
            AppSettings.DarkMode = MainWindow.IsDarkMode;
            AppSettings.ThemeColorIndex = MainWindow.ThemeColorIndex;
            AppSettings.FontSize = MainWindow.GlobalFontSize;
            
            if (!AppSettings.IsInitialized)
            {
                AppSettings.IsInitialized = true;
            }
            
            // 更新UI控件以反映当前设置 - 先禁用事件处理，以避免触发保存
            // 顺序很重要，先设置字体大小，再设置主题颜色，最后设置深色模式
            sldFontSize.Value = AppSettings.FontSize;
            cmbThemeColor.SelectedIndex = AppSettings.ThemeColorIndex;
            chkDarkMode.IsChecked = AppSettings.DarkMode;
            
            // 设置其他UI控件值
            chkAutoUpdate.IsChecked = AppSettings.AutoUpdate;
            chkEnableNotifications.IsChecked = AppSettings.EnableNotifications;
            chkRememberLogin.IsChecked = AppSettings.RememberLogin;
            cmbCacheSize.SelectedIndex = AppSettings.CacheSizeIndex;
            
            switch (AppSettings.CacheStrategy)
            {
                case 0:
                    rbCacheAll.IsChecked = true;
                    break;
                case 1:
                    rbCacheCommon.IsChecked = true;
                    break;
                case 2:
                    rbCacheNone.IsChecked = true;
                    break;
            }
                
            // 应用初始设置
            ApplyAllSettings();
            
            // 初始化完成，允许事件触发
            _isInitializing = false;
        }
        
        private void SaveCurrentSettingsToStatic()
        {
            // 将当前设置保存到静态存储
            AppSettings.DarkMode = chkDarkMode.IsChecked ?? false;
            AppSettings.ThemeColorIndex = cmbThemeColor.SelectedIndex;
            AppSettings.FontSize = sldFontSize.Value;  // 明确保存字体大小
            AppSettings.AutoUpdate = chkAutoUpdate.IsChecked ?? true;
            AppSettings.EnableNotifications = chkEnableNotifications.IsChecked ?? true;
            AppSettings.RememberLogin = chkRememberLogin.IsChecked ?? false;
            AppSettings.CacheSizeIndex = cmbCacheSize.SelectedIndex;
            
            if (rbCacheAll.IsChecked == true) AppSettings.CacheStrategy = 0;
            else if (rbCacheCommon.IsChecked == true) AppSettings.CacheStrategy = 1;
            else if (rbCacheNone.IsChecked == true) AppSettings.CacheStrategy = 2;
            
            // 同时更新MainWindow中的全局主题设置
            MainWindow.IsDarkMode = AppSettings.DarkMode;
            MainWindow.ThemeColorIndex = AppSettings.ThemeColorIndex;
            MainWindow.GlobalFontSize = AppSettings.FontSize;  // 确保更新全局字体大小
        }
        
        private void ApplyAllSettings()
        {
            ApplyDarkMode(chkDarkMode.IsChecked == true);
            ApplyThemeColor();
            ApplyFontSize();
        }
        
        private void ApplyDarkMode(bool isDarkMode)
        {
            if (_mainWindow == null) return;
            
            // 更新全局主题状态
            MainWindow.IsDarkMode = isDarkMode;
            
            // 应用深色模式到主窗口
            if (isDarkMode)
            {
                _mainWindow.ApplyDarkModeToMainWindow();
                ApplyThemeColors(true);
            }
            else
            {
                // 浅色模式
                _mainWindow.ApplyLightModeToMainWindow();
                ApplyThemeColors(false);
            }
        }
        
        private void ApplyThemeColors(bool isDarkMode)
        {
            var darkBackground = new SolidColorBrush(Color.FromRgb(70, 70, 70));
            var darkForeground = Brushes.White;
            var lightBackground = Brushes.White;
            var lightForeground = Brushes.Black;
            
            // 设置页面本身的背景和前景
            this.Background = isDarkMode ? darkBackground : lightBackground;
            this.Foreground = isDarkMode ? darkForeground : lightForeground;
            
            // 更新控件样式
            foreach (var grid in FindVisualChildren<Grid>(this))
            {
                grid.Background = isDarkMode ? darkBackground : lightBackground;
            }
            
            foreach (var scrollViewer in FindVisualChildren<ScrollViewer>(this))
            {
                scrollViewer.Background = isDarkMode ? darkBackground : lightBackground;
            }
            
            foreach (var stackPanel in FindVisualChildren<StackPanel>(this))
            {
                stackPanel.Background = isDarkMode ? darkBackground : Brushes.Transparent;
            }
            
            foreach (var textBlock in FindVisualChildren<TextBlock>(this))
            {
                textBlock.Foreground = isDarkMode ? darkForeground : lightForeground;
            }
            
            foreach (var checkBox in FindVisualChildren<CheckBox>(this))
            {
                checkBox.Foreground = isDarkMode ? darkForeground : lightForeground;
            }
            
            foreach (var radioButton in FindVisualChildren<RadioButton>(this))
            {
                radioButton.Foreground = isDarkMode ? darkForeground : lightForeground;
            }
            
            foreach (var comboBox in FindVisualChildren<ComboBox>(this))
            {
                comboBox.Background = isDarkMode ? new SolidColorBrush(Color.FromRgb(50, 50, 50)) : lightBackground;
                comboBox.Foreground = isDarkMode ? darkForeground : lightForeground;
                comboBox.BorderBrush = new SolidColorBrush(isDarkMode ? Color.FromRgb(100, 100, 100) : Color.FromRgb(200, 200, 200));
            }
            
            foreach (var slider in FindVisualChildren<Slider>(this))
            {
                slider.Background = isDarkMode ? new SolidColorBrush(Color.FromRgb(50, 50, 50)) : lightBackground;
                slider.Foreground = isDarkMode ? darkForeground : lightForeground;
            }
        }
        
        private void ApplyThemeColor()
        {
            if (_mainWindow == null || cmbThemeColor.SelectedItem is not ComboBoxItem selectedItem) return;
            
            // 更新全局主题颜色索引
            MainWindow.ThemeColorIndex = cmbThemeColor.SelectedIndex;
            
            // 获取所选的主题颜色
            string colorName = selectedItem.Content.ToString();
            Brush accentBrush;
            
            // 根据选择设置主题色
            switch (colorName)
            {
                case "绿色":
                    accentBrush = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                    break;
                case "紫色":
                    accentBrush = new SolidColorBrush(Color.FromRgb(156, 39, 176));
                    break;
                case "橙色":
                    accentBrush = new SolidColorBrush(Color.FromRgb(255, 152, 0));
                    break;
                default: // 蓝色
                    accentBrush = new SolidColorBrush(Color.FromRgb(33, 150, 243));
                    break;
            }
            
            // 应用主题色到所有导航按钮
            if (_mainWindow.BtnHome != null) _mainWindow.BtnHome.Background = accentBrush;
            if (_mainWindow.BtnProducts != null) _mainWindow.BtnProducts.Background = accentBrush;
            if (_mainWindow.BtnSettings != null) _mainWindow.BtnSettings.Background = accentBrush;
            if (_mainWindow.BtnAbout != null) _mainWindow.BtnAbout.Background = accentBrush;
            
            // 使用MainWindow中的方法更新选定按钮的颜色
            _mainWindow.UpdateSelectedButtonWithThemeColor(accentBrush);
            
            // 应用主题色到设置页面的按钮
            Button[] themeButtons = FindVisualChildren<Button>(this)
                .Where(b => b != btnReset && b.Background != Brushes.Transparent)
                .ToArray();
                
            foreach (var button in themeButtons)
            {
                if (MainWindow.IsDarkMode)
                {
                    // 深色模式下，非特殊按钮使用主题色
                    button.Background = accentBrush;
                    button.Foreground = Brushes.White;
                }
                else
                {
                    // 浅色模式下，保持默认样式或应用淡化的主题色
                    var lightAccentBrush = new SolidColorBrush(Color.FromArgb(100, 
                        ((SolidColorBrush)accentBrush).Color.R,
                        ((SolidColorBrush)accentBrush).Color.G,
                        ((SolidColorBrush)accentBrush).Color.B));
                    button.Background = lightAccentBrush;
                    button.Foreground = Brushes.Black;
                }
            }
            
            // 单独处理重置按钮，给它一个特殊样式
            if (btnReset != null)
            {
                if (MainWindow.IsDarkMode)
                {
                    btnReset.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
                    btnReset.BorderBrush = accentBrush;
                    btnReset.Foreground = Brushes.White;
                }
                else
                {
                    btnReset.Background = Brushes.White;
                    btnReset.BorderBrush = accentBrush;
                    btnReset.Foreground = Brushes.Black;
                }
            }
        }
        
        private void ApplyFontSize()
        {
            if (_mainWindow == null) return;
            
            // 应用字体大小
            double fontSize = sldFontSize.Value;
            
            // 更新全局字体大小
            MainWindow.GlobalFontSize = fontSize;
            
            // 立即保存到静态存储，确保字体大小设置持久化
            AppSettings.FontSize = fontSize;
            
            // 更新主窗口和各个按钮的字体大小
            _mainWindow.FontSize = fontSize;
            
            // 更新所有可见页面的字体大小
            if (_mainWindow.MainFrame.Content is Page currentPage)
            {
                currentPage.FontSize = fontSize;
            }
        }
        
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            // 设置初始化标志，防止在控件设置值时触发不必要的保存操作
            _isInitializing = true;
            
            // 将所有设置重置为默认值
            chkDarkMode.IsChecked = _defaultDarkMode;
            cmbThemeColor.SelectedIndex = _defaultThemeColorIndex;
            sldFontSize.Value = _defaultFontSize;
            chkAutoUpdate.IsChecked = _defaultAutoUpdate;
            chkEnableNotifications.IsChecked = _defaultEnableNotifications;
            chkRememberLogin.IsChecked = _defaultRememberLogin;
            cmbCacheSize.SelectedIndex = _defaultCacheSizeIndex;
            
            switch (_defaultCacheStrategy)
            {
                case 0:
                    rbCacheAll.IsChecked = true;
                    break;
                case 1:
                    rbCacheCommon.IsChecked = true;
                    break;
                case 2:
                    rbCacheNone.IsChecked = true;
                    break;
            }
            
            // 初始化完成，允许事件触发
            _isInitializing = false;
            
            // 保存到静态存储
            SaveCurrentSettingsToStatic();
            
            // 应用设置
            ApplyAllSettings();
        }

        private void ApplyDarkModeToFrameContent(Frame frame, Brush lightDarkBrush, Brush darkBrush)
        {
            if (frame.Content is DependencyObject content)
            {
                ApplyBrushesToContent(content, true, lightDarkBrush, darkBrush);
            }
        }

        private void ApplyLightModeToFrameContent(Frame frame)
        {
            if (frame.Content is DependencyObject content)
            {
                ApplyBrushesToContent(content, false, Brushes.White, Brushes.Transparent);
            }
        }
        
        // 统一处理内容的样式应用
        private void ApplyBrushesToContent(DependencyObject content, bool isDarkMode, Brush background, Brush secondary)
        {
            var foreground = isDarkMode ? Brushes.White : Brushes.Black;
            var borderBrush = new SolidColorBrush(isDarkMode ? Color.FromRgb(100, 100, 100) : Color.FromRgb(200, 200, 200));
            
            foreach (var grid in FindVisualChildren<Grid>(content))
            {
                grid.Background = background;
            }
            
            foreach (var textBlock in FindVisualChildren<TextBlock>(content))
            {
                textBlock.Foreground = foreground;
            }
            
            foreach (var button in FindVisualChildren<Button>(content))
            {
                if (button.Background != Brushes.Transparent)
                {
                    button.Background = secondary;
                    button.Foreground = foreground;
                    button.BorderBrush = borderBrush;
                }
            }
            
            foreach (var stackPanel in FindVisualChildren<StackPanel>(content))
            {
                stackPanel.Background = background;
            }
            
            foreach (var border in FindVisualChildren<Border>(content))
            {
                border.Background = secondary;
            }
            
            foreach (var checkBox in FindVisualChildren<CheckBox>(content))
            {
                checkBox.Foreground = foreground;
            }
            
            foreach (var radioButton in FindVisualChildren<RadioButton>(content))
            {
                radioButton.Foreground = foreground;
            }
            
            foreach (var comboBox in FindVisualChildren<ComboBox>(content))
            {
                comboBox.Background = secondary;
                comboBox.Foreground = foreground;
                comboBox.BorderBrush = borderBrush;
            }
        }
        
        // 查找视觉树中的所有指定类型控件
        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
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
    }
} 