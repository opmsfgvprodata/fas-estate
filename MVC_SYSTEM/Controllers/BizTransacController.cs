//using MVC_SYSTEM.App_LocalResources;
//using MVC_SYSTEM.Attributes;
//using MVC_SYSTEM.Class;
//using MVC_SYSTEM.log;
//using MVC_SYSTEM.MasterModels;
//using MVC_SYSTEM.Models;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using MVC_SYSTEM.CustomModels;
//using System.ServiceModel;
//using System.Net;
//using MVC_SYSTEM.OPMStoSAP;

//namespace MVC_SYSTEM.Controllers
//{
//    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
//    public class BizTransacController : Controller
//    {
//        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
//        private GetIdentity getidentity = new GetIdentity();
//        private GetTriager GetTriager = new GetTriager();
//        private GetNSWL GetNSWL = new GetNSWL();
//        private ChangeTimeZone timezone = new ChangeTimeZone();
//        private errorlog geterror = new errorlog();
//        private GetConfig GetConfig = new GetConfig();
//        private GetIdentity GetIdentity = new GetIdentity();
//        private GetWilayah GetWilayah = new GetWilayah();
//        private Connection Connection = new Connection();
//        private CheckrollFunction EstateFunction = new CheckrollFunction();
//        // GET: BizTransac
//        public ActionResult Index()
//        {
//            ViewBag.ClosingTransaction = "class = active";
//            int? getuserid = GetIdentity.ID(User.Identity.Name);
//            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
//            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
//            ViewBag.BizTransacMenu = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "BizTransac" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fld_Val", "fld_Desc");
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Index(string BizTransacMenu)
//        {
//            return RedirectToAction(BizTransacMenu, "BizTransac");
//        }

//        public ActionResult SAPPosting()
//        {
//            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
//            int? getuserid = getidentity.ID(User.Identity.Name);
//            string host, catalog, user, pass = "";

//            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
//            int year = Minus1month.Year;
//            int month = Minus1month.Month;
//            int drpyear = 0;
//            int drprangeyear = 0;

//            ViewBag.ClosingTransaction = "class = active";

//            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
//            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
//            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

//            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
//            drprangeyear = timezone.gettimezone().Year;

//            var yearlist = new List<SelectListItem>();
//            for (var i = drpyear; i <= drprangeyear; i++)
//            {
//                if (i == year)
//                {
//                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
//                }
//                else
//                {
//                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
//                }
//            }

//            ViewBag.YearList = yearlist;

//            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

//            return View();
//        }

//        public ViewResult _SAPPostingSearch(int MonthList, int YearList)
//        {
//            if (MonthList != 0 && YearList != 0)
//            {
//                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
//                int? getuserid = getidentity.ID(User.Identity.Name);
//                string host, catalog, user, pass = "";
//                CustMod_SAPPostingData CustMod_SAPPostingData = new CustMod_SAPPostingData();

//                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
//                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
//                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

//                var GetSapPostData = dbr.tbl_SAPPostRef.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_Purpose == 1 && x.fld_StatusProceed == false).FirstOrDefault();

//                if (GetSapPostData != null)
//                {
//                    CustMod_SAPPostingData.GetSAPPostRef = GetSapPostData;
//                    var GetSapPostVendor = dbr.tbl_SAPPostVendorDataDetails.Where(x => x.fld_SAPPostRefID == GetSapPostData.fld_ID).FirstOrDefault();
//                    CustMod_SAPPostingData.GetSAPPostVendorDataDetails = GetSapPostVendor;
//                    var GetSapPostGL = dbr.tbl_SAPPostGLIODataDetails.Where(x => x.fld_SAPPostRefID == GetSapPostData.fld_ID).ToList();
//                    CustMod_SAPPostingData.SAPPostGLIODataDetails = GetSapPostGL;
//                    return View(CustMod_SAPPostingData);
//                }
//                else
//                {
//                    ViewBag.Message = "Tiada data";
//                    return View();
//                }
//            }
//            else
//            {
//                ViewBag.Message = "Sila pilih bulan dan tahun";
//                return View();
//            }
            
//        }

//        [HttpPost]
//        public ActionResult _SAPSaveData(CustMod_SAPPostingSave CustMod_SAPPostingSave)
//        {
//            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
//            string  host, catalog, user, pass = "";
//            string msg = "";
//            string statusmsg = "";

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    int? getuserid = getidentity.ID(User.Identity.Name);
//                    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
//                    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
//                    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

