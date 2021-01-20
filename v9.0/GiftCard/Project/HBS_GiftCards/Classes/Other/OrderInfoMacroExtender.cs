using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS;
using CMS.Ecommerce;

using CMS.MacroEngine;
using System.Data;
using HBS_GiftCards;
[assembly: RegisterExtension(typeof(OrderInfoMacroExtender), typeof(OrderInfo))]
[assembly: RegisterExtension(typeof(OrderInfoMacroExtender), typeof(UtilNamespace))]
namespace HBS_GiftCards
{
    public class OrderInfoMacroExtender : MacroMethodContainer
    {
        [MacroMethod(typeof(object), "Gets an orders Gift Card data", 1)]
        [MacroMethodParam(0, "Order", typeof(OrderInfo), "The Order")]
        public static object Order_GiftCards(EvaluationContext context, params object[] parameters)
        {
            // Branches according to the number of the method's parameters
            switch (parameters.Length)
            {
                case 1:
                    // Overload with one parameter
                    OrderInfo OrderObj = (OrderInfo)parameters[0];
                    if (OrderObj != null)
                    {
                        GiftCardDiscountCustomData GiftCardData = GiftCardDiscountCustomData.FromXML((string)OrderObj.OrderCustomData.GetValue(HBS_GiftCards.GiftCardDiscountCustomData.GiftCardDiscountCustomDataKey));
                        return GiftCardData;
                    }
                    else
                    {
                        return null;
                    }
                default:
                    // No other overloads are supported
                    throw new NotSupportedException();
            }
        }
    }
}
