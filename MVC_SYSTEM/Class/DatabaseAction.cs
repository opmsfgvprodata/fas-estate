using MVC_SYSTEM.Models;
//using MVC_SYSTEM.AuthModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MVC_SYSTEM.MasterModels;

namespace MVC_SYSTEM.Class
{
    public class DatabaseAction
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        //private MVC_SYSTEM_ModelsCorporate db3 = new MVC_SYSTEM_ModelsCorporate();
        private ChangeTimeZone timezone = new ChangeTimeZone();

        //new Class
        //private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        //private MVC_SYSTEM_Auth dbA = new MVC_SYSTEM_Auth();

        public void InsertDataTotbltblTaskRemainder(string filename, string kdldg, int NegaraID, int? SyarikatID, int? WilayahID, int LadangID, string kdpurpose)
        {
            var getTaskRemainder = db.tblTaskRemainders.Where(x => x.fldFileName.Trim() == filename.Trim() && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID && x.fldPurpose == kdpurpose).FirstOrDefault();

            if (getTaskRemainder == null)
            {
                MasterModels.tblTaskRemainder tblTaskRemainder = new MasterModels.tblTaskRemainder();
                tblTaskRemainder.fldFileName = filename;
                tblTaskRemainder.fldCodeLadang = kdldg;
                tblTaskRemainder.fldNegaraID = NegaraID;
                tblTaskRemainder.fldSyarikatID = SyarikatID;
                tblTaskRemainder.fldWilayahID = WilayahID;
                tblTaskRemainder.fldLadangID = LadangID;
                tblTaskRemainder.fldPurpose = kdpurpose;
                tblTaskRemainder.fldStatus = 0;
                tblTaskRemainder.fldDateTimeStamp = timezone.gettimezone();

                db.tblTaskRemainders.Add(tblTaskRemainder);
                db.SaveChanges();
            }
        }

