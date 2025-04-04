(function () {
    'use strict';
    angular.module('app').controller('ObraController', ObraController);
    ObraController.$inject = ['$scope', '$http', '$location', 'ViewBag', 'Auth', '$rootScope', '$window', 'Validacao', 'Email'];
    function ObraController($scope, $http, $location, ViewBag, Auth, $rootScope, $window, Validacao, Email) {

        // tela de CADASTRO de obra

        this.pagedlist = {};
        var controller = this;
        this.statusObra = [{ id: 0, nome: 'Selecione...' }, { id: 1, nome: 'Em execução' }, { id: 2, nome: 'Finalizadas' },];
        this.filtroParam = { descricao: '', idColaborador: '', idContratante: 0, idContratada: 0, dataInicio: '', dataPrevisaoFim: '', dataFim: '', statusObra: 0, idMunicipio: 0, idUf: 0, setor: 0 };
        this.cadastroParam = {
            descricao: '', idContratante: 0, idContratada: 0, dataInicio: '', dataPrevisaoFim: '', dataFim: '', logradouro: '', idColaborador: '', idObraColaborador: '',
            numeroEndereco: '', complemento: '', cep: '', bairro: '', idUf: 0, idMunicipio: 0, qtdHrsSemana: '', qtdHrsSabado: '', qtdHrsDomingo: '', art: '', IdObraEquipamento: '',
            areaTotalObra: '', areaTotalConstruida: '', foto: '', listaColaboradores: [], listaEquipamentos: [], usuario: {}, colaboradorObj: { nome: '' }, equipamentoObj: {}, descricaoFoto: '',
            listaColaboradoresRemovidos: [], listaEquipamentosRemovidos: [], contratanteContratada: 't', emailConvidada: '', tipoLicenca: '', novaObra: '', idGrupoConvite: ''
        }

        //this.sexo = [{ id: 'S', nome: 'Selecione...' }, { id: 'M', nome: 'Masculino' }, { id: 'F', nome: 'Feminino' }, ];
        this.sexo = [{ id: 'M', nome: 'Masculino' }, { id: 'F', nome: 'Feminino' },];
        this.tipoAquisicao = [{ id: 'A', nome: 'Aluguel' }, { id: 'C', nome: 'Compra' },];
        this.obras = [];
        this.empresas = [];
        this.municipios = [];
        this.ufs = [];
        this.setores = [];
        this.grupos = [];
        this.cargos = [];
        this.colaboradores = {};
        this.equipamentos = {};
        this.empresaLicenca = {};
        this.desabilitarCamposColaborador;
        this.desabilitarCamposColaborador2;
        this.desabilitarCamposEquipamento;
        this.desabilitarCpf = false;
        this.nomeColaboradorModoEdicao = false;
        this.imageFileObra = '';
        this.telaCadastro = false;
        this.empresa = {};

        this.desabilitarRadioContratanteContratada;
        this.desabilitarEscolhaContratante;
        this.desabilitarEscolhaContratada;
        this.desabilitarAdicaoColaboradores = false;// true;
        this.imgFileTextObra = '';
        this.desabilitarSalvarObra = false;
        this.desabilitarCamposObra = false;



        this.municipiosColaborador = [];
        this.ufsColaborador = [];

        controller.perfilColaborador = false;

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

        this.verificarObraFinalizada = function () {
            if (Auth.getUser().obra != null) {
                controller.desabilitarCamposObra = Auth.getUser().obra.obraFinalizada;
            }

            if (ViewBag.get('desabilitarCamposObra') == true) {
                controller.desabilitarCamposObra = true;
            }
        }


        this.carregarTelaCadastro = function () {
            var copiarObra = ViewBag.get('copiarObra');
            controller.imageFileObra = '';
            if (copiarObra == true) {
                controller.carregarCopia();
            }
            else {
                controller.carregarEdicao();
            }

            controller.verificarObraFinalizada();
            controller.carregaListaEmpresa();
            controller.carregaListaUF();
            controller.carregarCargos();
            controller.habilitarContratanteContratada();
            controller.verificarAdicaoColaborador();

            controller.carregarMarcas();
            controller.carregarModelos();
            controller.carregaTipoEquipamento();
        }

        this.carregarTelaIndex = function () {
            controller.carregarListaIndex(1);
            controller.carregaListaEmpresa();
            controller.carregaListaUF();
            controller.carregarSetores();
        }

        this.maskPhone = function (phone) {

            if (phone == undefined || phone == '' || phone == null) {
                return;
            }

            try {
                if (phone.length == 13 || phone.length == 15) {
                    var x = phone.replace(/\D/g, '').match(/(\d{0,2})(\d{0,5})(\d{0,4})/);
                    var p = '(' + x[1] + ') ' + x[2] + '-' + x[3];
                    return p;
                }
                else if (phone.length == 12 || phone.length == 14) {
                    var x = phone.replace(/\D/g, '').match(/(\d{0,2})(\d{0,4})(\d{0,4})/);
                    var p = '(' + x[1] + ') ' + x[2] + '-' + x[3];
                    return p;
                }
            }
            catch (ex) {
                return phone;
            }
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

        //controller.buscarColaboradorNome = function (searchText) {
        //    controller.cadastroParam.ColaboradorNaoEncontrado = searchText;
        //    if (searchText != '') {
        //        return $http
        //       .get("api/Colaborador/ObterColaboradorNome/", { params: { nome: searchText } })
        //       .then(function (data) {
        //           var listaColaborador = data.data;
        //           return listaColaborador;
        //       });
        //    }

        //};

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

        this.buscarEmpresa = function (idEmpresa) {
            let param = idEmpresa;

            $http({
                url: "api/empresa/ObterEmpresa",
                method: "POST",
                data: param
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.empresa = data;
            });

        };

        this.loginUsuarioObra = function (callback) {
            var postdata = {};
            postdata.idUsuario = Auth.getUser().usuario.id;
            postdata.idObra = ViewBag.get('obraId');

            $http({
                url: "api/Usuario/LoginObra",
                method: "POST",
                data: postdata
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                Auth.updateUser(data);
                console.log("Update uSuario Obra", Auth.getUser());
                if (callback) {
                    callback();
                }
            });
        }

        this.verificarLicencaEmpresa = function () {
            var postdata = {};
            postdata.token = 'asdflkjsdhfiehur2134h9fhhlajkdfha';

            $http({
                // mudar para url de raul
                url: "api/Usuario/LoginObra",
                method: "POST",
                data: postdata
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                // mudar os dados do parametro para o tipo nome dos parametros de raul
                var statusLicenca = data.status;
                var tipoLicenca = data.tipoLicenca;
            });
        }

        this.carregarMarcas = function () {
            $http({
                url: "api/equipamentos/ListaMarca",
                method: "POST"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, "Erro")
            }).success(function (data, status, headers, config) {
                controller.marcas = data.split(/,+/g).map(function (marca) {
                    return {
                        id: marca.toLowerCase(),
                        nome: marca
                    };
                });
            });
        }


        this.carregarModelos = function () {
            $http({
                url: "api/equipamentos/ListaModelo",
                method: "POST"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, "Erro")
            }).success(function (data, status, headers, config) {
                controller.modelos = data.split(/,+/g).map(function (modelo) {
                    return {
                        id: modelo.toLowerCase(),
                        nome: modelo
                    };
                });
            });
        }

        this.carregaTipoEquipamento = function () {
            $http({
                url: "api/equipamentos/GetTipoEquipamento",
                method: "GET"
            }).error(function (data) {
                toastr.error(data.exceptionMessage, "Erro")
            }).success(function (data) {
                controller.tiposEquipamentos = data;
            });
        }

        this.habilitarContratanteContratada = function () {

            var obra = Auth.getUser().obra;
            var temContratanteContratadaEscolhido = (obra != null && obra.idContratada == 0 && obra.idContratante == 0);

            var temContratanteContratadaEscolhido = (obra != null && (obra.idObra != null && obra.idObra > 0));

            if (temContratanteContratadaEscolhido) {
                controller.desabilitarRadioContratanteContratada = true;

            }
            else {
                controller.desabilitarRadioContratanteContratada = false;
            }

            if (controller.cadastroParam.contratanteContratada == 't') { //&& obra.idContratante == 0
                controller.desabilitarEscolhaContratante = false;
                controller.desabilitarEscolhaContratada = true;
                controller.cadastroParam.idContratada = 0;


            }
            else if (controller.cadastroParam.contratanteContratada == 'd') { // && obra.idContratada == 0
                controller.desabilitarEscolhaContratante = true;
                controller.desabilitarEscolhaContratada = false;
                controller.cadastroParam.idContratante = 0;


            }
            else {
                controller.desabilitarEscolhaContratante = true;
                controller.desabilitarEscolhaContratada = true;
                //if (temContratanteContratadaEscolhido) {
                //    controller.desabilitarRadioContratanteContratada = true;
                //}
            }

        }


        this.verificarLicencaEmpresa = function (idEmpresa) {
            $http({
                url: "api/Empresa/VerificarPermissaoLicencaEmpresa",
                method: "POST",
                data: idEmpresa
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                if (data) {
                    controller.desabilitarSalvarObra = false;
                }
                else {
                    controller.desabilitarSalvarObra = true;
                    toastr.error("A licença desta empresa não permite que novas obras sejam adicionadas");
                }
            });
        }



        this.verificarAdicaoColaborador = function () {

            if (controller.cadastroParam != null) {
                if (controller.cadastroParam.idContratada != 0 || controller.cadastroParam.idContratante != 0) {
                    controller.desabilitarAdicaoColaboradores = false;
                }
                else {
                    controller.desabilitarAdicaoColaboradores = true;
                }

                if (controller.cadastroParam.idContratada != 0 || controller.cadastroParam.idContratante != 0) {
                    var idEmpresa = 0;
                    if (controller.cadastroParam.idContratada != 0) {
                        idEmpresa = controller.cadastroParam.idContratada;
                    }
                    else if (controller.cadastroParam.idContratante != 0) {
                        idEmpresa = controller.cadastroParam.idContratante;
                    }
                    else {
                        return;
                    }
                    controller.verificarLicencaEmpresa(idEmpresa);
                    controller.buscarEmpresa(idEmpresa);
                }
            }
        }


        // Tela de SELEÇÃO de obra
        this.carregarLista = function () {
            $rootScope.tema = "tema-azul-escuro";
            controller.filtroParam.idColaborador = Auth.getUser().usuario.id;

            $http({
                url: "api/obra/ObterObras",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                //toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.obras = data;
            });
        }


        this.carregaListaEmpresa = function () {
            controller.filtroParam.idColaborador = Auth.getUser().usuario.id;
            controller.filtroParam.novaObra = ViewBag.get('novaObra');

            if (Auth.getUser().obra != null) {
                controller.filtroParam.idObra = Auth.getUser().obra.idObra;
            }


            $http({
                url: "api/empresa/ListaEmpresa",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.empresas = data;
                controller.empresas.unshift({ idEmpresa: 0, nomeFantasia: 'Selecione...' });
            });
        }


        this.carregarSetores = function () {
            $http({
                url: "api/setor/Lista/",
                method: "GET",
                data: undefined
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.setores = data;
                controller.setores.unshift({ id: 0, nome: 'Selecione...' });
            });
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
                return;
            }
            if (user.obraColaborador && user.obraColaborador.idLicenca) {
                postData.idLicenca = user.obraColaborador.idLicenca;
            } else if (controller.empresa.idLicenca) {
                postData.idLicenca = controller.empresa.idLicenca;
            } else {
                return;
            }

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


        this.carregarCargos = function () {
            $http({
                url: "api/cargo/Lista/",
                method: "POST",
                data: undefined
            }).error(function (data) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data) {
                controller.cargos = data;
                //controller.cargos.unshift({ id: 0, nome: 'Selecione...' });
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

        this.carregarListaIndex = function (page, order) {
            controller.filtroParam.page = page;
            controller.filtroParam.idColaborador = Auth.getUser().usuario.id;


            controller.filtroParam.dataInicioDe = $('.dataInicioBuscaDe').val();
            controller.filtroParam.dataInicioAte = $('.dataInicioBuscaAte').val();
            controller.filtroParam.dataPrevisaoFimDe = $('.dataPrevisaoFimDe').val();
            controller.filtroParam.dataPrevisaoFimAte = $('.dataPrevisaoFimAte').val();

            if (controller.filtroParam.orderby == '') {
                controller.filtroParam.orderbydescending = '';
                controller.filtroParam.orderby = order;
            }
            else {
                controller.filtroParam.orderby = '';
                controller.filtroParam.orderbydescending = order;
            }

            controller.filtroParam.order = order;

            if (controller.filtroParam.idMunicipio == null || controller.filtroParam.idMunicipio == undefined) {
                controller.filtroParam.idMunicipio = 0;
            }

            $http({
                url: "api/obra/CarregarLista",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.pagedlist = data;
            });
        }



        this.carregarEdicao = function () {
            $rootScope.tema = "tema-azul-claro";
            var id = ViewBag.get('obraId');
            var user = Auth.getUser();
            var idObraColaborador = 0
            if (user.obraColaborador != null) {
                idObraColaborador = user.obraColaborador.idObraColaborador;
            }

            var postdata = {};
            postdata.idObra = id;
            postdata.idObraColaborador = idObraColaborador;

            if (id != "undefined" && id > 0) {
                $http({
                    url: "api/obra/ObterObra/",
                    method: "POST",
                    data: postdata
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    controller.telaCadastro = false;
                    controller.cadastroParam = data;
                    controller.imageFileObra = controller.cadastroParam.descricaoFoto;
                    controller.carregaListaMunicipio('cadastro', data.idMunicipio);
                    controller.desabilitarAdicaoColaboradores = false;
                    //controller.loginUsuarioObra();
                });
            }
            else {
                controller.telaCadastro = true;
            }
        }


        this.carregarCopia = function () {
            $rootScope.tema = "tema-azul-claro";

            var id = ViewBag.get('obraId');
            ViewBag.set('obraId', undefined);
            ViewBag.set('copiarObra', false);

            var user = Auth.getUser();
            var idObraColaborador = user.obraColaborador.idObraColaborador;


            var postdata = {};
            postdata.idObra = id;
            postdata.idObraColaborador = idObraColaborador;


            //var logOutUser = ViewBag.get('logOutUser');
            //if (logOutUser) {
            //    Auth.updateUser(Auth.getLoginUser());
            //    ViewBag.set('logOutUser', false);
            //}


            if (id != "undefined" && id > 0) {
                $http({
                    url: "api/obra/ObterObra/",
                    method: "POST",
                    data: postdata
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    controller.cadastroParam = data;
                    controller.cadastroParam.idObra = 0;
                    controller.cadastroParam.dataFim = '';
                    controller.limparDadosCopia();
                    controller.carregaListaMunicipio('cadastro', data.idMunicipio);
                    controller.desabilitarAdicaoColaboradores = false;
                    controller.habilitarContratanteContratada();

                });
            }
            else {
                //console.log("id nao informado");
            }
        }


        this.limparDadosCopia = function () {
            var colaboradores = controller.cadastroParam.listaColaboradores;
            var equipamentos = controller.cadastroParam.listaEquipamentos;
            controller.cadastroParam.idObra = 0;

            for (var i = 0; i < colaboradores.length; i++) {
                colaboradores[i].idObraColaborador = 0;
            }

            for (var i = 0; i < equipamentos.length; i++) {
                equipamentos[i].IdObraEquipamento = 0;
            }

            controller.cadastroParam.listaColaboradores = colaboradores;
            controller.cadastroParam.listaEquipamentos = equipamentos;

        }

        this.salvar = function () {

            if (Validacao.required(controller.cadastroParam.descricao)) {
                toastr.error("O Nome da Obra deve ser preenchido");
                return;
            }
            if (Validacao.required(controller.cadastroParam.contratanteContratada)) {
                toastr.error("Informe se a empresa é contratante ou contratada");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.descricao)) {
                toastr.error("Por favor, introduza pelo menos três caracteres no Nome da Obra");
                return;
            }
            if (Validacao.required(controller.cadastroParam.dataInicio)) {
                toastr.error("A Data Inicial deve ser preenchida.");
                return;
            }
            if (!Validacao.data(controller.cadastroParam.dataInicio)) {
                toastr.error("A Data Inicial é inválida.");
                return;
            }
            if (Validacao.required(controller.cadastroParam.dataPrevisaoFim) || !Validacao.data(controller.cadastroParam.dataPrevisaoFim)) {
                toastr.error("A Data Previsão Final deve ser preenchida");
                return;
            }
            if (Validacao.required(controller.cadastroParam.logradouro)) {
                toastr.error("O Logradouro deve ser preenchido");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.logradouro)) {
                toastr.error("Por favor, introduza pelo menos três caracteres no Logradouro");
                return;
            }
            if (Validacao.required(controller.cadastroParam.numeroEndereco)) {
                toastr.error("O Número deve ser preenchido");
                return;
            }
            //if (parseInt(controller.cadastroParam.numeroEndereco) < 0) {
            //    toastr.error("Por favor, introduza um valor maior ou igual a zero em Número");
            //    return;
            //}
            if (Validacao.required(controller.cadastroParam.cep)) {
                toastr.error("O CEP deve ser preenchido");
                return;
            }
            if (Validacao.required(controller.cadastroParam.idUf)) {
                toastr.error("A UF ser preenchida");
                return;
            }
            if (Validacao.required(controller.cadastroParam.idMunicipio)) {
                toastr.error("O Munícipio deve ser preenchido");
                return;
            }
            if (parseInt(controller.cadastroParam.qtdHrsSemana) < 0) {
                toastr.error("Por favor, introduza um valor maior ou igual a zero em Qtd. hrs/tarefa/semana");
                return;
            }
            if (parseInt(controller.cadastroParam.qtdHrsSabado) < 0) {
                toastr.error("Por favor, introduza um valor maior ou igual a zero em Qtd. hrs/tarefa/sábados");
                return;
            }
            if (parseInt(controller.cadastroParam.qtdHrsDomingo) < 0) {
                toastr.error("Por favor, introduza um valor maior ou igual a zero em Qtd. hrs/tarefa/domingos");
                return;
            }
            if (parseInt(controller.cadastroParam.areaTotalObra) < 0) {
                toastr.error("Por favor, introduza um valor maior ou igual a zero em Área total da obra (m²)");
                return;
            }
            if (parseInt(controller.cadastroParam.areaTotalConstruida) < 0) {
                toastr.error("Por favor, introduza um valor maior ou igual a zero em Área total construída (m²)");
                return;
            }
            if (window.validate("#form-cadastro-obra")) {
                var user = Auth.getUser();
                var idObra = ViewBag.get('obraId');

                controller.cadastroParam.idObra = idObra;
                controller.cadastroParam.usuario = user.usuario;

                controller.cadastroParam.dataInicio = $('.dataInicio').val();
                controller.cadastroParam.dataPrevisaoFim = $('.dataPrevisaoFim').val();
                console.log(controller.imageFileObra);
                if (controller.imageFileObra != undefined) {
                    var size = controller.imageFileObra.filesize;

                    if (size > 2097152) {
                        toastr.error("O tamanho do arquivo selecionado não pode ser superior a 2MB");
                        return;
                    }
                    else {
                        controller.cadastroParam.foto = controller.imageFileObra;
                    }
                }
                else {
                    controller.cadastroParam.foto = '';
                }

                var idEmpresa = 0
                if (controller.cadastroParam.contratanteContratada == 't') {
                    idEmpresa = controller.cadastroParam.idContratante;
                }
                else if (controller.cadastroParam.contratanteContratada == 'd') {
                    idEmpresa = controller.cadastroParam.idContratada;
                }
                else {
                    return;
                }
                for (var i = 0; i < controller.empresas.length; i++) {
                    if (controller.empresas[i].idEmpresa == idEmpresa) {
                        controller.empresaLicenca = controller.empresas[i];
                    }
                }
                var licencaEmpresa = controller.empresaLicenca.descricaoLicenca;
                if (licencaEmpresa != 'basica') {
                    licencaEmpresa = 'gratuita';
                }

                $http({
                    url: "api/obra/Salvar",
                    method: "POST",
                    data: this.cadastroParam
                }).error(function (data, status, headers, config) {
                    controller.carregarEdicao();
                    toastr.error(data.Message)
                }).success(function (data, status, headers, config) {
                    controller.escolherObra(data);
                    toastr.success("Registro salvo com sucesso.");
                });
            }
            else {
                console.log('not validated!!!');
            }
        }

        this.finalizarObra = function () {
            var today = new Date();
            var dd = today.getDate();
            var mm = today.getMonth() + 1;
            var yyyy = today.getFullYear();

            if (dd < 10) {
                dd = '0' + dd
            }

            if (mm < 10) {
                mm = '0' + mm
            }

            today = dd + '/' + mm + '/' + yyyy;
            controller.cadastroParam.dataFim = today;
            controller.salvar();
            //controller.escolherObra(Auth.getUser().obra);
        }


        this.deletar = function (obra) {
            var currentObra = Auth.getUser().obra.idObra;

            MensagemConfirmacao("Tem certeza que deseja excluir o registro: " + obra.descricao + "?", function () {
                $http({
                    url: "api/obra/Deletar",
                    method: "POST",
                    data: obra
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage)
                }).success(function (data, status, headers, config) {
                    controller.carregarListaIndex(1);
                    if (data) {
                        toastr.success("Registro excluído com sucesso.");
                        var user = Auth.getUser();

                        if (obra.idObra == currentObra) {
                            Auth.updateUser(Auth.getLoginUser());
                            $location.path('/obra/escolher');
                            return;
                        }
                    }
                    else {
                        toastr.error("Não é possível excluir esta obra. Existem registros dependentes.");
                    }
                });

            });
        }

        this.editar = function (obra) {
            ViewBag.set('obraId', obra.idObra);

            controller.loginUsuarioObra(function () {
                ViewBag.set('copiarObra', false);
                ViewBag.set('novaObra', false);
                ViewBag.set('desabilitarCamposObra', false);
                ViewBag.set('last-page', $location.path());
                $location.path('/obra/cadastro');
            });
        }

        this.visualizar = function (obra) {
            ViewBag.set('obraId', obra.idObra);

            controller.loginUsuarioObra(function () {
                ViewBag.set('copiarObra', false);
                ViewBag.set('novaObra', false);
                ViewBag.set('desabilitarCamposObra', true);
                ViewBag.set('last-page', $location.path());
                $location.path('/obra/cadastro');
            });
        }


        this.copiar = function (obra) {
            ViewBag.set('obraId', obra.idObra);
            ViewBag.set('desabilitarCampos', true);
            ViewBag.set('copiarObra', true);

            ViewBag.set('last-page', $location.path());

            $location.path('/obra/cadastro');
        }


        this.convidar = function (obra) {
            controller.cadastroParam = obra;
        }


        this.enviarConvite = function () {
            controller.cadastroParam.idGrupoConvite = Auth.getUser().usuario.idGrupo;

            if (Validacao.required(controller.cadastroParam.emailConvidada)) {
                toastr.error("O Email do Convidado deve ser preenchido.");
                return;
            }

            if (!Validacao.email(controller.cadastroParam.emailConvidada)) {
                toastr.error("O Email é inválido.");
                return;
            }

            $http({
                url: "api/obra/Convidar",
                method: "POST",
                data: this.cadastroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage)
            }).success(function (data, status, headers, config) {
                $location.path('/obra/index');
                toastr.success("Convite enviado com sucesso.");
            });

        }



        //this.escolherObra = function (obra) {
        //    var objLogin = { idUsuario: Auth.getUser().usuario.id, idObra: obra.idObra, obra: obra, user: Auth.getUser() }

        //    $http({
        //        //esse método deve vir de obra e não de usuário
        //        url: "api/login/LoginObra",
        //        method: "POST",
        //        data: objLogin
        //    }).error(function (data, status, headers, config) {
        //        toastr.error(data.Message);
        //    }).success(function (data, status, headers, config) {
        //        var found = false;
        //        for (var i in data.routes) {
        //            if (data.routes[i].path == '/tarefa/cards') {
        //                found = true;
        //            }
        //        }
        //        if (found) {
        //            Auth.updateUser(data);
        //            $location.path('/tarefa/cards');
        //        }
        //        else {
        //            toastr.error('Seu usuário não tem permissão. Favor contate o administrador.');
        //        }
        //    });
        //}


        this.escolherObra = function (obra) {
            var objLogin = { idUsuario: Auth.getUser().usuario.id, idObra: obra.idObra, obra: obra, user: Auth.getUser() }

            $http({
                //esse método deve vir de obra e não de usuário
                url: "api/login/LoginObra",
                method: "POST",
                data: objLogin
            }).error(function (data) {
                toastr.error(data.Message);
            }).success(function (data) {
                var found = false;
                for (var i in data.routes) {
                    if (data.routes[i].path == '/tarefa/index') {
                        found = true;
                    }
                }
                if (found) {
                    Auth.updateUser(data);
                    $location.path('/tarefa/cards');
                }
                else {
                    toastr.error('Seu usuário não tem permissão. Favor contate o administrador.');
                }
            });
        }

        this.novo = function () {
            var authObj = Auth.getUser();
            controller.cadastroParam.listaColaboradores = [];
            controller.cadastroParam.listaEquipamentos = [];
            controller.cadastroParam.listaColaboradoresRemovidos = [];
            controller.cadastroParam.listaEquipamentosRemovidos = [];

            ViewBag.set('last-page', '/obra/index');

            ViewBag.set('novaObra', true);
            ViewBag.set('obraId', undefined);
            $location.path('/obra/cadastro');
        }

        this.removerObraColaborador = function (colaborador, index) {
            this.cadastroParam.listaColaboradoresRemovidos.push(colaborador);
            this.cadastroParam.listaColaboradores.splice(index, 1);
        }

        //$scope.$watch(controller.cadastroParam.ColaboradorEncontrado, function () {
        //    console.log('controller.cadastroParam.ColaboradorEncontrado foi modificado!');
        //});


        this.adicionarColaborador = function () {
            this.cadastroParam.colaboradorObj = this.cadastroParam.colaboradorObj == undefined ? {} : this.cadastroParam.colaboradorObj;

            if (Validacao.required(controller.cadastroParam.colaboradorObj.grupo)) {
                toastr.error("O Perfil do Colaborador deve ser preenchido.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.colaboradorObj.cpf) && !controller.perfilColaborador) {
                toastr.error("O CPF deve ser preenchido.");
                return;
            }

            //if (controller.cadastroParam.listaColaboradores.filter(f => f.cpf == controller.cadastroParam.colaboradorObj.cpf).length > 0) {
            //    toastr.error("Um Colaborador com o CPF '" + controller.cadastroParam.colaboradorObj.cpf + "' já foi adicionado.");
            //    return;
            //}

            //if (!controller.nomeColaboradorModoEdicao) {
            //    controller.cadastroParam.ColaboradorEncontrado = {};
            //    controller.cadastroParam.ColaboradorEncontrado.nome = controller.buscaTextoColaborador;
            //    controller.cadastroParam.colaboradorObj.nome = controller.buscaTextoColaborador;

            //    if (controller.cadastroParam.ColaboradorEncontrado != null) {
            //        if (Validacao.required(controller.cadastroParam.ColaboradorEncontrado.nome)) {
            //            if (Validacao.required(controller.cadastroParam.ColaboradorNaoEncontrado)) {
            //                toastr.error("O Nome deve ser preenchido.");
            //                return;
            //            }
            //            else {
            //                controller.cadastroParam.colaboradorObj.nome = controller.cadastroParam.ColaboradorNaoEncontrado;
            //            }
            //        }
            //        else {
            //            controller.cadastroParam.colaboradorObj.nome = controller.cadastroParam.ColaboradorEncontrado.nome;
            //        }
            //    }
            //    else {
            //        if (Validacao.required(controller.cadastroParam.ColaboradorNaoEncontrado)) {
            //            toastr.error("O Nome deve ser preenchido.");
            //            return;
            //        }
            //        else {
            //            controller.cadastroParam.colaboradorObj.nome = controller.cadastroParam.ColaboradorNaoEncontrado;
            //        }
            //    }
            //} else if (Validacao.required(controller.cadastroParam.colaboradorObj.nome)) {
            //    toastr.error("O Nome deve ser preenchido.");
            //    return;
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

            if (controller.cadastroParam.colaboradorObj.senha != controller.cadastroParam.colaboradorObj.confirmacaoSenha && !controller.perfilColaborador) {
                toastr.error("As senhas digitadas devem ser idênticas.");
                return;
            }

            //if (!controller.perfilColaborador) {
            //    //if (!Validacao.minLenght(controller.cadastroParam.colaboradorObj.nome.nome)) {
            //    //    toastr.error("Por favor, introduza pelo menos três caracteres no Nome do Colaborador");
            //    //    return;
            //    //}



            //    if (!Validacao.cpf(controller.cadastroParam.colaboradorObj.cpf)) {
            //        toastr.error("O CPF do Colaborador é inválido.");
            //        return;
            //    }

            //    if (Validacao.required(controller.cadastroParam.colaboradorObj.dataNascimento)) {
            //        toastr.error("A Data de Nascimento do Colaborador deve ser preenchida.");
            //        return;
            //    }

            //    if (!Validacao.data(controller.cadastroParam.colaboradorObj.dataNascimento)) {
            //        toastr.error("A Data de Nascimento do Colaborador é inválida.");
            //        return;
            //    }




            //    if (!Validacao.email(controller.cadastroParam.colaboradorObj.email)) {
            //        toastr.error("O E-mail do Colaborador é inválido.");
            //        return;
            //    }





            //    if (Validacao.required(controller.cadastroParam.colaboradorObj.cargo)) {
            //        toastr.error("O Cargo do Colaborador deve ser preenchido.");
            //        return;
            //    }

            //    if (Validacao.required(controller.cadastroParam.colaboradorObj.senha)) {
            //        toastr.error("Por favor, Digite a Senha.");
            //        return;
            //    }

            //    if (!Validacao.minLenght(controller.cadastroParam.colaboradorObj.senha, 4)) {
            //        toastr.error("Por favor, introduza pelo menos quatro caracteres no Nome do Colaborador");
            //        return;
            //    }

            //    if (Validacao.required(controller.cadastroParam.colaboradorObj.confirmacaoSenha)) {
            //        toastr.error("Por favor, Confirme a Senha.");
            //        return;
            //    }

            //    if (!Validacao.minLenght(controller.cadastroParam.colaboradorObj.confirmacaoSenha, 4)) {
            //        toastr.error("Por favor, introduza pelo menos quatro caracteres no Nome do Colaborador");
            //        return;
            //    }

            //    if (controller.cadastroParam.colaboradorObj.senha != controller.cadastroParam.colaboradorObj.confirmacaoSenha) {
            //        toastr.error("As senhas digitadas devem ser idênticas.");
            //        return;
            //    }
            //}

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


            //for (var i = 0; i < controller.cadastroParam.listaColaboradores.length; i++) {
            //    if (controller.cadastroParam.listaColaboradores[i].cpf == controller.cadastroParam.colaboradorObj.cpf) {
            //        toastr.error("Esse colaborador já está vinculado a essa obra.");
            //        return;
            //    }
            //}

            //this.cadastroParam.colaboradorObj.descricaoCargo = controller.cadastroParam.colaboradorObj.cargo;
            //this.cadastroParam.colaboradorObj.descricaoGrupo = controller.cadastroParam.colaboradorObj.grupo;

            if (window.validate("#form-cadastro-colaborador")) {
                this.cadastroParam.colaboradorObj.excluivel = true;
                this.cadastroParam.colaboradorObj.editado = true;
                this.cadastroParam.listaColaboradores = this.cadastroParam.listaColaboradores.filter(f => f.idColaborador != this.cadastroParam.colaboradorObj.idColaborador || f.idColaborador == undefined)
                this.cadastroParam.listaColaboradores.push(this.cadastroParam.colaboradorObj);
                $('#novo-colaborador').modal('toggle');
            }
            else {
                console.log('colaborador não validado.');
            }
        }

        this.carregarEdicaoColaborador = function (colaborador) {
            controller.carregarGrupos();

            if (colaborador.editado) {
                controller.cadastroParam.colaboradorObj = colaborador;
                controller.verificaPerfilColaborador();
            } else {
                var user = Auth.getUser();
                var idObra = user.obra.idObra;
                var id = colaborador.idColaborador;
                var postData = { id: id, idObra: idObra };
                if (id != "undefined" && id > 0) {
                    $http({
                        url: "api/colaborador/ObterColaborador/",
                        method: "POST",
                        data: postData
                    }).error(function (data, status, headers, config) {
                        toastr.error(data.exceptionMessage, data.message)
                    }).success(function (data, status, headers, config) {
                        controller.cadastroParam.colaboradorObj = data;
                        controller.cadastroParam.colaboradorObj.confirmacaoSenha = data.senha;
                        controller.carregaListaMunicipioColaborador('cadastro', data.idMunicipio);
                        controller.desabilitarCamposColaborador = false;
                        controller.desabilitarCpf = false;
                        controller.nomeColaboradorModoEdicao = true;
                        controller.verificaPerfilColaborador();
                    });
                }
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
            if (!controller.nomeColaboradorModoEdicao)
                controller.desabilitarCamposColaborador = true;

            controller.desabilitarCamposColaborador2 = true;
            controller.carregaListaMunicipioColaborador('cadastro', colaborador.idMunicipio);
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
            controller.nomeColaboradorModoEdicao = false;

            controller.cadastroParam.colaboradorObj.dataContratacao = undefined;
            controller.cadastroParam.colaboradorObj.dataNascimento = undefined;
        }

        this.limparModalColaborador = function () {
            controller.cadastroParam.colaboradorObj = {};
            controller.cadastroParam.colaboradorObj.grupo = '';
            controller.cadastroParam.colaboradorObj.sexo = '';
            controller.cadastroParam.colaboradorObj.cargo = '';
            controller.cadastroParam.colaboradorObj.uf = '';
            controller.cadastroParam.colaboradorObj.idMunicipio = '';
        }


        //equipamentos
        this.removerObraEquipamento = function (equipamento, index) {

            if (equipamento.id) {
                $http({
                    url: "api/equipamentos/VerificarExclusao",
                    method: "POST",
                    data: equipamento
                }).error(function (data, status, headers, config) {
                    toastr.error(data.Message);
                }).success(function (data, status, headers, config) {
                    if (data == true) {
                        controller.cadastroParam.listaEquipamentosRemovidos.push(equipamento);
                        controller.cadastroParam.listaEquipamentos.splice(index, 1);
                    }
                    else {
                        toastr.error("Este equipamento não pode ser removido por estar relacionado a alguma tarefa");
                    }
                });
            }
            else {
                controller.cadastroParam.listaEquipamentosRemovidos.push(equipamento);
                controller.cadastroParam.listaEquipamentos.splice(index, 1);
            }

        }

        this.adicionarEquipamento = function () {
            this.cadastroParam.equipamentoObj = this.cadastroParam.equipamentoObj == undefined ? {} : this.cadastroParam.equipamentoObj;

            if (controller.cadastroParam.equipamentoObj.idTipoEquipamento == undefined || controller.cadastroParam.equipamentoObj.idTipoEquipamento <= 0) {
                toastr.error("O Tipo de Equipamento deve ser preenchido.");
                return;
            }

            controller.cadastroParam.equipamentoObj.tipoEquipamento = controller.tiposEquipamentos.find(t => t.id == controller.cadastroParam.equipamentoObj.idTipoEquipamento).nome;

            if (Validacao.required(this.cadastroParam.equipamentoObj.descricao)) {
                toastr.error("A Descrição deve ser preenchida.");
                return;
            }
            if (!Validacao.minLenght(this.cadastroParam.equipamentoObj.descricao, 2)) {
                toastr.error("Por favor, introduza pelo menos dois caracteres na Descrição");
                return;
            }
            //if (Validacao.required(this.cadastroParam.equipamentoObj.marca)) {
            //    toastr.error("A Marca precisa ser preenchida");
            //    return;
            //}
            //if (Validacao.required(this.cadastroParam.equipamentoObj.modelo)) {
            //    toastr.error("O Modelo precisa ser preenchido");
            //    return;
            //}
            //if (Validacao.required(this.cadastroParam.equipamentoObj.tipoAquisicao)) {
            //    toastr.error("O Tipo de Aquisição precisa ser preenchido");
            //    return;
            //}
            //if (Validacao.required(controller.cadastroParam.equipamentoObj.dataAquisicao)) {
            //    toastr.error("A Data de Aquisição deve ser preenchida.");
            //    return;
            //}
            //if (!Validacao.data(controller.cadastroParam.equipamentoObj.dataAquisicao)) {
            //    toastr.error("A Data de Aquisição deve ser preenchida.");
            //    return;
            //}
            //if (Validacao.required(this.cadastroParam.equipamentoObj.fabricanteFornecedor)) {
            //    toastr.error("O Fabricante/Fornecedor precisa ser preenchido");
            //    return;
            //}
            //if (Validacao.required(this.cadastroParam.equipamentoObj.contato)) {
            //    toastr.error("O Contato precisa ser preenchido");
            //    return;
            //}
            //if (Validacao.required(this.cadastroParam.equipamentoObj.telefone)) {
            //    toastr.error("O Telefone precisa ser preenchido");
            //    return;
            //}

            if (window.validate("#form-cadastro-equipamento")) {

                if (this.cadastroParam.equipamentoObj.telefone) {
                    if (this.cadastroParam.equipamentoObj.telefone.replace(/_/g, '').length < 14) {
                        toastr.error("O Telefone é inválido.");
                        return;
                    }
                }

                this.cadastroParam.equipamentoObj.foto = '';
                this.cadastroParam.listaEquipamentos.push(this.cadastroParam.equipamentoObj);
                $('#novo-equipamento').modal('toggle');
            }
            else {
                console.log('equipamento não validado.');
            }

        }

        this.preencherModalEquipamento = function (equipamento) {
            controller.cadastroParam.equipamentoObj = equipamento;
            controller.desabilitarCamposEquipamento = true;
        }

        this.novoEquipamento = function () {
            controller.cadastroParam.equipamentoObj = {};
            controller.cadastroParam.equipamentoObj.idTipoEquipamento = '';
            controller.cadastroParam.equipamentoObj.marca = '';
            controller.cadastroParam.equipamentoObj.modelo = '';
            controller.cadastroParam.equipamentoObj.tipoAquisicao = '';
            controller.desabilitarCamposEquipamento = false;
        }

        this.limparModalEquipamento = function () {
            controller.cadastroParam.equipamentoObj = undefined;
        }

        this.voltar = function () {
            var lastPage = ViewBag.get("last-page");
            if (typeof (lastPage) == "undefined") {
                lastPage = Auth.getUser().obraColaborador == null ? "/obra/escolher" : "/tarefa/cards";
            }
            $location.path(lastPage);
        }

        this.lista = function () {
            $location.path('/obra/index');
        }


        this.enviar = function () {
            $http({
                url: "api/login/RecuperarSenha",
                method: "POST",
                data: this.filter
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage) //mensagem de erro
            }).success(function (data, status, headers, config) {
                toastr.success("Senha enviada para: " + data);
                controller.voltar();
            });
        }

        $scope.changeImage = function (event) {
            var reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = function (e) {
                let verificarExtensao = e.target.result.substring(0, 24);
                if (!verificarExtensao.includes("jpeg") && !verificarExtensao.includes("jpq") && !verificarExtensao.includes("bmp") && !verificarExtensao.includes("png")) {
                    controller.cadastroParam.Imagem = '';
                    toastr.error("O arquivo selecionado não está em um formato de imagem válido.");
                    setTimeout(function () { $("#imagePreview").attr("src", "Assets/images/add-img.png"); }, 300);
                    return;
                }
                if (e.total > 2097152) {
                    controller.cadastroParam.Imagem = '';
                    toastr.error("O tamanho do arquivo selecionado não pode ser superior a 2MB");
                    setTimeout(function () { $("#imagePreview").attr("src", "Assets/images/add-img.png"); }, 300);
                    return;
                }
                controller.imageFileObra = e.target.result;
                $scope.$apply();
            };
        }

        this.removerImagem = function () {
            controller.imageFileObra = '';
            controller.cadastroParam.descricaoFoto = '';
            $('#imagePreview').attr('src', '');
        }

        //function createFilterFor(value) {
        //    console.log(controller.buscaTextoColaborador);
        //    var lowercaseQuery = angular.lowercase(controller.buscaTextoColaborador);
        //    return value.nome == lowercaseQuery;
        //}

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
    }
})();
