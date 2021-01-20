<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderPaidInFull.ascx.cs" Inherits="CMSModules_HBS_GiftCards_Ecommerce_Checkout_PaidByGiftCardForm_PaidFullyByGiftCard" %>
<!-- Nothing here as the order will auto-complete, this should only be a usable Payment form if the order is fully paid by gift cards !-->
<asp:Label ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel" Visible="false" />
<asp:Panel runat="server" ID="pnlAutoSubmit" Visible="false">
    <script type="text/javascript">
        var $jq = $ || jquery || $cmsj;
        if($jq != null) {
            $jq(document).ready(function () {
                $jq("input[id*='btnProcessPayment']").attr("value", "Ordering...").trigger("click");
            });
        }
    </script>
</asp:Panel>