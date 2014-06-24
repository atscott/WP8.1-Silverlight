using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using SmallShopsUnitedDomainLayer;

namespace sdkMapControlWP8CS.Pages
{
    public partial class MerchantDetails
    {
        private Merchant _merchant;
        public MerchantDetails()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string msg;

            NavigationContext.QueryString.TryGetValue("name", out msg);
            if (msg == null)
            {
                MerchantName.Text = "Merchant Not Found";
                return;
            }

            _merchant = new MerchantService().GetMerchants().FirstOrDefault(f => f.Name.StartsWith(msg));
            if (_merchant == null)
            {
                MerchantName.Text = "Merchant Not Found";
            }
            else
            {
                MerchantName.Text = _merchant.Name.Trim();
                MoreInfoHyperlink.Content = _merchant.Url.Trim();
                TxtLocation.Text = _merchant.Location.Trim();
                TxtNeighborhood.Text = _merchant.Neighborhood.Trim();
                TxtCategory.Text = _merchant.Category.Trim();
                TxtRewards.Text = _merchant.Rewards.Aggregate("", (current, reward) => current + (reward + "\n")).Trim();
                TxtNotes.Text = _merchant.NotesAndConditions.Aggregate("", (current, notesAndCondition) => current + ("• " + notesAndCondition + "\n")).Trim();
            }
        }

        private void MoreInfoHyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            var webBrowserTask = new WebBrowserTask {Uri = new Uri(_merchant.Url, UriKind.Absolute)};
            webBrowserTask.Show();
        }
    }
}