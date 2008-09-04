using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

// EndTimeFrom - EndTimeTo input parameters
// SearchFlag
// StoreSearch
// ItemSort
// ItemType
// Sistemare QueryKeywords

namespace LINQtoEbay
{
    internal class EbayHelper
    {

#error Insert your Application key

        internal static string APP_ID = "";
        internal static string URL = "http://open.api.sandbox.ebay.com/shopping?";
        internal static string CALLNAME = "FindItemsAdvanced";
        internal static XNamespace NS = "urn:ebay:apis:eBLBaseComponents";
        internal static string SITE_ID = "0";
        internal static string VERSION = "565";
        internal static string MAX_ENTRIES = "20";

        private static string orgQuery = "";

        internal static EbayItem[] PerformWebQuery(EbaySearchCriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria");

            orgQuery = string.Format("{0}&callname={1}&responseeconding=XML&appid={2}&siteid={3}&version={4}&MaxEntries={5}{6}",
URL, CALLNAME, APP_ID, SITE_ID, VERSION, MAX_ENTRIES, criteria.ToString());

            /*return new EbayItem[] { 
                new EbayItem() { Title = "SECOND", EndTime = new DateTime(2008, 8, 15, 10, 10, 0, 0)  }, 
                new EbayItem() { Title = "FIRST", EndTime = new DateTime(2008, 8, 15, 10, 0, 0, 0) }, 
                new EbayItem() { Title = "THIRD", EndTime = new DateTime(2008, 8, 15, 10, 10, 20, 0)  } };*/

            return GetItems(criteria, orgQuery, true).ToArray();
        }

        private static IEnumerable<EbayItem> GetItems(EbaySearchCriteria criteria, string query, bool firstTime)
        {
            XElement root, itemArray;
            IEnumerable<XElement> errorsEl;
            ErrorType[] errors;
            int numItems, pages, index;
            string query2;
            char[] splitChar = new char[] { '|' };

            root = XElement.Load(query);

#if DEBUG
            Console.WriteLine(query);
            Console.WriteLine();
            Console.WriteLine(root.ToString());
            Console.WriteLine();
#endif

            errorsEl = root.Descendants(NS + "Errors");

            if (errorsEl.Count() > 0)
            {
                errors = new ErrorType[errorsEl.Count()];
                index = 0;

                foreach (XElement err in errorsEl)
                {
                    errors[index] = ParseEbayError(err);
                    index++;
                }

                throw new EbayErrorException("Error", errors);
            }

            if ((numItems = int.Parse(root.Element(NS + "TotalItems").Value)) == 0)
                return new EbayItem[] { };

            itemArray = root.Element(NS + "SearchResult").Element(NS + "ItemArray");
            var items = from it in itemArray.Descendants(NS + "Item")
                        select EbayItem.Parse(it);

            if (firstTime)
            {
                if ((pages = int.Parse(root.Element(NS + "TotalPages").Value)) != 0)
                {
                    for (int i = 2; i <= pages; i++)
                    {
                        query2 = string.Format(orgQuery + "&PageNumber={0}", i);
                        items = items.Union<EbayItem>(GetItems(criteria, query2, false));
                    }
                }
            }

            return ApplyCriteria(criteria, items);
        }

        private static IEnumerable<EbayItem> ApplyCriteria(EbaySearchCriteria criteria, IEnumerable<EbayItem> items)
        {
            char[] splitChar = new char[] { '|' };

            if (criteria == null)
                throw new ArgumentNullException("criteria");

            if (items == null)
                throw new ArgumentNullException("items");

            if (criteria.BuyItNowPriceMax >= 0)
            {
                items = from it in items
                        where it.BuyItNowPrice <= criteria.BuyItNowPriceMax
                        select it;
            }

            if (criteria.BuyItNowPriceMin >= 0)
            {
                items = from it in items
                        where it.BuyItNowPrice >= criteria.BuyItNowPriceMin
                        select it;
            }

            if (criteria.StartTimeMax != DateTime.MaxValue)
            {
                items = from it in items
                        where it.StartTime <= criteria.StartTimeMax
                        select it;
            }

            if (criteria.StartTimeMin != DateTime.MinValue)
            {
                items = from it in items
                        where it.StartTime >= criteria.StartTimeMin
                        select it;
            }

            if (criteria.EndTimeMax != DateTime.MaxValue)
            {
                items = from it in items
                        where it.EndTime <= criteria.EndTimeMax
                        select it;
            }

            if (criteria.EndTimeMin != DateTime.MinValue)
            {
                items = from it in items
                        where it.EndTime >= criteria.EndTimeMin
                        select it;
            }

            return items;
        }

        public static string BuildQuery(EbaySearchCriteria criteria)
        {
            return criteria.ToString();
        }

        internal static ErrorType ParseEbayError(XElement root)
        {
            XElement el;
            ErrorType error = new ErrorType();

            if ((el = root.Element(NS + "ErrorClassification")) != null)
                error.ErrorClassification = (ErrorClassificationCodeType)Enum.Parse(typeof(ErrorClassificationCodeType), el.Value); ;

            if ((el = root.Element(NS + "ErrorCode")) != null)
                error.ErrorCode = el.Value;

            // ErrorParameters 

            if ((el = root.Element(NS + "LongMessage")) != null)
                error.LongMessage = el.Value;

            if ((el = root.Element(NS + "SeverityCode")) != null)
                error.SeverityCode = (SeverityCodeType)Enum.Parse(typeof(SeverityCodeType), el.Value);

            if ((el = root.Element(NS + "ShortMessage")) != null)
                error.ShortMessage = el.Value;

            return error;
        }
    }

    internal class EbayErrorException : ApplicationException
    {
        private ErrorType error;
        private ErrorType[] errors;

        internal EbayErrorException(string msg, ErrorType error) : base(msg)
        {
            this.error = error;
        }

        internal EbayErrorException(string msg, ErrorType[] errors) : base(msg)
        {
            this.errors = errors;
        }

        internal EbayErrorException(string msg, ErrorType error, Exception innerException) : base(msg, innerException)
        {
            this.error = error;
        }

        internal EbayErrorException(string msg, ErrorType[] errors, Exception innerException) : base(msg, innerException)
        {
            this.errors = errors;
        }

        internal ErrorType Error
        {
            get
            {
                return error;
            }
        }

        internal ErrorType[] Errors
        {
            get
            {
                return errors;
            }
        }
    }
}
