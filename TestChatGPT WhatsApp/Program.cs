using System;
using System.IO;
using System.Net;
using System.Text;


class Program
{
    static void Main(string[] args)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("https://gupshopciistec-api.herokuapp.com/incoming/");
        listener.Start();
        Console.WriteLine("Servidor iniciado en el puerto 3000");
        
        while (true)
        {
            var context = listener.GetContext();
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == "POST")
            {
                using (var reader = new StreamReader(request.InputStream))
                {
                    var body = reader.ReadToEnd();
                    dynamic message = Newtonsoft.Json.JsonConvert.DeserializeObject(body);
                    if (message != null) 
                    { 
                        if (message.type == "message")
                        {
                            if (message.text == "Hola")
                            {
                                SendWhatsAppMessage(message.from, "Hola! ¿En qué puedo ayudarte?");
                            }
                            else
                            {
                                SendWhatsAppMessage(message.from, "Lo siento, no he entendido lo que quieres decir.");
                            }
                        }
                    }
                }
            }

            response.Close();
        }
    }

    static void SendWhatsAppMessage(string to, string message)
    {
        string apiKey = "tnii3lrqvwkdav7giiwoedryjoyzaq40"; // API Key proporcionada por Gupshup
        string phoneNumber = "917834811114"; // Número de teléfono de Gupshup
        string url = "http://api.gupshup.io/sm/api/v1/msg";
        dynamic data = new
        {
            to = to,
            text = message,
            channel = "whatsapp",
            src = phoneNumber,
            apikey = apiKey
        };
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.ContentLength = bytes.Length;
        using (var stream = request.GetRequestStream())
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        var response = (HttpWebResponse)request.GetResponse();
        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Console.WriteLine("Response: " + responseString);
    }
}
