
// 我的库存
var memberStocks = new Vue({
    el: '#memberStocks',
    data: {
        title:'', // 标题 xxx的库存
        items: [],
        error: {
            message: '',
            show: false
        },
        displayPages: [],
        totalPages: 0,
        currentPage: 1,
        totalCount: 0,
        rowFrom: 0,
        rowTo: 0
    },
    mounted: function () {
        getMyStocks(this, 1);
    },
    methods: {
        getPages: function (page) {
            getMyStocks(this, page);
        },
        showCode: function (item) {
            showCode(item);
        },
        update: function (item) {
            showUpdate(item);
        }
    }
});


// 识别码
var code = new Vue({
    el: '#code',
    data: {
        title: '',
        ProductID: '', // 产品ID
        items: [],
        error: {
            message: '',
            show: false
        },
        displayPages: [],
        totalPages: 0,
        currentPage: 1,
        totalCount: 0,
        rowFrom: 0,
        rowTo: 0
    },

    methods: {
        getPages: function (page) {
            getCodes(this, page);
        }
    }
});



// 修改库存
var updateStock = new Vue({
    el: '#updateStock',
    data: {
        title: '',
        model: {
            MemberProductID:'', // 会员产品表库存
            ProductID:'', // 产品编号
            ProductName:'', // 产品名称
            Quantity:'' // 库存数量
        },
        error: {
            message: '',
            show: false
        }
    },

    methods: {
        save: function () {
            saveStock(this, model); // 保存库存
        }
    }
});

// 保存库存
function saveStock(vm, model){
    if ($('#updateStock').data('bootstrapValidator').isValid()) {
        var url = 'SaveStock';

        showDialog('正在保存...');
        var data = {
            memberProductId: vm.model.MemberProductID,
            quantity: vm.model.Quantity
        }
        vm.$http.post(url, data).then(
            function (result) {
                hideAllDialog();
                if (relogin(result.data)) {
                    return;
                }
                hideModal($('#updateStock'));
                memberStocks.getPages(1);
            },
            function (error) {
                showError(vm, error);
            }
            );
    }
}

// 显示识别码
function showCode(item) {
    code.title = '【' + item.Product.Name + '】识别码明细';
    code.ProductID = item.ProductID;
    getCodes(code, 1);
    showModal($('#code'));
}

// 获取识别码
function getCodes(vm, page) {
    showDialog('正在查询，请稍后...');
    var url = '/Member/GetCodes';
    var data = {
        productId: vm.ProductID,
        page: page,
        rows: getRowsCount()
    }
    vm.$http.post(url, data).then(
       function (result) {
           if (relogin(result.data)) {
               return;
           }
           vm.items = result.data.IdentityCodes;
           CalculateTablePages(vm, page, result);
       },
       function (error) {
           showError(error);
       }
       );
}

// 获取我的库存
function getMyStocks(vm, page) {
    vm.title = sessionStorage.MemberName + '的库存信息';
    showDialog('正在查询，请稍后...');
    var url = 'MemberStock';
    var data = {
        memberId: sessionStorage.MemberID,
        page: page,
        rows: getRowsCount()
    };
    vm.$http.post(url, data).then(
        function (result) {
            if (relogin(result.data)) {
                return;
            }
            vm.items = result.data.MemberProducts;
            CalculateTablePages(vm, page, result);
        },
        function (error) {
            showError(error);
        }
        );
}
