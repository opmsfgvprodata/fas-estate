using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.Class;


namespace MVC_SYSTEM.Class
{
    public class GetEstateDetail
    {
        Connection Connection = new Connection();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();

        // add by fitri 27-01-2021
        public string GetGroupName(string groupcode, int wlyh, int syrkt, int ngra, int ldg)
        {
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, wlyh, syrkt, ngra);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string getgroupname = dbr.vw_KumpulanKerja.Where(x => x.fld_KodKumpulan == groupcode && x.fld_NegaraID == ngra && x.fld_SyarikatID == syrkt && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg && x.fld_deleted == false).Select(s => s.fld_Keterangan).FirstOrDefault();
            return getgroupname;
        }
        public string LevelName(string lvlcode, int wlyh, int syrkt, int ngra, int ldg)
        {
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, wlyh, syrkt, ngra);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string getglvl = dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == ngra && x.fld_SyarikatID == syrkt && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg && x.fld_KodPktUtama == lvlcode || x.fld_Pkt == lvlcode).Select(s => s.fld_NamaPkt).FirstOrDefault();
            return getglvl;
        }
        // end add

        public string GroupName(int groupID,int? getuserid, string getusername)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, getusername);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string groupname = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KumpulanID == groupID).Select(s => s.fld_Keterangan).FirstOrDefault();
            return groupname;
        }

        public string GroupCode(int? groupID, int? getuserid, string getusername)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, getusername);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string groupcode = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KumpulanID == groupID).Select(s => s.fld_KodKumpulan).FirstOrDefault();
            return groupcode;
        }

        public string Name(string nopkj, int wlyh, int syrkt, int ngra, int ldg)
        {
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, wlyh, syrkt, ngra);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var name = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == nopkj && x.fld_WilayahID == wlyh && x.fld_SyarikatID == syrkt && x.fld_NegaraID == ngra && x.fld_LadangID == ldg).Select(s => s.fld_Nama).FirstOrDefault();
            return name;
        }

        public string GetImageUrl(string nopkj, int ngra, int syrkt, int wlyh, int ldg)
        {
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, wlyh, syrkt, ngra);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            // Comment by fitri 17-09-2020
            //var findImage = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == nopkj && x.fld_Flag == "picPkj" && x.fld_NegaraID == ngra && x.fld_SyarikatID == syrkt && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg && x.fld_Deleted == false).Select(s => s.fld_Url).FirstOrDefault();
            // Add by fitri 17-09-2020
            var findImage = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == nopkj && x.fld_Flag == "ProfPic" && x.fld_NegaraID == ngra && x.fld_SyarikatID == syrkt && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg && x.fld_Deleted == false).Select(s => s.fld_Url).FirstOrDefault();
            if (findImage == null)
            {
                findImage = "/Asset/Images/default-user.png";
            }
            return findImage;
        }

        public string GetWBSCode(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, string MainPktCode)
        {
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string WBSCode = "";

            WBSCode = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama == MainPktCode).Select(s => s.fld_IOcode).FirstOrDefault();

            return WBSCode;
        }
    }
}