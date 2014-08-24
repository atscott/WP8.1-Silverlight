
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SmallShopsUnitedDomainLayer
{
    public interface IMerchantService
    {
        IList<Merchant> GetMerchants();
        Task<IList<Merchant>> RefreshMerchants();
    }

    public class MerchantService : IMerchantService
    {
        private const string MerchantsKey = "MerchantList";
        private static readonly IsolatedStorageSettings AppSettings = IsolatedStorageSettings.ApplicationSettings;
        public IList<Merchant> GetMerchants()
        {
            if (AppSettings.Contains(MerchantsKey))
            {
                return (IList<Merchant>)AppSettings[MerchantsKey];
            }
            return new List<Merchant>();
        }

        public async Task<IList<Merchant>> RefreshMerchants()
        {
            var merchants = await SmallShopsMerchantsScraper.GetMerchants();

            await GetGeolocationsForMerchants(merchants);

            AppSettings[MerchantsKey] = merchants;
            return (IList<Merchant>)AppSettings[MerchantsKey];
        }

        private static async Task GetGeolocationsForMerchants(IEnumerable<Merchant> merchants)
        {
            var oldMerchantList = new List<Merchant>();
            if (AppSettings.Contains(MerchantsKey))
            {
                oldMerchantList = (List<Merchant>)AppSettings[MerchantsKey];
            }

            foreach (var merchant in merchants)
            {
                var oldMerchantWithSameLocation = oldMerchantList.FirstOrDefault(f => f.Location == merchant.Location);
                if (oldMerchantWithSameLocation != null)
                {
                    merchant.LocationGeoposition = oldMerchantWithSameLocation.LocationGeoposition;
                }
                else
                {
                    var geo = new Geocoder();
                    var coordinates = await geo.GetCoordinates(merchant.Location);
                    merchant.LocationGeoposition = coordinates;
                }
            }
        }
    }
}
