﻿namespace Emiplus.Model
{
    using Data.Database;
    using Emiplus.Data.Helpers;
    using SqlKata;
    using System;

    class FormaPagamento : Model
    {
        public FormaPagamento() : base("FORMAPGTO") {}

        #region CAMPOS 

        [Ignore]
        [Key("ID")]
        public int Id { get; set; }

        public int Excluir { get; set; }
        public DateTime Criado { get; private set; }
        public DateTime Atualizado { get; private set; }
        public DateTime Deletado { get; private set; }
        public string id_empresa { get; private set; }
        public string Nome { get; set; }

        #endregion 
    }
}