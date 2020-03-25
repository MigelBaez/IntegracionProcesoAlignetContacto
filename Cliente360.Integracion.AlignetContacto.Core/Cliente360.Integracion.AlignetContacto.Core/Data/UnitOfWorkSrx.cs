using Oracle.ManagedDataAccess.Client;
using Release.Helper.Data.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;

namespace Cliente360.Integracion.AlignetContacto.Core.Data
{
    public class UnitOfWorkSrx : BaseOracleUnitOfWorkAdo, IUnitOfWorkSrx
    {
        public UnitOfWorkSrx(IDb db) : base(db)
        {
        }

        public List<T> GetData<T>(string sqlText, CommandType commandType = CommandType.StoredProcedure) where T : class
        {
            var param = new OracleParameter[] { };
            var items = this.ExecuteReader<T>(sqlText, commandType, ref param);

            return items.ToList();
        }


        public List<T> GetDataSrx<T>(string sqlText, List<string> fieldIn, CommandType commandType = CommandType.Text) where T : class
        {
            var dtFinal = new List<T>();
            fieldIn = fieldIn.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            int count = fieldIn.Count;
            var param = new OracleParameter[] { };
            string sqlTextOriginal = sqlText;
            var groups = fieldIn.Select((folio, index) => new { folio, index }) .GroupBy(g => g.index / 999, i => i.folio);

            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Connection = (OracleConnection)Connection;

                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                foreach (var group in groups) 
                {
                    string concat = string.Join(",", group);
                    string sqlTextFinal = string.Format("{0} ({1})", sqlTextOriginal, concat);
                    var items = this.ExecuteReader<T>(sqlTextFinal, commandType, ref param);
                    dtFinal.AddRange(items);
                }


            }


            return dtFinal;
        }

    }
}
