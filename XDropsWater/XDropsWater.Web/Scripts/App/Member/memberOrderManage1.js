
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
            operate: sessionStorage.memberOrderManageOperate,
            isFinancial: sessionStorage.isFinancial
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
            sessionStorage.PreviousPage = '/Member/MemberOrderManage1';
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
        },
        financialConfirm: function (order, index) {
            financialConfirm(order, index);
        },
        
        //calculatePages: function (current, length, displayLength) {
        //    var indexes = calculatePages(current, length, displayLength);
        //    //if (this.currentPage > indexes.length && displayLength > indexes.length > 1) {
        //    //    this.currentPage = this.currentPage - 1;
        //    //}
        //    return indexes;
        //},
    }
});


/*
功能: 财务确认
参数:
    item: 唯一识别码信息
    index: 唯一识别码索引
    orderIndex: 订单索引
*/
function financialConfirm(model, index) {
    var sTotal = model.Total + '';
    var title = '您确定收到[' + model.Member.MemberName + ': ' + sTotal.bold() + ']元订单款吗？'
    showConfirm(title, function () {
        showDialog('正在确认...');
        var url = '/Member/FinancialConfirm';
        var data = {
            orderId: model.ID
        }
        rows.$http.post(url, data).then(
            function (success) {
                hideAllDialog();
                updateRecordForFinancial(index);
                showDialog('确定收款成功', 500);
            },
            function (error) {
                hideAllDialog();
                showAlert(error)
            });
    });
}

/*
功能: 更新记录
*/
function updateRecordForFinancial(index) {
    rows.items[index].FinancialStatus = 1;
}




