(function () {
    'use strict';
    angular.module('app').controller('MenuController', MenuController);
    MenuController.$inject = ['$http', '$location', 'ViewBag', '$scope'];
    function MenuController($http, $location, ViewBag, $scope) {
        var controller = this;

        this.menus = [];
        this.paginasMenu = [];
        this.paginas = [];
        this.filter = { titulo: '', idMenu: '' };
        this.cadastroParam = { id: '', titulo: '', descricao: '', status: '', alias: '', listaPagina: [], listaPaginasRemovidas: [] };
        this.alias = { alias: '' };
        this.selectedPage = 0;
        this.selectedDelete = 0;
        this.menuorder = '';
        this.titulo = '';

        this.estrutura = [{ "id": "1", "name": "menu 1", "children": [] }, { "id": "3", "name": "menu 3", "children": [] }, { "id": "4", "name": "menu 4", "children": [] }, { "id": "5", "name": "menu 5", "children": [] }];

        this.adicionarPagina = function () {
            for (var i = 0; i < controller.paginas.length; i++) {
                if (controller.paginas[i].id == controller.selectedPage) {
                    //this.estrutura.push({ "id": controller.paginas[i].id, "name": controller.paginas[i].titulo, "children": [] });
                    controller.cadastroParam.listaPagina.push(controller.paginas[i]);
                    controller.paginas.splice(i, 1);
                    for (var i = 0; i < controller.cadastroParam.listaPaginasRemovidas.length; i++) {
                        if (controller.paginas[i].id == controller.cadastroParam.listaPaginasRemovidas[i].id) {
                            controller.paginas.splice(i, 1);
                        }
                    }
                }
            }
            controller.selectedPage = 0;
        }


        this.carregarLista = function () {
            $http({
                url: "api/menu/ObterMenu",
                method: "POST",
                data: this.filter
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, "Erro")
            }).success(function (data, status, headers, config) {
                controller.menus = data;
            });
        };
        
        this.obterPaginas = function () {
            $http({
                url: "api/Pagina/Lista",
                method: "POST",
                data: this.filter
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, "Erro")
            }).success(function (data, status, headers, config) {
                controller.paginas = [];
                controller.paginas = data;
                controller.paginas.unshift({ id: 0, titulo: 'Selecione...' });
            });
        }

        
        this.carregarEdicao = function () {
            var id = ViewBag.get('menuId');
            controller.filter.idMenu = id;
            
            if (id != "undefined" && id > 0) {
                $http({
                    url: "api/Menu/ObterDetalheMenu/",
                    method: "POST",
                    data: controller.filter
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, "Erro")
                }).success(function (data, status, headers, config) {
                    controller.cadastroParam = data;
                    controller.obterPaginas();
                });
            }
        }



        this.salvar = function () {
            $http({
                url: "api/menu/Salvar",
                method: "POST",
                data: controller.cadastroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, "Erro")
            }).success(function (data, status, headers, config) {
                toastr.success("Registro salvo com sucesso.");
                controller.voltar();
            });
        }

        this.editar = function (menu) {
            ViewBag.set('menuId', menu.men_id_menu);
            $location.path('/menu/cadastro');
        }

        this.delete = function (menu) {
            MensagemConfirmacao("Tem certeza que deseja excluir esse registro?", function () {
                $http({
                    url: "api/menu/Excluir",
                    method: "POST",
                    data: { "id": menu.men_id_menu }
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, "Erro")
                }).success(function (data, status, headers, config) {
                    controller.carregarLista();
                });
            });
        }

        this.add = function () {
            ViewBag.set('menuId', 0);
            $location.path('/menu/cadastro');
        }

        this.voltar = function () {
            $location.path('/menu/index');
        }

        this.deletarPagina = function (idPagina) {

            for (var i = 0; i < controller.cadastroParam.listaPagina.length; i++) {
                if (controller.cadastroParam.listaPagina[i].id == idPagina) {
                    this.paginas.push(controller.cadastroParam.listaPagina[i]);
                    var itemIndex = controller.cadastroParam.listaPagina.indexOf(this.paginasMenu[i]);
                    controller.cadastroParam.listaPagina.splice(itemIndex, 1);
                    controller.cadastroParam.listaPaginasRemovidas.push(controller.cadastroParam.listaPagina[i]);
                }
            }
        }
    }
})();





