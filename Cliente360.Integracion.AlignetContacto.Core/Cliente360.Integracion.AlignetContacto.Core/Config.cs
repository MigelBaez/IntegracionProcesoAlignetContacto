using System;
using System.Collections.Generic;

namespace Cliente360.Integracion.AlignetContacto.Core
{
    public class Config
    {
        public string DB_CONNECTION_STRING { get; set; }
        public string DB_CONNECTION_STRING_SRX { get; set; }
        public AppSettings APP { get; set; }

    }
    public class AlignetConfig
    {
        private static Config _settings;

        public static Config Get
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Config
                    {
                        DB_CONNECTION_STRING = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"),
                        DB_CONNECTION_STRING_SRX = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_SRX")
                    };
                }

                return _settings;
            }
        }
    }

    public class AppSettings
    {
        public AppSettings()
        {
           
        }
        public string USUARIO_SIEBEL { get; set; }
        public string PASSWORD_SIEBEL { get; set; }
        public string URL_SIEBEL { get; set; }
    }
}
