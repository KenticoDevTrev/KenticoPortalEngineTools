using HBS_GiftCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Base.Web.UI;

public partial class CMSModules_HBS_GiftCards_Pages_Tools_GiftCard_AddDeduct : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterBootstrapScripts(this.Page);
        ScriptHelper.RegisterJQuery(this.Page);
        
        GiftCardInfo CurrentCard = GiftCardInfoProvider.GetGiftCardInfo(ValidationHelper.GetInteger(URLHelper.GetUrlParameter(HttpContext.Current.Request.Url.AbsoluteUri, "objectid"), -1));
        if (CurrentCard == null)
        {
            AddError("No Gift Card Found. Must pass the Gift Card ID to the objectid url parameter.");
            return;
        }
    }

    protected void btnProcesses_Click(object sender, EventArgs e)
    {
        // Get the current Gift Card
        GiftCardInfo CurrentCard = GiftCardInfoProvider.GetGiftCardInfo(ValidationHelper.GetInteger(URLHelper.GetUrlParameter(HttpContext.Current.Request.Url.AbsoluteUri, "objectid"), -1));
        decimal Amount = ValidationHelper.GetDecimal(tbxAmount.Text, 0);
        bool IsDeduction = Convert.ToBoolean(ddlAmountIsDeduction.SelectedValue);
        // Amount should always be positive, the drop down determines how it operates
        if (Amount < 0)
        {
            Amount = Amount * -1;
        }
        if (CurrentCard == null)
        {
            AddError("No Gift Card Found. Must pass the Gift Card ID to the objectid url parameter.");
            return;
        }

        if (IsDeduction)
        {
            // check to make sure can deduct that amount.
            if (CurrentCard.AmountRemaining < Amount)
            {
                AddError("Cannot deduct more than what is on the Gift Card (" + CurrentCard.AmountRemaining + ")");
                return;
            }

            CurrentCard.AmountRemaining -= Amount;
            CurrentCard.Update();

        }
        else
        {
            CurrentCard.AmountRemaining += Amount;
            CurrentCard.Update();
        }

        if (cbxCreateHistory.Checked)
        {
            GiftCardUsageHistoryInfo UsageNote = new GiftCardUsageHistoryInfo();
            UsageNote.Note = tbxHistoryNote.Text;
            UsageNote.Amount = Amount;
            UsageNote.AmountIsDeduction = IsDeduction;
            UsageNote.NewBalance = CurrentCard.AmountRemaining;
            UsageNote.GiftCardID = CurrentCard.GiftCardID;
            UsageNote.GiftCardUsageHistoryCreated = DateTime.Now;
            UsageNote.Insert();
        }
        AddConfirmation("Operation Complete.");
        tbxAmount.Text = "";
        ddlAmountIsDeduction.SelectedValue = "True";
        cbxCreateHistory.Checked = true;
        tbxHistoryNote.Text = "";
    }
}