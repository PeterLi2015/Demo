using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Attributes;
using XDropsWater.Dal.Entity;
using XDropsWater.DataAccess.Interface;
using XDropsWater.Model;

namespace XDropsWater.Bll.Interface
{
    public interface IMemberService : IService
    {
        //[Dependency]
        //IUnitOfWork Uow { get; set; }
        //[Dependency]
        //IRepository<MemberEntity> MemDb { get; set; }
        ErrorCodes Add(MemberModel model);
        ErrorCodes Update(MemberModel model);
        MemberModel Get(string mobile);
        bool CheckIDCard(string cardNo);
        List<MemberModel> Get(string idCardNo, string name, ref int total, int page = 1, int rows = 10);
        string UpdateMemberForCheck(MemberModel model);

        ErrorCodes CheckedByOperator(Guid memberId);

        List<MemberModel> GetMemberForCheck(Guid createBy, string idCardNo, int hasChecked, ref int total, int page = 1, int rows = 10);

        List<MemberModel> GetMemberForHighNumber(string memberName, string idCardNo, ref int total, int page = 1, int rows = 10);

        string AddHeighNumber(string cardNo, string memberName,
            string bankName, string bankCardNo, string parentCardNo, string recommendCardNo, int cardLevelId, int agencyLevelId);

        /// <summary>
        /// 添加代理
        /// </summary>
        /// <param name="model"></param>
        void AddMember(Member model);

        /// <summary>
        /// 添加代理
        /// </summary>
        /// <param name="model"></param>
        void AddMember1(Member model);

        string AddChildMemberOrder(string mobile, string quantity);

        string AddMemberOrder(string mobile, string quantity);

        void UpdateMember(AddMemberModel model);

        void UpdateChildMember(AddMemberModel model);

        void UpdateCurrentRoleStock(AddMemberModel model);

        string UpdateHeighNumber(Guid memberId, string cardNo, string memberName,
            string bankName, string bankCardNo, string parentCardNo, string recommendCardNo, int cardLevelId);

        void UpdateOrderExpress(Guid orderId, string expressContent);

        void RemoveCode(Guid Id);

        /// <summary>
        /// 检查代理是否已经存在
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns>cardNoNotExists;cardNoExists;exception</returns>
        string IDCardExists(string cardNo);

        /// <summary>
        /// 检查代理身份证号码是否有效
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns>Yes有效IDCardInvalid无效Exception有异常</returns>
        string IDCardValidate(string cardNo);

        /// <summary>
        /// 检查是否新代理
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        string IsNewMember(string cardNo);

        /// <summary>
        /// 验证代理是新代理还是老代理，新代理只能在【新代理入单】处操作
        /// 老代理只能在【老代理补单】或【老代理个人消费】处操作
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="checkType"></param>
        /// <returns></returns>
        string CheckNewOrOldMember(string cardNo, int checkType);

        /// <summary>
        /// 添加不存在的代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ErrorCodes AddNoExistsMember(MemberModel model);

        /// <summary>
        /// 推荐的人能放在哪些代理的后面
        /// </summary>
        /// <param name="recommendCardNo"></param>
        /// <param name="total"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        List<MemberModel> GetRecommendMemberLocation(string recommendCardNo, ref int total, int page = 1, int rows = 10);

         /// <summary>
        /// 获取代理信息，用来修改专卖店账号
        /// </summary>
        /// <param name="createBy"></param>
        /// <param name="idCardNo"></param>
        /// <param name="hasChecked"></param>
        /// <param name="total"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        List<MemberModel> GetMemberForUpdateStore(string cardNo, string name, ref int total, int page = 1, int rows = 10);
        
        /// <summary>
        /// 修改代理专卖店
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        string UpdateStore(string cardNo, string account);

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        List<MemberModel> GetDepartInfo(string cardNo);

        List<MemberModel> GetMember(string search);

        MemberSummary GetMember1(int page, int size, string search);

        int GetMembersCount();

        int GetMemberCount();

        int GetSelfNewOrderCount();

        int GetNewOrderCount();

        string GetMemberName(string mobile);

        /// <summary>
        /// 获取代理订单
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="iSubmit"></param>
        /// <param name="total"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        List<MemberOrderModel> GetMemberOrder(string mobileOrName, bool? iSubmit);

        OrderSummary GetMemberOrder1(int page, int size, string mobileOrName, bool? isDelivery, enmOrderLevel orderLevel);

        IdentityCodeSummary GetCodePages(int page, int rows, Guid orderDetailsId);

        /// <summary>
        /// 添加唯一识别码
        /// </summary>
        /// <param name="model"></param>
        void AddCode(IdentityCode model);

