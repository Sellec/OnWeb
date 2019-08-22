CREATE VIEW dbo.[состояние индексов]
AS
SELECT DISTINCT 
                         TOP (100) PERCENT dbschemas.name AS [Schema], dbtables.name AS [Table], dbindexes.name AS [Index], indexstats.avg_fragmentation_in_percent, indexstats.page_count, MAX(ddps.row_count) 
                         AS row_count
FROM            sys.indexes AS dbindexes INNER JOIN
                         sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, NULL) AS indexstats ON dbindexes.object_id = indexstats.object_id AND indexstats.index_id = dbindexes.index_id INNER JOIN
                         sys.schemas AS dbschemas INNER JOIN
                         sys.tables AS dbtables ON dbschemas.schema_id = dbtables.schema_id ON dbindexes.object_id = dbtables.object_id INNER JOIN
                         sys.dm_db_partition_stats AS ddps ON dbindexes.object_id = ddps.object_id
WHERE        (indexstats.database_id = DB_ID()) AND (dbindexes.index_id > 0)
GROUP BY dbschemas.name, dbtables.name, dbindexes.name, indexstats.avg_fragmentation_in_percent, indexstats.page_count
ORDER BY indexstats.avg_fragmentation_in_percent DESC
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'состояние индексов';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'состояние индексов';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "dbindexes"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 251
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "indexstats"
            Begin Extent = 
               Top = 6
               Left = 289
               Bottom = 68
               Right = 475
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "dbschemas"
            Begin Extent = 
               Top = 6
               Left = 513
               Bottom = 119
               Right = 699
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "dbtables"
            Begin Extent = 
               Top = 6
               Left = 737
               Bottom = 136
               Right = 1005
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ddps"
            Begin Extent = 
               Top = 120
               Left = 289
               Bottom = 250
               Right = 581
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'состояние индексов';

