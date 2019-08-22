$(function () {
    $(".nofollow").hover(function () {
        $(this).attr("href", $(this).data("url"))
    })
    $(".nofollow").click(function (e) {
        e.preventDefault();

        var blank = '_self';
        if (e.which == 2) var blank = '_blank';
        if ($(this).data('window') != undefined) blank = $(this).data('window');
        if ($(this).data('url').length > 0) window.open($(this).data('url'), blank);
    })

    /* ---------------------------------------------------------- */
    // Скрытие раскрытие селектов
    $(".js-filter-select__label").on("click", function (e) {
        var thisObject = $(this),
            parentObject = thisObject.parent();

        if (parentObject.hasClass("active")) {
            parentObject.removeClass("active");
            parentObject.trigger("activeChanged");
        } else {
            $(".filter-select").removeClass("active");
            parentObject.addClass("active");
            parentObject.trigger("activeChanged");
        }
    });

    $(document).mouseup(function (e) {
        var visibleObject = $(".filter-select__drop:visible"),
            parentObject = visibleObject.parent(),
            selectItemObject = $(".js-filter-select__item", visibleObject),
            labelObject = $(".js-filter-select__label", parentObject);

        if ($(".js-filter-select__label").is(e.target) || $(".js-filter-select__label").has(e.target).length !== 0) {
            return false;
        }
        else if (!visibleObject.is(e.target) && selectItemObject.is(e.target)) {
            $(".filter-select.active").removeClass("active");
        }
        else if (!visibleObject.is(e.target) && $(e.target.closest(".js-filter-select__item")).length > 0) {
            // Случай, когда кликнули по элементу, вложенному в .js-filter-select__item
            $(".filter-select.active").removeClass("active");
        }
        else if (visibleObject.has(e.target).length === 0) {
            $(".filter-select.active").removeClass("active");
        }
    });

    // Фильтр в селекте городов
    $(".filter-select__search").keyup(function (e) {
        var parentObject = $(this).closest(".filter-select__drop-content"),
            childrenObjects = $(".js-filter-select__item", parentObject);

        if (e.keyCode == 27 || $(this).val() == "") {
            $(this).val("");
            childrenObjects.show();
        }
        else {
            filter(childrenObjects, $(this).val());
        }
    });

    function filter(filter, query) {
        query = $.trim(query);
        $(filter).each(function () {
            ($(this).text().search(new RegExp(query, "i")) < 0) ? $(this).hide() : $(this).show();
        });
    }

    // Заполнение данными
    function selected(thisObject) {
        var thisObject = thisObject,
            parentObject = thisObject.closest(".filter-select"),
            selectText = $(".filter-select__text", parentObject);

        if (parentObject.data("type") === "select") {
            var selectedObject = $(".js-filter-select__item.selected", parentObject),
                inputObject = $(".select-input", parentObject);

            selectedObject.removeClass("selected");
            thisObject.addClass("selected");
            inputObject.val(thisObject.find(".filter-select__link").text());
            selectText.text(thisObject.find(".filter-select__link").text());

            $("[name='" + thisObject.data("field") + "']").val(thisObject.data("value")).change();
        }
        else if (parentObject.data("type") === "radio") {
            console.log("1", thisObject);
            thisObject.attr("checked", "checked");
            $("[name='" + thisObject.data("field") + "']").val(thisObject.data("value")).change();
        }
    }

    $(".filter-select[data-type=select]").each(function () {
        if ($(".js-filter-select__item", this).hasClass("selected")) {
            selected($(".js-filter-select__item.selected", this));
        } else {
            // selected($(".js-filter-select__item", this).filter(":first"));
        }
    });

    $(".filter-select[data-type=radio]").each(function () {
        var checkedList = $(".filter-select__radio-input", this).filter(function (i, elem) { return $(elem).is(":checked"); });
        if (checkedList.length == 0) {
            selected($(".filter-select__radio-input", this).filter(":first"));
        } else {
            selected(checkedList.first());
        }
    });

    $(".js-filter-select__item").on("click", function () {
        selected($(this));
    });

    // Слаидер диапазона
    $(".js-filter-slider__element").each(function () {
        var thisObject = $(this),
            parentMainBlock = thisObject.closest(".filter-select"),
            fromField = $(".value-from", parentMainBlock),
            beforeField = $(".value-before", parentMainBlock),
            fromText = $(".filter-from", parentMainBlock),
            beforeText = $(".filter-before", parentMainBlock),
            unitSlider = " " + thisObject.data("unit"),
            dataName = thisObject.data("name"),
            unitPostfixSlider = " " + thisObject.data("unit-postfix"),
            minValue = +thisObject.data("min"),
            maxValue = +thisObject.data("max"),
            step = +thisObject.data("step"),
            hideRange = thisObject.data("hide-range") != undefined ? true : false,
            stepValues = thisObject.data("steps"),
            prettify = function (num) {
                num = num > 1000000 ? number_format(Math.ceil(num / 1000000), 0, "", " ") + " млн." : number_format(num, 0, "", " ")
                return num;
            },
            slider = thisObject.ionRangeSlider({
                type: "double",
                grid: false,
                min: minValue,
                max: maxValue,
                postfix: unitPostfixSlider,
                from: minValue,
                values: stepValues,
                to: maxValue,
                step: step,
                hide_from_to: hideRange,
                prettify_enabled: true,
                prettify: prettify,
                input_values_separator: ':'
            });
        fromField.val(slider.data("from") + unitSlider);
        beforeField.val(slider.data("to") + unitSlider);

        $(fromText).text(prettify(slider.data("from")));
        $(beforeText).text(prettify(slider.data("to")));

        //updateValues(dataName, slider.data("from"), slider.data("to"));

        slider.on("change", function () {
            fromField.val(slider.data("from") + unitSlider);
            beforeField.val(slider.data("to") + unitSlider);

            $(fromText).text(prettify(slider.data("from")));
            $(beforeText).text(prettify(slider.data("to")));

            //updateValues(dataName, slider.data("from"), slider.data("to"));
        });

        $(this).bind("dataChange", function () {
            var from = $.isNumeric($(this).data("from")) ? $(this).data("from") : 1.7;
            var to = $.isNumeric($(this).data("to")) ? $(this).data("to") : 5.5;
            var min = $.isNumeric($(this).data("min")) ? $(this).data("min") : 1.0;
            var max = $.isNumeric($(this).data("max")) ? $(this).data("max") : 10.0;

            var slider = $(this).data("ionRangeSlider");
            if (slider != null) {
                slider.update({
                    min: min,
                    max: max,
                    from: from,
                    to: to,
                });

                $(this).trigger("change");
            }
        });

    });

    // Диапазон (элементы от и до)
    $(".js-filter-range__item").on('click', function () {
        var thisObject = $(this),
            parentMainBlock = thisObject.closest(".js-search-filter__form"),
            dataName = thisObject.data("name"),
            inputElement = $("input[name='" + dataName + "']", parentMainBlock),
            rangeElements = $(".js-filter-range__item.selected", parentMainBlock).filter(function (i, elem) { return $(elem).data("name") == dataName; }),
            rangeDelimiter = ":";

        var value = "";
        rangeElements.each(function () {
            var val = $(this).data("value");
            if (val != null && val.toString().length > 0) {
                if (value.length > 0) value = value + ":";
                value += val.toString();
            }
        });

        inputElement.val(value);
    });

    function updateValues(elem, min, max, IsUpdateSlider) {
        $("input[name='" + elem + "']").val((min + ':' + max));

        /*if (IsUpdateSlider == true) 
        {
            try
            {
                var s = $("#slider_" + elem);
                if (s.length > 0)
                {
                    s.slider({values: [min, max]});
                    s.slider("option", "slide").call(s, null, { values: s.slider("values") });
                }
            }
            catch (err) {}
        }*/
    }

    /* ---------------------------------------------------------- */

    // Попап фильтра
    $(".pull-filter-button").on("click", function (e) {
        e.preventDefault();
        $("#filter-window").fadeIn(100);
        $("body").addClass("active-window");
    });

    $("#close-window").on("click", function () {
        $("#filter-window").fadeOut(100);
        $("body").removeClass("active-window");
    });

    $(window).bind("resize", function () {
        if (window.innerWidth > 1449) {
            $("#filter-window").removeAttr("style");
            $("body").removeClass("active-window");
        }
    });

    // Плагин стилизации форм
    $("select:not(.nostyler)").styler();

    // Галерея превью товаров
    $(".thumb-item").brazzersCarousel();

    // Табличные объекты
    $(".trade").each(function () {
        var thisObject = $(this);

        if ($(".trade__img", thisObject).length > 1) {
            $(".numbers-slides", thisObject).addClass("active-number");
            $(".tmb-wrap-table div", thisObject).addClass("act");
        }

        $(".numbers-slides__all", thisObject).text($(".trade__img", thisObject).length);
        $(".numbers-slides__current", thisObject).text($(".tmb-wrap-table div.active", thisObject).index() + 1);
    });

    $(".trade__img-content").mousemove(function (e) {
        var thisObject = $(this),
            parentObject = thisObject.closest(".trade");

        $(".numbers-slides__current", parentObject).text($(".tmb-wrap-table div.active", parentObject).index() + 1);
    });

    // Линейные объекты	
    $(".trade-object").each(function () {
        var thisObject = $(this);

        if ($(".trade-object__img", thisObject).length > 1) {
            $(".numbers-slides", thisObject).addClass("active-number");
            $(".tmb-wrap-table div", thisObject).addClass("act");
        }

        $(".numbers-slides__all", thisObject).text($(".trade-object__img", thisObject).length);
        $(".numbers-slides__current", thisObject).text($(".tmb-wrap-table div.active", thisObject).index() + 1);
    });

    $(".trade-object__img-block").mousemove(function (e) {
        var thisObject = $(this),
            parentObject = thisObject.closest(".trade-object");

        $(".numbers-slides__current", parentObject).text($(".tmb-wrap-table div.active", parentObject).index() + 1);
    });

    // Табы
    /*
    $("#offers").easytabs({
        animate: false,
        updateHash: false,
        animationSpeed: 300,
        panelContext: $("#offers__wrapper"),
        defaultTab: ".offers__title-link-text:first-of-type",
        tabs: ".offers__title-link-text"
    });
    */

    // Карусель специалистов
    $("#specialists-carousel")
        .on("init", function (event, slick) {
            var countObjects = $(".specialists-carousel__item.slick-active").last();

            $("#specialists-count__all").text(slick.slideCount);
            $("#specialists-count__current").text(countObjects.data("index"));
        })
        .slick({
            slidesToShow: 4,
            slidesToScroll: 4,
            arrows: false,
            dots: true,
            appendDots: "#specialists-dots",
            centerMode: false,
            focusOnSelect: true,
            centerPadding: 0,
            infinite: false,
            swipe: true,
            responsive: [
                {
                    breakpoint: 1450,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3
                    }
                },
                {
                    breakpoint: 1200,
                    settings: {
                        slidesToShow: 2,
                        slidesToScroll: 2
                    }
                },
                {
                    breakpoint: 990,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 2,
                        slidesToScroll: 2
                    }
                },
                {
                    breakpoint: 480,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1
                    }
                }
            ]
        })
        .on("afterChange", function (event, slick, currentSlide) {
            var countObjects = $(".specialists-carousel__item.slick-active").last();

            $("#specialists-count__current").text(countObjects.data("index"));
        });

    // Мобильное меню
    function hide() {
        $("#mobile-nav").animate({
            left: -275
        }, 200).removeClass("active");
    }

    function display() {
        $("#mobile-nav").animate({
            left: 0
        }, 200).addClass("active");
    }

    $("#pull").on("click", function (e) {
        e.preventDefault();
        $("#bg-mobile").fadeIn(200);
        if ($("#mobile-nav").hasClass("active")) {
            hide();
        } else {
            display();
        }
    });

    $("#bg-mobile").on("click", function () {
        $(this).fadeOut(200);
        hide();
    });

    $(document).on("swiperight", function () {
        if (window.innerWidth < 990) {
            $("#bg-mobile").fadeIn(200);
            display();
        }

        if (window.innerWidth < 990 && $("div").is(".lk-user")) {
            $("body").addClass("body-nav");
            $("#lk-user-bg").fadeIn(200);
        }
    });

    $(document).on("swipeleft", function () {
        if (window.innerWidth < 990) {
            hide();
            $("#bg-mobile").fadeOut(200);
        }

        if (window.innerWidth < 990 && $("div").is(".lk-user")) {
            $("body").removeClass("body-nav");
            $("#lk-user-bg").fadeOut(200);
        }
    });

    $(window).on("resize", function () {
        if (window.innerWidth > 989) {
            $("#bg-mobile, #mobile-nav").removeAttr("style");
            $("#mobile-nav").removeClass("active");
        }

        if (window.innerWidth > 1449 && $("body").hasClass("body-nav")) {
            $("body").removeClass("body-nav");
            $("#lk-user-bg").fadeOut(200);
        }
    });

    $("#lk-header-nav__link").on("click", function (e) {
        e.preventDefault();
        $("body").addClass("body-nav");
        $("#lk-user-bg").fadeIn(200);
    });

    $("#lk-user-bg").on("click", function () {
        $("body").removeClass("body-nav");
        $(this).fadeOut(200);
    });

    // Попапы
    $(".link-popup").on("click", function (e) {
        e.preventDefault();

        var thisLink = $(this),
            idPopups = thisLink.data("id");

        $.arcticmodal("close");

        $("#" + idPopups).arcticmodal({
            overlay: {
                css: {
                    backgroundColor: "#222",
                    opacity: .85
                }
            }
        });
    });

    $(".nofollow").hover(function () {
        $(this).attr("href", $(this).data("url"))
    })

    $(".nofollow").click(function (e) {
        e.preventDefault();

        var blank = '_self';
        if (e.which == 2) var blank = '_blank';
        if ($(this).data('window') != undefined) blank = $(this).data('window');
        window.open($(this).data('url'), blank);
    })

    // Маска телефона
    $(".phone-input").mask("+7 (999) 999-99-99");

    // Скрытие/показ текста на странице агентства
    $(".agency__link-all").on("click", function (e) {
        e.preventDefault();

        var thisObject = $(this),
            parentObject = thisObject.closest(".agency-desc-block");

        if (!$(".text-hidden", parentObject).is(":visible")) {
            $(".text-hidden", parentObject).stop(true, true).slideDown(100);
            parentObject.addClass("active");
            thisObject.text("Свернуть");
        } else {
            var textLink = thisObject.data("text");
            $(".text-hidden", parentObject).stop(true, true).slideUp(100, function () {
                parentObject.removeClass("active");
                thisObject.text(textLink);
            });
        }
    });

    // Предложения на странице агентства
    var carousels = function (countView, blockCarousel) {
        $(blockCarousel)
            .on("init", function (event, slick) {
                var countObjects = $(".offer-carousel__item.slick-active").last();

                $("#offers-count__all").text(slick.slideCount);
                $("#offers-count__current").text(countObjects.data("index"));
            })
            .slick({
                slidesToShow: countView,
                slidesToScroll: 4,
                arrows: false,
                dots: true,
                appendDots: "#offers-dots",
                centerMode: false,
                focusOnSelect: false,
                centerPadding: 0,
                infinite: false,
                swipe: true,
                responsive: [
                    {
                        breakpoint: 1450,
                        settings: {
                            slidesToShow: 3,
                            slidesToScroll: 3
                        }
                    },
                    {
                        breakpoint: 1200,
                        settings: {
                            slidesToShow: 2,
                            slidesToScroll: 2
                        }
                    },
                    {
                        breakpoint: 600,
                        settings: {
                            slidesToShow: 1,
                            slidesToScroll: 1
                        }
                    }
                ]
            })
            .on("afterChange", function (event, slick, currentSlide) {
                var countObjects = $(".offer-carousel__item.slick-active").last();

                $("#offers-count__current").text(countObjects.data("index"));
            });
    };

    $("#agency-offers").easytabs({
        animate: false,
        updateHash: false,
        animationSpeed: 300,
        panelContext: $("#offers__wrapper"),
        defaultTab: ".offers__title-link-text:first-of-type",
        tabs: ".offers__title-link-text"
    })
        .bind("easytabs:after", function () {
            var tabActive = $(".offer-content.active"),
                sliderBlock = $(".offer-carousel__list", tabActive);

            carousels(4, sliderBlock);
        })
        .bind("easytabs:before", function () {
            var tabActive = $(".offer-content.active"),
                sliderBlock = $(".offer-carousel__list", tabActive);

            if ($(".offer-content .offer-carousel__list").hasClass("slick-slider")) {
                sliderBlock.slick("unslick");
            }

        });

    carousels(4, ".offer-content.active .offer-carousel__list");

    // Слидер диапазона цен
    $(".sort-slider__block").each(function () {
        var parentBlock = $(this).closest(".filter-elements-string");
        var slider = $(this).ionRangeSlider({
            type: "double",
            grid: false,
            min: 1.0,
            max: 10.0,
            from: 1.7,
            to: 5.5,
            step: .2,
            hide_min_max: true,
            onChange: function (data) {
                parentBlock.addClass("active");
            },
            onFinish: function (data) {
                parentBlock.removeClass("active");
            }
        });

        $(".sort-from", parentBlock).val(slider.data("from") + " млн");
        $(".sort-before", parentBlock).val(slider.data("to") + " млн");

        slider.on("change", function () {
            $(".sort-from", parentBlock).val(slider.data("from") + " млн");
            $(".sort-before", parentBlock).val(slider.data("to") + " млн");
        });
    });

    // Добавление класса родителю инпутов на странице подбора
    $(".sort-input").focus(function () {
        var parentObject = $(this).closest(".sort-input-bar");
        parentObject.addClass("active");
    });

    $(".sort-input").blur(function () {
        var parentObject = $(this).closest(".sort-input-bar");
        parentObject.removeClass("active");
    });

    // Скрытие/показ дополнительных фильтров на странице подбора
    $("#link-dop-sort").on("click", function (e) {
        e.preventDefault();

        var thisObject = $(this),
            thisObjectText = thisObject.data("text");

        if (!thisObject.hasClass("active")) {
            $(".sort-string-hidden").stop(true, true).slideDown(100);
            thisObject.addClass("active").text("Скрыть");
        } else {
            $(".sort-string-hidden").stop(true, true).slideUp(100);
            thisObject.removeClass("active").text(thisObjectText);
        }
    });

    // Галерея на странице объекта
    $("#images-gallery__slides").slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: true,
        dots: false,
        appendArrows: "#gallery-arrows",
        centerMode: false,
        focusOnSelect: false,
        centerPadding: 0,
        asNavFor: "#images-preview__list",
        fade: true
    });

    $("#images-preview__list").slick({
        slidesToShow: 3,
        slidesToScroll: 1,
        asNavFor: "#images-gallery__slides",
        arrows: false,
        dots: false,
        centerMode: false,
        focusOnSelect: true,
        centerPadding: 0,
        infinite: true,
        vertical: true,
        swipe: false,
        responsive: [
            {
                breakpoint: 1450,
                settings: {
                    vertical: false,
                    slidesToShow: 4
                }
            },
            {
                breakpoint: 480,
                settings: {
                    vertical: false,
                    slidesToShow: 2
                }
            }
        ]
    });

    // Карта на странице объекта
    if ($("div").is("#map")) {
        ymaps.ready(init);

        var myMap,
            myPlacemark,
            blockMapContent = $("div#map").closest("div.js-map-object__content");

        function init() {
            var center = null;
            var items = new Array();
            $("div.js-map-object__content_item", blockMapContent).each(function () {
                var thisObject = $(this),
                    coordinateX = parseFloat(thisObject.data("coordinatex")),
                    coordinateY = parseFloat(thisObject.data("coordinatey"));

                if (!isNaN(coordinateX) && !isNaN(coordinateY)) {
                    if (center == null) center = [coordinateX, coordinateY];
                    items[items.length] = { coordinates: [coordinateX, coordinateY] };
                }
            });

            var mapOptions = {
                center: [0, 0],
                zoom: 17,
                controls: ['zoomControl']
            };
            if (center != null) mapOptions.center = center;

            myMap = new ymaps.Map("map", mapOptions);

            for (var item in items) {
                var myPlacemark = new ymaps.Placemark(
                    items[item].coordinates, {}, {
                        iconLayout: "default#image",
                        iconImageHref: "/data/images/placemark-address.svg",
                        iconImageSize: [45, 67],
                        iconImageOffset: [-22, -67]
                    }
                );
                myMap.geoObjects.add(myPlacemark);
            }

            myMap.behaviors.disable("scrollZoom");
        }
    }

    // Карусель на странице объекта
    $("#offer-carousel__list").on("init", function (event, slick) {
        var countObjects = $(".offer-carousel__item.slick-active").last();

        $("#offers-count__all").text(slick.slideCount);
        $("#offers-count__current").text(countObjects.data("index"));
    }).slick({
        slidesToShow: 4,
        slidesToScroll: 4,
        arrows: true,
        dots: false,
        appendArrows: "#offers-arrows",
        centerMode: false,
        focusOnSelect: false,
        centerPadding: 0,
        infinite: false,
        swipe: true,
        responsive: [
            {
                breakpoint: 1450,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3
                }
            },
            {
                breakpoint: 1200,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    }).on("afterChange", function (event, slick, currentSlide) {
        var countObjects = $(".offer-carousel__item.slick-active").last();

        $("#offers-count__current").text(countObjects.data("index"));
    });

    // Скрытие/показ фильтра карты
    $("#close-filter").on("click", function () {
        $("body").toggleClass("active-body");
    });

    // Табы в фильтре карты
    $("#map-filter-tabs").easytabs({
        animate: false,
        updateHash: false,
        animationSpeed: 300,
    })
        .bind("easytabs:after", function (event, $clicked, $targetPanel, settings) {
            $(".map-buttons-variables__link.active").removeClass("active");
            $("#" + $clicked.data("id")).addClass("active");
        });

    $(".map-buttons-variables__link").on("click", function (e) {
        e.preventDefault();
        var thisAttr = $(this).attr("href");
        $("#map-filter-tabs").easytabs("select", thisAttr);
        $(".map-buttons-variables__link.active").removeClass("active");
        $(this).addClass("active");
    })

    // Фильтр карты
    $(".variables-filter-map__label").on("click", function () {
        var thisObject = $(this),
            parentObject = thisObject.closest(".variables-filter-map__item");

        if (!parentObject.hasClass("active")) {
            $(".variables-filter-map__item.active").removeClass("active");
            parentObject.addClass("active");

            $("#map-filter__content").addClass("active-filter");
        } else {
            parentObject.removeClass("active");
            $("#map-filter__content").removeClass("active-filter");
        }
    });

    $("#filter-map-link-dop").on("click", function (e) {
        e.preventDefault();
        if (!$(this).hasClass("active")) {
            $(".hidden-filter-map").slideDown(100);
            $(this).text("Скрыть доп. фильтры").addClass("active");
        } else {
            $(".hidden-filter-map").slideUp(100);
            $(this).text($(this).data("text")).removeClass("active");
        }
    });

    // Скрытие/показ плавающего блока с фильтром
    $(window).bind("load scroll", function () {
        if ($("div").is("#float-panel")) {
            var coords = $(this).scrollTop(),
                positionPink = $("#pink").offset().top;
            if (coords > positionPink) {
                $("#float-panel").stop(true, true).fadeIn(200);
            }
            else {
                $("#float-panel").stop(true, true).fadeOut(200);
            }
        }
    });

    // Подключение стилизации скролла
    /*$(".scroll-bar").mCustomScrollbar({
        axis: "y",
        scrollInertia: 1000,
        autoHideScrollbar: true
    });

    $(".scroll-content").mCustomScrollbar({
        axis: "y",
        scrollInertia: 1000,
        autoHideScrollbar: false
    });
    */

    // Подключение автокомплита
    var citiesAutocomplete = [
        "Апрелевка",
        "Голицыно",
        "Домодедово",
        "Дубна",
        "Жуковский",
        "Зарайск",
        "Звенигород",
        "Кашира",
        "Клин",
        "Коломна"
    ];

    var addressAutocomplete = [
        "Советская ул.",
        "Софьи Перовской ул.",
        "Сосновая ул."
    ];

    $(".autocomplete").each(function () {
        var parentObject = $(this).closest(".lk-edit-bar__autocomplete"),
            listObjects;

        if ($(this).data("type") === "cities") {
            listObjects = citiesAutocomplete;
        } else if ($(this).data("type") === "address") {
            listObjects = addressAutocomplete;
        }

        $(this).autocomplete({
            source: listObjects,
            appendTo: parentObject
        });
    });

    // Реализация переключателя в личном кабинете
    $(".switch").each(function () {
        if (!$(this).hasClass("active")) {
            $(".switch-no", this).prop("checked", true);
        } else {
            $(".switch-yes", this).prop("checked", true);
        }
    });

    $(".switch").on("click", function () {
        if (!$(this).hasClass("active")) {
            $(this).addClass("active");
            $(".switch-yes", this).prop("checked", true);
        } else {
            $(this).removeClass("active");
            $(".switch-no", this).prop("checked", true);
        }
    });

    // Попапы
    $(".link-requestmembership").on("click", function (e) {
        e.preventDefault();
        var tt = $(this);

        $.requestJSON("/realty/RequestMembershipSave/" + $(this).data("id"), null, function (result, message, data) {
            var isOk = false;

            if (result == JsonResult.OK) {
                try {
                    isOk = data;
                }
                catch (err) { console.log("requestmembership: " + err); }
            }

            if (data == true) {
                tt.hide();
                alert("Заявка на членство в агентстве подана! Ждите ответа по электронной почте или смс (в зависимости от данных, указанных в профиле). " + message);
            }
            else {
                tt.show();
                alert("Заявка на членство в агентстве не подана! Возможно, произошла ошибка. Обратитесь в техническую поддержку. " + message);
            }

        })

    });

    //DropDown для чекбоксов
    $(".lk-edit-form__dropdown").each(function () {
        prepareDropDownList($(this));
    })
    function prepareDropDownList(obj) {
        obj.find(".custom-select").on("click", function () {
            var block = $(this).closest(".lk-edit-form__dropdown"),
                box = block.find(".custom-select-option-box");
            box.toggle();
            if (box.is(":visible")) block.addClass("opened");
            else block.removeClass("opened");
        });

        function toggleFillColor(obj) {
            var block = $(obj).closest(".lk-edit-form__dropdown"),
                box = block.find(".custom-select-option-box"),
                select = block.find("select"),
                option = select.find("option[value='" + obj.val() + "']")

            box.show();

            if ($(obj).prop('checked') == true) {
                $(obj).parent().addClass("active");
            } else {
                $(obj).parent().removeClass("active");
            }

            if (option.prop('selected') == false) {
                option.prop('selected', true)
            } else {
                option.prop("selected", false);
            }

            countSelected(block);
        }

        $(".custom-select-option").on("click", function (e) {
            var checkboxObj = $(e.target).closest(".custom-select-option").children("input");
            if (!$(e.target).hasClass("custom-select-option-checkbox")) {
                if ($(checkboxObj).prop('checked') == false) {
                    $(checkboxObj).prop('checked', true)
                } else {
                    $(checkboxObj).prop("checked", false);
                }
            } else {

            }
            toggleFillColor(checkboxObj);
        });

        $(".custom-select-option-checkbox").on('change', function () {
                select = block.find("select");

            alert($(this).val())
        })

        function countSelected(obj) {
            var elements = obj.find("select option:selected"),
                count = elements.length,
                label = obj.find(".custom-select-text");

            if (count == 0) label.text("Не выбрано");
            else if (count == 1) label.text(elements.text());
            else {
                label.text(obj.find(".custom-select-option-box input:checked:first").closest(".custom-select-option").text() + " и еще " + (count - 1));
            }
        }

        countSelected(obj);
    }
    
    $("body")
        .on(
            "click",
            function (e) {
                if (e.target.className != "custom-select"
                    && !$(e.target).closest(".custom-select").length
                    && !$(e.target).closest(".custom-select-option-box").length) {
                    $(".custom-select-option-box").hide();
                    $(".custom-select-option-box").closest(".lk-edit-form__dropdown").removeClass("opened");
                }
            });

    $(".lk-edit-form__dropdown").each(function () {
        var block = $(this),
            select = block.find("select"),
            list = block.find("ul"),
            newList = $(
                '<div class="checkbox-dropdown-container">'
               +'   <div>'
               +'       <div class="custom-select">'
               +'           <div class="custom-select-text jq-selectbox__select-text">Не выбрано</div>'
               +'           <div class="jq-selectbox__trigger"><div class="jq-selectbox__trigger-arrow"></div></div>'
               +'       </div>'
               +'       <div class="custom-select-option-box"></div>'
               +'   </div>'
               +'</div>'
            ),
            newListSet = newList.find(".custom-select-option-box");

        list.after(newList);
        list.remove();

        select.find("option").each(function () {
            var checked = ($(this).attr("selected") ? true : false);

            newListSet.append(
                '<div class="custom-select-option' + (checked ? " active" : "") + '">'
               +'   <input class="custom-select-option-checkbox" type="checkbox" value="' + $(this).val() + '" '+ (checked ? "checked" : "" ) + '> ' + $(this).text()
               +'</div>'
            );
        })

        prepareDropDownList(block);
    })

    // Запрос номера телефона объявления по нажатию кнопки-ссылки
    $(".js-object-agent__link-phone").click(function (e) {
        e.preventDefault();

        var thisObject = $(this),
            IdObject = thisObject.data("id");

        $.requestJSON("/realty/AgencyObjectPhone/" + IdObject, null, function (result, message, data) {
            if (result == JsonResult.OK) {
                thisObject.attr("href", "tel:" + data).text(data);
                thisObject.removeClass("js-object-agent__link-phone");
                thisObject.unbind('click');
            }

            if (message.length > 0) alert(message);
        });
    });

    $(".js-header-handsend-object").click(function (e) {
        e.preventDefault();

        var thisLink = $(this),
            idPopups = thisLink.data("id");

        $.arcticmodal("close");

        $("#" + idPopups).arcticmodal({
            overlay: {
                css: {
                    backgroundColor: "#222",
                    opacity: .85
                }
            }
        });

        /*var thisObject = $(this);

        $("div#popup-call").showDialog({
            buttons: ShowDialogButtons.NOBUTTONS,
            closeOnPressEscape: true,
            closeOnClickOutOfForm: true,
            show: function () {
                var thisObject = $(this),
                    thisForm = $("form", thisObject);

                thisForm.requestJSON("destroy");
                thisForm.requestJSON({
                    before: function () {

                    },
                    after: function (result, message) {
                        if (message.length > 0) alert(message);
                        if (result == JsonResult.OK) {
                            thisObject.showDialog("destroy");
                        }
                    }
                });
            }
        });
        */
    });

    //$("js-sort-apply")
});

function number_format(number, decimals, dec_point, thousands_sep)    // Format a number with grouped thousands
{
    // 
    // +   original by: Jonas Raoni Soares Silva (http://www.jsfromhell.com)
    // +   improved by: Kevin van Zonneveld (http://kevin.vanzonneveld.net)
    // +     bugfix by: Michael White (http://crestidg.com)

    var i, j, kw, kd, km;

    // input sanitation & defaults
    if (isNaN(decimals = Math.abs(decimals)))
    {
        decimals = 2;
    }
    if (dec_point == undefined)
    {
        dec_point = ",";
    }
    if (thousands_sep == undefined)
    {
        thousands_sep = ".";
    }

    i = parseInt(number = (+number || 0).toFixed(decimals)) + "";

    if ((j = i.length) > 3)
    {
        j = j % 3;
    } else
    {
        j = 0;
    }

    km = (j ? i.substr(0, j) + thousands_sep : "");
    kw = i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousands_sep);
    //kd = (decimals ? dec_point + Math.abs(number - i).toFixed(decimals).slice(2) : "");
    kd = (decimals ? dec_point + Math.abs(number - i).toFixed(decimals).replace(/-/, 0).slice(2) : "");


    return km + kw + kd;
}
