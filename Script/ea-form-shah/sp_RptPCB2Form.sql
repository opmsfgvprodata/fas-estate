



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[sp_RptPCB2Form] 
	-- Add the parameters for the stored procedure here
	@NegaraID INT,
	@SyarikatID INT,
	@WilayahID INT,
	@LadangID INT,
	@DivisionID INT,
	@Year INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @tbl_Pkjmast TABLE(
		[fld_UniqueID] [uniqueidentifier] NOT NULL,
		[fld_Nopkj] [nvarchar](20) NULL,
		[fld_Nokp] [nvarchar](15) NULL,
		[fld_Nama] [nvarchar](100) NULL,
		[fld_Almt1] [nvarchar](100) NULL,
		[fld_Daerah] [nvarchar](30) NULL,
		[fld_Neg] [nvarchar](5) NULL,
		[fld_Negara] [nvarchar](5) NULL,
		[fld_Poskod] [nvarchar](5) NULL,
		[fld_Notel] [nvarchar](15) NULL,
		[fld_Nofax] [nvarchar](15) NULL,
		[fld_Kdjnt] [nvarchar](1) NULL,
		[fld_Kdbgsa] [nvarchar](2) NULL,
		[fld_Kdagma] [nvarchar](1) NULL,
		[fld_Trlhr] [date] NULL,
		[fld_Kdrkyt] [nvarchar](2) NULL,
		[fld_Kdkwn] [nvarchar](1) NULL,
		[fld_Kpenrka] [nvarchar](1) NULL,
		[fld_Kdaktf] [nvarchar](1) NULL,
		[fld_Trtakf] [date] NULL,
		[fld_Sbtakf] [nvarchar](60) NULL,
		[fld_Trmlkj] [date] NULL,
		[fld_Trshjw] [date] NULL,
		[fld_Ktgpkj] [nvarchar](2) NULL,
		[fld_Jenispekerja] [nvarchar](2) NULL,
		[fld_Kodbkl] [nvarchar](3) NULL,
		[fld_KodSocso] [varchar](5) NULL,
		[fld_Noperkeso] [nvarchar](12) NULL,
		[fld_KodKWSP] [varchar](5) NULL,
		[fld_Nokwsp] [nvarchar](15) NULL,
		[fld_Kdbank] [nvarchar](50) NULL,
		[fld_NoAkaun] [nvarchar](50) NULL,
		[fld_Visano] [nvarchar](15) NULL,
		[fld_T1visa] [date] NULL,
		[fld_T2visa] [date] NULL,
		[fld_Nogilr] [nvarchar](15) NULL,
		[fld_Prmtno] [nvarchar](20) NULL,
		[fld_T1prmt] [date] NULL,
		[fld_T2prmt] [date] NULL,
		[fld_Psptno] [nvarchar](20) NULL,
		[fld_T1pspt] [date] NULL,
		[fld_T2pspt] [date] NULL,
		[fld_Kdldg] [nvarchar](5) NULL,
		[fld_NegaraID] [int] NULL,
		[fld_SyarikatID] [int] NULL,
		[fld_WilayahID] [int] NULL,
		[fld_LadangID] [int] NULL,
		[fld_DivisionID] [int] NULL,
		[fld_DateApply] [datetime] NULL,
		[fld_AppliedBy] [nvarchar](50) NULL,
		[fld_StatusApproved] [int] NULL,
		[fld_ActionBy] [nvarchar](50) NULL,
		[fld_ActionDate] [datetime] NULL,
		[fld_Batch] [nvarchar](50) NULL,
		[fld_IDpkj] [nvarchar](15) NULL,
		[fld_KumpulanID] [int] NULL,
		[fld_StatusKwspSocso] [nvarchar](1) NULL,
		[fld_StatusAkaun] [nvarchar](1) NULL,
		[fld_Remarks] [nvarchar](max) NULL,
		[fld_KodSAPPekerja] [nvarchar](50) NULL,
		[fld_Almt2] [nvarchar](200) NULL,
		[fld_Negara2] [nvarchar](5) NULL,
		[fld_PurposeRequest] [nvarchar](5) NULL,
		[fld_PaymentMode] [nvarchar](15) NULL,
		[fld_Last4Pan] [nvarchar](4) NULL,
		[fld_PassportStatus] [nvarchar](10) NULL,
		[fld_PassportRenewalStatus] [nvarchar](50) NULL,
		[fld_PassportRenewalStartDate] [date] NULL,
		[fld_PermitStatus] [nvarchar](10) NULL,
		[fld_PermitRenewalStatus] [nvarchar](50) NULL,
		[fld_PermitRenewalStartDate] [date] NULL,
		[fld_ContractStartDate] [date] NULL,
		[fld_ContractExpiryDate] [date] NULL,
		[fld_NopkjPermanent] [nvarchar](20) NULL,
		primary key ([fld_UniqueID])
	)

	DECLARE @tbl_GajiBulanan TABLE(
		[fld_ID] [uniqueidentifier] NOT NULL,
		[fld_Nopkj] [nvarchar](20) NULL,
		[fld_ByrKerja] [numeric](8, 2) NULL,
		[fld_KWSPPkj] [numeric](8, 2) NULL,
		[fld_KWSPMjk] [numeric](8, 2) NULL,
		[fld_SocsoPkj] [numeric](8, 2) NULL,
		[fld_SocsoMjk] [numeric](8, 2) NULL,
		[fld_LainInsentif] [numeric](8, 2) NULL,
		[fld_OT] [numeric](8, 2) NULL,
		[fld_ByrCuti] [numeric](8, 2) NULL,
		[fld_BonusHarian] [numeric](8, 2) NULL,
		[fld_LainPotongan] [numeric](8, 2) NULL,
		[fld_TargetProd] [numeric](8, 2) NULL,
		[fld_CapaiProd] [numeric](8, 2) NULL,
		[fld_ProdInsentif] [numeric](8, 2) NULL,
		[fld_KuaTarget] [smallint] NULL,
		[fld_KuaCapai] [smallint] NULL,
		[fld_KuaInsentif] [numeric](8, 2) NULL,
		[fld_HdrTarget] [int] NULL,
		[fld_HdrCapai] [int] NULL,
		[fld_HdrInsentif] [numeric](8, 2) NULL,
		[fld_AIPS] [numeric](8, 2) NULL,
		[fld_GajiKasar] [numeric](8, 2) NULL,
		[fld_GajiBersih] [numeric](8, 2) NULL,
		[fld_PurataGaji] [numeric](8, 2) NULL,
		[fld_PurataGaji12Bln] [numeric](8, 2) NULL,
		[fld_Year] [int] NULL,
		[fld_Month] [int] NULL,
		[fld_NegaraID] [int] NULL,
		[fld_SyarikatID] [int] NULL,
		[fld_WilayahID] [int] NULL,
		[fld_LadangID] [int] NULL,
		[fld_CreatedBy] [int] NULL,
		[fld_DTCreated] [datetime] NULL,
		[fld_ByrKwsnSkr] [numeric](18, 2) NULL,
		[fld_ByrKerjaORP] [numeric](8, 2) NULL,
		[fld_BonusHarianORP] [numeric](8, 2) NULL,
		[fld_LainInsentifORP] [numeric](8, 2) NULL,
		[fld_TotalByrKerjaORP] [numeric](8, 2) NULL,
		[fld_PaymentMode] [nvarchar](15) NULL,
		[fld_NoPkjPermanent] [nvarchar](20) NULL,
		primary key ([fld_ID])
	)

    -- Insert statements for procedure here
	INSERT INTO @tbl_Pkjmast (
		[fld_UniqueID]
		,[fld_Nopkj]
		,[fld_Nokp]
		,[fld_Nama]
		,[fld_Almt1]
		,[fld_Daerah]
		,[fld_Neg]
		,[fld_Negara]
		,[fld_Poskod]
		,[fld_Notel]
		,[fld_Nofax]
		,[fld_Kdjnt]
		,[fld_Kdbgsa]
		,[fld_Kdagma]
		,[fld_Trlhr]
		,[fld_Kdrkyt]
		,[fld_Kdkwn]
		,[fld_Kpenrka]
		,[fld_Kdaktf]
		,[fld_Trtakf]
		,[fld_Sbtakf]
		,[fld_Trmlkj]
		,[fld_Trshjw]
		,[fld_Ktgpkj]
		,[fld_Jenispekerja]
		,[fld_Kodbkl]
		,[fld_KodSocso]
		,[fld_Noperkeso]
		,[fld_KodKWSP]
		,[fld_Nokwsp]
		,[fld_Kdbank]
		,[fld_NoAkaun]
		,[fld_Visano]
		,[fld_T1visa]
		,[fld_T2visa]
		,[fld_Nogilr]
		,[fld_Prmtno]
		,[fld_T1prmt]
		,[fld_T2prmt]
		,[fld_Psptno]
		,[fld_T1pspt]
		,[fld_T2pspt]
		,[fld_Kdldg]
		,[fld_NegaraID]
		,[fld_SyarikatID]
		,[fld_WilayahID]
		,[fld_LadangID]
		,[fld_DivisionID]
		,[fld_DateApply]
		,[fld_AppliedBy]
		,[fld_StatusApproved]
		,[fld_ActionBy]
		,[fld_ActionDate]
		,[fld_Batch]
		,[fld_IDpkj]
		,[fld_KumpulanID]
		,[fld_StatusKwspSocso]
		,[fld_StatusAkaun]
		,[fld_Remarks]
		,[fld_KodSAPPekerja]
		,[fld_Almt2]
		,[fld_Negara2]
		,[fld_PurposeRequest]
		,[fld_PaymentMode]
		,[fld_Last4Pan]
		,[fld_PassportStatus]
		,[fld_PassportRenewalStatus]
		,[fld_PassportRenewalStartDate]
		,[fld_PermitStatus]
		,[fld_PermitRenewalStatus]
		,[fld_PermitRenewalStartDate]
		,[fld_ContractStartDate]
		,[fld_ContractExpiryDate]
		,[fld_NopkjPermanent]
	)
	(
	SELECT 
		[fld_UniqueID]
		,[fld_Nopkj]
		,[fld_Nokp]
		,[fld_Nama]
		,[fld_Almt1]
		,[fld_Daerah]
		,[fld_Neg]
		,[fld_Negara]
		,[fld_Poskod]
		,[fld_Notel]
		,[fld_Nofax]
		,[fld_Kdjnt]
		,[fld_Kdbgsa]
		,[fld_Kdagma]
		,[fld_Trlhr]
		,[fld_Kdrkyt]
		,[fld_Kdkwn]
		,[fld_Kpenrka]
		,[fld_Kdaktf]
		,[fld_Trtakf]
		,[fld_Sbtakf]
		,[fld_Trmlkj]
		,[fld_Trshjw]
		,[fld_Ktgpkj]
		,[fld_Jenispekerja]
		,[fld_Kodbkl]
		,[fld_KodSocso]
		,[fld_Noperkeso]
		,[fld_KodKWSP]
		,[fld_Nokwsp]
		,[fld_Kdbank]
		,[fld_NoAkaun]
		,[fld_Visano]
		,[fld_T1visa]
		,[fld_T2visa]
		,[fld_Nogilr]
		,[fld_Prmtno]
		,[fld_T1prmt]
		,[fld_T2prmt]
		,[fld_Psptno]
		,[fld_T1pspt]
		,[fld_T2pspt]
		,[fld_Kdldg]
		,[fld_NegaraID]
		,[fld_SyarikatID]
		,[fld_WilayahID]
		,[fld_LadangID]
		,[fld_DivisionID]
		,[fld_DateApply]
		,[fld_AppliedBy]
		,[fld_StatusApproved]
		,[fld_ActionBy]
		,[fld_ActionDate]
		,[fld_Batch]
		,[fld_IDpkj]
		,[fld_KumpulanID]
		,[fld_StatusKwspSocso]
		,[fld_StatusAkaun]
		,[fld_Remarks]
		,[fld_KodSAPPekerja]
		,[fld_Almt2]
		,[fld_Negara2]
		,[fld_PurposeRequest]
		,[fld_PaymentMode]
		,[fld_Last4Pan]
		,[fld_PassportStatus]
		,[fld_PassportRenewalStatus]
		,[fld_PassportRenewalStartDate]
		,[fld_PermitStatus]
		,[fld_PermitRenewalStatus]
		,[fld_PermitRenewalStartDate]
		,[fld_ContractStartDate]
		,[fld_ContractExpiryDate]
		,[fld_NopkjPermanent]
	FROM tbl_Pkjmast WITH (NOLOCK) 
	WHERE fld_DivisionID = @DivisionID
	)

	INSERT INTO @tbl_GajiBulanan (
		[fld_ID]
        ,[fld_Nopkj]
        ,[fld_ByrKerja]
        ,[fld_KWSPPkj]
        ,[fld_KWSPMjk]
        ,[fld_SocsoPkj]
        ,[fld_SocsoMjk]
        ,[fld_LainInsentif]
        ,[fld_OT]
        ,[fld_ByrCuti]
        ,[fld_BonusHarian]
        ,[fld_LainPotongan]
        ,[fld_TargetProd]
        ,[fld_CapaiProd]
        ,[fld_ProdInsentif]
        ,[fld_KuaTarget]
        ,[fld_KuaCapai]
        ,[fld_KuaInsentif]
        ,[fld_HdrTarget]
        ,[fld_HdrCapai]
        ,[fld_HdrInsentif]
        ,[fld_AIPS]
        ,[fld_GajiKasar]
        ,[fld_GajiBersih]
        ,[fld_PurataGaji]
        ,[fld_PurataGaji12Bln]
        ,[fld_Year]
        ,[fld_Month]
        ,[fld_NegaraID]
        ,[fld_SyarikatID]
        ,[fld_WilayahID]
        ,[fld_LadangID]
        ,[fld_CreatedBy]
        ,[fld_DTCreated]
        ,[fld_ByrKwsnSkr]
        ,[fld_ByrKerjaORP]
        ,[fld_BonusHarianORP]
        ,[fld_LainInsentifORP]
        ,[fld_TotalByrKerjaORP]
        ,[fld_PaymentMode]
        ,[fld_NoPkjPermanent]
	)
	(
	SELECT 
		[fld_ID]
        ,[fld_Nopkj]
        ,[fld_ByrKerja]
        ,[fld_KWSPPkj]
        ,[fld_KWSPMjk]
        ,[fld_SocsoPkj]
        ,[fld_SocsoMjk]
        ,[fld_LainInsentif]
        ,[fld_OT]
        ,[fld_ByrCuti]
        ,[fld_BonusHarian]
        ,[fld_LainPotongan]
        ,[fld_TargetProd]
        ,[fld_CapaiProd]
        ,[fld_ProdInsentif]
        ,[fld_KuaTarget]
        ,[fld_KuaCapai]
        ,[fld_KuaInsentif]
        ,[fld_HdrTarget]
        ,[fld_HdrCapai]
        ,[fld_HdrInsentif]
        ,[fld_AIPS]
        ,[fld_GajiKasar]
        ,[fld_GajiBersih]
        ,[fld_PurataGaji]
        ,[fld_PurataGaji12Bln]
        ,[fld_Year]
        ,[fld_Month]
        ,[fld_NegaraID]
        ,[fld_SyarikatID]
        ,[fld_WilayahID]
        ,[fld_LadangID]
        ,[fld_CreatedBy]
        ,[fld_DTCreated]
        ,[fld_ByrKwsnSkr]
        ,[fld_ByrKerjaORP]
        ,[fld_BonusHarianORP]
        ,[fld_LainInsentifORP]
        ,[fld_TotalByrKerjaORP]
        ,[fld_PaymentMode]
        ,[fld_NoPkjPermanent]
	FROM tbl_GajiBulanan WITH (NOLOCK) 
	WHERE fld_LadangID = @LadangID AND fld_Year = @Year 
	AND fld_NoPkjPermanent IN (SELECT fld_NoPkjPermanent FROM @tbl_Pkjmast WHERE fld_DivisionID = @DivisionID))

	SELECT * FROM @tbl_Pkjmast

	SELECT * FROM @tbl_GajiBulanan

	SELECT * FROM tbl_TaxWorkerInfo WITH (NOLOCK) WHERE fld_NoPkjPermanent IN (SELECT fld_NoPkjPermanent FROM @tbl_Pkjmast) AND fld_Year = @Year

	SELECT * FROM tbl_ByrCarumanTambahan WITH (NOLOCK) WHERE fld_GajiID IN (SELECT fld_ID FROM @tbl_GajiBulanan) AND fld_KodSubCaruman = 'PCB02'

	SELECT * FROM PUPOPMSCORP..tblOptionConfigsWeb WITH (NOLOCK) WHERE fldOptConfFlag1 IN ('taxResidency','taxMaritalStatus','designation')

	SELECT * FROM tbl_TaxPCB2Form WITH (NOLOCK) WHERE fld_GajiID IN (SELECT fld_ID FROM @tbl_GajiBulanan)
END
GO


