using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using MVC_SYSTEM.ConfigModels;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ViewingModels;
using tblSystemConfig = MVC_SYSTEM.MasterModels.tblSystemConfig;

namespace MVC_SYSTEM.Class
{
    public class GetConfig
    {
        MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        MVC_SYSTEM_Models dbr = new MVC_SYSTEM_Models();

        ChangeTimeZone changeTimeZone = new ChangeTimeZone();

        Connection Connection = new Connection();

        public string GetData(string data)
        {
           
            tblSystemConfig Config;
            string value = "";
            Config = db.tblSystemConfigs.Where(u => u.fldFlag1.Equals(data)).FirstOrDefault();
            if (Config != null)
            {
                value = Config.fldConfigValue.ToString();
            }

            return value;
        }

        public string GetWebConfigDesc(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfValue == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfDesc)
                .FirstOrDefault();

            return getvalue;
        }
        
        public string GetWebConfigDescFromFlag2(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag2 == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfDesc)
                .FirstOrDefault();

            return getvalue;
        }

        public string GetWebConfigDescFromFlag3(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag3 == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfDesc)
                .FirstOrDefault();

            return getvalue;
        }

        public string GetWebConfigValue(string flag1, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == flag1 && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
                && x.fldDeleted == false).Select(s => s.fldOptConfValue)
                .FirstOrDefault();
            return getvalue;
        }

        public string GetWebConfigFlag2FromValue(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfValue == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfFlag2)
                .FirstOrDefault();

            return getvalue;
        }

        public string GetBank(string kod, int negara, int syrkt)
        {
            string bankname = db.tbl_Bank.Where(x => x.fld_KodBank == kod && x.fld_NegaraID == negara && x.fld_SyarikatID == syrkt && x.fld_Deleted == false).Select(s => s.fld_NamaBank).FirstOrDefault();
            return bankname;
        }

        public string GetKwspSocso(string kod, int syrkt, int negara)
        {
            string name = db.tbl_JenisCaruman.Where(x => x.fld_KodCaruman == kod && x.fldNegaraID == negara && x.fldSyarikatID == syrkt && x.fld_Deleted == false).Select(s => s.fld_Keterangan).FirstOrDefault();
            return name;
        }

        public string GetAktvt(string code, int negara, int syarikat)
        {
            var aktvt = db.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_Desc).FirstOrDefault();
            return aktvt;
        }

        public List<tbl_UpahAktiviti> GetAktvt2(int negara, int syarikat)
        {
            var aktvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).ToList();
            return aktvt;
        }

        //atun tmbah 19/4
        public string GetAktvtCode(string code, int negara, int syarikat)
        {
            var aktvtCode = db.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_KodAktvt).FirstOrDefault();
            return aktvtCode;
        }

        //atun tmbah 25/4
        public string GetKodAktvt(string code, int negara, int syarikat)
        {
            var GLCode = db.tbl_MapGL.Where(x => x.fld_KodAktvt == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_KodAktvt).FirstOrDefault();
            return GLCode;
        }

        public string GetKodGL(string code, int negara, int syarikat)
        {
            var GLCode = db.tbl_MapGL.Where(x => x.fld_KodGL == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_KodGL).FirstOrDefault();
            return GLCode;
        }

        //atun tmbah 15/5
        public string GetjnsAktvtCode(string code, int negara, int syarikat)
        {
            var aktvtCode = db.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_KodAktvt).FirstOrDefault();
            return aktvtCode;
        }

        public string GetjnsAktvt (string code, int negara, int syarikat)
        {
            var aktvt = db.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == code && x.fld_NegaraID == negara && x.fld_SyarikatID == syarikat && x.fld_Deleted == false).Select(s => s.fld_Desc).FirstOrDefault();
            return aktvt;
        }

        


        public void GetCutiDesc(string data, string flag1, out string keterangan, out string status, out short KadarByrn, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == data && x.fldOptConfFlag1 == flag1 && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).FirstOrDefault();
            keterangan = getvalue.fldOptConfDesc;
            status = getvalue.fldOptConfFlag2;
            KadarByrn = short.Parse(getvalue.fldOptConfFlag3);
        }

        public string GetPembekal(string kodbkl, int ngra, int syrkt)
        {
            string pembekal = db.tbl_Pembekal.Where(x => x.fld_KodPbkl == kodbkl && x.fld_NegaraID == ngra && x.fld_SyarikatID == syrkt && x.fld_Deleted == false).Select(s => s.fld_NamaPbkl).FirstOrDefault();
            return pembekal;
        }

        public decimal? UpahManual(decimal Hasil, int? NegaraID, int? SyarikatID)
        {
            decimal? upah = db.tbl_UpahMenuai.Where(x => x.fld_Jadual == "A" && x.fld_HasilLower <= Hasil && x.fld_HasilUpper >= Hasil && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Kadar).FirstOrDefault();
            return upah;
        }

        public decimal? UpahMesin(decimal Hasil, int? NegaraID, int? SyarikatID)
        {
            decimal? upah = db.tbl_UpahMenuai.Where(x => x.fld_Jadual == "B" && x.fld_HasilLower <= Hasil && x.fld_HasilUpper >= Hasil && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Kadar).FirstOrDefault();
            return upah;
        }
        
        public string Insentif(int kod, int? NegaraID, int? SyarikatID)
        {
            string insentif = "";
            string code = kod.ToString();
            insentif = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "insentif" && x.fldOptConfValue == code && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfDesc).FirstOrDefault();
            return insentif;
        }
        
        public string GetStatusAktif(string kod, int? NegaraID, int? SyarikatID)
        {
            var status = "";
            if (kod != null)
            {
                status = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldOptConfValue == kod && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfDesc).FirstOrDefault();
            }
            return status;
        }

        public string GetAdditionalContributionDesc(string kod, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tbl_SubCarumanTambahan
                .Where(x => x.fld_KodSubCaruman == kod && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
                            && x.fld_Deleted == false).Select(s => s.fld_KeteranganSubCaruman)
                .FirstOrDefault();
            return getvalue;
        }

        public string GetCompanyGroupDesc(int id)
        {
            var getvalue = db.tbl_KumpulanSyarikat
                .Where(x => x.fld_KmplnSyrktID == id)
                .Select(s => s.fld_NamaKmplnSyrkt)
                .FirstOrDefault();
            return getvalue;
        }

        public string GetGLDesc(string kodLejar, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tbl_Lejar
                .Where(x => x.fld_KodLejar == kodLejar
                            && x.fld_Deleted == false)
                .Select(s =>  s.fld_Desc )
                .FirstOrDefault();

            var GLCodeDesc = kodLejar + " - " + getvalue;

            return GLCodeDesc;
        }

        public string GetCompanyCountryDesc(int id)
        {

            var getData = from tbl_Negara in db.tbl_Negara
                join tbl_KumpulanSyarikat in db.tbl_KumpulanSyarikat on tbl_Negara.fld_KmplnSyrktID equals
                tbl_KumpulanSyarikat.fld_KmplnSyrktID
                where tbl_Negara.fld_NegaraID == id
                select new { Negara = tbl_Negara.fld_NamaNegara, KumpulanSyarikat = tbl_KumpulanSyarikat.fld_NamaKmplnSyrkt };

            var getvalue = getData.Select(x => x.KumpulanSyarikat).FirstOrDefault() + " (" + getData.Select(x => x.Negara).FirstOrDefault() + ")";

            return getvalue;
        }

        public string GetIncentiveCodeFromID(int id)
        {
            var getvalue = db.tbl_JenisInsentif
                .Where(x => x.fld_JenisInsentifID == id)
                .Select(s => s.fld_KodInsentif).SingleOrDefault();

            return getvalue;
        }

        public string GetIncentiveDescFromCode(string incentiveCode, int? NegaraID, int? SyarikatID)
        {

            var getvalue = db.tbl_JenisInsentif
                .Where(x => x.fld_KodInsentif == incentiveCode && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fld_Keterangan).SingleOrDefault();

            return getvalue;
        }

        public bool GetIncentiveIsValidRange(string incentiveCode, decimal incentiveValue, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tbl_JenisInsentif
                .SingleOrDefault(x => x.fld_KodInsentif == incentiveCode && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID);

            var result = getvalue.fld_MinValue <= incentiveValue && getvalue.fld_MaxValue >= incentiveValue;

            return result;
        }

        public string GetPaidLeaveDescFromCode(string paidLeaveCode, int? NegaraID, int? SyarikatID)
        {

            var getvalue = db.tbl_CutiKategori
                .Where(x => x.fld_KodCuti == paidLeaveCode && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fld_KeteranganCuti).SingleOrDefault();

            return getvalue;
        }

        public string GetLadangNegeriFromID(int? id)
        {
            var getvalue = db.tbl_Ladang
                .SingleOrDefault(x => x.fld_ID == id).fld_KodNegeri.ToString();

            return getvalue;
        }

        public string getActiveYieldType(string flag1, string selected, int? NegaraID, int? SyarikatID)
        {
            var getvalue = db.tblOptionConfigsWebs.SingleOrDefault(x =>
                x.fldOptConfFlag1 == flag1 && x.fldOptConfFlag2 == selected &&
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).fldOptConfValue;

            return getvalue;
        }

        public void GetYieldStatus(int MonthList, int YearList, out bool status, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, bool deleted, string host, string user, string catalog, string pass, string yieldType)
        {
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var yieldBracketMonth = Convert.ToInt32(db.tblOptionConfigsWebs.SingleOrDefault(x =>
                x.fldOptConfFlag1 == "yieldBracketMonth" && x.fld_NegaraID == NegaraID &&
                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).fldOptConfValue);

            var getAllPktCount = dbr.tbl_PktUtama
                .Count(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            var getAllSubPktCount = dbr.tbl_SubPkt
                .Count(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            var getAllBlokCount = dbr.tbl_Blok
                .Count(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            var totalForecastRecord = (getAllPktCount * yieldBracketMonth) + (getAllSubPktCount * yieldBracketMonth) +
                                       (getAllBlokCount * yieldBracketMonth);

            var totalActualRecord = 0;

            var last12Month = Enumerable
                .Range(0, yieldBracketMonth)
                .Select(i => DateTime.Now.AddMonths(i - yieldBracketMonth))
                .Select(monthYear => new { monthYear.Month, monthYear.Year });

            foreach (var monthYear in last12Month)
            {
                var getHasilSawitPkt = dbr.tbl_HasilSawitPkt
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Bulan == monthYear.Month &&
                                x.fld_Tahun == monthYear.Year && x.fld_YieldType == yieldType);

                totalActualRecord += getHasilSawitPkt.Count();

                var getHasilSawitSubPkt = dbr.tbl_HasilSawitSubPkt
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Bulan == monthYear.Month &&
                                x.fld_Tahun == monthYear.Year && x.fld_YieldType == yieldType);

                totalActualRecord += getHasilSawitSubPkt.Count();

                var getHasilSawitBlok = dbr.tbl_HasilSawitBlok
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Bulan == monthYear.Month &&
                                x.fld_Tahun == monthYear.Year && x.fld_YieldType == yieldType);

                totalActualRecord += getHasilSawitBlok.Count();
            }

            if (totalForecastRecord != totalActualRecord)
            {
                status = false;
            }

            else
            {
                status = true;
            }

            dbr.Dispose();
        }

        public string UppercaseFirst(string s)
        {
            string toLower = s.ToLower();

            char[] array = toLower.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }

        public void AddUserAuditTrail(int? userID, string userAction)
        {
            tblUserAuditTrail userAuditTrail = new tblUserAuditTrail();

            userAuditTrail.fld_UserActivity = userAction;
            userAuditTrail.fld_CreatedBy = userID;
            userAuditTrail.fld_CreatedDT = changeTimeZone.gettimezone();

            db.tblUserAuditTrails.Add(userAuditTrail);
            db.SaveChanges();
        }

        public string getPkjNameFromPkjNo(string nopkj, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, string host, string user, string catalog, string pass)
        {
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var getvalue = dbr.tbl_Pkjmast.SingleOrDefault(x =>
                x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).fld_Nama;

            return getvalue;
        }

        public string GetDescData(string value, string flag1, int? NegaraID, int? SyarikatID)
        {
            string getdesc = db.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == value && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfDesc).FirstOrDefault();

            return getdesc;
        }

        public int GetConfigValueParseIntData(string flag1, int? NegaraID, int? SyarikatID)
        {
            string getvalue = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == flag1 && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfValue).FirstOrDefault();

            return int.Parse(getvalue);
        }

        public string GetJnsPkj(string kod, int? NegaraID, int? SyarikatID)
        {

            string JnsPkjList = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldOptConfValue == kod && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfDesc).FirstOrDefault();
            return JnsPkjList;
        }

        public int GetKerjaHadirCount(string KodKumpulan, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var currentMonth = changeTimeZone.gettimezone().Month;

            var kerjaHadirCount = dbr.tbl_Kerjahdr
                .Count(x => x.fld_Kum == KodKumpulan && x.fld_Tarikh.Value.Month == currentMonth && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

            return kerjaHadirCount;
        }
        public string GetHutang(string kod, int? NegaraID, int? SyarikatID)
        {

            string HutangList = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "hutangPekerja" && x.fldOptConfValue == kod && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfDesc).FirstOrDefault();
            return HutangList;
        }

        public string GetDivisionName(int? DivisionID, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            var divisionName = "";

            var divisionData = db.tbl_Division.SingleOrDefault(x =>
                x.fld_ID == DivisionID && x.fld_NegaraID == NegaraID &&
                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

            if (divisionData != null)
            {
                divisionName = divisionData.fld_DivisionName;
            }

            return divisionName;
        }
    }
}