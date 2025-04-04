function GerarGraficoPizza(container, data) {
    Highcharts.chart(container, {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: 'Colaboradores por cargo'
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b><b>{series.quantidade}</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            name: '<b>Percentual</b>',
            colorByPoint: true,
            data: data
        }]
    });


};

function GerarGraficoPizzaEfetivo(container, data) {
    Highcharts.chart(container, {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: 'Efetivo diário nos ultimos 30 dias'
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b><b>{series.quantidade}</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            name: '<b>Percentual</b>',
            colorByPoint: true,
            data: data
        }]
    });


};


function GerarGraficoBarra2(container, dataCategories, dataValues, colors) {

    Highcharts.chart(container, {
        chart: {
            type: 'bar'
        },
        title: {
            text: 'Tarefas por status'
        },
        xAxis: {
            categories: dataCategories,
            title: {
                text: null
            }
        },

        yAxis: {
            min: 0,
            title: {
                text: 'Tarefas (quantidade)',
                align: 'high'
            },
            labels: {
                overflow: 'justify'
            }
        },
        tooltip: {
            valueSuffix: ' '
        },
        plotOptions: {
            bar: {
                dataLabels: {
                    enabled: true
                }
            },
            series: {
                colorByPoint: true
            }
        },
        colors: colors,


        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'top',
            x: -40,
            y: 80,
            floating: true,
            borderWidth: 1,
            backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
            shadow: true
        },
        credits: {
            enabled: false
        },
        series: [{

            showInLegend: false,
            name: 'Tarefas',
            data: dataValues
        }]

    });

};


function GerarGraficoBarra(container, dataCategories, dataValues) {

    Highcharts.chart(container, {
        chart: {
            type: 'bar'
        },
        title: {
            text: 'Tarefas por status'
        },
        xAxis: {
            categories: dataCategories,
            title: {
                text: null
            }
        },
       
        yAxis: {
            min: 0,
            title: {
                text: 'Tarefas (quantidade)',
                align: 'high'
            },
            labels: {
                overflow: 'justify'
            }
        },
        tooltip: {
            valueSuffix: ' '
        },
        plotOptions: {
            bar: {
                dataLabels: {
                    enabled: true
                }
            },
            series: {
                colorByPoint: true
            }
        },
        colors: [
            '#ffcc00', 
            '#AA4643', 
            '#89A54E', 
            '#80699B', 
            '#3D96AE', 
            '#DB843D', 
            '#92A8CD', 
            '#A47D7C', 
            '#B5CA92'
        ],


        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'top',
            x: -40,
            y: 80,
            floating: true,
            borderWidth: 1,
            backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
            shadow: true
        },
        credits: {
            enabled: false
        },
        series: [{
           
            showInLegend: false,
            name: 'Tarefas',
            data: dataValues
        }]

    });

};

