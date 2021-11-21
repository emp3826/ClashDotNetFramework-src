using ClashDotNetFramework.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClashDotNetFramework.Dialogs
{
    /// <summary>
    /// ProfileSettingsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ProfileSettingsDialog : Window
    {
        public bool IsAccepted { get; set; } = false;

        public ProfileSettingsDialog()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
        }

        public ProfileSettingsDialog(ProfileItem profileItem) : this()
        {
            LoadInformation(profileItem);
        }

        private void LoadInformation(ProfileItem profileItem)
        {
            Name.Text = profileItem.Name;
            URL.Text = profileItem.Url;
            UpdateInterval.Text = Convert.ToString(profileItem.UpdateInterval);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            IsAccepted = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
