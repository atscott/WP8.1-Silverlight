﻿using System.Linq;
using System.Text;
using System.Windows.Navigation;
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
                MerchantName.Text = _merchant.Name;
                TxtNeighborhood.Text = _merchant.Neighborhood;
                TxtLocation.Text = _merchant.Location;
                TxtCategory.Text = _merchant.Category;
                TxtNotes.Text = _merchant.NotesAndConditions.Aggregate("", (current, notesAndCondition) => current + (notesAndCondition + "\n"));
                TxtRewards.Text = _merchant.Rewards.Aggregate("", (current, reward) => current + (reward + "\n"));

            }
        }
    }
}