using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace laudosPiscinasProject.Api.Models
{
    public class EquipamentosModel
    {
        public static List<EquipamentoViewModel> Lista(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            int idObra = param.idObra ?? 0;

            List<equipamento> ListaEquipamentos = new List<equipamento>();
            List<obra_equipamento> ListaObraEquipamento = context.Set<obra_equipamento>().ToList();

            ListaObraEquipamento = ListaObraEquipamento.Where(oeq => oeq.oeq_id_obra == idObra).ToList();
            ListaObraEquipamento.ForEach(equ => ListaEquipamentos.Add(equ.equipamento));


            if (param != null)
            {
                string descricao = param.descricao.ToString();
                string marca = param.marca.ToString();
                string modelo = param.modelo.ToString();
                string tipoEquipamento = param.tipoEquipamento.ToString();

                if (!string.IsNullOrEmpty(descricao))
                {
                    ListaEquipamentos = ListaEquipamentos.Where(equ => equ.equ_ds_equipamento.ToLower().Contains(descricao.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(marca))
                {
                    ListaEquipamentos = ListaEquipamentos.Where(equ => equ.equ_ds_marca.ToLower().Contains(marca.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(modelo))
                {
                    ListaEquipamentos = ListaEquipamentos.Where(equ => equ.equ_ds_modelo.ToLower().Contains(modelo.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(tipoEquipamento))
                {
                    ListaEquipamentos = ListaEquipamentos.Where(equ => equ.tipo_equipamento.teq_nm_tipo_equipamento.ToLower().Contains(tipoEquipamento.ToLower())).ToList();
                }
            }

            List<EquipamentoViewModel> Lista = new List<EquipamentoViewModel>();
            ListaEquipamentos.ForEach(equ => Lista.Add(new EquipamentoViewModel
            {
                Descricao = equ.equ_ds_equipamento,
                Id = equ.equ_id_equipamento,
                Marca = equ.equ_ds_marca,
                Modelo = equ.equ_ds_modelo,
                TipoAquisicao = equ.obra_equipamento.FirstOrDefault(x => x.oeq_id_equipamento == equ.equ_id_equipamento) == null ? "S" : equ.obra_equipamento.FirstOrDefault(x => x.oeq_id_equipamento == equ.equ_id_equipamento).oeq_tp_aquisicao,
                FabricanteFornecedor = equ.obra_equipamento.FirstOrDefault(x => x.oeq_id_equipamento == equ.equ_id_equipamento) == null ? "" : equ.obra_equipamento.FirstOrDefault(x => x.oeq_id_equipamento == equ.equ_id_equipamento).oeq_ds_fabricante_fornecedor,
                Contato = equ.obra_equipamento.FirstOrDefault(x => x.oeq_id_equipamento == equ.equ_id_equipamento) == null ? "" : equ.obra_equipamento.FirstOrDefault(x => x.oeq_id_equipamento == equ.equ_id_equipamento).oeq_ds_contato,
                Telefone = equ.obra_equipamento.FirstOrDefault(x => x.oeq_id_equipamento == equ.equ_id_equipamento) == null ? "" : String.IsNullOrEmpty(equ.obra_equipamento.FirstOrDefault(x => x.oeq_id_equipamento == equ.equ_id_equipamento).oeq_ds_telefone) ? "" : equ.obra_equipamento.FirstOrDefault(x => x.oeq_id_equipamento == equ.equ_id_equipamento).oeq_ds_telefone.Length == 11 ? Int64.Parse(equ.obra_equipamento.FirstOrDefault(x => x.oeq_id_equipamento == equ.equ_id_equipamento).oeq_ds_telefone).ToString("(##) #####-####") : Int64.Parse(equ.obra_equipamento.FirstOrDefault(x => x.oeq_id_equipamento == equ.equ_id_equipamento).oeq_ds_telefone).ToString("(##) ####-####"),
                TipoEquipamento = equ.tipo_equipamento.teq_nm_tipo_equipamento
                //parte do obra_equipamento

            }));

            string tipoAquisicao = param.tipoAquisicao;
            string fabricanteFornecedor = Convert.ToString(param.fabricanteFornecedor);
            //DateTime dataAquisicao = Convert.ToString(param.dataAquisicao) == "" ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataAquisicao));

            if (tipoAquisicao != "S")
            {
                Lista = Lista.Where(x => x.TipoAquisicao == tipoAquisicao).ToList();
            }
            if (!string.IsNullOrEmpty(fabricanteFornecedor))
            {
                Lista = Lista.Where(x => x.FabricanteFornecedor.ToLower().Contains(fabricanteFornecedor)).ToList();
            }

            string orderby = param.orderby ?? "";
            string orderbydescending = param.orderbydescending ?? "";

            if (!string.IsNullOrEmpty(orderby))
            {
                return Lista.OrderBy(x => x.Descricao).ToList();
            }
            if (!string.IsNullOrEmpty(orderbydescending))
            {
                return Lista.OrderByDescending(x => x.Descricao).ToList();
            }

            return Lista;
        }

        public static EquipamentoViewModel Salvar(EquipamentoViewModel param)
        {
            if (string.IsNullOrEmpty(param.Descricao.ToString()))
            {
                throw new Exception("A descricao deve ser preenchida.");
            }
            //if (string.IsNullOrEmpty(param.marca.ToString()))
            //{
            //    throw new Exception("A marca deve ser preenchida");
            //}
            //if (string.IsNullOrEmpty(param.modelo.ToString()))
            //{
            //    throw new Exception("O modelo deve ser preenchido");
            //}

            //if (string.IsNullOrEmpty(param.tipoAquisicao.ToString()) || param.tipoAquisicao.ToString() == "S")
            //{
            //    throw new Exception("O tipo de aquisicao do equipamento deve ser preenchido");
            //}



            int idEquipamento = param.Id;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            equipamento _equipamento = context.equipamento.Where(x => x.equ_id_equipamento == idEquipamento).FirstOrDefault() ?? new equipamento();

            _equipamento.equ_ds_equipamento = param.Descricao;

            //atualiza as tabelas de marca e de modelo


            string modelo = param.Modelo;
            if (!string.IsNullOrEmpty(modelo))
            {
                if (context.modelo.Where(x => x.mod_ds_modelo.Contains(modelo)).Count() == 0)
                {
                    context.modelo.Add(new modelo { mod_ds_modelo = modelo });
                }
            }

            string marca = param.Marca;
            if (!string.IsNullOrEmpty(marca))
            {
                if (context.marca.Where(x => x.mar_ds_marca.Contains(marca)).Count() == 0)
                {
                    context.marca.Add(new marca { mar_ds_marca = marca });
                }
            }

            string tipoEquipamento = param.TipoEquipamento;
            tipo_equipamento teq = new tipo_equipamento();
            if (!string.IsNullOrEmpty(tipoEquipamento))
            {
                if (context.tipo_equipamento.Where(x => x.teq_nm_tipo_equipamento.Contains(tipoEquipamento)).Count() == 0)
                {
                    context.tipo_equipamento.Add(new tipo_equipamento { teq_nm_tipo_equipamento = tipoEquipamento });
                    context.SaveChanges();
                }
                teq = context.tipo_equipamento.FirstOrDefault(x => x.teq_nm_tipo_equipamento.Contains(tipoEquipamento));
            }

            _equipamento.equ_ds_marca = param.Marca;
            _equipamento.equ_ds_modelo = param.Modelo;
            _equipamento.equ_id_tipo_equipamento = teq.teq_id_tipo_equipamento > 0 ? teq.teq_id_tipo_equipamento : param.IdTipoEquipamento;

            //if (param.foto != null && param.foto.filename != _equipamento.equ_ds_imagem)
            //{
            //    _equipamento.equ_ds_imagem = UtilsModel.SalvarImagem(param.foto, "ImagensEquipamentos");
            //}

            //if (param != null && param.foto != null)
            //{
            //    string nomeFoto = param.foto.ToString();
            //    if (!String.IsNullOrEmpty(nomeFoto))
            //    {
            //        string nomeArquivo = param.foto.ToString();
            //        if (nomeArquivo != _equipamento.equ_ds_imagem)
            //        {
            //            _equipamento.equ_ds_imagem = UtilsModel.SalvarImagem(param.foto, "ImagensEquipamentos");
            //        }
            //    }
            //}

            int idObra = param.IdObra;
            obra_equipamento _obra_equipamento = context.obra_equipamento.Where(x => x.oeq_id_equipamento == idEquipamento && x.oeq_id_obra == idObra).FirstOrDefault() ?? new obra_equipamento();

            _obra_equipamento.oeq_tp_aquisicao = param.TipoAquisicao;
            _obra_equipamento.oeq_ds_fabricante_fornecedor = param.FabricanteFornecedor;
            if (param.DataAquisicao != null && !string.IsNullOrEmpty(param.DataAquisicao.ToString().Replace("_", "").Replace("/", "")))
            {
                _obra_equipamento.oeq_dt_aquisicao = Convert.ToDateTime(Convert.ToString(param.DataAquisicao));
            }
            _obra_equipamento.oeq_ds_contato = param.Contato;
            string telefone = param.Telefone;
            _obra_equipamento.oeq_ds_telefone = !string.IsNullOrEmpty(telefone) ? telefone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : telefone;
            _obra_equipamento.oeq_id_obra = idObra;
            _obra_equipamento.oeq_id_equipamento = idEquipamento;


            if (_equipamento.equ_id_equipamento > 0 && _obra_equipamento.oeq_id_obra_equipamento > 0)
            {
                context.equipamento.Attach(_equipamento);
                context.Entry(_equipamento).State = EntityState.Modified;


                context.obra_equipamento.Attach(_obra_equipamento);
                context.Entry(_obra_equipamento).State = EntityState.Modified;

            }
            else if (_equipamento.equ_id_equipamento > 0 && !(_obra_equipamento.oeq_id_obra_equipamento > 0))
            {
                context.equipamento.Attach(_equipamento);
                context.Entry(_equipamento).State = EntityState.Modified;

                context.obra_equipamento.Add(_obra_equipamento);
            }
            else
            {
                context.equipamento.Add(_equipamento);

                context.obra_equipamento.Add(_obra_equipamento);
            }

            bool result = false;

            try
            {
                result = context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
            }


            dynamic MyDynamic = new System.Dynamic.ExpandoObject();
            MyDynamic.id = _equipamento.equ_id_equipamento;
            MyDynamic.idObra = _obra_equipamento.oeq_id_obra;
            EquipamentoViewModel evm = ObterRegistro(MyDynamic);

            return evm;
        }

        public static bool AssociarEquipamentoTarefa(int idObra, int idEquipamento, int idTarefa)
        {

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            equipamento _equipamento = context.equipamento.Where(x => x.equ_id_equipamento == idEquipamento).FirstOrDefault() ?? new equipamento();
            obra_equipamento _obra_equipamento = context.obra_equipamento.Where(x => x.oeq_id_equipamento == idEquipamento && x.oeq_id_obra == idObra).FirstOrDefault() ?? new obra_equipamento();
            obra_tarefa_equipamento _obra_tarefa_equipamento = context.obra_tarefa_equipamento.Where(x => x.ote_id_obra_equipamento == _obra_equipamento.oeq_id_obra_equipamento && x.ote_id_tarefa == idTarefa).FirstOrDefault() ?? new obra_tarefa_equipamento();

            _obra_tarefa_equipamento.ote_id_tarefa = idTarefa;
            _obra_tarefa_equipamento.ote_id_obra_equipamento = _obra_equipamento.oeq_id_obra_equipamento;


            if (_obra_tarefa_equipamento.ote_id_obra_tarefa_euipamento > 0)
            {
                context.obra_tarefa_equipamento.Attach(_obra_tarefa_equipamento);
                context.Entry(_obra_tarefa_equipamento).State = EntityState.Modified;
            }
            else
            {
                context.obra_tarefa_equipamento.Add(_obra_tarefa_equipamento);
            }

            bool result = context.SaveChanges() > 0;
            return result;
        }

        internal static bool VerificarExclusao(dynamic param)
        {
            if (string.IsNullOrEmpty(param.id.ToString()))
            {
                return true;
            }

            int idEquipamento = (int)param.id;

            if (idEquipamento <= 0)
            {
                return true;
            }

            int idObra = (int)param.idObra;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            return !context.equipamento.Find(idEquipamento).obra_equipamento.Any(oe => oe.obra_tarefa_equipamento.Count() > 0);
        }

        internal static bool Deletar(dynamic param)
        {
            int idEquipamento = (int)param.id;
            int idObra = (int)param.idObra;
            //int idTarefa = (int)param.idTarefa;

            EquipamentoViewModel equipamento = new EquipamentoViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            try
            {
                // os equipamentos não serão excluidos só os relacionamentos, já deixei pronto para quando mudar.

                //context.obra_tarefa_equipamento.Where(x => x.ote_id_tarefa == idTarefa).ToList().ForEach(y => context.obra_tarefa_equipamento.Remove(y));
                context.obra_equipamento.Where(x => x.oeq_id_obra == idObra && x.oeq_id_equipamento == idEquipamento).ToList().ForEach(y => context.obra_equipamento.Remove(y));
                context.equipamento.Where(x => x.equ_id_equipamento == idEquipamento).ToList().ForEach(y => context.equipamento.Remove(y));

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Não é possível excluir este equipamento. Já existem registros dependentes.");
            }

            return true;
        }

        internal static EquipamentoViewModel ObterRegistro(dynamic param)
        {
            //int idEquipamento = (int)param;
            //int idObra = (int)param;



            int idEquipamento = param.id ?? 0;
            int idObra = param.idObra ?? 0;


            EquipamentoViewModel equipamento = new EquipamentoViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var resultado = context.equipamento.FirstOrDefault(equ => equ.equ_id_equipamento == idEquipamento);

            equipamento.Id = resultado.equ_id_equipamento;
            equipamento.Descricao = resultado.equ_ds_equipamento;
            equipamento.Marca = resultado.equ_ds_marca;
            equipamento.Modelo = resultado.equ_ds_modelo;
            equipamento.DescricaoFoto = resultado.equ_ds_imagem;
            equipamento.Foto = resultado.equ_ds_imagem;
            equipamento.TipoEquipamento = resultado.tipo_equipamento.teq_nm_tipo_equipamento;

            obra_equipamento equipamentoObra = context.obra_equipamento.FirstOrDefault(oeq => oeq.oeq_id_equipamento == idEquipamento && oeq.oeq_id_obra == idObra);
            equipamento.Contato = equipamentoObra.oeq_ds_contato;
            equipamento.DataAquisicao = equipamentoObra.oeq_dt_aquisicao == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(equipamentoObra.oeq_dt_aquisicao).Substring(0, 10));
            equipamento.TipoAquisicao = equipamentoObra.oeq_tp_aquisicao;
            equipamento.Telefone = equipamentoObra.oeq_ds_telefone;
            equipamento.FabricanteFornecedor = equipamentoObra.oeq_ds_fabricante_fornecedor;
            equipamento.IdObra = idObra;
            equipamento.IdObraEquipamento = equipamentoObra.oeq_id_obra_equipamento;

            return equipamento;
        }

        internal static EquipamentoViewModel ObterRegistro(int idObra, int idEquipamento)
        {
            EquipamentoViewModel equipamento = new EquipamentoViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            equipamento resultado = context.equipamento.FirstOrDefault(equ => equ.equ_id_equipamento == idEquipamento);

            equipamento.Id = resultado.equ_id_equipamento;
            equipamento.Descricao = resultado.equ_ds_equipamento;
            equipamento.Marca = resultado.equ_ds_marca;
            equipamento.Modelo = resultado.equ_ds_modelo;
            equipamento.DescricaoFoto = resultado.equ_ds_imagem;
            equipamento.TipoEquipamento = resultado.tipo_equipamento.teq_nm_tipo_equipamento;
            //equipamento.Foto = resultado.equ_ds_imagem;

            obra_equipamento equipamentoObra = context.obra_equipamento.FirstOrDefault(oeq => oeq.oeq_id_equipamento == idEquipamento && oeq.oeq_id_obra == idObra);

            equipamento.Contato = equipamentoObra.oeq_ds_contato;
            equipamento.DataAquisicao = equipamentoObra.oeq_dt_aquisicao == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(equipamentoObra.oeq_dt_aquisicao).Substring(0, 10));
            equipamento.TipoAquisicao = equipamentoObra.oeq_tp_aquisicao;
            equipamento.Telefone = equipamentoObra.oeq_ds_telefone == null || equipamentoObra.oeq_ds_telefone == "" ? "" : equipamentoObra.oeq_ds_telefone.Length == 10 ? Int64.Parse(equipamentoObra.oeq_ds_telefone).ToString("(##) ####-####") : Int64.Parse(equipamentoObra.oeq_ds_telefone).ToString("(##) #####-####");
            equipamento.FabricanteFornecedor = equipamentoObra.oeq_ds_fabricante_fornecedor;
            equipamento.IdObraEquipamento = equipamentoObra.oeq_id_obra_equipamento;
            equipamento.IdObra = equipamentoObra.oeq_id_obra;
            equipamento.DescricaoTipoAquisicao = equipamentoObra.oeq_tp_aquisicao;

            return equipamento;
        }

        internal static string ListaMarca()
        {
            var listaMarca = string.Join(",", new LaudosPiscinasEntities().marca.OrderBy(t => t.mar_ds_marca).Select(m => m.mar_ds_marca));

            return listaMarca;
        }

        internal static string ListaModelo()
        {
            var listaModelo = string.Join(",", new LaudosPiscinasEntities().modelo.OrderBy(t => t.mod_ds_modelo).Select(m => m.mod_ds_modelo));

            return listaModelo;
        }

        internal static string ListaTipoEquipamento()
        {
            var listaTipoEquipamento = string.Join(",", new LaudosPiscinasEntities().tipo_equipamento.OrderBy(t => t.teq_nm_tipo_equipamento).Select(m => m.teq_nm_tipo_equipamento));

            return listaTipoEquipamento;
        }
        
        //internal static string ListaDescricao()
        //{
        //    var listaDescricao = string.Join(",", new LaudosPiscinasEntities().equipamento.Select(m => m.equ_ds_equipamento));

        //    return listaDescricao;
        //}

    }

    public class EquipamentoViewModel
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string TipoEquipamento { get; set; }
        public string TipoAquisicao { get; set; }
        public string DataAquisicao { get; set; }
        public string DescricaoFoto { get; set; }
        public string Foto { get; set; }
        public string Contato { get; set; }
        public string Telefone { get; set; }
        public string FabricanteFornecedor { get; set; }
        public int IdObra { get; set; }
        public int IdObraEquipamento { get; set; }
        public string DescricaoTipoAquisicao { get; set; }
        public int IdTipoEquipamento { get; set; }
        public string NomeTipoEquipamento { get; set; }
        public EquipamentoViewModel() { }
        public EquipamentoViewModel(dynamic param)
        {
            if (param != null)
            {
                Id = param.id == null ? 0 : param.id;
                Descricao = param.descricao;
                Marca = param.marca;
                Modelo = param.modelo;
                DataAquisicao = param.dataAquisicao;
                Telefone = param.telefone;
                IdObra = param.idObra;
                IdObraEquipamento = param.idObraEquipamento == null ? 0 : param.idObraEquipamento;
                DescricaoTipoAquisicao = param.descricaoTipoAquisicao;
                IdTipoEquipamento = param.idTipoEquipamento;
                FabricanteFornecedor = param.fabricanteFornecedor;
                TipoAquisicao = param.tipoAquisicao;
                Contato = param.contato;
            }
        }
    }

    public class TipoEquipamentoModel
    {
        private IQueryable<tipo_equipamento> Filter(TipoEquipamentoViewModel filter, LaudosPiscinasEntities context)
        {
            IQueryable<tipo_equipamento> query = context.tipo_equipamento;
            if (filter != null)
            {
                if (filter.Id > 0)
                {
                    query = query.Where(e => e.teq_id_tipo_equipamento == filter.Id);
                }
                if (!string.IsNullOrEmpty(filter.Nome))
                {
                    query = query.Where(e => e.teq_nm_tipo_equipamento.Contains(filter.Nome));
                }
            }
            return query;
        }
        internal static int Create(TipoEquipamentoViewModel view)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                context.tipo_equipamento.Add(TipoEquipamentoViewModel.ViewToEntity(view));
                var result = context.SaveChanges();
                return result;
            }
        }
        internal static List<TipoEquipamentoViewModel> Retrieve(TipoEquipamentoViewModel filter)
        {
            var result = new List<TipoEquipamentoViewModel>();

            using (var context = new LaudosPiscinasEntities())
            {
                IQueryable<tipo_equipamento> query = new TipoEquipamentoModel().Filter(filter, context);

                var list = query.OrderBy(x => x.teq_nm_tipo_equipamento).ToList();

                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        result.Add(new TipoEquipamentoViewModel(item));
                    }
                }
            }

            return result;
        }
        internal static int Update(TipoEquipamentoViewModel view)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var entity = context.tipo_equipamento.Find(view.Id);
                entity.teq_nm_tipo_equipamento = view.Nome;

                context.tipo_equipamento.Add(entity);
                context.Entry(entity).State = System.Data.EntityState.Modified;

                var result = context.SaveChanges();
                return result;
            }
        }
        internal static int Delete(int id)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var entity = context.tipo_equipamento.Find(id);
                context.tipo_equipamento.Remove(entity);
                var result = context.SaveChanges();
                return result;
            }
        }
    }

    public class TipoEquipamentoViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public TipoEquipamentoViewModel() { }
        public TipoEquipamentoViewModel(tipo_equipamento entity)
        {
            if (entity != null)
            {
                Id = entity.teq_id_tipo_equipamento;
                Nome = entity.teq_nm_tipo_equipamento;
                Descricao = entity.teq_ds_tipo_equipamento;
            }
        }
        internal static tipo_equipamento ViewToEntity(TipoEquipamentoViewModel view)
        {
            if (view != null)
            {
                var entity = new tipo_equipamento();
                entity.teq_id_tipo_equipamento = view.Id;
                entity.teq_nm_tipo_equipamento = view.Nome;
                entity.teq_ds_tipo_equipamento = view.Descricao;
                return entity;
            }
            return null;
        }
    }
}