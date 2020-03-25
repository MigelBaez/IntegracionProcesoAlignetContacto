using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cliente360.Integracion.AlignetContacto.Entities
{
    public class BaseAsl
    {
        public long ID { get; set; }
        public string CODIGO_UNICO {get;set;}
        public string NEGOCIO {get;set;}
        public string CANAL {get;set;}
        public string NUMERO_ORDEN_COMPRA {get;set;}
        public string RUT_CLIENTE {get;set;}
        public string NOMBRE_CLIENTE {get;set;}
        public string FECHA_COLOCACION {get;set;}
        public string NUMERO_TARJETA {get;set;}
        public string TIPO_TARJETA {get;set;}
        public string IMPORTE_PEDIDO {get;set;}
        public string PUNTOS {get;set;}
        public string FECHA_ENVIO_DESDE {get;set;}
        public string FECHA_ENVIO_HASTA {get;set;}
        public string DIRECCION_DESPACHO {get;set;}
        public string ESTADO_ORDEN_COMPRA {get;set;}
        public string NOVIOS {get;set;}
        public string NUMERO_FOLIO {get;set;}
        public string FECHA_PICKING {get;set;}
        public string MEDIO_PAGO {get;set;}
        public string METODO_ENVIO {get;set;}
        public string FUENTE_ABAST {get;set;}
        public string NRO_BOLETA_FACTURA {get;set;}
        public string REGION {get;set;}
        public string CIUDAD {get;set;}
        public string COMUNA {get;set;}
        public string NRO_OC_CHILECOMPRA {get;set;}
        public string CODIGO_AUTORIZACION {get;set;}
        public string FECHA_EXPIRACION_EFECTIVO {get;set;}
        public string CODIGO_CIP {get;set;}
        public string CODIGO_PAGO {get;set;}
        public string NRO_RESERVA {get;set;}
        public string RECAUDADOR {get;set;}
        public string LOCAL {get;set;}
        public string CAJA {get;set;}
        public string SECUENCIA {get;set;}
        public int ESTADO_OPERACION { get; set; }
        public DateTime FECHA_OPERACION { get; set; }

        public string TELEFONO { get; set; }
    }
}
