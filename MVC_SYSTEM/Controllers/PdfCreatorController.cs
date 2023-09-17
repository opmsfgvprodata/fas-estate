using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_SYSTEM.Controllers
{
    public class PdfCreatorController : Controller
    {
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        [HttpPost]
        public ActionResult PDFIndetifier(string controller, string action, string param)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string CookiesValue = Request.Cookies[FormsAuthentication.FormsCookieName].Value;

            tbl_PdfGen tbl_PdfGen = new tbl_PdfGen();

            tbl_PdfGen.fld_Controller = controller;
            tbl_PdfGen.fld_Action = action;
            tbl_PdfGen.fld_Param = param;
            tbl_PdfGen.fld_UserID = getuserid;
            tbl_PdfGen.fld_CookiesVal = CookiesValue;
            dbr.tbl_PdfGen.Add(tbl_PdfGen);
            dbr.SaveChanges();
            
            var link = Url.Action("PrintPdf", "PdfCreator", new { id = tbl_PdfGen.fld_ID});

            return Json(new { link });
        }

        public ActionResult PrintPdf(Guid id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var getPdfGen = dbr.tbl_PdfGen.Find(id);

            return Redirect(Url.Action(getPdfGen.fld_Action, getPdfGen.fld_Controller) + getPdfGen.fld_Param + "&id=" + getPdfGen.fld_UserID + "&genid=" + getPdfGen.fld_ID);
        }
    }
}