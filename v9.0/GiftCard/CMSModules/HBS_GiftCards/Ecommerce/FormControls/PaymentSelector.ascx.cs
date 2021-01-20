using System;
using System.Data;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using HBS_GiftCards;
using System.Linq;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_FormControls_PaymentSelector : SiteSeparatedObjectSelector
{
    #region "Properties"

    /// <summary>
    ///  If true, selected value is PaymentName, if false, selected value is PaymentOptionID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UsePaymentNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UsePaymentNameForSelection", value);
            base.UseNameForSelection = value;
        }
    }


    /// <summary>
    /// Gets a column name for shipping option ID column.
    /// </summary>
    public string ShippingOptionIDColumnName
    {
        get
        {
            return GetValue("ShippingOptionIDColumnName", String.Empty);
        }
        set
        {
            SetValue("ShippingOptionIDColumnName", value);
        }
    }


    /// <summary>
    /// Allows to access selector object.
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Indicates if only payment options that are allowed to be used without shipping are displayed.
    /// </summary>
    public bool DisplayOnlyAllowedIfNoShipping
    {
        get;
        set;
    }


    /// <summary>
    /// Shopping cart.
    /// </summary>
    public ShoppingCartInfo ShoppingCart
    {
        get
        {
            return GetValue("ShoppingCart") as ShoppingCartInfo;
        }
        set
        {
            SetValue("ShoppingCart", value);
        }
    }

    #endregion


    #region "Life cycle"

    /// <summary>
    /// Sets up ShippingOptionIDColumnName property if shipping option id column name is configured.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!String.IsNullOrEmpty(ShippingOptionIDColumnName))
        {
            var shippingID = (Form != null) ? ValidationHelper.GetInteger(Form.Data.GetValue(ShippingOptionIDColumnName), 0) : 0;
            DisplayOnlyAllowedIfNoShipping = (shippingID == 0);
        }
    }
    
    #endregion


    #region "Methods"

    /// <summary>
    /// Convert given payment option name to its ID for specified site.
    /// </summary>
    /// <param name="name">Name of the payment option to be converted.</param>
    /// <param name="siteName">Name of the site of the payment option.</param>
    protected override int GetID(string name, string siteName)
    {
        var payment = PaymentOptionInfoProvider.GetPaymentOptionInfo(name, siteName);

        return (payment != null) ? payment.PaymentOptionID : 0;
    }


    /// <summary>
    /// Appends where condition filtering only payments marked with PaymentOptionAllowIfNoShipping flag when requested.
    /// </summary>
    /// <param name="where">Original where condition.</param>
    protected override string AppendExclusiveWhere(string where)
    {
        // Filter out only payment options that are allowed to be used without shipping
        if (DisplayOnlyAllowedIfNoShipping)
        {
            where = SqlHelper.AddWhereCondition(where, "PaymentOptionAllowIfNoShipping = 1");
        }

        // CUSTOM HBS_GIFT CARD add where to exclude the Gift Card Only Payment unless the order total is 0 and Gift cards used
        string PaidInFullOption = ValidationHelper.GetString(SettingsKeyInfoProvider.GetValue("Accents_PaidByGiftCardPaymentMethod", new SiteInfoIdentifier((ShoppingCart != null && ShoppingCart.ShoppingCartSiteID > 0 ? ShoppingCart.ShoppingCartSiteID : SiteContext.CurrentSiteID))), "");
        if(!string.IsNullOrWhiteSpace(PaidInFullOption)) {
            where = SqlHelper.AddWhereCondition(where, string.Format("PaymentOptionName {0} '{1}'", (GetTotalAfterGiftCards() == 0 ? "=" : "<>"), PaidInFullOption));
        }

        return base.AppendExclusiveWhere(where);
    }

    protected override void OnPreRender(EventArgs e)
    {
        // Do a final check for the gift cards
        string where = "";
        where = AppendExclusiveWhere(where);
        uniSelector.WhereCondition = where;
        uniSelector.Reload(true);

        // Preselect if only 1 option
        if (UniSelector.DropDownItems.Count == 2)
        {
            UniSelector.DropDownItems[0].Selected = false;
            UniSelector.DropDownItems[1].Selected = true;
        }
    }

    private decimal GetTotalBeforeGiftCards()
    {
        // Get current cart's Gift Cards, apply the code if one is found
        ShoppingCartInfo Cart = null;
        if (ShoppingCart != null && ShoppingCart.OrderId > 0)
        {
            OrderInfo Order = OrderInfoProvider.GetOrderInfo(ShoppingCart.OrderId);
            // Convert to Shopping Cart to calculate
            Cart = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(Order.OrderID);
        }
        else
        {
            Cart = ECommerceContext.CurrentShoppingCart;
        }
        Cart.InvalidateCalculations();
        return Convert.ToDecimal(Cart.TotalPriceInMainCurrency);
    }

    private decimal GetTotalAfterGiftCards()
    {
        decimal CartItemPrice = GetTotalBeforeGiftCards();
        decimal GiftCardsUsageAmounts = GetGiftCardOrderData().GiftCardUsages.Sum(x => x.Amount);
        if (CartItemPrice < GiftCardsUsageAmounts)
        {
            return 0;
        }
        else
        {
            return CartItemPrice - GiftCardsUsageAmounts;
        }

    }

    private GiftCardOrderCustomData GetGiftCardOrderData()
    {
        // Get current cart's Gift Cards, apply the code if one is found
        if (ShoppingCart != null && ShoppingCart.OrderId > 0)
        {
            OrderInfo Order = OrderInfoProvider.GetOrderInfo(ShoppingCart.OrderId);
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


    /// <summary>
    /// Ensures that only applicable payment options are displayed in selector.
    /// </summary>
    /// <param name="ds">Dataset with payment options.</param>
    protected override DataSet OnAfterRetrieveData(DataSet ds)
    {
        if (DataHelper.IsEmpty(ds) || (ShoppingCart == null))
        {
            return ds;
        }

        foreach (DataRow paymentRow in ds.Tables[0].Select())
        {
            PaymentOptionInfo paymentOptionInfo;

            if (UseNameForSelection)
            {
                var paymentName = DataHelper.GetStringValue(paymentRow, "PaymentOptionName");
                paymentOptionInfo = PaymentOptionInfoProvider.GetPaymentOptionInfo(paymentName, ShoppingCart.SiteName);
            }
            else
            {
                var paymentID = DataHelper.GetIntValue(paymentRow, "PaymentOptionID");
                paymentOptionInfo = PaymentOptionInfoProvider.GetPaymentOptionInfo(paymentID);
            }

            // Do not remove already selected item even if the option is not applicable anymore 
            // The user would see a different value in UI as is actually stored in the database
            var canBeRemoved = !EnsureSelectedItem || (ShoppingCart.ShoppingCartPaymentOptionID != paymentOptionInfo.PaymentOptionID);
            if (canBeRemoved && !PaymentOptionInfoProvider.IsPaymentOptionApplicable(ShoppingCart, paymentOptionInfo))
            {
                // Remove not applicable payment methods from the selector
                ds.Tables[0].Rows.Remove(paymentRow);
            }
        }

        return ds;
    }

    #endregion
}