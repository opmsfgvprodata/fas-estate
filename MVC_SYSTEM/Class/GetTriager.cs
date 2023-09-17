using System;
using System.Collections.Generic;
using System.Linq;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;

namespace MVC_SYSTEM.Class
{
    public class GetTriager
    {
        ChangeTimeZone timezone = new ChangeTimeZone();
        MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        GetConfig GetConfig = new GetConfig();
        
        public string GetTotalForMoney(decimal? no)
        {
            string result = "";

            if (no == 0 || !no.HasValue)
            {
                result = "0.00";
            }
            else
            {
                result = Math.Round((Double)no, 2).ToString("N");
            }

            return result;
        }
        
        public string GetMonthName(int month)
        {
            string result = "";

            switch (month)
            {
                case 1:
                    result = GlobalResGeneral.hdrM1.ToUpper();
                    break;
                case 2:
                    result = GlobalResGeneral.hdrM2.ToUpper();
                    break;
                case 3:
                    result = GlobalResGeneral.hdrM3.ToUpper();
                    break;
                case 4:
                    result = GlobalResGeneral.hdrM4.ToUpper();
                    break;
                case 5:
                    result = GlobalResGeneral.hdrM5.ToUpper();
                    break;
                case 6:
                    result = GlobalResGeneral.hdrM6.ToUpper();
                    break;
                case 7:
                    result = GlobalResGeneral.hdrM7.ToUpper();
                    break;
                case 8:
                    result = GlobalResGeneral.hdrM8.ToUpper();
                    break;
                case 9:
                    result = GlobalResGeneral.hdrM9.ToUpper();
                    break;
                case 10:
                    result = GlobalResGeneral.hdrM10.ToUpper();
                    break;
                case 11:
                    result = GlobalResGeneral.hdrM11.ToUpper();
                    break;
                case 12:
                    result = GlobalResGeneral.hdrM12.ToUpper();
                    break;
            }

            return result;
        }

        public string getDayName(int day)
        {
            string result = "";

            switch (day)
            {
                case 0:
                    result = GlobalResGeneral.hdrD1;
                    break;
                case 1:
                    result = GlobalResGeneral.hdrD2;
                    break;
                case 2:
                    result = GlobalResGeneral.hdrD3;
                    break;
                case 3:
                    result = GlobalResGeneral.hdrD4;
                    break;
                case 4:
                    result = GlobalResGeneral.hdrD5;
                    break;
                case 5:
                    result = GlobalResGeneral.hdrD6;
                    break;
                case 6:
                    result = GlobalResGeneral.hdrD7;
                    break;
            }

            return result;
        }

        public string getHQURL(int? userid)
        {
            var urlhq = db.tbl_EstateSelection.Where(x => x.fld_UserID == userid).Select(s => s.fld_HQUrl).FirstOrDefault();

            return urlhq;
        }

        public int GetTotalForDays(List<int> no)
        {
            int result = 0;

            if (no.Count == 0)
            {
                result = 0;
            }
            else
            {
                result = no.Sum(x => Convert.ToInt32(x));
            }

            return result;
        }

        public static string GetTotalForMoneyList(List<decimal?> no)
        {
            string result = "";

            if (no.Count == 0)
            {
                result = "0.00";
            }
            else
            {
                result = Math.Round(no.Sum(x => Convert.ToDecimal(x)), 2).ToString("n2");
            }

            return result;
        }

        public static string GetPercentageDivision(decimal? no)
        {
            decimal? result = 0;

            if (no == 0 || !no.HasValue)
            {
                result = 0;
            }
            else
            {
                result = no/100;
            }

            return result.ToString();
        }

        public string GetRowNoMaybank(int no)
        {
            string returndata = "";

            string noS = no.ToString();

            switch (noS.Length)
            {
                case 1:
                    returndata = "0000000" + noS;
                    break;
                case 2:
                    returndata = "000000" + noS;
                    break;
                case 3:
                    returndata = "00000" + noS;
                    break;
                case 4:
                    returndata = "0000" + noS;
                    break;
                case 5:
                    returndata = "000" + noS;
                    break;
                case 6:
                    returndata = "00" + noS;
                    break;
                case 7:
                    returndata = "0" + noS;
                    break;
                case 8:
                    returndata = noS;
                    break;
            }
            
            return returndata;
        }
        
        public string GetSAPItemNo(int? no)
        {
            string returndata = "";

            string noS = no.ToString();

            switch (noS.Length)
            {
                case 1:
                    returndata = "000000000" + noS;
                    break;
                case 2:
                    returndata = "00000000" + noS;
                    break;
                case 3:
                    returndata = "0000000" + noS;
                    break;
                case 4:
                    returndata = "000000" + noS;
                    break;
                case 5:
                    returndata = "00000" + noS;
                    break;
                case 6:
                    returndata = "0000" + noS;
                    break;
                case 7:
                    returndata = "000" + noS;
                    break;
                case 8:
                    returndata = "00" + noS;
                    break;
                case 9:
                    returndata = "0" + noS;
                    break;
                case 10:
                    returndata = noS;
                    break;
            }
            return returndata;
        }

        public static string GetProductofTwoNumber(decimal? no1, decimal? no2)
        {
            decimal? result = 0;

            if (no1 != null || no2 != null)
            {
                result = no1 * no2;
            }

            return result.ToString();
        }
        public string GetDashForNull(string data)
        {
            string result = "";
            if (data == null || data == "")
            {
                result = "-";
            }
            else
            {
                result = data;
            }
            return result;
        }

    }
}