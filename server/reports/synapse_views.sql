SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- alerts view
CREATE OR ALTER VIEW [dbo].[alerts]
AS
SELECT       
      [muni] as [muni]
      ,[location] as [location]
      ,[cluster] as [cluster]
      ,CAST(reportedTime AS datetime)  as [reportedTime]
      ,[id] as [id]
      , cast([weight] as int) [Weight]
      , muni+'|'+cluster as municluster
FROM OPENROWSET(​'CosmosDB',N'Account=<your account>;Database=<your cosmos db>;key=<your key>',<your alerts collection>) as q1
GO


CREATE OR ALTER VIEW [dbo].[device_inventory]
AS
SELECT [heartbeat]
      ,[lon]
      ,[location]
      ,[lat]
      ,[muni]
      ,[cluster]
      ,[alt]
    , cast([ingesttimestamp] as datetime) as ingesttimestamp
      ,[id]
      , muni+'|'+cluster as municluster
FROM OPENROWSET(​'CosmosDB',N'Account=<your account>;Database=<your cosmos db>;key=<your key>',<your inventory collection>) as q1
GO


CREATE ALTER VIEW [dbo].[telemetry]
AS
SELECT       cast([deviceheartbeat] as datetime) as [deviceheartbeat]
      ,[model1]
      ,[model2]
      ,[model3]
      ,[detected]
      ,cast([ingesttimestamp] as datetime) as [ingesttimestamp]
      ,[detectionValue]
      ,[muni]
      ,[cluster]
      ,[alt]
      ,[deviceId]
      ,[lat]
      ,[location]
      ,[lon]
      ,cast([heartbeat] as datetime) as [heartbeat]
      ,[id]
      , muni+'|'+cluster as municluster
FROM OPENROWSET(​'CosmosDB',N'Account=<your account>;Database=<your cosmos db>;key=<your key>',<your inventory telemetry>) as q1
GO



CREATE OR ALTER VIEW [dbo].[map_layers]
AS
SELECT 
        'device' as [type]
       ,[id] --,[lat],[lon]
      ,[muni]
      ,[cluster]
      ,[location]
      , ingesttimestamp as reportedTime
      , CASE WHEN datediff(minute,ingesttimestamp,getdate()) <3 THEN -1 ELSE -2 END as [Weight] -- -1 : online -2 : offline
      , CASE WHEN datediff(minute,ingesttimestamp,getdate()) <3 THEN 'DEVICE ONLINE' ELSE 'DEVICE OFFLINE' END as [Description] 
      ,  municluster
FROM [dbo].[device_inventory]
UNION ALL
SELECT       
      'alert' as [type]
      ,[id]
      ,[muni]
      ,[cluster]
      ,[location]
      ,[reportedTime]
      ,  [Weight]
      ,  CASE  WHEN [Weight] <=2 THEN 'LOW CONFIDENCE' WHEN [Weight] > 2 and [Weight] <=3 THEN 'MEDIUM CONFIDENCE' ELSE 'HIGH CONFIDENCE' END as [Description] 
      ,  municluster
FROM alerts;
GO
