(function () {
    'use strict';

    angular.module('app').factory('Etapa', ['$resource', function ($resource) {
        return $resource('api/etapa/:id', { id: '@_id' }, {
            update: {
                method: 'PUT'
            },
            get: {
                isArray: true
            }
        });
    }]);

    angular.module('app').controller('EtapaController', EtapaController);
    EtapaController.$inject = ['$scope', '$location', 'Auth', 'Etapa', '$http', 'ViewBag', 'Validacao'];
    function EtapaController($scope, $location, Auth, Etapa, $http, ViewBag, Validacao) {
        var controller = this;
        this.cadastroParam = { id: '', idObra: '', titulo: '', ordem: '', idEtapa: '' };
        this.filtroParam = { idObra: Auth.getUser().obra.idObra, titulo: '' };

        this.carregarTelaIndex = function () {
            Etapa.query(function (data) {
                $scope.etapas = data;
            });
        }

        this.carregarListaIndex = function () {
            Etapa.query(controller.filtroParam, function (data) {
                $scope.etapas = data;
            });
        }

        this.carregarLista = function (page, order) {
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
                url: "api/etapa/CarregarLista",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.pagedlist = data;
            });
        }

        this.novo = function () {
            ViewBag.set('etapaId', undefined);
            //controller.cadastroParam = { id: '', idObra: '', titulo: '', ordem: '' };
            //ViewBag.set('retornoCadastroEtapa', $location.path());
            $location.path('etapa/cadastro');
        }

        this.voltar = function () {
            $location.path('etapa/index');
        }

        this.excluir = function (id, titulo) {
            MensagemConfirmacao("Tem certeza que deseja excluir o registro: " + titulo + "?", function () {
                $http({
                    url: "api/etapa/excluir",
                    method: "POST",
                    data: id
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, 'Ocorreu um erro')
                }).success(function (data, status, headers, config) {
                    toastr.success("Registro excluído com sucesso.");
                    controller.carregarListaIndex();
                });

            });
        }

        this.editar = function (id) {
            ViewBag.set('etapaId', id);
            $location.path('etapa/cadastro');
        }

        this.obterEtapa = function () {
            let idEtapa = ViewBag.get('etapaId');
            if (idEtapa !== undefined && idEtapa > 0) {
                $http({
                    url: "api/etapa/obterEtapa",
                    method: "POST",
                    data: idEtapa
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    controller.cadastroParam = data;
                });
            }
        }

        this.salvar = function () {
            var user = Auth.getUser();
            this.cadastroParam.idobra = user.obra.idObra;
            let idEtapa = ViewBag.get('etapaId');
            if (Validacao.required(controller.cadastroParam.titulo)) {
                toastr.error("O Nome deve ser preenchido")
                return;
            }
            if (Validacao.required(controller.cadastroParam.ordem)) {
                toastr.error("O Nº Ordem deve ser preenchido.");
                return;
            }
            if (idEtapa === undefined || idEtapa === 0) {
                Etapa.save(this.cadastroParam, function () {
                    toastr.success("Registro salvo com sucesso.");
                    let telaAnterior = ViewBag.get('retornoCadastroEtapa');
                    if (telaAnterior == undefined || telaAnterior == '') {
                        $location.path('etapa/index');
                        return;
                    }
                    $location.path(telaAnterior);
                },
               function (error) {
                   //toastr.error(error.data.exceptionMessage);
                   //toastr.error(error.data.message);
                   //toastr.error(error.data.exceptionMessage, error.data.message);
                   toastr.error(error.data.exceptionMessage, 'Ocorreu um erro');
               });
            }
            else {
                $http({
                    url: "api/etapa/atualizarEtapa",
                    method: "POST",
                    data: controller.cadastroParam
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    toastr.success("Registro salvo com sucesso.");
                    $location.path('etapa/index');
                });
            }
        }
    }
})();
