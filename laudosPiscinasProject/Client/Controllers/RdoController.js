﻿(function () {
    'use strict';
    angular.module('app').controller('RdoController', RdoController);
    RdoController.$inject = ['$http', '$location', 'ViewBag', 'Auth', 'Download', 'Validacao', 'Convert', '$scope'];
    function RdoController($http, $location, ViewBag, Auth, Download, Validacao, Convert, $scope) {
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
            idRdo: '', dataRdo: '', comentario: '', comentarioAssinatura: '', dataPrevisaoFim: '', statusRdo: 0,
            fabricanteFornecedor: '', dataAquisicao: '', contato: '', telefone: '', foto: '',
            idObra: '', listaTarefas: [], climaManhaCheckValue: '', climaTardeCheckValue: '', climaNoiteCheckValue: '',
            chuvaManhaCheckValue: '', chuvaTardeCheckValue: '', chuvaNoiteCheckValue: '', tipoComentarioAssinatura: '', tipoComentarioGeracao: 0 
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
        //this.climaManhaCheckValue = '';
        //this.climaTardeCheckValue = '';
        //this.climaNoiteCheckValue = '';


        //this.chuvaManhaCheckValue = '';
        //this.chuvaTardeCheckValue = '';
        //this.chuvaNoiteCheckValue = '';

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
                url: "api/rdo/CarregarLista",
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
            angular.forEach(controller.cadastroParam.listaTarefas, function (tarefa, key) {
                if (tarefa.existeRegistroMedicaoDiaAnterior && !tarefa.existeRegistroMedicaoDataRequerida) {
                    controller.tarefasSemHistoricos += controller.tarefasSemHistoricos == '' ? tarefa.descricao : ', ' + tarefa.descricao;
                    controller.existeTarefasSemHistoricos = true;
                }
                if (tarefa.existeRegistroMedicaoDataRequerida) {
                    controller.existeTarefasComHistoricos = true;
                }
            });
           
            if (Validacao.required(controller.cadastroParam.dataRdo)) {
                toastr.error("A Data deve ser preenchida.");
                return;
            }

            if (!Validacao.data(controller.cadastroParam.dataRdo)) {
                toastr.error("A Data é inválida.");
                return;
            }
            //if (!controller.existeTarefasComHistoricos && !controller.existeTarefasSemHistoricos) {
            //    toastr.error("Não existem registros para serem gerados no relatório.");
            //    return;
            //}
            if (controller.existeTarefasSemHistoricos) {
                MensagemConfirmacao("Tem certeza que deseja gerar o Relatório Diário para a data: " + controller.cadastroParam.dataRdo.substring(0, 10) + "? Caso já tenha sido gerado um relatório diário para esta data, o mesmo será substituído.", function () {
                    MensagemConfirmacao("Não existem lançamentos para o dia " + controller.cadastroParam.dataRdo.substring(0, 10) + " na(s) tarefa(s): " + controller.tarefasSemHistoricos + ". Deseja gerar o relatório diário para esta data com os lançamentos do dia anterior?", function () {
                        var user = Auth.getUser();
                        // controller.cadastroParam.tipoComentarioAssinatura - Saber se o usuário é contratante ou contratada
                        // controller.cadastroParam.tipoComentarioGeracao - Saber se foi like ou deslike
                        controller.cadastroParam.tipoComentarioAssinatura = Auth.getUser().obraColaborador.contratanteContratada;
                        controller.cadastroParam.idColaborador = Auth.getUser().usuario.id;
                        controller.cadastroParam.idObra = user.obra.idObra;
                        controller.cadastroParam.existeTarefasComHistoricos = controller.existeTarefasComHistoricos;
                        controller.cadastroParam.existeTarefasSemHistoricos = controller.existeTarefasSemHistoricos;
                        $http({
                            url: "api/rdo/Salvar",
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
                });
            }
            else {
                MensagemConfirmacao("Tem certeza que deseja gerar o Relatório Diário para a data: " + controller.cadastroParam.dataRdo.substring(0, 10) + "? Caso já tenha sido gerado um Relatório Diário para esta data, o mesmo será substituído.", function () {
                    var user = Auth.getUser();
                    // controller.cadastroParam.tipoComentarioAssinatura - Saber se o usuário é contratante ou contratada
                    // controller.cadastroParam.tipoComentarioGeracao - Saber se foi like ou deslike
                    controller.cadastroParam.tipoComentarioAssinatura = Auth.getUser().obraColaborador.contratanteContratada;
                    controller.cadastroParam.idColaborador = Auth.getUser().usuario.id;
                    controller.cadastroParam.idObra = user.obra.idObra;
                    controller.cadastroParam.existeTarefasComHistoricos = controller.existeTarefasComHistoricos;
                    controller.cadastroParam.existeTarefasSemHistoricos = controller.existeTarefasSemHistoricos;
                    $http({
                        url: "api/rdo/Salvar",
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
                url: "api/rdo/GerarDocumentoRdo",
                method: "POST",
                data: { idRdo: rdo.idRdo, gerarRelatorioFotografico: controller.mostrarFotos }
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage)
            }).success(function (data, status, headers, config) {
                controller.lista();
                var dataRdo = rdo.dataRdo;
                //var ano = dataAssinatura.substring(0, 4);
                //var mes = dataAssinatura.substring(5, 7);
                //var dia = dataAssinatura.substring(8, 10);
                //var date = dia + '_' + mes + '_' + ano;
                var nomeDocumento = "RDO_" + dataRdo + ".pdf";

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
            $location.path('/rdo/cadastro');
        }

        this.voltar = function () {
            $location.path('/tarefa/cards');
        }

        this.lista = function () {
            $location.path('/rdo/index');
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
    }
})();

