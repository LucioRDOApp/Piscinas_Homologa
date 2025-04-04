(function () {
    'use strict';
    angular.module('app').controller('ColaboradorController', ColaboradorController);
    ColaboradorController.$inject = ['$http', '$location', 'ViewBag', 'Auth', '$scope', '$rootScope', 'Validacao', 'Email'];
    function ColaboradorController($http, $location, ViewBag, Auth, $scope, $rootScope, Validacao, Email) {
        var controller = this;

        this.pagedlist = {};
        this.colaboradores = [];
        this.menus = [];
        this.municipios = [];
        this.ufs = []; { }
        this.filtroParam = {
            nome: '', cpf: '', sexo: 'S', grupo: 0, dataNascimento: '', TelefonePrincipal: '', TelefoneSecundario: '', email: '',
            crea: '', Logradouro: '', NumeroEndereco: '', Complemento: '', Bairro: '', IdMunicipio: 0, uf: 0, cep: '', senha: '',
            confirmacaoSenha: '', foto: '', idObra: '', cargo: 0, dataContratacao: ''
        };
        this.sexo = [{ id: 'S', nome: 'Selecione...' }, { id: 'M', nome: 'Masculino' }, { id: 'F', nome: 'Feminino' }, ];
        this.grupos = [];
        this.cargos = [];
        this.cadastroParam = {
            Id: '', nome: '', cpf: '', sexo: 'S', grupo: 0, dataNascimento: '', TelefonePrincipal: '', descricaoFoto: '',
            TelefoneSecundario: '', email: '', crea: '', Logradouro: '', NumeroEndereco: '', Complemento: '', descricaoAssinatura: '',
            Bairro: '', IdMunicipio: 0, uf: 0, cep: 'aaa', senha: '', confirmacaoSenha: '', foto: '', idObra: '', fotoAssinatura: '',
            cargo: 0, dataContratacao: '', contratanteContratada: '', senhaAtual: '', descricaoGrupo: ''
        };
        this.orderby = '';
        this.orderbydescending = '';
        this.selectedMenu = 1;
        this.colaborador = {};
        this.desabilitarCampos = false;
        this.desabilitarCampos2 = false;
        this.perfilColaborador = false;
        this.desabilitarCpf = true;

        this.carregarLista = function (page, order) {
            $rootScope.tema = "tema-azul-claro";

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

            var user = Auth.getUser();
            controller.filtroParam.idObra = user.obra.idObra;
            controller.filtroParam.idColaborador = user.usuario.id;
            controller.filtroParam.contratanteContratada = user.obraColaborador.contratanteContratada;

            $http({
                url: "api/colaborador/CarregarLista",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.pagedlist = data;
            });
        }


        this.carregarGrupos = function () {
            var user = Auth.getUser();
            var postData = {};
            postData.contratanteContratada = user.obraColaborador.contratanteContratada;
            postData.tipoLicenca = user.obraColaborador.tipoLicencaColaboradorGrupo;
            postData.idLicenca = user.obraColaborador.idLicenca;

            $http({
                url: "api/grupo/ListaPeloGrupo/",
                method: "POST",
                data: postData
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.grupos = data;
                controller.grupos.unshift({ id: 0, nome: 'Selecione...' });
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
                controller.cargos.unshift({ id: 0, nome: 'Selecione...' });
            });
        }


        this.carregaListaMunicipio = function (tipoTela, selected) {
            var postdata;

            if (tipoTela == 'listagem') {
                postdata = controller.filtroParam.uf;
            }
            else {
                postdata = controller.cadastroParam.uf;
            }

            $http({
                url: "api/municipio/ListaMunicipio/",
                method: "POST",
                data: postdata
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.municipios = data;
                if (tipoTela == 'listagem') {
                    controller.filtroParam.idMunicipio = 0;
                }
                else if (selected != undefined) {
                    controller.cadastroParam.idMunicipio = selected;
                }
                else {
                    controller.cadastroParam.idMunicipio = 0;
                }
                controller.municipios.unshift({ idMunicipio: 0, municipio: 'Selecione...' });
            });
        }

        this.carregaListaUF = function () {
            $http({
                url: "api/municipio/ListaUF",
                method: "POST"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.ufs = data;
                controller.ufs.unshift({ idUF: 0, uf: 'Selecione...' });
            });
        }



        this.carregarEdicao = function () {
            var user = Auth.getUser();
            var idObra = user.obra.idObra;
            var id = ViewBag.get('colaboradorId');

            var postData = { id: id, idObra: idObra };
            if (id != "undefined" && id > 0) {
                $http({
                    url: "api/colaborador/ObterColaborador/",
                    method: "POST",
                    data: postData
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    controller.cadastroParam = data;

                    controller.cadastroParam.confirmacaoSenha = controller.cadastroParam.senha;
                    controller.carregaListaMunicipio('cadastro', data.idMunicipio);
                    controller.desabilitarCampos = ViewBag.get('desabilitarCampos');
                    controller.desabilitarCampos2 = controller.desabilitarCampos;
                    if (!controller.desabilitarCampos2) {
                        controller.desabilitarCampos2 = (user.obra.idColaborador == id);
                    }
                    setTimeout(function () {
                        if (controller.cadastroParam.grupo != undefined && !isNaN(controller.cadastroParam.grupo)) {
                            //todo: ver maneira de pegar o valor do colaborador parametrizado, pois está fixado o id
                            if (controller.cadastroParam.grupo == controller.grupos.find(grupo => grupo.nome == 'Colaborador').id || controller.cadastroParam.grupo == controller.grupos.find(grupo => grupo.nome == 'Terceirizado').id) { // Colaborador 
                                controller.perfilColaborador = true;
                                controller.desabilitarCpf = false;
                                $scope.$apply();
                                return;
                            }
                            controller.perfilColaborador = false;
                        }
                    }, 400);
                });
            }
            else {
                controller.desabilitarCpf = false;
                //$location.path("/colaborador/index");
            }
        }

        controller.verificaPerfilColaborador = function () {
            if (controller.cadastroParam.grupo != undefined && !isNaN(controller.cadastroParam.grupo)) {

                //todo: ver maneira de pegar o valor do colaborador parametrizado, pois está fixado o id
                if (controller.cadastroParam.grupo == controller.grupos.find(grupo => grupo.nome == 'Colaborador').id || controller.cadastroParam.grupo == controller.grupos.find(grupo => grupo.nome == 'Terceirizado').id) { // Colaborador 
                    controller.perfilColaborador = true;
                    return;
                }
                controller.perfilColaborador = false;
            }
        }


        this.salvar = function () {
            if (Validacao.required(controller.cadastroParam.cpf) && !controller.perfilColaborador) {
                toastr.error("O CPF deve ser preenchido.");
                return;
            }
            if (controller.cadastroParam.cpf != undefined && !Validacao.cpf(controller.cadastroParam.cpf) && !controller.perfilColaborador) {
                toastr.error("O CPF é inválido.");
                return;
            }
            if (Validacao.required(controller.cadastroParam.grupo)) {
                toastr.error("O Perfil deve ser preenchido.");
                return;
            }
            if (Validacao.required(controller.cadastroParam.nome)) {
                toastr.error("O Nome deve ser preenchido.");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.nome)) {
                toastr.error("Por favor, introduza pelo menos três caracteres no Nome.");
                return;
            }
            if (Validacao.required(controller.cadastroParam.cargo)) {
                toastr.error("O Cargo deve ser preenchido.");
                return;
            }
            if ((Validacao.required(controller.cadastroParam.sexo) || controller.cadastroParam.sexo == 'S') && !controller.perfilColaborador) {
                toastr.error("O Sexo deve ser preenchido.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.dataNascimento) && !controller.perfilColaborador) {
                toastr.error("A Data de Nascimento deve ser preenchida.");
                return;
            }

            if (!Validacao.data(controller.cadastroParam.dataNascimento) && !controller.perfilColaborador) {
                toastr.error("A Data de Nascimento é inválida.");
                return;
            }
            if (Validacao.required(controller.cadastroParam.telefonePrincipal) && !controller.perfilColaborador) {
                toastr.error("O Telefone Principal deve ser preenchido.");
                return;
            }

            if (controller.cadastroParam.telefonePrincipal != '' && controller.cadastroParam.telefonePrincipal != undefined && controller.cadastroParam.telefonePrincipal.replace(/_/g, '').replace('(', '').replace(')', '').replace(' ', '').replace('-', '').length < 10) {
                toastr.error("Telefone Principal inválido");
                return;
            }

            if (controller.cadastroParam.telefoneSecundario != '' && controller.cadastroParam.telefoneSecundario != undefined && controller.cadastroParam.telefoneSecundario.replace(/_/g, '').replace('(', '').replace(')', '').replace(' ', '').replace('-', '').length < 10) {
                toastr.error("Telefone Secundário inválido");
                return;
            }

            if (Validacao.required(controller.cadastroParam.email) && !controller.perfilColaborador) {
                toastr.error("O E-mail precisa ser preenchido.");
                return;
            }

            if (!Validacao.required(controller.cadastroParam.email) && !Email.checkEmail(controller.cadastroParam.email)) {
                toastr.error("O E-mail é inválido.");
                return;
            }
            
            if (Validacao.required(controller.cadastroParam.logradouro) && !controller.perfilColaborador) {
                toastr.error("O Logradouro deve ser preenchido.");
                return;
            }
            if (Validacao.required(controller.cadastroParam.numeroEndereco)) {
                toastr.error("O Número deve ser preenchido.");
                return;
            }
            if (Validacao.required(controller.cadastroParam.uf) && !controller.perfilColaborador) {
                toastr.error("A UF deve ser preenchida.");
                return;
            }
            if (Validacao.required(controller.cadastroParam.idMunicipio) && !controller.perfilColaborador) {
                toastr.error("O Município deve ser preenchido.");
                return;
            }
            if (Validacao.required(controller.cadastroParam.cep)) {
                toastr.error("O CEP deve ser preenchido.");
                return;
            }
            if (Validacao.required(controller.cadastroParam.senha) && !controller.perfilColaborador) {
                toastr.error("A Senha deve ser preenchida.");
                return;
            }

            if (!Validacao.required(controller.cadastroParam.confirmacaoSenha) && !Validacao.minLenght(controller.cadastroParam.senha, 4)) {
                toastr.error("Por favor, introduza pelo menos 4 caracteres na Senha do Colaborador.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.confirmacaoSenha) && !controller.perfilColaborador) {
                toastr.error("A Confirmação Senha deve ser preenchida.");
                return;
            }

            if (!Validacao.required(controller.cadastroParam.confirmacaoSenha) && !Validacao.minLenght(controller.cadastroParam.confirmacaoSenha, 4)) {
                toastr.error("Por favor, introduza pelo menos 4 caracteres na Confirmação da Senha.");
                return;
            }

            if (window.validate("#form-cadastro-colaborador")) {
                if (!Validacao.required(controller.cadastroParam.confirmacaoSenha) && !Validacao.required(controller.cadastroParam.senha) && controller.cadastroParam.senha != controller.cadastroParam.confirmacaoSenha) {
                    toastr.error("As senhas digitadas devem ser idênticas.");
                    return;
                }
                var user = Auth.getUser();
                controller.cadastroParam.idObra = user.obra.idObra;
                controller.cadastroParam.contratanteContratada = user.obraColaborador.contratanteContratada;

                controller.cadastroParam.dataContratacao = $('.dataContratacao').val();
                controller.cadastroParam.dataNascimento = $('.dataNascimento').val();

                if ($scope.imageFileColaborador != undefined) {
                    controller.cadastroParam.foto = $scope.imageFileColaborador;
                }

                if ($scope.imageFileAssinaturaColaborador != undefined) {
                    controller.cadastroParam.fotoAssinatura = $scope.imageFileAssinaturaColaborador;
                }

                $http({
                    url: "api/colaborador/Salvar",
                    method: "POST",
                    data: this.cadastroParam
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, "Erro.");
                }).success(function (data, status, headers, config) {
                    controller.lista();
                    toastr.success("Registro salvo com sucesso.");
                });
            }

            else {
                console.log('not validated!!!');
            }
        }

        this.alterarSenha = function () {
            if (Validacao.required(controller.cadastroParam.senhaAtual)) {
                toastr.error("Por favor, Digite a senha atual do colaborador");
                return;
            }

            if (!Validacao.minLenght(controller.cadastroParam.senhaAtual, 4)) {
                toastr.error("Por favor, introduza pelo menos 4 caracteres na Senha Atual");
                return;
            }

            if (Validacao.required(controller.cadastroParam.senha)) {
                toastr.error("Por favor, Digite a nova senha do coloborador");
                return;
            }

            if (!Validacao.minLenght(controller.cadastroParam.senha, 4)) {
                toastr.error("Por favor, introduza pelo menos 4 caracteres na Nova Senha");
                return;
            }

            if (Validacao.required(controller.cadastroParam.confirmacaoSenha)) {
                toastr.error("Por favor, Confirme a nova senha do coloborador");
                return;
            }

            if (!Validacao.minLenght(controller.cadastroParam.confirmacaoSenha, 4)) {
                toastr.error("Por favor, introduza pelo menos 4 caracteres na Confirmação de Senha");
                return;
            }
            var senhaAtual = Auth.getUser().usuario.senha;
            var idColaborador = Auth.getUser().usuario.id;
            controller.cadastroParam.Id = idColaborador;


            if (window.validate("#form-senha-colaborador")) {
                if (controller.cadastroParam.senhaAtual != senhaAtual) {
                    toastr.error("A senha atual inválida.");
                    return;
                }
                if (controller.cadastroParam.senha != controller.cadastroParam.confirmacaoSenha) {
                    toastr.error("As senhas digitads devem ser ideênticas.");
                    return;
                }


                $http({
                    url: "api/colaborador/AtualizarSenha",
                    method: "POST",
                    data: this.cadastroParam
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, "Erro.");
                }).success(function (data, status, headers, config) {
                    toastr.success("Cadastro realizado com sucesso.");
                    $location.path('/sair');
                });
            }

            else {
                console.log('not validated!!!');
            }
        }


        this.deletar = function (colaborador) {
            var user = Auth.getUser();
            var meuid = user.usuario.id;
            colaborador.idObra = user.obra.idObra;

            MensagemConfirmacao("Tem certeza que deseja excluir o registro: " + colaborador.nome + "?", function () {
                $http({
                    url: "api/colaborador/Deletar",
                    method: "POST",
                    data: colaborador
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, "Erro")
                }).success(function (data, status, headers, config) {
                    if (data) {
                        controller.carregarLista(controller.pagedlist.currentPage);
                        toastr.success("Registro excluído com sucesso.");
                        if (colaborador.idColaborador == meuid) {
                            $location.path("/obra/escolher");
                        }
                    }
                    else {
                        toastr.error("O usuário não pode ser excluído pois existem registros associados a ele.")
                    }
                });
            });
        }

        this.editar = function (colaborador) {
            var user = Auth.getUser();
            controller.cadastroParam.idObra = user.obra.idObra;
            colaborador.idObra = user.obra.idObra;
            ViewBag.set('colaboradorIdObra', user.obra.idObra);

            ViewBag.set('desabilitarCampos', false);

            ViewBag.set('colaboradorId', colaborador.idColaborador);
            $location.path('/colaborador/cadastro');
        }


        this.visualizar = function (colaborador) {
            var user = Auth.getUser();
            controller.cadastroParam.idObra = user.obra.idObra;
            colaborador.idObra = user.obra.idObra;
            ViewBag.set('colaboradorIdObra', user.obra.idObra);

            ViewBag.set('desabilitarCampos', true);

            ViewBag.set('colaboradorId', colaborador.idColaborador);
            $location.path('/colaborador/cadastro');
        }

        this.novo = function () {
            ViewBag.set('colaboradorId', undefined)
            $location.path('colaborador/cadastro');
        }

        this.voltar = function () {
            $location.path('colaborador/index');
        }

        this.cards = function () {
            $location.path('tarefa/cards');
        }

        this.lista = function () {
            $location.path('colaborador/index');
        }

        this.completarColaborador = function () {
            if (typeof (controller.cadastroParam.cpf) == 'undefined' || controller.cadastroParam.cpf.length == 0) {
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
                    controller.preencherModalColaborador(colaborador);
                    controller.desabilitarCamposColaborador2 = false;
                }
                else {
                    controller.desabilitarCamposColaborador = false;
                    controller.desabilitarCamposColaborador2 = false;
                }
            });
        }

        this.preencherModalColaborador = function (colaborador) {
            controller.carregarGrupos();
            let dadosImportantes = { grupo: controller.cadastroParam.grupo, cargo: controller.cadastroParam.cargo };
            colaborador.confirmacaoSenha = colaborador.senha;
            controller.cadastroParam = colaborador;
            controller.cadastroParam.grupo = dadosImportantes.grupo;
            controller.cadastroParam.cargo = dadosImportantes.cargo;
            controller.buscaTextoColaborador = colaborador.nome;
            controller.desabilitarCamposColaborador = true;
            controller.desabilitarCamposColaborador2 = true;
            controller.carregaListaMunicipio('cadastro', colaborador.idMunicipio);
        }
    }
})();

