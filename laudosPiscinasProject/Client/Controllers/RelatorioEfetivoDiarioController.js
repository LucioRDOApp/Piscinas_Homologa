(function () {
    'use strict';
    angular.module('app').controller('RelatorioEfetivoDiarioController', RelatorioEfetivoDiarioController);
    RelatorioEfetivoDiarioController.$inject = ['$http', '$scope', '$location', 'ViewBag', 'Auth', 'Download', 'Convert'];
    function RelatorioEfetivoDiarioController($http, $scope, $location, ViewBag, Auth, Download, Convert) {

        $scope.filtroParam = {};

        this.gerarDocumento = function () {
            $scope.filtroParam.idObra = Auth.getUser().obra.idObra;

            $http({
                url: "api/relatorioProdutividade/GerarRelatorio",
                method: "POST",
                data: $scope.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage)
            }).success(function (data, status, headers, config) {
                controller.redireciona();
                Download.base64(data, "Relatório de Produtividade.pdf");
            });
        }

        this.redireciona = function () {
            $location.path('/relatorioefetivodiario/index');
        }

        this.gerarPDF = function () {
            if (this.errorMessage().length > 0) {
                toastr.error(this.errorMessage(), 'Erro');
                return;
            }
            const params = {
                dataInicial: Convert.toJSONfromDateBR($scope.filtroParam.dataInicial),
                dataFinal: Convert.toJSONfromDateBR($scope.filtroParam.dataFinal),
                idObra: Auth.getUser().obra.idObra,
                tipoRelatorio: 'PDF'
            }
            $http.post('api/efetivo/gerarrelatorio', params)
                .success(function (data) {
                    Download.base64(data, "EfetivoDiario_" + $scope.filtroParam.dataInicial + "_" + $scope.filtroParam.dataFinal + ".pdf");
                })
                .error(function (data) {
                    toastr.error(data.Message);
                });
        }

        this.gerarExcel = function () {
            if (this.errorMessage().length > 0) {
                toastr.error(this.errorMessage(), 'Erro');
                return;
            }
            const params = {
                dataInicial: Convert.toJSONfromDateBR($scope.filtroParam.dataInicial),
                dataFinal: Convert.toJSONfromDateBR($scope.filtroParam.dataFinal),
                idObra: Auth.getUser().obra.idObra,
                tipoRelatorio: 'Excel'
            }
            $http.post('api/efetivo/gerarrelatorio', params)
                .success(function (data) {
                    Download.base64(data, "EfetivoDiario_" + $scope.filtroParam.dataInicial + "_" + $scope.filtroParam.dataFinal + ".xls", 'EXCEL');
                })
                .error(function (data) {
                    toastr.error(data.Message);
                });
        }

        this.errorMessage = function () {
            if ($scope.filtroParam == {}) {
                return 'Por favor, preencha os campos obrigatórios.';
            }
            if ($scope.filtroParam.dataInicial == undefined || $scope.filtroParam.dataInicial == "") {
                return 'Por favor, escolha a Data Inicial.';
            }
            if ($scope.filtroParam.dataFinal == undefined || $scope.filtroParam.dataFinal == "") {
                return 'Por favor, escolha a Data Final.';
            }

            return '';

        }
    }
})();