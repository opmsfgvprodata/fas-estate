using iTextSharp.text.pdf;
using iTextSharp.text;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Data.SqlClient;
using Dapper;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class PCBReportController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private MVC_SYSTEM_Models dbp = new MVC_SYSTEM_Models();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        ChangeTimeZone timezone = new ChangeTimeZone();
        GetConfig GetConfig = new GetConfig();
        errorlog geterror = new errorlog();
        GetTriager GetTriager = new GetTriager();
        private GetIdentity getidentity = new GetIdentity();

        List<tbl_Pkjmast> tbl_Pkjmast = new List<tbl_Pkjmast>();
        List<tbl_GajiBulanan> tbl_GajiBulanan = new List<tbl_GajiBulanan>();
        List<tbl_TaxWorkerInfo> tbl_TaxWorkerInfo = new List<tbl_TaxWorkerInfo>();
        List<tbl_ByrCarumanTambahan> tbl_ByrCarumanTambahan = new List<tbl_ByrCarumanTambahan>();
        List<MasterModels.tblOptionConfigsWeb> tblOptionConfigsWebs = new List<MasterModels.tblOptionConfigsWeb>();
        // GET: PCBReport
        public ActionResult Index()
        {
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var yearlist = new List<SelectListItem>();
            for (var i = rangeyear; i <= year; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }
            var monthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            //code asal
            //SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
            //edit by fitri 24-11-2020
            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;
            ViewBag.MonthList = monthList;
            ViewBag.YearList = yearlist;
            ViewBag.StatusList = StatusList;
            return View();
        }

        public FileStreamResult PCBPdf(int? RadioGroup, int? MonthList, int? YearList, string SelectionList, string StatusList)
        {

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string constr = Connection.GetConnectionString(WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("WilayahID", WilayahID);
                parameters.Add("LadangID", LadangID);
                parameters.Add("DivisionID", DivisionID);
                parameters.Add("Month", MonthList);
                parameters.Add("Year", YearList);
                con.Open();
                SqlMapper.Settings.CommandTimeout = 300;
                var reader = SqlMapper.QueryMultiple(con, "sp_RptPCB", parameters);

                tbl_Pkjmast = reader.Read<tbl_Pkjmast>().ToList();
                tbl_GajiBulanan = reader.Read<tbl_GajiBulanan>().ToList();
                tbl_TaxWorkerInfo = reader.Read<tbl_TaxWorkerInfo>().ToList();
                tbl_ByrCarumanTambahan = reader.Read<tbl_ByrCarumanTambahan>().ToList();
                tblOptionConfigsWebs = reader.Read<MasterModels.tblOptionConfigsWeb>().ToList();

                con.Close();
            }
            catch (Exception)
            {
                throw;
            }


            if (RadioGroup == 0)
            {
                //individu
                if (StatusList == "0")
                {
                    // aktif & xaktif
                    if (SelectionList == "0")
                    {
                        tbl_Pkjmast = tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama).ToList();
                    }
                    else
                    {
                        tbl_Pkjmast = tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama).ToList();
                    }

                }
                else
                {
                    // aktif/xaktif
                    if (SelectionList == "0")
                    {
                        tbl_Pkjmast = tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama).ToList();
                    }
                    else
                    {
                        tbl_Pkjmast = tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama).ToList();
                    }
                }
            }
            else
            {
                //group
                if (SelectionList == "0")
                {
                    tbl_Pkjmast = tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama).ToList();
                }
                else
                {
                    var kumpID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_DivisionID == DivisionID).Select(s => s.fld_KumpulanID).FirstOrDefault();
                    //original code
                    //var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1);
                    //modified by Faeza on 02.06.2020
                    tbl_Pkjmast = tbl_Pkjmast.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").ToList();
                }
            }

            MemoryStream ms = new MemoryStream();
            MemoryStream output = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4, 30, 30, 25, 30);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
            pdfWriter.PageEvent = new PDFFooter();
            Chunk chunk = new Chunk();
            Paragraph para = new Paragraph();
            pdfDoc.Open();

            var getWorkerPCBAvailable = tbl_Pkjmast.Join(tbl_GajiBulanan, e => e.fld_Nopkj, d => d.fld_Nopkj,
                (tbl1, tbl2) => new { tbl_Pkjmast = tbl1, tbl_GajiBulanan = tbl2 }).Join(tbl_ByrCarumanTambahan, ee => ee.tbl_GajiBulanan.fld_ID, dd => dd.fld_GajiID,
                (tbl1, tbl2) => new { tbl_GajiBulanan = tbl1, tbl_ByrCarumanTambahan = tbl2 }).ToList();

            if (getWorkerPCBAvailable.Count() > 0)
            {
                foreach (var item in getWorkerPCBAvailable)
                {
                    pdfDoc.NewPage();

                    Header(pdfDoc);

                    WorkerInfo(pdfDoc, item.tbl_GajiBulanan.tbl_Pkjmast.fld_Nopkj);

                    WorkerChildInfo(pdfDoc, item.tbl_GajiBulanan.tbl_Pkjmast.fld_Nopkj);

                    pdfDoc.NewPage();

                    WorkerRemuneration1(pdfDoc, item.tbl_GajiBulanan.tbl_Pkjmast.fld_Nopkj);

                    WorkerRemuneration2(pdfDoc, item.tbl_GajiBulanan.tbl_Pkjmast.fld_Nopkj);

                    WorkerRemuneration3(pdfDoc, item.tbl_GajiBulanan.tbl_Pkjmast.fld_Nopkj);

                    pdfDoc.NewPage();

                    WorkerDeduction(pdfDoc, item.tbl_GajiBulanan.tbl_Pkjmast.fld_Nopkj);

                    pdfDoc.NewPage();

                    WorkerRebate(pdfDoc, item.tbl_GajiBulanan.tbl_Pkjmast.fld_Nopkj);

                    WorkerPCBCalculation(pdfDoc, item.tbl_GajiBulanan.tbl_Pkjmast.fld_Nopkj);

                    WorkerPCBResultCalculation(pdfDoc, item.tbl_GajiBulanan.tbl_Pkjmast.fld_Nopkj);
                }
            }
            else
            {
                NotFound(pdfDoc);
            }
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

        public Document NotFound(Document pdfDoc)
        {
            PdfPTable table = new PdfPTable(1);
            float[] widths = new float[] { 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;

            Chunk chunk = new Chunk("Data Not Found", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.Border = 0;
            table.AddCell(cell);
            pdfDoc.Add(table);
            return pdfDoc;
        }
        public Document Header(Document pdfDoc)
        {
            PdfPTable table = new PdfPTable(2);
            float[] widths = new float[] { 0.5f, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            Image image = Image.GetInstance(Server.MapPath("~/Asset/Images/LHDN_Logo.png"));
            PdfPCell cell = new PdfPCell(image);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            cell.Rowspan = 2;
            image.ScaleAbsolute(80, 50);
            table.AddCell(cell);

            Chunk chunk = new Chunk("KALKULATOR PCB", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            cell.Border = 0;
            table.AddCell(cell);

            chunk = new Chunk("LEMBAGA HASIL DALAM NEGERI MALAYSIA", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.Border = 0;
            table.AddCell(cell);
            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerInfo(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(6);
            float[] widths = new float[] { 1, 0.1f, 0.8f, 0.5f, 0.1f, 0.8f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            var pkjInfo = tbl_Pkjmast.Where(x => x.fld_Nopkj == NoPkj).FirstOrDefault();
            var pkjTaxInfo = tbl_TaxWorkerInfo.Where(x => x.fld_Nopkj == NoPkj).FirstOrDefault();
            var pkjGajiInfo = tbl_GajiBulanan.Where(x => x.fld_Nopkj == NoPkj).FirstOrDefault();
            var pkjPCBInfo = tbl_ByrCarumanTambahan.Where(x => x.fld_GajiID == pkjGajiInfo.fld_ID).FirstOrDefault();
            var monthName = ((Constans.Month)pkjGajiInfo.fld_Month).ToString().ToUpper();
            var taxResidency = tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "taxResidency" && x.fldOptConfValue == pkjTaxInfo.fld_TaxResidency).Select(s => s.fldOptConfDesc).FirstOrDefault();
            var taxMaritalStatus = tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "taxMaritalStatus" && x.fldOptConfValue == pkjTaxInfo.fld_TaxMaritalStatus).Select(s => s.fldOptConfDesc).FirstOrDefault();
            var personDisable = ((Constans.YaTidak)int.Parse(pkjPCBInfo.fld_IsIndividuOKU)).ToString();
            var spouseDisable = ((Constans.YaTidak)int.Parse(pkjPCBInfo.fld_IsSpouseOKU)).ToString();

            Chunk chunk = new Chunk("NAMA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk(pkjInfo.fld_Nama, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            chunk = new Chunk("NOMBOR PENGENALAN DIRI", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            pkjTaxInfo.fld_TaxNo = pkjTaxInfo.fld_TaxNo == null ? "" : pkjTaxInfo.fld_TaxNo;

            chunk = new Chunk(pkjTaxInfo.fld_TaxNo.ToUpper(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            chunk = new Chunk("BULAN", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk(monthName, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("TAHUN", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk(pkjGajiInfo.fld_Year.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("STATUS", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk(taxResidency, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            chunk = new Chunk("STATUS PERKAHWINAN", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk(taxMaritalStatus, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            chunk = new Chunk("INDIVIDU KURANG UPAYA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk(personDisable, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            chunk = new Chunk("PASANGAN KURANG UPAYA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk(spouseDisable, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerChildInfo(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(4);
            float[] widths = new float[] { 0.3f, 3, 0.5f, 0.5f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            var pkjGajiInfo = tbl_GajiBulanan.Where(x => x.fld_Nopkj == NoPkj).FirstOrDefault();
            var pkjPCBInfo = tbl_ByrCarumanTambahan.Where(x => x.fld_GajiID == pkjGajiInfo.fld_ID).FirstOrDefault();

            Chunk chunk = new Chunk("ANAK (KANDUNG/TIRI/ANGKAT)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 2, 10, 10);

            chunk = new Chunk("Bilangan anak yang dituntut oleh diri sendiri", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 2, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 2, 10, 10);

            chunk = new Chunk("Kadar kelayakan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 2, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 2, 10, 10);

            chunk = new Chunk("100%", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 10, 1);

            chunk = new Chunk("50%", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 10, 1);

            chunk = new Chunk("(a)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 10);

            chunk = new Chunk("Di bawah 18 tahun", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(pkjPCBInfo.fld_ChildBelow18Full.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(pkjPCBInfo.fld_ChildBelow18Half.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(b)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 10);

            chunk = new Chunk("18 tahun dan ke atas yang masih belajar (termasuk sijil/matrikulasi)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(pkjPCBInfo.fld_ChildAbove18CertFull.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(pkjPCBInfo.fld_ChildAbove18CertHalf.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(c)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 10);

            chunk = new Chunk("Lebih 18 tahun dan sedang belajar sepenuh masa di peringkat diploma ke atas (Malaysia) atau di peringkat ijazah dan ke atas (luar Malaysia)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(pkjPCBInfo.fld_ChildAbove18HigherFull.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(pkjPCBInfo.fld_ChildAbove18HigherHalf.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(d)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 10);

            chunk = new Chunk("Kurang upaya", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(pkjPCBInfo.fld_DisabledChildFull.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(pkjPCBInfo.fld_DisabledChildHalf.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(e)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 10);

            chunk = new Chunk("Kurang upaya belajar di IPT", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(pkjPCBInfo.fld_DisabledChildStudyFull.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(pkjPCBInfo.fld_DisabledChildStudyHalf.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerRemuneration1(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 3, 1, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            var pkjGajiInfo = tbl_GajiBulanan.Where(x => x.fld_Nopkj == NoPkj).FirstOrDefault();
            var pkjPCBInfo = tbl_ByrCarumanTambahan.Where(x => x.fld_GajiID == pkjGajiInfo.fld_ID).FirstOrDefault();

            Chunk chunk = new Chunk("SARAAN/PCB/REBAT/POTONGAN TERKUMPUL BULAN SEBELUM DALAM TAHUN SEMASA (TERMASUK DI MAJIKAN LAMA)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 1, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("Saraan/Manfaat Berupa Barangan (MBB)/Nilai Tempat Kediaman (NTK) terkumpul", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Y), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("KWSP dan Kumpulan Wang Lain Yang Diluluskan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_K), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Saraan bersih terkumpul", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            var YMinusK = pkjPCBInfo.fld_Y - pkjPCBInfo.fld_K;
            chunk = new Chunk(GetTriager.GetTotalForMoney(YMinusK), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("PCB bulanan terkumpul yang telah dibayar (termasuk PCB saraan tambahan)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_X), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah terkumpul zakat/fitrah yang telah dibayar", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah terkumpul levi yang telah dibayar", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Lain-lain potongan terkumpul", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("1) Perbelanjaan rawatan perubatan, keperluan khas dan penjaga untuk ibu bapa (keadaan kesihatan disahkan oleh pengamal perubatan); ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("2) Peralatan sokongan asas untuk kegunaan sendiri, suami/isteri, anak atau ibu bapa yang kurang upaya ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("3) Yuran Pengajian (sendiri) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) Peringkat selain sarjana atau doktor falsafah - bidang undang-undang, perakaunan, kewangan Islam, teknikal, vokasional, industri, saintifik atau teknologi; ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Peringkat sarjana atau doktor falsafah - sebarang bidang atau kursus pengajian.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("c) Bayaran kursus peningkatan kemahiran/kemajuan diri", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah (a) + (b) + (c)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("4) Perbelanjaan perubatan bagi: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) Penyakit serius untuk diri sendiri/ pasangan / anak ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Rawatan kesuburan untuk diri sendiri/ pasangan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("c) Pemvaksinan ke atas diri sendiri / pasangan / anak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("d) Pemeriksaan perubatan penuh, ujian pengesanan COVID-19 termasuk pembelian kit ujian kendiri, pemeriksaan kesihatan mental atau konsultasi ke atas diri sendiri / pasangan / anak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("e) Penilaian bagi tujuan diagnosis, program Intervensi awal atau rawatan pemulihan bagi anak kurang upaya pembelanjaran.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah a + b + c + d + e", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("5) Pembelian bahan bacaan, komputer, telefon pintar, tablet, alat sukan, yuran keahlian gimnasium dan langganan internet", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("6) Pembelian peralatan sukan, bayaran sewa atau fi kemasukan ke fasiliti sukan dan fi bayaran pendaftaran pertandingan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("7) Pembelian peralatan penyusuan ibu untuk kegunaan diri sendiri bagi anak berumur 2 tahun dan ke bawah (Potongan dibenarkan sekali setiap 2 tahun taksiran)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("8) Yuran penghantaran anak berumur 6 tahun dan ke bawah ke taman asuhan kanak-kanak / tadika yang berdaftar", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("9) Tabungan bersih dalam Skim Simpanan Pendidikan Nasional (Jumlah simpanan dalam tahun semasa tolak jumlah pengeluaran dalam tahun semasa)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("10) Bayaran alimoni kepada bekas isteri", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("11) KWSP Sukarela / Insurans Nyawa", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) KWSP Sukarela;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Insuran nyawa / KWSP Sukarela", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah (a) + (b)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("12) Skim persaraan swasta dan anuiti tertangguh", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("13) Insurans pendidikan dan perubatan ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("14) Caruman kepada PERKESO mengikut Akta Keselamatan Sosial Pekerja 1969 / Akta Sistem Insurans Pekerjaan 2017", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("15) Perbelanjaan bayaran pemasangan/sewaan/pembelian termasuk sewa-beli peralatan / langganan bagi penggunaan kemudahan pengecasan kenderaan elektrik bagi kenderaan sendiri (Bukan untuk kegunaan perniagaan).", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah potongan terkumpul", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_LP), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerRemuneration2(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 3, 1, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            var pkjGajiInfo = tbl_GajiBulanan.Where(x => x.fld_Nopkj == NoPkj).FirstOrDefault();
            var pkjPCBInfo = tbl_ByrCarumanTambahan.Where(x => x.fld_GajiID == pkjGajiInfo.fld_ID).FirstOrDefault();

            Chunk chunk = new Chunk("SARAAN BULAN SEMASA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 1, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("Saraan bulan semasa", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Y1), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("KWSP dan Kumpulan Wang Lain Yang Diluluskan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_K1), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah saraan bersih bulan semasa", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            var Y1MinusK1 = pkjPCBInfo.fld_Y1 - pkjPCBInfo.fld_K1;
            chunk = new Chunk(GetTriager.GetTotalForMoney(Y1MinusK1), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Manfaat Berupa Barangan (MBB)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Nilai Tempat Kediaman (NTK)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerRemuneration3(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 3, 1, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("SARAAN TAMBAHAN BULAN SEMASA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 1, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("(a) Bonus", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(b) Tunggakan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(c) Komisen (tidak dibayar setiap bulan) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(d) Ganjaran", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(e) Pampasan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(f) Yuran pengarah (tidak dibayar setiap bulan) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(g) Cukai Pendapatan yang dibayar oleh majikan bagi pihak pekerja", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(h) Lain-lain ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(i) Jumlah saraan tambahan (a) hingga (h)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(j) KWSP saraan tambahan [terhad RM4000 setahun] (a) hingga (h)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(k) Jumlah saraan tambahan bersih [(i)-(j)]", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerDeduction(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 3, 1, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("POTONGAN BULAN SEMASA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 1, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("1) Perbelanjaan rawatan perubatan, keperluan khas dan penjaga untuk ibu bapa (keadaan kesihatan disahkan oleh pengamal perubatan); ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("2) Peralatan sokongan asas untuk kegunaan sendiri, suami/isteri, anak atau ibu bapa yang kurang upaya", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("3) Yuran Pengajian (sendiri) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) Peringkat selain sarjana atau doktor falsafah - bidang undang-undang, perakaunan, kewangan Islam, teknikal, vokasional, industri, saintifik atau teknologi;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Peringkat sarjana atau doktor falsafah - sebarang bidang atau kursus pengajian.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("c) Bayaran kursus peningkatan kemahiran/kemajuan diri", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah (a) + (b) + (c) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("4) Perbelanjaan perubatan bagi: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) Penyakit serius untuk diri sendiri/ pasangan / anak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Rawatan kesuburan untuk diri sendiri/ pasangan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("c) Pemvaksinan ke atas diri sendiri / pasangan / anak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("d) Pemeriksaan perubatan penuh, ujian pengesanan COVID-19 termasuk pembelian kit ujian kendiri, pemeriksaan kesihatan mental atau konsultasi ke atas diri sendiri / pasangan / anak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("e) Penilaian bagi tujuan diagnosis, program Intervensi awal atau rawatan pemulihan bagi anak kurang upaya pembelanjaran.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah a + b + c + d + e ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("5) Pembelian bahan bacaan, komputer, telefon pintar, tablet, alat sukan, yuran keahlian gimnasium dan langganan internet", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("6) Pembelian peralatan sukan, bayaran sewa atau fi kemasukan ke fasiliti sukan dan fi bayaran pendaftaran pertandingan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("7) Pembelian peralatan penyusuan ibu untuk kegunaan diri sendiri bagi anak berumur 2 tahun dan ke bawah (Potongan dibenarkan sekali setiap 2 tahun taksiran)\r\n", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("8) Yuran penghantaran anak berumur 6 tahun dan ke bawah ke taman asuhan kanak-kanak / tadika yang berdafta", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("9) Tabungan bersih dalam Skim Simpanan Pendidikan Nasional (Jumlah simpanan dalam tahun semasa tolak jumlah pengeluaran dalam tahun semasa)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("10) Bayaran alimoni kepada bekas isteri", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("11) KWSP Sukarela / Insurans Nyawa ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) KWSP Sukarela;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Insuran nyawa / KWSP Sukarela", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah (a) + (b)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("12) Skim persaraan swasta dan anuiti tertangguh", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("13) Insurans pendidikan dan perubatan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("14) Caruman kepada PERKESO mengikut Akta Keselamatan Sosial Pekerja 1969 / Akta Sistem Insurans Pekerjaan 2017", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("15) Perbelanjaan bayaran pemasangan/sewaan/pembelian termasuk sewa-beli peralatan / langganan bagi penggunaan kemudahan pengecasan kenderaan elektrik bagi kenderaan sendiri (Bukan untuk kegunaan perniagaan).", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah Potongan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerRebate(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(2);
            float[] widths = new float[] { 3, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("MAKLUMAT REBAT BULAN SEMASA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("i. Zakat atau Fitrah", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("ii. Levi pelepasan bagi perjalanan umrah / perjalanan tujuan keagamaan agama lain (Terhad 2 kali tuntutan seumur hidup) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerPCBCalculation(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(4);
            float[] widths = new float[] { 0.5f, 0.3f, 3, 0.8f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            var pkjGajiInfo = tbl_GajiBulanan.Where(x => x.fld_Nopkj == NoPkj).FirstOrDefault();
            var pkjPCBInfo = tbl_ByrCarumanTambahan.Where(x => x.fld_GajiID == pkjGajiInfo.fld_ID).FirstOrDefault();

            Chunk chunk = new Chunk("PENGIRAAN PCB", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 4, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("1. PCB Ke Atas Saraan Bersih Setahun Tidak Termasuk Saraan Tambahan:", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 3, 10, 5);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("PCB Bersih", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(" = [((P-M)R + B - ( Z + X) )/n+1] - Zakat/Fi/Levi Bulan Semasa", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("P = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Jumlah pendapatan bercukai untuk setahun tidak termasuk saraan tambahan semasa \r\n[E(Y-K)+(Y1-K1)+[(Y2-K2)n] + (Yt-Kt)]-(D+S+Du+Su\r\n+QC+ELP+LP1) iaitu(Yt-Kt) = 0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("E(Y-K) = Jumlah saraan bersih terkumpul termasuk saraan tambahan yang telah dibayar kepada pekerja sehingga sebelum bulan semasa termasuk saraan bersih yang dibayar oleh majikan lama (sekiranya ada);", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            var YMinusK = pkjPCBInfo.fld_Y - pkjPCBInfo.fld_K;
            chunk = new Chunk(GetTriager.GetTotalForMoney(YMinusK), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Y = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Jumlah saraan kasar bulanan dan saraan tambahan yang telah dibayar termasuk saraan kasar bulanan yang telah dibayar oleh majikan lama (sekiranya ada);", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Y), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("K = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Jumlah caruman KWSP atau Kumpulan Wang Lain Yang Diluluskan yang telah dibuat ke atas semua saraan (saraan bulanan, saraan tambahan dan saraan daripada majikan terdahulu dalam tahun semasa) yang telah dibayar (termasuk premium yang dituntut di bawah penggajian terdahulu dalam tahun semasa, jika ada) tidak melebihi RM4,000.00 setahun;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_K), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Y1 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Saraan biasa bulan semasa;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Y1), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("K1 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Caruman KWSP atau Kumpulan Wang Lain Yang Diluluskan yang telah dibayar tertakluk kepada jumlah yang layak bagi saraan bulan semasa tidak melebihi RM4,000.00 setahun;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_K1), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Y2 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Anggaran saraan seperti Y1 untuk bulan seterusnya;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Y2), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("K2 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Anggaran baki jumlah caruman KWSP atau Kumpulan Wang Lain Yang Diluluskan yang dibayar bagi baki bulan yang layak [[RM4,000 (Terhad) - (K + K1 + Kt)]/n] atau K1, yang mana lebih rendah;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_K2), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("n = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Baki bulan bekerja dalam setahun;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(pkjPCBInfo.fld_n.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("n + 1 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Baki bulan bekerja dalam setahun termasuk bulan semasa;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk((pkjPCBInfo.fld_n + 1).ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("D = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Potongan individu;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_D), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("S = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Potongan pasangan;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_S), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Du = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Potongan individu kurang upaya.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Du), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Su = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Potongan pasangan kurang upaya.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Su), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Q = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Potongan bagi anak yang layak;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Q), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("C = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Bilangan anak yang layak; ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(pkjPCBInfo.fld_C.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("ELP = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Lain-lain potongan terkumpul yang telah dibenarkan termasuk daripada penggajian terdahulu, (sekiranya ada);", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(0), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("LP1 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Lain-lain potongan bulan semasa yang dibenarkan;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_LP1), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("P = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            var P = "[E(" + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Y) + " - " + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_K) + ") + (" + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Y1) + " - " + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_K1) + ") x " + pkjPCBInfo.fld_n + "] + (" + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Yt) + " - " + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Kt) + ")] - (" + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_D) + " + " + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_S) + " + " + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Du) + " + " + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Su) + " + (" + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Q) + " x " + pkjPCBInfo.fld_C + ") + 0.00 + " + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_LP1) + ")";
            chunk = new Chunk(P, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_P), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("M = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Amaun pendapatan bercukai yang pertama bagi setiap banjaran pendapatan bercukai setahun; ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_M), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("R = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Kadar peratusan cukai; ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_R), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("B = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Amaun cukai atas jumlah M selepas tolak rebat cukai individu dan pasangan (sekiranya layak); ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_B), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Z = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Zakat/Fitrah/Levi terkumpul yang telah dibayar tidak termasuk Zakat/Fitrah/Levi bulan semasa; ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Z), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("X = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("PCB terkumpul yang telah dibayar pada bulan sebelum termasuk daripada penggajian terdahulu (termasuk PCB saraan tambahan);", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_X), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Cukai Setahun", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(" =", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_CarumanPekerjaYearly), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("PCB Bulan Semasa", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            var PCB = " = [(" + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_P) + " - " + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_M) + ") x " + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_R) + "(" + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_B) + ") - (" + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Z) + " + " + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_X) + ")) / " + pkjPCBInfo.fld_n + " + 1]";
            chunk = new Chunk(PCB, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_CarumanPekerja), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("PCB Bersih", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(" = PCB Bulan Semasa - (" + GetTriager.GetTotalForMoney(pkjPCBInfo.fld_Z) + ")", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_CarumanPekerjaNet), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("2. PCB Perlu dipotong:", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 3, 10, 5);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("PCB", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_CarumanPekerjaNet), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.BackgroundColor = BaseColor.YELLOW;
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerPCBResultCalculation(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(2);
            float[] widths = new float[] { 3, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            var pkjGajiInfo = tbl_GajiBulanan.Where(x => x.fld_Nopkj == NoPkj).FirstOrDefault();
            var pkjPCBInfo = tbl_ByrCarumanTambahan.Where(x => x.fld_GajiID == pkjGajiInfo.fld_ID).FirstOrDefault();

            Chunk chunk = new Chunk("HASIL PENGIRAAN", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("PCB Bulan JANUARI Perlu Dipotong (setelah dibundarkan ke perpuluhan atas yang terdekat)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLUE));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(GetTriager.GetTotalForMoney(pkjPCBInfo.fld_CarumanPekerjaNet), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.BackgroundColor = BaseColor.YELLOW;
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            var phrase = new Phrase();
            chunk = new Chunk("Nota : PCB bagi setiap bulan bermula pada bulan FEBRUARI sehingga bulan DISEMBER adalah RM 0.00 sekiranya tiada perubahan saraan, potongan dan rebat", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            phrase.Add(chunk);
            chunk = new Chunk(" (PCB tidak dikenakan jika amaun kurang dari RM10)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            phrase.Add(chunk);

            cell = new PdfPCell(phrase);
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public void CellWithBgColorPropoties(PdfPCell cell, PdfPTable table, int HoriElement, int VertiElement, int Border, int Colspan, int PaddingBottom, int PaddingLeft, BaseColor BGColor)
        {
            cell.HorizontalAlignment = HoriElement;
            cell.VerticalAlignment = VertiElement;
            cell.BorderColorTop = BaseColor.WHITE;
            cell.BorderColorBottom = BaseColor.WHITE;
            cell.BorderColorLeft = BaseColor.WHITE;
            cell.BorderColorRight = BaseColor.WHITE;
            cell.BorderWidthLeft = 1;
            cell.BorderWidthRight = 1;
            cell.PaddingBottom = PaddingBottom;
            cell.PaddingLeft = PaddingLeft;
            cell.Colspan = Colspan;
            cell.BackgroundColor = BGColor;
            table.AddCell(cell);
        }

        public void CellPropoties(PdfPCell cell, PdfPTable table, int HoriElement, int VertiElement, int Border, int Colspan, int PaddingBottom, int PaddingLeft)
        {
            cell.HorizontalAlignment = HoriElement;
            cell.VerticalAlignment = VertiElement;
            cell.Border = Border;
            cell.PaddingBottom = PaddingBottom;
            cell.PaddingLeft = PaddingLeft;
            cell.Colspan = Colspan;
            table.AddCell(cell);
        }
    }

    public class PDFFooter : PdfPageEventHelper
    {
        // write on top of document
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
        }

        // write on start of each page
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
        }

        // write on end of each page
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            PdfPCell cell;
            tabFot.TotalWidth = 300F;
            Chunk chunk = new Chunk(writer.PageNumber.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Border = 0;
            tabFot.AddCell(cell);
            tabFot.WriteSelectedRows(0, -1, 265, 40, writer.DirectContent);
        }

        //write on close of document
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
        }
    }
}