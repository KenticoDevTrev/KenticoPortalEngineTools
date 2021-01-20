using System;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.FormControls;
using System.Data;
using Avastone.Ecommerce;
using CMS.DataEngine;
using System.Collections.Generic;
using CMS.DocumentEngine;
using CMS.Membership;
using System.Web.UI.WebControls;
using CMS.ExtendedControls;

// Uses AvastoneProductOptionSelector wihch has additional overridable hooks
public partial class CMSModules_Ecommerce_Controls_ProductOptions_ProductOptionSelector : AvastoneProductOptionSelector
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadSelector();
    }


    /// <summary>
    /// Loads selector's data.
    /// </summary>
    private void LoadSelector()
    {
		// Set up your control here
		
		// You have access to the following fields which are set from the Shopping Cart Item Selector
		
		// ShoppingCartInfo LocalShoppingCartObj
		// bool ShowPriceIncludingTax
		// bool IsLiveSite
		// string CSSClassFade
		// string CssClassNormal
		// int SKUID
		// OptionCategoryInfo OptionCategory 
    }

    public override void OnAddToCartBefore()
    {
		// Use this to set the field SelectedProductOptions which is available through the CMS.Ecommerce.ProductOptionSelector class that the AvastoneProductOPtionSelector inherits
	
		
		// If you need to dynamically create a Sku Product Option for the category, use the following logic
		
        /*
		int MyNewSku = 0;
		var OptionLookup = SKUInfoProvider.GetSKUOptions(OptionCategoryId, true);
        OptionLookup.WhereCondition = string.Format("SKUOptionCategoryID = {0} and Dimension_Length = {1} and Dimension_Width = {2} and Dimension_Height = {3}", 
            OptionCategory.CategoryID, length, width, height);
        var MatchingOptions = OptionLookup.Execute();
        if (MatchingOptions.Tables[0].Rows.Count == 0)
        {
            // Create new Category Option and add to the Option Category
            SKUInfo newSku = new SKUInfo();
            newSku.SKUEnabled = true;
            newSku.SetValue("SkuEnabled", 1);
            newSku.SKUName = "Something";
            newSku.SKUPrice = 0;
            newSku.SKUGUID = Guid.NewGuid();
            newSku.SKUSellOnlyAvailable = false;
            newSku.SKUSiteID = CMS.SiteProvider.SiteContext.CurrentSiteID;
            newSku.SKUProductType = SKUProductTypeEnum.Product;
            newSku.SKUNeedsShipping = false;
            newSku.SKUPrivateDonation = false;
            newSku.SKUConversionValue = "0";
            newSku.SKUInStoreFrom = DateTime.Now;
            newSku.SKUTrackInventory = TrackInventoryTypeEnum.ByProduct;
            newSku.SetValue("SKUOptionCategoryID", OptionCategoryId);

            // Get order
            newSku.SetValue("SKUOrder", OptionCategory.Children.Count+1);
            newSku.SetValue("SKUAllowAllVariants", false);
            newSku.SetValue("SKUInheritsTaxClasses", false);
            newSku.SetValue("SKUInheritsDiscounts", false);
            newSku.Insert();
			
			// Add the sku to the OptionCategory
            SKUOptionCategoryInfoProvider.AddOptionCategoryToSKU(OptionCategory.CategoryID, newSku.SKUID);
            MyNewSku = newSku.SKUID;
        }
        else
        {
            MyNewSku = (int)MatchingOptions.Tables[0].Rows[0]["SKUID"];
        }

        // Ensure that of the Categories used to create Variants for this product, that all possible Variants are created for the Sizes
        List<int> CategoryIDsUsedInVariants = new List<int>();

        var Categories = VariantHelper.GetProductVariantsCategories(SKUID, false);
        if (Categories != null)
        {
            foreach (var Category in Categories)
            {
                List<int> CategoryIDListTemp = new List<int>();
                CategoryIDListTemp.Add(Category.CategoryID);
                if (VariantHelper.AreCategoriesUsedInVariants(SKUID, CategoryIDListTemp))
                {
                    CategoryIDsUsedInVariants.Add(Category.CategoryID);
                }
            }
        }
        
        // If no Variants exist at all yet, then add in the Category to find possible Variants.
        if (!CategoryIDsUsedInVariants.Contains(OptionCategory.CategoryID))
        {
            CategoryIDsUsedInVariants.Add(OptionCategory.CategoryID);
        }

        // Make sure that variants exist for all possible category types
        var allVariants = VariantHelper.GetAllPossibleVariants(SKUID, CategoryIDsUsedInVariants);
        foreach (var Variant in allVariants)
        {
            if (!Variant.Existing)
            {
                // Create Variant
                Variant.Set();
            }
        }
		
        // Set the SelectedProductOptions to the new Sku ID.
        SelectedProductOptions = MyNewSku.ToString();
		*/
		
        // Raise This event to tell the ShoppingCartItemSelector to reload cached Option Category and Variant information
        OnOptionCategoryModified(EventArgs.Empty);
    }
    public override void OnAddToCartAfter()
    {
        base.OnAddToCartAfter();
    }
    public override void OnAddToWishListBefore()
    {
        base.OnAddToWishListBefore();
    }
    public override string GetSelectedSKUOptionsCustom()
    {
        // This was set in the OnAddToCartBefore()
        return SelectedProductOptions;
    }

    // Reloads selector's data
    public void ReloadSelector()
    {
        LoadSelector();
    }

    /// <summary>
    /// Validates selected/entered product option. If it is valid, returns true, otherwise returns false.
    /// </summary> 
    public override bool IsValid()
    {
		bool IsValid = true;
		// Validate your control here, return true or false and set your own validation messages if false.
        return IsValid;
    }


    /// <summary>
    /// Show no product options label visibility
    /// </summary>
    /// <param name="visible">Is label "no product options" visible</param>
    protected override void SetEmptyInfoVisibility(bool visible)
    {
        base.SetEmptyInfoVisibility(visible);
        if (!IsLiveSite)
        {
            lblNoProductOptions.Visible = visible;
        }
    }
}