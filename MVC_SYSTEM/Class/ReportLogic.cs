using MVC_SYSTEM.CustomModels2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace MVC_SYSTEM.Class
{
    public class ReportLogic
    {
        public List<CustMod_DetailWork2> CustMod_DetailWork2(List<CustMod_DetailWork2> CustMod_DetailWork2, List<string> cc, List<string> pkt, List<string> actvt, List<string> gl, List<string> grp, List<string> wrk)
        {
            var getreportdata = new List<CustMod_DetailWork2>();
            //1
            if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => actvt.Contains(x.actvt)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => gl.Contains(x.gl)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => wrk.Contains(x.wrk)).ToList();
            }
            //1
            //2
            else if (cc.Count() > 0 && pkt.Count() > 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && actvt.Contains(x.actvt)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && gl.Contains(x.gl)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt) && actvt.Contains(x.actvt)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() == 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt) && gl.Contains(x.gl)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => actvt.Contains(x.actvt) && gl.Contains(x.gl)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => actvt.Contains(x.actvt) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => actvt.Contains(x.actvt) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => gl.Contains(x.gl) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => gl.Contains(x.gl) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => grp.Contains(x.grp) && wrk.Contains(x.wrk)).ToList();
            }
            //2
            //3
            else if (cc.Count() > 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt) && actvt.Contains(x.actvt)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() > 0 && actvt.Count() == 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt) && gl.Contains(x.gl)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() > 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() > 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && actvt.Contains(x.actvt) && gl.Contains(x.gl)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && actvt.Contains(x.actvt) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && actvt.Contains(x.actvt) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && gl.Contains(x.gl) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && gl.Contains(x.gl) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && grp.Contains(x.grp) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && gl.Contains(x.gl)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => actvt.Contains(x.actvt) && gl.Contains(x.gl) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => actvt.Contains(x.actvt) && gl.Contains(x.gl) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => gl.Contains(x.gl) && grp.Contains(x.grp) && wrk.Contains(x.wrk)).ToList();
            }
            //add by fitri 7.12.2021  (grop + worker + level)
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() == 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt) && grp.Contains(x.grp) && wrk.Contains(x.wrk)).ToList();
            }
            //add by fitri 7.12.2021  (grop + worker + actvt)
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => actvt.Contains(x.actvt) && grp.Contains(x.grp) && wrk.Contains(x.wrk)).ToList();
            }
            //3
            //4
            else if (cc.Count() > 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && gl.Contains(x.gl)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() == 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && actvt.Contains(x.actvt) && gl.Contains(x.gl) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && actvt.Contains(x.actvt) && gl.Contains(x.gl) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() == 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && gl.Contains(x.gl) && grp.Contains(x.grp) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && gl.Contains(x.gl) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && gl.Contains(x.gl) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => actvt.Contains(x.actvt) && gl.Contains(x.gl) && grp.Contains(x.grp) && wrk.Contains(x.wrk)).ToList();
            }
            //4
            //5
            else if (cc.Count() > 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() == 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && gl.Contains(x.gl) && grp.Contains(x.grp)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() == 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && gl.Contains(x.gl) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() > 0 && pkt.Count() == 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && actvt.Contains(x.actvt) && gl.Contains(x.gl) && grp.Contains(x.grp) && wrk.Contains(x.wrk)).ToList();
            }
            else if (cc.Count() == 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && gl.Contains(x.gl) && grp.Contains(x.grp) && wrk.Contains(x.wrk)).ToList();
            }
            //5
            //6
            else if (cc.Count() > 0 && pkt.Count() > 0 && actvt.Count() > 0 && gl.Count() > 0 && grp.Count() > 0 && wrk.Count() > 0)
            {
                getreportdata = CustMod_DetailWork2.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && gl.Contains(x.gl) && grp.Contains(x.grp) && wrk.Contains(x.wrk)).ToList();
            }
            //6
            return getreportdata;
        }
        //public List<CustMod_DetailWork2> Q_CustMod_DetailWork2(List<CustMod_DetailWork2> CustMod_DetailWork2, string sorting)
        //{
        //    IQueryable<CustMod_DetailWork2> Q_CustMod_DetailWork2 = CustMod_DetailWork2.AsQueryable();
        //    Q_CustMod_DetailWork2 = SortingHelper.OrderBy(Q_CustMod_DetailWork2, sorting, true);
        //    CustMod_DetailWork2 = Q_CustMod_DetailWork2.ToList();
        //    return CustMod_DetailWork2;
        //}
        public List<CustMod_DetailWork> DetailReport(List<CustMod_DetailWork2> CustMod_DetailWork2, List<coloumgroup> coloumgroup)
        {
            var result = new List<CustMod_DetailWork>();
            var id = 1;
            switch (coloumgroup.Count())
            {
                case 1:
                    var gettype = coloumgroup.Select(s => s.type).FirstOrDefault();
                    switch (gettype)
                    {
                        case 1:
                            //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                            var getcc = CustMod_DetailWork2.Select(s => s.cc).Distinct().ToList();
                            foreach (var cc in getcc.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata(CustMod_DetailWork2, "cc", cc);
                                result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + cc + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = cc, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                            }
                            break;
                        case 2:
                            var getpkt = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt)).Select(s => s.pkt).Distinct().ToList();
                            foreach (var pkt in getpkt.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata(CustMod_DetailWork2, "pkt", pkt);
                                result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + pkt + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, levelcode = pkt, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                            }
                            break;
                        case 3:
                            var getactvt = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.actvt, s.actvtdesc }).Distinct().ToList();
                            foreach (var actvt in getactvt.OrderBy(o => o.actvt).ToList())
                            {
                                var getamount = amountdata(CustMod_DetailWork2, "actvt", actvt.actvt);
                                result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + actvt.actvt + " - " + actvt.actvtdesc + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, actvtcode = actvt.actvt, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                            }
                            break;
                        case 4:
                            var getgl = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl)).Select(s => s.gl).Distinct().ToList();
                            foreach (var gl in getgl.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata(CustMod_DetailWork2, "gl", gl);
                                result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + gl + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, gl = gl, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                            }
                            break;
                        case 5:
                            var getgrp = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.grp)).Select(s => s.grp).Distinct().ToList();
                            foreach (var grp in getgrp.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata(CustMod_DetailWork2, "grp", grp);
                                result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, grp = grp, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                            }
                            break;
                        case 6:
                            var getwrk = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.wrk, s.wrkname }).Distinct().ToList();
                            foreach (var wrk in getwrk.OrderBy(o => o.wrkname).ToList())
                            {
                                var getamount = amountdata(CustMod_DetailWork2, "wrk", wrk.wrk);
                                result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + wrk.wrk + " - " + wrk.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, nopkj = wrk.wrk, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                            }
                            break;
                    }
                    break;
                case 2:
                    var gettype2 = coloumgroup.Select(s => s.type).ToArray();
                    int[] type1 = new int[] { 1, 2 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)).Select(s => new { s.cc, s.pkt }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = data.cc, levelcode = data.pkt, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 3 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.cc, s.actvt, s.actvtdesc }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = data.cc, actvtcode = data.actvt, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 4 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = data.cc, actvtcode = data.gl, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = data.cc, grp = data.grp, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = data.cc, nopkj = data.wrk, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 2, 3 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.pkt, s.actvt, s.actvtdesc }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, levelcode = data.pkt, actvtcode = data.actvt, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 2, 4 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.pkt, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "gl", data.gl);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, levelcode = data.pkt, gl = data.gl, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 2, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, levelcode = data.pkt, grp = data.grp, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 2, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, levelcode = data.pkt, nopkj = data.wrk, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 3, 4 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.actvt, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, actvtcode = data.actvt, gl = data.gl, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 3, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.actvt, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "actvt", data.actvt, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, actvtcode = data.actvt, grp = data.grp, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 3, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "actvt", data.actvt, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, actvtcode = data.actvt, nopkj = data.wrk, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 4, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.gl).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "gl", data.gl, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, grp = data.grp, gl = data.gl, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 4, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.gl).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "gl", data.gl, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, gl = data.gl, nopkj = data.wrk, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 5, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.grp).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "grp", data.grp, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, grp = data.grp, nopkj = data.wrk, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    break;
                case 3:
                    var gettype3 = coloumgroup.Select(s => s.type).ToArray();
                    type1 = new int[] { 1, 2, 3 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, levelcode = data.pkt, costcenter = data.cc, actvtcode = data.actvt, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 2, 4 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = aktvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.pkt, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "gl", data.gl);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, levelcode = data.pkt, costcenter = data.cc, gl = data.gl, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 2, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = aktvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.pkt, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, levelcode = data.pkt, costcenter = data.cc, grp = data.grp, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 2, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = aktvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, levelcode = data.pkt, costcenter = data.cc, nopkj = data.wrk, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 3, 4 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = data.cc, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 3, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = data.cc, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 3, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = data.cc, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 4, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = data.cc, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 4, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = data.cc, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 1, 5, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "grp", data.grp, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, costcenter = data.cc, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 2, 3, 4 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 2, 3, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 2, 3, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 2, 4, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "gl", data.gl, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 2, 4, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "gl", data.gl, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 3, 4, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 3, 4, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 3, 5, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.actvtdesc, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "actvt", data.actvt, "grp", data.grp, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    type1 = new int[] { 4, 5, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.gl).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "gl", data.gl, "grp", data.grp, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    break;
                case 4:
                    var gettype4 = coloumgroup.Select(s => s.type).ToArray();
                    type1 = new int[] { 1, 2, 3, 4 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 1, 2, 3, 5 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 1, 2, 3, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 1, 3, 4, 5 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 1, 3, 4, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 1, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl, "grp", data.grp, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 2, 3, 4, 5 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 2, 3, 4, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 2, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.gl, "gl", data.grp, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 3, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    break;
                case 5:
                    var gettype5 = coloumgroup.Select(s => s.type).ToArray();
                    type1 = new int[] { 1, 2, 3, 4, 5 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 1, 2, 3, 4, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 1, 2, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 1, 3, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }

                    type1 = new int[] { 2, 3, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk);
                            result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                        }
                    }
                    break;
                case 6:

                    //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                    var getdata2 = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                    && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                    foreach (var data in getdata2.OrderBy(o => o.cc).ToList())
                    {
                        var getamount = amountdata(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk);
                        result.Add(new CustMod_DetailWork { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", netincome = getamount.netincome, totalincome = getamount.total, piecerateqnty = getamount.getquantityp, ottime = getamount.getquantityot, kongpay = getamount.getamountk, otpay = getamount.getamountot, otherincome = getamount.getamountothr, pieceratepay = getamount.getamountp, incentive = getamount.getamountinc, totaldeduction = getamount.getamountdec });
                    }
                    break;
            }

            return result;
        }

        public List<CustMod_DetailWork3> DetailReport2(List<CustMod_DetailWork2> CustMod_DetailWork2, List<coloumgroup> coloumgroup, string Selection1)
        {
            var result = new List<CustMod_DetailWork3>();
            var id = 1;
            switch (coloumgroup.Count())
            {
                case 1:
                    var gettype = coloumgroup.Select(s => s.type).FirstOrDefault();
                    switch (gettype)
                    {
                        case 1:
                            //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                            var getcc = CustMod_DetailWork2.Select(s => s.cc).Distinct().ToList();
                            foreach (var cc in getcc.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata2(CustMod_DetailWork2, "cc", cc, Selection1);
                                result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + cc + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                            }
                            break;
                        case 2:
                            var getpkt = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt)).Select(s => s.pkt).Distinct().ToList();
                            foreach (var pkt in getpkt.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata2(CustMod_DetailWork2, "pkt", pkt, Selection1);
                                result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + pkt + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                            }
                            break;
                        case 3:
                            var getactvt = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.actvt, s.actvtdesc }).Distinct().ToList();
                            foreach (var actvt in getactvt.OrderBy(o => o.actvt).ToList())
                            {
                                var getamount = amountdata2(CustMod_DetailWork2, "actvt", actvt.actvt, Selection1);
                                result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + actvt.actvt + " - " + actvt.actvtdesc + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                            }
                            break;
                        case 4:
                            var getgl = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl)).Select(s => s.gl).Distinct().ToList();
                            foreach (var gl in getgl.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata2(CustMod_DetailWork2, "gl", gl, Selection1);
                                result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + gl + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                            }
                            break;
                        case 5:
                            var getgrp = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.grp)).Select(s => s.grp).Distinct().ToList();
                            foreach (var grp in getgrp.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata2(CustMod_DetailWork2, "grp", grp, Selection1);
                                result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                            }
                            break;
                        case 6:
                            var getwrk = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.wrk, s.wrkname }).Distinct().ToList();
                            foreach (var wrk in getwrk.OrderBy(o => o.wrkname).ToList())
                            {
                                var getamount = amountdata2(CustMod_DetailWork2, "wrk", wrk.wrk, Selection1);
                                result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + wrk.wrk + " - " + wrk.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                            }
                            break;
                    }
                    break;
                case 2:
                    var gettype2 = coloumgroup.Select(s => s.type).ToArray();
                    int[] type1 = new int[] { 1, 2 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)).Select(s => new { s.cc, s.pkt }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 1, 3 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.cc, s.actvt, s.actvtdesc }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 1, 4 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 1, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 1, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 2, 3 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.pkt, s.actvt, s.actvtdesc }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 2, 4 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.pkt, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 2, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 2, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 3, 4 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.actvt, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 3, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.actvt, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "actvt", data.actvt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 3, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "actvt", data.actvt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 4, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.gl).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 4, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.gl).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 5, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.grp).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    break;
                case 3:
                    var gettype3 = coloumgroup.Select(s => s.type).ToArray();
                    type1 = new int[] { 1, 2, 3 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 2, 4 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = aktvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.pkt, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 2, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = aktvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.pkt, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 2, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = aktvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 3, 4 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 3, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 3, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 4, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 4, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 5, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 2, 3, 4 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 2, 3, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 2, 3, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 2, 4, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 2, 4, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 3, 4, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 3, 4, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 3, 5, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.actvtdesc, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "actvt", data.actvt, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 4, 5, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.gl).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    break;
                case 4:
                    var gettype4 = coloumgroup.Select(s => s.type).ToArray();
                    type1 = new int[] { 1, 2, 3, 4 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 2, 3, 5 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 2, 3, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 3, 4, 5 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 3, 4, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 2, 3, 4, 5 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 2, 3, 4, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 2, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.gl, "gl", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 3, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    break;
                case 5:
                    var gettype5 = coloumgroup.Select(s => s.type).ToArray();
                    type1 = new int[] { 1, 2, 3, 4, 5 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 2, 3, 4, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 2, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 3, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 2, 3, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata2(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                        }
                    }
                    break;
                case 6:

                    //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                    var getdata2 = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                    && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                    foreach (var data in getdata2.OrderBy(o => o.cc).ToList())
                    {
                        var getamount = amountdata2(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                        result.Add(new CustMod_DetailWork3 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", getjan = getamount.getjan, getfeb = getamount.getfeb, getmac = getamount.getmac, getapr = getamount.getapr, getmay = getamount.getmay, getjun = getamount.getjun, getjul = getamount.getjul, getaug = getamount.getaug, getsep = getamount.getsep, getoct = getamount.getoct, getnov = getamount.getnov, getdec = getamount.getdec, gettotal = getamount.gettotal });
                    }
                    break;
            }
            return result;
        }

        public List<CustMod_DetailWork4> DetailReport3(List<CustMod_DetailWork2> CustMod_DetailWork2, List<coloumgroup> coloumgroup, string Selection1)
        {
            var result = new List<CustMod_DetailWork4>();
            var id = 1;
            switch (coloumgroup.Count())
            {
                case 1:
                    var gettype = coloumgroup.Select(s => s.type).FirstOrDefault();
                    switch (gettype)
                    {
                        case 1:
                            //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                            var getcc = CustMod_DetailWork2.Select(s => s.cc).Distinct().ToList();
                            foreach (var cc in getcc.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata3(CustMod_DetailWork2, "cc", cc, Selection1);
                                result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + cc + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                            }
                            break;
                        case 2:
                            var getpkt = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt)).Select(s => s.pkt).Distinct().ToList();
                            foreach (var pkt in getpkt.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata3(CustMod_DetailWork2, "pkt", pkt, Selection1);
                                result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + pkt + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                            }
                            break;
                        case 3:
                            var getactvt = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.actvt, s.actvtdesc }).Distinct().ToList();
                            foreach (var actvt in getactvt.OrderBy(o => o.actvt).ToList())
                            {
                                var getamount = amountdata3(CustMod_DetailWork2, "actvt", actvt.actvt, Selection1);
                                result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + actvt.actvt + " - " + actvt.actvtdesc + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                            }
                            break;
                        case 4:
                            var getgl = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl)).Select(s => s.gl).Distinct().ToList();
                            foreach (var gl in getgl.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata3(CustMod_DetailWork2, "gl", gl, Selection1);
                                result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + gl + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                            }
                            break;
                        case 5:
                            var getgrp = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.grp)).Select(s => s.grp).Distinct().ToList();
                            foreach (var grp in getgrp.OrderBy(o => o).ToList())
                            {
                                var getamount = amountdata3(CustMod_DetailWork2, "grp", grp, Selection1);
                                result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                            }
                            break;
                        case 6:
                            var getwrk = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.wrk, s.wrkname }).Distinct().ToList();
                            foreach (var wrk in getwrk.OrderBy(o => o.wrkname).ToList())
                            {
                                var getamount = amountdata3(CustMod_DetailWork2, "wrk", wrk.wrk, Selection1);
                                result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + wrk.wrk + " - " + wrk.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                            }
                            break;
                    }
                    break;
                case 2:
                    var gettype2 = coloumgroup.Select(s => s.type).ToArray();
                    int[] type1 = new int[] { 1, 2 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)).Select(s => new { s.cc, s.pkt }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 1, 3 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.cc, s.actvt, s.actvtdesc }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 1, 4 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 1, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 1, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 2, 3 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.pkt, s.actvt, s.actvtdesc }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 2, 4 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.pkt, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 2, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 2, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 3, 4 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.actvt, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 3, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.actvt, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "actvt", data.actvt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 3, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "actvt", data.actvt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 4, 5 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.gl).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 4, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.gl).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal }); ;
                        }
                    }
                    type1 = new int[] { 5, 6 };
                    if (type1.SequenceEqual(gettype2))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.grp).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    break;
                case 3:
                    var gettype3 = coloumgroup.Select(s => s.type).ToArray();
                    type1 = new int[] { 1, 2, 3 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 2, 4 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = aktvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.pkt, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 2, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = aktvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.pkt, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 2, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = aktvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 3, 4 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 3, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 3, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 4, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 4, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 1, 5, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 2, 3, 4 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 2, 3, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 2, 3, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 2, 4, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 2, 4, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 3, 4, 5 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 3, 4, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 3, 5, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.actvtdesc, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "actvt", data.actvt, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    type1 = new int[] { 4, 5, 6 };
                    if (type1.SequenceEqual(gettype3))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.gl).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    break;
                case 4:
                    var gettype4 = coloumgroup.Select(s => s.type).ToArray();
                    type1 = new int[] { 1, 2, 3, 4 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 2, 3, 5 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 2, 3, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt)
                        && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 3, 4, 5 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 3, 4, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 2, 3, 4, 5 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 2, 3, 4, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 2, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)
                        && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.gl, "gl", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 3, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype4))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.actvt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    break;
                case 5:
                    var gettype5 = coloumgroup.Select(s => s.type).ToArray();
                    type1 = new int[] { 1, 2, 3, 4, 5 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 2, 3, 4, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 2, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 1, 3, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.actvt)
                        && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.cc).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }

                    type1 = new int[] { 2, 3, 4, 5, 6 };
                    if (type1.SequenceEqual(gettype5))
                    {
                        //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                        var getdata = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt) && !string.IsNullOrEmpty(x.gl)
                        && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                        foreach (var data in getdata.OrderBy(o => o.pkt).ToList())
                        {
                            var getamount = amountdata3(CustMod_DetailWork2, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                            result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                        }
                    }
                    break;
                case 6:

                    //1 = cc, 2 = pkt, 3 = actvt, 4 = gl, 5 = grp, 6 = wrk 
                    var getdata2 = CustMod_DetailWork2.Where(x => !string.IsNullOrEmpty(x.cc) && !string.IsNullOrEmpty(x.pkt) && !string.IsNullOrEmpty(x.actvt)
                    && !string.IsNullOrEmpty(x.gl) && !string.IsNullOrEmpty(x.grp) && !string.IsNullOrEmpty(x.wrk)).Select(s => new { s.cc, s.pkt, s.actvt, s.actvtdesc, s.gl, s.grp, s.wrk, s.wrkname }).Distinct().ToList();
                    foreach (var data in getdata2.OrderBy(o => o.cc).ToList())
                    {
                        var getamount = amountdata3(CustMod_DetailWork2, "cc", data.cc, "pkt", data.pkt, "actvt", data.actvt, "gl", data.gl, "grp", data.grp, "wrk", data.wrk, Selection1);
                        result.Add(new CustMod_DetailWork4 { id = id, columnheader = "<td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.cc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.pkt + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.actvt + " - " + data.actvtdesc + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.gl + "</td><td align=\"center\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.grp + "</td><td align=\"left\" style=\"vertical-align:middle!important; border: 1px solid black;\" border=\"1\">" + data.wrk + " - " + data.wrkname + "</td>", day1 = getamount.day1, day2 = getamount.day2, day3 = getamount.day3, day4 = getamount.day4, day5 = getamount.day5, day6 = getamount.day6, day7 = getamount.day7, day8 = getamount.day8, day9 = getamount.day9, day10 = getamount.day10, day11 = getamount.day11, day12 = getamount.day12, day13 = getamount.day13, day14 = getamount.day14, day15 = getamount.day15, day16 = getamount.day16, day17 = getamount.day17, day18 = getamount.day18, day19 = getamount.day19, day20 = getamount.day20, day21 = getamount.day21, day22 = getamount.day22, day23 = getamount.day23, day24 = getamount.day24, day25 = getamount.day25, day26 = getamount.day26, day27 = getamount.day27, day28 = getamount.day28, day29 = getamount.day29, day30 = getamount.day30, day31 = getamount.day31, gettotal = getamount.gettotal });
                    }
                    break;
            }
            return result;
        }

        public amountdata amountdata(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata(data1);
        }

        public amountdata amountdata(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata(data1);
        }
        public amountdata amountdata(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata(data1);
        }
        public amountdata amountdata(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3, string column4, string value4)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3, column4, value4);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata(data1);
        }
        public amountdata amountdata(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3, string column4, string value4, string column5, string value5)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3, column4, value4, column5, value5);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata(data1);
        }
        public amountdata amountdata(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3, string column4, string value4, string column5, string value5, string column6, string value6)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3, column4, value4, column5, value5, column6, value6);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata(data1);
        }
        public amountdata2 amountdata2(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata2(data1, Selection1);
        }

        public amountdata2 amountdata2(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata2(data1, Selection1);
        }
        public amountdata2 amountdata2(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata2(data1, Selection1);
        }
        public amountdata2 amountdata2(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3, string column4, string value4, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3, column4, value4);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata2(data1, Selection1);
        }
        public amountdata2 amountdata2(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3, string column4, string value4, string column5, string value5, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3, column4, value4, column5, value5);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata2(data1, Selection1);
        }
        public amountdata2 amountdata2(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3, string column4, string value4, string column5, string value5, string column6, string value6, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3, column4, value4, column5, value5, column6, value6);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata2(data1, Selection1);
        }
        public amountdata3 amountdata3(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata3(data1, Selection1);
        }

        public amountdata3 amountdata3(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata3(data1, Selection1);
        }
        public amountdata3 amountdata3(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata3(data1, Selection1);
        }
        public amountdata3 amountdata3(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3, string column4, string value4, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3, column4, value4);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata3(data1, Selection1);
        }
        public amountdata3 amountdata3(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3, string column4, string value4, string column5, string value5, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3, column4, value4, column5, value5);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata3(data1, Selection1);
        }
        public amountdata3 amountdata3(List<CustMod_DetailWork2> CustMod_DetailWork2, string column, string value, string column2, string value2, string column3, string value3, string column4, string value4, string column5, string value5, string column6, string value6, string Selection1)
        {
            var filter = GetDynamicQueryWithExpresionTrees(column, value, column2, value2, column3, value3, column4, value4, column5, value5, column6, value6);
            var data1 = CustMod_DetailWork2.Where(filter).ToList();
            return amountdata3(data1, Selection1);
        }
        public amountdata amountdata(List<CustMod_DetailWork2> data1)
        {
            var getamountk = data1.Where(x => x.amounttype == 1).Sum(s => s.amount);
            var getquantityot = data1.Where(x => x.amounttype == 2).Sum(s => s.quantity);
            var getamountot = data1.Where(x => x.amounttype == 2).Sum(s => s.amount);
            var getamountothr = data1.Where(x => x.amounttype == 3).Sum(s => s.amount);
            var getquantityp = data1.Where(x => x.amounttype == 4).Sum(s => s.quantity);
            var getamountp = data1.Where(x => x.amounttype == 4).Sum(s => s.amount);
            var getamountinc = data1.Where(x => x.amounttype == 5).Sum(s => s.amount);
            var getamountdec = data1.Where(x => x.amounttype == 6).Sum(s => s.amount);
            var getemplyrcon = data1.Where(x => x.amounttype == 7).Sum(s => s.amount); //fitri add 27.09.2021
            var result = new amountdata
            {
                getamountk = getamountk,
                getquantityot = getquantityot,
                getamountot = getamountot,
                getamountothr = getamountothr,
                getquantityp = getquantityp,
                getamountp = getamountp,
                getamountinc = getamountinc,
                getamountdec = getamountdec,
                getemplyrcntribtn = getemplyrcon, //fitri add 27.09.2021
                //total = getamountk + getamountot + getamountothr + getamountp + getamountinc, //fitri comment 27.09.2021
                //netincome = getamountk + getamountot + getamountothr + getamountp + getamountinc + getamountdec, //fitri comment 27.09.2021
                total = getamountk + getamountot + getamountothr + getamountp + getamountinc + getemplyrcon, //fitri add 27.09.2021
                netincome = getamountk + getamountot + getamountothr + getamountp + getamountinc + getamountdec + getemplyrcon, //fitri edit 27.09.2021
            };
            return result;
        }
        public amountdata2 amountdata2(List<CustMod_DetailWork2> data1, string Selection1)
        {
            var getjan = Selection1 == "0" ? data1.Where(x => x.month == 1).Sum(s => s.amount) : data1.Where(x => x.month == 1).Sum(s => s.quantity);
            var getfeb = Selection1 == "0" ? data1.Where(x => x.month == 2).Sum(s => s.amount) : data1.Where(x => x.month == 2).Sum(s => s.quantity);
            var getmac = Selection1 == "0" ? data1.Where(x => x.month == 3).Sum(s => s.amount) : data1.Where(x => x.month == 3).Sum(s => s.quantity);
            var getapr = Selection1 == "0" ? data1.Where(x => x.month == 4).Sum(s => s.amount) : data1.Where(x => x.month == 4).Sum(s => s.quantity);
            var getmay = Selection1 == "0" ? data1.Where(x => x.month == 5).Sum(s => s.amount) : data1.Where(x => x.month == 5).Sum(s => s.quantity);
            var getjun = Selection1 == "0" ? data1.Where(x => x.month == 6).Sum(s => s.amount) : data1.Where(x => x.month == 6).Sum(s => s.quantity);
            var getjul = Selection1 == "0" ? data1.Where(x => x.month == 7).Sum(s => s.amount) : data1.Where(x => x.month == 7).Sum(s => s.quantity);
            var getaug = Selection1 == "0" ? data1.Where(x => x.month == 8).Sum(s => s.amount) : data1.Where(x => x.month == 8).Sum(s => s.quantity);
            var getsep = Selection1 == "0" ? data1.Where(x => x.month == 9).Sum(s => s.amount) : data1.Where(x => x.month == 9).Sum(s => s.quantity);
            var getoct = Selection1 == "0" ? data1.Where(x => x.month == 10).Sum(s => s.amount) : data1.Where(x => x.month == 10).Sum(s => s.quantity);
            var getnov = Selection1 == "0" ? data1.Where(x => x.month == 11).Sum(s => s.amount) : data1.Where(x => x.month == 11).Sum(s => s.quantity);
            var getdec = Selection1 == "0" ? data1.Where(x => x.month == 12).Sum(s => s.amount) : data1.Where(x => x.month == 12).Sum(s => s.quantity);
            var gettotal = getjan + getfeb + getmac + getapr + getmay + getjun + getjul + getaug + getsep + getoct + getnov + getdec;
            var result = new amountdata2
            {
                getjan = getjan,
                getfeb = getfeb,
                getmac = getmac,
                getapr = getapr,
                getmay = getmay,
                getjun = getjun,
                getjul = getjul,
                getaug = getaug,
                getsep = getsep,
                getoct = getoct,
                getnov = getnov,
                getdec = getdec,
                gettotal = gettotal
            };
            return result;
        }
        public amountdata3 amountdata3(List<CustMod_DetailWork2> data1, string Selection1)
        {
            var day1 = Selection1 == "0" ? data1.Where(x => x.day == 1).Sum(s => s.amount) : data1.Where(x => x.day == 1).Sum(s => s.quantity);
            var day2 = Selection1 == "0" ? data1.Where(x => x.day == 2).Sum(s => s.amount) : data1.Where(x => x.day == 2).Sum(s => s.quantity);
            var day3 = Selection1 == "0" ? data1.Where(x => x.day == 3).Sum(s => s.amount) : data1.Where(x => x.day == 3).Sum(s => s.quantity);
            var day4 = Selection1 == "0" ? data1.Where(x => x.day == 4).Sum(s => s.amount) : data1.Where(x => x.day == 4).Sum(s => s.quantity);
            var day5 = Selection1 == "0" ? data1.Where(x => x.day == 5).Sum(s => s.amount) : data1.Where(x => x.day == 5).Sum(s => s.quantity);
            var day6 = Selection1 == "0" ? data1.Where(x => x.day == 6).Sum(s => s.amount) : data1.Where(x => x.day == 6).Sum(s => s.quantity);
            var day7 = Selection1 == "0" ? data1.Where(x => x.day == 7).Sum(s => s.amount) : data1.Where(x => x.day == 7).Sum(s => s.quantity);
            var day8 = Selection1 == "0" ? data1.Where(x => x.day == 8).Sum(s => s.amount) : data1.Where(x => x.day == 8).Sum(s => s.quantity);
            var day9 = Selection1 == "0" ? data1.Where(x => x.day == 9).Sum(s => s.amount) : data1.Where(x => x.day == 9).Sum(s => s.quantity);
            var day10 = Selection1 == "0" ? data1.Where(x => x.day == 10).Sum(s => s.amount) : data1.Where(x => x.day == 10).Sum(s => s.quantity);
            var day11 = Selection1 == "0" ? data1.Where(x => x.day == 11).Sum(s => s.amount) : data1.Where(x => x.day == 11).Sum(s => s.quantity);
            var day12 = Selection1 == "0" ? data1.Where(x => x.day == 12).Sum(s => s.amount) : data1.Where(x => x.day == 12).Sum(s => s.quantity);
            var day13 = Selection1 == "0" ? data1.Where(x => x.day == 13).Sum(s => s.amount) : data1.Where(x => x.day == 13).Sum(s => s.quantity);
            var day14 = Selection1 == "0" ? data1.Where(x => x.day == 14).Sum(s => s.amount) : data1.Where(x => x.day == 14).Sum(s => s.quantity);
            var day15 = Selection1 == "0" ? data1.Where(x => x.day == 15).Sum(s => s.amount) : data1.Where(x => x.day == 15).Sum(s => s.quantity);
            var day16 = Selection1 == "0" ? data1.Where(x => x.day == 16).Sum(s => s.amount) : data1.Where(x => x.day == 16).Sum(s => s.quantity);
            var day17 = Selection1 == "0" ? data1.Where(x => x.day == 17).Sum(s => s.amount) : data1.Where(x => x.day == 17).Sum(s => s.quantity);
            var day18 = Selection1 == "0" ? data1.Where(x => x.day == 18).Sum(s => s.amount) : data1.Where(x => x.day == 18).Sum(s => s.quantity);
            var day19 = Selection1 == "0" ? data1.Where(x => x.day == 19).Sum(s => s.amount) : data1.Where(x => x.day == 19).Sum(s => s.quantity);
            var day20 = Selection1 == "0" ? data1.Where(x => x.day == 20).Sum(s => s.amount) : data1.Where(x => x.day == 20).Sum(s => s.quantity);
            var day21 = Selection1 == "0" ? data1.Where(x => x.day == 21).Sum(s => s.amount) : data1.Where(x => x.day == 21).Sum(s => s.quantity);
            var day22 = Selection1 == "0" ? data1.Where(x => x.day == 22).Sum(s => s.amount) : data1.Where(x => x.day == 22).Sum(s => s.quantity);
            var day23 = Selection1 == "0" ? data1.Where(x => x.day == 23).Sum(s => s.amount) : data1.Where(x => x.day == 23).Sum(s => s.quantity);
            var day24 = Selection1 == "0" ? data1.Where(x => x.day == 24).Sum(s => s.amount) : data1.Where(x => x.day == 24).Sum(s => s.quantity);
            var day25 = Selection1 == "0" ? data1.Where(x => x.day == 25).Sum(s => s.amount) : data1.Where(x => x.day == 25).Sum(s => s.quantity);
            var day26 = Selection1 == "0" ? data1.Where(x => x.day == 26).Sum(s => s.amount) : data1.Where(x => x.day == 26).Sum(s => s.quantity);
            var day27 = Selection1 == "0" ? data1.Where(x => x.day == 27).Sum(s => s.amount) : data1.Where(x => x.day == 27).Sum(s => s.quantity);
            var day28 = Selection1 == "0" ? data1.Where(x => x.day == 28).Sum(s => s.amount) : data1.Where(x => x.day == 28).Sum(s => s.quantity);
            var day29 = Selection1 == "0" ? data1.Where(x => x.day == 29).Sum(s => s.amount) : data1.Where(x => x.day == 29).Sum(s => s.quantity);
            var day30 = Selection1 == "0" ? data1.Where(x => x.day == 30).Sum(s => s.amount) : data1.Where(x => x.day == 30).Sum(s => s.quantity);
            var day31 = Selection1 == "0" ? data1.Where(x => x.day == 31).Sum(s => s.amount) : data1.Where(x => x.day == 31).Sum(s => s.quantity);
            var gettotal = day1 + day2 + day3 + day4 + day5 + day6 + day7 + day8 + day9 + day10 + day11 + day12 + day13 + day14 + day15 + day16 + day17 + day18 + day19 + day20 + day21 + day22 + day23 + day24 + day25 + day26 + day27 + day28 + day29 + day30 + day31;
            var result = new amountdata3
            {
                day1 = day1,
                day2 = day2,
                day3 = day3,
                day4 = day4,
                day5 = day5,
                day6 = day6,
                day7 = day7,
                day8 = day8,
                day9 = day9,
                day10 = day10,
                day11 = day11,
                day12 = day12,
                day13 = day13,
                day14 = day14,
                day15 = day15,
                day16 = day16,
                day17 = day17,
                day18 = day18,
                day19 = day19,
                day20 = day20,
                day21 = day21,
                day22 = day22,
                day23 = day23,
                day24 = day24,
                day25 = day25,
                day26 = day26,
                day27 = day27,
                day28 = day28,
                day29 = day29,
                day30 = day30,
                day31 = day31,
                gettotal = gettotal
            };
            return result;
        }

        private static Func<CustMod_DetailWork2, bool> GetDynamicQueryWithExpresionTrees(string propertyName, string val)
        {
            var parameter = Expression.Parameter(typeof(CustMod_DetailWork2), "x");
            var member = Expression.Property(parameter, propertyName);
            var constant = Expression.Constant(val);
            var body = Expression.Equal(member, constant);
            var finalExpression = Expression.Lambda<Func<CustMod_DetailWork2, bool>>(body, parameter);
            return finalExpression.Compile();
        }
        private static Func<CustMod_DetailWork2, bool> GetDynamicQueryWithExpresionTrees(string propertyName, string val, string propertyName2, string val2)
        {
            var parameter = Expression.Parameter(typeof(CustMod_DetailWork2), "x");
            var member = Expression.Property(parameter, propertyName);
            var member2 = Expression.Property(parameter, propertyName2);
            var constant = Expression.Constant(val);
            var constant2 = Expression.Constant(val2);
            var body = Expression.Equal(member, constant);
            var body2 = Expression.Equal(member2, constant2);
            var combinebody = Expression.And(body, body2);
            var finalExpression = Expression.Lambda<Func<CustMod_DetailWork2, bool>>(combinebody, parameter);
            return finalExpression.Compile();
        }
        private static Func<CustMod_DetailWork2, bool> GetDynamicQueryWithExpresionTrees(string propertyName, string val, string propertyName2, string val2, string propertyName3, string val3)
        {
            var parameter = Expression.Parameter(typeof(CustMod_DetailWork2), "x");
            var member = Expression.Property(parameter, propertyName);
            var member2 = Expression.Property(parameter, propertyName2);
            var member3 = Expression.Property(parameter, propertyName3);
            var constant = Expression.Constant(val);
            var constant2 = Expression.Constant(val2);
            var constant3 = Expression.Constant(val3);
            var body = Expression.Equal(member, constant);
            var body2 = Expression.Equal(member2, constant2);
            var body3 = Expression.Equal(member3, constant3);
            var combinebody = Expression.And(body, body2);
            combinebody = Expression.And(body3, combinebody);
            var finalExpression = Expression.Lambda<Func<CustMod_DetailWork2, bool>>(combinebody, parameter);
            return finalExpression.Compile();
        }
        private static Func<CustMod_DetailWork2, bool> GetDynamicQueryWithExpresionTrees(string propertyName, string val, string propertyName2, string val2, string propertyName3, string val3, string propertyName4, string val4)
        {
            var parameter = Expression.Parameter(typeof(CustMod_DetailWork2), "x");
            var member = Expression.Property(parameter, propertyName);
            var member2 = Expression.Property(parameter, propertyName2);
            var member3 = Expression.Property(parameter, propertyName3);
            var member4 = Expression.Property(parameter, propertyName4);
            var constant = Expression.Constant(val);
            var constant2 = Expression.Constant(val2);
            var constant3 = Expression.Constant(val3);
            var constant4 = Expression.Constant(val4);
            var body = Expression.Equal(member, constant);
            var body2 = Expression.Equal(member2, constant2);
            var body3 = Expression.Equal(member3, constant3);
            var body4 = Expression.Equal(member4, constant4);
            var combinebody = Expression.And(body, body2);
            combinebody = Expression.And(body3, combinebody);
            combinebody = Expression.And(body4, combinebody);
            var finalExpression = Expression.Lambda<Func<CustMod_DetailWork2, bool>>(combinebody, parameter);
            return finalExpression.Compile();
        }
        private static Func<CustMod_DetailWork2, bool> GetDynamicQueryWithExpresionTrees(string propertyName, string val, string propertyName2, string val2, string propertyName3, string val3, string propertyName4, string val4, string propertyName5, string val5)
        {
            var parameter = Expression.Parameter(typeof(CustMod_DetailWork2), "x");
            var member = Expression.Property(parameter, propertyName);
            var member2 = Expression.Property(parameter, propertyName2);
            var member3 = Expression.Property(parameter, propertyName3);
            var member4 = Expression.Property(parameter, propertyName4);
            var member5 = Expression.Property(parameter, propertyName5);
            var constant = Expression.Constant(val);
            var constant2 = Expression.Constant(val2);
            var constant3 = Expression.Constant(val3);
            var constant4 = Expression.Constant(val4);
            var constant5 = Expression.Constant(val5);
            var body = Expression.Equal(member, constant);
            var body2 = Expression.Equal(member2, constant2);
            var body3 = Expression.Equal(member3, constant3);
            var body4 = Expression.Equal(member4, constant4);
            var body5 = Expression.Equal(member5, constant5);
            var combinebody = Expression.And(body, body2);
            combinebody = Expression.And(body3, combinebody);
            combinebody = Expression.And(body4, combinebody);
            combinebody = Expression.And(body5, combinebody);
            var finalExpression = Expression.Lambda<Func<CustMod_DetailWork2, bool>>(combinebody, parameter);
            return finalExpression.Compile();
        }
        private static Func<CustMod_DetailWork2, bool> GetDynamicQueryWithExpresionTrees(string propertyName, string val, string propertyName2, string val2, string propertyName3, string val3, string propertyName4, string val4, string propertyName5, string val5, string propertyName6, string val6)
        {
            var parameter = Expression.Parameter(typeof(CustMod_DetailWork2), "x");
            var member = Expression.Property(parameter, propertyName);
            var member2 = Expression.Property(parameter, propertyName2);
            var member3 = Expression.Property(parameter, propertyName3);
            var member4 = Expression.Property(parameter, propertyName4);
            var member5 = Expression.Property(parameter, propertyName5);
            var member6 = Expression.Property(parameter, propertyName6);
            var constant = Expression.Constant(val);
            var constant2 = Expression.Constant(val2);
            var constant3 = Expression.Constant(val3);
            var constant4 = Expression.Constant(val4);
            var constant5 = Expression.Constant(val5);
            var constant6 = Expression.Constant(val6);
            var body = Expression.Equal(member, constant);
            var body2 = Expression.Equal(member2, constant2);
            var body3 = Expression.Equal(member3, constant3);
            var body4 = Expression.Equal(member4, constant4);
            var body5 = Expression.Equal(member5, constant5);
            var body6 = Expression.Equal(member6, constant6);
            var combinebody = Expression.And(body, body2);
            combinebody = Expression.And(body3, combinebody);
            combinebody = Expression.And(body4, combinebody);
            combinebody = Expression.And(body5, combinebody);
            combinebody = Expression.And(body6, combinebody);
            var finalExpression = Expression.Lambda<Func<CustMod_DetailWork2, bool>>(combinebody, parameter);
            return finalExpression.Compile();
        }
    }
}