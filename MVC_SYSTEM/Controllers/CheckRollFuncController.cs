using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.CustomModels;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class CheckRollFuncController : Controller
    {
        private GetIdentity GetIdentity = new GetIdentity();
        private Connection Connection = new Connection();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetConfig GetConfig = new GetConfig();
        private ChangeTimeZone ChangeTimeZone = new ChangeTimeZone();
        private CheckrollFunction EstateFunction = new CheckrollFunction();
        private GetLadang GetLadang = new GetLadang();
        private GetIdentity getidentity = new GetIdentity();
        // GET: CheckRollAtt
        public ActionResult AttendanceForm()
        {
            string host, catalog, user, pass = "";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            DateTime DateTimeNow = ChangeTimeZone.gettimezone();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

            List<SelectListItem> GroupSelection = new List<SelectListItem>();
            List<SelectListItem> KodTakHadirCuti1 = new List<SelectListItem>();
            List<SelectListItem> KodTakHadirCuti2 = new List<SelectListItem>();

            GroupSelection = new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }), "Value", "Text").ToList();
            GroupSelection.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "ALL" }));
            string DateTimeConvertString = DateTimeNow.ToString("d");
            DateTime DateTimeConvert = Convert.ToDateTime(DateTimeConvertString);
            KodTakHadirCuti1 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "cuti" && x.fldOptConfFlag2 == "xhadirkerja" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            KodTakHadirCuti2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "cuti" && x.fldOptConfFlag2 == "hadirkerja" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            ViewBag.GroupSelection = GroupSelection;
            ViewBag.DateTimeNow = DateTimeConvert;
            ViewBag.KodTakHadirCuti1 = KodTakHadirCuti1;
            ViewBag.KodTakHadirCuti2 = KodTakHadirCuti2;

            dbr.Dispose();
            return View();
        }

        public PartialViewResult _AttendanceForm(string GroupSelection, DateTime SelectedDate)
        {
            string host, catalog, user, pass = "";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int KumpulanID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<tbl_Pkjmast> tbl_Pkjmasts = new List<tbl_Pkjmast>();
            List<tbl_Pkjmast> tbl_PkjmastsForLookUp = new List<tbl_Pkjmast>();
            List<tbl_Kerjahdr> tbl_Kerjahdrs = new List<tbl_Kerjahdr>();
            List<CustMod_Kerjahdr> CustMod_Kerjahdrs = new List<CustMod_Kerjahdr>();
            tbl_PkjmastsForLookUp = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").ToList();
            if (GroupSelection == "ALL")
            {
                tbl_Pkjmasts = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null).ToList();
                tbl_Kerjahdrs = dbr.tbl_Kerjahdr.Where(x => x.fld_Tarikh == SelectedDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            }
            else
            {
                KumpulanID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == GroupSelection && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KumpulanID).FirstOrDefault();
                tbl_Pkjmasts = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").ToList();
                tbl_Kerjahdrs = dbr.tbl_Kerjahdr.Where(x => x.fld_Kum == GroupSelection && x.fld_Tarikh == SelectedDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            }

            var GetPkjRecordHdrs = tbl_Kerjahdrs.Select(s => s.fld_Nopkj).ToList();
            var GetPkjAllRecords = tbl_Pkjmasts.Select(s => s.fld_Nopkj).ToList();
            List<string> GetPkjXRecordHdrs = GetPkjAllRecords.Except(GetPkjRecordHdrs).ToList();

            if (GetPkjXRecordHdrs.Count() > 0)
            {
                foreach (var GetPkjXRecordHdr in GetPkjXRecordHdrs)
                {
                    var PkjData = tbl_Pkjmasts.Where(x => x.fld_Nopkj.Trim() == GetPkjXRecordHdr.Trim()).FirstOrDefault();
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr { fld_Nopkj = PkjData.fld_Nopkj.Trim(), fld_Nama = PkjData.fld_Nama, fld_HdrCt = "-", fld_Status = "Tiada Rekod" });
                }

                foreach (var tbl_Kerjahdr in tbl_Kerjahdrs)
                {
                    var PkjData = tbl_PkjmastsForLookUp.Where(x => x.fld_Nopkj.Trim() == tbl_Kerjahdr.fld_Nopkj.Trim()).FirstOrDefault();
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr { fld_Nopkj = PkjData.fld_Nopkj.Trim(), fld_Nama = PkjData.fld_Nama.Trim(), fld_HdrCt = GetConfig.GetWebConfigDesc(tbl_Kerjahdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_Status = EstateFunction.GetHadirStatus(tbl_Kerjahdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_UniqueID = tbl_Kerjahdr.fld_UniqueID });
                }
            }
            else
            {
                foreach (var tbl_Kerjahdr in tbl_Kerjahdrs)
                {
                    var PkjData = tbl_PkjmastsForLookUp.Where(x => x.fld_Nopkj.Trim() == tbl_Kerjahdr.fld_Nopkj.Trim()).FirstOrDefault();
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr { fld_Nopkj = PkjData.fld_Nopkj.Trim(), fld_Nama = PkjData.fld_Nama, fld_HdrCt = GetConfig.GetWebConfigDesc(tbl_Kerjahdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_Status = EstateFunction.GetHadirStatus(tbl_Kerjahdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_UniqueID = tbl_Kerjahdr.fld_UniqueID });
                }
            }
            ViewBag.SelectedDate = SelectedDate.ToString("dd-MM-yyyy");
            ViewBag.TotalKhdrnPkrja = tbl_Kerjahdrs.Count();
            ViewBag.TotalPkrja = GetPkjAllRecords.Count();
            ViewBag.TotalPkrjaXHdr = CustMod_Kerjahdrs.Where(x => x.fld_Status == "Tidak Hadir").Count();
            ViewBag.TotalPkrjaHdr = CustMod_Kerjahdrs.Where(x => x.fld_Status == "Hadir").Count();
            return PartialView(CustMod_Kerjahdrs.OrderBy(o => o.fld_Nama));
        }

        public JsonResult _AttendanceFormChange(DateTime SelectedDate, string GroupSelection)
        {
            string Msg = "";
            string StatusMsg = "";
            bool GnrlStatus = false;
            GroupSelection = "ALL";

            Msg = "Change Date";
            StatusMsg = "success";

            string UrlLoad = Url.Action("_AttendanceForm", "CheckRollFunc", new { GroupSelection, SelectedDate }, "http");
            return Json(new { UrlLoad, Msg, StatusMsg, GnrlStatus });
        }

        public JsonResult _AttendanceFormDelete(Guid Data, DateTime SelectedDate, string GroupSelection)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string Msg = "";
            string StatusMsg = "";
            bool GnrlStatus = false;
            GroupSelection = "ALL";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetKerjaHdr = dbr.tbl_Kerjahdr.Find(Data);

            if (EstateFunction.IndividuCheckLeaveTake(GetKerjaHdr.fld_Kdhdct, NegaraID, SyarikatID))
            {
                EstateFunction.LeaveAdd(dbr, GetKerjaHdr.fld_Tarikh.Value.Year, GetKerjaHdr.fld_Nopkj, GetKerjaHdr.fld_Kdhdct, NegaraID, SyarikatID, WilayahID, LadangID);
            }

            if (GetKerjaHdr != null)
            {
                var GetKerja = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh == GetKerjaHdr.fld_Tarikh && x.fld_Nopkj == GetKerjaHdr.fld_Nopkj).ToList();
                if (GetKerja.Count > 0)
                {
                    dbr.tbl_Kerja.RemoveRange(GetKerja);
                }
                dbr.tbl_Kerjahdr.Remove(GetKerjaHdr);
                dbr.SaveChanges();
                Msg = GlobalResEstate.msgDelete2;
                StatusMsg = "success";
            }

            string UrlLoad = Url.Action("_AttendanceForm", "CheckRollFunc", new { GroupSelection, SelectedDate }, "http");
            return Json(new { UrlLoad, Msg, StatusMsg, GnrlStatus });
        }

        public JsonResult _AttendanceFormSave(string NoPkj, string KodHadirCuti, DateTime SelectedDate, short StatusHadir, string GroupSelection)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string Msg = "";
            string Msg2 = "";
            string StatusMsg = "";
            bool GnrlStatus = false;
            int? LadangNegeriCode = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            DateTime DateTimeNow = ChangeTimeZone.gettimezone();
            GroupSelection = "ALL";
            bool CutOfDateStatus = false;
            bool LeaveSelection = false;
            tbl_Kerjahdr tbl_Kerjahdr = new tbl_Kerjahdr();
            try
            {
                LadangNegeriCode = int.Parse(GetLadang.GetLadangNegeriCode(LadangID));
                if (EstateFunction.GetCutiAmMgguMatchDate(NegaraID, SyarikatID, WilayahID, LadangID, SelectedDate, KodHadirCuti, out Msg))
                {
                    CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbr, SelectedDate, NegaraID, SyarikatID, WilayahID, LadangID);
                    if (!CutOfDateStatus)
                    {
                        //hadir
                        if (StatusHadir == 1)
                        {
                            var DataHdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Tarikh == SelectedDate && x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                            if (DataHdr == 0)
                            {
                                var pkjdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Select(s => new { s.fld_Nopkj, s.fld_KumpulanID }).FirstOrDefault();
                                var KumpulanKod = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjdata.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).FirstOrDefault();

                                tbl_Kerjahdr.fld_Nopkj = NoPkj;
                                tbl_Kerjahdr.fld_Tarikh = SelectedDate;
                                tbl_Kerjahdr.fld_DataSource = "B";
                                tbl_Kerjahdr.fld_Hujan = 0;
                                tbl_Kerjahdr.fld_Kum = KumpulanKod;
                                tbl_Kerjahdr.fld_Kdhdct = KodHadirCuti;
                                tbl_Kerjahdr.fld_NegaraID = NegaraID;
                                tbl_Kerjahdr.fld_SyarikatID = SyarikatID;
                                tbl_Kerjahdr.fld_WilayahID = WilayahID;
                                tbl_Kerjahdr.fld_LadangID = LadangID;
                                tbl_Kerjahdr.fld_CreatedBy = getuserid;
                                tbl_Kerjahdr.fld_CreatedDT = DateTimeNow;

                                dbr.tbl_Kerjahdr.Add(tbl_Kerjahdr);
                                dbr.SaveChanges();

                                Msg = "Save Successfully";
                                StatusMsg = "success";
                                GnrlStatus = true;
                            }
                            else
                            {
                                Msg = "Already Saved";
                                StatusMsg = "warning";
                                GnrlStatus = false;
                            }
                        }
                        else //tak hadir
                        {
                            LeaveSelection = EstateFunction.CheckLeaveType(KodHadirCuti, NegaraID, SyarikatID) ? true : false;
                            if (LeaveSelection)
                            {
                                if (EstateFunction.LeaveCalBal(dbr, SelectedDate.Year, NoPkj, KodHadirCuti, NegaraID, SyarikatID, WilayahID, LadangID))
                                {
                                    var DataHdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Tarikh == SelectedDate && x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                                    if (DataHdr == 0)
                                    {
                                        var pkjdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Select(s => new { s.fld_Nopkj, s.fld_KumpulanID }).FirstOrDefault();
                                        var KumpulanKod = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjdata.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).FirstOrDefault();

                                        tbl_Kerjahdr.fld_Nopkj = NoPkj;
                                        tbl_Kerjahdr.fld_Tarikh = SelectedDate;
                                        tbl_Kerjahdr.fld_DataSource = "B";
                                        tbl_Kerjahdr.fld_Hujan = 0;
                                        tbl_Kerjahdr.fld_Kum = KumpulanKod;
                                        tbl_Kerjahdr.fld_Kdhdct = KodHadirCuti;
                                        tbl_Kerjahdr.fld_NegaraID = NegaraID;
                                        tbl_Kerjahdr.fld_SyarikatID = SyarikatID;
                                        tbl_Kerjahdr.fld_WilayahID = WilayahID;
                                        tbl_Kerjahdr.fld_LadangID = LadangID;
                                        tbl_Kerjahdr.fld_CreatedBy = getuserid;
                                        tbl_Kerjahdr.fld_CreatedDT = DateTimeNow;

                                        dbr.tbl_Kerjahdr.Add(tbl_Kerjahdr);
                                        dbr.SaveChanges();
                                        EstateFunction.LeaveDeduct(dbr, SelectedDate.Year, NoPkj, KodHadirCuti, NegaraID, SyarikatID, WilayahID, LadangID);

                                        Msg = "Save Successfully";
                                        StatusMsg = "success";
                                        GnrlStatus = true;
                                    }
                                    else
                                    {
                                        Msg = "Already Saved";
                                        StatusMsg = "warning";
                                        GnrlStatus = false;
                                    }

                                }
                                else
                                {
                                    Msg = "Leave balance not enough";
                                    StatusMsg = "warning";
                                    GnrlStatus = true;
                                }
                            }
                            else
                            {
                                var DataHdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Tarikh == SelectedDate && x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                                if (DataHdr == 0)
                                {
                                    var pkjdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Select(s => new { s.fld_Nopkj, s.fld_KumpulanID }).FirstOrDefault();
                                    var KumpulanKod = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjdata.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).FirstOrDefault();

                                    tbl_Kerjahdr.fld_Nopkj = NoPkj;
                                    tbl_Kerjahdr.fld_Tarikh = SelectedDate;
                                    tbl_Kerjahdr.fld_DataSource = "B";
                                    tbl_Kerjahdr.fld_Hujan = 0;
                                    tbl_Kerjahdr.fld_Kum = KumpulanKod;
                                    tbl_Kerjahdr.fld_Kdhdct = KodHadirCuti;
                                    tbl_Kerjahdr.fld_NegaraID = NegaraID;
                                    tbl_Kerjahdr.fld_SyarikatID = SyarikatID;
                                    tbl_Kerjahdr.fld_WilayahID = WilayahID;
                                    tbl_Kerjahdr.fld_LadangID = LadangID;
                                    tbl_Kerjahdr.fld_CreatedBy = getuserid;
                                    tbl_Kerjahdr.fld_CreatedDT = DateTimeNow;

                                    dbr.tbl_Kerjahdr.Add(tbl_Kerjahdr);
                                    dbr.SaveChanges();

                                    Msg = "Save Successfully";
                                    StatusMsg = "success";
                                    GnrlStatus = true;
                                }
                                else
                                {
                                    Msg = "Already Saved";
                                    StatusMsg = "warning";
                                    GnrlStatus = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        Msg = GlobalResEstate.msgError;
                        StatusMsg = "warning";
                        GnrlStatus = false;
                    }
                }
                else
                {
                    Msg = Msg2;
                    StatusMsg = "warning";
                    GnrlStatus = false;
                }
            }
            catch (Exception ex)
            {
                Msg = "Unsuccessfully to save";
                StatusMsg = "danger";
                GnrlStatus = false;
            }
            string DescKodHadirCuti = EstateFunction.GetHadirCutiDesc(KodHadirCuti, "cuti", NegaraID, SyarikatID);
            string UrlLoad = Url.Action("_AttendanceForm", "CheckRollFunc", new { GroupSelection, SelectedDate, DescKodHadirCuti }, "http");
            return Json(new { UrlLoad, Msg, StatusMsg, GnrlStatus });
        }
    }
}