
set nocount off;

declare @Mobile varchar(20) = '13666666666';
declare @UserRoleID int = 3;
declare @Password varchar(100) = 'X_SECRET_gAQI9lDs4duU8JwJcNLFSQ==';
declare @UserName varchar(20) = '财务人员';
declare @MemberID uniqueidentifier = newid();

begin transaction;

if not exists(select 1 from Members where Mobile='13666666666') 
begin

if object_id('TEMPDB..#MemberTable') is not null
begin
	drop table DBO.#MemberTable
end

select * into #MemberTable from Members
where ID='00000000-0000-0000-0000-000000000000';

update #MemberTable
set ID=@MemberID,
Mobile=@Mobile;

insert into Members
select * from #MemberTable;

if object_id('TEMPDB..#UserTable') is not null
begin
	drop table DBO.#UserTable
end

select * into #UserTable from Users
where Account='18888888888';

update #UserTable
set ID=newid(),
MemberID=@MemberID,
Account=@Mobile,
UserRoleID=@UserRoleID,
UserName=@UserName,
[Password]=@Password;

insert into Users
select * from #UserTable;
end
commit transaction;