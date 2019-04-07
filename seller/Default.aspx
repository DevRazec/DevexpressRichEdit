<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="seller.Default" %>

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

        function OnInvoiceCreate(OrderID) {
            grvOrders.PerformCallback('InvoiceNew:' + OrderID);
        }

        function ShowPopupInvoiceEdit(OrderID) {
            PopupViewer.SetContentUrl('InvoiceEdit.aspx?OrderID=' + OrderID);
            PopupViewer.Show();
        }

        function ShowPopupInvoiceView(OrderID) {
            PopupViewer.SetContentUrl('InvoiceView.aspx?OrderID=' + OrderID);
            PopupViewer.Show();
        }

        function OnDownloadInvoicePdf(OrderID) {
            document.getElementById("HiddenOrderID").value = OrderID;
            document.getElementById("btnPdf").click();

            
            //btnPdf.click();
        }

        function OnCustomButtonClick(s, e) {

            if (e.buttonID == "InvoiceNew") {
                grvOrders.GetRowValues(grvOrders.GetFocusedRowIndex(), 'OrderID', OnInvoiceCreate);
            }

            if (e.buttonID == "InvoiceEdit") {
                grvOrders.GetRowValues(grvOrders.GetFocusedRowIndex(), 'OrderID', ShowPopupInvoiceEdit);
            }

            if (e.buttonID == "InvoiceView") {
                grvOrders.GetRowValues(grvOrders.GetFocusedRowIndex(), 'OrderID', ShowPopupInvoiceView);
            }

            if (e.buttonID == "InvoicePdf") {
                grvOrders.GetRowValues(grvOrders.GetFocusedRowIndex(), 'OrderID', OnDownloadInvoicePdf);
            }
        }

    </script>
</head>
<body>
    <div id="content">
        <form id="form1" runat="server">

            <asp:HiddenField ID="HiddenOrderID" runat="server" Value="0" ClientIDMode="Static" />

            <asp:Button ID="btnPdf" runat="server" OnClick="btnPdf_Click" CssClass="displayNone" UseSubmitBehavior="false" />

            <div class="navbar navbar-default" role="navigation">
                <div class="navbar-header">
                    <div class="panel-heading">

                        <a class="btn">
                            <h4>Orders</h4>
                        </a>

                    </div>
                </div>
            </div>

            <div class="container-fluid">
                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12">

                        <dx:ASPxGridView ID="grvOrders" ClientInstanceName="grvOrders" runat="server" AutoGenerateColumns="true" DataSourceID="sdsOrders" KeyFieldName="OrderID" Width="100%" Theme="Moderno"
                            OnCustomCallback="grvOrders_CustomCallback">

                            <Columns>

                                <dx:GridViewCommandColumn VisibleIndex="0" Caption="Invoice Control" ButtonRenderMode="Button" Width="20%">
                                    <CustomButtons>
                                        <dx:GridViewCommandColumnCustomButton Visibility="AllDataRows" ID="InvoiceNew" Text="New">
                                        </dx:GridViewCommandColumnCustomButton>
                                        <dx:GridViewCommandColumnCustomButton Visibility="AllDataRows" ID="InvoiceEdit" Text="Edit">
                                        </dx:GridViewCommandColumnCustomButton>
                                        <dx:GridViewCommandColumnCustomButton Visibility="AllDataRows" ID="InvoiceView" Text="View">
                                        </dx:GridViewCommandColumnCustomButton>
                                        <dx:GridViewCommandColumnCustomButton Visibility="AllDataRows" ID="InvoicePdf" Text="PDF">
                                        </dx:GridViewCommandColumnCustomButton>
                                    </CustomButtons>
                                </dx:GridViewCommandColumn>

                                <dx:GridViewDataTextColumn FieldName="OrderID" VisibleIndex="1" Width="10%">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="CreatedDate" VisibleIndex="2" PropertiesTextEdit-DisplayFormatString="dd/MM/yyyy" Width="10%">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="CustomerName" VisibleIndex="3" Width="20%">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Email" VisibleIndex="4" Width="20%">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Phone" VisibleIndex="5" Width="20%">
                                </dx:GridViewDataTextColumn>

                            </Columns>

                            <ClientSideEvents CustomButtonClick="OnCustomButtonClick" />

                            <SettingsPager PageSize="15" Position="Bottom">
                                <PageSizeItemSettings Items="15, 30, 60" Visible="true" />
                            </SettingsPager>
                            <Settings ShowFilterRow="false" ShowTitlePanel="false" ShowHeaderFilterButton="false" ShowGroupPanel="false" />
                            <SettingsBehavior AllowFocusedRow="true" ColumnResizeMode="Disabled" />
                            <Styles AlternatingRow-Enabled="True" FocusedRow-BackColor="#5cb85c" SelectedRow-BackColor="#F0AD4E"></Styles>
                            <SettingsLoadingPanel Text="" />

                        </dx:ASPxGridView>
                    </div>
                </div>
            </div>

            <dx:ASPxPopupControl ID="PopupViewer" runat="server" AllowDragging="false" PopupAnimationType="Slide" ClientInstanceName="PopupViewer" EnableViewState="false" ShowOnPageLoad="false"
                PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Maximized="true" Modal="true" CloseAction="CloseButton" CloseOnEscape="true" ShowCloseButton="false" LoadContentViaCallback="None" ShowHeader="false">
                <ContentCollection>
                    <dx:PopupControlContentControl ID="PopupControlContentControl2" runat="server">
                    </dx:PopupControlContentControl>
                </ContentCollection>
            </dx:ASPxPopupControl>

            <asp:SqlDataSource ID="sdsOrders" runat="server" ConnectionString="<%$ ConnectionStrings:seller %>"
                SelectCommand="SELECT o.OrderID, o.CreatedDate, c.CustomerName, c.Email, c.Phone 
                           FROM Orders AS o 
                           JOIN Customers c on o.CustomerID = c.CustomerID                          
                           ORDER BY o.OrderID"></asp:SqlDataSource>

        </form>
    </div>
</body>
</html>
