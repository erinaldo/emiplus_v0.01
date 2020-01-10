﻿namespace Emiplus.Model
{
    using Data.Database;
    using Data.Helpers;
    using SqlKata;
    using System;
    using Valit;

    class Pessoa : Model
    {
        public Pessoa() : base("PESSOA") { }

        [Ignore]
        [Key("ID")]
        public int Id { get; set; }
        public string Tipo { get; set; }
        public int Excluir { get; set; }
        public DateTime Criado { get; private set; }
        public DateTime Atualizado { get; private set; }
        public DateTime Deletado { get; private set; }
        public string id_empresa { get; private set; } = Program.UNIQUE_ID_EMPRESA;
        public string Nome { get; set; }
        public string Fantasia { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public string Aniversario { get; set; }
        public string Pessoatipo { get; set; }
        public int Isento { get; set; }
        public string Transporte_placa { get; set; }
        public string Transporte_uf { get; set; }
        public string Transporte_rntc { get; set; }
        public int id_sync { get; set; }
        public string status_sync { get; set; }

        #region SQL Create
        //CREATE TABLE PESSOA
        //(
        //id integer not null primary key,
        //tipo integer not null,
        //excluir integer,
        //criado timestamp,
        //atualizado timestamp,
        //deletado timestamp,
        //empresaid varchar(255),
        //pessoasinc varchar(50),
        //padrao integer,
        //nome varchar(255),
        //fantasia varchar(255),
        //rg varchar(50),
        //cpf varchar(50),
        //aniversario date
        //);
        #endregion

        public bool Save(Pessoa data, bool message = true)
        {
            if(String.IsNullOrEmpty(data.Aniversario))
                data.Aniversario = null;

            if (data.Id == 0)
            {
                data.id_sync = Validation.RandomSecurity();
                data.status_sync = "CREATE";
                data.Criado = DateTime.Now;
                if (Data(data).Create() == 1)
                {
                    return true;
                }
                else
                {
                    if (message)
                        Alert.Message("Opss", "Erro ao criar, verifique os dados.", Alert.AlertType.error);

                    return false;
                }
            }
            else
            {
                if (ValidarDados(data))
                    return false;

                data.status_sync = "UPDATE";
                data.Atualizado = DateTime.Now;
                if (Data(data).Update("ID", data.Id) == 1)
                {
                    if (message)
                        Alert.Message("Tudo certo!", "Atualizado com sucesso.", Alert.AlertType.success);
                }
                else
                {
                    if (message)
                        Alert.Message("Opss", "Erro ao atualizar, verifique os dados.", Alert.AlertType.error);
                    return false;
                }
            }

            return true;
        }

        public bool Remove(int id)
        {
            var data = new { Excluir = 1, Deletado = DateTime.Now, status_sync = "UPDATE" };
            if (Data(data).Update("ID", id) == 1)
            {
                Alert.Message("Pronto!", "Removido com sucesso.", Alert.AlertType.info);
                return true;
            }

            Alert.Message("Opss!", "Não foi possível remover.", Alert.AlertType.error);
            return false;
        }

        /// <summary>
        /// <para>Valida os campos do Model</para>
        /// <para>Documentação: <see cref="https://valitdocs.readthedocs.io/en/latest/validation-rules/index.html"/> </para>
        /// </summary>
        /// <param name="data">Objeto com valor dos atributos do Model Item</param>
        /// <returns>Retorna booleano e Mensagem</returns>
        public bool ValidarDados(Pessoa data)
        {
            var result = ValitRules<Pessoa>
                .Create()
                .Ensure(m => m.CPF, _ => _
                    .Required()
                    .WithMessage("CPF ou CNPJ é obrigatório.")
                    .MinLength(11)
                    .WithMessage("CPF ou CNPJ inválido."))
                .For(data)
                .Validate();

            if (!result.Succeeded)
            {
                foreach (var message in result.ErrorMessages)
                {
                    Alert.Message("Opss!", message, Alert.AlertType.error);
                    return true;
                }
                return true;
            }

            return false;
        }
    }
}
