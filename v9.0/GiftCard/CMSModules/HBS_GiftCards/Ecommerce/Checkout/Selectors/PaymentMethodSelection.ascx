<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_HBS_GiftCards_Ecommerce_Checkout_Selectors_PaymentMethodSelection"  CodeFile="~/CMSModules/HBS_GiftCards/Ecommerce/Checkout/Selectors/PaymentMethodSelection.ascx.cs" %>
<%@ Register Src="~/CMSModules/HBS_GiftCards/Ecommerce/FormControls/PaymentSelector.ascx" TagName="PaymentSelector" TagPrefix="hbs" %>

<asp:Panel ID="pnlPayment" runat="server" CssClass="PanelPayment">    
    <asp:Label runat="server" ID="lblError" Visible="false" CssClass="ErrorLabel" />                         
    <hbs:PaymentSelector  AddAllItemsRecord="false" ID="drpPayment" runat="server" AddNoneRecord="true" DisplayOnlyEnabled="true" IsLiveSite="true" CssClass="SelectorClass" />
</asp:Panel>
