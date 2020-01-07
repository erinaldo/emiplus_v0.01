﻿using SqlKata.Execution;
using System;

namespace Emiplus.Data.Database
{
    using Helpers;

    internal abstract class Model
    {
        protected Log Log;

        protected QueryFactory db = new Connect().Open();
        protected string Entity;
        protected object Objetos;

        protected Model(string entity)
        {
            Entity = entity;

            Log = new Log();
        }

        public Model SetDbOnline()
        {
            db = new ConnectOnline().Open();

            return this;
        }

        /// <summary>
        /// Alimenta a query Create e Update com os objetos
        /// </summary>
        /// <param name="obj">Objeto com os dados(data)</param>
        /// <returns></returns>
        public Model Data(object obj)
        {
            Objetos = obj;
            return this;
        }

        /// <summary>
        /// Busca todos os registros
        /// </summary>
        /// <returns></returns>
        public SqlKata.Query FindAll(string[] columns = null)
        {
            try {
                columns = columns ?? new[] { "*" };
                var data = db.Query(Entity).Select(columns);
                return data;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public int Count()
        {
            int count = 0;
            foreach (var data in db.Select("SELECT COUNT(ID) AS \"COUNT\" FROM " + Entity + " WHERE EXCLUIR = 0"))
            {
                count = Validation.ConvertToInt32(data.COUNT);
            }

            return count;
        }

        /// <summary>
        /// Monte sua query com esse método
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// Query().Select().Where().OrderBy() etc...
        /// Exemplos:
        /// Query().Get() - retorna todos registros
        /// Query().OrderBy().Get() - ordena os registros
        /// Query().WhereNull("ColunaNull").AsUpdate(Object) - Update
        /// Documentação https://sqlkata.com/docs/
        /// </code>
        /// </example>
        public SqlKata.Query Query()
        {
            try
            {
                var data = db.Query(Entity);
                return data;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        /// <summary>
        /// Buscar registro por ID
        /// </summary>
        /// <param name="id">ID do registro</param>
        /// <returns>Retorna objeto</returns>
        public SqlKata.Query FindById(int id)
        {
            try 
            {
                var data = db.Query(Entity).Where("ID", id);
                return data;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public int GetLastId()
        {
            try {
                int id_num = 0;
                foreach (var item in db.Select("select gen_id(GEN_" + Entity + "_ID, 0) as num from rdb$database;"))
                {
                    id_num = Validation.ConvertToInt32(item.NUM);
                }

                return id_num;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public int GetNextId()
        {
            int id_num = 0;

            foreach (var item in db.Select("select gen_id(GEN_" + Entity + "_ID, 0) as num from rdb$database;"))
            {
                id_num = Validation.ConvertToInt32(item.NUM);
            }

            return id_num + 1;
        }

        /// <summary>
        /// Executa o Insert();
        /// </summary>
        /// <returns>Retorna 1 para criado e 0 para não criado.</returns>
        public int Create()
        {
            try
            {
                var data = db.Query(Entity).Insert(Objetos);
                return data;
            }
            catch (Exception ex)
            {
                Log.Add(Entity, ex.Message + " | " + ex.InnerException, Log.LogType.fatal);
                return 0;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        /// <summary>
        /// Executa o Insert();
        /// </summary>
        /// <returns>Retorna o ID do insert.</returns>
        public int CreateGetId()
        {
            try
            {
                var data = db.Query(Entity).InsertGetId<int>(Objetos);
                return data;
            }
            catch (Exception ex)
            {
                Log.Add(Entity, ex.Message + " | " + ex.InnerException, Log.LogType.fatal);
                return 0;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        /// <summary>
        /// Executa o Update();
        /// </summary>
        /// <param name="id">Passar ID (Key)</param>
        /// <returns>Retorna int 1 para atualizado e 0 para não atualizado.</returns>
        public int Update(string key, int value)
        {
            try
            {
                var data = db.Query(Entity).Where(key, value).Update(Objetos);
                return data;
            }
            catch (Exception ex)
            {
                Log.Add(Entity, ex.Message + " | " + ex.InnerException, Log.LogType.fatal);
                return 0;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        /// <summary>
        /// Executa o Delete();
        /// </summary>
        /// <param name="id">Passar ID (key) para deletar o registro</param>
        /// <returns>Retorna int 1 para deletado e 0 para não deletado.</returns>
        public int Delete(string key, int value)
        {
            try
            {
                int data = db.Query(Entity).Where(key, value).Delete();
                return data;
            }
            catch (Exception ex)
            {
                Log.Add(Entity, ex.Message + " | " + ex.InnerException, Log.LogType.fatal);
                return 0;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }
}