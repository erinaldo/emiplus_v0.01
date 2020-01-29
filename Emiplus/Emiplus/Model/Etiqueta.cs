﻿namespace Emiplus.Model
{
    using Data.Database;
    using Emiplus.Data.Helpers;
    using SqlKata;
    using SqlKata.Execution;
    using System;

    internal class Etiqueta : Model
    {
        public Etiqueta() : base("ETIQUETA")
        {
        }

        #region CAMPOS

        [Ignore]
        [Key("ID")]
        public int Id { get; set; }

        public int Excluir { get; set; }
        public DateTime Criado { get; private set; }
        public DateTime Atualizado { get; private set; }
        public DateTime Deletado { get; private set; }
        public string id_empresa { get; private set; }
        public int id_item { get; set; }
        public int quantidade { get; set; }
        public int id_sync { get; set; }
        public string status_sync { get; set; }
        #endregion CAMPOS



        public bool Clean()
        {
            Query().Delete();
            return true;
        }

        public bool Save(Etiqueta data)
        {
            data.id_empresa = Program.UNIQUE_ID_EMPRESA;

            if (data.Id == 0)
            {
                data.status_sync = "CREATE";
                data.id_sync = Validation.RandomSecurity();
                data.Criado = DateTime.Now;
                if (Data(data).Create() == 1)
                {
                    return true;
                }
                else
                {
                    Alert.Message("Opss", "Erro ao adicionar, verifique os dados.", Alert.AlertType.error);
                    return false;
                }
            }

            return true;
        }
    }
}