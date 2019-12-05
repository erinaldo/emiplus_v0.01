﻿using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emiplus.View.Comercial
{
    public partial class OpcoesCfe : Form
    {
        public static int idPedido { get; set; } // id pedido

        private Model.Nota _modelNota = new Model.Nota();
        private BackgroundWorker WorkerBackground = new BackgroundWorker();

        private string _msg;

        public OpcoesCfe()
        {
            InitializeComponent();

            Eventos();
        }

        private void KeyDowns(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {                
                //case Keys.Escape:
                //    Close();
                //    break;
            }
        }

        /// <summary>
        /// Eventos do form
        /// </summary>
        public void Eventos()
        {
            //KeyPreview = true;
            //KeyDown += KeyDowns;

            Load += (s, e) =>
            {
                var checkNota = _modelNota.FindByIdPedidoAndTipo(idPedido, "CFe").FirstOrDefault<Model.Nota>();
                if (checkNota == null)
                    Emitir.Text = "Emitir";
                else
                {
                    if(checkNota.Status == "Autorizada")
                        Emitir.Text = "Cancelar";

                    if (checkNota.Status == "Cancelada")
                    {
                        Emitir.Text = "Cancelar";
                        Emitir.Enabled = false;
                    }
                }
            };

            //Emitir.KeyDown += KeyDowns;

            Emitir.Click += (s, e) =>
            {
                var checkNota = _modelNota.FindByIdPedidoAndTipo(idPedido, "CFe").FirstOrDefault<Model.Nota>();
                if (checkNota == null)
                {
                    _modelNota.Id = 0;
                    _modelNota.Tipo = "CFe";
                    _modelNota.id_pedido = idPedido;
                    _modelNota.Save(_modelNota, false);
                }

                WorkerBackground.RunWorkerAsync();
            };

            using (var b = WorkerBackground)
            {
                b.DoWork += async (s, e) =>
                {                    
                    _msg = new Controller.Fiscal().Emitir(idPedido, "CFe");
                };

                b.RunWorkerCompleted += async (s, e) =>
                {
                    retorno.Text = _msg;
                    Emitir.Enabled = true;
                };
            }
        }
    }
}