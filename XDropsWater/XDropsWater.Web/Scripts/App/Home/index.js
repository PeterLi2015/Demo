
var updateAddress = new Vue({
    el: '#updateAddressModal',
    data: {
        title: '麻烦您更新一下您的地址',

        show: {
            city: false,
            district: false,
            street: false
        },

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

        error: {
            show: false,
            message: ''
        }
    },
    mounted: function () {
        checkAddessUpdate(this);
        getProvinces(this);
    },
    methods: {
        onSubmit: function (e) {

            saveAddress(this);

        },
        provinceSelected: function () {
            this.cities = [];
            this.districts = [];
            this.streets = [];
            getCities(this);

        },
        citySelected: function () {
            this.districts = [];
            this.streets = [];
            getDistricts(this);
        },
        districtSelected: function () {
            this.streets = [];
            getStreets(this);
        },
    }
});

function saveAddress(vm) {
    var $valid = $('#frmUpdateAddress').valid();
    $validator = $('#frmUpdateAddress').validate();
    if (!$valid) {
        $validator.focusInvalid();
        return false;
    }
    showDialog("正在保存，请稍后...");
    var data = {
        Province: {
            ProvinceName: vm.province.ProvinceName,
            ProvinceCode: vm.province.ProvinceCode
        },
        City: {
            CityName: vm.city.CityName,
            CityCode: vm.city.CityCode
        },
        District: {
            DistrictName: vm.district.DistrictName,
            DistrictCode: vm.district.DistrictCode
        },
        Street: {
            StreetName: vm.street.StreetName,
            StreetCode: vm.street.StreetCode
        },
        Description: vm.addressDescription

    }
    vm.$http.post("/Address/UpdateAddress", { addressModel: data }).then(function (result) {
        hideModal($('#updateAddressModal'));
        showDialog('保存成功', 1000);
    }, function (error) {
        showError(vm, error);
    });
}

function checkAddessUpdate(vm) {
    vm.$http.post("/Address/CheckAddressUpdate", {}).then(function (result) {
        var addressUpdated = result.data;
        if (addressUpdated=='False') {
            showModal($('#updateAddressModal'));
        }
    }, function (error) { });
}

function getProvinces(vm) {
    vm.$http.post("/Register/GetProvinces", {}).then(function (result) {
        vm.provinces = result.data;
    }, function (error) { });
}


function getCities(vm) {
    if (vm.province) {
        vm.$http.post("/Register/GetCities", {
            province: vm.province.ProvinceName
        }).then(function (result) {
            vm.cities = result.data;
            if (vm.cities.length > 0) {
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
    if (vm.city) {
        vm.$http.post("/Register/GetDistricts", {
            province: vm.province.ProvinceName,
            city: vm.city.CityName
        }).then(function (result) {
            vm.districts = result.data;
            if (vm.districts.length > 0) {
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
    if (vm.district) {
        vm.$http.post("/Register/GetStreets", {
            province: vm.province.ProvinceName,
            city: vm.city.CityName,
            district: vm.district.DistrictName
        }).then(function (result) {
            vm.streets = result.data;
            if (vm.streets.length > 0) {
                vm.show.street = true;
            }
            else {
                vm.show.street = false;
            }
        }, function (error) { });
    }
    
}



$(document).ready(function () {

    // Code for the Validator
    var $validator = $('#frmUpdateAddress').validate({
        rules: {

            address_province: {
                required: true
            },
            address_city: {
                required: function (element) {
                    return $('#address_province').val() != '';
                }
            },
            address_district: {
                required: function (element) {
                    return $('#address_city').val() != '';
                }
            },
            address_street: {
                required: function (element) {
                    return $('#address_district').val() != '';
                }
            },
            address_description: {
                required: true
            }
        },
        messages: {

            address_province: {
                required: '请选择省份'
            },
            address_city: {
                required: '请选择城市'
            },
            address_district: {
                required: '请选择区县'
            },
            address_street: {
                required: '选择乡镇/街道'
            },

            address_description: {
                required: '请输入具体地址'
            }
        },
        errorElement: "em",
        errorPlacement: function (error, element) {
            // Add the `help-block` class to the error element
            error.addClass("help-block");

            if (element.prop("type") === "checkbox") {
                error.insertAfter(element.parent("label"));
            } else {
                error.insertAfter(element);
            }
        },
        highlight: function (element, errorClass, validClass) {
            $(element).parents(".col-sm-5").addClass("has-error").removeClass("has-success");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).parents(".col-sm-5").addClass("has-success").removeClass("has-error");
        }
    });
});