function GerarGraficoBar(container, data) {
    Highcharts.chart(container, {
        chart: {
            type: 'bar'
        },
        title: {
            text: ''
        },
        xAxis: {
            categories: data.categories,
            title: {
                text: null
            },
        },
        yAxis: {
            min: 0,
            allowDecimals: false,
            title: {
                text: '',
                align: 'high'
            },
            labels: {
                overflow: 'justify'
            }
        },
        plotOptions: {
            bar: {
                dataLabels: {
                    enabled: true
                }
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'top',
            x: -40,
            y: 80,
            floating: true,
            borderWidth: 1,
            backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
            shadow: true
        },
        credits: {
            enabled: false
        },
        series: data.series
    });
}
//ACCORDION DAS TAREFAS===================================
$(document).arrive('.accordion', function () {
    var i = 0;
    var accordion = $(this);
    accordion.find(".panel").each(function () {
        $(this).find(".panel-heading > h4 a").attr("children", ".item-ac" + i);
        $(this).find(".panel-collapse").addClass("item-ac" + i);
        i++;
        $(this).find(".panel-heading > h4 a").click(function () {
            var colapsedElement = accordion.find($(this).attr("children"));
            if (colapsedElement.is(":hidden")) {
                $(this).addClass('active');
                colapsedElement.collapse('show');
            } else {
                $(this).removeClass('active');
                colapsedElement.collapse('hide');

            }
        });

    });
});


$(document).ready(function () {

    $(document).arrive('.datepicker-here', function () {
        var el = this;
        $(el).datepicker({
            autoClose: true,
            language: "pt-BR",
            onSelect: function () {
                $(el).trigger('input'); // For Angular update its model. Use for Chrome/Firefox/Edge
                $(el).trigger('change'); // For Angular update its model. Use for Chrome/Firefox/Edge + IE11
            }
        });
    });


    $(document).arrive('.datepicker-hour-here', function () {
        var el = this;
        $(el).datepicker({
            autoClose: true,
            language: "pt-BR",
            timepicker: true,
            onSelect: function () {
                $(el).trigger('input'); // For Angular update its model. Use for Chrome/Firefox/Edge
                $(el).trigger('change'); // For Angular update its model. Use for Chrome/Firefox/Edge + IE11
            }
        });
    });

    $(document).arrive('.navbar .menu-lateral li > a', function () {
        $(this).click(function () {

            if (!$(this).hasClass('sub')) {
                $(".nav-mobile #menu-toggle").trigger('click');
            }
        });
    });

    $(document).arrive('.menu-sidebar', function () {


        //ao clicar em um drop menu os outros fecham=======
        $(".topo .collapsed-menu").click(function () {
            var parent = $(this).parent().find("ul").length;
            if (parent > 0) {
                $(".topo .collapse.in").collapse("hide");
            }
        });

        //links dinamicos para itens do accordion
        var i = 0;
        $(".menu-sidebar ul li").each(function () {
            if ($(this).find("ul").length) {
                $(this).find("a").addClass("sub");
                $(this).find("a.collapsed").attr("collapse-open", ".item-ac" + i);
                $(this).find(".panel-collapse").addClass("item-ac" + i);

                $(this).find("a.collapsed").click(function () {
                    console.log($(this).attr('collapse-open'));
                    $($(this).attr('collapse-open')).collapse('toggle');
                });

                i++;

            } else {
                $(this).find("a").removeAttr("data-toggle");
            }
        });




        //Ao clica no item de accordion acrescentar a classe active para saber que está aberto
        $('.menu-sidebar ul a').click(function () {
            $(".menu-sidebar ul a").parent().removeClass('active');
            $(this).parent().toggleClass('active');
        });

        //focus em todos os primeiros inputs================
        $(document).arrive('input', function () {
            if ($(this).hasClass('newInput')) {
                console.log('');   
            }
            else {
                $(':input:enabled:visible:first').focus();
                $(':input:enabled:visible:first').focus();
            }
            
        });


        $(document).arrive('.status', function () {
            //AO CLICAR NO BOTAO DE STATUS DO CARD DE TAREFAS ABRIR BOTOES DE STATUS

            $(".bt-status").click(function (event) {
                $(this).parents('.card').find(".status").addClass('active');
                event.stopPropagation();
            });
            $(".status a").click(function () {
                //var nomeClass = $(this).attr("data-class");
                //$(this).parents('.card:first').find('.head').removeClass("bg-cinza").removeClass("bg-azul").removeClass("bg-verde").removeClass("bg-vermelho").addClass(nomeClass);
                $(".lista-tarefas .status").delay().removeClass('active');

            });
            $('html').click(function () {
                $(".lista-tarefas .status.active").removeClass("active");
            });

        });

    });


    $(document).arrive('.btn-view', function () {
        //ativar ou desativar modo de visualização
        $(document).on("click", ".btn-card", function () {
            $(".btn-view i").removeClass("fa-table").addClass("fa-columns");
            $(".btn-view span").text("LISTA");
            $(".btn-card").removeClass("btn-card").addClass("btn-lista");

        });
        $(document).on("click", ".btn-lista", function () {
            $(".btn-view i").removeClass("fa-columns").addClass("fa-table");
            $(".btn-view span").text("CARDS");
            $(".btn-lista").removeClass("btn-lista").addClass("btn-card");

        });
    });

    $(document).arrive('.lista-tarefas', function () {
        //ao marcar seleciona todos os checkboxs da lista ==========
        //generico=
        $('#selecionar-tudo').click(function () {
            if ($("#selecionar-tudo input").is(':checked')) {
                $('.selecionar .checkbox input[type=checkbox]').prop('checked', true);
            } else {
                $('.selecionar .checkbox input[type=checkbox]').attr('checked', false);
            }
        });
       

        //para cards=
        $('#selecionar-tudo').click(function () {
            if ($("#selecionar-tudo input").is(':checked')) {
                $('.card .checkbox input[type=checkbox]').prop('checked', true);
            } else {
                $('.card .checkbox input[type=checkbox]').attr('checked', false);
            }
        });
        //para tabela==
        $('.selecionar-tudo-tabela').click(function () {
            if ($(this).find("input").is(':checked')) {
                $(this).parents(".table").find('tbody .checkbox input[type=checkbox]').prop('checked', true);;

            } else {
                $(this).parents(".table").find('tbody .checkbox input[type=checkbox]').attr('checked', false);
            }
        });
    });

    $(document).arrive('.btn-blue', function () {

       toastr.options = {
           
            "showDuration": "1500",
            "hideDuration": "1000",
            "timeOut": "2500"
            
        }
        //toastr.options = {
        //    "closeButton": false,
        //    "debug": false,
        //    "newestOnTop": false,
        //    "progressBar": false,
        //    "positionClass": "toast-top-right",
        //    "preventDuplicates": false,
        //    "onclick": null,
        //    "showDuration": "800",
        //    "hideDuration": "88888",
        //    "timeOut": "999999999",
        //    "extendedTimeOut": "9999999",
        //    "showMethod": "fadeIn"
        //}
    });

    $(document).arrive('.lista-tarefas .card input', function () {
        //selecionar card ( aplicaçoa da sombra e contagem de itens selecionados )
        $('.lista-tarefas .card div.checkbox input:checkbox').change(function (e) {

            //const parent = $(this).parents('.card:first');

            //const count = $(this).parent().parent();

            //// const hasClass = parent.hasClass("checked-count");
            //const hasClass = count.hasClass("count");

            //if (hasClass) {
            //    count.removeClass("count");
            //    parent.removeClass("checked-count");
            //} else {
            //    count.addClass("count");
            //    parent.addClass("checked-count");
            //}

            //$(".contador-selecionados strong").html($(".count").length);

            //if ($(".count").length) {
            //    $(".contador-selecionados").addClass("active");
            //}
            //if (!$(".count").length) {
            //    $(".contador-selecionados").removeClass("active");
            //}
        });
    });

    $(document).arrive('input[type=number]', function () {
        var el = this;
        var max = $(el).prop('maxlength');
        if (typeof (max) != "undefined") {
            el.oninput = function () {
                if (this.value.length > parseInt(max)) {
                    this.value = this.value.slice(0, parseInt(max));
                }
            }
        }
    });

    $(document).arrive('.datepicker-here', function () {


        $('input.datepicker-here, input.datepicker-hour-here').on("focus", function () {
            var $modal = $(this).parents('.modal:first');
            if ($modal.length > 0) {
                var _datepicker = $(this).data('datepicker');
                _datepicker.$datepicker.parent().addClass('in-modal');
                $modal.off('scroll.datepicker').on('scroll.datepicker', function () {
                    //_datepicker.update({position: "bottom left"});

                    var iFits = false;
                    // Loop through a few possible position and see which one fits
                    $.each(['right center', 'right bottom', 'right top', 'top center', 'bottom center'], function (i, pos) {
                        if (!iFits) {
                            _datepicker.update('position', pos);
                            var fits = isElementInViewport(_datepicker.$datepicker[0]);
                            if (fits.all) {
                                iFits = true;
                            }
                        }
                    });

                });

                setTimeout(function () { //correção de bug ao abrir imediatamente
                    $modal.trigger('scroll.datepicker');
                }, 1);

                $modal.off('hidden.bs.modal').on('hidden.bs.modal', function () {
                    $modal.off('scroll.datepicker');
                    _datepicker.$datepicker.parent().removeClass('in-modal');
                });
            }
        });


    });
    //função para verificar se o datepiker se encaixa dentro da viewport:
    function isElementInViewport(el) {
        var rect = el.getBoundingClientRect();
        var fitsLeft = (rect.left >= 0 && rect.left <= $(window).width());
        var fitsTop = (rect.top >= 0 && rect.top <= $(window).height());
        var fitsRight = (rect.right >= 0 && rect.right <= $(window).width());
        var fitsBottom = (rect.bottom >= 0 && rect.bottom <= $(window).height());
        return {
            top: fitsTop,
            left: fitsLeft,
            right: fitsRight,
            bottom: fitsBottom,
            all: (fitsLeft && fitsTop && fitsRight && fitsBottom)
        };
    }



    $(document).arrive('.scrollbar-inner', function () {
        //script barra rolagem custumizada
        jQuery('.scrollbar-inner').scrollbar();

    });

    $(document).arrive('.avatar-upload ', function () {
        function readURL(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    //$('.avatar-upload .image').css('background-image', 'url(' + e.target.result + ')');
                    $('#imagePreview').attr("src", e.target.result);
                    $('.avatar-upload .image').hide();
                    $('.avatar-upload .image').fadeIn(650);
                }
                reader.readAsDataURL(input.files[0]);
            }
        }
        $(".upimage").change(function () {
            readURL(this);


        });

    });
    





});

