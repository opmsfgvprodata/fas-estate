namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_EAWorkerSiriNo
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_WorkerNo { get; set; }

        [StringLength(20)]
        public string fld_SiriNo { get; set; }
    }
}
