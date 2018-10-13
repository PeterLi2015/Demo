//注册
var main = new Vue({
    el: '#main',
    data: {
        level: '',
        levels: {
            one: '一级代理',
            two: '二级代理',
            county: '县级代理',
            city: '市级代理',
            province: '省级代理',
            general: '总代理',
            biggeneral: '大总代',
            director: '董事'
        }
    },
    mounted: function () {
    },
    methods: {
        selectlevel: function (level) {
            selectlevel(level);
        },
    }
});

function selectlevel(level) {
    if (level == '一级代理') {
        one.show = true;
        two.show = false;
        county.show = false;
        city.show = false;
        province.show = false;
        general.show = false;
        biggeneral.show = false;
        director.show = false;
    }
    else if (level == '二级代理') {
        one.show = false;
        two.show = true;
        county.show = false;
        city.show = false;
        province.show = false;
        general.show = false;
        biggeneral.show = false;
        director.show = false;
    }
    else if (level == '县级代理') {
        one.show = false;
        two.show = false;
        county.show = true;
        city.show = false;
        province.show = false;
        general.show = false;
        biggeneral.show = false;
        director.show = false;
    }
    else if (level == '市级代理') {
        one.show = false;
        two.show = false;
        county.show = false;
        city.show = true;
        province.show = false;
        general.show = false;
        biggeneral.show = false;
        director.show = false;
    }
    else if (level == '省级代理') {
        one.show = false;
        two.show = false;
        county.show = false;
        city.show = false;
        province.show = true;
        general.show = false;
        biggeneral.show = false;
        director.show = false;
    }
    else if (level == '总代理') {
        one.show = false;
        two.show = false;
        county.show = false;
        city.show = false;
        province.show = false;
        general.show = true;
        biggeneral.show = false;
        director.show = false;
    }
    else if (level == '大总代') {
        one.show = false;
        two.show = false;
        county.show = false;
        city.show = false;
        province.show = false;
        general.show = false;
        biggeneral.show = true;
        director.show = false;
    }
    else if (level == '董事') {
        one.show = false;
        two.show = false;
        county.show = false;
        city.show = false;
        province.show = false;
        general.show = false;
        biggeneral.show = false;
        director.show = true;
    }
}

var one = new Vue({
    el: '#one',
    data: {
        show: false,
        sale: {
            price: 410,
            quantity: 0,
            total: 0
        },
        total: 0
    },
    mounted: function () {
    },
    methods: {
        calculate: function () {
            calculateone();
        },
    }
});

function calculateone() {
    one.sale.total = one.sale.price * one.sale.quantity;
    one.total = one.sale.total;
}

var two = new Vue({
    el: '#two',
    data: {
        show: false,
        total: 0,
        sale: {
            price: 690,
            quantity: 0,
            total: 0
        },
        wholesale: {
            one: {
                price: 280,
                quantity: 0,
                total: 0
            }
        }
    },
    mounted: function () {
    },
    methods: {
        calculate: function () {
            calculatetwo();
        },
    }
});

function calculatetwo() {
    two.sale.total = two.sale.price * two.sale.quantity;
    two.wholesale.one.total = two.wholesale.one.price * two.wholesale.one.quantity;
    two.total = two.sale.total + two.wholesale.one.total;
}

var county = new Vue({
    el: '#county',
    data: {
        show: false,
        total: 0,
        sale: {
            price: 900,
            quantity: 0,
            total: 0
        },
        wholesale: {
            one: {
                price: 490,
                quantity: 0,
                total: 0
            },
            two: {
                price: 210,
                quantity: 0,
                total: 0
            }
        }
    },
    mounted: function () {
    },
    methods: {
        calculate: function () {
            calculatecounty();
        },
    }
});

function calculatecounty() {
    county.sale.total = county.sale.price * county.sale.quantity;
    county.wholesale.one.total = county.wholesale.one.price * county.wholesale.one.quantity;
    county.wholesale.two.total = county.wholesale.two.price * county.wholesale.two.quantity;
    county.total = county.sale.total + county.wholesale.one.total + county.wholesale.two.total;
}

var city = new Vue({
    el: '#city',
    data: {
        show: false,
        total: 0,
        sale: {
            price: 1090,
            quantity: 0,
            total: 0
        },
        wholesale: {
            one: {
                price: 680,
                quantity: 0,
                total: 0
            },
            two: {
                price: 400,
                quantity: 0,
                total: 0
            },
            county: {
                price: 190,
                quantity: 0,
                total: 0
            }
        }
    },
    mounted: function () {
    },
    methods: {
        calculate: function () {
            calculatecity();
        },
    }
});

