
//发货记录
var rows = new Vue({
    el: '#rows',
    data: {
        items: [],
        search: {
            status: -1, // 0未发货，1发货中，2已发货，-1全部
            mobileOrName: '' //电话号码或姓名
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
        rowTo: 0,
        admin: sessionStorage.isAdmin
    },
    mounted: function () {
        this.getPages(1)
    },
    methods: {
        getPages: function (page) {
            getExpress(this, page);
        },
        update: function (item, index) {
            update(item, index);
        },
        add: function () { // 新增发货记录
            clear(); // 清空发货信息
        },
        send: function (item, index) {
            send(item, index);
        },
        sendConfirm: function (item, index) {
            sendConfirm(item, index);
        },
        removeExpress: function (item, index) {
            removeExpress(item, index, this);
        }
    }
});

// 删除快递
function removeExpress(item, index, vm) {
    var title = '确定要删除[' + item.RecipientName + ']的发货记录吗？删除之后，数据将不可恢复。';
    showConfirm(title, function () {
        var url = '/Member/RemoveExpress';
        var data = {
            id: item.ID
        }
        vm.$http.post(url, data).then(
            function (success) {
                removeRecord(index, vm);
                showDialog('删除成功', 1000);

            },
            function (error) {
                showAlert(error);
            });
    })
}


/*
功能: 删除代理成功后，相应的从显示的代理列表中将该代理移除
参数:
    index: 已经删除的代理在代理列表中的索引
    $this: 显示代理列表的Vue对象
*/
function removeRecord(index, $this) {
    $this.items.splice(index, 1);
}


// 重置发货记录并弹出修改窗口
function sendConfirm(item, index) {
    var model = sendModal.model;
    model.Title = '发货';
    model.MemberName = item.Member.MemberName;
    model.RecipientName = item.RecipientName;
    model.RecipientMobile = item.RecipientMobile;
    model.RecipientAddress = item.RecipientAddress;
    model.Content = item.Content;
    model.ID = item.ID;
    model.Index = index;
    model.ExpressName = item.ExpressName;
    model.ExpressNo = item.ExpressNo;
    model.Status = item.Status;
    sendModal.error.show = false;
    sendModal.error.message = '';
    sendModal.model.IsUpdate = false;
    showModal($('#sendModal'));
}



// 清空发货信息并弹出发货窗口
function clear(item) {
    var model = addOrUpdateModal.model;
    model.RecipientName = '';
    model.RecipientMobile = '';
    model.RecipientAddress = '';
    model.Content = '';
    model.ID = '';
    model.Title = '添加发货记录';
    addOrUpdateModal.error.show = false;
    addOrUpdateModal.error.message = '';
    showModal($('#addOrUpdateModal'));
}


// 重置发货记录并弹出修改窗口
function update(item, index) {
    var model = addOrUpdateModal.model;
    model.Title = '修改发货记录';
    model.RecipientName = item.RecipientName;
    model.RecipientMobile = item.RecipientMobile;
    model.RecipientAddress = item.RecipientAddress;
    model.Content = item.Content;
    model.ID = item.ID;
    model.Index = index;
    addOrUpdateModal.error.show = false;
    addOrUpdateModal.error.message = '';
    showModal($('#addOrUpdateModal'));
}



// 新增或修改发货记录
var addOrUpdateModal = new Vue({
    el: '#addOrUpdateModal',
    data: {
        model: {
            Title: '添加发货记录',
            ID: '', // 发货记录ID
            RecipientName: '', // 收件人姓名
            RecipientMobile: '', // 收件人手机号码
            RecipientAddress: '', // 收件人地址
            Content: '', // 发货内容
            Index: 0 // 要修改的记录的索引
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
        submit: function () {
            addOrUpdate(this);
        }
    }
});


// 新增或修改发货记录
var sendModal = new Vue({
    el: '#sendModal',
    data: {
        model: {
            Title: '修改发货记录',
            ID: '', // 发货记录ID
            RecipientName: '', // 收件人姓名
            RecipientMobile: '', // 收件人手机号码
            RecipientAddress: '', // 收件人地址
            Content: '', // 发货内容
            ExpressName: '', // 快递名称
            ExpressNo: '', // 快递单号
            ExpressDate: '', // 发货日期
            Index: 0, // 要修改的记录的索引
            IsUpdate: false,
            Status: 0
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
        submit: function () {
            addOrUpdate(this);
        }
    }
});


// 重置发货记录并弹出修改窗口
function send(item, index) {
    var model = sendModal.model;
    model.Title = '修改发货记录';
    model.MemberName = item.Member.MemberName;
    model.RecipientName = item.RecipientName;
    model.RecipientMobile = item.RecipientMobile;
    model.RecipientAddress = item.RecipientAddress;
    model.Content = item.Content;
    model.ID = item.ID;
    model.Index = index;
    model.ExpressName = item.ExpressName;
    model.ExpressNo = item.ExpressNo;
    model.Status = item.Status;
    sendModal.error.show = false;
    sendModal.error.message = '';
    sendModal.model.IsUpdate = true;
    showModal($('#sendModal'));
}

// 新增或修改发货记录
function addOrUpdate(vm) {
    var url = 'AddOrUpdateExpress';

    if (rows.admin) {
        if (!validateAll($('#sendForm'))) {
            return;
        }
    }
    else {
        if (!validateAll($('#addOrUpdateExpressForm'))) {
            return;
        }
    }


    if (vm.model.Title == '添加发货记录') {


        showDialog('正在保存，请稍后...');

        url = 'AddUpdateExpress';
        AddExpress(vm, url);

    }
    else if (vm.model.Title == '修改发货记录') {
        url = 'AddUpdateExpress';
        UpdateExpress(vm, url);

    }
    else if (vm.model.Title == '发货') {
        url = 'SendExpress';
        UpdateExpress(vm, url);

    }

}


// 修改发货
function UpdateExpress(vm, url) {
    var data = {
        RecipientName: vm.model.RecipientName,
        RecipientMobile: vm.model.RecipientMobile,
        RecipientAddress: vm.model.RecipientAddress,
        Content: vm.model.Content,
        ExpressName: vm.model.ExpressName,
        ExpressNo: vm.model.ExpressNo,
        Status: vm.model.Status,
        ID: vm.model.ID
    }
    vm.$http.post(url, data).then(function (result) {
        if (relogin(result.data)) {
            return;
        }
        hideAllDialog();

        if (rows.admin) {
            hideModal($('#sendModal'));
        }
        else {
            hideModal($('#addOrUpdateModal'));
        }

        //vm.search.mobileOrName = '';
        //vm.search.status = -1;

        //rows.getPages(1);
        if (vm.model.Title == '发货') {
            vm.model.Status = 1;
            vm.model.ExpressDate = new Date();
        }
        if (vm.model.Title == '修改发货记录') {
            if (!vm.model.Status) {
                vm.model.Status = 0;
            }
            else if (vm.model.Status == 0) {
                vm.model.ExpressDate = '';
            }
        }
        resetRecord(vm.model);

        showDialog('修改成功', 1000);

    }, function (error) {
        showError(vm, error);
    });
}

// 重置修改记录
function resetRecord(model) {
    var item = rows.items[model.Index];
    item.RecipientName = model.RecipientName;
    item.RecipientMobile = model.RecipientMobile;
    item.RecipientAddress = model.RecipientAddress;
    item.Content = model.Content;
    item.ExpressName = model.ExpressName;
    item.ExpressNo = model.ExpressNo;
    item.Status = model.Status;
    item.ExpressDate = model.ExpressDate;
}


// 添加发货记录
function AddExpress(vm, url) {
    var data = {
        RecipientName: vm.model.RecipientName,
        RecipientMobile: vm.model.RecipientMobile,
        RecipientAddress: vm.model.RecipientAddress,
        Content: vm.model.Content
    }
    vm.$http.post(url, data).then(function (result) {
        if (relogin(result.data)) {
            return;
        }
        hideAllDialog();
        hideModal($('#addOrUpdateModal'));

        rows.search.mobileOrName = '';
        rows.search.status = -1;

        rows.getPages(1);

        showDialog('添加成功', 1000);

    }, function (error) {
        showError(vm, error);
    });
}



// 获取发货记录
function getExpress(vm, page) {
    showDialog('正在查询...');
    var url = '/Member/GetExpress';
    var data = {
        page: page,
        size: getRowsCount(),
        status: vm.search.status, // 状态，0未发货，1已发货，-1全部
        mobileOrName: vm.search.mobileOrName // 电话号码或姓名模糊查询

    };
    vm.$http.post(url, data).then(function (result) {
        if (relogin(result.data)) {
            return;
        }
        hideAllDialog();
        vm.items = result.data.ExpressList; // 发货信息列表
        CalculateTablePages(vm, page, result);
    }, function (error) {
        showError(vm, error);
    })
}


/*
功能: 页面加载时初始化
*/
$(function () {

    addOrUpdateValidator();
    sendFormValidator();

});

//添加代理Modal验证销毁重构
$('#addOrUpdateModal').on('hidden.bs.modal', function () {
    $("#addOrUpdateExpressForm").data('bootstrapValidator').destroy();
    $('#addOrUpdateExpressForm').data('bootstrapValidator', null);
    addOrUpdateValidator();
});

//添加代理Modal验证销毁重构
$('#sendModal').on('hidden.bs.modal', function () {
    $("#sendForm").data('bootstrapValidator').destroy();
    $('#sendForm').data('bootstrapValidator', null);
    sendFormValidator();
});



/*
功能: 订单表单验证
*/
function addOrUpdateValidator() {

    $('#addOrUpdateExpressForm').bootstrapValidator({
        message: '值不能为空',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            RecipientName: {
                message: '收件人姓名验证失败',
                validators: {
                    notEmpty: {
                        message: '收件人姓名不能为空'
                    }
                }
            },
            RecipientMobile: {
                message: '收件人手机号码验证失败',
                validators: {
                    notEmpty: {
                        message: '收件人手机号码不能为空'
                    },
                    stringLength: {
                        min: 11,
                        max: 11,
                        message: '收件人手机号码长度必须是11位'
                    },
                    regexp: {
                        regexp: /^1\d{10}$/,
                        message: '收件人手机号码必须是以1开头的11位数字'
                    },
                }
            },
            RecipientAddress: {
                message: '收件人地址验证失败',
                validators: {
                    notEmpty: {
                        message: '收件人地址不能为空'
                    }
                }
            },
            Content: {
                message: '发货内容验证失败',
                validators: {
                    notEmpty: {
                        message: '发货内容不能为空'
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
功能: 订单表单验证
*/
function sendFormValidator() {

    $('#sendForm').bootstrapValidator({
        message: '值不能为空',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            RecipientName: {
                message: '收件人姓名验证失败',
                validators: {
                    notEmpty: {
                        message: '收件人姓名不能为空'
                    }
                }
            },
            RecipientMobile: {
                message: '收件人手机号码验证失败',
                validators: {
                    notEmpty: {
                        message: '收件人手机号码不能为空'
                    },
                    stringLength: {
                        min: 11,
                        max: 11,
                        message: '收件人手机号码长度必须是11位'
                    },
                    regexp: {
                        regexp: /^1\d{10}$/,
                        message: '收件人手机号码必须是以1开头的11位数字'
                    },
                }
            },
            RecipientAddress: {
                message: '收件人地址验证失败',
                validators: {
                    notEmpty: {
                        message: '收件人地址不能为空'
                    }
                }
            },
            Content: {
                message: '发货内容验证失败',
                validators: {
                    notEmpty: {
                        message: '发货内容不能为空'
                    }
                }
            },
            ExpressName: {
                message: '快递名称验证失败',
                validators: {
                    notEmpty: {
                        message: '快递名称不能为空'
                    }
                }
            },
            ExpressNo: {
                message: '快递单号不能为空',
                validators: {
                    notEmpty: {
                        message: '快递单号不能为空'
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
