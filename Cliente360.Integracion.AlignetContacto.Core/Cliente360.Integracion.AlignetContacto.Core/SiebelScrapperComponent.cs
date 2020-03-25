using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Cliente360.Integracion.AlignetContacto.Util;

namespace Cliente360.Integracion.AlignetContacto.Core
{
    public class SiebelContacto
    {
        public string RowId { get; set; }
        public string ID_Type { get; set; }
        public string Social_Security_Number { get; set; }
        public string CRM_DV { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Maiden_Name { get; set; }
        public string CRM_Type_FAL { get; set; }
        public string Alternate_Phone { get; set; }
        public string Alternate_Email_Address { get; set; }
        public string Citizenship { get; set; }
        public string Status { get; set; }
    }

    public static class SweCommand
    {
        public class Type
        {
            public static readonly string ExecuteLogin = "ExecuteLogin";
            public static readonly string GotoView = "GotoView";
            public static readonly string NewQuery = "NewQuery";
            public static readonly string ExecuteQuery = "ExecuteQuery";


        }
    }
    public class SiebelScrapperComponent
    {
        RestClient _client;
        RestRequest request;
        private string _usuario;
        private string _password;
        private string _srn;
        private string _cookie;

        public SiebelScrapperComponent(AlignetContacto.Core.Config config)
        {
            _client = new RestClient(config.APP.URL_SIEBEL);
            _usuario = config.APP.USUARIO_SIEBEL;
            _password = config.APP.PASSWORD_SIEBEL;

            InitAuth();
        }

        public void InitAuth()
        {

            request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");

            Dictionary<string, string> parametersLogin = new Dictionary<string, string>();
            parametersLogin.Add("SWECmd", SweCommand.Type.ExecuteLogin);
            parametersLogin.Add("SWEUserName", _usuario);
            parametersLogin.Add("SWEPassword", _password);
            parametersLogin.Add("SWESetMarkup", "XML");
            AddParameters(parametersLogin);
            IRestResponse response = _client.Execute(request);
            _srn = GetSRN(response.Content);
            var cookie = response.Cookies.FirstOrDefault();
            _cookie = cookie != null ? cookie.Value : string.Empty;

        }

        public SiebelContacto GetContact(string email)
        {
            Dictionary<string, string> parametersGoToView = GetParameterGoToView();
            AddParametersWithCookie(parametersGoToView);
            while (true)
            {
                IRestResponse response = _client.Execute(request);
                if (response.Content.Contains("Target"))
                    break;
            }

            Dictionary<string, string> parametersNewQuery = GetParameterNewQuery();
            AddParametersWithCookie(parametersNewQuery);
            IRestResponse responseNewQuery = _client.Execute(request);
            if (responseNewQuery.Content.Contains("Completed"))
            {
                Dictionary<string, string> parametersExecuteQuery = GetParameterExecuteQuery(email);
                AddParametersWithCookie(parametersExecuteQuery);
                IRestResponse responseExecuteQuery = _client.Execute(request);
                if (responseExecuteQuery.Content.Contains("Completed"))
                {
                    var contactArray = GetContactData(responseExecuteQuery.Content, email);
                      
                   return contactArray;
                 
                }
            }
            return null;
        }