//                    var GetSAPPostRefDetail = dbr.tbl_SAPPostRef.Find(CustMod_SAPPostingSave.PostingID);
//                    GetSAPPostRefDetail.fld_CpdName = CustMod_SAPPostingSave.Name;
//                    GetSAPPostRefDetail.fld_CpdName2 = CustMod_SAPPostingSave.Name2;
//                    GetSAPPostRefDetail.fld_PostingDate = CustMod_SAPPostingSave.PostingDate;
//                    GetSAPPostRefDetail.fld_InvoiceDate = CustMod_SAPPostingSave.InvoiceDate;
//                    GetSAPPostRefDetail.fld_RefNo = CustMod_SAPPostingSave.RefNo;
//                    GetSAPPostRefDetail.fld_ModifiedBy = getuserid;
//                    GetSAPPostRefDetail.fld_ModifiedDT = timezone.gettimezone();
//                    dbr.Entry(GetSAPPostRefDetail).State = EntityState.Modified;
//                    dbr.SaveChanges();

//                    var GetSAPPsitVendorDetails = dbr.tbl_SAPPostVendorDataDetails.Where(x => x.fld_SAPPostRefID == CustMod_SAPPostingSave.PostingID).FirstOrDefault();
//                    GetSAPPsitVendorDetails.fld_VendorNo = CustMod_SAPPostingSave.VendorNo;
//                    GetSAPPsitVendorDetails.fld_Desc = CustMod_SAPPostingSave.DescVendor;
//                    dbr.Entry(GetSAPPsitVendorDetails).State = EntityState.Modified;
//                    dbr.SaveChanges();

//                    msg = "Berjaya disimpan.";
//                    statusmsg = "success";
//                }
//                catch (Exception ex)
//                {
//                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
//                    msg = "Gagal disimpan.";
//                    statusmsg = "warning";
//                }
//            }
            
//            return Json(new { msg, statusmsg });
//        }
        
//        [HttpPost]
//        public ActionResult _PostToSAP(Guid PostingID, string SAPUsername, string SAPPassword)
//        {
//            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
//            string host, catalog, user, pass = "";
//            string msg = "";
//            string statusmsg = "";

//            int? getuserid = getidentity.ID(User.Identity.Name);
//            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
//            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
//            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

//            BasicHttpBinding binding = new BasicHttpBinding();
//            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
//            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
//            NetworkCredential Cred = new NetworkCredential();
//            BAPIACHE09 InputDataDocHeader = new BAPIACHE09();
//            BAPIACPA09 InputDataCustPD = new BAPIACPA09();
//            BAPIACGL09 InputDataAccGL_ = new BAPIACGL09();
//            BAPIACAP09 InputDataAccPay_ = new BAPIACAP09();
//            BAPIACTX09 InputDataAccTax_ = new BAPIACTX09();
//            BAPIACCR09 InputDataCurAmt_ = new BAPIACCR09();
//            BAPIACCR09 InputDataCurAmt2_ = new BAPIACCR09();
//            BAPIRET2 OutputReturn_ = new BAPIRET2();

//            EndpointAddress endpoint = new EndpointAddress("http://cifld.felhqr.myfelda:8000/sap/bc/srt/rfc/sap/zwsopmsfiar01/210/zwsopmsfiar01/zwsopmsfiar01");
//            ZFMOPMSFIAR01Response SAPPostingResponse = new ZFMOPMSFIAR01Response();
//            zwsopmsfiar01Client SAPPosting = new zwsopmsfiar01Client(binding, endpoint);
//            ZFMOPMSFIAR01 SAPPostingCollectionData = new ZFMOPMSFIAR01();
//            int i = 0;
//            try
//            {
//                var GetSAPPostRefDetail = dbr.tbl_SAPPostRef.Find(PostingID);
//                var GetSAPPostVendorDetails = dbr.tbl_SAPPostVendorDataDetails.Where(x => x.fld_SAPPostRefID == PostingID).FirstOrDefault();
//                var GetSAPPostGLIODetails = dbr.tbl_SAPPostGLIODataDetails.Where(x => x.fld_SAPPostRefID == PostingID).OrderBy(o => o.fld_ItemNo).ToList();

