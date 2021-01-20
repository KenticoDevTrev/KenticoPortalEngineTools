using CMS.Base.Web.UI;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using HBS_GiftCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CMSModules_HBS_GiftCards_FormControls_GiftCardEntry_GiftCardEntry : CMSCheckoutWebPart
{
    private bool IsUIMode = false;

    public string GiftCardHeader
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GiftCardHeader"), "");
        }
        set
        {
            SetValue("GiftCardHeader", value);
        }
    }

    public bool ShowPriceBefore
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPriceBefore"), true);
        }
        set
        {
            SetValue("ShowPriceBefore", value);
        }
    }
    public bool ShowPriceAfter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPriceAfter"), true);
        }
        set
        {
            SetValue("ShowPriceAfter", value);
        }
    }
    public bool OnlyShowAfterPriceIfGiftCardsApplied
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("OnlyShowAfterPriceIfGiftCardsApplied"), true);
        }
        set
        {
            SetValue("OnlyShowAfterPriceIfGiftCardsApplied", value);
        }
    }

    public string HTMLBeforeGiftCard
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HTMLBeforeGiftCard"), "");
        }
        set
        {
            SetValue("HTMLBeforeGiftCard", value);
        }
    }

    public string HTMLInbetweenGiftCard
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HTMLInbetweenGiftCard"), "");
        }
        set
        {
            SetValue("HTMLInbetweenGiftCard", value);
        }
    }

    public string HTMLAfterGiftCard
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HTMLAfterGiftCard"), "");
        }
        set
        {
            SetValue("HTMLAfterGiftCard", value);
        }
    }


    public int OrderID
    {
        get
        {
            // If it's in the Order > Gift Card user interface, grab from URL Parameter.
            if (IsUIElement())
            {
                return ValidationHelper.GetInteger(URLHelper.GetUrlParameter(HttpContext.Current.Request.Url.AbsoluteUri, "orderid"), -1);
            } else
            {
                return ValidationHelper.GetInteger(GetValue("OrderID"), -1);
            }
        }
        set
        {
            SetValue("OrderID", value);
        }
    }

    private bool IsUIElement()
    {
        return (UIContext.UIElement != null && UIContext.UIElement.ElementGUID.ToString().ToLower() == "6dd655fc-f22b-4a73-ac47-b78ae6228d25");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        HideErrors();
        if(IsUIElement())
        {
            CssRegistration.RegisterBootstrap(this.Page);
            pnlUIOnly.Visible = true;
        }
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        // Reset the card data in case pricing changed
        GiftCardOrderCustomData CardData = GetGiftCardOrderData();
        SetGiftCardOrderData(CardData.CreateGiftCardDiscount(GetItemsTotalBeforeGiftCards(), GetShippingTotalBeforeGiftCards()));
        SetExistingGiftCards();
        if (IsPaidOrder())
        {
            ltrOrderAlreadyPaid.Visible = true;
            pnlError.Visible = true;
            return;
        }
    }

    private void SetExistingGiftCards()
    {
        // Grab current Gift cards and display in panel.
        GiftCardOrderCustomData CardData = GetGiftCardOrderData();
        pnlAppliedGiftCards.Controls.Clear();
        GiftCardDiscountCustomData CardDiscountData = CardData.CreateGiftCardDiscount(GetItemsTotalBeforeGiftCards(), GetShippingTotalBeforeGiftCards());

        Dictionary<string, decimal> GiftCardCodeToAmount = new Dictionary<string, decimal>();
        CardDiscountData.GiftCardUsages.ForEach(x => {
            if (!GiftCardCodeToAmount.ContainsKey(x.GiftCard.GiftCardCode)) {
                GiftCardCodeToAmount.Add(x.GiftCard.GiftCardCode, x.Amount);
            } else {
                GiftCardCodeToAmount[x.GiftCard.GiftCardCode] += x.Amount;
                }
            });

        foreach (GiftCardUsage GCU in CardData.GiftCardUsages)
        {
            pnlAppliedGiftCards.Controls.Add(new Literal()
            {
                Text = string.Format("<div class=\"AppliedCard\">{0}{1} [{2}] {3} {4} {5}", 
                HTMLBeforeGiftCard,
                GCU.GiftCard.GiftCardDisplayName, 
                GCU.GiftCard.GiftCardCode, 
                HTMLInbetweenGiftCard,
                string.Format(ECommerceContext.CurrentCurrency.CurrencyFormatString, GiftCardCodeToAmount[GCU.GiftCard.GiftCardCode]),
                (GiftCardCodeToAmount[GCU.GiftCard.GiftCardCode] != GCU.Amount ? string.Format("[{0} {1}]", ResHelper.LocalizeString("{$ hbs_giftcard.oforiginalamount $}"), string.Format(ECommerceContext.CurrentCurrency.CurrencyFormatString, GCU.Amount) ) : ""))
            });
            LinkButton RemoveBtn = new LinkButton()
            {
                ID = "GiftCard_Remove_" + GCU.GiftCard.GiftCardID,
                Text = "X",
                CssClass = "RemoveGiftCard",
                ToolTip = "Remove Gift Card"
            };
            RemoveBtn.Click += RemoveBtn_Click;
            pnlAppliedGiftCards.Controls.Add(RemoveBtn);
            pnlAppliedGiftCards.Controls.Add(new Literal()
            {
                Text = string.Format("{0}</div>", HTMLAfterGiftCard)
            });
        }
    }

    /// <summary>
    /// Validates the data.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">The StepEventArgs instance containing the event data.</param>
    protected override void ValidateStepData(object sender, StepEventArgs e)
    {
        base.ValidateStepData(sender, e);
    }
    protected override void SaveStepData(object sender, StepEventArgs e)
    {
        // Get current Gift Card data and finalize
        GiftCardOrderCustomData CardData = GetGiftCardOrderData();
        GiftCardDiscountCustomData CardDiscountData = CardData.CreateGiftCardDiscount(GetItemsTotalBeforeGiftCards(), GetShippingTotalBeforeGiftCards());
        SetGiftCardOrderData(CardData);
        SetGiftCardOrderData(CardDiscountData);
    }

    protected override void OnPreRender(EventArgs e)
    {
        // If there is no price, don't even show the control
        decimal PriceBeforeGiftCards = GetGrandTotalBeforeGiftCards();
        if(PriceBeforeGiftCards == 0)
        {
            pnlGiftCardEntryContainer.Visible = false;
        } else {
            ltrGiftCardHeader.Text = GiftCardHeader;
            pnlValueBefore.Visible = ShowPriceBefore;
            if (ShowPriceBefore)
            {
                ltrPriceBefore.Text = ECommerceContext.CurrentCurrency.FormatPrice(Convert.ToDouble(GetGrandTotalBeforeGiftCards()));
            }
            bool ShowAfterPrice = (ShowPriceBefore && (!OnlyShowAfterPriceIfGiftCardsApplied || GetGiftCardOrderData().GiftCardUsages.Sum(x => x.Amount) > 0));
            pnlValueAfter.Visible = ShowAfterPrice;
            if (ShowAfterPrice)
            {
                ltrPriceAfter.Text = ECommerceContext.CurrentCurrency.FormatPrice(Convert.ToDouble(GetGrandTotalAfterGiftCards()));
            }
        }
    }

    private void RemoveBtn_Click(object sender, EventArgs e)
    {
        LinkButton ButtonControl = (LinkButton)sender;
        int GiftCardID = ValidationHelper.GetInteger(ButtonControl.ID.Replace("GiftCard_Remove_", ""), -1);
        GiftCardOrderCustomData CardData = GetGiftCardOrderData();
        GiftCardDiscountCustomData CardDiscountData = GetGiftCardDiscountOrderData();
        GiftCardInfo GiftCardObject = GiftCardInfoProvider.GetGiftCardInfo(GiftCardID);

        CardData.GiftCardUsages.RemoveAll(x => x.GiftCard.GiftCardCode == GiftCardObject.GiftCardCode);
        CardDiscountData.GiftCardUsages.RemoveAll(x => x.GiftCard.GiftCardCode == GiftCardObject.GiftCardCode);
        SetGiftCardOrderData(CardData);
        SetGiftCardOrderData(CardDiscountData);
        SetExistingGiftCards();
    }

    protected void btnApplyGiftCard_Click(object sender, EventArgs e)
    {
        HideErrors();

        // Get current cart's Gift Cards, apply the code if one is found
        GiftCardOrderCustomData CardData = GetGiftCardOrderData();

        GiftCardInfo GiftCard = GiftCardInfoProvider.GetGiftCardInfo(tbxGiftCardCode.Text.Trim());
        if (GiftCard == null)
        {
            pnlError.Visible = true;
            ltrNoGiftCardFound.Visible = true;
        }
        else
        {
            if (GiftCard.AmountRemaining == 0)
            {
                pnlError.Visible = true;
                ltrGiftCardNoBalance.Visible = true;
            }
            else if (!GiftCard.Enabled || GiftCard.ExpirationDate < DateTime.Now)
            {
                pnlError.Visible = true;
                ltrGiftCardDisabled.Visible = true;
            } else if (ValidationHelper.GetInteger(GiftCard.CustomerID, 0) > 0)
            {
                CustomerInfo CustomerObj = CustomerInfoProvider.GetCustomerInfo(GiftCard.CustomerID);
                if (CustomerObj != null && MembershipContext.AuthenticatedUser.UserID != CustomerObj.CustomerUserID)
                {
                    pnlError.Visible = true;
                    ltrGiftCardNotUsableByCustomer.Visible = true;
                }
            }
        }

        if (pnlError.Visible)
        {
            return;
        }

        // Only add if new card
        if (!CardData.GiftCardUsages.Exists(x => x.GiftCard.GiftCardCode == GiftCard.GiftCardCode))
        {
            CardData.AddGiftCard(GiftCard, Convert.ToDecimal(GiftCard.AmountRemaining));
        }

        SetGiftCardOrderData(CardData);
        SetGiftCardOrderData(CardData.CreateGiftCardDiscount(GetItemsTotalBeforeGiftCards(), GetShippingTotalBeforeGiftCards()));
        SetExistingGiftCards();
        tbxGiftCardCode.Text = "";
    }

    private void HideErrors()
    {
        // Hide errors
        pnlError.Visible = false;
        ltrNoGiftCardFound.Visible = false;
        ltrGiftCardNoBalance.Visible = false;
        ltrGiftCardDisabled.Visible = false;
        ltrOrderAlreadyPaid.Visible = false;
        ltrGiftCardNotUsableByCustomer.Visible = false;
    }

    private bool IsPaidOrder()
    {
        if (OrderID > 0)
        {
            OrderInfo Order = OrderInfoProvider.GetOrderInfo(OrderID);
            return Order.OrderIsPaid;
        }
        else
        {
            return false;
        }
    }

    private GiftCardOrderCustomData GetGiftCardOrderData()
    {
        // Get current cart's Gift Cards, apply the code if one is found
        if (OrderID > 0)
        {
            OrderInfo Order = OrderInfoProvider.GetOrderInfo(OrderID);
            object GiftCardData = Order.OrderCustomData.GetValue(GiftCardOrderCustomData.GiftCardOrderCustomDataKey);
            return (GiftCardData == null ? new GiftCardOrderCustomData() : GiftCardOrderCustomData.FromXML((string)GiftCardData));
        }
        else
        {
            ShoppingCartInfo Cart = ECommerceContext.CurrentShoppingCart;
            object GiftCardData = Cart.ShoppingCartCustomData.GetValue(GiftCardOrderCustomData.GiftCardOrderCustomDataKey);
            return (GiftCardData == null ? new GiftCardOrderCustomData() : GiftCardOrderCustomData.FromXML((string)GiftCardData));
        }
    }

    private GiftCardDiscountCustomData GetGiftCardDiscountOrderData()
    {
        // Get current cart's Gift Cards, apply the code if one is found
        if (OrderID > 0)
        {
            OrderInfo Order = OrderInfoProvider.GetOrderInfo(OrderID);
            object GiftCardData = Order.OrderCustomData.GetValue(GiftCardDiscountCustomData.GiftCardDiscountCustomDataKey);
            return (GiftCardData == null ? new GiftCardDiscountCustomData() : GiftCardDiscountCustomData.FromXML((string)GiftCardData));
        }
        else
        {
            ShoppingCartInfo Cart = ECommerceContext.CurrentShoppingCart;
            object GiftCardData = Cart.ShoppingCartCustomData.GetValue(GiftCardDiscountCustomData.GiftCardDiscountCustomDataKey);
            return (GiftCardData == null ? new GiftCardDiscountCustomData() : GiftCardDiscountCustomData.FromXML((string)GiftCardData));
        }
    }


    private void SetGiftCardOrderData(GiftCardOrderCustomData CardData, bool AutoFinalizedIfUIElement = true)
    {
        if (OrderID > 0)
        {
            OrderInfo Order = OrderInfoProvider.GetOrderInfo(OrderID);
            if (Order.OrderIsPaid)
            {
                ltrOrderAlreadyPaid.Visible = true;
                pnlError.Visible = true;
                return;
            }

            // If UI Element, auto save, no step needed
            if(IsUIElement() && AutoFinalizedIfUIElement)
            {
                GiftCardDiscountCustomData CardDiscountData = CardData.CreateGiftCardDiscount(GetItemsTotalBeforeGiftCards(), GetShippingTotalBeforeGiftCards());
                SetGiftCardOrderData(CardDiscountData, true);
            }

            Order.OrderCustomData.SetValue(GiftCardOrderCustomData.GiftCardOrderCustomDataKey, CardData.ToXML());
            OrderInfoProvider.SetOrderInfo(Order);
            // Convert to cart, recalculate and reset
            ShoppingCartInfo CartFromOrder = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(Order.OrderID);
            ShoppingCartInfoProvider.SetOrder(CartFromOrder);
            CartFromOrder.InvalidateCalculations();
        }
        else
        {
            // If UI Element, auto save, no step needed
            if (IsUIElement() && AutoFinalizedIfUIElement)
            {
                GiftCardDiscountCustomData CardDiscountData = CardData.CreateGiftCardDiscount(GetItemsTotalBeforeGiftCards(), GetShippingTotalBeforeGiftCards());
                SetGiftCardOrderData(CardDiscountData, true);
            }
            ECommerceContext.CurrentShoppingCart.ShoppingCartCustomData.SetValue(GiftCardOrderCustomData.GiftCardOrderCustomDataKey, CardData.ToXML());
            ShoppingCartInfoProvider.SetShoppingCartInfo(ECommerceContext.CurrentShoppingCart);
            ECommerceContext.CurrentShoppingCart.InvalidateCalculations();
        }
    }

    private void SetGiftCardOrderData(GiftCardDiscountCustomData CardData, bool AutoFinalizedIfUIElement = true)
    {
        if (OrderID > 0)
        {
            OrderInfo Order = OrderInfoProvider.GetOrderInfo(OrderID);
            if (Order.OrderIsPaid)
            {
                ltrOrderAlreadyPaid.Visible = true;
                pnlError.Visible = true;
                return;
            }

            Order.OrderCustomData.SetValue(GiftCardDiscountCustomData.GiftCardDiscountCustomDataKey, CardData.ToXML());
            OrderInfoProvider.SetOrderInfo(Order);
            // Convert to cart, recalculate and reset
            ShoppingCartInfo CartFromOrder = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(Order.OrderID);
            ShoppingCartInfoProvider.SetOrder(CartFromOrder);
            CartFromOrder.InvalidateCalculations();   
        }
        else
        {
            ECommerceContext.CurrentShoppingCart.ShoppingCartCustomData.SetValue(GiftCardDiscountCustomData.GiftCardDiscountCustomDataKey, CardData.ToXML());
            ShoppingCartInfoProvider.SetShoppingCartInfo(ECommerceContext.CurrentShoppingCart);
            ECommerceContext.CurrentShoppingCart.InvalidateCalculations();
        }
    }


    #region "Cart Values"

    private ShoppingCartInfo GetCart()
    {
        ShoppingCartInfo Cart = null;
        if (OrderID > 0)
        {
            OrderInfo Order = OrderInfoProvider.GetOrderInfo(OrderID);
            // Convert to Shopping Cart to calculate
            Cart = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(Order.OrderID);
        }
        else
        {
            Cart = ECommerceContext.CurrentShoppingCart;
        }
        return Cart;
    }

    private decimal GetItemsTotalWithAllDiscounts()
    {
        ShoppingCartInfo Cart = GetCart();
        Cart.InvalidateCalculations();
        double TotalItemPrice = ShoppingCartInfoProvider.CalculateTotalItemsPrice(Cart) - ShoppingCartInfoProvider.CalculateOrderDiscount(Cart);
        return Convert.ToDecimal(TotalItemPrice);
    }

    private decimal GetItemsGCDiscount()
    {
        ShoppingCartInfo Cart = GetCart();
        Cart.InvalidateCalculations();
        GiftCardDiscountCustomData GCDiscountObj = GetGiftCardDiscountOrderData();
        return GCDiscountObj.GiftCardUsages.Where(x => x.UsedOn == GiftCardUsedOnEnum.Products).Sum(x => x.Amount);
    }

    private decimal GetItemsTotalBeforeGiftCards()
    {
        return GetItemsTotalWithAllDiscounts() + GetItemsGCDiscount();
    }

    private decimal GetShippingTotalWithAllDiscounts()
    {
        ShoppingCartInfo Cart = GetCart();
        Cart.InvalidateCalculations();
        double ShippingTotal = ShippingOptionInfoProvider.CalculateTotalShipping(Cart);
        return Convert.ToDecimal(ShippingTotal);
    }

    private decimal GetShippingGCDiscount()
    {
        ShoppingCartInfo Cart = GetCart();
        Cart.InvalidateCalculations();
        GiftCardDiscountCustomData GCDiscountObj = GetGiftCardDiscountOrderData();
        return GCDiscountObj.GiftCardUsages.Where(x => x.UsedOn == GiftCardUsedOnEnum.Shipping).Sum(x => x.Amount);
    }

    private decimal GetShippingTotalBeforeGiftCards()
    {
        return GetShippingTotalWithAllDiscounts() + GetShippingGCDiscount();
    }

    private decimal GetGrandTotalBeforeGiftCards()
    {
        return GetShippingTotalBeforeGiftCards() + GetItemsTotalBeforeGiftCards();
    }

    private decimal GetGrandTotalAfterGiftCards()
    {
        return GetItemsTotalWithAllDiscounts() + GetShippingTotalWithAllDiscounts();
    }

    #endregion

}