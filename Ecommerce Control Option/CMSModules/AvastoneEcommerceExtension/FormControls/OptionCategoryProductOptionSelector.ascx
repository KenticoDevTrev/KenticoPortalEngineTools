<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OptionCategoryProductOptionSelector.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_BentleyOptionCategoryProductOptionSelector" %>

<cms:LocalizedLabel runat="server" ID="lblInfoMessage" ResourceString="com.skuoptions.notavailablecreating" CssClass="explanation-text" Visible="false" />
<cms:CMSDropDownList ID="ddlDropDown" runat="server" OnDataBound="SelectionControl_DataBound" DataTextField="SKUName" DataValueField="SKUID" CssClass="DropDownField" />
<cms:CMSCheckBoxList ID="chbCheckBoxes" runat="server" OnDataBound="SelectionControl_DataBound" DataTextField="SKUName" DataValueField="SKUID" />
<cms:CMSRadioButtonList ID="rblRadioButtons" runat="server" OnDataBound="SelectionControl_DataBound" DataTextField="SKUName" DataValueField="SKUID" />
<asp:Label runat="server" ID="lblTextBoxLabel" Visible="false" />
<cms:CMSTextBox ID="txtText" runat="server" />
<asp:Label ID="lblTextPrice" runat="server" />