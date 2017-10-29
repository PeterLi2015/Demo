
/**************************************Vue实例 begin**********************************************/

//代理管理
var members = new Vue({
    el: '#members',
    data: {
        members: [],
        search: {
            page: 1,
            rows: 10,
            mobileOrName: ''
        },
        addModel: {
            ID: '',
            Mobile: '',
            MemberName: '',
            IdentityNo: '',
            Address: '',
            RoleId: 0,
            ProvinceAvailable: 0,
            GeneralAvailable: 0,
            ParentMobile: '',
            ParentMemberName: ''
        },
        permission: {
            add: sessionStorage.memberManageAdd,
            operate: sessionStorage.memberManageOperate
        },
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
        this.getPages(1)
    },
    methods: {
        getPages: function (page) {
            getMembers(page, this);
        },
        reset: function (index, member) {
            reset(index, member);
        },
        add: function () {
            clear();
        },
        calculatePages: function (current, length, displayLength) {
            var indexes = calculatePages(current, length, displayLength);
            if (this.currentPage > indexes.length && 5 > indexes.length > 1) {
                this.currentPage = this.currentPage - 1;
            }
            return indexes;
        },
        deleteMember: function (index, member) {
            deleteMember(index, member, this);
        },
        resetPwd: function (member) {
            resetPwd(member, this);
        },
        stock: function (item) {
            showStock(item);
            //sessionStorage.MemberID = item.ID;
            //sessionStorage.PreviousPage = '/Member/MemberManage1';
            //sessionStorage.MemberName = item.MemberName;
            //window.location = '/Member/MemberStock'
        }
    }
});

// 显示库存
function showStock(item) {
    stock.memberId = item.ID;
    stock.title = item.MemberName + '库存信息';
    getMyStocks(stock, 1);
    showModal($('#stock'));

}

//添加代理
var addMember = new Vue({
    el: '#memManageModal',
    data: {
        addModel: {
            ID: '',
            Mobile: '',
            MemberName: '',
            IdentityNo: '',
            Address: '',
            RoleID: 0,
            ProvinceAvailable: 0,
            GeneralAvailable: 0,
            ParentMember: {
                Mobile: '',
                MemberName: ''
            },
            CurrentRoleAmount: '', // 角色金额
            TotalAmount: '', // 总金额
            OperationTitle: '',
            Index: 0
        },
        error: {
            message: '',
            show: false
        }
    },
    methods: {
        getParentName: function (mobile) {
            getParentName(mobile, this);
        },
        onSubmit: function (e) {

            AddUpdateMember(this, e);

        }

    }
});


//库存
var stock = new Vue({
    el: '#stock',
    data: {
        memberId:'',
        items: [],
      
        error: {
            message: '',
            show: false
        },
        title:'',
        displayPages: [],
        totalPages: 0,
        currentPage: 1,
        totalCount: 0,
        rowFrom: 0,
        rowTo: 0
    },
   
    methods: {
        getPages: function (page) {
            getMyStocks(this, page);
        }
    }
});

// 获取我的库存
function getMyStocks(vm, page) {
    showDialog('正在查询，请稍后...');
    var url = 'MemberStock';
    var data = {
        memberId: vm.memberId,
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
            showError(vm, error);
        }
        );
}

/**************************************Vue实例 end**********************************************/


/**************************************代理管理 begin**********************************************/


/*
功能: 获取代理列表
参数:
    page: 要显示第几页
    members: 处理代理列表的Vue对象
*/
function getMembers(page, members) {
    var dialog = bootbox.dialog({
        size: 'small',
        onEscape: false,
        message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i> 正在查询，请稍后...</div>'
    });

    var url = 'MemberManage1';
    members.search.page = page;
    members.$http.post(url, members.search)
        .then(function (result) {

            if (relogin(result.data)) {
                return;
            }
            hideAllDialog();
            members.members = result.data.Members;
            CalculateTablePages(members, page, result);

        }, function (error) {
            showError(members, error);

        });
}


