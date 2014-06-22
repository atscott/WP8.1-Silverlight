using System.Collections.Generic;
using Windows.Devices.Geolocation;

namespace SmallShopsUnitedDomainLayer
{
    public class Merchant
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public IList<Rewards> Rewards { get; set; }
        public string Location { get; set; }
        public string Url { get; set; }
        public IList<string> NotesAndConditions { get; set; }
        public string Neighborhood { get; set; }
        public BasicGeoposition LocationGeoposition { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
