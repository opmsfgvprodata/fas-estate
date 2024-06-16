USE [PUPOPMSESTPUP]
GO

/****** Object:  StoredProcedure [dbo].[sp_Payslip_V2]    Script Date: 10/06/2024 22:01:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		< Faeza >
-- Create date: < 13/02/2023 >
-- Description:	< Report Payslip ORP>
-- =============================================
CREATE PROCEDURE [dbo].[sp_Payslip_V2]
	@NegaraID INT,
	@SyarikatID INT,
	@WilayahID INT,
	@LadangID INT,
	@Month INT,
	@Year INT,
	@Workers [dbo].[Workers] readonly
AS
	DECLARE @flag int
	DECLARE @flagincome int
	DECLARE @desc varchar(10)
	DECLARE @dbhq nvarchar(50)
	DECLARE @dbname nvarchar(50)
	
	CREATE TABLE #tbl_payslip(
		fldID INT IDENTITY(1,1),
		fldNopkj varchar(20),
		fldKodPkt varchar(50),
		fldKod varchar(10),
		fldKeterangan varchar(200),
		fldKuantiti numeric(18,2),
		fldUnit varchar(10),
		fldKadar numeric(18,2),
		fldGandaan int,
		fldJumlah numeric(18,2),
		fldBulan int,
		fldTahun int,
		fldNegaraID int,
		fldSyarikatID int,
		fldWilayahID int,
		fldLadangID int,
		fldFlag int,
		fldFlagIncome int, --added by faeza 13.02.2023
		primary key (fldID)
	)
BEGIN

--------------------------------------------------CARUMAN---------------------------------------------------
set @flag=1	
set @flagincome=1 --added by faeza 13.02.2023

	--KWSP/SOCSO (M)--
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldFlagIncome, --added by faeza 13.02.2023
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		fld_Nopkj,
		Caruman,
		case Caruman
			when 'KWSP_M' then 'KWSP (M)'
			when 'SOCSO_M' then 'SOCSO (M)'
			else Caruman end as 'Caruman',
		Kadar,
		@flag,
		@flagincome,
		fld_Month,
		fld_Year,
		fld_NegaraID,
		fld_SyarikatID,
		fld_WilayahID,
		fld_LadangID
		from(
			select fld_Nopkj,
			fld_KWSPMjk as KWSP_M, fld_SocsoMjk as SOCSO_M,
			fld_Month,fld_Year,fld_NegaraID,fld_SyarikatID,fld_WilayahID,fld_LadangID
			from [tbl_GajiBulanan]
			where fld_Month=@Month and fld_Year=@Year and 
			fld_NegaraID=@NegaraID and fld_SyarikatID=@SyarikatID and  fld_WilayahID=@WilayahID and fld_LadangID=@LadangID)p
		unpivot
			(Kadar for Caruman in (KWSP_M,Socso_M) )
		as unpvt
	)
	
	--SIP/caruman tambahan (M)--
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		b.fld_KodCaruman,
		b.fld_KodCaruman,
		b.fld_CarumanMajikan,
		@flag,
		@flagincome,
		b.fld_Month,
		b.fld_Year,
		b.fld_NegaraID,
        b.fld_SyarikatID,
        b.fld_WilayahID,
        b.fld_LadangID
		from [tbl_GajiBulanan] a,[tbl_ByrCarumanTambahan] b
		where b.fld_Month=@Month and b.fld_Year=@Year and a.fld_ID=b.fld_GajiID
		and b.fld_NegaraID=@NegaraID and b.fld_SyarikatID=@SyarikatID and b.fld_WilayahID=@WilayahID and b.fld_LadangID=@LadangID
	)
--------------------------------------------------PENDAPATAN---------------------------------------------------	
set @flag=2	
	
	--Kerja (H01- Hadir hari biasa)--
	set @flagincome=2 --added by faeza 13.02.2023
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldGandaan,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodAktvt,
		b.fld_Desc,
		SUM(a.fld_JumlahHasil)as JumlahHasil, 
		a.fld_Unit,
		a.fld_KadarByr, 
		d.fldOptConfFlag3,
		SUM(a.fld_Amount)as Amount,
		@flag,
		@flagincome,
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		FROM [tbl_Kerja] a,[PUPOPMSCORP].[dbo].[tbl_UpahAktiviti] b,[tbl_Kerjahdr] c,[PUPOPMSCORP].[dbo].[tblOptionConfigsWeb] d,[tbl_PktUtama] e
		where MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_KodAktvt=b.fld_KodAktvt and a.fld_JnisAktvt = b.fld_KodJenisAktvt and b.fld_NegaraID=a.fld_NegaraID and b.fld_SyarikatID=a.fld_SyarikatID and a.fld_WilayahID = b.fld_WilayahID
		and c.fld_Nopkj=a.fld_Nopkj and c.fld_Tarikh=a.fld_Tarikh and c.fld_NegaraID=a.fld_NegaraID and c.fld_SyarikatID=a.fld_SyarikatID and c.fld_WilayahID=a.fld_WilayahID and c.fld_LadangID=a.fld_LadangID
		and d.fldOptConfFlag1='cuti' and d.fldOptConfValue=c.fld_Kdhdct and d.fldOptConfValue = 'H01' and d.fld_NegaraID=a.fld_NegaraID and d.fld_SyarikatID=a.fld_SyarikatID
		and e.fld_NegaraID=a.fld_NegaraID and e.fld_SyarikatID=a.fld_SyarikatID and e.fld_WilayahID=a.fld_WilayahID and e.fld_LadangID=a.fld_LadangID and e.fld_PktUtama=a.fld_KodPkt
		group by a.fld_KodAktvt,d.fldOptConfFlag3,a.fld_Nopkj,a.fld_Unit,a.fld_KadarByr,b.fld_Desc,a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID,MONTH(a.fld_Tarikh),YEAR(a.fld_Tarikh)
	)

	--Kerja (H02- Hadir hari mggu & H03 - Hadir cuti umum)--
	set @flagincome=3 --added by faeza 13.02.2023
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldGandaan,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodAktvt,
		b.fld_Desc,
		SUM(a.fld_JumlahHasil)as JumlahHasil, 
		a.fld_Unit,
		a.fld_KadarByr, 
		d.fldOptConfFlag3,
		SUM(a.fld_Amount)as Amount,
		@flag,
		@flagincome,
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		FROM [tbl_Kerja] a,[PUPOPMSCORP].[dbo].[tbl_UpahAktiviti] b,[tbl_Kerjahdr] c,[PUPOPMSCORP].[dbo].[tblOptionConfigsWeb] d,[tbl_PktUtama] e
		where MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_KodAktvt=b.fld_KodAktvt and a.fld_JnisAktvt = b.fld_KodJenisAktvt and b.fld_NegaraID=a.fld_NegaraID and b.fld_SyarikatID=a.fld_SyarikatID and a.fld_WilayahID = b.fld_WilayahID
		and c.fld_Nopkj=a.fld_Nopkj and c.fld_Tarikh=a.fld_Tarikh and c.fld_NegaraID=a.fld_NegaraID and c.fld_SyarikatID=a.fld_SyarikatID and c.fld_WilayahID=a.fld_WilayahID and c.fld_LadangID=a.fld_LadangID
		and d.fldOptConfFlag1='cuti' and d.fldOptConfValue=c.fld_Kdhdct and (d.fldOptConfValue = 'H02' or d.fldOptConfValue = 'H03') and d.fld_NegaraID=a.fld_NegaraID and d.fld_SyarikatID=a.fld_SyarikatID
		and e.fld_NegaraID=a.fld_NegaraID and e.fld_SyarikatID=a.fld_SyarikatID and e.fld_WilayahID=a.fld_WilayahID and e.fld_LadangID=a.fld_LadangID and e.fld_PktUtama=a.fld_KodPkt
		group by a.fld_KodAktvt,d.fldOptConfFlag3,a.fld_Nopkj,a.fld_Unit,a.fld_KadarByr,b.fld_Desc,a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID,MONTH(a.fld_Tarikh),YEAR(a.fld_Tarikh)
	)

	--Kerja Kwsn Sukar--
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldGandaan,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodAktvt,
		'Critical Area - '+b.fld_Desc,
		SUM(a.fld_JumlahHasil)as JumlahHasil, 
		a.fld_Unit,
		SUM(a.fld_HrgaKwsnSkar)/SUM(a.fld_JumlahHasil), 
		d.fldOptConfFlag3,
		SUM(a.fld_HrgaKwsnSkar)as Amount,
		@flag,
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		FROM [tbl_Kerja] a,[PUPOPMSCORP].[dbo].[tbl_UpahAktiviti] b,[tbl_Kerjahdr] c,[PUPOPMSCORP].[dbo].[tblOptionConfigsWeb] d
		where a.fld_HrgaKwsnSkar <> 0 and MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_KodAktvt=b.fld_KodAktvt and b.fld_NegaraID=a.fld_NegaraID and b.fld_SyarikatID=a.fld_SyarikatID and a.fld_WilayahID = b.fld_WilayahID
		and c.fld_Nopkj=a.fld_Nopkj and c.fld_Tarikh=a.fld_Tarikh and c.fld_Kum=a.fld_Kum and c.fld_NegaraID=a.fld_NegaraID and c.fld_SyarikatID=a.fld_SyarikatID and c.fld_WilayahID=a.fld_WilayahID and c.fld_LadangID=a.fld_LadangID
		and d.fldOptConfFlag1='cuti' and d.fldOptConfValue=c.fld_Kdhdct and d.fld_NegaraID=a.fld_NegaraID and d.fld_SyarikatID=a.fld_SyarikatID
		group by a.fld_KodAktvt,d.fldOptConfFlag3,a.fld_Nopkj,a.fld_Unit,a.fld_KadarByr,b.fld_Desc,a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID,MONTH(a.fld_Tarikh),YEAR(a.fld_Tarikh)
	)
	
	--OT--
	set @desc='OT'
	set @flagincome=3
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		@desc,
		'Over Time - '+d.fldOptConfFlag3,
		SUM(a.fld_JamOT),
		b.fldOptConfDesc,
		a.fld_Kadar,
		SUM(a.fld_Jumlah),
		@flag, 
		@flagincome,
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_KerjaOT] a, [PUPOPMSCORP].[dbo].[tblOptionConfigsWeb] b, [tbl_Kerjahdr] c, [PUPOPMSCORP].[dbo].[tblOptionConfigsWeb] d
		where MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year 
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and b.fldOptConfFlag1='unit' and b.fldOptConfValue='HOUR' and b.fldDeleted=0 and b.fld_NegaraID=a.fld_NegaraID and b.fld_SyarikatID=a.fld_SyarikatID
		and c.fld_Nopkj=a.fld_Nopkj and c.fld_Tarikh=a.fld_Tarikh 
		and c.fld_NegaraID=a.fld_NegaraID and c.fld_SyarikatID=a.fld_SyarikatID and c.fld_WilayahID=a.fld_WilayahID and c.fld_LadangID=a.fld_LadangID
		and d.fldOptConfFlag1='kiraot' and d.fldOptConfFlag2=c.fld_Kdhdct and d.fld_NegaraID=a.fld_NegaraID and d.fld_SyarikatID=a.fld_SyarikatID and d.fldDeleted=0
		group by d.fldOptConfFlag3,MONTH(a.fld_Tarikh),YEAR(a.fld_Tarikh),a.fld_Nopkj,a.fld_Kadar,b.fldOptConfDesc,
		a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID
	)
	
	--byr cuti--
	set @desc='HARI'
	set @flagincome=2
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		b.fld_Kdhdct,
		c.fldOptConfDesc,
		COUNT(b.fld_Kdhdct),
		@desc,
		a.fld_Kadar,
		SUM(a.fld_Jumlah),
		@flag, 
		@flagincome,
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_KerjahdrCuti] a,[tbl_Kerjahdr] b, [PUPOPMSCORP].[dbo].[tblOptionConfigsWeb] c
		where MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year 
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID 
		and a.[fld_KerjahdrID]=b.[fld_UniqueID] and c.fldOptConfFlag1='cuti' --and b.fld_Kdhdct!='c02'
		and c.fldOptConfValue=b.fld_Kdhdct and c.fld_NegaraID=b.fld_NegaraID and c.fld_SyarikatID=b.fld_SyarikatID
		GROUP BY a.fld_Nopkj,b.fld_Kdhdct,a.fld_Kadar,a.fld_Jumlah,c.fldOptConfDesc,MONTH(a.fld_Tarikh),YEAR(a.fld_Tarikh),
		a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID
	)	

	--Baki cuti tahunan--
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodCuti,
		b.fld_KeteranganCuti,
		a.fld_JumlahCuti,
		@desc,
		a.fld_Kadar,
		a.fld_JumlahAmt,
		@flag,
		@flagincome,
		a.fld_Month,
		a.fld_Year,
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		FROM [tbl_KerjahdrCutiTahunan] a WITH (NOLOCK) Inner Join [PUPOPMSCORP].[dbo].[tbl_CutiKategori] b  WITH (NOLOCK) ON a.fld_KodCuti = b.fld_KodCuti
		where a.fld_Month=@Month and a.fld_Year=@Year and
		a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID and a.fld_KodCuti = 'C99'
	)	
	
	--bonus harian-- (H01 - include in ORP)
	set @desc='Bonus'
	set @flagincome=2
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		b.fld_KodAktvt,
		'Daily Incentive - '+ d.fld_Desc,
		COUNT(a.fld_Tarikh),
		c.fldOptConfDesc,
		a.fld_Kadar,
		SUM(a.fld_Jumlah),
		@flag,
		@flagincome,
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_KerjaBonus] a,[tbl_Kerja] b,[PUPOPMSCORP].[dbo].[tblOptionConfigsWeb] c,[PUPOPMSCORP].[dbo].[tbl_UpahAktiviti] d, [tbl_kerjahdr] e
		where MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year 
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_NegaraID=b.fld_NegaraID and a.fld_SyarikatID=b.fld_SyarikatID and a.fld_WilayahID=b.fld_WilayahID and a.fld_LadangID=b.fld_LadangID
		and a.fld_KerjaID=b.fld_ID and c.fldOptConfFlag1='unit' and c.fldOptConfValue='DAILY' and d.fld_KodAktvt=b.fld_KodAktvt
		and d.fld_NegaraID=a.fld_NegaraID and d.fld_SyarikatID=a.fld_SyarikatID and d.fld_WilayahID = b.fld_WilayahID
		and e.fld_Tarikh = a.fld_Tarikh and e.fld_Nopkj = a.fld_Nopkj and e.fld_LadangID = a.fld_LadangID and e.fld_Kdhdct = 'H01'
		group by b.fld_KodAktvt,a.fld_Nopkj,a.fld_Kadar,c.fldOptConfDesc,d.fld_Desc,
		YEAR(a.fld_Tarikh),MONTH(a.fld_Tarikh),a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID
	)

	--bonus harian (H02 & H03)--
	set @desc='Bonus'
	set @flagincome=3
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		b.fld_KodAktvt,
		'Daily Incentive - '+ d.fld_Desc,
		COUNT(a.fld_Tarikh),
		c.fldOptConfDesc,
		a.fld_Kadar,
		SUM(a.fld_Jumlah),
		@flag,
		@flagincome,
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_KerjaBonus] a,[tbl_Kerja] b,[PUPOPMSCORP].[dbo].[tblOptionConfigsWeb] c,[PUPOPMSCORP].[dbo].[tbl_UpahAktiviti] d, [tbl_kerjahdr] e
		where MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year 
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_NegaraID=b.fld_NegaraID and a.fld_SyarikatID=b.fld_SyarikatID and a.fld_WilayahID=b.fld_WilayahID and a.fld_LadangID=b.fld_LadangID
		and a.fld_KerjaID=b.fld_ID and c.fldOptConfFlag1='unit' and c.fldOptConfValue='DAILY' and d.fld_KodAktvt=b.fld_KodAktvt
		and d.fld_NegaraID=a.fld_NegaraID and d.fld_SyarikatID=a.fld_SyarikatID and d.fld_WilayahID = b.fld_WilayahID
		and e.fld_Tarikh = a.fld_Tarikh and e.fld_Nopkj = a.fld_Nopkj and e.fld_LadangID = a.fld_LadangID and (e.fld_Kdhdct = 'H02' or e.fld_Kdhdct = 'H03')
		group by b.fld_KodAktvt,a.fld_Nopkj,a.fld_Kadar,c.fldOptConfDesc,d.fld_Desc,
		YEAR(a.fld_Tarikh),MONTH(a.fld_Tarikh),a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID
	)
	
	--insentif (include in ORP)--
	set @flagincome=2
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodInsentif,
		b.fld_Keterangan,
		a.fld_NilaiInsentif,
		@flag,
		@flagincome,
		a.fld_Month,
		a.fld_Year,
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_Insentif] a,[PUPOPMSCORP].[dbo].[tbl_JenisInsentif] b
		where a.fld_KodInsentif like 'P%' and a.fld_KodInsentif=b.fld_KodInsentif and a.fld_Deleted=0
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_NegaraID=b.fld_NegaraID and a.fld_SyarikatID=b.fld_SyarikatID and b.fld_AdaORP = 1 and b.fld_InclSecondPayslip = 0
		and a.fld_Month=@Month and a.fld_Year=@Year
	)

	--insentif (exclude in ORP)--
	set @flagincome=3
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodInsentif,
		b.fld_Keterangan,
		a.fld_NilaiInsentif,
		@flag,
		@flagincome,
		a.fld_Month,
		a.fld_Year,
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_Insentif] a,[PUPOPMSCORP].[dbo].[tbl_JenisInsentif] b
		where a.fld_KodInsentif like 'P%' and a.fld_KodInsentif=b.fld_KodInsentif and a.fld_Deleted=0
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_NegaraID=b.fld_NegaraID and a.fld_SyarikatID=b.fld_SyarikatID and b.fld_AdaORP = 0 and b.fld_InclSecondPayslip = 0
		and a.fld_Month=@Month and a.fld_Year=@Year
	)

	----AIPS--
	--insert into #tbl_payslip
	--(
	--	fldNopkj,
	--	fldKod,
	--	fldKeterangan,
	--	fldJumlah,
	--	fldFlag,
	--	fldBulan,
	--	fldTahun,
	--	fldNegaraID,
	--	fldSyarikatID,
	--	fldWilayahID,
	--	fldLadangID
	--)
	--(
	--select 
	--	fld_Nopkj,
	--	'AIPS',
	--	'AIPS',
	--	fld_AIPS,
	--	@flag,
	--	fld_Month,
	--	fld_Year,
	--	fld_NegaraID,
 --       fld_SyarikatID,
 --       fld_WilayahID,
 --       fld_LadangID
	--	from [tbl_GajiBulanan]
	--	where fld_Nopkj=@Nopkj and fld_Month=@Month and fld_Year=@Year
	--	and fld_NegaraID=@NegaraID and fld_SyarikatID=@SyarikatID and fld_WilayahID=@WilayahID and fld_LadangID=@LadangID
	--)
	
	
	
--------------------------------------------------POTONGAN---------------------------------------------------
set @flag=3
set @flagincome=4

	--KWSP/SOCSO (P)--
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		fld_Nopkj,
		Caruman,
		case Caruman
			when 'KWSP_P' then 'KWSP (P)'
			when 'SOCSO_P' then 'SOCSO (P)'
			else Caruman end as 'Caruman',
		Kadar,
		@flag,
		@flagincome,
		fld_Month,
		fld_Year,
		fld_NegaraID,
		fld_SyarikatID,
		fld_WilayahID,
		fld_LadangID
		from(
			select fld_Nopkj,fld_KWSPPkj as KWSP_P,fld_SocsoPkj as SOCSO_P,
			fld_Month,fld_Year,fld_NegaraID,fld_SyarikatID,fld_WilayahID,fld_LadangID
			from [tbl_GajiBulanan]
			where fld_Month=@Month and fld_Year=@Year and 
			fld_NegaraID=@NegaraID and fld_SyarikatID=@SyarikatID and  fld_WilayahID=@WilayahID and fld_LadangID=@LadangID)p
		unpivot
			(Kadar for Caruman in (KWSP_P,Socso_P) )
		as unpvt
	)
	
	--SIP/caruman tambahan (P)--
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		b.fld_KodCaruman,
		b.fld_KodCaruman,
		b.fld_CarumanPekerja,
		@flag,
		@flagincome,
		b.fld_Month,
		b.fld_Year,
		b.fld_NegaraID,
        b.fld_SyarikatID,
        b.fld_WilayahID,
        b.fld_LadangID
		from [tbl_GajiBulanan] a,[tbl_ByrCarumanTambahan] b
		where b.fld_Month=@Month and b.fld_Year=@Year and a.fld_ID=b.fld_GajiID
		and b.fld_NegaraID=@NegaraID and b.fld_SyarikatID=@SyarikatID and b.fld_WilayahID=@WilayahID and b.fld_LadangID=@LadangID
	)
	
	--potongan--
	insert into #tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldFlagIncome,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodInsentif,
		b.fld_Keterangan,
		a.fld_NilaiInsentif,
		@flag,
		@flagincome,
		a.fld_Month,
		a.fld_Year,
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_Insentif] a,[PUPOPMSCORP].[dbo].[tbl_JenisInsentif] b
		where a.fld_KodInsentif like 'T%' and a.fld_KodInsentif=b.fld_KodInsentif and a.fld_Deleted=0
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_NegaraID=b.fld_NegaraID and a.fld_SyarikatID=b.fld_SyarikatID
		and a.fld_Month=@Month and a.fld_Year=@Year
	)
	
	
END
SELECT * FROM #tbl_payslip where fldNopkj IN (select WorkerID from @Workers)
DROP TABLE #tbl_payslip
RETURN 0
GO


