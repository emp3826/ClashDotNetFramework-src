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
    /// DuplicateProfileDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DuplicateProfileDialog : Window
    {
        public bool IsAccepted { get; set; } = false;

        public DuplicateProfileDialog()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
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
