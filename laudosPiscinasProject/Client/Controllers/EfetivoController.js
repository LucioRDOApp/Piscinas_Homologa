(function () {
    'use strict';
    angular.module('app').controller('EfetivoController', EfetivoController);
    EfetivoController.$inject = ['$http', '$location', 'ViewBag', 'Auth', '$scope', '$rootScope', 'Validacao', 'Email'];
    function EfetivoController($http, $location, ViewBag, Auth, $scope, $rootScope, Validacao, Email) {
        var controller = this;

        this.pagedlist = {};
        this.filtroParam = {
            col_nm_colaborador: '', oco_id_cargo: 0, est_id_efetivo_status: 0, efe_data: moment(new Date()).format('DD-MM-YYYY'), data_copia: ''
        };
        this.efetivo_status = [];
        this.cargos = [];
        this.cadastroParam = {
            Id: '', nome: '', cpf: '', sexo: 'S', grupo: 0, dataNascimento: '', TelefonePrincipal: '', descricaoFoto: '',
            TelefoneSecundario: '', email: '', crea: '', Logradouro: '', NumeroEndereco: '', Complemento: '', descricaoAssinatura: '',
            Bairro: '', IdMunicipio: 0, uf: 0, cep: 'aaa', senha: '', confirmacaoSenha: '', foto: '', idObra: '', fotoAssinatura: '',
            cargo: 0, dataContratacao: '', contratanteContratada: '', senhaAtual: '', descricaoGrupo: '', colaboradorObj: { nome: '' },
        };
        this.orderby = '';
        this.orderbydescending = '';

        this.desabilitarCamposColaborador;
        this.desabilitarCamposColaborador2;
        this.desabilitarCpf = false;
        controller.perfilColaborador = false;
        this.municipiosColaborador = [];
        this.ufsColaborador = [];

        this.sexo = [{ id: 'M', nome: 'Masculino' }, { id: 'F', nome: 'Feminino' },];

        this.changeData = function () {
            if (Validacao.data(this.filtroParam.efe_data)) {
                controller.carregarLista(1)
            }
        }

        this.carregarLista = function (page, order) {
            if (!Validacao.minLenght(this.filtroParam.efe_data)) {
                toastr.error("Por favor, selecione uma data.");
                return;
            }

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
            controller.filtroParam.oco_id_obra = user.obra.idObra;

            $http({
                url: "api/efetivo/CarregarLista",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.pagedlist = data;
            });
        }

        this.carregarStatus = function () {
            $http({
                url: "api/efetivo/CarregarListaEfetivoStatus/",
                method: "POST",
                data: undefined
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.efetivo_status = data;
                controller.efetivo_status.unshift({ est_id_efetivo_status: 0, est_ds_efetivo_status: 'Selecione...' });
            });
        }

        this.UltimaDataLancada = function () {
            var user = Auth.getUser();
            $http({
                url: "api/efetivo/UltimaDataLancada?oco_id_obra=" + user.obra.idObra,
                method: "GET",
                data: undefined
            }).error(function (data, status, headers, config) {
                toastr.error(data.Message, data.message)
            }).success(function (data, status, headers, config) {
                let dataStr = data.split('T')[0];
                controller.filtroParam.efe_data = dataStr.split('-')[2] + '-' + dataStr.split('-')[1] + '-' + dataStr.split('-')[0]
                controller.carregarLista(1)
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

        this.changeEfetivoStatus = function (efetivo) {
            $http({
                url: "api/efetivo/ChangeEfetivoStatus",
                method: "POST",
                data: efetivo
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.carregarLista(1)
            });
        }

        this.copiarEfetivo = function () {
            if (!Validacao.minLenght(this.filtroParam.data_copia)) {
                toastr.error("Por favor, selecione uma data que deseja copiar o efetivo.");
                return;
            }

            const user = Auth.getUser();

            const params = {
                efe_data: controller.filtroParam.efe_data,
                oco_id_obra: user.obra.idObra,
                data_copia: controller.filtroParam.data_copia
            }
            $http({
                url: "api/efetivo/CopiarEfetivo",
                method: "POST",
                data: params
            }).error(function (data, status, headers, config) {
                toastr.error(data.Message, data.message)
            }).success(function (data, status, headers, config) {
                if (data) {
                    controller.filtroParam.efe_data = params.data_copia;
                    controller.carregarLista(1)
                    toastr.success("Efetivo copiado com sucesso");
                    $('#modalCopiarEfetivo').modal('hide')
                } else {
                    toastr.error("Por favor, selecione uma data que deseja copiar o efetivo.");
                }
            });
        }

        this.novoColaborador = function () {
            controller.carregarGrupos();
            controller.buscarColaboradoresPorPerfil(0);
            controller.cadastroParam.colaboradorObj = {};
            controller.cadastroParam.colaboradorObj.grupo = '';
            controller.cadastroParam.colaboradorObj.sexo = '';
            controller.cadastroParam.colaboradorObj.cargo = '';
            controller.cadastroParam.colaboradorObj.uf = '';
            controller.cadastroParam.colaboradorObj.idMunicipio = '';

            controller.desabilitarCamposColaborador = false;
            controller.desabilitarCamposColaborador2 = false;

            controller.cadastroParam.colaboradorObj.dataContratacao = undefined;
            controller.cadastroParam.colaboradorObj.dataNascimento = undefined;
        }

        this.carregarGrupos = function () {
            var user = Auth.getUser();
            var idEmpresa = 0;

            var postData = {};
            postData.contratanteContratada = controller.cadastroParam.contratanteContratada;
            postData.tipoLicenca = '';

            if (controller.cadastroParam.contratanteContratada == 't') {
                idEmpresa = controller.cadastroParam.idContratante;
            }
            else if (controller.cadastroParam.contratanteContratada == 'd') {
                idEmpresa = controller.cadastroParam.idContratada;
            }
            else {
                console.log('por um acaso entrou aqui?')
                return;
            }

            postData.idLicenca = user.obraColaborador.idLicenca;

            $http({
                //url: "api/grupo/Lista/",
                url: "api/grupo/ListaPeloGrupo/",
                method: "POST",
                data: postData
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.grupos = data;
                //controller.grupos.unshift({ id: 0, nome: 'Selecione...' });
            });
        }

        this.buscarColaboradoresPorPerfil = function (idPerfil) {
            let param = idPerfil;

            $http({
                url: "api/colaborador/ObterColaboradoresPorPerfil",
                method: "POST",
                data: param
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.listaColaborador = data;
                controller.buscaTextoColaborador = '';
            });

        };

        controller.verificaPerfilColaborador = function () {
            var idPerfilColaborador = controller.cadastroParam.colaboradorObj.grupo;

            if (idPerfilColaborador != undefined && !isNaN(idPerfilColaborador)) {

                //todo: ver maneira de pegar o valor do colaborador parametrizado, pois está fixado o id
                if (idPerfilColaborador == controller.grupos.find(grupo => grupo.nome == 'Colaborador').id || idPerfilColaborador == controller.grupos.find(grupo => grupo.nome == 'Terceirizado').id) { // Colaborador 
                    controller.perfilColaborador = true;
                    return;
                }
                controller.perfilColaborador = false;
            }
        }

        this.buscarColaborador = function (query) {
            if (query == '' && controller.desabilitarCpf == true) {
                controller.desabilitarCamposColaborador = false;
                controller.desabilitarCpf = false;
                let dadosImportantes = { grupo: controller.cadastroParam.colaboradorObj.grupo, cargo: controller.cadastroParam.colaboradorObj.cargo };
                controller.cadastroParam.colaboradorObj = {};
                controller.cadastroParam.colaboradorObj.grupo = dadosImportantes.grupo;
                controller.cadastroParam.colaboradorObj.sexo = '';
                controller.cadastroParam.colaboradorObj.cargo = dadosImportantes.cargo;
                controller.cadastroParam.colaboradorObj.uf = '';
                controller.cadastroParam.colaboradorObj.idMunicipio = '';
            }
            return query ? controller.listaColaborador.filter(function (value) { return value.nome.toLowerCase().includes(query.toLowerCase()) }) : controller.listaColaborador;
        }

        this.completarColaborador = function () {
            if (typeof (controller.cadastroParam.colaboradorObj.cpf) == 'undefined' || controller.cadastroParam.colaboradorObj.cpf.length == 0) {
                //controller.novoColaborador();
                return;
            }

            $http({
                url: "api/Colaborador/ObterColaboradorCPF",
                method: "POST",
                data: { cpf: controller.cadastroParam.colaboradorObj.cpf }
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                var colaborador = data;
                if (data.cpf == controller.cadastroParam.colaboradorObj.cpf) {
                    controller.preencherModalColaborador(colaborador);
                    controller.desabilitarCamposColaborador2 = false;
                }
                else {
                    controller.desabilitarCamposColaborador = false;
                    controller.desabilitarCamposColaborador2 = false;
                }
            });
        }

        controller.completarColaboradorNome = function () {
            var colaborador = controller.cadastroParam.ColaboradorEncontrado;
            if (colaborador != null && colaborador != undefined) {
                $http
                    .post("api/Colaborador/ObterColaborador/", { id: colaborador.idColaborador })
                    .then(function (data) {
                        var colaboradorCompleto = data.data;
                        controller.preencherModalColaborador(colaboradorCompleto);
                        controller.desabilitarCamposColaborador2 = false;
                    });
            }
        }

        this.preencherModalColaborador = function (colaborador) {
            controller.carregarGrupos();
            let dadosImportantes = { grupo: controller.cadastroParam.colaboradorObj.grupo, cargo: controller.cadastroParam.colaboradorObj.cargo };
            colaborador.confirmacaoSenha = colaborador.senha;
            controller.cadastroParam.colaboradorObj = colaborador;
            controller.cadastroParam.colaboradorObj.grupo = dadosImportantes.grupo;
            controller.cadastroParam.colaboradorObj.cargo = dadosImportantes.cargo;
            controller.buscaTextoColaborador = colaborador.nome;
            controller.desabilitarCamposColaborador = true;
            controller.desabilitarCamposColaborador2 = true;
            controller.carregaListaMunicipioColaborador('cadastro', colaborador.idMunicipio);
        }

        this.completarColaboradorPeloId = function (item) {
            if (item != undefined && item.idColaborador != undefined) {
                $http({
                    url: "api/Colaborador/ObterColaboradorDoSistema",
                    method: "POST",
                    data: item.idColaborador
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    var colaborador = data;
                    if (data.nome == item.nome) {
                        controller.preencherModalColaborador(colaborador);
                        controller.desabilitarCamposColaborador2 = false;
                        controller.desabilitarCpf = true;
                    }
                    else {
                        controller.desabilitarCamposColaborador = false;
                        controller.desabilitarCamposColaborador2 = false;
                    }
                });
            }
        }

        this.carregaListaMunicipioColaborador = function (tipoTela, selected) {
            var postdata = controller.cadastroParam.colaboradorObj.uf;

            $http({
                url: "api/municipio/ListaMunicipio/",
                method: "POST",
                data: postdata
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.municipiosColaborador = data;
                if (tipoTela == 'listagem') {
                    controller.filtroParam.colaboradorObj.idMunicipio = 0;
                }
                else if (selected != undefined) {
                    controller.cadastroParam.colaboradorObj.idMunicipio = selected;
                }
                else {
                    controller.cadastroParam.colaboradorObj.idMunicipio = 0;
                }
            });
        }

        this.limparModalColaborador = function () {
            controller.cadastroParam.colaboradorObj = {};
            controller.cadastroParam.colaboradorObj.grupo = '';
            controller.cadastroParam.colaboradorObj.sexo = '';
            controller.cadastroParam.colaboradorObj.cargo = '';
            controller.cadastroParam.colaboradorObj.uf = '';
            controller.cadastroParam.colaboradorObj.idMunicipio = '';
        }

        this.adicionarColaborador = function () {
            this.cadastroParam.colaboradorObj = this.cadastroParam.colaboradorObj == undefined ? {} : this.cadastroParam.colaboradorObj;

            //controller.cadastroParam.ColaboradorEncontrado = {};
            //controller.cadastroParam.ColaboradorEncontrado.nome = controller.buscaTextoColaborador;
            //controller.cadastroParam.colaboradorObj.nome = controller.buscaTextoColaborador;

            if (Validacao.required(controller.cadastroParam.colaboradorObj.grupo)) {
                toastr.error("O Perfil do Colaborador deve ser preenchido.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.colaboradorObj.cpf) && !controller.perfilColaborador) {
                toastr.error("O CPF deve ser preenchido.");
                return;
            }

            //if (controller.cadastroParam.ColaboradorEncontrado != null) {
            //    if (Validacao.required(controller.cadastroParam.ColaboradorEncontrado.nome)) {
            //        if (Validacao.required(controller.cadastroParam.ColaboradorNaoEncontrado)) {
            //            toastr.error("O Nome deve ser preenchido.");
            //            return;
            //        }
            //        else {
            //            controller.cadastroParam.colaboradorObj.nome = controller.cadastroParam.ColaboradorNaoEncontrado;
            //        }
            //    }
            //    else {
            //        controller.cadastroParam.colaboradorObj.nome = controller.cadastroParam.ColaboradorEncontrado.nome;
            //    }
            //}
            //else {
            //    if (Validacao.required(controller.cadastroParam.ColaboradorNaoEncontrado)) {
            //        toastr.error("O Nome deve ser preenchido.");
            //        return;
            //    }
            //    else {
            //        controller.cadastroParam.colaboradorObj.nome = controller.cadastroParam.ColaboradorNaoEncontrado;
            //    }
            //}

            if (Validacao.required(controller.cadastroParam.colaboradorObj.nome)) {
                toastr.error("O Nome deve ser preenchido.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.colaboradorObj.cargo)) {
                toastr.error("O Cargo deve ser preenchido.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.colaboradorObj.sexo) && !controller.perfilColaborador) {
                toastr.error("O Sexo deve ser preenchido.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.colaboradorObj.dataNascimento) && !controller.perfilColaborador) {
                toastr.error("A Data de Nascimento deve ser preenchida.");
                return;
            }

            if (!Validacao.required(controller.cadastroParam.colaboradorObj.dataNascimento) && !Validacao.data(controller.cadastroParam.colaboradorObj.dataNascimento)) {
                toastr.error("A Data de Nascimento é inválida.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.colaboradorObj.telefonePrincipal) && !controller.perfilColaborador) {
                toastr.error("O Telefone Principal deve ser preenchido.");
                return;
            }

            if (!Validacao.required(controller.cadastroParam.colaboradorObj.telefonePrincipal) && controller.cadastroParam.colaboradorObj.telefonePrincipal.replace(/_/g, '').replace('(', '').replace(')', '').replace(' ', '').replace('-', '').length < 10) {
                toastr.error("O Telefone Principal é inválido.");
                return;
            }

            if (!Validacao.required(controller.cadastroParam.colaboradorObj.telefoneSecundario) && controller.cadastroParam.colaboradorObj.telefoneSecundario.replace(/_/g, '').replace('(', '').replace(')', '').replace(' ', '').replace('-', '').length < 10) {
                toastr.error("O Telefone Secundário é inválido.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.colaboradorObj.email) && !controller.perfilColaborador) {
                toastr.error("O E-mail deve ser preenchido.");
                return;
            }

            if (!Validacao.required(controller.cadastroParam.colaboradorObj.email) && !Email.checkEmail(controller.cadastroParam.colaboradorObj.email)) {
                toastr.error("O E-mail é inválido.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.colaboradorObj.logradouro) && !controller.perfilColaborador) {
                toastr.error("O Logradouro deve ser preenchido.");
                return;
            }


            if (Validacao.required(controller.cadastroParam.colaboradorObj.uf) && !controller.perfilColaborador) {
                toastr.error("A UF deve ser preenchida");
                return;
            }

            if (Validacao.required(controller.cadastroParam.colaboradorObj.idMunicipio) && (!controller.perfilColaborador)) {
                toastr.error("O Municipio deve ser preenchido.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.colaboradorObj.senha) && !controller.perfilColaborador) {
                toastr.error("A Senha deve ser preenchida.");
                return;
            }

            if (!Validacao.minLenght(controller.cadastroParam.colaboradorObj.senha, 4) && !controller.perfilColaborador) {
                toastr.error("Por favor, introduza pelo menos quatro caracteres na Senha");
                return;
            }

            if (Validacao.required(controller.cadastroParam.colaboradorObj.confirmacaoSenha) && !controller.perfilColaborador) {
                toastr.error("A Confirmação Senha deve ser preenchida.");
                return;
            }

            if (!Validacao.minLenght(controller.cadastroParam.colaboradorObj.confirmacaoSenha, 4) && !controller.perfilColaborador) {
                toastr.error("Por favor, introduza pelo menos quatro caracteres na Confirmação de Senha");
                return;
            }

            if (controller.cadastroParam.colaboradorObj.senha != controller.cadastroParam.colaboradorObj.confirmacaoSenha) {
                toastr.error("As senhas digitadas devem ser idênticas.");
                return;
            }

            var descricaoCargo = '';
            var descricaoGrupo = '';
            for (var i = 0; i < controller.cargos.length; i++) {
                if (controller.cargos[i].id == controller.cadastroParam.colaboradorObj.cargo) {
                    controller.cadastroParam.colaboradorObj.descricaoCargo = controller.cargos[i].nome;
                }
            }

            for (var i = 0; i < controller.grupos.length; i++) {
                if (controller.grupos[i].id == controller.cadastroParam.colaboradorObj.grupo) {
                    controller.cadastroParam.colaboradorObj.descricaoGrupo = controller.grupos[i].nome;
                }
            }

            if (window.validate("#form-cadastro-colaborador")) {
                this.cadastroParam.colaboradorObj.excluivel = true;
                this.cadastroParam.listaColaboradores.push(this.cadastroParam.colaboradorObj);
                var user = Auth.getUser();
                this.cadastroParam.colaboradorObj.idObra = user.obra.idObra;
                $http({
                    url: "api/colaborador/Salvar",
                    method: "POST",
                    data: this.cadastroParam.colaboradorObj
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, "Erro.");
                }).success(function (data, status, headers, config) {
                    $('#novo-colaborador').modal('toggle');
                    controller.carregarLista(1);
                    toastr.success("Registro salvo com sucesso.");
                });
            }
            else {
                console.log('colaborador não validado.');
            }
        }

        this.carregarObra = function () {
            var user = Auth.getUser();
            var idObraColaborador = 0

            if (user.obraColaborador != null) {
                idObraColaborador = user.obraColaborador.idObraColaborador;
            }
            var postdata = {};
            postdata.idObra = user.obra.idObra;
            postdata.idObraColaborador = idObraColaborador;

            $http({
                url: "api/obra/ObterObra/",
                method: "POST",
                data: postdata
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.cadastroParam = data;
                controller.imageFileObra = controller.cadastroParam.descricaoFoto;
                controller.carregaListaMunicipio('cadastro', data.idMunicipio);
            });
        }

        this.carregaListaMunicipio = function (tipoTela, selected) {
            var postdata;

            if (tipoTela == 'listagem') {
                postdata = controller.filtroParam.idUf;
            }
            else {
                postdata = controller.cadastroParam.idUf;
            }

            if (postdata == null || postdata == undefined || postdata <= 0) {
                controller.municipios = [];
                postdata = controller.cadastroParam.idUf = 0;
                postdata = controller.filtroParam.idUf = 0;
                postdata = controller.filtroParam.idMunicipio = 0;
                return;
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
            });
        }

        this.carregaListaUF = function () {
            controller.cadastroParam.colaboradorObj.uf = 0;

            $http({
                url: "api/municipio/ListaUF",
                method: "POST"
            }).error(function (data) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data) {
                controller.ufs = data;
                controller.ufsColaborador = data;
            });
        }
    }
})();