//                Cred.UserName = SAPUsername;
//                Cred.Password = SAPPassword;
//                SAPPosting.ClientCredentials.UserName.UserName = Cred.UserName;
//                SAPPosting.ClientCredentials.UserName.Password = Cred.Password;
//                SAPPosting.Open();

//                InputDataDocHeader.USERNAME = SAPUsername;
//                InputDataDocHeader.HEADER_TXT = "OPMS";
//                InputDataDocHeader.COMP_CODE = GetSAPPostRefDetail.fld_CompCode;
//                InputDataDocHeader.DOC_DATE = GetSAPPostRefDetail.fld_InvoiceDate.Value.ToString("yyyy-MM-dd");
//                InputDataDocHeader.PSTNG_DATE = GetSAPPostRefDetail.fld_PostingDate.Value.ToString("yyyy-MM-dd");
//                InputDataDocHeader.DOC_TYPE = "KR";
//                InputDataDocHeader.REF_DOC_NO = GetSAPPostRefDetail.fld_RefNo;

//                InputDataCustPD.NAME = GetSAPPostRefDetail.fld_CpdName;
//                InputDataCustPD.NAME_2 = GetSAPPostRefDetail.fld_CpdName2;
//                InputDataCustPD.POSTL_CODE = GetSAPPostRefDetail.fld_PostCode;
//                InputDataCustPD.CITY = GetSAPPostRefDetail.fld_City;
//                InputDataCustPD.COUNTRY = GetSAPPostRefDetail.fld_Country;
//                InputDataCustPD.STREET = GetSAPPostRefDetail.fld_City;

//                //InputDataAccGL_.ITEMNO_ACC = "0000000002";
//                //InputDataAccGL_.GL_ACCOUNT = "0076510010";
//                //InputDataAccGL_.ITEM_TEXT = "GL 1";
//                //InputDataAccGL_.TAX_CODE = "TZ";
//                //InputDataAccGL_.COSTCENTER = "0113005000";
//                //InputDataAccGL_.ORDERID = "C113005203";
//                List<BAPIACGL09> InputDataAccGL = new List<BAPIACGL09>();
//                var CC = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_CostCentre).FirstOrDefault();
//                foreach (var GetSAPPostGLIODetail in GetSAPPostGLIODetails)
//                {
//                    InputDataAccGL.Add(new BAPIACGL09() { ITEMNO_ACC = GetTriager.GetSAPItemNo(GetSAPPostGLIODetail.fld_ItemNo), GL_ACCOUNT = GetSAPPostGLIODetail.fld_GL, ITEM_TEXT = GetSAPPostGLIODetail.fld_Desc, TAX_CODE = "Q4", COSTCENTER = CC, ORDERID = GetSAPPostGLIODetail.fld_IO });
//                }

//                //
//                InputDataAccPay_.ITEMNO_ACC = GetTriager.GetSAPItemNo(GetSAPPostVendorDetails.fld_ItemNo);
//                InputDataAccPay_.VENDOR_NO = GetSAPPostVendorDetails.fld_VendorNo;
//                InputDataAccPay_.PMNTTRMS = "Z030";
//                InputDataAccPay_.BLINE_DATE = GetSAPPostVendorDetails.fld_BaseDate.Value.ToString("yyyy-MM-dd");
//                InputDataAccPay_.ITEM_TEXT = GetSAPPostVendorDetails.fld_Desc;
//                InputDataAccPay_.ALLOC_NMBR = "OPMS POSTING";

//                BAPIACAP09[] InputDataAccPay = new BAPIACAP09[] { InputDataAccPay_ };

//                InputDataCurAmt_.ITEMNO_ACC = GetTriager.GetSAPItemNo(GetSAPPostVendorDetails.fld_ItemNo);
//                InputDataCurAmt_.CURRENCY = GetSAPPostVendorDetails.fld_Currency;
//                InputDataCurAmt_.AMT_DOCCUR = GetSAPPostVendorDetails.fld_Amount.Value;
//                InputDataCurAmt_.AMT_BASE = 0;

//                //InputDataCurAmt2_.ITEMNO_ACC = "0000000002";
//                //InputDataCurAmt2_.CURRENCY = "RM";
//                //InputDataCurAmt2_.AMT_DOCCUR = 2000;
//                //InputDataCurAmt2_.AMT_BASE = 0;

//                List<BAPIACCR09> InputDataCurAmt = new List<BAPIACCR09>();

