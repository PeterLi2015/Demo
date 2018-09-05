
//注册
var rows = new Vue({
    el: '#row',
    data: {
        show: {
            city: false,
            district: false,
            street: false
        },
        currentUser: {
            mobile: '',
            password: ''
        },
        registerUser: {
            showParentName: false,
            mobile: '',
            memberName: '',
            identityNo: '',
            parentMobile: '',
            parentMemberName: '',

            province: {
                ProvinceName: '',
                ProvinceCode: ''
            },
            city: {
                CityName: '',
                CityCode: ''
            },
            district: {
                DistrictName: '',
                DistrictCode: ''
            },
            street: {
                StreetName: '',
                StreetCode: ''
            },
            addressDescription: '',

            provinces: [],
            cities: [],
            districts: [],
            streets: [],
        },
        product: {
            product1Quantity: 0,
            product2Quantity: 0,
            product3Quantity: 0,
            product4Quantity: 0,
            product5Quantity: 0,
            product6Quantity: 0,
            product7Quantity: 0,
            product8Quantity: 0,
            product9Quantity: 0,
            product10Quantity: 0,
            product11Quantity: 0,
            product12Quantity: 0,

            product1Show: false,
            product2Show: false,
            product3Show: false,
            product4Show: false,
            product5Show: false,
            product6Show: false,
            product7Show: false,
            product8Show: false,
            product9Show: false,
            product10Show: false,
            product11Show: false,
            product12Show: false,

            product: {
                productId: 0,
                productName: ''
            }
            ,
            products: [
               {
                   productId: 1,
                   productName: '小水滴二代饮宝'
               },
               {
                   productId: 2,
                   productName: '小水滴二代浴宝'
               },
               {
                   productId: 3,
                   productName: '小水滴前置'
               },
               {
                   productId: 4,
                   productName: '小水滴304不锈钢Y1饮宝'
               },
               {
                   productId: 5,
                   productName: '小水滴304不锈钢M1浴宝'
               },
               {
                   productId: 6,
                   productName: '小水滴304不锈钢L1龙头宝'
               },
               {
                   productId: 7,
                   productName: '小水滴茶吧机'
               },
               {
                   productId: 8,
                   productName: '小水滴能量杯'
               },
               {
                   productId: 9,
                   productName: '小水滴洗衣球'
               },
               {
                   productId: 10,
                   productName: '小水滴五级滤芯水质处理器'
                },
                {
                    productId: 11,
                    productName: '小水滴油净宝'
                },
                {
                    productId: 12,
                    productName: '小水滴空气净化器'
                },
            ]
        },
        error: {
            show: false,
            message: ''
        }
    },
    mounted: function () {
        getProvinces(this);
    },
    methods: {

        getParentName: function (mobile) {
            getParentName(mobile, this);
        },
        getProvinces: function () {
            getProvinces(this);
        },
        provinceSelected: function () {
            this.registerUser.cities = [];
            this.registerUser.districts = [];
            this.registerUser.streets = [];
            getCities(this);

        },
        citySelected: function () {
            this.registerUser.districts = [];
            this.registerUser.streets = [];
            getDistricts(this);
        },
        districtSelected: function () {
            this.registerUser.streets = [];
            getStreets(this);
        },
        save: function () {
            save(this);
        },
        //next: function () {
        //    next(this);
        //},
        selectProduct: function () {
            selectProduct(this);
        }
    }
});

function registerSucc() {
    var title = '<div class="text-center"><i class="fa fa-spin fa-spinner"></i>' + '标题' + '</div>'
    bootbox.dialog({
        size: 'small',
        onEscape: false,
        message: "注册成功，<label id='lblTime'>3</label>秒之后自动跳转到登录页面"+
            "<script>" +
            "function delayUrl(){" +
                " var delay=parseInt($('#lblTime').html());" +
                    " if(delay>0){" +
                    " delay--;" +
                    " $('#lblTime').html(delay);"+
                    "}" +
                    "else{" +
                    " window.top.location.href='/login/logout';"+
                    "}" +
                   
                "}" +
                 " setInterval('delayUrl()',1000);" +
            "</script>",
    });
}