$(document).arrive('.topo', function () {

    //ao clicar em um drop menu os outros fecham=======
    $(".topo .collapsed-menu").click(function () {
        var parent = $(this).parent().find("ul").length;
        if (parent > 0) {
            $(".topo .collapse.in").collapse("hide");
        }
    });//.trigger();
});


/*! jquery.validate extension begin */
$(document).ready(function () {
    if (jQuery.validator != undefined) {
        jQuery.extend(jQuery.validator.messages, {
            required: "Campo de preenchimento obrigat&oacute;rio.",
            remote: "Por favor, corrija este campo.",
            email: "Formato de E-mail inv&aacute;lido!",
            url: "Por favor, introduza um URL v&aacute;lido.",
            date: "Por favor, introduza uma data v&aacute;lida.",
            dateISO: "Por favor, introduza uma data v&aacute;lida (ISO).",
            number: "Por favor, introduza um n&uacute;mero v&aacute;lido.",
            digits: "Por favor, introduza apenas d&iacute;gitos.",
            creditcard: "Por favor, introduza um n&uacute;mero de cart&atilde;o de cr&eacute;dito v&aacute;lido.",
            equalTo: "Por favor, introduza de novo o mesmo valor.",
            accept: "Por favor, introduza um ficheiro com uma extens&atilde;o v&aacute;lida.",
            maxlength: jQuery.validator.format("Por favor, n&atilde;o introduza mais do que {0} caracteres."),
            minlength: jQuery.validator.format("Por favor, introduza pelo menos {0} caracteres."),
            rangelength: jQuery.validator.format("Por favor, introduza entre {0} e {1} caracteres."),
            range: jQuery.validator.format("Por favor, introduza um valor entre {0} e {1}."),
            max: jQuery.validator.format("Por favor, introduza um valor menor ou igual a {0}."),
            min: jQuery.validator.format("Por favor, introduza um valor maior ou igual a {0}.")
        });

        function ExtendValidation() {
            var validationsArray = [];
            validationsArray.push("monthRange");
            validationsArray.push("cpf");
            validationsArray.push("cnpj");
            validationsArray.push("dateRange");
            validationsArray.push("dateITA");
            validationsArray.push("dateBR");
            validationsArray.push("phone-validator");

            //ja existem
            validationsArray.push("required");
            validationsArray.push("url");
            validationsArray.push("email");
            validationsArray.push("number");
            validationsArray.push("date");
            validationsArray.push("creditcard");

            jQuery.validator.addMethod("cpf", function (value, element) {

                var cpf = value.replace(/[^\d]+/g, '');

                if (cpf == '') return false;
                var numeros, digitos, soma, i, resultado, digitos_iguais;
                digitos_iguais = 1;
                if (cpf.length < 11) {
                    return false;
                }
                for (i = 0; i < cpf.length - 1; i++) {
                    if (cpf.charAt(i) != cpf.charAt(i + 1)) {
                        digitos_iguais = 0;
                        break;
                    }
                }
                if (!digitos_iguais) {
                    numeros = cpf.substring(0, 9);
                    digitos = cpf.substring(9);
                    soma = 0;
                    for (i = 10; i > 1; i--) {
                        soma += numeros.charAt(10 - i) * i;
                    }
                    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                    if (resultado != digitos.charAt(0)) {
                        return false;
                    }
                    numeros = cpf.substring(0, 10);
                    soma = 0;
                    for (i = 11; i > 1; i--) {
                        soma += numeros.charAt(11 - i) * i;
                    }
                    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                    if (resultado != digitos.charAt(1)) {
                        return false;
                    }
                    return this.optional(element) || /(^$)|([0-9]{3}\.?[0-9]{3}\.?[0-9]{3}\-?[0-9]{2})/.test(value);
                }
                else {
                    return false;
                }
            }, "CPF inválido!");

            jQuery.validator.addMethod("dateBR", function (value, element) {
                if (value.length == 0) {
                    return true;
                }

                if (!/^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$/.test(value)) {
                    return false;
                }
                return true;
            }, "Data inválida");

            jQuery.validator.addMethod("cnpj", function (value, element) {

                var cnpj = value.replace(/[^\d]+/g, '');

                if (cnpj == '') return true;

                if (cnpj.length != 14)
                    return false;

                // Valida DVs
                tamanho = cnpj.length - 2
                numeros = cnpj.substring(0, tamanho);
                digitos = cnpj.substring(tamanho);
                soma = 0;
                pos = tamanho - 7;
                for (i = tamanho; i >= 1; i--) {
                    soma += numeros.charAt(tamanho - i) * pos--;
                    if (pos < 2)
                        pos = 9;
                }
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(0))
                    return false;

                tamanho = tamanho + 1;
                numeros = cnpj.substring(0, tamanho);
                soma = 0;
                pos = tamanho - 7;
                for (i = tamanho; i >= 1; i--) {
                    soma += numeros.charAt(tamanho - i) * pos--;
                    if (pos < 2)
                        pos = 9;
                }
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(1))
                    return false;

                return this.optional(element) || /(^$)|([0-9]{2}\.?[0-9]{3}\.?[0-9]{3}\/?[0-9]{4}\-?[0-9]{2})/.test(value);

            }, "CNPJ inválido!");

            jQuery.validator.addMethod("dateRange", function (value, element) {
                var lowerDate = $(element).val();
                var parent = $(element).parent(".dateRange-group");
                var higherDate = parent.find("input.date-to").eq(0).val();

                if (moment(lowerDate, "DD/MM/YYYY", true).isValid()) {
                    if (!moment(higherDate, "DD/MM/YYYY", true).isValid()) { return false; }
                } else {
                    if (higherDate == '' && lowerDate == '') {
                        return true;
                    }
                    return false;
                }

                if (moment(higherDate, "DD/MM/YYYY", true).isAfter(moment(lowerDate, "DD/MM/YYYY", true)) || lowerDate == higherDate) {
                    return true;
                } else {
                    return false;
                }

            }, "Intervalo de datas inválido.");

            jQuery.validator.addMethod("monthRange", function (value, element) {
                var lowerDate = $(element).val();
                var parent = $(element).parent(".monthRange-group");
                var higherDate = parent.find("input.date-to").eq(0).val();

                if (moment(lowerDate, "MM/YYYY", true).isValid()) {
                    if (!moment(higherDate, "MM/YYYY", true).isValid()) { return false; }
                } else {
                    if (higherDate == '' && lowerDate == '') {
                        return true;
                    }
                    return false;
                }

                if (moment(higherDate, "MM/YYYY", true).isAfter(moment(lowerDate, "MM/YYYY", true)) || lowerDate == higherDate) {
                    return true;
                } else {
                    return false;
                }

            }, "Intervalo de datas inválido.");

            jQuery.validator.addMethod('phone-validator', function (value, element) {
                value = value.replace("(", "");
                value = value.replace(")", "");
                value = value.replace("-", "");
                value = value.replace(" ", "").trim();
                if (value == '0000000000') {
                    return (this.optional(element) || false);
                } else if (value == '00000000000') {
                    return (this.optional(element) || false);
                }
                if (["00", "01", "02", "03", , "04", , "05", , "06", , "07", , "08", "09", "10"].indexOf(value.substring(0, 2)) != -1) {
                    return (this.optional(element) || false);
                }
                if (value.length < 10 || value.length > 11) {
                    return (this.optional(element) || false);
                }
                if (["1", "2", "3", "4", "5", "6", "7", "8", "9"].indexOf(value.substring(2, 3)) == -1) {
                    return (this.optional(element) || false);
                }
                return (this.optional(element) || true);
            }, 'Informe um telefone válido');

            return validationsArray;
        }

        function uuidv4() {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }

        if ($.isFunction($.fn.validate)) {
            var validationsArray = ExtendValidation();
            $(document).arrive('form [validate]', function () {

                var $field = $(this);
                var $form = $field.parents("form");

                var opts = $form.validate().settings;
                if (!opts) {
                    opts = {
                        ignore: ".ignore",
                        rules: {},
                        messages: {},
                        errorElement: 'span',
                        errorClass: 'validate-has-error',
                        highlight: function (element) {
                            $(element).addClass("validate-has-error");
                            $(element).closest('.form-group').addClass('validate-has-error');
                        },
                        unhighlight: function (element) {
                            $(element).removeClass("validate-has-error");
                            $(element).closest('.form-group').removeClass('validate-has-error');
                        },
                        errorPlacement: function (error, element) {
                            if (element.closest('.has-switch').length) {
                                error.insertAfter(element.closest('.has-switch'));
                            }
                            else
                                if (element.parent('.checkbox, .radio').length || element.parent('.input-group').length) {
                                    error.insertAfter(element.parent());
                                }
                                else {
                                    error.insertAfter(element);
                                }
                        }
                    };
                }

                var name = $field.attr('name');
                var validate = $field.attr('validate');
                var _validate = validate.split(',');

                if (name == null) {
                    name = uuidv4();
                    $field.attr('name', name);
                }

                for (var k in _validate) {
                    var rule = _validate[k],
                        params,
                        message;

                    if (typeof opts['rules'][name] == 'undefined') {
                        opts['rules'][name] = {};
                        opts['messages'][name] = {};
                    }

                    if ($.inArray(rule, validationsArray) != -1) {
                        opts['rules'][name][rule] = true;

                        message = $field.data('message-' + rule);

                        if (message) {
                            opts['messages'][name][rule] = message;
                        }
                    }
                        // Parameter Value (#1 parameter)
                    else
                        if (params = rule.match(/(\w+)\[(.*?)\]/i)) {
                            if ($.inArray(params[1], ['min', 'max', 'minlength', 'maxlength', 'equalTo']) != -1) {
                                opts['rules'][name][params[1]] = params[2];


                                message = $field.data('message-' + params[1]);

                                if (message) {
                                    opts['messages'][name][params[1]] = message;
                                }
                            }
                        }
                }

                $.removeData($form, 'validator');
                $form.validate(opts);
            });
        }
    }
});

