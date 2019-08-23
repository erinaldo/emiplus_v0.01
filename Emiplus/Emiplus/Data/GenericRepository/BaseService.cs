﻿using Emiplus.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emiplus.Data.GenericRepository
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        IUnitOfWork unitOfWork = new ContextoData();
        IBaseRepository<T> _repository;

        public BaseService()
        {
            _repository = new BaseRepository<T>(unitOfWork);
        }

        public T Find(int id)
        {
            return _repository.Find(id);
        }

        public IQueryable<T> List()
        {
            return _repository.List();
        }

        public void Add(T item)
        {
            _repository.Add(item);
            unitOfWork.Save();
        }

        public void Remove(T item)
        {
            _repository.Remove(item);
            unitOfWork.Save();
        }

        public void Edit(T item)
        {
            _repository.Edit(item);
            unitOfWork.Save();
        }

        public void Dispose()
        {
            //_repository.Dispose();
        }
    }
}