        /// <summary>
        /// 按范围添加唯一识别码
        /// </summary>
        /// <param name="model"></param>
        void AddCodeRange(IdentityCode model);

        List<MemberModel> MemberOrderSearch(string mobileOrName, DateTime? dateFrom, DateTime? dateTo, int roleId);

        string UpdateMemberOrder(Guid orderId, string quantity);

        void RemoveMemberOrder(Guid orderId);

        string RemoveChildMemberOrder(Guid orderId);

        void SendMemberOrder(Guid orderId, string expressContent);

        List<MemberOrderModel> GetPersonalOrder(bool? isDeliverly);

        /// <summary>
        /// get personal orders
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="isDeliverly">if the order has been delivered</param>
        /// <returns></returns>
        OrderSummary GetPersonalOrder1(int page, int rows, bool? isDeliverly);

        void AddPersonalOrder(AddPersonalOrderModel model);
        void SavePersonalInfo(MemberModel model);

        /// <summary>
        /// 保存基本资料
        /// </summary>
        /// <param name="model"></param>
        void SaveBaseInfo(BaseInfo model);

        MemberModel GetPersonalInfo();

        /// <summary>
        /// 基本资料
        /// </summary>
        /// <returns></returns>
        BaseInfo GetPersonalInfo1();

        void RemoveMember(Guid memberId);

        /// <summary>
        /// remove member
        /// </summary>
        /// <param name="memberId"></param>
        void RemoveMember1(Guid memberId);

        void AddDirectChildMember(AddChildMemberModel model);

        /// <summary>
        /// add a member
        /// </summary>
        /// <param name="model">member object</param>
        void AddDirectChildMember1(AddUpdateMember model);

        /// <summary>
        /// get all products
        /// </summary>
        /// <returns></returns>
        ProductSummary GetProducts();

        /// <summary>
        /// 获取自己角色下的产品信息
        /// </summary>
        /// <returns></returns>
        ProductMemberRoleSummary GetProductMemberRole();

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="products"></param>
        void AddOrder(IEnumerable<ProductMemberRole> products);

        List<MemberModel> GetDirectChildMember(string mobileOrName);

        void ResetPassword(Guid memberId);

        void UpdatePersonalOrder(AddPersonalOrderModel model);

        IEnumerable<MemberRoleModel> GetMemberRole();

        void ChangePassword(ChangePasswordModel model);

        void ChangePassword1(UserPassword model);

        List<SubMemberModel> GetAllSubMembers(string mobileOrName, int levelId);

        /// <summary>
        /// 获取所有底下会员
        /// </summary>
        /// <param name="mobileOrName"></param>
        /// <param name="levelId"></param>
        /// <returns></returns>
        SubMemberSummary GetAllSubMembers1(int page, int rows, string mobileOrName, int levelId);

        List<SubMemberModel> GetHighSubMembers(string mobileOrName);

        SubMemberSummary GetHighSubMembers1(int page, int rows, string mobileOrName);

        /// <summary>
        /// 后台管理员修改代理
        /// </summary>
        /// <param name="model">要修改的代理对象</param>
        void UpdateChildMemberForAdmin(Member model);

        /// <summary>
        /// 后台管理员修改代理
        /// </summary>
        /// <param name="model">要修改的代理对象</param>
        void UpdateChildMemberForAdmin1(Member model);

        /// <summary>
        /// 更新上下级关系表
        /// </summary>
        void UpdateParentChild();

        /// <summary>
        /// 设置有效代理
        /// </summary>
        void SetAgentValidRole();

        /// <summary>
        /// 计算总进货量
        /// </summary>
        void CalculateTotalOrder();

        /// <summary>
        /// 更新董事
        /// </summary>
        void UpdateDirector();

        /// <summary>
        /// 清空上下级关系表
        /// </summary>
        void CleanUpParentChild();

        /// <summary>
        /// 更新上面订单上面三个总代
        /// </summary>
        void UpdateOrderGeneral();

        /// <summary>
        /// 更新董事日期
        /// </summary>
        void UpdateDirectorDate();

        /// <summary>
        /// 更新获奖的董事
        /// </summary>
        void UpdateAwardDirector();

        /// <summary>
        /// 更新订单发货日期
        /// </summary>
        void UpdateOrderSendDate();

        /// <summary>
        /// 计算50，30，20
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        List<Member503020> Calculate503020(string mobileOrName, DateTime? dateFrom, DateTime? dateTo);

        List<DetailsModel> Details503020(int detailsType, Guid memberId, DateTime? dateFrom, DateTime? dateTo);

