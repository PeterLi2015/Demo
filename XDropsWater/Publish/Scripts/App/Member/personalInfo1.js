
var baseInfo = new Vue({
    el: '#baseInfo',
    data: {
        model: {
            MemberName: '',
            Mobile: '',
            IdentityNo: '',
            UpgradeAmount: '', // 升级金额
            MinusAmount: '', // 优惠金额
            Address: '',
            UserRoleID: '',
            MemberRole: {
                ID: '',
                RoleName: ''
            }
        },
        error: {
            message: '',
            show: false
        },
    },
    mounted: function () {
        getBaseInfo(this);
    },
    methods: {
        submit: function () {
            saveBaseInfo(this);
        }
    }
});

var password = new Vue({
    el: '#password',
    data: {
        model: {
            OldPassword: '',
            NewPassword: '',
            ConfirmPassword: ''
        },
        error: {
            message: '',
            show: false
        },
    },
    methods: {
        submit: function () {
            savePassword(this);
        }
    }
});

function saveBaseInfo(vm) {
    vm.error.show = false;
    var form = $('#baseInfoForm');
    if (formIsValid(form)) {
        var url = 'SaveBaseInfo';
        var data = {
            Mobile: vm.model.Mobile,
            MemberName: vm.model.MemberName,
            Address: vm.model.Address,
            IdentityNo: vm.model.IdentityNo,
        }
        showDialog('正在保存');
        vm.$http.post(url, data).then(function (result) {
            hideAllDialog();
            showInfo('保存成功');
        },
        function (error) {
            hideAllDialog();
            showError(vm, error);
        });
    }

}

function savePassword(vm) {
    vm.error.show = false;
    var form = $('#passwordForm');
    if (formIsValid(form)) {
        var url = 'ChangePassword1';
        var data = {
            OldPassword: vm.model.OldPassword,
            NewPassword: vm.model.NewPassword,
            ConfirmPassword: vm.model.ConfirmPassword
        }
        showDialog('正在修改');
        vm.$http.post(url, data).then(function (result) {
            hideAllDialog();
            showInfo('密码修改成功');
        },
        function (error) {
            hideAllDialog();
            showError(vm, error);
        });
    }

}

function getBaseInfo(vm) {
    var url = 'GetPersonalInfo1';
    var data = {};
    vm.$http.post(url, data).then(function (result) {
        vm.model.MemberName = result.data.MemberName;
        vm.model.Mobile = result.data.Mobile;
        vm.model.IdentityNo = result.data.IdentityNo;
        vm.model.UpgradeAmount = result.data.UpgradeAmount;
        vm.model.MinusAmount = result.data.MinusAmount;
        vm.model.Address = result.data.Address;
        vm.model.UserRoleID = result.data.UserRoleID;
        vm.model.MemberRole = result.data.MemberRole;
    },
    function (error) {
        showError(vm, error);
    });
}

$(function () {
    baseInfoFormValidate();
    passwordFormValidate();
});

function baseInfoFormValidate() {

    $('#baseInfoForm').bootstrapValidator({
        message: '值不能为空',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            MemberName: {
                message: '姓名不能为空',
                validators: {
                    notEmpty: {
                        message: '姓名不能为空'
                    }
                }
            },
            Mobile: {
                message: '手机号码验证失败',
                validators: {
                    notEmpty: {
                        message: '手机号码不能为空'
                    },
                    regexp: {
                        regexp: /^1\d{10}$/,
                        message: '手机号码必须是以1开头的11位数字'
                    }
                }
            },
            Address: {
                message: '地址不能为空',
                validators: {
                    notEmpty: {
                        message: '地址不能为空'
                    }
                }
            },

        }
    })
    .on('success.form.bv', function (e) {
        e.preventDefault();
    });

}


function passwordFormValidate() {

    $('#passwordForm').bootstrapValidator({
        message: '值不能为空',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            OldPassword: {
                message: '原始密码不能为空',
                validators: {
                    notEmpty: {
                        message: '原始密码不能为空'
                    }
                }
            },
            NewPassword: {
                message: '新密码不能为空',
                validators: {
                    notEmpty: {
                        message: '新密码不能为空'
                    }
                }
            },
            ConfirmPassword: {
                message: '确认密码验证失败',
                validators: {
                    notEmpty: {
                        message: '确认密码不能为空'
                    },
                    identical: {
                        field: 'NewPassword',
                        message: '确认密码和新密码不一致'
                    }
                }
            },

        }
    })
    .on('success.form.bv', function (e) {
        e.preventDefault();
    });

}