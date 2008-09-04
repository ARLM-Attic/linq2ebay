using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LINQtoEbay
{
    public enum Condition { New, Used, Empty }

    public class EbayItem
    {
        public bool BuyItNowAvailable { get; set; }
        public double BuyItNowPrice { get; set; }
        public int BidCount { get; set; }
        public string Categories { get; set; }
        public Condition Condition { get; set; }
        public double CurrentPrice { get; set; }
        public string Description { get; set; }
        public DateTime EndTime { get; set; }
        public string ItemID { get; set; }
        public ListingStatusCodeType ListingStatus { get; set; }
        public ListingTypeCodeType ListingType { get; set; }
        public string Location { get; set; }
        public bool PictureExists { get; set; }
        public string[] PictureURL { get; set; }
        public DateTime StartTime { get; set; }
        public string TimeLeft { get; set; }
        public string Title { get; set; }

        public CountryCodeType Country { get; set; }
        //public SimpleUserType HighBidder { get; set; }
        //public SimpleUserType Seller { get; set; } // controllare
        //public string SellerComments { get; set; }
        //public string Subtitle { get; set; }

        public override string ToString()
        {
            double price = (BuyItNowAvailable ? BuyItNowPrice : CurrentPrice);

            return string.Format("Title={0}\n\rItemID={1}\n\rPrice={2}\n\r\n\r", Title, ItemID, price);
        }

        public static EbayItem Parse(XElement root)
        {
            XElement el;
            bool buyItNow;

            if (root == null)
                throw new ArgumentNullException("root");

            XNamespace ns = "urn:ebay:apis:eBLBaseComponents";
            EbayItem item = new EbayItem();

            item.Title = "";
            item.Description = "";
            item.CurrentPrice = -1;
            item.BuyItNowPrice = -1;
            item.BidCount = -1;

            buyItNow = (root.Element(ns + "BuyItNowAvailable") != null ? bool.Parse(root.Element(ns + "BuyItNowAvailable").Value) : false);

            if (buyItNow)
            {
                if ((el = root.Element(ns + "BuyItNowPrice")) != null)
                    item.BuyItNowPrice = double.Parse(el.Element(ns + "Value").Value);
                else if ((el = root.Element(ns + "ConvertedBuyItNowPrice")) != null)
                {
                    //BuyItNowPrice = double.Parse(el.Element(ns + "Value").Value);
                    item.BuyItNowPrice = double.Parse(el.Value);
                }
            }

            if ((el = root.Element(ns + "CurrentPrice")) != null)
                item.CurrentPrice = double.Parse(el.Element(ns + "Value").Value);
            else if ((el = root.Element(ns + "ConvertedCurrentPrice")) != null)
            {
                item.CurrentPrice = double.Parse(el.Value);
                //CurrentPrice = double.Parse(el.Element(ns + "Value").Value);
            }

            item.ItemID = root.Element(ns + "ItemID").Value;
            item.EndTime = DateTime.Parse(root.Element(ns + "EndTime").Value);
            item.ListingType = (ListingTypeCodeType)Enum.Parse(typeof(ListingTypeCodeType), root.Element(ns + "ListingType").Value);
            item.ListingStatus = (ListingStatusCodeType)Enum.Parse(typeof(ListingStatusCodeType), root.Element(ns + "ListingStatus").Value);
            item.TimeLeft = root.Element(ns + "TimeLeft").Value;
            item.Title = root.Element(ns + "Title").Value;

            if (item.ListingType != ListingTypeCodeType.FixedPriceItem)
                item.BidCount = int.Parse(root.Element(ns + "BidCount").Value);

            //Categories = root.Element(ns + "PrimaryCategoryName").Value.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            
            if ((el  = root.Element(ns + "PrimaryCategoryName")) != null)
                item.Categories = el.Value;

            // ShippingCostSummary

            return item;
        }
    }
}
