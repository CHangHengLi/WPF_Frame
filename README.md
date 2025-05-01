# WPF Frame控件演示程序

这是一个演示WPF Frame控件基本用法的示例应用程序，基于.NET 8.0开发。

## 项目简介

本项目是WPF Frame控件的实际应用示例，展示了如何在WPF应用程序中实现页面导航和页面管理。Frame控件是WPF中用于导航和页面管理的重要容器控件，它提供了在同一窗口内加载和切换不同页面的能力。

## 功能特点

- **基本页面导航**：使用Frame控件实现基本的页面切换
- **导航历史管理**：演示前进和后退操作
- **页面间参数传递**：通过对象传递方式在页面间共享数据
- **菜单高亮**：当前页面对应的菜单项会自动高亮显示

## 项目结构

- **MainWindow.xaml**：主窗口，包含Frame控件和导航菜单
- **Pages目录**：包含所有页面
  - **HomePage.xaml**：首页
  - **ProductsPage.xaml**：产品列表页
  - **ProductDetailPage.xaml**：产品详情页
  - **SettingsPage.xaml**：设置页面
  - **AboutPage.xaml**：关于页面

## 如何运行

1. 确保安装了.NET 8.0 SDK
2. 双击`WPF_Frame.sln`打开解决方案
3. 按F5或点击"开始调试"按钮运行项目

## 实现细节

本示例演示了Frame控件的以下功能：

### 基本导航

```csharp
// 导航到指定页面
MainFrame.Navigate(new Pages.HomePage());
```

### 导航历史管理

```csharp
// 后退
if (MainFrame.CanGoBack)
{
    MainFrame.GoBack();
}

// 前进
if (MainFrame.CanGoForward)
{
    MainFrame.GoForward();
}
```

### 页面间参数传递

```csharp
// 传递参数到另一个页面
ProductDetailPage detailPage = new ProductDetailPage(product);
NavigationService.Navigate(detailPage);
```

## 学习资源

如果您想深入了解WPF Frame控件，可以参考以下资源：

- [Microsoft官方文档 - Frame类](https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.controls.frame)
- [Microsoft官方文档 - WPF中的导航概述](https://docs.microsoft.com/zh-cn/dotnet/framework/wpf/app-development/navigation-overview)

## 贡献

欢迎提交问题和建议，或者通过Pull Request贡献代码。

## 许可证

MIT 