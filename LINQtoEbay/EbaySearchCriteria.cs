using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LINQtoEbay
{
    internal class EbaySearchCriteria
    {
        internal EbaySearchCriteria()
        {
            BidCountMax = -1;
            BidCountMin = -1;
            FeedbackScoreMax = -1;
            FeedbackScoreMin = -1;
            BuyItNowPriceMax = -1;
            BuyItNowPriceMin = -1;
            PriceMax = -1;
            PriceMin = -1;
            Condition = Condition.Empty;
            Description = "";
            Title = "";
            Categories = "";
            Keywords = "";
            //Location = "";
            //Country = CountryCodeType.CustomCode;
            StartTimeMax = DateTime.MaxValue;
            StartTimeMin = DateTime.MinValue;
            EndTimeMax = DateTime.MaxValue;
            EndTimeMin = DateTime.MinValue;
        }

        // query

        public int BidCountMax { get; set; }
        public int BidCountMin { get; set; }
        public double PriceMax { get; set; }
        public double PriceMin { get; set; }
        public Condition Condition { get; set; }
        public int FeedbackScoreMax { get; set; } // da gestire
        public int FeedbackScoreMin { get; set; } // da gestire
        public string Keywords { get; set; }

        // gestisco io

        public double BuyItNowPriceMax { get; set; }
        public double BuyItNowPriceMin { get; set; }
        public string Categories { get; set; }
        public string Description { get; set; }
        public DateTime EndTimeMax { get; set; }
        public DateTime EndTimeMin { get; set; }
        //public ListingStatusCodeType ListingStatus { get; set; } // vedi IncludeSelecotr
        //public ShippingCostSummaryType ShippingCostSummary { get; set; } // vedi IncludeSelector 
        public DateTime StartTimeMax { get; set; }
        public DateTime StartTimeMin { get; set; }
        //public string TimeLeftMax { get; set; }
        //public string TimeLeftMin { get; set; }
        public string Title { get; set; }
        //public CountryCodeType Country { get; set; }
        //public string Location { get; set; }

        //public bool GetItFast { get; set; }
        //public BuyerPaymentMethodCodeType PaymentMethods { get; set; }


        // IncludeSelector=Details --> Country
        // IncludeSelector=SearchDetails --> Location

        public override string ToString()
        {
            string ret = "";
            string queryKeywords = "";
            string includeSelector = "";
            string[] keys;

            if (Keywords != "")
            {
                keys = Keywords.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string key in keys)
                    queryKeywords += key + "%20";

                queryKeywords = queryKeywords.Remove(queryKeywords.Length - 3, 3);

                ret += string.Format("&QueryKeywords={0}", queryKeywords);
            }
            if (BidCountMin != -1)
                ret += string.Format("&BidCountMin={0}", BidCountMin);
            if (BidCountMax != -1)
                ret += string.Format("&BidCountMax={0}", BidCountMax);
            if (PriceMin != -1)
                ret += string.Format("&PriceMin.Value={0}", PriceMin);
            if (PriceMax != -1)
                ret += string.Format("&PriceMax.Value={0}", PriceMax);
            if (FeedbackScoreMin != -1)
                ret += string.Format("&FeedbackScoreMin={0}", FeedbackScoreMin);
            if (FeedbackScoreMax != -1)
                ret += string.Format("&FeedbackScoreMax={0}", FeedbackScoreMax);
            if (Condition != Condition.Empty)
                ret += string.Format("&Condition={0}", Condition.ToString());

            // IncludeSelector

            /*if (Country != CountryCodeType.CustomCode)
                includeSelector += "Details,";
            if (Location != "")
                includeSelector += "SearchDetails";

            if (includeSelector != "" && includeSelector[includeSelector.Length - 1] == ',')
                includeSelector = includeSelector.Remove(includeSelector.Length - 2, 1);

            if (includeSelector != "")
                ret += string.Format("&IncludeSelector={0}", includeSelector);*/

            return ret;
        }
    }
}