function validate(form) {
    var validation = $(form).validate();
    if (validation.checkForm() == false) {
        console.log("form has errors!");
        $("[validate].validate-has-error").removeClass("validate-has-error");
        validation.showErrors();
        return false;
    }
    return true;
}
/*! jquery.validate extension end */


function ConstruirHighchartComentario(data) {
    let chart = {
        categories: [],
        series: [],
        isEmpty: false
    };

    let comentarioPositivoContratada = [];
    let comentarioPositivoContratante = [];

    let comentarioNegativoContratada = [];
    let comentarioNegativoContratante = [];

    data.forEach(d => {
        chart.categories.push(d.obr_ds_obra);

        comentarioPositivoContratada.push(d.positivo_contratada);
        comentarioPositivoContratante.push(d.positivo_contratante);

        comentarioNegativoContratada.push(d.negativo_contratada);
        comentarioNegativoContratante.push(d.negativo_contratante);
    });
    const positivoContratada = {
        name: 'Positivos Contratada',
        color: '#254B59',
        data: comentarioPositivoContratada
    };

    const positivoContratante = {
        name: 'Positivos Contratante',
        color: '#84CCF2',
        data: comentarioPositivoContratante
    };

    const negativoContratada = {
        name: 'Negativos Contratada',
        color: '#92819F',
        data: comentarioNegativoContratada
    };


    const negativoContratante = {
        name: 'Negativos Contratante',
        color: '#FACD7F',
        data: comentarioNegativoContratante
    };
    console.log('positivoContratada', positivoContratada);
    console.log('positivoContratante', positivoContratante);
    console.log('negativoContratada', negativoContratada);
    console.log('negativoContratante', negativoContratante);

    chart.series.push(positivoContratada);
    chart.series.push(positivoContratante);
    chart.series.push(negativoContratada);
    chart.series.push(negativoContratante);

    isEmpty(chart);

    return chart;
}

