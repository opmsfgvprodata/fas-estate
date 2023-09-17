using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetGenerateFile
    {
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetTriager GetTriager = new GetTriager();
        public string GenFileMaybank(List<vw_MaybankFile> vw_MaybankFile, MasterModels.tbl_Ladang tbl_Ladang, string bulan, string tahun, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, out string filename)
        {
            string filePath = "~/MaybankFile/" + tahun + "/" + bulan + "/" + NegaraID.ToString() + "_" + SyarikatID.ToString() + "/" + WilayahID.ToString() + "/";
            string path = HttpContext.Current.Server.MapPath(filePath);
            filename = "MBBOPMS" + tbl_Ladang.fld_LdgCode + bulan + tahun + ".txt";
            string filecreation = path + filename;
            decimal? TotalSalary = vw_MaybankFile.Sum(s => s.fld_GajiBersih) * 100;
            decimal TotalSalaryC = Math.Round((decimal)TotalSalary,0);
            int TotalSalaryInt = int.Parse(TotalSalaryC.ToString());
            int rowno = 1;
            DateTime NowDate = timezone.gettimezone();
            TryToDelete(filecreation);
            if (!Directory.Exists(path))
            {
                //If No any such directory then creates the new one
                Directory.CreateDirectory(path);
            }
            string Header1 = "H";
            string Header2 = "APS";
            string Header3 = "P";
            string Header4 = tbl_Ladang.fld_LdgCode;
            string Header5 = tbl_Ladang.fld_OriginatorID.ToString();
            string Header6 = "";
            string Header7 = "";
            string Header8 = tbl_Ladang.fld_NoAcc.ToString();
            string Header9 = string.Format("{0:ddMMyyyy}", NowDate);
            string Header10 = "";
            string Header11 = string.Format("{0:ddMMyyyy}", NowDate);
            string Header12 = TotalSalaryInt.ToString();
            string Header13 = "WORKERS SALARY";
            string Header14 = " " + bulan + "/" + tahun;
            string Header15 = "";
            string Header16 = "";
            string Header17 = "";
            string Header18 = "";
            string Header19 = "";
            string Header20 = "";
            string Header21 = "";
            string Header22 = "";
            string Header23 = "";
            string Header24 = "";
            string Header25 = "";
            string Header26 = "D" + tbl_Ladang.fld_OriginatorID.ToString() + string.Format("{0:ddMMyyyyhhmmss}", NowDate); ;
            string Header27 = "";

            decimal? Salary = 0; 
            decimal SalaryC = 0; 
            int SalaryInt = 0; 
            string Body1 = "D";
            string Body2 = "";
            string Body3 = "";
            string Body4 = "";
            string Body5 = "";
            string Body6 = "";
            string Body7 = "015";
            string Body8 = "001";
            string Body9 = "";
            string Body10 = "";
            string Body11 = "";
            string Body12 = "";
            string Body13 = "";
            string Body14 = "";
            string Body15 = "";
            string Body16 = "";
            string Body17 = "";

            string Footer1 = "T";
            string Footer2 = "";
            string Footer3 = "";
            string Footer4 = "";
            string Footer5 = "";
            string Footer6 = "";
            string Footer7 = "";
            string Footer8 = "";
            string Footer9 = "";
            string Footer10 = "";
            string Footer11 = "";
            string Footer12 = "";
            string Footer13 = "";

            using (StreamWriter writer = new StreamWriter(filecreation, true))
            {
                writer.Write(Header1.PadRight(1, ' ')); //
                writer.Write(Header2.PadRight(3, ' ')); //
                writer.Write(Header3.PadRight(1, ' ')); //
                writer.Write(Header4.PadRight(3, ' ')); //
                writer.Write(Header5.PadRight(5, ' ')); //
                writer.Write(Header6.PadRight(13, ' ')); //
                writer.Write(Header7.PadRight(40, ' ')); //
                writer.Write(Header8.PadRight(12, ' ')); //
                writer.Write(Header9.PadRight(8, ' ')); //
                writer.Write(Header10.PadRight(20, ' ')); //
                writer.Write(Header11.PadRight(8, ' ')); //
                writer.Write(Header12.PadLeft(15, '0')); //
                writer.Write(Header13.PadRight(14, ' ')); //
                writer.Write(Header14.PadRight(40, ' ')); //
                writer.Write(Header15.PadRight(1, ' ')); //
                writer.Write(Header16.PadRight(1, ' ')); //
                writer.Write(Header17.PadRight(1, ' ')); //
                writer.Write(Header18.PadRight(3, ' ')); //
                writer.Write(Header19.PadRight(12, ' ')); //
                writer.Write(Header20.PadRight(12, ' ')); //
                writer.Write(Header21.PadRight(8, ' ')); //
                writer.Write(Header22.PadRight(6, ' ')); //
                writer.Write(Header23.PadRight(40, ' ')); //
                writer.Write(Header24.PadRight(1, ' ')); //
                writer.Write(Header25.PadRight(40, ' ')); //
                writer.Write(Header26.PadRight(18, ' ')); //
                writer.WriteLine(Header27.PadRight(54, ' '));
                foreach (var MaybankFileDetail in vw_MaybankFile)
                {
                    Body2 = GetTriager.GetRowNoMaybank(rowno);
                    Body3 = MaybankFileDetail.fld_Nama;
                    Body5 = MaybankFileDetail.fld_Kdrkyt == "MA" ? MaybankFileDetail.fld_Nokp : "";
                    Body6 = MaybankFileDetail.fld_Kdrkyt == "MA" ? "" : MaybankFileDetail.fld_Nokp;
                    Body9 = MaybankFileDetail.fld_NoAkaun == null ? "" : MaybankFileDetail.fld_NoAkaun;
                    Salary = MaybankFileDetail.fld_GajiBersih * 100;
                    SalaryC = Math.Round((decimal)Salary, 0);
                    SalaryInt = int.Parse(SalaryC.ToString());
                    Body10 = SalaryInt.ToString();
                    Body12 = MaybankFileDetail.fld_Nopkj;

                    writer.Write(Body1.PadRight(1, ' ')); //
                    writer.Write(Body2.PadRight(8, ' ')); //
                    writer.Write(Body3.PadRight(40, ' ')); //
                    writer.Write(Body4.PadRight(12, ' ')); //
                    writer.Write(Body5.PadRight(12, ' ')); //
                    writer.Write(Body6.PadRight(12, ' ')); //
                    writer.Write(Body7.PadRight(3, ' ')); //
                    writer.Write(Body8.PadRight(3, ' ')); //
                    writer.Write(Body9.PadRight(20, ' ')); //
                    writer.Write(Body10.PadLeft(15, '0')); //
                    writer.Write(Body11.PadRight(40, ' ')); //
                    writer.Write(Body12.PadRight(20, ' ')); //
                    writer.Write(Body13.PadRight(40, ' ')); //
                    writer.Write(Body14.PadRight(120, ' ')); //
                    writer.Write(Body15.PadRight(2, ' ')); //
                    writer.Write(Body16.PadRight(30, ' ')); //
                    writer.WriteLine(Body17.PadRight(2, ' ')); //

                    rowno++;
                }
                rowno = rowno - 1;
                Footer2 = rowno.ToString();
                Footer3 = Header12;
                Footer4 = "0";
                Footer5 = "0";
                Footer6 = "0";
                Footer7 = "0";
                Footer8 = "0";
                Footer9 = "0";
                Footer10 = "0";
                Footer11 = "0";
                Footer12 = "0";
                Footer13 = "";

                writer.Write(Footer1.PadRight(1, ' ')); //
                writer.Write(Footer2.PadLeft(15, '0')); //
                writer.Write(Footer3.PadLeft(15, '0')); //
                writer.Write(Footer4.PadLeft(30, '0')); //
                writer.Write(Footer5.PadLeft(15, '0')); //
                writer.Write(Footer6.PadLeft(15, '0')); //
                writer.Write(Footer7.PadLeft(15, '0')); //
                writer.Write(Footer8.PadLeft(15, '0')); //
                writer.Write(Footer9.PadLeft(15, '0')); //
                writer.Write(Footer10.PadLeft(15, '0')); //
                writer.Write(Footer11.PadLeft(15, '0')); //
                writer.Write(Footer12.PadLeft(15, '0')); //
                writer.Write(Footer13.PadRight(199, ' ')); //

            }

            return filePath;
        }

        static bool TryToDelete(string f)
        {
            try
            {
                // A.
                // Try to delete the file.
                File.Delete(f);
                return true;
            }
            catch (IOException)
            {
                // B.
                // We could not delete the file.
                return false;
            }
        }
    }
}