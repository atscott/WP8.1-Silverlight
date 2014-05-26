using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace WebScraper
{
    public class Geocoder
    {
        private const string apiUrl = @"http://dev.virtualearth.net/REST/v1/Locations?query={QUERY_HERE}&o=json&key=Ah0lUhUorbBEefBM-h-Ozp7sHI0qp4TMvbR7rvqT_gxWDTM8aF0i-7Jj_37FwfUY";
        public async Task<BasicGeoposition> GetCoordinates(string address)
        {
            var encoded = address.Replace(" ", "%20").Replace(",", "%2C");

            var target = apiUrl.Replace("{QUERY_HERE}", encoded);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(target);
            var response = await request.GetResponseAsync();

            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader htmlStream = new StreamReader(responseStream, Encoding.UTF8))
                {
                    var result = htmlStream.ReadToEnd();
                    var reg = new Regex(@"\[-*\d{1,3}.\.\d*,-*\d{1,3}\.\d*\]");
                    var match = reg.Match(result);
                    if (match.Length > 0)
                    {
                        var matchString = match.Value.Replace("[", "").Replace("]", "");
                        var lat = Double.Parse(matchString.Substring(0, matchString.IndexOf(",")));
                        var lng = Double.Parse(matchString.Substring(matchString.IndexOf(",") + 1));
                        return new BasicGeoposition { Latitude = lat, Longitude = lng };
                    }
                    else
                    {
                        return new BasicGeoposition();
                    }
                }
            }
        }
    }
}
