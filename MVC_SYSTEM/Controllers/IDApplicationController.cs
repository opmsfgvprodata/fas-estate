using MVC_SYSTEM.Class;
using MVC_SYSTEM.CustomModels;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    public class IDApplicationController : Controller
    {
        private DatabaseAction DatabaseAction = new DatabaseAction();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private errorlog geterror = new errorlog();
        private GetConfig GetConfig = new GetConfig();

        //new Class
        private MVC_SYSTEM_MasterModels dbC = new MVC_SYSTEM_MasterModels();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetIdentity getidentity = new GetIdentity();

        // GET: IDApplication
        public ActionResult Index()
        {
            List<SelectListItem> PositionList = new List<SelectListItem>();
            PositionList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "position" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
            ViewBag.PositionList = PositionList;
            return View();
        }

        public JsonResult GetDetail(string kdprmhnan, string kdldg)
        {
            List<SelectListItem> batchlist = new List<SelectListItem>();
            List<SelectListItem> PositionList = new List<SelectListItem>();

            int detailexisting = 0;
            int batchdetail = 0;
            var GetNSWLDetail = GetNSWL.GetLadangDetail(kdprmhnan, kdldg);
            if (GetNSWLDetail != null)
            {
                PositionList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "position" && x.fldDeleted == false && x.fld_NegaraID == GetNSWLDetail.fld_NegaraID && x.fld_SyarikatID == GetNSWLDetail.fld_SyarikatID).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text").ToList();
                detailexisting = 1;
                batchlist = new SelectList(dbC.tblASCApprovalFileDetails.Where(x => x.fldNegaraID == GetNSWLDetail.fld_NegaraID && x.fldSyarikatID == GetNSWLDetail.fld_SyarikatID && x.fldWilayahID == GetNSWLDetail.fld_WilayahID && x.fldLadangID == GetNSWLDetail.fld_LadangID && x.fldASCFileStatus == 1 && x.fldPurpose == 1).Select(s => new SelectListItem { Value = s.fldID.ToString(), Text = s.fldFileName.ToString() }), "Value", "Text").ToList();
                if (batchlist.Count() >= 1)
                {
                    batchdetail = 1;
                }
                else
                {
                    batchdetail = 0;
                }
            }
            return Json(new { getdetail = GetNSWLDetail, batchlist, detailexisting, batchdetail, PositionList });
        }
        public JsonResult CheckUserID(string Username)
        {
            List<SelectListItem> batchlist = new List<SelectListItem>();
            int detailexisting = 0;
            var getcountfromtblUserIDApps = dbC.tblUserIDApps.Where(x => x.fldUserid.ToUpper() == Username.ToUpper()).Count();
            var getcountfromtblUsers = dbC.tblUsers.Where(x => x.fldUserName.ToUpper() == Username.ToUpper()).Count();
            if (getcountfromtblUserIDApps >= 1 || getcountfromtblUsers >= 1)
            {
                detailexisting = 1;
            }
            else
            {
                detailexisting = 0;
            }

            return Json(detailexisting);
        }

        public JsonResult GetBatchNo(string kodladang, string kdprmhnan)
        {
            int? convertint = 0;
            string genbatchno = "";

            var GetNSWLDetail = GetNSWL.GetLadangDetail(kdprmhnan, kodladang);
            var getbatchno = dbC.tbl_BatchRunNo.Where(x => x.fld_BatchFlag == "useridbatchno" && x.fld_NegaraID == GetNSWLDetail.fld_NegaraID && x.fld_SyarikatID == GetNSWLDetail.fld_SyarikatID && x.fld_WilayahID == GetNSWLDetail.fld_WilayahID && x.fld_LadangID == GetNSWLDetail.fld_LadangID).FirstOrDefault();

            if (getbatchno == null)
            {
                tbl_BatchRunNo tbl_BatchRunNo = new tbl_BatchRunNo();
                tbl_BatchRunNo.fld_BatchRunNo = 2;
                tbl_BatchRunNo.fld_BatchFlag = "useridbatchno";
                tbl_BatchRunNo.fld_NegaraID = GetNSWLDetail.fld_NegaraID;
                tbl_BatchRunNo.fld_SyarikatID = GetNSWLDetail.fld_SyarikatID;
                tbl_BatchRunNo.fld_WilayahID = GetNSWLDetail.fld_WilayahID;
                tbl_BatchRunNo.fld_LadangID = GetNSWLDetail.fld_LadangID;
                dbC.tbl_BatchRunNo.Add(tbl_BatchRunNo);
                dbC.SaveChanges();
                convertint = 1;
                genbatchno = kdprmhnan.ToUpper() + "_USERID_" + kodladang.ToUpper() + "_" + convertint;
            }
            else
            {
                convertint = getbatchno.fld_BatchRunNo;
                genbatchno = kdprmhnan.ToUpper() + "_USERID_" + kodladang.ToUpper() + "_" + convertint;
                convertint = convertint + 1;
                getbatchno.fld_BatchRunNo = convertint;
                dbC.Entry(getbatchno).State = EntityState.Modified;
                dbC.SaveChanges();
            }
            return Json(genbatchno);
        }

        public JsonResult SearchBatch(int batchno, string kodladang, string kdprmhnan)
        {
            string msg = "";
            string statusmsg = "";
            bool status = false;
            int itemno = 1;
            string tablelisting = "";
            var GetNSWLDetail = GetNSWL.GetLadangDetail(kdprmhnan, kodladang);
            var getUserDetails = dbC.tblUserIDApps.Where(x => x.fldFileID == batchno && x.fldNegaraID == GetNSWLDetail.fld_NegaraID && x.fldSyarikatID == GetNSWLDetail.fld_SyarikatID && x.fldWilayahID == GetNSWLDetail.fld_WilayahID && x.fldLadangID == GetNSWLDetail.fld_LadangID).OrderBy(o => o.fldFileID).ToList();
            var batchno2 = dbC.tblASCApprovalFileDetails.Where(x => x.fldID == batchno && x.fldNegaraID == GetNSWLDetail.fld_NegaraID && x.fldSyarikatID == GetNSWLDetail.fld_SyarikatID && x.fldWilayahID == GetNSWLDetail.fld_WilayahID && x.fldLadangID == GetNSWLDetail.fld_LadangID).Select(s => s.fldFileName).FirstOrDefault();
            var getNotiStatus = dbC.tblEmailNotiStatus.Where(x => x.fldEmailNotiFlag == batchno2 && x.fldEmailNotiSource == "Ladang" && x.fldNegaraID == GetNSWLDetail.fld_NegaraID && x.fldSyarikatID == GetNSWLDetail.fld_SyarikatID && x.fldWilayahID == GetNSWLDetail.fld_WilayahID && x.fldLadangID == GetNSWLDetail.fld_LadangID).FirstOrDefault();

            string statussbmt = "";
            if (getNotiStatus != null)
            {
                statussbmt = "Application Submitted";
            }
            else
            {
                statussbmt = "Application On Going";
            }

            tablelisting += "<thead>";
            tablelisting += "<tr>";
            tablelisting += "<th bgcolor=\"#073e5f\" colspan = \"7\" width=\"100%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Batch No : " + batchno2 + "</th>";
            tablelisting += "</tr>";
            tablelisting += "<tr>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"5%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">No</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"20%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Name</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"15%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">IC No</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"20%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Position</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"15%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">User ID</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"15%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Status</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"10%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Action</th>";
            tablelisting += "</tr>";
            tablelisting += "</thead>";
            tablelisting += "<tbody>";
            foreach (var getUserDetail in getUserDetails)
            {
                tablelisting += "<tr>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + itemno + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getUserDetail.fldNama + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getUserDetail.fldNoKP + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + GetConfig.GetDescData(getUserDetail.fldJawatan, "position", getUserDetail.fldNegaraID, getUserDetail.fldSyarikatID) + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getUserDetail.fldUserid + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + statussbmt + "</td>";
                if (getNotiStatus != null)
                {
                    //tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">Telah dihantar ke HQ</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\"><a data-modal = \"\" href = \"" + Url.Action("Edit", "IDApplication") + "/" + getUserDetail.fldID + "\" title=\"Edit\" class=\"btn btn-success\"> <span class=\"glyphicon glyphicon-edit\"> </span> </a></td>";
                }
                else
                {
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\"><a data-modal = \"\" href = \"" + Url.Action("Edit", "IDApplication") + "/" + getUserDetail.fldID + "\" title=\"Edit\" class=\"btn btn-success\"> <span class=\"glyphicon glyphicon-edit\"> </span> </a></td>";
                }
                tablelisting += "</tr>";
                itemno++;
            }
            tablelisting += "</tbody>";

            if (getNotiStatus != null)
            {
                msg = "Data found and data cannot be changed.";
                statusmsg = "success";
                status = true;
            }
            else
            {
                msg = "Data found.";
                statusmsg = "success";
                status = false;
            }

            return Json(new { tablelisting = tablelisting, msg = msg, statusmsg = statusmsg, status = status, batchno = batchno2 });
        }

        public JsonResult AddData(UserIDApplication UserIDApplication)//string batchno, string kdldng, string kdprmhnan, string Username, string Name, string IC, 
                                                                      //string PositionList)
        {
            EncryptDecrypt Encrypt = new EncryptDecrypt();
            DateTime DT = timezone.gettimezone();
            string password = Encrypt.Encrypt("init123");
            string tablelisting = "";
            string msg = "";
            string statusmsg = "";
            bool status = false;
            int itemno = 1;
            long batchid = 0;
            int roleid = 0;
            try
            {
                var getdetail = GetNSWL.GetLadangDetail(UserIDApplication.kdprmhnan, UserIDApplication.kdldng);
                roleid = GetConfig.GetConfigValueParseIntData(UserIDApplication.PositionList, getdetail.fld_NegaraID, getdetail.fld_SyarikatID);
                var checkbatchexisting = dbC.tblASCApprovalFileDetails.Where(x => x.fldFileName == UserIDApplication.batchno && x.fldCodeLadang == UserIDApplication.kdldng && x.fldNegaraID == getdetail.fld_NegaraID && x.fldSyarikatID == getdetail.fld_SyarikatID && x.fldWilayahID == getdetail.fld_WilayahID && x.fldLadangID == getdetail.fld_LadangID).FirstOrDefault();
                int detailexisting = 0;
                var getcountfromtblUserIDApps = dbC.tblUserIDApps.Where(x => x.fldUserid.ToUpper() == UserIDApplication.Username.ToUpper()).Count();
                var getcountfromtblUsers = dbC.tblUsers.Where(x => x.fldUserName.ToUpper() == UserIDApplication.Username.ToUpper()).Count();
                if (getcountfromtblUserIDApps >= 1 || getcountfromtblUsers >= 1)
                {
                    detailexisting = 1;
                }
                else
                {
                    detailexisting = 0;
                }

                if (detailexisting == 0)
                {

                }

                if (checkbatchexisting == null)
                {
                    batchid = DatabaseAction.InsertDataTotblASCApprovalFileDetail(UserIDApplication.batchno, UserIDApplication.kdldng, getdetail.fld_NegaraID, getdetail.fld_SyarikatID, getdetail.fld_WilayahID, getdetail.fld_LadangID, 0, 1, 1, DT);
                    DatabaseAction.InsertDataTotblUserIDApp(UserIDApplication.Username.ToUpper(), UserIDApplication.Name.ToUpper(), UserIDApplication.IC, UserIDApplication.kdldng, getdetail.fld_NamaLadang, DT, UserIDApplication.PositionList, password, "2", DT, batchid, getdetail.fld_NegaraID, getdetail.fld_SyarikatID, getdetail.fld_WilayahID, getdetail.fld_LadangID, null, null);
                    DatabaseAction.InsertDataTotblUser(UserIDApplication.Username.ToUpper(), UserIDApplication.Name.ToUpper(), UserIDApplication.shortname.ToUpper(), UserIDApplication.email.ToLower(), password, roleid, getdetail.fld_KmplnSyrktID, getdetail.fld_NegaraID, getdetail.fld_SyarikatID, getdetail.fld_WilayahID, getdetail.fld_LadangID, 1, 99, true, 0, DT, "CHECKROLL");
                }
                else
                {
                    batchid = checkbatchexisting.fldID;
                    DatabaseAction.InsertDataTotblUserIDApp(UserIDApplication.Username.ToUpper(), UserIDApplication.Name.ToUpper(), UserIDApplication.IC, UserIDApplication.kdldng, getdetail.fld_NamaLadang, DT, UserIDApplication.PositionList, password, "2", DT, batchid, getdetail.fld_NegaraID, getdetail.fld_SyarikatID, getdetail.fld_WilayahID, getdetail.fld_LadangID, null, null);
                    DatabaseAction.InsertDataTotblUser(UserIDApplication.Username.ToUpper(), UserIDApplication.Name.ToUpper(), UserIDApplication.shortname.ToUpper(), UserIDApplication.email.ToLower(), password, roleid, getdetail.fld_KmplnSyrktID, getdetail.fld_NegaraID, getdetail.fld_SyarikatID, getdetail.fld_WilayahID, getdetail.fld_LadangID, 1, 99, true, 0, DT, "CHECKROLL");
                }

                var getUserDetails = dbC.tblUserIDApps.Where(x => x.fldFileID == batchid).OrderBy(o => o.fldFileID).ToList();
                var batchno2 = dbC.tblASCApprovalFileDetails.Where(x => x.fldID == batchid && x.fldNegaraID == getdetail.fld_NegaraID && x.fldSyarikatID == getdetail.fld_SyarikatID && x.fldWilayahID == getdetail.fld_WilayahID && x.fldLadangID == getdetail.fld_LadangID).Select(s => s.fldFileName).FirstOrDefault();
                var getNotiStatus = dbC.tblEmailNotiStatus.Where(x => x.fldEmailNotiFlag == batchno2 && x.fldEmailNotiSource == "Ladang" && x.fldNegaraID == getdetail.fld_NegaraID && x.fldSyarikatID == getdetail.fld_SyarikatID && x.fldWilayahID == getdetail.fld_WilayahID && x.fldLadangID == getdetail.fld_LadangID).FirstOrDefault();
                string statussbmt = "";
                if (getNotiStatus != null)
                {
                    statussbmt = "Application Submitted";
                }
                else
                {
                    statussbmt = "Application On Going";
                }
                tablelisting += "<thead>";
                tablelisting += "<tr>";
                tablelisting += "<th bgcolor=\"#073e5f\" colspan = \"7\" width=\"100%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Batch No : " + batchno2 + "</th>";
                tablelisting += "</tr>";
                tablelisting += "<tr>";
                tablelisting += "<th bgcolor=\"#073e5f\" width=\"5%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">No</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" width=\"20%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Name</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" width=\"15%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">IC No</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" width=\"20%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Position</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" width=\"15%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">User ID</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" width=\"15%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Status</th>";
                tablelisting += "<th bgcolor=\"#073e5f\" width=\"10%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Action</th>";
                tablelisting += "</tr>";
                tablelisting += "</thead>";
                tablelisting += "<tbody>";
                foreach (var getUserDetail in getUserDetails)
                {
                    tablelisting += "<tr>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + itemno + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getUserDetail.fldNama + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getUserDetail.fldNoKP + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + GetConfig.GetDescData(getUserDetail.fldJawatan, "position", getUserDetail.fldNegaraID, getUserDetail.fldSyarikatID) + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getUserDetail.fldUserid + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + statussbmt + "</td>";
                    tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\"><a data-modal = \"\" href = \"" + Url.Action("Edit", "IDApplication") + "/" + getUserDetail.fldID + "\" title=\"Edit\" class=\"btn btn-success\"> <span class=\"glyphicon glyphicon-edit\"> </span> </a></td>";
                    tablelisting += "</tr>";
                    itemno++;
                }
                tablelisting += "</tbody>";

                msg = "Data already entered.";
                statusmsg = "success";
                status = true;
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = "Masalah sistem.";
                statusmsg = "danger";
                status = false;
            }

            return Json(new { tablelisting = tablelisting, msg = msg, statusmsg = statusmsg, status = status });
        }

        public ActionResult Edit(int id)
        {
            bool submitstatus = false;
            vw_UserIDDetail vw_UserIDDetail = new vw_UserIDDetail();
            vw_UserIDDetail = dbC.vw_UserIDDetail.Where(x => x.fldID == id).FirstOrDefault();
            List<SelectListItem> PositionList = new List<SelectListItem>();
            PositionList = new SelectList(dbC.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "position" && x.fldDeleted == false && x.fld_NegaraID == vw_UserIDDetail.fldNegaraID && x.fld_SyarikatID == vw_UserIDDetail.fldSyarikatID), "fldOptConfValue", "fldOptConfDesc", vw_UserIDDetail.fldJawatan.Trim()).ToList();
            ViewBag.PositionList = PositionList;
            var getNotiStatus = dbC.tblEmailNotiStatus.Where(x => x.fldEmailNotiFlag == vw_UserIDDetail.fldFileName && x.fldEmailNotiSource == "Ladang" && x.fldNegaraID == vw_UserIDDetail.fldNegaraID && x.fldSyarikatID == vw_UserIDDetail.fldSyarikatID && x.fldWilayahID == vw_UserIDDetail.fldWilayahID && x.fldLadangID == vw_UserIDDetail.fldLadangID).FirstOrDefault();

            if (getNotiStatus != null)
            {
                submitstatus = true;
            }
            else
            {
                submitstatus = false;
            }

            ViewBag.submitstatus = submitstatus;

            return PartialView("Edit", vw_UserIDDetail);
        }

        [HttpPost]
        public ActionResult Edit(int fldID, string fldUserFullName, string fldNoKP, string PositionList, string fldUserShortName, string fldUserEmail, int BatchID)
        {
            DateTime DT = timezone.gettimezone();
            int roleid = 0;
            string tablelisting = "";
            int itemno = 1;
            string msg = "";
            string statusmsg = "";
            bool status = false;
            try
            {
                var getuserdetail = dbC.vw_UserIDDetail.Where(x => x.fldID == fldID).FirstOrDefault();
                roleid = GetConfig.GetConfigValueParseIntData(PositionList, getuserdetail.fldNegaraID, getuserdetail.fldSyarikatID);
                DatabaseAction.UpdateDataTotblUserIDApp(getuserdetail.fldUserName.ToUpper(), fldUserFullName.ToUpper(), fldNoKP, "", "", DT, PositionList, "", "0", DT, 0, 0, 0, 0, 0, null, null);
                DatabaseAction.UpdateDataTotblUser(getuserdetail.fldUserName.ToUpper(), fldUserFullName.ToUpper(), fldUserShortName.ToUpper(), fldUserEmail.ToLower(), "", roleid, 0, 0, 0, 0, 0, 1, 99, true, 0, DT, "CHECKROLL");
                msg = "Data updated.";
                statusmsg = "success";
                status = true;
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = "Problem.";
                statusmsg = "danger";
                status = false;
            }

            var getUserDetails = dbC.tblUserIDApps.Where(x => x.fldFileID == BatchID).OrderBy(o => o.fldFileID).ToList();
            var getdetail = getUserDetails.Select(s => new { s.fldNegaraID, s.fldSyarikatID, s.fldWilayahID, s.fldLadangID }).Distinct().FirstOrDefault();
            var batchno2 = dbC.tblASCApprovalFileDetails.Where(x => x.fldID == BatchID && x.fldNegaraID == getdetail.fldNegaraID && x.fldSyarikatID == getdetail.fldSyarikatID && x.fldWilayahID == getdetail.fldWilayahID && x.fldLadangID == getdetail.fldLadangID).Select(s => s.fldFileName).FirstOrDefault();
            var getNotiStatus = dbC.tblEmailNotiStatus.Where(x => x.fldEmailNotiFlag == batchno2 && x.fldEmailNotiSource == "Ladang" && x.fldNegaraID == getdetail.fldNegaraID && x.fldSyarikatID == getdetail.fldSyarikatID && x.fldWilayahID == getdetail.fldWilayahID && x.fldLadangID == getdetail.fldLadangID).FirstOrDefault();
            string statussbmt = "";
            if (getNotiStatus != null)
            {
                statussbmt = "Application Submitted";
            }
            else
            {
                statussbmt = "Application On Going";
            }
            tablelisting += "<thead>";
            tablelisting += "<tr>";
            tablelisting += "<th bgcolor=\"#073e5f\" colspan = \"7\" width=\"100%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Batch No : " + batchno2 + "</th>";
            tablelisting += "</tr>";
            tablelisting += "<tr>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"5%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">No</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"20%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Name</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"15%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">IC No</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"20%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Position</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"15%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">User ID</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"15%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Status</th>";
            tablelisting += "<th bgcolor=\"#073e5f\" width=\"10%\" style=\"color:white; text-align:center; vertical-align:middle;border:1px solid black;\" border=\"1\">Action</th>";
            tablelisting += "</tr>";
            tablelisting += "</thead>";
            tablelisting += "<tbody>";
            foreach (var getUserDetail in getUserDetails)
            {
                tablelisting += "<tr>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + itemno + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getUserDetail.fldNama + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getUserDetail.fldNoKP + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + GetConfig.GetDescData(getUserDetail.fldJawatan, "position", getUserDetail.fldNegaraID, getUserDetail.fldSyarikatID) + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + getUserDetail.fldUserid + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\">" + statussbmt + "</td>";
                tablelisting += "<td align=\"center\" style=\"vertical-align:middle !important; border: 1px solid black;\" border=\"1\"><a data-modal = \"\" href = \"" + Url.Action("Edit", "IDApplication", new { id = "" }) + "/" + getUserDetail.fldID + "\" title=\"Edit\" class=\"btn btn-success\"> <span class=\"glyphicon glyphicon-edit\"> </span> </a></td>";
                tablelisting += "</tr>";
                itemno++;
            }
            tablelisting += "</tbody>";

            return Json(new { tablelisting = tablelisting, success = status, msg = msg, status = statusmsg });
        }
    }
}