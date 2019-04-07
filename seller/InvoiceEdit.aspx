<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceEdit.aspx.cs" Inherits="seller.InvoiceEdit" %>

<%@ Register Assembly="DevExpress.Web.ASPxRichEdit.v18.2, Version=18.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxRichEdit" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v18.2, Version=18.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />

    <style>
        .displayNone {
            display: none;
        }
    </style>

    <script src="Scripts/jquery-1.9.1.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>

    <script type="text/javascript">

        function OnCloseClick(s, e) {
            $(content).empty();
            window.parent.PopupViewer.Hide();
        }

        function OnEndCallback(s, e) {

        }

        $(document).ready(function () {
            ASPxRichEdit1.PerformCallback('DisableHeader');
        });

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

                        <dx:ASPxButton ID="btnSave" runat="server" EnableTheming="false" CssClass="btn btn-success"
                            Text="Save" UseSubmitBehavior="false" AutoPostBack="false" Width="120px">
                            <ClientSideEvents Click="function(s, e) { ASPxRichEdit1.PerformCallback('InvoiceSave'); }" />
                        </dx:ASPxButton>

                        <dx:ASPxButton ID="btnPrint" runat="server" EnableTheming="false" CssClass="btn btn-info"
                            Text="Print" UseSubmitBehavior="false" AutoPostBack="false" Width="120px">
                            <ClientSideEvents Click="function(s, e) { ASPxRichEdit1.commands.filePrint.execute(); }" />
                        </dx:ASPxButton>

                        <dx:ASPxButton ID="btnPdf" runat="server" EnableTheming="false" CssClass="btn btn-warning"
                            Text="PDF Donwload" UseSubmitBehavior="false" AutoPostBack="false" Width="120px" OnClick="btnPdf_Click">
                        </dx:ASPxButton>

                        <a class="btn">
                            <h4>Invoice Edit</h4>
                        </a>

                    </div>
                </div>
            </div>

            <div class="container-fluid">
                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12">

                        <dx:ASPxRichEdit ID="ASPxRichEdit1" ClientInstanceName="ASPxRichEdit1" runat="server" WorkDirectory="~\App_Data\WorkDirectory" Width="100%" Height="800px"
                            OnInit="ASPxRichEdit1_Init"
                            OnCallback="ASPxRichEdit1_Callback">

                            <ClientSideEvents EndCallback="OnEndCallback" />

                        </dx:ASPxRichEdit>

                    </div>
                </div>
            </div>

        </form>
    </div>
</body>
</html>
