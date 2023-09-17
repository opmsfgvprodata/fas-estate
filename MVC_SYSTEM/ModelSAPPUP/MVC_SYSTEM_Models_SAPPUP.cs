namespace MVC_SYSTEM.ModelSAPPUP
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using MVC_SYSTEM.Models;

    public partial class MVC_SYSTEM_Models_SAPPUP : DbContext
    {
        public static string host1 = "";
        public static string catalog1 = "";
        public static string user1 = "";
        public static string pass1 = "";

        public MVC_SYSTEM_Models_SAPPUP()
            : base(nameOrConnectionString: "BYOWN")
        {
            base.Database.Connection.ConnectionString = "data source=" + host1 + ";initial catalog=" + catalog1 + ";user id=" + user1 + ";password=" + pass1 + ";MultipleActiveResultSets=True;App=EntityFramework";
        }

        public static MVC_SYSTEM_Models_SAPPUP ConnectToSqlServer(string host, string catalog, string user, string pass)
        {
            host1 = host;
            catalog1 = catalog;
            user1 = user;
            pass1 = pass;

            return new MVC_SYSTEM_Models_SAPPUP();
        }

        public virtual DbSet<tbl_SAPPostDataDetails> tbl_SAPPostDataDetails { get; set; }
        public virtual DbSet<tbl_SAPPostRef> tbl_SAPPostRef { get; set; }
        public virtual DbSet<tbl_SAPPostReturn> tbl_SAPPostReturn { get; set; }
        public virtual DbSet<vw_SAPPostDataFullDetails> vw_SAPPostDataFullDetails { get; set; }
        public virtual DbSet<tbl_TutupUrusNiaga> tbl_TutupUrusNiaga { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
