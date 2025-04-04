(function () {
    'use strict';

    angular.module('app').factory('Medicao', ['$resource', function ($resource) {
        return $resource('api/medicao/:id', { id: '@_id' }, {
            update: {
                method: 'PUT'
            },
            get: {
                isArray: true
            }
        });
    }]);

    angular.module('app').controller('RelatorioMedicaoController', RelatorioMedicaoController);
    RelatorioMedicaoController.$inject = ['$http', '$scope', '$location', 'Auth', 'Medicao', 'Download', 'Convert'];
    function RelatorioMedicaoController($http, $scope, $location, Auth, Medicao, Download, Convert) {

        $scope.filtroParam = {};
        $scope.statusTarefaList = [];
        $scope.obraList = [];

        this.carregaTela = function () {
            this.carregaStatusTarefaList();
            this.carregaObraList();
        }

        this.carregaStatusTarefaList = function () {
            $http.post('api/statusTarefa/Lista/')
            .success(function (data) { $scope.statusTarefaList = data; });
        }

        this.carregaObraList = function () {
            let idColaborador = Auth.getUser().usuario.id;
            $http.post('api/obra/obterobras/', { idColaborador: idColaborador })
            .success(function (data) { $scope.obraList = data; });
        }

        this.gerarPDF = function () {
            if (this.errorMessage().length > 0) {
                toastr.error(this.errorMessage(), 'Erro');
                return;
            }
            $scope.filtroParam.dataInicial = Convert.toJSONfromDateBR($scope.filtroParam.dataInicialTela);
            $scope.filtroParam.dataPrevisaoFinalObra = Convert.toJSONfromDateBR($scope.filtroParam.dataPrevisaoFinalObraTela);
            $scope.filtroParam.idObra = Auth.getUser().obra.idObra;
            $scope.filtroParam.tipoRelatorio = 'PDF';
            $http.post('api/medicao/gerarrelatorio', $scope.filtroParam)
            .success(function (data) {
                //let nomeRelatorio = 
                Download.base64(data, "Medicao.pdf");
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
            $scope.filtroParam.dataInicial = Convert.toJSONfromDateBR($scope.filtroParam.dataInicialTela);
            $scope.filtroParam.dataPrevisaoFinalObra = Convert.toJSONfromDateBR($scope.filtroParam.dataPrevisaoFinalObraTela);
            $scope.filtroParam.idObra = Auth.getUser().obra.idObra;
            $scope.filtroParam.tipoRelatorio = 'Excel';
            console.log(Auth.getUser().obra.descricao);
            $http.post('api/medicao/gerarrelatorio', $scope.filtroParam)
            .success(function (data) {
                Download.base64(data, "Medicao.xls", 'EXCEL');
            })
            .error(function (error) {
                toastr.error('Não foi possível gerar o Relatório. É necessário existir uma empresa contratada associada à obra: ' + Auth.getUser().obra.descricao, 'Erro');
            });
        }

        this.errorMessage = function () {
            if ($scope.filtroParam == {}) {
                return 'Por favor, preencha os campos obrigatórios.';
            }
            if ($scope.filtroParam.dataInicialTela == undefined || $scope.filtroParam.dataInicialTela == "") {
                return 'Por favor, escolha a Data Inicial.';
            }
            if ($scope.filtroParam.dataPrevisaoFinalObraTela == undefined || $scope.filtroParam.dataPrevisaoFinalObraTela == "") {
                return 'Por favor, escolha a Data Final.';
            }

            //if ($scope.filtroParam.idObra == undefined || $scope.filtroParam.idObra <= 0) {
            //    return 'Por favor selecione uma Obra.';
            //}
            return '';

        }
    }
})();
