using CMS.Ecommerce;
using CMS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS_GiftCards
{
    public class GiftCardHelper
    {
        /// <summary>
        /// Gets the Shipping Tax Rate so you can adjust the Gift Card Discount displays and such (Gift Card Shipping Discount / (1+ThisRate/100))
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public static decimal GetShippingTaxRate(ShoppingCartInfo cart)
        {
            // Gets the Tax info, which include TaxClassID, TaxClassDisplayName, TaxClassZeroIfIDSupplied TaxValue TaxIsGlobal
            decimal ActualTaxRate = 0;
            decimal TotalTaxRate = 0;
            decimal TotalTaxRateIfTaxExempt = 0;
            foreach (DataRow dr in TaxClassInfoProvider.GetShippingTaxes(cart).Tables[0].Rows)
            {
                TotalTaxRate += ValidationHelper.GetDecimal(dr["TaxValue"], 0);
                if (!ValidationHelper.GetBoolean(dr["TaxClassZeroIfIDSupplied"], true))
                {
                    TotalTaxRateIfTaxExempt += ValidationHelper.GetDecimal(dr["TaxValue"], 0);
                }
            }

            // Tax rate based solely on the customer
            ActualTaxRate = !string.IsNullOrWhiteSpace(cart.Customer.CustomerTaxRegistrationID) ? TotalTaxRateIfTaxExempt : TotalTaxRate;
            return ActualTaxRate;
        }

        /// <summary>
        /// Given the Gift Card amount and the tax rate, will return the amount that needs to be discounted from the pre-tax order.
        /// </summary>
        /// <param name="GiftCardAmountApplied">The amount from the gift card that will be applied</param>
        /// <param name="TaxRate">The Tax Rate for the order. May get incorrect amounts if tax is not consistant throughout order.</param>
        /// <param name="TaxRateIsPercent">If true, the Tax Rate is treated as a percent (ex 5 => 5%) and will be divided by 100.</param>
        /// <param name="Currency">If provided, will round the discount to the amount of that currency's CurrencyRoundTo value.  Otherwise will not round.</param>
        public static decimal GetGiftCardDiscountAmount(decimal GiftCardAmountApplied, decimal TaxRate, bool TaxRateIsPercent = true, CurrencyInfo Currency = null)
        {
            decimal Discount = (GiftCardAmountApplied / (1 + (TaxRateIsPercent ? TaxRate/100 : TaxRate)));
            if (Currency != null)
            {
                return Math.Round(Discount, Currency.CurrencyRoundTo);
            } else
            {
                return Discount;
            }
        }

        /// <summary>
        /// Determins if Gift Cards can be used based on there either being no taxes or all items in the cart have the same tax (and thus same tax rate)
        /// </summary>
        /// <param name="Cart"></param>
        /// <returns></returns>
        public static bool CanUseGiftCards(ShoppingCartInfo Cart)
        {
            bool CustomerIsTaxExempt = (Cart.Customer != null && !string.IsNullOrWhiteSpace(Cart.Customer.CustomerTaxRegistrationID));
            if (Cart.CartItems.Count == 0)
            {
                return true;
            }
            List<string> CheckProductTaxIDs = new List<string>();
            bool IsFirst = true;
            foreach (ShoppingCartItemInfo CartItem in Cart.CartItems)
            {
                List<string> ThisProductsTaxIDs = new List<string>();
                foreach (ItemTax ProductTax in CartItem.ProductTaxes)
                {
                    if(!((CustomerIsTaxExempt && ProductTax.ItemTaxZeroIfIDSupplied) || ProductTax.ItemTaxValue == 0))
                    {
                        ThisProductsTaxIDs.Add(ProductTax.ItemTaxID);
                        if(IsFirst)
                        {
                            CheckProductTaxIDs.Add(ProductTax.ItemTaxID);
                        }
                    }
                }
                if(!IsFirst)
                {
                    // If there are any Positive tax values where the Tax is not in the original check, then cannot apply gift cards since different taxes exist.
                    if(ThisProductsTaxIDs.Count != ThisProductsTaxIDs.Union(CheckProductTaxIDs).Distinct().Count())
                    {
                        return false;
                    }
                } else
                {
                    // Distinct the items
                    CheckProductTaxIDs = CheckProductTaxIDs.Distinct().ToList();
                }
                IsFirst = false;
            }

            return true;
        }

        /// <summary>
        /// Cached Get Gift Card Info
        /// </summary>
        /// <param name="GiftCardCode">The Gift Card Code</param>
        /// <param name="SiteName">Optional the Site Name</param>
        /// <returns>The Gift Card Info</returns>
        public static GiftCardInfo GetGiftCard(string GiftCardCode, string SiteName = null)
        {
            return CacheHelper.Cache<GiftCardInfo>(cs => GetGiftCard(cs, GiftCardCode, SiteName), new CacheSettings(1440, "GetGiftCard", GiftCardCode, SiteName));
        }

        /// <summary>
        /// Private cache helper function
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="GiftCardCode"></param>
        /// <param name="SiteName"></param>
        /// <returns></returns>
        private static GiftCardInfo GetGiftCard(CacheSettings cs, string GiftCardCode, string SiteName)
        {
            GiftCardInfo GC = GiftCardInfoProvider.GetGiftCardInfo(GiftCardCode, SiteName);
            if(GC != null && cs.Cached)
            {
                cs.CacheDependency = CacheHelper.GetCacheDependency("HBS_GiftCard.GiftCard|byid|" + GC.GiftCardID);
            }
            return GC;
        }

        /// <summary>
        /// Cached Get Gift Card Info
        /// </summary>
        /// <param name="GiftCardID">The Gift Card ID</param>
        /// <returns>The Gift Card Info</returns>
        public static GiftCardInfo GetGiftCard(int GiftCardID)
        {
            return CacheHelper.Cache<GiftCardInfo>(cs => GetGiftCard(cs, GiftCardID), new CacheSettings(1440, "GetGiftCard", GiftCardID));
        }

        /// <summary>
        /// Private cache helper function
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="GiftCardID"></param>
        /// <returns></returns>
        private static GiftCardInfo GetGiftCard(CacheSettings cs, int GiftCardID)
        {
            GiftCardInfo GC = GiftCardInfoProvider.GetGiftCardInfo(GiftCardID);
            if (GC != null && cs.Cached)
            {
                cs.CacheDependency = CacheHelper.GetCacheDependency("HBS_GiftCard.GiftCard|byid|" + GC.GiftCardID);
            }
            return GC;
        }

        public static DataTable GetGiftCardDataTable(int OrderID)
        {
            OrderInfo OrderObject = OrderInfoProvider.GetOrderInfo(OrderID);
            object GiftCardDataXml = OrderObject.OrderCustomData.GetValue(GiftCardDiscountCustomData.GiftCardDiscountCustomDataKey);
            if (GiftCardDataXml != null)
            {
                GiftCardDiscountCustomData Data = GiftCardDiscountCustomData.FromXML((string)GiftCardDataXml);
                return Data.GetGiftCardUsageDataTable();
            }
            else
            {
                return new DataTable();
            }
        }


        /// <summary>
        /// For the given Order, gets the grand total Gift Card amount used.
        /// </summary>
        /// <param name="OrderID">Order ID</param>
        /// <returns>The Amout</returns>
        public static decimal GetTotalGiftCardsUsed(int OrderID)
        {

            return GiftCardHelper.GetGiftCardsUsedOnOrder(OrderID) + GiftCardHelper.GetGiftCardsUsedOnShipping(OrderID);
        }

        /// <summary>
        /// For the given Order, gets the amount of Gift Cards used on the Order (not shipping)
        /// </summary>
        /// <param name="OrderID">Order ID</param>
        /// <returns>The amount used on the Order (not shipping)</returns>
        public static decimal GetGiftCardsUsedOnOrder(int OrderID)
        {
            OrderInfo OrderObj = OrderInfoProvider.GetOrderInfo(OrderID);
            string GCDText = (string)OrderObj.OrderCustomData.GetValue(GiftCardDiscountCustomData.GiftCardDiscountCustomDataKey);
            if(!string.IsNullOrWhiteSpace(GCDText))
            {
                GiftCardDiscountCustomData GiftCardObj = GiftCardDiscountCustomData.FromXML(GCDText);
                return GiftCardObj.GiftCardUsages.Where(x => x.UsedOn == GiftCardUsedOnEnum.Products).Sum(x => x.Amount);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// For the given Order, gets the amount of Gift Cards used on the Shipping
        /// </summary>
        /// <param name="OrderID">Order ID</param>
        /// <returns>The amount used on the Shipping</returns>
        public static decimal GetGiftCardsUsedOnShipping(int OrderID)
        {
            OrderInfo OrderObj = OrderInfoProvider.GetOrderInfo(OrderID);
            string GCDText = (string)OrderObj.OrderCustomData.GetValue(GiftCardDiscountCustomData.GiftCardDiscountCustomDataKey);
            if (!string.IsNullOrWhiteSpace(GCDText))
            {
                GiftCardDiscountCustomData GiftCardObj = GiftCardDiscountCustomData.FromXML(GCDText);
                return GiftCardObj.GiftCardUsages.Where(x => x.UsedOn == GiftCardUsedOnEnum.Shipping).Sum(x => x.Amount);
            } else
            {
                return 0;
            }
        }

        /// <summary>
        /// Disables gift cards that are expired, used primarily as a Scheduled Task
        /// </summary>
        /// <param name=""></param>
        public static void ExpireGiftCards(ref string Results)
        {
            int Count = 0;
            GiftCardInfoProvider.GetGiftCards()
                .WhereNotNull("ExpirationDate")
                .WhereLessThan("ExpirationDate", DateTime.Now)
                .WhereEquals("Enabled", true)
                .ForEachObject(x =>
                {
                    ((GiftCardInfo)x).Enabled = false;
                    x.Update();
                    Count++;
                });
            Results = string.Format("{0} Cards Expired and Disabled", Count);

        }

    }

}
