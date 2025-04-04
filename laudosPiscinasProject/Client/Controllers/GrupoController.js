(function () {
    'use strict';
    angular.module('app').controller('GrupoController', GrupoController);
    GrupoController.$inject = ['$http', '$location', 'ViewBag', 'Validacao'];
    function GrupoController($http, $location, ViewBag, Validacao) {
        var controller = this;

        this.pagedlist = {};
        this.grupos = [];
        this.licencas = [];
        this.menus = [];
        this.filter = { nome: '' };
        this.filtroParam = { nome: '' };
        this.cadastroParam = { Id: '', nome: '', IdMenu: '', Menu: '', ObraColaborador: [], Usuario: [], listaGrupoPaginaAcao: [] };
        this.orderby = '';
        this.orderbydescending = '';
        this.selectedMenu = 1;
        this.grupo = {};

        this.diretores = [{ id: 0, titulo: "Comum" }, { id: 1, titulo: "Diretor" }]
        this.contratante = [{ id: 0, titulo: "Sem vínculo" }, { id: 1, titulo: "Contratante" }, { id: 2, titulo: "Contratada" }]

        this.init = function () {
            controller.obterMenus();
            controller.obterLicencas();
            controller.carregarEdicao();
            controller.carregaListaGrupoPaginaAcao();
        }

        this.carregaListaGrupoPaginaAcao = function () {
            var id = ViewBag.get('grupoId');

            $http({
                url: "api/grupo/CarregaListaGrupoPaginaAcao",
                method: "POST",
                data: id
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.cadastroParam.listaGrupoPaginaAcao = data;
                console.log(controller.cadastroParam.listaGrupoPaginaAcao);
            });
        }

        this.obterLicencas = function () {
            $http({
                url: "api/licenca/ObterLicencas",
                method: "GET"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.licencas = data;
                controller.licencas.unshift({ id: 0, nome: "Sem vínculo" })
            });
        }

        this.obterMenus = function () {
            $http({
                url: "api/menu/ObterMenu",
                method: "POST",
                data: this.filter
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.menus = data;
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
                url: "api/grupo/CarregarLista",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.pagedlist = data;
            });
        }



        this.carregarEdicao = function () {
            var id = ViewBag.get('grupoId');
            console.log(ViewBag);
            console.log('id:' + id);
            if (id != "undefined" && id > 0) {
                console.log("editar grupo " + id);
                $http({
                    url: "api/grupo/ObterGrupo/",
                    method: "POST",
                    data: id
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    console.log(data);
                    controller.cadastroParam = data;
                });
            }
        }


        this.salvar = function () {
            if (Validacao.required(controller.cadastroParam.nome)) {
                toastr.error("A Descrição precisa ser preenchida");
                return;
            }
            $http({
                url: "api/grupo/Salvar",
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
                    toastr.error("Não foi possível realizar as atualizações")
                }
                controller.lista();
            });
        }


        this.deletar = function (grupo) {
            MensagemConfirmacao("Tem certeza que deseja excluir o registro: " + grupo.nome + "?", function () {
                $http({
                    url: "api/grupo/Deletar",
                    method: "POST",
                    data: grupo
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

            });
        }

        this.editar = function (grupo) {
            ViewBag.set('grupoId', grupo.id);
            $location.path('/grupo/cadastro');
        }

        this.novo = function () {
            ViewBag.set('grupoId', undefined)
            $location.path('/grupo/cadastro');
        }

        this.voltar = function () {
            $location.path('/grupo/index');
        }

        this.lista = function () {
            $location.path('/grupo/index');
        }

    }
})();
