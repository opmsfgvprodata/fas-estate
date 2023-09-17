using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MVC_SYSTEM.CustomModels;
using MVC_SYSTEM.ModelSAPPUP;
using MVC_SYSTEM.OPMStoSAP_3;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.CorpNewModels;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class ClosingTransactionController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private GetIdentity getidentity = new GetIdentity();
        private GetTriager GetTriager = new GetTriager();
        private GetNSWL GetNSWL = new GetNSWL();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private errorlog geterror = new errorlog();
        private GetConfig GetConfig = new GetConfig();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetWilayah GetWilayah = new GetWilayah();
        private Connection Connection = new Connection();
        private CheckrollFunction EstateFunction = new CheckrollFunction();
        private GlobalFunction GlobalFunction = new GlobalFunction();
        //private MVC_SYSTEM_Models_SAPPUP SapModel = new MVC_SYSTEM_Models_SAPPUP();
        // GET: ClosingTransaction

        public ActionResult Index()
        {
            ViewBag.Maintenance = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.CloseTransactionSubList = sublist;
            ViewBag.ClosingTransaction = "class = active";
            ViewBag.CloseTransactionList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "closeTransaction" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            db.Dispose();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string CloseTransactionList, string CloseTransactionSubList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (CloseTransactionSubList != null)
            {
                return RedirectToAction(CloseTransactionSubList, "ClosingTransaction");
            }
            else
            {
                int maintenancelist = int.Parse(CloseTransactionList);
                var action = db.tblMenuLists.Where(x => x.fld_ID == maintenancelist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "ClosingTransaction");
            }
        }

        public JsonResult GetSubList(int ListID)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var findsub = db.tblMenuLists.Where(x => x.fld_ID == ListID).Select(s => s.fld_Sub).FirstOrDefault();
            List<SelectListItem> sublist = new List<SelectListItem>();
            if (findsub != null)
            {
                sublist = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == findsub && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_Val, Text = s.fld_Desc }), "Value", "Text").ToList();
            }
            db.Dispose();
            return Json(sublist);
        }

        public ActionResult CloseTransaction()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.ClosingTransaction = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> CloseOpen = new List<SelectListItem>();
            CloseOpen.Insert(0, (new SelectListItem { Text = "Tutup Urus Niaga", Value = "true" }));
            if (getidentity.HQAuth(User.Identity.Name))
            {
                CloseOpen.Insert(1, (new SelectListItem { Text = "Buka Urus Niaga", Value = "false" }));
            }

            ViewBag.CloseOpen = CloseOpen;

            //ViewBag.ProcessList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "gensalary" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc");

            dbr.Dispose();
            return View();
        }

        [HttpPost]
        public ActionResult CloseTransaction(int Month, int Year, bool CloseOpen)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            int? AuditTrailStatus = 0;
            int? DivisionID = 0;
            SendEmail SendEmailNotification = new SendEmail();
            ViewBag.ClosingTransaction = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string monthstring = Month.ToString();
            if (monthstring.Length == 1)
            {
                monthstring = "0" + monthstring;
            }
            var ClosingTransaction = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_DivisionID == DivisionID).FirstOrDefault();
            var CheckScTransSalary = dbr.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KodAktvt == "4000").Select(s => s.fld_Amt).FirstOrDefault();
            var CheckSkbReg = dbr.tbl_Skb.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Bulan == monthstring && x.fld_Tahun == Year && x.fld_DivisionID == DivisionID).FirstOrDefault();
            //add by faeza
            var CheckPermohonanWang = db.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month).FirstOrDefault();


            //financial request - pilot for 3 estates
            //if (LadangID == 1 || LadangID == 17 || LadangID == 30)
            //{

                if (ClosingTransaction != null)
                {
                    //if (CheckSkbReg.fld_NoSkb != null)
                    //{
                    //if (CheckSkbReg.fld_GajiBersih == CheckScTransSalary)
                    //{
                    if (ClosingTransaction.fld_Credit == ClosingTransaction.fld_Debit)
                    {
                        if (CloseOpen == true && ClosingTransaction.fld_StsTtpUrsNiaga == true)
                        {
                            msg = "Urus niaga telah ditutup";
                            statusmsg = "warning";
                        }
                        else
                        {
                            if (CloseOpen == false)
                            {
                                if ((CheckPermohonanWang.fld_SemakWil_Status == 1 && CheckPermohonanWang.fld_SokongWilGM_Status == 1 && CheckPermohonanWang.fld_TerimaHQ_Status == 1) && (CheckPermohonanWang.fld_TolakWil_Status == 0 || CheckPermohonanWang.fld_TolakWilGM_Status == 0 || CheckPermohonanWang.fld_TolakHQ_Status == 0))
                                {
                                    msg = "Urus niaga tidak boleh dibuka. Maklumkan kepada HQ untuk membatalkan permohonan wang.";
                                    statusmsg = "warning";
                                }
                                else
                                {
                                    AuditTrailStatus = CloseOpen == true ? 1 : 0;
                                    ClosingTransaction.fld_StsTtpUrsNiaga = CloseOpen;
                                    ClosingTransaction.fld_ModifiedDT = timezone.gettimezone();
                                    ClosingTransaction.fld_ModifiedBy = getuserid;
                                    dbr.Entry(ClosingTransaction).State = EntityState.Modified;
                                    dbr.SaveChanges();
                                    UpdateAuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, DivisionID, Year, Month, AuditTrailStatus);
                                    //FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, CheckSkbReg.fld_GajiBersih, CheckSkbReg.fld_NoSkb, getuserid);
                                    //modified by faeza
                                    FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, getuserid);
                                    try
                                    {
                                        var GetEstate = db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID, s.fld_CostCentre }).FirstOrDefault();
                                        string Subject = "Tutup Urus Niaga";
                                        string Message = "";
                                        string Department = "";
                                        string[] cc = new string[] { };
                                        List<string> cclist = new List<string>();
                                        string[] bcc = new string[] { };
                                        List<string> bcclist = new List<string>();


                                        Message = "<html>";
                                        Message += "<body>";
                                        Message += "<p>Assalamualaikum,</p>";
                                        Message += "<p>Urus niaga telah ditutup. Keterangan seperti dibawah:-</p>";
                                        Message += "<table border=\"1\">";
                                        Message += "<thead>";
                                        Message += "<tr>";
                                        Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th>";
                                        Message += "</tr>";
                                        Message += "</thead>";
                                        Message += "<tbody>";
                                        Message += "<tr>";
                                        Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + monthstring + "</td><td align=\"center\">" + Year + "</td><td align=\"center\">" + ClosingTransaction.fld_Credit + "</td>";
                                        Message += "</tr>";
                                        Message += "</tbody>";
                                        Message += "</table>";
                                        Message += "<p>Data sedia untuk dihantar ke SAP.</p>";
                                        Message += "<p>Terima Kasih.</p>";
                                        Message += "</body>";
                                        Message += "</html>";

                                        cclist.Add(GetEstate.fld_SyarikatEmail);
                                        cclist.Add(GetEstate.fld_LdgEmail);
                                        cc = cclist.ToArray();
                                        if (GetEstate.fld_CostCentre == "FASSB")
                                        {
                                            Department = "HQ_FINANCE_APPROVAL_FASSB";
                                        }
                                        else
                                        {
                                            Department = "HQ_FINANCE_APPROVAL_RNDSB";
                                        }
                                        //var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == Department && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
                                        //SendEmailNotification.SendEmailDetail(Subject, Message, ToEmail.fldEmail, cc, bcc);
                                    }
                                    catch (Exception ex)
                                    {
                                        msg = ", Maaf masalah penghantaran email";
                                        statusmsg = "warning";
                                        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                                    }
                                    msg = GlobalResEstate.msgUpdate + msg;
                                    statusmsg = "success";
                                }
                            }
                            else
                            {
                                AuditTrailStatus = CloseOpen == true ? 1 : 0;
                                ClosingTransaction.fld_StsTtpUrsNiaga = CloseOpen;
                                ClosingTransaction.fld_ModifiedDT = timezone.gettimezone();
                                ClosingTransaction.fld_ModifiedBy = getuserid;
                                dbr.Entry(ClosingTransaction).State = EntityState.Modified;
                                dbr.SaveChanges();
                                UpdateAuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, DivisionID, Year, Month, AuditTrailStatus);
                                //FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, CheckSkbReg.fld_GajiBersih, CheckSkbReg.fld_NoSkb, getuserid);
                                //modified by faeza
                                FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, getuserid);

                                try
                                {
                                    var GetEstate = db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID, s.fld_CostCentre }).FirstOrDefault();
                                    string Subject = "Tutup Urus Niaga";
                                    string Message = "";
                                    string Department = "";
                                    string[] cc = new string[] { };
                                    List<string> cclist = new List<string>();
                                    string[] bcc = new string[] { };
                                    List<string> bcclist = new List<string>();


                                    Message = "<html>";
                                    Message += "<body>";
                                    Message += "<p>Assalamualaikum,</p>";
                                    Message += "<p>Urus niaga telah ditutup. Keterangan seperti dibawah:-</p>";
                                    Message += "<table border=\"1\">";
                                    Message += "<thead>";
                                    Message += "<tr>";
                                    Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th>";
                                    Message += "</tr>";
                                    Message += "</thead>";
                                    Message += "<tbody>";
                                    Message += "<tr>";
                                    Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + monthstring + "</td><td align=\"center\">" + Year + "</td><td align=\"center\">" + ClosingTransaction.fld_Credit + "</td>";
                                    Message += "</tr>";
                                    Message += "</tbody>";
                                    Message += "</table>";
                                    Message += "<p>Data sedia untuk dihantar ke SAP.</p>";
                                    Message += "<p>Terima Kasih.</p>";
                                    Message += "</body>";
                                    Message += "</html>";

                                    cclist.Add(GetEstate.fld_SyarikatEmail);
                                    cclist.Add(GetEstate.fld_LdgEmail);
                                    cc = cclist.ToArray();
                                    if (GetEstate.fld_CostCentre == "FASSB")
                                    {
                                        Department = "HQ_FINANCE_APPROVAL_FASSB";
                                    }
                                    else
                                    {
                                        Department = "HQ_FINANCE_APPROVAL_RNDSB";
                                    }
                                    //var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == Department && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
                                    //SendEmailNotification.SendEmailDetail(Subject, Message, ToEmail.fldEmail, cc, bcc);
                                }
                                catch (Exception ex)
                                {
                                    msg = ", Maaf masalah penghantaran email";
                                    statusmsg = "warning";
                                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                                }
                                msg = GlobalResEstate.msgUpdate + msg;
                                statusmsg = "success";
                            }

                        }

                    }
                    else
                    {
                        msg = GlobalResEstate.msgBalance;
                        statusmsg = "warning";
                    }
                    //}
                    //else
                    //{
                    //    msg = "Sila pastikan nilai pemohonan sama seperti didaftar di No SKB sebelum urusniaga ditutup";
                    //    statusmsg = "warning";
                    //}

                    //}
                    //else
                    //{
                    //    msg = "Sila daftar No SKB sebelum urusniaga ditutup";
                    //    statusmsg = "warning";
                    //}
                }
                else
                {
                    msg = GlobalResEstate.msgGenSalary;
                    statusmsg = "warning";
                }
            //}
            //ladang selain 3 ladang yg pilot financial req
            //else
            //{

            //    if (ClosingTransaction != null)
            //    {
            //        //if (CheckSkbReg.fld_NoSkb != null)
            //        //{
            //        //if (CheckSkbReg.fld_GajiBersih == CheckScTransSalary)
            //        //{
            //        if (ClosingTransaction.fld_Credit == ClosingTransaction.fld_Debit)
            //        {
            //            if (CloseOpen == true && ClosingTransaction.fld_StsTtpUrsNiaga == true)
            //            {
            //                msg = "Urus niaga telah ditutup";
            //                statusmsg = "warning";
            //            }
            //            else
            //            {                            
            //                if (CloseOpen == false)
            //                {                                
            //                        AuditTrailStatus = CloseOpen == true ? 1 : 0;
            //                        ClosingTransaction.fld_StsTtpUrsNiaga = CloseOpen;
            //                        ClosingTransaction.fld_ModifiedDT = timezone.gettimezone();
            //                        ClosingTransaction.fld_ModifiedBy = getuserid;
            //                        dbr.Entry(ClosingTransaction).State = EntityState.Modified;
            //                        dbr.SaveChanges();
            //                        UpdateAuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, DivisionID, Year, Month, AuditTrailStatus);
            //                        //FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, CheckSkbReg.fld_GajiBersih, CheckSkbReg.fld_NoSkb, getuserid);
            //                        //modified by faeza
            //                        //FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, getuserid);
            //                        try
            //                        {
            //                            var GetEstate = db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID, s.fld_CostCentre }).FirstOrDefault();
            //                            string Subject = "Tutup Urus Niaga";
            //                            string Message = "";
            //                            string Department = "";
            //                            string[] cc = new string[] { };
            //                            List<string> cclist = new List<string>();
            //                            string[] bcc = new string[] { };
            //                            List<string> bcclist = new List<string>();


            //                            Message = "<html>";
            //                            Message += "<body>";
            //                            Message += "<p>Assalamualaikum,</p>";
            //                            Message += "<p>Urus niaga telah ditutup. Keterangan seperti dibawah:-</p>";
            //                            Message += "<table border=\"1\">";
            //                            Message += "<thead>";
            //                            Message += "<tr>";
            //                            Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th>";
            //                            Message += "</tr>";
            //                            Message += "</thead>";
            //                            Message += "<tbody>";
            //                            Message += "<tr>";
            //                            Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + monthstring + "</td><td align=\"center\">" + Year + "</td><td align=\"center\">" + ClosingTransaction.fld_Credit + "</td>";
            //                            Message += "</tr>";
            //                            Message += "</tbody>";
            //                            Message += "</table>";
            //                            Message += "<p>Data sedia untuk dihantar ke SAP.</p>";
            //                            Message += "<p>Terima Kasih.</p>";
            //                            Message += "</body>";
            //                            Message += "</html>";

            //                            cclist.Add(GetEstate.fld_SyarikatEmail);
            //                            cclist.Add(GetEstate.fld_LdgEmail);
            //                            cc = cclist.ToArray();
            //                            if (GetEstate.fld_CostCentre == "FASSB")
            //                            {
            //                                Department = "HQ_FINANCE_APPROVAL_FASSB";
            //                            }
            //                            else
            //                            {
            //                                Department = "HQ_FINANCE_APPROVAL_RNDSB";
            //                            }
            //                            //var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == Department && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
            //                            //SendEmailNotification.SendEmailDetail(Subject, Message, ToEmail.fldEmail, cc, bcc);
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            msg = ", Maaf masalah penghantaran email";
            //                            statusmsg = "warning";
            //                            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            //                        }
            //                        msg = GlobalResEstate.msgUpdate + msg;
            //                        statusmsg = "success";
                                
            //                }
            //                else
            //                {
            //                    AuditTrailStatus = CloseOpen == true ? 1 : 0;
            //                    ClosingTransaction.fld_StsTtpUrsNiaga = CloseOpen;
            //                    ClosingTransaction.fld_ModifiedDT = timezone.gettimezone();
            //                    ClosingTransaction.fld_ModifiedBy = getuserid;
            //                    dbr.Entry(ClosingTransaction).State = EntityState.Modified;
            //                    dbr.SaveChanges();
            //                    UpdateAuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, DivisionID, Year, Month, AuditTrailStatus);
            //                    //FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, CheckSkbReg.fld_GajiBersih, CheckSkbReg.fld_NoSkb, getuserid);
            //                    //modified by faeza
            //                    //FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, getuserid);

            //                    try
            //                    {
            //                        var GetEstate = db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID, s.fld_CostCentre }).FirstOrDefault();
            //                        string Subject = "Tutup Urus Niaga";
            //                        string Message = "";
            //                        string Department = "";
            //                        string[] cc = new string[] { };
            //                        List<string> cclist = new List<string>();
            //                        string[] bcc = new string[] { };
            //                        List<string> bcclist = new List<string>();


            //                        Message = "<html>";
            //                        Message += "<body>";
            //                        Message += "<p>Assalamualaikum,</p>";
            //                        Message += "<p>Urus niaga telah ditutup. Keterangan seperti dibawah:-</p>";
            //                        Message += "<table border=\"1\">";
            //                        Message += "<thead>";
            //                        Message += "<tr>";
            //                        Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th>";
            //                        Message += "</tr>";
            //                        Message += "</thead>";
            //                        Message += "<tbody>";
            //                        Message += "<tr>";
            //                        Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + monthstring + "</td><td align=\"center\">" + Year + "</td><td align=\"center\">" + ClosingTransaction.fld_Credit + "</td>";
            //                        Message += "</tr>";
            //                        Message += "</tbody>";
            //                        Message += "</table>";
            //                        Message += "<p>Data sedia untuk dihantar ke SAP.</p>";
            //                        Message += "<p>Terima Kasih.</p>";
            //                        Message += "</body>";
            //                        Message += "</html>";

            //                        cclist.Add(GetEstate.fld_SyarikatEmail);
            //                        cclist.Add(GetEstate.fld_LdgEmail);
            //                        cc = cclist.ToArray();
            //                        if (GetEstate.fld_CostCentre == "FASSB")
            //                        {
            //                            Department = "HQ_FINANCE_APPROVAL_FASSB";
            //                        }
            //                        else
            //                        {
            //                            Department = "HQ_FINANCE_APPROVAL_RNDSB";
            //                        }
            //                        //var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == Department && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
            //                        //SendEmailNotification.SendEmailDetail(Subject, Message, ToEmail.fldEmail, cc, bcc);
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        msg = ", Maaf masalah penghantaran email";
            //                        statusmsg = "warning";
            //                        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            //                    }
            //                    msg = GlobalResEstate.msgUpdate + msg;
            //                    statusmsg = "success";
            //                }
            //            }

            //        }
            //        else
            //        {
            //            msg = GlobalResEstate.msgBalance;
            //            statusmsg = "warning";
            //        }
            //        //}
            //        //else
            //        //{
            //        //    msg = "Sila pastikan nilai pemohonan sama seperti didaftar di No SKB sebelum urusniaga ditutup";
            //        //    statusmsg = "warning";
            //        //}

            //        //}
            //        //else
            //        //{
            //        //    msg = "Sila daftar No SKB sebelum urusniaga ditutup";
            //        //    statusmsg = "warning";
            //        //}
            //    }
            //    else
            //    {
            //        msg = GlobalResEstate.msgGenSalary;
            //        statusmsg = "warning";
            //    }
            //}



            //original code - commented by faeza 24.02.2021
            //if (ClosingTransaction != null)
            //{
            //    //if (CheckSkbReg.fld_NoSkb != null)
            //    //{
            //    //if (CheckSkbReg.fld_GajiBersih == CheckScTransSalary)
            //    //{
            //    if (ClosingTransaction.fld_Credit == ClosingTransaction.fld_Debit)
            //    {
            //        if (CloseOpen == true && ClosingTransaction.fld_StsTtpUrsNiaga == true)
            //        {
            //            msg = "Urus niaga telah ditutup";
            //            statusmsg = "warning";
            //        }
            //        else
            //        {
            //            AuditTrailStatus = CloseOpen == true ? 1 : 0;
            //            ClosingTransaction.fld_StsTtpUrsNiaga = CloseOpen;
            //            ClosingTransaction.fld_ModifiedDT = timezone.gettimezone();
            //            ClosingTransaction.fld_ModifiedBy = getuserid;
            //            dbr.Entry(ClosingTransaction).State = EntityState.Modified;
            //            dbr.SaveChanges();
            //            UpdateAuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, DivisionID, Year, Month, AuditTrailStatus);
            //            //FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, CheckSkbReg.fld_GajiBersih, CheckSkbReg.fld_NoSkb, getuserid);
            //            try
            //            {
            //                var GetEstate = db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID, s.fld_CostCentre }).FirstOrDefault();
            //                string Subject = "Tutup Urus Niaga";
            //                string Message = "";
            //                string Department = "";
            //                string[] cc = new string[] { };
            //                List<string> cclist = new List<string>();
            //                string[] bcc = new string[] { };
            //                List<string> bcclist = new List<string>();


            //                Message = "<html>";
            //                Message += "<body>";
            //                Message += "<p>Assalamualaikum,</p>";
            //                Message += "<p>Urus niaga telah ditutup. Keterangan seperti dibawah:-</p>";
            //                Message += "<table border=\"1\">";
            //                Message += "<thead>";
            //                Message += "<tr>";
            //                Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th>";
            //                Message += "</tr>";
            //                Message += "</thead>";
            //                Message += "<tbody>";
            //                Message += "<tr>";
            //                Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + monthstring + "</td><td align=\"center\">" + Year + "</td><td align=\"center\">" + ClosingTransaction.fld_Credit + "</td>";
            //                Message += "</tr>";
            //                Message += "</tbody>";
            //                Message += "</table>";
            //                Message += "<p>Data sedia untuk dihantar ke SAP.</p>";
            //                Message += "<p>Terima Kasih.</p>";
            //                Message += "</body>";
            //                Message += "</html>";

            //                cclist.Add(GetEstate.fld_SyarikatEmail);
            //                cclist.Add(GetEstate.fld_LdgEmail);
            //                cc = cclist.ToArray();
            //                if (GetEstate.fld_CostCentre == "FASSB")
            //                {
            //                    Department = "HQ_FINANCE_APPROVAL_FASSB";
            //                }
            //                else
            //                {
            //                    Department = "HQ_FINANCE_APPROVAL_RNDSB";
            //                }
            //                var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == Department && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
            //                SendEmailNotification.SendEmailDetail(Subject, Message, ToEmail.fldEmail, cc, bcc);
            //            }
            //            catch (Exception ex)
            //            {
            //                msg = ", Maaf masalah penghantaran email";
            //                statusmsg = "warning";
            //                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            //            }
            //            msg = GlobalResEstate.msgUpdate + msg;
            //            statusmsg = "success";
            //        }

            //    }
            //    else
            //    {
            //        msg = GlobalResEstate.msgBalance;
            //        statusmsg = "warning";
            //    }
            //    //}
            //    //else
            //    //{
            //    //    msg = "Sila pastikan nilai pemohonan sama seperti didaftar di No SKB sebelum urusniaga ditutup";
            //    //    statusmsg = "warning";
            //    //}

            //    //}
            //    //else
            //    //{
            //    //    msg = "Sila daftar No SKB sebelum urusniaga ditutup";
            //    //    statusmsg = "warning";
            //    //}
            //}
            //else
            //{
            //    msg = GlobalResEstate.msgGenSalary;
            //    statusmsg = "warning";
            //}

            dbr.Dispose();
            return Json(new { msg, statusmsg });
        }

        public ActionResult AuditTrail()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? DivisionID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;
            //List<SelectListItem> SelectionData = new List<SelectListItem>();

            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();

            var GetAuditTrail = db.tbl_AuditTrail.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID && x.fld_Thn == year).FirstOrDefault();

            ViewBag.YearList = yearlist;
            ViewBag.Tahun = year;
            return View("AuditTrail", GetAuditTrail);
        }

        [HttpPost]
        public ActionResult AuditTrail(int YearList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;
            //List<SelectListItem> SelectionData = new List<SelectListItem>();

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == YearList)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();

            var GetAuditTrail = db.tbl_AuditTrail.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID && x.fld_Thn == YearList).FirstOrDefault();

            ViewBag.YearList = yearlist;
            ViewBag.Tahun = YearList;
            return View("AuditTrail", GetAuditTrail);
        }

        public void UpdateAuditTrail(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? DivisionID, int? Year, int? Month, int? UpdateData)
        {
            var checkAuditTrail = db.tbl_AuditTrail.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID && x.fld_Thn == Year).FirstOrDefault();
            switch (Month)
            {
                case 1:
                    checkAuditTrail.fld_Bln1 = UpdateData;
                    break;
                case 2:
                    checkAuditTrail.fld_Bln2 = UpdateData;
                    break;
                case 3:
                    checkAuditTrail.fld_Bln3 = UpdateData;
                    break;
                case 4:
                    checkAuditTrail.fld_Bln4 = UpdateData;
                    break;
                case 5:
                    checkAuditTrail.fld_Bln5 = UpdateData;
                    break;
                case 6:
                    checkAuditTrail.fld_Bln6 = UpdateData;
                    break;
                case 7:
                    checkAuditTrail.fld_Bln7 = UpdateData;
                    break;
                case 8:
                    checkAuditTrail.fld_Bln8 = UpdateData;
                    break;
                case 9:
                    checkAuditTrail.fld_Bln9 = UpdateData;
                    break;
                case 10:
                    checkAuditTrail.fld_Bln10 = UpdateData;
                    break;
                case 11:
                    checkAuditTrail.fld_Bln11 = UpdateData;
                    break;
                case 12:
                    checkAuditTrail.fld_Bln12 = UpdateData;
                    break;
            }

            db.Entry(checkAuditTrail).State = EntityState.Modified;
            db.SaveChanges();
        }

        //modified by faeza 24.02.2021
        //public void FinanceApplication(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? DivisionID, int? Year, int? Month, bool? UrusniagaStatus, decimal? Amount, string SkbNo, int? UserID)
        public void FinanceApplication(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Year, int? Month, bool? UrusniagaStatus, int? UserID)
        {
            var CheckPermohonanWang = db.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month).FirstOrDefault();
            var GetLadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();
            if (CheckPermohonanWang == null)
            {
                tbl_SokPermhnWang tbl_SokPermhnWang = new tbl_SokPermhnWang();
                tbl_SokPermhnWang.fld_SemakWil_Status = 0;
                tbl_SokPermhnWang.fld_SokongWilGM_Status = 0;
                tbl_SokPermhnWang.fld_TerimaHQ_Status = 0;
                tbl_SokPermhnWang.fld_TolakWil_Status = 0;
                tbl_SokPermhnWang.fld_TolakWilGM_Status = 0;
                tbl_SokPermhnWang.fld_TolakHQ_Status = 0;
                tbl_SokPermhnWang.fld_NoCIT = GetLadangDetail.fld_NoCIT;
                tbl_SokPermhnWang.fld_NoAcc = GetLadangDetail.fld_NoAcc;
                tbl_SokPermhnWang.fld_NoGL = GetLadangDetail.fld_NoGL;
                //tbl_SokPermhnWang.fld_JumlahPermohonan = Amount;
                tbl_SokPermhnWang.fld_JumlahPermohonan = 0;
                tbl_SokPermhnWang.fld_JumlahWorkerNet = 0; //added by faeza 30.09.2021
                tbl_SokPermhnWang.fld_JumlahCash = 0; //added by faeza 30.09.2021
                tbl_SokPermhnWang.fld_JumlahCheque = 0; //added by faeza 30.09.2021
                tbl_SokPermhnWang.fld_JumlahEwallet = 0; //added by faeza 30.09.2021
                tbl_SokPermhnWang.fld_JumlahCdmas = 0; //added by faeza 30.09.2021
                tbl_SokPermhnWang.fld_JumlahLain = 0; //added by faeza 30.09.2021
                //tbl_SokPermhnWang.fld_SkbNo = SkbNo;
                tbl_SokPermhnWang.fld_StsTtpUrsNiaga = true;
                tbl_SokPermhnWang.fld_NegaraID = NegaraID;
                tbl_SokPermhnWang.fld_SyarikatID = SyarikatID;
                tbl_SokPermhnWang.fld_WilayahID = WilayahID;
                tbl_SokPermhnWang.fld_LadangID = LadangID;
                tbl_SokPermhnWang.fld_Year = Year;
                tbl_SokPermhnWang.fld_Month = Month;
                db.tbl_SokPermhnWang.Add(tbl_SokPermhnWang);
                db.SaveChanges();
            }
            else
            {
                CheckPermohonanWang.fld_SemakWil_Status = 0;
                CheckPermohonanWang.fld_SokongWilGM_Status = 0;
                CheckPermohonanWang.fld_TerimaHQ_Status = 0;
                CheckPermohonanWang.fld_TolakWil_Status = 0;
                CheckPermohonanWang.fld_TolakWilGM_Status = 0;
                CheckPermohonanWang.fld_TolakHQ_Status = 0;
                CheckPermohonanWang.fld_NoCIT = GetLadangDetail.fld_NoCIT;
                CheckPermohonanWang.fld_NoAcc = GetLadangDetail.fld_NoAcc;
                CheckPermohonanWang.fld_NoGL = GetLadangDetail.fld_NoGL;
                CheckPermohonanWang.fld_JumlahPermohonan = 0;
                CheckPermohonanWang.fld_JumlahWorkerNet = 0; //added by faeza 30.09.2021
                CheckPermohonanWang.fld_JumlahCash = 0; //added by faeza 30.09.2021
                CheckPermohonanWang.fld_JumlahCheque = 0; //added by faeza 30.09.2021
                CheckPermohonanWang.fld_JumlahEwallet = 0; //added by faeza 30.09.2021
                CheckPermohonanWang.fld_JumlahCdmas = 0; //added by faeza 30.09.2021
                CheckPermohonanWang.fld_JumlahLain = 0; //added by faeza 30.09.2021
                //CheckPermohonanWang.fld_SkbNo = SkbNo;
                CheckPermohonanWang.fld_StsTtpUrsNiaga = UrusniagaStatus;
                db.Entry(CheckPermohonanWang).State = EntityState.Modified;
                db.SaveChanges();

                if (UrusniagaStatus == false)
                {
                    tblSokPermhnWangHisAction tblSokPermhnWangHisAction = new tblSokPermhnWangHisAction();
                    tblSokPermhnWangHisAction.fldHisSPWID = CheckPermohonanWang.fld_ID;
                    tblSokPermhnWangHisAction.fldHisDesc = "Urus Niaga Dibuka Semula";
                    tblSokPermhnWangHisAction.fldHisUserID = UserID;
                    //tblSokPermhnWangHisAction.fldHisAppLevel = 2; //commented by faeza 30.09.2021
                    tblSokPermhnWangHisAction.fldHisAppLevel = 0; //added by faeza 30.09.2021
                    tblSokPermhnWangHisAction.fldHisDT = timezone.gettimezone();
                    db.tblSokPermhnWangHisActions.Add(tblSokPermhnWangHisAction);
                    db.SaveChanges();
                }
            }
        }

        public ActionResult PostingSAP(string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);

            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            ViewBag.YearList = yearlist;
            ViewBag.ClosingTransaction = "class = active";

            return View();
        }

        public ActionResult _PostingSAP(int? MonthList, int? YearList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models_SAPPUP SapModel = MVC_SYSTEM_Models_SAPPUP.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_CorpNewModels CorpNewModels = new MVC_SYSTEM_CorpNewModels();
            MVC_SYSTEM_MasterModels MasterModel = new MVC_SYSTEM_MasterModels();

            var message = "";

            var postingData = new List<vw_SAPPostDataFullDetails>();

            if (!String.IsNullOrEmpty(MonthList.ToString()) && !String.IsNullOrEmpty(YearList.ToString()))
            {
                postingData = SapModel.vw_SAPPostDataFullDetails
                    .Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID).ToList();

                var ClosingTransaction = SapModel.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == MonthList && x.fld_Year == YearList && x.fld_DivisionID == DivisionID).FirstOrDefault();
                ViewBag.ClosingStatus = ClosingTransaction.fld_StsTtpUrsNiaga;
                if (!postingData.Any())
                {
                    message = GlobalResEstate.msgErrorSearch;
                }
            }

            else
            {
                message = GlobalResEstate.msgChooseMonthYear;
            }

            ViewBag.Message = message;

            ViewBag.Existing = db.tbl_SokPermhnWang.Where(x => x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_LadangID == LadangID).Any();
            //ViewBag.GetApproval = CorpNewModels.tbl_SalaryRequest.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_ApproveStatus == true).Any();
            //ViewBag.AfterRequest = CorpNewModels.tbl_SalaryRequest.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_ReviewStatus == true).Any();
            ViewBag.GetTerimaHQ = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TerimaHQ_Status == 1).Any();
            ViewBag.GetTolakHQ = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakHQ_Status == 1).Any();
            ViewBag.GetTolakWilGM = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakWilGM_Status == 1).Any();
            ViewBag.GetTolakWil = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakWil_Status == 1).Any();
            ViewBag.GetJumPermohonan = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).Select(s => s.fld_JumlahPermohonan).FirstOrDefault();
            ViewBag.Year = YearList;
            ViewBag.Month = MonthList;
            ViewBag.LadangID = LadangID;
            return View(postingData);
        }

        public JsonResult GenerateRefNo(string docType, Guid SAPPostRefNoID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models_SAPPUP SapModel = MVC_SYSTEM_Models_SAPPUP.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var SAPPostRefData = SapModel.tbl_SAPPostRef.SingleOrDefault(x => x.fld_ID == SAPPostRefNoID);

                var postingMonthYear = SAPPostRefData.fld_PostingDate.Value.ToString("MMyy");

                //var sapPostingRefNo = "";

                var checkrollPostingCode = db.tblOptionConfigsWebs.SingleOrDefault(x =>
                    x.fldOptConfFlag1 == "sapPostingRefNo" &&
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false);

                var checkrollDocumentPostingCode = db.tblOptionConfigsWebs.SingleOrDefault(x =>
                    x.fldOptConfFlag1 == docType &&
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false);

                //var docPostingRunningNo = db.tbl_BatchRunNo
                //    .SingleOrDefault(x => x.fld_BatchFlag == "sapPostingRefNo" && x.fld_BatchFlag2 == docType &&
                //                          x.fld_NegaraID == NegaraID &&
                //                          x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                //                          x.fld_LadangID == LadangID && x.);

                var checkrollRefNo = "";

                checkrollRefNo = GlobalFunction.BatchNoSAPPostFunc(LadangID, checkrollPostingCode.fldOptConfValue + postingMonthYear + "-" + checkrollDocumentPostingCode.fldOptConfValue, "sapPostingRefNo", SAPPostRefData.fld_DocType, SAPPostRefData.fld_Month.Value, SAPPostRefData.fld_Year.Value);
                //checkrollPostingCode.fldOptConfValue + postingMonthYear + "-" +
                //checkrollDocumentPostingCode.fldOptConfValue + generateRunningNo;

                SAPPostRefData.fld_RefNo = checkrollRefNo;
                SAPPostRefData.fld_HeaderText = checkrollRefNo;
                SapModel.SaveChanges();

                //if (docPostingRunningNo != null)
                //{
                //    //current month and posting year already exist
                //    if (docPostingRunningNo.fld_BatchRunNo2 == SAPPostRefData.fld_PostingDate.Value.Month &&
                //        docPostingRunningNo.fld_BatchRunNo3 == SAPPostRefData.fld_PostingDate.Value.Year)
                //    {
                //        //var generateRunningNo = (docPostingRunningNo.fld_BatchRunNo.Value + 1).ToString("000");

                //        checkrollRefNo = GlobalFunction.BatchNoSAPPostFunc(LadangID, checkrollPostingCode.fldOptConfValue + postingMonthYear + "-" + checkrollDocumentPostingCode.fldOptConfValue, sapPostingRefNo, docType, SAPPostRefData.fld_Month.Value, SAPPostRefData.fld_Year.Value);
                //            //checkrollPostingCode.fldOptConfValue + postingMonthYear + "-" +
                //            //checkrollDocumentPostingCode.fldOptConfValue + generateRunningNo;

                //        SAPPostRefData.fld_RefNo = checkrollRefNo;
                //        SAPPostRefData.fld_HeaderText = checkrollRefNo;
                //        SapModel.SaveChanges();

                //        //docPostingRunningNo.fld_BatchRunNo = docPostingRunningNo.fld_BatchRunNo.Value + 1;
                //        //db.SaveChanges();
                //    }

                //    else
                //    {
                //        var generateRunningNo = 1.ToString("000");

                //        checkrollRefNo = 
                //                           //checkrollPostingCode.fldOptConfValue + postingMonthYear + "-" +
                //                         //checkrollDocumentPostingCode.fldOptConfValue + generateRunningNo;

                //        SAPPostRefData.fld_RefNo = checkrollRefNo;
                //        SAPPostRefData.fld_HeaderText = checkrollRefNo;
                //        SapModel.SaveChanges();

                //        //docPostingRunningNo.fld_BatchRunNo = 1;
                //        //docPostingRunningNo.fld_BatchRunNo2 = SAPPostRefData.fld_PostingDate.Value.Month;
                //        //docPostingRunningNo.fld_BatchRunNo3 = SAPPostRefData.fld_PostingDate.Value.Year;

                //        //db.SaveChanges();
                //    }
                //}

                //else
                //{
                //    tbl_BatchRunNo batchRunNo = new tbl_BatchRunNo();

                //    batchRunNo.fld_BatchRunNo = 1;
                //    batchRunNo.fld_BatchRunNo2 = SAPPostRefData.fld_PostingDate.Value.Month;
                //    batchRunNo.fld_BatchRunNo3 = SAPPostRefData.fld_PostingDate.Value.Year;
                //    batchRunNo.fld_BatchFlag = "sapPostingRefNo";
                //    batchRunNo.fld_BatchFlag2 = docType;
                //    batchRunNo.fld_NegaraID = SAPPostRefData.fld_NegaraID;
                //    batchRunNo.fld_SyarikatID = SAPPostRefData.fld_SyarikatID;
                //    batchRunNo.fld_WilayahID = SAPPostRefData.fld_WilayahID;
                //    batchRunNo.fld_LadangID = SAPPostRefData.fld_LadangID;

                //    db.tbl_BatchRunNo.Add(batchRunNo);
                //    db.SaveChanges();

                //    checkrollRefNo = checkrollPostingCode.fldOptConfValue + postingMonthYear + "-" +
                //                     checkrollDocumentPostingCode.fldOptConfValue + batchRunNo.fld_BatchRunNo.Value.ToString("000");

                //    SAPPostRefData.fld_RefNo = checkrollRefNo;
                //    SAPPostRefData.fld_HeaderText = checkrollRefNo;
                //    SapModel.SaveChanges();
                //}

                return Json(checkrollRefNo);
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json("Error");
            }

            finally
            {
                db.Dispose();
            }
        }

        public ActionResult _SAPCredentialLogin(string postGLToGL, string PostGLToVendor, string PostGLToCustomer)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 13;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            CustMod_SAPCredential sapCredential = new CustMod_SAPCredential();

            sapCredential.GLtoGLGuid = postGLToGL;
            sapCredential.GLtoGVendorGuid = PostGLToVendor;
            sapCredential.GLtoGCustomerGuid = PostGLToCustomer;

            return PartialView("_SAPCredentialLogin", sapCredential);
        }

        public JsonResult SapPostData(string userName, string password, Guid? postGLToGL, Guid? postGLToVendor, Guid? postGLToCustomer)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models_SAPPUP SapModel = MVC_SYSTEM_Models_SAPPUP.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                CustMod_ReturnJsonToView returnJsonToView = new CustMod_ReturnJsonToView();
                List<CustMod_ReturnJson> returnJsonList = new List<CustMod_ReturnJson>();

                var month = 0;
                var year = 0;

                try
                {
                    Guid sapPostRefID = new Guid();

                    try
                    {
                        var sapDocNo = "";
                        var sortCount = 0;

                        if (!String.IsNullOrEmpty(postGLToGL.ToString()))
                        {
                            BasicHttpBinding binding = new BasicHttpBinding();
                            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                            binding.MaxReceivedMessageSize = 2147483647;
                            NetworkCredential Cred = new NetworkCredential();

                            EndpointAddress endpoint = new EndpointAddress(
                                "http://sapfgp.fgv.felhqr.myfelda:8002/sap/bc/srt/rfc/sap/zws_acc_doc_post_fgvassb/840/zws_acc_doc_post_fgvassb/zws_acc_doc_post_fgvassb");

                            //EndpointAddress endpoint = new EndpointAddress(
                            //    "http://sapfgq.fgv.felhqr.myfelda:8001/sap/bc/srt/rfc/sap/zws_acc_doc_post_fgvassb/840/zws_acc_doc_post_fgvassb/zws_acc_doc_post_fgvassb?sap-client=840");

                            //for testing 02.08.2022
                            //EndpointAddress endpoint = new EndpointAddress("http://sapfgq.fgv.felhqr.myfelda:8001/sap/bc/srt/rfc/sap/zws_acc_doc_post_fgvassb/840/zws_acc_doc_post_fgvassb/zws_acc_doc_post_fgvassb");

                            ZWS_ACC_DOC_POST_FGVASSBClient SAPPosting = new ZWS_ACC_DOC_POST_FGVASSBClient(binding, endpoint);
                            ZFI_ACC_DOC_POST Request = new ZFI_ACC_DOC_POST();
                            ZFI_ACC_DOC_POSTResponse Response = new ZFI_ACC_DOC_POSTResponse();

                            Cred.UserName = userName;
                            Cred.Password = password;
                            SAPPosting.ClientCredentials.UserName.UserName = Cred.UserName;
                            SAPPosting.ClientCredentials.UserName.Password = Cred.Password;
                            SAPPosting.Open();

                            BAPIACHE09 GLToGLHeader = new BAPIACHE09();
                            List<BAPIACGL09> GLToGLList = new List<BAPIACGL09>();
                            List<BAPIACCR09> GLToGLAmountList = new List<BAPIACCR09>();
                            List<ModelSAPPUP.tbl_SAPPostReturn> sapPostReturnList = new List<ModelSAPPUP.tbl_SAPPostReturn>();
                            BAPIRET2 Return = new BAPIRET2();

                            Return.TYPE = null;
                            Return.ID = null;
                            Return.MESSAGE = null;
                            Return.NUMBER = null;
                            Return.LOG_NO = null;
                            Return.LOG_MSG_NO = null;
                            Return.MESSAGE_V1 = null;
                            Return.MESSAGE_V2 = null;
                            Return.MESSAGE_V3 = null;
                            Return.MESSAGE_V4 = null;
                            Return.PARAMETER = null;
                            Return.ROW = 0;
                            Return.FIELD = null;
                            Return.SYSTEM = null;

                            var GLToGLPostingData =
                                SapModel.vw_SAPPostDataFullDetails.Where(x => x.fld_SAPPostRefID == postGLToGL)
                                    .OrderBy(o => o.fld_ItemNo);

                            if (GLToGLPostingData.DistinctBy(x => x.fld_SAPPostRefID).Select(s => s.fld_StatusProceed).SingleOrDefault() == false)
                            {
                                foreach (var headerData in GLToGLPostingData.DistinctBy(x => x.fld_SAPPostRefID))
                                {
                                    GLToGLHeader.HEADER_TXT = headerData.fld_HeaderText;
                                    GLToGLHeader.USERNAME = userName;
                                    GLToGLHeader.COMP_CODE = headerData.fld_CompCode;
                                    GLToGLHeader.DOC_DATE = headerData.fld_DocDate.Value.ToString("yyyy-MM-dd");
                                    GLToGLHeader.PSTNG_DATE = headerData.fld_PostingDate.Value.ToString("yyyy-MM-dd");
                                    GLToGLHeader.DOC_TYPE = headerData.fld_DocType;
                                    GLToGLHeader.REF_DOC_NO = headerData.fld_RefNo;

                                    sapPostRefID = (Guid)headerData.fld_SAPPostRefID;

                                    year = (int)headerData.fld_Year;
                                    month = (int)headerData.fld_Month;
                                }
                                var GetNetworkNoData = GLToGLPostingData.Select(x => x.fld_Item).ToList();
                                var GetSAPPDData = db.tbl_SAPPDPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && GetNetworkNoData.Contains(x.fld_NetworkNo)).ToList();
                                foreach (var GLtoGLItem in GLToGLPostingData)
                                {
                                    var Desc = GLtoGLItem.fld_Desc.Replace("POTONGAN", "PTGN");
                                    Desc = Desc.Replace("BAYARAN", "BYRN");
                                    Desc = Desc.Replace("ELAUN", "ELN");
                                    if (!String.IsNullOrEmpty(GLtoGLItem.fld_SAPActivityCode))
                                    {
                                        BAPIACGL09 GLToGL = new BAPIACGL09();
                                        var GetCC = GetSAPPDData.Where(x => x.fld_NetworkNo == GLtoGLItem.fld_Item && x.fld_ActivityCode == GLtoGLItem.fld_SAPActivityCode).Select(s => s.fld_CostCenter).FirstOrDefault();
                                        GLToGL.ITEMNO_ACC = string.Format("{0:0000000000}", GLtoGLItem.fld_ItemNo);
                                        GLToGL.GL_ACCOUNT = GLtoGLItem.fld_GL.PadLeft(10, '0');
                                        GLToGL.ITEM_TEXT = Desc;
                                        GLToGL.NETWORK = GLtoGLItem.fld_Item.PadLeft(12, '0');
                                        GLToGL.ACTIVITY = GLtoGLItem.fld_SAPActivityCode;
                                        GLToGL.COSTCENTER = GetCC;
                                        GLToGLList.Add(GLToGL);
                                    }

                                    else
                                    {
                                        BAPIACGL09 GLToGL = new BAPIACGL09();

                                        GLToGL.ITEMNO_ACC = string.Format("{0:0000000000}", GLtoGLItem.fld_ItemNo);
                                        GLToGL.GL_ACCOUNT = GLtoGLItem.fld_GL.PadLeft(10, '0');
                                        GLToGL.ITEM_TEXT = Desc;

                                        if (!String.IsNullOrEmpty(GLtoGLItem.fld_Item))
                                        {
                                            if (GLtoGLItem.fld_Item.Substring(0, 1) == "P")
                                            {
                                                GLToGL.ORDERID = GLtoGLItem.fld_Item.PadLeft(10, '0');
                                            }
                                            else
                                            {
                                                GLToGL.COSTCENTER = GLtoGLItem.fld_Item.PadLeft(10, '0');
                                            }
                                        }

                                        GLToGLList.Add(GLToGL);
                                    }

                                    BAPIACCR09 GLToGLAmount = new BAPIACCR09();

                                    GLToGLAmount.ITEMNO_ACC = string.Format("{0:0000000000}", GLtoGLItem.fld_ItemNo);
                                    GLToGLAmount.CURRENCY = GLtoGLItem.fld_Currency;
                                    GLToGLAmount.AMT_DOCCUR = (decimal)GLtoGLItem.fld_Amount;

                                    GLToGLAmountList.Add(GLToGLAmount);
                                }

                                BAPIACGL09[] GLToGLArray = GLToGLList.ToArray();
                                BAPIACCR09[] GLToGLAmountArray = GLToGLAmountList.ToArray();
                                BAPIRET2[] ReturnArray = new BAPIRET2[] { Return };

                                Request.DOCUMENTHEADER = GLToGLHeader;
                                Request.ACCOUNTGL = GLToGLArray;
                                Request.CURRENCYAMOUNT = GLToGLAmountArray;
                                Request.RETURN = ReturnArray;

                                Response = SAPPosting.ZFI_ACC_DOC_POST(Request);

                                foreach (var returnMsg in Response.RETURN)
                                {
                                    var returnMsgData =
                                        SapModel.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == sapPostRefID);

                                    SapModel.tbl_SAPPostReturn.RemoveRange(returnMsgData);
                                    SapModel.SaveChanges();

                                    sortCount++;

                                    if (returnMsg.TYPE == "S")
                                    {
                                        sapDocNo = returnMsg.MESSAGE_V2;
                                    }

                                    ModelSAPPUP.tbl_SAPPostReturn sapPostReturn = new ModelSAPPUP.tbl_SAPPostReturn();

                                    sapPostReturn.fld_SortNo = sortCount;
                                    sapPostReturn.fld_Type = returnMsg.TYPE;
                                    sapPostReturn.fld_ReturnID = returnMsg.ID;
                                    sapPostReturn.fld_Number = returnMsg.NUMBER;
                                    sapPostReturn.fld_LogNo = returnMsg.LOG_NO;
                                    sapPostReturn.fld_Msg = returnMsg.MESSAGE;
                                    sapPostReturn.fld_Msg1 = returnMsg.MESSAGE_V1;
                                    sapPostReturn.fld_Msg2 = returnMsg.MESSAGE_V2;
                                    sapPostReturn.fld_Msg3 = returnMsg.MESSAGE_V3;
                                    sapPostReturn.fld_Msg4 = returnMsg.MESSAGE_V4;
                                    sapPostReturn.fld_Param = returnMsg.PARAMETER;
                                    sapPostReturn.fld_Row = returnMsg.ROW.ToString();
                                    sapPostReturn.fld_Field = returnMsg.FIELD;
                                    sapPostReturn.fld_System = returnMsg.SYSTEM;
                                    sapPostReturn.fld_SAPPostRefID = sapPostRefID;

                                    sapPostReturnList.Add(sapPostReturn);
                                }

                                if (sapPostReturnList.Any())
                                {
                                    SapModel.tbl_SAPPostReturn.AddRange(sapPostReturnList);
                                    SapModel.SaveChanges();
                                }

                                if (sapPostReturnList.Select(s => s.fld_Type).Contains("E"))
                                {
                                    CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                                    returnJson.Message = "Posting error for GL to GL, kindly check posting report for more information.";
                                    returnJson.Status = "danger";
                                    returnJson.Success = "false";
                                    returnJson.TransactionType = "GL to GL";

                                    returnJsonList.Add(returnJson);
                                }

                                if (sapPostReturnList.Select(s => s.fld_Type).Contains("S"))
                                {
                                    CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                                    returnJson.Message = "Succesfully post GL to GL document.";
                                    returnJson.Status = "success";
                                    returnJson.Success = "false";
                                    returnJson.TransactionType = "GL to GL";

                                    returnJsonList.Add(returnJson);

                                    var getGLPostingData =
                                        SapModel.tbl_SAPPostRef.SingleOrDefault(x => x.fld_ID == postGLToGL);

                                    getGLPostingData.fld_NoDocSAP = sapDocNo;
                                    getGLPostingData.fld_StatusProceed = true;

                                    SapModel.SaveChanges();
                                }
                            }
                            SAPPosting.Close();
                        }
                    }

                    catch (Exception ex)
                    {
                        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                        CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                        returnJson.Message = ex.Message;
                        returnJson.Status = "danger";
                        returnJson.Success = "false";
                        returnJson.TransactionType = "GL to GL";

                        returnJsonList.Add(returnJson);
                    }

                    try
                    {
                        var sapDocNo = "";
                        var sortCount = 0;

                        if (!String.IsNullOrEmpty(postGLToVendor.ToString()))
                        {
                            BasicHttpBinding binding = new BasicHttpBinding();
                            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                            binding.MaxReceivedMessageSize = 2147483647;
                            NetworkCredential Cred = new NetworkCredential();

                            EndpointAddress endpoint = new EndpointAddress(
                                "http://sapfgp.fgv.felhqr.myfelda:8002/sap/bc/srt/rfc/sap/zws_acc_doc_post_fgvassb/840/zws_acc_doc_post_fgvassb/zws_acc_doc_post_fgvassb");

                            //EndpointAddress endpoint = new EndpointAddress(
                            //    "http://sapfgq.fgv.felhqr.myfelda:8001/sap/bc/srt/rfc/sap/zws_acc_doc_post_fgvassb/840/zws_acc_doc_post_fgvassb/zws_acc_doc_post_fgvassb?sap-client=840");

                            //for testing 02.08.2022
                            //EndpointAddress endpoint = new EndpointAddress("http://sapfgq.fgv.felhqr.myfelda:8001/sap/bc/srt/rfc/sap/zws_acc_doc_post_fgvassb/840/zws_acc_doc_post_fgvassb/zws_acc_doc_post_fgvassb");

                            ZWS_ACC_DOC_POST_FGVASSBClient SAPPosting = new ZWS_ACC_DOC_POST_FGVASSBClient(binding, endpoint);
                            ZFI_ACC_DOC_POST Request = new ZFI_ACC_DOC_POST();
                            ZFI_ACC_DOC_POSTResponse Response = new ZFI_ACC_DOC_POSTResponse();

                            Cred.UserName = userName;
                            Cred.Password = password;
                            SAPPosting.ClientCredentials.UserName.UserName = Cred.UserName;
                            SAPPosting.ClientCredentials.UserName.Password = Cred.Password;
                            SAPPosting.Open();

                            BAPIACHE09 GLToVendorHeader = new BAPIACHE09();
                            List<BAPIACGL09> GLToVendorList = new List<BAPIACGL09>();
                            List<BAPIACAP09> VendorList = new List<BAPIACAP09>();
                            List<BAPIACCR09> GLToVendorAmountList = new List<BAPIACCR09>();
                            List<ModelSAPPUP.tbl_SAPPostReturn> sapPostReturnList = new List<ModelSAPPUP.tbl_SAPPostReturn>();
                            BAPIRET2 Return = new BAPIRET2();

                            Return.TYPE = null;
                            Return.ID = null;
                            Return.MESSAGE = null;
                            Return.NUMBER = null;
                            Return.LOG_NO = null;
                            Return.LOG_MSG_NO = null;
                            Return.MESSAGE_V1 = null;
                            Return.MESSAGE_V2 = null;
                            Return.MESSAGE_V3 = null;
                            Return.MESSAGE_V4 = null;
                            Return.PARAMETER = null;
                            Return.ROW = 0;
                            Return.FIELD = null;
                            Return.SYSTEM = null;

                            var GLToVendorPostingData =
                                SapModel.vw_SAPPostDataFullDetails.Where(x => x.fld_SAPPostRefID == postGLToVendor)
                                    .OrderBy(o => o.fld_ItemNo);

                            if (GLToVendorPostingData.DistinctBy(x => x.fld_SAPPostRefID).Select(s => s.fld_StatusProceed).SingleOrDefault() == false)
                            {
                                foreach (var headerData in GLToVendorPostingData.DistinctBy(x => x.fld_SAPPostRefID))
                                {
                                    GLToVendorHeader.HEADER_TXT = headerData.fld_HeaderText;
                                    GLToVendorHeader.USERNAME = userName;
                                    GLToVendorHeader.COMP_CODE = headerData.fld_CompCode;
                                    GLToVendorHeader.DOC_DATE = headerData.fld_DocDate.Value.ToString("yyyy-MM-dd");
                                    GLToVendorHeader.PSTNG_DATE = headerData.fld_PostingDate.Value.ToString("yyyy-MM-dd");
                                    GLToVendorHeader.DOC_TYPE = headerData.fld_DocType;
                                    GLToVendorHeader.REF_DOC_NO = headerData.fld_RefNo;

                                    sapPostRefID = (Guid)headerData.fld_SAPPostRefID;
                                }

                                foreach (var GLtoVendorItem in GLToVendorPostingData)
                                {
                                    if (!String.IsNullOrEmpty(GLtoVendorItem.fld_GL))
                                    {
                                        BAPIACGL09 GLToVendor = new BAPIACGL09();

                                        GLToVendor.ITEMNO_ACC = string.Format("{0:0000000000}", GLtoVendorItem.fld_ItemNo);
                                        GLToVendor.GL_ACCOUNT = GLtoVendorItem.fld_GL.PadLeft(10, '0');
                                        GLToVendor.ITEM_TEXT = GLtoVendorItem.fld_Desc;

                                        GLToVendorList.Add(GLToVendor);

                                        BAPIACCR09 GLToVendorAmount = new BAPIACCR09();

                                        GLToVendorAmount.ITEMNO_ACC = string.Format("{0:0000000000}", GLtoVendorItem.fld_ItemNo);
                                        GLToVendorAmount.CURRENCY = GLtoVendorItem.fld_Currency;
                                        GLToVendorAmount.AMT_DOCCUR = (decimal)GLtoVendorItem.fld_Amount;

                                        GLToVendorAmountList.Add(GLToVendorAmount);
                                    }

                                    if (!String.IsNullOrEmpty(GLtoVendorItem.fld_Item))
                                    {
                                        BAPIACAP09 Vendor = new BAPIACAP09();

                                        Vendor.ITEMNO_ACC = string.Format("{0:0000000000}", GLtoVendorItem.fld_ItemNo);
                                        Vendor.VENDOR_NO = GLtoVendorItem.fld_Item.PadLeft(10, '0');
                                        Vendor.ITEM_TEXT = GLtoVendorItem.fld_Desc;

                                        VendorList.Add(Vendor);

                                        BAPIACCR09 GLToVendorAmount = new BAPIACCR09();

                                        GLToVendorAmount.ITEMNO_ACC = string.Format("{0:0000000000}", GLtoVendorItem.fld_ItemNo);
                                        GLToVendorAmount.CURRENCY = GLtoVendorItem.fld_Currency;
                                        GLToVendorAmount.AMT_DOCCUR = (decimal)GLtoVendorItem.fld_Amount;

                                        GLToVendorAmountList.Add(GLToVendorAmount);
                                    }
                                }

                                BAPIACGL09[] GLToVendorArray = GLToVendorList.ToArray();
                                BAPIACAP09[] VendorArray = VendorList.ToArray();
                                BAPIACCR09[] GLToVendorAmountArray = GLToVendorAmountList.ToArray();
                                BAPIRET2[] ReturnArray = new BAPIRET2[] { Return };

                                Request.DOCUMENTHEADER = GLToVendorHeader;
                                Request.ACCOUNTGL = GLToVendorArray;
                                Request.ACCOUNTPAYABLE = VendorArray;
                                Request.CURRENCYAMOUNT = GLToVendorAmountArray;
                                Request.RETURN = ReturnArray;

                                Response = SAPPosting.ZFI_ACC_DOC_POST(Request);

                                foreach (var returnMsg in Response.RETURN)
                                {
                                    var returnMsgData =
                                        SapModel.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == sapPostRefID);

                                    SapModel.tbl_SAPPostReturn.RemoveRange(returnMsgData);
                                    SapModel.SaveChanges();

                                    sortCount++;

                                    if (returnMsg.TYPE == "S")
                                    {
                                        sapDocNo = returnMsg.MESSAGE_V2;
                                    }

                                    ModelSAPPUP.tbl_SAPPostReturn sapPostReturn = new ModelSAPPUP.tbl_SAPPostReturn();

                                    sapPostReturn.fld_SortNo = sortCount;
                                    sapPostReturn.fld_Type = returnMsg.TYPE;
                                    sapPostReturn.fld_ReturnID = returnMsg.ID;
                                    sapPostReturn.fld_Number = returnMsg.NUMBER;
                                    sapPostReturn.fld_LogNo = returnMsg.LOG_NO;
                                    sapPostReturn.fld_Msg = returnMsg.MESSAGE;
                                    sapPostReturn.fld_Msg1 = returnMsg.MESSAGE_V1;
                                    sapPostReturn.fld_Msg2 = returnMsg.MESSAGE_V2;
                                    sapPostReturn.fld_Msg3 = returnMsg.MESSAGE_V3;
                                    sapPostReturn.fld_Msg4 = returnMsg.MESSAGE_V4;
                                    sapPostReturn.fld_Param = returnMsg.PARAMETER;
                                    sapPostReturn.fld_Row = returnMsg.ROW.ToString();
                                    sapPostReturn.fld_Field = returnMsg.FIELD;
                                    sapPostReturn.fld_System = returnMsg.SYSTEM;
                                    sapPostReturn.fld_SAPPostRefID = sapPostRefID;

                                    sapPostReturnList.Add(sapPostReturn);
                                }

                                if (sapPostReturnList.Any())
                                {
                                    SapModel.tbl_SAPPostReturn.AddRange(sapPostReturnList);
                                    SapModel.SaveChanges();
                                }

                                if (sapPostReturnList.Select(s => s.fld_Type).Contains("E"))
                                {
                                    CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                                    returnJson.Message = "Posting error for GL to GL, kindly check posting report for more information.";
                                    returnJson.Status = "danger";
                                    returnJson.Success = "false";
                                    returnJson.TransactionType = "GL to Vendor";

                                    returnJsonList.Add(returnJson);
                                }

                                if (sapPostReturnList.Select(s => s.fld_Type).Contains("S"))
                                {
                                    CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                                    returnJson.Message = "Succesfully post GL to GL document.";
                                    returnJson.Status = "success";
                                    returnJson.Success = "false";
                                    returnJson.TransactionType = "GL to Vendor";

                                    returnJsonList.Add(returnJson);

                                    var getVendorPostingData =
                                        SapModel.tbl_SAPPostRef.SingleOrDefault(x => x.fld_ID == postGLToVendor);

                                    getVendorPostingData.fld_NoDocSAP = sapDocNo;
                                    getVendorPostingData.fld_StatusProceed = true;

                                    SapModel.SaveChanges();
                                }
                            }
                            SAPPosting.Close();
                        }
                    }

                    catch (Exception ex)
                    {
                        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                        CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                        returnJson.Message = ex.Message;
                        returnJson.Status = "danger";
                        returnJson.Success = "false";
                        returnJson.TransactionType = "GL to Vendor";

                        returnJsonList.Add(returnJson);
                    }

                    try
                    {
                        var sapDocNo = "";
                        var sortCount = 0;

                        if (!String.IsNullOrEmpty(postGLToCustomer.ToString()))
                        {
                            BasicHttpBinding binding = new BasicHttpBinding();
                            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                            NetworkCredential Cred = new NetworkCredential();

                            EndpointAddress endpoint = new EndpointAddress(
                                "http://sapfgp.fgv.felhqr.myfelda:8002/sap/bc/srt/rfc/sap/zws_acc_doc_post_fgvassb/840/zws_acc_doc_post_fgvassb/zws_acc_doc_post_fgvassb");

                            //EndpointAddress endpoint = new EndpointAddress(
                            //    "http://sapfgq.fgv.felhqr.myfelda:8001/sap/bc/srt/rfc/sap/zws_acc_doc_post_fgvassb/840/zws_acc_doc_post_fgvassb/zws_acc_doc_post_fgvassb?sap-client=840");

                            //for testing 02.08.2022
                            //EndpointAddress endpoint = new EndpointAddress("http://sapfgq.fgv.felhqr.myfelda:8001/sap/bc/srt/rfc/sap/zws_acc_doc_post_fgvassb/840/zws_acc_doc_post_fgvassb/zws_acc_doc_post_fgvassb");

                            ZWS_ACC_DOC_POST_FGVASSBClient SAPPosting = new ZWS_ACC_DOC_POST_FGVASSBClient(binding, endpoint);
                            ZFI_ACC_DOC_POST Request = new ZFI_ACC_DOC_POST();
                            ZFI_ACC_DOC_POSTResponse Response = new ZFI_ACC_DOC_POSTResponse();

                            Cred.UserName = userName;
                            Cred.Password = password;
                            SAPPosting.ClientCredentials.UserName.UserName = Cred.UserName;
                            SAPPosting.ClientCredentials.UserName.Password = Cred.Password;
                            SAPPosting.Open();

                            BAPIACHE09 GLToCustomerHeader = new BAPIACHE09();
                            List<BAPIACGL09> GLToCustomerList = new List<BAPIACGL09>();
                            List<BAPIACAR09> CustomerList = new List<BAPIACAR09>();
                            List<BAPIACCR09> GLToCustomerAmountList = new List<BAPIACCR09>();
                            List<ModelSAPPUP.tbl_SAPPostReturn> sapPostReturnList = new List<ModelSAPPUP.tbl_SAPPostReturn>();
                            BAPIRET2 Return = new BAPIRET2();

                            Return.TYPE = null;
                            Return.ID = null;
                            Return.MESSAGE = null;
                            Return.NUMBER = null;
                            Return.LOG_NO = null;
                            Return.LOG_MSG_NO = null;
                            Return.MESSAGE_V1 = null;
                            Return.MESSAGE_V2 = null;
                            Return.MESSAGE_V3 = null;
                            Return.MESSAGE_V4 = null;
                            Return.PARAMETER = null;
                            Return.ROW = 0;
                            Return.FIELD = null;
                            Return.SYSTEM = null;

                            var GLToCustomerPostingData =
                                SapModel.vw_SAPPostDataFullDetails.Where(x => x.fld_SAPPostRefID == postGLToCustomer)
                                    .OrderBy(o => o.fld_ItemNo);

                            if (GLToCustomerPostingData.DistinctBy(x => x.fld_SAPPostRefID).Select(s => s.fld_StatusProceed).SingleOrDefault() == false)
                            {
                                foreach (var headerData in GLToCustomerPostingData.DistinctBy(x => x.fld_SAPPostRefID))
                                {
                                    GLToCustomerHeader.HEADER_TXT = headerData.fld_HeaderText;
                                    GLToCustomerHeader.USERNAME = userName;
                                    GLToCustomerHeader.COMP_CODE = headerData.fld_CompCode;
                                    GLToCustomerHeader.DOC_DATE = headerData.fld_DocDate.Value.ToString("yyyy-MM-dd");
                                    GLToCustomerHeader.PSTNG_DATE = headerData.fld_PostingDate.Value.ToString("yyyy-MM-dd");
                                    GLToCustomerHeader.DOC_TYPE = headerData.fld_DocType;
                                    GLToCustomerHeader.REF_DOC_NO = headerData.fld_RefNo;

                                    sapPostRefID = (Guid)headerData.fld_SAPPostRefID;
                                }

                                foreach (var GLtoCustomerItem in GLToCustomerPostingData)
                                {
                                    if (!String.IsNullOrEmpty(GLtoCustomerItem.fld_GL))
                                    {
                                        BAPIACGL09 GLToCustomer = new BAPIACGL09();

                                        GLToCustomer.ITEMNO_ACC = string.Format("{0:0000000000}", GLtoCustomerItem.fld_ItemNo);
                                        GLToCustomer.GL_ACCOUNT = GLtoCustomerItem.fld_GL.PadLeft(10, '0');
                                        GLToCustomer.ITEM_TEXT = GLtoCustomerItem.fld_Desc;

                                        GLToCustomerList.Add(GLToCustomer);

                                        BAPIACCR09 GLToCustomerAmount = new BAPIACCR09();

                                        GLToCustomerAmount.ITEMNO_ACC = string.Format("{0:0000000000}", GLtoCustomerItem.fld_ItemNo);
                                        GLToCustomerAmount.CURRENCY = GLtoCustomerItem.fld_Currency;
                                        GLToCustomerAmount.AMT_DOCCUR = (decimal)GLtoCustomerItem.fld_Amount;

                                        GLToCustomerAmountList.Add(GLToCustomerAmount);
                                    }

                                    if (!String.IsNullOrEmpty(GLtoCustomerItem.fld_Item))
                                    {
                                        BAPIACAR09 Customer = new BAPIACAR09();

                                        Customer.ITEMNO_ACC = string.Format("{0:0000000000}", GLtoCustomerItem.fld_ItemNo);
                                        Customer.CUSTOMER = GLtoCustomerItem.fld_Item.PadLeft(10, '0');
                                        Customer.ITEM_TEXT = GLtoCustomerItem.fld_Desc;

                                        CustomerList.Add(Customer);

                                        BAPIACCR09 GLToCustomerAmount = new BAPIACCR09();

                                        GLToCustomerAmount.ITEMNO_ACC = string.Format("{0:0000000000}", GLtoCustomerItem.fld_ItemNo);
                                        GLToCustomerAmount.CURRENCY = GLtoCustomerItem.fld_Currency;
                                        GLToCustomerAmount.AMT_DOCCUR = (decimal)GLtoCustomerItem.fld_Amount;

                                        GLToCustomerAmountList.Add(GLToCustomerAmount);
                                    }
                                }

                                BAPIACGL09[] GLToCustomerArray = GLToCustomerList.ToArray();
                                BAPIACAR09[] CustomerArray = CustomerList.ToArray();
                                BAPIACCR09[] GLToCustomerAmountArray = GLToCustomerAmountList.ToArray();
                                BAPIRET2[] ReturnArray = new BAPIRET2[] { Return };

                                Request.DOCUMENTHEADER = GLToCustomerHeader;
                                Request.ACCOUNTGL = GLToCustomerArray;
                                Request.ACCOUNTRECEIVABLE = CustomerArray;
                                Request.CURRENCYAMOUNT = GLToCustomerAmountArray;
                                Request.RETURN = ReturnArray;

                                Response = SAPPosting.ZFI_ACC_DOC_POST(Request);

                                foreach (var returnMsg in Response.RETURN)
                                {
                                    var returnMsgData =
                                        SapModel.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == sapPostRefID);

                                    SapModel.tbl_SAPPostReturn.RemoveRange(returnMsgData);
                                    SapModel.SaveChanges();

                                    sortCount++;

                                    if (returnMsg.TYPE == "S")
                                    {
                                        sapDocNo = returnMsg.MESSAGE_V2;
                                    }

                                    ModelSAPPUP.tbl_SAPPostReturn sapPostReturn = new ModelSAPPUP.tbl_SAPPostReturn();

                                    sapPostReturn.fld_SortNo = sortCount;
                                    sapPostReturn.fld_Type = returnMsg.TYPE;
                                    sapPostReturn.fld_ReturnID = returnMsg.ID;
                                    sapPostReturn.fld_Number = returnMsg.NUMBER;
                                    sapPostReturn.fld_LogNo = returnMsg.LOG_NO;
                                    sapPostReturn.fld_Msg = returnMsg.MESSAGE;
                                    sapPostReturn.fld_Msg1 = returnMsg.MESSAGE_V1;
                                    sapPostReturn.fld_Msg2 = returnMsg.MESSAGE_V2;
                                    sapPostReturn.fld_Msg3 = returnMsg.MESSAGE_V3;
                                    sapPostReturn.fld_Msg4 = returnMsg.MESSAGE_V4;
                                    sapPostReturn.fld_Param = returnMsg.PARAMETER;
                                    sapPostReturn.fld_Row = returnMsg.ROW.ToString();
                                    sapPostReturn.fld_Field = returnMsg.FIELD;
                                    sapPostReturn.fld_System = returnMsg.SYSTEM;
                                    sapPostReturn.fld_SAPPostRefID = sapPostRefID;

                                    sapPostReturnList.Add(sapPostReturn);
                                }

                                if (sapPostReturnList.Any())
                                {
                                    SapModel.tbl_SAPPostReturn.AddRange(sapPostReturnList);
                                    SapModel.SaveChanges();
                                }

                                if (sapPostReturnList.Select(s => s.fld_Type).Contains("E"))
                                {
                                    CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                                    returnJson.Message = "Posting error for GL to Customer, kindly check posting report for more information.";
                                    returnJson.Status = "danger";
                                    returnJson.Success = "false";
                                    returnJson.TransactionType = "GL to Customer";

                                    returnJsonList.Add(returnJson);
                                }

                                if (sapPostReturnList.Select(s => s.fld_Type).Contains("S"))
                                {
                                    CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                                    returnJson.Message = "Succesfully post GL to Customer document.";
                                    returnJson.Status = "success";
                                    returnJson.Success = "false";
                                    returnJson.TransactionType = "GL to Customer";

                                    returnJsonList.Add(returnJson);

                                    var getCustomerPostingData =
                                        SapModel.tbl_SAPPostRef.SingleOrDefault(x => x.fld_ID == postGLToCustomer);

                                    getCustomerPostingData.fld_NoDocSAP = sapDocNo;
                                    getCustomerPostingData.fld_StatusProceed = true;

                                    SapModel.SaveChanges();
                                }
                            }
                            SAPPosting.Close();
                        }
                    }

                    catch (Exception ex)
                    {
                        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                        CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                        returnJson.Message = ex.Message;
                        returnJson.Status = "danger";
                        returnJson.Success = "false";
                        returnJson.TransactionType = "GL to Customer";

                        returnJsonList.Add(returnJson);
                    }
                }

                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                    CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                    returnJson.Message = ex.Message;
                    returnJson.Status = "danger";
                    returnJson.Success = "false";
                    returnJson.TransactionType = "GL to Customer";

                    returnJsonList.Add(returnJson);
                }

                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/")
                {
                    domain = domain + appname;
                }

                returnJsonToView.ReturnJsonList = returnJsonList;
                returnJsonToView.RootUrl = domain;
                returnJsonToView.Action = "_PostingSAP";
                returnJsonToView.Controller = "ClosingTransaction";
                returnJsonToView.Div = "closeTransactionDetails";
                returnJsonToView.ParamName1 = "MonthList";
                returnJsonToView.ParamValue1 = month.ToString();
                returnJsonToView.ParamName2 = "YearList";
                returnJsonToView.ParamValue2 = year.ToString();

                return Json(returnJsonToView);
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                    checkingdata = "0"
                });
            }

            finally
            {
                db.Dispose();
            }
        }

        public ActionResult _SAPReturnMsg(Guid? postRefID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models_SAPPUP SapModel = MVC_SYSTEM_Models_SAPPUP.ConnectToSqlServer(host, catalog, user, pass);

            var getSAPReturnMsgData = SapModel.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == postRefID).OrderBy(o => o.fld_SortNo);

            return PartialView("_SAPReturnMsg", getSAPReturnMsgData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                //db2.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult RegisterSkb(string MonthList = "", int YearList = 0, int page = 1, string sort = "fld_ID", string sortdir = "ASC")
        {
            //ViewBag.Maintenance = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            ViewBag.CheckRoll = "class = active";

            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Skb>();

            int range = int.Parse(GetConfig.GetData("yeardisplay"));
            int startyear = DateTime.Now.AddYears(-range).Year;
            int currentyear = DateTime.Now.Year;
            DateTime selectdate = DateTime.Now.AddMonths(-1);

            if (MonthList == "" && YearList == 0)
            {
                var yearlist = new List<SelectListItem>();
                for (var i = startyear; i <= currentyear; i++)
                {
                    if (i == selectdate.Year)
                    {
                        yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                    }
                    else
                    {
                        yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                    }
                }
                var monthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", selectdate.Month);
                records.Content = dbview.tbl_Skb.Where(x => x.fld_Bulan == selectdate.Month.ToString() && x.fld_Tahun == selectdate.Year && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tbl_Skb.Where(x => x.fld_Bulan == selectdate.Month.ToString() && x.fld_Tahun == selectdate.Year && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;

                ViewBag.MonthList = monthList;
                ViewBag.YearList = yearlist;
                ViewBag.Datacount = records.TotalRecords;
                return View(records);
            }
            else
            {
                var yearlist = new List<SelectListItem>();
                for (var i = startyear; i <= currentyear; i++)
                {
                    if (i == YearList)
                    {
                        yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                    }
                    else
                    {
                        yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                    }
                }
                var monthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", MonthList);
                GetConfig GetConfig = new GetConfig();
                string bulanString = GetConfig.GetWebConfigDesc(MonthList, "monthlist", NegaraID, SyarikatID);
                records.Content = dbview.tbl_Skb.Where(x => x.fld_Bulan == bulanString && x.fld_Tahun == YearList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tbl_Skb.Where(x => x.fld_Bulan == bulanString && x.fld_Tahun == YearList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;

                ViewBag.MonthList = monthList;
                ViewBag.YearList = yearlist;
                ViewBag.Datacount = records.TotalRecords;
                return View(records);
            }
        }

        public ActionResult RegisterSkbUpdate(int id)
        {
            if (id < 1)
            {
                return RedirectToAction("RegisterSkb");
            }
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_Skb tbl_Skb = dbr.tbl_Skb.Where(w => w.fld_ID == id).FirstOrDefault();
            if (tbl_Skb == null)
            {
                return RedirectToAction("RegisterSkb");
            }

            return PartialView(tbl_Skb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterSkbUpdate(int id, Models.tbl_Skb tbl_Skb)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int? getuserid = GetIdentity.ID(User.Identity.Name);
                    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                    string host, catalog, user, pass = "";
                    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                    var getdata = dbr.tbl_Skb.Where(w => w.fld_ID == id).FirstOrDefault();

                    getdata.fld_NoSkb = tbl_Skb.fld_NoSkb;

                    dbr.Entry(getdata).State = EntityState.Modified;
                    dbr.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }
    }
}