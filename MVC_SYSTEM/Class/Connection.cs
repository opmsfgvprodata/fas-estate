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

            //debug prod
            host = "4.144.178.156,11433";

        }

        public string GetConnectionString(int? wlyhID, int? syrktID, int? ngrID)
        {
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
            var getConnection = db.tblConnections.Where(x => x.wilayahID == wlyhID && x.syarikatID == syrktID && x.negaraID == ngrID && x.deleted == false).FirstOrDefault();
            var host = getConnection.DataSource;
            //var host = getConnection.DataSourceInternal;
            var catalog = getConnection.InitialCatalog;
            var user = getConnection.userID;
            var pass = getConnection.Password;
            //debug prod
            host = "4.144.178.156,11433";
            var connectionString = String.Format("data source={0};initial catalog={1};user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=300", host, catalog, user, pass);
            return connectionString;
        }
    }
}