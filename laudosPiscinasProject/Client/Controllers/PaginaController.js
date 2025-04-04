(function () {
    'use strict';
    angular.module('app').controller('PaginaController', PaginaController);
    PaginaController.$inject = ['$http', '$location', 'ViewBag'];
    function PaginaController($http, $location, ViewBag) {
        var controller = this;

        this.pagedlist = {};
        this.status = [{ id: -1, nome: 'Selecione...' }, { id: 1, nome: 'Ativo' }, { id: 0, nome: 'Inativo' }, ];
        this.cadastroParam = { Id: '', titulo: '', alias: '', caminho: '', status: -1, listaAcao: [] };
        this.filtroParam = { titulo: '', status: 1 };
        this.orderby = '';
        this.orderbydescending = '';

        this.carregarEdicao = function () {
            var id = ViewBag.get('paginaId');
            if (id != "undefined" && id > 0) {
                $http({
                    url: "api/pagina/ObterPagina/",
                    method: "POST",
                    data: id
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    controller.cadastroParam = data;
                });
            }
            else {
                //console.log("cadastrar pagina");
            }
        }

        this.carregaListaAcao = function () {
            $http({
                url: "api/pagina/CarregaListaAcao",
                method: "POST"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.cadastroParam.listaAcao = data;
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
                url: "api/pagina/CarregarLista",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.pagedlist = data;
            });
        }

        this.salvar = function () {
            $http({
                url: "api/pagina/Salvar",
                method: "POST",
                data: this.cadastroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage)
            }).success(function (data, status, headers, config) {
                console.log(data);
                if (data) {
                    toastr.success("Registro salvo com sucesso.");
                }
                else {
                    toastr.error("Ocorreram erros na atualização")
                }
                controller.lista();
            });
        }

        this.deletar = function (linha) {
            $http({
                url: "api/pagina/Deletar",
                method: "POST",
                data: linha
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage)
            }).success(function (data, status, headers, config) {
                if (data) {
                    toastr.success("Registro excluído com sucesso.");
                }
                else {
                    toastr.error("Não foi possível realizar as atualizações");
                }
                controller.carregarLista(controller.pagedlist.currentPage);
            });
        }

        this.confirmarDeletar = function (linha) {
            MensagemConfirmacao('Tem certeza que deseja excluir ?', function () { controller.deletar(linha) });
        }

        this.lista = function () {
            $location.path('/pagina/index');
        }

        this.novo = function () {
            ViewBag.set('paginaId', undefined)
            $location.path('/pagina/cadastro');
        }

        this.editar = function (linha) {
            ViewBag.set('paginaId', linha.id)
            $location.path('/pagina/cadastro');
        }
    }
})();
