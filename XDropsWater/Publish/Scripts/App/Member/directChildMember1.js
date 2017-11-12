
/************************************** vue instances **********************************************/

// my children members
var members = new Vue({
    el: '#members',
    data: {
        items: [],

        search: {
            mobileOrName: ''
        },


        displayPages: [], //显示多少页
        totalPages: 0, //总页数
        currentPage: 1, //当前页码
        rowFrom: 0, //从第几行开始显示
        rowTo: 0, //到第几行显示结束
        totalCount: 0, //总行数

        permission: {
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
        getPages: function (page) {
            getDirectChildMember(this, page);
        },

        remove: function (item, index) {
            removeMember(this, item, index);
        },
        add: function () {
            showAddMember();
        }
    }
});


//add or update member
var addUpdateMember = new Vue({
    el: '#addUpdateMember',
    data: {
        title: '',
        model: {
            Mobile: '',
            MemberName: '',
            IdentityNo: '',
            IdentityCode: '',
            Address: '',
            Products: [],
            ProductID: '',
            ParentMember: {
                Mobile: '',
                MemberName: ''
            },
        },
        error: {
            show: '',
            message: ''
        },
    },

    methods: {
        submit: function () {
            newMember(this, this.model);
        },
        remove: function () {
            remove();
        },
        getParentName: function (mobile) {
            getParentName(mobile, this);
        },
    }
});



/*
功能: 添加/修改代理的时候，根据上级代理的手机号码获取上级代理的姓名
参数:
    mobile: 上级代理的手机号码
    $this: 添加/修改代理的Vue对象
*/
function getParentName(mobile, $this) {
    $this.$http.post("GetMemberName", { mobile: mobile }).then(function (result) {
        $this.model.ParentMember.MemberName = result.data.MemberName;
    }, function (error) { });
}


/*
    get all children members
    vm: vue instance
    page: No of page
*/
function getDirectChildMember(vm, page) {

    showDialog('正在查询，请稍后...');
    var url = 'DirectChildMember1';
    var data = {
        mobileOrName: vm.search.mobileOrName,
        page: page,
        rows: getRowsCount()
    };
    vm.$http.post(url, data)
        .then(function (result) {
            hideAllDialog();
            if (relogin(result.data)) {
                return;
            }
            vm.items = result.data.Members;
            CalculateTablePages(vm, page, result);
        }, function (error) {
            showError(vm, error);
        });
}

/*
功能: 删除代理
参数:
    vm: vue实例
    item: 要删除的代理记录
    index: 要删除的代理在列表中的索引
*/
function removeMember(vm, item, index) {
    var message = '删除后数据不可恢复，您确定要删除【' + item.MemberName + '】吗？';
    showConfirm(message, function () {
        showDialog('正在删除，请稍后...', 1000);
        var url = 'RemoveMember1';
        var data = {
            memberId: item.ID
        };
        vm.$http.post(url, data).then(
            function (result) {
                showDialog('删除成功', 1000);
                hideAllDialog();
                //remove member from list
                removeMemberRecord(vm, index);
            },
            function (error) {
                showAlert(error);
            }
            );
    });
}

/*
remove member from list
params:
    vm: vue instance
    index: index of member in list
*/
function removeMemberRecord(vm, index) {
    vm.totalCount -= 1;
    vm.items.splice(index, 1);
}

/*
功能: 显示添加代理窗口
*/
function showAddMember() {
    addUpdateMember.title = '添加代理';
    showModal($('#addUpdateMember'));
}

/**************************************下级代理 end*******************************************/

/************************************** add/update member *******************************************/

/*
    添加新代理
*/
function newMember(vm, model) {
    var url = 'AddDirectChildMember1';

    if (formIsValid($('#addUpdateMemberForm'))) {
        showDialog('正在保存，请稍后...');
        vm.$http.post(url, model).then(function (result) {
            if (relogin(result.data)) {
                return;
            }
            hideAllDialog();
            hideModal($('#addUpdateMember'));
            members.getPages(1);
        }, function (error) {
            showError(vm, error);
        })
    }
}


/************************************** add/update member *******************************************/


/**************************** page initialization *************************************************/


/*
    destroy/refactor the modal frame
*/
$('#addUpdateMember').on('hidden.bs.modal', function () {
    $("#addUpdateMemberForm").data('bootstrapValidator').destroy();
    $('#addUpdateMemberForm').data('bootstrapValidator', null);
    addUpdateMemberFormValidator();
});

/*
    page initialization
*/
$(function () {

    //validator of add/update member form
    addUpdateMemberFormValidator();

});


/*
    validator of add/update member form
*/
function addUpdateMemberFormValidator() {
    $('#addUpdateMemberForm').bootstrapValidator({
        message: '验证失败',
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
                    regexp: {
                        regexp: /^1\d{10}$/,
                        message: '手机号码只能是以1开头的11位数字'
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
                    }
                    //,
                    //regexp: {
                    //    regexp: /^(\d{14}(X|x)|\d{18})$/,
                    //    message: '身份证号码只能是15或18位数字'
                    //}
                }
            },

            Address: {
                message: '地址验证失败',
                validators: {
                    notEmpty: {
                        message: '地址不能为空'
                    }
                }
            },
            ParentMobile: {
                message: '上级手机号码验证失败',
                validators: {
                    notEmpty: {
                        message: '地址不能为空'
                    },
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

/**************************** page initialization *************************************************/