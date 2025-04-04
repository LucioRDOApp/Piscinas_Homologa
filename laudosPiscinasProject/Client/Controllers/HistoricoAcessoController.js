(function () {
    'use strict';
    angular.module('app').controller('HistoricoAcessoController', HistoricoAcessoController);
    HistoricoAcessoController.$inject = ['$http', '$location', 'ViewBag', 'Auth', '$scope', '$rootScope', 'Validacao', 'Email'];
    function HistoricoAcessoController($http, $location, ViewBag, Auth, $scope, $rootScope, Validacao, Email) {
        var controller = this;

        this.pagedlist = {};
        this.filtroParam = {
            col_nr_cpf: '', col_nm_colaborador: '', col_ds_email: '', data_login: moment(new Date()).format('DD-MM-YYYY')
        };
        this.orderby = '';
        this.orderbydescending = '';

        this.carregarLista = function (page, order) {

            $rootScope.tema = "tema-azul-claro";

            controller.filtroParam.page = page;

            if (controller.filtroParam.orderby == '') {
                controller.filtroParam.orderbydescending = '';
                controller.filtroParam.orderby = order;
            }
            else {
                controller.filtroParam.orderby = '';
                controller.filtroParam.orderbydescending = order;
            }

            controller.filtroParam.order = order;

            $http({
                url: "api/login/GetHistoricoLogin",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.pagedlist = data;
            });
        }

        this.cpfFilter = function (input) {
            var str = input + '';
            if (str.length <= 11) {
                str = str.replace(/\D/g, '');
                str = str.replace(/(\d{3})(\d)/, "$1.$2");
                str = str.replace(/(\d{3})(\d)/, "$1.$2");
                str = str.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
            }
            return str;
        };
    }
})();