function ConstruirHighChartDiaImprodutivo(data) {
    let chart = {
        categories: [],
        series: [],
        isEmpty: false
    };

    let imp_clima = [];
    let imp_falta_material = [];
    let imp_paralizacao = [];
    let imp_acidente = [];
    let imp_contratante = [];
    let imp_maodeobra = [];
    let imp_projeto = [];
    let imp_planejamento = [];
    let imp_fornecedores = [];
    let imp_equipamento = [];


    data.forEach(d => {
        chart.categories.push(d.obr_ds_obra);

        imp_clima.push(d.imp_clima);
        imp_falta_material.push(d.imp_falta_material);
        imp_paralizacao.push(d.imp_paralizacao);

        imp_acidente.push(d.imp_acidente);
        imp_contratante.push(d.imp_contratante);

        imp_maodeobra.push(d.imp_maodeobra);
        imp_projeto.push(d.imp_projeto);
        imp_planejamento.push(d.imp_planejamento);
        imp_fornecedores.push(d.imp_fornecedores);
        imp_equipamento.push(d.imp_equipamento);
    });

    const improdutividadeMaodeObra = {
        name: 'Mão de Obra',
        color: '#FACD7F',
        data: imp_maodeobra
    };

    const improdutividadeProjeto = {
        name: 'Projeto',
        color: '#59922B',
        data: imp_projeto
    };

    const improdutividadePlanejamento = {
        name: 'Planejamento',
        color: '#8A2BE2',
        data: imp_planejamento
    };

    const improdutividadeFornecedores = {
        name: 'Fornecedores',
        color: '#84CCF2',
        data: imp_fornecedores
    };

    const improdutividadeEquipamento = {
        name: 'Equipamento',
        color: '00FFFF',
        data: imp_equipamento
    };

    const improdutividadeClima = {
        name: 'Clima',
        color: '#254B59',
        data: imp_clima
    };

    const improdutividadeFaltaMaterial = {
        name: 'Materiais',
        color: '#92819F',
        data: imp_falta_material
    };

    const improdutividadeParalizacao = {
        name: 'Paralizações',
        color: '#B5BBA8',
        data: imp_paralizacao
    };

    const improdutividadeAcidente = {
        name: 'Acidentes',
        color: '#B8860B',
        data: imp_acidente
    };

    const improdutividadeContratante = {
        name: 'Contratante',
        color: '#FF7F50',
        data: imp_contratante
    };

    chart.series.push(improdutividadeClima);
    chart.series.push(improdutividadeFaltaMaterial);
    chart.series.push(improdutividadeParalizacao);
    chart.series.push(improdutividadeAcidente);
    chart.series.push(improdutividadeContratante);
    chart.series.push(improdutividadeMaodeObra);
    chart.series.push(improdutividadeProjeto);
    chart.series.push(improdutividadePlanejamento);
    chart.series.push(improdutividadeFornecedores);
    chart.series.push(improdutividadeEquipamento);

    isEmpty(chart);

    return chart;
}

