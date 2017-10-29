
// 我的库存
var myStocks = new Vue({
    el: '#myStocks',
    data: {
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
    showDialog('正在查询，请稍后...');
    var url = 'MyStock';
    var data = {
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
