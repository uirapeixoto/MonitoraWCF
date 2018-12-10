using System;
using System.Net;
using System.Linq;

namespace MonitoraWCF.Helper
{
    public class CheckService
    {
        private string _endereco;
        private string _descricao;
        private string _erro;
        private HttpStatusCode _status;

        private string Endereco {
            get { return _endereco; }
            set { _endereco = value; }
        }

        private string Descricao
        {
            get { return _descricao; }
            set { _descricao = value; }
        }
        public string Erro
        {
            get { return _erro; }
            set { _erro = value; }
        }

        public HttpStatusCode Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public CheckService(string endereco)
        {
            _endereco = endereco;
        }

        public void StatusServico()
        {
            string strRequestWSDL;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_endereco);
            // 5 segundos
            request.Timeout = 5000;
            try
            {
                WebResponse oWebResponse = request.GetResponse();
                strRequestWSDL = $@"{_endereco.Trim()}?wsdl";
                oWebResponse = WebRequest.Create(strRequestWSDL).GetResponse();
                if (oWebResponse.ContentType.ToUpper() != "TEXT/XML; CHARSET=UTF-8")
                {
                    Erro = "Erro: Não é xml válido.";
                }
                Status = ((HttpWebResponse)oWebResponse).StatusCode;
            }
            catch (TimeoutException ex)
            {
                Erro += $@"Erro de TimeOut: {ex.Message}";
                if(ex.InnerException != null && ex.InnerException.Message != null)
                {
                    _erro += $@"<b>Inner Exception: {ex.InnerException.Message}<b/>";
                }

                Status = HttpStatusCode.RequestTimeout;

            }
            catch(WebException ex)
            {
                HttpStatusCode status;
                var webResponse = ex.Response as System.Net.HttpWebResponse;
                if (Enum.TryParse(webResponse.StatusCode.ToString(), true, out  status)){
                    Status = status;
                }
                else
                {
                    Status = HttpStatusCode.BadRequest;
                }

                Erro += $@"<b>Inner Exception: {ex.Message}<b/>";
                
            }
        }
    }
}
