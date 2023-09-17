using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class MaybankFileGenController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private GetIdentity getidentity = new GetIdentity();
        private GetTriager GetTriager = new GetTriager();
        private GetNSWL GetNSWL = new GetNSWL();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private errorlog geterror = new errorlog();
        private GetConfig GetConfig = new GetConfig();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetWilayah GetWilayah = new GetWilayah();
        private Connection Connection = new Connection();
        private GetGenerateFile GetGenerateFile = new GetGenerateFile();
        // GET: MaybankFileGen
        public ActionResult Index()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

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

            dbr.Dispose();
            return View();
        }

        [HttpPost]
        public ActionResult Index(int Month, int Year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
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

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                var GetGaji = dbr.vw_MaybankFile.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month && (x.fld_NoAkaun != null || x.fld_NoAkaun.Length == 12)).ToList();

                var LadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();

                filePath = GetGenerateFile.GenFileMaybank(GetGaji, LadangDetail, stringmonth, stringyear, NegaraID, SyarikatID, WilayahID, LadangID, out filename);

                link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

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

        public JsonResult CheckGenDataDetail(int Month, int Year)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            string stringyear = "";
            string stringmonth = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalGaji = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetGaji = dbr.vw_MaybankFile.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month && (x.fld_NoAkaun != null || x.fld_NoAkaun.Length == 12)).ToList();

            var LadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();

            string filename = "MBBOPMS" + LadangDetail.fld_LdgCode + stringmonth + stringyear + ".txt";



            if (GetGaji.Count() != 0)
            {
                TotalGaji = GetGaji.Sum(s => s.fld_GajiBersih);
                CountData = GetGaji.Count();
                msg = GlobalResEstate.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResEstate.msgDataNotFound;
                statusmsg = "warning";
            }
                
            dbr.Dispose();
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData });
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

        public ActionResult ChequeGen()
        {
            ViewBag.MaybankFileGen = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nama)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> RangeSalaryPrint = new List<SelectListItem>();
            RangeSalaryPrint.Insert(0, (new SelectListItem { Text = "Below RM20", Value = "0" }));
            RangeSalaryPrint.Insert(0, (new SelectListItem { Text = "Above RM20", Value = "1" }));

            ViewBag.SelectionList = SelectionList;
            ViewBag.RangeSalaryPrint = RangeSalaryPrint;

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

        public FileStreamResult ChequeResult(int? MonthList, int? YearList, string RangeSalaryPrint, string SelectionList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            DateTime ChequeDate = timezone.gettimezone();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            NumberToWord NumberToWord = new NumberToWord();
            Rectangle rec = new Rectangle(504, 244);
            //rec.Border = Rectangle.BOX;
            //rec.BorderWidth = 3;
            //rec.BorderColor = new BaseColor(18, 6, 40);
            Document doc = new Document(rec, 0, 12, 0, 48);
            MemoryStream ms = new MemoryStream();
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            BaseFont bf2 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font font = new Font(bf, 8, Font.NORMAL);
            Font font2 = new Font(bf2, 8, Font.NORMAL);
            MemoryStream output = new MemoryStream();
            Phrase phrase = new Phrase();
            Phrase phrase2 = new Phrase();
            Phrase phrase3 = new Phrase();
            Phrase phrase4 = new Phrase();
            PdfPCell cell = new PdfPCell();
            PdfPTable table = new PdfPTable(2);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();
            string NamaPkj = "";
            decimal? Gaji = 0;
            string GajiWord = "";
            string DateString = ChequeDate.ToString("ddMMyy");
            string DateStringFinal = "";
            //int DateStrgLngth = DateString.Length;
            //1 Inch = 72
            for (int i = 0; i < 6; i++)
            {
                if (i == 0)
                {
                    DateStringFinal = DateString.Substring(i, 1);
                }
                else
                {
                    DateStringFinal = DateStringFinal + "    " + DateString.Substring(i, 1);
                }
            }

            if (RangeSalaryPrint == "1")
            {
                if (SelectionList == "0")
                {
                    var ListNoPkj = dbr.tbl_Pkjmast
                        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID && x.fld_Kdaktf == "1")
                        .OrderBy(o => o.fld_Nopkj).Select(s => s.fld_Nopkj).ToList();
                    var GetDataGajiPkrjas = dbr.tbl_GajiBulanan.Join(dbr.tbl_Pkjmast, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, j.fld_Nopkj }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, k.fld_Nopkj }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_Nopkj, k.fld_Nama, j.fld_GajiBersih, j.fld_Month, j.fld_Year, k.fld_DivisionID }).Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == MonthList && x.fld_Year == YearList && x.fld_DivisionID == DivisionID && ListNoPkj.Contains(x.fld_Nopkj) && x.fld_GajiBersih > 20).OrderBy(o => o.fld_Nama).ToList();
                    foreach (var GetDataGajiPkrja in GetDataGajiPkrjas)
                    {
                        NamaPkj = GetDataGajiPkrja.fld_Nama.ToUpper();
                        Gaji = GetDataGajiPkrja.fld_GajiBersih;
                        GajiWord = NumberToWord.ConvertToWords(Gaji.ToString()).ToUpper();

                        phrase = new Phrase();
                        phrase2 = new Phrase();
                        phrase3 = new Phrase();
                        phrase4 = new Phrase();
                        doc.NewPage();
                        table = new PdfPTable(2)
                        {
                            TotalWidth = 504,
                            LockedWidth = true
                        };
                        table.SetWidths(new float[] { 330, 174 });
                        phrase.Add(new Chunk("**" + NamaPkj + "**", font2));
                        cell = new PdfPCell(phrase)
                        {
                            PaddingLeft = 70,
                            PaddingTop = 84
                        };
                        cell.BorderColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        phrase2.Add(new Chunk(DateStringFinal, font));
                        cell = new PdfPCell(phrase2)
                        {
                            PaddingLeft = 50.4f,
                            PaddingTop = 53
                        };
                        cell.BorderColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        doc.Add(table);

                        table = new PdfPTable(2)
                        {
                            TotalWidth = 504,
                            LockedWidth = true
                        };
                        table.SetWidths(new float[] { 330, 174 });
                        phrase3.Add(new Chunk("**" + GajiWord + "**", font));
                        cell = new PdfPCell(phrase3)
                        {
                            PaddingLeft = 82,
                            PaddingTop = -6
                        };
                        cell.SetLeading(30, 0);
                        cell.BorderColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        phrase4.Add(new Chunk("**" + Gaji.Value.ToString("N2") + "**", font));
                        cell = new PdfPCell(phrase4)
                        {
                            PaddingLeft = 50.4f,
                            PaddingTop = 16
                        };
                        cell.BorderColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        doc.Add(table);
                    }
                    if (GetDataGajiPkrjas.Count() > 0)
                    {
                        doc.Close();
                        byte[] file = ms.ToArray();
                        output.Write(file, 0, file.Length);
                        output.Position = 0;
                    }
                    else
                    {
                        table = new PdfPTable(1)
                        {
                            TotalWidth = 504,
                            LockedWidth = true
                        };
                        phrase.Add(new Chunk("No Data", font));
                        cell = new PdfPCell(phrase)
                        {
                            HorizontalAlignment = PdfPCell.ALIGN_CENTER
                        };
                        cell.BorderColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        doc.Add(table);

                        doc.Close();
                        byte[] file = ms.ToArray();
                        output.Write(file, 0, file.Length);
                        output.Position = 0;
                    }
                }
                else
                {
                    var GetDataGajiPkrjas = dbr.tbl_GajiBulanan.Join(dbr.tbl_Pkjmast, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, j.fld_Nopkj }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, k.fld_Nopkj }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_Nopkj, k.fld_Nama, j.fld_GajiBersih, j.fld_Month, j.fld_Year, k.fld_DivisionID }).Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == MonthList && x.fld_Year == YearList && x.fld_Nopkj == SelectionList && x.fld_DivisionID == DivisionID && x.fld_GajiBersih > 20).ToList();
                    foreach (var GetDataGajiPkrja in GetDataGajiPkrjas)
                    {
                        NamaPkj = GetDataGajiPkrja.fld_Nama.ToUpper();
                        Gaji = GetDataGajiPkrja.fld_GajiBersih;
                        GajiWord = NumberToWord.ConvertToWords(Gaji.ToString()).ToUpper();

                        phrase = new Phrase();
                        phrase2 = new Phrase();
                        phrase3 = new Phrase();
                        phrase4 = new Phrase();
                        doc.NewPage();
                        table = new PdfPTable(2)
                        {
                            TotalWidth = 504,
                            LockedWidth = true
                        };
                        table.SetWidths(new float[] { 330, 174 });
                        phrase.Add(new Chunk("**" + NamaPkj + "**", font2));
                        cell = new PdfPCell(phrase)
                        {
                            PaddingLeft = 70,
                            PaddingTop = 86
                        };
                        cell.BorderColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        phrase2.Add(new Chunk(DateStringFinal, font));
                        cell = new PdfPCell(phrase2)
                        {
                            PaddingLeft = 50.4f,
                            PaddingTop = 53
                        };
                        cell.BorderColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        doc.Add(table);

                        table = new PdfPTable(2)
                        {
                            TotalWidth = 504,
                            LockedWidth = true
                        };
                        table.SetWidths(new float[] { 330, 174 });
                        phrase3.Add(new Chunk("**" + GajiWord + "**", font));
                        cell = new PdfPCell(phrase3)
                        {
                            PaddingLeft = 82,
                            PaddingTop = -6
                        };
                        cell.SetLeading(30, 0);
                        cell.BorderColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        phrase4.Add(new Chunk("**" + Gaji.Value.ToString("N2") + "**", font));
                        cell = new PdfPCell(phrase4)
                        {
                            PaddingLeft = 50.4f,
                            PaddingTop = 16
                        };
                        cell.BorderColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        doc.Add(table);
                    }

                    if (GetDataGajiPkrjas.Count() > 0)
                    {
                        doc.Close();
                        byte[] file = ms.ToArray();
                        output.Write(file, 0, file.Length);
                        output.Position = 0;
                    }
                    else
                    {
                        table = new PdfPTable(1)
                        {
                            TotalWidth = 504,
                            LockedWidth = true
                        };
                        phrase.Add(new Chunk("No Data", font));
                        cell = new PdfPCell(phrase)
                        {
                            HorizontalAlignment = PdfPCell.ALIGN_CENTER
                        };
                        cell.BorderColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        doc.Add(table);

                        doc.Close();
                        byte[] file = ms.ToArray();
                        output.Write(file, 0, file.Length);
                        output.Position = 0;
                    }
                }
            }
            else
            {
                var ListNoPkj = dbr.tbl_Pkjmast
                        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID && x.fld_Kdaktf == "1")
                        .OrderBy(o => o.fld_Nopkj).Select(s => s.fld_Nopkj).ToList();
                var GetDataGajiPkrjas = dbr.tbl_GajiBulanan.Join(dbr.tbl_Pkjmast, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, j.fld_Nopkj }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, k.fld_Nopkj }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_Nopkj, k.fld_Nama, j.fld_GajiBersih, j.fld_Month, j.fld_Year, k.fld_DivisionID }).Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == MonthList && x.fld_Year == YearList && x.fld_DivisionID == DivisionID && ListNoPkj.Contains(x.fld_Nopkj) && x.fld_GajiBersih <= 20).ToList();
                var GetTotalSalary = GetDataGajiPkrjas.Sum(s => s.fld_GajiBersih);

                if(GetTotalSalary > 0)
                {
                    NamaPkj = GetDataGajiPkrjas.OrderByDescending(o => o.fld_GajiBersih).Take(1).Select(s => s.fld_Nama).FirstOrDefault();
                    Gaji = GetTotalSalary;
                    GajiWord = NumberToWord.ConvertToWords(GetTotalSalary.ToString()).ToUpper();

                    phrase = new Phrase();
                    phrase2 = new Phrase();
                    phrase3 = new Phrase();
                    phrase4 = new Phrase();
                    doc.NewPage();
                    table = new PdfPTable(2)
                    {
                        TotalWidth = 504,
                        LockedWidth = true
                    };
                    table.SetWidths(new float[] { 330, 174 });
                    phrase.Add(new Chunk("**" + NamaPkj + "**", font2));
                    cell = new PdfPCell(phrase)
                    {
                        PaddingLeft = 70,
                        PaddingTop = 86
                    };
                    cell.BorderColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    phrase2.Add(new Chunk(DateStringFinal, font));
                    cell = new PdfPCell(phrase2)
                    {
                        PaddingLeft = 50.4f,
                        PaddingTop = 53
                    };
                    cell.BorderColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    doc.Add(table);

                    table = new PdfPTable(2)
                    {
                        TotalWidth = 504,
                        LockedWidth = true
                    };
                    table.SetWidths(new float[] { 330, 174 });
                    phrase3.Add(new Chunk("**" + GajiWord + "**", font));
                    cell = new PdfPCell(phrase3)
                    {
                        PaddingLeft = 82,
                        PaddingTop = -6
                    };
                    cell.SetLeading(30, 0);
                    cell.BorderColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    phrase4.Add(new Chunk("**" + Gaji.Value.ToString("N2") + "**", font));
                    cell = new PdfPCell(phrase4)
                    {
                        PaddingLeft = 50.4f,
                        PaddingTop = 16
                    };
                    cell.BorderColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    doc.Add(table);

                    doc.Close();
                    byte[] file = ms.ToArray();
                    output.Write(file, 0, file.Length);
                    output.Position = 0;
                }
                else
                {
                    table = new PdfPTable(1)
                    {
                        TotalWidth = 504,
                        LockedWidth = true
                    };
                    phrase.Add(new Chunk("No Data", font));
                    cell = new PdfPCell(phrase)
                    {
                        HorizontalAlignment = PdfPCell.ALIGN_CENTER
                    };
                    cell.BorderColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    doc.Add(table);

                    doc.Close();
                    byte[] file = ms.ToArray();
                    output.Write(file, 0, file.Length);
                    output.Position = 0;
                }
            }
            return new FileStreamResult(output, "application/pdf");
        }
    }
}