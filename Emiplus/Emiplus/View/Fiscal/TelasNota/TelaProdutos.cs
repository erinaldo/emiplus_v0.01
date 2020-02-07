﻿using Emiplus.Controller;
using Emiplus.Data.Helpers;
using Emiplus.Data.SobreEscrever;
using Emiplus.View.Comercial;
using Emiplus.View.Common;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Emiplus.View.Fiscal.TelasNota
{
    public partial class TelaProdutos : Form
    {
        #region V

        private int ModoRapAva { get; set; }
        private static int Id { get; set; }
        private int ModoRapAvaConfig { get; set; }

        public static int idImposto { get; set; }

        private Model.Item _mItem = new Model.Item();
        private Model.Pedido _mPedido = new Model.Pedido();
        private Model.PedidoItem _mPedidoItens = new Model.PedidoItem();
        private Model.Nota _mNota = new Model.Nota();

        private KeyedAutoCompleteStringCollection collection = new KeyedAutoCompleteStringCollection();

        #endregion V

        public TelaProdutos()
        {
            InitializeComponent();
            Id = Nota.Id;
            _mNota = _mNota.FindById(Id).FirstOrDefault<Model.Nota>();

            if (_mNota == null)
            {
                Alert.Message("Ação não permitida", "Referência de Pedido não identificada", Alert.AlertType.warning);
                return;
            }

            _mPedido = _mPedido.FindById(_mNota.id_pedido).FirstOrDefault<Model.Pedido>();

            DisableCampos();
            Eventos();
        }

        /// <summary>
        /// Carrega e adiciona os itens no pedido!
        /// Function: collection.Lookup recupera o ID
        /// </summary>
        private void LoadItens()
        {
            // Abre modal de Itens caso não encontre nenhum item no autocomplete, ou pressionando Enter.
            ModalItens();

            // Valida a busca pelo produto e faz o INSERT, gerencia também o estoque e atualiza os totais
            AddItem();

            PedidoModalItens.NomeProduto = "";
        }

        private void DisableCampos()
        {
            if (Nota.disableCampos)
            {
                BuscarProduto.Enabled = false;
                Quantidade.Enabled = false;
                Preco.Enabled = false;
                Medidas.Enabled = false;
                DescontoPorcentagem.Enabled = false;
                DescontoReais.Enabled = false;
                addProduto.Enabled = false;
            }
        }

        /// <summary>
        /// Atualiza o pedido com as somas totais.
        /// </summary>
        private void LoadTotais()
        {
            label44.Text = GridListaProdutos.RowCount.ToString();

            var data = _mPedido.SaveTotais(_mPedidoItens.SumTotais(_mNota.id_pedido));
            _mPedido.Save(data);

            label35.Text = Validation.FormatPrice(_mPedido.GetTotal(), false);
            label29.Text = Validation.FormatPrice(_mPedido.GetDesconto(), false);
        }

        /// <summary>
        /// Janela para selecionar itens não encontrados.
        /// </summary>
        private void ModalItens()
        {
            if (collection.Lookup(BuscarProduto.Text) == 0)
            {
                if ((Application.OpenForms["PedidoModalItens"] as PedidoModalItens) == null)
                {
                    PedidoModalItens.txtSearch = BuscarProduto.Text;
                    using (PedidoModalItens form = new PedidoModalItens()) {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            BuscarProduto.Text = PedidoModalItens.NomeProduto;

                            if (PedidoModalItens.ValorVendaProduto == 0 && ModoRapAva == 0)
                            {
                                AlterarModo();
                                ModoRapAvaConfig = 1;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Limpa os input text.
        /// </summary>
        private void ClearForms()
        {
            BuscarProduto.Clear();
            Quantidade.Clear();
            Preco.Clear();
            DescontoPorcentagem.Clear();
            DescontoReais.Clear();
        }

        /// <summary>
        /// Autocomplete do campo de busca de produtos.
        /// </summary>
        private void AutoCompleteItens()
        {
            var item = _mItem.Query().Select("id", "nome").Where("excluir", 0).Where("tipo", "Produtos").Get();

            foreach (var itens in item)
            {
                collection.Add(itens.NOME, itens.ID);
            }

            BuscarProduto.AutoCompleteCustomSource = collection;
        }

        /// <summary>
        /// Altera o modo do pedido, para avançado e simples.
        /// 1 = Avançado, 0 = Simples
        /// </summary>
        private void AlterarModo()
        {
            if (ModoRapAva == 1)
            {
                ModoRapAva = 0;
                panelAvancado.Visible = false;
                ModoRapido.Text = "Modo Avançado (F1) ?";
            }
            else
            {
                ModoRapAva = 1;
                panelAvancado.Visible = true;
                ModoRapido.Text = "Modo Rápido (F1) ?";
            }
        }

        /// <summary>
        /// Adiciona item ao pedido, controla o estoque e atualiza os totais.
        /// </summary>
        private void AddItem()
        {
            if (collection.Lookup(BuscarProduto.Text) > 0 && String.IsNullOrEmpty(PedidoModalItens.NomeProduto))
            {
                var itemId = collection.Lookup(BuscarProduto.Text);
                Model.Item item = _mItem.FindById(itemId).Where("excluir", 0).Where("tipo", "Produtos").First<Model.Item>();

                double QuantidadeTxt = Validation.ConvertToDouble(Quantidade.Text);
                double DescontoReaisTxt = Validation.ConvertToDouble(DescontoReais.Text);
                double DescontoPorcentagemTxt = Validation.ConvertToDouble(DescontoPorcentagem.Text);
                string MedidaTxt = Medidas.Text;
                double PriceTxt = Validation.ConvertToDouble(Preco.Text);

                var pedidoItem = new Model.PedidoItem();
                pedidoItem.SetId(0)
                    .SetTipo("Produtos")
                    .SetPedidoId(_mNota.id_pedido)
                    .SetItem(item)
                    .SetQuantidade(QuantidadeTxt)
                    .SetMedida(MedidaTxt)
                    .SetDescontoReal(DescontoReaisTxt);

                if (!pedidoItem.SetValorVenda(PriceTxt))
                {
                    if (ModoRapAva == 0)
                    {
                        AlterarModo();
                        ModoRapAvaConfig = 1;
                    }

                    Preco.Select();
                    Preco.Focus();
                    return;
                }

                pedidoItem.SetDescontoPorcentagens(DescontoPorcentagemTxt);
                pedidoItem.SomarTotal();
                pedidoItem.Save(pedidoItem);

                new Controller.Imposto().SetImposto(pedidoItem.GetLastId());

                // Carrega a Grid com o Item adicionado acima.
                PedidoItem.impostos = true;
                new Controller.PedidoItem().GetDataTableItens(GridListaProdutos, _mNota.id_pedido);

                // Atualiza o total do pedido, e os totais da tela
                LoadTotais();

                // Limpa os campos
                ClearForms();

                // Verifica se modo é avançado e altera para modo simples, apenas se modo simples for padrão
                if (ModoRapAva == 1 && ModoRapAvaConfig == 1)
                {
                    AlterarModo();
                    ModoRapAvaConfig = 0;
                }

                BuscarProduto.Select();
            }
        }

        /// <summary>
        /// Adiciona os eventos de 'KeyDown' a todos os controls com a function KeyDowns
        /// </summary>
        private void KeyDowns(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    GridListaProdutos.Focus();
                    Support.UpDownDataGrid(false, GridListaProdutos);
                    e.Handled = true;
                    break;

                case Keys.Down:
                    GridListaProdutos.Focus();
                    Support.UpDownDataGrid(true, GridListaProdutos);
                    e.Handled = true;
                    break;

                case Keys.Escape:
                    Close();
                    break;

                case Keys.F1:
                    AlterarModo();
                    break;

                case Keys.F2:
                    BuscarProduto.Focus();
                    break;

                case Keys.F3:
                    if (GridListaProdutos.SelectedRows.Count > 0)
                    {
                        if (Validation.ConvertToInt32(GridListaProdutos.SelectedRows[0].Cells["ID"].Value) > 0)
                        {
                            var itemName = GridListaProdutos.SelectedRows[0].Cells["Nome do Produto"].Value;

                            var result = AlertOptions.Message("Atenção!", $"Cancelar o item ('{itemName}') no pedido?", AlertBig.AlertType.info, AlertBig.AlertBtn.YesNo);
                            if (result)
                            {
                                var idPedidoItem = Validation.ConvertToInt32(GridListaProdutos.SelectedRows[0].Cells["ID"].Value);
                                _mPedidoItens.Remove(idPedidoItem);

                                GridListaProdutos.Rows.RemoveAt(GridListaProdutos.SelectedRows[0].Index);

                                LoadTotais();
                            }
                        }
                    }
                    break;
            }
        }

        private void Eventos()
        {
            BuscarProduto.Select();

            KeyDown += KeyDowns;
            ModoRapido.KeyDown += KeyDowns;
            BuscarProduto.KeyDown += KeyDowns;
            GridListaProdutos.KeyDown += KeyDowns;
            Quantidade.KeyDown += KeyDowns;

            Masks.SetToUpper(this);

            Load += (s, e) =>
            {
                AutoCompleteItens();

                Medidas.DataSource = new List<String> { "UN", "KG", "PC", "MÇ", "BD", "DZ", "GR", "L", "ML", "M", "M2", "ROLO", "CJ", "SC", "CX", "FD", "PAR", "PR", "KIT", "CNT", "PCT" };

                PedidoItem.impostos = true;
                new Controller.PedidoItem().GetDataTableItens(GridListaProdutos, _mNota.id_pedido);
                LoadTotais();
                ClearForms();
                BuscarProduto.Select();

                if (_mNota != null && !String.IsNullOrEmpty(_mNota.Status))
                {
                    progress5.Visible = false;
                    pictureBox1.Visible = false;
                    label9.Visible = false;
                }
            };

            ModoRapido.Click += (s, e) => AlterarModo();

            Next.Click += (s, e) =>
            {
                OpenForm.Show<TelaFrete>(this);
            };

            Back.Click += (s, e) => Close();

            addProduto.Click += (s, e) => LoadItens();

            BuscarProduto.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    LoadItens();
            };

            GridListaProdutos.DoubleClick += (s, e) =>
            {
                if (GridListaProdutos.SelectedRows.Count > 0)
                {
                    EditProduct.idPdt = Convert.ToInt32(GridListaProdutos.SelectedRows[0].Cells["ID"].Value);
                    EditProduct.nrItem = Convert.ToInt32(GridListaProdutos.SelectedRows[0].Cells["#"].Value);
                    EditProduct f = new EditProduct();
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        PedidoItem.impostos = true;
                        new Controller.PedidoItem().GetDataTableItens(GridListaProdutos, _mNota.id_pedido);
                    }
                }
            };

            GridListaProdutos.SelectionChanged += (s, e) =>
            {
                if (GridListaProdutos.SelectedRows.Count > 0)
                {
                    AlterarImposto.Text = "Alterar Imposto (Item: " + GridListaProdutos.SelectedRows[0].Cells["#"].Value + ")";
                    AlterarImposto.Refresh();
                }
            };

            Preco.TextChanged += (s, e) =>
            {
                TextBox txt = (TextBox)s;
                Masks.MaskPrice(ref txt);
            };

            Preco.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    LoadItens();
            };

            Quantidade.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (String.IsNullOrEmpty(BuscarProduto.Text)) BuscarProduto.Focus();
                    else if (ModoRapAva == 1 && !String.IsNullOrEmpty(BuscarProduto.Text)) Preco.Focus();
                    else { LoadItens(); ClearForms(); }
                }
            };

            DescontoPorcentagem.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    LoadItens();
            };
            DescontoReais.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    LoadItens();
            };

            AlterarImposto.Click += (s, e) =>
            {
                if (GridListaProdutos.SelectedRows.Count > 0)
                {
                    AlterarImposto f = new AlterarImposto();
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        if (idImposto > 0)
                            new Controller.Imposto().SetImposto(Validation.ConvertToInt32(GridListaProdutos.SelectedRows[0].Cells["ID"].Value), idImposto, "NFe");

                        PedidoItem.impostos = true;
                        new Controller.PedidoItem().GetDataTableItens(GridListaProdutos, _mNota.id_pedido);
                    }
                }

                BuscarProduto.Select();
            };
        }
    }
}