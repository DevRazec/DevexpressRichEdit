using DevExpress.Web.ASPxRichEdit;
using DevExpress.Web.Internal;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace seller
{
    public partial class InvoiceEdit : System.Web.UI.Page
    {      
        string conStr = ConfigurationManager.ConnectionStrings["seller"].ConnectionString;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsCallback && !IsPostBack)
            {
                HiddenInvoiceId.Value = Request.Params["OrderID"];

                var arrayInvoice = new ArrayList();

                using (SqlConnection con = new SqlConnection(conStr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;

                        cmd.CommandText = @"select i.InvoiceID, i.OrderID, i.InvoiceRtf 
                                            from Invoices i                                                             
                                            where i.OrderID = " + HiddenInvoiceId.Value + " order by i.InvoiceID desc";

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            arrayInvoice.Add(reader["InvoiceID"]);
                            arrayInvoice.Add(reader["OrderID"]);
                            arrayInvoice.Add(reader["InvoiceRtf"]);
                        }

                        reader.Close();                    
                        con.Close();                      
                    }
                }

                if (arrayInvoice.Count > 0)
                {
                    // Stream var
                    MemoryStream updatedStream = new MemoryStream();

                    // Load stream from database
                    MemoryStream stream = new MemoryStream((byte[])arrayInvoice[2]);

                    // Load document from stream
                    RichEditDocumentServer docServer = new RichEditDocumentServer();
                    docServer.LoadDocument(stream, DocumentFormat.Rtf);

                    // Create document server
                    Document document = docServer.Document;

                    // If the document is not protected
                    if (!document.IsDocumentProtected)
                    {
                        // Protect the document with a password
                        document.Protect("123", DocumentProtectionType.ReadOnly);

                        // Tables 
                        DevExpress.XtraRichEdit.API.Native.Table table1 = document.Tables[1];
                        DocumentRange rangeTable1 = table1.Rows[1].Range;

                        DevExpress.XtraRichEdit.API.Native.Table table2 = document.Tables[2];
                        DocumentRange rangeTable2 = table2.Rows[1].Range;

                        // Add table1 as range / Allow editing row 1 over table1 
                        RangePermissionCollection coletionTable1 = document.BeginUpdateRangePermissions();
                        RangePermission permissionTable1 = coletionTable1.CreateRangePermission(rangeTable1);
                        permissionTable1.Group = "Everyone";
                        coletionTable1.Add(permissionTable1);
                        document.EndUpdateRangePermissions(coletionTable1);

                        // Add table2 as range / Allow editing row 1 over table2
                        RangePermissionCollection coletionTable2 = document.BeginUpdateRangePermissions();
                        RangePermission permissionTable2 = coletionTable2.CreateRangePermission(rangeTable2);
                        permissionTable2.Group = "Everyone";
                        coletionTable2.Add(permissionTable2);
                        document.EndUpdateRangePermissions(coletionTable2);
                    }

                    // Update updatedStream var from stream var                    
                    document.SaveDocument(updatedStream, DocumentFormat.Rtf);                   

                    // Open document from updatedStream var
                    ASPxRichEdit1.Open(Guid.NewGuid().ToString(), DocumentFormat.Rtf, () => { return updatedStream.ToArray(); });
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ASPxRichEdit1_Init(object sender, EventArgs e)
        {
            ASPxRichEdit richEdit = (ASPxRichEdit)sender;

            richEdit.CreateDefaultRibbonTabs(true);

            //Hide Mail Merge Tab
            richEdit.RibbonTabs.Find(tab => tab.Text == "Mail Merge").Visible = false;

            //Hide Page Layout Tab
            richEdit.RibbonTabs.Find(tab => tab.Text == "Page Layout").Visible = false;

            //Hide File Tab
            richEdit.RibbonTabs.Find(tab => tab.Text == "File").Visible = false;

            //Hide Group Buttons by Name
            richEdit.RibbonTabs.Find(tab => tab.Text == "Insert").Groups.Find(group => group.Text == "Pages").Visible = false;
            richEdit.RibbonTabs.Find(tab => tab.Text == "Insert").Groups.Find(group => group.Text == "Illustrations").Visible = false;
            richEdit.RibbonTabs.Find(tab => tab.Text == "Insert").Groups.Find(group => group.Text == "Header & Footer").Visible = false;
            //richEdit.RibbonTabs.Find(tab => tab.Text == "Page Layout").Groups.Find(group => group.Text == "Background").Visible = false;
            //richEdit.RibbonTabs.Find(tab => tab.Text == "Page Layout").Groups.Find(group => group.Text == "Page Setup").Visible = false;

            //Hide Page Layout Buttons by Name Id it is not working well
            //richEdit.RibbonTabs.Find(tab => tab.Text == "Page Layout").Groups.Find(group => group.Text == "Page Setup").Items[0].Visible = false;
            //richEdit.RibbonTabs.Find(tab => tab.Text == "Page Layout").Groups.Find(group => group.Text == "Page Setup").Items[1].Visible = false;
            //richEdit.RibbonTabs.Find(tab => tab.Text == "Page Layout").Groups.Find(group => group.Text == "Page Setup").Items[2].Visible = false;
            //richEdit.RibbonTabs.Find(tab => tab.Text == "Page Layout").Groups.Find(group => group.Text == "Page Setup").Items[3].Visible = false;

            //Hide Page Layout Buttons by Index Id it is not working well
            //richEdit.RibbonTabs[3].Groups[1].Items[0].Visible = false;
            //richEdit.RibbonTabs[3].Groups[0].Items[0].Visible = false;
            //richEdit.RibbonTabs[3].Groups[0].Items[1].Visible = false;
            //richEdit.RibbonTabs[3].Groups[0].Items[2].Visible = false;
            //richEdit.RibbonTabs[3].Groups[0].Items[3].Visible = false;            

            //Disable Header and Footer
            //richEdit.Settings.DocumentCapabilities.HeadersFooters = DevExpress.XtraRichEdit.DocumentCapability.Disabled;

            //Hide Bookmark
            richEdit.Settings.DocumentCapabilities.Bookmarks = DevExpress.XtraRichEdit.DocumentCapability.Hidden;

            //Hide Hyperlink
            richEdit.Settings.DocumentCapabilities.Hyperlinks = DevExpress.XtraRichEdit.DocumentCapability.Hidden;

            //Hide buttons
            richEdit.Settings.Behavior.SaveAs = DevExpress.XtraRichEdit.DocumentCapability.Hidden;
            richEdit.Settings.Behavior.Open = DevExpress.XtraRichEdit.DocumentCapability.Hidden;
            richEdit.Settings.Behavior.CreateNew = DevExpress.XtraRichEdit.DocumentCapability.Hidden;

            // Show Confirmation Message
            richEdit.ShowConfirmOnLosingChanges = false;
                        
            // Definitions Page
            //RichEditDocumentServer server = new RichEditDocumentServer();
            //server.Document.Sections[0].Page.PaperKind = System.Drawing.Printing.PaperKind.A4;
        }

        // Callback 
        protected void ASPxRichEdit1_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            switch (e.Parameter)
            {
                case ("InvoiceSave"):
                    InvoiceSave();
                    break;

                case ("DisableHeader"):
                    ASPxRichEdit1.Settings.DocumentCapabilities.HeadersFooters = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
                    break;              
            }
        }
        
        // Download PDF
        protected void btnPdf_Click(object sender, EventArgs e)
        {
            using (MemoryStream pdfStream = new MemoryStream())
            {
                ASPxRichEdit1.ExportToPdf(pdfStream);
                HttpUtils.WriteFileToResponse(Page, pdfStream, "Invoice-Number-" + HiddenInvoiceId.Value, true, "pdf");
            }
        }

        // Saving Invoice at DataBase
        public void InvoiceSave()
        {           
            MemoryStream rtfStream = new MemoryStream();
            MemoryStream pdfStream = new MemoryStream();            

            ASPxRichEdit1.SaveCopy(rtfStream, DocumentFormat.Rtf);            
            ASPxRichEdit1.ExportToPdf(pdfStream);           

            RichEditDocumentServer docServer = new RichEditDocumentServer();
            docServer.LoadDocument(rtfStream, DocumentFormat.Rtf);            

            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;

                    cmd.CommandText = @"update Invoices set InvoiceRtf = @InvoiceRtf, InvoicePdf = @InvoicePdf
                                        where OrderID = @OrderID";

                    cmd.Parameters.AddWithValue("@OrderID", HiddenInvoiceId.Value);
                    cmd.Parameters.AddWithValue("@InvoiceRtf", SqlDbType.VarBinary).Value = rtfStream.ToArray();
                    cmd.Parameters.AddWithValue("@InvoicePdf", SqlDbType.VarBinary).Value = pdfStream.ToArray();

                    con.Open();
                    cmd.ExecuteNonQuery();                   
                    con.Close();
                }
            }                     
        }        
    }
}