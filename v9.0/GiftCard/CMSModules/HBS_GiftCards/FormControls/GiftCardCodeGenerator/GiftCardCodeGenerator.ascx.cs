using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HBS_GiftCards;

public partial class CMSModules_HBS_GiftCards_FormControls_GiftCardCodeGenerator_GiftCardCodeGenerator : FormEngineUserControl
{

    public string Prefix
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Prefix"), "");
        }
        set
        {
            SetValue("Prefix", value);
        }
    }

    public string PostFix
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PostFix"), "");
        }
        set
        {
            SetValue("PostFix", value);
        }
    }

    public int TotalLength
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("TotalLength"), 10);
        }
        set
        {
            SetValue("TotalLength", value);
        }
    }

    public override object Value
    {
        get
        {
            return tbxValue.Text;
        }
        set
        {
            // Selects the matching value in the drop-down
            tbxValue.Text = ValidationHelper.GetString(value, "");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnGenerateNew_Click(object sender, EventArgs e)
    {
        string RandomCode = Prefix + RandomString(TotalLength - Prefix.Length - PostFix.Length) + PostFix;
        while (GiftCardInfoProvider.GetGiftCardInfo(RandomCode) != null)
        {
            RandomCode = Prefix + RandomString(TotalLength - Prefix.Length - PostFix.Length) + PostFix;
        }
        tbxValue.Text = RandomCode;
    }

    private string RandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var stringChars = new char[length];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
        return new String(stringChars);
    }
}