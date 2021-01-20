using CMS;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.EventLog;
using CMS.SiteProvider;

// Registers the custom module into the system
[assembly: RegisterModule(typeof(AuthorizeNetDelayPaymentProviderCustomLoader))]

public class AuthorizeNetDelayPaymentProviderCustomLoader : Module
{
    // Module class constructor, the system registers the module under the name "CustomInit"
    public AuthorizeNetDelayPaymentProviderCustomLoader()
        : base("CustomInit")
    {
    }

    // Contains initialization code that is executed when the application starts
    protected override void OnInit()
    {
        base.OnInit();

        OrderInfo.TYPEINFO.Events.Update.After += OrderUpdate_After;
    }

    private void OrderUpdate_After(object sender, ObjectEventArgs e)
    {
        OrderInfo OrderObject = (OrderInfo)e.Object;
        string SiteName = SiteContext.CurrentSiteName;
        // If order uses the Delay Payment, it will have this value in the Payment Results XML
        if(OrderObject.OrderPaymentResult != null && OrderObject.OrderPaymentResult.GetPaymentResultXml().ToLower().Contains("previouslyauthorizedamount"))
        {
            // Grab the proper OrderIDs
            string ProcessOrderStatusName = SettingsKeyInfoProvider.GetValue("PaymentChargeOrderStatus");
            string ProcessSuccessOrderStatusName = SettingsKeyInfoProvider.GetValue("PaymentChargeOrderSuccessStatus");
            string ProcessFailureOrderStatusName = SettingsKeyInfoProvider.GetValue("PaymentChargeOrderFailureStatus");
            OrderStatusInfo ProcessOrderStatus = OrderStatusInfoProvider.GetOrderStatusInfo(ProcessOrderStatusName, SiteName);
            OrderStatusInfo ProcessSuccessOrderStatus = OrderStatusInfoProvider.GetOrderStatusInfo(ProcessSuccessOrderStatusName, SiteName);
            OrderStatusInfo ProcessFailureOrderStatus = OrderStatusInfoProvider.GetOrderStatusInfo(ProcessFailureOrderStatusName, SiteName);

            if(ProcessOrderStatus == null || ProcessSuccessOrderStatus == null || ProcessFailureOrderStatus == null)
            {
                EventLogProvider.LogEvent("E", "AuthrizeNetDelayPaymentCustomLoader", "INVALIDCONFIGURATION", "One or more of the Order Statuses could not be located.  Please go to your Settings - Ecommerce - Payment Gateways - Authorize.NET - Authorize and Capture (Delay Payment) and re-configure.  Then save this order to re-trigger payment capture.");
                return;
            }

            if(OrderObject.OrderStatusID == ProcessOrderStatus.StatusID)
            {
                if(CMSAuthorizeNetDelayPaymentProvider.CapturePayment(OrderObject))
                {
                    OrderObject.OrderStatusID = ProcessSuccessOrderStatus.StatusID;
                    OrderInfoProvider.SetOrderInfo(OrderObject);
                } else
                {
                    OrderObject.OrderStatusID = ProcessFailureOrderStatus.StatusID;
                    OrderInfoProvider.SetOrderInfo(OrderObject);
                }
            }

        }
    }
}
