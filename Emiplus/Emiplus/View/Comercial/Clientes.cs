﻿using DotLiquid;
using Emiplus.Data.Core;
using Emiplus.Data.Helpers;
using Emiplus.Properties;
using Emiplus.View.Common;
using Emiplus.View.Reports;
using SqlKata.Execution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Emiplus.View.Comercial
{
    /// <summary>
    /// Responsavel por Clientes, Fornecedores e Transportadoras
    /// </summary>
    public partial class Clientes : Form
    {
        public static int Id { get; set; }
        private Controller.Pessoa _controller = new Controller.Pessoa();

        private IEnumerable<dynamic> dataTable;
        private BackgroundWorker WorkerBackground = new BackgroundWorker();

        private Timer timer = new Timer(Configs.TimeLoading);
        public List<int> listProdutos = new List<int>();

        public Clientes()
        {
            InitializeComponent();
            Eventos();

            label1.Text = Home.pessoaPage + ":";
            label6.Text = Home.pessoaPage;

            if (Home.pessoaPage == "Fornecedores")
            {
                pictureBox2.Image = Properties.Resources.box;
                label8.Text = "Produtos";
                label2.Text = "Gerencie os Fornecedores da sua empresa aqui! Adicione, edite ou delete um Fornecedor.";
            }
            else if (Home.pessoaPage == "Transportadoras")
            {
                pictureBox2.Image = Properties.Resources.box;
                label8.Text = "Produtos";
                label2.Text = "Gerencie as Transportadoras da sua empresa aqui! Adicione, edite ou delete uma Transportadora.";
            }
        }

        private async void DataTable()
        {
            await SetContentTableAsync(GridLista, null, search.Text);
            dynamic totalRegistros = new Model.Pessoa().Query().SelectRaw("COUNT(ID) as TOTAL").Where("Excluir", 0).Where("Tipo", Home.pessoaPage).FirstOrDefault();
            nrRegistros.Text = $"Exibindo: {GridLista.Rows.Count} de {totalRegistros.TOTAL ?? 0} registros";
        }
        private void EditClientes(bool create = false)
        {
            if (create)
            {
                Id = 0;
                OpenForm.Show<AddClientes>(this);
            }

            if (GridLista.SelectedRows.Count > 0)
            {
                Id = Validation.ConvertToInt32(GridLista.SelectedRows[0].Cells["ID"].Value);
                OpenForm.Show<AddClientes>(this);
            }
        }

        private void KeyDowns(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    Support.UpDownDataGrid(false, GridLista);
                    e.Handled = true;
                    break;

                case Keys.Down:
                    Support.UpDownDataGrid(true, GridLista);
                    e.Handled = true;
                    break;

                case Keys.Enter:
                    EditClientes();
                    break;

                case Keys.Escape:
                    Close();
                    break;
            }
        }

        private void SetHeadersTable(DataGridView Table)
        {
            Table.ColumnCount = 5;

            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, Table, new object[] { true });
            
            Table.RowHeadersVisible = false;

            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.HeaderText = "Selecione";
            checkColumn.Name = "Selecione";
            checkColumn.FlatStyle = FlatStyle.Standard;
            checkColumn.CellTemplate = new DataGridViewCheckBoxCell();
            checkColumn.Width = 60;
            Table.Columns.Insert(0, checkColumn);

            Table.Columns[1].Name = "ID";
            Table.Columns[1].Visible = false;

            Table.Columns[2].Name = "Nome / Razão social";
            Table.Columns[2].MinimumWidth = 150;

            Table.Columns[3].Name = "Nome Fantasia";
            Table.Columns[3].Width = 150;

            Table.Columns[4].Name = "CPF / CNPJ";
            Table.Columns[4].Width = 150;

            Table.Columns[5].Name = "RG / IE";
            Table.Columns[5].Width = 150;
        }

        private async Task SetContentTableAsync(DataGridView Table, IEnumerable<dynamic> Data = null, string SearchText = "")
        {
            Table.Rows.Clear();

            if (Data == null)
            {
                IEnumerable<dynamic> dados = await _controller.GetDataTableClientes(SearchText);
                Data = dados;
            }

            foreach (dynamic item in Data)
            {
                Table.Rows.Add(
                    false,
                    item.ID,
                    item.NOME,
                    item.FANTASIA,
                    item.CPF,
                    item.RG
                );
            }

            Table.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Eventos()
        {
            KeyDown += KeyDowns;
            KeyPreview = true;
            Masks.SetToUpper(this);

            Load += (s, e) =>
            {
                search.Select();
                SetHeadersTable(GridLista);

                DataTable();
            };

            search.TextChanged += (s, e) =>
            {
                timer.Stop();
                timer.Start();
                Loading.Visible = true;
                GridLista.Visible = false;
            };

            search.KeyDown += KeyDowns;

            btnAdicionar.Click += (s, e) => EditClientes(true);
            btnEditar.Click += (s, e) => EditClientes();
            GridLista.DoubleClick += (s, e) => EditClientes();
            GridLista.KeyDown += KeyDowns;

            btnExit.Click += (s, e) => Close();
            label5.Click += (s, e) => Close();
            label8.Click += (s, e) => Close();

            btnHelp.Click += (s, e) => Support.OpenLinkBrowser("https://ajuda.emiplus.com.br");

            using (var b = WorkerBackground)
            {
                b.DoWork += async (s, e) =>
                {
                    dataTable = await _controller.GetDataTableClientes();
                };

                b.RunWorkerCompleted += async (s, e) =>
                {
                    await SetContentTableAsync(GridLista, dataTable);

                    Loading.Visible = false;
                    GridLista.Visible = true;
                };
            }

            timer.AutoReset = false;
            timer.Elapsed += (s, e) => search.Invoke((MethodInvoker)delegate
            {
                DataTable();
                Loading.Visible = false;
                GridLista.Visible = true;
                Refresh();
            });

            btnMarcarCheckBox.Click += (s, e) =>
            {
                foreach (DataGridViewRow item in GridLista.Rows)
                {
                    if ((bool)item.Cells["Selecione"].Value == true)
                    {
                        item.Cells["Selecione"].Value = false;
                        btnMarcarCheckBox.Text = "Marcar Todos";
                        btnRemover.Visible = false;
                        btnEditar.Enabled = true;
                        btnAdicionar.Enabled = true;
                    }
                    else
                    {
                        item.Cells["Selecione"].Value = true;
                        btnMarcarCheckBox.Text = "Desmarcar Todos";
                        btnRemover.Visible = true;
                        btnEditar.Enabled = false;
                        btnAdicionar.Enabled = false;
                    }
                }
            };

            btnRemover.Click += (s, e) =>
            {
                listProdutos.Clear();
                foreach (DataGridViewRow item in GridLista.Rows)
                    if ((bool)item.Cells["Selecione"].Value == true)
                        listProdutos.Add(Validation.ConvertToInt32(item.Cells["ID"].Value));

                var result = AlertOptions.Message("Atenção!", "Você está prestes a deletar os CLIENTES selecionados, continuar?", AlertBig.AlertType.warning, AlertBig.AlertBtn.YesNo);
                if (result)
                {
                    foreach (var item in listProdutos)
                        new Model.Pessoa().Remove(item, false);

                    DataTable();
                }

                btnMarcarCheckBox.Text = "Marcar Todos";
                btnRemover.Visible = false;
                btnEditar.Enabled = true;
                btnAdicionar.Enabled = true;
            };

            GridLista.CellClick += (s, e) =>
            {
                if (GridLista.Columns[e.ColumnIndex].Name == "Selecione")
                {
                    if ((bool)GridLista.SelectedRows[0].Cells["Selecione"].Value == false)
                    {
                        GridLista.SelectedRows[0].Cells["Selecione"].Value = true;
                        btnRemover.Visible = true;
                        btnEditar.Enabled = false;
                        btnAdicionar.Enabled = false;
                    }
                    else
                    {
                        GridLista.SelectedRows[0].Cells["Selecione"].Value = false;

                        bool hideBtns = false;
                        bool hideBtnsTop = true;
                        foreach (DataGridViewRow item in GridLista.Rows)
                            if ((bool)item.Cells["Selecione"].Value == true)
                            {
                                hideBtns = true;
                                hideBtnsTop = false;
                            }

                        btnRemover.Visible = hideBtns;
                        btnEditar.Enabled = hideBtnsTop;
                        btnAdicionar.Enabled = hideBtnsTop;
                    }
                }
            };

            GridLista.CellMouseEnter += (s, e) =>
            {
                if (e.ColumnIndex < 0 || e.RowIndex < 0)
                    return;

                var dataGridView = (s as DataGridView);
                if (GridLista.Columns[e.ColumnIndex].Name == "Selecione")
                    dataGridView.Cursor = Cursors.Hand;
            };

            GridLista.CellMouseLeave += (s, e) =>
            {
                if (e.ColumnIndex < 0 || e.RowIndex < 0)
                    return;

                var dataGridView = (s as DataGridView);
                if (GridLista.Columns[e.ColumnIndex].Name == "Selecione")
                    dataGridView.Cursor = Cursors.Default;
            };

            search.Enter += async (s, e) =>
            {
                await Task.Delay(100);
                DataTable();
            };

            imprimir.Click += async (s, e) => await RenderizarAsync();
        }

        private async Task RenderizarAsync()
        {
            IEnumerable<dynamic> dados = await _controller.GetDataTableClientes(search.Text);

            ArrayList data = new ArrayList();
            foreach (var item in dados)
            {
                data.Add(new
                {
                    ID = item.ID,
                    NOME = item.NOME,
                    FANTASIA = item.FANTASIA,
                    CPF = item.CPF,
                    RG = item.RG
                });
            }

            var html = Template.Parse(File.ReadAllText($@"{Program.PATH_BASE}\html\Pessoas.html"));
            var render = html.Render(Hash.FromAnonymousObject(new
            {
                INCLUDE_PATH = Program.PATH_BASE,
                URL_BASE = Program.PATH_BASE,
                Data = data,
                NomeFantasia = Settings.Default.empresa_nome_fantasia,
                Logo = Settings.Default.empresa_logo,
                Emissao = DateTime.Now.ToString("dd/MM/yyyy"),
                Titulo = Home.pessoaPage
            }));

            Browser.htmlRender = render;
            var f = new Browser();
            f.ShowDialog();
        }
    }
}