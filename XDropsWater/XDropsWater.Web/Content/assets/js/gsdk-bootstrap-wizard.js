/*!

 =========================================================
 * Bootstrap Wizard - v1.1.1
 =========================================================
 
 * Product Page: https://www.creative-tim.com/product/bootstrap-wizard
 * Copyright 2017 Creative Tim (http://www.creative-tim.com)
 * Licensed under MIT (https://github.com/creativetimofficial/bootstrap-wizard/blob/master/LICENSE.md)
 
 =========================================================
 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

// Get Shit Done Kit Bootstrap Wizard Functions

searchVisible = 0;
transparent = true;



$(document).ready(function () {

    /*  Activate the tooltips      */
    $('[rel="tooltip"]').tooltip();

    // Code for the Validator
    var $validator = $('.wizard-card form').validate({
        rules: {
            currentUser_account: {
                required: true,
                isMobile: true
            },
            currentUser_password: {
                required: true
            },

            registerUser_mobile: {
                required: true,
                isMobile: true
            },
            registerUser_memberName: {
                required: true
            },
            registerUser_identityNo: {
                required: true
            },
            registerUser_parentMobile: {
                required: true,
                isMobile: true
            },
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
            },

            product1Quantity: {
                isNumber: true
            },
            product2Quantity: {
                isNumber: true
            },
            product3Quantity: {
                isNumber: true
            },
            product4Quantity: {
                isNumber: true
            },
            product5Quantity: {
                isNumber: true
            },
            product6Quantity: {
                isNumber: true
            },
            product7Quantity: {
                isNumber: true
            },
        },
        messages: {
            currentUser_account: {
                required: '账号必填',
                isMobile: '账号必须是以1开头的11位手机号码'
            },
            currentUser_password: {
                required: '密码必填'
            },

            registerUser_mobile: {
                required: '手机号码必填',
                isMobile: '手机号码必须是以1开头的11位手机号码'
            },
            registerUser_memberName: {
                required: '代理姓名必填'
            },
            registerUser_identityNo: {
                required: '代理身份证号必填'
            },
            registerUser_parentMobile: {
                required: '上级手机号码必填',
                isMobile: '上级手机号码必须是以1开头的11位手机号码'
            },
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
            },
            product1Quantity: {
                isNumber: '产品数量必须是数字'
            },
            product2Quantity: {
                isNumber: '产品数量必须是数字'
            },
            product3Quantity: {
                isNumber: '产品数量必须是数字'
            },
            product4Quantity: {
                isNumber: '产品数量必须是数字'
            },
            product5Quantity: {
                isNumber: '产品数量必须是数字'
            },
            product6Quantity: {
                isNumber: '产品数量必须是数字'
            },
            product7Quantity: {
                isNumber: '产品数量必须是数字'
            }
        }
    });

    // Wizard Initialization
    $('.wizard-card').bootstrapWizard({
        'tabClass': 'nav nav-pills',
        'nextSelector': '.btn-next',
        'previousSelector': '.btn-previous',

        onNext: function (tab, navigation, index) {
            var $valid = $('.wizard-card form').valid();
            if (!$valid) {
                $validator.focusInvalid();
                return false;
            }
            if (index == 3) {
                if ($('#addressValidate').val() == 'false') {
                    return false;
                }
            }
        },

        onInit: function (tab, navigation, index) {

            //check number of tabs and fill the entire row
            var $total = navigation.find('li').length;
            $width = 100 / $total;
            var $wizard = navigation.closest('.wizard-card');

            $display_width = $(document).width();

            if ($display_width < 600 && $total > 3) {
                $width = 50;
            }

            navigation.find('li').css('width', $width + '%');
            $first_li = navigation.find('li:first-child a').html();
            $moving_div = $('<div class="moving-tab">' + $first_li + '</div>');
            $('.wizard-card .wizard-navigation').append($moving_div);
            refreshAnimation($wizard, index);
            $('.moving-tab').css('transition', 'transform 0s');
        },

        onTabClick: function (tab, navigation, index) {

            var $valid = $('.wizard-card form').valid();

            if (!$valid) {
                return false;
            } else {
                return true;
            }
        },

        onTabShow: function (tab, navigation, index) {
            var $total = navigation.find('li').length;
            var $current = index + 1;

            var $wizard = navigation.closest('.wizard-card');

            // If it's the last tab then hide the last button and show the finish instead
            if ($current >= $total) {
                $($wizard).find('.btn-next').hide();
                $($wizard).find('.btn-finish').show();
            } else {
                $($wizard).find('.btn-next').show();
                $($wizard).find('.btn-finish').hide();
            }

            button_text = navigation.find('li:nth-child(' + $current + ') a').html();

            setTimeout(function () {
                $('.moving-tab').text(button_text);
            }, 150);

            var checkbox = $('.footer-checkbox');

            if (!index == 0) {
                $(checkbox).css({
                    'opacity': '0',
                    'visibility': 'hidden',
                    'position': 'absolute'
                });
            } else {
                $(checkbox).css({
                    'opacity': '1',
                    'visibility': 'visible'
                });
            }

            refreshAnimation($wizard, index);
        }
    });


    // Prepare the preview for profile picture
    $("#wizard-picture").change(function () {
        readURL(this);
    });

    $('[data-toggle="wizard-radio"]').click(function () {
        wizard = $(this).closest('.wizard-card');
        wizard.find('[data-toggle="wizard-radio"]').removeClass('active');
        $(this).addClass('active');
        $(wizard).find('[type="radio"]').removeAttr('checked');
        $(this).find('[type="radio"]').attr('checked', 'true');
    });

    $('[data-toggle="wizard-checkbox"]').click(function () {
        if ($(this).hasClass('active')) {
            $(this).removeClass('active');
            $(this).find('[type="checkbox"]').removeAttr('checked');
        } else {
            $(this).addClass('active');
            $(this).find('[type="checkbox"]').attr('checked', 'true');
        }
    });

    $('.set-full-height').css('height', 'auto');

});



//Function to show image before upload

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#wizardPicturePreview').attr('src', e.target.result).fadeIn('slow');
        }
        reader.readAsDataURL(input.files[0]);
    }
}

$(window).resize(function () {
    $('.wizard-card').each(function () {
        $wizard = $(this);
        index = $wizard.bootstrapWizard('currentIndex');
        refreshAnimation($wizard, index);

        $('.moving-tab').css({
            'transition': 'transform 0s'
        });
    });
});

function refreshAnimation($wizard, index) {
    total_steps = $wizard.find('li').length;
    move_distance = $wizard.width() / total_steps;
    step_width = move_distance;
    move_distance *= index;

    $wizard.find('.moving-tab').css('width', step_width);
    $('.moving-tab').css({
        'transform': 'translate3d(' + move_distance + 'px, 0, 0)',
        'transition': 'all 0.3s ease-out'

    });
}

function debounce(func, wait, immediate) {
    var timeout;
    return function () {
        var context = this, args = arguments;
        clearTimeout(timeout);
        timeout = setTimeout(function () {
            timeout = null;
            if (!immediate) func.apply(context, args);
        }, wait);
        if (immediate && !timeout) func.apply(context, args);
    };
};