//                InputDataCurAmt.Add(new BAPIACCR09() { ITEMNO_ACC = GetTriager.GetSAPItemNo(GetSAPPostVendorDetails.fld_ItemNo), CURRENCY = GetSAPPostVendorDetails.fld_Currency, AMT_DOCCUR = GetSAPPostVendorDetails.fld_Amount.Value, AMT_BASE = 0 });

//                foreach (var GetSAPPostGLIODetail in GetSAPPostGLIODetails)
//                {
//                    InputDataCurAmt.Add(new BAPIACCR09() { ITEMNO_ACC = GetTriager.GetSAPItemNo(GetSAPPostGLIODetail.fld_ItemNo), CURRENCY = GetSAPPostGLIODetail.fld_Currency, AMT_DOCCUR = GetSAPPostGLIODetail.fld_Amount.Value, AMT_BASE = 0 });
//                }

//                OutputReturn_.FIELD = null;
//                OutputReturn_.ID = null;
//                OutputReturn_.LOG_MSG_NO = null;
//                OutputReturn_.LOG_NO = null;
//                OutputReturn_.MESSAGE = null;
//                OutputReturn_.MESSAGE_V1 = null;
//                OutputReturn_.MESSAGE_V2 = null;
//                OutputReturn_.MESSAGE_V3 = null;
//                OutputReturn_.MESSAGE_V4 = null;
//                OutputReturn_.NUMBER = null;
//                OutputReturn_.PARAMETER = null;
//                OutputReturn_.ROW = 0;
//                OutputReturn_.SYSTEM = null;
//                OutputReturn_.TYPE = null;

//                BAPIRET2[] OutputReturn = new BAPIRET2[] { OutputReturn_ };

//                SAPPostingCollectionData.DOCUMENTHEADER = InputDataDocHeader;
//                SAPPostingCollectionData.CUSTOMERCPD = InputDataCustPD;
//                SAPPostingCollectionData.ACCOUNTGL = InputDataAccGL.ToArray();
//                SAPPostingCollectionData.ACCOUNTPAYABLE = InputDataAccPay;
//                //SAPPostingCollectionData.ACCOUNTTAX = InputDataAccTax;
//                SAPPostingCollectionData.CURRENCYAMOUNT = InputDataCurAmt.ToArray();
//                SAPPostingCollectionData.RETURN = OutputReturn;
//                SAPPostingResponse = SAPPosting.ZFMOPMSFIAR01(SAPPostingCollectionData);
                
//                List<tbl_SAPPostReturn> SAPReturnList = new List<tbl_SAPPostReturn>();

//                if (SAPPostingResponse.RETURN.Count() > 1)
//                {
//                    EstateFunction.DeleteReturnSAPPost(PostingID, dbr);
//                    int NoSort = 1;
//                    foreach (var SAPReturn in SAPPostingResponse.RETURN)
//                    {
//                        SAPReturnList.Add(new tbl_SAPPostReturn() { fld_SortNo = NoSort, fld_Msg1 = SAPReturn.MESSAGE, fld_Msg2 = SAPReturn.MESSAGE_V1, fld_Msg3 = SAPReturn.MESSAGE_V2, fld_Msg4 = SAPReturn.MESSAGE_V3, fld_Msg5 = SAPReturn.MESSAGE_V4, fld_SAPPostRefID = PostingID });
//                        NoSort++;
//                    }
//                    EstateFunction.AddReturnSAPPost(dbr, SAPReturnList);
//                    msg = "Tidak berjaya dihantar. Sila semak data yang dihantar.";
//                    statusmsg = "warning";
//                }
//                else
//                {
//                    EstateFunction.DeleteReturnSAPPost(PostingID, dbr);
//                    int NoSort = 1;
//                    foreach (var SAPReturn in SAPPostingResponse.RETURN)
//                    {
//                        SAPReturnList.Add(new tbl_SAPPostReturn() { fld_SortNo = NoSort, fld_Msg1 = SAPReturn.MESSAGE, fld_Msg2 = SAPReturn.MESSAGE_V1, fld_Msg3 = SAPReturn.MESSAGE_V2, fld_Msg4 = SAPReturn.MESSAGE_V3, fld_Msg5 = SAPReturn.MESSAGE_V4, fld_SAPPostRefID = PostingID });
//                        NoSort++;
//                    }
//                    EstateFunction.AddReturnSAPPost(dbr, SAPReturnList);
//                    msg = "Berjaya dihantar.";
//                    statusmsg = "success";
//                }
//            }
//            catch (Exception ex)
//            {
//                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
//                msg = "Tidak berjaya dihantar. Sila semak data yang dihantar.";
//                statusmsg = "warning";
//            }
//            return Json(new { msg, statusmsg });
//        }