function ConstruirHighChartTarefa(tarefa, status) {
    let chart = {
        categories: [],
        series: [],
        isEmpty: false
    };

    let atraso_inicio = [];
    let atraso_fim = [];
    let pausadas = [];
    let canceladas = [];
    let planejadas = [];
    let em_execucao = [];
    let finalizadas = [];


    tarefa.forEach((d, index) => {
        chart.categories.push(d.obr_ds_obra);

        atraso_inicio.push(d.atraso_inicio);
        atraso_fim.push(d.atraso_fim);
        pausadas.push(d.pausadas);
        canceladas.push(d.canceladas);

        planejadas.push(status[index].planejada);
        em_execucao.push(status[index].em_execucao);
        finalizadas.push(status[index].finalizada);
    });

    const tarefasCanceladas = {
        name: 'Canceladas',
        color: '#254B59',
        data: canceladas
    };

    const tarefasPausadas = {
        name: 'Pausadas',
        color: '#84CCF2',
        data: pausadas
    };

    const inicioEmAtraso = {
        name: 'Início em Atraso',
        color: '#92819F',
        data: atraso_inicio
    };

    const fimEmAtraso = {
        name: 'Fim em Atraso',
        color: '#FACD7F',
        data: atraso_fim
    };

    const planejada = {
        name: 'Planejada',
        color: '#B5BBA8',
        data: planejadas
    };

    const emExecucao = {
        name: 'Em Execução',
        color: '#59922B',
        data: em_execucao
    };

    const finalizada = {
        name: 'Finalizada',
        color: '#00FFFF',
        data: finalizadas
    };

    chart.series.push(tarefasCanceladas);
    chart.series.push(tarefasPausadas);
    chart.series.push(inicioEmAtraso);
    chart.series.push(fimEmAtraso);

    chart.series.push(planejada);
    chart.series.push(emExecucao);
    chart.series.push(finalizada);

    isEmpty(chart);

    return chart;
}

