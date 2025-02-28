using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Web.Mvc;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ViewingModels;
using System.Collections.Generic;
using MVC_SYSTEM.App_LocalResources;
using System.Web;
using System.IO;
using MVC_SYSTEM.Attributes;
using System.Text.RegularExpressions;
using MVC_SYSTEM.CustomModels;
using static MVC_SYSTEM.Class.GlobalFunction;
using System.Globalization;
using Itenso.TimePeriod;

namespace MVC_SYSTEM.Controllers
{
    public class GenTextFileController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        ChangeTimeZone timezone = new ChangeTimeZone();
        GetConfig GetConfig = new GetConfig();
        private GetGenerateEwalletFile GetGenerateEwalletFile = new GetGenerateEwalletFile();
        private errorlog geterror = new errorlog();

        // GET: GenTextFile
        public ActionResult Index()
        {

            int? getuserid = getidentity.ID(User.Identity.Name);
            int? getroleid = getidentity.getRoleID(getuserid);
            int?[] reportid = new int?[] { };
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.GenTextFile = "class = active";
            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.MenuSubList = sublist;
            ViewBag.MenuList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "GenTxtFile" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            db.Dispose();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string MenuList, string MenuSubList)
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (MenuSubList != null)
            {
                if (MenuSubList.Contains("|"))
                {
                    string[] split = MenuSubList.Split('|');
                    return RedirectToAction(split[1], split[0]);
                }
                else
                    return RedirectToAction(MenuSubList, "GenTextFile");
            }
            else
            {
                int menulist = int.Parse(MenuList);
                var action = db.tblMenuLists.Where(x => x.fld_ID == menulist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "MaybankFileGen");
            }
        }

        public ActionResult eWallet()
        {
            ViewBag.GenTextFile = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
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

            ViewBag.YearList = yearlist;

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            var monthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);

            ViewBag.MonthList = monthList;
            ViewBag.StatusList = statusList;

            return View();
        }

        //added by faeza 26.02.2023
        public ActionResult eWalletInsentive()
        {
            ViewBag.GenTextFile = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
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

            ViewBag.YearList = yearlist;

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            var monthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);

            //add by faeza 09.04.2024
            var incentiveList = new List<SelectListItem>();
            incentiveList = new SelectList(db.tbl_JenisInsentif.Where(x => x.fld_InclSecondPayslip == true && x.fld_JenisInsentif == "P" && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodInsentif).Select(s => new SelectListItem { Value = s.fld_KodInsentif, Text = s.fld_Keterangan }), "Value", "Text").ToList();

            ViewBag.MonthList = monthList;
            ViewBag.StatusList = statusList;
            ViewBag.IncentiveList = incentiveList;

            return View();
        }

        public ViewResult _eWallet(int? MonthList, int? YearList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string LdgName = "";
            string LdgCode = "";
            string TelNo, NewTelNo, NoKp, NewNoKp = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            //List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<vw_PaySheetPekerjaCustomModel>();
            List<vw_PaySheetPekerja> PaySheetPekerjaList = new List<vw_PaySheetPekerja>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            LdgName = db.tbl_Ladang
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
                .Select(s => s.fld_LdgName)
                .FirstOrDefault();
            LdgCode = db.tbl_Ladang
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
                .Select(s => s.fld_LdgCode)
                .FirstOrDefault();
            ViewBag.Ladang = LdgName.Trim();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;
            ViewBag.Description = LdgCode + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                //return View();
                return View(PaySheetPekerjaList);
            }
            else
            {
                //IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                var salaryData = dbview.vw_PaySheetPekerja
                    .Where(x => x.fld_Year == YearList && x.fld_Month == MonthList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_DivisionID == DivisionID && x.fld_PaymentMode == "3")
                    .OrderBy(x => x.fld_Nama);


                foreach (var salary in salaryData)
                {
                    //remove space &special char NoTel
                    TelNo = salary.fld_Notel;
                    NewTelNo = Regex.Replace(TelNo, @"[^0-9]+", "");

                    if (NewTelNo.Substring(0, 1) == "0")
                    {
                        TelNo = "6" + NewTelNo;
                    }
                    else
                    {
                        TelNo = NewTelNo;
                    }

                    //remove space & special char NoKp
                    NoKp = salary.fld_Nokp;
                    NewNoKp = Regex.Replace(NoKp, @"[^0-9a-zA-Z]+", "");

                    PaySheetPekerjaList.Add(
                        new vw_PaySheetPekerja()
                        {
                            fld_Nopkj = salary.fld_Nopkj,
                            fld_Nama = salary.fld_Nama,
                            fld_Notel = TelNo,
                            fld_Nokp = NewNoKp,
                            fld_Last4Pan = salary.fld_Last4Pan,
                            fld_GajiBersih = salary.fld_GajiBersih
                        });


                    //PaySheetPekerjaList.Add(
                    //    new vw_PaySheetPekerjaCustomModel()
                    //    {
                    //        PaySheetPekerja = salary
                    //    });
                }

                ViewBag.RecordNo = PaySheetPekerjaList.Count();

                if (PaySheetPekerjaList.Count() == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                }
                return View(PaySheetPekerjaList);
            }
        }

        //added by faeza 26.02.2023
        public ViewResult _eWalletInsentive(int? MonthList, int? YearList, string IncentiveList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string LdgName = "";
            string LdgCode = "";
            string IncentiveDescription = "";
            string TelNo, NewTelNo, NoKp, NewNoKp = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<vw_SpecialInsentive> SpecialInsentiveList = new List<vw_SpecialInsentive>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            LdgName = db.tbl_Ladang
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
                .Select(s => s.fld_LdgName)
                .FirstOrDefault();
            LdgCode = db.tbl_Ladang
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
                .Select(s => s.fld_LdgCode)
                .FirstOrDefault();
            IncentiveDescription = db.tbl_JenisInsentif
                .Where(x => x.fld_KodInsentif == IncentiveList && x.fld_JenisInsentif == "P" && x.fld_Deleted == false && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_Keterangan)
                .FirstOrDefault();

            ViewBag.Ladang = LdgName.Trim();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;
            ViewBag.Description = LdgCode + " " + IncentiveDescription + " " + MonthList + "/" + YearList;
            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                //return View();
                return View(SpecialInsentiveList);
            }
            else
            {
                var salaryData = dbview.vw_SpecialInsentive
                    .Where(x => x.fld_Year == YearList && x.fld_Month == MonthList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_DivisionID == DivisionID && x.fld_PaymentMode == "3" && x.fld_KodInsentif == IncentiveList)
                    .OrderBy(x => x.fld_Nama);


                foreach (var salary in salaryData)
                {
                    //remove space &special char NoTel
                    TelNo = salary.fld_Notel;
                    NewTelNo = Regex.Replace(TelNo, @"[^0-9]+", "");

                    if (NewTelNo.Substring(0, 1) == "0")
                    {
                        TelNo = "6" + NewTelNo;
                    }
                    else
                    {
                        TelNo = NewTelNo;
                    }

                    //remove space & special char NoKp
                    NoKp = salary.fld_Nokp;
                    NewNoKp = Regex.Replace(NoKp, @"[^0-9a-zA-Z]+", "");

                    SpecialInsentiveList.Add(
                        new vw_SpecialInsentive()
                        {
                            fld_Nopkj = salary.fld_Nopkj,
                            fld_Nama = salary.fld_Nama,
                            fld_Notel = TelNo,
                            fld_Nokp = NewNoKp,
                            fld_Last4Pan = salary.fld_Last4Pan,
                            //fld_NilaiInsentif = salary.fld_NilaiInsentif,
                            fld_GajiBersih = salary.fld_GajiBersih,
                            fld_Keterangan = salary.fld_Keterangan //added by faeza 18.04.2023
                        });
                }

                ViewBag.RecordNo = SpecialInsentiveList.Count();

                if (SpecialInsentiveList.Count() == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                }
                return View(SpecialInsentiveList);
            }
        }

        public JsonResult GetEwalletRecord(int Month, int Year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            decimal? TotalSalary = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<vw_PaySheetPekerjaCustomModel>();

            var salaryData = dbview.vw_PaySheetPekerja
                   .Where(x => x.fld_Year == Year && x.fld_Month == Month &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID &&
                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                               x.fld_DivisionID == DivisionID && x.fld_PaymentMode == "3")
                   .OrderBy(x => x.fld_Nama).ToList();

            var LadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();

            //string filename = "MBBOPMS" + LadangDetail.fld_LdgCode + stringmonth + stringyear + ".txt";

            if (salaryData.Count() != 0)
            {
                TotalSalary = salaryData.Sum(s => s.fld_GajiBersih);
                CountData = salaryData.Count();
                msg = GlobalResEstate.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResEstate.msgDataNotFound;
                statusmsg = "warning";
            }

            dbview.Dispose();
            return Json(new { msg, statusmsg, salary = TotalSalary, recordno = CountData });

        }

        //added by faeza 26.02.2023
        public JsonResult GetEwalletInsentiveRecord(int Month, int Year, string Incentive)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            decimal? TotalSalary = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<vw_PaySheetPekerjaCustomModel>();

            var salaryData = dbview.vw_SpecialInsentive
                   .Where(x => x.fld_Year == Year && x.fld_Month == Month &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID &&
                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                               x.fld_DivisionID == DivisionID && x.fld_PaymentMode == "3" && x.fld_KodInsentif == Incentive)
                   .OrderBy(x => x.fld_Nama).ToList();

            var LadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();

            //string filename = "MBBOPMS" + LadangDetail.fld_LdgCode + stringmonth + stringyear + ".txt";

            if (salaryData.Count() != 0)
            {
                //TotalSalary = salaryData.Sum(s => s.fld_NilaiInsentif);
                TotalSalary = salaryData.Sum(s => s.fld_GajiBersih);
                CountData = salaryData.Count();
                msg = GlobalResEstate.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResEstate.msgDataNotFound;
                statusmsg = "warning";
            }

            dbview.Dispose();
            return Json(new { msg, statusmsg, salary = TotalSalary, recordno = CountData });

        }

        [HttpPost]
        public ActionResult _eWallet(int Month, int Year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? DivisionID = 0;
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            ViewBag.GenTextFile = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
                MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
                DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);

                var salaryData = dbview.vw_PaySheetPekerja.Where(x => x.fld_Year == Year && x.fld_Month == Month && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID && x.fld_PaymentMode == "3").OrderBy(x => x.fld_Nama).ToList();

                var LadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();

                filePath = GetGenerateEwalletFile.GenFileEwallet(salaryData, LadangDetail, stringmonth, stringyear, NegaraID, SyarikatID, WilayahID, LadangID, out filename);

                link = Url.Action("Download", "GenTextFile", new { filePath, filename });

                dbr.Dispose();

                msg = GlobalResEstate.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResEstate.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

        //added by faeza 26.02.2023
        public ActionResult _eWalletInsentiveGen(int Month, int Year, string Incentive)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? DivisionID = 0;
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            ViewBag.GenTextFile = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
                MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
                DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);

                var salaryData = dbview.vw_SpecialInsentive.Where(x => x.fld_Year == Year && x.fld_Month == Month && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID && x.fld_PaymentMode == "3" && x.fld_KodInsentif == Incentive).OrderBy(x => x.fld_Nama).ToList();

                var LadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();

                filePath = GetGenerateEwalletIncentiveFile.GenFileEwallet(salaryData, LadangDetail, stringmonth, stringyear, NegaraID, SyarikatID, WilayahID, LadangID, out filename);

                link = Url.Action("Download", "GenTextFile", new { filePath, filename });

                dbr.Dispose();

                msg = GlobalResEstate.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResEstate.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

        public FileResult Download(string filePath, string filename)
        {
            string path = HttpContext.Server.MapPath(filePath);

            DownloadFiles.FileDownloads objs = new DownloadFiles.FileDownloads();

            var filesCol = objs.GetFiles(path);
            var CurrentFileName = filesCol.Where(x => x.FileName == filename).FirstOrDefault();

            string contentType = string.Empty;
            contentType = "application/txt";
            return File(CurrentFileName.FilePath, contentType, CurrentFileName.FileName);
        }

        public JsonResult GetSubList(int ListID)
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
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


        public ActionResult ewalletindividu()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.GenTextFile = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

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

            ViewBag.UserID = getuserid;
            return View();
        }

        public ViewResult _ewalletindividu(int? MonthList, int? YearList, string print, string filter, string[] WorkerId)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<vw_MaybankRcms> maybankrcmsList = new List<vw_MaybankRcms>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            var company = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            ViewBag.NamaSyarikat = company.fld_NamaSyarikat;
            ViewBag.NamaPendekSyarikat = company.fld_NamaPndkSyarikat;
            ViewBag.NoSyarikat = company.fld_NoSyarikat;
            ViewBag.CorpID = company.fld_CorporateID;
            var ClientId = company.fld_ClientBatchID;

            if (ClientId == null || ClientId == "")
            {
                if (company.fld_NamaPndkSyarikat == "FASSB")
                {
                    ViewBag.clientid = "FGVASB" + MonthList + YearList;
                }

                if (company.fld_NamaPndkSyarikat == "RNDSB")
                {
                    ViewBag.clientid = "RNDSB" + MonthList + YearList;
                }
            }
            else
            {
                ViewBag.clientid = ClientId;
            }

            ViewBag.AccNo = company.fld_AccountNo;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            //ViewBag.WilayahName = WilayahName;

            ViewBag.Description = "Region " + company.fld_NamaSyarikat + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = "Please select month, year, company and payment date";
                return View(maybankrcmsList);
            }
            else
            {
                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbr.vw_MaybankRcms.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).ToList();
                }
                else
                {
                    maybankrcmsList = dbr.vw_MaybankRcms.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && WorkerId.Contains(x.fld_Nopkj)).ToList();
                }
                
                var BankList = db.tbl_Bank
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .ToList();

                ViewBag.RecordNo = maybankrcmsList.Count();

                if (maybankrcmsList.Count() == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                }

                if (filter != "")
                {
                    ViewBag.filter = filter;
                }
                return View(maybankrcmsList);
            }
        }

        public ActionResult ewalletInsentiveindividu()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.GenTextFile = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

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

            ViewBag.UserID = getuserid;
            return View();
        }

        public ViewResult _ewalletInsentiveindividu(int? MonthList, int? YearList, string print, string filter, string[] WorkerId)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<vw_MaybankRcmsOthers> maybankrcmsList = new List<vw_MaybankRcmsOthers>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            var company = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            ViewBag.NamaSyarikat = company.fld_NamaSyarikat;
            ViewBag.NamaPendekSyarikat = company.fld_NamaPndkSyarikat;
            ViewBag.NoSyarikat = company.fld_NoSyarikat;
            ViewBag.CorpID = company.fld_CorporateID;
            var ClientId = company.fld_ClientBatchID;

            if (ClientId == null || ClientId == "")
            {
                if (company.fld_NamaPndkSyarikat == "FASSB")
                {
                    ViewBag.clientid = "FGVASB" + MonthList + YearList;
                }

                if (company.fld_NamaPndkSyarikat == "RNDSB")
                {
                    ViewBag.clientid = "RNDSB" + MonthList + YearList;
                }
            }
            else
            {
                ViewBag.clientid = ClientId;
            }

            ViewBag.AccNo = company.fld_AccountNo;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            //ViewBag.WilayahName = WilayahName;

            ViewBag.Description = "Region " + company.fld_NamaSyarikat + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = "Please select month, year, company and payment date";
                return View(maybankrcmsList);
            }
            else
            {
                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbr.vw_MaybankRcmsOthers.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).ToList();
                }
                else
                {
                    maybankrcmsList = dbr.vw_MaybankRcmsOthers.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var BankList = db.tbl_Bank
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .ToList();

                ViewBag.RecordNo = maybankrcmsList.Count();

                if (maybankrcmsList.Count() == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                }

                if (filter != "")
                {
                    ViewBag.filter = filter;
                }
                return View(maybankrcmsList);
            }
        }

        public JsonResult GetWorker(int Year, int Month)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> workerList = new List<SelectListItem>();
            var maybankrcmsList = dbr.vw_MaybankRcms.Where(x => x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month).Select(s => new { s.fld_Nopkj, s.fld_Nama }).OrderBy(o => o.fld_Nama).ToList();

            if (maybankrcmsList.Count() > 0)
            {
                workerList = new SelectList(maybankrcmsList.Select(s => new SelectListItem { Value = s.fld_Nopkj.ToString(), Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();
            }

            return Json(workerList);
        }

        public JsonResult GetWorker2(int Year, int Month)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> workerList = new List<SelectListItem>();
            var maybankrcmsList = dbr.vw_MaybankRcmsOthers.Where(x => x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month).Select(s => new { s.fld_Nopkj, s.fld_Nama }).OrderBy(o => o.fld_Nama).ToList();

            if (maybankrcmsList.Count() > 0)
            {
                workerList = new SelectList(maybankrcmsList.Select(s => new SelectListItem { Value = s.fld_Nopkj.ToString(), Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();
            }

            return Json(workerList);
        }

        public JsonResult CheckGenDataDetail(int Month, int Year, string[] WorkerId)
        {
            string msg = "";
            string statusmsg = "";

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string stringyear = "";
            string stringmonth = "";
            string CorpID = "";
            string ClientID = "";
            string ClientIDText = "";
            string AccNo = "";
            string InitialName = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalGaji = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            GetNSWL.GetSyarikatRCMSDetail(SyarikatID, out CorpID, out ClientID, out AccNo, out InitialName);

            List<vw_MaybankRcms> maybankrcmsList = new List<vw_MaybankRcms>();
            if (WorkerId == null)
                WorkerId = new string[] { "0" };

            if (WorkerId.Contains("0"))
            {
                maybankrcmsList = dbr.vw_MaybankRcms.Where(x => x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month).ToList();
            }
            else
            {
                maybankrcmsList = dbr.vw_MaybankRcms.Where(x => x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month && WorkerId.Contains(x.fld_Nopkj)).ToList();
            }
            var SyarikatDetail = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            string filename = "M2E LABOR (" + SyarikatDetail.fld_NamaPndkSyarikat.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";

            if (ClientID == null || ClientID == " ")
            {
                if (SyarikatDetail.fld_NamaPndkSyarikat == "FASSB")
                {
                    ClientIDText = "FGVASB" + stringmonth + stringyear;
                }

                if (SyarikatDetail.fld_NamaPndkSyarikat == "RNDSB")
                {
                    ClientIDText = "RNDSB" + stringmonth + stringyear;
                }
            }
            else
            {
                ClientIDText = ClientID;
            }

            if (maybankrcmsList.Count() != 0)
            {
                TotalGaji = maybankrcmsList.Sum(s => s.fld_GajiBersih);
                CountData = maybankrcmsList.Count();
                msg = GlobalResEstate.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResEstate.msgDataNotFound;
                statusmsg = "warning";
            }

            db.Dispose();
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData, clientid = ClientIDText });
        }

        [HttpPost]
        public ActionResult DownloadText(int Month, int Year, string filter, string[] WorkerId, DateTime PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                List<vw_MaybankRcms> maybankrcmsList = new List<vw_MaybankRcms>();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbr.vw_MaybankRcms.Where(x => x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month).ToList();
                }
                else
                {
                    maybankrcmsList = dbr.vw_MaybankRcms.Where(x => x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month && WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var SyarikatDetail = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                filePath = GetGenerateFile.GenerateFileMaybank(maybankrcmsList, SyarikatDetail, stringmonth, stringyear, NegaraID, SyarikatID, filter, PaymentDate, out filename);

                link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

                //dbr.Dispose();

                msg = GlobalResEstate.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResEstate.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

        public JsonResult CheckGenDataDetail2(int Month, int Year, string[] WorkerId)
        {
            string msg = "";
            string statusmsg = "";

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string stringyear = "";
            string stringmonth = "";
            string CorpID = "";
            string ClientID = "";
            string ClientIDText = "";
            string AccNo = "";
            string InitialName = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalGaji = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            GetNSWL.GetSyarikatRCMSDetail(SyarikatID, out CorpID, out ClientID, out AccNo, out InitialName);

            List<vw_MaybankRcmsOthers> maybankrcmsList = new List<vw_MaybankRcmsOthers>();
            if (WorkerId == null)
                WorkerId = new string[] { "0" };

            if (WorkerId.Contains("0"))
            {
                maybankrcmsList = dbr.vw_MaybankRcmsOthers.Where(x => x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month).ToList();
            }
            else
            {
                maybankrcmsList = dbr.vw_MaybankRcmsOthers.Where(x => x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month && WorkerId.Contains(x.fld_Nopkj)).ToList();
            }
            var SyarikatDetail = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            string filename = "M2E LABOR (" + SyarikatDetail.fld_NamaPndkSyarikat.ToUpper() + ") " + "" + stringmonth + stringyear + ".txt";

            if (ClientID == null || ClientID == " ")
            {
                if (SyarikatDetail.fld_NamaPndkSyarikat == "FASSB")
                {
                    ClientIDText = "FGVASB" + stringmonth + stringyear;
                }

                if (SyarikatDetail.fld_NamaPndkSyarikat == "RNDSB")
                {
                    ClientIDText = "RNDSB" + stringmonth + stringyear;
                }
            }
            else
            {
                ClientIDText = ClientID;
            }

            if (maybankrcmsList.Count() != 0)
            {
                TotalGaji = maybankrcmsList.Sum(s => s.fld_GajiBersih);
                CountData = maybankrcmsList.Count();
                msg = GlobalResEstate.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResEstate.msgDataNotFound;
                statusmsg = "warning";
            }

            db.Dispose();
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData, clientid = ClientIDText });
        }

        [HttpPost]
        public ActionResult DownloadText2(int Month, int Year, string filter, string[] WorkerId, DateTime PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                List<vw_MaybankRcmsOthers> maybankrcmsList = new List<vw_MaybankRcmsOthers>();

                if (WorkerId == null)
                    WorkerId = new string[] { "0" };

                if (WorkerId.Contains("0"))
                {
                    maybankrcmsList = dbr.vw_MaybankRcmsOthers.Where(x => x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month).ToList();
                }
                else
                {
                    maybankrcmsList = dbr.vw_MaybankRcmsOthers.Where(x => x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month && WorkerId.Contains(x.fld_Nopkj)).ToList();
                }

                var SyarikatDetail = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();

                filePath = GetGenerateFile.GenerateFileMaybank2(maybankrcmsList, SyarikatDetail, stringmonth, stringyear, NegaraID, SyarikatID, filter, PaymentDate, out filename);

                link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

                //dbr.Dispose();

                msg = GlobalResEstate.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResEstate.msgGenerateFailed;
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg, link });
        }

    }
}