//发货或修改快递
var modal = new Vue({
    el: '#modal',
    data: {
        model: {
            ID: '',
            OrderDetails: '', // 订单详细信息
            Express: '',

            Title: '',
            OrderNo: '',
            Index: 0,
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


//登记唯一识别码
var codeModal = new Vue({
    el: '#codeModal',
    data: {

        items: [],

        displayPages: [], //显示多少页
        totalPages: 0, //总页数
        currentPage: 1, //当前页码
        rowFrom: 0, //从第几行开始显示
        rowTo: 0, //到第几行显示结束
        totalCount: 0, //总行数

        search: {
            orderId: '',
        },

        title: '',
        orderId: '',
        memberId: '',
        codeFull: false,//唯一识别码是否填满
        orderIsDeliverly: false,//订单是否已经发货
        orderQuantity: 0,//订货数量
        orderIndex: 0,//订单在列表中的索引
        error: {
            show: '',
            message: ''
        }
    },
    methods: {
        getPages: function (page) {
            getCodePages(page, this, this.orderIndex);
        },
        removeCode: function (item, index) {
            removeCode(item, index, this.orderIndex);
        },
        addSingle: function () {
            showAddSingle(this.memberId, this.orderId, this.orderQuantity);
        },
        addRange: function () {
            showAddRange(this.memberId, this.orderId);
        }
    }
});

//添加单个唯一识别码
var addSingle = new Vue({
    el: '#addSingle',
    data: {
        title: '添加唯一识别码',
        model: {
            OrderID: '',
            MemberID: '',
            Code: '',
        },
        error: {
            show: false,
            message: ''
        }
    },
    methods: {
        onSubmit: function (e) {

            addSingle1(this.model);

        }

    }
});


//按范围添加唯一识别码
var addRange = new Vue({
    el: '#addRange',
    data: {
        title: '范围添加',
        model: {
            OrderID: '',
            MemberID: '',
            CodeFrom: '',
            CodeTo: '',
        },
        error: {
            show: false,
            message: ''
        }
    },
    methods: {
        onSubmit: function (e) {

            addRange1(this.model);

        }

    }
});


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


/**************************************Vue实例 end**********************************************/

/**************************************订单管理 begin*******************************************/

/*
功能: 显示登记唯一识别码窗口
参数:
    model: 订单对象
    index: 订单索引
*/
function showAddCode(orderModel, orderIndex) {
    codeModal.title = '【' + orderModel.Member.MemberName + '】订货【' + orderModel.Quantity + '】套 -- 登记唯一识别码';
    codeModal.memberId = orderModel.Member.ID;
    codeModal.orderId = orderModel.ID;
    codeModal.search.orderId = orderModel.ID;
    codeModal.codeFull = orderModel.CodeFull;
    codeModal.orderIsDeliverly = orderModel.IsDeliverly;
    codeModal.orderQuantity = orderModel.Quantity;
    codeModal.orderIndex = orderIndex;
    showModal($('#codeModal'));
}

/*
功能: 显示唯一识别码
参数:
    orderModel: 订单对象
    orderIndex: 订单索引
*/
function showCodes(orderModel, orderIndex) {
    codeModal.title = '【' + orderModel.Member.MemberName + '】订货【' + orderModel.Quantity + '】套 -- 唯一识别码明细';
    codeModal.memberId = orderModel.Member.ID;
    codeModal.orderId = orderModel.ID;
    codeModal.search.orderId = orderModel.ID;
    codeModal.codeFull = orderModel.CodeFull;
    codeModal.orderIsDeliverly = orderModel.IsDeliverly;
    codeModal.orderQuantity = orderModel.Quantity;
    codeModal.orderIndex = orderIndex;
    showModal($('#codeModal'));
}


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
    var operateType = '发货';
    reset(order, index, title, operateType);

    //showConfirm(title, function () {
    //    var url = '';
    //    var data = {};
    //    $this.$http.post(url, data).then(function (success) { },
    //        function (error) {

    //    });
    //});

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
    modal.model.Mobile = order.Member.Mobile;
    modal.model.MemberName = order.Member.MemberName;
    modal.model.Quantity = order.Quantity;
    modal.model.Express = order.Express;

    modal.model.Title = title;
    modal.model.Index = index;
    modal.operateType = operateType;
    modal.model.OrderNo = order.OrderNo;

    getOrderDetails(modal, order.ID);

    $('#modal').modal('show');
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

/**************************************订单管理 end*******************************************/


/**************************************登记唯一识别码 begin*******************************************/

/*
功能:  获取唯一识别码列表

参数
    page: 页码
    $this: Vue实例
    orderIndex: 订单索引
*/
function getCodePages(page, $this, orderIndex) {
    showDialog('正在查询...');
    var url = '/Member/GetCodePages';
    var data = {
        orderId: $this.search.orderId,
        page: page,
        rows: getRowsCount()
    };
    $this.$http.post(url, data).then(function (result) {
        if (relogin(result.data)) {
            return;
        }
        hideModal($('#addSingle'));
        hideModal($('#addRange'));
        hideAllDialog();
        $this.items = result.data.IdentityCodes;
        CalculateTablePages($this, page, result);
        $this.codeFull = result.data.TotalCount == $this.orderQuantity ? true : false;
        rows.items[orderIndex].CodeFull = $this.codeFull;
    }, function (error) {
        showError($this, error);
    })
}


/*
功能: 显示添加单个唯一识别码窗口
参数:
    memberId: 代理Id
    orderId: 订单Id
    orderQuantity: 订货数量
*/
function showAddSingle(memberId, orderId, orderQuantity) {
    addSingle.model.OrderID = orderId;
    addSingle.model.MemberID = memberId;
    addSingle.model.Code = '';
    addSingle.orderQuantity = orderQuantity;
    addSingle.error.show = false;
    showModal($('#addSingle'));
}


/*
功能: 显示按范围添加唯一识别码窗口
参数:
    memberId: 代理Id
    orderId: 订单Id
*/
function showAddRange(memberId, orderId) {
    addRange.model.OrderID = orderId;
    addRange.model.MemberID = memberId;
    addRange.model.CodeFrom = '';
    addRange.model.CodeTo = '';
    addRange.error.show = false;
    $('#addRange').modal('show');
}


/*
功能: 删除唯一识别码
参数:
    item: 唯一识别码信息
    index: 唯一识别码索引
    orderIndex: 订单索引
*/
function removeCode(item, index, orderIndex) {
    var title = '您确定要删除唯一识别码【' + item.Code + '】吗？'
    showConfirm(title, function () {
        showDialog('正在删除...');
        var url = '/Member/RemoveCode';
        var data = {
            id: item.ID
        }
        codeModal.$http.post(url, data).then(
            function (success) {
                codeModal.totalCount = codeModal.totalCount - 1;
                hideAllDialog();
                //唯一识别码没有填满
                codeModal.codeFull = false;
                removeCodeRecord(index, orderIndex);
                showDialog('删除成功', 500);
            },
            function (error) {
                hideAllDialog();
                showAlert(error)
            });
    });
}


/*
功能: 从唯一识别码列表中删除该条唯一识别码记录
参数:
    index: 唯一识别码记录索引
    orderIndex: 订单记录索引
*/
function removeCodeRecord(index, orderIndex) {
    codeModal.items.splice(index, 1);
    rows.items[orderIndex].CodeFull = false;
}


/**************************************登记唯一识别码 end*******************************************/


/************************************** 添加单个唯一识别码 begin **************************************/

/*
功能: 添加单个唯一识别码
参数:
    model: 唯一识别码信息
*/
function addSingle1(model) {

    if (!validateAll($('#addSingleForm'))) {
        return;
    }
    showDialog('正在添加，请稍后...');
    var url = '/Member/AddCode';
    addSingle.$http.post(url, model).then(function (succ) {
        showDialog('添加成功', 1000);
        hideModal($('#addSingle'));
        //refresh code list
        codeModal.getPages(1);
    }, function (error) {
        showError(addSingle, error)
    });
}

/************************************** 添加单个唯一识别码 end **************************************/

/************************************** 按范围添加唯一识别码 begin **************************************/

/*
功能: 添加单个唯一识别码
参数:
    model: 唯一识别码信息
*/
function addRange1(model) {

    if (!validateAll($('#addRangeForm'))) {
        return;
    }
    showDialog('正在添加，请稍后...');
    var url = '/Member/AddCode';
    addRange.$http.post(url, model).then(function (succ) {
        showDialog('添加成功', 1000);
        //hideModal($('#addRange'));
        //refresh code list
        codeModal.getPages(1);
    }, function (error) {
        showError(addRange, error)
    });
}

/************************************** 按范围添加唯一识别码 end **************************************/


/************************************ 发货/修改快递 begin ************************************/

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
    else if (operateType == '确认收款') {
        financialConfirm(model);
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

/************************************ 发货/修改快递 end ************************************/


/**************************** 初始化 begin *************************************************/

//发货或修改快递模态框验证销毁重构
$('#modal').on('hidden.bs.modal', function () {
    $("#form").data('bootstrapValidator').destroy();
    $('#form').data('bootstrapValidator', null);
    orderFormValidator();
});

//添加单个唯一识别码模态框验证销毁重构
$('#addSingle').on('hidden.bs.modal', function () {
    $("#addSingleForm").data('bootstrapValidator').destroy();
    $('#addSingleForm').data('bootstrapValidator', null);
    addSingleValidator();
});

//按范围添加唯一识别码模态框验证销毁重构
$('#addRange').on('hidden.bs.modal', function () {
    $("#addRangeForm").data('bootstrapValidator').destroy();
    $('#addRangeForm').data('bootstrapValidator', null);
    addRangeValidator();
});

/*
功能: 页面加载时初始化
*/
$(function () {

    //订单表单验证
    orderFormValidator();

    //唯一识别码表单验证
    codeFormValidator();

    //单个唯一识别码添加表单验证
    addSingleValidator();

    //按范围添加唯一识别码验证
    addRangeValidator();

    //唯一识别码窗口显示时获取唯一识别码列表
    $('#codeModal').on('shown.bs.modal', function () {
        codeModal.getPages(1);
    })
});


/*
功能: 添加单个唯一识别码表单验证
*/
function addSingleValidator() {
    $('#addSingleForm').bootstrapValidator({
        message: '唯一识别码不能为空',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            Code: {
                message: '唯一识别码验证失败',
                validators: {
                    notEmpty: {
                        message: '唯一识别码不能为空'
                    },
                    regexp: {
                        regexp: /^\d+$/,
                        message: '唯一识别码只能是数字'
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


/*
功能: 按范围添加唯一识别码窗口验证
*/
function addRangeValidator() {
    $('#addRangeForm').bootstrapValidator({
        message: '唯一识别码不能为空',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            CodeFrom: {
                message: '起始唯一识别码验证失败',
                validators: {
                    notEmpty: {
                        message: '起始唯一识别码不能为空'
                    },
                    regexp: {
                        regexp: /^\d+$/,
                        message: '起始唯一识别码只能是数字'
                    }
                }
            },
            CodeTo: {
                message: '结束唯一识别码验证失败',
                validators: {
                    notEmpty: {
                        message: '结束唯一识别码不能为空'
                    },
                    regexp: {
                        regexp: /^\d+$/,
                        message: '结束唯一识别码只能是数字'
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

/**************************** 初始化 end *************************************************/