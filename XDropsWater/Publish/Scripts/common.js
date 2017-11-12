Vue.filter('formatDate', function (value) {
    if (value) {
        return moment(String(value)).format('YYYY/MM/DD HH:MM:ss')
    }
});


//session过期重新登录
function relogin(data) {
    if (data == 'Login') {
        var dialog = bootbox.dialog({
            size: 'small',
            onEscape: false,
            message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i> 由于您长时间未操作，请重新登录</div>'
        });

        setTimeout(function () {
            bootbox.hideAll();
            var top = getTopWinow();
            top.location.href = '/login/login';
        }, 1000);
        return true;
    }
    return false;
}

/**
* 在页面中任何嵌套层次的窗口中获取顶层窗口
 * @return 当前页面的顶层窗口对象
 */
function getTopWinow() {
    var p = window;
    while (p != p.parent) {
        p = p.parent;
    }
    return p;
}

/*
功能: 计算从第几页显示到第几页
参数:
    current: 当前页码
    length: 总页数
    displayLength: 要显示多少页
*/
function calculatePages(current, length, displayLength) {
    var indexes = [];
    var start = Math.ceil(current - displayLength / 2);
    var end = Math.floor(current + displayLength / 2);
    if (start <= 1) {
        start = 1;
        end = start + displayLength - 1;
        if (end >= length - 1) {
            end = length - 1;
        }
    }
    if (end >= length - 1) {
        end = length;
        start = end - displayLength + 1;
        if (start <= 1) {
            start = 1;
        }
    }
    for (var i = start; i <= end; i++) {
        indexes.push(i);
    }

    return indexes;
}

function showDialog(title, delay) {
    title = '<div class="text-center"><i class="fa fa-spin fa-spinner"></i>' + title + '</div>'
    bootbox.dialog({
        size: 'small',
        onEscape: false,
        message: title
    });

    if (delay) {
        setTimeout(function () {
            hideAllDialog();
        }, delay);
    }
}

function hideAllDialog() {
    bootbox.hideAll();
}

/*
功能: 确定操作提示
参数:
    message: 提示消息
    callback: 确定之后要操作的函数
*/
function showConfirm(message, callback) {

    bootbox.confirm({
        size: 'small',
        onEscape: function () { },
        closeButton: true,
        message: message,
        buttons: {
            cancel: {
                label: '<i class="fa fa-times"></i> 取消'
            },
            confirm: {
                label: '<i class="fa fa-check"></i> 确定'
            }
        },
        callback: function (result) {
            if (!result) {
                return;
            }
            callback();
        }
    });


}

function validateAll($form) {
    var data = $form.data('bootstrapValidator');
    if (data) {
        // 修复记忆的组件不验证
        data.validate();
        if (!data.isValid()) {
            return false;
        }
    }
    return true;
}

/*
功能: 弹出警告框
参数:
    error: 错误对象
*/
function showAlert(error) {
    var message = '<span style="color:red">' + error.body.message + '</span>'
    bootbox.alert({
        message: message,
        size: 'small',
        buttons: {
            ok: {
                label: '<i class="fa fa-check"></i> 确定',
                className: 'btn-success'
            }
        }
    });
}

// 提示
function showInfo(message) {
    message = '<span>' + message + '</span>'
    bootbox.alert({
        message: message,
        size: 'small',
        buttons: {
            ok: {
                label: '<i class="fa fa-check"></i> 确定',
                className: 'btn-success'
            }
        }
    });
}

/*
vm: Vue instance
error: error object from respond
*/
function showError(vm, error) {
    hideAllDialog();
    vm.error.message = error.body.message;
    vm.error.show = true;
}

function hideModal(dialog) {
    dialog.modal('hide');
}

function showModal(dialog) {
    dialog.modal('show');
}


/*
功能: 显示多少页
*/
function displayPagesLength() {
    return 5;
}


/*
功能: 每页显示多少行
*/
function getRowsCount() {
    return 10;
}

