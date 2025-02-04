DELETE [dbo].[tblMenuList] WHERE [fld_Flag] = 'GenTxtFile'

INSERT INTO [dbo].[tblMenuList] ([fld_Val], [fld_Desc], [fld_Flag], [fld_Sub], [fld_WidthReport], [fld_HeightReport], [fldDeleted], [fld_NegaraID], [fld_SyarikatID]) VALUES (NULL, N'1. eWallet (Monthly)', N'GenTxtFile', N'm2esubest', NULL, NULL, 0, 1, 1)
INSERT INTO [dbo].[tblMenuList] ([fld_Val], [fld_Desc], [fld_Flag], [fld_Sub], [fld_WidthReport], [fld_HeightReport], [fldDeleted], [fld_NegaraID], [fld_SyarikatID]) VALUES (NULL, N'2. eWallet (Others)', N'GenTxtFile', N'm2esubestotr', NULL, NULL, 0, 1, 1)
INSERT INTO [dbo].[tblMenuList] ([fld_Val], [fld_Desc], [fld_Flag], [fld_Sub], [fld_WidthReport], [fld_HeightReport], [fldDeleted], [fld_NegaraID], [fld_SyarikatID]) VALUES (N'cdmas', N'3. CDMAS', N'GenTxtFile', NULL, NULL, NULL, 0, 1, 1)
INSERT INTO [dbo].[tblMenuList] ([fld_Val], [fld_Desc], [fld_Flag], [fld_Sub], [fld_WidthReport], [fld_HeightReport], [fldDeleted], [fld_NegaraID], [fld_SyarikatID]) VALUES (N'cheque', N'4. Cheque', N'GenTxtFile', NULL, NULL, NULL, 0, 1, 1)

DELETE [dbo].[tblMenuList] WHERE [fld_Flag] = 'm2esubest'
INSERT INTO [dbo].[tblMenuList] ([fld_Val], [fld_Desc], [fld_Flag], [fld_Sub], [fld_WidthReport], [fld_HeightReport], [fldDeleted], [fld_NegaraID], [fld_SyarikatID]) VALUES (N'ewallet', N'1. Generate Maybank Text File-Batch', N'm2esubest', NULL, NULL, NULL, 0, 1, 1)
INSERT INTO [dbo].[tblMenuList] ([fld_Val], [fld_Desc], [fld_Flag], [fld_Sub], [fld_WidthReport], [fld_HeightReport], [fldDeleted], [fld_NegaraID], [fld_SyarikatID]) VALUES (N'ewalletindividu', N'2. Generate Maybank Text File-Individu', N'm2esubest', NULL, NULL, NULL, 0, 1, 1)

DELETE [dbo].[tblMenuList] WHERE [fld_Flag] = 'm2esubestotr'
INSERT INTO [dbo].[tblMenuList] ([fld_Val], [fld_Desc], [fld_Flag], [fld_Sub], [fld_WidthReport], [fld_HeightReport], [fldDeleted], [fld_NegaraID], [fld_SyarikatID]) VALUES (N'ewalletInsentive', N'1. Generate Maybank Text File-Batch', N'm2esubestotr', NULL, NULL, NULL, 0, 1, 1)
INSERT INTO [dbo].[tblMenuList] ([fld_Val], [fld_Desc], [fld_Flag], [fld_Sub], [fld_WidthReport], [fld_HeightReport], [fldDeleted], [fld_NegaraID], [fld_SyarikatID]) VALUES (N'ewalletInsentiveindividu', N'2. Generate Maybank Text File-Individu', N'm2esubestotr', NULL, NULL, NULL, 0, 1, 1)
