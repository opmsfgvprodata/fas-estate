using MVC_SYSTEM.CustomModels;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class CheckrollFunction
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetConfig GetConfig = new GetConfig();
        private Connection Connection = new Connection();

        public bool LeaveCalBal(MVC_SYSTEM_Models dbr, int year, string NoPkj, string KodKatCuti, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            bool result = false;

            var getdata = dbr.tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NoPkj == NoPkj && x.fld_KodCuti == KodKatCuti && x.fld_Tahun == year).Select(s => s.fld_JumlahCuti - s.fld_JumlahCutiDiambil).FirstOrDefault();

            result = getdata > 0 ? true : false;

            return result;
        }

        public bool CheckLeaveType(string KodKatCuti, int? NegaraID, int? SyarikatID)
        {
            bool result = false;

            var getdata = db.tbl_CutiKategori.Where(x => x.fld_KodCuti == KodKatCuti && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Count();

            result = getdata > 0 ? true : false;

            return result;
        }

        public void LeaveDeduct(MVC_SYSTEM_Models dbr, int year, string NoPkj, string KodKatCuti, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            var getdata = dbr.tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NoPkj == NoPkj && x.fld_KodCuti == KodKatCuti && x.fld_Tahun == year).FirstOrDefault();
            getdata.fld_JumlahCutiDiambil = getdata.fld_JumlahCutiDiambil + 1;
            dbr.SaveChanges();
        }

        public void LeaveAdd(MVC_SYSTEM_Models dbr, int year, string NoPkj, string KodKatCuti, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            var getdata = dbr.tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NoPkj == NoPkj && x.fld_KodCuti == KodKatCuti && x.fld_Tahun == year).FirstOrDefault();
            getdata.fld_JumlahCutiDiambil = getdata.fld_JumlahCutiDiambil - 1;
            dbr.SaveChanges();
        }

        public bool GroupCheckLeaveTake(List<tbl_Kerjahdr> tbl_Kerjahdrs, int? NegaraID, int? SyarikatID, out List<tbl_Kerjahdr> returntbl_Kerjahdr)
        {
            bool result = false;
            returntbl_Kerjahdr = new List<tbl_Kerjahdr>();

            var getkodct = tbl_Kerjahdrs.Select(s => s.fld_Kdhdct).ToArray();
            var getdata = db.tbl_CutiKategori.Where(x => getkodct.Contains(x.fld_KodCuti) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_KodCuti).ToArray();

            result = getdata.Count() > 0 ? true : false;

            if (result)
            {
                returntbl_Kerjahdr = tbl_Kerjahdrs.Where(x => getdata.Contains(x.fld_Kdhdct)).ToList();
            }

            return result;
        }

        public bool IndividuCheckLeaveTake(string kodcuti, int? NegaraID, int? SyarikatID)
        {
            bool result = false;

            var getdata = db.tbl_CutiKategori.Where(x => x.fld_KodCuti == kodcuti && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_KodCuti).Count();

            result = getdata > 0 ? true : false;

            return result;
        }

        public string PkjName(MVC_SYSTEM_Models dbr, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, string Nopkj)
        {
            string namepkj = "";

            namepkj = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == Nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nama).FirstOrDefault();

            return namepkj;
        }

        public List<CustMod_WorkerWork> RecordWorkingList(MVC_SYSTEM_Models dbr, int SelectionCategory, string SelectionData, DateTime SelectDate, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
            List<CustMod_WorkerWork> CustMod_WorkerWorks = new List<CustMod_WorkerWork>();
            string namepkj = "";
            if (SelectionCategory == 1)
            {
                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Kum == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
            }
            else
            {
                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
            }

            foreach (var tbl_KerjaData in tbl_KerjaList)
            {
                namepkj = PkjName(dbr, NegaraID, SyarikatID, WilayahID, LadangID, tbl_KerjaData.fld_Nopkj);
                CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_Unit = tbl_KerjaData.fld_Unit, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount, fld_DailyIncentive = tbl_KerjaData.fld_DailyIncentive });
            }

            return CustMod_WorkerWorks;
        }

        public string GetHariTerabaiJnsCharge(MVC_SYSTEM_Models dbr, string NoPkj, DateTime? SelectDate, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            string Result = "";
            Result = "";//dbr.tbl_KerjaHariTerabai.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_JenisCharge).FirstOrDefault();
            return Result;
        }

        public CustMod_NNActCode GetNNActCode(MVC_SYSTEM_Models dbr, byte JnisPkt, string GetPkt, string AktvtCd, string NNCC, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? DivisionID, byte TrnsfrLvl)
        {
            CustMod_NNActCode CustMod_NNActCode = new CustMod_NNActCode();
            string GetWBSCode = "";

            var GetSAPActCode = db.tbl_SAPOPMSActMapping.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_OPMSActCode == AktvtCd).Select(s => s.fld_SAPActCode).FirstOrDefault();

            if (GetSAPActCode != null)
            {
                if (TrnsfrLvl == 0)
                {
                    if (JnisPkt == 1)
                    {
                        GetWBSCode = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID && x.fld_PktUtama == GetPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                    }
                    else
                    {
                        var GetSubPktDetail = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktUtama).FirstOrDefault();
                        GetWBSCode = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID && x.fld_PktUtama == GetSubPktDetail).Select(s => s.fld_IOcode).FirstOrDefault();
                    }

                    var GetSAPNN = db.tbl_SAPPDPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_WBSCode == GetWBSCode && x.fld_KodKategori == NNCC && x.fld_ActivityCode == GetSAPActCode).Select(s => s.fld_NetworkNo).FirstOrDefault();
                    if (GetSAPNN == null)
                    {
                        CustMod_NNActCode = null;
                    }
                    else
                    {
                        CustMod_NNActCode.ActvtCodeSAP = GetSAPActCode;
                        CustMod_NNActCode.NetworkNoSAP = GetSAPNN;
                    }
                }
                else
                {
                    var SelectPkt = dbr.tbl_PktPinjam.Where(x => x.fld_KodPkt == GetPkt && x.fld_JenisPkt == JnisPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                    GetWBSCode = SelectPkt.fld_SAPCode;

                    var GetSAPNN = db.tbl_SAPPDPUP.Where(x => x.fld_NegaraID == SelectPkt.fld_NegaraIDAsal && x.fld_SyarikatID == SelectPkt.fld_SyarikatIDAsal && x.fld_WilayahID == SelectPkt.fld_WilayahIDAsal && x.fld_LadangID == SelectPkt.fld_LadangIDAsal && x.fld_WBSCode == GetWBSCode && x.fld_KodKategori == NNCC && x.fld_ActivityCode == GetSAPActCode).Select(s => s.fld_NetworkNo).FirstOrDefault();
                    if (GetSAPNN == null)
                    {
                        CustMod_NNActCode = null;
                    }
                    else
                    {
                        CustMod_NNActCode.ActvtCodeSAP = GetSAPActCode;
                        CustMod_NNActCode.NetworkNoSAP = GetSAPNN;
                    }
                }
            }
            else
            {
                CustMod_NNActCode = null;
            }

            return CustMod_NNActCode;
        }

        public bool CheckSAPGLMap(MVC_SYSTEM_Models dbr, byte? JenisPkt, string GetPkt, string AktvtCd, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, bool HariTerabai, string JenisKiraanHariTerabai, out string GLCode)
        {
            bool Result = false;
            GLCode = "";
            if (!HariTerabai)
            {
                //switch (JenisPkt)
                //{
                //    case 1:
                //        //Take GetPkt Direct
                //        break;
                //    case 2:
                //        GetPkt = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktUtama).FirstOrDefault();
                //        break;
                //    case 3:
                //        GetPkt = dbr.tbl_Blok.Where(x => x.fld_Blok == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktutama).FirstOrDefault();
                //        break;
                //}
                ////Check untuk lot felda and peneroka
                //var PktData = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama == GetPkt).Select(s => new { s.fld_StatusTnmn, s.fld_IOcode }).FirstOrDefault();
                //string GetPaySheetID = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == PktData.fld_StatusTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfFlag2).FirstOrDefault();

                //get GL Code
                var GLMap = db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();

                Result = GLMap != null ? true : false;
                GLCode = GLMap != null ? GLMap.fld_KodGL : "";
            }
            else
            {
                if (JenisKiraanHariTerabai == "kong")
                {
                    //switch (JenisPkt)
                    //{
                    //    case 1:
                    //        //Take GetPkt Direct
                    //        break;
                    //    case 2:
                    //        GetPkt = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktUtama).FirstOrDefault();
                    //        break;
                    //    case 3:
                    //        GetPkt = dbr.tbl_Blok.Where(x => x.fld_Blok == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktutama).FirstOrDefault();
                    //        break;
                    //}

                    //var PktData = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama == GetPkt).Select(s => new { s.fld_StatusTnmn, s.fld_IOcode }).FirstOrDefault();
                    //string GetPaySheetID = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == PktData.fld_StatusTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfFlag2).FirstOrDefault();

                    //get GL Code
                    var GLMap = db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();

                    Result = GLMap != null ? true : false;
                    GLCode = GLMap != null ? GLMap.fld_KodGL : "";
                }
                else
                {
                    Result = true;
                    GLCode = "";
                }
            }
            return Result;
        }

        public bool CheckSAPGLMap2(MVC_SYSTEM_Models dbr, byte? JenisPkt, string GetPkt, string AktvtCd, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, bool HariTerabai, string JenisKiraanHariTerabai, out string GLCode, List<tbl_MapGL> List_MapGL)
        {
            bool Result = false;
            GLCode = "";
            if (!HariTerabai)
            {
                //switch (JenisPkt)
                //{
                //    case 1:
                //        //Take GetPkt Direct
                //        break;
                //    case 2:
                //        GetPkt = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktUtama).FirstOrDefault();
                //        break;
                //    case 3:
                //        GetPkt = dbr.tbl_Blok.Where(x => x.fld_Blok == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktutama).FirstOrDefault();
                //        break;
                //}
                ////Check untuk lot felda and peneroka
                //var PktData = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama == GetPkt).Select(s => new { s.fld_StatusTnmn, s.fld_IOcode }).FirstOrDefault();
                //string GetPaySheetID = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == PktData.fld_StatusTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfFlag2).FirstOrDefault();

                //get GL Code
                var GLMap = db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();

                Result = GLMap != null ? true : false;
                GLCode = GLMap != null ? GLMap.fld_KodGL : "";
            }
            else
            {
                if (JenisKiraanHariTerabai == "kong")
                {
                    //switch (JenisPkt)
                    //{
                    //    case 1:
                    //        //Take GetPkt Direct
                    //        break;
                    //    case 2:
                    //        GetPkt = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktUtama).FirstOrDefault();
                    //        break;
                    //    case 3:
                    //        GetPkt = dbr.tbl_Blok.Where(x => x.fld_Blok == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktutama).FirstOrDefault();
                    //        break;
                    //}

                    //var PktData = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama == GetPkt).Select(s => new { s.fld_StatusTnmn, s.fld_IOcode }).FirstOrDefault();
                    //string GetPaySheetID = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == PktData.fld_StatusTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfFlag2).FirstOrDefault();

                    //get GL Code
                    var GLMap = db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();

                    Result = GLMap != null ? true : false;
                    GLCode = GLMap != null ? GLMap.fld_KodGL : "";
                }
                else
                {
                    Result = true;
                    GLCode = "";
                }
            }
            return Result;
        }

        public bool CheckSAPGLMap2(MVC_SYSTEM_Models dbr, string NoPkj, string AktvtCd, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, bool HariTerabai, string JenisKiraanHariTerabai, out string GLCode, out string PaySheetID)
        {
            bool Result = false;
            GLCode = "";
            PaySheetID = "";
            var GetJenisPkjGL = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsGL" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new { s.fldOptConfValue, s.fldOptConfFlag3 }).ToList();
            var GetPkerja = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            var GetGLKodPkjRefer = GetPkerja.Join(GetJenisPkjGL, j => new { krkytan = j.fld_Kdrkyt }, k => new { krkytan = k.fldOptConfFlag3 }, (j, k) => new { k.fldOptConfValue, j.fld_Nopkj, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID }).Select(s => s.fldOptConfValue).FirstOrDefault();
            var GetJnsPkj = GetGLKodPkjRefer == null ? GetJenisPkjGL.Where(x => x.fldOptConfFlag3 == "OTH").Select(s => s.fldOptConfValue).FirstOrDefault() : GetGLKodPkjRefer;
            if (!HariTerabai)
            {
                //get GL Code
                var GLMap = db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_Paysheet == GetJnsPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();

                Result = GLMap != null ? true : false;
                GLCode = GLMap != null ? GLMap.fld_KodGL : "";
                PaySheetID = GetJnsPkj;
            }
            else
            {
                if (JenisKiraanHariTerabai == "kong")
                {
                    //get GL Code
                    var GLMap = db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_Paysheet == GetJnsPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();

                    Result = GLMap != null ? true : false;
                    GLCode = GLMap != null ? GLMap.fld_KodGL : "";
                }
                else
                {
                    Result = true;
                    GLCode = "";
                }
            }
            return Result;
        }

        public void SaveDataKerjaSAP(MVC_SYSTEM_Models dbr, List<tbl_Kerja> DataKerja, List<CustMod_Work> HadirData, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, string GLCode, string NNCC, string SAPActCode)
        {
            //get PaysheetID
            //int? JenisPkt = DataKerja.Select(s => s.fld_JnsPkt).Take(1).FirstOrDefault();
            //string GetPkt = DataKerja.Select(s => s.fld_KodPkt).Take(1).FirstOrDefault();
            //switch (JenisPkt)
            //{
            //    case 1:
            //        //Take GetPkt Direct
            //        break;
            //    case 2:
            //        GetPkt = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktUtama).FirstOrDefault();
            //        break;
            //    case 3:
            //        GetPkt = dbr.tbl_Blok.Where(x => x.fld_Blok == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktutama).FirstOrDefault();
            //        break;
            //}

            //var PktData = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama == GetPkt).Select(s=>new { s.fld_StatusTnmn, s.fld_IOcode }).FirstOrDefault();
            //string GetPaySheetID = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == PktData.fld_StatusTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfFlag2).FirstOrDefault();

            //get GL Code
            //string AktvtCd = DataKerja.Select(s => s.fld_KodAktvt).Take(1).FirstOrDefault();
            string GLCd = "";//db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_Paysheet == GetPaySheetID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_KodGL).FirstOrDefault();
            var GetSapKodAct = db.tbl_SAPOPMSActMapping.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).ToList();

            List<tbl_KerjaSAPData> KerjaSAPDatas = new List<tbl_KerjaSAPData>();

            foreach (var EachDataKerja in DataKerja)
            {
                if (EachDataKerja.fld_JnisAktvt == "E")
                {
                    SAPActCode = GetSapKodAct.Where(x => x.fld_OPMSActCode == EachDataKerja.fld_KodAktvt).Select(s => s.fld_SAPActCode).FirstOrDefault();
                }
                else
                {
                    SAPActCode = null;
                }
                GLCd = HadirData.Where(x => x.nopkj == EachDataKerja.fld_Nopkj).Select(s => s.sapgl).FirstOrDefault();
                KerjaSAPDatas.Add(new tbl_KerjaSAPData { fld_GLKod = GLCd, fld_IOKod = null, fld_KerjaID = EachDataKerja.fld_ID, fld_PaySheetID = null, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_NNCC = NNCC, fld_KodAktivitiSAP = SAPActCode });
            }

            dbr.tbl_KerjaSAPData.AddRange(KerjaSAPDatas);
            dbr.SaveChanges();

        }

        public void SaveDataKerjaSAP2(MVC_SYSTEM_Models dbr, List<tbl_Kerja> DataKerja, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            //get PaysheetID
            string GetPkt = DataKerja.Select(s => s.fld_KodPkt).Take(1).FirstOrDefault();

            string GetPaySheetID = "";

            //get GL Code
            string GLCd = "";

            List<tbl_KerjaSAPData> KerjaSAPDatas = new List<tbl_KerjaSAPData>();

            foreach (var EachDataKerja in DataKerja)
            {
                CheckSAPGLMap2(dbr, EachDataKerja.fld_Nopkj, EachDataKerja.fld_KodAktvt, NegaraID, SyarikatID, WilayahID, LadangID, false, "-", out GLCd, out GetPaySheetID);
                KerjaSAPDatas.Add(new tbl_KerjaSAPData { fld_GLKod = GLCd, fld_IOKod = "-", fld_KerjaID = EachDataKerja.fld_ID, fld_PaySheetID = GetPaySheetID, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
            }

            dbr.tbl_KerjaSAPData.AddRange(KerjaSAPDatas);
            dbr.SaveChanges();

        }

        public bool GetStatusCutProcess(MVC_SYSTEM_Models dbr, DateTime? DateSelected, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? DivisionID)
        {
            bool result = false;
            DateTime NowDate = timezone.gettimezone();
            DateTime FirstDayNowDate = new DateTime(NowDate.Year, NowDate.Month, 1);
            DateTime FirstDaySelectedDate = new DateTime(DateSelected.Value.Year, DateSelected.Value.Month, 1);
            int GetCutOfDateDay = int.Parse(GetConfig.GetWebConfigValue("haritrakhir", NegaraID, SyarikatID));
            DateTime CutOfDate = FirstDayNowDate.AddDays(GetCutOfDateDay);

            var getTtpUrsNiaga = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID  && x.fld_DivisionID == DivisionID && x.fld_Year == DateSelected.Value.Year && x.fld_Month == DateSelected.Value.Month).FirstOrDefault();

            if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga != null)
            {
                result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
            }
            else if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga == null)
            {
                result = false;
            }
            else if (FirstDayNowDate > FirstDaySelectedDate && (getTtpUrsNiaga != null || getTtpUrsNiaga == null))
            {
                if (NowDate >= CutOfDate && (getTtpUrsNiaga == null || getTtpUrsNiaga != null))
                {
                    result = true;
                }
                else if (NowDate < CutOfDate && getTtpUrsNiaga != null)
                {
                    result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
                }
                else if (NowDate < CutOfDate && getTtpUrsNiaga == null)
                {
                    result = false;
                }
            }

            return result;
        }

        public bool GetStatusCutGenProcess(MVC_SYSTEM_Models dbr, DateTime? DateSelected, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? DivisionID, out bool LastCloseBizStatus)
        {
            bool result = false;
            LastCloseBizStatus = false;
            DateTime NowDate = timezone.gettimezone();
            DateTime FirstDayNowDate = new DateTime(NowDate.Year, NowDate.Month, 1);
            DateTime FirstDaySelectedDate = new DateTime(DateSelected.Value.Year, DateSelected.Value.Month, 1);
            int GetCutOfDateDay = int.Parse(GetConfig.GetWebConfigValue("haritrakhir", NegaraID, SyarikatID));
            DateTime CutOfDate = FirstDayNowDate.AddDays(GetCutOfDateDay);
            DateTime LastMonthCheck = DateSelected.Value.AddMonths(-1);

            var getTtpUrsNiaga = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == DateSelected.Value.Year && x.fld_Month == DateSelected.Value.Month && x.fld_DivisionID == DivisionID).FirstOrDefault();
            var LastMonthStatus = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == LastMonthCheck.Year && x.fld_Month == LastMonthCheck.Month && x.fld_StsTtpUrsNiaga == true && x.fld_DivisionID == DivisionID).FirstOrDefault();

            if (LastMonthStatus != null)
            {
                LastCloseBizStatus = true;
                if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga != null)
                {
                    result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
                }
                else if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga == null)
                {
                    result = false;
                }
                else if (FirstDayNowDate > FirstDaySelectedDate && (getTtpUrsNiaga != null || getTtpUrsNiaga == null))
                {
                    if (NowDate >= CutOfDate && (getTtpUrsNiaga == null || getTtpUrsNiaga != null))
                    {
                        result = true;
                    }
                    else if (NowDate < CutOfDate && getTtpUrsNiaga != null)
                    {
                        result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
                    }
                    else if (NowDate < CutOfDate && getTtpUrsNiaga == null)
                    {
                        result = false;
                    }
                }
            }
            else
            {
                var NullDataTtpUrsNiaga = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID).Count();
                if (NullDataTtpUrsNiaga > 0)
                {
                    result = true;
                    LastCloseBizStatus = false;
                }
                else
                {
                    result = false;
                    LastCloseBizStatus = true;
                }
            }
            return result;
        }

        public bool GetStatusCutGenProcess2(MVC_SYSTEM_Models dbr, DateTime? DateSelected, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? DivisionID, out bool LastCloseBizStatus)
        {
            bool result = false;
            LastCloseBizStatus = false;
            DateTime NowDate = timezone.gettimezone();
            DateTime FirstDayNowDate = new DateTime(NowDate.Year, NowDate.Month, 1);
            DateTime FirstDaySelectedDate = new DateTime(DateSelected.Value.Year, DateSelected.Value.Month, 1);
            int GetCutOfDateDay = int.Parse(GetConfig.GetWebConfigValue("haritrakhir", NegaraID, SyarikatID));
            DateTime CutOfDate = FirstDayNowDate.AddDays(GetCutOfDateDay);
            DateTime LastMonthCheck = DateSelected.Value.AddMonths(-1);

            var getTtpUrsNiaga = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == DateSelected.Value.Year && x.fld_Month == DateSelected.Value.Month && x.fld_DivisionID == DivisionID && x.fld_StsTtpUrsNiaga == true).FirstOrDefault();
            if (getTtpUrsNiaga != null)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            //var LastMonthStatus = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == LastMonthCheck.Year && x.fld_Month == LastMonthCheck.Month && x.fld_StsTtpUrsNiaga == true && x.fld_DivisionID == DivisionID).FirstOrDefault();
            //if (LastMonthStatus != null)
            //{
            //    LastCloseBizStatus = true;
            //    if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga != null)
            //    {
            //        result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
            //    }
            //    else if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga == null)
            //    {
            //        result = false;
            //    }
            //    else if (FirstDayNowDate > FirstDaySelectedDate && (getTtpUrsNiaga != null || getTtpUrsNiaga == null))
            //    {
            //        if (NowDate >= CutOfDate && (getTtpUrsNiaga == null || getTtpUrsNiaga != null))
            //        {
            //            result = true;
            //        }
            //        else if (NowDate < CutOfDate && getTtpUrsNiaga != null)
            //        {
            //            result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
            //        }
            //        else if (NowDate < CutOfDate && getTtpUrsNiaga == null)
            //        {
            //            result = false;
            //        }
            //    }
            //}
            //else
            //{
            //    var NullDataTtpUrsNiaga = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID).Count();
            //    if (NullDataTtpUrsNiaga > 0)
            //    {
            //        result = true;
            //        LastCloseBizStatus = false;
            //    }
            //    else
            //    {
            //        result = false;
            //        LastCloseBizStatus = true;
            //    }
            //}
            return result;
        }

        public decimal? SpecialRateTable(DateTime SelectDate, int JnisPkt, string PilihanPkt, string kdhmnuai, MVC_SYSTEM_Models dbr, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, out bool YieldBracketFullMonth, byte Option)
        {
            decimal? kadar = 0;
            decimal? hasilsawit = 0;
            decimal? hasilsawit2 = 0;
            DateTime Last12Month = SelectDate.AddMonths(-12);
            DateTime LatestMonth = SelectDate.AddMonths(-1);
            YieldBracketFullMonth = false;

            switch (Option)
            {
                case 1:
                    switch (JnisPkt)
                    {
                        case 1:
                            for (var fordate = Last12Month; fordate <= LatestMonth; fordate = fordate.AddMonths(1))
                            {
                                hasilsawit = dbr.tbl_HasilSawitPkt.Where(x => x.fld_Bulan == fordate.Month && x.fld_Tahun == fordate.Year && x.fld_KodPeringkat == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_HasilTan).FirstOrDefault();
                                hasilsawit = hasilsawit == null ? 0 : hasilsawit;
                                if (hasilsawit == 0)
                                {
                                    hasilsawit2 = 0;
                                    YieldBracketFullMonth = false;
                                    break;
                                }
                                else
                                {
                                    YieldBracketFullMonth = true;
                                    hasilsawit2 += hasilsawit;
                                }
                            }
                            break;
                        case 2:
                            for (var fordate = Last12Month; SelectDate > fordate; fordate = fordate.AddMonths(1))
                            {
                                hasilsawit = dbr.tbl_HasilSawitSubPkt.Where(x => x.fld_Bulan == fordate.Month && x.fld_Tahun == fordate.Year && x.fld_KodSubPeringkat == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_HasilTan).FirstOrDefault();
                                hasilsawit = hasilsawit == null ? 0 : hasilsawit;
                                if (hasilsawit == 0)
                                {
                                    hasilsawit2 = 0;
                                    YieldBracketFullMonth = false;
                                    break;
                                }
                                else
                                {
                                    YieldBracketFullMonth = true;
                                    hasilsawit2 += hasilsawit;
                                }
                            }
                            break;
                        case 3:
                            for (var fordate = Last12Month; SelectDate > fordate; fordate = fordate.AddMonths(1))
                            {
                                hasilsawit = dbr.tbl_HasilSawitBlok.Where(x => x.fld_Bulan == fordate.Month && x.fld_Tahun == fordate.Year && x.fld_KodBlok == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_HasilTan).FirstOrDefault();
                                hasilsawit = hasilsawit == null ? 0 : hasilsawit;
                                if (hasilsawit == 0)
                                {
                                    hasilsawit2 = 0;
                                    YieldBracketFullMonth = false;
                                    break;
                                }
                                else
                                {
                                    YieldBracketFullMonth = true;
                                    hasilsawit2 += hasilsawit;
                                }
                            }
                            break;
                    }

                    kadar = hasilsawit2 == 0 ? 0 : db.tbl_UpahMenuai.Where(x => hasilsawit2 >= x.fld_HasilLower && hasilsawit2 <= x.fld_HasilUpper && x.fld_Jadual == kdhmnuai).Select(s => s.fld_Kadar).FirstOrDefault();
                    break;
                case 2:
                    string PktUtama = "";

                    switch (JnisPkt)
                    {
                        case 1:
                            var SelectPkt = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                            PktUtama = SelectPkt.fld_PktUtama;
                            break;
                        case 2:
                            var SelectPkt2 = dbr.tbl_SubPkt.Join(dbr.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktUtama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Pkt, j.fld_NamaPkt, j.fld_Deleted, j.fld_KodPktUtama }).Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Pkt == PilihanPkt).FirstOrDefault();
                            PktUtama = SelectPkt2.fld_Pkt;
                            break;
                        case 3:
                            var SelectPkt3 = dbr.tbl_Blok.Join(dbr.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktutama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Blok, j.fld_NamaBlok, j.fld_Deleted, k.fld_PktUtama }).Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Blok == PilihanPkt).FirstOrDefault();
                            PktUtama = SelectPkt3.fld_PktUtama;
                            break;
                    }

                    var GetHarvestRate = dbr.tbl_HargaMenuai.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_KodPeringkatUtama == PktUtama).Select(s => s.fld_HargaMenuai).FirstOrDefault();
                    if (GetHarvestRate != null)
                    {
                        kadar = GetHarvestRate;
                        YieldBracketFullMonth = true;
                    }
                    else
                    {
                        kadar = 0;
                        YieldBracketFullMonth = false;
                    }
                    break;
                case 0:
                    kadar = 0;
                    break;
            }

            return kadar;
        }

        //added by shah on 03.08.2020
        public decimal? SpecialRateTableIncldPnjmPkrja(DateTime SelectDate, int JnisPkt, string PilihanPkt, string kdhmnuai, MVC_SYSTEM_Models dbr, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, out bool YieldBracketFullMonth, byte Option, byte trnferlvl)
        {
            decimal? kadar = 0;
            decimal? hasilsawit = 0;
            decimal? hasilsawit2 = 0;
            DateTime Last12Month = SelectDate.AddMonths(-12);
            DateTime LatestMonth = SelectDate.AddMonths(-1);
            YieldBracketFullMonth = false;

            switch (Option)
            {
                case 1:
                    switch (JnisPkt)
                    {
                        case 1:
                            for (var fordate = Last12Month; fordate <= LatestMonth; fordate = fordate.AddMonths(1))
                            {
                                hasilsawit = dbr.tbl_HasilSawitPkt.Where(x => x.fld_Bulan == fordate.Month && x.fld_Tahun == fordate.Year && x.fld_KodPeringkat == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_HasilTan).FirstOrDefault();
                                hasilsawit = hasilsawit == null ? 0 : hasilsawit;
                                if (hasilsawit == 0)
                                {
                                    hasilsawit2 = 0;
                                    YieldBracketFullMonth = false;
                                    break;
                                }
                                else
                                {
                                    YieldBracketFullMonth = true;
                                    hasilsawit2 += hasilsawit;
                                }
                            }
                            break;
                        case 2:
                            for (var fordate = Last12Month; SelectDate > fordate; fordate = fordate.AddMonths(1))
                            {
                                hasilsawit = dbr.tbl_HasilSawitSubPkt.Where(x => x.fld_Bulan == fordate.Month && x.fld_Tahun == fordate.Year && x.fld_KodSubPeringkat == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_HasilTan).FirstOrDefault();
                                hasilsawit = hasilsawit == null ? 0 : hasilsawit;
                                if (hasilsawit == 0)
                                {
                                    hasilsawit2 = 0;
                                    YieldBracketFullMonth = false;
                                    break;
                                }
                                else
                                {
                                    YieldBracketFullMonth = true;
                                    hasilsawit2 += hasilsawit;
                                }
                            }
                            break;
                        case 3:
                            for (var fordate = Last12Month; SelectDate > fordate; fordate = fordate.AddMonths(1))
                            {
                                hasilsawit = dbr.tbl_HasilSawitBlok.Where(x => x.fld_Bulan == fordate.Month && x.fld_Tahun == fordate.Year && x.fld_KodBlok == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_HasilTan).FirstOrDefault();
                                hasilsawit = hasilsawit == null ? 0 : hasilsawit;
                                if (hasilsawit == 0)
                                {
                                    hasilsawit2 = 0;
                                    YieldBracketFullMonth = false;
                                    break;
                                }
                                else
                                {
                                    YieldBracketFullMonth = true;
                                    hasilsawit2 += hasilsawit;
                                }
                            }
                            break;
                    }

                    kadar = hasilsawit2 == 0 ? 0 : db.tbl_UpahMenuai.Where(x => hasilsawit2 >= x.fld_HasilLower && hasilsawit2 <= x.fld_HasilUpper && x.fld_Jadual == kdhmnuai).Select(s => s.fld_Kadar).FirstOrDefault();
                    break;
                case 2:
                    string PktUtama = "";
                    switch (trnferlvl)
                    {
                        case 0:
                            switch (JnisPkt)
                            {
                                case 1:
                                    var SelectPkt = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                                    PktUtama = SelectPkt.fld_PktUtama;
                                    break;
                                case 2:
                                    var SelectPkt2 = dbr.tbl_SubPkt.Join(dbr.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktUtama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Pkt, j.fld_NamaPkt, j.fld_Deleted, j.fld_KodPktUtama }).Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Pkt == PilihanPkt).FirstOrDefault();
                                    PktUtama = SelectPkt2.fld_Pkt;
                                    break;
                                case 3:
                                    var SelectPkt3 = dbr.tbl_Blok.Join(dbr.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktutama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Blok, j.fld_NamaBlok, j.fld_Deleted, k.fld_PktUtama }).Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Blok == PilihanPkt).FirstOrDefault();
                                    PktUtama = SelectPkt3.fld_PktUtama;
                                    break;
                            }
                            var GetHarvestRate = dbr.tbl_HargaMenuai.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_KodPeringkatUtama == PktUtama).Select(s => s.fld_HargaMenuai).FirstOrDefault();
                            if (GetHarvestRate != null)
                            {
                                kadar = GetHarvestRate;
                                YieldBracketFullMonth = true;
                            }
                            else
                            {
                                kadar = 0;
                                YieldBracketFullMonth = false;
                            }
                            break;
                        case 1:
                            var SelectPktP = dbr.tbl_PktPinjam.Where(x => x.fld_JenisPkt == JnisPkt && x.fld_KodPkt == PilihanPkt).FirstOrDefault();
                            var SelectPkt4 = dbr.tbl_PktUtama.Where(x => x.fld_IOcode == SelectPktP.fld_SAPCode && x.fld_Deleted == false).FirstOrDefault();
                            PktUtama = SelectPkt4.fld_PktUtama;
                            var GetHarvestRate2 = dbr.tbl_HargaMenuai.Where(x => x.fld_NegaraID == SelectPkt4.fld_NegaraID && x.fld_SyarikatID == SelectPkt4.fld_SyarikatID && x.fld_WilayahID == SelectPkt4.fld_WilayahID && x.fld_LadangID == SelectPkt4.fld_LadangID && x.fld_Deleted == false && x.fld_KodPeringkatUtama == PktUtama).Select(s => s.fld_HargaMenuai).FirstOrDefault();
                            if (GetHarvestRate2 != null)
                            {
                                kadar = GetHarvestRate2;
                                YieldBracketFullMonth = true;
                            }
                            else
                            {
                                kadar = 0;
                                YieldBracketFullMonth = false;
                            }
                            break;
                    }
                    break;
                case 0:
                    kadar = 0;
                    break;
            }

            return kadar;
        }

        public bool CheckHariTerabai(string nopkj, DateTime? Tarikh, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            bool Result = false;
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetHariTerabai = dbr.tbl_KerjaHariTerabai.Where(x => x.fld_Nopkj == nopkj && x.fld_Tarikh == Tarikh && x.fld_JenisCharge == "kong" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();

            Result = GetHariTerabai > 0 ? true : false;

            return Result;
        }

        public string GetAttType(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, DateTime SelectedDate)
        {
            string Result = "";
            string DefaultCode = "H01";
            //Cuti Hari Minggu
            int getday = (int)SelectedDate.DayOfWeek;//SelectedDate.DayOfWeek.ToString();
            var CheckData = db.tbl_JenisMingguLadang.Where(x => x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            Result = CheckData != null ? "C07" : DefaultCode;
            //Cuti Am
            var CheckHoliday = db.vw_CutiUmumLdgDetails.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_CutiUmumDeleted == false).FirstOrDefault();
            Result = CheckHoliday != null ? "C01" : Result;

            return Result;
        }

        public string GetEasyAttType(int? CodeNegeri, int? NegaraID, int? SyarikatID, DateTime SelectedDate)
        {
            string Result = "";
            string DefaultCode = "H01";
            //Cuti Hari Minggu
            int getday = (int)SelectedDate.DayOfWeek;//SelectedDate.DayOfWeek.ToString();
            var CheckData = db.tbl_MingguNegeri.Where(x => x.fld_NegeriID == CodeNegeri && x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            Result = CheckData != null ? "H02" : DefaultCode;
            //Cuti Am
            int? CodeNegeriConvrt = CodeNegeri;
            var CheckHoliday = db.tbl_CutiUmum.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_Negeri == CodeNegeriConvrt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_IsSelected == true).FirstOrDefault();
            Result = CheckHoliday != null ? "H03" : Result;

            return Result;
        }

        public string GetHadirStatus(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            string ReturnData = "";
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfValue == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfFlag2)
                .FirstOrDefault();

            if (getvalue == "hadirkerja")
            {
                ReturnData = "Hadir";
            }
            else
            {
                ReturnData = "Tidak Hadir";
            }

            return ReturnData;
        }

        public string GetHadirCutiDesc(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            string ReturnData = "";
            ReturnData = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfValue == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfDesc)
                .FirstOrDefault();

            return ReturnData;
        }

        public bool GetCutiAmMgguMatchDate(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, DateTime SelectedDate, string CutiCode, out string Msg)
        {
            bool Result = true;
            Msg = "";
            if (CutiCode == "C01" || CutiCode == "H03")
            {
                //var CheckHoliday = db.vw_CutiUmumLdgDetails.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_CutiUmumDeleted == false).FirstOrDefault();
                //Result = CheckHoliday != null ? true : false;
                //Msg = "Tarikh pilihan bukan cuti am";
            }
            //else if (CutiCode == "C07" || CutiCode == "H02")
            //{
            //    int getday = (int)SelectedDate.DayOfWeek;
            //    var CheckData = db.tbl_MingguNegeri.Where(x => x.fld_NegeriID == CodeNegeri && x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            //    Result = CheckData != null ? true : false;
            //    Msg = "Tarikh pilihan bukan hari minggu";
            //}
            else
            {
                var CheckHoliday = db.vw_CutiUmumLdgDetails.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_CutiUmumDeleted == false).FirstOrDefault();
                int getday = (int)SelectedDate.DayOfWeek;
                var CheckData = db.tbl_JenisMingguLadang.Where(x => x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                if (CheckHoliday == null && CheckData == null)
                {
                    Result = true;
                }
                else
                {
                    if (CheckHoliday != null)
                    {
                        //Msg = "Tarikh pilihan adalah cuti am";
                        Result = true;
                    }
                    //else if (CheckData != null)
                    //{
                    //    Msg = "Tarikh pilihan adalah cuti mingguan";
                    //    Result = false;
                    //}
                    else
                    {
                        Result = true;
                    }


                }
            }
            return Result;
            //bool Result = true;
            //Msg = "";
            //if (CutiCode == "C01" || CutiCode == "H03")
            //{
            //    var CheckHoliday = db.vw_CutiUmumLdgDetails.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_CutiUmumDeleted == false).FirstOrDefault();
            //    Result = CheckHoliday != null ? true : false;
            //    Msg = "Tarikh pilihan bukan cuti am";
            //}
            ////else if (CutiCode == "C07" || CutiCode == "H02")
            ////{
            ////    int getday = (int)SelectedDate.DayOfWeek;
            ////    var CheckData = db.tbl_MingguNegeri.Where(x => x.fld_NegeriID == CodeNegeri && x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            ////    Result = CheckData != null ? true : false;
            ////    Msg = "Tarikh pilihan bukan hari minggu";
            ////}
            //else
            //{
            //    var CheckHoliday = db.vw_CutiUmumLdgDetails.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_CutiUmumDeleted == false).FirstOrDefault();
            //    int getday = (int)SelectedDate.DayOfWeek;
            //    var CheckData = db.tbl_JenisMingguLadang.Where(x => x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            //    if (CheckHoliday == null && CheckData == null)
            //    {
            //        Result = true;
            //    }
            //    else
            //    {
            //        if (CheckHoliday != null)
            //        {
            //            Msg = "Tarikh pilihan adalah cuti am";
            //            Result = false;
            //        }
            //        else if (CheckData != null)
            //        {
            //            if (CutiCode == "C07" || CutiCode == "H02")
            //            {
            //                Result = true;
            //            }
            //            else
            //            {
            //                Msg = "Tarikh pilihan adalah cuti mingguan";
            //                Result = false;
            //            }
            //        }
            //        else
            //        {
            //            Result = true;
            //        }


            //    }
            //}
            //return Result;
        }

        public short GetDataKerjaStatus(string nopkj, DateTime? Tarikh, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            short Return = 0;

            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == "cuti" && x.fldOptConfFlag2 == "xhadirkerja" && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfValue).ToArray();

            var GetKhdiran = dbr.tbl_Kerjahdr.Where(x => x.fld_Tarikh == Tarikh && x.fld_Nopkj == nopkj && getvalue.Contains(x.fld_Kdhdct) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
            if (GetKhdiran > 0)
            {
                Return = 1;
            }
            else
            {
                var GetDataKerja = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == nopkj && x.fld_Tarikh == Tarikh && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                if (GetDataKerja > 0)
                {
                    Return = 2;
                }
                else
                {
                    Return = 3;
                }

            }

            return Return;
        }

        public void DeleteReturnSAPPost(Guid PostingID, MVC_SYSTEM_Models dbr)
        {
            var SAPPostReturn = dbr.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == PostingID).ToList();

            if (SAPPostReturn.Count > 0)
            {
                dbr.tbl_SAPPostReturn.RemoveRange(SAPPostReturn);
                dbr.SaveChanges();
            }
        }

        public void AddReturnSAPPost(MVC_SYSTEM_Models dbr, List<tbl_SAPPostReturn> SAPReturnList)
        {
            dbr.tbl_SAPPostReturn.AddRange(SAPReturnList);
            dbr.SaveChanges();
        }

        public List<CustMod_DailyIncentive> GetDailyIncentiveList(int? NegaraID, int? SyarikatID)
        {
            int ID = 2;
            List<CustMod_DailyIncentive> DailyIncentiveList = new List<CustMod_DailyIncentive>();

            var GetIncentives = db.tbl_JenisInsentif.Where(x => x.fld_KelayakanKepada == 10 && x.fld_Deleted == false).ToList();

            DailyIncentiveList.Add(new CustMod_DailyIncentive() { ID = 1, IncentiveCode = "N/A", IncentiveDesc = "No Incentive" });

            foreach (var GetIncentive in GetIncentives)
            {
                DailyIncentiveList.Add(new CustMod_DailyIncentive() { ID = ID, IncentiveCode = GetIncentive.fld_KodInsentif, IncentiveDesc = GetIncentive.fld_Keterangan });
                ID++;
            } 
            
            return DailyIncentiveList;
        }

        public decimal? LastMonthPayment(string nopkj, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int LastMonth_Month, int LastMonth_Year, int CurrentMonth_Month, int CurrentMonth_Year)
        {
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetLastMonthSalary = dbr.tbl_GajiBulanan.Where(x => x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == LastMonth_Month && x.fld_Year == LastMonth_Year).FirstOrDefault();

            if (GetLastMonthSalary != null)
            {
                return GetLastMonthSalary.fld_PurataGaji;
            }
            else
            {
                return 0;
            }
        }

        public void PaySlipAddInfo(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int Month, int Year, string NoPkj, out int? WorkingDay, out decimal? SlryCurrentMonth, out decimal? SlryLastMonth)
        {
            //shah 20/08/2020
            WorkingDay = 0;
            SlryLastMonth = 0m;
            SlryCurrentMonth = 0m;
            try
            {
                var GetLadangInfo = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).FirstOrDefault();
                int KodNegeri = int.Parse(GetLadangInfo.fld_KodNegeri);
                var GetWorkingDayOffer = db.tbl_HariBekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Month == Month && x.fld_Year == Year && x.fld_NegeriID == KodNegeri).FirstOrDefault();

                WorkingDay = GetWorkingDayOffer.fld_BilanganHariBekerja.Value;

                DateTime CurrentMonth = new DateTime(Year, Month, 1);
                DateTime LastMonth = CurrentMonth.AddMonths(-1);
                int YearLastMonth = LastMonth.Year;
                int MonthLastMonth = LastMonth.Month;
                string host, catalog, user, pass = "";
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                var GetSalaryInfo = dbr.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && ((x.fld_Month == Month && x.fld_Year == Year) || (x.fld_Month == MonthLastMonth && x.fld_Year == YearLastMonth)) && x.fld_Nopkj == NoPkj).ToList();
                var GetPublicHoliday = db.vw_CutiUmumLdgDetails.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_TarikhCuti.Value.Month == Month && x.fld_TarikhCuti.Value.Year == Year && x.fld_Deleted == false).Count();
                var GetSlryCurrentMonth = GetSalaryInfo.Where(x => x.fld_Year == Year && x.fld_Month == Month).FirstOrDefault();
                var GetSlryLastMonth = GetSalaryInfo.Where(x => x.fld_Year == YearLastMonth && x.fld_Month == MonthLastMonth).FirstOrDefault();
                SlryCurrentMonth = GetSlryCurrentMonth.fld_PurataGaji.Value;
                WorkingDay = WorkingDay - GetPublicHoliday;
                if (GetSlryLastMonth == null)
                {
                    SlryLastMonth = GetSlryCurrentMonth.fld_PurataGaji.Value;
                }
                else
                {
                    SlryLastMonth = GetSlryLastMonth.fld_PurataGaji.Value;
                }
            }
            catch
            {
                WorkingDay = WorkingDay > 0 ? WorkingDay : 0;
                SlryLastMonth = SlryLastMonth > 0 ? SlryLastMonth : 0;
                SlryCurrentMonth = SlryCurrentMonth > 0 ? SlryCurrentMonth : 0;
            }
        }
    }
}