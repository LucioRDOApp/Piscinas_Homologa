(function () {
    'use strict';
    angular.module('app').controller('RelatorioProdutividadeController', RelatorioProdutividadeController);
    RelatorioProdutividadeController.$inject = ['$http', '$location', 'ViewBag', 'Auth', 'Download'];
    function RelatorioProdutividadeController($http, $location, ViewBag, Auth, Download) {
        var controller = this;
        this.statusTarefa = [];
        this.filtroParam = { dataInicial: '', dataFinal: '', statusTarefa: 0, idObra: 0 };

        this.carregarStatusTarefa = function () {
            $http({
                url: "api/statusTarefa/Lista/",
                method: "POST",
                data: undefined
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.statusTarefa = data;
                controller.statusTarefa.unshift({ id: 0, nome: 'Selecione...' });
            });
        }

        this.gerarDocumento = function () {
            controller.filtroParam.idObra = Auth.getUser().obra.idObra;

            $http({
                url: "api/relatorioProdutividade/GerarRelatorio",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage)
            }).success(function (data, status, headers, config) {
                controller.redireciona();
                Download.base64(data, "Relatório de Produtividade.pdf");
            });
        }

        this.redireciona = function () {
            $location.path('/relatorioprodutividade/index');
        }
    }
})();