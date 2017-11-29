
//订单明细
var orderDetailsRows = new Vue({
    el: '#orderDetailsRows',
    data: {
        items: [],
        header: {
            OrderNo: '',
            Description: ''
        },
        error: {
            show: false,
            message: ''
        },
        displayPages: [],
        totalPages: 0,
        currentPage: 1,
        totalCount: 0,
        rowFrom: 0,
        rowTo: 0
    },
    mounted: function () {
        this.getPages(1)
    },
    methods: {
        getPages: function (page) {
            getOrderDetails(this, page);
        },
        redirect: function (item) {
            sessionStorage.OrderDetailsID = item.ID;
            sessionStorage.ProductID = item.ProductID;
            window.location = '/Member/NewIdentityCode';
        },
        backToOrder: function () {
            window.location = sessionStorage.PreviousPage;
        }
    }
});

var identityCode = new Vue({
    el: '#identityCodeModal',
    data: {
        title: '添加唯一识别码',
        model: {
            OrderDetailsID: '',
            Quantity: 0,
            CodeFull: false,
            AlreadyAddedQuantity: 0
        },
        error: {
            message: '',
            show: false
        },
        items: [],
        displayPages: [],
        totalPages: 0,
        currentPage: 1,
        totalCount: 0,
        rowFrom: 0,
        rowTo: 0
    },
    methods: {
        getPages: function (page) {
            getCodePages(this, page);
        },
        removeCode: function (item, index) {
        },
        addSingle: function () {

        },
        addRange: function () {

        }
    }
});

//显示唯一识别码窗口
function showIdentityCodeModal(item) {
    identityCode.title = '【' + item.Product.Name + '】' + item.Quantity + '套 -- 登记唯一识别码';

    var model = identityCode.model;
    model.OrderDetailsID = item.ID;
    model.Quantity = item.Quantity;
    model.CodeFull = item.CodeFull;

    showModal($('#identityCodeModal'));
}

//获取唯一识别码列表
function getCodePages(vm, page) {
    showDialog('正在查询...');
    var url = '/Member/GetCodePages';
    var data = {
        orderDetailsId: vm.model.OrderDetailsID,
        page: page,
        rows: getRowsCount()
    };
    vm.$http.post(url, data).then(function (result) {
        if (relogin(result.data)) {
            return;
        }
        //hideModal($('#addSingle'));
        //hideModal($('#addRange'));
        hideAllDialog();
        vm.items = result.data.IdentityCodes;
        CalculateTablePages(vm, page, result);
        vm.model.CodeFull = result.data.TotalCount == vm.model.Quantity ? true : false;
        //rows.items[orderIndex].CodeFull = $this.codeFull;
    }, function (error) {
        showError($this, error);
    })
}

//查询订单明细
function getOrderDetails(vm, page) {
    var url = 'OrderDetails';
    var data = {
        page: page,
        rows: getRowsCount(),
        orderId: sessionStorage.OrderID
    }
    showDialog('正在查询，请稍后...');
    vm.$http.post(url, data).then(
        function (result) {
            if (relogin(result.data)) {
                return;
            }
            hideAllDialog();

            vm.items = result.data.OrderDetails;
            vm.header.OrderNo = result.data.OrderNo;
            vm.header.Description = result.data.Description;
            CalculateTablePages(vm, page, result);
        }
        );
}