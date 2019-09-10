﻿using Emiplus.Data.Helpers;
using System;
using System.Windows.Forms;

namespace Emiplus.View.Common
{
    public partial class TelaProdutosInicial : Form
    {
        public TelaProdutosInicial()
        {
            InitializeComponent();
        }

        private void Produtos_Click(object sender, EventArgs e)
        {
            OpenForm.Show<Produtos.Produtos>(this);
        }

        private void Categorias_Click(object sender, EventArgs e)
        {
            OpenForm.Show<Produtos.Categorias>(this);
        }

        private void TelaProdutosInicial_Load(object sender, EventArgs e)
        {

        }

        private void Impostos_Click(object sender, EventArgs e)
        {
            OpenForm.Show<Produtos.Impostos>(this);
        }
    }
}
