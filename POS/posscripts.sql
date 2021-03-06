USE [POS]
GO
/****** Object:  DatabaseRole [aspnet_ChangeNotification_ReceiveNotificationsOnlyAccess]    Script Date: 31-01-2017 06:46:57 AM ******/
CREATE ROLE [aspnet_ChangeNotification_ReceiveNotificationsOnlyAccess]
GO
/****** Object:  Schema [aspnet_ChangeNotification_ReceiveNotificationsOnlyAccess]    Script Date: 31-01-2017 06:46:58 AM ******/
CREATE SCHEMA [aspnet_ChangeNotification_ReceiveNotificationsOnlyAccess]
GO
/****** Object:  StoredProcedure [dbo].[AspNet_SqlCachePollingStoredProcedure]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AspNet_SqlCachePollingStoredProcedure] AS
         SELECT tableName, changeId FROM dbo.AspNet_SqlCacheTablesForChangeNotification
         RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[AspNet_SqlCacheQueryRegisteredTablesStoredProcedure]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AspNet_SqlCacheQueryRegisteredTablesStoredProcedure] 
         AS
         SELECT tableName FROM dbo.AspNet_SqlCacheTablesForChangeNotification   
GO
/****** Object:  StoredProcedure [dbo].[AspNet_SqlCacheRegisterTableStoredProcedure]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AspNet_SqlCacheRegisterTableStoredProcedure] 
             @tableName NVARCHAR(450) 
         AS
         BEGIN

         DECLARE @triggerName AS NVARCHAR(3000) 
         DECLARE @fullTriggerName AS NVARCHAR(3000)
         DECLARE @canonTableName NVARCHAR(3000) 
         DECLARE @quotedTableName NVARCHAR(3000) 

         /* Create the trigger name */ 
         SET @triggerName = REPLACE(@tableName, '[', '__o__') 
         SET @triggerName = REPLACE(@triggerName, ']', '__c__') 
         SET @triggerName = @triggerName + '_AspNet_SqlCacheNotification_Trigger' 
         SET @fullTriggerName = 'dbo.[' + @triggerName + ']' 

         /* Create the cannonicalized table name for trigger creation */ 
         /* Do not touch it if the name contains other delimiters */ 
         IF (CHARINDEX('.', @tableName) <> 0 OR 
             CHARINDEX('[', @tableName) <> 0 OR 
             CHARINDEX(']', @tableName) <> 0) 
             SET @canonTableName = @tableName 
         ELSE 
             SET @canonTableName = '[' + @tableName + ']' 

         /* First make sure the table exists */ 
         IF (SELECT OBJECT_ID(@tableName, 'U')) IS NULL 
         BEGIN 
             RAISERROR ('00000001', 16, 1) 
             RETURN 
         END 

         BEGIN TRAN
         /* Insert the value into the notification table */ 
         IF NOT EXISTS (SELECT tableName FROM dbo.AspNet_SqlCacheTablesForChangeNotification WITH (NOLOCK) WHERE tableName = @tableName) 
             IF NOT EXISTS (SELECT tableName FROM dbo.AspNet_SqlCacheTablesForChangeNotification WITH (TABLOCKX) WHERE tableName = @tableName) 
                 INSERT  dbo.AspNet_SqlCacheTablesForChangeNotification 
                 VALUES (@tableName, GETDATE(), 0)

         /* Create the trigger */ 
         SET @quotedTableName = QUOTENAME(@tableName, '''') 
         IF NOT EXISTS (SELECT name FROM sysobjects WITH (NOLOCK) WHERE name = @triggerName AND type = 'TR') 
             IF NOT EXISTS (SELECT name FROM sysobjects WITH (TABLOCKX) WHERE name = @triggerName AND type = 'TR') 
                 EXEC('CREATE TRIGGER ' + @fullTriggerName + ' ON ' + @canonTableName +'
                       FOR INSERT, UPDATE, DELETE AS BEGIN
                       SET NOCOUNT ON
                       EXEC dbo.AspNet_SqlCacheUpdateChangeIdStoredProcedure N' + @quotedTableName + '
                       END
                       ')
         COMMIT TRAN
         END
   
GO
/****** Object:  StoredProcedure [dbo].[AspNet_SqlCacheUnRegisterTableStoredProcedure]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AspNet_SqlCacheUnRegisterTableStoredProcedure] 
             @tableName NVARCHAR(450) 
         AS
         BEGIN

         BEGIN TRAN
         DECLARE @triggerName AS NVARCHAR(3000) 
         DECLARE @fullTriggerName AS NVARCHAR(3000)
         SET @triggerName = REPLACE(@tableName, '[', '__o__') 
         SET @triggerName = REPLACE(@triggerName, ']', '__c__') 
         SET @triggerName = @triggerName + '_AspNet_SqlCacheNotification_Trigger' 
         SET @fullTriggerName = 'dbo.[' + @triggerName + ']' 

         /* Remove the table-row from the notification table */ 
         IF EXISTS (SELECT name FROM sysobjects WITH (NOLOCK) WHERE name = 'AspNet_SqlCacheTablesForChangeNotification' AND type = 'U') 
             IF EXISTS (SELECT name FROM sysobjects WITH (TABLOCKX) WHERE name = 'AspNet_SqlCacheTablesForChangeNotification' AND type = 'U') 
             DELETE FROM dbo.AspNet_SqlCacheTablesForChangeNotification WHERE tableName = @tableName 

         /* Remove the trigger */ 
         IF EXISTS (SELECT name FROM sysobjects WITH (NOLOCK) WHERE name = @triggerName AND type = 'TR') 
             IF EXISTS (SELECT name FROM sysobjects WITH (TABLOCKX) WHERE name = @triggerName AND type = 'TR') 
             EXEC('DROP TRIGGER ' + @fullTriggerName) 

         COMMIT TRAN
         END
   
GO
/****** Object:  StoredProcedure [dbo].[AspNet_SqlCacheUpdateChangeIdStoredProcedure]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AspNet_SqlCacheUpdateChangeIdStoredProcedure] 
             @tableName NVARCHAR(450) 
         AS

         BEGIN 
             UPDATE dbo.AspNet_SqlCacheTablesForChangeNotification WITH (ROWLOCK) SET changeId = changeId + 1 
             WHERE tableName = @tableName
         END
   
GO
/****** Object:  StoredProcedure [dbo].[sp_AddPurchaseOrder]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_AddPurchaseOrder]
(
	@in_OrderName VARCHAR(50),
	@in_Description VARCHAR(1000)
)
AS
BEGIN
	INSERT INTO PurchaseOrder
				(
				OrderName, [Description], [TimeStamp]
				)
	VALUES     
				(
				@in_OrderName, @in_Description,GETDATE()
				)

				select @@IDENTITY
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeletePurchaseOrder]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_DeletePurchaseOrder]
(
 @in_Id int
)
AS
BEGIN
	delete from PurchaseOrder
			where Id=@in_Id
END

GO
/****** Object:  StoredProcedure [dbo].[sp_GetPurchaseOrder]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_GetPurchaseOrder]
(
 @in_Id int
)
AS
BEGIN
	select id,ordername,description,timestamp from PurchaseOrder
			where Id=@in_Id
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Notification]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[sp_Notification]
(
 @in_Timestamp Datetime
)
AS
begin
select id,ordername,Description,TimeStamp from purchaseorder where Timestamp > @in_Timestamp order by Timestamp desc
end
GO
/****** Object:  StoredProcedure [dbo].[sp_ReadPurchaseOrder]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[sp_ReadPurchaseOrder]
as
begin
	select id,ordername,description,timestamp from PurchaseOrder

end
GO
/****** Object:  StoredProcedure [dbo].[sp_RecentPurchaseOrder]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_RecentPurchaseOrder]
as 
begin
select top 5 id,ordername,Description,TimeStamp from purchaseorder order by TimeStamp desc
end
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdatePurchaseOrder]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[sp_UpdatePurchaseOrder]
(
    @in_Id int,
	@in_OrderName VARCHAR(50),
	@in_Description VARCHAR(1000)
)
AS
BEGIN
	UPDATE PurchaseOrder
			SET OrderName=@in_OrderName,Description=@in_Description
			where Id=@in_Id
END
GO
/****** Object:  Table [dbo].[AspNet_SqlCacheTablesForChangeNotification]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNet_SqlCacheTablesForChangeNotification](
	[tableName] [nvarchar](450) NOT NULL,
	[notificationCreated] [datetime] NOT NULL,
	[changeId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[tableName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseOrder]    Script Date: 31-01-2017 06:46:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PurchaseOrder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderName] [varchar](100) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[AspNet_SqlCacheTablesForChangeNotification] ([tableName], [notificationCreated], [changeId]) VALUES (N'PurchaseOrder', CAST(0x0000A70B0180D148 AS DateTime), 492)
SET IDENTITY_INSERT [dbo].[PurchaseOrder] ON 

INSERT [dbo].[PurchaseOrder] ([Id], [OrderName], [Description], [TimeStamp]) VALUES (2, N'Dove', N'Soap', CAST(0x0000A70B015EE782 AS DateTime))
INSERT [dbo].[PurchaseOrder] ([Id], [OrderName], [Description], [TimeStamp]) VALUES (3, N'Sandal', N'Soap', CAST(0x0000A70B015F22E6 AS DateTime))
INSERT [dbo].[PurchaseOrder] ([Id], [OrderName], [Description], [TimeStamp]) VALUES (167, N'Lux', N'Soap', CAST(0x0000A70C00625FED AS DateTime))
INSERT [dbo].[PurchaseOrder] ([Id], [OrderName], [Description], [TimeStamp]) VALUES (241, N'sdfsd', N'eoirtor', CAST(0x0000A70C006B5BFA AS DateTime))
INSERT [dbo].[PurchaseOrder] ([Id], [OrderName], [Description], [TimeStamp]) VALUES (242, N'sdfsd', N'eoirtor', CAST(0x0000A70C006B606A AS DateTime))
INSERT [dbo].[PurchaseOrder] ([Id], [OrderName], [Description], [TimeStamp]) VALUES (240, N'asd', N'asd', CAST(0x0000A70C006B46A3 AS DateTime))
SET IDENTITY_INSERT [dbo].[PurchaseOrder] OFF
ALTER TABLE [dbo].[AspNet_SqlCacheTablesForChangeNotification] ADD  DEFAULT (getdate()) FOR [notificationCreated]
GO
ALTER TABLE [dbo].[AspNet_SqlCacheTablesForChangeNotification] ADD  DEFAULT ((0)) FOR [changeId]
GO