        List<DirectorBonusModel> CalculateDirectorBonus(string mobileOrName, string yearMonth);

        int CalcualteCompanyBonusMemberCount(string yearMonth);

        /// <summary>
        /// 根据电话号码获取代理对象
        /// </summary>
        /// <param name="mobile">电话号码</param>
        /// <returns>代理对象</returns>
        Member GetMemberByMobile(string mobile);

        /// <summary>
        /// 获取代理订单
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页显示行数</param>
        /// <param name="mobileOrName">查询条件：手机号码或姓名</param>
        /// <param name="isDelivery">查询条件：是否发货</param>
        /// <param name="orderLevel">订单级别：公司发货的订单，代理发货的订单</param>
        /// <returns></returns>
        MemberSummary GetDirectChildMember1(int page, int size, string mobileOrName);

        /// <summary>
        /// 购物车
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        ShoppingCartSummary ShoppingCart(int page, int size);

        void AddUpdateShoppingCart(ShoppingCart model);

        void RemoveShoppingCart(Guid ID);

        void AddOrder1();

        OrderDetailsSummary OrderDetails(Guid orderId, int page, int size);

        /// <summary>
        /// 更新订单明细
        /// </summary>
        /// <param name="model"></param>
        void AddUpdateOrderDetails(OrderDetails model);

        /// <summary>
        /// 删除订单明细
        /// </summary>
        /// <param name="ID"></param>
        void RemoveOrderDetails(Guid ID);

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="orderId"></param>
        void RemoveOrder(Guid orderId);

        /// <summary>
        /// 保存订单
        /// </summary>
        void SaveOrder(decimal amount);

        /// <summary>
        /// 获取唯一识别码
        /// </summary>
        /// <param name="orderDetailsId"></param>
        /// <returns></returns>
        IdentityCodeSummary NewIdentityCode(Guid orderDetailsId, int page, int rows);

        /// <summary>
        /// 添加唯一识别码
        /// </summary>
        /// <returns></returns>
        void AddNewIdentityCode(Guid orderDetailsId, long codeFrom, long codeTo);

        /// <summary>
        /// 删除所有识别码
        /// </summary>
        /// <param name="orderDetailsId"></param>
        void RemoveAllCode(Guid orderDetailsId);

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="expressContent"></param>
        void SendMemberOrder1(Guid orderId, string expressContent);

        /// <summary>
        /// 确认收款
        /// </summary>
        /// <param name="orderId"></param>
        void FinancialConfirm(Guid orderId);

        /// <summary>
        /// 获取我的库存
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        MemberProductSummary MyStock(int page, int rows);

        /// <summary>
        /// 获取代理库存
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        MemberProductSummary MemberStock(Guid memberId, int page, int rows);

        /// <summary>
        /// 保存库存
        /// </summary>
        /// <param name="memberProductId"></param>
        /// <param name="quantity"></param>
        void SaveStock(Guid memberProductId, int quantity);

        /// <summary>
        /// 获取产品识别码
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        IdentityCodeSummary GetCodes(int productId, int page, int rows);

        /// <summary>
        /// 获取订单详细信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        string GetOrderDetails(Guid orderId);

        /// <summary>
        /// 获取快递信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="status"></param>
        /// <param name="mobileOrName"></param>
        /// <returns></returns>
        ExpressSummary GetExpress(int page, int size, int status, string mobileOrName, bool recieve);

        /// <summary>
        /// 新增修改快递信息
        /// </summary>
        /// <param name="express"></param>
        void AddUpdateExpress(Express express);

        /// <summary>
        /// 新发货记录
        /// </summary>
        /// <returns></returns>
        int GetNewExpress();

        /// <summary>
        /// 删除发货记录
        /// </summary>
        /// <param name="id"></param>
        void RemoveExpress(Guid id);

        void BulkAddCode(Guid orderDetailsId, IEnumerable<long> codeList);

        /// <summary>
        /// 获取要修改的识别码
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="mobileOrName"></param>
        /// <param name="code"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        //UpdateCodeSummary GetUpdateCodes(int page, int size, string mobileOrName = "", long code = -1, int productId = -1);

        /// <summary>
        /// 修改识别码
        /// </summary>
        /// <param name="newCode">新识别码</param>
        /// <param name="oldCode">老识别码</param>
        /// <param name="productId">产品ID</param>
        void UpdateCode(long newCode, long oldCode, int productId);

        /// <summary>
        /// 根据时间获取总代订单
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        GeneralOrderSummary GeneralOrder(int page, int size, DateTime? dateFrom, DateTime? dateTo);

    }
}
