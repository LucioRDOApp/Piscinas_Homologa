(() => {
    'use strict';
    angular.module('app').controller('GraficoController', GraficoController);
    GraficoController.$inject = [
        '$http', '$location', 'Auth', 'Styles', 'StoreService', 'Grafico', '$scope', 'Validacao', 'Format'
    ];

    function GraficoController($http, $location, Auth, Styles, StoreService, Grafico, $scope, Validacao, Format) {
        Styles.loadStyles([
            //Mudar quando subir versão
            '/Assets/Styles/waves.min.css',
            '/Assets/Styles/style.css'
        ]);

        Styles.unloadStyles([
            
        ]);

        //if ($('#containerGrafico').height() <= 400) {
        //    $('footer').addClass('navbar-fixed-bottom');
        //} else {
        //    $('footer').removeClass('navbar-fixed-bottom');
        //}

        const controller = this;

        this.filter = {
            periodoIni: '',
            periodoFim: '',
            ColaboradorId: Auth.getUser().usuario.id
        };

        $scope.filter = {
            periodoIni: '',
            periodoFim: ''
        };

        $scope.chart = {};
        $scope.rdosAtrasados = [];

        this.carregarTelaIndex = () => {
            Grafico.getRdoAtrasado({
                periodoIni: moment(new Date()).subtract(1, 'months').format('YYYY-MM-DD'),
                periodoFim: moment(new Date()).format('YYYY-MM-DD'),
                ColaboradorId: Auth.getUser().usuario.id
            }).then((data, status, headers, config) => {
                $scope.rdosAtrasados = data;

                controller.rdos = data;
                controller.buildRdosAtrasados(data);

                Grafico.getRdoGerado({
                    periodoIni: moment(new Date()).subtract(1, 'months').format('YYYY-MM-DD'),
                    periodoFim: moment(new Date()).format('YYYY-MM-DD'),
                    ColaboradorId: Auth.getUser().usuario.id
                }).then((data, status, headers, config) => {
                    $scope.rdos = buildTableRdos(data, $scope.rdosAtrasados);
                }).catch((data, status, headers, config) => {
                    toastr.error(data.exceptionMessage, data.message);
                });
            }).catch((data, status, headers, config) => {
                toastr.error(data.exceptionMessage, data.message);
            });

            Grafico.getDiaImprodutivo({
                periodoIni: moment(new Date()).subtract(1, 'months').format('YYYY-MM-DD'),
                periodoFim: moment(new Date()).format('YYYY-MM-DD'),
                ColaboradorId: Auth.getUser().usuario.id
            }).then((data, status, headers, config) => {
                $scope.improdutivos = data;
                controller.buildDiasImprodutivos(data);
            }).catch((data, status, headers, config) => {
                toastr.error(data.exceptionMessage, data.message);
            });

            Grafico.getTarefa({
                periodoIni: moment(new Date()).subtract(1, 'months').format('YYYY-MM-DD'),
                periodoFim: moment(new Date()).format('YYYY-MM-DD'),
                ColaboradorId: Auth.getUser().usuario.id
            }).then((data, status, headers, config) => {
                $scope.tarefas = data;

                controller.buildTarefasComInicioEmAtraso(data);

                Grafico.getStatusTarefa({
                    periodoIni: moment(new Date()).subtract(1, 'months').format('YYYY-MM-DD'),
                    periodoFim: moment().format('YYYY-MM-DD'),
                    ColaboradorId: Auth.getUser().usuario.id
                }).then((data, status, headers, config) => {
                    $scope.trs = buildTableTarefa($scope.tarefas, data);
                }).catch((data, status, headers, config) => {
                    toastr.error(data.exceptionMessage, data.message);
                });
            }).catch((data, status, headers, config) => {
                toastr.error(data.exceptionMessage, data.message);
            });

            Grafico.getComentario({
                periodoIni: moment(new Date()).subtract(1, 'months').format('YYYY-MM-DD'),
                periodoFim: moment(new Date()).format('YYYY-MM-DD'),
                ColaboradorId: Auth.getUser().usuario.id
            }).then((data, status, headers, config) => {
                $scope.comentarios = data;
                controller.buildComentarios(data);
            }).catch((data, status, headers, config) => {
                toastr.error(data.exceptionMessage, data.message);
            });
        };

        this.carregarTelaGraficoRdo = () => {
            if (StoreService.isData()) this.filter = StoreService.getData();
            else this.setDate();

            if (!moment(this.filter.periodoIni).isValid() ||
                !moment(this.filter.periodoFim).isValid() ||
                moment(this.filter.periodoFim)
                .isBefore(moment(this.filter.periodoIni))) {

                $location.path('/chart');
                toastr.error('Período fornecido inválido.', 'Dados incorretos');
            } else {
                Grafico.getRdoAtrasado(this.filter)
                    .then((data, status, headers, config) => {
                        $scope.atrasados = data;

                        Grafico.getRdoGerado(this.filter)
                            .then((data, status, headers, config) => {
                                $scope.rdos = buildTableRdos(data, $scope.atrasados);
                                $scope.chart = ConstruirHighchartRdo(data, $scope.atrasados);
                                if ($scope.chart.isEmpty) {
                                    $location.path('/chart');
                                    toastr.info(
                                        'Não existem dados cadastrados para que o gráfico da categoria de RDO´S seja gerado.');
                                } else {
                                    GerarGraficoBar('container', $scope.chart);
                                }
                            }).catch((data, status, headers, config) => {
                                toastr.error(data.exceptionMessage, data.message);
                            });
                    }).catch((data, status, headers, config) => {
                        toastr.error(data.exceptionMessage, data.message);
                    });
            }
        };

        this.carregarTelaGraficoDiaImprodutivo = () => {
            if (StoreService.isData()) this.filter = StoreService.getData();
            else this.setDate();

            if (!moment(this.filter.periodoIni).isValid() ||
                !moment(this.filter.periodoFim).isValid() ||
                moment(this.filter.periodoFim).isBefore(moment(this.filter.periodoIni))) {

                $location.path('/chart');
                toastr.error('Dados incorretos', 'Período fornecido inválido.');
            } else {
                Grafico.getDiaImprodutivo(this.filter).then((data, status, headers, config) => {
                    $scope.diasImprodutivos = data;
                    $scope.chart = ConstruirHighChartDiaImprodutivo(data);

                    if ($scope.chart.isEmpty) {
                        $location.path('/chart');
                        toastr.info(
                            'Não existem dados cadastrados para que o gráfico da categoria de Dias Improdutivos seja gerado.');
                    } else {
                        GerarGraficoBar('container', $scope.chart);
                    }
                }).catch((data, status, headers, config) => {
                    toastr.error(data.exceptionMessage, data.message);
                });
            }
        };

        this.carregarTelaGraficoTarefa = () => {
            if (StoreService.isData()) this.filter = StoreService.getData();
            else this.setDate();

            if (!moment(this.filter.periodoIni).isValid() ||
                !moment(this.filter.periodoFim).isValid() ||
                moment(this.filter.periodoFim).isBefore(moment(this.filter.periodoIni))) {

                $location.path('/chart');
                toastr.error('Dados incorretos', 'Período fornecido inválido.');
            } else {
                Grafico.getTarefa(this.filter)
                    .then((data, status, headers, config) => {
                        $scope.tarefas = data;

                        Grafico.getStatusTarefa(this.filter)
                            .then((data, status, headers, config) => {
                                $scope.trs = buildTableTarefa($scope.tarefas, data);
                                $scope.chart = ConstruirHighChartTarefa($scope.tarefas, data);

                                if ($scope.chart.isEmpty) {
                                    $location.path('/chart');
                                    toastr.info('Não existem dados cadastrados para que o gráfico da categoria de Tarefas seja gerado.');
                                } else GerarGraficoBar('container', $scope.chart);
                            }).catch((data, status, headers, config) => {
                                toastr.error(data.exceptionMessage, data.message);
                            });
                    }).catch((data, status, headers, config) => {
                        toastr.error(data.exceptionMessage, data.message);
                    });
            }
        };

        this.carregarTelaGraficoComentario = () => {
            if (StoreService.isData()) this.filter = StoreService.getData();
            else this.setDate();

            if (!moment(this.filter.periodoIni).isValid() ||
                !moment(this.filter.periodoFim).isValid() ||
                moment(this.filter.periodoFim).isBefore(moment(this.filter.periodoIni))) {

                $location.path('/chart');
                toastr.error('Dados incorretos', 'Período fornecido inválido.');
            } else {
                Grafico.getComentario(this.filter)
                    .then((data, status, headers, config) => {
                        $scope.comentarios = data;
                        console.log("comentarios", data);
                        $scope.chart = ConstruirHighchartComentario(data);
                        console.log($scope.chart);
                        if ($scope.chart.isEmpty) {
                            $location.path('/chart');
                            toastr.info(
                                'Não existem dados cadastrados para que o gráfico da categoria de Comentários seja gerado.');
                        } else {
                            GerarGraficoBar('container', $scope.chart);
                        }
                    }).catch(function(data, status, headers, config) {
                        toastr.error(data.exceptionMessage, data.message);
                    });
            }
        };

        this.rdo = () => {
            if ($scope.filter.periodoIni === '' || $scope.filter.periodoFim === '') {
                this.setDate();

                StoreService.setData(this.filter);

                $location.path('/chart/rdos');

                return;
            }

            if (!Validacao.data($scope.filter.periodoIni)) {
                toastr.error("O período inicial é inválido.");
                return;
            }

            if (!Validacao.data($scope.filter.periodoFim)) {
                toastr.error("O período final é inválido.");
                return;
            }

            $scope.periodoIni = Format.mask($scope.filter.periodoIni);
            $scope.periodoFim = Format.mask($scope.filter.periodoFim);

            if (!moment($scope.periodoIni).isValid() ||
                !moment($scope.periodoFim).isValid() ||
                moment($scope.periodoFim).isBefore(moment($scope.periodoIni))) toastr.error('Dados incorretos', 'Período fornecido inválido.');
            else if (!isValid(moment($scope.periodoIni).format()) || !isValid(moment($scope.periodoFim).format())) toastr.error('Dados incorretos', 'Período fornecido inválido.');
            else {
                this.filter.periodoIni = moment($scope.periodoIni).format('YYYY-MM-DD');
                this.filter.periodoFim = moment($scope.periodoFim).format('YYYY-MM-DD');

                StoreService.setData(this.filter);

                $location.path('/chart/rdos');
            }
        };

        this.diaImprodutivo = () => {
            if ($scope.filter.periodoIni === '' || $scope.filter.periodoFim === '') {
                this.setDate();

                StoreService.setData(this.filter);

                $location.path('/chart/diaimprodutivo');

                return;
            }

            if (!Validacao.data($scope.filter.periodoIni)) {
                toastr.error("O período inicial é inválido.");
                return;
            }

            if (!Validacao.data($scope.filter.periodoFim)) {
                toastr.error("O período final é inválido.");
                return;
            }

            $scope.periodoIni = Format.mask($scope.filter.periodoIni);
            $scope.periodoFim = Format.mask($scope.filter.periodoFim);

            if (!moment($scope.periodoIni).isValid() ||
                !moment($scope.periodoFim).isValid() ||
                moment($scope.periodoFim).isBefore(moment($scope.periodoIni))) toastr.error('Dados incorretos', 'Período fornecido inválido.');
            else if (!isValid(moment($scope.periodoIni).format()) ||
                !isValid(moment($scope.periodoFim).format()))
                toastr.error('Dados incorretos', 'Período fornecido inválido.');
            else {
                this.filter.periodoIni = moment($scope.periodoIni).format('YYYY-MM-DD');
                this.filter.periodoFim = moment($scope.periodoFim).format('YYYY-MM-DD');

                StoreService.setData(this.filter);

                $location.path('/chart/diaimprodutivo');
            }
        };

        this.buildRdosAtrasados = (data) => {
            $scope.atrasados = [];
            data.forEach(d => {
                let aux = {
                    obr_ds_obra: d.obr_ds_obra,
                    qtde: d.dias_atraso
                };

                $scope.atrasados.push(aux);
            });
        };

        this.buildDiasImprodutivos = (data) => {
            $scope.diasImprodutivos = [];
            data.forEach(d => {
                let aux = {
                    obr_ds_obra: d.obr_ds_obra,
                    total: (d.imp_clima +
                        d.imp_falta_material +
                        d.imp_paralizacao +
                        d.imp_acidente +
                        d.imp_contratante +
                        d.imp_equipamento +
                        d.imp_fornecedores +
                        d.imp_maodeobra +
                        d.imp_projeto +
                        d.imp_planejamento)
                };
                $scope.diasImprodutivos.push(aux);
            });
        };

        this.buildComentarios = (data) => {
            $scope.comments = [];
            data.forEach(d => {
                let aux = {
                    obr_ds_obra: d.obr_ds_obra,
                    negativoContratada: d.negativo_contratada,
                    negativoContratante: d.negativo_contratante
                };
                $scope.comments.push(aux);
            });
        };

        this.buildTarefasComInicioEmAtraso = (data) => {
            $scope.tarefasInicioEmAtraso = [];
            data.forEach((d) => {
                let aux = {
                    obr_ds_obra: d.obr_ds_obra,
                    total: d.atraso_inicio

                };
                $scope.tarefasInicioEmAtraso.push(aux);
            });
        };

        this.tarefa = () => {
            if ($scope.filter.periodoIni === '' || $scope.filter.periodoFim === '') {
                this.setDate();

                StoreService.setData(this.filter);

                $location.path('/chart/tarefa');

                return;
            }

            if (!Validacao.data($scope.filter.periodoIni)) {
                toastr.error("O período inicial é inválido.");
                return;
            }

            if (!Validacao.data($scope.filter.periodoFim)) {
                toastr.error("O período final é inválido.");
                return;
            }

            $scope.periodoIni = Format.mask($scope.filter.periodoIni);
            $scope.periodoFim = Format.mask($scope.filter.periodoFim);

            if (!moment($scope.periodoIni).isValid() ||
                !moment($scope.periodoFim).isValid() ||
                moment($scope.periodoFim).isBefore(moment($scope.periodoIni))) toastr.error('Dados incorretos', 'Período fornecido inválido.');
            else if (!isValid(moment($scope.periodoIni).format()) ||
                !isValid(moment($scope.periodoFim).format()))
                toastr.error('Dados incorretos', 'Período fornecido inválido.');
            else {
                this.filter.periodoIni = moment($scope.periodoIni).format('YYYY-MM-DD');
                this.filter.periodoFim = moment($scope.periodoFim).format('YYYY-MM-DD');

                StoreService.setData(this.filter);

                $location.path('/chart/tarefa');
            }
        };

        this.comentario = () => {
            if ($scope.filter.periodoIni === '' || $scope.filter.periodoFim === '') {
                this.setDate();

                StoreService.setData(this.filter);

                $location.path('/chart/comentario');

                return;
            }

            if (!Validacao.data($scope.filter.periodoIni)) {
                toastr.error("O período inicial é inválido.");
                return;
            }

            if (!Validacao.data($scope.filter.periodoFim)) {
                toastr.error("O período final é inválido.");
                return;
            }

            $scope.periodoIni = Format.mask($scope.filter.periodoIni);
            $scope.periodoFim = Format.mask($scope.filter.periodoFim);

            if (!moment($scope.periodoIni).isValid() ||
                !moment($scope.periodoFim).isValid() ||
                moment($scope.periodoFim).isBefore(moment($scope.periodoIni))) toastr.error('Dados incorretos', 'Período fornecido inválido.');
            else if (!isValid(moment($scope.periodoIni, 'yyyy-MM-dd').format()) ||
                !isValid(moment($scope.periodoFim, 'yyyy-MM-dd').format()))
                toastr.error('Dados incorretos', 'Período fornecido inválido.');
            else {
                this.filter.periodoIni = moment($scope.periodoIni).format('YYYY-MM-DD');
                this.filter.periodoFim = moment($scope.periodoFim).format('YYYY-MM-DD');

                StoreService.setData(this.filter);

                $location.path('/chart/comentario');
            }
        };

        this.setDate = () => {
            if (this.filter.periodoIni === '' || this.filter.periodoFim === '') {
                this.filter.periodoIni = moment().subtract(1, 'months').format('YYYY-MM-DD');
                this.filter.periodoFim = moment().format('YYYY-MM-DD');
            }
        };
    };
})();