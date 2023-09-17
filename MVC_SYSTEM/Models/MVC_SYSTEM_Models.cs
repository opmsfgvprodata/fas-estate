namespace MVC_SYSTEM.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.SqlClient;
    using System.Data.Entity.Core.EntityClient;
    [DbConfigurationType(typeof(MVC_SYSTEM_Models_Config))]
    public partial class MVC_SYSTEM_Models : DbContext
    {
        public static string host1 = "";
        public static string catalog1 = "";
        public static string user1 = "";
        public static string pass1 = "";
        public MVC_SYSTEM_Models()
            : base(nameOrConnectionString: "BYOWN")
        {
            base.Database.Connection.ConnectionString = "data source=" + host1 + ";initial catalog=" + catalog1 + ";user id=" + user1 + ";password=" + pass1 + ";MultipleActiveResultSets=True;App=EntityFramework";
        }

        public static MVC_SYSTEM_Models ConnectToSqlServer(string host, string catalog, string user, string pass)
        {
            //SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            //sqlBuilder.DataSource = host;
            //sqlBuilder.InitialCatalog = catalog;
            //sqlBuilder.MultipleActiveResultSets = true;
            //sqlBuilder.UserID = user;
            //sqlBuilder.Password = pass;
            //sqlBuilder.ConnectTimeout = 100;
            //sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.IntegratedSecurity = true;

            //var entityConnectionStringBuilder = new EntityConnectionStringBuilder();
            //entityConnectionStringBuilder.Provider = "System.Data.SqlClient";
            //entityConnectionStringBuilder.ProviderConnectionString = sqlBuilder.ConnectionString;
            //entityConnectionStringBuilder.Metadata = "res://*/";
            host1 = host;
            catalog1 = catalog;
            user1 = user;
            pass1 = pass;

            return new MVC_SYSTEM_Models();

        }
        public virtual DbSet<vw_Kerja_Hdr_Cuti> vw_Kerja_Hdr_Cuti { get; set; }
        public virtual DbSet<vw_Kerja_Bonus> vw_Kerja_Bonus { get; set; }
        public virtual DbSet<tbl_Asaspkt> tbl_Asaspkt { get; set; }
        public virtual DbSet<tbl_Ladang> tbl_Ladang { get; set; }
        public virtual DbSet<tbl_LogDetail> tbl_LogDetail { get; set; }
        public virtual DbSet<tbl_Pkjmast> tbl_Pkjmast { get; set; }
        public virtual DbSet<tbl_Sctran> tbl_Sctran { get; set; }
        public virtual DbSet<tbl_ServicesList> tbl_ServicesList { get; set; }
        public virtual DbSet<tbl_SevicesProcess> tbl_SevicesProcess { get; set; }
        public virtual DbSet<tbl_SevicesProcessHistory> tbl_SevicesProcessHistory { get; set; }
        public virtual DbSet<tbl_Skb> tbl_Skb { get; set; }
        public virtual DbSet<vw_Kerja_OT> vw_Kerja_OT { get; set; }
        //public virtual DbSet<tblHtmlReport> tblHtmlReports { get; set; }
        public virtual DbSet<tbl_Blok> tbl_Blok { get; set; }
        public virtual DbSet<tbl_PktUtama> tbl_PktUtama { get; set; }
        public virtual DbSet<tbl_SubPkt> tbl_SubPkt { get; set; }
        public virtual DbSet<tbl_KumpulanKerja> tbl_KumpulanKerja { get; set; }
        public virtual DbSet<vw_KumpulanPekerja> vw_KumpulanPekerja { get; set; }
        public virtual DbSet<vw_KumpulanKerja> vw_KumpulanKerja { get; set; }
        public virtual DbSet<tbl_CutiPeruntukan> tbl_CutiPeruntukan { get; set; }
        public virtual DbSet<tbl_AktvtKerja> tbl_AktvtKerja { get; set; }
        public virtual DbSet<tbl_Insentif> tbl_Insentif { get; set; }
        public virtual DbSet<vw_MaklumatInsentif> vw_MaklumatInsentif { get; set; }
        public virtual DbSet<vw_InsentifPekerja> vw_InsentifPekerja { get; set; }
        public virtual DbSet<tbl_Produktiviti> tbl_Produktiviti { get; set; }
        public virtual DbSet<tbl_HasilSawitPkt> tbl_HasilSawitPkt { get; set; }
        public virtual DbSet<tbl_HasilSawitSubPkt> tbl_HasilSawitSubPkt { get; set; }
        public virtual DbSet<tbl_HasilSawitBlok> tbl_HasilSawitBlok { get; set; }
         public virtual DbSet<tbl_Photo> tbl_Photo { get; set; }
        //public virtual DbSet<tbl_Photo> tbl_Photo { get; set; }
        public virtual DbSet<tbl_CutiDiambil> tbl_CutiDiambil { get; set; }
        public virtual DbSet<vw_RptAIPS> vw_RptAIPS { get; set; }
        public virtual DbSet<tbl_Kerja> tbl_Kerja { get; set; }
        public virtual DbSet<tbl_Kerjahdr> tbl_Kerjahdr { get; set; }
        public virtual DbSet<vw_Kerjahdr> vw_Kerjahdr { get; set; }
        public virtual DbSet<tbl_TutupUrusNiaga> tbl_TutupUrusNiaga { get; set; }
        public virtual DbSet<tbl_GajiMinima> tbl_GajiMinima { get; set; }
        public virtual DbSet<vw_MaybankFile> vw_MaybankFile { get; set; }
        public virtual DbSet<tbl_PdfGen> tbl_PdfGen { get; set; }
        public virtual DbSet<vw_rptKwspSocso> vw_rptKwspSocso { get; set; }
        public virtual DbSet<tbl_KawTidakBerhasil> tbl_KawTidakBerhasil { get; set; }
        public virtual DbSet<tbl_ByrCarumanTambahan> tbl_ByrCarumanTambahan { get; set; }
        public virtual DbSet<tblHtmlReport> tblHtmlReports { get; set; }
        public virtual DbSet<tbl_IO> tbl_IO { get; set; }
        public virtual DbSet<tbl_MklmtKeluargaPkj> tbl_MklmtKeluargaPkj { get; set; }
        //public virtual DbSet<tbl_IO> tbl_IO { get; set; }
        public virtual DbSet<tbl_KerjaHariTerabai> tbl_KerjaHariTerabai { get; set; }
        public virtual DbSet<vw_JnsPkt> vw_JnsPkt { get; set; }
        public virtual DbSet<tbl_KerjaSAPData> tbl_KerjaSAPData { get; set; }
        public virtual DbSet<tbl_PkjCarumanTambahan> tbl_PkjCarumanTambahan { get; set; }
        public virtual DbSet<tbl_Kepuasan> tbl_Kepuasan { get; set; }
        public virtual DbSet<tbl_PktUtamaOthr> tbl_PktUtamaOthr { get; set; }
        public virtual DbSet<tbl_SupportedDoc> tbl_SupportedDoc { get; set; }
        public virtual DbSet<tbl_TamatPermitPassport> tbl_TamatPermitPassport { get; set; }
        public virtual DbSet<tbl_SAPPostGLIODataDetails> tbl_SAPPostGLIODataDetails { get; set; }
        public virtual DbSet<tbl_SAPPostRef> tbl_SAPPostRef { get; set; }
        public virtual DbSet<tbl_SAPPostVendorDataDetails> tbl_SAPPostVendorDataDetails { get; set; }
        public virtual DbSet<tbl_SAPPostReturn> tbl_SAPPostReturn { get; set; }
        public virtual DbSet<tbl_PkjIncrmntSalary> tbl_PkjIncrmntSalary { get; set; }
        public virtual DbSet<tbl_PkjIncrmntSalaryHistory> tbl_PkjIncrmntSalaryHistory { get; set; }

        public virtual DbSet<tbl_HargaMenuaiHistory> tbl_HargaMenuaiHistory { get; set; }
        public virtual DbSet<tbl_HargaMenuai> tbl_HargaMenuai { get; set; }
        public virtual DbSet<tbl_HutangPekerja> tbl_HutangPekerja { get; set; }
        public virtual DbSet<tbl_HutangPekerjaJumlah> tbl_HutangPekerjaJumlah { get; set; }
        public virtual DbSet<tbl_GajiBulanan> tbl_GajiBulanan { get; set; }
        public virtual DbSet<tbl_KeluargaPkj> tbl_KeluargaPkj { get; set; }

        public virtual DbSet<vw_hutangPekerjaLadang> vw_hutangPekerjaLadang { get; set; }
        public virtual DbSet<vw_PkjCarumanTambahan> vw_PkjCarumanTambahan { get; set; }
        public object tblOptionConfigsWebs { get; internal set; }
        public virtual DbSet<tbl_PktPinjam> tbl_PktPinjam { get; set; }
        public virtual DbSet<vw_GajiBulananPekerja_2> vw_GajiBulananPekerja_2 { get; set; }
        public virtual DbSet<vw_KerjaInfoDetails2> vw_KerjaInfoDetails2 { get; set; }
        public virtual DbSet<vw_KerjaHdrOT> vw_KerjaHdrOT { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbl_Kerja>()
                .Property(e => e.fld_KadarByr)
                .HasPrecision(18, 3);

            //commented by faeza 07.09.2022
            //added by faeza 10.08.2022
            //modelBuilder.Entity<tbl_Kerja>()
            //    .Property(e => e.fld_Amount)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<tbl_Kerja>()
            //    .Property(e => e.fld_OverallAmount)
            //    .HasPrecision(18, 3);
            //end added

            modelBuilder.Entity<tbl_PktUtamaOthr>()
               .Property(e => e.fld_Luas)
               .HasPrecision(10, 3);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_ByrKerja)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_KWSPPkj)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_KWSPMjk)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_SocsoPkj)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_SocsoMjk)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_LainInsentif)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_OT)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_ByrCuti)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_BonusHarian)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_LainPotongan)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_TargetProd)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_CapaiProd)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_ProdInsentif)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_KuaInsentif)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_HdrInsentif)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_AIPS)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_GajiKasar)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_GajiBersih)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_PurataGaji)
                .HasPrecision(8, 2);

            modelBuilder.Entity<tbl_GajiBulanan>()
                .Property(e => e.fld_PurataGaji12Bln)
                .HasPrecision(8, 2);
            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LsBlok)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LsBlok_Sblm)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LuasKawTnmanBlok)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LuasBerhasilBlok)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LuasBlmBerhasilBlok)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LuasKawTiadaTanamanBlok)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_LsPktUtama)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_LsPktUtama_Sblm)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_LuasKawTnman)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_LuasBerhasil)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_LuasBlmBerhasil)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_LuasKawTiadaTanaman)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LsPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LsPkt_Sblm)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LuasKawTnmanPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LuasBerhasilPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LuasBlmBerhasilPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LuasKawTiadaTanamanPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<vw_rptKwspSocso>()
            //    .Property(e => e.fld_KWSPPkj)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_rptKwspSocso>()
            //    .Property(e => e.fld_KWSPMjk)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_rptKwspSocso>()
            //    .Property(e => e.fld_SocsoPkj)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_rptKwspSocso>()
            //    .Property(e => e.fld_SocsoMjk)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_KawTidakBerhasil>()
            //    .Property(e => e.fld_LuasKaw)
            //    .HasPrecision(8, 3);


            //modelBuilder.Entity<vw_MaybankFile>()
            //    .Property(e => e.fld_GajiBersih)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_Kerja>()
            //    .Property(e => e.fld_JumlahHasil)
            //    .HasPrecision(6, 2);

            //modelBuilder.Entity<tbl_Kerja>()
            //    .Property(e => e.fld_BrtGth)
            //    .HasPrecision(6, 2);

            //modelBuilder.Entity<tbl_Kerja>()
            //    .Property(e => e.fld_HrgaKwsnSkar)
            //    .HasPrecision(6, 2);

            //modelBuilder.Entity<tbl_Kerja>()
            //    .Property(e => e.fld_JamOT)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Pklon1)
            //    .HasPrecision(5, 2);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Pklon2)
            //    .HasPrecision(5, 2);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Pklon3)
            //    .HasPrecision(5, 2);


            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Pklon4)
            //    .HasPrecision(5, 2);


            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Pklon5)
            //    .HasPrecision(5, 2);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lspkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lstnm)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lshsl)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lsdsn)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lskmpg)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lspaya)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lsbatu)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lsltrb)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lskbr)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lsklg)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lslln)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Lstank)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Jlnjkr)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Jlnptn)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Kosbgn)
            //    .HasPrecision(9, 2);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Bilpok)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Prtluas)
            //    .HasPrecision(5, 2);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Ktgpok)
            //    .HasPrecision(5, 2);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Llstsq)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Dec1)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Dec2)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_Asaspkt>()
            //    .Property(e => e.fld_Dec3)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_Gaji_Bulanan>()
            //    .Property(e => e.fld_Gaji_Kasar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_Gaji_Bulanan>()
            //    .Property(e => e.fld_Epf_Pkj)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_Gaji_Bulanan>()
            //    .Property(e => e.fld_Epf_Mjk)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_Gaji_Bulanan>()
            //    .Property(e => e.fld_Socso_Pkj)
            //    .HasPrecision(6, 2);

            //modelBuilder.Entity<tbl_Gaji_Bulanan>()
            //    .Property(e => e.fld_Socso_Mjk)
            //    .HasPrecision(6, 2);

            //modelBuilder.Entity<tbl_Gaji_Bulanan>()
            //    .Property(e => e.fld_Insentif)
            //    .HasPrecision(5, 2);

            //modelBuilder.Entity<tbl_Gaji_Bulanan>()
            //    .Property(e => e.fld_Gaji_Bersih)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_Gaji_Bulanan>()
            //    .Property(e => e.fld_Gaji)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_Gaji_Bulanan>()
            //    .Property(e => e.fld_OT)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_Gaji_Bulanan>()
            //    .Property(e => e.fld_Insentif_Mandor)
            //    .HasPrecision(5, 2);

            //modelBuilder.Entity<tbl_Gaji_Bulanan>()
            //    .Property(e => e.fld_Elaun_Motor)
            //    .HasPrecision(5, 2);

            //modelBuilder.Entity<tbl_Sctran>()
            //    .Property(e => e.fld_Amt)
            //    .HasPrecision(13, 2);

            //modelBuilder.Entity<tbl_Blok>()
            //     .Property(e => e.fld_LsBlok)
            //     .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LsBlok_Sblm)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LuasKawTnmanBlok)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LuasBerhasilBlok)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LuasBlmBerhasilBlok)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LuasKawTiadaTanamanBlok)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_LsPktUtama)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_LsPktUtama_Sblm)
            //    .HasPrecision(8, 3);


            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LsPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LsPkt_Sblm)
            //    .HasPrecision(8, 3);


            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LuasKawTnmanPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LuasBerhasilPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LuasBlmBerhasilPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LuasKawTiadaTanamanPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_AktvtKerja>()
            //  .Property(e => e.fld_Peratus)
            //  .HasPrecision(3, 0);

            //modelBuilder.Entity<tbl_AktvtKerja>()
            //    .Property(e => e.fld_Harga)
            //    .HasPrecision(6, 2);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_LuasHektar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_LuasHektar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_LuasHektar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_LuasHektar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_LuasHektar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_LuasHektar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<vw_RptAIPS>()
            //    .Property(e => e.fld_TargetProd)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_RptAIPS>()
            //    .Property(e => e.fld_CapaiProd)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_RptAIPS>()
            //    .Property(e => e.fld_ProdInsentif)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_RptAIPS>()
            //    .Property(e => e.fld_KuaInsentif)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_RptAIPS>()
            //    .Property(e => e.fld_HdrInsentif)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_RptAIPS>()
            //    .Property(e => e.fld_AIPS)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_rptKwspSocso>()
            //    .Property(e => e.fld_KWSPPkj)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_rptKwspSocso>()
            //    .Property(e => e.fld_KWSPMjk)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_rptKwspSocso>()
            //    .Property(e => e.fld_SocsoPkj)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_rptKwspSocso>()
            //    .Property(e => e.fld_SocsoMjk)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_KawTidakBerhasil>()
            //    .Property(e => e.fld_LuasKaw)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_IO>()
            //   .Property(e => e.fld_Luas)
            //   .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_IO>()
            //    .Property(e => e.fld_LuasKawTnmn)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_IO>()
            //    .Property(e => e.fld_LuasKawTiadaTnmn)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_MklmtKeluargaPkj>()
            //    .Property(e => e.fld_Hubungan)
            //    .IsFixedLength();
        }
    }
}
