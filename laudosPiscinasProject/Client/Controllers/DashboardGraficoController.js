(function () {
    'use strict';
    angular.module('app').controller('DashboardGraficoController', DashboardGraficoController);
    DashboardGraficoController.$inject = ['$http', '$location', 'Auth', '$rootScope', '$scope'];
    function DashboardGraficoController($http, $location, Auth, $rootScope, $scope) {
        var controller = this;
        this.filtroParam = { unidadeEscolar: 0, dataInicial: '', dataFinal: '' };
        this.unidadesEscolares = [];

        this.totalLaudos = 0;

        this.exibirResultado = true;

        //Variaveis da tabela
        this.nivelCloroSim = 2;
        this.nivelCloroNao = 1;

        this.phSim = 1;
        this.phNao = 1;

        this.limpidezSim = 1;
        this.limpidezNao = 1;

        this.superficieSim = 1;
        this.superficieNao = 1;

        this.fundoSim = 1;
        this.fundoNao = 1;

        this.nivelCloro2Sim = 1;
        this.nivelCloro2Nao = 1;

        this.bacteriasSim = 1;
        this.bacteriasNao = 1;

        this.proliferacaoSim = 1;
        this.proliferacaoNao = 1;

        this.carregarDashboards = function () {
            this.carregarLista();
        }

        // Tela de SELEÇÃO de obra
        this.carregarLista = function () {
            $rootScope.tema = "tema-azul-escuro";
            controller.filtroParam.idColaborador = Auth.getUser().usuario.id;

            $http({
                url: "api/obra/ObterObras",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.unidadesEscolares = data;
            });
        }

        this.carregarDashboard = function () {
            controller.filtroParam.dataInicial = $('.txbDataInicial').val();
            controller.filtroParam.dataFinal = $('.txbDataFinal').val();

            $http({
                url: "api/laudo/DashboardGrafico",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.totalLaudos = data.length;

                //Preenchendo a tabela
                controller.nivelCloroSim = data.filter(x => x.laudo_tp_nivel_cloro === true).length;
                controller.nivelCloroNao = data.filter(x => x.laudo_tp_nivel_cloro === false).length;

                controller.phSim = data.filter(x => x.laudo_tp_ph === true).length;
                controller.phNao = data.filter(x => x.laudo_tp_ph === false).length;

                controller.limpidezSim = data.filter(x => x.laudo_tp_limpidez === true).length;
                controller.limpidezNao = data.filter(x => x.laudo_tp_limpidez === false).length;

                controller.superficieSim = data.filter(x => x.laudo_tp_superficie === true).length;
                controller.superficieNao = data.filter(x => x.laudo_tp_superficie === false).length;

                controller.fundoSim = data.filter(x => x.laudo_tp_fundo === true).length;
                controller.fundoNao = data.filter(x => x.laudo_tp_fundo === false).length;

                controller.nivelCloro2Sim = data.filter(x => x.laudo_tp_nivel_cloro_2 === true).length;
                controller.nivelCloro2Nao = data.filter(x => x.laudo_tp_nivel_cloro_2 === false).length;

                controller.bacteriasSim = data.filter(x => x.laudo_tp_nivel_bacterias === true).length;
                controller.bacteriasNao = data.filter(x => x.laudo_tp_nivel_bacterias === false).length;

                controller.proliferacaoSim = data.filter(x => x.laudo_tp_nivel_proliferacao === true).length;
                controller.proliferacaoNao = data.filter(x => x.laudo_tp_nivel_proliferacao === false).length;

                $scope.chart = ConstruirHighchartLaudos(data, controller);
                GerarGraficoBar('container', $scope.chart);

                controller.exibirResultado = false;
            });
        }
    }
})();
