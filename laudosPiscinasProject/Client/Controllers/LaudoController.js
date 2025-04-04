(function () {
    'use strict';
    angular.module('app').controller('LaudoController', LaudoController);
    LaudoController.$inject = ['$http', '$location', 'ViewBag', 'Auth', 'Download', 'Validacao', 'Convert', '$scope', '$timeout'];
    function LaudoController($http, $location, ViewBag, Auth, Download, Validacao, Convert, $scope, $timeout) {
        var controller = this;

        this.pagedlist = {};
        //this.rdo = [];
        //this.rdo = {};
        //this.menus = [];
        //this.filter = { dataRdo: '' };
        //this.selectedMenu = 1;
        this.filtroParam = { dataRdo: '', statusRdo: 0, idObra: '', dataInicial: '', dataFinal: '' };
        this.filtroTarefasParam = { descricao: '', statusTarefa: 0, idObra: '' };


        this.cadastroParam = {
            idLaudo: '', dataLaudo: '', comentario: '', comentarioAssinatura: '', dataPrevisaoFim: '', statusLaudo: 0,
            fabricanteFornecedor: '', dataAquisicao: '', contato: '', telefone: '', foto: '',
            idObra: '', listaTarefas: [], tipoComentarioAssinatura: '', tipoComentarioGeracao: 0,
            nivelCloro: null, ph: null, limpidez: null, superficie: null, fundo: null,
            nivelCloro2: null, bacterias: null, proliferacao: null
        };

        this.orderby = '';
        this.orderbydescending = '';
        this.tarefas = [];
        this.imagens = [];
        this.statusRdo = [];
        this.statusTarefa = [];
        this.unidadeMedida = [];
        this.desabilitarSalvar = false;
        this.tarefa = {};
        this.desabilitarAssinarRdo = "";
        this.rdoTemporario = {};
        this.contratanteOucontratada = Auth.getUser().obraColaborador.contratanteContratada;
        this.desabilitarComentarioContratada = false;
        this.foto = '';
        this.tarefasSemHistoricos = '';
        this.existeTarefasSemHistoricos = false;
        this.existeTarefasComHistoricos = false;
        this.rdoSelecionado = {};

        this.mostrarFotos = false;
        controller.tarefaRDO = {};
        this.listaImagens = [];

        this.abrirfoto = function (caminho) {
            controller.foto = caminho;
        }

        this.carregarLista = function (page, order) {
            controller.filtroParam.dataInicial = $('.txbDataInicial').val();
            controller.filtroParam.dataFinal = $('.txbDataFinal').val();


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


            $http({
                url: "api/laudo/CarregarLista",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.pagedlist = data;
            });
        }

        this.carregarPaginaIndex = function () {
            controller.carregarLista(1);
            controller.getCurrentDate();
            controller.carregarStatusRdo();
            controller.verificarObraFinalizada();
        }

        this.carregarPaginaCadastro = function () {
            controller.carregarEdicao();
            controller.carregarStatusTarefa();
            controller.carregarUnidadeMedida();
            controller.getCurrentDate();
            controller.verificarObraFinalizada();

        }


        this.verificarObraFinalizada = function () {
            controller.desabilitarSalvar = Auth.getUser().obra.obraFinalizada;
        }


        this.carregarStatusRdo = function () {
            $http({
                url: "api/statusRdo/Lista/",
                method: "POST",
                data: undefined
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.statusRdo = data;
                controller.statusRdo.unshift({ id: 0, nome: 'Selecione...' });
            });
        }


        //this.carregarListaTarefas = function () {
        //    controller.filtroTarefasParam.idObra = Auth.getUser().obra.idObra;

        //    $http({
        //        //url: "api/tarefa/CarregarListaSimples",
        //        url: "api/tarefa/CarregarListaTarefasRdo",
        //        method: "POST",
        //        data: this.filtroTarefasParam
        //    }).error(function (data, status, headers, config) {
        //        toastr.error(data.exceptionMessage, data.message)
        //    }).success(function (data, status, headers, config) {
        //        controller.tarefas = data;
        //        controller.marcarTarefas();
        //    });
        //}


        this.carregarListaTarefas = function () {
            //controller.filtroTarefasParam.idObra = Auth.getUser().obra.idObra;
            controller.acordeonTarefas = [];
            controller.cadastroParam.listaTarefas = [];
            controller.cadastroParam.listaImagems = [];
            var dataRDO = controller.cadastroParam.dataRdo;
            if (dataRDO == undefined || dataRDO == '') {
                dataRDO = new Date().toJSON();
            }
            else {
                dataRDO = Convert.toJSONfromDateBR(dataRDO);
            }

            var filter = { idObra: Auth.getUser().obra.idObra, dataMedicao: dataRDO };
            $http({
                //url: "api/tarefa/CarregarListaSimples",
                //url: "api/etapa/get",
                url: "api/etapa/ObterTarefasParaRDO",
                method: "POST",
                data: filter
            }).error(function (data) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data) {
                controller.existeTarefasSemHistoricos = false;
                controller.existeTarefasComHistoricos = false;
                controller.etapas = data;
                var idRdo = ViewBag.get('rdoId');
                if (idRdo != "undefined" && idRdo > 0) {
                    controller.associarTarefas();
                    controller.associarImagens();
                    if (controller.imagens.length > 0) {
                        controller.mostrarFotos = true;

                    }
                } else {
                    var botaoRapido = ViewBag.get('botaoRapido');
                    if (botaoRapido) {
                        controller.etapas.forEach(function (etapa) {
                            controller.selectTarefas(etapa, etapa.id, true);
                        });
                        controller.salvar();
                        ViewBag.set('botaoRapido', false);
                    }
                }
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


        this.marcarTarefas = function () {
            for (var i = 0; i < controller.tarefas.length; i++) {
                controller.tarefas[i].marcado = true;
            }
        }

        this.habilitarAssinarRdo = function (rdo) {
            var user = Auth.getUser();
            var contratanteContratada = user.obraColaborador.contratanteContratada;

            if (contratanteContratada == 't' && rdo.statusRdo == 1) {
                controller.desabilitarAssinarRdo = true;
                return true;
            }
            else if (contratanteContratada == 't' && rdo.statusRdo == 3) {
                controller.desabilitarAssinarRdo = false;
                return false;
            }
            else if (contratanteContratada == 't' && rdo.statusRdo == 3) {
                controller.desabilitarAssinarRdo = true;
                return true;
            }
            else if (contratanteContratada == 'd' && rdo.statusRdo == 1) {
                controller.desabilitarAssinarRdo = false;
                return false;
            }
            else if (contratanteContratada == 'd' && rdo.statusRdo == 3) {
                controller.desabilitarAssinarRdo = true;
                return true;
            }
            else if (rdo.statusRdo == 2) {
                controller.desabilitarAssinarRdo = true;
                return true;
            }
            else {
                controller.desabilitarAssinarRdo = true;
                return true;
            }
        }

        this.carregarEdicao = function () {
            var idRdo = ViewBag.get('rdoId');
            if (idRdo != "undefined" && idRdo > 0) {
                $http({
                    url: "api/rdo/ObterRdo/",
                    method: "POST",
                    data: idRdo
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    controller.cadastroParam = JSON.parse(JSON.stringify(data));
                    controller.cadastroParam.listaImagens = [];
                    controller.tarefas = JSON.parse(JSON.stringify(data.listaTarefas));
                    controller.imagens = JSON.parse(JSON.stringify(data.listaImagens));
                    controller.carregarListaTarefas();
                });
            }
            else {
                controller.carregarListaTarefas();
            }
        }

        this.salvar = function () {
            controller.tarefasSemHistoricos = '';
            controller.cadastroParam.listaImagens = this.listaImagens;

            angular.forEach(controller.cadastroParam.listaTarefas, function (tarefa, key) {
                if (tarefa.existeRegistroMedicaoDiaAnterior && !tarefa.existeRegistroMedicaoDataRequerida) {
                    controller.tarefasSemHistoricos += controller.tarefasSemHistoricos == '' ? tarefa.descricao : ', ' + tarefa.descricao;
                    controller.existeTarefasSemHistoricos = true;
                }
                if (tarefa.existeRegistroMedicaoDataRequerida) {
                    controller.existeTarefasComHistoricos = true;
                }
            });

            if (Validacao.required(controller.cadastroParam.dataLaudo)) {
                
                toastr.error("A Data deve ser preenchida.");
                return;
            }

            if (!Validacao.data(controller.cadastroParam.dataLaudo)) {
                toastr.error("A Data é inválida.");
                return;
            }
            //if (!controller.existeTarefasComHistoricos && !controller.existeTarefasSemHistoricos) {
            //    toastr.error("Não existem registros para serem gerados no relatório.");
            //    return;
            //}
            if (controller.existeTarefasSemHistoricos) {
                MensagemConfirmacao("Tem certeza que deseja gerar o laudo para a data: " + controller.cadastroParam.dataLaudo.substring(0, 10) + "? Caso já tenha sido gerado um laudo para esta data, o mesmo será substituído.", function () {
                    MensagemConfirmacao("Não existem lançamentos para o dia " + controller.cadastroParam.dataLaudo.substring(0, 10) + " na(s) tarefa(s): " + controller.tarefasSemHistoricos + ". Deseja gerar o laudo para esta data com os lançamentos do dia anterior?", function () {
                        var user = Auth.getUser();
                        // controller.cadastroParam.tipoComentarioAssinatura - Saber se o usuário é contratante ou contratada
                        // controller.cadastroParam.tipoComentarioGeracao - Saber se foi like ou deslike
                        controller.cadastroParam.tipoComentarioAssinatura = Auth.getUser().obraColaborador.contratanteContratada;
                        controller.cadastroParam.idColaborador = Auth.getUser().usuario.id;
                        controller.cadastroParam.idObra = user.obra.idObra;
                        controller.cadastroParam.existeTarefasComHistoricos = controller.existeTarefasComHistoricos;
                        controller.cadastroParam.existeTarefasSemHistoricos = controller.existeTarefasSemHistoricos;
                        $http({
                            url: "api/laudo/Salvar",
                            method: "POST",
                            data: controller.cadastroParam
                        }).error(function (data) {
                            //if (data.exceptionMessage.toLowerCase().includes('rdo pendente')) {
                            //    MensagemConfirmacao('Não existem lançamentos para o dia [Data]. Deseja gerar o RDO para esta data com os lançamentos do dia anterior?', function () { return MensagemConfirmacao('você é gay?', function () { toastr.success('eu já sabia') }) });
                            //    return;
                            //}
                            console.log("");
                            toastr.error(data.Message);
                        }).success(function (data) {
                            controller.lista();
                            controller.gerarDocumento(data);
                            toastr.success("Registro salvo com sucesso.");
                        });
                    });
                });
            }
            else {
                MensagemConfirmacao("Tem certeza que deseja gerar o laudo para a data: " + controller.cadastroParam.dataLaudo.substring(0, 10) + "? Caso já tenha sido gerado um laudo para esta data, o mesmo será substituído.", function () {
                    var user = Auth.getUser();
                    // controller.cadastroParam.tipoComentarioAssinatura - Saber se o usuário é contratante ou contratada
                    // controller.cadastroParam.tipoComentarioGeracao - Saber se foi like ou deslike
                    controller.cadastroParam.tipoComentarioAssinatura = Auth.getUser().obraColaborador.contratanteContratada;
                    controller.cadastroParam.idColaborador = Auth.getUser().usuario.id;
                    controller.cadastroParam.idObra = user.obra.idObra;
                    controller.cadastroParam.existeTarefasComHistoricos = controller.existeTarefasComHistoricos;
                    controller.cadastroParam.existeTarefasSemHistoricos = controller.existeTarefasSemHistoricos;
                    $http({
                        url: "api/laudo/Salvar",
                        method: "POST",
                        data: controller.cadastroParam
                    }).error(function (data) {
                        //if (data.exceptionMessage.toLowerCase().includes('rdo pendente')) {
                        //    MensagemConfirmacao('Não existem lançamentos para o dia [Data]. Deseja gerar o RDO para esta data com os lançamentos do dia anterior?', function () { return MensagemConfirmacao('você é gay?', function () { toastr.success('eu já sabia') }) });
                        //    return;
                        //}
                        toastr.error(data.Message);
                    }).success(function (data) {
                        controller.lista();
                        controller.gerarDocumento(data);
                        toastr.success("Registro salvo com sucesso.");
                    });
                });
            }
        }

        this.mudarTipoComentarioGeracao = function (val) {
            controller.cadastroParam.tipoComentarioGeracao = val;
        }

        this.mudarTipoComentarioAssinatura = function (val) {
            controller.cadastroParam.tipoComentarioAssinatura = val;
        }

        this.associarTarefas = function () {
            for (var i = 0; i < controller.tarefas.length; i++) {
                let idTarefa = controller.etapas.find(e => e.tarefas.find(t => t.agrupador == controller.tarefas[i].agrupador)).tarefas.find(t => t.agrupador == controller.tarefas[i].agrupador).id;
                controller.selectTarefa(true, idTarefa, 0);
            }
        }

        this.associarImagens = function () {
            for (var i = 0; i < controller.imagens.length; i++) {
                let idTarefa = controller.etapas.find(e => e.tarefas.find(t => t.agrupador == controller.imagens[i].agrupadorTarefa)).tarefas.find(t => t.agrupador == controller.imagens[i].agrupadorTarefa).id;
                controller.selectImagem(true, controller.imagens[i].idImagem, idTarefa);
            }
        }

        this.deletar = function (rdo) {
            var user = Auth.getUser();
            rdo.idObra = user.obra.idObra;

            MensagemConfirmacao("Tem certeza que deseja excluir o registro de data: " + rdo.dataRdo + "?", function () {
                $http({
                    url: "api/rdo/Deletar",
                    method: "POST",
                    data: rdo
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage)
                }).success(function (data, status, headers, config) {
                    controller.carregarLista(controller.pagedlist.currentPage);
                    toastr.success("Registro excluído com sucesso.");
                });
            });
        }

        this.editar = function (rdo) {
            var user = Auth.getUser();
            controller.cadastroParam.idObra = user.obra.idObra;
            rdo.idObra = user.obra.idObra;
            ViewBag.set('rdoIdObra', user.obra.idObra);


            ViewBag.set('rdoId', rdo.idRdo);
            $location.path('/rdo/cadastro');
        }

        this.assinar = function (rdo) {
            //var dataAssinatura = rdo.dataRdo.substring(0, 10);
            //var ano = dataAssinatura.substring(0, 4);
            //var mes = dataAssinatura.substring(5, 7);
            //var dia = dataAssinatura.substring(8, 10);
            //var date = dia + '/' + mes + '/' + ano;
            this.cadastroParam.comentarioAssinatura = "";
            var user = Auth.getUser();
            rdo.idObra = user.obra.idObra;
            rdo.idAssinante = user.obraColaborador.idObraColaborador;
            var postData = {};
            postData.rdo = rdo;
            postData.objIp = { ip: ''};

            //$.getJSON('http://freegeoip.net/json/?callback=?', function (data) {
            //    console.log('ip', data);
            //    postData.objIp = data; // JSON.stringify(data, null, 2);
            //});

            $.getJSON("https://api.ipify.org?format=json",
            function (data) {
                postData.objIp.ip = data.ip;
            })
            .fail(function () {
                $.getJSON("http://ip-api.com/json/",
                function (data) {
                    postData.objIp.ip = data.query;
                })
                .fail(function () {
                    $.getJSON("https://api.myip.com",
                    function (data) {
                        postData.objIp.ip = data.ip;
                    })
                })
            });

            var dataAssinatura = postData.rdo.dataRdo.substring(0, 10);

            this.rdoTemporario = postData;
            
            controller.desabilitarComentarioContratada = rdo.statusstatusContratanteContratadaDonoRdo != controller.contratanteOucontratada ? false : true;
            
            MensagemConfirmacao("Tem certeza que deseja assinar o RDO de data: " + dataAssinatura + "?", function () {
                $http({
                    url: "api/rdo/Assinar",
                    method: "POST",
                    data: postData
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage)
                }).success(function (data, status, headers, config) {
                    controller.carregarLista(controller.pagedlist.currentPage);
                    toastr.success("Registro assinado.");
                });

            });
        }

        this.adicionarComentarioAssinatura = function () {
            var postData = { rdo: '', objIp: {} };
            postData.rdo = controller.rdoSelecionado;

            $.getJSON("https://api.ipify.org?format=json",
            function (data) {
                postData.objIp.ip = data.ip;
            })
            .fail(function () {
                $.getJSON("http://ip-api.com/json/",
                function (data) {
                    postData.objIp.ip = data.query;
                })
                .fail(function () {
                    $.getJSON("https://api.myip.com",
                    function (data) {
                        postData.objIp.ip = data.ip;
                    })
                })
            });

            var user = Auth.getUser();
            postData.rdo.idObra = user.obra.idObra;
            postData.rdo.idAssinante = user.obraColaborador.idObraColaborador;
            postData.rdo.comentarioAssinatura = controller.cadastroParam.comentarioAssinatura;
            postData.rdo.tipoComentarioAssinatura = controller.cadastroParam.tipoComentarioAssinatura;
            var dataAssinatura = postData.rdo.dataRdo.substring(0, 10);
            $('#comentario-assinatura').modal('hide');
            MensagemConfirmacao("Tem certeza que deseja assinar o RDO de data: " + dataAssinatura + "?", function () {
                $http({
                    url: "api/rdo/Assinar",
                    method: "POST",
                    data: postData
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage)
                }).success(function (data, status, headers, config) {
                    controller.carregarLista(controller.pagedlist.currentPage);
                    toastr.success("Registro assinado.");
                });

            });
        }


        function convertURIToBinary(base64) {
            var raw = window.atob(base64);
            var rawLength = raw.length;
            var array = new Uint8Array(new ArrayBuffer(rawLength));

            for (i = 0; i < rawLength; i++) {
                array[i] = raw.charCodeAt(i);
            }
            return array;
        }


        //this.saveByteArray = function (reportName, array) {
        //    var blob = new Blob([array], { type: 'application/octet-stream' });
        //    var link = document.createElement('a');
        //    link.href = window.URL.createObjectURL(blob);
        //    var fileName = reportName + ".pdf";
        //    link.download = fileName;
        //    link.click();
        //};

        this.gerarDocumento = function (rdo) {
            $http({
                url: "api/laudo/GerarDocumentoLaudo",
                method: "POST",
                data: { idRdo: rdo.laudo_id_laudo, gerarRelatorioFotografico: controller.mostrarFotos }
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage)
            }).success(function (data, status, headers, config) {
                controller.lista();
                var dataRdo = rdo.laudo_dt_laudo;
                //var ano = dataAssinatura.substring(0, 4);
                //var mes = dataAssinatura.substring(5, 7);
                //var dia = dataAssinatura.substring(8, 10);
                //var date = dia + '_' + mes + '_' + ano;
                var nomeDocumento = "LAUDO_" + dataRdo + ".pdf";

                Download.base64(data, nomeDocumento);
                //controller.saveByteArray("Rdo", convertURIToBinary(data));
            });
        }


        this.editarTarefa = function (tarefa) {
            var user = Auth.getUser();
            controller.cadastroParam.idObra = user.obra.idObra;
            tarefa.idObra = user.obra.idObra;
            ViewBag.set('tarefaIdObra', user.obra.idObra);

            ViewBag.set('statusTela', 'editar');
            ViewBag.set('tarefaId', tarefa.id);
            $location.path('/tarefa/cadastro');
        }

        this.selecionaTodasTarefas = function () {
            for (var i in controller.tarefas) {
                controller.tarefas[i].marcado = controller.selecionarTarefas;
            }
        }

        controller.selectTarefas = function (etapa, idEtapa, check) {
            angular.forEach(etapa.tarefas, function (tarefa, index) {
                controller.selectTarefa(check, tarefa.id, index);
            }) 
        }

        controller.selectTarefa = function (checked, idTarefa, index) {
            if (checked) {
                if (controller.acordeonTarefas == undefined) {
                    controller.acordeonTarefas = [];
                }
                
                let tarefa = controller.etapas.find(e => e.tarefas.find(t => t.id == idTarefa)).tarefas.find(t => t.id == idTarefa);
                
                if (controller.cadastroParam.listaTarefas.find(x => x.id === idTarefa) == undefined) {
                    controller.cadastroParam.listaTarefas.push(tarefa);
                }
                if(controller.acordeonTarefas.find(x => x.id === idTarefa) == undefined){
                    controller.acordeonTarefas.push(tarefa);
                }
                controller.etapas.find(e => e.tarefas.find(t => t.id == idTarefa)).tarefas.find(t => t.id == idTarefa).check = checked;
                
                return;
            }
            else {
                if (controller.acordeonTarefas == undefined) {
                    controller.acordeonTarefas = [];
                }
                let indexListaTarefas = controller.cadastroParam.listaTarefas.findIndex(x => x.id === idTarefa);
                let indexTarefaAcordeon = controller.acordeonTarefas.findIndex(x => x.id === idTarefa);
                if (indexListaTarefas != undefined) {
                    controller.cadastroParam.listaTarefas.splice(indexListaTarefas, 1);
                }
                if (indexTarefaAcordeon != undefined) {
                    controller.acordeonTarefas.splice(indexTarefaAcordeon, 1);
                }
                controller.etapas.find(e => e.tarefas.find(t => t.id == idTarefa)).tarefas.find(t => t.id == idTarefa).check = checked;
                return;
            }
        }

        controller.selectImagens = function (tarefa, check) {
            angular.forEach(tarefa.listaImagem, function (imagem, index) {
                if (controller.cadastroParam.listaImagems.find(x => x.idImagem == imagem.idImagem) == undefined) {
                    controller.selectImagem(check, imagem.idImagem, tarefa.id);
                }
                else if (check == false) {
                    controller.selectImagem(check, imagem.idImagem, tarefa.id);
                }
            })
        }

        controller.selectImagem = function (checked, idImagem, idTarefa) {
            if (checked) {
                if (controller.cadastroParam.listaImagems == undefined) {
                    controller.cadastroParam.listaImagems = [];
                }
                controller.cadastroParam.listaImagems.push({ idImagem: idImagem });
                controller.etapas.find(e => e.tarefas.find(t => t.id == idTarefa)).tarefas.find(t => t.listaImagem.find(t => t.idImagem == idImagem)).listaImagem.find(i => i.idImagem == idImagem).check = checked;
            }
            else {
                controller.etapas.find(e => e.tarefas.find(t => t.id == idTarefa)).tarefas.find(t => t.listaImagem.find(t => t.idImagem == idImagem)).listaImagem.find(i => i.idImagem == idImagem).check = checked;
                var indexImagem = controller.cadastroParam.listaImagems.findIndex(x => x.idImagem == idImagem);
                controller.cadastroParam.listaImagems.splice(indexImagem, 1);
            }
        }

        this.preencherModalTarefa = function (tarefa) {
            controller.tarefa = tarefa;
        }

        this.novo = function () {
            ViewBag.set('rdoId', undefined)
            ViewBag.set('botaoRapido', false);
            $location.path('/laudos/cadastro');
        }

        this.voltar = function () {
            $location.path('/tarefa/cards');
        }

        this.lista = function () {
            $location.path('/laudos/index');
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

            //controller.filtroParam.dataInicial = today;
            controller.cadastroParam.dataRdo = today;

        }

        this.selecionarRdo = function (rdo) {
            controller.rdoSelecionado = rdo;
            controller.cadastroParam.tipoComentarioAssinatura = '';
            controller.cadastroParam.comentarioAssinatura = '';
        }

        this.adicionarImagem = function () {

            if (controller.listaImagens.length == 16) {
                toastr.error("A licença adquirida só permite adicionar 16 fotos por laudo. Favor verificar com o administrador.", "Erro");
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

        this.removerImagem = function (imagem, index) {
            if (!this.listaImagensRemovidas) {
                this.listaImagensRemovidas = [];
            }

            if (imagem.idRDO != null) {
                toastr.error("Ela está vinculada a um Laudo", "Não é possível remover esta imagem")
                return;
            }
            this.listaImagensRemovidas.push(imagem);
            this.listaImagens.splice(index, 1);
        }

        //Funções da imagem
        this.toFileReader = function (dataurl, filename) {
            var bstr = atob(dataurl.base64),
                n = bstr.length,
                u8arr = new Uint8Array(n);

            while (n--) {
                u8arr[n] = bstr.charCodeAt(n);
            }
            return new File([u8arr], dataurl.filename, { type: dataurl.filetype });
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

        this.tobase64 = function (evt, callback) {
            var file = evt;
            var reader = new FileReader();

            reader.readAsDataURL(file);
            reader.onloadend = function () {
                callback(reader.result);
            }
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
    }
})();

