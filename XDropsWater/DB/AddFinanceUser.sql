declare @memberId uniqueidentifier=newid();
  insert into Members (ID, Mobile, MemberName, RoleID, PreviousRoleID, PreviousRoleQuantity, TotalQuantity, 
  ChildTotalQuantity, CurrentRoleQuantity, ValidRole, ProvinceAvailable, GeneralAvailable, TotalCount, 
  DirectorCount, CurrentRoleAmount, TotalAmount, CityMinus)
  values(@memberId, '13666666666', N'公司', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
  insert into Users (ID, MemberID, UserName, Account, [Password], UserRoleID)
  values(newid(), @memberId, N'公司', '13666666666', 'X_SECRET_gAQI9lDs4duU8JwJcNLFSQ==',
  3)