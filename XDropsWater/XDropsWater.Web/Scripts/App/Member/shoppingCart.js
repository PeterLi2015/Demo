

var shoppingCartRows = new Vue({
    el: '#shoppingCartRows',
    data: {
        items: [],
        error: {
            message: '',
            show: false
        },
        description: '',
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
            getShoppingCarts(this, page);
        },
        update: function (index, item) {
            showUpdate(item);
        },
        remove: function (index, item) {
            removeShoppingCart(this, item);
        },
        add: function () {
            clear();
        },
        addOrder: function () {
            addOrder(this);
        }
    }
});

var shoppingCart = new Vue({
    el: '#shoppingCart',
    data: {
        title: '',
        model: {
            ID: '',
            ProductID: '',
            Quantity: ''
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
            AddUpdateShoppingCart(this);
        },
        add: function () {
            clear();
        }
    }
});

function showUpdate(item) {
    shoppingCart.title = '修改产品';
    shoppingCart.model.ProductID = item.ProductID;
    shoppingCart.model.Quantity = item.Quantity;
    shoppingCart.model.ID = item.ID;
    shoppingCart.error.show = false;
    showModal($('#shoppingCart'));
}

function AddUpdateShoppingCart(vm) {
    var url = 'AddUpdateShoppingCart';
    if ($('#shoppingCartForm').data('bootstrapValidator').isValid()) {
        showDialog('正在保存...');
        vm.$http.post(url, vm.model).then(
            function (result) {
                hideAllDialog();
                if (relogin(result.data)) {
                    return;
                }
                hideModal($('#shoppingCart'));
                shoppingCartRows.getPages(1);
            },
            function (error) {
                showError(vm, error);
            }
            );
    }

}

function clear() {
    shoppingCart.title = '添加产品';
    shoppingCart.model.ID = '';
    shoppingCart.model.ProductID = '';
    shoppingCart.model.Quantity = '';
    shoppingCart.error.show = false;
    showModal($('#shoppingCart'));
}

function getProducts(vm) {
    var url = 'GetProducts';
    vm.$http.post(url).then(
        function (result) {
            vm.Products = result.data.Products;
        }
        );
}

function getShoppingCarts(vm, page) {
    var url = 'ShoppingCart';
    var data = {
        page: page,
        rows: getRowsCount()
    }
    showDialog('正在查询，请稍后...');
    vm.$http.post(url, data).then(
        function (result) {
            if (relogin(result.data)) {
                return;
            }
            hideAllDialog();
            vm.items = result.data.ShoppingCarts;
            vm.description = result.data.Description;
            CalculateTablePages(vm, page, result);
        }
        );
}

function removeShoppingCart(vm, item) {
    var message = '您确定要删除【' + item.Product.Name + '】吗?';
    showConfirm(message, function () {
        showDialog('正在删除...');
        var url = '/Member/RemoveShoppingCart';
        var data = {
            ID: item.ID
        }
        vm.$http.post(url, data).then(
            function (result) {
                if (relogin(result.data)) {
                    return;
                }
                shoppingCartRows.getPages(1);
            },
            function (error) {
                showAlert(error);
            }
            );
    });

}

function addOrder(vm) {
    var message = '您确定要提交订单吗？';
    showConfirm(message, function () {
        var url = '/Member/AddOrder1';
        var data = {};
        vm.$http.post(url, data).then(
            function (result) {
                window.location = '/Member/PersonalOrder1';
            },
            function (error) {
                showAlert(error);
            }
            );
    });
}

$(function () {
    formValidator();
});

$('#shoppingCart').on('hidden.bs.modal', function () {
    $("#shoppingCartForm").data('bootstrapValidator').destroy();
    $('#shoppingCartForm').data('bootstrapValidator', null);
    formValidator();
});


function formValidator() {

    $('#shoppingCartForm').bootstrapValidator({
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