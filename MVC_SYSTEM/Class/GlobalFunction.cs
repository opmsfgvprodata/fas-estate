using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.ModelSAPPUP;

namespace MVC_SYSTEM.Class
{
    public class GlobalFunction
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

        public static class PropertyCopy
        {
            public static void Copy<TDest, TSource>(TDest destination, TSource source)
                where TSource : class
                where TDest : class
            {
                var destProperties = destination.GetType()
                    .GetProperties()
                    .Where(x => x.CanRead && x.CanWrite && !x.GetGetMethod().IsVirtual);
                var sourceProperties = source.GetType()
                    .GetProperties()
                    .Where(x => x.CanRead && x.CanWrite && !x.GetGetMethod().IsVirtual);
                var copyProperties = sourceProperties.Join(destProperties, x => x.Name, y => y.Name, (x, y) => x);
                foreach (var sourceProperty in copyProperties)
                {
                    var prop = destProperties.FirstOrDefault(x => x.Name == sourceProperty.Name);
                    prop.SetValue(destination, sourceProperty.GetValue(source));
                }
            }
        }

        public string BatchNoSAPPostFunc(int? LadangID, string BatchWord, string BatchFlag, string BatchFlag2, int Month, int Year)
        {
            GetNSWL GetNSWL = new GetNSWL();
            var GetNSWLDetail = GetNSWL.GetLadangDetail(LadangID.Value);
            var getbatchno = db.tbl_BatchRunNo.Where(x => x.fld_BatchFlag == BatchFlag && x.fld_NegaraID == GetNSWLDetail.fld_NegaraID && x.fld_SyarikatID == GetNSWLDetail.fld_SyarikatID && x.fld_BatchRunNo2 == Month && x.fld_BatchRunNo3 == Year && x.fld_BatchFlag2 == BatchFlag2).FirstOrDefault();
            int? convertint = 0;
            string genbatchno = "";

            if (getbatchno == null)
            {
                tbl_BatchRunNo tbl_BatchRunNo = new tbl_BatchRunNo();
                tbl_BatchRunNo.fld_BatchRunNo = 1;
                tbl_BatchRunNo.fld_BatchFlag = BatchFlag;
                tbl_BatchRunNo.fld_BatchFlag2 = BatchFlag2;
                tbl_BatchRunNo.fld_NegaraID = GetNSWLDetail.fld_NegaraID;
                tbl_BatchRunNo.fld_SyarikatID = GetNSWLDetail.fld_SyarikatID;
                tbl_BatchRunNo.fld_WilayahID = 0;
                tbl_BatchRunNo.fld_LadangID = 0;
                tbl_BatchRunNo.fld_BatchRunNo2 = Month;
                tbl_BatchRunNo.fld_BatchRunNo3 = Year;
                db.tbl_BatchRunNo.Add(tbl_BatchRunNo);
                db.SaveChanges();
                convertint = 1;
                genbatchno = BatchWord + convertint.Value.ToString("000");
            }
            else
            {
                convertint = getbatchno.fld_BatchRunNo;
                convertint = convertint + 1;
                getbatchno.fld_BatchRunNo = convertint;
                db.Entry(getbatchno).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                genbatchno = BatchWord + convertint.Value.ToString("000");
            }

            return genbatchno;
        }

        public List<tbl_SAPPDPUP> GetSAPPDData (IEnumerable<vw_SAPPostDataFullDetails> vw_SAPPostDataFullDetails)
        {
            List<tbl_SAPPDPUP> ReturnData = new List<tbl_SAPPDPUP>();

            var GetNN = vw_SAPPostDataFullDetails.Select(s => s.fld_Item).ToArray();

            ReturnData = db.tbl_SAPPDPUP.Where(x => GetNN.Contains(x.fld_NetworkNo)).ToList();

            return ReturnData;
        }
    }
}