USE [Maka2]
GO
/****** Object:  StoredProcedure [dbo].[P_GetAllSubMembers]    Script Date: 11/10/2016 01:47:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	获取底下所有会员
-- =============================================
ALTER PROCEDURE [dbo].[P_GetAllSubMembers]
(
	-- Add the parameters for the stored procedure here
	@MemberID UNIQUEIDENTIFIER,
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
    AND (Mobile LIKE '%'+@MobileOrName+'%' OR MemberName LIKE '%'+@MobileOrName+'%')
    ORDER BY LevelID;
END
