
/************************* Vue instances *********************************/



// orders
var orders = new Vue({
    el: '#orders',
    data: {
        items: [],
        search: {
            isDeliverly: ''
        },
        error: {
            message: '',
            show: false
        },
        allDeliverly: true,
        displayPages: [],
        totalPages: 0,
        currentPage: 1,
        totalCount: 0,
        rowFrom: 0,
        rowTo: 0
    },
    mounted: function () {
        getPages(this, 1);
    },
    methods: {
        getPages: function (page) {
            getPages(this, page);
        },
        add: function () {
            showAddModal();
        },
        update: function (item) {
            sessionStorage.OrderID = item.ID;
            window.location = '/Member/UpdateOrder';

        },
        shopping: function () {
            window.location = '/Member/ShoppingCart';
        },
        remove: function (item, index) {
            //删除订单
            removeOrder(this, item);
        },
        orderDetails: function (item) {
            //跳转到登记唯一识别码页面
            sessionStorage.OrderID = item.ID;
            sessionStorage.PreviousPage = '/Member/PersonalOrder1';
            window.location = '/Member/AddIdentityCode';
        }
    }
});


// add or update order
var addUpdateOrder = new Vue({
    el: '#addUpdateOrder',

    data: {
        title: '',
        items: [],
        error: {
            show: false,
            message: ''
        }
    },
    mounted: function () {
        getProducts(this);
    },
    methods: {
        submit: function () {
            addOrUpdateOrder(this, this.items);
        }
        //validateBeforeSubmit(scope) {
        //    this.$validator.validateAll(scope);

        //}
    }
});


/************************* Vue instances *********************************/


/*************************** orders ****************************/

/**
*** get order rows
*** vm: Vue instance
*** page: No of page
**/
function getPages(vm, page) {
    showDialog('正在查询，请稍后...');
    var url = 'PersonalOrder1';
    var data = {
        page: page,
        rows: getRowsCount(),
        isDeliverly: vm.search.isDeliverly
    };
    vm.$http.post(url, data).then(
        function (result) {
            if (relogin(result.data)) {
                return;
            }
            vm.items = result.data.Orders;
            vm.allDeliverly = result.data.AllDeliverly;
            CalculateTablePages(vm, page, result);
        },
        function (error) {
            showError(error);
        }
        );
}

/*
    show add modal
*/
function showAddModal() {
    addUpdateOrder.title = '添加订单';
    addUpdateOrder.error.show = false;
    var items = addUpdateOrder.items;
    for (i = 0; i < items.length; i++) {
        items[i].Quantity = '';
    }
    getProducts(addUpdateOrder);
    
    showModal($('#addUpdateOrder'));
}

//删除订单
function removeOrder(vm, item) {
    var message = '您确定要删除订单【' + item.OrderNo + '】吗？';
    showConfirm(message, function () {
        var url = '/Member/RemoveOrder';
        data = {
            orderId: item.ID
        }
        vm.$http.post(url, data).then(
            function (result) {
                if (relogin(result.data)) {
                    return;
                }

                showDialog('删除成功', 1000);
                vm.getPages(1);
            }
            );
    });
}

/*************************** orders ****************************/

/*************************** add update order ****************************/

/*
    get all products
    vm: vue instance
*/
function getProducts(vm) {
    var url = 'GetRoleProducts';
    vm.$http.post(url).then(function (result) {
        vm.items = result.data.Products;
    }, function (error) { });
}

/* 
    add or update order
    vm: vue instance
    items: product purchase information
*/
function addOrUpdateOrder(vm, items) {
    if (vm.title = '添加订单') {
        var url = 'AddOrder1';

        vm.$http.post(url, items).then(
            function (result) {
                if (relogin(result.data)) {
                    return;
                }
                hideAllDialog();
                hideModal($('#addUpdateOrder'));
                rows.getPages(1);
            },
            function (error) {
                showError(vm, error);
                //addUpdateOrderFormValidator();
            }
            );
    }
}

/*************************** add update order ****************************/

/*************************** initialization *****************************/

$(function () {

    //addUpdateOrderFormValidator();

});

//$('#addUpdateOrder').on('hidden.bs.modal', function () {
//    $("#addUpdateOrderForm").data('bootstrapValidator').destroy();
//    $('#addUpdateOrderForm').data('bootstrapValidator', null);
//    addUpdateOrderFormValidator();
//});

///*
//    add update order form validation
//*/
//function addUpdateOrderFormValidator() {

//    $('#addUpdateOrderForm').bootstrapValidator()
//   .on('success.form.bv', function (e) {

//       e.preventDefault();

//   });

//}

/*************************** initialization *****************************/


