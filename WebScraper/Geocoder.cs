using GeocodeSharp.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper
{
    public class Geocoder
    {
        public GeoCoordinate GetAddress(string address)
        {
            var client = new GeocodeClient();
            var response = client.GeocodeAddress(address, false);
            response.Wait(500);

            if (response.Result.Status == GeocodeStatus.Ok)
            {
                var firstResult = response.Result.Results.First();
                return firstResult.Geometry.Location;
            }

            return null;
        }
    }
}
