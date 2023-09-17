using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels2
{
    public class CustMod_DetailWork
    {
        public long id { get; set; }
        public string columnheader { get; set; }
        public string costcenter { get; set; }
        public string workercostcenter { get; set; }
        public string levelmaincode { get; set; }
        public string levelcode { get; set; }
        public string leveldesc { get; set; }
        public string actvtcode { get; set; }
        public string actvtdesc { get; set; }
        public string gl { get; set; }
        public string grp { get; set; }
        public string nopkj { get; set; }
        public string namepkj { get; set; }
        public decimal kongpay { get; set; }
        public decimal ottime { get; set; }
        public decimal otpay { get; set; }
        public decimal piecerateqnty { get; set; }
        public decimal pieceratepay { get; set; }
        public decimal incentive { get; set; }
        public decimal otherincome { get; set; }
        public decimal totalincome { get; set; }
        public decimal totaldeduction { get; set; }
        public decimal netincome { get; set; }
        public int fld_NegaraID { get; set; }
        public int fld_SyarikatID { get; set; }
        public int fld_WilayahID { get; set; }
        public int fld_LadangID { get; set; }
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string active { get; set; }
        public decimal emplyrcntrbtn { get; set; } //fitri add 7-12-2021
    }

    public class CustMod_DetailWork2
    {
        public long id { get; set; }
        public string cc { get; set; }
        public string pkt { get; set; }
        public string actvt { get; set; }
        public string actvtdesc { get; set; }
        public string gl { get; set; }
        public string grp { get; set; }
        public string wrk { get; set; }
        public string wrkname { get; set; }
        public short amounttype { get; set; }
        public decimal quantity { get; set; }
        public decimal amount { get; set; }
        public int fld_NegaraID { get; set; }
        public int fld_SyarikatID { get; set; }
        public int fld_WilayahID { get; set; }
        public int fld_LadangID { get; set; }
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string active { get; set; }
    }

    public class CustMod_DetailWork3
    {
        public long id { get; set; }
        public string columnheader { get; set; }
        public string cc { get; set; }
        public string pkt { get; set; }
        public string actvt { get; set; }
        public string actvtdesc { get; set; }
        public string gl { get; set; }
        public string grp { get; set; }
        public string wrk { get; set; }
        public string wrkname { get; set; }
        public decimal getjan { get; set; }
        public decimal getfeb { get; set; }
        public decimal getmac { get; set; }
        public decimal getapr { get; set; }
        public decimal getmay { get; set; }
        public decimal getjun { get; set; }
        public decimal getjul { get; set; }
        public decimal getaug { get; set; }
        public decimal getsep { get; set; }
        public decimal getoct { get; set; }
        public decimal getnov { get; set; }
        public decimal getdec { get; set; }
        public decimal gettotal { get; set; }
    }

    public class CustMod_DetailWork4
    {
        public long id { get; set; }
        public string columnheader { get; set; }
        public string cc { get; set; }
        public string pkt { get; set; }
        public string actvt { get; set; }
        public string actvtdesc { get; set; }
        public string gl { get; set; }
        public string grp { get; set; }
        public string wrk { get; set; }
        public string wrkname { get; set; }
        public decimal day1 { get; set; }
        public decimal day2 { get; set; }
        public decimal day3 { get; set; }
        public decimal day4 { get; set; }
        public decimal day5 { get; set; }
        public decimal day6 { get; set; }
        public decimal day7 { get; set; }
        public decimal day8 { get; set; }
        public decimal day9 { get; set; }
        public decimal day10 { get; set; }
        public decimal day11 { get; set; }
        public decimal day12 { get; set; }
        public decimal day13 { get; set; }
        public decimal day14 { get; set; }
        public decimal day15 { get; set; }
        public decimal day16 { get; set; }
        public decimal day17 { get; set; }
        public decimal day18 { get; set; }
        public decimal day19 { get; set; }
        public decimal day20 { get; set; }
        public decimal day21 { get; set; }
        public decimal day22 { get; set; }
        public decimal day23 { get; set; }
        public decimal day24 { get; set; }
        public decimal day25 { get; set; }
        public decimal day26 { get; set; }
        public decimal day27 { get; set; }
        public decimal day28 { get; set; }
        public decimal day29 { get; set; }
        public decimal day30 { get; set; }
        public decimal day31 { get; set; }
        public decimal gettotal { get; set; }
        public decimal insentif { get; set; } //fitri add 7-12-2021
        public decimal getemplyrcntribtn { get; set; } //fitri add 7-12-2021
        public decimal totaldeduct { get; set; } //fitri add 7-12-2021

    }

    public class coloumgroup
    {
        public int id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
    }
    public class detailsreport
    {
        public List<CustMod_DetailWork2> CustMod_DetailWork2 { get; set; }
        public List<coloumgroup> coloumgroup { get; set; }
    }

    public class amountdata
    {
        public decimal getamountk { get; set; }
        public decimal getquantityot { get; set; }
        public decimal getamountot { get; set; }
        public decimal getamountothr { get; set; }
        public decimal getquantityp { get; set; }
        public decimal getamountp { get; set; }
        public decimal getamountinc { get; set; }
        public decimal getamountdec { get; set; }
        public decimal total { get; set; }
        public decimal netincome { get; set; }
        public decimal getemplyrcntribtn { get; set; } //fitri add 7-12-2021
    }

    public class amountdata2
    {
        public decimal getjan { get; set; }
        public decimal getfeb { get; set; }
        public decimal getmac { get; set; }
        public decimal getapr { get; set; }
        public decimal getmay { get; set; }
        public decimal getjun { get; set; }
        public decimal getjul { get; set; }
        public decimal getaug { get; set; }
        public decimal getsep { get; set; }
        public decimal getoct { get; set; }
        public decimal getnov { get; set; }
        public decimal getdec { get; set; }
        public decimal gettotal { get; set; }
    }

    public class amountdata3
    {
        public decimal day1 { get; set; }
        public decimal day2 { get; set; }
        public decimal day3 { get; set; }
        public decimal day4 { get; set; }
        public decimal day5 { get; set; }
        public decimal day6 { get; set; }
        public decimal day7 { get; set; }
        public decimal day8 { get; set; }
        public decimal day9 { get; set; }
        public decimal day10 { get; set; }
        public decimal day11 { get; set; }
        public decimal day12 { get; set; }
        public decimal day13 { get; set; }
        public decimal day14 { get; set; }
        public decimal day15 { get; set; }
        public decimal day16 { get; set; }
        public decimal day17 { get; set; }
        public decimal day18 { get; set; }
        public decimal day19 { get; set; }
        public decimal day20 { get; set; }
        public decimal day21 { get; set; }
        public decimal day22 { get; set; }
        public decimal day23 { get; set; }
        public decimal day24 { get; set; }
        public decimal day25 { get; set; }
        public decimal day26 { get; set; }
        public decimal day27 { get; set; }
        public decimal day28 { get; set; }
        public decimal day29 { get; set; }
        public decimal day30 { get; set; }
        public decimal day31 { get; set; }
        public decimal gettotal { get; set; }
        public decimal insentif { get; set; } //fitri add 7-12-2021
        public decimal getemplyrcntribtn { get; set; } //fitri add 7-12-2021
        public decimal totaldeduct { get; set; } //fitri add 7-12-2021

    }
}