        private SiebelContacto GetContactData(string content,string email)
        {
            var mainArray = content.Split('`').ToArray();
         
            var lista = new List<SiebelContacto>();
            try
            { 
                var dataIndex = mainArray.Select((v,i) => new { value = v, index = i  });
                var indexValues = dataIndex.Where(e => e.value == "ValueArray");
                var indexs = indexValues.Select(x=> x.index + 1).ToArray();
                var values = dataIndex.Where(x=> indexs.Contains(x.index) ).Select(x=> x.value).ToArray();

                foreach (var dataArray in values) 
                {
                    var contactArray = new List<string>();
                    var dicValue = dataArray.ToArray();
                    while (dicValue.Length > 0)
                    {
                        int ilongitud = 0;
                        var valuesLongitud = dicValue.TakeWhile(e => e != '*');
                        var slongitud = string.Join("", valuesLongitud);
                        var dataArrayValue = dicValue.SkipWhile(e => e != '*').Skip(1).ToArray();
                        int.TryParse(slongitud, out ilongitud);
                        var valueArray = dataArrayValue.Take(ilongitud).ToArray();
                        var valueString = string.Join("", dataArrayValue);
                        valueString = valueString.Remove(0, ilongitud);
                        dicValue = valueString.ToArray();

                        var value = string.Join("", valueArray);
                        contactArray.Add(value);
                    }

                    var contacto = new SiebelContacto();
              

                    contacto.ID_Type = contactArray[0];
                    contacto.Social_Security_Number = contactArray[2];
                    contacto.CRM_DV = contactArray[3];
                    contacto.First_Name = contactArray[4];
                    contacto.Last_Name = contactArray[5];
                    contacto.Maiden_Name = contactArray[6];
                    contacto.CRM_Type_FAL = contactArray[7];
                    contacto.Alternate_Phone = contactArray[8];
                    contacto.Alternate_Email_Address = contactArray[9];
                    contacto.Citizenship = contactArray[10];
                    contacto.Status = contactArray[11];

               
                     

                    if (string.IsNullOrEmpty(contacto.ID_Type))
                        continue;

                    if (email.Equals(contacto.Alternate_Email_Address, StringComparison.OrdinalIgnoreCase))
                        lista.Add(contacto);
                }

                if (lista.Any()) 
                {

                    var groups = lista.Select(x=> new { Value= x }).GroupBy(y => new {Key= y.Value.Social_Security_Number,Value= y.Value });                  

                    if (groups.Count() > 1) 
                    {
                        Console.WriteLine(email);
                        return null;
                    }                     
                    else
                        return groups.FirstOrDefault().Select(x=> x.Value).FirstOrDefault();

                }
                else 
                {
                    return null;
                }

                
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        private Dictionary<string, string> GetParameterExecuteQuery(string email)
        {
            Dictionary<string, string> parametersExecuteQuery = new Dictionary<string, string>();
            parametersExecuteQuery.Add("SWEIPS", Constantes.siebelSWEIPS);
            parametersExecuteQuery.Add("s_1_2_20_0", "");
            parametersExecuteQuery.Add("s_1_2_79_0", "");
            parametersExecuteQuery.Add("s_1_2_17_0", "");
            parametersExecuteQuery.Add("s_1_2_101_0", "");
            parametersExecuteQuery.Add("s_1_2_100_0", "");
            parametersExecuteQuery.Add("s_1_2_21_0", "");
            parametersExecuteQuery.Add("s_1_2_132_0", "");
            parametersExecuteQuery.Add("s_1_2_19_0", "");
            parametersExecuteQuery.Add("s_1_2_46_0", email);
            parametersExecuteQuery.Add("s_1_2_18_0", "");
            parametersExecuteQuery.Add("s_1_2_81_0", "");
            parametersExecuteQuery.Add("SWECmd", "InvokeMethod");
            parametersExecuteQuery.Add("SWEVI", "");
            parametersExecuteQuery.Add("SWEView", Constantes.siebelViewName);
            parametersExecuteQuery.Add("SWEApplet", Constantes.siebelAppletName);
            parametersExecuteQuery.Add("SWEMethod", SweCommand.Type.ExecuteQuery);
            parametersExecuteQuery.Add("SWERowId", "");
            parametersExecuteQuery.Add("SWEReqRowId", "0");
            parametersExecuteQuery.Add("SWERowIds", "");
            parametersExecuteQuery.Add("SWEActiveApplet", Constantes.siebelAppletName);
            parametersExecuteQuery.Add("SWENeedContext", "true");
            parametersExecuteQuery.Add("SWEC", "20");
            parametersExecuteQuery.Add("SWERPC", "1");
            parametersExecuteQuery.Add("SRN", _srn);
            parametersExecuteQuery.Add("SWEActiveView", Constantes.siebelViewName);
            parametersExecuteQuery.Add("SWETS", Convert.ToString(GetSeconds()));
            parametersExecuteQuery.Add("SWESetMarkup", "HTML");

            return parametersExecuteQuery;
        }

        private Dictionary<string, string> GetParameterNewQuery()
        {
            Dictionary<string, string> parametersNewQuery = new Dictionary<string, string>();

            parametersNewQuery.Add("SWEField", "s_1_1_7_0");
            parametersNewQuery.Add("SWENeedContext", "true");
            parametersNewQuery.Add("SWER", "65535");
            parametersNewQuery.Add("SWESP", "false");
            parametersNewQuery.Add("SWEMethod", SweCommand.Type.NewQuery);
            parametersNewQuery.Add("SWECmd", "InvokeMethod");
            parametersNewQuery.Add("SWEDIC", "false");
            parametersNewQuery.Add("SWEReqRowId", "0");
            parametersNewQuery.Add("SWEView", Constantes.siebelViewName);
            parametersNewQuery.Add("SWEC", "9");
            parametersNewQuery.Add("SWEBID", "-1");
            parametersNewQuery.Add("SRN", _srn);
            parametersNewQuery.Add("SWEApplet", Constantes.siebelAppletName);
            parametersNewQuery.Add("SWEVI", "");
            parametersNewQuery.Add("SWERowId", "");
            parametersNewQuery.Add("SWERowIds", "");
            parametersNewQuery.Add("SWEActiveApplet", Constantes.siebelAppletName);
            parametersNewQuery.Add("SWERPC", "1");
            parametersNewQuery.Add("SWEActiveView", Constantes.siebelViewName);
            parametersNewQuery.Add("SWETS", Convert.ToString(GetSeconds()));
            parametersNewQuery.Add("SWESetMarkup", "HTML");
            return parametersNewQuery;
        }

        private Dictionary<string, string> GetParameterGoToView()
        {
            Dictionary<string, string> parametersGoToView = new Dictionary<string, string>();
            parametersGoToView.Add("SWECmd", SweCommand.Type.GotoView);
            parametersGoToView.Add("SWEView", Constantes.siebelViewName);
            parametersGoToView.Add("SWEKeepContext", "0");
            parametersGoToView.Add("SWENeedContext", "false");
            parametersGoToView.Add("SWERPC", "1");
            parametersGoToView.Add("SRN", _srn);
            parametersGoToView.Add("SWEC", "7");
            parametersGoToView.Add("SWESetMarkup", "HTML");
            return parametersGoToView;
        }

        public void AddParameters(Dictionary<string, string> values)
        {
            request.Parameters.Clear();

            foreach (var value in values)
            {
                request.AddParameter(value.Key, value.Value);
            }
        }

        public void AddParametersWithCookie(Dictionary<string, string> values)
        {
            request.Parameters.Clear();
            request.AddParameter("_sn", _cookie, ParameterType.Cookie);
            foreach (var value in values)
            {
                request.AddParameter(value.Key, value.Value);
            }
        }

        private static int GetSeconds()
        {
            DateTime myDate1 = new DateTime(1970, 1, 9, 0, 0, 00);
            TimeSpan myDateResult;
            DateTime myDate = DateTime.Now;
            myDateResult = myDate - myDate1;
            return (int)myDateResult.TotalSeconds;
        }
        private static string GetSRN(string value)
        {
            string valueReturn = string.Empty;
            string expresion = @"SRN=(?<SRN>\w+)";

            try
            {
                Regex rgx = new Regex(expresion);
                foreach (Match match in rgx.Matches(value))
                {


                    valueReturn = Convert.ToString(match.Groups["SRN"].Value);
                    if (!string.IsNullOrEmpty(valueReturn))
                        return valueReturn;

                }
            }
            catch (Exception ex)
            {

                valueReturn = string.Empty;
            }

            return valueReturn;
        }

    }

}
