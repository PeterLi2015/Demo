
/**************************************Vue实例 begin**********************************************/

//订单管理
var subMembers = new Vue({
    el: '#subMembers',
    data: {
        items: [],

        search: {
            MobileOrName: '',
            LevelID: '-1'
        },

        displayPages: [], //显示多少页
        totalPages: 0, //总页数
        currentPage: 1, //当前页码
        rowFrom: 0, //从第几行开始显示
        rowTo: 0, //到第几行显示结束
        totalCount: 0, //总行数
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
            getSubMembers(page, this);
        }
    }
});



/*
功能: 获取订单列表
参数:
    page: 页码
    vm: 当前Vue对象
*/
function getSubMembers(page, vm) {

    showDialog('正在查询，请稍后...');
    var url = 'GetAllSubMembers1';
    vm.search.page = page;
    var data = {
        mobileOrName: vm.search.MobileOrName,
        levelId: vm.search.LevelID,
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
            for (var i = 0; i < vm.items.length; i++) {
                if (vm.items[i].LevelID == 1) {
                    vm.items[i].BackgroundColor = 'lightpink';
                    vm.items[i].MemberName += '(A)';
                }
                else if (vm.items[i].LevelID == 2) {
                    vm.items[i].BackgroundColor = 'lightgrey';
                    vm.items[i].MemberName += '(B)';
                }
                else{
                    vm.items[i].BackgroundColor = 'lightblue';
                    vm.items[i].MemberName += '(C)';
                }
            }
            CalculateTablePages(vm, page, result);
        }, function (error) {
            hideAllDialog();
            $('#errors').html(error.body.message).removeClass('hide');
        });
}
