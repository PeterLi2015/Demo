
var login = new Vue({
    el: '#ff',
    data: {
        item: {
            uname: '',
            pwd: '',
        }

    },
    methods: {
        UNameWatch: function (body) {
            if ($('i[data-bv-icon-for=UName]').css('display')) {
                $('#UName-icon').hide();
            }
        },
        PwdWatch: function (body) {
            if ($('i[data-bv-icon-for=Pwd]').css('display')) {
                $('#Pwd-icon').hide();
            }
        },
        onSubmit: function (e) {
            bootbox.dialog({
                size: 'small',
                onEscape: false,
                //closeButton: false,
                message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i> 正在登录，请稍后...</div>'
            });

            var url = e.target.action;
            var vm = this;

            var $form = $('#ff');
            var data = $form.data('bootstrapValidator');
            if (data) {
                // 修复记忆的组件不验证
                data.validate();

                if (!data.isValid()) {
                    bootbox.hideAll();
                    return false;
                }
            }

            vm.$http.post(url, vm.item)
                .then(function (result) {
                    setPermission(result.body.Permission);
                    window.location = result.body.MainUrl;
                }, function (error) {
                    bootbox.hideAll();
                    $('#errors').html(error.body.message).removeClass('hide');
                    hideAllDialog();
                });
        }
    },
    watch: {
        'item.uname': 'UNameWatch',
        'item.pwd': 'PwdWatch'
    }
});

function setPermission(permission)
{
    sessionStorage.memberManageAdd = permission.MemberManageAdd ? "true" : "";
    sessionStorage.memberManageOperate = permission.MemberManageOperate ? "true" : "";
    sessionStorage.memberOrderManageOperate = permission.MemberOrderManageOperate ? "true" : "";
    sessionStorage.isAdmin = permission.IsAdmin ? "true" : "";
    sessionStorage.isFinancial = permission.IsFinancial ? "true" : "";
}

$(document).ready(function () {
    //$('input').iCheck({
    //    checkboxClass: 'icheckbox_square-blue',
    //    radioClass: 'iradio_square-blue',
    //    increaseArea: '20%' // optional
    //});
    $('#UName').focus();

    formValidator();
   
});

function formValidator() {
    $('#ff').bootstrapValidator({
        //        live: 'disabled',
        message: '输入无效',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {

            UName: {
                message: '用户名无效',
                validators: {
                    notEmpty: {
                        message: '用户名必须输入并且不能为空'
                    },
                    stringLength: {
                        min: 11,
                        max: 11,
                        message: '用户名长度必须11位'
                    },
                    regexp: {
                        regexp: /^[0-9]+$/,
                        message: '用户名必须是11位手机号码'
                    },

                    different: {
                        field: 'Pwd',
                        message: '用户名不能和密码相同'
                    }
                }
            },
            Pwd: {
                validators: {
                    notEmpty: {
                        message: '密码必须输入并且不能为空'
                    },
                    different: {
                        field: 'UName',
                        message: '密码不能和用户名相同'
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