function selectProduct(vm) {
    if (vm.product.product && vm.product.product.productId > 0) {
        var productId = vm.product.product.productId;
        if (productId == 1) {
            vm.product.product1Show = true;
        }
        else if (productId == 2) {
            vm.product.product2Show = true;
        }
        else if (productId == 3) {
            vm.product.product3Show = true;
        }
        else if (productId == 4) {
            vm.product.product4Show = true;
        }
        else if (productId == 5) {
            vm.product.product5Show = true;
        }
        else if (productId == 6) {
            vm.product.product6Show = true;
        }
        else if (productId == 7) {
            vm.product.product7Show = true;
        }
        else if (productId == 8) {
            vm.product.product8Show = true;
        }
        else if (productId == 9) {
            vm.product.product9Show = true;
        }
        else if (productId == 10) {
            vm.product.product10Show = true;
        }
        else if (productId == 11) {
            vm.product.product11Show = true;
        }
        else if (productId == 12) {
            vm.product.product12Show = true;
        }
    }
    else {
        var error = {
            body: {
                message: '请选择商品'
            }
        }
        showAlert(error);
    }
}

//function next(vm) {
//    $('.wizard-card').each(function () {
//        $wizard = $(this);
//        var index = $wizard.bootstrapWizard('currentIndex');
//        if (index == 2) {
//            if (vm.registerUser.provinces.length > 0) {
//                if (!vm.registerUser.province || vm.registerUser.province.ProvinceName == '') {

//                    var error = {
//                        body: {
//                            message: '请选择省份'
//                        }
//                    }
//                    showAlert(error);
//                    $('#addressValidate').val('false');
//                    return;
//                }
//                else {
//                    $('#addressValidate').val('true');
//                }

//            }
//            if (vm.registerUser.cities.length > 0) {
//                if (!vm.registerUser.city || vm.registerUser.city.CityName == '') {
//                    var error = {
//                        body: {
//                            message: '请选择城市'
//                        }
//                    }
//                    showAlert(error);
//                    $('#addressValidate').val('false');
//                    return;
//                }
//                else {
//                    $('#addressValidate').val('true');
//                }
//            }
//            if (vm.registerUser.districts.length > 0) {
//                if (!vm.registerUser.district || vm.registerUser.district.DistrictName == '') {
//                    var error = {
//                        body: {
//                            message: '请选择区县'
//                        }
//                    }
//                    showAlert(error);
//                    $('#addressValidate').val('false');
//                    return;
//                }
//                else {
//                    $('#addressValidate').val('true');
//                }
//            }
//            if (vm.registerUser.streets.length > 0) {
//                if (!vm.registerUser.street || vm.registerUser.street.StreetName == '') {
//                    var error = {
//                        body: {
//                            message: '请选择乡镇/街道'
//                        }
//                    }
//                    showAlert(error);
//                    $('#addressValidate').val('false');
//                    return;
//                }
//                else {
//                    $('#addressValidate').val('true');
//                }
//            }
//        }
//    });
//}