//        public ActionResult SAPReturnReport(Guid PostingID)
//        {
//            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
//            string host, catalog, user, pass = "";

//            int? getuserid = getidentity.ID(User.Identity.Name);
//            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
//            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
//            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

//            var GetSAPReportList = dbr.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == PostingID).OrderBy(o => o.fld_SortNo).ToList();

//            return View("SAPReturnReport", GetSAPReportList);
//        }

//        public ActionResult CloseTransaction()
//        {
//            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
//            int? getuserid = getidentity.ID(User.Identity.Name);
//            string host, catalog, user, pass = "";

//            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
//            int year = Minus1month.Year;
//            int month = Minus1month.Month;
//            int drpyear = 0;
//            int drprangeyear = 0;

//            ViewBag.ClosingTransaction = "class = active";

//            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
//            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
//            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

//            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
//            drprangeyear = timezone.gettimezone().Year;

//            var yearlist = new List<SelectListItem>();
//            for (var i = drpyear; i <= drprangeyear; i++)
//            {
//                if (i == year)
//                {
//                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
//                }
//                else
//                {
//                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
//                }
//            }

//            ViewBag.YearList = yearlist;

//            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

//            List<SelectListItem> CloseOpen = new List<SelectListItem>();
//            CloseOpen.Insert(0, (new SelectListItem { Text = "Tutup Urus Niaga", Value = "true" }));
//            if (getidentity.HQAuth(User.Identity.Name))
//            {
//                CloseOpen.Insert(1, (new SelectListItem { Text = "Buka Urus Niaga", Value = "false" }));
//            }

//            ViewBag.CloseOpen = CloseOpen;

//            //ViewBag.ProcessList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "gensalary" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc");

//            dbr.Dispose();
//            return View();
//        }

//        [HttpPost]
//        public ActionResult CloseTransaction(int Month, int Year, bool CloseOpen)
//        {
//            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
//            int? getuserid = getidentity.ID(User.Identity.Name);
//            string host, catalog, user, pass = "";
//            string msg = "";
//            string statusmsg = "";
//            int? AuditTrailStatus = 0;

//            ViewBag.ClosingTransaction = "class = active";

//            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
//            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
//            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
//            string monthstring = Month.ToString();
//            if (monthstring.Length == 1)
//            {
//                monthstring = "0" + monthstring;
//            }
//            var ClosingTransaction = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year).FirstOrDefault();
//            var CheckScTransSalary = dbr.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KodAktvt == "4000").Select(s => s.fld_Amt).FirstOrDefault();
//            var CheckSkbReg = dbr.tbl_Skb.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Bulan == monthstring && x.fld_Tahun == Year).FirstOrDefault();
//            if (ClosingTransaction != null)
//            {
//                if (CheckSkbReg.fld_NoSkb != null)
//                {
//                    if (CheckSkbReg.fld_GajiBersih == CheckScTransSalary)
//                    {
//                        if (ClosingTransaction.fld_Credit == ClosingTransaction.fld_Debit)
//                        {
//                            if (CloseOpen == true && ClosingTransaction.fld_StsTtpUrsNiaga == true)
//                            {
//                                msg = "Urus niaga telah ditutup";
//                                statusmsg = "warning";
//                            }
//                            else
//                            {
//                                AuditTrailStatus = CloseOpen == true ? 1 : 0;
//                                ClosingTransaction.fld_StsTtpUrsNiaga = CloseOpen;
//                                ClosingTransaction.fld_ModifiedDT = timezone.gettimezone();
//                                ClosingTransaction.fld_ModifiedBy = getuserid;
//                                dbr.Entry(ClosingTransaction).State = EntityState.Modified;
//                                dbr.SaveChanges();
//                                UpdateAuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, AuditTrailStatus);
//                                FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, CheckSkbReg.fld_GajiBersih, CheckSkbReg.fld_NoSkb, getuserid);
//                                msg = GlobalResEstate.msgUpdate;
//                                statusmsg = "success";
//                            }

