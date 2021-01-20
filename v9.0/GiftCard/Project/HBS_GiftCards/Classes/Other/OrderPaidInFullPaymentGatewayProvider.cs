using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HBS_GiftCards;
using CMS;
using CMS.Ecommerce.Web.UI;
using System.Collections;
using CMS.Helpers;
using CMS.Ecommerce;
using CMS.Membership;
using System.Web.UI;

[assembly: RegisterCustomClass("OrderPaidInFull", typeof(OrderPaidInFull))]

namespace HBS_GiftCards
{
    public class OrderPaidInFull : CMSPaymentGatewayProvider
    {
    
        /// <summary>
        /// Process payment.
        /// </summary>
        public override void ProcessPayment()
        {
            if (Order.OrderTotalPriceInMainCurrency == 0 && Order.OrderTotalShippingInMainCurrency == 0)
            {
                PaymentResult.PaymentIsCompleted = true;
                // Update order payment result in database
                UpdateOrderPaymentResult();
            } else { 
                PaymentResult.PaymentDescription = ErrorMessage;
                PaymentResult.PaymentIsCompleted = false;
                // Update order payment result in database
                UpdateOrderPaymentResult();
            }
        }
        public override void AddAdditionalInfoToPaymentResult()
        {
            base.AddAdditionalInfoToPaymentResult();
        }
        public override string GetPaymentGatewayUrl()
        {
            return "";
        }
        public override string GetPaymentDataFormPath()
        {
            return "~/CMSModules/HBS_GiftCards/Ecommerce/Controls/PaymentGateways/OrderPaidInFull.ascx";
        }
        public override bool IsUserAuthorizedToFinishPayment(UserInfo user, ShoppingCartInfo cart, bool internalOrder = false)
        {
            double TotalItemPrice = ShoppingCartInfoProvider.CalculateTotalItemsPrice(cart) - ShoppingCartInfoProvider.CalculateOrderDiscount(cart);
            double ShippingTotal = ShippingOptionInfoProvider.CalculateShipping(cart);
            return (TotalItemPrice + ShippingTotal == 0);
        }
    }
}
