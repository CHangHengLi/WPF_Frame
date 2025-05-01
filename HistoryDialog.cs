using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WPF_Frame
{
    /// <summary>
    /// 导航历史对话框
    /// </summary>
    public class HistoryDialog : Window
    {
        private Frame _frame;
        private ListBox _historyListBox;
        private Button _previewBackButton;
        private Button _previewForwardButton;
        private Button _resetPreviewButton;
        private TextBlock _previewInfoBlock;
        
        // 是否处于预览模式
        private bool _isInPreviewMode = false;
        // 预览前的导航状态，用于还原
        private object _originalContent;
        
        public HistoryDialog(Frame frame)
        {
            _frame = frame;
            
            // 保存原始内容
            _originalContent = _frame.Content;
            
            // 设置窗口属性
            this.Title = "导航历史";
            this.Width = 500;
            this.Height = 450;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            // 创建主布局
            Grid mainGrid = new Grid();
            mainGrid.Margin = new Thickness(10);
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            // 创建标题
            TextBlock titleBlock = new TextBlock
            {
                Text = "导航历史记录",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(titleBlock, 0);
            
            // 创建列表框
            _historyListBox = new ListBox
            {
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(_historyListBox, 1);
            
            // 创建预览信息文本块
            _previewInfoBlock = new TextBlock
            {
                Text = "预览模式：关闭",
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(0, 5, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(_previewInfoBlock, 2);
            
            // 创建预览控制区域
            StackPanel previewControlPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            };
            
            // 创建预览后退按钮
            _previewBackButton = new Button
            {
                Content = "预览后退",
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(5),
                IsEnabled = _frame.CanGoBack
            };
            _previewBackButton.Click += PreviewBackButton_Click;
            
            // 创建预览前进按钮
            _previewForwardButton = new Button
            {
                Content = "预览前进",
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(5),
                IsEnabled = _frame.CanGoForward
            };
            _previewForwardButton.Click += PreviewForwardButton_Click;
            
            // 创建重置预览按钮
            _resetPreviewButton = new Button
            {
                Content = "重置预览",
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(5),
                IsEnabled = false
            };
            _resetPreviewButton.Click += ResetPreviewButton_Click;
            
            // 添加按钮到预览控制面板
            previewControlPanel.Children.Add(_previewBackButton);
            previewControlPanel.Children.Add(_previewForwardButton);
            previewControlPanel.Children.Add(_resetPreviewButton);
            
            Grid.SetRow(previewControlPanel, 3);
            
            // 创建关闭按钮
            Button closeButton = new Button
            {
                Content = "关闭",
                Padding = new Thickness(20, 5, 20, 5),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            closeButton.Click += CloseButton_Click;
            Grid.SetRow(closeButton, 4);
            
            // 添加控件到布局
            mainGrid.Children.Add(titleBlock);
            mainGrid.Children.Add(_historyListBox);
            mainGrid.Children.Add(_previewInfoBlock);
            mainGrid.Children.Add(previewControlPanel);
            mainGrid.Children.Add(closeButton);
            
            // 设置窗口内容
            this.Content = mainGrid;
            
            // 加载历史记录
            LoadNavigationHistory();
            
            // 窗口关闭事件
            this.Closed += HistoryDialog_Closed;
        }
        
        private void LoadNavigationHistory()
        {
            if (_frame == null) return;
            
            List<string> historyItems = new List<string>();
            
            // 添加当前页面
            if (_frame.Content is Page currentPage)
            {
                historyItems.Add($"当前页面: {currentPage.GetType().Name}");
                
                // 如果页面有Title属性，也显示它
                if (!string.IsNullOrEmpty(currentPage.Title))
                {
                    historyItems.Add($"页面标题: {currentPage.Title}");
                }
            }
            
            // 添加更多Frame信息
            historyItems.Add($"导航UI可见性: {_frame.NavigationUIVisibility}");
            historyItems.Add($"可后退: {_frame.CanGoBack}");
            historyItems.Add($"可前进: {_frame.CanGoForward}");
            
            // 添加历史记录数量估计
            int journalCount = EstimateJournalSize(_frame);
            historyItems.Add($"导航日志大小: {journalCount} 条记录");
            
            // 添加后退历史信息
            if (_frame.CanGoBack)
            {
                // 简单地添加"可后退"信息
                historyItems.Add("--- 后退历史 ---");
                historyItems.Add("后退操作将返回上一个页面");
            }
            
            // 添加前进历史信息
            if (_frame.CanGoForward)
            {
                // 简单地添加"可前进"信息
                historyItems.Add("--- 前进历史 ---");
                historyItems.Add("前进操作将导航到下一个页面");
            }
            
            // 如果没有历史记录
            if (journalCount <= 1 && !_frame.CanGoBack && !_frame.CanGoForward)
            {
                historyItems.Add("没有导航历史记录");
            }
            
            // 更新列表
            _historyListBox.ItemsSource = historyItems;
        }
        
        // 估算日志大小的辅助方法
        private int EstimateJournalSize(Frame frame)
        {
            int count = 1; // 当前页面计数为1
            
            if (frame.CanGoBack) count++;
            if (frame.CanGoForward) count++;
            
            return count;
        }
        
        private void PreviewBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_frame.CanGoBack)
            {
                // 保存原始内容（如果尚未进入预览模式）
                if (!_isInPreviewMode)
                {
                    _originalContent = _frame.Content;
                    _isInPreviewMode = true;
                    _resetPreviewButton.IsEnabled = true;
                    _previewInfoBlock.Text = "预览模式：开启 - 当前查看历史页面";
                }
                
                // 后退导航
                _frame.GoBack();
                
                // 更新按钮状态
                UpdatePreviewButtons();
                
                // 更新历史记录显示
                LoadNavigationHistory();
            }
        }
        
        private void PreviewForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_frame.CanGoForward)
            {
                // 保存原始内容（如果尚未进入预览模式）
                if (!_isInPreviewMode)
                {
                    _originalContent = _frame.Content;
                    _isInPreviewMode = true;
                    _resetPreviewButton.IsEnabled = true;
                    _previewInfoBlock.Text = "预览模式：开启 - 当前查看历史页面";
                }
                
                // 前进导航
                _frame.GoForward();
                
                // 更新按钮状态
                UpdatePreviewButtons();
                
                // 更新历史记录显示
                LoadNavigationHistory();
            }
        }
        
        private void ResetPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // 恢复原始内容
            if (_isInPreviewMode && _originalContent != null)
            {
                // 不再尝试创建新的页面实例，而是直接记录我们需要返回的页面类型
                if (_originalContent is Page originalPage)
                {
                    // 获取页面类型
                    Type pageType = originalPage.GetType();
                    
                    // 设置标志，准备返回初始页面
                    _frame.Navigated += Frame_Navigated_AfterReset;
                    
                    // 如果可以后退，继续后退直到返回初始位置
                    // 使用循环导航法重置页面，不需要创建新实例
                    ReturnToOriginalPage();
                }
                
                _isInPreviewMode = false;
                _resetPreviewButton.IsEnabled = false;
                _previewInfoBlock.Text = "预览模式：关闭";
                
                // 更新按钮状态
                UpdatePreviewButtons();
                
                // 更新历史记录显示
                LoadNavigationHistory();
            }
        }
        
        // 负责在重置后处理导航完成事件
        private void Frame_Navigated_AfterReset(object sender, NavigationEventArgs e)
        {
            // 导航完成后取消事件订阅
            _frame.Navigated -= Frame_Navigated_AfterReset;
            
            // 更新UI状态
            _isInPreviewMode = false;
            _resetPreviewButton.IsEnabled = false;
            _previewInfoBlock.Text = "预览模式：关闭";
            
            // 更新按钮状态
            UpdatePreviewButtons();
            
            // 更新历史记录显示
            LoadNavigationHistory();
        }
        
        // 尝试找回原始页面
        private void ReturnToOriginalPage()
        {
            if (_originalContent is Page originalPage)
            {
                string originalTypeName = originalPage.GetType().FullName;
                
                // 检查当前页面是否已经是原始页面
                if (_frame.Content is Page currentPage && 
                    currentPage.GetType().FullName == originalTypeName)
                {
                    return; // 已经是原始页面，不需要操作
                }
                
                // 尝试通过后退找回原始页面
                bool foundOriginalPage = false;
                
                // 尝试后退导航
                while (_frame.CanGoBack && !foundOriginalPage)
                {
                    Page beforePage = _frame.Content as Page;
                    _frame.GoBack();
                    Page afterPage = _frame.Content as Page;
                    
                    if (afterPage != null && afterPage.GetType().FullName == originalTypeName)
                    {
                        foundOriginalPage = true;
                        break;
                    }
                    
                    // 防止无限循环
                    if (beforePage == afterPage)
                    {
                        break;
                    }
                }
                
                // 如果后退导航没找到，尝试前进导航
                if (!foundOriginalPage)
                {
                    while (_frame.CanGoForward && !foundOriginalPage)
                    {
                        Page beforePage = _frame.Content as Page;
                        _frame.GoForward();
                        Page afterPage = _frame.Content as Page;
                        
                        if (afterPage != null && afterPage.GetType().FullName == originalTypeName)
                        {
                            foundOriginalPage = true;
                            break;
                        }
                        
                        // 防止无限循环
                        if (beforePage == afterPage)
                        {
                            break;
                        }
                    }
                }
                
                // 如果仍然找不到原始页面，尝试创建一个新的首页实例
                if (!foundOriginalPage)
                {
                    try
                    {
                        // 最后的尝试：导航到首页
                        _frame.Navigate(new WPF_Frame.Pages.HomePage());
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"无法恢复到原始页面: {ex.Message}", "导航错误", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        
        private void UpdatePreviewButtons()
        {
            // 更新按钮状态
            _previewBackButton.IsEnabled = _frame.CanGoBack;
            _previewForwardButton.IsEnabled = _frame.CanGoForward;
        }
        
        private void HistoryDialog_Closed(object sender, EventArgs e)
        {
            // 确保关闭窗口时恢复原始内容
            if (_isInPreviewMode && _originalContent != null)
            {
                // 取消可能的事件订阅
                _frame.Navigated -= Frame_Navigated_AfterReset;
                
                // 使用改进的方法返回原始页面
                ReturnToOriginalPage();
                
                _isInPreviewMode = false;
            }
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 