(function () {
    'use strict';
    angular.module('app').controller('ConvidadaController', ConvidadaController);
    ConvidadaController.$inject = ['$http', '$location', 'Auth', 'Validacao'];
    function ConvidadaController($http, $location, Auth, Validacao) {
        var controller = this;
        this.cadastroParam = {
            cnpjEmpresa: '', razaoSocial: '', nomeFantasia: '', idRamo: '', idSetor: '', cpf: '', nomeColaborador: '', emailColaborador: '', idObra: '', tipoConvite: '',
            telefoneColaborador: '', senha: '', confirmacaoSenha: '', cargo: 0, dataContratacao: '', contratanteContratada: '', tipoLicenca: ''
        };

        this.bloquearCampos = true;
        this.bloquearCamposEmpresa = true;

        this.ufs = [];
        this.municipios = [];
        this.setores = [];
        this.ramos = [];
        this.cargos = [];

        this.init = function () {
            if (Auth.isLoggedIn()) {
                Auth.logout();
            }
            
            controller.verificarConvite();
            controller.carregaListaUF();
            controller.carregaListaMunicipio();
            controller.carregarListaSetores();
            controller.carregarListaRamos();
            controller.carregarCargos();
        }

        this.verificarConvite = function () {
            var url = window.location.href;
            var param = { idObra: controller.getParameterByName('o', url) };
            $http({
                url: "api/obra/VerificarConvite",
                method: "POST",
                data: param
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, 'Erro')
                controller.bloquearCampos = true;
                controller.bloquearCamposEmpresa = true;
            }).success(function (data, status, headers, config) {
                controller.bloquearCampos = false;
                controller.bloquearCamposEmpresa = false;
            });
        }

        this.login = function () {
            $http({
                url: "api/login/LoginUser",
                method: "POST",
                data: this.cadastroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, 'Erro')
            }).success(function (data, status, headers, config) {
                Auth.setUser(data);
                $location.path('/obra/escolher');
            });
        }

        this.carregarListaSetores = function () {
            controller.cadastroParam.idSetor = 0;
            $http({
                url: "api/setor/Lista",
                method: "GET"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.setores = data;
            });
        }

        this.carregarListaRamos = function () {
            controller.cadastroParam.idRamo = 0;
            $http({
                url: "api/ramo/Lista",
                method: "GET"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.ramos = data;
            });
        }

        this.carregaListaUF = function () {
            controller.cadastroParam.idUf = 0;            
            $http({
                url: "api/municipio/ListaUFConvidada",
                method: "POST"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.ufs = data;
            });
        }

        this.carregarCargos = function () {
            $http({
                url: "api/cargo/Lista/",
                method: "POST",
                data: undefined
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.cargos = data;
                controller.cargos.unshift({ id: 0, nome: 'Selecione o cargo...' });
            });
        }

        this.completarEmpresa = function () {
            if (typeof (controller.cadastroParam.cnpjEmpresa) == 'undefined' || controller.cadastroParam.cnpjEmpresa.length == 0) {
                controller.acessar();
                return;
            }
           

            $http({
                url: "api/empresa/ObterEmpresaCNPJ",
                method: "POST",
                data: { cnpj: controller.cadastroParam.cnpjEmpresa }
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                var empresa = data;
                if (data.cnpj == controller.cadastroParam.cnpjEmpresa) {
                    controller.preencherEmpresa(empresa);                   
                    controller.bloquearCamposEmpresa = true;
                }
                else {
                    controller.bloquearCamposEmpresa = false;                   
                }
            });
        }

        this.completarColaborador = function () {
            if (typeof (controller.cadastroParam.cpf) == 'undefined' || controller.cadastroParam.cpf.length == 0) {
                controller.acessar();
                return;
            }

            $http({
                url: "api/Colaborador/ObterColaboradorCPF",
                method: "POST",
                data: { cpf: controller.cadastroParam.cpf }
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                var colaborador = data;
                if (data.cpf == controller.cadastroParam.cpf) {
                    controller.preencherColaborador(colaborador);
                    controller.bloquearCampos = true;
                }
                else {
                    controller.bloquearCampos = false;
                }
            });
        }

        this.preencherEmpresa = function (empresa) {
            //controller.cadastroParam = empresa;
            controller.cadastroParam.razaoSocial = empresa.razaoSocial;
            controller.cadastroParam.nomeFantasia = empresa.nomeFantasia;
            controller.cadastroParam.idSetor = empresa.idSetor;
            controller.cadastroParam.idRamo = empresa.idRamo;
            
            
        }

        this.preencherColaborador = function (colaborador) {      
            colaborador.confirmacaoSenha = colaborador.senha;
            //controller.cadastroParam = colaborador;
            controller.cadastroParam.senha = colaborador.senha;
            controller.cadastroParam.confirmacaoSenha = colaborador.confirmacaoSenha;
            controller.cadastroParam.nomeColaborador = colaborador.nome;
            controller.cadastroParam.emailColaborador = colaborador.email;
            controller.cadastroParam.telefoneColaborador = colaborador.telefonePrincipal;
            controller.cadastroParam.idUf = colaborador.uf;
            controller.carregaListaMunicipioColaborador();
            controller.cadastroParam.idMunicipio = colaborador.idMunicipio;
        }

        this.carregaListaMunicipioColaborador = function () {
            var postdata = controller.cadastroParam.idUf;
            $http({
                url: "api/municipio/ListaMunicipioConvidada",
                method: "POST",
                data: postdata
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.municipios = data;
                controller.municipios.unshift({ idMunicipio: 0, municipio: 'Selecione o município...' });
            });
        }

        this.carregaListaMunicipio = function () {
            controller.cadastroParam.idMunicipio = 0;
            var postdata = controller.cadastroParam.idUf;
            $http({
                url: "api/municipio/ListaMunicipioConvidada",
                method: "POST",
                data: postdata
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.municipios = data;
                controller.municipios.unshift({ idMunicipio: 0, municipio: 'Selecione o município...' });
            });
        }

        this.acessar = function () {
            if (Validacao.required(controller.cadastroParam.cnpjEmpresa)) {
                toastr.error("O CNPJ deve ser preenchido");
                return;
            }
            if (!Validacao.cnpj(controller.cadastroParam.cnpjEmpresa)) {
                toastr.error("O CNPJ é inválido");
                return;
            }
            if (Validacao.required(controller.cadastroParam.razaoSocial)) {
                toastr.error("A Razão Social deve ser preenchida");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.razaoSocial)) {
                toastr.error("Por favor, introduza pelo menos três caracteres na Razão Social");
                return;
            }
            if (Validacao.required(controller.cadastroParam.nomeFantasia)) {
                toastr.error("O Nome Fantasia deve ser preenchido");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.nomeFantasia)) {
                toastr.error("Por favor, introduza pelo menos três caracteres no Nome Fantasia");
                return;
            }
            if (Validacao.required(controller.cadastroParam.cpf)) {
                toastr.error("O CPF deve ser preenchido");
                return;
            }
            if (!Validacao.cpf(controller.cadastroParam.cpf)) {
                toastr.error("O CPF é inválido");
                return;
            }
            if (Validacao.required(controller.cadastroParam.nomeColaborador)) {
                toastr.error("O Nome do Colaborador deve ser preenchido");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.nomeColaborador)) {
                toastr.error("Por favor, introduza pelo menos três caracteres no Nome do Colaborador");
                return;
            }
            if (Validacao.required(controller.cadastroParam.emailColaborador)) {
                toastr.error("O E-MAIL deve ser preenchido");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.emailColaborador)) {
                toastr.error("Por favor, introduza pelo menos três caracteres no E-MAIL");
                return;
            }
            if (!Validacao.email(controller.cadastroParam.emailColaborador)) {
                toastr.error("O E-MAIL é inválido");
                return;
            }
            if (Validacao.required(controller.cadastroParam.telefoneColaborador)) {
                toastr.error("O Telefone deve ser preenchido");
                return;
            }
            if (Validacao.required(controller.cadastroParam.idUf)) {
                toastr.error("A UF deve ser preenchida");
                return;
            }
            if (Validacao.required(controller.cadastroParam.idMunicipio)) {
                toastr.error("O Município deve ser preenchido");
                return;
            }
            if (Validacao.required(controller.cadastroParam.senha)) {
                toastr.error("A Senha deve ser preenchida");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.senha, 4)) {
                toastr.error("Por favor, introduza pelo menos quatro caracteres na Senha");
                return;
            }
            if (Validacao.required(controller.cadastroParam.confirmacaoSenha)) {
                toastr.error("A Confirmação de Senha deve ser preenchida");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.confirmacaoSenha, 4)) {
                toastr.error("Por favor, introduza pelo menos quatro caracteres na Confirmação de Senha");
                return;
            }

            if (controller.cadastroParam.senha != controller.cadastroParam.confirmacaoSenha) {
                toastr.error("As senhas digitadas devem ser idênticas.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.cargo)) {
                toastr.error("O Cargo deve ser preenchido.");
                return;
            }

            if (window.validate("#form-convidada")) {
                if (controller.cadastroParam.senha != controller.cadastroParam.confirmacaoSenha) {
                    toastr.error("As senhas digitads devem ser idênticas.");
                    return;
                }

                var url = window.location.href;
                controller.cadastroParam.idObra = controller.getParameterByName('o', url);

                $http({
                    url: "api/login/AcessoConvidada",
                    method: "POST",
                    data: this.cadastroParam
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, 'Erro')
                }).success(function (data, status, headers, config) {
                    Auth.setUser(data);
                    $location.search({});
                    $location.path('/obra/escolher');
                });
            }
            else {
                console.log('not validated');
            }
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

        this.voltar = function () {
            $location.search({});
            $location.path('/login');
        }


    }
})();
