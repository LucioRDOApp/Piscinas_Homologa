(function () {
    'use strict';
    angular.module('app').controller('DashboardController', DashboardController);
    DashboardController.$inject = ['$http', '$location', 'Auth'];
    function DashboardController($http, $location, Auth) {
        var controller = this;
        this.filter = { "cpf": "", "email": "", "senha": "", "manterLogado": false };
        this.pieChartData = [];
        this.controllerData = 'lalala';

        this.carregarDashboards = function () {
            controller.filter.idObra = Auth.getUser().obra.idObra;

            $http({
                url: "api/dashboard/CarregarDashboards",
                method: "POST",
                data: this.filter
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, 'Erro');
            }).success(function (data, status, headers, config) {
                GerarGraficoPizza("containerPizza", data.pizzaData);
                GerarGraficoPizzaEfetivo("containerEfetivo", data.pizzaEfetivoDataValues);

                var colors = [];
                for (var i in data.barraDataCategories) {
                    switch(data.barraDataCategories[i]){
                        case 'Em Execução': colors.push("#51BCDC");
                        case 'Planejada': colors.push("#999999");
                        case 'Cancelada': colors.push("#D04541");
                        case 'Pausada': colors.push("#FF8000");
                        case 'Finalizada': colors.push("#57B257");
                    }
                }

                GerarGraficoBarra2("containerBarr", data.barraDataCategories, data.barraDataValues, colors);

                $('#containerBarr .highcharts-legend').hide();
            });
        }


        this.cards = function () {
            $location.path('/tarefa/cards');
        }

        this.relatorioProdutividade = function () {
            $location.path('/relatorioprodutividade/index');
        }

        this.relatorioEfetivoDiario = function () {
            $location.path('/relatorioefetivodiario/index');
        }

        this.relatorioMedicao = function () {
            $location.path('/relatoriomedicao');
        }
    }
})();
