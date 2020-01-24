﻿using Emiplus.Data.Helpers;
using System.Windows.Forms;

namespace Emiplus.View.Common
{
    public partial class TelaConfigInicial : Form
    {
        public TelaConfigInicial()
        {
            InitializeComponent();
            Eventos();
        }

        public void Eventos()
        {
            preferencias.Click += (s, e) => OpenForm.ShowInPanel<Configuracoes.Preferencias>(panel);
            email.Click += (s, e) => OpenForm.ShowInPanel<Configuracoes.Email>(panel);
            sat.Click += (s, e) => OpenForm.ShowInPanel<Configuracoes.Cfesat>(panel);
            comercial.Click += (s, e) => OpenForm.ShowInPanel<Configuracoes.Comercial>(panel);
            impressao.Click += (s, e) => OpenForm.ShowInPanel<Configuracoes.Impressao>(panel);
            database.Click += (s, e) => OpenForm.ShowInPanel<Configuracoes.Database>(panel);
        }
    }
}