        public void UpdateDataTotbltblTaskRemainder(string filename, int NegaraID, int? SyarikatID, int? WilayahID, int LadangID, string kdpurpose)
        {
            var getTaskRemainder = db.tblTaskRemainders.Where(x => x.fldFileName.Trim() == filename.Trim() && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID && x.fldPurpose == kdpurpose).FirstOrDefault();
            if (getTaskRemainder != null)
            {
                getTaskRemainder.fldStatus = 1;

                db.Entry(getTaskRemainder).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void UpdateDataTotblASCApprovalFileDetail(long fileid)
        {
            var getASCFileDetail = db.tblASCApprovalFileDetails.Where(x => x.fldID == fileid).FirstOrDefault();

            getASCFileDetail.fldGenStatus = 1;

            db.Entry(getASCFileDetail).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void UpdateDataTotblSokPermhnWang(long ID, int semakwil, int tolakwil, int sokongwilgm, int tolakwilgm, int terimahq, int tolakhq, string flag, int userid, DateTime getdatetime, decimal PDP, decimal CIT, string NoAcc, string NoGL, string NoCIT, decimal Manual)
        {
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_ID == ID).FirstOrDefault();

            switch (flag)
            {
                case "SemakWil":
                    getdata.fld_SemakWil_Status = semakwil;
                    getdata.fld_SemakWil_By = userid;
                    getdata.fld_SemakWil_DT = getdatetime;
                    getdata.fld_TolakWil_Status = tolakwil;
                    getdata.fld_TolakWil_By = 0;
                    getdata.fld_TolakWil_DT = null;
                    getdata.fld_JumlahPDP = PDP;
                    //getdata.fld_JumlahTT = TT;
                    getdata.fld_JumlahCIT = CIT;
                    getdata.fld_NoAcc = NoAcc;
                    getdata.fld_NoGL = NoGL;
                    getdata.fld_NoCIT = NoCIT;
                    getdata.fld_JumlahManual = Manual;
                    break;

                case "TolakWil":
                    getdata.fld_SemakWil_Status = semakwil;
                    getdata.fld_SemakWil_By = 0;
                    getdata.fld_SemakWil_DT = null;
                    getdata.fld_TolakWil_Status = tolakwil;
                    getdata.fld_TolakWil_By = userid;
                    getdata.fld_TolakWil_DT = getdatetime;
                    break;
            }

            db.Entry(getdata).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void UpdateDataTotblSokPermhnWangGM(int ID, string flag, int Month, int Year, int userid, DateTime getdatetime)
        {
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == ID && x.fld_Month == Month && x.fld_Year == Year).ToList();

            switch (flag)
            {
                case "SokongGMWil":
                    getdata.ForEach(x => x.fld_SokongWilGM_Status = 1);
                    getdata.ForEach(x => x.fld_SokongWilGM_By = userid);
                    getdata.ForEach(x => x.fld_SokongWilGM_DT = getdatetime);
                    getdata.ForEach(x => x.fld_TolakWilGM_Status = 0);
                    getdata.ForEach(x => x.fld_TolakWilGM_By = 0);
                    getdata.ForEach(x => x.fld_TolakWilGM_DT = null);
                    break;

                case "TolakGMWil":
                    getdata.ForEach(x => x.fld_TolakWilGM_Status = 1);
                    getdata.ForEach(x => x.fld_TolakWilGM_By = userid);
                    getdata.ForEach(x => x.fld_TolakWilGM_DT = getdatetime);
                    getdata.ForEach(x => x.fld_SokongWilGM_Status = 0);
                    getdata.ForEach(x => x.fld_SokongWilGM_By = userid);
                    getdata.ForEach(x => x.fld_SokongWilGM_DT = null);
                    break;
            }
            db.SaveChanges();
        }

        public void UpdateDataTotblSokPermhnWangHQ(int ID, string flag, int Month, int Year, int userid, DateTime getdatetime)
        {
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == ID && x.fld_Month == Month && x.fld_Year == Year).ToList();

            switch (flag)
            {
                case "TerimaHQ":
                    getdata.ForEach(x => x.fld_TerimaHQ_Status = 1);
                    getdata.ForEach(x => x.fld_TerimaHQ_By = userid);
                    getdata.ForEach(x => x.fld_TerimaHQ_DT = getdatetime);
                    getdata.ForEach(x => x.fld_TolakHQ_Status = 0);
                    getdata.ForEach(x => x.fld_TolakHQ_By = 0);
                    getdata.ForEach(x => x.fld_TolakHQ_DT = null);
                    break;

                case "TolakHQ":
                    getdata.ForEach(x => x.fld_TolakHQ_Status = 1);
                    getdata.ForEach(x => x.fld_TolakHQ_By = userid);
                    getdata.ForEach(x => x.fld_TolakHQ_DT = getdatetime);
                    getdata.ForEach(x => x.fld_TerimaHQ_Status = 0);
                    getdata.ForEach(x => x.fld_TerimaHQ_By = userid);
                    getdata.ForEach(x => x.fld_TerimaHQ_DT = null);
                    break;
            }
            db.SaveChanges();
        }
        public void InsertDataTotblSokPermhnWangHisAction(string hisdesc, int hisuserid, DateTime hisDT, long SPWID, int HisAppLevel)
        {
            MasterModels.tblSokPermhnWangHisAction tblSokPermhnWangHisAction = new MasterModels.tblSokPermhnWangHisAction();

            tblSokPermhnWangHisAction.fldHisDesc = hisdesc;
            tblSokPermhnWangHisAction.fldHisUserID = hisuserid;
            tblSokPermhnWangHisAction.fldHisDT = hisDT;
            tblSokPermhnWangHisAction.fldHisSPWID = SPWID;
            tblSokPermhnWangHisAction.fldHisAppLevel = HisAppLevel;

            db.tblSokPermhnWangHisActions.Add(tblSokPermhnWangHisAction);
            db.SaveChanges();
        }

        public int InsertDataTotblASCApprovalFileDetail(string fldFileName, string fldCodeLadang, int fldNegaraID,
            int? fldSyarikatID, int? fldWilayahID, int fldLadangID, int fldGenStatus, int fldASCFileStatus,
            int fldPurpose, DateTime fldDateTimeCreated)
        {
            MasterModels.tblASCApprovalFileDetail tblASCApprovalFileDetail = new MasterModels.tblASCApprovalFileDetail();

            tblASCApprovalFileDetail.fldFileName = fldFileName;
            tblASCApprovalFileDetail.fldCodeLadang = fldCodeLadang;
            tblASCApprovalFileDetail.fldNegaraID = fldNegaraID;
            tblASCApprovalFileDetail.fldSyarikatID = fldSyarikatID;
            tblASCApprovalFileDetail.fldWilayahID = fldWilayahID;
            tblASCApprovalFileDetail.fldLadangID = fldLadangID;
            tblASCApprovalFileDetail.fldGenStatus = fldGenStatus;
            tblASCApprovalFileDetail.fldASCFileStatus = fldASCFileStatus;
            tblASCApprovalFileDetail.fldPurpose = fldPurpose;
            tblASCApprovalFileDetail.fldDateApplied = fldDateTimeCreated;

            db.tblASCApprovalFileDetails.Add(tblASCApprovalFileDetail);
            db.SaveChanges();

            return tblASCApprovalFileDetail.fldID;
        }

        public void InsertDataTotblUserIDApp(string fldUserid, string fldNama, string fldNoKP, string fldKdLdg, string fldNamaLdg,
            DateTime fldTarikh, string fldJawatan, string fldPassword, string fldStatus, DateTime fldTrkdload, long fldFileID,
            int fldNegaraID, int? fldSyarikatID, int? fldWilayahID, int fldLadangID, int? fldActionBy, DateTime? fldDateTimeApprove)
        {
            MasterModels.tblUserIDApp tblUserIDApp = new MasterModels.tblUserIDApp();

            tblUserIDApp.fldUserid = fldUserid;
            tblUserIDApp.fldNama = fldNama;
            tblUserIDApp.fldNoKP = fldNoKP;
            tblUserIDApp.fldKdLdg = fldKdLdg;
            tblUserIDApp.fldNamaLdg = fldNamaLdg;
            tblUserIDApp.fldTarikh = fldTarikh;
            tblUserIDApp.fldJawatan = fldJawatan;
            tblUserIDApp.fldPassword = fldPassword;
            tblUserIDApp.fldStatus = fldStatus;
            tblUserIDApp.fldTrkdload = fldTrkdload;
            tblUserIDApp.fldFileID = fldFileID;
            tblUserIDApp.fldNegaraID = fldNegaraID;
            tblUserIDApp.fldSyarikatID = fldSyarikatID;
            tblUserIDApp.fldWilayahID = fldWilayahID;
            tblUserIDApp.fldLadangID = fldLadangID;
            tblUserIDApp.fldActionBy = fldActionBy;
            tblUserIDApp.fldDateTimeApprove = fldDateTimeApprove;

            db.tblUserIDApps.Add(tblUserIDApp);
            db.SaveChanges();
        }

        public void InsertDataTotblUser(string fldUserName, string fldUserFullName, string fldUserShortName,
            string fldUserEmail, string fldUserPassword, int? fldRoleID, int? fld_KmplnSyrktID, int? fldNegaraID,
            int? fldSyarikatID, int? fldWilayahID, int? fldLadangID, int? fldFirstTimeLogin, int? fldClientID,
            bool? fldDeleted, int? fld_CreatedBy, DateTime? fld_CreatedDT, string fldUserCategory)
        {
            MasterModels.tblUser tblUser = new MasterModels.tblUser();

            tblUser.fldUserName = fldUserName;
            tblUser.fldUserFullName = fldUserFullName;
            tblUser.fldUserShortName = fldUserShortName;
            tblUser.fldUserEmail = fldUserEmail;
            tblUser.fldUserPassword = fldUserPassword;
            tblUser.fldRoleID = fldRoleID;
            tblUser.fld_KmplnSyrktID = fld_KmplnSyrktID;
            tblUser.fldNegaraID = fldNegaraID;
            tblUser.fldSyarikatID = fldSyarikatID;
            tblUser.fldWilayahID = fldWilayahID;
            tblUser.fldLadangID = fldLadangID;
            tblUser.fldFirstTimeLogin = fldFirstTimeLogin;
            tblUser.fldClientID = fldClientID;
            tblUser.fldDeleted = fldDeleted;
            tblUser.fld_CreatedBy = fld_CreatedBy;
            tblUser.fld_CreatedDT = fld_CreatedDT;
            tblUser.fldUserCategory = fldUserCategory;

            db.tblUsers.Add(tblUser);
            db.SaveChanges();
        }

        public void UpdateDataTotblUserIDApp(string fldUserid, string fldNama, string fldNoKP, string fldKdLdg, string fldNamaLdg,
            DateTime fldTarikh, string fldJawatan, string fldPassword, string fldStatus, DateTime fldTrkdload, int fldFileID,
            int fldNegaraID, int? fldSyarikatID, int? fldWilayahID, int fldLadangID, int? fldActionBy, DateTime? fldDateTimeApprove)
        {
            var gettblUserIDApp = db.tblUserIDApps.Where(x => x.fldUserid == fldUserid).FirstOrDefault();

            gettblUserIDApp.fldNama = fldNama;
            gettblUserIDApp.fldNoKP = fldNoKP;
            gettblUserIDApp.fldJawatan = fldJawatan;

            db.Entry(gettblUserIDApp).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void UpdateDataTotblUser(string fldUserName, string fldUserFullName, string fldUserShortName,
            string fldUserEmail, string fldUserPassword, int? fldRoleID, int? fld_KmplnSyrktID, int? fldNegaraID,
            int? fldSyarikatID, int? fldWilayahID, int? fldLadangID, int? fldFirstTimeLogin, int? fldClientID,
            bool? fldDeleted, int? fld_CreatedBy, DateTime? fld_CreatedDT, string fldUserCategory)
        {
            var gettblUsersdata = db.tblUsers.Where(x => x.fldUserName == fldUserName).FirstOrDefault();

            gettblUsersdata.fldUserFullName = fldUserFullName;
            gettblUsersdata.fldUserShortName = fldUserShortName;
            gettblUsersdata.fldUserEmail = fldUserEmail;
            gettblUsersdata.fldRoleID = fldRoleID;

            db.Entry(gettblUsersdata).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void InsertDataTotbl_PkjIncrmntApp(List<tbl_PkjIncrmntSalary> tbl_PkjIncrmntSalarys, long? FileID)
        {
            List<tbl_PkjIncrmntApp> tbl_PkjIncrmntApp = new List<tbl_PkjIncrmntApp>();

            foreach (var tbl_PkjIncrmntSalary in tbl_PkjIncrmntSalarys)
            {
                tbl_PkjIncrmntApp.Add(new tbl_PkjIncrmntApp() { fld_AppBy = tbl_PkjIncrmntSalary.fld_AppBy, fld_AppDT = tbl_PkjIncrmntSalary.fld_AppDT, fld_AppStatus = tbl_PkjIncrmntSalary.fld_AppStatus, fld_Deleted = tbl_PkjIncrmntSalary.fld_Deleted, fld_FileID = FileID, fld_IncrmntSalary = tbl_PkjIncrmntSalary.fld_IncrmntSalary, fld_LadangID = tbl_PkjIncrmntSalary.fld_LadangID, fld_NegaraID = tbl_PkjIncrmntSalary.fld_NegaraID, fld_Nopkj = tbl_PkjIncrmntSalary.fld_Nopkj, fld_ProcessStage = tbl_PkjIncrmntSalary.fld_ProcessStage, fld_ReqBy = tbl_PkjIncrmntSalary.fld_ReqBy, fld_ReqDT = tbl_PkjIncrmntSalary.fld_ReqDT, fld_SyarikatID = tbl_PkjIncrmntSalary.fld_SyarikatID, fld_WilayahID = tbl_PkjIncrmntSalary.fld_WilayahID, fld_Year = tbl_PkjIncrmntSalary.fld_Year, fld_DailyInsentif = tbl_PkjIncrmntSalary.fld_DailyInsentif });
            }

            db.tbl_PkjIncrmntApp.AddRange(tbl_PkjIncrmntApp);
            db.SaveChanges();
        }
    }
}