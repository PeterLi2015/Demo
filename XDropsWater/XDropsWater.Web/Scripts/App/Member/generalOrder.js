
//总代订单
var rows = new Vue({
    el: '#rows',
    data: {
        items: [],
        search: {
            dateFrom: '', // 开始日期
            dateTo: '' //结束日期
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
        this.getPages(1);
        this.datePick();
    },
    methods: {
        getPages: function (page) {
            getGeneralOrder(this, page);
        },
        datePick: function () {
            var $this = this;
            $("#dateFrom").datepicker({
                language: "zh-CN",
                autoclose: true,//选中之后自动隐藏日期选择框
                clearBtn: true,//清除按钮
                todayBtn: true,//今日按钮
                format: "yyyy-mm-dd"
            }).on('hide', function (ev) {
                $this.search.dateFrom = $('#dateFrom').val();
            });
            $("#dateTo").datepicker({
                language: "zh-CN",
                autoclose: true,//选中之后自动隐藏日期选择框
                clearBtn: true,//清除按钮
                todayBtn: true,//今日按钮
                format: "yyyy-mm-dd"
            }).on('hide', function (ev) {
                $this.search.dateTo = $('#dateTo').val();
            });
        }
        
    }
});



// 获取总代订单
function getGeneralOrder(vm, page) {
    showDialog('正在查询...');
    var url = '/Member/GeneralOrder';
    var data = {
        page: page,
        size: getRowsCount(),
        dateFrom: vm.search.dateFrom, // 开始日期
        dateTo: vm.search.dateTo // 结束日期

    };
    vm.$http.post(url, data).then(function (result) {
        if (relogin(result.data)) {
            return;
        }
        hideAllDialog();
        vm.items = result.data.GeneralOrderList; // 发货信息列表
        for (var i = 0; i < vm.items.length; i++) {
            if (vm.items[i].Level == 1) {
                vm.items[i].BackgroundColor = 'lightpink';
                vm.items[i].MemberName += '(A)';
            }
            else if (vm.items[i].Level == 2) {
                vm.items[i].BackgroundColor = 'lightgrey';
                vm.items[i].MemberName += '(B)';
            }
            else {
                vm.items[i].BackgroundColor = 'lightblue';
                vm.items[i].MemberName += '(C)';
            }
        }
        CalculateTablePages(vm, page, result);
    }, function (error) {
        showError(vm, error);
    })
}

