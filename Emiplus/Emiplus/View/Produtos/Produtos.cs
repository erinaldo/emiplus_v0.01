﻿using DotLiquid;
using Emiplus.Data.Core;
using Emiplus.Data.Helpers;
using Emiplus.Properties;
using Emiplus.View.Reports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Emiplus.View.Produtos
{
    public partial class Produtos : Form
    {
        public static int idPdtSelecionado { get; set; }
        private Controller.Item _controller = new Controller.Item();

        private IEnumerable<dynamic> dataTable;
        private BackgroundWorker WorkerBackground = new BackgroundWorker();

        private Timer timer = new Timer(Configs.TimeLoading);

        public Produtos()
        {
            InitializeComponent();
            Eventos();

            ToolHelp.Show("Pesquise pelo produto utilizando a Descrição, Cód. de Barras ou Categoria do Produto.", pictureBox6, ToolHelp.ToolTipIcon.Info, "Ajuda!");
        }

        private void DataTableStart()
        {
            GridListaProdutos.Visible = false;
            Loading.Size = new System.Drawing.Size(GridListaProdutos.Width, GridListaProdutos.Height);
            Loading.Visible = true;
            WorkerBackground.RunWorkerAsync();
        }

        private async void DataTable() => await _controller.SetTable(GridListaProdutos, null, search.Text);

        private void EditProduct(bool create = false)
        {
            if (create)
            {
                idPdtSelecionado = 0;
                OpenForm.Show<AddProduct>(this);
                return;
            }

            if (GridListaProdutos.SelectedRows.Count > 0)
            {
                idPdtSelecionado = Validation.ConvertToInt32(GridListaProdutos.SelectedRows[0].Cells["ID"].Value);
                OpenForm.Show<AddProduct>(this);
            }
        }

        private void KeyDowns(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    Support.UpDownDataGrid(false, GridListaProdutos);
                    e.Handled = true;
                    break;

                case Keys.Down:
                    Support.UpDownDataGrid(true, GridListaProdutos);
                    e.Handled = true;
                    break;

                case Keys.Enter:
                    EditProduct();
                    break;

                case Keys.Escape:
                    Close();
                    break;
            }
        }

        private void Eventos()
        {
            KeyDown += KeyDowns;
            KeyPreview = true;
            Masks.SetToUpper(this);

            Load += (s, e) =>
            {
                search.Select();
                DataTableStart();
            };

            btnAdicionar.Click += (s, e) => EditProduct(true);
            btnEditar.Click += (s, e) => EditProduct();
            GridListaProdutos.DoubleClick += (s, e) => EditProduct();

            label5.Click += (s, e) =>
            {
                Close();
            };

            btnExit.Click += (s, e) =>
            {
                Close();
            };

            search.TextChanged += (s, e) =>
            {
                timer.Stop();
                timer.Start();
                Loading.Visible = true;
                GridListaProdutos.Visible = false;
            };
            search.KeyPress += (s, e) => e.KeyChar = Char.ToUpper(e.KeyChar);
            //search.KeyDown += KeyDowns;
            //GridListaProdutos.KeyDown += KeyDowns;

            btnHelp.Click += (s, e) => Support.OpenLinkBrowser("https://ajuda.emiplus.com.br");

            using (var b = WorkerBackground)
            {
                b.DoWork += async (s, e) =>
                {
                    dataTable = await _controller.GetDataTable();
                };

                b.RunWorkerCompleted += async (s, e) =>
                {
                    await _controller.SetTable(GridListaProdutos, dataTable);

                    Loading.Visible = false;
                    GridListaProdutos.Visible = true;
                };
            }

            timer.AutoReset = false;
            timer.Elapsed += (s, e) => search.Invoke((MethodInvoker)delegate
            {
                DataTable();
                Loading.Visible = false;
                GridListaProdutos.Visible = true;
            });

            search.Enter += async (s, e) =>
            {
                await Task.Delay(100);
                DataTable();
            };

            imprimir.Click += async (s, e) => await RenderizarAsync();
        }

        private async Task RenderizarAsync()
        {
            IEnumerable<dynamic> dados = await _controller.GetDataTable(search.Text);

            ArrayList data = new ArrayList();
            foreach (var item in dados)
            {
                data.Add(new
                {
                    ID = item.ID,
                    NOME = item.NOME,
                    REFERENCIA = item.REFERENCIA,
                    CODEBARRAS = item.CODEBARRAS,
                    CUSTO = Validation.FormatPrice(Validation.ConvertToDouble(item.VALORCOMPRA)),
                    VENDA = Validation.FormatPrice(Validation.ConvertToDouble(item.VALORVENDA)),
                    ESTOQUEATUAL = item.ESTOQUEATUAL,
                    CATEGORIA = item.CATEGORIA
                });
            }

            var html = Template.Parse(File.ReadAllText($@"{Program.PATH_BASE}\html\Produtos.html"));
            var render = html.Render(Hash.FromAnonymousObject(new
            {
                INCLUDE_PATH = Program.PATH_BASE,
                URL_BASE = Program.PATH_BASE,
                Data = data,
                NomeFantasia = Settings.Default.empresa_nome_fantasia,
                Logo = Settings.Default.empresa_logo,
                Emissao = DateTime.Now.ToString("dd/MM/yyyy")
            }));

            Browser.htmlRender = render;
            var f = new Browser();
            f.ShowDialog();
        }
    }
}