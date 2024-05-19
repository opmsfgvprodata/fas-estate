using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class EAFormReportController : Controller
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
        // GET: EAFormReport
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
            ViewBag.YearList = yearlist;
            ViewBag.StatusList = StatusList;
            return View();
        }

        public FileStreamResult EAFormPdf(int? RadioGroup, int? YearList, string SelectionList, string StatusList)
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
                parameters.Add("Year", YearList);
                con.Open();
                SqlMapper.Settings.CommandTimeout = 300;
                var readerMapper = SqlMapper.QueryMultiple(con, "sp_RptEA", parameters);

                tbl_Pkjmast = readerMapper.Read<tbl_Pkjmast>().ToList();
                tbl_GajiBulanan = readerMapper.Read<tbl_GajiBulanan>().ToList();
                tbl_TaxWorkerInfo = readerMapper.Read<tbl_TaxWorkerInfo>().ToList();
                tbl_ByrCarumanTambahan = readerMapper.Read<tbl_ByrCarumanTambahan>().ToList();
                tblOptionConfigsWebs = readerMapper.Read<MasterModels.tblOptionConfigsWeb>().ToList();

                con.Close();
            }
            catch (Exception ex)
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

            MemoryStream output = new MemoryStream();

            string oldFile = GetConfig.PdfPathFile("EA.pdf");

            // open the reader
            PdfReader reader = new PdfReader(oldFile);
            Rectangle size = reader.GetPageSizeWithRotation(1);
            Document document = new Document(size);

            // open the writer
            MemoryStream ms = new MemoryStream();
            //FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            document.Open();

            var getWorkerPCBAvailable = tbl_Pkjmast.Join(tbl_GajiBulanan, e => e.fld_NopkjPermanent, d => d.fld_NoPkjPermanent,
                (tbl1, tbl2) => new { tbl_Pkjmast = tbl1, tbl_GajiBulanan = tbl2 }).Join(tbl_ByrCarumanTambahan, ee => ee.tbl_GajiBulanan.fld_ID, dd => dd.fld_GajiID,
                (tbl1, tbl2) => new { tbl_GajiBulanan = tbl1, tbl_ByrCarumanTambahan = tbl2 }).ToList();

            var getWorkerIds = getWorkerPCBAvailable.Select(s => s.tbl_GajiBulanan.tbl_Pkjmast.fld_NopkjPermanent).Distinct().ToList();

            // the pdf content
            if (getWorkerIds.Count() > 0)
            {
                foreach (var WorkerId in getWorkerIds)
                {
                    var WorkerPCB = getWorkerPCBAvailable.Where(x => x.tbl_GajiBulanan.tbl_Pkjmast.fld_NopkjPermanent == WorkerId).ToList();
                    var pkjInfo = tbl_Pkjmast.Where(x => x.fld_NopkjPermanent == WorkerId).FirstOrDefault();
                    var pkjTaxInfo = tbl_TaxWorkerInfo.Where(x => x.fld_NopkjPermanent == WorkerId).FirstOrDefault();
                    var pkjGajiInfo = tbl_GajiBulanan.Where(x => x.fld_NoPkjPermanent == WorkerId).ToList();
                    var gajiID = pkjGajiInfo.Select(s => s.fld_ID).ToList();
                    var pkjPcbContribution = tbl_ByrCarumanTambahan.Where(x => gajiID.Contains(x.fld_GajiID.Value)).ToList();

                    document.NewPage();
                    PdfContentByte cb = writer.DirectContent;

                    // select the font properties
                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
                    cb.SetColorFill(BaseColor.DARK_GRAY);
                    cb.SetFontAndSize(bf, 8);

                    // write the text in the pdf content
                    cb.BeginText();
                    string text = YearList.ToString(); //Year
                                                       // put the alignment and coordinates here
                    cb.ShowTextAligned(0, text, 373f, 725f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = "CAW. JALAN DUTA"; //Year
                                              // put the alignment and coordinates here
                    cb.ShowTextAligned(0, text, 492f, 725f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = pkjTaxInfo.fld_TaxNo; //Tax number
                                                 // put the alignment and coordinates here
                    cb.ShowTextAligned(0, text, 420f, 738f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = pkjInfo.fld_Nama; //Name
                                             // put the alignment and coordinates here
                    cb.ShowTextAligned(0, text, 270f, 683f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fldOptConfValue == pkjInfo.fld_Ktgpkj).Select(s => s.fldOptConfDesc).FirstOrDefault(); //Position
                                                                                                                                                                                          // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(0, text, 163f, 670f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = pkjInfo.fld_Nopkj; //Worker No
                                              // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(0, text, 436f, 670f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = pkjInfo.fld_Nokp; //IC No
                                             // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(0, text, 163f, 656f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = pkjInfo.fld_Psptno; //Passport
                                               // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(0, text, 436f, 656f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = pkjInfo.fld_Nokwsp; //KWSP No
                                               // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(0, text, 163f, 642f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = pkjInfo.fld_Noperkeso; //Perkeso No
                                                  // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(0, text, 436f, 642f, 0); //-10
                    cb.EndText();

                    var fld_ChildAbove18CertFull = pkjTaxInfo.fld_ChildAbove18CertFull == null ? 0 : pkjTaxInfo.fld_ChildAbove18CertFull;
                    var fld_ChildAbove18CertHalf = pkjTaxInfo.fld_ChildAbove18CertHalf == null ? 0 : pkjTaxInfo.fld_ChildAbove18CertHalf;
                    var fld_ChildAbove18HigherFull = pkjTaxInfo.fld_ChildAbove18HigherFull == null ? 0 : pkjTaxInfo.fld_ChildAbove18HigherFull;
                    var fld_ChildAbove18HigherHalf = pkjTaxInfo.fld_ChildAbove18HigherHalf == null ? 0 : pkjTaxInfo.fld_ChildAbove18HigherHalf;
                    var fld_ChildBelow18Full = pkjTaxInfo.fld_ChildBelow18Full == null ? 0 : pkjTaxInfo.fld_ChildBelow18Full;
                    var fld_ChildBelow18Half = pkjTaxInfo.fld_ChildBelow18Half == null ? 0 : pkjTaxInfo.fld_ChildBelow18Half;
                    var fld_DisabledChildFull = pkjTaxInfo.fld_DisabledChildFull == null ? 0 : pkjTaxInfo.fld_DisabledChildFull;
                    var fld_DisabledChildHalf = pkjTaxInfo.fld_DisabledChildHalf == null ? 0 : pkjTaxInfo.fld_DisabledChildHalf;
                    var fld_DisabledChildStudyFull = pkjTaxInfo.fld_DisabledChildStudyFull == null ? 0 : pkjTaxInfo.fld_DisabledChildStudyFull;
                    var fld_DisabledChildStudyHalf = pkjTaxInfo.fld_DisabledChildStudyHalf == null ? 0 : pkjTaxInfo.fld_DisabledChildStudyHalf;

                    var totalChild = fld_ChildAbove18CertFull
                        + fld_ChildAbove18CertHalf
                        + fld_ChildAbove18HigherFull
                        + fld_ChildAbove18HigherHalf
                        + fld_ChildBelow18Full
                        + fld_ChildBelow18Half
                        + fld_DisabledChildFull
                        + fld_DisabledChildHalf
                        + fld_DisabledChildStudyFull
                        + fld_DisabledChildStudyHalf;

                    cb.BeginText();
                    text = totalChild.ToString(); //Child
                                                  // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(0, text, 197f, 618f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = ""; //Mula Kerja
                               // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(0, text, 436f, 616f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = ""; //Tamat Kerja
                               // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(0, text, 436f, 602f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = pkjGajiInfo.Sum(s => s.fld_GajiKasar).ToString(); //Gross pay
                                                                             // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(1, text, 515f, 555f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = ""; //Bonus pay
                               // put the alignment and coordinates here
                    cb.ShowTextAligned(1, text, 515f, 542f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = pkjGajiInfo.Sum(s => s.fld_GajiKasar).ToString(); //Gross pay
                    // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(1, text, 515f, 327f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = pkjPcbContribution.Where(x => x.fld_KodCaruman == "PCB").Sum(s => s.fld_CarumanPekerja).ToString(); //PCB
                                                                                                                               // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(1, text, 515f, 295f, 0); //-10
                    cb.EndText();

                    var QC = pkjPcbContribution.Where(x => x.fld_KodCaruman == "PCB").OrderBy(o => o.fld_n).Take(1).FirstOrDefault();
                    cb.BeginText();
                    decimal qC = 0m;
                    if (QC != null)
                    {
                        qC = Math.Round(QC.fld_Q.Value * QC.fld_C.Value, 2);
                    }
                    text = qC.ToString(); //QC
                                          // put the alignment and coordinates here
                    cb.ShowTextAligned(1, text, 515f, 208f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = "KUMPULAN WANG SIMPANAN PEKERJA"; //KWSP
                                                             // put the alignment and coordinates here
                    cb.ShowTextAligned(0, text, 185f, 172f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = pkjGajiInfo.Sum(s => s.fld_KWSPPkj).ToString(); //KWSP Worker Pay
                                                                           // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(1, text, 515f, 160f, 0); //-10
                    cb.EndText();

                    string[] perkesoConCode = new string[] { "SIP", "SBKP" };
                    cb.BeginText();
                    text = pkjPcbContribution.Where(x => perkesoConCode.Contains(x.fld_KodCaruman)).Sum(s => s.fld_CarumanPekerja).ToString(); //PERKESO Worker Pay
                                                                                                                                               // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(1, text, 515f, 147f, 0); //-10
                    cb.EndText();

                    cb.BeginText();
                    text = timezone.gettimezone().ToString("dd/MM/yyyy"); //Date
                                                                          // put the alignment and coordinates here
                    cb.ShowTextAligned(0, text, 96f, 40f, 0); //-10
                    cb.EndText();

                    // create the new page and add it to the pdf
                    PdfImportedPage page = writer.GetImportedPage(reader, 1);
                    cb.AddTemplate(page, 0, 0);
                }

                document.Close();
                writer.Close();
                reader.Close();
            }
            // close the streams and voilá the file should be changed :)
            else
            {
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 5);
                ms = new MemoryStream();
                output = new MemoryStream();
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                Chunk chunk = new Chunk();
                Paragraph para = new Paragraph();
                pdfDoc.Open();
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
            }

            ms.Close();

            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

    }
}