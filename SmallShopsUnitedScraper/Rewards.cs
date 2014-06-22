using System.Collections.Generic;
using System.Linq;

namespace SmallShopsUnitedDomainLayer
{
    public class Rewards
    {
        public string CategoryDescription { get; set;}
        public IList<string> Items {get;set;}

        public Rewards()
        {
            Items = new List<string>();
        }

        public override string ToString()
        {
            return Items.Aggregate(CategoryDescription + "\n", (current, item) => current + ("\t" + item + "\n"));
        }
    }
}
