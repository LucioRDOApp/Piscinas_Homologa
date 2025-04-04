(function () {
    'use strict';
    angular.module('app').controller('UsuarioController', UsuarioController);
    UsuarioController.$inject = ['$http', '$location', 'ViewBag'];
    function UsuarioController($http, $location, ViewBag) {
        var controller = this;

        this.usuarios = [];
        this.filter = { email: '', senha: '' };
        this.cadastroParam = { email: '', senha: '' };

        this.add = function () {
            $http({
                url: "api/usuario/Adicionar",
                method: "POST",
                data: this.cadastroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.voltar();
            });
        }

        this.cadastro = function () {
            ViewBag.set(undefined);
            $location.path('/usuario/cadastro');
        }

        this.editar = function (u) {
            ViewBag.set(u);
            $location.path('/usuario/cadastro');
        }

        this.delete = function (u) {
            MensagemConfirmacao("Vou apagar o usuario pode ser?", function () {
                $http({
                    url: "api/usuario/Excluir",
                    method: "POST",
                    data: { "id": u.usu_id_usuario }
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    controller.carregarLista();
                });
            });
        }

        this.voltar = function() {
            $location.path('/usuario/index');
        }

        this.limpar = function () {
            this.filter.email = '';
            this.filter.senha = '';
            this.carregarLista();
        }

        this.carregarLista = function () {
            $http({
                url: "api/usuario/ObterUsuarios",
                method: "POST",
                data: this.filter
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.usuarios = data;
            });
        }

        this.carregarUsuario = function () {
            var data = ViewBag.get();
            if (data != undefined)
            {
                controller.cadastroParam.email = data.usu_ds_email;
                controller.cadastroParam.senha = data.usu_ds_senha;
            }
        }
    }
})();
