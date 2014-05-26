using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebScraper
{
    public class SmallShopsMerchantsScraper
    {
        private const string target = @"https://www.smallshopsunited.com/merchants.php";

        public static IList<Merchant> GetMerchants()
        {
            HtmlDocument doc = GetHtmlDocForPage();
            HtmlNode merchantTable = doc.DocumentNode.SelectSingleNode("//table[@id=\"merchTable\"]");
            var merchants = new List<Merchant>();
            foreach (var merchantRow in merchantTable.SelectNodes(merchantTable.XPath + "//tr"))
            {
                if (merchantRow.SelectNodes(merchantRow.XPath + "//th") != null)
                    continue;

                merchants.Add(getMerchantFromTableRow(merchantRow));
            }
            return merchants;
        }

        private static Merchant getMerchantFromTableRow(HtmlNode merchantRow)
        {
            var merchantDetailedInfos = merchantRow.SelectNodes(merchantRow.XPath + "//td");

            var merchant = new Merchant();
            merchant.Url = GetUrlFromCell(merchantDetailedInfos[0]);
            merchant.Name = GetNameFromCell(merchantDetailedInfos[0]);
            merchant.Location = GetLocationFromCell(merchantDetailedInfos[0]);
            merchant.Neighborhood = GetNeighborhoodFromCell(merchantDetailedInfos[1]);
            merchant.Category = GetCategoryFromCell(merchantDetailedInfos[2]);
            merchant.Rewards = GetRewardsFromCell(merchantDetailedInfos[3]);
            merchant.NotesAndConditions = GetNotesAndConditionsFromCell(merchantDetailedInfos[4]).ToList();
            return merchant;
        }

        private static IEnumerable<string> GetNotesAndConditionsFromCell(HtmlNode htmlNode)
        {
            if (htmlNode.SelectNodes(htmlNode.XPath + "//p") != null)
            {
                foreach (var reward in htmlNode.SelectNodes(htmlNode.XPath + "//p"))
                {
                    yield return reward.InnerText;
                }
            }
        }

        private static List<Rewards> GetRewardsFromCell(HtmlNode htmlNode)
        {
            var rewardGroups = htmlNode.SelectNodes(htmlNode.XPath + "//ul");
            var rewardDescriptions = htmlNode.SelectNodes(htmlNode.XPath + "//strong");
            var allRewards = new List<Rewards>();

            for (int i = 0; i < rewardGroups.Count; i++)
            {
                var rewards = new Rewards();
                rewards.CategoryDescription = rewardDescriptions[0].InnerText;
                foreach (var item in rewardGroups[i].SelectNodes(rewardGroups[i].XPath + "//li"))
                {
                    rewards.Items.Add(item.InnerText);
                }
                allRewards.Add(rewards);
            }
            return allRewards;
        }

        private static IEnumerable<string> getRewardsFromUnorderedList(HtmlNode htmlNode)
        {
            foreach (var reward in htmlNode.SelectNodes(htmlNode.XPath + "//li"))
            {
                yield return reward.InnerText;
            }
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
            var potentialLocation = htmlNode.InnerText.Replace("\n", "");
            potentialLocation = RemoveDoubleSpacesFromString(potentialLocation);
            return separateCityNameFromStreetIfTogether(potentialLocation);
        }

        private static string RemoveDoubleSpacesFromString(string potentialLocation)
        {
            while (potentialLocation != potentialLocation.Replace("  ", " "))
            {
                potentialLocation = potentialLocation.Replace("  ", " ");
            }
            return potentialLocation;
        }

        private static string separateCityNameFromStreetIfTogether(string potentialLocation)
        {
            var r1 = new Regex(@"([^\s])([A-Z].*,)");
            Match match = r1.Match(potentialLocation);
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
            var link = href.Attributes.FirstOrDefault(w => w.Name == "href").Value;
            if (link.StartsWith("../"))
            {
                link = @"https://www.smallshopsunited.com" + link.Substring(2);
            }


            return link;
        }

        private static HtmlDocument GetHtmlDocForPage()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(target);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader htmlStream = new StreamReader(responseStream, Encoding.UTF8))
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(htmlStream);
                    return doc;
                }
            }
        }


    }
}
