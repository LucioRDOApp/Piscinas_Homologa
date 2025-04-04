(function () {
    'use strict';
    angular.module('app').controller('EmpresaController', EmpresaController);
    EmpresaController.$inject = ['$http', '$location', 'ViewBag', 'Auth', 'Validacao'];
    function EmpresaController($http, $location, ViewBag, Auth, Validacao) {
        var controller = this;

        this.pagedlist = {};
        this.empresa = [];
        this.menus = [];
        this.filtroParam = {
            idEmpresa: '', nomeFantasia: '', razaoSocial: '', idMunicipio: 0,
            idUf: 0, idRamo: 0, idSetor: 0, cnpj: '', idDono: '', logradouro: '', idColaborador: '',
            numeroEndereco: '', bairro: '', cep: '', complemento: '', telefone: ''
        };
        //this.statusEmpresa = [{ id: 'S', nome: 'Selecione...' }, { id: 'A', nome: 'Aluguel' }, { id: 'C', nome: 'Compra' }, ];
        this.ramos = [];
        this.setores = [];

        this.cadastroParam = {
            idEmpresa: '', nomeFantasia: '', razaoSocial: '', idMunicipio: 0, idColaborador: '',
            idUf: 0, idRamo: 0, idSetor: 0, cnpj: '', idDono: '', logradouro: '',
            numeroEndereco: '', bairro: '', cep: '', complemento: '', telefone: ''
        };
        this.orderby = '';
        this.orderbydescending = '';
        this.selectedMenu = 1;
        this.empresa = {};
        this.desabilitarCampos = false;


        this.carregaListaMunicipio = function (tipoTela, selected) {
            var postdata;

            if (tipoTela == 'listagem') {
                postdata = controller.filtroParam.idUf;
            }
            else {
                postdata = controller.cadastroParam.idUf;
            }

            console.log(postdata);

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

        this.carregarLista = function (page, order) {
            controller.filtroParam.page = page;
            controller.filtroParam.idColaborador = Auth.getUser().usuario.id;
            console.log(Auth.getUser().usuario.id);
            console.log(Auth.getUser());


            controller.filtroParam.idObra = Auth.getUser().obra.idObra;
            controller.filtroParam.contratanteContratada = Auth.getUser().obraColaborador.contratanteContratada;



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
                url: "api/empresa/CarregarLista",
                method: "POST",
                data: this.filtroParam
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.pagedlist = data;
            });
        }

        this.carregarEdicao = function () {
            var id = ViewBag.get('empresaId');

            if (id != "undefined" && id > 0) {
                $http({
                    url: "api/empresa/ObterEmpresa/",
                    method: "POST",
                    data: id
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, data.message)
                }).success(function (data, status, headers, config) {
                    controller.cadastroParam = data;
                    controller.carregaListaMunicipio('cadastro', data.idMunicipio);
                    var enebled = ViewBag.get('statusTela');

                    controller.desabilitarCampos = ViewBag.get('desabilitarCamposEmpresa');

                    controller.controlEnabled(enebled);
                });
            }
            else {
                console.log("id = 0!!!");
            }
        }

        this.salvar = function () {
            if (Validacao.required(controller.cadastroParam.razaoSocial)) {
                toastr.error("A Razão Social deve ser preenchida");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.razaoSocial)) {
                toastr.error("Por favor, introduza pelo menos três caracteres na Razão Social");
                return;
            }
            if (!Validacao.cnpj(controller.cadastroParam.cnpj)) {
                toastr.error("CNPJ Inválido");
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
            if (Validacao.required(controller.cadastroParam.idRamo)) {
                toastr.error("O Ramo Atividade deve ser preenchido");
                return;
            }
            if (Validacao.required(controller.cadastroParam.idSetor)) {
                toastr.error("O Setor deve ser preenchido");
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
            if (!Validacao.minLenght(controller.cadastroParam.complemento) && controller.cadastroParam.complemento != "") {
                toastr.error("Por favor, introduza pelo menos três caracteres no Complemento");
                return;
            }
            if (!Validacao.minLenght(controller.cadastroParam.bairro) && controller.cadastroParam.bairro != "") {
                toastr.error("Por favor, introduza pelo menos três caracteres no Bairro");
                return;
            }
      
            if (Validacao.required(controller.cadastroParam.idUf)) {
                toastr.error("A UF deve ser preenchida");
                return;
            }
            if (Validacao.required(controller.cadastroParam.idMunicipio)) {
                toastr.error("O Munícipio deve ser preenchido");
                return;
            }
            if (Validacao.required(controller.cadastroParam.telefone)) {
                toastr.error("O Telefone deve ser preenchido");
                return;
            }
        
            if (window.validate("#form-cadastro-empresa")) {
                var user = Auth.getUser();
                controller.cadastroParam.idColaborador = user.usuario.id;


                $http({
                    url: "api/empresa/Salvar",
                    method: "POST",
                    data: this.cadastroParam
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, "Erro.")
                }).success(function (data, status, headers, config) {
                    controller.lista();
                    toastr.success("Registro salvo com sucesso.");
                });
            }
        }


        this.deletar = function (empresa) {
            //var user = Auth.getUser();
            //empresa.idColaborador = user.usuario.id;

            //MensagemConfirmacao("Deseja realmente excluir esse registro?", function () {
            MensagemConfirmacao("Tem certeza que deseja excluir o registro: " + empresa.nomeFantasia + "?", function () {
                $http({
                    url: "api/empresa/Deletar",
                    method: "POST",
                    data: empresa
                }).error(function (data, status, headers, config) {
                    toastr.error(data.exceptionMessage, "Erro")
                }).success(function (data, status, headers, config) {
                    controller.carregarLista(controller.pagedlist.currentPage);
                    toastr.success("Registro excluído com sucesso.");
                });

            });
        }

        this.editar = function (empresa) {
            var user = Auth.getUser();
            controller.cadastroParam.idObra = user.obra.idObra;
            empresa.idObra = user.obra.idObra;
            ViewBag.set('empresaIdObra', user.obra.idObra);

            ViewBag.set('desabilitarCamposEmpresa', false);

            ViewBag.set('statusTela', 'editar');
            ViewBag.set('empresaId', empresa.idEmpresa);
            $location.path('/empresa/cadastro');
        }

        this.visualizar = function (empresa) {
            var user = Auth.getUser();
            controller.cadastroParam.idObra = user.obra.idObra;
            empresa.idObra = user.obra.idObra;
            ViewBag.set('empresaIdObra', user.obra.idObra);
            // limpar o que não precisa

            ViewBag.set('desabilitarCamposEmpresa', true);

            ViewBag.set('statusTela', 'visualizar');
            ViewBag.set('empresaId', empresa.idEmpresa);
            $location.path('/empresa/cadastro');
        }

        this.novo = function () {
            ViewBag.set('empresaId', undefined);
            $location.path('/empresa/cadastro');
        }

        this.lista = function () {
            $location.path('/empresa/index');
        }

        this.voltar = function () {
            $location.path('/empresa/index');
        }

        this.controlEnabled = function (enabled) {
            if (enabled == 'editar') {
                $('.txbDescricaoEmpresa').removeAttr('disabled');
                $('.txbDataInicialPlanejada').removeAttr('disabled');
                $('.txbDataFinalPlanejada').removeAttr('disabled');
                $('.ddlStatus').removeAttr('disabled');

                $('.txbHorasTrabalhadas').removeAttr('disabled');
                $('.txbQtdConstruida').removeAttr('disabled');
                $('.ddlUnidadeMedida').removeAttr('disabled');
                $('.txbComentario').removeAttr('disabled');
            }
            else {
                $('.txbDescricaoEmpresa').attr('disabled', 'disabled');
                $('.txbDataInicialPlanejada').attr('disabled', 'disabled');
                $('.txbDataFinalPlanejada').attr('disabled', 'disabled');
                $('.ddlStatus').attr('disabled', 'disabled');

                $('.txbHorasTrabalhadas').attr('disabled', 'disabled');
                $('.txbQtdConstruida').attr('disabled', 'disabled');
                $('.ddlUnidadeMedida').attr('disabled', 'disabled');
                $('.txbComentario').attr('disabled', 'disabled');
            }
        }

        this.maskPhone = function (phone) {
            try {
                var x = phone.replace(/\D/g, '').match(/(\d{0,2})(\d{0,5})(\d{0,4})/);
                var p = '(' + x[1] + ') ' + x[2] + '-' + x[3];
                return p;
            }
            catch (ex) {
                return phone;
            }
        }

        this.carregarRamos = function () {
            $http({
                url: "api/ramo/Lista/",
                method: "GET",
                data: undefined
            }).error(function (data, status, headers, config) {
                toastr.error(data.exceptionMessage, data.message)
            }).success(function (data, status, headers, config) {
                controller.ramos = data;
                controller.ramos.unshift({ id: 0, nome: 'Selecione...' });
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


        //this.carregarUnidadeMedida = function () {
        //    $http({
        //        url: "/api/unidadeMedida/Lista/",
        //        method: "POST",
        //        data: undefined
        //    }).error(function (data, status, headers, config) {
        //        toastr.error(data.exceptionMessage, data.message)
        //    }).success(function (data, status, headers, config) {
        //        controller.unidadeMedida = data;
        //        controller.unidadeMedida.unshift({ id: 0, nome: 'Selecione...' });
        //    });
        //}

        //this.cards = function () {
        //    $location.path('/empresa/cards');
        //}

        //this.getCurrentDate = function () {
        //    var today = new Date();
        //    var dd = today.getDate();
        //    var mm = today.getMonth() + 1;
        //    var yyyy = today.getFullYear();

        //    if (dd < 10) {
        //        dd = '0' + dd
        //    }

        //    if (mm < 10) {
        //        mm = '0' + mm
        //    }

        //    today = dd + '/' + mm + '/' + yyyy;
        //    controller.cadastroParam.dataInicio = today;
        //}

        //this.changeStatus = function (empresa, idStatus) {
        //    var filterObj = { empresa: empresa, idStatus: idStatus };

        //    $http({
        //        url: "/api/empresa/AtualizarStatus",
        //        method: "POST",
        //        data: filterObj
        //    }).error(function (data, status, headers, config) {
        //        toastr.error(data.exceptionMessage, "Errrrrrrou")
        //    }).success(function (data, status, headers, config) {
        //        controller.cards();
        //        toastr.success("Sucesso", "Cadastro realizado com sucesso.");
        //    });

        //}


    }
})();

