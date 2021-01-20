using CMS.Ecommerce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avastone.Ecommerce { 
/// <summary>
/// Summary description for AvastoneProductOptionSelector
/// </summary>
public class AvastoneProductOptionSelector : ProductOptionSelector
{
    /// <summary>
    /// Called after the "IsValid" on the Add to Cart button. Use this to do any custom logic.  Recommended you set teh "SelectedProductOptions" string value to a comma seperated list of the values you want this to render.
    /// </summary>
    public virtual void OnAddToCartBefore()
    {
        return;
    }
    /// <summary>
    /// This is called just before the Redirect to the cart page, or the call to refresh the page after the items have been added to the cart.
    /// </summary>
    public virtual void OnAddToCartAfter()
    {
        return;
    }
    /// <summary>
    /// This is called before the Redirect to the Wishlist page with the SKUID.
    /// </summary>
    public virtual void OnAddToWishListBefore()
    {
        return;
    }
    /// <summary>
    /// This is called instead of the GetSelectedSKUOptions() (which isn't overwrittable from the ProductOptionSelectorClass)
    /// </summary>
    /// <returns></returns>
    public virtual string GetSelectedSKUOptionsCustom()
    {
        return GetSelectedSKUOptions();
    }

    public void OnOptionCategoryModified(EventArgs e)
    {
        OptionCategoryModifiedHandler handler = OptionCategoryModified;
        if (handler != null)
        {
            handler(this, new EventArgs());
        }
    }
    public delegate void OptionCategoryModifiedHandler (object sender, EventArgs e);
    public event OptionCategoryModifiedHandler OptionCategoryModified;
}


}