function save(vm) {



    var product1Quantity = tryParseInt(vm.product.product1Quantity, 0);
    var product2Quantity = tryParseInt(vm.product.product2Quantity, 0);
    var product3Quantity = tryParseInt(vm.product.product3Quantity, 0);
    var product4Quantity = tryParseInt(vm.product.product4Quantity, 0);
    var product5Quantity = tryParseInt(vm.product.product5Quantity, 0);
    var product6Quantity = tryParseInt(vm.product.product6Quantity, 0);
    var product7Quantity = tryParseInt(vm.product.product7Quantity, 0);
    var product8Quantity = tryParseInt(vm.product.product8Quantity, 0);
    var product9Quantity = tryParseInt(vm.product.product9Quantity, 0);
    var product10Quantity = tryParseInt(vm.product.product10Quantity, 0);
    var product11Quantity = tryParseInt(vm.product.product11Quantity, 0);
    var product12Quantity = tryParseInt(vm.product.product12Quantity, 0);

    var total = product1Quantity + product2Quantity + product3Quantity + product4Quantity
        + product5Quantity + product6Quantity + product7Quantity + product8Quantity
        + product9Quantity + product10Quantity + product11Quantity + product12Quantity;

    if (total == 0) {

        var error = {
            body: {
                message: '必须至少购买一个商品'
            }
        }
        showAlert(error);
        return;
    }
    var data = {
        User: {
            Id:'00000000-0000-0000-0000-000000000000',
            UserName: vm.currentUser.userName,
            Password: vm.currentUser.password
        }
        ,
        Member: {
            MemberId: '00000000-0000-0000-0000-000000000000',
            Mobile: vm.registerUser.mobile,
            MemberName: vm.registerUser.memberName,
            Address:'',
            IdentityNo: vm.registerUser.identityNo,
            Password: '',
            ParentId: '00000000-0000-0000-0000-000000000000',
            ParentMobile: vm.registerUser.parentMobile,
            ParentMemberName:''
        },
        MemberAddress: {
            Province: {
                ProvinceName: vm.registerUser.province.ProvinceName,
                ProvinceCode: vm.registerUser.province.ProvinceCode
            },
            City: {
                CityName: vm.registerUser.city.CityName,
                CityCode: vm.registerUser.city.CityCode
            },
            District: {
                DistrictName: vm.registerUser.district.DistrictName,
                DistrictCode: vm.registerUser.district.DistrictCode
            },
            Street: {
                StreetName: vm.registerUser.street.StreetName,
                StreetCode: vm.registerUser.street.StreetCode
            },
            Description: vm.registerUser.addressDescription

        },
        Product: {
            Product1Quantity: vm.product.product1Quantity == '' ? 0 : parseInt(vm.product.product1Quantity),
            Product2Quantity: vm.product.product2Quantity == '' ? 0 : parseInt(vm.product.product2Quantity),
            Product3Quantity: vm.product.product3Quantity == '' ? 0 : parseInt(vm.product.product3Quantity),
            Product4Quantity: vm.product.product4Quantity == '' ? 0 : parseInt(vm.product.product4Quantity),
            Product5Quantity: vm.product.product5Quantity == '' ? 0 : parseInt(vm.product.product5Quantity),
            Product6Quantity: vm.product.product6Quantity == '' ? 0 : parseInt(vm.product.product6Quantity),
            Product7Quantity: vm.product.product7Quantity == '' ? 0 : parseInt(vm.product.product7Quantity),
            Product8Quantity: vm.product.product8Quantity == '' ? 0 : parseInt(vm.product.product8Quantity),
            Product9Quantity: vm.product.product9Quantity == '' ? 0 : parseInt(vm.product.product9Quantity),
            Product10Quantity: vm.product.product10Quantity == '' ? 0 : parseInt(vm.product.product10Quantity),
            Product11Quantity: vm.product.product11Quantity == '' ? 0 : parseInt(vm.product.product11Quantity),
            Product12Quantity: vm.product.product12Quantity == '' ? 0 : parseInt(vm.product.product12Quantity),
        }
    }
    showDialog('正在注册，请稍后...');
    vm.$http.post("/Register/Register", { register: data }).then(function (result) {
        hideAllDialog();
        registerSucc();
    }, function (error) {
        hideAllDialog();
        showAlert(error);
    });
}

function tryParseInt(str, defaultValue) {
    var retValue = defaultValue;
    if (str !== null) {
        if (str.length > 0) {
            if (!isNaN(str)) {
                retValue = parseInt(str);
            }
        }
    }
    return retValue;
}

function getParentName(mobile, $this) {
    $this.$http.post("/Register/GetMemberName", { mobile: mobile }).then(function (result) {
        $this.registerUser.showParentName = true;
        $this.registerUser.parentMemberName = result.data.MemberName;
    }, function (error) { });
}

function getProvinces(vm) {
    vm.$http.post("/Register/GetProvinces", {}).then(function (result) {
        vm.registerUser.provinces = result.data;
    }, function (error) { });
}


function getCities(vm) {
    if (vm.registerUser.province) {
        vm.$http.post("/Register/GetCities", {
            province: vm.registerUser.province.ProvinceName
        }).then(function (result) {
            vm.registerUser.cities = result.data;
            if (vm.registerUser.cities.length > 0) {
                vm.show.city = true;
                vm.show.district = false;
                vm.show.street = false;
            }
            else {
                vm.show.city = false;
                vm.show.district = false;
                vm.show.street = false;
            }
        }, function (error) { });
    }
}


function getDistricts(vm) {
    if(vm.registerUser.city){
        vm.$http.post("/Register/GetDistricts", {
            province: vm.registerUser.province.ProvinceName,
            city: vm.registerUser.city.CityName
        }).then(function (result) {
            vm.registerUser.districts = result.data;
            if (vm.registerUser.districts.length > 0) {
                vm.show.district = true;
                vm.show.street = false;
            }
            else {
                vm.show.district = false;
                vm.show.street = false;
            }
        }, function (error) { });
    }
    
}


function getStreets(vm) {
    if (vm.registerUser.district) {
        vm.$http.post("/Register/GetStreets", {
            province: vm.registerUser.province.ProvinceName,
            city: vm.registerUser.city.CityName,
            district: vm.registerUser.district.DistrictName
        }).then(function (result) {
            vm.registerUser.streets = result.data;
            if (vm.registerUser.streets.length > 0) {
                vm.show.street = true;
            }
            else {
                vm.show.street = false;
            }
        }, function (error) { });
    }
    
}
