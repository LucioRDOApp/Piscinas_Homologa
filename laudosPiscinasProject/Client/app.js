(function () {
    'use strict';

    var app = angular.module('app', ['ngResource', 'ui.router', 'ui.mask', 'naif.base64', 'ngMaterial', 'ngLocale'])
        .config(function ($mdDateLocaleProvider) {
            $mdDateLocaleProvider.months = 'janeiro_fevereiro_março_abril_maio_junho_julho_agosto_setembro_outubro_novembro_dezembro'.split('_');

            $mdDateLocaleProvider.shortMonths = 'jan_fev_mar_abr_mai_jun_jul_ago_set_out_nov_dez'.split('_');

            $mdDateLocaleProvider.days = 'domingo_segunda-feira_terça-feira_quarta-feira_quinta-feira_sexta-feira_sábado'.split('_');

            $mdDateLocaleProvider.shortDays = 'dom_seg_ter_qua_qui_sex_sáb'.split('_');

            // Can change week display to start on Monday.
            $mdDateLocaleProvider.firstDayOfWeek = 1;

            // Optional.
            //$mdDateLocaleProvider.dates = [1, 2, 3, 4, 5, 6, ...];

            //$mdDateLocaleProvider.monthHeaderFormatter = function (date) {
            //    return myShortMonths[date.getMonth()] + ' ' + date.getFullYear();
            //};

            // In addition to date display, date components also need localized messages
            // for aria-labels for screen-reader users.

            $mdDateLocaleProvider.weekNumberFormatter = function (weekNumber) {
                return 'Semana ' + weekNumber;
            };

            $mdDateLocaleProvider.msgCalendar = 'Calendário';

            $mdDateLocaleProvider.msgOpenCalendar = 'Calendário aberto';

            /**
             * @param date {Date}
             * @returns {string} string representation of the provided date
             */
            $mdDateLocaleProvider.formatDate = function (date) {
                return date ? moment(date).format('DD/MM/YYYY') : '';
            };

            /**
             * @param dateString {string} string that can be converted to a Date
             * @returns {Date} JavaScript Date object created from the provided dateString
             */
            $mdDateLocaleProvider.parseDate = function (dateString) {
                var m = moment(dateString, 'DD/MM/YYYY', true);
                return m.isValid() ? m.toDate() : new Date(NaN);
            };

            /**
             * Check if the date string is complete enough to parse. This avoids calls to parseDate
             * when the user has only typed in the first digit or two of the date.
             * Allow only a day and month to be specified.
             * @param dateString {string} date string to evaluate for parsing
             * @returns {boolean} true if the date string is complete enough to be parsed
             */
            $mdDateLocaleProvider.isDateComplete = function (dateString) {
                dateString = dateString.trim();
                // Look for two chunks of content (either numbers or text) separated by delimiters.
                var re = /^(([a-zA-Z]{3,}|[0-9]{1,4})([ .,]+|[/-]))([a-zA-Z]{3,}|[0-9]{1,4})/;
                return re.test(dateString);
            };
        });

    app.factory('Auth', function () {
        var index = 0;
        return {
            setUser: function (aUser) {
                localStorage.setItem("loginUser", aUser == undefined ? undefined : JSON.stringify(aUser));
                localStorage.setItem("user", aUser == undefined ? undefined : JSON.stringify(aUser));
                index++;
            },
            updateUser: function (aUser) {
                localStorage.setItem("user", aUser == undefined ? undefined : JSON.stringify(aUser));
                index++;
            },
            getUser: function () {
                var u = localStorage.getItem("user");
                if (u != "undefined") {
                    var user = JSON.parse(u);
                    return user;
                }
                return undefined;
            },
            getLoginUser: function () {
                var u = localStorage.getItem("loginUser");
                if (u != "undefined") {
                    return JSON.parse(u);
                }
                return undefined;
            },
            changed: function () {
                return index;
            },
            isLoggedIn: function () {
                return typeof (this.getUser()) != "undefined";
            },
            logout: function () {
                this.setUser(undefined);
            }
        }
    });

    app.directive('customOnChange', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var onChangeHandler = scope.$eval(attrs.customOnChange);
                element.on('change', onChangeHandler);
                element.on('$destroy', function () {
                    element.off();
                });

            }
        };
    });

    app.directive('numericOnly', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, modelCtrl) {

                modelCtrl.$parsers.push(function (inputValue) {
                    var transformedInput = inputValue ? inputValue.replace(/[^\d.-]/g, '') : null;

                    if (transformedInput != inputValue) {
                        modelCtrl.$setViewValue(transformedInput);
                        modelCtrl.$render();
                    }

                    return transformedInput;
                });
            }
        };
    });


    /* app.directive('numberOnly', function () {
         return {
             restrict: 'A',
             require: 'ngModel',
             link: function (scope, element, attrs, ngModel) {
                 var negativo = /\-/.test(attrs.numberOnly);
                 var decimal = /\.|\,/.test(attrs.numberOnly) ? /\.|\,/.exec(attrs.numberOnly)[0] : null;
 
                 var regExp = '^';
                 regExp += negativo ? '[\\-]{0,1}' : '';
                 regExp += '[\\d]+';
                 if (decimal != null) {
                     regExp += '[\\' + decimal + '][\\d]+|';
                     if (negativo) {
                         regExp += '[\\-]{0,1}'
                     }
                     regExp += '[\\d]+'
                 }
                 regExp += '';
                 regExp = new RegExp(regExp);
 
                 ngModel.$parsers.unshift(function (input) {
                     if (input === undefined) return null;
                     if (input === null) return;
 
                     input = input.toString().replace(/\./, decimal);
                     if (input == '-') return input;
                     if (decimal !== null && input.charAt(input.length - 1) == decimal) return input;
 
                     input = regExp.test(input) ? regExp.exec(input)[0] : null;
 
                     var viewVal = null;
 
                    if (input !== null) {
                         input = decimal != null ? parseFloat(input.replace(/\,/, '.')) : parseInt(input);
                     }
 
                     viewVal = isNaN(input) || input === null ? '' : input;
 
                     ngModel.$setViewValue(decimal != null ? viewVal.toString().replace(/\./, decimal) : viewVal.toString());
                     ngModel.$render();
 
                     return isNaN(input) ? null : input;
                 });
 
                 ngModel.$formatters.unshift(function (value) {
                     if (value !== undefined && value !== null) {
                        return decimal != null ? value.toString().replace(/\./, decimal) : value.toString();
                     }
                 });
             }
         }
     }); */

    //app.factory('Download', function () {
    //    return {
    //        base64: function (data, fileName) {
    //            function convertURIToBinary(base64) {
    //                var raw = window.atob(base64);
    //                var rawLength = raw.length;
    //                var array = new Uint8Array(new ArrayBuffer(rawLength));

    //                for (var i = 0; i < rawLength; i++) {
    //                    array[i] = raw.charCodeAt(i);
    //                }
    //                return array;
    //            }

    //            //var blob = new Blob([convertURIToBinary(data)], { type: 'application/octet-stream' });
    //            var blob = new Blob([convertURIToBinary(data)], { type: 'application/pdf' });
    //            var link = document.createElement('a');
    //            link.href = window.URL.createObjectURL(blob);
    //            //var fileName = reportName + ".pdf";
    //            link.download = fileName;
    //            link.click();

    //        }
    //    }
    //});

    app.factory('Download', function () {
        return {
            base64: function (data, fileName, typeFile) {
                function convertURIToBinary(base64) {
                    var raw = window.atob(base64);
                    var rawLength = raw.length;
                    var array = new Uint8Array(new ArrayBuffer(rawLength));

                    for (var i = 0; i < rawLength; i++) {
                        array[i] = raw.charCodeAt(i);
                    }
                    return array;
                }

                //var blob = new Blob([convertURIToBinary(data)], { type: 'application/octet-stream' });

                var type = 'application/pdf';
                if (typeFile == "EXCEL") {
                    var type = 'application/excel';
                }

                var blob = new Blob([convertURIToBinary(data)], { type: type });
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                //var fileName = reportName + ".pdf";
                link.download = fileName;
                link.click();

            }
        }
    });

    app.factory('Grafico', function ($http, $q) {
        return {
            getRdoGerado: (filter) => {
                return $q((resolve, reject) => {
                    $http({
                        url: ' api/grafico/ContagemRdoGerado',
                        method: 'POST',
                        data: filter
                    }).success((data, status, headers, config) => {
                        resolve(data, status, headers, config);
                    }).error((data, status, headers, config) => {
                        reject(data, status, headers, config);
                    });
                });
            },
            getRdoAtrasado: (filter) => {
                return $q((resolve, reject) => {
                    $http({
                        url: ' api/grafico/ContagemRdoAtrasado',
                        method: 'POST',
                        data: filter
                    }).success((data, status, headers, config) => {
                        resolve(data, status, headers, config);
                    }).error((data, status, headers, config) => {
                        reject(data, status, headers, config);
                    });
                });
            },
            getDiaImprodutivo: (filter) => {
                return $q((resolve, reject) => {
                    $http({
                        url: 'api/grafico/ContagemDiaImprodutivo',
                        method: 'POST',
                        data: filter
                    }).success((data, status, headers, config) => {
                        resolve(data, status, headers, config);
                    }).error((data, status, headers, config) => {
                        reject(data, status, headers, config);
                    });
                });
            },
            getTarefa: (filter) => {
                return $q((resolve, reject) => {
                    $http({
                        url: 'api/grafico/ContagemTarefa',
                        method: 'POST',
                        data: filter
                    }).success((data, status, headers, config) => {
                        resolve(data, status, headers, config);
                    }).error((data, status, headers, config) => {
                        reject(data, status, headers, config);
                    });
                });
            },
            getStatusTarefa: (filter) => {
                return $q((resolve, reject) => {
                    $http({
                        url: 'api/grafico/ContagemStatusTarefa',
                        method: 'POST',
                        data: filter
                    }).success((data, status, headers, config) => {
                        resolve(data, status, headers, config);
                    }).error((data, status, headers, config) => {
                        reject(data, status, headers, config);
                    });
                });
            },
            getComentario: (filter) => {
                return $q((resolve, reject) => {
                    $http({
                        url: 'api/grafico/ContagemComentario',
                        method: 'POST',
                        data: filter
                    }).success((data, status, headers, config) => {
                        resolve(data, status, headers, config);
                    }).error((data, status, headers, config) => {
                        reject(data, status, headers, config);
                    });
                });
            }
        }
    });

    app.factory('ViewBag', function () {
        var param = {};
        return {
            set: function (key, val) {
                param[key] = val;
            },
            get: function (key) {
                return param[key];
            }
        }
    });

    app.factory('Validacao', function () {
        return {
            required: function (val) {
                if (val != null) {
                    if (typeof (val) == 'string' && val.length > 0) {
                        return false;
                    }
                    if (typeof (val) == 'number' && val != 0) {
                        return false;
                    }
                }

                console.log("validation erros: ", val);
                return true;
            },
            cpf: function (val) {
                // criar validacao cpf
                var Soma;
                var Resto;
                Soma = 0;

                if (val == undefined ||
                    val == "00000000000" ||
                    val == "11111111111" ||
                    val == "22222222222" ||
                    val == "33333333333" ||
                    val == "44444444444" ||
                    val == "55555555555" ||
                    val == "66666666666" ||
                    val == "77777777777" ||
                    val == "88888888888" ||
                    val == "99999999999" ||
                    val.length != 11
                ) {
                    return false;
                }
                for (var i = 1; i <= 9; i++) {
                    Soma = Soma + parseInt(val.substring(i - 1, i)) * (11 - i);
                }
                Resto = (Soma * 10) % 11;
                if ((Resto == 10) || (Resto == 11)) {
                    Resto = 0;
                }
                if (Resto != parseInt(val.substring(9, 10))) {
                    return false;
                }
                Soma = 0;
                for (var i = 1; i <= 10; i++) {
                    Soma = Soma + parseInt(val.substring(i - 1, i)) * (12 - i);
                }
                Resto = (Soma * 10) % 11;
                if ((Resto == 10) || (Resto == 11)) {
                    Resto = 0;
                }
                if (Resto != parseInt(val.substring(10, 11))) {
                    return false;
                }
                return true;
            },
            email: function (val) {
                if (val == null || val == undefined) {
                    return false;
                }
                var somaArroba = 0;

                for (var i = 0; i <= val.length; i++) {
                    if (val[i] == "@") {
                        somaArroba += 1;
                    }
                }
                if (somaArroba == 1) {
                    return true;
                }
                return false;
            },
            cnpj: function (val) {


                if (val == '') {
                    return false;
                }

                if (val.length != 14) {
                    return false;
                }
                // Elimina CNPJs invalidos conhecidos
                if (val == "00000000000000" ||
                    val == "11111111111111" ||
                    val == "22222222222222" ||
                    val == "33333333333333" ||
                    val == "44444444444444" ||
                    val == "55555555555555" ||
                    val == "66666666666666" ||
                    val == "77777777777777" ||
                    val == "88888888888888" ||
                    val == "99999999999999") {
                    return false;
                }
                // Valida DVs
                var tamanho = val.length - 2
                var numeros = val.substring(0, tamanho);
                var digitos = val.substring(tamanho);
                var soma = 0;
                var pos = tamanho - 7;
                for (var i = tamanho; i >= 1; i--) {
                    soma += numeros.charAt(tamanho - i) * pos--;
                    if (pos < 2) {
                        pos = 9;
                    }
                }
                var resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(0)) {
                    return false;
                }
                tamanho = tamanho + 1;
                numeros = val.substring(0, tamanho);
                soma = 0;
                pos = tamanho - 7;
                for (var i = tamanho; i >= 1; i--) {
                    soma += numeros.charAt(tamanho - i) * pos--;
                    if (pos < 2) {
                        pos = 9;
                    }
                }
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(1)) {
                    return false;
                }
                return true;
            },
            aquisicaoVal: function (val) {
                if (val != 'S') {
                    return false;
                }
                return true;
            },
            minLenght: function (val, qtd) {
                if (typeof (qtd) == "undefined") {
                    qtd = 3;
                }
                if (val != null) {
                    if (val.length < qtd) {
                        return false;
                    }
                }
                return true;
            },
            data: function (val) {
                val = val.replace(/\D/g, '');
                var primeiroCampo = val.substring(0, 2);
                var segundoCampo = val.substring(2, 4);
                var ultimoCampo = val.substring(4, 8);
                var dataFinal = ultimoCampo + "-" + segundoCampo + "-" + primeiroCampo;

                if (val.length > 8) {
                    var hora = parseInt(val.substring(8, 10));
                    var minuto = parseInt(val.substring(10, 12));
                }

                if (new Date(dataFinal) == "Invalid Date") {
                    return false;
                }

                if (hora > 23 || minuto > 59) {
                    return false;
                }



                return true;
            },
            hourTime: function (val) {
                var timeValue = val;
                if (timeValue == "" || timeValue.indexOf(":") < 0) {
                    return false;
                }
                else {
                    var sHours = timeValue.split(':')[0];
                    var sMinutes = timeValue.split(':')[1];
                    if (sHours == "" || isNaN(sHours) || parseInt(sHours) > 23) {
                        return false;
                    }
                    else if (parseInt(sHours) == 0)
                        sHours = "00";
                    else if (sHours < 10)
                        sHours = "0" + sHours;

                    if (sMinutes == "" || isNaN(sMinutes) || parseInt(sMinutes) > 59) {
                        return false;
                    }
                    else if (parseInt(sMinutes) == 0)
                        sMinutes = "00";
                    else if (sMinutes < 10)
                        sMinutes = "0" + sMinutes;

                    val = sHours + ":" + sMinutes;
                }

                return true;
            }

        }
    });

    app.factory('Convert', function () {
        return {
            toDate(date) {
                dtArray = date.split('/').reverse();
                return new Date(dtArray[0], Number(dtArray[1]) - 1, dtArray[2]);
            },
            toDateFromStringDateBR(dateBR) {
                let dtArray = dateBR.split(' ')[0].split('T')[0].split('/').reverse();
                return new Date(dtArray[0].trim(), Number(dtArray[1].trim()) - 1, dtArray[2].trim());
            },
            toJSON(date) {
                if (typeof (date) == 'object')
                    date.setHours(date.getHours() - 3);
                return typeof (date) == 'object' ? date.toJSON() : Convert.toDate(moment(date).format('DD/MM/YYYY')).toJSON();
            },
            toJSONfromDateBR: function (datebr) {
                if (datebr === undefined) {
                    return;
                }
                // se o parâmetro vier como número
                if (!isNaN(datebr)) {
                    datebr = datebr + "";
                    while (datebr.length < 8) datebr = "0" + datebr;
                }
                var dataj;
                if (datebr.length == 8) {
                    // se tiver 8 caracteres, provavelmente não colocaram um separador
                    let day = datebr.substring(0, 2);
                    let month = datebr.substring(2, 4);
                    let year = datebr.substring(4, 8);
                    dataj = new Date(year, month - 1, day);
                }
                else {
                    // verifica o separador se é '-' ou '/'
                    let splitchar = datebr.charAt(2);
                    let splitedstr = datebr.split(splitchar);
                    let hora = "";
                    let minuto = "";

                    // se tiver hora incluída (separada por espaço)
                    if (splitedstr[2].includes(" ")) {
                        hora = splitedstr[2].split(" ")[1].split(":")[0].substring(0, 2);
                        minuto = splitedstr[2].split(" ")[1].split(":")[1].substring(0, 2);
                    }

                    dataj = new Date(splitedstr[2].substring(0, 4), splitedstr[1] - 1, splitedstr[0], hora, minuto);
                }
                //return dataj.toJSON();
                // resolvida questão do timezone (tira até o horário de verão)
                return new Date(dataj.getTime() - (dataj.getTimezoneOffset() * 60000)).toJSON()
            }
        }
    });

    app.factory('Format', () => {
        return {
            mask: (val) => {
                val = val.replace(/\D/g, '');
                const primeiroCampo = val.substring(0, 2);
                const segundoCampo = val.substring(2, 4);
                const ultimoCampo = val.substring(4, 8);
                const date = ultimoCampo + '-' + segundoCampo + '-' + primeiroCampo;

                return date;
            }
        }
    });

    app.directive('loading', ['$http', function ($http) {
        return {
            restrict: 'A',
            link: function (scope, elm, attrs) {
                scope.isLoading = function () {
                    return $http.pendingRequests.length > 0;
                };

                scope.$watch(scope.isLoading, function (v) {
                    if (v) {
                        elm.show();
                    } else {
                        elm.hide();
                    }
                });
            }
        };

    }]);

    app.directive('file', function () {
        return {
            scope: {
                file: '='
            },
            link: function (scope, el, attrs) {
                el.bind('change', function (event) {
                    var file = event.target.files[0];
                    scope.file = file ? file : undefined;
                    scope.$apply();
                });
            }
        };
    });

    app.directive('permission', ['Auth', '$location', '$compile', 'Permission', function (Auth, $location, $compile, Permission) {
        return {
            restrict: 'A',
            scope: {
                items: "="
            },
            link: function (scope, element, attrs) {
                var action = element[0].attributes.permission.value;
                var route = element[0].attributes['permission-route'] != null ? element[0].attributes['permission-route'].value : null;
                var hasPermission = Permission.check(action, route);
                if (!hasPermission) {
                    var html = '';
                    var e = $compile(html)(scope);
                    element.replaceWith(e);
                }
            }
        };

    }]);

    app.factory('Permission', ['Auth', '$location', function (Auth, $location) {
        return {
            check: function (perm, route) {
                var currentPath = route == null ? $location.path() : route;
                var retorno = true;
                var routeFound = false;
                if (Auth.isLoggedIn()) {
                    var user = Auth.getUser();
                    if (!user.routes) {
                        return false;
                    }
                    for (var k in user.routes) {
                        var route = user.routes[k];
                        if (route.path == currentPath) {
                            routeFound = true;
                            if (!route.permissions) {
                                return false;
                            }
                            for (var i in route.permissions) {
                                if (route.permissions[i] == perm) {
                                    return true;
                                }
                            }
                            return false;
                        }
                    }
                }
                if (routeFound == false) {
                    return false;
                }
                return retorno;
            }
        }
    }]);

    app.factory('Styles', function ($location) {
        String.prototype.hashCode = function () {
            var hash = 0, i, chr;
            if (this.length === 0) return hash;
            for (i = 0; i < this.length; i++) {
                chr = this.charCodeAt(i);
                hash = ((hash << 5) - hash) + chr;
                hash |= 0; // Convert to 32bit integer
            }
            return hash;
        };

        let loadedStyles = [];

        let extensao = window.location.href.replace($location.path(), "").split("/");

        const elements = document.getElementsByTagName('link');
        for (var i = 0; i < elements.length; i++) {
            elements[i].setAttribute('id', elements[i].href.substring(22).toString().hashCode());
            if (i > 0) loadedStyles.push(elements[i].href.substring(22).toString().hashCode());
        }

        const addLink = function (styleHref, styleHash) {
            const link = document.getElementById(styleHash);

            if (typeof link !== 'undefined' && link !== null) link.disabled = false;

            else {
                const newLink = document.createElement('link');
                newLink.rel = 'stylesheet';
                newLink.type = 'text/css';
                newLink.id = styleHash;
                newLink.href = extensao[3] != null || extensao[3] != undefined ? "/" + extensao[3] + styleHref : styleHref;

                document.getElementsByTagName('head')[0].appendChild(newLink);
            }

        };
        const removeLink = function (styleHash) {
            const link = document.getElementById(styleHash);

            if (link !== 'undefined' && link != null) link.disabled = true;
        };

        return {
            loadStyles: function (styles) {
                styles.forEach(function (styleHref) {
                    const styleHash = styleHref.toString().hashCode();

                    let isLoaded = false;
                    loadedStyles.forEach(function (h) {
                        if (h.toString().hashCode() == styleHash) isLoaded = true;
                    });

                    if (!isLoaded) {
                        loadedStyles.push(styleHash);
                        addLink(styleHref, styleHash);
                    }
                });
            },
            unloadStyles: function (styles) {
                styles.forEach(function (styleHref) {
                    const styleHash = styleHref.toString().hashCode();
                    loadedStyles.forEach(function (h, index) {
                        if (h == styleHash) {
                            loadedStyles.splice(index, 1);
                            removeLink(styleHash);
                        }
                    });
                });
            },
            hasStyle: function (style) {
                let hasStyle = false;

                loadedStyles.forEach(function (h, index) {
                    if (h == style.toString().hashCode()) hasStyle = true
                });

                return hasStyle;
            }
        }
    });

    app.factory('StoreService', function () {
        let filter = {};

        return {
            getData: () => {
                return filter;
            },
            setData: data => {
                filter = data;
            },
            resetData: () => {
                filter = {};
            },
            isData: () => {
                return filter.periodoIni !== '' && filter.periodoIni !== null && filter.periodoIni !== undefined ? true : false;
            }
        }
    });

    app.factory('Email', function () {
        var EMAIL_REGX = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$/;
        return {
            checkEmail: function (email) {
                if (EMAIL_REGX.test(email)) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    });

    app.config(['$locationProvider', '$urlRouterProvider', '$stateProvider', 'uiMask.ConfigProvider', function ($locationProvider, $urlRouterProvider, $stateProvider, uiMaskConfigProvider) {
        uiMaskConfigProvider.clearOnBlur(false);
        uiMaskConfigProvider.clearOnBlurPlaceholder(true);

        $locationProvider.html5Mode(true).hashPrefix('!');

        $urlRouterProvider.when('/default/index', '/default')
        $urlRouterProvider.otherwise('/login');

        // Layout Externo
        $stateProvider.state({
            name: 'layoutexterno',
            templateUrl: 'Client/Views/Layout/layout-externo.html'
        });

        // Layout Interno
        $stateProvider.state({
            name: 'layoutinterno',
            templateUrl: 'Client/Views/Layout/layout-interno.html'
        });

        // Layout Interno
        $stateProvider.state({
            name: 'layoutinternoazul',
            templateUrl: 'Client/Views/Layout/layout-interno-azul.html'
        });

        //Layout Interno Gráfico
        $stateProvider.state({
            name: 'layoutinternografico',
            templateUrl: 'Client/Views/Layout/layout-interno-grafico.html'
        });

        $stateProvider.state({
            name: 'sair',
            url: '/sair'
        });

        $stateProvider.state({
            title: 'Login',
            name: 'layoutexterno.login',
            url: '/login',
            templateUrl: 'Client/Views/Login/index.html',
        });

        $stateProvider.state({
            name: 'layoutexterno.convidada',
            url: '/convidada',
            templateUrl: 'Client/Views/Convidada/index.html',
        });

        $stateProvider.state({
            title: 'EsqueciSenha',
            name: 'layoutexterno.esquecisenha',
            url: '/esquecisenha',
            templateUrl: 'Client/Views/EsqueciSenha/index.html',
        });

        //$stateProvider.state({
        //    name: 'layoutinterno.default',
        //    url: '/default',
        //    templateUrl: 'Client/Views/Default/index.html',
        //});

        // Obra (index, cadastro, escolher)
        $stateProvider.state({
            name: 'layoutinterno.obra',
            url: '/obra/index',
            templateUrl: 'Client/Views/Obra/index.html',
        });
        $stateProvider.state({
            name: 'layoutinterno.obracadastro',
            url: '/obra/cadastro',
            templateUrl: 'Client/Views/Obra/cadastro.html',
        });
        $stateProvider.state({
            name: 'layoutinternoazul.obraescolher',
            url: '/obra/escolher',
            templateUrl: 'Client/Views/Obra/escolher.html',
        });

        // Tarefa (cards, index, cadastro)
        $stateProvider.state({
            name: 'layoutinterno.tarefacards',
            url: '/tarefa/cards',
            templateUrl: 'Client/Views/Tarefa/cards.html',
        });
        $stateProvider.state({
            name: 'layoutinterno.tarefa',
            url: '/tarefa/index',
            templateUrl: 'Client/Views/Tarefa/index.html',
        });
        $stateProvider.state({
            name: 'layoutinterno.tarefacadastro',
            url: '/tarefa/cadastro',
            templateUrl: 'Client/Views/Tarefa/cadastro.html',
        });

        // Cadastro (index, cadastro)
        $stateProvider.state({
            name: 'layoutinterno.grupo',
            url: '/grupo/index',
            templateUrl: 'Client/Views/Grupo/index.html',
        });
        $stateProvider.state({
            name: 'layoutinterno.grupocadastro',
            url: '/grupo/cadastro',
            templateUrl: 'Client/Views/Grupo/cadastro.html',
        });

        // Menu (index, cadastro)
        $stateProvider.state({
            name: 'layoutinterno.menucadastro',
            url: '/menu/cadastro',
            templateUrl: 'Client/Views/Menu/cadastro.html',
        });
        $stateProvider.state({
            name: 'layoutinterno.menu',
            url: '/menu/index',
            templateUrl: 'Client/Views/Menu/index.html',
        });

        // Página (index, cadastro)
        $stateProvider.state({
            name: 'layoutinterno.paginacadastro',
            url: '/pagina/cadastro',
            templateUrl: 'Client/Views/Pagina/cadastro.html',
        });
        $stateProvider.state({
            name: 'layoutinterno.pagina',
            url: '/pagina/index',
            templateUrl: 'Client/Views/Pagina/index.html',
        });

        // Equipamentos (index, cadastro)
        $stateProvider.state({
            name: 'layoutinterno.equipamentoscadastro',
            url: '/equipamentos/cadastro',
            templateUrl: 'Client/Views/Equipamentos/cadastro.html',
        });
        $stateProvider.state({
            name: 'layoutinterno.equipamentos',
            url: '/equipamentos/index',
            templateUrl: 'Client/Views/Equipamentos/index.html',
        });

        // Colaborador (index, cadastro)
        $stateProvider.state({
            name: 'layoutinterno.colaboradorcadastro',
            url: '/colaborador/cadastro',
            templateUrl: 'Client/Views/Colaborador/cadastro.html',
        });
        $stateProvider.state({
            name: 'layoutinterno.colaborador',
            url: '/colaborador/index',
            templateUrl: 'Client/Views/Colaborador/index.html',
        });
        $stateProvider.state({
            name: 'layoutinterno.colaboradoralterarsenha',
            url: '/colaborador/alterarsenha',
            templateUrl: 'Client/Views/Colaborador/alterarsenha.html',
        });

        // Efetivo (index)
        $stateProvider.state({
            name: 'layoutinterno.efetivo',
            url: '/efetivo/index',
            templateUrl: 'Client/Views/Efetivo/index.html',
        });
        // historicoacesso (index)
        $stateProvider.state({
            name: 'layoutinterno.historicoacesso',
            url: '/historicoacesso/index',
            templateUrl: 'Client/Views/HistoricoAcesso/index.html',
        });

        // Laudos (index, cadastro)
        $stateProvider.state({
            name: 'layoutinterno.laudos',
            url: '/laudos/index',
            templateUrl: 'Client/Views/Laudos/index.html',
        });
        $stateProvider.state({
            name: 'layoutinterno.laudocadastro',
            url: '/laudos/cadastro',
            templateUrl: 'Client/Views/Laudos/cadastro.html',
        });

        // Rdo (index, cadastro)
        $stateProvider.state({
            name: 'layoutinterno.rdocadastro',
            url: '/rdo/cadastro',
            templateUrl: 'Client/Views/Rdo/cadastro.html',
        });
        $stateProvider.state({
            name: 'layoutinterno.rdo',
            url: '/rdo/index',
            templateUrl: 'Client/Views/Rdo/index.html',
        });

        $stateProvider.state({
            title: 'Grafico',
            name: 'layoutinterno.chart',
            url: '/chart',
            templateUrl: 'Client/Views/Grafico/index.html'
        });

        $stateProvider.state({
            title: 'Grafico RDOs',
            name: 'layoutinterno.chartrdos',
            url: '/chart/rdos',
            templateUrl: 'Client/Views/Grafico/rdo.html'
        });

        $stateProvider.state({
            title: 'Gráfico Dias Improdutivos',
            name: 'layoutinterno.chartdiaimprodutivo',
            url: '/chart/diaimprodutivo',
            templateUrl: 'Client/Views/Grafico/diaimprodutivo.html'
        });

        $stateProvider.state({
            title: 'Gráfico Tarefas',
            name: 'layoutinterno.charttarefa',
            url: '/chart/tarefa',
            templateUrl: 'Client/Views/Grafico/tarefa.html'
        });

        $stateProvider.state({
            title: 'Gráfico Comentários',
            name: 'layoutinterno.chartcomentario',
            url: '/chart/comentario',
            templateUrl: 'Client/Views/Grafico/comentario.html'
        });

        // Dashboard (index)
        $stateProvider.state({
            name: 'layoutinterno.dashboard',
            url: '/dashboard/index',
            templateUrl: 'Client/Views/Dashboard/index.html',
        });

        $stateProvider.state({
            title: 'Dashboard Gráfico',
            name: 'layoutinterno.dashboardGrafico',
            url: '/dashboardGrafico',
            templateUrl: 'Client/Views/DashboardGrafico/index.html'
        });
        //$stateProvider.state({
        //    name: 'layoutinterno.dashboardPie',
        //    url: '/dashboard/index#pie',
        //    templateUrl: 'Client/Views/Dashboard/index#pie.html',
        //});
        //$stateProvider.state({
        //    name: 'layoutinterno.dashboardBarr',
        //    url: '/dashboard/index#column',
        //    templateUrl: 'Client/Views/Dashboard/index#column.html',
        //});



        // Empresa (index, cadastro)
        $stateProvider.state({
            name: 'layoutinterno.empresacadastro',
            url: '/empresa/cadastro',
            templateUrl: 'Client/Views/Empresa/cadastro.html',
        });

        $stateProvider.state({
            name: 'layoutinterno.empresa',
            url: '/empresa/index',
            templateUrl: 'Client/Views/Empresa/index.html',
        });


        // Relatório de Produtividade (index)
        $stateProvider.state({
            name: 'layoutinterno.relatorioprodutividade',
            url: '/relatorioprodutividade/index',
            templateUrl: 'Client/Views/RelatorioProdutividade/index.html',
        });


        // Relatório de Efetivo Diario (index)
        $stateProvider.state({
            name: 'layoutinterno.relatorioefetivodiario',
            url: '/relatorioefetivodiario/index',
            templateUrl: 'Client/Views/RelatorioEfetivoDiario/index.html',
        });


        // Relatório de Medição (index)
        $stateProvider.state({
            name: 'layoutinterno.relatoriomedicao',
            url: '/relatoriomedicao',
            templateUrl: 'Client/Views/RelatorioMedicao/index.html',
        });

        // Etapa (index, cadastro)
        $stateProvider.state({
            name: 'layoutinterno.etapacadastro',
            url: '/etapa/cadastro',
            templateUrl: 'Client/Views/Etapa/cadastro.html',
        });

        $stateProvider.state({
            name: 'layoutinterno.etapa',
            url: '/etapa/index',
            templateUrl: 'Client/Views/Etapa/index.html',
        });

    }]);

    app.run(['$rootScope', '$location', 'Auth', 'Permission', '$transitions', '$state', function ($rootScope, $location, Auth, Permission, $transitions, $state) {

        $transitions.onStart({}, function () {
            $rootScope.permission = Permission;
            $rootScope.title = 'Piscinas'; //$state.current.title;//(current.$$route === undefined) ? '' : current.$$route.title;
            $rootScope.dateVariable = new Date();
            $(window).scrollTop(0);

            var allowed = ['/login', '/convidada', '/esquecisenha']

            // caso sair
            if ($location.path() == '/sair') {
                Auth.logout();
                $location.path('/login');
                return;
            }

            //caso não logado tentando acessar tela de logado
            if (!Auth.isLoggedIn() && allowed.indexOf($location.path()) == -1 && $location.path() != '') {
                console.log('DENY 1');
                event.preventDefault();
                toastr.error('Você não tem permissão para acessar a funcionalidade.', '');
                $location.path('/login');
            }
            else if (Auth.isLoggedIn() && $location.path() == '/default') {
                //console.log('ALLOW DEFAULT');
            }
            else if (Auth.isLoggedIn()) { // caso logado e tentando acessar tela de logado
                if (allowed.indexOf($location.path()) == -1) {
                    var userData = Auth.getUser();
                    var found = false;
                    if (userData.routes) {
                        userData.routes.forEach(function (route) {
                            if (route.path == $location.path()) {
                                found = true;
                            }
                        });
                    }
                    if (!found) {
                        console.log('DENY 2');
                        event.preventDefault();
                        toastr.error('Você não tem permissão para acessar a funcionalidade.', '');
                        $location.path('/obra/escolher');
                    }
                }
                //console.log('ALLOW');
            }

        });

    }]);
})();