<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftCardEntry.ascx.cs" Inherits="CMSModules_HBS_GiftCards_FormControls_GiftCardEntry_GiftCardEntry" %>
<asp:Panel runat="server" ID="pnlGiftCardEntryContainer" CssClass="GiftCardEntry">
    <asp:Panel runat="server" ID="pnlUIOnly" Visible="false">
        <style>
            div[id*="pnlGiftCardEntryContainer"] {
                margin: 20px;
            }

            .RemoveGiftCard {
                padding-left: 10px;
            }

            input[id*="tbxGiftCardCode"] {
                width: 300px !important;
                display: inline-block !important;
            }

            div[id*="pnlAppliedGiftCards"] {
                clear: both;
                padding-top: 10px !important;
            }
        </style>
    </asp:Panel>

    <asp:Literal runat="server" ID="ltrGiftCardHeader" />

    <asp:Panel runat="server" ID="pnlError" CssClass="alert alert-warning">
        <%-- You can set these values through the Localize module in Kentico --%>
        <cms:LocalizedLiteral ID="ltrNoGiftCardFound" runat="server" ResourceString="hbs_giftcard.NoGiftCardFound" />
        <cms:LocalizedLiteral ID="ltrGiftCardDisabled" runat="server" ResourceString="hbs_giftcard.GiftCardDisabled" />
        <cms:LocalizedLiteral ID="ltrGiftCardNotUsableByCustomer" runat="server" ResourceString="hbs_giftcard.GiftCardNotUsableByCustomer" />
        <cms:LocalizedLiteral ID="ltrGiftCardNoBalance" runat="server" ResourceString="hbs_giftcard.GiftCardNoBalance" />
        <cms:LocalizedLiteral ID="ltrOrderAlreadyPaid" runat="server" ResourceString="hbs_giftcard.OrderAlreadyPaid" />
    </asp:Panel>

    <section class="form-container">

        <asp:Panel runat="server" ID="pnlValueBefore" CssClass="form-group row">
            <div class="col-lg-2 col-md-3 col-sm-4 col-xs-6">
                <cms:LocalizedLiteral runat="server" ID="ltrPriceBeforeLabel" ResourceString="hbs_giftcard.currenttotal" />
            </div>
            <div class="col-lg-10 col-md-9 col-sm-8 col-xs-6">
                <asp:Literal runat="server" ID="ltrPriceBefore" />
            </div>
        </asp:Panel>

        <div class="form-group row">
            <asp:Panel runat="server" ID="pnlAppliedGiftCards"></asp:Panel>
        </div>

        <asp:Panel runat="server" ID="pnlValueAfter" CssClass="form-group row">
                <div class="col-lg-2 col-md-3 col-sm-4 col-xs-8">
                    <cms:LocalizedLiteral runat="server" ID="ltrPriceAfterLabel" ResourceString="hbs_giftcard.newtotalaftergiftcards" />
                </div>
                <div class="col-lg-10 col-md-9 col-sm-8 col-xs-4">
                    <asp:Literal runat="server" ID="ltrPriceAfter" />
                </div>
        </asp:Panel>

        <div class="form-group row HBS_GiftCard">
            <div class="col-lg-2 col-md-3 col-sm-4 col-xs-12 editing-form-label-cell">
                <cms:FormLabel runat="server" ID="lblGiftCardCode" AssociatedControlID="tbxGiftCardCode" ResourceString="hbs_giftcard.EnterGiftCardLabel" CssClass="control-label editing-form-label" />
            </div>
            <div class="col-lg-10 col-md-9 col-sm-8 col-xs-12 editing-form-value-cell form-input">
                <asp:TextBox runat="server" ID="tbxGiftCardCode" CssClass="form-control"></asp:TextBox>
                <asp:Button runat="server" ID="btnApplyGiftCard" CssClass="btn btn-primary" Text="Apply" OnClick="btnApplyGiftCard_Click" />
            </div>
        </div>

    </section>

</asp:Panel>