//                        }
//                        else
//                        {
//                            msg = GlobalResEstate.msgBalance;
//                            statusmsg = "warning";
//                        }
//                    }
//                    else
//                    {
//                        msg = "Sila pastikan nilai pemohonan sama seperti didaftar di No SKB sebelum urusniaga ditutup";
//                        statusmsg = "warning";
//                    }

//                }
//                else
//                {
//                    msg = "Sila daftar No SKB sebelum urusniaga ditutup";
//                    statusmsg = "warning";
//                }
//            }
//            else
//            {
//                msg = GlobalResEstate.msgGenSalary;
//                statusmsg = "warning";
//            }

//            dbr.Dispose();
//            return Json(new { msg, statusmsg });
//        }

//        public ActionResult AuditTrail()
//        {
//            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
//            int? getuserid = getidentity.ID(User.Identity.Name);
//            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
//            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
//            int year = Minus1month.Year;
//            int month = Minus1month.Month;
//            int drpyear = 0;
//            int drprangeyear = 0;
//            //List<SelectListItem> SelectionData = new List<SelectListItem>();

//            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
//            drprangeyear = timezone.gettimezone().Year;

//            var yearlist = new List<SelectListItem>();
//            for (var i = drpyear; i <= drprangeyear; i++)
//            {
//                if (i == year)
//                {
//                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
//                }
//                else
//                {
//                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
//                }
//            }

//            ViewBag.NamaSyarikat = db.tbl_Syarikat
//                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
//                .Select(s => s.fld_NamaSyarikat)
//                .FirstOrDefault();
//            ViewBag.NoSyarikat = db.tbl_Syarikat
//                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
//                .Select(s => s.fld_NoSyarikat)
//                .FirstOrDefault();

//            var GetAuditTrail = db.tbl_AuditTrail.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Thn == year).FirstOrDefault();

//            ViewBag.YearList = yearlist;
//            ViewBag.Tahun = year;
//            return View("AuditTrail", GetAuditTrail);
//        }

//        [HttpPost]
//        public ActionResult AuditTrail(int YearList)
//        {
//            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
//            int? getuserid = getidentity.ID(User.Identity.Name);
//            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
//            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
//            int year = Minus1month.Year;
//            int month = Minus1month.Month;
//            int drpyear = 0;
//            int drprangeyear = 0;
//            //List<SelectListItem> SelectionData = new List<SelectListItem>();

//            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
//            drprangeyear = timezone.gettimezone().Year;

//            var yearlist = new List<SelectListItem>();
//            for (var i = drpyear; i <= drprangeyear; i++)
//            {
//                if (i == YearList)
//                {
//                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
//                }
//                else
//                {
//                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
//                }
//            }

//            ViewBag.NamaSyarikat = db.tbl_Syarikat
//                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
//                .Select(s => s.fld_NamaSyarikat)
//                .FirstOrDefault();
//            ViewBag.NoSyarikat = db.tbl_Syarikat
//                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
//                .Select(s => s.fld_NoSyarikat)
//                .FirstOrDefault();

//            var GetAuditTrail = db.tbl_AuditTrail.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Thn == YearList).FirstOrDefault();

//            ViewBag.YearList = yearlist;
//            ViewBag.Tahun = YearList;
//            return View("AuditTrail", GetAuditTrail);
//        }

//        public void UpdateAuditTrail(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Year, int? Month, int? UpdateData)
//        {
//            var checkAuditTrail = db.tbl_AuditTrail.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Thn == Year).FirstOrDefault();
//            switch (Month)
//            {
//                case 1:
//                    checkAuditTrail.fld_Bln1 = UpdateData;
//                    break;
//                case 2:
//                    checkAuditTrail.fld_Bln2 = UpdateData;
//                    break;
//                case 3:
//                    checkAuditTrail.fld_Bln3 = UpdateData;
//                    break;
//                case 4:
//                    checkAuditTrail.fld_Bln4 = UpdateData;
//                    break;
//                case 5:
//                    checkAuditTrail.fld_Bln5 = UpdateData;
//                    break;
//                case 6:
//                    checkAuditTrail.fld_Bln6 = UpdateData;
//                    break;
//                case 7:
//                    checkAuditTrail.fld_Bln7 = UpdateData;
//                    break;
//                case 8:
//                    checkAuditTrail.fld_Bln8 = UpdateData;
//                    break;
//                case 9:
//                    checkAuditTrail.fld_Bln9 = UpdateData;
//                    break;
//                case 10:
//                    checkAuditTrail.fld_Bln10 = UpdateData;
//                    break;
//                case 11:
//                    checkAuditTrail.fld_Bln11 = UpdateData;
//                    break;
//                case 12:
//                    checkAuditTrail.fld_Bln12 = UpdateData;
//                    break;
//            }

