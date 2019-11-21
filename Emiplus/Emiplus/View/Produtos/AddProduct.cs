﻿using Emiplus.Data.Helpers;
using Emiplus.Model;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Collections;
using System.ComponentModel;

namespace Emiplus.View.Produtos
{
    public partial class AddProduct : Form
    {
        public static int idPdtSelecionado { get; set; }
        private Item _modelItem = new Item();
        private Controller.Item _controllerItem = new Controller.Item();

        public AddProduct()
        {
            InitializeComponent();
            Eventos();
        }
               
        private void Start()
        {
			ToolHelp.Show("Para selecionar a categoria do produto, a mesma deve estar cadastrada previamente." + Environment.NewLine + "Para cadastrar uma nova categoria acesse Produtos>Categorias>Adicionar.", pictureBox4, ToolHelp.ToolTipIcon.Info, "Ajuda!");
			ToolHelp.Show("Descreva seu produto... Lembre-se de utilizar as características do produto." + Environment.NewLine + "Utilize informações como Marca, Tamanho, Cor etc. ", pictureBox5, ToolHelp.ToolTipIcon.Info, "Ajuda!");
			ToolHelp.Show("Para selecionar o imposto do produto, o mesmo deve estar cadastrado previamente." + Environment.NewLine + "Para cadastrar um novo imposto acesse Produtos>Impostos>Adicionar. ", pictureBox6, ToolHelp.ToolTipIcon.Info, "Ajuda!");
			ToolHelp.Show("Para selecionar o fornecedor do produto, o mesmo deve estar cadastrado previamente." + Environment.NewLine + "Para cadastrar um novo Fornecedor acesse Produtos>Fornecedores>Adicionar.", pictureBox14, ToolHelp.ToolTipIcon.Info, "Ajuda!");
			ToolHelp.Show("Digite a quantidade mínima que você deve ter em estoque deste produto." , pictureBox7, ToolHelp.ToolTipIcon.Info, "Ajuda!");
			ToolHelp.Show("Digite a quantidade que você tem em estoque atualmente." + Environment.NewLine + "Para inserir a quantidade atual em estoque clique no botao Alterar Estoque." , pictureBox8, ToolHelp.ToolTipIcon.Info, "Ajuda!");

			ActiveControl = nome;

            //ToolHelp.Show("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s", pictureBox4, ToolHelp.ToolTipIcon.Info, "Ajuda!");

            var cat = new Categoria().FindAll().Where("tipo", "Produtos").WhereFalse("excluir").OrderByDesc("nome").Get();
            if (cat.Count() > 0)
            {
                Categorias.DataSource = cat;
                Categorias.DisplayMember = "NOME";
                Categorias.ValueMember = "ID";
            }

            var fornecedor = new Pessoa().FindAll().Where("tipo", "Fornecedores").WhereFalse("excluir").OrderByDesc("nome").Get();
            if (fornecedor.Count() > 0)
            {
                Fornecedor.DataSource = fornecedor;
                Fornecedor.DisplayMember = "NOME";
                Fornecedor.ValueMember = "ID";
            }

            Medidas.DataSource = new List<String> { "UN", "KG", "PC", "MÇ", "BD", "DZ", "GR", "L", "ML", "M", "M2", "ROLO", "CJ", "SC", "CX", "FD", "PAR", "PR", "KIT", "CNT", "PCT" };

            var imposto = new Model.Imposto().FindAll().WhereFalse("excluir").OrderByDesc("nome").Get();
            if (imposto.Count() > 0)
            {
                Impostos.DataSource = imposto;
                Impostos.DisplayMember = "NOME";
                Impostos.ValueMember = "ID";
            }

            var origens = new ArrayList();
            origens.Add(new ArrayTipo("0", "0 - Nacional, exceto as indicadas nos códigos 3, 4, 5 e 8"));
            origens.Add(new ArrayTipo("1", "1 - Estrangeira - Importação direta, exceto a indicada no código 6"));
            origens.Add(new ArrayTipo("2", "2 - Estrangeira - Adquirida no mercado interno, exceto a indicada no código 7"));
            origens.Add(new ArrayTipo("3", "3 - Nacional, mercadoria ou bem com Conteúdo de Importação superior a 40 % e inferior ou igual a 70 %"));
            origens.Add(new ArrayTipo("4", "4 - Nacional, cuja produção tenha sido feita em conformidade com os processos produtivos básicos de que tratam as legislações citadas nos Ajustes"));
            origens.Add(new ArrayTipo("5", "5 - Nacional, mercadoria ou bem com Conteúdo de Importação inferior ou igual a 40 %"));
            origens.Add(new ArrayTipo("6", "6 - Estrangeira - Importação direta, sem similar nacional, constante em lista da CAMEX e gás natural"));
            origens.Add(new ArrayTipo("7", "7 - Estrangeira - Adquirida no mercado interno, sem similar nacional, constante lista CAMEX e gás natural."));
            origens.Add(new ArrayTipo("8", "8 - Nacional, mercadoria ou bem com Conteúdo de Importação superior a 70 %"));

            Origens.DataSource = origens;
            Origens.DisplayMember = "Nome";
            Origens.ValueMember = "Id";
        }

        private class ArrayTipo
        {
            public string Id { get; set; }
            public string Nome { get; set; }

            public ArrayTipo(string Id, string Nome)
            {
                this.Id = Id;
                this.Nome = Nome;
            }
        }

