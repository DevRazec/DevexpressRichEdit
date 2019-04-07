using DevExpress.Web.ASPxRichEdit;
using DevExpress.XtraRichEdit;
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
    public partial class InvoiceView : System.Web.UI.Page
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
                    MemoryStream rtfStream = new MemoryStream((byte[])arrayInvoice[2]);
                    ASPxRichEdit1.Open("rtf" + Guid.NewGuid().ToString(), DocumentFormat.Rtf, () => { return rtfStream.ToArray(); });                    
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ASPxRichEdit1_Init(object sender, EventArgs e)
        {
            ASPxRichEdit richEdit = (ASPxRichEdit)sender;

            //Disable Horizontal Ruler           
            richEdit.Settings.HorizontalRuler.Visibility = RichEditRulerVisibility.Hidden;
        }
    }
}