//            db.Entry(checkAuditTrail).State = EntityState.Modified;
//            db.SaveChanges();
//        }

//        public void FinanceApplication(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Year, int? Month, bool? UrusniagaStatus, decimal? Amount, string SkbNo, int? UserID)
//        {
//            var CheckPermohonanWang = db.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month).FirstOrDefault();
//            var GetLadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();
//            if (CheckPermohonanWang == null)
//            {
//                tbl_SokPermhnWang tbl_SokPermhnWang = new tbl_SokPermhnWang();
//                tbl_SokPermhnWang.fld_SemakWil_Status = 0;
//                tbl_SokPermhnWang.fld_SokongWilGM_Status = 0;
//                tbl_SokPermhnWang.fld_TerimaHQ_Status = 0;
//                tbl_SokPermhnWang.fld_TolakWil_Status = 0;
//                tbl_SokPermhnWang.fld_TolakWilGM_Status = 0;
//                tbl_SokPermhnWang.fld_TolakHQ_Status = 0;
//                tbl_SokPermhnWang.fld_NoCIT = GetLadangDetail.fld_NoCIT;
//                tbl_SokPermhnWang.fld_NoAcc = GetLadangDetail.fld_NoAcc;
//                tbl_SokPermhnWang.fld_NoGL = GetLadangDetail.fld_NoGL;
//                tbl_SokPermhnWang.fld_JumlahPermohonan = Amount;
//                tbl_SokPermhnWang.fld_SkbNo = SkbNo;
//                tbl_SokPermhnWang.fld_StsTtpUrsNiaga = true;
//                tbl_SokPermhnWang.fld_NegaraID = NegaraID;
//                tbl_SokPermhnWang.fld_SyarikatID = SyarikatID;
//                tbl_SokPermhnWang.fld_WilayahID = WilayahID;
//                tbl_SokPermhnWang.fld_LadangID = LadangID;
//                tbl_SokPermhnWang.fld_Year = Year;
//                tbl_SokPermhnWang.fld_Month = Month;
//                db.tbl_SokPermhnWang.Add(tbl_SokPermhnWang);
//                db.SaveChanges();
//            }
//            else
//            {
//                CheckPermohonanWang.fld_SemakWil_Status = 0;
//                CheckPermohonanWang.fld_SokongWilGM_Status = 0;
//                CheckPermohonanWang.fld_TerimaHQ_Status = 0;
//                CheckPermohonanWang.fld_TolakWil_Status = 0;
//                CheckPermohonanWang.fld_TolakWilGM_Status = 0;
//                CheckPermohonanWang.fld_TolakHQ_Status = 0;
//                CheckPermohonanWang.fld_NoCIT = GetLadangDetail.fld_NoCIT;
//                CheckPermohonanWang.fld_NoAcc = GetLadangDetail.fld_NoAcc;
//                CheckPermohonanWang.fld_NoGL = GetLadangDetail.fld_NoGL;
//                CheckPermohonanWang.fld_JumlahPermohonan = Amount;
//                CheckPermohonanWang.fld_SkbNo = SkbNo;
//                CheckPermohonanWang.fld_StsTtpUrsNiaga = UrusniagaStatus;
//                db.Entry(CheckPermohonanWang).State = EntityState.Modified;
//                db.SaveChanges();

//                if (UrusniagaStatus == false)
//                {
//                    tblSokPermhnWangHisAction tblSokPermhnWangHisAction = new tblSokPermhnWangHisAction();
//                    tblSokPermhnWangHisAction.fldHisSPWID = CheckPermohonanWang.fld_ID;
//                    tblSokPermhnWangHisAction.fldHisDesc = "Urus Niaga Dibuka Semula";
//                    tblSokPermhnWangHisAction.fldHisUserID = UserID;
//                    tblSokPermhnWangHisAction.fldHisAppLevel = 2;
//                    tblSokPermhnWangHisAction.fldHisDT = timezone.gettimezone();
//                    db.tblSokPermhnWangHisActions.Add(tblSokPermhnWangHisAction);
//                    db.SaveChanges();
//                }
//            }
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//                //db2.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}