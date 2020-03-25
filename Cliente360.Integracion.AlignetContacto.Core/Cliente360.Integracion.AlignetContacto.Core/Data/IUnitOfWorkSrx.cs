using Release.Helper.Data.ICore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Cliente360.Integracion.AlignetContacto.Core.Data
{
    public interface IUnitOfWorkSrx : IBaseOracleUnitOfWorkAdo 
    {
        List<T> GetData<T>(string sqlText, CommandType commandType = CommandType.StoredProcedure) where T : class;
        List<T> GetDataSrx<T>(string sqlText, List<string> fieldIn, CommandType commandType = CommandType.Text) where T : class;
    }
}
