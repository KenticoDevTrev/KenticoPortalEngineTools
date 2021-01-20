using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Ecommerce;

public partial class CMSModules_HBS_GiftCards_Ecommerce_Checkout_PaidByGiftCardForm_PaidFullyByGiftCard : CMSPaymentGatewayForm
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public override string ValidateData()
    {
        // Get order from ID, note that i had to adjust the PaymentForm.ascx.cs to add the OrderID to the PaymentProvider so this was available.
        OrderInfo OrderObject = OrderInfoProvider.GetOrderInfo(PaymentProvider.OrderId);
        if(OrderObject.OrderTotalPriceInMainCurrency+ OrderObject.OrderTotalShippingInMainCurrency != 0)
        {
            lblError.Text = "Order is not fully paid for by the gift cards.  Cannot proceed.";
            return lblError.Text;
        }
        return "";
    }

    public override void LoadData()
    {
        // Get order from ID, note that i had to adjust the PaymentForm.ascx.cs to add the OrderID to the PaymentProvider so this was available.
        OrderInfo OrderObject = OrderInfoProvider.GetOrderInfo(PaymentProvider.OrderId);
        if (OrderObject.OrderTotalPriceInMainCurrency + OrderObject.OrderTotalShippingInMainCurrency != 0)
        {
            lblError.Text = "Order is not fully paid for by the gift cards.  Cannot proceed.";
        }
        else
        {
            // enable the auto-submit
            pnlAutoSubmit.Visible = true;
        }
    }
}