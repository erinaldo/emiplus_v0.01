﻿using System;
using System.Windows.Forms;
using Emiplus.Data.Helpers;
using SqlKata.Execution;

namespace Emiplus.Controller
{
    public class Item : Data.Core.Controller
    {
        public void GetDataTable(DataGridView Table, string SearchText)
        {
            Table.ColumnCount = 6;

            Table.Columns[0].Name = "ID";
            Table.Columns[0].Visible = false;
            
            Table.Columns[1].Name = "Categoria";
            Table.Columns[1].Width = 150;
            
            Table.Columns[2].Name = "Cód. Personalizado";
            Table.Columns[2].Width = 200;

            Table.Columns[3].Name = "Descrição";
            Table.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Table.Columns[4].Name = "Custo";
            Table.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Table.Columns[4].Width = 100;

            Table.Columns[5].Name = "Venda";
            Table.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Table.Columns[5].Width = 100;

            Table.Rows.Clear();

            var produtos = new Model.Item();
            var categoria = new Model.Categoria();

            var search = "%" + SearchText + "%";

            var lista = produtos.Query()
                .LeftJoin("categoria", "categoria.id", "item.categoriaid")
                .Select("item.*", "categoria.nome as categoria")
                .Where("item.EXCLUIR", 0)
                .Where
                (
                    //q => q.WhereLike("item.nome", search, false).OrWhere("item.referencia", search, false).OrWhere("categoria.nome", search, false)
                    q => q.WhereLike("item.nome", search, false)
                )
                .OrderByDesc("item.criado")
                .Get();

            foreach (var item in lista)
            {
                Table.Rows.Add(
                    item.ID,                    
                    item.CATEGORIA,
                    item.REFERENCIA,
                    item.NOME,
                    Validation.Price(Validation.ConvertToDouble(item.VALORCOMPRA)),
                    Validation.Price(Validation.ConvertToDouble(item.VALORVENDA))
                );
            }
        }
    }
}