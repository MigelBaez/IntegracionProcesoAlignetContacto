using Cliente360.Integracion.AlignetContacto.Entities;
using Newtonsoft.Json;
using Release.Helper.Data.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Cliente360.Integracion.AlignetContacto.Core
{
    public class UnitOfWork : BaseUnitOfWorkAdo, IUnitOfWork
    {
        public UnitOfWork(IDb db) : base(db)
        {
        }



        public IEnumerable<BaseTransacciones> ObtenerTransaccionesSinContacto() 
        {
            var parameters = new Parameter[] { };
           return this.ExecuteReader<BaseTransacciones>("get_transacciones_sin_contacto", CommandType.StoredProcedure, ref parameters);
        }

        public void AddTransacciones(List<BaseTransacciones> baseAlignet, List<BaseAsl> baseAslContacto)
        {
            var jsonAlignetData = Newtonsoft.Json.JsonConvert.SerializeObject(baseAlignet);
            var jsonAslData = Newtonsoft.Json.JsonConvert.SerializeObject(baseAslContacto);
            var parameters = new Parameter[] {
            new Parameter { Name = "@json_transacciones_alignet", Value = jsonAlignetData },
            new Parameter { Name = "@json_asl", Value = jsonAslData }
            };

            this.ExecuteNonQuery("add_transacciones_alignet", CommandType.StoredProcedure, ref parameters);

        }
    }
}
