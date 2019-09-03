﻿using Emiplus.Data.Helpers;
using SqlKata.Execution;
using System;
using System.Windows.Forms;
using Emiplus.View.Common;

namespace Emiplus.View.Comercial
{
    using Model;

    public partial class AddClientes : Form
    {
        private Controller.Pessoa _controller = new Controller.Pessoa();
        private Pessoa _modelPessoa = new Pessoa();
        private string page = TelaComercialInicial.page;
        private int Id = Clientes.Id;

        public AddClientes()
        {
            InitializeComponent();
            label6.Text = page;
            label1.Text = "Adicionar " + page;
            label1.Left = 288;
            pictureBox2.Left = 265;

            if (page == "Fornecedores")
            {
                label1.Left = 322;
                pictureBox2.Left = 299;
            }

            tabControl1.TabPages.Remove(tabTransporte); // Tab 'Transporte'
            if (page == "Transportadoras")
            {
                label1.Left = 338;
                pictureBox2.Left = 315;
                tabControl1.TabPages.Add(tabTransporte);
            }

            if (Id > 0)
            {
                tabControl1.Visible = true;
                btnRemover.Visible = true;
                LoadData();
            }
        }

        private void DataTableAddress()
        {
            _controller.GetDataTableEnderecos(ListaEnderecos);
        }

        private void LoadData()
        {
            _modelPessoa = _modelPessoa.FindById(Id).First<Pessoa>();

            nomeRS.Text = _modelPessoa.Nome;
            nomeFantasia.Text = _modelPessoa.Fantasia;
            nascimento.Text = _modelPessoa.Aniversario;
            cpfCnpj.Text = _modelPessoa.CPF;
            rgIE.Text = _modelPessoa.RG;
        }

        private void BtnAdicionarEndereco_Click(object sender, EventArgs e)
        {
            OpenForm.ShowInPanel<AddClienteEndereco>(panelEnderecos);
        }

        private void Label6_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnAdicionarContato_Click(object sender, EventArgs e)
        {
            OpenForm.ShowInPanel<AddClienteContato>(panelContatos);
        }

        private void AddClientes_Load(object sender, EventArgs e)
        {
            DataTableAddress();
        }

        private void AddClientes_Activated(object sender, EventArgs e)
        {
            DataTableAddress();
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            Support.OpenLinkBrowser("https://www.google.com.br");
        }

        private void BtnRemover_Click(object sender, EventArgs e)
        {
            if (_modelPessoa.Remove(Id))
                Close();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            _modelPessoa.Id = Id;
            _modelPessoa.Tipo = page;
            _modelPessoa.Nome = nomeRS.Text;
            _modelPessoa.Fantasia = nomeFantasia.Text;
            _modelPessoa.Aniversario = nascimento.Text;
            _modelPessoa.CPF = cpfCnpj.Text;
            _modelPessoa.RG = rgIE.Text;

            if (_modelPessoa.Save(_modelPessoa))
                tabControl1.Visible = true;
        }

        private void BtnEditarContato_Click(object sender, EventArgs e)
        {

        }

        private void BtnEditarEndereco_Click(object sender, EventArgs e)
        {

        }
    }
}