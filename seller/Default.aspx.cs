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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace seller
{
    public partial class Default : System.Web.UI.Page
    {     
        string conStr = ConfigurationManager.ConnectionStrings["seller"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // Callback 
        protected void grvOrders_CustomCallback(object sender, DevExpress.Web.ASPxGridViewCustomCallbackEventArgs e)
        {
            string[] parameter = e.Parameters.Split(':');

            switch (parameter[0])
            {
                case ("InvoiceNew"):

                    InvoiceCreate(parameter[1]);

                    break;
            }
        }

        // Create New Invoice
        public void InvoiceCreate(string OrderID)
        {
            MemoryStream rtfStream = new MemoryStream();
            MemoryStream pdfStream = new MemoryStream();

            RichEditDocumentServer docServer = new RichEditDocumentServer();
            docServer.Document.LoadDocument(Server.MapPath("~/App_Data/WorkDirectory/Invoice.rtf"), DocumentFormat.Rtf);

            // Rtf file
            Document rtf = docServer.Document;

            // Customer's Data
            var arrayCustomer = SelectOrder(OrderID);

            var arrayOrderDetail = SelectOrderDetails(OrderID);

            // Replace Header String
            SubDocument header = docServer.Document.Sections[0].BeginUpdateHeader();
            header.ReplaceAll("#invoiceid", arrayCustomer[7].ToString(), SearchOptions.WholeWord);

            // Replace Fields
            rtf.ReplaceAll("#orderid", arrayCustomer[0].ToString(), SearchOptions.WholeWord);
            rtf.ReplaceAll("#customerid", arrayCustomer[1].ToString(), SearchOptions.WholeWord);
            rtf.ReplaceAll("#date", Convert.ToDateTime(arrayCustomer[2]).ToShortDateString(), SearchOptions.WholeWord);
            rtf.ReplaceAll("#customer", arrayCustomer[3].ToString(), SearchOptions.WholeWord);
            rtf.ReplaceAll("#address", arrayCustomer[4].ToString().Trim(), SearchOptions.WholeWord);
            rtf.ReplaceAll("#email", arrayCustomer[5].ToString().Trim(), SearchOptions.WholeWord);
            rtf.ReplaceAll("#phone", arrayCustomer[6].ToString(), SearchOptions.WholeWord);

            docServer.SaveDocument(rtfStream, DocumentFormat.Rtf);
            docServer.ExportToPdf(pdfStream);

            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;

                    cmd.CommandText = @"insert into Invoices (OrderID, InvoiceRtf, InvoicePdf) 
                                        values (@OrderID, @InvoiceRtf, @InvoicePdf)";

                    cmd.Parameters.AddWithValue("@OrderID", OrderID);
                    cmd.Parameters.AddWithValue("@InvoiceRtf", SqlDbType.VarBinary).Value = rtfStream.ToArray();
                    cmd.Parameters.AddWithValue("@InvoicePdf", SqlDbType.VarBinary).Value = pdfStream.ToArray();

                    con.Open();
                    cmd.ExecuteNonQuery();                    
                    con.Close();
                }
            }
        }

        // Select Order
        public ArrayList SelectOrder(string OrderID)
        {
            var arrayObj = new ArrayList();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;

                    cmd.CommandText = @"select top 1 o.OrderID, o.CustomerID, o.CreatedDate, c.CustomerName, c.Address, c.Email, c.Phone,
                                        (select top 1 (i.InvoiceID)
                                        from Invoices i
                                        where i.OrderID = o.OrderID order by i.InvoiceID desc) as InvoiceID
                                        from Orders o 
                                        join Customers c on o.CustomerID = c.CustomerID                              
                                        where o.OrderID = " + OrderID;

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        arrayObj.Add(reader["OrderID"]);
                        arrayObj.Add(reader["CustomerID"]);
                        arrayObj.Add(reader["CreatedDate"]);
                        arrayObj.Add(reader["CustomerName"]);
                        arrayObj.Add(reader["Address"]);
                        arrayObj.Add(reader["Email"]);
                        arrayObj.Add(reader["Phone"]);
                        arrayObj.Add(reader["InvoiceID"]);
                    }

                    reader.Close();                   
                    con.Close();
                }
            }
            return arrayObj;
        }

        // Select Order's Products
        public ArrayList SelectOrderDetails(string OrderID)
        {
            var arrayObj = new ArrayList();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;

                    cmd.CommandText = @"select od.OrderDetailID, od.OrderID, od.ProductID, p.ProductName, p.UnitPrice 
                                        from OrderDetails od 
                                        join Products p on od.ProductID = p.ProductID                              
                                        where od.OrderID = " + OrderID;

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        arrayObj.Add(new
                        {
                            OrderDetailID = reader["OrderDetailID"],
                            OrderID = reader["OrderID"],
                            ProductID = reader["ProductID"],
                            ProductName = reader["ProductName"],
                            UnitPrice = reader["UnitPrice"],
                        });
                    }

                    reader.Close();                   
                    con.Close();
                }
            }
            return arrayObj;
        }

        // Download PDF
        protected void btnPdf_Click(object sender, EventArgs e)
        {
            var arrayObj = new ArrayList();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;

                    cmd.CommandText = @"select top 1 i.InvoiceID, i.OrderID, i.InvoicePdf 
                                        from Invoices i                                                             
                                        where i.OrderID = " + HiddenOrderID.Value + " order by i.InvoiceID desc";

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        arrayObj.Add(reader["InvoiceID"]);
                        arrayObj.Add(reader["OrderID"]);
                        arrayObj.Add(reader["InvoicePdf"]);
                    }

                    reader.Close();                 
                    con.Close();

                    var pdfInvoice = new MemoryStream((byte[])arrayObj[2]);
                    HttpUtils.WriteFileToResponse(Page, pdfInvoice, "Invoice-Number-" + arrayObj[0].ToString(), true, "pdf");
                }
            }
        }

        #region Code Samples

        //var domains = new Dictionary<string, object>();

        //object[] values = new object[reader.FieldCount];
        //reader.GetValues(values);
        //arrayObj.Add(values);

        //List<object> lstObj = new List<object>();

        //RangePermission permissionTable1 = rangeTable1.CreateRangePermission(document.Tables[1].Rows[1].Range);
        //permissionTable1.Group = "Everyone";                        
        //rangeTable1.Add(permissionTable1);                        
        //document.EndUpdateRangePermissions(rangeTables);

        // Table 2 permissions 
        //RangePermissionCollection rangeTable2 = document.BeginUpdateRangePermissions();
        //RangePermission permissionTable2 = rangeTable2.CreateRangePermission(document.Tables[2].Rows[1].Range);
        //permissionTable2.Group = "Everyone";
        //rangeTable2.Add(permissionTable2); 

        //DocumentPosition positionTable1 = document.CaretPosition(document.Tables[1].Rows[1]);

        //DocumentPosition positionTable1 = document.CreatePosition(table1.Rows[1].Range.End.ToInt());
        //int positionTable1 = document.CaretPosition(table1.Range.End.ToInt() - 1);
        //richEditControl1.Document.Tables.GetTableCell(richEditControl1.Document.CreatePosition(documentPosition));

        //RangePermission rangePermissions = new RangePermission(rangeTable1.CreateRangePermission(document.Tables[1].Rows[1].Range));

        // Customer's table                        
        //DevExpress.XtraRichEdit.API.Native.Table tblCustomer = document.Tables[1];

        // Table 1 cell permissions 
        //RangePermissionCollection rangeTable0 = document.BeginUpdateRangePermissions();
        //RangePermission permissionTable1 = rangeTable0.CreateRangePermission(tbl1Borders).Range;
        //permissionTable1.Group = "Everyone";
        //rangeTable1.Add(permissionTable1);
        //docServer.Options.RangePermissions.HighlightColor = Color.Yellow;
        //docServer.Options.RangePermissions.HighlightBracketsColor = Color.YellowGreen;
        //document.EndUpdateRangePermissions(rangeTable1);

        //DevExpress.XtraRichEdit.API.Native.TableBorders tbl1Borders = document.Tables[1].Borders;

        //DocumentRange rangeAdmin = tblCustomer.Range;

        //// Begin range of permissions 
        //RangePermissionCollection rangePermissions = document.BeginUpdateRangePermissions();

        //// Create range permission from table 1 range / Allow editing only the table 1
        //RangePermission permission = rangePermissions.CreateRangePermission(rangeAdmin);                        

        //// Set Everyone group
        //permission.Group = "Everyone";

        //// Add range permission
        //rangePermissions.Add(permission);

        //// End range of permissions 
        ////document.EndUpdateRangePermissions(rangePermissions);

        //docServer.Document.Protect("123");
        //docServer.Options.Authentication.UserName = "User";
        //docServer.Options.Authentication.Group = "User";
        //docServer.Options.RangePermissions.Visibility = DevExpress.XtraRichEdit.RichEditRangePermissionVisibility.Hidden;

        //using (var stream = new MemoryStream())
        //{
        //    docServer.SaveDocument(stream, DocumentFormat.Rtf);
        //    stream.Position = 0;
        //    ASPxRichEdit1.Open("rtf" + Guid.NewGuid().ToString(), DocumentFormat.Rtf, () => { return stream.ToArray(); });
        //}

        //FileStream stream = new FileStream(Server.s.MapPath(rtf), FileMode.Open);

        //docServer.Document.Paragraphs.Insert(docServer.Document.Range.End);
        //DocumentPosition pos = docServer.Document.CreatePosition(docServer.Document.Range.End.ToInt() - 2);
        //DocumentRange range = docServer.Document.InsertDocumentContent(pos, rtfStream, DocumentFormat.Rtf);

        //using (MemoryStream streamWithModifiedRichEditContent = new MemoryStream())
        //{
        //    docServer.SaveDocument(streamWithModifiedRichEditContent, DocumentFormat.Rtf);
        //    streamWithModifiedRichEditContent.Position = 0;
        //    ASPxRichEdit1.Open(Guid.NewGuid().ToString(), DocumentFormat.Rtf, () => { return streamWithModifiedRichEditContent; });
        //}

        //ASPxRichEdit1.Open("rtf" + Guid.NewGuid().ToString(), DocumentFormat.Rtf, () => { return rtfStream.ToArray(); });

        //DocumentRange rangeAdmin = docServer.Document.Tables[1].Range;

        //DocumentRange rangeAdmin = docServer.Document.Range;

        //RangePermissionCollection rangePermissions = docServer.Document.BeginUpdateRangePermissions();

        //RangePermission permission = rangePermissions.CreateRangePermission(rangeAdmin);
        //permission.UserName = "Admin";
        //permission.Group = "Admin";
        //rangePermissions.Add(permission);

        //docServer.Document.EndUpdateRangePermissions(rangePermissions);
        //// Enforce protection and set password.
        //docServer.Document.Protect("123");          

        //tblCustomer.BeginUpdate();
        //tblCustomer.EndUpdate();
        //rtf.ReplaceAll.InsertSingleLineText(tblCustomer[0, 0].Range.Start, "Name");
        //SubDocument customer = docServer.Document.Sections[0].DifferentFirstPage.();
        //docServer.Options.MailMerge.DataSource = GetMerged();

        //rtf.AppendText("ABCDEFGH");
        //DocumentRange r1 = rtf.CreateRange(1, 3);
        //DocumentPosition pos1 = rtf.CreatePosition(2);
        //DocumentRange r2 = rtf.InsertText(pos1, ">>NewText<<");
        //string s1 = String.Format("Range r1 starts at {0}, ends at {1}", r1.Start, r1.End);
        //string s2 = String.Format("Range r2 starts at {0}, ends at {1}", r2.Start, r2.End);
        //rtf.Paragraphs.Append();
        //rtf.AppendText(s1);
        //rtf.Paragraphs.Append();
        //rtf.AppendText(s2);

        //e.Handled = true;
        //ASPxRichEdit1.SaveCopy(DocumentFormat.Rtf);
        //RichEdit.Save(fileName)
        //InvoiceSaving();

        //comm.Parameters.AddWithValue("@InvoiceRtf", SqlDbType.VarBinary).Value = rtfStream.ToArray();
        //comm.Parameters.AddWithValue("@InvoicePdf", SqlDbType.VarBinary).Value = pdfStream.ToArray();

        //comm.Parameters.AddWithValue("@InvoiceRtf", SqlDbType.VarBinary).Value = byteRtf;
        //comm.Parameters.AddWithValue("@InvoicePdf", SqlDbType.VarBinary).Value = bytePdf;

        //comm.Parameters.AddWithValue("@InvoiceRtf", SqlDbType.VarBinary).Value = System.Text.Encoding.ASCII.GetString(byteRtf);
        //comm.Parameters.AddWithValue("@InvoicePdf", SqlDbType.VarBinary).Value = System.Text.Encoding.ASCII.GetString(bytePdf);

        //RichEditDocumentServer docServer = new RichEditDocumentServer();
        //docServer.Document.LoadDocument(rtfStream, DocumentFormat.Rtf);
        //docServer.SaveDocument(rtfStream, DocumentFormat.Rtf);

        //rtfStream.Position = 0;

        //ASPxRichEdit1.SaveCopy((byte[])rtfStream, DocumentFormat.Rtf);           

        //using (MemoryStream rtfStream = new MemoryStream())
        //{
        //    RichEditDocumentServer docServer = new RichEditDocumentServer();
        //    docServer.Document.LoadDocument(rtfStream, DocumentFormat.Rtf);                
        //    docServer.SaveDocument(rtfStream, DocumentFormat.Rtf);
        //    rtfStream.Position = 0;

        //    //ASPxRichEdit1.SaveCopy((byte[])rtfStream, DocumentFormat.Rtf);

        //    SqlCommand comm = new SqlCommand();
        //    comm.Connection = con;

        //    comm.CommandText = @"update Invoices set InvoiceRtf = @InvoiceRtf
        //                         where OrderID = @OrderID";               

        //    comm.Parameters.AddWithValue("@OrderID", HiddenInvoiceId.Value);
        //    comm.Parameters.AddWithValue("@InvoiceRtf", SqlDbType.VarBinary).Value = rtfStream.ToArray();                

        //    con.Open();
        //    comm.ExecuteNonQuery();
        //    con.Close();
        //}

        //Open Stream
        //ASPxRichEdit1.Open(Guid.NewGuid().ToString(), DevExpress.XtraRichEdit.DocumentFormat.Rtf, () =>
        //{
        //    System.IO.FileStream fakeStreamLoadedFromFileButLoadItFromDbInstead = File.OpenRead(MapPath("~/App_Data/WorkDirectory/Certificado.rtf"));                    
        //    fakeStreamLoadedFromFileButLoadItFromDbInstead.Seek(0, SeekOrigin.Begin);
        //    return fakeStreamLoadedFromFileButLoadItFromDbInstead;
        //});

        //var pdfInvoice = new MemoryStream((byte[])varTable.Rows[0]["InvoicePdf"]);

        //ASPxRichEdit1.SaveDocument(rtfStream, DocumentFormat.Rtf);
        //server.SaveDocument(memoryStream, DocumentFormat.Rtf);

        //string SessionKey = "EditedDocuemntID";
        //protected string EditedDocuemntID
        //{
        //    get { return (string)Session[SessionKey] ?? string.Empty; }
        //    set { Session[SessionKey] = value; }
        //}

        //RichEditDocumentServer server = new RichEditDocumentServer();
        //server.Document.SetPageBackground(Color.Yellow);
        //MemoryStream memoryStream = new MemoryStream();
        //server.SaveDocument(memoryStream, DocumentFormat.Rtf);
        //ASPxRichEdit1.Open("doc" + Guid.NewGuid().ToString(), DocumentFormat.Rtf, () => { return memoryStream.ToArray(); });

        //var streamPdf = new MemoryStream(docServer.ExportToPdf(pdfInvoice));

        //using (MemoryStream pdfInvoice = new MemoryStream())
        //{
        //    docServer.ExportToPdf(pdfInvoice);
        //   HttpUtils.WriteFileToResponse(Page, pdfInvoice, "ExportedDocument", true, "pdf");
        //}

        //var varTable = SelectOrderDetails(OrderID);

        //HttpContext.Current.Response.Clear();
        //HttpContext.Current.Response.ClearHeaders();
        //HttpContext.Current.Response.ClearContent();
        //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //HttpContext.Current.Response.ContentType = "application/pdf";
        //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename = Certificado-Analitico-" + varTable.Rows[0]["InvoiceID"] + ".pdf");
        //HttpContext.Current.Response.AddHeader("Content-Length", pdfBuffer.Length.ToString());
        //HttpContext.Current.Response.BinaryWrite(pdfBuffer);
        //HttpContext.Current.Response.Flush();
        //HttpContext.Current.Response.End();

        #endregion
    }
}