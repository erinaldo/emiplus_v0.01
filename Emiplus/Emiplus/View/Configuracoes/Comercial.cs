﻿using Emiplus.Data.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        /// <summary>
        /// Eventos do form
        /// </summary>
        public void Eventos()
        {
            btnExit.Click += (s, e) => Close();
        }
    }
}
