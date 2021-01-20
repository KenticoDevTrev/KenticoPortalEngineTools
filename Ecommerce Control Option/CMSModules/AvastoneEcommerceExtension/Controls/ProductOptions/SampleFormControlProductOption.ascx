<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ProductOptions_ProductOptionSelector"
    CodeFile="SampleFormControlProductOption.ascx.cs" %>
<cms:LocalizedLabel ID="lblNoProductOptions" runat="server" Visible="false" EnableViewState="false" ResourceString="optioncategory_edit.noproductoptions" />
<asp:Panel ID="pnlContainer" runat="server" CssClass="ProductOptionSelectorContainer form-group">
    <asp:Panel ID="plnError" runat="server" Visible="false" CssClass="OptionCategoryErrorContainer">
        <cms:LocalizedLabel ID="lblError" runat="server" CssClass="ErrorLabel OptionCategoryError" EnableViewState="false" />
    </asp:Panel>
</asp:Panel>
