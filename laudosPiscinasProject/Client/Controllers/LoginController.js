(function () {
    'use strict';
    angular.module('app').controller('LoginController', LoginController);
    LoginController.$inject = ['$http', '$location', 'Auth'];
    function LoginController($http, $location, Auth) {
        var controller = this;
        this.filter = { "cpf": "", "email": "", "senha": "", "manterLogado": false }

        //######## remover depois
        //ver obra
        //controller.filter.cpf = 10924415061;
        //controller.filter.senha = 1234;

        // cadastrar tarefa
        //controller.filter.cpf = 74551730831;
        //controller.filter.senha = 1234;

        // cadastrar tarefa
        //controller.filter.cpf = 26019633101;
        //controller.filter.senha = 1234;

        // cadastrar tarefa
        // tem que cadastrar um colaborador com grupo 'operador'
        //controller.filter.cpf = 29600570400;
        //controller.filter.senha = 1234;
        //######## remover depois

        


        this.init = function () {
            if(Auth.isLoggedIn())
            {
                $location.path('/obra/escolher');
            }
        }

        this.login = function () {
            $http({
                url: "api/login/LoginUser",
                method: "POST",
                data: this.filter
            }).error(function (data, status, headers, config) {
                console.log(data);
                toastr.error(data.Message);
            }).success(function (data, status, headers, config) {
                Auth.setUser(data);
                $location.path('/obra/escolher');
            });
        }


        this.recuperarSenha = function () {
            $http({
                url: "api/colaborador/RecuperarSenha",
                method: "POST",
                data: this.filter
            }).error(function (data, status, headers, config) {
                toastr.error(data.Message);
            }).success(function (data, status, headers, config) {
                toastr.success("Senha enviada para: " + data);
                controller.voltar();
            });
        }

        this.voltar = function () {
            $location.path('/login');
        }

        this.esqueciSenha = function () {
            $location.path('/esquecisenha');
        }


        this.carregarObraConvidada = function () {
            var url = console.log(window.location.href);

            var idObra = controller.getParameterByName('o', url);
            var tipoConvite = controller.getParameterByName('t', url);

            console.log(idObra);
            console.log(tipoConvite);
        }

        this.getParameterByName = function (name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        }

        this.enviar = function () {
            $http({
                url: "api/login/RecuperarSenha",
                method: "POST",
                data: this.filter
            }).error(function (data, status, headers, config) {
                toastr.error(data.Message);
            }).success(function (data, status, headers, config) {
                toastr.success("Senha enviada para: " + data);
                controller.voltar();
            });
        }

    }
})();
