using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace laudosPiscinasProject.Api.Models
{
    public class EmpresaModel
    {
        internal static List<EmpresaViewModel> ListaEmpresa(dynamic param)
        {
            int idColaborador = param.idColaborador ?? 0;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<EmpresaViewModel> ListaEmpresa = new List<EmpresaViewModel>();
            List<empresa> ListaEmpresaAux = new List<empresa>();

            colaborador col = context.colaborador.ToList().FirstOrDefault(x => x.col_id_colaborador == idColaborador);
            ListaEmpresaAux = col.empresa.ToList();


            if (param.novaObra != null && !(bool)param.novaObra)
            {
                if (param.idObra != null)
                {
                    int idObra = (int)param.idObra;
                    obra obr = context.obra.FirstOrDefault(x => x.obr_id_obra == idObra);
                    ListaEmpresaAux = new List<empresa>();

                    if (obr.empresa != null)
                    {
                        ListaEmpresaAux.Add(obr.empresa);
                    }
                    if (obr.empresa1 != null && !ListaEmpresaAux.Any(x => x.emp_id_empresa == obr.empresa1.emp_id_empresa))
                    {
                        ListaEmpresaAux.Add(obr.empresa1);
                    }
                    if (obr.empresa2 != null && !ListaEmpresaAux.Any(x => x.emp_id_empresa == obr.empresa2.emp_id_empresa))
                    {
                        ListaEmpresaAux.Add(obr.empresa2);
                    }
                }
                else
                {
                    ListaEmpresaAux = context.empresa.Where(x => x.emp_id_colaborador == idColaborador).ToList();
                }
            }
            else
            {
                ListaEmpresaAux = context.empresa.Where(x => x.emp_id_colaborador == idColaborador).ToList();
            }


            string razaoSocial = param.razaoSocial == null ? "" : param.razaoSocial.ToString();
            string cnpj = param.cnpj == null ? "" : param.cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Replace("_", "").ToString();
            int idUf = param.idUf ?? 0;
            int idMunicipio = param.idMunicipio ?? 0;
            int idRamo = param.idRamo ?? 0;
            int idSetor = param.idSetor ?? 0;


            if (!string.IsNullOrEmpty(razaoSocial))
            {
                ListaEmpresaAux = ListaEmpresaAux.Where(emp => emp.emp_ds_razao_social.ToLower().Contains(razaoSocial.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(cnpj))
            {
                ListaEmpresaAux = ListaEmpresaAux.Where(emp => emp.emp_nr_cnpj == cnpj).ToList();
            }
            if (idUf > 0)
            {
                ListaEmpresaAux = ListaEmpresaAux.Where(emp => emp.municipio.mun_id_uf == idUf).ToList();
            }
            if (param.idMunicipio > 0)
            {
                ListaEmpresaAux = ListaEmpresaAux.Where(emp => emp.emp_id_municipio == idMunicipio).ToList();
            }
            if (param.idRamo > 0)
            {
                ListaEmpresaAux = ListaEmpresaAux.Where(emp => emp.emp_id_ramo == idRamo).ToList();
            }
            if (param.idSetor > 0)
            {
                ListaEmpresaAux = ListaEmpresaAux.Where(emp => emp.emp_id_setor == idSetor).ToList();
            }



            ListaEmpresaAux.ToList().ForEach(emp =>
            ListaEmpresa.Add(new EmpresaViewModel
            {
                idEmpresa = emp.emp_id_empresa,
                nomeFantasia = emp.emp_nm_fantasia,
                IdLicenca = emp.emp_id_licenca ?? 0,
                DescricaoLicenca = emp.licenca == null ? "" : emp.licenca.lic_ds_licenca
            }));

            return ListaEmpresa;
        }

        public static bool VerificarPermissaoLicencaEmpresa(dynamic param)
        {
            int idEmpresa = param == null ? 0 : (int)param;

            bool result = false;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            empresa emp = context.empresa.FirstOrDefault(x => x.emp_id_empresa == idEmpresa);
            int numeroObrasEmpresa = context.obra.Where(x => x.obr_id_dono == emp.emp_id_empresa).Count();
            int numeroObrasLicenca = emp.licenca == null ? 0 : emp.licenca.lic_nr_qtd_obras ?? 0;

            result = numeroObrasEmpresa >= numeroObrasLicenca;
            return !result;
        }

        internal static EmpresaViewModel ObterRegistroCNPJ(dynamic param)
        {
            string cnpj = param.cnpj ?? string.Empty;

            EmpresaViewModel empresa = new EmpresaViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var resultado = context.empresa.FirstOrDefault(emp => emp.emp_nr_cnpj == cnpj);

            if (resultado == null)
            {
                return empresa;
            }

            empresa.idEmpresa = resultado.emp_id_empresa;
            empresa.nomeFantasia = resultado.emp_nm_fantasia;
            empresa.RazaoSocial = resultado.emp_ds_razao_social;
            empresa.Cnpj = resultado.emp_nr_cnpj;
            empresa.idMunicipio = resultado.emp_id_municipio ?? 0;
            empresa.idRamo = resultado.emp_id_ramo ?? 0;
            empresa.idSetor = resultado.emp_id_setor ?? 0;
            empresa.idDono = resultado.emp_id_colaborador;
            empresa.Logradouro = resultado.emp_ds_logradouro;
            empresa.NumeroEndereco = resultado.emp_ds_numero;
            empresa.Bairro = resultado.emp_ds_bairro;
            empresa.Complemento = resultado.emp_ds_complemento;
            empresa.Cep = resultado.emp_ds_cep;
            empresa.idMunicipio = resultado.emp_id_municipio ?? 0;
            empresa.idUf = resultado.municipio == null ? 0 : resultado.municipio.mun_id_uf;
            empresa.Telefone = resultado.emp_ds_telefone;
            empresa.IdLicenca = resultado.emp_id_licenca ?? 0;
            empresa.DescricaoLicenca = resultado.licenca == null ? "" : resultado.licenca.lic_ds_licenca;

            return empresa;
        }

        internal static EmpresaViewModel ObterRegistro(dynamic param)
        {
            string id = Convert.ToString(param);
            int idEmpresa = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(id);


            EmpresaViewModel empresa = new EmpresaViewModel();

            if (idEmpresa == 0)
            {
                return empresa;
            }

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            empresa resultado = context.empresa.FirstOrDefault(emp => emp.emp_id_empresa == idEmpresa);

            empresa.idEmpresa = resultado.emp_id_empresa;
            empresa.nomeFantasia = resultado.emp_nm_fantasia;
            empresa.RazaoSocial = resultado.emp_ds_razao_social;
            empresa.Cnpj = resultado.emp_nr_cnpj;
            empresa.idMunicipio = resultado.emp_id_municipio ?? 0;
            empresa.idRamo = resultado.emp_id_ramo ?? 0;
            empresa.idSetor = resultado.emp_id_setor ?? 0;
            empresa.idDono = resultado.emp_id_colaborador;
            empresa.Logradouro = resultado.emp_ds_logradouro;
            empresa.NumeroEndereco = resultado.emp_ds_numero;
            empresa.Bairro = resultado.emp_ds_bairro;
            empresa.Complemento = resultado.emp_ds_complemento;
            empresa.Cep = resultado.emp_ds_cep;
            empresa.idMunicipio = resultado.emp_id_municipio ?? 0;
            empresa.idUf = resultado.municipio == null ? 0 : resultado.municipio.mun_id_uf;
            empresa.Telefone = resultado.emp_ds_telefone;
            empresa.IdLicenca = resultado.emp_id_licenca ?? 0;
            empresa.DescricaoLicenca = resultado.licenca == null ? "" : resultado.licenca.lic_ds_licenca;
            empresa.Token = resultado.emp_id_token;

            return empresa;
        }

        public static List<EmpresaViewModel> Lista(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            int idColaborador = param.idColaborador ?? 0;
            int idObra = param.idObra ?? 0;

            obra obr = context.obra.Find(idObra);

            List<empresa> ListaEmpresas = new List<empresa>();
            if (obr.empresa1 != null)
            {
                ListaEmpresas.Add(obr.empresa1);
            }
            if (obr.empresa2 != null)
            {
                ListaEmpresas.Add(obr.empresa2);
            }

            if (param != null)
            {
                string razaoSocial = param.razaoSocial.ToString();
                string cnpj = param.cnpj.ToString();
                int uf = param.idUf ?? 0;
                int municipio = param.idMunicipio ?? 0;
                int ramo = param.idRamo ?? 0;
                int setor = param.idSetor ?? 0;


                if (!string.IsNullOrEmpty(razaoSocial))
                {
                    ListaEmpresas = ListaEmpresas.Where(emp => emp.emp_ds_razao_social.ToLower().Contains(razaoSocial.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(cnpj))
                {
                    cnpj = !string.IsNullOrEmpty(cnpj) ? cnpj.Replace(".", "").Replace("/", "").Replace("-", "") : cnpj;
                    ListaEmpresas = ListaEmpresas.Where(emp => emp.emp_nr_cnpj == cnpj).ToList();
                }
                if (uf > 0)
                {
                    ListaEmpresas = ListaEmpresas.Where(emp => emp.municipio != null && emp.municipio.mun_id_uf == uf).ToList();
                }
                if (municipio > 0)
                {
                    ListaEmpresas = ListaEmpresas.Where(emp => emp.emp_id_municipio == municipio).ToList();
                }
                if (ramo > 0)
                {
                    ListaEmpresas = ListaEmpresas.Where(emp => emp.emp_id_ramo == ramo).ToList();
                }
                if (setor > 0)
                {
                    ListaEmpresas = ListaEmpresas.Where(emp => emp.emp_id_setor == setor).ToList();
                }

            }

            List<EmpresaViewModel> Lista = new List<EmpresaViewModel>();
            ListaEmpresas.ForEach(emp => Lista.Add(new EmpresaViewModel
            {
                idEmpresa = emp.emp_id_empresa,
                nomeFantasia = emp.emp_nm_fantasia,
                RazaoSocial = emp.emp_ds_razao_social,
                Cnpj = emp.emp_nr_cnpj,
                idDono = emp.emp_id_colaborador,
                idRamo = emp.emp_id_ramo ?? 0,
                idMunicipio = emp.emp_id_municipio ?? 0,
                idUf = emp.municipio == null ? 0 : emp.municipio.mun_id_uf,
                Cep = emp.emp_ds_bairro,
                Telefone = emp.emp_ds_telefone,
                Bairro = emp.emp_ds_bairro,
                Complemento = emp.emp_ds_complemento,
                idSetor = emp.emp_id_setor ?? 0,
                Logradouro = emp.emp_ds_logradouro,
                NumeroEndereco = emp.emp_ds_numero,
                Editavel = emp.emp_id_colaborador == idColaborador ? true : false,
                Local = MunicipioModel.ObterRegistro(emp.emp_id_municipio ?? 0).Municipio + "-" + MunicipioModel.ObterRegistro(emp.emp_id_municipio ?? 0).UF
            }));


            string orderby = param.orderby ?? "";
            string orderbydescending = param.orderbydescending ?? "";

            if (!string.IsNullOrEmpty(orderby))
            {
                return Lista.OrderBy(x => x.nomeFantasia).ToList();
            }
            if (!string.IsNullOrEmpty(orderbydescending))
            {
                return Lista.OrderByDescending(x => x.nomeFantasia).ToList();
            }

            return Lista;
        }



        public static bool AddEmpresaServico(dynamic param)
        {
            if (string.IsNullOrEmpty(param.razaoSocial.ToString()))
            {
                throw new Exception("A Razão Social da empresa deve ser preenchida");
            }
            if (string.IsNullOrEmpty(param.nomeFantasia.ToString()))
            {
                throw new Exception("O Nome Fantasia da empresa deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.cnpj.ToString()))
            {
                throw new Exception("O CNPJ da empresa deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.idColaborador.ToString()))
            {
                throw new Exception("A empresa deve possuir um cliente responsável");
            }
            if (string.IsNullOrEmpty(param.idRamo.ToString()))
            {
                throw new Exception("O ramo da empresa deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.idSetor.ToString()))
            {
                throw new Exception("O setor da empresa deve ser preenchido");
            }


            string idEmpresaString = Convert.ToString(param.idEmpresa);
            int idEmpresa = String.IsNullOrEmpty(idEmpresaString) ? 0 : Convert.ToInt32(idEmpresaString);

            int idColaborador = param.idColaborador ?? 0;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            empresa _empresa = context.empresa.Where(x => x.emp_id_empresa == idEmpresa).FirstOrDefault() ?? new empresa();

            string cnpj = param.cnpj;
            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            if (!string.IsNullOrEmpty(cnpj) && _empresa.emp_id_empresa == 0)
            {
                _empresa = context.empresa.Where(x => x.emp_nr_cnpj == cnpj).FirstOrDefault() ?? new empresa();
            }

            _empresa.emp_nm_fantasia = param.nomeFantasia;
            _empresa.emp_ds_razao_social = param.razaoSocial;
            _empresa.emp_nr_cnpj = cnpj;

            _empresa.emp_id_token = param.token;

            _empresa.emp_id_colaborador = param.idColaborador;
            _empresa.emp_id_licenca = param.idLicenca;
            _empresa.emp_id_ramo = param.idRamo;
            _empresa.emp_id_setor = param.idSetor;

            if (_empresa.emp_id_empresa > 0)
            {
                context.empresa.Attach(_empresa);
                context.Entry(_empresa).State = EntityState.Modified;
            }
            else
            {
                context.empresa.Add(_empresa);
            }

            bool result = context.SaveChanges() > 0;

            return result;
        }


        public static EmpresaViewModel Salvar(dynamic param, bool sobrescrever)
        {
            if (string.IsNullOrEmpty(param.razaoSocial.ToString()))
            {
                throw new Exception("A razão social deve ser preenchida");
            }
            if (string.IsNullOrEmpty(param.cnpj.ToString()))
            {
                throw new Exception("O cnpj deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.nomeFantasia.ToString()))
            {
                throw new Exception("O nome fantasia deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.idColaborador.ToString()))
            {
                throw new Exception("A empresa deve possuir um colaborador responsável");
            }


            string idEmpresaString = Convert.ToString(param.idEmpresa);
            int idEmpresa = String.IsNullOrEmpty(idEmpresaString) ? 0 : Convert.ToInt32(idEmpresaString);


            //int idEmpresa = param.idEmpresa ?? 0;
            int idColaborador = param.idColaborador ?? 0;
            string cnpj = param.cnpj;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            empresa _empresa = context.empresa.Where(x => x.emp_id_empresa == idEmpresa).FirstOrDefault() ?? new empresa();

            if (!string.IsNullOrEmpty(cnpj) && _empresa.emp_id_empresa == 0)
            {
                _empresa = context.empresa.Where(x => x.emp_nr_cnpj == cnpj).FirstOrDefault();
                if (_empresa != null && sobrescrever == false)
                {
                    return ObterRegistro(_empresa.emp_id_empresa);
                }
                _empresa = new empresa();
            }

            if (param.idLicenca == null)
            {
                throw new Exception("A licença não pode ser nula");
            }


            _empresa.emp_nm_fantasia = param.nomeFantasia;
            _empresa.emp_ds_razao_social = param.razaoSocial;
            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            _empresa.emp_nr_cnpj = cnpj;
            _empresa.emp_id_municipio = param.idMunicipio;
            _empresa.emp_id_ramo = param.idRamo > 0 ? param.idRamo : null;
            _empresa.emp_id_setor = param.idSetor > 0 ? param.idSetor : null;
            _empresa.emp_ds_logradouro = param.logradouro;
            _empresa.emp_ds_numero = param.numeroEndereco;
            _empresa.emp_ds_bairro = param.bairro;
            string cep = param.cep;
            cep = !string.IsNullOrEmpty(cep) ? cep.Replace("-", "").Replace(".", "").Replace("_", "") : cep;
            _empresa.emp_ds_cep = cep;
            string telefone = param.telefone;
            telefone = !string.IsNullOrEmpty(telefone) ? telefone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : telefone;
            _empresa.emp_ds_telefone = telefone;
            _empresa.emp_ds_complemento = param.complemento;
            _empresa.emp_id_colaborador = param.idColaborador;
            _empresa.emp_id_licenca = param.idLicenca;
            //_empresa.emp_id_token = param.token;

            if (_empresa.emp_id_empresa > 0)
            {
                context.empresa.Attach(_empresa);
                context.Entry(_empresa).State = EntityState.Modified;
            }
            else
            {
                context.empresa.Add(_empresa);
            }

            bool result = context.SaveChanges() > 0;

            EmpresaViewModel evm = ObterRegistro(_empresa.emp_id_empresa);

            return evm;
        }


        internal static bool Deletar(dynamic param)
        {
            int idEmpresa = (int)param.idEmpresa;

            EmpresaViewModel empresa = new EmpresaViewModel();
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            try
            {
                context.empresa.Where(x => x.emp_id_empresa == idEmpresa).ToList().ForEach(y => context.empresa.Remove(y));
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }




    }

    public class EmpresaViewModel
    {
        public int idEmpresa { get; set; }
        public string nomeFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public int idMunicipio { get; set; }
        public int idUf { get; set; }
        public int idRamo { get; set; }
        public int idSetor { get; set; }
        public string Cnpj { get; set; }
        public int idDono { get; set; }
        public string Local { get; set; }
        public string DescricaoLicenca { get; set; }
        public int IdLicenca { get; set; }
        public string Logradouro { get; set; }
        public string NumeroEndereco { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Complemento { get; set; }
        public string Telefone { get; set; }
        public string Token { get; set; }
        public bool Editavel { get; set; }
    }
}