/*
功能: 计算表格显示的页面信息
参数:
    $this: Vue实例
    page: 页码
    response: server返回的响应信息
*/
function CalculateTablePages($this, page, response) {
    hideAllDialog();
    $this.totalPages = response.data.TotalPages;
    $this.currentPage = page;
    $this.displayPages = calculatePages(page, response.data.TotalPages, displayPagesLength());
    $this.rowFrom = response.data.RowFrom;
    $this.rowTo = response.data.RowTo;
    $this.totalCount = response.data.TotalCount;
}

/*
分页组件
*/
Vue.component('xdropswater-pagination', {
    // declare the props
    props: ['rowFrom', 'rowTo', 'totalCount', 'currentPage', 'totalPages', 'displayPages'],
    // just like data, the prop can be used inside templates
    // and is also made available in the vm as this.message
    template: '<div style="margin-left: 10px" v-show="totalCount>0">' +
                    '显示{{rowFrom}}到{{rowTo}}行，共{{totalCount}}行' +
                 '<div class="pagination-container" v-show="totalPages>1">' +
                    '<ul class="pagination">' +
                        '<li v-show="!(currentPage==1 || totalPages==0)"><a href="javascript:void(0);" v-on:click="getFirst()"><<</a></li>' +
                        '<li v-show="!(currentPage==1 || totalPages==0)"><a href="javascript:void(0);" v-on:click="getPrevious()"><</a></li>' +
                        '<li v-bind:class="{ active: page==currentPage }" v-for="page in displayPages"><a href="javascript:void(0);" v-on:click="getPages(page)">{{ page }}</a></li>' +
                        '<li v-show="!(currentPage==totalPages || totalPages==0)"><a href="javascript:void(0);" v-on:click="getNext()">></a></li>' +
                        '<li v-show="!(currentPage==totalPages || totalPages==0)"><a href="javascript:void(0);" v-on:click="getLast()">>></a></li>' +
                    '</ul>' +
                '</div>' +
            '</div>',
    methods: {
        getPages: function (page) {
            this.$parent.getPages(page);
        },
        getFirst: function () {
            this.getPages(1);
        },
        getLast: function () {
            this.getPages(this.totalPages);
        },
        getNext: function () {
            this.getPages(this.currentPage + 1);
        },
        getPrevious: function () {
            this.getPages(this.currentPage - 1);
        }
    }
});

/*
No data 组件
*/
Vue.component('xdropswater-nodata', {
    props: ['totalCount'],
    template: '<div style="margin-left: 10px; color:red" v-show="totalCount==0">' +
                                '<strong>没有查到数据</strong>' +
                            '</div>'
});

/*
No data 组件
*/
Vue.component('xdropswater-error', {
    props: ['error'],
    template: '<div class="form-group has-error" v-show="error.show">' +
                                    '<div class="col-sm-offset-1  col-sm-10">' +
                                       ' <span class="help-block">{{error.message}}</span>' +
                                    '</div>' +
                                '</div>'
});

/*
modal footer 组件
*/
Vue.component('xdropswater-modal-footer', {
    template: '<div class="form-group modal-footer modal-footer-bg-xdropswater">' +
                                   ' <button type="button" class="btn btn-default" data-dismiss="modal"><i class="glyphicon glyphicon-remove"></i> 关闭</button>' +
                                    '<button type="submit" class="btn btn-primary"><i class="glyphicon glyphicon-save"></i> 保存</button>' +
                                '</div>'
});

/*
search button 组件
*/
Vue.component('xdropswater-search-button', {
    template: '<button v-on:click="getPages(1)" class="btn btn-info" type="button">' +
                                '<i class="glyphicon glyphicon-search"></i> 查询</button>',
    methods: {
        getPages: function (page) {
            this.$parent.getPages(page);
        }
    }
});

/*
add button 组件
*/
Vue.component('xdropswater-add-button', {
    template: '<button v-on:click="add()" class="btn btn-success" type="button">' +
                                '<i class="glyphicon glyphicon-plus"></i> 添加</button>',
    methods: {
        add: function () {
            this.$parent.add();
        }
    }
});

//表单验证是否通过
function formIsValid(form) {
    return form.data('bootstrapValidator').isValid();
}
    
