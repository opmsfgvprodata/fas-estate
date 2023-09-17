using MVC_SYSTEM.ViewingModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace MVC_SYSTEM.Class
{
    public class GetGenerateEwalletFile
    {
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetTriager GetTriager = new GetTriager();

        //added by faeza 02.08.2022 - new algorithm - top up portal 4.0
        public string GenFileEwallet(List<vw_PaySheetPekerja> vw_PaySheetPekerja, MasterModels.tbl_Ladang tbl_Ladang, string bulan, string tahun, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, out string filename)
        {
            decimal? TotalSalary = vw_PaySheetPekerja.Sum(s => s.fld_GajiBersih);
            decimal TotalSalaryC = Math.Round((decimal)TotalSalary, 0);
            int TotalSalaryInt = int.Parse(TotalSalaryC.ToString());
            decimal TotalSHA256 = 0;
            decimal Last4pan = 0;
            decimal TotalPan = 0;
            string month = "";
            string day = "";
            string TelNo = "";
            string NewTelNo = "";
            string NoKp = "";
            string NewNoKp = "";
            decimal? Salary = 0;
            decimal Last4panTimeSalary = 0;

            DateTime NowDate = timezone.gettimezone();
            string yy = NowDate.Year.ToString();
            string mm = NowDate.Month.ToString();
            string dd = NowDate.Day.ToString();
            if (mm.Length == 1)
            {
                month = "0" + mm;
            }
            else
            {
                month = mm;
            }

            if (dd.Length == 1)
            {
                day = "0" + dd;
            }
            else
            {
                day = dd;
            }

            string filePath = "~/eWalletFile/" + tahun + "/" + bulan + "/" + LadangID.ToString() + "/";
            string path = HttpContext.Current.Server.MapPath(filePath);
            filename = "PAY" + yy + month + day + "01" + ".txt";
            string filecreation = path + filename;

            TryToDelete(filecreation);
            if (!Directory.Exists(path))
            {
                //If No any such directory then creates the new one
                Directory.CreateDirectory(path);
            }

            string Header1 = "StaffId,";
            string Header2 = "CustomerName,";
            string Header3 = "IdentificationNumber,";
            string Header4 = "MobileNumber,";
            string Header5 = "LastFourPAN,";
            string Header6 = "SalaryAmount,";
            string Header7 = "PaymentDescription";

            string Body1 = "";//fld_noPkj
            string Body2 = "";//fld_nama
            string Body3 = "";//fld_nokp
            string Body4 = "";//fld_notel
            string Body5 = "";//fld_last4pan
            string Body6 = "";//fld_gajibersih
            string Body7 = "";//description

            string Footer = "";

            using (StreamWriter writer = new StreamWriter(filecreation, true))
            {
                writer.Write(Header1);
                writer.Write(Header2);
                writer.Write(Header3);
                writer.Write(Header4);
                writer.Write(Header5);
                writer.Write(Header6);
                writer.WriteLine(Header7);

                foreach (var eWalletFileDetail in vw_PaySheetPekerja)
                {
                    Last4pan = decimal.Parse(eWalletFileDetail.fld_Last4Pan);
                    Salary = eWalletFileDetail.fld_GajiBersih;

                    Last4panTimeSalary = (decimal)(Last4pan * Salary);

                    //remove space & special char NoTel
                    TelNo = eWalletFileDetail.fld_Notel;
                    NewTelNo = Regex.Replace(TelNo, @"[^0-9]+", "");

                    if (NewTelNo.Substring(0, 1) == "0")
                    {
                        TelNo = "6" + NewTelNo;
                    }
                    else
                    {
                        TelNo = NewTelNo;
                    }

                    //remove space & special char NoKp
                    NoKp = eWalletFileDetail.fld_Nokp;
                    NewNoKp = Regex.Replace(NoKp, @"[^0-9a-zA-Z]+", "");

                    Body1 = eWalletFileDetail.fld_Nopkj + ",";
                    Body2 = eWalletFileDetail.fld_Nama.ToUpper() + ",";
                    Body3 = NewNoKp.ToUpper() + ",";
                    Body4 = TelNo + ",";
                    Body5 = eWalletFileDetail.fld_Last4Pan + ",";
                    Body6 = Salary + ",";
                    Body7 = tbl_Ladang.fld_LdgCode.Trim() + "- Salary payment for " + bulan + "/" + tahun;

                    writer.Write(Body1);
                    writer.Write(Body2);
                    writer.Write(Body3);
                    writer.Write(Body4);
                    writer.Write(Body5);
                    writer.Write(Body6);
                    writer.WriteLine(Body7);

                    TotalPan += Last4panTimeSalary;
                }

                TotalSHA256 = TotalPan;
                var sha256 = ComputeSha256Hash(TotalSHA256.ToString());

                Footer = sha256;

                writer.Write(Footer);
            }

            return filePath;
        }

        //commented by faeza 30.09.2022 - previous algorithm - 3.0
        //public string GenFileEwallet(List<vw_PaySheetPekerja> vw_PaySheetPekerja, MasterModels.tbl_Ladang tbl_Ladang, string bulan, string tahun, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, out string filename)
        //{
        //    decimal? TotalSalary = vw_PaySheetPekerja.Sum(s => s.fld_GajiBersih);
        //    decimal TotalSalaryC = Math.Round((decimal)TotalSalary, 0);
        //    int TotalSalaryInt = int.Parse(TotalSalaryC.ToString());
        //    int SinglePan1, SinglePan2, SinglePan3, SinglePan4, TotalSinglePan, TotalPan = 0;
        //    decimal TotalSHA256 = 0;
        //    string Last4pan = "";
        //    string month = "";
        //    string day = "";
        //    string TelNo = "";
        //    string NewTelNo = "";
        //    string NoKp = "";
        //    string NewNoKp = "";
        //    decimal? Salary = 0;
        //    decimal SalaryC = 0;
        //    int SalaryInt = 0;

        //    DateTime NowDate = timezone.gettimezone();
        //    string yy = NowDate.Year.ToString();
        //    string mm = NowDate.Month.ToString();
        //    string dd = NowDate.Day.ToString();
        //    if (mm.Length == 1)
        //    {
        //        month = "0" + mm;
        //    }
        //    else
        //    {
        //        month = mm;
        //    }

        //    if (dd.Length == 1)
        //    {
        //        day = "0" + dd;
        //    }
        //    else
        //    {
        //        day = dd;
        //    }

        //    string filePath = "~/eWalletFile/" + tahun + "/" + bulan + "/" + LadangID.ToString() + "/";
        //    string path = HttpContext.Current.Server.MapPath(filePath);
        //    filename = "PAY" + yy + month + day + "01" + ".txt";
        //    string filecreation = path + filename;

        //    TryToDelete(filecreation);
        //    if (!Directory.Exists(path))
        //    {
        //        //If No any such directory then creates the new one
        //        Directory.CreateDirectory(path);
        //    }

        //    string Header1 = "StaffId,";
        //    string Header2 = "CustomerName,";
        //    string Header3 = "IdentificationNumber,";
        //    string Header4 = "MobileNumber,";
        //    string Header5 = "LastFourPAN,";
        //    string Header6 = "SalaryAmount,";
        //    string Header7 = "PaymentDescription";

        //    string Body1 = "";//fld_noPkj
        //    string Body2 = "";//fld_nama
        //    string Body3 = "";//fld_nokp
        //    string Body4 = "";//fld_notel
        //    string Body5 = "";//fld_last4pan
        //    string Body6 = "";//fld_gajibersih
        //    string Body7 = "";//description

        //    string Footer = "";
        //    //string Footer1 = "";
        //    //string Footer2 = "";

        //    using (StreamWriter writer = new StreamWriter(filecreation, true))
        //    {
        //        writer.Write(Header1);
        //        writer.Write(Header2);
        //        writer.Write(Header3);
        //        writer.Write(Header4);
        //        writer.Write(Header5);
        //        writer.Write(Header6);
        //        writer.WriteLine(Header7);

        //        foreach (var eWalletFileDetail in vw_PaySheetPekerja)
        //        {
        //            Last4pan = eWalletFileDetail.fld_Last4Pan;
        //            SinglePan1 = Int32.Parse(Last4pan[0].ToString());
        //            SinglePan2 = Int32.Parse(Last4pan[1].ToString());
        //            SinglePan3 = Int32.Parse(Last4pan[2].ToString());
        //            SinglePan4 = Int32.Parse(Last4pan[3].ToString());
        //            TotalSinglePan = SinglePan1 + SinglePan2 + SinglePan3 + SinglePan4;

        //            //remove space & special char NoTel
        //            TelNo = eWalletFileDetail.fld_Notel;
        //            NewTelNo = Regex.Replace(TelNo, @"[^0-9]+", "");

        //            if (NewTelNo.Substring(0, 1) == "0")
        //            {
        //                TelNo = "6" + NewTelNo;
        //            }
        //            else
        //            {
        //                TelNo = NewTelNo;
        //            }

        //            //remove space & special char NoKp
        //            NoKp = eWalletFileDetail.fld_Nokp;
        //            NewNoKp = Regex.Replace(NoKp, @"[^0-9a-zA-Z]+", "");

        //            Body1 = eWalletFileDetail.fld_Nopkj + ",";
        //            Body2 = eWalletFileDetail.fld_Nama.ToUpper() + ",";
        //            Body3 = NewNoKp.ToUpper() + ",";
        //            Body4 = TelNo + ",";
        //            Body5 = eWalletFileDetail.fld_Last4Pan + ",";
        //            Salary = eWalletFileDetail.fld_GajiBersih;
        //            //SalaryC = Math.Round((decimal)Salary, 0);
        //            //SalaryInt = int.Parse(SalaryC.ToString());
        //            Body6 = Salary + ",";
        //            Body7 = tbl_Ladang.fld_LdgCode.Trim() + "- Salary payment for " + bulan + "/" + tahun;

        //            writer.Write(Body1);
        //            writer.Write(Body2);
        //            writer.Write(Body3);
        //            writer.Write(Body4);
        //            writer.Write(Body5);
        //            writer.Write(Body6);
        //            writer.WriteLine(Body7);

        //            TotalPan += TotalSinglePan;
        //        }

        //        TotalSHA256 = (decimal)(TotalPan + TotalSalary);
        //        var sha256 = ComputeSha256Hash(TotalSHA256.ToString());

        //        Footer = sha256;
        //        //Footer1 = TotalSHA256.ToString();
        //        //Footer2 = TotalPan.ToString();

        //        writer.Write(Footer);
        //        //writer.Write(Footer1);
        //        //writer.Write(Footer2);
        //    }

        //    return filePath;
        //}

        static string ComputeSha256Hash(string rawData)
        {
            //var md5Hasher = MD5.Create();
            //var bytes = md5Hasher.ComputeHash(Encoding.Default.GetBytes(rawData));
            // Create a SHA256   
            //string returnStr = "";

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                //returnStr = System.Convert.ToBase64String(bytes);
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }

            //return returnStr;
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