/*
功能: 删除代理
参数:
    index: 要删除的代理在显示的代理列表中的索引
    $this: 操作删除功能的Vue对象
*/
function deleteMember(index, member, $this) {

    var title = '确定要删除[' + member.MemberName + ']吗？删除之后，数据将不可恢复。';
    showConfirm(title, function () {
        var url = '/Member/RemoveMember';
        var data = {
            id: member.ID
        }
        $this.$http.post(url, data).then(
            function (success) {
                removeRecord(index, $this);
                showDialog('删除成功', 1000);
                //dialog = bootbox.dialog({
                //    size: 'small',
                //    onEscape: false,
                //    message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i>删除成功</div>'
                //});
                //setTimeout(function () {
                //    bootbox.hideAll();
                //}, 1000);
            },
            function (error) {
                showAlert(error);
                //bootbox.hideAll();
                //bootbox.alert({ message: error.body.message, size: 'small' });
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
    $this.members.splice(index, 1);
}


/*
功能: 重置密码
参数:
    member: 需要重置密码的代理
    $this: 操作重置密码功能的Vue对象
*/
function resetPwd(member, $this) {
    var message = '重置密码后只能用新密码登陆，你真的要为代理[' + member.MemberName + ']重置密码吗？';
    showConfirm(message, function () {
        var url = '/Member/ResetPassword';
        var data = {
            memberId: member.ID
        }
        $this.$http.post(url, data).then(
            function (success) {
                showDialog('重置密码成功', 1000);
            },
            function (error) {
                showAlert(error);
            }
            );
    });

}



/*
功能: 为修改代理重置实体对象
参数:
    index: 要修改的代理在代理列表中的索引
    member: 要修改的代理的实体对象
*/
function reset(index, member) {
    addMember.$data.addModel.ID = member.ID,
    addMember.$data.addModel.Mobile = member.Mobile;
    addMember.$data.addModel.MemberName = member.MemberName;
    addMember.$data.addModel.IdentityNo = member.IdentityNo;
    addMember.$data.addModel.Address = member.Address;
    addMember.$data.addModel.RoleID = member.RoleID;
    addMember.$data.addModel.ProvinceAvailable = member.ProvinceAvailable;
    addMember.$data.addModel.GeneralAvailable = member.GeneralAvailable;
    addMember.$data.addModel.ParentMember.Mobile = member.ParentMember ? member.ParentMember.Mobile : '';
    addMember.$data.addModel.ParentMember.MemberName = member.ParentMember ? member.ParentMember.MemberName : '';
    addMember.$data.addModel.CurrentRoleAmount = member.CurrentRoleAmount; // 角色金额
    addMember.$data.addModel.TotalAmount = member.TotalAmount; // 总金额
    addMember.$data.addModel.OperationTitle = '修改代理';
    addMember.$data.addModel.Index = index;
    addMember.$data.error.show = false;
    addMember.$data.error.message = '';
    showModal($('#memManageModal'));
}

/*
功能: 为添加代理清空实体对象
*/
function clear() {
    addMember.$data.addModel.ID = '00000000-0000-0000-0000-000000000000';
    addMember.$data.addModel.Mobile = '';
    addMember.$data.addModel.MemberName = '';
    addMember.$data.addModel.IdentityNo = '';
    addMember.$data.addModel.Address = '';
    addMember.$data.addModel.RoleID = '';
    addMember.$data.addModel.ProvinceAvailable = '';
    addMember.$data.addModel.GeneralAvailable = '';
    addMember.$data.addModel.ParentMember.Mobile = '';
    addMember.$data.addModel.ParentMember.MemberName = '';
    addMember.$data.addModel.CurrentRoleAmount = '';
    addMember.$data.addModel.TotalAmount = '';
    addMember.$data.addModel.OperationTitle = '添加代理';
    addMember.$data.error.show = false;
    addMember.$data.error.message = '';
    showModal($('#memManageModal'));
}

/**************************************代理管理 end**********************************************/


/**************************************添加代理 begin**********************************************/

/*
功能: 修改代理成功后，将修改后的内容写回到代理列表中对应的代理对象
参数: 
    model: 修改后的代理对象
*/
function resetMember(model) {
    var member = members.$data.members[model.Index];
    member.ID = model.ID;
    member.Mobile = model.Mobile;
    member.MemberName = model.MemberName;
    member.IdentityNo = model.IdentityNo;
    member.Address = model.Address;
    member.RoleID = model.RoleID;
    member.ProvinceAvailable = model.ProvinceAvailable;
    member.GeneralAvailable = model.GeneralAvailable;
    member.ParentMember.Mobile = model.ParentMember.Mobile;
    member.ParentMember.MemberName = model.ParentMember.MemberName;
    member.CurrentRoleAmount = model.CurrentRoleAmount;
    member.TotalAmount = model.TotalAmount;
}

/*
功能: 添加/修改代理的时候，根据上级代理的手机号码获取上级代理的姓名
参数:
    mobile: 上级代理的手机号码
    $this: 添加/修改代理的Vue对象
*/
function getParentName(mobile, $this) {
    $this.$http.post("GetMemberName", { mobile: mobile }).then(function (result) {
        $this.addModel.ParentMember.MemberName = result.data.MemberName;
    }, function (error) { });
}

/*
功能: 添加或修改代理
参数:
    vm: 操作该功能的Vue对象
    e: 添加或修改代理的Form表单对象
*/
function AddUpdateMember(vm, e) {
    var url = e.target.action;

    //var $form = $('#frmAdd');

    //var data = $form.data('bootstrapValidator');
    //if (data) {
    //    // 修复记忆的组件不验证
    //    data.validate();

    //    if (!data.isValid()) {
    //        return false;
    //    }
    //}

    if (!validateAll($('#frmAdd'))) {
        return;
    }

    ////开启验证
    //$('#frmAdd').data('bootstrapValidator').validate();
    //if (!$('#frmAdd').data('bootstrapValidator').isValid()) {
    //    return;
    //}

    //var dialog = bootbox.dialog({
    //    size: 'small',
    //    onEscape: false,
    //    message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i> 正在保存，请稍后...</div>'
    //});

    showDialog('正在保存，请稍后...');

    if (vm.addModel.OperationTitle == '修改代理') {

        url = 'UpdateMemberForAdmin1';
        UpdateMember(vm, url);
       
    }
    else if (vm.addModel.OperationTitle == '添加代理') {
        url = 'AddMember1';
        AddMember(vm, url);
       
    }

}

/*
功能: 添加代理
参数:
    vm: Vue实例
    url: 后台API地址
*/
function AddMember(vm, url) {
    vm.$http.post(url, vm.addModel).then(function (result) {
        if (relogin(result.data)) {
            return;
        }
        //bootbox.hideAll();
        hideAllDialog();
        hideModal($('#memManageModal'));
        //$('#memManageModal').modal('hide');

        members.search.mobileOrName = '';
        members.getPages(1);

        showDialog('添加成功', 1000);

    }, function (error) {
        showError(vm, error);
        //bootbox.hideAll();
        //vm.error.message = error.body.message;
        //vm.error.show = true;
    });
}


/*
功能: 修改代理
参数:
    vm: Vue实例
    url: 后台API地址
*/
function UpdateMember(vm, url) {
    vm.$http.post(url, vm.addModel).then(function (result) {
        if (relogin(result.data)) {
            return;
        }

        hideAllDialog();
        hideModal($('#memManageModal'));
        //$('#memManageModal').modal('hide');
        //bootbox.hideAll();
        resetMember(vm.addModel);
        showDialog('修改成功', 1000);
        //dialog = bootbox.dialog({
        //    size: 'small',
        //    onEscape: false,
        //    message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i>修改成功</div>'
        //});
        //setTimeout(function () {
        //    bootbox.hideAll();
        //}, 1000);
    }, function (error) {
        showError(vm, error);
        //bootbox.hideAll();
        //vm.error.message = error.body.message;
        //vm.error.show = true;
    });
}

/**************************************添加代理 end**********************************************/



/**************************************初始化 begin**********************************************/

/*
功能: 页面加载时初始化
*/
$(function () {

    //初始化表单验证
    formValidator();

    //$("#frmAdd").each(function () {
    //    var $form = $(this);
    //    $form.bootstrapValidator();

    //    // 修复bootstrap validator重复向服务端提交bug
    //    $form.on('success.form.bv', function (e) {
    //        // Prevent form submission
    //        e.preventDefault();
    //    });
    //});

});

//添加代理Modal验证销毁重构
$('#memManageModal').on('hidden.bs.modal', function () {
    $("#frmAdd").data('bootstrapValidator').destroy();
    $('#frmAdd').data('bootstrapValidator', null);
    formValidator();
});



function formValidator() {

    $('#frmAdd').bootstrapValidator({
        message: '值不能为空',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            Mobile: {
                message: '手机号码验证失败',
                validators: {
                    notEmpty: {
                        message: '手机号码不能为空'
                    },
                    stringLength: {
                        min: 11,
                        max: 11,
                        message: '手机号码长度必须11位'
                    },
                    regexp: {
                        regexp: /^1\d{10}$/,
                        message: '手机号码必须是以1开头的11位数字'
                    }
                }
            },
            MemberName: {
                message: '姓名验证失败',
                validators: {
                    notEmpty: {
                        message: '姓名不能为空'
                    }
                }
            },
            IdentityNo: {
                message: '身份证号码验证失败',
                validators: {
                    notEmpty: {
                        message: '身份证号码不能为空'
                    },
                    regexp: {
                        regexp: /^(\d{14}(X|x)|\d{18})$/,
                        message: '身份证号码只能是15或18位数字'
                    }
                }
            },
            
            CurrentRoleAmount: {
                message: '角色金额验证失败',
                validators: {
                    notEmpty: {
                        message: '角色金额不能为空'
                    },
                    regexp: {
                        regexp: /^\d+$/,
                        message: '角色金额必须是数字'
                    }
                }
            },
            TotalAmount: {
                message: '总金额验证失败',
                validators: {
                    notEmpty: {
                        message: '总金额不能为空'
                    },
                    regexp: {
                        regexp: /^\d+$/,
                        message: '总金额必须是数字'
                    }
                }
            },
            RoleID: {
                message: '级别验证失败',
                validators: {
                    notEmpty: {
                        message: '级别必选'
                    }
                }
            },
            ProvinceAvailable: {
                message: '是否有效省代验证失败',
                validators: {
                    notEmpty: {
                        message: '是否有效省代必选'
                    }
                }
            },
            GeneralAvailable: {
                message: '是否有效总代验证失败',
                validators: {
                    notEmpty: {
                        message: '是否有效总代必选'
                    }
                }
            },
            ParentMobile: {
                message: '上级手机号码验证失败',
                validators: {
                    stringLength: {
                        min: 11,
                        max: 11,
                        message: '上级手机号码长度必须11位'
                    },
                    regexp: {
                        regexp: /^1\d{10}$/,
                        message: '上级手机号码必须是以1开头的11位数字'
                    },
                    remote: {
                        url: '/Member/CheckMobile',
                        message: '上级手机号码不存在',
                        name: 'mobile',
                        type: 'POST',
                        delay: 1000
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

/**************************************初始化 end**********************************************/

