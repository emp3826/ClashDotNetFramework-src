using ClashDotNetFramework.Dialogs;
using ClashDotNetFramework.Models.Enums;
using ClashDotNetFramework.Models.Items;
using ClashDotNetFramework.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ClashDotNetFramework.Pages
{
    /// <summary>
    /// ProfilesPage.xaml 的交互逻辑
    /// </summary>
    public partial class ProfilesPage : Page
    {
        public ProfilesPage()
        {
           InitializeComponent();
        }

        #region 界面事件

        private void ProfilesPage_Loaded(object sender, RoutedEventArgs e)
        {
            // 手动设置Items源
            ProfileItemListView.ItemsSource = Global.Settings.Profile;
        }

        private async void ProfileGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                foreach (string file in files)
                {
                    if (file.EndsWith(".yml") || file.EndsWith(".yaml"))
                    {
                        await ImportProfileFromFile(Path.GetFileName(file), file);
                    }
                }
            }
        }

        #endregion

        #region 内部方法

        private async Task ImportProfileFromFile(string fileName, string filePath)
        {
            // 复制配置文件
            string path = Path.Combine(Utils.Utils.GetClashProfilesDir(), fileName);
            File.Copy(filePath, path, true);
            // 创建配置
            ProfileItem profileItem = new ProfileItem
            {
                Name = fileName,
                FileName = path,
            };
            // 更新配置
            profileItem.Update();
            // 添加配置
            Dispatcher.Invoke(new Action(() =>
            {
                Global.Settings.Profile.Add(profileItem);
            }));
            // 设置为默认配置
            await SetAsDefaultProfile(profileItem);
            // 设置按钮
            Dispatcher.Invoke(new Action(() =>
            {
                ImportButton.Content = "Done!";
                ImportButton.Background = new SolidColorBrush(Color.FromRgb(138, 222, 78));
            }));
            // 保存配置
            Configuration.Save();
        }

        private async Task SetAsDefaultProfile(ProfileItem profile)
        {
            try
            {
                if (!profile.IsSelected)
                {
                    await Global.clashClient.ReloadClashConfig(false, profile.FileName);
                    foreach (var p in Global.Settings.Profile)
                    {
                        p.IsSelected = false;
                    }
                    profile.IsSelected = true;
                    Global.Refresh = true;
                    Configuration.Save();
                }
            }
            catch
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show($"Load config \"{profile.Name}\" error", "Clash .NET Framework", MessageBoxButton.OK, MessageBoxImage.Error);
                }));
            }
        }

        #endregion

        #region 按钮点击事件

        /// <summary>
        /// 下载按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string text = UrlTextBox.Text;
            if (string.IsNullOrWhiteSpace(text))
                return;

            DownloadButton.Content = "Downloading";

            // 开启新线程下载配置文件
            Task.Run(async() =>
            {
                try
                {
                    Uri uri = new Uri(text);
                    ProfileItem profileItem = new ProfileItem
                    {
                        Type = ProfileType.Remote,
                        IsRemote = true,
                        Host = uri.Host,
                        Url = uri.AbsoluteUri,
                        UpdateInterval = 0,
                        LastUpdate = DateTime.Now
                    };
                    if (profileItem.Update())
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            // 添加配置
                            Global.Settings.Profile.Add(profileItem);
                            // 设置文字和颜色
                            DownloadButton.Content = "Success!";
                            DownloadButton.Background = new SolidColorBrush(Color.FromRgb(138, 222, 78));
                        }));
                        await SetAsDefaultProfile(profileItem);
                    }
                }
                catch
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        DownloadButton.Content = "Error!";
                        DownloadButton.Background = new SolidColorBrush(Color.FromRgb(236, 38, 88));
                    }));
                }
                Configuration.Save();
            });
        }

        private void UpdateAllButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateAllButton.Content = "Updating";

            // 开启新线程进行更新
            Task.Run(() =>
            {
                foreach (var profile in Global.Settings.Profile)
                {
                    profile.Update();
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    UpdateAllButton.Content = "Done!";
                    UpdateAllButton.Background = new SolidColorBrush(Color.FromRgb(138, 222, 78));
                }));
                Configuration.Save();
            });
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            ImportButton.Content = "Selecting";

            OpenFileDialog dlg = new OpenFileDialog();
            // 设置默认类型
            dlg.DefaultExt = ".yaml";
            // 设置过滤器
            dlg.Filter = "YAML Files (*.yaml)|*.yaml|YML Files (*.yml)|*.yml";
            // 打开页面获取结果
            bool? result = dlg.ShowDialog();
            // 如果成功, 导入配置文件
            if (result == true)
            {
                // 开启新线程导入
                Task.Run(async() =>
                {
                    await ImportProfileFromFile(dlg.SafeFileName, dlg.FileName);
                });
            }
            else
            {
                ImportButton.Content = "Canceled";
                ImportButton.Background = new SolidColorBrush(Color.FromRgb(27, 161, 226));
            }
        }

        #endregion

        #region ContextMenu 点击事件

        private async void SelectProfile_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (ListView)contextMenu.PlacementTarget;
            if (item.SelectedItems.Count == 0)
                return;
            var profile = item.SelectedItems[0] as ProfileItem;
            if (profile == null)
                return;

            await SetAsDefaultProfile(profile);
        }

        private void UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (ListView)contextMenu.PlacementTarget;
            if (item.SelectedItems.Count == 0)
                return;
            var profile = item.SelectedItems[0] as ProfileItem;
            if (profile == null)
                return;

            Task.Run(async() =>
            {
                if (profile.Update())
                {
                    await SetAsDefaultProfile(profile);
                }
            });
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (ListView)contextMenu.PlacementTarget;
            if (item.SelectedItems.Count == 0)
                return;
            var profile = item.SelectedItems[0] as ProfileItem;
            if (profile == null)
                return;

            try
            {
                Process.Start(profile.FileName);
            }
            catch
            {
            }
        }

        private void DuplicateProfile_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (ListView)contextMenu.PlacementTarget;
            if (item.SelectedItems.Count == 0)
                return;
            var profile = item.SelectedItems[0] as ProfileItem;
            if (profile == null)
                return;

            try
            {
                var dlg = new DuplicateProfileDialog();
                dlg.ShowDialog();
                if (dlg.IsAccepted)
                {
                    Task.Run(() =>
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            ProfileItem profileItem = profile.Copy(dlg.Name.Text);
                            Global.Settings.Profile.Add(profileItem);
                        }));
                    });
                }
            }
            catch
            {
            }
        }

        private void GoToURL_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (ListView)contextMenu.PlacementTarget;
            if (item.SelectedItems.Count == 0)
                return;
            var profile = item.SelectedItems[0] as ProfileItem;
            if (profile == null)
                return;

            try
            {
                if (profile.IsRemote)
                {
                    Process.Start(profile.Url);
                }
            }
            catch
            {
            }
        }

        private void ProfileSettings_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (ListView)contextMenu.PlacementTarget;
            if (item.SelectedItems.Count == 0)
                return;
            var profile = item.SelectedItems[0] as ProfileItem;
            if (profile == null)
                return;

            try
            {
                var dlg = new ProfileSettingsDialog(profile);
                dlg.ShowDialog();
                if (dlg.IsAccepted)
                {
                    Task.Run(() =>
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            profile.Name = dlg.Name.Text;
                            profile.Url = dlg.URL.Text;
                            profile.UpdateInterval = int.Parse(dlg.UpdateInterval.Text);
                        }));
                    });
                }
            }
            catch
            {
            }
        }

        private void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (ListView)contextMenu.PlacementTarget;
            if (item.SelectedItems.Count == 0)
                return;
            var profile = item.SelectedItems[0] as ProfileItem;
            if (profile == null)
                return;

            try
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure to delete \"{profile.Name}\"?", "Clash .NET Framework", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Task.Run(() =>
                    {
                        File.Delete(profile.FileName);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            Global.Settings.Profile.Remove(profile);
                            if (Global.Settings.Profile.FirstOrDefault(p => p.IsSelected == true) == null)
                            {
                                var lastProfile = Global.Settings.Profile.Last();
                                lastProfile.IsSelected = true;
                                SetAsDefaultProfile(lastProfile).GetAwaiter().GetResult();
                            }
                        }));
                        Configuration.Save();
                    });
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}