function calculatecity() {
    city.sale.total = city.sale.price * city.sale.quantity;
    city.wholesale.one.total = city.wholesale.one.price * city.wholesale.one.quantity;
    city.wholesale.two.total = city.wholesale.two.price * city.wholesale.two.quantity;
    city.wholesale.county.total = city.wholesale.county.price * city.wholesale.county.quantity;
    city.total = city.sale.total + city.wholesale.one.total + city.wholesale.two.total + city.wholesale.county.total;
}

var province = new Vue({
    el: '#province',
    data: {
        show: false,
        total: 0,
        sale: {
            price: 1250,
            quantity: 0,
            total: 0
        },
        wholesale: {
            one: {
                price: 840,
                quantity: 0,
                total: 0
            },
            two: {
                price: 560,
                quantity: 0,
                total: 0
            },
            county: {
                price: 350,
                quantity: 0,
                total: 0
            },
            city: {
                price: 160,
                quantity: 0,
                total: 0
            }
        }
    },
    mounted: function () {
    },
    methods: {
        calculate: function () {
            calculateprovince();
        },
    }
});

function calculateprovince() {
    province.sale.total = province.sale.price * province.sale.quantity;
    province.wholesale.one.total = province.wholesale.one.price * province.wholesale.one.quantity;
    province.wholesale.two.total = province.wholesale.two.price * province.wholesale.two.quantity;
    province.wholesale.county.total = province.wholesale.county.price * province.wholesale.county.quantity;
    province.wholesale.city.total = province.wholesale.city.price * province.wholesale.city.quantity;
    province.total = province.sale.total + province.wholesale.one.total + province.wholesale.two.total + province.wholesale.county.total;
}

var general = new Vue({
    el: '#general',
    data: {
        show: false,
        total: 0,
        sale: {
            price: 1380,
            quantity: 0,
            total: 0
        },
        wholesale: {
            one: {
                price: 970,
                quantity: 0,
                total: 0
            },
            two: {
                price: 690,
                quantity: 0,
                total: 0
            },
            county: {
                price: 480,
                quantity: 0,
                total: 0
            },
            city: {
                price: 290,
                quantity: 0,
                total: 0
            },
            province: {
                price: 130,
                quantity: 0,
                total: 0
            }
        }
    },
    mounted: function () {
    },
    methods: {
        calculate: function () {
            calculategeneral();
        },
    }
});

function calculategeneral() {
    general.sale.total = general.sale.price * general.sale.quantity;
    general.wholesale.one.total = general.wholesale.one.price * general.wholesale.one.quantity;
    general.wholesale.two.total = general.wholesale.two.price * general.wholesale.two.quantity;
    general.wholesale.county.total = general.wholesale.county.price * general.wholesale.county.quantity;
    general.wholesale.city.total = general.wholesale.city.price * general.wholesale.city.quantity;
    general.wholesale.province.total = general.wholesale.province.price * general.wholesale.province.quantity;
    general.total = general.sale.total + general.wholesale.one.total + general.wholesale.two.total + general.wholesale.county.total + general.wholesale.province.total;
}

var biggeneral = new Vue({
    el: '#biggeneral',
    data: {
        show: false,
        total: 0,
        sale: {
            price: 1380,
            quantity: 0,
            total: 0
        },
        wholesale: {
            one: {
                price: 970,
                quantity: 0,
                total: 0
            },
            two: {
                price: 690,
                quantity: 0,
                total: 0
            },
            county: {
                price: 480,
                quantity: 0,
                total: 0
            },
            city: {
                price: 290,
                quantity: 0,
                total: 0
            },
            province: {
                price: 130,
                quantity: 0,
                total: 0
            },
            general50: {
                price: 50,
                quantity: 0,
                generalquantity: 0,
                total: 0
            },
            general30: {
                price: 30,
                quantity: 0,
                generalquantity: 0,
                total: 0
            },
            general20: {
                price: 20,
                quantity: 0,
                generalquantity: 0,
                total: 0
            }
        }
    },
    mounted: function () {
    },
    methods: {
        calculate: function () {
            calculatebiggeneral();
        },
    }
});

