USE [XDropsWater]
GO
/****** Object:  StoredProcedure [dbo].[P_BulkAddCodes]    Script Date: 11/11/2017 10:15:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	批量添加货号
-- =============================================
CREATE PROCEDURE [dbo].[P_BulkAddCodes]
	-- Add the parameters for the stored procedure here
(
	@codeFrom bigint, --开始货号
	@codeTo bigint, --结束货号
	@createBy uniqueidentifier, --操作用户
	@orderDetailsId uniqueidentifier, --订单明细id
	@status int --0没有识别码，1识别码未填满，2识别码已填满
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	begin tran;

	DECLARE @codeFrom1 BIGINT = @codeFrom;
    -- Insert statements for procedure here
    --插入货号
    while(@codeFrom1<=@codeTo)
    begin
    
		insert into IdentityCode values
		(NEWID(),@orderDetailsId, @codeFrom1, 0, @createBy, GETDATE(), null,NULL);
		
		set @codeFrom1 = @codeFrom1 + 1;
    end
    
    DECLARE @ProductID INT;
    SELECT @ProductID = ProductID FROM OrderDetails WHERE ID = @orderDetailsId;
    
    --当前使用这些货号的订单明细以后不能使用这些货号
	update IdentityCode
	set [Status] = 1
	where OrderDetailsID IN( 
		SELECT ID FROM dbo.OrderDetails
		WHERE CreateBy=@createBy
		AND ProductID = @ProductID
	)
	and Code >= @codeFrom
	and Code <= @codeTo;
    
    --货号数量达到订货数量，更新订单明细
    update OrderDetails
	set [Status] = @status
	where ID = @orderDetailsId
    
	--所有订单明细货号数量达到订货数量，更新订单
	DECLARE @orderId UNIQUEIDENTIFIER;
	SELECT @orderId = OrderID FROM dbo.OrderDetails
	WHERE ID=@orderDetailsId;

	DECLARE @count INT;
	SELECT @count = COUNT(1) FROM dbo.OrderDetails
	WHERE OrderID = @orderId
	AND [Status] = 1; --识别码未填满

	IF(@count = 0)
	BEGIN
		UPDATE dbo.Orders
		SET [Status] = 3 --识别码已填满
		WHERE ID = @orderId
    END
    
	
    
    commit tran;
	
END


GO
/****** Object:  StoredProcedure [dbo].[P_BulkRemoveAllCodes]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	删除所有识别码
-- =============================================
CREATE PROCEDURE [dbo].[P_BulkRemoveAllCodes]
	-- Add the parameters for the stored procedure here
(
	@orderDetailsId UNIQUEIDENTIFIER
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRAN;
    -- Insert statements for procedure here
    DECLARE @productId INT;
    DECLARE @orderId UNIQUEIDENTIFIER;
    SELECT @productId = ProductID, @orderId = OrderID FROM OrderDetails
    WHERE ID = @orderDetailsId;
    
    UPDATE IdentityCode
    SET [Status] = 0 -- 可用
    WHERE ID IN(
		SELECT code.ID FROM IdentityCode code
		INNER JOIN OrderDetails od ON code.OrderDetailsID = od.ID
		WHERE od.ProductID = @productId
		AND code.[Status] = 1 -- 不可用，但下家还没发货
    )
    
	-- 删除指定订单明细的所有识别码
	DELETE IdentityCode
	WHERE ID IN(
		SELECT ID FROM IdentityCode WHERE OrderDetailsID = @orderDetailsId
	)
	
	--订单明细识别码未填满
	UPDATE OrderDetails
	SET [Status]=1
	WHERE ID=@orderDetailsId;
	
	--订单识别码未填满
	UPDATE Orders
	SET [Status]=2
	WHERE ID=@orderId;
	
	COMMIT TRAN;
END


GO
/****** Object:  StoredProcedure [dbo].[P_BulkUpdateMemberProduct]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	批量插入产品库存到会员产品表中
-- =============================================
CREATE PROCEDURE [dbo].[P_BulkUpdateMemberProduct]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRAN;
    -- Insert statements for procedure here
    
    -- 删除会员产品表记录
    DELETE FROM MemberProduct;
    
    DECLARE @strUnitPrice VARCHAR(50);
    SELECT @strUnitPrice = ConfigValue FROM SystemConfig WHERE Name = 'Price';
    
    DECLARE @unitPrice DECIMAL;
    SET @unitPrice = CONVERT(DECIMAL, @strUnitPrice);
    
    DECLARE @memberId UNIQUEIDENTIFIER; -- 会员ID
    DECLARE @quantity INT; -- 库存数量
    
    
    
    -- 递归插入库存到会员产品表
    DECLARE cur CURSOR FOR
	SELECT ID, TotalQuantity FROM Members
	WHERE ISNULL(TotalQuantity,0) > 0;
	OPEN cur;
	FETCH NEXT FROM cur INTO @memberId, @quantity;
	WHILE(@@FETCH_STATUS=0)
	BEGIN
	
		-- 更新库存表，1小水滴二代饮水宝，2小水滴二代沐浴宝
		INSERT INTO MemberProduct VALUES
		(NEWID(), @memberId, 1, @quantity, NULL,NULL,NULL,NULL);
		INSERT INTO MemberProduct VALUES
		(NEWID(), @memberId, 2, @quantity, NULL,NULL,NULL,NULL);
		
		-- 更新会员总金额，会员当前角色总金额
		UPDATE Members
		SET TotalAmount = ISNULL(TotalCount, 0) * @unitPrice,
			CurrentRoleAmount = ISNULL(CurrentRoleQuantity, 0) * @unitPrice
		WHERE ID = @memberId;
		
		FETCH NEXT FROM cur INTO @memberId, @quantity;
	END
	CLOSE cur;
	DEALLOCATE cur;
	
	COMMIT TRAN;
END


GO
/****** Object:  StoredProcedure [dbo].[P_CalculateTotalCount]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	计算总进货量
-- =============================================
CREATE PROCEDURE [dbo].[P_CalculateTotalCount]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ID uniqueidentifier;
	DECLARE @RoleID INT;
	DECLARE @Quantity INT;

     --1.取会员
    DECLARE cur CURSOR FOR
    SELECT ID, RoleID
    FROM Members;
    OPEN cur;
    FETCH NEXT FROM cur INTO @ID, @RoleID;
    WHILE(@@FETCH_STATUS = 0)
    BEGIN
		SELECT @Quantity=ISNULL(SUM(Quantity),0) FROM Orders WHERE MemberID=@ID AND IsDeliverly=1;
		UPDATE Members SET TotalCount=@Quantity WHERE ID=@ID;
		FETCH NEXT FROM cur INTO @ID, @RoleID;
    END
    CLOSE cur;
    DEALLOCATE cur;
END

GO
/****** Object:  StoredProcedure [dbo].[P_CalculateValidRole]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	计算有效的省代、总代
-- =============================================
CREATE PROCEDURE [dbo].[P_CalculateValidRole]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ID uniqueidentifier;
	DECLARE @RoleID INT;
	DECLARE @Quantity INT;

    --1.取会员
    DECLARE cur CURSOR FOR
    SELECT ID, RoleID
    FROM Members;
    OPEN cur;
    FETCH NEXT FROM cur INTO @ID, @RoleID;
    WHILE(@@FETCH_STATUS = 0)
    BEGIN
		SELECT @Quantity=ISNULL(SUM(Quantity),0) FROM Orders WHERE MemberID=@ID AND IsDeliverly=1;
		IF(@Quantity>=527)
		BEGIN
			UPDATE Members SET ProvinceAvailable=1, GeneralAvailable=1 WHERE ID=@ID;
		END
		ELSE IF(@Quantity>=167)
		BEGIN
			UPDATE Members SET ProvinceAvailable=1 WHERE ID=@ID;
		END
		FETCH NEXT FROM cur INTO @ID, @RoleID;
    END
    CLOSE cur;
    DEALLOCATE cur;
    UPDATE Members SET ProvinceAvailable=1, GeneralAvailable=1 WHERE RoleID=8;
END

GO
/****** Object:  StoredProcedure [dbo].[P_CleanUpParentChild]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	????????
-- =============================================
CREATE PROCEDURE [dbo].[P_CleanUpParentChild]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @ID UNIQUEIDENTIFIER;
	DECLARE @RoleID INT;
	DECLARE cur CURSOR FOR
	SELECT ID, RoleID FROM Members WHERE ValidRole=1;
	OPEN cur;
	FETCH NEXT FROM cur INTO @ID, @RoleID;
	WHILE(@@FETCH_STATUS=0)
	BEGIN
		IF(@RoleID=6)
		BEGIN
			UPDATE Members SET ProvinceAvailable=1 WHERE ID=@ID;
		END
		ELSE IF(@RoleID>6)
		BEGIN
			UPDATE Members SET ProvinceAvailable=1, GeneralAvailable=1 WHERE ID=@ID;
		END
		FETCH NEXT FROM cur INTO @ID, @RoleID;	
	END;
	CLOSE cur;
	DEALLOCATE cur;
	
	--更新公司直属下级
	UPDATE Members
	SET ParentMemberID=NULL
	WHERE ParentMemberID='8EC4AFC3-4410-41AE-986B-8EB7A417ACEE';
	
	--删除'康德宁'用户
	DELETE FROM Orders WHERE MemberID='69ABABC0-37AA-4223-98AA-D46363596F61'
	DELETE FROM Members WHERE ID='69ABABC0-37AA-4223-98AA-D46363596F61'
	DELETE FROM Users WHERE MemberID='69ABABC0-37AA-4223-98AA-D46363596F61'
	DELETE FROM ParentChild;
END

GO
/****** Object:  StoredProcedure [dbo].[P_DirectorCount]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	董事数量
-- =============================================
create PROCEDURE [dbo].[P_DirectorCount]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE Members
	SET DirectorCount=1
	WHERE RoleID=8
	AND ISNULL(DirectorCount,0)=0
END

GO
/****** Object:  StoredProcedure [dbo].[P_GetAllParents]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	获取所有上级
-- =============================================
CREATE PROCEDURE [dbo].[P_GetAllParents]
	-- Add the parameters for the stored procedure here
	@MemberId UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--获取所有上级会员
	WITH SubMembers(ID,RoleID,ParentMemberID,Level)AS
	(SELECT ID,RoleID,ParentMemberID,0 AS Level FROM Members
	WHERE ID=@MemberId
	UNION ALL
	SELECT a.ID,a.RoleID,a.ParentMemberID,b.Level+1 as Level FROM Members a
	INNER JOIN SubMembers b
	ON a.ID=b.ParentMemberID)
	SELECT ID FROM SubMembers order by [Level]
END

GO
/****** Object:  StoredProcedure [dbo].[P_GetAllSubMembers]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	获取底下所有会员
-- =============================================
CREATE PROCEDURE [dbo].[P_GetAllSubMembers]
(
	-- Add the parameters for the stored procedure here
	@MemberID UNIQUEIDENTIFIER,
	@MobileOrName VARCHAR(100)='',
	@LevelID INT = -1
)
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--declare @MemberID UNIQUEIDENTIFIER
	--set @MemberID='0E91683A-09DC-49E4-A052-3548351271A1';
	--declare @MobileOrName VARCHAR(100)
	--set @MobileOrName='1232232232';
	--declare @LevelID int;
	--set @LevelID = 2;
	--print(@MobileOrName+'dddd');
	IF(@LevelID = -1)
	BEGIN
		WITH SubMembers(ID,Mobile,MemberName,ParentMemberID,RoleID,CreateOn,[Address],LevelID)AS
		(
		SELECT ID,Mobile,MemberName,ParentMemberID,RoleID,CreateOn,[Address],0 AS LevelID FROM Members
		WHERE ID=@MemberID
		UNION ALL
		SELECT a.ID,a.Mobile,a.MemberName,a.ParentMemberID,a.RoleID,a.CreateOn,a.[Address],b.LevelID+1 
		FROM Members a 
		INNER JOIN SubMembers b
		ON a.ParentMemberID=b.ID)
		SELECT * FROM SubMembers where ID<>@MemberID
		AND (Mobile LIKE '%'+@MobileOrName+'%' OR MemberName LIKE '%'+@MobileOrName+'%')
		ORDER BY LevelID;
	END
    ELSE
    BEGIN
		WITH SubMembers(ID,Mobile,MemberName,ParentMemberID,RoleID,CreateOn,[Address],LevelID)AS
		(
		SELECT ID,Mobile,MemberName,ParentMemberID,RoleID,CreateOn,[Address],0 AS LevelID FROM Members
		WHERE ID=@MemberID
		UNION ALL
		SELECT a.ID,a.Mobile,a.MemberName,a.ParentMemberID,a.RoleID,a.CreateOn,a.[Address],b.LevelID+1 
		FROM Members a 
		INNER JOIN SubMembers b
		ON a.ParentMemberID=b.ID)
		SELECT * FROM SubMembers where ID<>@MemberID
		AND (Mobile LIKE '%'+@MobileOrName+'%' OR MemberName LIKE '%'+@MobileOrName+'%')
		AND LevelID = @LevelID
		ORDER BY LevelID;
    END
END

GO
/****** Object:  StoredProcedure [dbo].[P_GetFirstDirector]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	获取第一个董事(上个月或上个月之前上董事)
-- =============================================
CREATE PROCEDURE [dbo].[P_GetFirstDirector]
	-- Add the parameters for the stored procedure here
	@MemberId UNIQUEIDENTIFIER,
	@OrderDate DATETIME = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF(@OrderDate IS NULL)
	BEGIN
		SET @OrderDate = GETDATE();
	END
	
	DECLARE @StartOfMonth DATETIME;
	
	SELECT @StartOfMonth = DATEADD(month, DATEDIFF(month, 0, @OrderDate), 0);

     --获取所有上级会员
	WITH SubMembers(ID,RoleID,GeneralAvailable,ParentMemberID,DirectorDate,Level)AS
	(SELECT ID,RoleID,GeneralAvailable,ParentMemberID,DirectorDate,0 AS Level FROM Members
	WHERE ID=@MemberId
	UNION ALL
	SELECT a.ID,a.RoleID,a.GeneralAvailable,a.ParentMemberID,a.DirectorDate,b.Level+1 as Level FROM Members a
	INNER JOIN SubMembers b
	ON a.ID=b.ParentMemberID)
	SELECT ID FROM SubMembers where ISNULL(DirectorDate,GETDATE())<@StartOfMonth AND RoleID=8 AND GeneralAvailable=1 order by [Level];--获取上面三个总代(或董事)
END

GO
/****** Object:  StoredProcedure [dbo].[P_GetFirstHigherAgentMember]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[P_GetFirstHigherAgentMember]
	-- Add the parameters for the stored procedure here
	@MemberID UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @RoleID INT;
	SET @RoleID=0;
	SELECT @RoleID=RoleID FROM Members WHERE ID=@MemberID;

	DECLARE @FirstHigherAgencyID UNIQUEIDENTIFIER;
    SET @FirstHigherAgencyID=NULL;
		--查我上面第一个级别比我高的会员
		WITH SubMembers(ID,ParentMemberID,MemberName,Mobile,RoleID,LevelID) AS
		(
			SELECT ID,ParentMemberID,MemberName,Mobile,RoleID,0 AS LevelID FROM Members
			WHERE ID=@MemberID
			UNION ALL
			SELECT Members.ID,Members.ParentMemberID,Members.MemberName,Members.Mobile,Members.RoleID,SubMembers.LevelID+1 FROM Members
			INNER JOIN SubMembers ON Members.ID=SubMembers.ParentMemberID
		)
		SELECT * FROM Members WHERE ID IN(
		SELECT TOP 1 ID FROM SubMembers
		WHERE RoleID>@RoleID
		ORDER BY LevelID ASC)
END

GO
/****** Object:  StoredProcedure [dbo].[P_GetHighSubMembers]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	获取底下所有会员
-- =============================================
CREATE PROCEDURE [dbo].[P_GetHighSubMembers]
(
	-- Add the parameters for the stored procedure here
	@MemberID UNIQUEIDENTIFIER,
	@RoleID INT,
	@MobileOrName VARCHAR(100)=''
)
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--declare @MemberID UNIQUEIDENTIFIER
	--set @MemberID='0E91683A-09DC-49E4-A052-3548351271A1';
	--declare @MobileOrName VARCHAR(100)
	--set @MobileOrName='1232232232';
	--print(@MobileOrName+'dddd');
	
    WITH SubMembers(ID,Mobile,MemberName,ParentMemberID,RoleID,CreateOn,[Address],LevelID)AS
    (
    SELECT ID,Mobile,MemberName,ParentMemberID,RoleID,CreateOn,[Address],0 AS LevelID FROM Members
    WHERE ID=@MemberID
    UNION ALL
    SELECT a.ID,a.Mobile,a.MemberName,a.ParentMemberID,a.RoleID,a.CreateOn,a.[Address],b.LevelID+1 
    FROM Members a 
    INNER JOIN SubMembers b
    ON a.ParentMemberID=b.ID)
    SELECT * FROM SubMembers where ID<>@MemberID
    AND RoleID>=@RoleID
    AND (Mobile LIKE '%'+@MobileOrName+'%' OR MemberName LIKE '%'+@MobileOrName+'%')
    ORDER BY LevelID;
END


GO
/****** Object:  StoredProcedure [dbo].[P_GetThreeGeneralAgent]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	获取三个总代(或董事)
-- =============================================
CREATE PROCEDURE [dbo].[P_GetThreeGeneralAgent]
	-- Add the parameters for the stored procedure here
	@MemberId UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    --获取所有上级会员
	WITH SubMembers(ID,RoleID,GeneralAvailable,ParentMemberID,Level)AS
	(SELECT ID,RoleID,GeneralAvailable,ParentMemberID,0 AS Level FROM Members
	WHERE ID=@MemberId
	UNION ALL
	SELECT a.ID,a.RoleID,a.GeneralAvailable,a.ParentMemberID,b.Level+1 as Level FROM Members a
	INNER JOIN SubMembers b
	ON a.ID=b.ParentMemberID)
	SELECT top 3 ID FROM SubMembers where ID<>@MemberId AND RoleID>=7 AND GeneralAvailable=1 order by [Level];--获取上面三个总代(或董事)
END

GO
/****** Object:  StoredProcedure [dbo].[P_InsertCityAgentParentChild]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	更新市代上下级关系
-- =============================================
CREATE PROCEDURE [dbo].[P_InsertCityAgentParentChild]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRAN;
    -- Insert statements for procedure here
    
    DECLARE @MemberID UNIQUEIDENTIFIER; -- 代理ID
    DECLARE @RoleID INT; -- 代理角色ID
    DECLARE @Quantity INT; -- 总数量
    DECLARE @GeneralAvailable INT; -- 有效总代
    DECLARE @ProvinceAvailable INT; -- 有效省代
    
	-- 查找所有市代以上的代理
	DECLARE sor CURSOR FOR
	SELECT ID, RoleID, TotalCount, ProvinceAvailable, GeneralAvailable FROM Members WHERE RoleID >= 5;
	OPEN sor;
	FETCH NEXT FROM sor INTO @MemberID, @RoleID, @Quantity, @ProvinceAvailable, @GeneralAvailable;
	WHILE(@@FETCH_STATUS=0)
	BEGIN
		IF(@GeneralAvailable > 0)
		BEGIN
			-- 按总代更新父子关系表
			EXEC P_InsertParentChild @MemberId = @MemberID, @RoleID = 7;
		END
		ELSE IF(@ProvinceAvailable > 0)
		BEGIN
			-- 按省代更新父子关系表
			EXEC P_InsertParentChild @MemberId = @MemberID, @RoleID = 6;
		END
		ELSE
		BEGIN
			IF(@Quantity >= 47)
			BEGIN
				-- 按市代更新父子关系表
				EXEC P_InsertParentChild @MemberId = @MemberID, @RoleID = 5;
			END
		END
		
		FETCH NEXT FROM sor INTO @MemberID, @RoleID, @Quantity, @ProvinceAvailable, @GeneralAvailable;
	END
	CLOSE sor;
	DEALLOCATE sor;
	
	COMMIT TRAN;
END


GO
/****** Object:  StoredProcedure [dbo].[P_InsertDirector]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	添加记录到Director表
-- =============================================
create PROCEDURE [dbo].[P_InsertDirector]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @MemberID UNIQUEIDENTIFIER;
	DECLARE @DirectorDate DATETIME;
    DECLARE cur CURSOR FOR
    SELECT ID,DirectorDate FROM Members WHERE RoleID=8;
    OPEN cur;
    FETCH NEXT FROM cur INTO @MemberID,@DirectorDate;
    WHILE(@@FETCH_STATUS=0)
    BEGIN
    INSERT INTO Director(ID,MemberID,DirectorNo,CreateOn) VALUES
    (NEWID(),@MemberID,1,@DirectorDate)
    FETCH NEXT FROM cur INTO @MemberID,@DirectorDate;
    END
    CLOSE cur;
    DEALLOCATE cur;
END

GO
/****** Object:  StoredProcedure [dbo].[P_InsertParentChild]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	获取所有上级会员，并插入ParentChild表
-- =============================================
CREATE PROCEDURE [dbo].[P_InsertParentChild]
	-- Add the parameters for the stored procedure here
	@MemberId UNIQUEIDENTIFIER,
	@RoleId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ID UNIQUEIDENTIFIER;
	DECLARE @ParentMemberID UNIQUEIDENTIFIER;
	DECLARE @Count INT;
	DECLARE @ProvinceAgentCount INT;
	DECLARE @GeneralAgentCount INT;
	DECLARE @CityAgentCount INT; -- 市代理
	IF(@RoleId=5) -- 市代理
	BEGIN
		SET @ProvinceAgentCount=0;
		SET @GeneralAgentCount=0;
		SET @CityAgentCount=1;
	END
	ELSE IF(@RoleId=6)
	BEGIN
		SET @ProvinceAgentCount=1;
		SET @GeneralAgentCount=0;
		SET @CityAgentCount=1;
	END
	ELSE IF(@RoleId>6)
	BEGIN
		SET @ProvinceAgentCount=1;
		SET @GeneralAgentCount=1;
		SET @CityAgentCount=1;
	END
	ELSE
	BEGIN
		RETURN;
	END
	CREATE TABLE #t
	(
		ID UNIQUEIDENTIFIER,
		RoleID INT,
		ParentMemberID UNIQUEIDENTIFIER,
		[Level] INT
	);
    -- Insert statements for procedure here
	--获取所有上级会员
	WITH SubMembers(ID,RoleID,ParentMemberID,Level)AS
	(SELECT ID,RoleID,ParentMemberID,0 AS Level FROM Members
	WHERE ID=@MemberId
	UNION ALL
	SELECT a.ID,a.RoleID,a.ParentMemberID,b.Level+1 as Level FROM Members a
	INNER JOIN SubMembers b
	ON a.ID=b.ParentMemberID)
	INSERT INTO #t SELECT * FROM SubMembers order by [Level]
	DECLARE cur CURSOR FOR
	SELECT ID,ParentMemberID FROM #t 
	WHERE ID<>'8EC4AFC3-4410-41AE-986B-8EB7A417ACEE'--公司
	AND ParentMemberID IS NOT NULL;
	OPEN cur;
	FETCH NEXT FROM cur INTO @ID,@ParentMemberID;
	WHILE @@FETCH_STATUS=0
	BEGIN
		SELECT @Count=COUNT(1) FROM ParentChild WHERE ParentMemberID=@ParentMemberID
		AND ChildMemberID=@ID
		IF(@Count=0)
		BEGIN
			INSERT INTO ParentChild VALUES(NEWID(),@ParentMemberID,@ID,@ProvinceAgentCount,
			@GeneralAgentCount,NULL,GETDATE(),NULL,NULL, @CityAgentCount)
		END
		ELSE
		BEGIN
			UPDATE ParentChild
			SET ProvinceAgentCount=ISNULL(ProvinceAgentCount,0)+@ProvinceAgentCount,
				GeneralAgentCount=ISNULL(GeneralAgentCount,0)+@GeneralAgentCount,
				CityAgentCount=ISNULL(CityAgentCount,0)+@CityAgentCount
			WHERE ParentMemberID=@ParentMemberID
			AND ChildMemberID=@ID;
		END
		FETCH NEXT FROM cur INTO @ID,@ParentMemberID;
	END
	CLOSE cur;
	DEALLOCATE cur;
	DROP TABLE #t;
END

GO
/****** Object:  StoredProcedure [dbo].[P_UpdateAvailabledGeneralOrder]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	更新总代或董事向公司进货的订单，设置上面三个总代或董事
-- =============================================
CREATE PROCEDURE [dbo].[P_UpdateAvailabledGeneralOrder]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	CREATE TABLE #t
	(
		ID UNIQUEIDENTIFIER
	)

    -- Insert statements for procedure here
    DECLARE @Count INT;
    DECLARE @TempMemberID UNIQUEIDENTIFIER;
    DECLARE @MemberID UNIQUEIDENTIFIER;
    DECLARE @OrderID UNIQUEIDENTIFIER;
	DECLARE cur CURSOR for
	SELECT ID,MemberID FROM Orders
	WHERE MemberID IN(
	SELECT ID FROM Members 
	WHERE RoleID>=7 
	AND GeneralAvailable>=1 
	)
	AND SendMemberID='00000000-0000-0000-0000-000000000000'
	AND IsDeliverly = 1
	;
	OPEN cur;
	FETCH NEXT FROM cur INTO @OrderID,@MemberID;
	WHILE(@@FETCH_STATUS=0)
	BEGIN
		DELETE #t;
		INSERT INTO #t EXEC P_GetThreeGeneralAgent @MemberID;
		SELECT @Count=COUNT(1) FROM #t;
		IF(@Count>0)
		BEGIN
			SELECT TOP 1 @TempMemberID=ID FROM #t;
			UPDATE Orders
			SET GeneralAgent1ID = @TempMemberID
			WHERE ID=@OrderID;
		END
		
		DELETE #t WHERE ID=@TempMemberID;
		SELECT @Count=COUNT(1) FROM #t;
		IF(@Count>0)
		BEGIN
			SELECT TOP 1 @TempMemberID=ID FROM #t;
			UPDATE Orders
			SET GeneralAgent2ID = @TempMemberID
			WHERE ID=@OrderID;
		END
		
		DELETE #t WHERE ID=@TempMemberID;
		SELECT @Count=COUNT(1) FROM #t;
		IF(@Count>0)
		BEGIN
			SELECT TOP 1 @TempMemberID=ID FROM #t;
			UPDATE Orders
			SET GeneralAgent3ID = @TempMemberID
			WHERE ID=@OrderID;
		END
		FETCH NEXT FROM cur INTO @OrderID,@MemberID;
	END
	CLOSE cur;
	DEALLOCATE cur;
	DROP TABLE #t;
END

GO
/****** Object:  StoredProcedure [dbo].[P_UpdateCityAgentMinus]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	批量更新市代优惠数量
-- =============================================
CREATE PROCEDURE [dbo].[P_UpdateCityAgentMinus]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @Count INT;

    -- Insert statements for procedure here
    DECLARE @MemberID UNIQUEIDENTIFIER;
    DECLARE @RoleID INT;
	DECLARE cur CURSOR FOR
	SELECT ID, RoleID FROM Members
	OPEN cur;
	FETCH NEXT FROM cur INTO @MemberID, @RoleID;
	WHILE(@@FETCH_STATUS=0)
	BEGIN
		SELECT @Count = COUNT(1) FROM ParentChild
		WHERE ParentMemberID = @MemberID
		AND ISNULL(CityAgentCount,0) > 0;
		
		IF(@Count=3)
		BEGIN
			UPDATE Members
			SET CityMinus = 15
			WHERE ID = @MemberID;
		END
		ELSE IF(@Count=2)
		BEGIN
			UPDATE Members
			SET CityMinus = 10
			WHERE ID = @MemberID;
		END
		ELSE IF(@Count=1)
		BEGIN
			UPDATE Members
			SET CityMinus = 5
			WHERE ID = @MemberID;
		END
		
		FETCH NEXT FROM cur INTO @MemberID, @RoleID;
	END
	CLOSE cur;
	DEALLOCATE cur;
	
END

GO
/****** Object:  StoredProcedure [dbo].[P_UpdateCodes]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	更新识别码
-- =============================================
CREATE PROCEDURE [dbo].[P_UpdateCodes]
	-- Add the parameters for the stored procedure here
(
	@orderId UNIQUEIDENTIFIER -- 订单ID
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRAN;
    
    DECLARE @odId UNIQUEIDENTIFIER;
    DECLARE @productId INT;
    
    DECLARE cur CURSOR FOR
    SELECT ID, ProductID FROM OrderDetails WHERE OrderID = @orderId;
    OPEN cur;
    FETCH NEXT FROM cur INTO @odId, @productId;
    WHILE(@@FETCH_STATUS=0)
    BEGIN
		UPDATE IdentityCode
		SET [Status] = 2 -- 不可用
		WHERE [Status] = 1 -- 不可用，但还没发货
		AND OrderDetailsID IN(
			SELECT ID FROM OrderDetails WHERE ProductID = @productId
		)
		FETCH NEXT FROM cur INTO @odId, @productId;
    END
    CLOSE cur;
    DEALLOCATE cur;
    
    
    
    COMMIT TRAN;
END


GO
/****** Object:  StoredProcedure [dbo].[P_UpdateDirectorDate]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	更新董事日期
-- =============================================
CREATE PROCEDURE [dbo].[P_UpdateDirectorDate]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--三月份，徐剑平，沈建煜
	UPDATE Members 
	SET DirectorDate='2017-03-15'
	WHERE ID IN('22B86383-283E-4143-BD0C-565EE1EC11AD','6AC63510-7EC5-49B3-A798-B9BEDC242FB4');
	
	--四月份，沈贡云，曹玉丽
	UPDATE Members 
	SET DirectorDate='2017-04-15'
	WHERE ID IN('D6850756-69A5-47C7-9C87-4337BB297F72','BFE3BBC7-DEFF-4A6C-A6FC-C2D91B6935F2');
	
	--五月份，陆志城，沈金奎，沈斌，沈建芳
	UPDATE Members 
	SET DirectorDate='2017-05-15'
	WHERE ID IN('4ECAE8B8-0101-44F1-AF45-70925DE5AE75','5D5EF8D2-1EEF-457C-9B4E-099816AFC7FB',
	'B4FC6829-8CF1-4031-BC1C-38E2F71FAD6D','74055C75-7951-42D0-A4D7-4AFD44F8DBFB');
END

GO
/****** Object:  StoredProcedure [dbo].[P_UpdateOrderForDirector]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	更新总代或董事向公司进货的订单，设置获奖的董事
-- =============================================
CREATE PROCEDURE [dbo].[P_UpdateOrderForDirector]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    -- Insert statements for procedure here
    DECLARE @Count INT;
    DECLARE @MemberID UNIQUEIDENTIFIER;
    DECLARE @DirectorID UNIQUEIDENTIFIER;
    DECLARE @OrderID UNIQUEIDENTIFIER;
    DECLARE @SendDate DATETIME;
    CREATE TABLE #t
	(
		ID UNIQUEIDENTIFIER
	)
	DECLARE cur CURSOR for
	SELECT ID,MemberID,SendDate FROM Orders
	WHERE IsDeliverly = 1
	AND SendMemberID='00000000-0000-0000-0000-000000000000';
	OPEN cur;
	FETCH NEXT FROM cur INTO @OrderID,@MemberID,@SendDate;
	WHILE(@@FETCH_STATUS=0)
	BEGIN
		DELETE #t;
		INSERT INTO #t EXEC P_GetFirstDirector @MemberID,@SendDate;
		SELECT @Count=COUNT(1) FROM #t;
		IF(@Count>0)
		BEGIN
			SELECT TOP 1 @DirectorID=ID FROM #t;
			IF(@DirectorID<>'00000000-0000-0000-0000-000000000000')
			BEGIN
				UPDATE Orders
				SET DirectorID = @DirectorID
				WHERE ID=@OrderID;
			END
		END
		
		FETCH NEXT FROM cur INTO @OrderID,@MemberID, @SendDate;
	END
	CLOSE cur;
	DEALLOCATE cur;
	DROP TABLE #t;
END

GO
/****** Object:  StoredProcedure [dbo].[P_UpdateOrderSendDate]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	更新订单发货日期
-- =============================================
create PROCEDURE [dbo].[P_UpdateOrderSendDate]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE Orders SET SendDate=CreateOn
END

GO
/****** Object:  Table [dbo].[MemberRoles]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MemberRoles](
	[ID] [int] NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
	[RoleRiseDescription] [nvarchar](max) NULL,
	[AllowedDirectOrder] [bit] NOT NULL DEFAULT ((0)),
	[Price] [int] NOT NULL DEFAULT ((0)),
	[CreateBy] [int] NULL,
	[CreateOn] [datetime] NULL,
	[UpdateBy] [int] NULL,
	[UpdateOn] [datetime] NULL,
	[Total] [int] NOT NULL DEFAULT ((0)),
	[OneTimeAmount] [int] NOT NULL DEFAULT ((0)),
	[UpgradeCount] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_dbo.MemberRoles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Products]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[ImgSrc] [nvarchar](max) NULL,
	[ImgAlt] [nvarchar](max) NULL,
	[HasIdentityCode] [bit] NOT NULL,
	[CreateBy] [int] NULL,
	[CreateOn] [datetime] NULL,
	[UpdateBy] [int] NULL,
	[UpdateOn] [datetime] NULL,
 CONSTRAINT [PK_dbo.Products] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SystemConfig]    Script Date: 11/11/2017 10:15:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemConfig](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[ConfigValue] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](200) NULL,
	[EntityStatus] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SystemConfig] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
