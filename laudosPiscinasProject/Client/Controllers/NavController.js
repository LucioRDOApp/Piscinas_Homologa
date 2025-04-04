(function () {
    'use strict';
    angular.module('app').controller('NavController', NavController);
    NavController.$inject = ['$http', '$scope', 'Auth', '$location', 'ViewBag'];
    function NavController($http, $scope, Auth, $location, ViewBag) {
        var controller = this;
        this.userData = {};
        this.title = "Navigator";
        this.visible = true;
        this.nomeColaborador = "";
        this.desabilitarBotaoLogomarca = false;
        this.submenuCadastro = [];
        this.submenuRelatorio = [];

        $scope.$watch(Auth.changed, function (value) {
            controller.init();
        }, true);

        this.init = function () {
            controller.visible = !Auth.isLoggedIn();
            if (Auth.isLoggedIn()) {
                controller.userData = Auth.getUser();
                controller.listaPagina = controller.userData.menu.listaPagina.sort(function (a, b) {
                    if (a.titulo > b.titulo) {
                        return 1;
                    }
                    if (a.titulo < b.titulo) {
                        return -1;
                    }
                    return 0;
                });

                // -- SUBMENUS --------------------------------------------------------------------------------------------------------------
                //let listaPaginasCadastro = ['Colaboradores', 'Etapas', 'Empresas', 'Equipamentos', 'Obras'];
                //controller.submenuCadastro = controller.userData.menu.listaPagina.filter(p => listaPaginasCadastro.includes(p.titulo));

                //let listaPaginasRelatorio = ['RDOs', 'Efetivo Diário', 'Medição', 'Produtividade'];
                //controller.submenuRelatorio = controller.userData.menu.listaPagina.filter(p => listaPaginasRelatorio.includes(p.titulo));
                //---------------------------------------------------------------------------------------------------------------------------

                controller.desabilitarBotaoLogomarca = Auth.getUser().obraColaborador == null;
            }
            else {
                controller.userData = {};
                controller.desabilitarBotaoLogomarca = true;
            }
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        }

        this.goto = function (pagina) {
            $location.path(pagina.caminho);
        }

        this.mudarObra = function () {
            Auth.updateUser(Auth.getLoginUser());
            $location.path('/obra/escolher');
        }

        this.mudarSenha = function () {
            $location.path('/colaborador/alterarsenha');
        }

        this.dashboard = function () {
            var data = this.userData;
            var found = false;
            for (var i in data.routes) {
                if (data.routes[i].path == '/dashboard/index') {
                    found = true;
                }
            }
            if (found) {
                Auth.updateUser(data);
                $location.path('/dashboard/index');
            }
            else {
                toastr.error('Seu usuário não tem permissão. Favor contate o administrador.');
            }
            
        }

        this.novaObra = function () {
            ViewBag.set('obraId', 0);
            ViewBag.set('novaObra', true);

            ViewBag.set('last-page', $location.path());

            $location.path('/obra/cadastro');
        }

        this.listagemRdos = function () {
            $location.path('/rdo/index');
        }

        this.listagemLaudos = function () {
            $location.path('/laudos/index');
        }

        this.tarefaCards = function () {
            $location.path('/tarefa/cards');
        }

        this.listaObras = function () {
            $location.path('/obra/index');
        }

        this.redirectCharts = function () {
            $location.path('/chart');
        }

        this.redirectDashboard = function () {
            $location.path('/dashboardGrafico');
        }
    }
})();