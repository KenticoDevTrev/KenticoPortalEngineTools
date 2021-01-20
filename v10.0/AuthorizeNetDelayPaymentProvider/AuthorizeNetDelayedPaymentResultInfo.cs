using CMS.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Ecommerce.Web.UI
{
    /// <summary>
    /// Extends the Payment Results Info to include the Authorization code, copied the existing class in case we need more information also.
    /// </summary>
    public class AuthorizeNetDelayedPaymentResultInfo : PaymentResultInfo
    {
        private const string HEADER_AUTHORIZATIONCODE = "{$AuthorizeNet.AuthorizationCode$}";
        private const string HEADER_AUTHORIZATIONAMOUNT = "{$AuthorizeNet.AuthorizationAmount$}";

        public AuthorizeNetDelayedPaymentResultInfo()
        {
            base.EnsurePaymentResultItemInfo("authorizationcode", "{$AuthorizeNet.AuthorizationCode$}");
            base.EnsurePaymentResultItemInfo("previouslyauthorizedamount", "{$AuthorizeNet.AuthorizationAmount$}");
        }

        public string AuthorizationCode
        {
            get
            {
                PaymentResultItemInfo info = base.EnsurePaymentResultItemInfo("authorizationcode", "{$AuthorizeNet.AuthorizationCode$}");
                if (info == null)
                {
                    return "";
                }
                return info.Value;
            }
            set
            {
                PaymentResultItemInfo itemObj = base.EnsurePaymentResultItemInfo("authorizationcode", "{$AuthorizeNet.AuthorizationCode$}");
                if (itemObj != null)
                {
                    itemObj.Value = ValidationHelper.GetString(value, "", (CultureInfo)null);
                    base.SetPaymentResultItemInfo(itemObj);
                }
            }
        }

        public string PreviouslyAuthorizedAmount
        {
            get
            {
                PaymentResultItemInfo info = base.EnsurePaymentResultItemInfo("previouslyauthorizedamount", "{$AuthorizeNet.AuthorizationAmount$}");
                if (info == null)
                {
                    return "";
                }
                return info.Value;
            }
            set
            {
                PaymentResultItemInfo itemObj = base.EnsurePaymentResultItemInfo("previouslyauthorizedamount", "{$AuthorizeNet.AuthorizationAmount$}");
                if (itemObj != null)
                {
                    itemObj.Value = ValidationHelper.GetString(value, "", (CultureInfo)null);
                    base.SetPaymentResultItemInfo(itemObj);
                }
            }
        }

    }
}
