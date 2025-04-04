(function () {
    'use strict';
    angular.module('app').controller('TarefaController', TarefaController);
    TarefaController.$inject = ['$scope', '$http', '$location', 'ViewBag', 'Auth', '$rootScope', 'Validacao', 'Convert', 'Permission', 'Email', '$timeout'];
    function TarefaController($scope, $http, $location, ViewBag, Auth, $rootScope, Validacao, Convert, Permission, Email, $timeout) {
        var controller = this;
        controller.objEtapa = {};
        this.pagedlist = {};
        this.tarefa = [];
        this.listaTarefas = [];
        this.filtroParam = { descricao: '', statusTarefa: 0, idObra: '', dataInicial: '', dataFinalPlanejada: null, dataInicialExecutada: '', dataFinalExecutada: '' };
        this.cadastroParam = {
            Id: '', descricao: '', dataInicio: '', dataPrevisaoFim: '', status: 1, horasTrabalhadas: '',
            qtdConstruida: '', idUnidade: 0, unidade: '', comentario: '', telefone: '', foto: '', idObra: '',
            listaColaboradores: [], listaEquipamentos: [], listaAcidentes: [], colaboradorObj: {}, equipamentoObj: {}, contratanteContratada: '',
            listaColaboradoresRemovidos: [], listaEquipamentosRemovidos: [], listaAcidentesRemovidos: [], listaColaboradoresObra: [], listaEquipamentosObra: []
        };
        this.orderby = '';
        this.orderbydescending = '';
        this.tarefa = {};
        this.sexo = [{ id: '', nome: 'Selecione...' }, { id: 'M', nome: 'Masculino' }, { id: 'F', nome: 'Feminino' }];
        this.tipoAquisicao = [{ id: 'A', nome: 'Aluguel' }, { id: 'C', nome: 'Compra' }];
        this.statusTarefa = [];
        this.statusTarefaEmMassa = 0;
        this.unidadeMedida = [];
        this.grupos = [];
        this.cargos = [];
        this.habilitarCamposColaborador;
        this.habilitarCamposEquipamento;
        this.habilitarCamposAcidente;
        this.colaboradoresObra = [];
        this.equipamentosObra = [];
        this.houveAfastamentoCheckValue = '';
        this.objTarefaHistorico = {};
        this.acidenteObj = {};
        this.listaColaboradoresAcidente = [];
        this.colaboradorAcidenteObj = {};
        this.uploadFile = {};
        this.elements = [];
        this.checkedElements = [];
        this.desabilitarCampos = false;
        this.desabilitarSalvar = false;
        this.desabilitarCpf = false;
        this.nomeColaboradorModoEdicao = false;
        this.desabilitarCamposColaborador;
        this.desabilitarCamposColaborador2;

        controller.quantidadeMaximaFotos = 1;
        this.listaImagens = [];

        controller.perfilColaborador = false;

        // WF
        controller.cardsArray = [];

        controller.loadCards = function (titulo) {
            if (!controller.cardsArray['\'' + titulo + '\'']) {
                let values = controller.etapas.filter(function (i) { return i.titulo == titulo }).map(e => e.tarefas);

                if (values.length > 0) {
                    controller.cardsArray['\'' + titulo + '\''] = values[0];
                }
            }
        }

        controller.verificaPerfilColaborador = function () {
            var idPerfilColaborador = controller.cadastroParam.colaboradorObj.grupo;

            if (idPerfilColaborador != undefined && !isNaN(idPerfilColaborador)) {

                //todo: ver maneira de pegar o valor do colaborador parametrizado, pois está fixado o id
                if (idPerfilColaborador == controller.grupos.find(grupo => grupo.nome == 'Colaborador').id || idPerfilColaborador == controller.grupos.find(grupo => grupo.nome == 'Terceirizado').id) { // Colaborador Terceirizado
                    controller.perfilColaborador = true;
                    return;
                }
                controller.perfilColaborador = false;
            }
        }

        controller.buscarColaboradorNome = function (searchText) {

            controller.cadastroParam.ColaboradorNaoEncontrado = searchText;
            return $http
                .get("api/Colaborador/ObterColaboradorNome/", { params: { nome: searchText } })
                .then(function (data) {
                    var listaColaborador = data.data;
                    return listaColaborador;
                });
        };

        this.carregarTelaCadastro = function () {
            controller.carregarEdicao();
            //controller.carregarStatusTarefa();
            controller.carregarUnidadeMedida();
            controller.carregaDropEtapa();
            controller.getCurrentDate();
            controller.verificarObraFinalizada();

            controller.carregaListaUF();
            controller.carregarGrupos();
            controller.carregarCargos();
            controller.carregarEquipamentosObra();
            controller.carregarColaboradoresObra();

            //controller.carregarMarcas();
            //controller.carregarModelos();
            controller.carregarQuantidadeMaximaFotos();
            controller.carregaTipoEquipamento();

            // Setar valor do drop de Etapa
            if ($rootScope.idEtapa != null && $rootScope.idEtapa != undefined) {
                var idEtapa = $rootScope.idEtapa;
                $rootScope.idEtapa = undefined;
                controller.cadastroParam.idEtapa = idEtapa;
            }

            let idTarefa = ViewBag.get('tarefaId');
            if (idTarefa != undefined && idTarefa != null && idTarefa > 0) {
                //controller.preencherModalHistorico(idTarefa);
            }
            if (idTarefa != undefined && idEtapa != null) {

            }
            // não sei pq fica resquício quando é chamado a partir do modal do histórico da medição
            // precisamos remover estes elementos forçadamente
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open');
        }

        this.carregarQuantidadeMaximaFotos = function () {

            var idObra = Auth.getUser().obra.idObra;
            if (idObra != undefined) {
                $http.get('api/obra/quantidadeMaximaFotos', { params: { id: idObra } })
                    .success(function (data) {
                        controller.quantidadeMaximaFotos = data;
                    });
            }
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

        this.maskPhone = function (phone) {
            if (phone == undefined || phone == '' || phone == null) {
                return;
            }
            try {
                //if (phone.length == 13 || phone.length == 15) {
                //    var x = phone.replace(/\D/g, '').match(/(\d{0,2})(\d{0,5})(\d{0,4})/);
                //    var p = '(' + x[1] + ') ' + x[2] + '-' + x[3];
                //    return p;
                //}
                //else if (phone.length == 12 || phone.length == 14) {
                //    var x = phone.replace(/\D/g, '').match(/(\d{0,2})(\d{0,4})(\d{0,4})/);
                //    var p = '(' + x[1] + ') ' + x[2] + '-' + x[3];
                //    return p;
                //}    

                if (phone.length % 2 == 0) {
                    var x = phone.replace(/\D/g, '').match(/(\d{0,2})(\d{0,4})(\d{0,4})/);
                    var p = '(' + x[1] + ') ' + x[2] + '-' + x[3];
                    return p;
                }
                else {
                    var x = phone.replace(/\D/g, '').match(/(\d{0,2})(\d{0,5})(\d{0,4})/);
                    var p = '(' + x[1] + ') ' + x[2] + '-' + x[3];
                    return p;
                }
            }
            catch (ex) {
                return phone;
            }
        }

        this.maskCPF = function (cpf) {
            try {
                var x = cpf.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,3})(\d{0,2})/);
                var c = x[1] + '.' + x[2] + '.' + x[3] + '-' + x[4];
                return c;
            }
            catch (ex) {
                return cpf;
            }
        }

        this.getImage = function (imagem) {
            if (imagem) {
                if (typeof (imagem.href) != 'undefined') {
                    return imagem.href;
                }
                if (typeof (imagem.base64) != 'undefined') {
                    return "data:" + imagem.filetype + ";base64," + imagem.base64;
                }
                if (imagem.split(',')[0].includes('data:')) {
                    return imagem;
                }
                return '#';
            }
        }

        this.cadastroRdo = function () {
            ViewBag.set('botaoRapido', true);
            $location.path("/rdo/cadastro");
        }

        this.efetivoDiario = function () {
            $location.path("/efetivo/index");
        }

        this.verificarObraFinalizada = function () {
            controller.desabilitarSalvar = Auth.getUser().obra.obraFinalizada;
        }

        this.carregarLista = function (page, order) {

            controller.filtroParam.dataInicial = $('.txbDataInicialPlanejadaFiltro').val();
            controller.filtroParam.dataFinalPlanejada = $('.txbDataFinalPlanejadaFiltro').val();
            controller.filtroParam.dataInicialExecutada = $('.txtDataInicialExecutadaFiltro').val();
            controller.filtroParam.dataFinalExecutada = $('.txtDataFinalExecutadaFiltro').val();

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
            controller.filtroParam.idObra = Auth.getUser().obra.idObra;

            $http({
                url: "api/tarefa/CarregarLista",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.pagedlist = data;
            });

            controller.verificarObraFinalizada();
        }

        this.carregarListaCards = function () {
            controller.filtroParam.dataInicial = $('.txbDataInicialPlanejadaFiltro').val();
            controller.filtroParam.dataFinalPlanejada = $('.txbDataFinalPlanejadaFiltro').val();
            controller.filtroParam.dataInicialExecutada = $('.txtDataInicialExecutadaFiltro').val();
            controller.filtroParam.dataFinalExecutada = $('.txtDataFinalExecutadaFiltro').val();

            if (Auth.getUser().obra == null || Auth.getUser().obra.idObra == null || Auth.getUser().obra.idObra == 0) {
                Auth.updateUser(Auth.getLoginUser());
                $location.path('/obra/escolher');
                return;
            }

            controller.filtroParam.idObra = Auth.getUser().obra.idObra;

            $http({
                url: "api/tarefa/CarregarListaSimples",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.listaTarefas = data;
            });

            controller.verificarObraFinalizada();
        }

        this.carregaListaEtapa = function () {

            //$http({
            //    url: 'api/etapa/get/?idObra=' + Auth.getUser().obra.idObra,
            //    method: 'GET'
            //}).error(function (data) {
            //    toastr.error(data.exceptionMessage, data.message)
            //}).success(function (data) {
            //    controller.etapas = data;
            //});

            //controller.filtroParam.idObra = Auth.getUser().obra.idObra;
            //controller.filtroParam.id = controller.filtroParam.idEtapa;
            //$http.get('api/etapa/get/', { params: controller.filtroParam }).
            //    success(function (data) {
            //        controller.etapas = data;
            //    });

            controller.filtroParam.idObra = Auth.getUser().obra.idObra;
            controller.filtroParam.id = controller.filtroParam.idEtapa;
            $http.get('api/etapa/ObterEtapaTarefa/', { params: controller.filtroParam }).
                success(function (data) {
                    controller.etapas = data;
                    controller.cardsArray = [];
                });
        }

        this.carregaDropEtapa = function () {

            let idObra = Auth.getUser().obra.idObra;
            $http({
                url: 'api/etapa/Get/',
                method: 'GET',
                params: { idObra: idObra }
            }).error(function (data) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data) {
                controller.etapaList = data;
            });
        }

        this.carregarStatusTarefa = function () {
            $http({
                url: "api/statusTarefa/Lista/",
                method: "POST",
                data: undefined
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.statusTarefa = data;
                controller.statusTarefa.unshift({ id: 0, nome: 'Selecione...' });
            });
        }


        this.carregarStatusTarefaPermitido = function () {
            $http({
                url: "api/statusTarefa/ListaStatusTarefaPermitidos/",
                method: "POST",
                data: controller.cadastroParam.status
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.statusTarefa = data;
                controller.statusTarefa.unshift({ id: 0, nome: 'Selecione...' });
            });
        }


        this.carregarUnidadeMedida = function () {
            $http({
                url: "api/unidadeMedida/Lista/",
                method: "POST",
                data: undefined
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.unidadeMedida = data;
                controller.unidadeMedida.unshift({ id: 0, nome: 'Selecione...' });
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
                postdata = controller.cadastroParam.colaboradorObj.uf;
            }
            else {
                postdata = controller.cadastroParam.colaboradorObj.uf;
            }
            $http({
                url: "api/municipio/ListaMunicipio/",
                method: "POST",
                data: postdata
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.municipiosColaborador = data;
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
            $http({
                url: "api/municipio/ListaUF",
                method: "POST"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.ufsColaborador = data;
            });
        }



        this.carregarEdicao = function () {

            //para controlar a posição do drop de Status na tela
            // se estiver true é p mostrar no campo de tarefa 
            // se estiver false é p mostrar no campo de medição
            // se estiver com status Planejado é p ser true
            this.mostraDropStatusNaTarefa = true;

            var id = ViewBag.get('tarefaId');
            if (id != "undefined" && id > 0) {

                this.mostraDropStatusNaTarefa = false;
                const novaMedicao = ViewBag.get('novaMedicao');
                $http({
                    url: "api/tarefa/ObterTarefa/",
                    method: "POST",
                    data: id
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    controller.cadastroParam = data;

                    controller.cadastroParam.dataInicio = Convert.toDateFromStringDateBR(controller.cadastroParam.dataInicio)
                    controller.cadastroParam.dataPrevisaoFim = Convert.toDateFromStringDateBR(controller.cadastroParam.dataPrevisaoFim)
                    controller.cadastroParam.dataMedicaoTela = Convert.toDateFromStringDateBR(controller.cadastroParam.dataMedicaoTela)

                    if (controller.cadastroParam.status != 1) {
                        controller.statusTarefa = controller.statusTarefa.filter(function (obj) {
                            return obj.nome !== 'Planejada';
                        });
                    }
                    if (!novaMedicao) {
                        controller.carregarImagens();
                    }
                    controller.carregarStatusTarefaPermitido();
                    controller.desabilitarCampos = ViewBag.get('desabilitarCamposTarefa');
                    controller.cadastroParam.qtdPlanejada = controller.adicionarPonto(controller.cadastroParam.qtdPlanejada.toString().replace(".", ","));
                    controller.cadastroParam.valor = "R$ " + controller.adicionarPonto(controller.cadastroParam.valor.toString().replace(".", ","));
                    controller.cadastroParam.qtdConstruida = controller.adicionarPonto(controller.cadastroParam.qtdConstruida.toString().replace(".", ","));

                    if (novaMedicao) {
                        controller.cadastroParam.dataMedicaoTela = new Date();
                        controller.adicionarMedicao();
                    }
                });
            }
            else {
                controller.cadastroParam.status = 0;
                controller.carregarStatusTarefaPermitido();
                controller.cadastroParam.novaMedicao = true;
            }
        }

        this.carregarColaboradoresObra = function () {
            var filterObj = { idObra: Auth.getUser().obra.idObra };

            $http({
                url: "api/obra/ObterColaboradoresObra/",
                method: "POST",
                data: filterObj
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.colaboradoresObra = data;
            });
        }

        this.carregarEquipamentosObra = function () {
            var filterObj = { idObra: Auth.getUser().obra.idObra };

            $http({
                url: "api/obra/ObterEquipamentosObra/",
                method: "POST",
                data: filterObj
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.equipamentosObra = data;
            });
        }

        this.requestSave = function () {
            $http({
                url: "api/tarefa/Salvar",
                method: "POST",
                data: controller.cadastroParam
            }).error(function (data) {
                toastr.error(data.Message);
            }).success(function () {
                const novaMedicao = ViewBag.get('novaMedicao');
                if (novaMedicao) {
                    $('#nova-medicao-botao-rapido').modal('hide');
                    controller.carregarListaCards();
                    controller.carregarStatusTarefa();
                    controller.carregaListaEtapa();
                }
                controller.cards();
                toastr.success("Registro salvo com sucesso.");
            });
        }

        this.salvar = function () {
            if (Validacao.required(controller.cadastroParam.descricao)) {
                toastr.error("A Descrição deve ser preenchida.");
                return;
            }

            if (!Validacao.minLenght(controller.cadastroParam.descricao)) {
                toastr.error("Por favor, introduza pelo menos três caracteres na Descrição da Tarefa.");
                return;
            }

            if (controller.cadastroParam.idEtapa == undefined || controller.cadastroParam.idEtapa <= 0) {
                toastr.error("A Etapa deve ser preenchida.");
                return;
            }

            if ($('.txbDataInicialPlanejada .md-datepicker-input')[0] && Validacao.required($('.txbDataInicialPlanejada .md-datepicker-input').val())) {
                toastr.error("A Data Inicial Planejada deve ser preenchida.");
                return;
            }

            if ($('.txbDataInicialPlanejada .md-datepicker-input').val()?.length > 0 && !Validacao.data($('.txbDataInicialPlanejada .md-datepicker-input').val())) {
                toastr.error("A Data Inicial Planejada é inválida.");
                return;
            }

            if ($('.txbDataFinalPlanejada .md-datepicker-input')[0] && Validacao.required($('.txbDataFinalPlanejada .md-datepicker-input').val())) {
                toastr.error("A Data Final Planejada deve ser preenchida.");
                return;
            }

            if ($('.txbDataFinalPlanejada .md-datepicker-input').val()?.length > 0 && !Validacao.data($('.txbDataFinalPlanejada .md-datepicker-input').val())) {
                toastr.error("A Data Final Planejada é inválida.");
                return;
            }

            let dataInicialPlanejada = controller.cadastroParam.dataInicio;
            let dataFinalPlanejada = controller.cadastroParam.dataPrevisaoFim;

            if (dataInicialPlanejada > dataFinalPlanejada) {
                toastr.error("A Data Final não pode ser menor que a Data Inicial.");
                return;
            }

            if (controller.cadastroParam.status <= 0) {
                toastr.error("O Status deve ser preenchido.");
                return;
            }

            //if (controller.cadastroParam.status != 1 && (controller.cadastroParam.idUnidade == undefined || controller.cadastroParam.idUnidade <= 0)) {
            //    toastr.error("A Unidade de Medida deve ser preenchida.");
            //    return;
            //}
            //if (controller.cadastroParam.status != 1 && (controller.cadastroParam.qtdPlanejada == undefined || Number.isNaN(controller.cadastroParam.qtdPlanejada))) {
            //    toastr.error("A Quantidade Planejada deve ser preenchida.");
            //    return;
            //}
            //if (controller.cadastroParam.status != 1 && (controller.cadastroParam.valor == undefined || Number.isNaN(controller.cadastroParam.valor))) {
            //    toastr.error("O Valor Unitário deve ser preenchido.");
            //    return;
            //}

            if (controller.cadastroParam.status != 1 && Validacao.required($('.data-medicao .md-datepicker-input').val())) {
                toastr.error("A Data de Medição deve ser preenchida.");
                return;
            }

            if ($('.data-medicao .md-datepicker-input').val()?.length > 0 && !Validacao.data($('.data-medicao .md-datepicker-input').val())) {
                toastr.error("A Data de Medição é inválida.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.horaInicial)) {
                controller.cadastroParam.horaInicial = '00:00:00';
            }

            if (!Validacao.required(controller.cadastroParam.horaInicial) && !Validacao.hourTime(controller.cadastroParam.horaInicial)) {
                toastr.error("A Hora Inicial é inválida.");
                return;
            }

            if (Validacao.required(controller.cadastroParam.horaFinal)) {
                if (!Validacao.required(controller.cadastroParam.horaInicial)) {
                    controller.cadastroParam.horaFinal = controller.cadastroParam.horaInicial;
                } else {
                    controller.cadastroParam.horaFinal = '00:00:00';
                }
            }

            if (!Validacao.required(controller.cadastroParam.horaFinal) && !Validacao.hourTime(controller.cadastroParam.horaFinal)) {
                toastr.error("A Hora Final é inválida.");
                return;
            }

            if (window.validate("#form-cadastro-tarefa")) {                
                if (controller.cadastroParam.qtdPlanejada)
                    controller.cadastroParam.qtdPlanejada = parseFloat(controller.cadastroParam.qtdPlanejada.toString().split('.').join('').replace(",", "."));
                else
                    controller.cadastroParam.qtdPlanejada = 0;

                if (controller.cadastroParam.valor)
                    controller.cadastroParam.valor = parseFloat(controller.cadastroParam.valor.toString().split('.').join('').replace(",", ".").replace("R$ ", ""));
                else
                    controller.cadastroParam.valor = 0;

                if (!Validacao.required(controller.cadastroParam.qtdConstruida)) {
                    controller.cadastroParam.qtdConstruida = parseFloat(controller.cadastroParam.qtdConstruida.toString().split('.').join('').replace(",", "."));
                }

                var user = Auth.getUser();
                controller.cadastroParam.IdObra = user.obra.idObra;

                if ($('.txbDataInicialPlanejada').val())
                    controller.cadastroParam.dataInicio = $('.txbDataInicialPlanejada').val();
                if ($('.txbDataFinalPlanejada').val())
                    controller.cadastroParam.dataPrevisaoFim = $('.txbDataFinalPlanejada').val();

                controller.cadastroParam.contratanteContratada = user.obraColaborador.contratanteContratada;
                controller.cadastroParam.listaImagens = this.listaImagens;
                controller.cadastroParam.listaImagensRemovidas = this.listaImagensRemovidas;
                controller.cadastroParam.idColaboradorInsercao = user.usuario.id;
                //controller.cadastroParam.imageFile = $scope.imageFile;
                
                if (controller.cadastroParam.dataMedicaoTela != undefined) {
                    //controller.cadastroParam.dataMedicaoTela = controller.cadastroParam.dataMedicaoTela.toLocaleDateString();
                    //controller.cadastroParam.dataMedicao = Convert.toJSONfromDateBR(controller.cadastroParam.dataMedicaoTela);
                    controller.cadastroParam.dataMedicao = Convert.toJSON(controller.cadastroParam.dataMedicaoTela);
                }

                if (controller.cadastroParam.id != "undefined" && controller.cadastroParam.id > 0 && !controller.cadastroParam.novaMedicao && controller.cadastroParam.listaHistoricoTarefa.length > 0) {
                    MensagemConfirmacao("Tem certeza que deseja substituir medição existente?", function () {
                        controller.requestSave()
                    });
                } else {
                    controller.requestSave()
                }
            }
            else {
                console.log('not validated!!!');
            }
        }

        controller.selectTarefas = function (etapa, idEtapa, check) {
            angular.forEach(etapa.tarefas, function (tarefa, index) {
                controller.selectTarefa(check, tarefa.id, 0);
            })
        }

        controller.selectTarefa = function (checked, idTarefa, index) {
            if (checked) {
                if (controller.checkedElements == undefined) {
                    controller.checkedElements = [];
                }
                let tarefa = controller.etapas.find(e => e.tarefas.find(t => t.id == idTarefa)).tarefas.find(t => t.id == idTarefa);
                if (controller.checkedElements.find(x => x === idTarefa) == undefined) {
                    controller.checkedElements.push(idTarefa);
                    controller.etapas.find(e => e.tarefas.find(t => t.id == idTarefa)).tarefas.find(t => t.id == idTarefa).check = checked;
                }
                return;
            }
            else {
                if (controller.checkedElements == undefined) {
                    controller.checkedElements = [];
                }
                controller.checkedElements.splice(index, 1);
                controller.etapas.find(e => e.tarefas.find(t => t.id == idTarefa)).tarefas.find(t => t.id == idTarefa).check = checked;
                return;
            }
        }

        this.alterarStatusEmMassa = function (tipoTela) {
            //$(":input").each(function () {
            //    var input = $(this);
            //    controller.elements.push(input)
            //});

            //for (var i = 0; i < controller.elements.length; i++) {
            //    if ($(controller.elements[i][0]).is(':checked')) {
            //        controller.checkedElements.push(controller.elements[i][0].id);
            //    }
            //}

            var submitData = { tarefas: controller.checkedElements, status: controller.statusTarefaEmMassa };

            $http({
                url: "api/tarefa/AtualizarStatusEmMassa",
                method: "POST",
                data: submitData
            }).error(function (data, status, headers, config) {
                toastr.error(data.Message);
            }).success(function (data, status, headers, config) {
                if (true) {
                    toastr.success("Status alterados com sucesso.");
                }
                else {
                    toastr.error("Ocorreram erros");
                }

                $('input:checkbox').removeAttr('checked');
                if (tipoTela == 'cards') {
                    controller.carregarListaCards();
                }
                else {
                    controller.carregarLista(1);
                }

            });
        }

        this.cleanCheckedElements = function () {
            controller.checkedElements = [];
        }

        this.deletar = function (tarefa, tipoTela) {
            var user = Auth.getUser();
            tarefa.idObra = user.obra.idObra;


            //MensagemConfirmacao("Deseja realmente excluir esse registro?", function () {
            MensagemConfirmacao("Tem certeza que deseja excluir o registro: " + tarefa.descricao + "?", function () {
                $http({
                    url: "api/tarefa/Deletar",
                    method: "POST",
                    data: tarefa
                }).error(function (data, status, headers, config) {
                    toastr.error("Não é possível excluir esta tarefa. Existem registros dependentes.");
                }).success(function (data, status, headers, config) {
                    if (data) {
                        controller.removerTarefa(tarefa.id);
                        toastr.success("Registro excluído com sucesso.");
                    }
                    else {
                        toastr.error("Não é possível excluir esta tarefa. Existem registros dependentes.");
                    }

                });

            });
        }

        this.removerTarefa = function (idTarefa) {
            angular.forEach(controller.etapas, function (etapa, indexEtapa) {
                angular.forEach(etapa.tarefas, function (tarefa, indexTarefa) {
                    if (tarefa.id == idTarefa) {
                        controller.etapas[indexEtapa].tarefas.splice(indexTarefa, 1);
                    }
                })
            })

        };

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

        this.editar = function (tarefa, descricaoTarefa = '', novaMedicao = false) {
            var user = Auth.getUser();
            controller.cadastroParam.idObra = user.obra.idObra;
            controller.descricaoTarefa = descricaoTarefa;
            // tarefa.idObra = user.obra.idObra;

            ViewBag.set('tarefaIdObra', user.obra.idObra);

            ViewBag.set('desabilitarCamposTarefa', false);
            ViewBag.set('statusTela', 'editar');

            if (isNaN(tarefa)) {

                if (tarefa.idTarefa == undefined) {
                    ViewBag.set('tarefaId', tarefa.id);
                }
                else {
                    ViewBag.set('tarefaId', tarefa.idTarefa);
                }
            }
            else {
                ViewBag.set('tarefaId', tarefa);
            }

            ViewBag.set('novaMedicao', novaMedicao);

            $('#historico-tarefa-detalhe').modal('hide');
            controller.carregarTelaCadastro();

            if (novaMedicao) {
                $('#nova-medicao-botao-rapido').modal('show');
            } else {
                $location.path('tarefa/cadastro');
            }
        }

        this.visualizar = function (tarefa) {
            var user = Auth.getUser();
            controller.cadastroParam.idObra = user.obra.idObra;
            // tarefa.idObra = user.obra.idObra;

            ViewBag.set('tarefaIdObra', user.obra.idObra);

            ViewBag.set('desabilitarCamposTarefa', true);
            ViewBag.set('statusTela', 'visualizar');

            if (isNaN(tarefa)) {

                if (tarefa.idTarefa == undefined) {
                    ViewBag.set('tarefaId', tarefa.id);
                }
                else {
                    ViewBag.set('tarefaId', tarefa.idTarefa);
                }
            }
            else {
                ViewBag.set('tarefaId', tarefa);
            }

            $('#historico-tarefa-detalhe').modal('hide');
            controller.carregarTelaCadastro();
            $location.path('tarefa/cadastro');
        }

        this._visualizar = function (tarefa) {

            var user = Auth.getUser();

            controller.cadastroParam.idObra = user.obra.idObra;
            tarefa.idObra = user.obra.idObra;
            ViewBag.set('tarefaIdObra', user.obra.idObra);
            // limpar o que não precisa

            ViewBag.set('desabilitarCamposTarefa', true);
            ViewBag.set('statusTela', 'visualizar');

            if (isNaN(tarefa)) {
                ViewBag.set('tarefaId', tarefa.idTarefa);
            }
            else {
                ViewBag.set('tarefaId', tarefa);
            }

            if (controller.cadastroParam.listaHistoricoTarefa) {
                let novaMedicao = controller.cadastroParam.listaHistoricoTarefa.find(x => x.idTarefa == tarefa.idTarefa);

                let novaData = novaMedicao.dataStatus.split('T')[0].split('-').reverse().join('/');

                controller.cadastroParam.dataMedicaoTela = novaData;

                if (novaMedicao.descricaoStatusTarefa == 'Planejada') {
                    controller.cadastroParam.status = 1;
                }
                if (novaMedicao.descricaoStatusTarefa == 'Em Execução') {
                    controller.cadastroParam.status = 2;
                }
                if (novaMedicao.descricaoStatusTarefa == 'Finalizada') {
                    controller.cadastroParam.status = 3
                }
                if (novaMedicao.descricaoStatusTarefa == 'Pausada') {
                    controller.cadastroParam.status = 4
                }
                if (novaMedicao.descricaoStatusTarefa == 'Cancelada') {
                    controller.cadastroParam.status = 5
                }

                controller.cadastroParam.horaFinal = novaMedicao.horaFinal;
                controller.cadastroParam.horaInicial = novaMedicao.horaInicial;
                controller.cadastroParam.idHistoricoTarefa = novaMedicao.idHistoricoTarefa;
                controller.cadastroParam.idStatusTarefa = novaMedicao.idStatusTarefa;
                controller.cadastroParam.idTarefa = novaMedicao.idTarefa;
                controller.cadastroParam.nomeColaborador = novaMedicao.nomeColaborador;
                controller.cadastroParam.qtdConstruida = novaMedicao.qtdConstruida;
                controller.cadastroParam.unidade = novaMedicao.unidade;
                controller.cadastroParam.unidadeMedida = novaMedicao.unidadeMedida;

            }

            $('#historico-tarefa-detalhe').modal('hide');
            $location.path('/tarefa/cadastro');

        }

        this.novaTarefa = function (idEtapa) {
            ViewBag.set('tarefaId', undefined);
            if (idEtapa != null && idEtapa != undefined) {
                $rootScope.idEtapa = idEtapa
            }
            $location.path('/tarefa/cadastro');
        }

        this.novaEtapa = function () {
            ViewBag.set('retornoCadastroEtapa', $location.path());
            ViewBag.set('tarefaId', undefined);
            $location.path('/etapa/cadastro');
        }

        this.lista = function () {
            $location.path('/tarefa/index');
        }

        this.cards = function () {
            $location.path('/tarefa/cards');
        }


        this.editarObra = function () {
            var obra = Auth.getUser().obra;

            ViewBag.set('novaObra', false);
            ViewBag.set('obraId', obra.idObra);


            ViewBag.set('last-page', '/tarefa/cards');

            $location.path('/obra/cadastro');
        }


        this.getCurrentDate = function () {
            var today = new Date();
            //var dd = today.getDate();
            //var mm = today.getMonth() + 1;
            //var yyyy = today.getFullYear();

            //if (dd < 10) {
            //    dd = '0' + dd
            //}

            //if (mm < 10) {
            //    mm = '0' + mm
            //}
            
            //today = dd + '/' + mm + '/' + yyyy;
            controller.cadastroParam.dataInicio = today;
        }

        this.changeStatus = function (tarefa, idStatus) {
            var filterObj = { tarefa: tarefa, idStatus: idStatus, idColaboradorInsercao: Auth.getUser().usuario.id };

            $http({
                url: "api/tarefa/AtualizarStatus",
                method: "POST",
                data: filterObj
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage)
            }).success(function (data, status, headers, config) {
                controller.cards();
                //controller.carregarLista(1);
                controller.carregarListaCards();
                controller.carregarStatusTarefa();
                controller.carregaListaEtapa();
                toastr.success("Status alterado com sucesso.");
            });

        }


        //colaborador
        this.removerObraTarefaColaborador = function (colaborador, index) {
            this.cadastroParam.listaColaboradoresRemovidos.push(colaborador);
            this.cadastroParam.listaColaboradores.splice(index, 1);
        }

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

        this.associarColaboradores = function () {
            for (var i = 0; i < controller.colaboradoresObra.length; i++) {
                for (var j = 0; j < controller.cadastroParam.listaColaboradores.length; j++) {
                    if (controller.colaboradoresObra[i].idColaborador == controller.cadastroParam.listaColaboradores[j].idColaborador) {
                        controller.colaboradoresObra[i].marcado = true;
                    }
                }
            }
        }

        this.recarregarColaboradores = function () {
            controller.cadastroParam.listaColaboradores = controller.cadastroParam.listaColaboradores || [];

            for (var i = 0; i < controller.colaboradoresObra.length; i++) {
                let colaborador = controller.colaboradoresObra[i];
                let added = controller.cadastroParam.listaColaboradores.find(eq => eq.idColaborador == colaborador.idColaborador) != null;
                if (colaborador.marcado == true) {
                    if (!added) {
                        controller.cadastroParam.listaColaboradores.push(colaborador);
                    }
                }
                else {
                    if (added) {
                        controller.cadastroParam.listaColaboradores = controller.cadastroParam.listaColaboradores.filter(eq => eq.idColaborador != colaborador.idColaborador);
                    }
                    controller.cadastroParam.listaColaboradoresRemovidos.push(controller.colaboradoresObra[i]);
                }
            }



            //for (var i = 0; i < controller.colaboradoresObra.length; i++) {
            //    if (controller.colaboradoresObra[i].marcado == true) {
            //        controller.cadastroParam.listaColaboradores.push(controller.colaboradoresObra[i]);
            //    }
            //    else {
            //        controller.cadastroParam.listaColaboradoresRemovidos.push(controller.colaboradoresObra[i]);
            //    }

            //}
        }

        //equipamentos
        this.removerObraTarefaEquipamento = function (equipamento, index) {
            this.cadastroParam.listaEquipamentosRemovidos.push(equipamento);
            this.cadastroParam.listaEquipamentos.splice(index, 1);
            for (var i = 0; i < controller.equipamentosObra.length; i++) {
                if (controller.equipamentosObra[i].id == equipamento.id) {
                    controller.equipamentosObra[i].marcado = false;
                }
            }
        }

        this.associarEquipamentos = function () {
            for (var i = 0; i < controller.equipamentosObra.length; i++) {
                for (var j = 0; j < controller.cadastroParam.listaEquipamentos.length; j++) {
                    if (controller.equipamentosObra[i].id == controller.cadastroParam.listaEquipamentos[j].id) {
                        controller.equipamentosObra[i].marcado = true;
                    }
                }
            }
        }

        this.adicionarEquipamento = function () {
            this.cadastroParam.equipamentoObj = this.cadastroParam.equipamentoObj == undefined ? {} : this.cadastroParam.equipamentoObj;

            if (controller.cadastroParam.equipamentoObj.idTipoEquipamento == undefined || controller.cadastroParam.equipamentoObj.idTipoEquipamento <= 0) {
                toastr.error("O Tipo de Equipamento deve ser preenchido.");
                return;
            }

            controller.cadastroParam.equipamentoObj.tipoEquipamento = controller.tiposEquipamentos.find(t => t.id == controller.cadastroParam.equipamentoObj.idTipoEquipamento).nome;

            if (Validacao.required(controller.cadastroParam.equipamentoObj.descricao)) {
                toastr.error("A Descrição deve ser preenchida.");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.equipamentoObj.descricao, 2)) {
                toastr.error("Por favor, introduza pelo menos dois caracteres no Tipo de Equipamento");
                return;
            }

            if (window.validate("#novo-equipamento")) {

                if (this.cadastroParam.equipamentoObj.telefone) {
                    if (this.cadastroParam.equipamentoObj.telefone.replace(/_/g, '').length < 14) {
                        toastr.error("O Telefone é inválido.");
                        return;
                    }
                }

                this.cadastroParam.listaEquipamentos.push(this.cadastroParam.equipamentoObj);
                $('#novo-equipamento').modal('toggle');
            }

        }

        this.recarregarEquipamentos = function () {
            controller.cadastroParam.listaEquipamentos = controller.cadastroParam.listaEquipamentos || [];

            for (var i = 0; i < controller.equipamentosObra.length; i++) {
                const equipamento = controller.equipamentosObra[i];
                const added = controller.cadastroParam.listaEquipamentos.find(eq => eq.id == equipamento.id) != null;
                if (equipamento.marcado == true) {
                    if (!added) {
                        controller.cadastroParam.listaEquipamentos.push(equipamento);
                    }
                }
                else {
                    if (added) {
                        controller.cadastroParam.listaEquipamentos = controller.cadastroParam.listaEquipamentos.filter(eq => eq.id != equipamento.id);
                    }
                    controller.cadastroParam.listaEquipamentosRemovidos.push(controller.equipamentosObra[i]);
                }
            }
        }

        this.preencherModalEquipamento = function (equipamento) {
            controller.cadastroParam.equipamentoObj = equipamento;
            controller.habilitarCamposEquipamento = true;
        }

        this.novoEquipamento = function () {
            controller.cadastroParam.equipamentoObj = {};
            controller.cadastroParam.equipamentoObj.idTipoEquipamento = '';
            controller.cadastroParam.equipamentoObj.marca = '';
            controller.cadastroParam.equipamentoObj.modelo = '';
            controller.cadastroParam.equipamentoObj.tipoAquisicao = '';
            controller.habilitarCamposEquipamento = false;
        }

        this.limparModalEquipamento = function () {
            controller.cadastroParam.equipamentoObj = undefined;
        }


        //acidente
        this.carregarListaColaboradoresAcidente = function () {
            controller.listaColaboradoresAcidente = controller.cadastroParam.listaColaboradores;
        }


        this.novoAcidente = function () {
            controller.acidenteObj = {};
            controller.acidenteObj.listaAcidenteColaboradores = [];

            controller.carregarListaColaboradoresAcidente();
            controller.getCurrentDate();

            controller.habilitarCamposAcidente = false;
        }

        this.adicionarColaboradorAcidente = function () {
            controller.acidenteObj.listaAcidenteColaboradores.push(controller.colaboradorAcidenteObj);
        }

        this.removerColaboradorAcidente = function (colaborador, index) {
            controller.acidenteObj.listaAcidenteColaboradores.splice(index, 1);
        }

        this.removerAcidente = function (acidente, index) {
            controller.cadastroParam.listaAcidentes.splice(index, 1);
            controller.cadastroParam.listaAcidentesRemovidos.push(acidente);
        }



        this.adicionarAcidente = function () {
            this.acidenteObj = this.acidenteObj == undefined ? {} : this.acidenteObj;

            if (Validacao.required(controller.acidenteObj.descricao)) {
                toastr.error("A Descrição do Acidente deve ser preenchida");
                return;
            }
            if (!Validacao.minLenght(controller.acidenteObj.descricao)) {
                toastr.error("Por favor, introduza pelo menos três caracteres na Descrição do Acidente");
                return;
            }
            if (Validacao.required(controller.acidenteObj.dataAcidenteTela)) {
                toastr.error("A Data/Hora do Acidente deve ser preenchida.");
                return;
            }
            controller.acidenteObj.dataAcidente = Convert.toJSONfromDateBR(controller.acidenteObj.dataAcidenteTela);
            if (!Validacao.data(controller.acidenteObj.dataAcidenteTela)) {
                toastr.error("A Data/Hora do Acidente é inválida.");
                return;
            }
            if (window.validate("#novo-acidente")) {

                controller.acidenteObj.dataAcidenteTela = $('.dataAcidente').val();
                controller.acidenteObj.houveAfastamento = 'n';

                for (var i = 0; i < controller.acidenteObj.listaAcidenteColaboradores.length; i++) {
                    if (controller.acidenteObj.listaAcidenteColaboradores[i].houveAfastamento == 's') {
                        controller.acidenteObj.houveAfastamento = 's';
                    }
                }

                controller.cadastroParam.listaAcidentes.push(controller.acidenteObj);
                controller.acidenteObj = undefined;
                $('#novo-acidente').modal('toggle');
            }
        }


        this.preencherModalAcidente = function (acidente) {
            controller.acidenteObj = acidente;
            controller.habilitarCamposAcidente = true;
        }



        //historico
        this.preencherModalHistorico = function (tarefa) {
            controller.objTarefaHistorico = [];
            controller.objTarefaHistorico.descricao = tarefa.descricao;
            //controller.objTarefaHistorico.listaHistoricoTarefa = [];

            $http({
                url: "api/tarefa/CarregarHistoricoTarefa/",
                method: "POST",
                data: { id: tarefa.id }
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.objTarefaHistorico.listaHistoricoTarefa = data;
            });

        }

        this.preencherModalHistoricoDetalhe = function () {
            controller.objTarefaHistorico = controller.cadastroParam;
        }


        this.getCurrentDate = function () {
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

            controller.cadastroParam.dataRdo = today;

        }


        this.adicionarImagem = function () {

            if (controller.listaImagens.length == controller.quantidadeMaximaFotos) {
                toastr.error("A licença adquirida só permite adicionar " + controller.quantidadeMaximaFotos + " fotos por tarefa. Favor verificar com o administrador.", "Erro");
                return;
            }

            if ($scope.imageFile && $scope.imageFile.filesize > 2097152) {
                toastr.error("O tamanho máximo da imagem é de 2MB.", "Erro");
                return;
            }

            // // para funcionar remova base-sixty-four-input do componente input files
            //var _file = document.getElementById('inputFile').files[0];
            //if ($.inArray(_file, this.listaImagens) < 0) {
            //    if (!this.listaImagens) this.listaImagens = [];

            //    var outro = this;
            //    //var _file = outro.toFileReader($scope.imageFile);
            //    outro.getOrientation(_file, ori => {     
            //        outro.tobase64(_file, b64 => { 
            //            outro.fixOrientation(b64, ori, img => {
            //                outro.listaImagens.push(img);
            //            })
            //        })
            //    });
            //}

            if ($.inArray($scope.imageFile, this.listaImagens) < 0) {
                if (!this.listaImagens) this.listaImagens = [];
                if ($scope.imageFile) {

                    var outro = this;
                    var _file = outro.toFileReader($scope.imageFile);
                    outro.getOrientation(_file, ori => {
                        outro.tobase64(_file, b64 => {
                            outro.fixOrientation(b64, ori, img => {
                                outro.listaImagens.push(outro.toCompatible(img));
                            })
                        })
                    });
                    $scope.imageFile = undefined;
                    document.getElementById("imgFileText").value = "";
                }
            }

            //if ($.inArray($scope.imageFile, this.listaImagens) < 0) {
            //    if ($scope.imageFile != undefined) {
            //        if (!this.listaImagens) this.listaImagens = [];
            //        this.listaImagens.push($scope.imageFile);
            //        $scope.imageFile = undefined;
            //        document.getElementById("imgFileText").value = "";
            //    }
            //}
        }

        this.toCompatible = function (str) {
            var normal = new Object();
            var arr = str.split(',');

            normal.filetype = arr[0].split(':')[1].split(';')[0];
            normal.filename = this.uuidv4().replace(/-/g, '') + '.png';
            //normal.filesize = file.size;
            normal.base64 = arr[1];
            return normal;
        }

        this.uuidv4 = function () {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }

        this.selecionarTodosColaboradores = function () {
            for (var i in controller.colaboradoresObra) {
                controller.colaboradoresObra[i].marcado = controller.selecionaColaboradores;
            }
        }

        this.selecionaTodosEquipametos = function () {
            for (var i in controller.equipamentosObra) {
                controller.equipamentosObra[i].marcado = controller.selecionaEquipamentos;
            }
        }

        this.removerImagem = function (imagem, index) {
            if (!this.listaImagensRemovidas) {
                this.listaImagensRemovidas = [];
            }

            if (imagem.idRDO != null) {
                toastr.error("Ela está vinculada a um RDO", "Não é possível remover esta imagem")
                return;
            }
            this.listaImagensRemovidas.push(imagem);
            this.listaImagens.splice(index, 1);
        }


        this.carregarImagens = function () {
            var id = ViewBag.get('tarefaId');
            if (id != "undefined" && id > 0) {
                $http({
                    url: "api/tarefa/ObterImagensTarefa/",
                    method: "POST",
                    data: id
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    controller.listaImagens = data;
                });
            }
            else {
                console.log("id = 0!!!");
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
                        controller.carregaListaMunicipio('cadastro', data.idMunicipio);
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
            controller.carregaListaMunicipio('cadastro', colaborador.idMunicipio);
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
            controller.cadastroParam.colaboradorObj = undefined;
        }

        this.limparPainelMedicao = function () {
            controller.cadastroParam.dataMedicaoTela = undefined;
            controller.cadastroParam.horaInicial = undefined;
            controller.cadastroParam.horaFinal = undefined;
            controller.cadastroParam.qtdConstruida = undefined;
            controller.cadastroParam.comentario = undefined;
            //controller.cadastroParam.listaImagens = [];
            //controller.cadastroParam.listaImagensRemovidas = [];
            this.listaImagens = [];
            this.listaImagensRemovidas = [];
        }

        this.adicionarEtapa = function () {

            if (Validacao.required(controller.objEtapa.Titulo)) {
                toastr.error("O Nome deve ser preenchido.");
                return;
            }
            if (Validacao.required(controller.objEtapa.Ordem)) {
                toastr.error("O Nº Ordem deve ser preenchido.");
                return;
            }

            var user = Auth.getUser();
            controller.objEtapa.idObra = user.obra.idObra;
            $http.post('api/etapa/AdicionaRecupera', controller.objEtapa)
                .success(function (data) {
                    var etapa = data;
                    controller.carregaDropEtapa();
                    controller.cadastroParam.idEtapa = etapa.id;
                    controller.objEtapa = undefined;
                    $('#adicionar-etapa').modal('toggle');
                })
                .error(function (data) {
                    toastr.error(data.exceptionMessage, 'Ocorreu um erro');
                });
        }

        this.verificarPermissaoCadastroTarefa = function () {
            return Permission.check('criarNovo', '/tarefa/cards');
        }

        this.verificarPermissaoAdicionarEtapa = function () {
            return Permission.check('criarNovo', '/etapa/index');
        }

        this.adicionarPonto = function (number) {
            number += '';
            var x = number.split('.');
            var x1 = x[0];
            var x2 = x.length > 1 ? '.' + x[1] : '';
            var rgx = /(\d+)(\d{3})/;

            while (rgx.test(x1)) {
                x1 = x1.replace(rgx, '$1' + '.' + '$2');
            }

            return x1 + x2;
        }

        this.limparCamposMedicao = function () {
            controller.cadastroParam.horaInicial = '';
            controller.cadastroParam.horaFinal = '';
            controller.cadastroParam.horasTrabalhadas = '';
            controller.cadastroParam.qtdConstruida = '';
            controller.cadastroParam.comentario = '';
            controller.cadastroParam.dataMedicaoTela = new Date();
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

        this.adicionarMedicao = function () {
            controller.cadastroParam.rdoAssinado = false;
            controller.cadastroParam.novaMedicao = true;
            controller.limparCamposMedicao();
            controller.listaImagens = [];
            controller.tituloTarefa = ''
            if (controller.cadastroParam.status != 1) {
                controller.statusTarefa = controller.statusTarefa.filter(function (obj) {
                    return obj.nome !== 'Planejada';
                });
            }
        }

        //#################### Solução para modificar a orientação da imagem ####################

        this.tobase64 = function (evt, callback) {
            var file = evt;
            var reader = new FileReader();

            reader.readAsDataURL(file);
            reader.onloadend = function () {
                callback(reader.result);
            }
        }

        this.toFileReader = function (dataurl, filename) {
            var bstr = atob(dataurl.base64),
            n = bstr.length,
            u8arr = new Uint8Array(n);

            while (n--) {
                u8arr[n] = bstr.charCodeAt(n);
            }
            return new File([u8arr], dataurl.filename, { type: dataurl.filetype });
        }

        this.fixOrientation = function (srcBase64, srcOrientation, callback) {
            var img = new Image();
            img.onload = function () {
                var width = img.width,
                    height = img.height,
                    canvas = document.createElement('canvas'),
                    ctx = canvas.getContext("2d");

                // set proper canvas dimensions before transform & export
                if (4 < srcOrientation && srcOrientation < 9) {
                    canvas.width = height;
                    canvas.height = width;
                } else {
                    canvas.width = width;
                    canvas.height = height;
                }

                // transform context before drawing image
                switch (srcOrientation) {
                    case 2: ctx.transform(-1, 0, 0, 1, width, 0); break;
                    case 3: ctx.transform(-1, 0, 0, -1, width, height); break;
                    case 4: ctx.transform(1, 0, 0, -1, 0, height); break;
                    case 5: ctx.transform(0, 1, 1, 0, 0, 0); break;
                    case 6: ctx.transform(0, 1, -1, 0, height, 0); break;
                    case 7: ctx.transform(0, -1, -1, 0, height, width); break;
                    case 8: ctx.transform(0, -1, 1, 0, 0, width); break;
                    default: break;
                }

                // draw image
                ctx.drawImage(img, 0, 0);
                // export base64
                callback(canvas.toDataURL());
                $timeout();
            }
            img.src = srcBase64;
        }

        this.getOrientation = function (file, callback) {
            var reader = new FileReader();
            reader.onload = function (e) {

                var view = new DataView(e.target.result);
                if (view.getUint16(0, false) != 0xFFD8) {
                    return callback(-2);
                }
                var length = view.byteLength, offset = 2;
                while (offset < length) {
                    if (view.getUint16(offset + 2, false) <= 8) return callback(-1);
                    var marker = view.getUint16(offset, false);
                    offset += 2;
                    if (marker == 0xFFE1) {
                        if (view.getUint32(offset += 2, false) != 0x45786966) {
                            return callback(-1);
                        }

                        var little = view.getUint16(offset += 6, false) == 0x4949;
                        offset += view.getUint32(offset + 4, little);
                        var tags = view.getUint16(offset, little);
                        offset += 2;
                        for (var i = 0; i < tags; i++) {
                            if (view.getUint16(offset + (i * 12), little) == 0x0112) {
                                return callback(view.getUint16(offset + (i * 12) + 8, little));
                            }
                        }
                    }
                    else if ((marker & 0xFF00) != 0xFF00) {
                        break;
                    }
                    else {
                        offset += view.getUint16(offset, false);
                    }
                }
                return callback(-1);
            };
            reader.readAsArrayBuffer(file);
        }
    }
})();

