/****** Script for SelectTopNRows command from SSMS  ******/
insert into XDropsWater.dbo.MemberProductCode
SELECT newid() as ID, mp.ID as MemberProductID, a.Code as Code
--, a.Code, b.ProductID, c.MemberID
, null as CreateBy, null as CreateOn, null as UpdateBy, null as UpdateOn
  FROM XDropsWater.[dbo].[IdentityCode] a
  inner join XDropsWater.dbo.OrderDetails b
  on a.OrderDetailsID=b.ID
  inner join XDropsWater.dbo.Orders c
  on b.OrderID=c.ID
  inner join XDropsWater.dbo.MemberProduct mp
  on mp.MemberID=c.MemberID and mp.ProductID=b.ProductID
  where a.[Status]=0
