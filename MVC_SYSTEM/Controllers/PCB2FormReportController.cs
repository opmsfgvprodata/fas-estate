using Dapper;
using Itenso.TimePeriod;
using iTextSharp.text.pdf;
using iTextSharp.text;
using MVC_SYSTEM.App_LocalResources;
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
using MVC_SYSTEM.Attributes;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class PCB2FormReportController : Controller
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
        List<tbl_TaxPCB2Form> tbl_TaxPCB2Form = new List<tbl_TaxPCB2Form>();
        // GET: PCB2FormReport
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

        public FileStreamResult PCB2FormPdf(int? RadioGroup, int? YearList, string SelectionList, string StatusList)
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
                var readerMapper = SqlMapper.QueryMultiple(con, "sp_RptPCB2Form", parameters);

                tbl_Pkjmast = readerMapper.Read<tbl_Pkjmast>().ToList();
                tbl_GajiBulanan = readerMapper.Read<tbl_GajiBulanan>().ToList();
                tbl_TaxWorkerInfo = readerMapper.Read<tbl_TaxWorkerInfo>().ToList();
                tbl_ByrCarumanTambahan = readerMapper.Read<tbl_ByrCarumanTambahan>().ToList();
                tblOptionConfigsWebs = readerMapper.Read<MasterModels.tblOptionConfigsWeb>().ToList();
                tbl_TaxPCB2Form = readerMapper.Read<tbl_TaxPCB2Form>().ToList();

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

            string oldFile = GetConfig.PdfPathFile("PCB2.pdf");

            // open the reader
            PdfReader reader = new PdfReader(oldFile);
            Rectangle size = reader.GetPageSizeWithRotation(1);
            Document document = new Document(size);

            // open the writer
            MemoryStream ms = new MemoryStream();
            //FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            document.Open();

            var getWorkerPCBAvailable = tbl_Pkjmast.Join(tbl_GajiBulanan, e => e.fld_Nopkj, d => d.fld_Nopkj,
                (tbl1, tbl2) => new { tbl_Pkjmast = tbl1, tbl_GajiBulanan = tbl2 }).Join(tbl_ByrCarumanTambahan, ee => ee.tbl_GajiBulanan.fld_ID, dd => dd.fld_GajiID,
                (tbl1, tbl2) => new { tbl_GajiBulanan = tbl1, tbl_ByrCarumanTambahan = tbl2 }).ToList();
            //
            var getWorkerPCBAvailableForSpclInc = dbr.tbl_SpecialInsentif.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList).ToList();

            var getWorkerIds = getWorkerPCBAvailable.Select(s => s.tbl_GajiBulanan.tbl_Pkjmast.fld_NopkjPermanent).Distinct().ToList();

            var ladang = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).FirstOrDefault();
            var syarikat = db.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == ladang.fld_CostCentre).FirstOrDefault();
            // the pdf content
            if (getWorkerIds.Count() > 0)
            {
                foreach (var WorkerId in getWorkerIds)
                {
                    //var WorkerPCB = getWorkerPCBAvailable.Where(x => x.tbl_GajiBulanan.tbl_Pkjmast.fld_NopkjPermanent == WorkerId).ToList();
                    var pkjInfo = tbl_Pkjmast.Where(x => x.fld_NopkjPermanent == WorkerId).FirstOrDefault();
                    var pkjTaxInfo = tbl_TaxWorkerInfo.Where(x => x.fld_NopkjPermanent == WorkerId).FirstOrDefault();
                    var pkjGajiInfo = tbl_GajiBulanan.Where(x => x.fld_NoPkjPermanent == WorkerId).ToList();
                    var gajiID = pkjGajiInfo.Select(s => s.fld_ID).ToList();
                    var pkjPcbContribution = tbl_ByrCarumanTambahan.Where(x => gajiID.Contains(x.fld_GajiID.Value)).ToList();
                    var pkjPcbForm2 = tbl_TaxPCB2Form.Where(x => gajiID.Contains(x.fld_GajiID.Value)).ToList();
                    var pkjSpecialInc = getWorkerPCBAvailableForSpclInc.Where(x => x.fld_Nopkj == pkjInfo.fld_Nopkj).ToList();

                    document.NewPage();
                    PdfContentByte cb = writer.DirectContent;

                    // select the font properties
                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.EMBEDDED);
                    cb.SetColorFill(BaseColor.DARK_GRAY);
                    cb.SetFontAndSize(bf, 8);

                    // write the text in the pdf content
                    cb.BeginText();
                    string text = timezone.gettimezone().ToString("dd/MM/yyyy"); ; //Date
                                                                                   // put the alignment and coordinates here
                    cb.ShowTextAligned(0, text, 475f, 688f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = "CAW. JALAN DUTA"; //Cawangan
                                              // put the alignment and coordinates here
                    cb.ShowTextAligned(0, text, 145f, 648f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = YearList.ToString(); //Year
                                                // put the alignment and coordinates here
                    cb.ShowTextAligned(0, text, 285f, 610f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = pkjInfo.fld_Nama; //Name
                                             // put the alignment and coordinates here
                    cb.ShowTextAligned(0, text, 285f, 595f, 0);
                    cb.EndText();

                    if (!string.IsNullOrEmpty(pkjInfo.fld_Nokp))
                    {
                        text = pkjInfo.fld_Nokp; //IC No
                    }
                    else
                    {
                        text = pkjInfo.fld_Psptno; //Passport
                    }
                    cb.BeginText();

                    cb.ShowTextAligned(0, text, 285f, 582f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = pkjTaxInfo.fld_TaxNo; //Tax number
                                                 // put the alignment and coordinates here

                    text = text == null ? "" : text;

                    cb.ShowTextAligned(0, text, 285f, 568f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = pkjInfo.fld_Nopkj; //Worker No
                                              // put the alignment and coordinates here
                    text = text == null ? "" : text;
                    cb.ShowTextAligned(0, text, 285f, 556f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = syarikat.fld_EmployerTaxNo; //Worker No
                                                       // put the alignment and coordinates here
                    cb.ShowTextAligned(0, text, 285f, 543f, 0);
                    cb.EndText();

                    var pCB1 = pkjPcbContribution.Where(x => x.fld_Month == 1).FirstOrDefault();
                    var pkjSpecialInc1 = pkjSpecialInc.Where(x => x.fld_Month == 1).FirstOrDefault(); 
                    var pCB2Form1 = pkjPcbForm2.Where(x => x.fld_Month == 1).FirstOrDefault();
                    

                    if (pCB1 != null)
                    {
                        pCB1.fld_CarumanPekerja = (pkjSpecialInc1 != null ? pkjSpecialInc1.fld_PCBCarumanPekerja : 0) + pCB1.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB1.fld_CarumanPekerja.ToString() == "0.00" || pCB1.fld_CarumanPekerja <= 10 ? "" : pCB1.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 437f, 0);
                        cb.EndText();
                    }
                    if (pCB2Form1 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form1.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form1.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 437f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form1.fld_PCBReceiptNo == null ? "" : pCB2Form1.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 437f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form1.fld_CP38ReceiptNo == null ? "" : pCB2Form1.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 437f, 0);
                        cb.EndText();

                        if (pCB2Form1.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form1.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 437f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form1.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form1.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 437f, 0);
                            cb.EndText();
                        }

                    }

                    var pCB2 = pkjPcbContribution.Where(x => x.fld_Month == 2).FirstOrDefault();
                    var pkjSpecialInc2 = pkjSpecialInc.Where(x => x.fld_Month == 2).FirstOrDefault();
                    var pCB2Form2 = pkjPcbForm2.Where(x => x.fld_Month == 2).FirstOrDefault();

                    if (pCB2 != null)
                    {
                        pCB2.fld_CarumanPekerja = (pkjSpecialInc2 != null ? pkjSpecialInc2.fld_PCBCarumanPekerja : 0) + pCB2.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB2.fld_CarumanPekerja.ToString() == "0.00" || pCB2.fld_CarumanPekerja <= 10 ? "" : pCB2.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 424f, 0);
                        cb.EndText();
                    }

                    if (pCB2Form2 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form2.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form2.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 424f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form2.fld_PCBReceiptNo == null ? "" : pCB2Form2.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 424f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form2.fld_CP38ReceiptNo == null ? "" : pCB2Form2.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 424f, 0);
                        cb.EndText();

                        if (pCB2Form2.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form2.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 424f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form2.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form2.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 424f, 0);
                            cb.EndText();
                        }
                    }

                    var pCB3 = pkjPcbContribution.Where(x => x.fld_Month == 3).FirstOrDefault();
                    var pkjSpecialInc3 = pkjSpecialInc.Where(x => x.fld_Month == 3).FirstOrDefault();
                    var pCB2Form3 = pkjPcbForm2.Where(x => x.fld_Month == 3).FirstOrDefault();

                    if (pCB3 != null)
                    {
                        pCB3.fld_CarumanPekerja = (pkjSpecialInc3 != null ? pkjSpecialInc3.fld_PCBCarumanPekerja : 0) + pCB3.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB3.fld_CarumanPekerja.ToString() == "0.00" || pCB3.fld_CarumanPekerja <= 10 ? "" : pCB3.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 411f, 0);
                        cb.EndText();
                    }

                    if (pCB2Form3 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form3.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form3.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 411f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form3.fld_PCBReceiptNo == null ? "" : pCB2Form3.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 411f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form3.fld_CP38ReceiptNo == null ? "" : pCB2Form3.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 411f, 0);
                        cb.EndText();

                        if (pCB2Form3.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form3.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 411f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form3.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form3.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 411f, 0);
                            cb.EndText();
                        }
                    }

                    var pCB4 = pkjPcbContribution.Where(x => x.fld_Month == 4).FirstOrDefault();
                    var pkjSpecialInc4 = pkjSpecialInc.Where(x => x.fld_Month == 4).FirstOrDefault();
                    var pCB2Form4 = pkjPcbForm2.Where(x => x.fld_Month == 4).FirstOrDefault();

                    if (pCB4 != null)
                    {
                        pCB4.fld_CarumanPekerja = (pkjSpecialInc4 != null ? pkjSpecialInc4.fld_PCBCarumanPekerja : 0) + pCB4.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB4.fld_CarumanPekerja.ToString() == "0.00" || pCB4.fld_CarumanPekerja <= 10 ? "" : pCB4.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 398f, 0);
                        cb.EndText();
                    }

                    if (pCB2Form4 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form4.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form4.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 398f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form4.fld_PCBReceiptNo == null ? "" : pCB2Form4.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 398f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form4.fld_CP38ReceiptNo == null ? "" : pCB2Form4.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 398f, 0);
                        cb.EndText();

                        if (pCB2Form4.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form4.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 398f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form4.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form4.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 398f, 0);
                            cb.EndText();
                        }
                    }

                    var pCB5 = pkjPcbContribution.Where(x => x.fld_Month == 5).FirstOrDefault();
                    var pkjSpecialInc5 = pkjSpecialInc.Where(x => x.fld_Month == 5).FirstOrDefault();
                    var pCB2Form5 = pkjPcbForm2.Where(x => x.fld_Month == 5).FirstOrDefault();

                    if (pCB5 != null)
                    {
                        pCB5.fld_CarumanPekerja = (pkjSpecialInc5 != null ? pkjSpecialInc5.fld_PCBCarumanPekerja : 0) + pCB5.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB5.fld_CarumanPekerja.ToString() == "0.00" || pCB5.fld_CarumanPekerja <= 10 ? "" : pCB5.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 385f, 0);
                        cb.EndText();
                    }

                    if (pCB2Form5 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form5.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form5.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 385f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form5.fld_PCBReceiptNo == null ? "" : pCB2Form5.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 385f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form5.fld_CP38ReceiptNo == null ? "" : pCB2Form5.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 385f, 0);
                        cb.EndText();

                        if (pCB2Form5.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form5.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 385f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form5.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form5.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 385f, 0);
                            cb.EndText();
                        }
                    }

                    var pCB6 = pkjPcbContribution.Where(x => x.fld_Month == 6).FirstOrDefault();
                    var pkjSpecialInc6 = pkjSpecialInc.Where(x => x.fld_Month == 6).FirstOrDefault();
                    var pCB2Form6 = pkjPcbForm2.Where(x => x.fld_Month == 6).FirstOrDefault();

                    if (pCB6 != null)
                    {
                        pCB6.fld_CarumanPekerja = (pkjSpecialInc6 != null ? pkjSpecialInc6.fld_PCBCarumanPekerja : 0) + pCB6.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB6.fld_CarumanPekerja.ToString() == "0.00" || pCB6.fld_CarumanPekerja <= 10 ? "" : pCB6.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 372f, 0);
                        cb.EndText();
                    }

                    if (pCB2Form6 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form6.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form6.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 372f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form6.fld_PCBReceiptNo == null ? "" : pCB2Form6.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 372f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form6.fld_CP38ReceiptNo == null ? "" : pCB2Form6.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 372f, 0);
                        cb.EndText();

                        if (pCB2Form6.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form6.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 372f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form6.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form6.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 372f, 0);
                            cb.EndText();
                        }
                    }

                    var pCB7 = pkjPcbContribution.Where(x => x.fld_Month == 7).FirstOrDefault();
                    var pkjSpecialInc7 = pkjSpecialInc.Where(x => x.fld_Month == 7).FirstOrDefault();
                    var pCB2Form7 = pkjPcbForm2.Where(x => x.fld_Month == 7).FirstOrDefault();

                    if (pCB7 != null)
                    {
                        pCB7.fld_CarumanPekerja = (pkjSpecialInc7 != null ? pkjSpecialInc7.fld_PCBCarumanPekerja : 0) + pCB7.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB7.fld_CarumanPekerja.ToString() == "0.00" || pCB7.fld_CarumanPekerja <= 10 ? "" : pCB7.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 359f, 0);
                        cb.EndText();
                    }

                    if (pCB2Form7 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form7.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form7.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 359f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form7.fld_PCBReceiptNo == null ? "" : pCB2Form7.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 359f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form7.fld_CP38ReceiptNo == null ? "" : pCB2Form7.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 359f, 0);
                        cb.EndText();

                        if (pCB2Form7.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form7.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 359f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form7.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form7.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 359f, 0);
                            cb.EndText();
                        }
                    }

                    var pCB8 = pkjPcbContribution.Where(x => x.fld_Month == 8).FirstOrDefault();
                    var pkjSpecialInc8 = pkjSpecialInc.Where(x => x.fld_Month == 8).FirstOrDefault();
                    var pCB2Form8 = pkjPcbForm2.Where(x => x.fld_Month == 8).FirstOrDefault();

                    if (pCB8 != null)
                    {
                        pCB8.fld_CarumanPekerja = (pkjSpecialInc8 != null ? pkjSpecialInc8.fld_PCBCarumanPekerja : 0) + pCB8.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB8.fld_CarumanPekerja.ToString() == "0.00" || pCB8.fld_CarumanPekerja <= 10 ? "" : pCB8.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 346f, 0);
                        cb.EndText();
                    }

                    if (pCB2Form8 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form8.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form8.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 346f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form8.fld_PCBReceiptNo == null ? "" : pCB2Form8.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 346f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form8.fld_CP38ReceiptNo == null ? "" : pCB2Form8.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 346f, 0);
                        cb.EndText();

                        if (pCB2Form8.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form8.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 346f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form8.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form8.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 346f, 0);
                            cb.EndText();
                        }
                    }

                    var pCB9 = pkjPcbContribution.Where(x => x.fld_Month == 9).FirstOrDefault();
                    var pkjSpecialInc9 = pkjSpecialInc.Where(x => x.fld_Month == 9).FirstOrDefault();
                    var pCB2Form9 = pkjPcbForm2.Where(x => x.fld_Month == 9).FirstOrDefault();

                    if (pCB9 != null)
                    {
                        pCB9.fld_CarumanPekerja = (pkjSpecialInc9 != null ? pkjSpecialInc9.fld_PCBCarumanPekerja : 0) + pCB9.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB9.fld_CarumanPekerja.ToString() == "0.00" || pCB9.fld_CarumanPekerja <= 10 ? "" : pCB9.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 333f, 0);
                        cb.EndText();
                    }

                    if (pCB2Form9 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form9.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form9.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 333f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form9.fld_PCBReceiptNo == null ? "" : pCB2Form9.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 333f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form9.fld_CP38ReceiptNo == null ? "" : pCB2Form9.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 333f, 0);
                        cb.EndText();

                        if (pCB2Form9.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form9.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 333f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form9.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form9.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 333f, 0);
                            cb.EndText();
                        }
                    }

                    var pCB10 = pkjPcbContribution.Where(x => x.fld_Month == 10).FirstOrDefault();
                    var pkjSpecialInc10 = pkjSpecialInc.Where(x => x.fld_Month == 10).FirstOrDefault();
                    var pCB2Form10 = pkjPcbForm2.Where(x => x.fld_Month == 10).FirstOrDefault();

                    if (pCB10 != null)
                    {
                        pCB10.fld_CarumanPekerja = (pkjSpecialInc10 != null ? pkjSpecialInc10.fld_PCBCarumanPekerja : 0) + pCB10.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB10.fld_CarumanPekerja.ToString() == "0.00" || pCB10.fld_CarumanPekerja <= 10 ? "" : pCB10.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 320f, 0);
                        cb.EndText();
                    }

                    if (pCB2Form10 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form10.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form10.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 320f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form10.fld_PCBReceiptNo == null ? "" : pCB2Form10.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 320f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form10.fld_CP38ReceiptNo == null ? "" : pCB2Form10.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 320f, 0);
                        cb.EndText();

                        if (pCB2Form10.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form10.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 320f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form10.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form10.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 320f, 0);
                            cb.EndText();
                        }
                    }

                    var pCB11 = pkjPcbContribution.Where(x => x.fld_Month == 11).FirstOrDefault();
                    var pkjSpecialInc11 = pkjSpecialInc.Where(x => x.fld_Month == 11).FirstOrDefault();
                    var pCB2Form11 = pkjPcbForm2.Where(x => x.fld_Month == 11).FirstOrDefault();

                    if (pCB11 != null)
                    {
                        pCB11.fld_CarumanPekerja = (pkjSpecialInc11 != null ? pkjSpecialInc11.fld_PCBCarumanPekerja : 0) + pCB11.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB11.fld_CarumanPekerja.ToString() == "0.00" || pCB11.fld_CarumanPekerja <= 10 ? "" : pCB11.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 307f, 0);
                        cb.EndText();
                    }

                    if (pCB2Form11 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form11.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form11.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 307f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form11.fld_PCBReceiptNo == null ? "" : pCB2Form11.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 307f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form11.fld_CP38ReceiptNo == null ? "" : pCB2Form11.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 307f, 0);
                        cb.EndText();

                        if (pCB2Form11.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form11.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 307f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form11.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form11.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 307f, 0);
                            cb.EndText();
                        }
                    }

                    var pCB12 = pkjPcbContribution.Where(x => x.fld_Month == 12).FirstOrDefault();
                    var pkjSpecialInc12 = pkjSpecialInc.Where(x => x.fld_Month == 12).FirstOrDefault();
                    var pCB2Form12 = pkjPcbForm2.Where(x => x.fld_Month == 12).FirstOrDefault();

                    if (pCB12 != null)
                    {
                        pCB12.fld_CarumanPekerja = (pkjSpecialInc12 != null ? pkjSpecialInc12.fld_PCBCarumanPekerja : 0) + pCB12.fld_CarumanPekerja;
                        cb.BeginText();
                        text = pCB12.fld_CarumanPekerja.ToString() == "0.00" || pCB12.fld_CarumanPekerja <= 10 ? "" : pCB12.fld_CarumanPekerja.ToString();
                        cb.ShowTextAligned(1, text, 180f, 294f, 0);
                        cb.EndText();
                    }

                    if (pCB2Form12 != null)
                    {
                        cb.BeginText();
                        text = pCB2Form12.fld_CP38Amount.ToString() == "0.00" ? "" : pCB2Form12.fld_CP38Amount.ToString();
                        cb.ShowTextAligned(1, text, 235f, 294f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form12.fld_PCBReceiptNo == null ? "" : pCB2Form12.fld_PCBReceiptNo;
                        cb.ShowTextAligned(1, text, 305f, 294f, 0);
                        cb.EndText();

                        cb.BeginText();
                        text = pCB2Form12.fld_CP38ReceiptNo == null ? "" : pCB2Form12.fld_CP38ReceiptNo;
                        cb.ShowTextAligned(1, text, 385f, 294f, 0);
                        cb.EndText();

                        if (pCB2Form12.fld_PCBReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form12.fld_PCBReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 455f, 294f, 0);
                            cb.EndText();
                        }

                        if (pCB2Form12.fld_CP38ReceiptDate != null)
                        {
                            cb.BeginText();
                            text = pCB2Form12.fld_CP38ReceiptDate.Value.ToString("dd/MM/yyyy");
                            cb.ShowTextAligned(1, text, 510f, 294f, 0);
                            cb.EndText();
                        }

                    }

                    var totalPCB = pkjPcbContribution.Sum(x => x.fld_CarumanPekerja) + pkjSpecialInc.Sum(x => x.fld_PCBCarumanPekerja);
                    var totalCP38 = pkjPcbForm2.Sum(x => x.fld_CP38Amount);

                    cb.BeginText();
                    text = totalPCB.ToString() == "0.00" || totalPCB <= 10 ? "" : totalPCB.ToString();
                    cb.ShowTextAligned(1, text, 180f, 279f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = totalCP38.ToString() == "0" ? "" : totalCP38.ToString();
                    cb.ShowTextAligned(1, text, 235f, 279f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = ladang.fld_Pengurus == null ? "" : ladang.fld_Pengurus;
                    cb.ShowTextAligned(0, text, 213f, 118f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = "Manager";
                    cb.ShowTextAligned(0, text, 213f, 102f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = ladang.fld_Tel == null ? "" : ladang.fld_Tel;
                    cb.ShowTextAligned(0, text, 213f, 86f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = ladang.fld_LdgName == null ? "" : ladang.fld_LdgName;
                    cb.ShowTextAligned(0, text, 213f, 70f, 0);
                    cb.EndText();

                    cb.BeginText();
                    text = ladang.fld_Adress == null ? "" : ladang.fld_Adress;
                    cb.ShowTextAligned(0, text, 213f, 54f, 0);
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