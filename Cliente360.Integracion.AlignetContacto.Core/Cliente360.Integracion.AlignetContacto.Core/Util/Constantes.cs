using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente360.Integracion.AlignetContacto.Util
{
    public class Constantes
    {
        public const string NombreOrganizacion = "Falabella - Peru";
        public const string NombreCanalEntrada = "Amanda";
        public static string[] estados_rechazados = new string[] { "Linea Rechazada por Falta Stock en Tienda", "Rechazo SAC", "Orden con Rechazo Admin. Call Center", "Orden con Rechazo POS" };
        public static string[] estados_transaccion = new string[] { "Depositado", "Autorizado" };
        public static string siebelViewName = "CRM2014 FAL All Contacts across Organizations";
        public static string siebelAppletName = "CRM2014 FAL Contact List Applet";
        public static string siebelSWEIPS =  "@0`0`1`0``3``Command`*Browser Applet* *ExecuteQuery*CRM2014 FAL Contact List Applet* `";

    }
}
