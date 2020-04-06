using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Petrol.Model
{
    public class CurrencyObject
    {
        public string shortName { get; set; }
        public string longName { get; set; }
        public double value { get; set; }
        public double askValue { get; set; }
        public double bidValue { get; set; }
        public double changeCur { get; set; }
        public double changePer { get; set; }

        public double FindValue(List<CurrencyObject> result, string curr)
        {
            var usdValue = result.Where(x => x.shortName == curr).Select(p => p.value).ToList();
            return usdValue[0];
        }
    }

}