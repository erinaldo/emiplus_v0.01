﻿using Emiplus.Data.Core;
using Emiplus.Data.Helpers;
using Emiplus.Properties;
using Emiplus.View.Common;
using ESC_POS_USB_NET.Printer;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emiplus.Controller
{
    class Pedido
    {
        private Model.Pedido _modelPedido = new Model.Pedido();
        private Model.Titulo _modelTitulo = new Model.Titulo();

        /// <summary>
        /// Alimenta grid dos clientes
        /// </summary>
        /// <param name="Table">Grid para alimentar</param>
        /// <param name="SearchText">Input box</param>
        /// <param name="tipo">"Clientes" ou "Colaboradores"</param>
        public void GetDataTablePessoa(DataGridView Table, string SearchText, string tipo)
        {
            Table.ColumnCount = 5;

            Table.Columns[0].Name = "ID";
            Table.Columns[0].Visible = false;

            Table.Columns[1].Name = "Nome";
            Table.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Table.Columns[2].Name = "CNPJ/CPF";
            Table.Columns[2].Width = 100;

            Table.Columns[3].Name = "RG";
            Table.Columns[3].Width = 100;

            Table.Columns[4].Name = "Razão Social";
            Table.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Table.Columns[4].Width = 100;

            Table.Rows.Clear();

            var clientes = new Model.Pessoa();

            var search = "%" + SearchText + "%";

            var data = clientes.Query()
                .Select("id", "nome", "rg", "cpf", "fantasia")
                .Where("excluir", 0)
                .Where("tipo", tipo)
                .Where
                (
                    q => q.WhereLike("nome", search)
                        .OrWhere("fantasia", search)
                        .OrWhere("rg", search)
                        .OrWhere("cpf", search)
                )
                .OrderByDesc("criado")
                .Limit(25)
                .Get();

            foreach (var cliente in data)
            {
                Table.Rows.Add(
                    cliente.ID,
                    cliente.NOME,
                    cliente.CPF,
                    cliente.RG,
                    cliente.FANTASIA
                );
            }
        }

        /// <summary>
        /// Alimenta grid dos colaboradores
        /// </summary>
        /// <param name="Table">Grid para alimentar</param>
        /// <param name="SearchText">Input box</param>
        /// <param name="tipo">"Clientes" ou "Colaboradores"</param>
        public void GetDataTableColaboradores(DataGridView Table, string SearchText, string tipo)
        {
            Table.ColumnCount = 2;

            Table.Columns[0].Name = "ID";
            Table.Columns[0].Visible = false;

            Table.Columns[1].Name = "Nome";
            Table.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            
            Table.Rows.Clear();

            var clientes = new Model.Usuarios();

            var search = "%" + SearchText + "%";

            var data = clientes.Query()
                .Select("id_user", "nome")
                .Where("excluir", 0)
                .Where
                (
                    q => q.WhereLike("nome", search)
                )
                .OrderByDesc("criado")
                .Limit(25)
                .Get();

            foreach (var cliente in data)
            {
                Table.Rows.Add(
                    cliente.ID_USER,
                    cliente.NOME
                );
            }
        }

        public Task<IEnumerable<dynamic>> GetDataTablePedidos(string tipo, string dataInicial, string dataFinal, string SearchText = null, int excluir = 0, int idPedido = 0, int status = 0, int usuario = 0)
        {
            var search = "%" + SearchText + "%";

            var query = new Model.Pedido().Query();

            query
                //.LeftJoin("nota", "nota.id_pedido", "pedido.id")
                .LeftJoin("pessoa", "pessoa.id", "pedido.cliente")
                .LeftJoin("usuarios as colaborador", "colaborador.id_user", "pedido.colaborador")
                .LeftJoin("usuarios as usuario", "usuario.id_user", "pedido.id_usuario")
                //.Select("pedido.id", "pedido.tipo", "pedido.emissao", "pedido.total", "pessoa.nome", "colaborador.nome as colaborador", "usuario.nome as usuario", "pedido.criado", "pedido.excluir", "pedido.status", "nota.nr_nota as nfe", "nota.serie", "nota.status as statusnfe", "nota.tipo as tiponfe")                
                .Select("pedido.id", "pedido.tipo", "pedido.emissao", "pedido.total", "pessoa.nome", "colaborador.nome as colaborador", "usuario.nome as usuario", "pedido.criado", "pedido.excluir", "pedido.status")
                .Where("pedido.excluir", excluir)
                .Where("pedido.emissao", ">=", Validation.ConvertDateToSql(dataInicial))
                .Where("pedido.emissao", "<=", Validation.ConvertDateToSql(dataFinal));
            
            if (!tipo.Contains("Notas"))
                query.Where("pedido.tipo", tipo);
            
            if (usuario != 0)
               query.Where("pedido.colaborador", usuario);

            if (status != 0)
            {
                if(tipo == "Notas")
                {
                    //1-PENDENTES 2-AUTORIZADAS 3-CANCELADAS
                    switch (status)
                    {
                        case 1:
                            query.Where("nota.status", null);
                            break;
                        case 2:
                            query.Where("nota.status", "Autorizada");
                            break;
                        case 3:
                            query.Where("nota.status", "Cancelada");
                            break;
                    }                    
                }
                else
                {
                    query.Where("pedido.status", status);
                }
            }   

            if (idPedido != 0)
                query.Where("pedido.id", idPedido);

            if (!string.IsNullOrEmpty(SearchText))
                query.Where
                (
                    q => q.WhereLike("pessoa.nome", search, false)
                );

            query.OrderByDesc("pedido.id");

            return query.GetAsync<dynamic>();
        }

        public Task<IEnumerable<dynamic>> GetDataTableNotas(string tipo, string dataInicial, string dataFinal, string SearchText = null, int excluir = 0, int idPedido = 0, int status = 0, int usuario = 0)
        {
            var search = "%" + SearchText + "%";

            var query = new Model.Nota().Query();

            query
                .LeftJoin("pedido", "pedido.id", "nota.id_pedido")
                .LeftJoin("pessoa", "pessoa.id", "pedido.cliente")
                .LeftJoin("usuarios as colaborador", "colaborador.id_user", "pedido.colaborador")
                .LeftJoin("usuarios as usuario", "usuario.id_user", "pedido.id_usuario")
                .Select("pedido.id", "pedido.tipo", "pedido.emissao", "pedido.total", "pessoa.nome", "colaborador.nome as colaborador", "usuario.nome as usuario", "pedido.criado", "pedido.excluir", "pedido.status", "nota.nr_nota as nfe", "nota.serie", "nota.status as statusnfe", "nota.tipo as tiponfe")                
                .Where("pedido.excluir", excluir)
                .Where("pedido.emissao", ">=", Validation.ConvertDateToSql(dataInicial))
                .Where("pedido.emissao", "<=", Validation.ConvertDateToSql(dataFinal));

            if (tipo == "Notas")            
                query.Where("nota.tipo", "NFe");

            if (tipo == "Cupons")
            {
                query.Where("nota.tipo", "CFe");
                query.Where("nota.status", "<>", "Pendente");
            }
                

            if (usuario != 0)
                query.Where("pedido.colaborador", usuario);

            if (status != 0)
            {
                if (tipo == "Notas")
                {
                    //1-PENDENTES 2-AUTORIZADAS 3-CANCELADAS
                    switch (status)
                    {
                        case 1:
                            query.Where("nota.status", null);
                            break;
                        case 2:
                            query.Where("nota.status", "Autorizada");
                            break;
                        case 3:
                            query.Where("nota.status", "Cancelada");
                            break;
                    }
                }
                else
                {
                    query.Where("pedido.status", status);
                }
            }

            if (idPedido != 0)
                query.Where("pedido.id", idPedido);

            if (!string.IsNullOrEmpty(SearchText))
                query.Where
                (
                    q => q.WhereLike("pessoa.nome", search, false)
                );

            query.OrderByDesc("pedido.id");

            return query.GetAsync<dynamic>();
        }

        public Task<IEnumerable<dynamic>> GetDataTableTotaisPedidos(string tipo, string dataInicial, string dataFinal, string SearchText = null, int excluir = 0)
        {
            var search = "%" + SearchText + "%";

            var query = new Model.Pedido().Query();

            query
            .LeftJoin("pessoa", "pessoa.id", "pedido.cliente")
            .LeftJoin("usuarios as colaborador", "colaborador.id_user", "pedido.colaborador")
            .LeftJoin("usuarios as usuario", "usuario.id_user", "pedido.id_usuario")

            .SelectRaw("SUM(pedido.total) as total, COUNT(pedido.id) as id")

            .Where("pedido.excluir", excluir)
            .Where("pedido.tipo", tipo)
            .Where("pedido.emissao", ">=", Validation.ConvertDateToSql(dataInicial))
            .Where("pedido.emissao", "<=", Validation.ConvertDateToSql(dataFinal));

            if (!string.IsNullOrEmpty(SearchText))
                query.Where
                (
                    q => q.WhereLike("pessoa.nome", search, false)
                );

            return query.GetAsync<dynamic>();
        }

        public async Task SetTablePedidos(DataGridView Table, string tipo, string dataInicial, string dataFinal, IEnumerable<dynamic> Data = null, string SearchText = null, int excluir = 0, int idPedido = 0, int status = 0, int usuario = 0)
        {
            Table.ColumnCount = 12;

            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, Table, new object[] { true });
            //Table.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

            Table.RowHeadersVisible = false;

            Table.Columns[0].Name = "ID";
            Table.Columns[0].Visible = false;

            if (tipo == "Notas" || tipo == "Cupons")
            {
                Table.Columns[1].Name = "N° Sefaz";
                Table.Columns[1].Width = 75;
            }                
            else
            {
                Table.Columns[1].Name = "N°";
                Table.Columns[1].Width = 50;
            }

            Table.Columns[2].Name = "Emissão";
            Table.Columns[2].MinimumWidth = 80;

            if (tipo == "Compras")
                Table.Columns[3].Name = "Fornecedor";
            else
                Table.Columns[3].Name = "Cliente";

            Table.Columns[3].Width = 150;

            Table.Columns[4].Name = "Total";
            Table.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Table.Columns[4].Width = 70;
            
            Table.Columns[5].Name = "Colaborador";
            Table.Columns[5].Width = 150;

            Table.Columns[6].Name = "Criado em";
            Table.Columns[6].Width = 120;

            Table.Columns[7].Name = "Status";
            Table.Columns[7].MinimumWidth = 150;
            Table.Columns[7].Visible = true;

            //if (tipo == "Vendas" || tipo == "Notas")
            //    Table.Columns[7].Visible = true;

            Table.Columns[8].Name = "EXCLUIR";
            Table.Columns[8].Visible = false;

            Table.Columns[9].Name = "NF-e";
            Table.Columns[9].MinimumWidth = 80;
            Table.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Table.Columns[9].Visible = false;

            Table.Columns[10].Name = "CF-e";
            Table.Columns[10].MinimumWidth = 80;
            Table.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Table.Columns[10].Visible = false;

            if (tipo == "Vendas")
            {
                Table.Columns[9].Visible = true;
                Table.Columns[10].Visible = true;
            }
              
            Table.Columns[11].Name = "TIPO";
            Table.Columns[11].Visible = false;

            Table.Rows.Clear();
                        
            if (Data == null)
            {
                IEnumerable<dynamic> dados;

                switch (tipo)
                {
                    case "Notas":
                        dados = await GetDataTableNotas(tipo, dataInicial, dataFinal, SearchText, excluir, idPedido, status, usuario);
                        break;
                    case "Cupons":
                        dados = await GetDataTableNotas(tipo, dataInicial, dataFinal, SearchText, excluir, idPedido, status, usuario);
                        break;
                    default:
                        dados = await GetDataTablePedidos(tipo, dataInicial, dataFinal, SearchText, excluir, idPedido, status, usuario);
                        break;
                }
                
                Data = dados;                
            }

            foreach (var item in Data)
            {
                var statusNfePedido = "";

                if (tipo == "Vendas")
                    statusNfePedido = item.STATUS == 1 ? "Recebimento Pendente" : item.STATUS == 0 ? "Pendente" : @"Finalizado\Recebido";

                if (Home.pedidoPage == "Orçamentos" || Home.pedidoPage == "Devoluções" || Home.pedidoPage == "Consignações")
                    statusNfePedido = item.STATUS == 1 ? "Finalizado" : item.STATUS == 0 ? "Pendente" : @"Finalizado\Recebido";

                string n_cfe = "", n_nfe = "";
                foreach (dynamic item2 in GetDadosNota(item.ID))
                {
                    if (item2.TIPONFE == "NFe")
                    {
                        n_nfe = item2.NFE;

                        if (tipo == "Notas")
                            statusNfePedido = item2.STATUSNFE == null ? "Pendente" : item2.STATUSNFE;
                    }

                    if (item2.TIPONFE == "CFe")
                    {
                        n_cfe = item2.NFE;

                        if (tipo == "Cupons")
                            statusNfePedido = item2.STATUSNFE == null ? "Pendente" : item2.STATUSNFE;
                    }                        
                }

                Table.Rows.Add(
                    item.ID,
                    tipo == "Notas" ? n_nfe : tipo == "Cupons" ? n_cfe : item.ID,
                    Validation.ConvertDateToForm(item.EMISSAO),
                    item.NOME == "Consumidor Final" && Home.pedidoPage == "Compras" ? "N/D" : item.NOME,
                    Validation.FormatPrice(Validation.ConvertToDouble(item.TOTAL), true),
                    item.COLABORADOR,
                    item.CRIADO,
                    statusNfePedido,
                    item.EXCLUIR,
                    n_nfe,
                    n_cfe,
                    item.TIPO
                );
            } 

            Table.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private IEnumerable<dynamic> GetDadosNota(int idpedido)
        {
            var query = new Model.Nota().Query();

            query
                .Select("nota.nr_nota as nfe", "nota.serie", "nota.status as statusnfe", "nota.tipo as tiponfe")
                .Where("nota.excluir", 0)
                .Where("nota.id_pedido", idpedido);

            return query.Get();
        }

        public void Remove(int idPedido)
        {
            _modelPedido.Remove(idPedido, "id", false);
            _modelTitulo.Remove(idPedido, "id_pedido", false);

            if (Home.pedidoPage == "Vendas" || Home.pedidoPage == "Consignações" || Home.pedidoPage == "Orçamentos")
                new Controller.Estoque(idPedido, Home.pedidoPage, "Botão Apagar").Add().Pedido();
            else if (Home.pedidoPage == "Compras" || Home.pedidoPage == "Devoluções")
                new Controller.Estoque(idPedido, Home.pedidoPage, "Botão Apagar").Remove().Pedido();

            Alert.Message("Pronto!", "Removido com sucesso!", Alert.AlertType.info);
        }

        public void Imprimir(int idPedido)
        {
            if(IniFile.Read("Printer", "Comercial") == "Bobina 80mm")
            {
                #region IMPRESSAO 

                #region EMITENTE

                var _emitente = new Model.Pessoa();
                var _emitenteEndereco = new Model.PessoaEndereco();
                var _emitenteContato = new Model.PessoaContato();

                _emitente.RG = Settings.Default.empresa_inscricao_estadual;
                _emitente.CPF = Settings.Default.empresa_cnpj;

                _emitente.Nome = Settings.Default.empresa_razao_social;
                _emitente.Fantasia = Settings.Default.empresa_nome_fantasia;
                
                _emitenteEndereco.Rua = Settings.Default.empresa_rua;
                _emitenteEndereco.Nr = Settings.Default.empresa_nr;
                _emitenteEndereco.Bairro = Settings.Default.empresa_bairro;
                _emitenteEndereco.Cidade = Settings.Default.empresa_cidade;
                _emitenteEndereco.Cep = Settings.Default.empresa_cep;
                _emitenteEndereco.IBGE = Settings.Default.empresa_ibge;
                _emitenteEndereco.Estado = Settings.Default.empresa_estado;

                #endregion

                var _pedido = new Model.Pedido().FindById(idPedido).First<Model.Pedido>();
                var _destinatario = new Model.Pessoa().FindById(_pedido.Cliente).FirstOrDefault<Model.Pessoa>();

                var printername = IniFile.Read("PrinterName", "Comercial");

                if (printername == null)
                    return;

                Printer printer = new Printer(printername);

                //using (WebClient wc = new WebClient())
                //{
                //    byte[] originalData = wc.DownloadData("https://www.emiplus.com.br" + Settings.Default.empresa_logo);
                //    MemoryStream stream = new MemoryStream(originalData);
                //    System.Drawing.Image img = Validation.ResizeImage(System.Drawing.Image.FromStream(stream), 1, 1);
                //    Bitmap bitmap = new Bitmap(img);
                //    printer.Image(bitmap);
                //}

                printer.AlignCenter();
                printer.BoldMode(_emitente.Fantasia);
                printer.Append(_emitente.Nome);
                printer.Append(_emitenteEndereco.Rua + ", " + _emitenteEndereco.Nr + " - " + _emitenteEndereco.Bairro);
                printer.Append(_emitenteEndereco.Cidade + "/" + _emitenteEndereco.Estado);
                printer.Append(_emitenteContato.Telefone);

                printer.NewLines(2);

                printer.BoldMode("CNPJ: " + _emitente.CPF + " IE: " + _emitente.RG);
                printer.Separator();

                printer.BoldMode("Extrato N°" + _pedido.Id);

                if (_pedido.Tipo == "Orçamentos")
                {
                    printer.BoldMode("Orçamento".ToUpper());
                }
                else if (_pedido.Tipo == "Consignações")
                {
                    printer.BoldMode("Consignação".ToUpper());
                }
                else if (_pedido.Tipo == "Compras")
                {
                    printer.BoldMode("Compra".ToUpper());
                }
                else if (_pedido.Tipo == "Devoluções")
                {
                    printer.BoldMode("Devolução".ToUpper());
                }
                else
                {
                    printer.BoldMode("Venda".ToUpper());
                }

                printer.Separator();

                printer.AlignLeft();

                if(_pedido.Tipo == "Compras")                    
                    printer.Append("Fornecedor: " + _destinatario.Nome);
                else
                    printer.Append("Cliente: " + _destinatario.Nome);

                printer.Separator();

                printer.AlignCenter();
                printer.Append("#|COD|DESC|QTD|UN|VL UNIT|VL TR*|VLR ITEM R$|");
                printer.Separator();

                //printer.Append(AddSpaces("<n><cod><desc><qnt><un>x<vlrunit>", "0,00"));

                var itens = new Model.PedidoItem().Query()
                .LeftJoin("item", "pedido_item.item", "item.id")
                .Select("item.nome", "item.referencia", "item.codebarras", "pedido_item.quantidade", "pedido_item.valorvenda", "pedido_item.medida", "pedido_item.total as total", "pedido_item.federal", "pedido_item.estadual", "pedido_item.municipal")
                .Where("pedido_item.pedido", idPedido)
                .Where("pedido_item.excluir", 0)
                .Where("pedido_item.tipo", "Produtos")
                .OrderBy("pedido_item.id")
                .Get();

                int count = 0;

                foreach (var data in itens)
                {
                    count++;
                    printer.Append(Validation.AddSpaces(count + " " + data.NOME + " " + data.QUANTIDADE + " " + data.MEDIDA + " x " + Validation.FormatDecimalPrice(data.VALORVENDA) + " (" + Validation.FormatDecimalPrice(data.FEDERAL + data.ESTADUAL + data.MUNICIPAL) + ")", Validation.FormatDecimalPrice(data.TOTAL)));
                }

                printer.NewLines(2);

                printer.Append(Validation.AddSpaces("Subtotal", Validation.FormatPrice(_pedido.Produtos)));
                printer.Append(Validation.AddSpaces("Descontos", Validation.FormatPrice(_pedido.Desconto)));
                printer.BoldMode(Validation.AddSpaces("TOTAL R$", Validation.FormatPrice(_pedido.Total)));

                printer.NewLines(2);

                //pagamentos = new Model.Titulo().Query().Select("Total").Where("titulo.id_pedido", Pedido).Where("titulo.excluir", 0).Get();
                //total = total + Validation.ConvertToDouble(data.TOTAL);

                string formapgto = "";

                var pagamentos = new Model.Titulo().Query().Where("titulo.id_pedido", idPedido).Where("titulo.excluir", 0).Get();
                foreach (var data in pagamentos)
                {
                    switch (data.ID_FORMAPGTO)
                    {
                        case 1:
                            formapgto = "Dinheiro";
                            break;
                        case 2:
                            formapgto = "Cheque";
                            break;
                        case 3:
                            formapgto = "Cartão de Débito";
                            break;
                        case 4:
                            formapgto = "Cartão de Crédito";
                            break;
                        case 5:
                            formapgto = "Crediário";
                            break;
                        case 6:
                            formapgto = "Boleto";
                            break;
                        default:
                            formapgto = "N/D";
                            break;
                    }

                    printer.Append(Validation.AddSpaces(formapgto, Validation.FormatDecimalPrice(data.TOTAL)));
                }

                printer.Append(Validation.AddSpaces("Troco R$", "0,00"));

                printer.Separator();
                
                printer.NewLines(3);

                printer.FullPaperCut();
                printer.PrintDocument();

                #endregion

                return;
            }
            
            PedidoImpressao print = new PedidoImpressao();
            print.Print(idPedido);
        }

        private Random random = new Random();

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
