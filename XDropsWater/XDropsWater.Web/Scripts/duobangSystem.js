var XDropsWaterSystem = {
    executeResult: {
        Success: "Success",
        Exception: "Exception",
        Login: "Login"
    },
    /**
    * 在页面中任何嵌套层次的窗口中获取顶层窗口
    * @return 当前页面的顶层窗口对象
    */
    getTopWinow: function () {
        alert('dd');
        var p = window;
        while (p != p.parent) {
            p = p.parent;
        }
        return p;
    }
}
