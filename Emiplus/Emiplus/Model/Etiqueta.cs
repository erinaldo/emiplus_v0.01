﻿namespace Emiplus.Model
{
    using Data.Database;
    using Emiplus.Data.Helpers;
    using SqlKata;
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
        public int linha { get; set; }
        public int coluna { get; set; }
        #endregion 

        public bool Save(Etiqueta data)
        {
            if (data.Id == 0)
            {
                data.Criado = DateTime.Now;
                if (Data(data).Create() == 1)
                {
                    Alert.Message("Tudo certo!", "Etiqueta salva com sucesso.", Alert.AlertType.success);
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