function calculatebiggeneral() {
    biggeneral.sale.total = biggeneral.sale.price * biggeneral.sale.quantity;
    biggeneral.wholesale.one.total = biggeneral.wholesale.one.price * biggeneral.wholesale.one.quantity;
    biggeneral.wholesale.two.total = biggeneral.wholesale.two.price * biggeneral.wholesale.two.quantity;
    biggeneral.wholesale.county.total = biggeneral.wholesale.county.price * biggeneral.wholesale.county.quantity;
    biggeneral.wholesale.city.total = biggeneral.wholesale.city.price * biggeneral.wholesale.city.quantity;
    biggeneral.wholesale.province.total = biggeneral.wholesale.province.price * biggeneral.wholesale.province.quantity;
    biggeneral.wholesale.general50.total =
        biggeneral.wholesale.general50.price *
        biggeneral.wholesale.general50.quantity *
        biggeneral.wholesale.general50.generalquantity;

    biggeneral.wholesale.general30.total =
        biggeneral.wholesale.general30.price *
        biggeneral.wholesale.general30.quantity *
        biggeneral.wholesale.general30.generalquantity;

    biggeneral.wholesale.general20.total =
        biggeneral.wholesale.general20.price *
        biggeneral.wholesale.general20.quantity *
        biggeneral.wholesale.general20.generalquantity;

    biggeneral.total = biggeneral.sale.total + biggeneral.wholesale.one.total
        + biggeneral.wholesale.two.total + biggeneral.wholesale.county.total
        + biggeneral.wholesale.province.total
        + biggeneral.wholesale.general50.total
        + biggeneral.wholesale.general30.total
        + biggeneral.wholesale.general20.total;
}

var director = new Vue({
    el: '#director',
    data: {
        show: false,
        total: 0,
        sale: {
            price: 1380,
            quantity: 0,
            total: 0
        },
        wholesale: {
            one: {
                price: 970,
                quantity: 0,
                total: 0
            },
            two: {
                price: 690,
                quantity: 0,
                total: 0
            },
            county: {
                price: 480,
                quantity: 0,
                total: 0
            },
            city: {
                price: 290,
                quantity: 0,
                total: 0
            },
            province: {
                price: 130,
                quantity: 0,
                total: 0
            },
            general50: {
                price: 50,
                quantity: 0,
                generalquantity: 0,
                total: 0
            },
            general30: {
                price: 30,
                quantity: 0,
                generalquantity: 0,
                total: 0
            },
            general20: {
                price: 20,
                quantity: 0,
                generalquantity: 0,
                total: 0
            },
            self: {
                price: 30,
                quantity: 0,
                total: 0
            },
            other: {
                price: 30,
                quantity: 0,
                directorquantity: 1,
                total: 0
            }
        }
    },
    mounted: function () {
    },
    methods: {
        calculate: function () {
            calculatedirector();
        },
    }
});

function calculatedirector() {
    director.sale.total = director.sale.price * director.sale.quantity;
    director.wholesale.one.total = director.wholesale.one.price * director.wholesale.one.quantity;
    director.wholesale.two.total = director.wholesale.two.price * director.wholesale.two.quantity;
    director.wholesale.county.total = director.wholesale.county.price * director.wholesale.county.quantity;
    director.wholesale.city.total = director.wholesale.city.price * director.wholesale.city.quantity;
    director.wholesale.province.total = director.wholesale.province.price * director.wholesale.province.quantity;
    director.wholesale.general50.total =
        director.wholesale.general50.price *
        director.wholesale.general50.quantity *
        director.wholesale.general50.generalquantity;
    director.wholesale.general30.total =
        director.wholesale.general30.price *
        director.wholesale.general30.quantity *
        director.wholesale.general30.generalquantity;
    director.wholesale.general20.total =
        director.wholesale.general20.price *
        director.wholesale.general20.quantity *
        director.wholesale.general20.generalquantity;
    director.wholesale.self.total = director.wholesale.self.price * director.wholesale.self.quantity;
    director.wholesale.other.total = Math.round(director.wholesale.other.price * director.wholesale.other.quantity / director.wholesale.other.directorquantity);
    director.total = Math.round(director.sale.total + director.wholesale.one.total
        + director.wholesale.two.total + director.wholesale.county.total
        + director.wholesale.province.total
        + director.wholesale.general50.total
        + director.wholesale.general30.total
        + director.wholesale.general20.total
        + director.wholesale.self.total
        + director.wholesale.other.total);
}

