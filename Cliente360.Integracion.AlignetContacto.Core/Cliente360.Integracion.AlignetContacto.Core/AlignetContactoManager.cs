using Cliente360.Integracion.AlignetContacto.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Cliente360.Config;
using System.Data.SqlClient;
using Release.Helper.Data.Core;
using Serilog;
using System.Linq;
using Cliente360.Integracion.AlignetContacto.Entities;
using System.Text.RegularExpressions;

namespace Cliente360.Integracion.AlignetContacto.Core
{
    public class AlignetContactoManager : BaseManager
    {
        private AlignetContacto.Core.Config _config;
        private IUnitOfWork _unitOfWork;
        private IUnitOfWorkSrx _unitOfWorkSrx;
        private string _pathScripts;

        protected override void InitVariables()
        {
            _config = AlignetConfig.Get;
            _config.APP = this.ReadVariables<AppSettings>(System.IO.Path.Combine(Environment.CurrentDirectory, "appsettings.json"));
             Log.Information("Iniciando conexión a BD");
            _unitOfWork = new UnitOfWork(new Db(new SqlConnection(_config.DB_CONNECTION_STRING)));
        }
        public void Start()
        {
            //string.Format("{0:dd/MM/yyyy}", x.FECHA_OPERACION)
            //string.Format("{0:dd/MM/yyyy}", x.FECHA_OPERACION)
            var transacciones = _unitOfWork.ObtenerTransaccionesSinContacto().ToList();
           

            //transacciones.RemoveAll(x=> x.ESTADO_OPERACION != 0 );
            var groups = transacciones.Select((transaccion, index) => new { transaccion, index }).GroupBy(g => g.index / 100, i => i.transaccion);

            foreach (var group in groups) 
            {
                var siebelComponet = new SiebelScrapperComponent(_config);
                var base_alignet = group.ToList();
                var lista = new List<SiebelContacto>();
                foreach (var transaccion in base_alignet) 
                {
                   var contacto= siebelComponet.GetContact(transaccion.EMAIL);
                    if (contacto != null)
                    {
                        contacto.RowId = Convert.ToString(transaccion.CODIGO_UNICO);
                        lista.Add(contacto);
                    }
                }

                var base_asl = lista.Select(x => new BaseAsl
                {
                    NEGOCIO = "Falabella",
                    CODIGO_UNICO = x.RowId,
                    RUT_CLIENTE = x.Social_Security_Number,
                    NOMBRE_CLIENTE = string.Format("{0} {1} {2}", x.First_Name,x.Last_Name,x.Maiden_Name),
                    TELEFONO = FormatTelephone(x.Alternate_Phone)
                }).ToList();

                var listaTransacciones = new List<BaseTransacciones>();

                _unitOfWork.AddTransacciones(listaTransacciones, base_asl);


            }
        }

        private string FormatTelephone(string alternate_Phone)
        {
            alternate_Phone = alternate_Phone == null ? string.Empty : alternate_Phone.Trim();
            alternate_Phone=  Regex.Replace(alternate_Phone, @"\s+", "");
            if (alternate_Phone.Length > 9) 
            {               
                var arrayTelefono = alternate_Phone.ToArray();
                 Array.Reverse(arrayTelefono);
                var arrayReverse = arrayTelefono.Take(9).ToArray();
                Array.Reverse(arrayReverse);
                alternate_Phone = string.Join("", arrayReverse);
            
            }
            int number = 0;
            int.TryParse(alternate_Phone, out number);
            var snumber = number == 0 ? string.Empty : Convert.ToString(number);
            alternate_Phone = snumber.Length == 9 ? snumber : string.Empty;

            return alternate_Phone;
        }
    }
}
