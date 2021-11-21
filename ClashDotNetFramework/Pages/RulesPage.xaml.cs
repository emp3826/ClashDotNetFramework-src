using Clash.SDK.Models.Share;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClashDotNetFramework.Pages
{
    /// <summary>
    /// RulesPage.xaml 的交互逻辑
    /// </summary>
    public partial class RulesPage : Page
    {
        private ObservableCollection<ClashRuleData> RuleItemCollection = new ObservableCollection<ClashRuleData>();

        public RulesPage()
        {
            InitializeComponent();

            RuleItemListView.ItemsSource = RuleItemCollection;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                if (RuleItemCollection.Count > 0)
                    Dispatcher.Invoke(new Action(() =>
                    {
                        RuleItemCollection.Clear();
                    }));

                var response = await Global.clashClient.GetClashRules();
                foreach (var rule in response.Rules)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        RuleItemCollection.Add(rule);
                    }));
                }
            });
        }
    }
}