function ConstruirHighchartLaudos(gs, as) {
    let simValores = [];
    let naoValores = [];

    let chart = {
        categories: [
            'Os níveis de CLORO estão entre 1ppm e 3ppm?',
            'O PH está entre 7,2 e 7,6?',
            'A LIMPIDEZ DA ÁGUA permite perfeita visibilidade da parte mais profunda do tanque?',
            'A superfície da água está livre de MATÉRIAS FLUTUANTES, estranhas à piscina?',
            'O fundo do tanque está LIVRE DE DETRITOS?',
            'O NÍVEL DE CLORO no tanque está mantido entre 0,8 a 3,0 mg/l?',
            'A piscina contém BACTÉRIAS DO GRUPO COLIFORME e/ou STAPHYLOCOCCUS AUREUS?',
            'Há proliferação de ALGAS, LEVEDURAS E AMEBAS DE VIDA LIVRE na piscina?'
        ],
        series: [],
        isEmpty: false
    };
   

    simValores.push(as.nivelCloroSim);
    simValores.push(as.phSim);
    simValores.push(as.limpidezSim);
    simValores.push(as.superficieSim);
    simValores.push(as.fundoSim);
    simValores.push(as.nivelCloro2Sim);
    simValores.push(as.bacteriasSim);
    simValores.push(as.proliferacaoSim);

    naoValores.push(as.nivelCloroNao);
    naoValores.push(as.phNao);
    naoValores.push(as.limpidezNao);
    naoValores.push(as.superficieNao);
    naoValores.push(as.fundoNao);
    naoValores.push(as.nivelCloro2Nao);
    naoValores.push(as.bacteriasNao);
    naoValores.push(as.proliferacaoNao);

    const sim = {
        name: 'Sim',
        color: '#254B59',
        data: simValores
    };

    const nao = {
        name: 'Não',
        color: '#B5BBA8',
        data: naoValores
    };

    chart.series.push(sim);
    chart.series.push(nao);

    isEmpty(chart);

    return chart;
}

function ConstruirHighchartRdo(gs, as) {
    let dias_atraso = [];
    let sem_ass_contratante = [];

    let chart = {
        categories: [],
        series: [],
        isEmpty: false
    };

    as.forEach(d => {
        chart.categories.push(d.obr_ds_obra);

        dias_atraso.push(d.dias_atraso);
        sem_ass_contratante.push(d.sem_ass_contratante);
    });

    const serieDiasAtrasado = {
        name: 'Atrasados',
        color: '#254B59',
        data: dias_atraso
    };

    const serieSemAssinaturaContratante = {
        name: 'Sem Assinatura Contratante',
        color: '#92819F',
        data: sem_ass_contratante
    }

    chart.series.push(serieDiasAtrasado);
    //chart.series.push(serieSemAssinaturaContratante);

    let gerados = [];
    let contratadas = [];
    let contratantes = [];
    let a_gerar = [];

    gs.forEach(d => {
        chart.categories.push(d.obr_ds_obra);

        gerados.push(d.gerado);
        contratantes.push(d.assinado_contratante);
        contratadas.push(d.assinado_contratada);
        a_gerar.push(d.a_gerar);
    });

    const serieGerado = {
        name: 'Gerados',
        color: '#B5BBA8',
        data: gerados
    };

    const serieContratante = {
        name: 'Com Assinatura Contratante',
        color: '#FACD7F',
        data: contratantes
    };

    const serieContratada = {
        name: 'Assinados Contratada',
        color: '#84CCF2',
        data: contratadas
    };

    const serieGerar = {
        name: 'A Gerar',
        color: '#59922B',
        data: a_gerar
    };

    chart.series.push(serieGerado);
    //chart.series.push(serieContratante);
    chart.series.push(serieContratada);
    chart.series.push(serieGerar);

    isEmpty(chart);

    return chart;
}

