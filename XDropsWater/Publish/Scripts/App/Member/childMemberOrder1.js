
/**************************************Vue实例 begin**********************************************/

//订单管理
var rows = new Vue({
    el: '#rows',
    data: {
        items: [],

        search: {
            isDelivery: '',
            mobileOrName: ''
        },

        displayPages: [], //显示多少页
        totalPages: 0, //总页数
        currentPage: 1, //当前页码
        rowFrom: 0, //从第几行开始显示
        rowTo: 0, //到第几行显示结束
        totalCount: 0, //总行数

        permission: {
            operate: sessionStorage.memberOrderManageOperate
        },

        error: {
            show: false,
            message: ''
        }
    },
    mounted: function () {
        this.getPages(1)
    },
    methods: {
        addCode: function (item) {
            //跳转到登记唯一识别码页面
            sessionStorage.OrderID = item.ID;
            sessionStorage.PreviousPage = '/Member/ChildMemberOrder1';
            window.location = '/Member/AddIdentityCode';
        },
        getPages: function (page) {
            getOrderPages(page, this);
        },
        sendReset: function (order, index) {
            sendReset(order, index);
        },
        updateReset: function (order, index) {
            updateReset(order, index);
        }

    }
});


//发货或修改快递
var modal = new Vue({
    el: '#modal',
    data: {
        model: {
            ID: '',
            OrderDetails: '', // 订单详细信息

            Express: '',

            Title: '',
            Index: 0,
            MemberName: '',
            OrderNo: ''
        },
        error: {
            show: '',
            message: ''
        },
        operateType: ''
    },
    methods: {
        submit: function () {
            SendOrUpdate(this.model, this.operateType);
        }
    }
});


/**************************************Vue实例 end**********************************************/



/*
功能: 给订单发货
参数:
    order: 订单
    index: 订单索引
    $this: Vue对象
*/
function sendReset(order, index) {
    //var title = '请再次确认代理订单，因为发货后订单数据不能修改和删除，您确定要为' + order.Member.MemberName + '的订单【' + order.Quantity + '】套发货吗？'
    var title = '【' + order.Member.MemberName + '，' + order.Member.Mobile + '】订单编号【' + order.OrderNo + '】 -- 发货';
    modal.model.MemberName = order.Member.MemberName;
    modal.model.OrderNo = order.OrderNo;
    var operateType = '发货';
    reset(order, index, title, operateType);


}

/*
功能: 修改快递信息，重置数据
参数:
    order: 订单
    index: 订单索引
*/
function updateReset(order, index) {
    var title = '【' + order.Member.MemberName + '】订货【' + order.Quantity + '】套 -- 修改快递信息';
    var operateType = '修改';
    reset(order, index, title, operateType);
}

/*
功能: 发货或修改快递信息时重置数据
参数:
    order: 订单信息
    index: 订单索引
    title: 弹出框标题
    operateType: 操作类型, 发货还是修改快递
*/
function reset(order, index, title, operateType) {
    modal.model.ID = order.ID;
    modal.model.Express = order.Express;

    modal.model.Title = title;
    modal.model.Index = index;
    modal.operateType = operateType;
    getOrderDetails(modal, order.ID);
    $('#modal').modal('show');
}

// 获取订单详细信息
function getOrderDetails(vm, orderId) {
    var url = '/Member/GetOrderDetails';
    var data = {
        orderId: orderId
    }
    vm.$http.post(url, data).then(function (result) {
        vm.model.OrderDetails = result.data;
    },
    function (error) {
        showError(vm, error);
    })
}


/*
功能: 获取订单列表
参数:
    page: 页码
    $this: 当前Vue对象
*/
function getOrderPages(page, $this) {

    showDialog('正在查询，请稍后...');
    var url = 'MemberOrderManage1';
    $this.search.page = page;
    var data = {
        mobileOrName: $this.search.mobileOrName,
        isDelivery: $this.search.isDelivery,
        page: page,
        rows: getRowsCount()
    };
    $this.$http.post(url, data)
        .then(function (result) {
            hideAllDialog();
            if (relogin(result.data)) {
                return;
            }
            $this.items = result.data.Orders;
            CalculateTablePages($this, page, result);
        }, function (error) {
            hideAllDialog();
            $('#errors').html(error.body.message).removeClass('hide');
        });
}


/*
功能: 发货或修改快递信息
参数:
    model: 订单信息(包含ID, 数量, 快递等)
    operateType: 操作类型, 发货或修改快递信息
*/
function SendOrUpdate(model, operateType) {
    if (operateType == '修改') {
        updateExpress(model);
    }
    else if (operateType == '发货') {
        send(model);
    }
}

function updateExpress(model) {

    if (!validateAll($('#form'))) {
        return;
    }

    showDialog('正在保存，请稍后...');

    var url = 'UpdateOrderExpress';
    var data = {
        orderId: model.ID,
        expressContent: model.Express
    }
    modal.$http.post(url, data).then(function (result) {
        if (relogin(result.data)) {
            return;
        }
        $('#modal').modal('hide');
        hideAllDialog();
        updateOrderRow(modal.model);
        showDialog('修改成功', 1000);
    }, function (error) {
        hideAllDialog();
        modal.error.message = error.body.message;
        modal.error.show = true;
    });
}


/*
功能: 发货
参数:
    model: 订单信息
*/
function send(model) {

    if (!validateAll($('#form'))) {
        return;
    }
    var title = '发货后订单数据不能更改，您确定要为【' + model.MemberName + '】的订单【' + model.OrderNo + '】发货吗？'
    showConfirm(title, function () {
        sendOut(model)
    });

}


/*
功能: 发货
参数:
    model: 订单信息
*/
function sendOut(model) {

    showDialog('正在保存，请稍后...');
    var url = 'SendMemberOrder1';
    var data = {
        orderId: model.ID,
        expressContent: model.Express
    }
    modal.$http.post(url, data).then(function (result) {
        if (relogin(result.data)) {
            return;
        }
        hideModal($('#modal'));
        updateOrderRow(modal.model);
        showDialog('发货成功', 1000);

    }, function (error) {
        showError(modal, error);
    });
}



/*
功能: 更新订单
参数: 
    model: 订单信息
*/
function updateOrderRow(model) {
    var row = rows.items[model.Index];
    row.Express = model.Express;
    row.IsDeliverly = true;
}


//发货或修改快递模态框验证销毁重构
$('#modal').on('hidden.bs.modal', function () {
    $("#form").data('bootstrapValidator').destroy();
    $('#form').data('bootstrapValidator', null);
    orderFormValidator();
});

/*
功能: 页面加载时初始化
*/
$(function () {

    //订单表单验证
    orderFormValidator();


});



/*
唯一识别码表单验证
*/
function codeFormValidator() {

    $('#codeModalForm').bootstrapValidator()
    .on('success.form.bv', function (e) {
        // Prevent form submission
        e.preventDefault();
    });

}

/*
功能: 订单表单验证
*/
function orderFormValidator() {

    $('#form').bootstrapValidator({
        message: '值不能为空',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            Express: {
                message: '快递信息验证失败',
                validators: {
                    notEmpty: {
                        message: '快递信息不能为空'
                    }
                }
            },
        }
    })
    .on('success.form.bv', function (e) {
        // Prevent form submission
        e.preventDefault();
    });

}

