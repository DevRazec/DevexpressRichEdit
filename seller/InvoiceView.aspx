<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceView.aspx.cs" Inherits="seller.InvoiceView" %>

<%@ Register Assembly="DevExpress.Web.ASPxRichEdit.v18.2, Version=18.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxRichEdit" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v18.2, Version=18.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <script src="Scripts/jquery-1.9.1.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>    

    <script type="text/javascript">
        function OnCloseClick(s, e) {
            $(content).empty();
            window.parent.PopupViewer.Hide();
        }
    </script>

</head>
<body>
    <div id="content">
        <form id="form1" runat="server">

            <asp:HiddenField ID="HiddenInvoiceId" runat="server" Value="0" ClientIDMode="Static" />

            <div class="navbar navbar-default" role="navigation">
                <div class="navbar-header">
                    <div class="panel-heading">

                        <dx:ASPxButton ID="btnBack" runat="server" EnableTheming="false" CssClass="btn btn-danger"
                            Text="Back" UseSubmitBehavior="false" AutoPostBack="false" Width="120px">
                            <ClientSideEvents Click="function(s, e) { OnCloseClick(s, e); }" />
                        </dx:ASPxButton>

                        <a class="btn">
                            <h4>Invoice View</h4>
                        </a>

                    </div>
                </div>
            </div>

            <div class="container-fluid">
                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12">
                        <dx:ASPxRichEdit ID="ASPxRichEdit1" ClientInstanceName="ASPxRichEdit1" runat="server" RibbonMode="None" stylesruler-mode="none" ReadOnly="True" WorkDirectory="~\App_Data\WorkDirectory" Width="100%" Height="800px" 
                            OnInit="ASPxRichEdit1_Init">
                        </dx:ASPxRichEdit>
                    </div>
                </div>
            </div>

        </form>
    </div>
</body>
</html>
