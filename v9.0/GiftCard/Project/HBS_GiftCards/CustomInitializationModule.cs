using CMS;
using CMS.DataEngine;
using System;
using CMS.Membership;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.EventLog;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using HBS_GiftCards;
using CMS.Base;
using CMS.SiteProvider;

// Registers the custom module into the system
[assembly: RegisterModule(typeof(HBS_GiftCardsCustomInitializationModule))]

namespace HBS_GiftCards
{
    class HBS_GiftCardsCustomInitializationModule : Module
    {
        // Module class constructor, the system registers the module under the name "CustomInit"
        public HBS_GiftCardsCustomInitializationModule()
        : base("HBS_GiftCards_CustomInit")
        {
        }

        // Contains initialization code that is executed when the application starts
        protected override void OnInit()
        {
            base.OnInit();
            EcommerceEvents.OrderPaid.Execute += OrderPaid_Execute;

            // Add Gift Card Usage info to the OrderInfo class
            OrderInfo.TYPEINFO.OnLoadRelatedData += Order_OnLoadRelatedData;
        }

        private object Order_OnLoadRelatedData(BaseInfo infoObj)
        {
            OrderInfo OrderObj = (OrderInfo)infoObj;
            string CustomData = (string)OrderObj.OrderCustomData.GetValue(GiftCardDiscountCustomData.GiftCardDiscountCustomDataKey);
            GiftCardDiscountCustomData GiftCardCustomData = (!string.IsNullOrWhiteSpace(CustomData) ? GiftCardDiscountCustomData.FromXML(CustomData) : new GiftCardDiscountCustomData());
            //OrderObj.ColumnNames.AddRange(GiftCardCustomData.ColumnNames.Except(OrderObj.ColumnNames));
            return GiftCardCustomData;
        }

        private void OrderPaid_Execute(object sender, OrderPaidEventArgs e)
        {
            RecursionControl OrderPaid = new RecursionControl("GiftCards_OrderPaid_" + e.Order.OrderID);
            if (OrderPaid.Continue)
            {
                // Check order custom data for Gift Card Discount
                object GiftCardCustDataobj = e.Order.OrderCustomData.GetValue(GiftCardDiscountCustomData.GiftCardDiscountCustomDataKey);
                if (GiftCardCustDataobj != null)
                {
                    GiftCardDiscountCustomData GiftCardUsageObject = GiftCardDiscountCustomData.FromXML(ValidationHelper.GetString(GiftCardCustDataobj, ""));
                    // Loop through gift cards, and reverify amounts on all of them before adjusting.
                    bool GiftCardsValid = true;

                    // Create a dictionary of the GiftCard Code and total amount since Usage may be split between products and taxes
                    Dictionary<string, decimal> GiftCardCodeToAmount = new Dictionary<string, decimal>();

                    foreach(GiftCardUsage CardUsage in GiftCardUsageObject.GiftCardUsages)
                    {
                        if(!GiftCardCodeToAmount.ContainsKey(CardUsage.GiftCard.GiftCardCode))
                        {
                            GiftCardCodeToAmount.Add(CardUsage.GiftCard.GiftCardCode, 0);
                        }
                        GiftCardCodeToAmount[CardUsage.GiftCard.GiftCardCode] += CardUsage.Amount;
                    }

                    foreach (string GiftCardCode in GiftCardCodeToAmount.Keys)
                    {
                        if (GiftCardsValid)
                        {
                            GiftCardInfo GiftCardObj = GiftCardInfoProvider.GetGiftCardInfo(GiftCardCode);
                            if (GiftCardObj == null)
                            {
                                GiftCardsValid = false;
                                EventLogProvider.LogEvent("E", "HBS_GiftCards.OrderPaid", "GiftCardMissing", eventDescription: "The gift card of code " + GiftCardCode + " no longer exist and thus cannot be used on the order with ID " + e.Order.OrderID);
                            }
                            else if (GiftCardObj.AmountRemaining < GiftCardCodeToAmount[GiftCardCode])
                            {
                                GiftCardsValid = false;
                                EventLogProvider.LogEvent("E", "HBS_GiftCards.OrderPaid", "GiftCardBalanceLowerThanRequestedAmount", eventDescription: "The gift card of code " + GiftCardCode + " has a balance lower than the amount requested for order ID " + e.Order.OrderID + ", this can occur if they used the gift card in between ordering and paying.");
                            }
                        }
                    }
                    if (GiftCardsValid)
                    {
                        // Now loop through, charge the Gift Cards and disable any that need to be if set.
                        bool DisableZeroBalanceGCs = SettingsKeyInfoProvider.GetBoolValue("DisableGiftCardsUponZeroBalance", new SiteInfoIdentifier(SiteContext.CurrentSiteID), true);
                        foreach (string GiftCardCode in GiftCardCodeToAmount.Keys)
                        {
                            GiftCardInfo GiftCardObj = GiftCardInfoProvider.GetGiftCardInfo(GiftCardCode);
                            GiftCardObj.AmountRemaining -= GiftCardCodeToAmount[GiftCardCode];
                            if (GiftCardObj.AmountRemaining == 0 && DisableZeroBalanceGCs)
                            {
                                GiftCardObj.Enabled = false;
                            }
                            GiftCardObj.Update();
                            // create order history note
                            GiftCardUsageHistoryInfo GiftCardHistoryObj = new GiftCardUsageHistoryInfo();
                            GiftCardHistoryObj.GiftCardID = GiftCardObj.GiftCardID;
                            GiftCardHistoryObj.GiftCardUsageHistoryCreated = DateTime.Now;
                            GiftCardHistoryObj.GiftCardUsageHistoryGuid = Guid.NewGuid();
                            GiftCardHistoryObj.GiftCardUsageHistoryLastModified = DateTime.Now;
                            GiftCardHistoryObj.Amount = Convert.ToDecimal(GiftCardCodeToAmount[GiftCardCode]);
                            GiftCardHistoryObj.NewBalance = GiftCardObj.AmountRemaining;
                            GiftCardHistoryObj.OrderID = e.Order.OrderID;
                            GiftCardHistoryObj.AmountIsDeduction = true;
                            GiftCardHistoryObj.Insert();
                        }
                    }
                    else
                    {
                        // Set order to errored
                        OrderStatusInfo ErrorStatus = OrderStatusInfoProvider.GetOrderStatusInfo(DataHelper.GetNotEmpty(SettingsKeyInfoProvider.GetValue("GiftCardErrorStatus", new SiteInfoIdentifier(SiteContext.CurrentSiteID)), "PaymentFailed"), SiteContext.CurrentSiteName);
                        if (ErrorStatus == null)
                        {
                            EventLogProvider.LogEvent("E", "HBS_GiftCards.OrderPaid", "NoErrorStatusSet", eventDescription: "Could not set Order with ID " + e.Order.OrderID + " to an Errored status due to Gift Cards not being valid.  Please go to Settings -> Ecommerce -> Gift Cards and set the proper order status for when errors occur.");
                        }
                        else
                        {
                            // Set this to an errored state.
                            e.Order.OrderStatusID = ErrorStatus.StatusID;
                            e.Order.OrderIsPaid = false;
                            OrderInfoProvider.SetOrderInfo(e.Order);
                        }
                    }
                }

            }
        }
    }

}
