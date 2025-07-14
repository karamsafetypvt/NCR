using FPDAL.Data;
using NCRBAL.Interface;
using NCRDAL.Data;
using NCRMODEL.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.IO;
using System.Diagnostics.Eventing.Reader;

namespace NCRBAL.Business
{
    public class NCRAccess : INCR
    {
        NCRData data = new NCRData();

        string Receipt1_Unit1 = ConfigurationManager.AppSettings["Receipt1_Unit1"].ToString();
        string Receipt2_Unit1 = ConfigurationManager.AppSettings["Receipt2_Unit1"].ToString();
        string Receipt1_Unit2 = ConfigurationManager.AppSettings["Receipt1_Unit2"].ToString();
        string Receipt2_Unit2 = ConfigurationManager.AppSettings["Receipt2_Unit2"].ToString();
        public NCRModel GetNCRData()
        {
            NCRModel response = new NCRModel();
            List<NCRModel> NCRModelList = new List<NCRModel>();
            try
            {
                DataSet ds = data.SelectAll();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            response = new NCRModel();
                            response.SrNo = Convert.ToString(row["row"] ?? 0);
                            response.NCRNumber = Convert.ToString(row["Ncr_No"] ?? 0);
                            response.MRNno = Convert.ToString(row["MRN"].ToString());
                            response.PO = Convert.ToString(row["PO_Number"] ?? string.Empty);
                            response.VendorCode = Convert.ToString(row["Vendor"] ?? string.Empty);
                            response.TotalRecieved = Convert.ToString(row["Total_Received"] ?? string.Empty);
                            response.TotalRejected = Convert.ToString(row["Total_Reject"] ?? string.Empty);
                            response.NCRID = Convert.ToString(row["ID"] ?? string.Empty);
                            response.Status = Convert.ToString(row["Status"] ?? string.Empty);
                            response.NCRDate = Convert.ToString(row["Generated_date"] ?? string.Empty);
                            NCRModelList.Add(response);
                        }
                    }
                }
                response.ncrmodelList = NCRModelList;
                return response;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRAccess", "GetNCRData");
                return null;
            }

        }
        public NCRModel DataByPOID(NCRModel model)
        {
            NCRModel response = new NCRModel();
            List<NCRModel> NCRModelList = new List<NCRModel>();
            try
            {
                DataSet ds = data.DataByPOID(model);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        response.Contact = Convert.ToString(ds.Tables[0].Rows[0]["Name"] ?? 0);
                        response.Phone = Convert.ToString(ds.Tables[0].Rows[0]["Phone"].ToString());
                        response.Address = Convert.ToString(ds.Tables[0].Rows[0]["Address"] ?? string.Empty);
                        response.VendorCode = Convert.ToString(ds.Tables[0].Rows[0]["Vendor_Name"] ?? string.Empty);
                        response.Email = Convert.ToString(ds.Tables[0].Rows[0]["Email_Address"] ?? string.Empty);
                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    response.MRNNoList = UtilityAccess.MRNNumberList(ds.Tables[1], 1);
                }
                return response;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRAccess", "DataByPOID");
                return null;
            }
        }
        public NCRModel DataByMRNID(NCRModel model)
        {
            NCRModel response = new NCRModel();
            List<NCRModel> NCRModelList = new List<NCRModel>();
            try
            {
                DataSet ds = data.DataByMRNID(model);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        response.ItemCodeList = UtilityAccess.ItemCodeList(ds.Tables[0], 1);
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRAccess", "DataByMRNID");
                return null;
            }
        }
        public NCRModel GetItemDescription(NCRModel model)
        {
            NCRModel response = new NCRModel();
            List<NCRModel> NCRModelList = new List<NCRModel>();
            try
            {
                DataSet ds = data.GetItemDescription(model);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        response.ItemDescription = ds.Tables[0].Rows[0]["DESCRIPTION"].ToString();
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRAccess", "GetItemDescription");
                return null;
            }
        }
        public NCRModel AddOrEdit(NCRModel model)
        {
            Int32 returnResult = 0;
            Int32 NCRId = 0;
            NCRModel response = new NCRModel();
            response.ReturnCode = 0;
            response.ReturnMessage = MsgResponse.Message(0);
            try
            {
                DataSet ds = data.AddorEdit(model, out returnResult, out NCRId);
                response.ReturnCode = returnResult;
                int NCRNumber = NCRId;
                GeneratePDF(NCRNumber, model.TableName);
                NCREmailDetail(NCRNumber.ToString(),model.Email);
                response.ReturnMessage = MsgResponse.Message(returnResult);
                return response;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRAccess", "AddOrEdit");
                returnResult = -1;
                return null;
            }
        }
        public String NCRCountt()
        {
            try
            {
                string ds = data.NCRCountt();
                return ds;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRAccess", "NCRCountt");
                return null;
            }
        }
        public String NCRDataRefresh()
        {
            try
            {
                string ds = data.NCRDataRefresh();
                return ds;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRAccess", "NCRDataRefresh");
                return null;
            }
        }
        private void GeneratePDF(int NCRNumber, string TableName)
        {
            try
            {
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {
                    string path = ""; string Receipt1 = ""; string Receipt2 = "";


                    if (TableName == "NCR_GENERATED_UNIT1")
                    {
                        path = "~/Upload/UNIT1/";
                        Receipt1 = Receipt1_Unit1;
                        Receipt2 = Receipt2_Unit1;
                    }
                    else
                    {
                        path = "~/Upload/UNIT2/";
                        Receipt1 = Receipt1_Unit2;
                        Receipt2 = Receipt2_Unit2;
                    }
                    DataSet dt = data.DataForGeneratePDF(NCRNumber);
                    //string[] date = dt.Tables[0].Rows[0]["GenerateDate"].ToString().Split('/', ' ');
                    //string dd = date[1]; string mm = date[0]; string yy = date[1];
                    //string fDate = dd + "-" + mm + "-" + yy;
                    string fDate = dt.Tables[0].Rows[0]["GenerateDate"].ToString();
                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        if ((dt.Tables[0].Rows[0]["File"].ToString() != "" || dt.Tables[0].Rows[0]["file2"].ToString() != "") && dt.Tables[0].Rows[0]["File3"].ToString() == "" && dt.Tables[0].Rows[0]["File4"].ToString() == "")
                        {
                            string pdfName = dt.Tables[0].Rows[0]["Ncr_No"].ToString();
                            PdfPTable table = new PdfPTable(4);

                            // add a image
                            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/assets/images/karam.png"));
                            PdfPCell imageCell = new PdfPCell(jpg);
                            imageCell.Border = 0;
                            imageCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            imageCell.FixedHeight = 50f;
                            // add a image
                            imageCell.Colspan = 4;
                            imageCell.PaddingBottom = 10f;
                            table.AddCell(imageCell);

                            PdfPCell ppcell = new PdfPCell();
                            Paragraph pp1 = new Paragraph("QF/QA/09"); Paragraph pp2 = new Paragraph("Issue Dt. 12.06.2014"); Paragraph pp3 = new Paragraph("Rev Dt. 09.06.2018");
                            pp1.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);
                            pp2.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);
                            pp3.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);

                            pp1.SpacingBefore = 0; pp1.SpacingAfter = 0;
                            pp2.SpacingBefore = 0; pp2.SpacingAfter = 0;
                            pp3.SpacingBefore = 0; pp3.SpacingAfter = 0;
                            pp1.Alignment = Element.ALIGN_RIGHT; pp2.Alignment = Element.ALIGN_RIGHT; pp3.Alignment = Element.ALIGN_RIGHT;
                            ppcell.AddElement(pp1); ppcell.AddElement(pp2); ppcell.AddElement(pp3);
                            ppcell.Colspan = 4;

                            ppcell.Border = PdfPCell.NO_BORDER;
                            ppcell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                            ppcell.PaddingBottom = 8f;
                            table.AddCell(ppcell);

                            Document document = new Document(PageSize.A4, 0, 0, 10, 0);
                            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                            document.Open();
                            writer.CompressionLevel = PdfStream.BEST_COMPRESSION;

                            PdfPCell cellHeadr = new PdfPCell(new Phrase("NON-CONFORMANCE REPORT", FontFactory.GetFont("Arial", 20, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cellHeadr.Border = PdfPCell.NO_BORDER;
                            cellHeadr.Colspan = 4;
                            cellHeadr.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cellHeadr.PaddingBottom = 15f;
                            table.AddCell(cellHeadr);

                            PdfPCell cell = new PdfPCell(new Phrase("Vendor:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell.BorderWidthRight = 0f;
                            cell.BorderWidthBottom = 0f;
                            table.AddCell(cell);


                            PdfPCell cell1 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["VENDOR"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell1.UseVariableBorders = false;
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderColorRight = BaseColor.WHITE;
                            cell1.BorderWidthBottom = 0f;
                            table.AddCell(cell1);

                            PdfPCell cell2 = new PdfPCell(new Phrase("NCR No:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell2.BorderWidthRight = 0f;
                            cell2.BorderColorRight = BaseColor.WHITE;
                            cell2.BorderWidthBottom = 0f;
                            table.AddCell(cell2);

                            PdfPCell cell3 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Ncr_No"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell3.BorderWidthLeft = 0f;
                            cell3.BorderColorRight = BaseColor.BLACK;
                            cell3.BorderWidthBottom = 0f;
                            table.AddCell(cell3);

                            PdfPCell cell4 = new PdfPCell(new Phrase("Contact Person:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell4.BorderWidthRight = 0f;
                            cell4.BorderColorRight = BaseColor.WHITE;
                            cell4.BorderWidthBottom = 0f;
                            table.AddCell(cell4);

                            PdfPCell cell5 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Contact"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell5.UseVariableBorders = false;
                            cell5.BorderWidthLeft = 0f;
                            cell5.BorderColorRight = BaseColor.WHITE;
                            cell5.BorderWidthBottom = 0f;
                            table.AddCell(cell5);

                            PdfPCell cell6 = new PdfPCell(new Phrase("Date:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell6.BorderWidthRight = 0f;
                            cell6.BorderColorRight = BaseColor.WHITE;
                            cell6.BorderWidthBottom = 0f;
                            table.AddCell(cell6);

                            PdfPCell cell7 = new PdfPCell(new Phrase("" + fDate + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell7.BorderWidthLeft = 0f;
                            cell7.BorderColorRight = BaseColor.BLACK;
                            cell7.BorderWidthBottom = 0f;
                            table.AddCell(cell7);
                            ///////////////
                            PdfPCell cell8 = new PdfPCell(new Phrase("Phone:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell8.UseVariableBorders = false;
                            cell8.BorderWidthRight = 0f;
                            cell8.BorderWidthBottom = 0f;
                            table.AddCell(cell8);

                            PdfPCell cell9 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Phone"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell9.UseVariableBorders = false;
                            cell9.BorderWidthLeft = 0f;
                            cell9.BorderColorRight = BaseColor.WHITE;
                            cell9.BorderWidthBottom = 0f;
                            table.AddCell(cell9);

                            PdfPCell cell10 = new PdfPCell(new Phrase("M.R.N. No:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell10.BorderWidthRight = 0f;
                            cell10.BorderColorRight = BaseColor.WHITE;
                            cell10.BorderWidthBottom = 0f;
                            table.AddCell(cell10);

                            PdfPCell cell11 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["MRN"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell11.BorderWidthLeft = 0f;
                            cell11.BorderColorRight = BaseColor.BLACK;
                            cell11.BorderWidthBottom = 0f;
                            table.AddCell(cell11);

                            PdfPCell cell121 = new PdfPCell(new Phrase("Item Code:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell121.BorderWidthRight = 0f;
                            cell121.BorderColorRight = BaseColor.BLACK;
                            cell121.BorderWidthBottom = 0f;
                            table.AddCell(cell121);

                            PdfPCell cell131 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Item_Code"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell131.BorderWidthLeft = 0f;
                            cell131.BorderColorRight = BaseColor.BLACK;
                            cell131.BorderWidthBottom = 0f;
                            cell131.Colspan = 3;
                            table.AddCell(cell131);

                            PdfPCell cell12 = new PdfPCell(new Phrase("Address:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell12.BorderWidthRight = 0f;
                            cell12.BorderColorRight = BaseColor.BLACK;
                            cell12.BorderWidthBottom = 0f;
                            table.AddCell(cell12);

                            PdfPCell cell13 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Address"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell13.BorderWidthLeft = 0f;
                            cell13.BorderColorRight = BaseColor.BLACK;
                            cell13.BorderWidthBottom = 0f;
                            cell13.Colspan = 3;
                            table.AddCell(cell13);

                            PdfPCell cell14 = new PdfPCell(new Phrase("E-Mail:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell14.BorderWidthRight = 0f;
                            cell14.BorderColorRight = BaseColor.BLACK;
                            cell14.BorderWidthBottom = 0f;
                            table.AddCell(cell14);

                            PdfPCell cell15 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["email"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell15.BorderWidthLeft = 0f;
                            cell15.BorderColorRight = BaseColor.BLACK;
                            cell15.BorderWidthBottom = 0f;
                            cell15.Colspan = 3;
                            table.AddCell(cell15);

                            PdfPCell cell16 = new PdfPCell(new Phrase("Description:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell16.UseVariableBorders = false;
                            cell16.BorderWidthRight = 0f;
                            cell16.BorderWidthBottom = 0f;
                            table.AddCell(cell16);

                            PdfPCell cell17 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Description"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell17.UseVariableBorders = false;
                            cell17.BorderWidthLeft = 0f;
                            cell17.BorderColorRight = BaseColor.WHITE;
                            cell17.BorderWidthBottom = 0f;
                            table.AddCell(cell17);

                            PdfPCell cell18 = new PdfPCell(new Phrase("PO Number:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell18.BorderWidthRight = 0f;
                            cell18.BorderColorRight = BaseColor.WHITE;
                            cell18.BorderWidthBottom = 0f;
                            table.AddCell(cell18);

                            PdfPCell cell19 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["PO_Number"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell19.BorderWidthLeft = 0f;
                            cell19.BorderColorRight = BaseColor.BLACK;
                            cell19.BorderWidthBottom = 0f;
                            table.AddCell(cell19);

                            PdfPCell cell20 = new PdfPCell(new Phrase("Detail Of Non-Conformance:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell20.BorderWidthRight = 0f;
                            cell20.BorderColorRight = BaseColor.BLACK;
                            cell20.BorderWidthBottom = 1f;
                            cell20.Colspan = 2;
                            table.AddCell(cell20);

                            PdfPCell cell21 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Details"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell21.BorderWidthLeft = 0f;
                            cell21.BorderColorRight = BaseColor.BLACK;
                            cell21.BorderWidthBottom = 1f;
                            cell21.Colspan = 2;
                            table.AddCell(cell21);



                            PdfPCell cell22 = new PdfPCell(new Phrase("Total Received:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell22.UseVariableBorders = false;
                            cell22.BorderWidthRight = 0f;
                            cell22.BorderColorRight = BaseColor.BLACK;
                            cell22.BorderWidthBottom = 0f;
                            table.AddCell(cell22);

                            PdfPCell cell23 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Total_Received"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell23.UseVariableBorders = false;
                            cell23.BorderWidthLeft = 0f;
                            cell23.BorderColorRight = BaseColor.WHITE;
                            cell23.BorderWidthBottom = 0f;
                            table.AddCell(cell23);

                            PdfPCell cell24 = new PdfPCell(new Phrase("Total Rejected:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell24.BorderWidthRight = 0f;
                            cell24.BorderColorRight = BaseColor.WHITE;
                            cell24.BorderWidthBottom = 0f;
                            table.AddCell(cell24);

                            PdfPCell cell25 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Total_Reject"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell25.BorderWidthLeft = 0f;
                            cell25.BorderColorRight = BaseColor.BLACK;
                            cell25.BorderWidthBottom = 0f;
                            table.AddCell(cell25);

                            Paragraph p1 = new Paragraph("Result Of Investigation: e-mailed copy of N.C.R to Vendor for Corrective and Preventive actions/Possible credit.");
                            PdfPCell cell27 = new PdfPCell();
                            cell27.AddElement(p1);
                            cell27.Colspan = 4;
                            cell27.BorderWidthBottom = 0f;
                            table.AddCell(cell27);



                            Paragraph p2 = new Paragraph("To be filled by vendor how this happened and improvements in quality processes preventing it from happening in the future.Please fill out Corrective Actions portion og NCR, E-Mail to "+Receipt1+" & CC to "+Receipt2+" and send copy with new product to KARAM.Attn: Quality Assurance Deptt.");
                            Paragraph p5 = new Paragraph("Corrective Action Taken: ");
                            Paragraph p4 = new Paragraph(" ");
                            Paragraph p3 = new Paragraph("(Attach separate sheets is required)");

                            PdfPCell cell28 = new PdfPCell();
                            cell28.AddElement(p2); cell28.AddElement(p5); cell28.AddElement(p4); cell28.AddElement(p3);
                            cell28.Colspan = 4;
                            table.AddCell(cell28);

                            document.Add(table);

                            writer.PageEmpty = true;
                            document.NewPage();
                            PdfPTable tblimg = new PdfPTable(4);

                            if (dt.Tables[0].Rows[0]["File"].ToString() != "" && dt.Tables[0].Rows[0]["file2"].ToString() == "" && dt.Tables[0].Rows[0]["File3"].ToString() == "" && dt.Tables[0].Rows[0]["File4"].ToString() == "")
                            {
                                
                                iTextSharp.text.Image jpg1 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path + dt.Tables[0].Rows[0]["File"]), true);
                                PdfPCell imageCell1 = new PdfPCell(jpg1);
                                imageCell1.Border = PdfPCell.NO_BORDER;
                                imageCell1.PaddingTop = 20f;
                                imageCell1.PaddingBottom = 20f;
                                imageCell1.FixedHeight = 250f;
                                jpg1.ScaleToFit(450, 250f);
                                
                                imageCell1.Colspan = 4;
                                imageCell1.PaddingLeft = 0f;
                                tblimg.AddCell(imageCell1);
                            }
                            else if (dt.Tables[0].Rows[0]["File"].ToString() != "" && dt.Tables[0].Rows[0]["file2"].ToString() != "" && dt.Tables[0].Rows[0]["file3"].ToString() == "" && dt.Tables[0].Rows[0]["file4"].ToString() == "")
                            {
                                 
                                iTextSharp.text.Image jpg1 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path + dt.Tables[0].Rows[0]["File"]), true);
                                PdfPCell imageCell1 = new PdfPCell(jpg1);
                                imageCell1.Border = PdfPCell.NO_BORDER;
                                imageCell1.PaddingTop = 20f;
                                imageCell1.PaddingBottom = 20f;
                                imageCell1.FixedHeight = 250f;
                                jpg1.ScaleToFit(450, 250f);
                                // add a image
                                //imageCell1.Border = Rectangle.NO_BORDER;
                                imageCell1.Colspan = 4;
                                imageCell1.PaddingLeft = 0f;
                                tblimg.AddCell(imageCell1);

                                iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path + dt.Tables[0].Rows[0]["file2"]), true);
                                PdfPCell imageCell2 = new PdfPCell(jpg2);
                                imageCell2.Border = PdfPCell.NO_BORDER;
                                imageCell2.PaddingTop = 20f;
                                imageCell2.FixedHeight = 250f;
                                jpg2.ScaleToFit(450, 250f);
                                // add a image
                                imageCell2.Colspan = 4;
                                imageCell2.PaddingLeft = 0f;
                                tblimg.AddCell(imageCell2);
                            }

                            document.Add(tblimg);
                            document.Close();
                            byte[] bytes = memoryStream.ToArray();

                            //  System.IO.File.WriteAllBytes(Server.MapPath("~/PDF/UNIT1/") + id + ".pdf", bytes); // save
                            string filePath = path;
                            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(filePath)))
                                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(filePath));
                            System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath(filePath) + NCRNumber + ".pdf", bytes); // save
                            memoryStream.Close();
                            //Response.Clear();
                        }
                        else
                        {
                            
                            string pdfName = dt.Tables[0].Rows[0]["Ncr_No"].ToString();
                            PdfPTable table = new PdfPTable(4);
                            // add a image
                            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/assets/images/karam.png"));
                            PdfPCell imageCell = new PdfPCell(jpg);
                            imageCell.Border = 0;
                            imageCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            imageCell.FixedHeight = 50f;
                            // add a image
                            imageCell.Colspan = 4;
                            imageCell.PaddingBottom = 10f;
                            table.AddCell(imageCell);

                            PdfPCell ppcell = new PdfPCell();
                            Paragraph pp1 = new Paragraph("QF/QA/09"); Paragraph pp2 = new Paragraph("Issue Dt. 12.06.2014"); Paragraph pp3 = new Paragraph("Rev Dt. 09.06.2018");
                            pp1.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);
                            pp2.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);
                            pp3.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);

                            pp1.SpacingBefore = 0; pp1.SpacingAfter = 0;
                            pp2.SpacingBefore = 0; pp2.SpacingAfter = 0;
                            pp3.SpacingBefore = 0; pp3.SpacingAfter = 0;
                            pp1.Alignment = Element.ALIGN_RIGHT; pp2.Alignment = Element.ALIGN_RIGHT; pp3.Alignment = Element.ALIGN_RIGHT;
                            ppcell.AddElement(pp1); ppcell.AddElement(pp2); ppcell.AddElement(pp3);
                            ppcell.Colspan = 4;

                            ppcell.Border = PdfPCell.NO_BORDER;
                            ppcell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                            ppcell.PaddingBottom = 8f;
                            table.AddCell(ppcell);

                            Document document = new Document(PageSize.A4, 0, 0, 50, 0);
                            //Document document = new Document();  
                            document.PageCount = 2;
                            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                            //writer.PageEvent = new PDFFooter();
                            document.Open();
                            writer.CompressionLevel = PdfStream.BEST_COMPRESSION;


                            PdfPCell cellHeadr = new PdfPCell(new Phrase("NON-CONFORMANCE REPORT", FontFactory.GetFont("Arial", 20, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cellHeadr.Border = PdfPCell.NO_BORDER;
                            cellHeadr.Colspan = 4;
                            cellHeadr.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cellHeadr.PaddingBottom = 15f;
                            table.AddCell(cellHeadr);

                            PdfPCell cell = new PdfPCell(new Phrase("Vendor:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell.BorderWidthRight = 0f;
                            cell.BorderWidthBottom = 0f;
                            table.AddCell(cell);

                            PdfPCell cell1 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["VENDOR"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell1.UseVariableBorders = false;
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderColorRight = BaseColor.WHITE;
                            cell1.BorderWidthBottom = 0f;
                            table.AddCell(cell1);

                            PdfPCell cell2 = new PdfPCell(new Phrase("NCR No:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell2.BorderWidthRight = 0f;
                            cell2.BorderColorRight = BaseColor.WHITE;
                            cell2.BorderWidthBottom = 0f;
                            table.AddCell(cell2);

                            PdfPCell cell3 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Ncr_No"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell3.BorderWidthLeft = 0f;
                            cell3.BorderColorRight = BaseColor.BLACK;
                            cell3.BorderWidthBottom = 0f;
                            table.AddCell(cell3);

                            ////****////
                            PdfPCell cell4 = new PdfPCell(new Phrase("Contact Person:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell4.BorderWidthRight = 0f;
                            cell4.BorderColorRight = BaseColor.WHITE;
                            cell4.BorderWidthBottom = 0f;
                            table.AddCell(cell4);

                            PdfPCell cell5 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Contact"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell5.UseVariableBorders = false;
                            cell5.BorderWidthLeft = 0f;
                            cell5.BorderColorRight = BaseColor.WHITE;
                            cell5.BorderWidthBottom = 0f;
                            table.AddCell(cell5);

                            PdfPCell cell6 = new PdfPCell(new Phrase("Date:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell6.BorderWidthRight = 0f;
                            cell6.BorderColorRight = BaseColor.WHITE;
                            cell6.BorderWidthBottom = 0f;
                            table.AddCell(cell6);

                            PdfPCell cell7 = new PdfPCell(new Phrase("" + fDate + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell7.BorderWidthLeft = 0f;
                            cell7.BorderColorRight = BaseColor.BLACK;
                            cell7.BorderWidthBottom = 0f;
                            table.AddCell(cell7);
                            ///////////////
                            PdfPCell cell8 = new PdfPCell(new Phrase("Phone:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell8.UseVariableBorders = false;
                            cell8.BorderWidthRight = 0f;
                            cell8.BorderWidthBottom = 0f;
                            table.AddCell(cell8);

                            PdfPCell cell9 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Phone"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell9.UseVariableBorders = false;
                            cell9.BorderWidthLeft = 0f;
                            cell9.BorderColorRight = BaseColor.WHITE;
                            cell9.BorderWidthBottom = 0f;
                            table.AddCell(cell9);

                            PdfPCell cell10 = new PdfPCell(new Phrase("M.R.N. No:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell10.BorderWidthRight = 0f;
                            cell10.BorderColorRight = BaseColor.WHITE;
                            cell10.BorderWidthBottom = 0f;
                            table.AddCell(cell10);

                            PdfPCell cell11 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["MRN"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell11.BorderWidthLeft = 0f;
                            cell11.BorderColorRight = BaseColor.BLACK;
                            cell11.BorderWidthBottom = 0f;
                            table.AddCell(cell11);

                            PdfPCell cell121 = new PdfPCell(new Phrase("Item Code:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell121.BorderWidthRight = 0f;
                            cell121.BorderColorRight = BaseColor.BLACK;
                            cell121.BorderWidthBottom = 0f;
                            table.AddCell(cell121);

                            PdfPCell cell131 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Item_Code"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell131.BorderWidthLeft = 0f;
                            cell131.BorderColorRight = BaseColor.BLACK;
                            cell131.BorderWidthBottom = 0f;
                            cell131.Colspan = 3;
                            table.AddCell(cell131);

                            PdfPCell cell12 = new PdfPCell(new Phrase("Address:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell12.BorderWidthRight = 0f;
                            cell12.BorderColorRight = BaseColor.BLACK;
                            cell12.BorderWidthBottom = 0f;
                            table.AddCell(cell12);

                            PdfPCell cell13 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Address"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell13.BorderWidthLeft = 0f;
                            cell13.BorderColorRight = BaseColor.BLACK;
                            cell13.BorderWidthBottom = 0f;
                            cell13.Colspan = 3;
                            table.AddCell(cell13);

                            PdfPCell cell14 = new PdfPCell(new Phrase("E-Mail:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell14.BorderWidthRight = 0f;
                            cell14.BorderColorRight = BaseColor.BLACK;
                            cell14.BorderWidthBottom = 0f;
                            table.AddCell(cell14);

                            PdfPCell cell15 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["email"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell15.BorderWidthLeft = 0f;
                            cell15.BorderColorRight = BaseColor.BLACK;
                            cell15.BorderWidthBottom = 0f;
                            cell15.Colspan = 3;
                            table.AddCell(cell15);
                            //////////
                            PdfPCell cell16 = new PdfPCell(new Phrase("Description:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell16.UseVariableBorders = false;
                            cell16.BorderWidthRight = 0f;
                            cell16.BorderWidthBottom = 0f;
                            table.AddCell(cell16);

                            PdfPCell cell17 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Description"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell17.UseVariableBorders = false;
                            cell17.BorderWidthLeft = 0f;
                            cell17.BorderColorRight = BaseColor.WHITE;
                            cell17.BorderWidthBottom = 0f;
                            table.AddCell(cell17);

                            PdfPCell cell18 = new PdfPCell(new Phrase("PO Number:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell18.BorderWidthRight = 0f;
                            cell18.BorderColorRight = BaseColor.WHITE;
                            cell18.BorderWidthBottom = 0f;
                            table.AddCell(cell18);

                            PdfPCell cell19 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["PO_Number"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell19.BorderWidthLeft = 0f;
                            cell19.BorderColorRight = BaseColor.BLACK;
                            cell19.BorderWidthBottom = 0f;
                            table.AddCell(cell19);

                            PdfPCell cell20 = new PdfPCell(new Phrase("Detail Of Non-Conformance:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell20.BorderWidthRight = 0f;
                            cell20.BorderColorRight = BaseColor.BLACK;
                            cell20.BorderWidthBottom = 1f;
                            cell20.Colspan = 2;
                            table.AddCell(cell20);

                            PdfPCell cell21 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Details"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell21.BorderWidthLeft = 0f;
                            cell21.BorderColorRight = BaseColor.BLACK;
                            cell21.BorderWidthBottom = 1f;
                            cell21.Colspan = 2;
                            table.AddCell(cell21);

                            //////////////////*******************//////////
                            PdfPCell cell22 = new PdfPCell(new Phrase("Total Received:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell22.UseVariableBorders = false;
                            cell22.BorderWidthRight = 0f;
                            cell22.BorderColorRight = BaseColor.BLACK;
                            cell22.BorderWidthBottom = 0f;
                            table.AddCell(cell22);

                            PdfPCell cell23 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Total_Received"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell23.UseVariableBorders = false;
                            cell23.BorderWidthLeft = 0f;
                            cell23.BorderColorRight = BaseColor.WHITE;
                            cell23.BorderWidthBottom = 0f;
                            table.AddCell(cell23);

                            PdfPCell cell24 = new PdfPCell(new Phrase("Total Rejected:", FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                            cell24.BorderWidthRight = 0f;
                            cell24.BorderColorRight = BaseColor.WHITE;
                            cell24.BorderWidthBottom = 0f;
                            table.AddCell(cell24);

                            PdfPCell cell25 = new PdfPCell(new Phrase("" + dt.Tables[0].Rows[0]["Total_Reject"] + "", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            cell25.BorderWidthLeft = 0f;
                            cell25.BorderColorRight = BaseColor.BLACK;
                            cell25.BorderWidthBottom = 0f;
                            table.AddCell(cell25);

                            Paragraph p1 = new Paragraph("Result Of Investigation: e-mailed copy of N.C.R to Vendor for Corrective and Preventive actions/Possible credit.");
                            PdfPCell cell27 = new PdfPCell();
                            cell27.AddElement(p1);
                            cell27.Colspan = 4;
                            cell27.BorderWidthBottom = 0f;
                            table.AddCell(cell27);


                            Paragraph p2 = new Paragraph("To be filled by vendor how this happened and improvements in quality processes preventing it from happening in the future.Please fill out Corrective Actions portion og NCR, E-Mail to " + Receipt1 + " & Cc to " + Receipt2 + " and send copy with new product to KARAM.Attn: Quality Assurance Deptt.");
                            Paragraph p5 = new Paragraph("Corrective Action Taken: ");
                            Paragraph p4 = new Paragraph(" ");
                            Paragraph p3 = new Paragraph("(Attach separate sheets is required)");

                            PdfPCell cell28 = new PdfPCell();
                            cell28.AddElement(p2); cell28.AddElement(p5); cell28.AddElement(p4); cell28.AddElement(p3);
                            cell28.Colspan = 4;
                            table.AddCell(cell28);
                            ////////////////////////////////////////

                            /////////*********image 1******/////
                            Paragraph p6 = new Paragraph("Non-Conformance Images are attached on Next Page");
                            p6.Font.Color = BaseColor.RED;
                            PdfPCell ncell = new PdfPCell();
                            ncell.AddElement(p6);
                            ncell.Colspan = 4;
                            table.AddCell(ncell);

                            document.Add(table);

                            if (dt.Tables[0].Rows[0]["file"].ToString() != "" && dt.Tables[0].Rows[0]["file2"].ToString() != "" && dt.Tables[0].Rows[0]["file3"].ToString() != "" && dt.Tables[0].Rows[0]["file4"].ToString() == "")
                            {
                                
                                writer.PageEmpty = true;
                                document.NewPage();
                                PdfPTable tableTEST = new PdfPTable(4);

                                /////////////**************///////////////
                                iTextSharp.text.Image jpg2i = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/assets/images/karam.png"));
                                PdfPCell imageCell3i = new PdfPCell(jpg2i);
                                imageCell3i.Border = 0;
                                imageCell3i.HorizontalAlignment = Element.ALIGN_LEFT;
                                imageCell3i.FixedHeight = 50f;
                                // add a image
                                imageCell3i.Colspan = 4;
                                imageCell3i.PaddingBottom = 10f;
                                tableTEST.AddCell(imageCell3i);

                                PdfPCell ppcell3 = new PdfPCell();
                                Paragraph pp13 = new Paragraph("QF/QA/09"); Paragraph pp23 = new Paragraph("Issue Dt. 12.06.2014"); Paragraph pp33 = new Paragraph("Rev Dt. 09.06.2018");
                                pp13.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);
                                pp23.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);
                                pp33.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);

                                pp13.SpacingBefore = 0; pp13.SpacingAfter = 0;
                                pp23.SpacingBefore = 0; pp23.SpacingAfter = 0;
                                pp33.SpacingBefore = 0; pp33.SpacingAfter = 0;
                                pp13.Alignment = Element.ALIGN_RIGHT; pp23.Alignment = Element.ALIGN_RIGHT; pp33.Alignment = Element.ALIGN_RIGHT;
                                ppcell3.AddElement(pp1); ppcell3.AddElement(pp2); ppcell3.AddElement(pp33);
                                ppcell3.Colspan = 4;

                                ppcell3.Border = PdfPCell.NO_BORDER;
                                ppcell3.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                                ppcell3.PaddingBottom = 8f;
                                tableTEST.AddCell(ppcell3);
                                /////////////////////////////////////////

                                iTextSharp.text.Image jpg1 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path + dt.Tables[0].Rows[0]["File"]), true);
                                PdfPCell imageCell1 = new PdfPCell(jpg1);
                                imageCell1.Border = PdfPCell.NO_BORDER;
                                imageCell1.PaddingTop = 20f;
                                imageCell1.FixedHeight = 250f;
                                jpg1.ScaleToFit(450, 250f);
                                // add a image
                                //imageCell1.Border = Rectangle.NO_BORDER;
                                imageCell1.Colspan = 4;
                                imageCell1.PaddingLeft = 0f;
                                //table.AddCell(imageCell1);
                                tableTEST.AddCell(imageCell1);

                                iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path + dt.Tables[0].Rows[0]["file2"]), true);
                                PdfPCell imageCell2 = new PdfPCell(jpg2);
                                imageCell2.Border = PdfPCell.NO_BORDER;
                                imageCell2.PaddingTop = 20f;
                                imageCell2.FixedHeight = 250f;
                                jpg2.ScaleToFit(450, 250f);
                                // add a image
                                imageCell2.Colspan = 4;
                                imageCell2.PaddingLeft = 0f;
                                //table.AddCell(imageCell2);
                                tableTEST.AddCell(imageCell2);


                                iTextSharp.text.Image jpg3 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path + dt.Tables[0].Rows[0]["file3"]));
                                PdfPCell imageCell3 = new PdfPCell(jpg3);
                                imageCell3.Border = PdfPCell.NO_BORDER;
                                imageCell3.PaddingTop = 20f;
                                imageCell3.FixedHeight = 250f;
                                jpg3.ScaleToFit(450, 250f);
                                // add a image
                                imageCell3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                imageCell3.Colspan = 4;
                                imageCell3.PaddingLeft = 0f;
                                //table.AddCell(imageCell3);
                                tableTEST.AddCell(imageCell3);
                                document.Add(tableTEST);
                            }
                            else
                            {
                                writer.PageEmpty = true;
                                document.NewPage();
                                PdfPTable tableTEST = new PdfPTable(4);

                                /////////////**************///////////////
                                iTextSharp.text.Image jpg2i = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/assets/images/karam.png"));
                                PdfPCell imageCell3i = new PdfPCell(jpg2i);
                                imageCell3i.Border = 0;
                                imageCell3i.HorizontalAlignment = Element.ALIGN_LEFT;
                                imageCell3i.FixedHeight = 50f;
                                // add a image
                                imageCell3i.Colspan = 4;
                                imageCell3i.PaddingBottom = 10f;
                                tableTEST.AddCell(imageCell3i);

                                PdfPCell ppcell3 = new PdfPCell();
                                Paragraph pp13 = new Paragraph("QF/QA/09"); Paragraph pp23 = new Paragraph("Issue Dt. 12.06.2014"); Paragraph pp33 = new Paragraph("Rev Dt. 09.06.2018");
                                pp13.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);
                                pp23.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);
                                pp33.Font = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);

                                pp13.SpacingBefore = 0; pp13.SpacingAfter = 0;
                                pp23.SpacingBefore = 0; pp23.SpacingAfter = 0;
                                pp33.SpacingBefore = 0; pp33.SpacingAfter = 0;
                                pp13.Alignment = Element.ALIGN_RIGHT; pp23.Alignment = Element.ALIGN_RIGHT; pp33.Alignment = Element.ALIGN_RIGHT;
                                ppcell3.AddElement(pp1); ppcell3.AddElement(pp2); ppcell3.AddElement(pp33);
                                ppcell3.Colspan = 4;

                                ppcell3.Border = PdfPCell.NO_BORDER;
                                ppcell3.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                                ppcell3.PaddingBottom = 8f;
                                tableTEST.AddCell(ppcell3);
                                /////////////////////////////////////////

                                iTextSharp.text.Image jpg1 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path + dt.Tables[0].Rows[0]["File"]));
                                PdfPCell imageCell1 = new PdfPCell(jpg1);
                                jpg1.ScaleToFit(450, 250f);
                                imageCell1.Border = PdfPCell.NO_BORDER;
                                imageCell1.PaddingTop = 20f;
                                imageCell1.FixedHeight = 250f;
                                // add a image
                                //imageCell1.Border = Rectangle.NO_BORDER;
                                imageCell1.Colspan = 4;
                                imageCell1.PaddingLeft = 0f;
                                tableTEST.AddCell(imageCell1);

                                iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path + dt.Tables[0].Rows[0]["file2"]));
                                PdfPCell imageCell2 = new PdfPCell(jpg2);
                                jpg2.ScaleToFit(450, 250f);
                                imageCell2.Border = PdfPCell.NO_BORDER;
                                imageCell2.PaddingTop = 20f;
                                imageCell2.FixedHeight = 250f;
                                // add a image
                                imageCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                imageCell2.Colspan = 4;
                                imageCell2.PaddingLeft = 0f;
                                tableTEST.AddCell(imageCell2);

                                iTextSharp.text.Image jpg3 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path + dt.Tables[0].Rows[0]["file3"]));
                                PdfPCell imageCell3 = new PdfPCell(jpg3);
                                jpg3.ScaleToFit(450, 250f);
                                imageCell3.Border = PdfPCell.NO_BORDER;
                                imageCell3.PaddingTop = 20f;
                                imageCell3.FixedHeight = 250f;
                                // add a image
                                imageCell3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                imageCell3.Colspan = 4;
                                imageCell3.PaddingLeft = 0f;
                                tableTEST.AddCell(imageCell3);

                                iTextSharp.text.Image jpg4 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path + dt.Tables[0].Rows[0]["file4"]));
                                PdfPCell imageCell4 = new PdfPCell(jpg4);
                                jpg4.ScaleToFit(450, 250f); ;
                                imageCell4.Border = PdfPCell.NO_BORDER;
                                imageCell4.PaddingTop = 20f;
                                imageCell4.FixedHeight = 250f;
                                // add a image
                                imageCell4.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                imageCell4.Colspan = 4;
                                imageCell4.PaddingLeft = 0f;
                                tableTEST.AddCell(imageCell4);
                                document.Add(tableTEST);
                            }
                            document.Close();
                            byte[] bytes = memoryStream.ToArray();
                            if (TableName == "NCR_GENERATED_UNIT1")
                            {
                                System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Upload/UNIT1/") + NCRNumber + ".pdf", bytes); // save
                            }
                            else
                            {
                                System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Upload/UNIT2/") + NCRNumber + ".pdf", bytes); // save
                            }
                            memoryStream.Close();
                            //Response.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRAccess", "GeneratePDF");
            }
            finally
            {
            }
        }
        public NCRModel NCREmailDetail(string NCRNumber,string Email)
        {
            Int32 returnResult = 0;
            int ds1 = 0;
            NCRModel response = new NCRModel();
            try
            {
                DataSet ds = data.NCREmailDetail(NCRNumber);
                string table = HttpContext.Current.Session["UNIT"].ToString();
                if (table == "NCR_GENERATED_UNIT1")
                {
                    Attachment attachment = new Attachment(HttpContext.Current.Server.MapPath("~/Upload/UNIT1/" + NCRNumber + ".pdf"), System.Net.Mime.MediaTypeNames.Application.Pdf);
                    string t1 = ConfigurationManager.AppSettings["t1"].ToString();
                    
                    string vendor = NCRData(Convert.ToInt32(NCRNumber), attachment, Email);
                    ds1 = data.UpdateNCREmailStatus(NCRNumber, out returnResult);
                }
                else
                {
                    Attachment attachment = new Attachment(HttpContext.Current.Server.MapPath("~/Upload/UNIT2/" + NCRNumber + ".pdf"), System.Net.Mime.MediaTypeNames.Application.Pdf);
                    string t1 = ConfigurationManager.AppSettings["t1"].ToString();
                    string t2 = ConfigurationManager.AppSettings["t2"].ToString();
                    string vendor = NCRData(Convert.ToInt32(NCRNumber), attachment, Email);
                    ds1 = data.UpdateNCREmailStatus(NCRNumber, out returnResult);
                }
                if (ds1 == 1)
                {
                    response.ReturnCode = 12;
                    response.ReturnMessage = "Mail Send Successfully";
                }
                else
                {
                    response.ReturnCode = 99;
                    response.ReturnMessage = "Email Not send due to Server";
                }
                return response;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRAccess", "NCREmailDetail");
                return null;
            }
        }
        public string NCRData(int ID, Attachment attachment, string to)
        {
            string literal = null;
            DataTable dt = new DataTable();
            try
            {
                string table = HttpContext.Current.Session["UNIT"].ToString();
                string Recipt1 = ""; string Recipt2 = "";
               
                switch (table)
                {
                    case "NCR_GENERATED_UNIT1":
                        //Recipt1 = "pramesh.kumar@karam.in,abhishek.asthana1@karam.in";
                        //Recipt2 = "anant@karam.in";
                        Recipt1 = Receipt1_Unit1;
                        Recipt2 = Receipt2_Unit1;
                        break;
                    case "NCR_GENERATED":
                        //Recipt1 = "anshuman.singh@karam.in";
                        //Recipt2 = "saurabh@karam.in,mayanksrivastava@karam.in,kriti.rastogi@karam.in,gaurav.goyal@karam.in";
                        Recipt1 = Receipt1_Unit2;
                        Recipt2 = Receipt2_Unit2;

                        break;
                }
                DataSet ds = data.NCREmailDetail(ID.ToString());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("Dear " + ds.Tables[0].Rows[0]["Contact"] + ", <br/><br/>This is inform to you that we have received below material and found discrepancy mentioned below<br/><br/>");
                    stringBuilder.Append("<table border='1' id='datatable' style='5px solid #0e0e0e'>");
                    stringBuilder.Append("<tr style='color:black;font:bold 15px arial'><td>Product Code</td><td>MRN NO</td><td>PO No.</td><td>Non-Conformance</td><td>Total Quantity (Nos.)</td><td>Rejected Quantity (Nos.)</td><td>NCR NO.</td></td>");
                    stringBuilder.Append("</tr>");
                    stringBuilder.Append("<tr><td><span style='color:black;font:bold 12px arial'>" + ds.Tables[0].Rows[0]["Item_Code"] + "</span> - " + ds.Tables[0].Rows[0]["Description"] + "" + "</td>");
                    stringBuilder.Append("<td>" + ds.Tables[0].Rows[0]["MRN"] + "</td>");
                    stringBuilder.Append("<td>" + ds.Tables[0].Rows[0]["PO_Number"] + "</td>");
                    stringBuilder.Append("<td>" + ds.Tables[0].Rows[0]["Details"] + "</td>");
                    stringBuilder.Append("<td>" + ds.Tables[0].Rows[0]["Total_Received"] + "</td>");
                    stringBuilder.Append("<td>" + ds.Tables[0].Rows[0]["Total_Reject"] + "</td>");
                    stringBuilder.Append("<td>" + ds.Tables[0].Rows[0]["Ncr_No"] + "</td></tr>");
                    stringBuilder.Append("</table>");
                    stringBuilder.Append("<br/><br/>");
                    stringBuilder.Append("Kindly find attached here with the Non Conformance Report (NCR) for your easy reference and expecting Corrective and Preventive Action from your side to avoid repetition of same discrepancies in future supplies<br/><br/>");
                    stringBuilder.Append("We are also sending you the debit note for the same");
                    stringBuilder.Append("<br/><br/>");
                    stringBuilder.Append("“This is a system generated mail, for any query please mail to <span style='text-decoration-line:underline'>" + Recipt1 + "</span> and CC to " + Recipt2 + " .”");
                    literal = stringBuilder.ToString();
                    string CC = Recipt1 + "," + Recipt2;
                    UtilityAccess.SendEmail(to, CC, "NCR", literal, true, attachment);
                    //if(SuccesValue==1)
                    //{

                    //}
                }
                return literal;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRAccess", "NCREmailDetail");
                return null;
            }
        }
        public List<ReportModel> ReportDetail(ReportModel model)
        {
            ReportModel response = new ReportModel();
            List<ReportModel> responseList = new List<ReportModel>();
            try
            {
                DataSet ds = data.ReportDetail(model);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            response = new ReportModel();
                            response.NCRDate = Convert.ToString(row["Generated_date"] ?? 0);
                            response.NCRNumber = Convert.ToString(row["Ncr_No"] ?? 0);
                            response.VendorCode = Convert.ToString(row["VENDOR"] ?? string.Empty);
                            response.ItemCode = Convert.ToString(row["Item_Code"] ?? string.Empty);
                            response.MRNno = Convert.ToString(row["MRN"].ToString());
                            response.PO = Convert.ToString(row["PO_Number"] ?? string.Empty);
                            response.TotalRecieved = Convert.ToString(row["Total_Received"] ?? string.Empty);
                            response.TotalRejected = Convert.ToString(row["Total_Reject"] ?? string.Empty);
                            response.DetailofNonConformance = Convert.ToString(row["Details"] ?? string.Empty);
                            response.Email = Convert.ToString(row["Email"] ?? string.Empty);
                            response.ItemDescription = Convert.ToString(row["Description"] ?? string.Empty);
                            response.SendMailDate = Convert.ToString(row["LastSendMailDate"] ?? string.Empty);
                            responseList.Add(response);
                        }
                    }
                }
                response.reportModelsList = responseList;
                return response.reportModelsList;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRAccess", "ReportDetail");
                return null;
            }
        }
    }
}
