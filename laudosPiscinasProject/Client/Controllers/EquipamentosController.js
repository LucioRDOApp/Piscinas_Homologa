(function () {
    'use strict';
    angular.module('app').controller('EquipamentosController', EquipamentosController);
    EquipamentosController.$inject = ['$http', '$location', 'ViewBag', 'Auth', '$scope', '$timeout', '$q', '$log', 'Validacao'];
    function EquipamentosController($http, $location, ViewBag, Auth, $scope, $timeout, $q, $log, Validacao) {
        var controller = this;

        this.pagedlist = {};
        this.equipamentos = [];
        this.filtroParam = { Id: '', tipoEquipamento:'', descricao: '', marca: '', modelo: '', tipoAquisicao: 'S', fabricanteFornecedor: '', dataAquisicao: '', contato: '', telefone: '', foto: '', idObra: '' };
        this.tipoAquisicao = [{ id: 'S', nome: 'Selecione...' }, { id: 'A', nome: 'Aluguel' }, { id: 'C', nome: 'Compra' }, ];

        this.cadastroParam = {
            Id: '', descricao: '', marca: '', modelo: '', tipoAquisicao: 'S', descricaoFoto: '', tipoEquipamento: '', IdObraEquipamento: 0, DescricaoTipoAquisicao: '', IdTipoEquipamento: 0, NomeTipoEquipamento: '',
            fabricanteFornecedor: '', dataAquisicao: '', contato: '', telefone: '', foto: '', idObra: ''
        };
        this.orderby = '';
        this.orderbydescending = '';
        //this.selectedMenu = 1;
        this.equipamento = {};
        this.desabilitarCampos = false;

        this.carregarLista = function (page, order) {
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
                url: "api/equipamentos/CarregarLista",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.pagedlist = data;
            });
        }

        this.carregarEdicao = function () {
            var user = Auth.getUser();
            var idObra = user.obra.idObra;
            var id = ViewBag.get('equipamentoId');

            var postData = { id: id, idObra: idObra };

            if (id != "undefined" && id > 0) {
                $http({
                    url: "api/equipamentos/ObterEquipamento/",
                    method: "POST",
                    data: postData
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    controller.cadastroParam = data;
                    controller.selectedItemMarca = controller.cadastroParam.marca;
                    controller.selectedItemModelo = controller.cadastroParam.modelo;
                    controller.selectedItemTipoEquipamento = controller.cadastroParam.tipoEquipamento;
                    controller.desabilitarCampos = ViewBag.get('desabilitarCamposEquipamento');
                });
            }
            else {
                //console.log("cadastrar euipamento");
            }
        }

        this.salvar = function () {

            if (Validacao.required(controller.buscaTextoTipoEquipamento)) {
                toastr.error("O Tipo de equipamento deve ser preenchido.")
                return;
            }
            if (Validacao.required(controller.cadastroParam.descricao)) {
                toastr.error("A Descrição deve ser preenchida.")
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.descricao, 2)) {
                toastr.error("Por favor, introduza pelo menos dois caracteres na Descrição.");
                return;
            }
            //if (Validacao.required(controller.buscaTextoMarca)) {
            //    toastr.error("A Marca deve ser preenchida");
            //    return;
            //}
            //if (Validacao.required(controller.buscaTextoModelo)) {
            //    toastr.error("O Modelo deve ser preenchido");
            //    return;
            //}
            //if (Validacao.aquisicaoVal(controller.cadastroParam.tipoAquisicao)) {
            //    toastr.error("O Tipo de Aquisição deve ser preenchido");
            //    return;
            //}
            //if (Validacao.required(controller.cadastroParam.dataAquisicao)) {
            //    toastr.error("A Data de Aquisição deve ser preenchida.");
            //    return;
            //}
            if (controller.cadastroParam.dataAquisicao != undefined && controller.cadastroParam.dataAquisicao.length > 0 && !Validacao.data(controller.cadastroParam.dataAquisicao)) {
                toastr.error("A Data de Aquisição é inválida.");
                return;
            }
            //if (Validacao.required(controller.cadastroParam.fabricanteFornecedor)) {
            //    toastr.error("O Fornecedor/Fabricante deve ser preenchido");
            //    return;
            //}
            //if (Validacao.required(controller.cadastroParam.contato)) {
            //    toastr.error("O Contato deve ser preenchido");
            //    return;
            //}

            if (!Validacao.required(controller.cadastroParam.telefone) && controller.cadastroParam.telefone.replace(/_/g, '').length < 14) {
                toastr.error("O Telefone é inválido.");
                return;
            }
            
            if (window.validate("#form-cadastro-equipamento")) {
                var user = Auth.getUser();
                controller.cadastroParam.idObra = user.obra.idObra;

                controller.cadastroParam.marca = controller.buscaTextoMarca;
                controller.cadastroParam.modelo = controller.buscaTextoModelo;
                controller.cadastroParam.tipoEquipamento = controller.buscaTextoTipoEquipamento;

                controller.cadastroParam.dataAquisicao = $('.dataAquisicao').val();

                if ($scope.imageFileEquipamento != undefined) {
                    controller.cadastroParam.foto = $scope.imageFileEquipamento;
                }
                //else {
                //    controller.cadastroParam.foto = '';
                //}                   
                
                
                $http({
                    url: "api/equipamentos/Salvar",
                    method: "POST",
                    data: this.cadastroParam
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, "Erro.")
                }).success(function (data, status, headers, config) {
                    if (data != undefined) {
                        toastr.success("Registro salvo com sucesso.");
                    }
                    else {
                        toastr.error("Não foi possível realizar as atualizações.")
                    }
                    controller.voltar();
                });
            }
        }

        this.deletar = function (equipamento) {
            var user = Auth.getUser();
            equipamento.idObra = user.obra.idObra;

            MensagemConfirmacao("Tem certeza que deseja excluir o registro: " + equipamento.descricao + "?", function () {
                $http({
                    url: "api/equipamentos/Deletar",
                    method: "POST",
                    data: equipamento
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage)
                }).success(function (data, status, headers, config) {
                    controller.carregarLista(controller.pagedlist.currentPage);
                    toastr.success("Registro excluído com sucesso.");
                });

            });
        }

        this.editar = function (equipamento) {
            var user = Auth.getUser();
            controller.cadastroParam.idObra = user.obra.idObra;
            equipamento.idObra = user.obra.idObra;
            ViewBag.set('equipamentoIdObra', user.obra.idObra);

            ViewBag.set('desabilitarCamposEquipamento', false);            

            ViewBag.set('equipamentoId', equipamento.id);
            $location.path('/equipamentos/cadastro');
        }

        this.visualizar = function (equipamento) {
            var user = Auth.getUser();
            controller.cadastroParam.idObra = user.obra.idObra;
            equipamento.idObra = user.obra.idObra;
            ViewBag.set('equipamentoIdObra', user.obra.idObra);

            ViewBag.set('desabilitarCamposEquipamento', true);

            ViewBag.set('equipamentoId', equipamento.id);
            $location.path('/equipamentos/cadastro');
        }

        this.novo = function () {
            ViewBag.set('equipamentoId', undefined)
            $location.path('/equipamentos/cadastro');
        }

        this.voltar = function () {
            $location.path('/equipamentos/index');
        }

        //this.lista = function () {
        //    $location.path('/equipamentos/index');
        //}

        //####### Modelo #############

        controller.modelos = carregaTodosModelos();

        controller.buscaModelo = buscaModelo;

        function buscaModelo(query) {
            return query ? controller.modelos.filter(createFilterFor(query)) : controller.modelos;
        }

        function carregaTodosModelos() {
            $http({
                url: "api/equipamentos/ListaModelo",
                method: "POST"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage)
            }).success(function (data, status, headers, config) {
                controller.modelos = data.split(/,+/g).map(function (modelo) {
                    return {
                        value: modelo.toLowerCase(),
                        display: modelo
                    };
                });
            });
        }

        //###################################

        //####### Marca #############

        controller.marcas = carregaTodasMarcas();

        controller.buscaMarca = buscaMarca;

        function buscaMarca(query) {
            return query ? controller.marcas.filter(createFilterFor(query)) : controller.marcas;
        }

        function carregaTodasMarcas() {
            $http({
                url: "api/equipamentos/ListaMarca",
                method: "POST"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage)
            }).success(function (data, status, headers, config) {
                controller.marcas = data.split(/,+/g).map(function (marca) {
                    return {
                        value: marca.toLowerCase(),
                        display: marca
                    };
                });
            });
        }

        //###################################

        //####### Tipo Equipamento #############

        controller.tiposEquipamento = carregaTodosTiposEquipamento();

        controller.buscaTipoEquipamento = buscaTipoEquipamento;

        function buscaTipoEquipamento(query) {
            return query ? controller.tiposEquipamento.filter(createFilterFor(query)) : controller.tiposEquipamento;
        }

        function carregaTodosTiposEquipamento() {
            $http({
                url: "api/equipamentos/ListaTipoEquipamento",
                method: "POST"
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage)
            }).success(function (data, status, headers, config) {
                controller.tiposEquipamento = data.split(/,+/g).map(function (tipoEquipamento) {
                    return {
                        value: tipoEquipamento.toLowerCase(),
                        display: tipoEquipamento
                    };
                });
            });
        }

        //###################################

        //####### Equipamnto #############

        //controller.descricoesEquipamentos = carregarDescricaoEquipamentos();

        //controller.buscaDescricaoEquipamento = buscaDescricoesEquipamentos;

        //function buscaDescricoesEquipamentos(query) {
        //    return query ? controller.descricoesEquipamentos.filter(createFilterFor(query)) : controller.descricoesEquipamentos;
        //}

        //function carregarDescricaoEquipamentos() {
        //    $http({
        //        url: "api/equipamentos/ListaMarca",
        //        method: "POST"
        //    }).error(function (data, status, headers, config) {
        //        toastr.error(data.exceptionMessage, "Erro")
        //    }).success(function (data, status, headers, config) {
        //        controller.descricoesEquipamentos = data.split(/,+/g).map(function (marca) {
        //            return {
        //                value: marca.toLowerCase(),
        //                display: marca
        //            };
        //        });
        //    });
        //}

        //###################################



        function createFilterFor(query) {
            var lowercaseQuery = angular.lowercase(query);

            return function filterFn(state) {
                return (state.value.indexOf(lowercaseQuery) === 0);
            };
        }
    }
})();
