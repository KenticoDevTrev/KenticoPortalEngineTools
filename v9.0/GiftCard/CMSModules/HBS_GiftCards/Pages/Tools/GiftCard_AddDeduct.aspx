<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GiftCard_AddDeduct.aspx.cs" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_HBS_GiftCards_Pages_Tools_GiftCard_AddDeduct" %>

<asp:Content runat="server" ContentPlaceHolderID="plcContent" ID="plcOfficeList">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblAmount" AssociatedControlID="tbxAmount" CssClass="control-label" Text="Amount" />
            </div>
            <div class="editing-form-value-cell">
                <asp:TextBox ID="tbxAmount" runat="server" CssClass="form-control" />
                <asp:DropDownList ID="ddlAmountIsDeduction" runat="server" CssClass="form-control" >
                    <asp:ListItem Text="Deduct from Card (-)" Value="True" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Add onto Card (+)" Value="False"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
         <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblCreateHistory" AssociatedControlID="cbxCreateHistory" CssClass="control-label" Text="Create Usage History" ToolTip="If checked, a Gift Card Usage history will be created." />
            </div>
            <div class="editing-form-value-cell">
                <asp:CheckBox runat="server" ID="cbxCreateHistory" Checked="true" ToolTip="If checked, a Gift Card Usage history will be created." />
                
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblHistoryNote" AssociatedControlID="cbxCreateHistory" CssClass="control-label" Text="Usage History Note" ToolTip="The Gift Card Usage History Note" />
            </div>
            <div class="editing-form-value-cell">
                <asp:TextBox ID="tbxHistoryNote" runat="server" TextMode="MultiLine" ToolTip="The Gift Card Usage History Note" CssClass="form-control" />
            </div>
        </div>
    </div>
    <cms:FormSubmitButton runat="server" ID="btnProcesses" OnClick="btnProcesses_Click" />
</asp:Content>
