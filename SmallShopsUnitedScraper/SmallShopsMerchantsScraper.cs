using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SmallShopsUnitedDomainLayer
{
    public class SmallShopsMerchantsScraper
    {
        private const string Target = @"https://www.smallshopsunited.com/merchants.php";

        public async static Task<IList<Merchant>> GetMerchants()
        {
            var merchants = new List<Merchant>();
            var doc = await GetHtmlDocForPage();
            if (doc.DocumentNode == null)
                return merchants;

            var merchantTable = doc.DocumentNode.SelectSingleNode("//table[@id=\"merchTable\"]");
            foreach (var merchantRow in merchantTable.SelectNodes(merchantTable.XPath + "//tr"))
            {
                if (merchantRow.SelectNodes(merchantRow.XPath + "//th") != null)
                    continue;

                merchants.Add(GetMerchantFromTableRow(merchantRow));
            }
            return merchants;
        }

        
        private async static Task<HtmlDocument> GetHtmlDocForPage()
        {
            var request = (HttpWebRequest)WebRequest.Create(Target);
            var response = await request.GetResponseAsync();

            using (var responseStream = response.GetResponseStream())
            {
                using (var htmlStream = new StreamReader(responseStream, Encoding.UTF8))
                {
                    var doc = new HtmlDocument();
                    doc.Load(htmlStream);
                    return doc;
                }
            }
        }

        private static Merchant GetMerchantFromTableRow(HtmlNode merchantRow)
        {
            var merchantDetailedInfos = merchantRow.SelectNodes(merchantRow.XPath + "//td");

            var merchant = new Merchant
            {
                Url = GetUrlFromCell(merchantDetailedInfos[0]),
                Name = GetNameFromCell(merchantDetailedInfos[0]),
                Location = GetLocationFromCell(merchantDetailedInfos[0]),
                Neighborhood = GetNeighborhoodFromCell(merchantDetailedInfos[1]),
                Category = GetCategoryFromCell(merchantDetailedInfos[2]),
                Rewards = GetRewardsFromCell(merchantDetailedInfos[3]),
                NotesAndConditions = GetNotesAndConditionsFromCell(merchantDetailedInfos[4]).ToList()
            };
            return merchant;
        }

        private static IEnumerable<string> GetNotesAndConditionsFromCell(HtmlNode htmlNode)
        {
            if (htmlNode.SelectNodes(htmlNode.XPath + "//p") != null)
            {
                foreach (var reward in htmlNode.SelectNodes(htmlNode.XPath + "//p"))
                {
                    yield return reward.InnerText.Replace("&nbsp;"," ").Replace("&#39;", "'");
                }
            }
        }

        private static List<Rewards> GetRewardsFromCell(HtmlNode htmlNode)
        {
            var rewardGroups = htmlNode.SelectNodes(htmlNode.XPath + "//ul");
            var rewardDescriptions = htmlNode.SelectNodes(htmlNode.XPath + "//strong");
            var allRewards = new List<Rewards>();

            for (var index = 0; index < rewardGroups.Count; index++)
            {
                var rewardGroup = rewardGroups[index];
                var rewards = new Rewards
                {
                    CategoryDescription = rewardDescriptions[index].InnerText
                };
                foreach (var item in rewardGroup.SelectNodes(rewardGroup.XPath + "//li"))
                {
                    rewards.Items.Add(item.InnerText);
                }
                allRewards.Add(rewards);
            }
            return allRewards;
        }


        private static string GetCategoryFromCell(HtmlNode htmlNode)
        {
            return htmlNode.InnerText;
        }

        private static string GetNeighborhoodFromCell(HtmlNode htmlNode)
        {
            return htmlNode.InnerText;
        }

        private static string GetLocationFromCell(HtmlNode htmlNode)
        {
            var location = "";
            htmlNode.ChildNodes.Where(w => w.Name == "#text").ToList().ForEach(f => location += f.InnerText.Replace("\n", ""));
            location = RemoveDoubleSpacesFromString(location);
            return SeparateCityNameFromStreetIfTogether(location);
        }

        private static string RemoveDoubleSpacesFromString(string potentialLocation)
        {
            while (potentialLocation != potentialLocation.Replace("  ", " "))
            {
                potentialLocation = potentialLocation.Replace("  ", " ");
            }
            return potentialLocation;
        }

        private static string SeparateCityNameFromStreetIfTogether(string potentialLocation)
        {
            var r1 = new Regex(@"([^\s])([A-Z].*,)");
            var match = r1.Match(potentialLocation);
            if (match.Success)
            {
                potentialLocation = potentialLocation.Replace(match.Groups[0].Value, match.Groups[1].Value + " " + match.Groups[2].Value);
            }
            return potentialLocation;
        }

        private static string GetNameFromCell(HtmlNode htmlNode)
        {
            var href = htmlNode.SelectSingleNode(htmlNode.XPath + "//a[@href]");

            return href.InnerText;
        }

        private static string GetUrlFromCell(HtmlNode htmlNode)
        {
            var href = htmlNode.SelectSingleNode(htmlNode.XPath + "//a[@href]");
            var firstOrDefault = href.Attributes.FirstOrDefault(w => w.Name == "href");
            if (firstOrDefault != null)
            {
                var link = firstOrDefault.Value;
                if (link.StartsWith("../"))
                {
                    link = @"https://www.smallshopsunited.com" + link.Substring(2);
                }


                return link;
            }
            return "";
        }



    }
}
