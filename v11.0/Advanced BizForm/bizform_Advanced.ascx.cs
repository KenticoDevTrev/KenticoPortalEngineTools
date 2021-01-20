using System;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.WebAnalytics;
using CMS.OnlineForms;
using CMS.EventLog;
using CMS.MacroEngine;
using CMS.DataEngine;
using CMS.EmailEngine;
using System.Web;
using CMS.PortalEngine.Web.UI;
using CMS.OnlineForms.Web.UI;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;

public partial class CMSWebParts_Custom_BizForms_bizform_Advanced : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the form name of BizForm.
    /// </summary>
    public string BizFormName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BizFormName"), "");
        }
        set
        {
            SetValue("BizFormName", value);
        }
    }


    /// <summary>
    /// Gets or sets the alternative form full name (ClassName.AlternativeFormName).
    /// </summary>
    public string AlternativeFormName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternativeFormName"), "");
        }
        set
        {
            SetValue("AlternativeFormName", value);
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), "");
        }
        set
        {
            SetValue("SiteName", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the WebPart use colon behind label.
    /// </summary>
    public bool UseColonBehindLabel
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseColonBehindLabel"), true);
        }
        set
        {
            SetValue("UseColonBehindLabel", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after validation failed.
    /// </summary>
    public string ValidationErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ValidationErrorMessage"), "");
        }
        set
        {
            SetValue("ValidationErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion track name used after successful registration.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), "");
        }
        set
        {
            if (value.Length > 400)
            {
                value = value.Substring(0, 400);
            }
            SetValue("TrackConversionName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion value used after successful registration.
    /// </summary>
    public double ConversionValue
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("ConversionValue"), 0);
        }
        set
        {
            SetValue("ConversionValue", value);
        }
    }

    #endregion

    #region "Custom Properties"

    public string SaveType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SaveType"), "Inherit");
        }
        set
        {
            SetValue("SaveType", value);
            SetSaveTypeOverride();
        }
    }

    public string FormDisplayText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FormDisplayText"), "");
        }
        set
        {
            SetValue("FormDisplayText", value);
            SetSaveTypeOverride();
        }
    }

    public string RedirectUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectUrl"), "");
        }
        set
        {
            SetValue("RedirectUrl", value);
            SetSaveTypeOverride();
        }
    }

    public string SubmitButtonType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SubmitButtonType"), "");
        }
        set
        {
            SetValue("SubmitButtonType", value);
            SetSubmitButtonOverride();
        }
    }

    public string SubmitButtonText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SubmitButtonText"), "Submit");
        }
        set
        {
            SetValue("SubmitButtonText", value);
            SetSubmitButtonOverride();
        }
    }

    public string SubmitButtonImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SubmitButtonImageUrl"), "");
        }
        set
        {
            SetValue("SubmitButtonImageUrl", value);
            SetSubmitButtonOverride();
        }
    }

    public string EmailNotificationType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EmailNotificationType"), "Inherit");
        }
        set
        {
            SetValue("EmailNotificationType", value);
            SetEmailNotificationOverride();
        }
    }

    public string EmailNotificationFromAddress
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EmailNotificationFromAddress"), "");
        }
        set
        {
            SetValue("EmailNotificationFromAddress", value);
            SetEmailNotificationOverride();
        }
    }

    public string EmailNotificationToAddresses
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EmailNotificationToAddresses"), "");
        }
        set
        {
            SetValue("EmailNotificationToAddresses", value);
            SetEmailNotificationOverride();
        }
    }

    public string EmailNotificationSubject
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EmailNotificationSubject"), "");
        }
        set
        {
            SetValue("EmailNotificationSubject", value);
            SetEmailNotificationOverride();
        }
    }

    public string EmailNotificationFormEmailTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EmailNotificationFormEmailTemplate"), "");
        }
        set
        {
            SetValue("EmailNotificationFormEmailTemplate", value);
            SetEmailNotificationOverride();
        }
    }

    

    public string AutoResponderType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AutoResponderType"), "Inherit");
        }
        set
        {
            SetValue("AutoResponderType", value);
            SetAutoResponderOverride();
        }
    }

    public string AutoResponderFromAddress
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AutoResponderFromAddress"), "");
        }
        set
        {
            SetValue("AutoResponderFromAddress", value);
            SetAutoResponderOverride();
        }
    }

    public string AutoResponderToAddressFieldName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AutoResponderToAddressFieldName"), "");
        }
        set
        {
            SetValue("AutoResponderToAddressFieldName", value);
            SetAutoResponderOverride();
        }
    }

    public string AutoResponderFormEmailTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AutoResponderFormEmailTemplate"), "");
        }
        set
        {
            SetValue("AutoResponderFormEmailTemplate", value);
            SetAutoResponderOverride();
        }
    }

    public string AutoResponderSubject
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AutoResponderSubject"), "");
        }
        set
        {
            SetValue("AutoResponderSubject", value);
            SetAutoResponderOverride();
        }
    }

    

    #endregion

    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        if (!StopProcessing)
        {
            viewBiz.OnAfterSave += viewBiz_OnAfterSave;
            base.OnLoad(e);
        }
    }

    protected override void OnInit(EventArgs e)
    {
        if (!StopProcessing)
        {
            base.OnInit(e);
            SetSubmitButtonOverride();
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        if (!StopProcessing)
        {
            base.OnPreRender(e);
            SetSubmitButtonOverride();
        }
    }
    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        if (!StopProcessing)
        {
            base.OnContentLoaded();
            SetupControl();
        }
    }


    /// <summary>
    /// Reloads data for partial caching.
    /// </summary>
    public override void ReloadData()
    {
        if (!StopProcessing)
        {
            base.ReloadData();
            SetupControl();
        }
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
            viewBiz.StopProcessing = true;
        }
        else
        {
            viewBiz.StopProcessing = false;
            // Set BizForm properties
            viewBiz.FormName = BizFormName;
            viewBiz.SiteName = SiteName;
            viewBiz.UseColonBehindLabel = UseColonBehindLabel;
            viewBiz.AlternativeFormFullName = AlternativeFormName;
            viewBiz.ValidationErrorMessage = ValidationErrorMessage;

            SetSaveTypeOverride();

            SetSubmitButtonOverride();

            SetEmailNotificationOverride();

            SetAutoResponderOverride();

            // Set the live site context
            if (viewBiz != null)
            {
                viewBiz.ControlContext.ContextName = CMS.Base.Web.UI.ControlContext.LIVE_SITE;
            }
        }
    }

    private void SetSaveTypeOverride()
    {
        viewBiz.OnAfterSave -= ViewBiz_OnAfterSaveRedirect;
        viewBiz.OnAfterSave -= ViewBiz_OnAfterSaveClearForm;
        switch (SaveType)
        {
            case "Inherit":
            default:
                break;
            case "DisplayText":
                viewBiz.FormClearAfterSave = null;
                viewBiz.FormDisplayText = ContextResolver.ResolveMacros(FormDisplayText, (MacroSettings)null);
                break;
            case "RedirectToUrl":
                viewBiz.FormClearAfterSave = null;
                viewBiz.OnAfterSave += ViewBiz_OnAfterSaveRedirect;
                break;
        }
    }

    private void ViewBiz_OnAfterSaveClearForm(object sender, EventArgs e)
    {
        BizForm BizFormObj = (BizForm)sender;
        if(SaveType == "ClearForm")
        {
            BizFormObj.ItemID = 0;
            BizFormObj.Mode = FormModeEnum.Insert;
        }
        BizFormObj.ReloadData();
    }

    private void ViewBiz_OnAfterSaveRedirect(object sender, EventArgs e)
    {
        URLHelper.ResponseRedirect(RedirectUrl, true);
    }

    private void SetSubmitButtonOverride()
    {
        switch (SubmitButtonType)
        {
            case "Inherit":
            default:
                break;
            case "TextButton":
                viewBiz.ShowImageButton = false;
                // Since oddly the viewBiz.SubmitButton.Text is always overwritten and doesn't work when setting it, instead have to rely on client side
                // This adds a unique CSS Class, then registers a script to grab that will find that element and change it's value.
                if(string.IsNullOrWhiteSpace(hdnButtonUniqueID.Value))
                {
                    hdnButtonUniqueID.Value = Guid.NewGuid().ToString().Replace("-", "_");
                }
                string IdentifierAttribute = " SubmitButton_" + hdnButtonUniqueID.Value;
                string SetButtonFunction = "SetButton_" + hdnButtonUniqueID.Value;
                if(viewBiz.SubmitButton.CssClass.IndexOf(hdnButtonUniqueID.Value) == -1) { 
                    viewBiz.SubmitButton.CssClass += IdentifierAttribute;
                }
                ltrButtonTextOverride.Text = string.Format(@"
<script type=""text/javascript"">
    // Handles on load for normal calls, and immediate/update panel
    document.addEventListener(""DOMContentLoaded"", function(event) {{ {0}(); }});
    function {0}() {{
        var buttons = document.getElementsByTagName('input');
        for (var i = 0; i < buttons.length; i++)
        {{                
                if (buttons[i].getAttribute('type') == 'submit' && buttons[i].getAttribute('class').indexOf('{1}') >= 0)
                {{
                    buttons[i].value = '{2}';
                }}
        }}
    }}
</script>", SetButtonFunction, IdentifierAttribute.Trim(), ContextResolver.ResolveMacros(SubmitButtonText, (MacroSettings)null).Replace("'", "\\'"));
                // Add this for UpdatePanel purposes
                if(!IsPostBack) { 
                    ScriptHelper.RegisterClientScriptBlock(this.Page, this.Page.GetType(), hdnButtonUniqueID.Value, "document.addEventListener(\"DOMContentLoaded\", function(event) { Sys.WebForms.PageRequestManager.getInstance().add_endRequest(" + SetButtonFunction + ");});", true);
                }
                break;
            case "ImageButton":
                viewBiz.ShowImageButton = true;
                viewBiz.SubmitImageButton.ImageUrl = SubmitButtonImageUrl;
                break;
        }
    }

    private void SetEmailNotificationOverride()
    {
        viewBiz.OnAfterSave -= ViewBiz_OnAfterSave_SendNotification;
        switch (EmailNotificationType)
        {
            case "Inherit":
            default:
                break;
            case "SendAdditional":
                viewBiz.OnAfterSave += ViewBiz_OnAfterSave_SendNotification;
                break;
        }
    }

    private void SetAutoResponderOverride()
    {
        viewBiz.OnAfterSave -= ViewBiz_OnAfterSave_SendAutoResponder;
        switch (AutoResponderType)
        {
            case "Inherit":
            default:
                break;
            case "SendAdditional":
                viewBiz.OnAfterSave += ViewBiz_OnAfterSave_SendAutoResponder;
                break;
        }
    }

    private void ViewBiz_OnAfterSave_SendNotification(object sender, EventArgs e)
    {
        try
        {
            BizForm BizFormObj = (BizForm)sender;
            BizFormInfo BizFormInfoObj = BizFormInfoProvider.GetBizFormInfo(BizFormObj.FormName, SiteContext.CurrentSiteID);
            BizFormItem BizFormItemObj = GetBizFormItem(BizFormInfoObj, BizFormObj.ItemID);

            // Save originals to set back after done with save.
            string OriginalFormEmailTemplate = BizFormInfoObj.FormEmailTemplate;
            string OriginalFormEmailSubject = BizFormInfoObj.FormEmailSubject;
            
            string ToAddress = (!string.IsNullOrWhiteSpace(EmailNotificationToAddresses) ? EmailNotificationToAddresses : BizFormInfoObj.FormSendToEmail);
            string FromAddress = BizFormInfoObj.FormSendFromEmail;
            EmailTemplateInfo EmailTemplate = EmailTemplateProvider.GetEmailTemplate(EmailNotificationFormEmailTemplate, SiteContext.CurrentSiteID);
            if (EmailTemplate != null)
            {
                BizFormInfoObj.FormEmailTemplate = EmailTemplate.TemplateText;
                BizFormInfoObj.FormEmailSubject = (!string.IsNullOrWhiteSpace(EmailNotificationSubject) ? EmailNotificationSubject : EmailTemplate.TemplateSubject);
                FromAddress = (!string.IsNullOrWhiteSpace(EmailNotificationFromAddress) ? EmailNotificationFromAddress : (!string.IsNullOrWhiteSpace(EmailTemplate.TemplateFrom) ? EmailTemplate.TemplateFrom : FromAddress));
            } else
            {
                FromAddress = (!string.IsNullOrWhiteSpace(EmailNotificationFromAddress) ? EmailNotificationFromAddress : FromAddress);
                BizFormInfoObj.FormEmailSubject = (!string.IsNullOrWhiteSpace(EmailNotificationSubject) ? EmailNotificationSubject : BizFormInfoObj.FormEmailSubject);
            }
            if(!string.IsNullOrWhiteSpace(ToAddress) && !string.IsNullOrWhiteSpace(FromAddress)) { 
                viewBiz.SendNotificationEmail(FromAddress, ToAddress, BizFormItemObj, BizFormInfoObj);
            } else
            {
                EventLogProvider.LogEvent("W", "AdvancedOnlineForm", "NoToAddressForEmail", eventDescription: "There was no " + (string.IsNullOrWhiteSpace(ToAddress) ? " To Address " : "") + " " + (string.IsNullOrWhiteSpace(FromAddress) ? " From Address " : "") + " for the Email Notification to send out via the Advanced Online Form Webpart. Please specify a To Address in it's configuration on page " + HttpContext.Current.Request.RawUrl);
            }

            // Set fields back
            BizFormInfoObj.FormEmailTemplate = OriginalFormEmailTemplate;
            BizFormInfoObj.FormEmailSubject = OriginalFormEmailSubject;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("AdvancedOnlineForm", "ErrorSendingAdditionalNotification", ex);
        }
    }
    private void ViewBiz_OnAfterSave_SendAutoResponder(object sender, EventArgs e)
    {
        try
        {
            BizForm BizFormObj = (BizForm)sender;
            BizFormInfo BizFormInfoObj = BizFormInfoProvider.GetBizFormInfo(BizFormObj.FormName, SiteContext.CurrentSiteID);
            BizFormItem BizFormItemObj = GetBizFormItem(BizFormInfoObj, BizFormObj.ItemID);

            // Save originals to set back after done with save.
            string OriginalFormConfirmationTemplate = BizFormInfoObj.FormConfirmationTemplate;
            string OriginalFormConfirmationEmailSubject = BizFormInfoObj.FormConfirmationEmailSubject;
            string OriginalFormConfirmationEmailField = BizFormInfoObj.FormConfirmationEmailField;
            string OriginalFormConfirmationSendFromEmail = BizFormInfoObj.FormConfirmationSendFromEmail;

            string EmailToField = (!string.IsNullOrWhiteSpace(AutoResponderToAddressFieldName) ? AutoResponderToAddressFieldName : BizFormInfoObj.FormConfirmationEmailField);
            string ToAddress = BizFormItemObj.GetValue<string>(EmailToField, "");
            string FromAddress = BizFormInfoObj.FormConfirmationSendFromEmail;
            EmailTemplateInfo EmailTemplate = EmailTemplateProvider.GetEmailTemplate(AutoResponderFormEmailTemplate, SiteContext.CurrentSiteID);
            if (EmailTemplate != null)
            {
                BizFormInfoObj.FormConfirmationTemplate = EmailTemplate.TemplateText;
                BizFormInfoObj.FormConfirmationEmailSubject = (!string.IsNullOrWhiteSpace(AutoResponderSubject) ? AutoResponderSubject : EmailTemplate.TemplateSubject);
                FromAddress = (!string.IsNullOrWhiteSpace(AutoResponderFromAddress) ? AutoResponderFromAddress : (!string.IsNullOrWhiteSpace(EmailTemplate.TemplateFrom) ? EmailTemplate.TemplateFrom : FromAddress));
            }
            else
            {
                FromAddress = (!string.IsNullOrWhiteSpace(AutoResponderFromAddress) ? AutoResponderFromAddress : FromAddress);
                BizFormInfoObj.FormEmailSubject = (!string.IsNullOrWhiteSpace(AutoResponderSubject) ? AutoResponderSubject : BizFormInfoObj.FormEmailSubject);
            }
            if (!string.IsNullOrWhiteSpace(ToAddress) && !string.IsNullOrWhiteSpace(FromAddress))
            {
                viewBiz.SendConfirmationEmail(FromAddress, ToAddress, BizFormItemObj, BizFormInfoObj);
            }
            else
            {
                EventLogProvider.LogEvent("W", "AdvancedOnlineForm", "InvalidAddressForAutoResponderEmail", eventDescription: "There was no "+(string.IsNullOrWhiteSpace(ToAddress) ? " To Address " : "")+ " " + (string.IsNullOrWhiteSpace(FromAddress) ? " From Address " : "") + " for the Email Autoresponder to send out via the Advanced Online Form Webpart. Please specify a To Address in it's configuration on page " + HttpContext.Current.Request.RawUrl);
            }

            // Set values back.
            BizFormInfoObj.FormConfirmationTemplate = OriginalFormConfirmationTemplate;
            BizFormInfoObj.FormConfirmationEmailSubject = OriginalFormConfirmationEmailSubject;
            BizFormInfoObj.FormConfirmationEmailField = OriginalFormConfirmationEmailField;
            BizFormInfoObj.FormConfirmationSendFromEmail = OriginalFormConfirmationSendFromEmail;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("AdvancedOnlineForm", "ErrorSendingAdditionalAutoResponder", ex);
        }
    }

    private BizFormItem GetBizFormItem(BizFormInfo BizFormInfoObj, int ItemID)
    {
        // Gets the class name of the 'ContactUs' form
        DataClassInfo formClass = DataClassInfoProvider.GetDataClassInfo(BizFormInfoObj.FormClassID);
        // Loads the form's data
        return BizFormItemProvider.GetItem(ItemID, formClass.ClassName);
    }
    private void viewBiz_OnAfterSave(object sender, EventArgs e)
    {
        if (TrackConversionName != String.Empty)
        {
            string siteName = SiteContext.CurrentSiteName;

            if (AnalyticsHelper.AnalyticsEnabled(siteName) && !AnalyticsHelper.IsIPExcluded(siteName, RequestContext.UserHostAddress))
            {
                HitLogProvider.LogConversions(SiteContext.CurrentSiteName, LocalizationContext.PreferredCultureCode, TrackConversionName, 0, ConversionValue);
            }
        }
    }

    #endregion
}