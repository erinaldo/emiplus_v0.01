﻿using Emiplus.Data.Core;
using Emiplus.Data.Helpers;
using System;
using System.Windows.Forms;

namespace Emiplus.View.Configuracoes
{
    public partial class Comercial : Form
    {
        public Comercial()
        {
            InitializeComponent();
            Eventos();
        }

        public void Eventos()
        {
            ToolHelp.Show("Atribua um limite para lançar descontos a este item. O Valor irá influenciar nos descontos em reais e porcentagens.", pictureBox11, ToolHelp.ToolTipIcon.Info, "Ajuda!");
            ToolHelp.Show("Essa opção ativa o controle do estoque, não permitindo vender itens com o estoque zerado.", pictureBox1, ToolHelp.ToolTipIcon.Info, "Ajuda!");

            Shown += (s, e) =>
            {
                if (!String.IsNullOrEmpty(IniFile.Read("RetomarVenda", "Comercial")))
                {
                    if (IniFile.Read("RetomarVenda", "Comercial") == "True")
                        retomarVendaInicio.Toggled = true;
                    else
                        retomarVendaInicio.Toggled = false;
                }

                if (!String.IsNullOrEmpty(IniFile.Read("ControlarEstoque", "Comercial")))
                {
                    if (IniFile.Read("ControlarEstoque", "Comercial") == "True")
                        btnControlarEstoque.Toggled = true;
                    else
                        btnControlarEstoque.Toggled = false;
                }

                if (!String.IsNullOrEmpty(IniFile.Read("LimiteDesconto", "Comercial")))
                    txtLimiteDesconto.Text = Validation.Price(Validation.ConvertToDouble(IniFile.Read("LimiteDesconto", "Comercial")));

                if (!String.IsNullOrEmpty(IniFile.Read("TrocasCliente", "Comercial")))
                {
                    if (IniFile.Read("TrocasCliente", "Comercial") == "True")
                        Trocas_01.Toggled = true;
                    else
                        Trocas_01.Toggled = false;
                }

                if (!String.IsNullOrEmpty(IniFile.Read("UserNoDocument", "Comercial")))
                {
                    if (IniFile.Read("UserNoDocument", "Comercial") == "True")
                        UserNoDocument.Toggled = true;
                    else
                        UserNoDocument.Toggled = false;
                }
            };

            retomarVendaInicio.Click += (s, e) =>
            {
                if (retomarVendaInicio.Toggled)
                    IniFile.Write("RetomarVenda", "False", "Comercial");
                else
                    IniFile.Write("RetomarVenda", "True", "Comercial");
            };

            btnControlarEstoque.Click += (s, e) =>
            {
                if (btnControlarEstoque.Toggled)
                    IniFile.Write("ControlarEstoque", "False", "Comercial");
                else
                    IniFile.Write("ControlarEstoque", "True", "Comercial");
            };

            Trocas_01.Click += (s, e) =>
            {
                if (Trocas_01.Toggled)
                    IniFile.Write("TrocasCliente", "False", "Comercial");
                else
                    IniFile.Write("TrocasCliente", "True", "Comercial");
            };

            UserNoDocument.Click += (s, e) =>
            {
                if (UserNoDocument.Toggled)
                    IniFile.Write("UserNoDocument", "False", "Comercial");
                else
                    IniFile.Write("UserNoDocument", "True", "Comercial");
            };

            txtLimiteDesconto.Leave += (s, e) => IniFile.Write("LimiteDesconto", Validation.ConvertToDouble(txtLimiteDesconto.Text).ToString(), "Comercial");
            txtLimiteDesconto.TextChanged += (s, e) =>
            {
                TextBox txt = (TextBox)s;
                Masks.MaskPrice(ref txt);
            };

            btnExit.Click += (s, e) => Close();
        }
    }
}