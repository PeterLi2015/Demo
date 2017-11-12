//订单明细
var codeRows = new Vue({
    el: '#codeRows',
    data: {
        items: [],
        header: {
            ProductName: '',
            Quantity: '',
        },
        error: {
            show: false,
            message: ''
        },
        IsDeliverly: false,
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
            getIdentityCodes(this, page);
        },
        add: function () {

            var title = '【' + this.header.ProductName + '】添加识别码，已添加' + this.totalCount + '还差' + (this.header.Quantity - this.totalCount);
            showAddModal(title);

        },
        remove: function (item, index) {
            remove(this, item, index);
        },
        backToOD: function () {
            window.location = '/Member/AddIdentityCode';
        },
        removeAll: function () {
            removeAll(this)
        }
    }
});

var addCode = new Vue({
    el: '#addCode',
    data: {
        title: '添加唯一识别码',
        model: {
            orderDetailsId: '',
            codeFrom: '',
            codeTo: ''
        },
        error: {
            message: '',
            show: false
        }
    },
    methods: {
        add: function () {
            newIdentityCode(this);
        }
    }
});

//删除所有识别码
function removeAll(vm) {
    var message = '您确定要删除所有识别码吗？';
    showConfirm(message, function () {
        var url = '/Member/RemoveAllCode';
        var data = {
            orderDetailsId: sessionStorage.OrderDetailsID
        }
        vm.$http.post(url, data).then(
            function (result) {
                showDialog('删除成功', 500);
                vm.items = [];
                vm.totalCount = 0;
            },
            function (error) {
                showAlert(error);
            }
            );
    })
}

//删除识别码
function remove(vm, item, index) {
    var message = '您确定要删除唯一识别码【' + item.Code + '】吗？';
    showConfirm(message, function () {
        var url = '/Member/RemoveCode';
        var data = {
            Id: item.ID
        }
        vm.$http.post(url, data).then(
            function (result) {
                showDialog('删除成功', 500);
                removeRecord(vm, index);
            },
            function (error) {
                showAlert(error);
            }
            );
    });
}

function removeRecord(vm, index) {
    vm.items.splice(index, 1);
    vm.totalCount = vm.totalCount - 1;
}

//添加唯一识别码
function newIdentityCode(vm) {

    if ($('#addCodeForm').data('bootstrapValidator').isValid()) {
        var url = 'AddNewIdentityCode';

        showDialog('正在保存...');
        vm.model.orderDetailsId = sessionStorage.OrderDetailsID;
        vm.$http.post(url, vm.model).then(
            function (result) {
                hideAllDialog();
                if (relogin(result.data)) {
                    return;
                }
                hideModal($('#addCode'));
                codeRows.getPages(1);
            },
            function (error) {
                showError(vm, error);
            }
            );
    }
}


function showAddModal(title) {
    var model = addCode.model;
    model.codeFrom = '';
    model.codeTo = '';
    addCode.title = title;
    addCode.error.show = false;
    showModal($('#addCode'));
}


//查询唯一识别吗
function getIdentityCodes(vm, page) {
    var url = 'NewIdentityCode';
    var data = {
        orderDetailsId: sessionStorage.OrderDetailsID,
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

            vm.items = result.data.IdentityCodes;
            vm.header.ProductName = result.data.ProductName;
            vm.header.Quantity = result.data.Quantity;
            vm.IsDeliverly = result.data.IsDeliverly;
            CalculateTablePages(vm, page, result);
        }
        );
}

$(function () {
    formValidator();
});

$('#addCode').on('hidden.bs.modal', function () {
    $("#addCodeForm").data('bootstrapValidator').destroy();
    $('#addCodeForm').data('bootstrapValidator', null);
    formValidator();
});



function formValidator() {

    $('#addCodeForm').bootstrapValidator({
        message: '值不能为空',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            CodeFrom: {
                message: '唯一识别码验证失败',
                validators: {
                    notEmpty: {
                        message: '唯一识别码不能是空'
                    },
                    regexp: {
                        regexp: /^\d+$/,
                        message: '唯一识别码只能是数字'
                    }
                }
            },
            CodeTo: {
                message: '唯一识别码验证失败',
                validators: {
                    notEmpty: {
                        message: '唯一识别码不能是空'
                    },
                    regexp: {
                        regexp: /^\d+$/,
                        message: '唯一识别码只能是数字'
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