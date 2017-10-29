1. 会员表，会员角色表加数据。
2. 系统配置表加数据。
3. 产品表加数据。
4. 批量插入产品库存到会员产品表中， 包括更新会员总金额， 当前角色总金额。
   P_BulkUpdateMemberProduct
5. 批量更新上下级关系
   P_InsertCityAgentParentChild
6. 批量更新当前市代优惠数量
   P_UpdateCityAgentMinus
7. 计算省代，总代总金额时要减去市代优惠数量。(Done)
8. 修改代理，有效省代，有效总代时要更新董事。(To Do)
9. 清除Member表的TotalCount约束