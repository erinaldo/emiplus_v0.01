﻿using Emiplus.View.Common;
using System.Windows.Forms;

namespace Emiplus.View.Comercial
{
    public partial class PedidoModalCancelItem : Form
    {
        private int IdPedidoItem = AddPedidos.IdPedidoItem;

        public PedidoModalCancelItem()
        {
            InitializeComponent();
            Events();
        }

        private void CancelItem()
        {
            if (IdPedidoItem <= 0)
            {
                return;
            }

            Model.PedidoItem item = new Model.PedidoItem();

            if (Home.pedidoPage != "Compras")
                new Controller.Estoque(IdPedidoItem, 0, Home.pedidoPage).Add().Item();
            else
                new Controller.Estoque(IdPedidoItem, 0, Home.pedidoPage).Remove().Item();

            item.Id = IdPedidoItem;
            item.Remove(IdPedidoItem);

            DialogResult = DialogResult.OK;
            Close();

        }

        private void KeyDowns(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
                case Keys.Enter:
                    CancelItem();
                    break;
            }
        }

        private void Events()
        {
            KeyDown += KeyDowns;
            nr.KeyDown += KeyDowns;
            btnContinuar.KeyDown += KeyDowns;

            btnContinuar.Click += (s, e) => CancelItem();
        }
    }
}
