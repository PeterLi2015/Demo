

var orderDetailsRows = new Vue({
    el: '#orderDetailsRows',
    data: {
        items: [],
        error: {
            message: '',
            show: false
        },
        amount:0,
        description: '',
        orderNo: '',
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
        update: function (index, item) {
            showUpdate(item);
        },
        remove: function (index, item) {
            removeOrderDetails(this, item);
        },
        add: function () {
            clear();
        },
        backToOrder: function (amount) {
            backToOrder(this, amount);
        }
    }
});

var orderDetails = new Vue({
    el: '#orderDetails',
    data: {
        title: '',
        model: {
            ID: '',
            ProductID: '',
            Quantity: '',
            OrderID: ''
        },
        Products: [],
        error: {
            message: '',
            show: false
        },
    },
    mounted: function () {
        getProducts(this);
    },
    methods: {
        submit: function () {
            AddUpdateOrderDetails(this);
        },
        add: function () {
            clear();
        }
    }
});

function showUpdate(item) {
    orderDetails.title = '修改产品';
    orderDetails.model.ProductID = item.ProductID;
    orderDetails.model.Quantity = item.Quantity;
    orderDetails.model.ID = item.ID;
    orderDetails.error.show = false;
    showModal($('#orderDetails'));

}

function AddUpdateOrderDetails(vm) {
    var url = 'AddUpdateOrderDetails';
    if ($('#orderDetailsForm').data('bootstrapValidator').isValid()) {
        showDialog('正在保存...');
        vm.model.OrderID = sessionStorage.OrderID;
        vm.$http.post(url, vm.model).then(
            function (result) {
                hideAllDialog();
                if (relogin(result.data)) {
                    return;
                }
                hideModal($('#orderDetails'));
                orderDetailsRows.getPages(1);
            },
            function (error) {
                showError(vm, error);
            }
            );
    }

}

function clear() {
    orderDetails.title = '添加产品';
    orderDetails.model.ID = '';
    orderDetails.model.ProductID = '';
    orderDetails.model.Quantity = '';
    orderDetails.error.show = false;
    showModal($('#orderDetails'));

}

function getProducts(vm) {
    var url = 'GetProducts';
    vm.$http.post(url).then(
        function (result) {
            vm.Products = result.data.Products;
        }
        );
}

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
            if (result.data.OrderDetails.length == 0) {
                window.location = '/Member/PersonalOrder1'
            }
            vm.items = result.data.OrderDetails;
            vm.description = result.data.Description;
            vm.orderNo = result.data.OrderNo;
            vm.amount = result.data.TotalAmount;
            CalculateTablePages(vm, page, result);
        }
        );
}

function removeOrderDetails(vm, item) {
    var message = '您确定要删除【' + item.Product.Name + '】吗?';
    showConfirm(message, function () {
        showDialog('正在删除...');
        var url = '/Member/RemoveOrderDetails';
        var data = {
            ID: item.ID
        }
        vm.$http.post(url, data).then(
            function (result) {
                if (relogin(result.data)) {
                    return;
                }
                orderDetailsRows.getPages(1);
            },
            function (error) {
                showAlert(error);
            }
            );
    });

}

function backToOrder(vm,amount) {
    var url = '/Member/SaveOrder';
    var data = {
        amount: amount
    };
    vm.$http.post(url, data).then(
        function (result) {
            window.location = '/Member/PersonalOrder1';
        },
        function (error) {
            showAlert(error);
        }
        );

}

$(function () {
    formValidator();
});

$('#orderDetails').on('hidden.bs.modal', function () {
    $("#orderDetailsForm").data('bootstrapValidator').destroy();
    $('#orderDetailsForm').data('bootstrapValidator', null);
    formValidator();
});


function formValidator() {

    $('#orderDetailsForm').bootstrapValidator({
        message: '值不能为空',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            Product: {
                message: '请选择产品',
                validators: {
                    notEmpty: {
                        message: '请选择产品'
                    }
                }
            },
            Quantity: {
                message: '购买数量验证失败',
                validators: {
                    notEmpty: {
                        message: '购买数量不能为空'
                    },
                    regexp: {
                        regexp: /^\d+$/,
                        message: '购买数量只能是数字'
                    }
                }
            }

        }
    })
    .on('success.form.bv', function (e) {
        // Prevent form submission
        e.preventDefault();
    });

}