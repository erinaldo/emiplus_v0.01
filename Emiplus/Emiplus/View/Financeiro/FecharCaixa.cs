﻿using Emiplus.Data.Helpers;
using Emiplus.View.Common;
using SqlKata.Execution;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Emiplus.View.Financeiro
{
    public partial class FecharCaixa : Form
    {
        public static int idCaixa { get; set; }
        private Controller.Caixa _controllerCaixa = new Controller.Caixa();
        private Model.Caixa _modelCaixa = new Model.Caixa();
        private Model.CaixaMovimentacao _modelCaixaMov = new Model.CaixaMovimentacao();

        public FecharCaixa()
        {
            InitializeComponent();
            Eventos();
        }

        private void LoadData()
        {
            _modelCaixa = _modelCaixa.FindById(idCaixa).FirstOrDefault<Model.Caixa>();

            var Dinheiro = Validation.ConvertToDouble(_controllerCaixa.SumPagamento(_modelCaixa.Id, 1) + _modelCaixa.Saldo_Inicial - _controllerCaixa.SumSaidas(_modelCaixa.Id));
            txtSaldoDinheiro.Text = Validation.FormatPrice(Dinheiro, true);
            txtSaldoTotal.Text = Validation.FormatPrice(_controllerCaixa.SumSaldoFinal(idCaixa), true);
        }

        private void KeyDowns(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
            }
        }

        private void Eventos()
        {
            KeyDown += KeyDowns;
            KeyPreview = true;

            Load += (s, e) =>
            {
                LoadData();
            };

            btnFinalizar.Click += (s, e) =>
            {
                _modelCaixa.Tipo = "Fechado";
                _modelCaixa.Fechado = DateTime.Now;
                if (_modelCaixa.Save(_modelCaixa, false))
                {
                    Home.idCaixa = 0;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            };

            btnFinalizarImprimir.Click += (s, e) =>
            {
            };

            btnCancelar.Click += (s, e) => Close();
        }
    }
}