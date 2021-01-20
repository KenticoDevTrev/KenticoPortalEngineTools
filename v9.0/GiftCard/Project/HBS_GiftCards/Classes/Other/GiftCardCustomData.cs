using CMS.Base;
using CMS.DataEngine;
using CMS.DataEngine.Serialization;
using CMS.Ecommerce;
using CMS.EventLog;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HBS_GiftCards
{

    [Serializable]
    public class GiftCardCustomData : IDataContainer
    {
        public List<GiftCardUsage> GiftCardUsages;

        public GiftCardCustomData()
        {
            GiftCardUsages = new List<GiftCardUsage>();
        }

        public void AddGiftCard(string Code, decimal Amount, string SiteName = null)
        {
            GiftCardInfo TheCard = GiftCardHelper.GetGiftCard(Code, SiteName);
            AddGiftCard(TheCard, Amount);
        }

        public void AddGiftCard(GiftCardInfo GiftCard, decimal Amount)
        {
            GiftCardUsage ExistingCard = GiftCardUsages.Where(x => x.GiftCard.GiftCardCode == GiftCard.GiftCardCode).FirstOrDefault();
            if (ExistingCard != null)
            {
                ExistingCard.Amount = (ExistingCard.GiftCard.AmountRemaining >= Amount ? Amount : ExistingCard.GiftCard.AmountRemaining);
            }
            else
            {
                GiftCardUsage GCU = new GiftCardUsage(GiftCard, (GiftCard.AmountRemaining >= Amount ? Amount : GiftCard.AmountRemaining), GiftCardUsedOnEnum.Either);
                GiftCardUsages.Add(GCU);
            }
        }

        public string ToXML()
        {
            // Custom XML Serialize, sadly the XDocument, XmlWriter, and other items all failed to work :( 
            string XML = string.Format(@"
<GiftCardCustomData>
    <GiftCardUsages>
        {0}
    </GiftCardUsages>
</GiftCardCustomData>",
GiftCardUsages.Count > 0 ? string.Join("", GiftCardUsages.Select(x => x.ToXML())) : "").Trim();

            CacheHelper.TouchKey("GiftCardCustomDataFromXML|" + GetHash(XML));
            return XML;
        }

        public static string GetInnerText(XmlNode Node)
        {
            if (Node != null)
            {
                return Node.InnerText;
            }
            else
            {
                return null;
            }
        }

        public static string GetHash(string Value)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(Value);
            byte[] hash = sha1.ComputeHash(buf, 0, buf.Length);
            return System.BitConverter.ToString(hash).Replace("-", "");
        }

        /// <summary>
        /// Gets a DataTable with rows representing all the Gift Card Usage.
        /// </summary>
        /// <returns>Datatable of all the Gift Card Usages</returns>
        public DataTable GetGiftCardUsageDataTable()
        {
            // Get Columns
            DataTable GiftCardSummary = new DataTable();
            GiftCardSummary.Columns.AddRange(typeof(GiftCardUsage).GetProperties().Where(pi => pi.GetGetMethod() != null).Select(x => new DataColumn(x.Name, x.PropertyType)).ToArray());
            GiftCardSummary.Columns.Add(new DataColumn("Amount", typeof(decimal)));
            List<string> GiftCardColumnNames = GiftCardSummary.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            GiftCardColumnNames.Remove("Amount");
            // loop through objects and columns and build.
            foreach (GiftCardUsage GCUsage in GiftCardUsages)
            {
                DataRow GiftCardUsageDR = GiftCardSummary.NewRow();
                foreach (string PropertyName in GiftCardColumnNames)
                {
                    GiftCardUsageDR[PropertyName] = GCUsage.GiftCard.GetValue(PropertyName);
                }
                GiftCardUsageDR["Amount"] = GCUsage.Amount;
                GiftCardSummary.Rows.Add(GiftCardUsageDR);
            }
            return GiftCardSummary;
        }

        #region "IDataContainer Methods"

        public List<string> ColumnNames
        {
            get
            {
                return new List<string>() { "GiftCardUsages" };
            }
        }

        public object this[string columnName]
        {
            get
            {
                return GetValue(columnName);
            }

            set
            {
                SetValue(columnName, value);
            }
        }
        public bool TryGetValue(string columnName, out object value)
        {
            try
            {
                value = GetValue(columnName);
                return true;
            }
            catch (Exception ex)
            {
                value = null;
                return false;
            }
        }

        public bool ContainsColumn(string columnName)
        {
            return ColumnNames.Exists(x => x.ToLower() == columnName.ToLower());
        }

        public object GetValue(string columnName)
        {
            switch (columnName.ToLower())
            {
                case "giftcardusages":
                    DataTable GiftCardUsageTable = new DataTable();
                    GiftCardUsageTable.Columns.Add("GiftCardUsage", typeof(GiftCardUsage));
                    List<DataRow> GCURows = new List<DataRow>();
                    foreach (GiftCardUsage GCU in GiftCardUsages)
                    {
                        DataRow Row = GiftCardUsageTable.NewRow();
                        Row["GiftCardUsage"] = GCU;
                        GCURows.Add(Row);
                    }
                    return GCURows;
            }
            return null;
        }

        public bool SetValue(string columnName, object value)
        {
            try
            {
                switch (columnName.ToLower())
                {
                    case "giftcardusages":
                        GiftCardUsages = (List<GiftCardUsage>)value;
                        return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        #endregion
    }

    [Serializable]
    public class GiftCardOrderCustomData : GiftCardCustomData
    {
        public const string GiftCardOrderCustomDataKey = "GiftCardUsableHistoryCustomData";

        public GiftCardOrderCustomData()
        {
            GiftCardUsages = new List<GiftCardUsage>();
        }

        public static GiftCardOrderCustomData FromXML(string XML)
        {
            string Checksum = GetHash(XML);
            return CacheHelper.Cache<GiftCardOrderCustomData>(cs => FromXMLCache(cs, XML), new CacheSettings(1440, "GiftCardOrderCustomDataFromXML", false, Checksum));
        }

        public static GiftCardOrderCustomData FromXMLCache(CacheSettings cs, string XML)
        {
            try
            {
                GiftCardOrderCustomData RestoredObject = new GiftCardOrderCustomData();
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(XML);
                // Remember, the paths are Case Sensitive!
                foreach (XmlNode GiftCardUsageXML in XmlDoc.SelectNodes("/GiftCardCustomData/GiftCardUsages/GiftCardUsage"))
                {
                    RestoredObject.GiftCardUsages.Add(GiftCardUsage.FromXML(GiftCardUsageXML.OuterXml, true));
                }

                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency("GiftCardCustomDataFromXML|" + GetHash(XML));
                }
                return RestoredObject;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("GiftCardOrderCustomData", "ErrorParsingFromXML", ex, SiteContext.CurrentSiteID, additionalMessage: "Could not parse XML: " + XML.Replace("<", "&gt;").Replace(">", "&lt;"));
            }
            return null;
        }

        public decimal GetTotalWishedToDiscount()
        {
            return GiftCardUsages.Sum(x => x.Amount);
        }

        public GiftCardDiscountCustomData CreateGiftCardDiscount(decimal OrderTotalAfterTax, decimal OrderShippingAfterTax)
        {
            // Order the Gift cards by expiration date, then if the Amount = the remaining amount, then the amount.
            GiftCardUsages = GiftCardUsages.OrderBy(x => (x.GiftCard.ExpirationDate == new DateTime() ? new DateTime(3000, 1, 1) : x.GiftCard.ExpirationDate))
                .ThenBy(x => Convert.ToInt16(x.Amount != x.GiftCard.AmountRemaining))
                .ThenBy(x => x.Amount).ToList();
            GiftCardDiscountCustomData GiftCardDiscountObject = new GiftCardDiscountCustomData();
            foreach (GiftCardUsage GCUsage in GiftCardUsages)
            {
                bool GiftCardUsed = false;
                if (OrderTotalAfterTax > 0) {
                    if (OrderTotalAfterTax >= GCUsage.Amount)
                    {
                        GiftCardUsed = true;
                        OrderTotalAfterTax -= GCUsage.Amount;
                        GiftCardDiscountObject.GiftCardUsages.Add(new GiftCardUsage(GCUsage.GiftCard.Clone(), GCUsage.Amount, GiftCardUsedOnEnum.Products));
                    }
                    else
                    {
                        GiftCardUsed = true;
                        GiftCardDiscountObject.GiftCardUsages.Add(new GiftCardUsage(GCUsage.GiftCard.Clone(), OrderTotalAfterTax, GiftCardUsedOnEnum.Products));
                        OrderTotalAfterTax = 0;
                    }
                }
                if (OrderTotalAfterTax == 0 && OrderShippingAfterTax > 0)
                {
                    if (OrderShippingAfterTax >= GCUsage.Amount)
                    {
                        GiftCardUsed = true;
                        OrderShippingAfterTax -= GCUsage.Amount;
                        GiftCardDiscountObject.GiftCardUsages.Add(new GiftCardUsage(GCUsage.GiftCard.Clone(), GCUsage.Amount, GiftCardUsedOnEnum.Shipping));
                    }
                    else
                    {
                        GiftCardUsed = true;
                        GiftCardDiscountObject.GiftCardUsages.Add(new GiftCardUsage(GCUsage.GiftCard.Clone(), OrderShippingAfterTax, GiftCardUsedOnEnum.Shipping));
                        OrderShippingAfterTax = 0;
                    }
                }
                if(!GiftCardUsed)
                {
                    // Must still have a record of it, so add with 0 dollars
                    GiftCardDiscountObject.GiftCardUsages.Add(new GiftCardUsage(GCUsage.GiftCard.Clone(), 0, GiftCardUsedOnEnum.Products));
                }
            }
            return GiftCardDiscountObject;
        }
    }

    /// <summary>
    /// This is serialized into the Cart Data to show what Gift Card amounts were actually applied.  After order placed use this to deduct from gift cards.
    /// </summary>
    [Serializable]
    public class GiftCardDiscountCustomData : GiftCardCustomData
    {
        public const string GiftCardDiscountCustomDataKey = "GiftCardDiscountCustomData";
        public const string GiftCardDiscountApplied = "GiftCardDiscountApplied";
        public GiftCardDiscountCustomData()
        {
            GiftCardUsages = new List<GiftCardUsage>();
        }
        
        public static GiftCardDiscountCustomData FromXML(string XML)
        {
            string Checksum = GetHash(XML);
            return CacheHelper.Cache<GiftCardDiscountCustomData>(cs => FromXMLCache(cs, XML), new CacheSettings(1440, "GiftCardDiscountCustomDataFromXML", false, Checksum));
        }

        public static GiftCardDiscountCustomData FromXMLCache(CacheSettings cs, string XML)
        {
            try
            {
                GiftCardDiscountCustomData RestoredObject = new GiftCardDiscountCustomData();
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(XML);
                // Remember, the paths are Case Sensitive!
                foreach (XmlNode GiftCardUsageXML in XmlDoc.SelectNodes("/GiftCardCustomData/GiftCardUsages/GiftCardUsage"))
                {
                    RestoredObject.GiftCardUsages.Add(GiftCardUsage.FromXML(GiftCardUsageXML.OuterXml, false));
                }

                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency("GiftCardCustomDataFromXML|" + GetHash(XML));
                }

                return RestoredObject;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("GiftCardDiscountCustomData", "ErrorParsingFromXML", ex, SiteContext.CurrentSiteID, additionalMessage: "Could not parse XML: " + XML.Replace("<", "&gt;").Replace(">", "&lt;"));
            }
            return null;
        }

        /// <summary>
        /// Gets the total amount to Discount from the Post-Tax order total
        /// </summary>
        /// <returns>The Total Discounted amount</returns>
        public decimal GetTotalDiscounted(decimal MaxAmount = -1, GiftCardUsedOnEnum UsedOn = GiftCardUsedOnEnum.Either)
        {
            decimal Sum = GiftCardUsages.Where(x => UsedOn == GiftCardUsedOnEnum.Either || x.UsedOn == UsedOn).Sum(x => x.Amount);
            if (MaxAmount >= 0 && Sum > MaxAmount)
            {
                return MaxAmount;
            }
            else
            {
                return Sum;
            }
        }

        
    }

    /// <summary>
    /// This is serialized into the Cart Data to show what Gift Card and amounts the user wishes to apply to their order.  Use this to build the end Order Discount
    /// </summary>
    [Serializable]
    public class GiftCardUsage : IDataContainer
    {
        /// <summary>
        /// The Gift Card object
        /// </summary>
        public GiftCardInfo GiftCard;
        /// <summary>
        /// The Amount either Wished to Deduct, or Deducted depending on the context.
        /// </summary>
        public decimal Amount;
        public GiftCardUsedOnEnum UsedOn;

        /// <summary>
        /// Construction Method
        /// </summary>
        private GiftCardUsage()
        {

        }

        /// <summary>
        /// Constructor given the Gift Card and the Amount
        /// </summary>
        /// <param name="GiftCard">The Gift Card</param>
        /// <param name="Amount">The Amount either wished to deduct, or amount actually deducted depending on the context.</param>
        public GiftCardUsage(GiftCardInfo GiftCard, decimal Amount, GiftCardUsedOnEnum UsedOn)
        {
            this.GiftCard = GiftCard;
            this.Amount = Amount;
            this.UsedOn = UsedOn;
        }

        /// <summary>
        /// Custom Serializer, used to store an XML representation.
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            return string.Format(@"
<GiftCardUsage>
    <GiftCard>
        {0}
    </GiftCard>
    <Amount>{1}</Amount>
    <UsedOn>{2}</UsedOn>
</GiftCardUsage>",
GiftCard != null ? SerializationExtensions.Serialize(GiftCard).OuterXml : "",
Amount, GiftCardUsedOnEnumToString(UsedOn));
        }

        /// <summary>
        /// Custom Deserializer, constructs the GiftCardUsage object from the XML.
        /// </summary>
        /// <param name="XML">The XML of the class</param>
        /// <param name="ReloadFromDatabase">If true, reloads Gift Card from Database, otherwise does not.</param>
        /// <returns></returns>
        public static GiftCardUsage FromXML(string XML, bool ReloadFromDatabase = false)
        {
            string Checksum = GetHash(XML);
            return CacheHelper.Cache<GiftCardUsage>(cs => FromXMLCache(cs, XML, ReloadFromDatabase), new CacheSettings(1440, "FromXML", false, Checksum));
        }

        /// <summary>
        /// Helper method to Restore from XML the AccentsShoppingCartItemCustomData
        /// </summary>
        /// <param name="cs">Cache Helper</param>
        /// <param name="XML">The XML Serialized version of this Accents Shopping Cart Item Custom Data</param>
        /// <param name="ReloadFromDatabase">If to attempt to reload from the database.</param>
        /// <param name="SKU">The SKU Object, needed to restore price from database.  Null otherwise.</param>
        /// <param name="Cart">The Cart Object, needed to restore price from database.  Null otherwise.</param>
        /// <returns></returns>
        private static GiftCardUsage FromXMLCache(CacheSettings cs, string XML, bool ReloadFromDatabase)
        {
            try
            {
                GiftCardUsage RestoredObject = new GiftCardUsage();
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(XML);
                // Remember, the paths are Case Sensitive!

                RestoredObject.Amount = ValidationHelper.GetDecimal(GetInnerText(XmlDoc.SelectSingleNode("/GiftCardUsage/Amount")), 0);
                RestoredObject.UsedOn = StringToGiftCardUsedOnEnum(ValidationHelper.GetString(GetInnerText(XmlDoc.SelectSingleNode("/GiftCardUsage/UsedOn")), "Products"));
                var GiftCardUsageXml = XmlDoc.SelectSingleNode("/GiftCardUsage/GiftCard/hbs_giftcards.giftcard");
                if (GiftCardUsageXml != null)
                {
                    var Results = SerializationExtensions.Deserialize((XmlElement)GiftCardUsageXml);
                    RestoredObject.GiftCard = (Results.IsValid ? (GiftCardInfo)Results.DeserializedInfo : null);
                }
                if (ReloadFromDatabase)
                {
                    List<string> CacheDependencies = new List<string>();
                    if (RestoredObject.GiftCard != null)
                    {
                        var DatabaseInfo = GiftCardInfoProvider.GetGiftCardInfo(RestoredObject.GiftCard.GiftCardGuid);
                        RestoredObject.GiftCard = (DatabaseInfo == null ? RestoredObject.GiftCard : DatabaseInfo);
                        if (RestoredObject.Amount > RestoredObject.GiftCard.AmountRemaining)
                        {
                            RestoredObject.Amount = RestoredObject.GiftCard.AmountRemaining;
                        }
                        CacheDependencies.Add("hbs_giftcard.giftcard|byguid|" + RestoredObject.GiftCard.GiftCardGuid);
                    }
                    if (cs.Cached)
                    {
                        cs.CacheDependency = CacheHelper.GetCacheDependency(CacheDependencies.ToArray());
                    }
                }
                else if (cs.Cached)
                {
                    // nothing
                }
                return RestoredObject;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("GiftCardOrderCustomData_GiftCardUsage", "ErrorParsingFromXML", ex, SiteContext.CurrentSiteID, additionalMessage: "Could not parse XML: " + XML.Replace("<", "&gt;").Replace(">", "&lt;"));
            }
            return null;
        }

        /// <summary>
        /// Helper function to get the node's inner text, null if not found.
        /// </summary>
        /// <param name="Node"></param>
        /// <returns></returns>
        private static string GetInnerText(XmlNode Node)
        {
            if (Node != null)
            {
                return Node.InnerText;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Helper function to determine if the XML has changed for the FromXML build
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        private static string GetHash(string Value)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(Value);
            byte[] hash = sha1.ComputeHash(buf, 0, buf.Length);
            return System.BitConverter.ToString(hash).Replace("-", "");
        }

        private static string GiftCardUsedOnEnumToString(GiftCardUsedOnEnum UsedOn) 
        {
            switch(UsedOn)
            {
                case GiftCardUsedOnEnum.Products:
                    return "Products";
                    case GiftCardUsedOnEnum.Shipping:
                    return "Shipping";
                case GiftCardUsedOnEnum.Either:
                default:
                    return "Either";
            }
        }

        private static GiftCardUsedOnEnum StringToGiftCardUsedOnEnum(string UsedOn)
        {
            switch (UsedOn)
            {
                case "Products":
                    return GiftCardUsedOnEnum.Products;
                case "Shipping":
                    return GiftCardUsedOnEnum.Shipping;
                case "Either":
                default:
                    return GiftCardUsedOnEnum.Either;
            }
        }

        #region "IDataContainer Methods"

        public List<string> ColumnNames
        {
            get
            {
                return new List<string>() { "Amount", "GiftCard", "UsedOn" };
            }
        }

        public object this[string columnName]
        {
            get
            {
                return GetValue(columnName);
            }

            set
            {
                SetValue(columnName, value);
            }
        }

        public bool TryGetValue(string columnName, out object value)
        {
            try
            {
                value = GetValue(columnName);
                return true;
            }
            catch (Exception ex)
            {
                value = null;
                return false;
            }
        }

        public bool ContainsColumn(string columnName)
        {
            return ColumnNames.Exists(x => x.ToLower() == columnName.ToLower());
        }

        public object GetValue(string columnName)
        {
            switch (columnName.ToLower())
            {
                case "amount":
                    return Amount;
                case "giftcard":
                    return GiftCard;
                case "usedon":
                    return UsedOn;
            }
            return null;
        }

        public bool SetValue(string columnName, object value)
        {
            switch (columnName.ToLower())
            {
                case "amount":
                    Amount = (decimal)value;
                    return true;
                case "giftcard":
                    GiftCard = (GiftCardInfo)value;
                    return true;
                case "usedon":
                    UsedOn = (GiftCardUsedOnEnum)value;
                    return true;
            }
            return false;
        }

        #endregion  
    }
    public enum GiftCardUsedOnEnum
    {
        Products, Shipping, Either
    }

}