function isEmpty(chart) {
    let length = 0;
    let sum = 0;

    chart.series.forEach(s => {
        length += s.data.length;
        s.data.forEach(d => {

            if (d === 0 || d === undefined) {
                sum++;
            }
        });
    });

    if (length === sum) {
        chart.isEmpty = true;
    }
}

function isValid(date) {
    let today = new Date('2000-01-01');
    let dd = String(today.getDate()).padStart(2, '0');
    let mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    let yyyy = today.getFullYear();

    if (date < (yyyy + "-" + mm + "-" + dd)) return false;

    let [y, m, d] = date.split('-');

    d = d.substring(0, 2);
    // Assume not leap year by default (note zero index for Jan)
    const daysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    // If evenly divisible by 4 and not evenly divisible by 100,
    // or is evenly divisible by 400, then a leap year
    if ((!(y % 4) && y % 100) || !(y % 400)) {
        daysInMonth[1] = 29;
    }

    return !(/\D/.test(String(d))) && d > 0 && d <= daysInMonth[--m];
}

function buildTableRdos(data, atrasados) {
    const rdos = [];

    for (let i = 0; i < data.length; i++) {
        let aux = {
            obr_ds_obra: data[i].obr_ds_obra,
            atrasado: atrasados[i].dias_atraso,
            assinado_contratada: data[i].assinado_contratada,
            sem_ass_contratante: atrasados[i].sem_ass_contratante,
            assinado_contratante: data[i].assinado_contratante,
            gerado: data[i].gerado,
            a_gerar: data[i].a_gerar
        };
        rdos.push(aux);
    }

    return rdos;
}

function buildTableTarefa(ts, status) {
    const tarefas = [];

    ts.forEach((t, index) => {
        let aux = {
            obr_ds_obra: t.obr_ds_obra,
            atraso_inicio: t.atraso_inicio,
            atraso_fim: t.atraso_fim,
            pausada: t.pausadas,
            cancelada: t.canceladas,
            planejada: status[index].planejada,
            em_execucao: status[index].em_execucao,
            finalizada: status[index].finalizada
        };

        tarefas.push(aux);
    });

    return tarefas;
}

function CarregaJS() {





    // $(".collapsed-menu").click(function(){
    //  var parent = $(this).parent().find("ul").length;
    //  if(parent > 0){
    //      $(".collapse.in").collapse("hide");
    //  }
    // });









    // Ao clicar na data remover a class is-empty do campo 
    // $(".datepicker--cells div").click(function(){
    //     alert();

    // });
    jQuery(document).ready(function () {
        jQuery('.scrollbar-inner').scrollbar();
    });

    //datepicker para modais
    // function fixDatepickerPosition() {

    //     // if (input.parents(".modal").length == 1) {
    //     //     var modal = $('input:focus').parents(".modal").eq(0);
    //     //     $("#datepickers-container").appendTo(modal);
    //     //     if (modal.scrollTop() > 10) {
    //     //         var val = modal.scrollTop() + input.offset().top + 50;
    //     //     }

    //     //     $("#datepickers-container .datepicker").css("top", val);
    //     //     console.log("updating position: " + val);  
    //     // }

    // }
    //datepicker para modais
    //$('input.datepicker-here').on("focus", function () {
    //    var $modal = $(this).parents('.modal:first');
    //    if ($modal.length > 0) {
    //        var _datepicker = $(this).data('datepicker');
    //        _datepicker.$datepicker.parent().addClass('in-modal');
    //        $modal.off('scroll.datepicker').on('scroll.datepicker', function () {
    //            _datepicker.update({ position: "bottom left" });
    //        });
    //        $modal.off('hidden.bs.modal').on('hidden.bs.modal', function () {
    //            $modal.off('scroll.datepicker');
    //            _datepicker.$datepicker.parent().removeClass('in-modal');
    //        });
    //    }
    //});

    // $('.modal-datepicker').datepicker({
    //     onShow: function () {
    //         fixDatepickerPosition();
    //     },
    //     language: "pt-BR"
    // });

    // $('#ModalAluno').scroll(function () {
    //     fixDatepickerPosition();
    // });



};