        private void LoadData()
        {
            _modelItem = _modelItem.FindById(idPdtSelecionado).First<Item>();

            nome.Text = _modelItem?.Nome ?? "";
            codebarras.Text = _modelItem?.CodeBarras ?? "";
            referencia.Text = _modelItem?.Referencia ?? "";
            valorcompra.Text = Validation.Price(_modelItem.ValorCompra);
            valorvenda.Text = Validation.Price(_modelItem.ValorVenda);

            if (_modelItem.Medida == "KG")
            {
                estoqueminimo.Text = _modelItem.EstoqueMinimo.ToString();
                estoqueatual.Text = Validation.FormatNumberKilo(_modelItem.EstoqueAtual);
            }
            else
            {
                estoqueminimo.Text = _modelItem.EstoqueMinimo.ToString();
                estoqueatual.Text = Validation.FormatNumberUnidade(_modelItem.EstoqueAtual);
            }

            Impostos.SelectedValue = _modelItem.Impostoid;

            cest.Text = _modelItem?.Cest ?? "";
            ncm.Text = _modelItem?.Ncm ?? "";

            if (_modelItem.Origem != null)
                Origens.SelectedValue = _modelItem.Origem;

            if (_modelItem.Medida != null)
                Medidas.SelectedItem = _modelItem.Medida;

            Categorias.SelectedValue = _modelItem.Categoriaid;

            Fornecedor.SelectedValue = _modelItem.Fornecedor;

            aliq_federal.Text = Validation.Price(_modelItem.AliqFederal);
            aliq_estadual.Text = Validation.Price(_modelItem.AliqEstadual);
            aliq_municipal.Text = Validation.Price(_modelItem.AliqMunicipal);

            DataTableEstoque();
        }

        private void Save()
        {
            _modelItem.Id = idPdtSelecionado;
            _modelItem.Nome = nome.Text;
            _modelItem.CodeBarras = codebarras.Text;
            _modelItem.Referencia = referencia.Text;
            _modelItem.ValorCompra = Validation.ConvertToDouble(valorcompra.Text);
            _modelItem.ValorVenda = Validation.ConvertToDouble(valorvenda.Text);
            _modelItem.EstoqueMinimo = Validation.ConvertToDouble(estoqueminimo.Text);
            _modelItem.Medida = Medidas.Text;

            _modelItem.Cest = cest.Text;
            _modelItem.Ncm = ncm.Text;
            _modelItem.AliqFederal = Validation.ConvertToDouble(aliq_federal.Text);
            _modelItem.AliqEstadual = Validation.ConvertToDouble(aliq_estadual.Text);
            _modelItem.AliqMunicipal = Validation.ConvertToDouble(aliq_municipal.Text);

            if (Categorias.SelectedValue != null)
                _modelItem.Categoriaid = (int)Categorias.SelectedValue;

            if (Fornecedor.SelectedValue != null)
                _modelItem.Fornecedor = (int)Fornecedor.SelectedValue;

            if (Impostos.SelectedValue != null)
                _modelItem.Impostoid = (int)Impostos.SelectedValue;

            if (Origens.SelectedValue != null)
                _modelItem.Origem = Origens.SelectedValue.ToString();

            if (_modelItem.Save(_modelItem))
                Close();
        }

        private void DataTableEstoque() => _controllerItem.GetDataTableEstoque(listaEstoque, idPdtSelecionado);

        private void Eventos()
        {
            Load += (s, e) =>
            {
                Start();

                idPdtSelecionado = Produtos.idPdtSelecionado; // Recupera ID selecionado
                if (idPdtSelecionado > 0)
                {
                    LoadData();
                }
                else
                {
                    _modelItem.Id = idPdtSelecionado;
                    _modelItem.Nome = "Novo Produto";
                    if (_modelItem.Save(_modelItem, false))
                    {
                        idPdtSelecionado = _modelItem.GetLastId();
                        LoadData();
                    }
                    else
                    {
                        Alert.Message("Opss", "Erro ao criar.", Alert.AlertType.error);
                        Close();
                    }
                }
            };

            label6.Click += (s, e) => Close();
            btnExit.Click += (s, e) =>
            {
                if (nome.Text == "Novo Produto")
                {
                    if (MessageBox.Show($"Esse produto não foi editado, deseja deletar?", "Atenção!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        var data = _modelItem.Remove(idPdtSelecionado);
                        if (data)
                            Close();
                    }
                }

                Close();
            };

            btnSalvar.Click += (s, e) => Save();
            btnRemover.Click += (s, e) => 
            {
                var data = _modelItem.Remove(idPdtSelecionado);
                if (data)
                    Close();
            };

            btnEstoque.Click += (s, e) =>
            {
                AddEstoque f = new AddEstoque();
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LoadData();

                    estoqueminimo.Focus();
                    DataTableEstoque();
                }
            };

            referencia.KeyPress += (s, e) => Masks.MaskOnlyNumberAndChar(s, e);
            valorcompra.TextChanged += (s, e) =>
            {
                TextBox txt = (TextBox)s;
                Masks.MaskPrice(ref txt);
            };

            valorvenda.TextChanged += (s, e) =>
            {
                TextBox txt = (TextBox)s;
                Masks.MaskPrice(ref txt);
            };
            estoqueminimo.KeyPress += (s, e) => Masks.MaskDouble(s, e);

            btnHelp.Click += (s, e) => Support.OpenLinkBrowser("https://ajuda.emiplus.com.br");
        }
    }
}