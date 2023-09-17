using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.MasterModels;

namespace MVC_SYSTEM.Class
{
    public class Connection
    {
        public void GetConnection(out string host, out string catalog, out string user, out string pass, int? wlyhID, int? syrktID, int? ngrID)
        {

            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
            var getconnection = db.tblConnections.Where(x => x.wilayahID == wlyhID && x.syarikatID==syrktID && x.negaraID==ngrID && x.deleted==false && x.fld_Purpose == "CHECKROLL").FirstOrDefault();
            host = getconnection.DataSource;
            catalog = getconnection.InitialCatalog;
            user = getconnection.userID;
            pass = getconnection.Password;

        }
    }
}