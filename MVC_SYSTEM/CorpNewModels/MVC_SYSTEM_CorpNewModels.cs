namespace MVC_SYSTEM.CorpNewModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_SYSTEM_CorpNewModels : DbContext
    {
        public MVC_SYSTEM_CorpNewModels()
            : base("name=MVC_SYSTEM_HQ_CONN")
        {
        }

        public virtual DbSet<tbl_SalaryRequest> tbl_SalaryRequest { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
