using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using mmisharp;
using Newtonsoft.Json;
using Discord.Webhook;
using System.Net;
using System.IO;
using System.Xml;
using System.ServiceModel.Syndication;

namespace AppGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MmiCommunication mmiC;
        private DiscordWebhookClient _client;
        private Tts t;

        public MainWindow()
        {
            InitializeComponent();

            _client = new DiscordWebhookClient(643897438297391124, "-Jpr2aw_HQeS5iLVhC0TQzc7d9y4tTAp55aZZgfBvJvDuKjjmfxYGgcEEcLe3lnnAwyF");

            mmiC = new MmiCommunication("localhost", 8000, "User1", "GUI");
            mmiC.Message += MmiC_Message;
            mmiC.Start();
            t = new Tts();
        }

        private void MmiC_Message(object sender, MmiEventArgs e)
        {
            var doc = XDocument.Parse(e.Message);
            var com = doc.Descendants("command").FirstOrDefault().Value;
            dynamic json = JsonConvert.DeserializeObject(com);
            bool wake = false;

            Console.WriteLine(json.recognized);
            if ((string)json.recognized[1].ToString() == "WAKE")
            {
                wake = true;
            }
            if (wake && json.recognized[3] != null)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    switch ((string)json.recognized[3].ToString())
                    {
                        case "ADD":
                            if((string)json.recognized[7].ToString() != null)
                            {
                                if ((string)json.recognized[5].ToString() == "TEXT_CHANNEL")
                                {
                                    _client.SendMessageAsync("!ADD_TEXT " + (string)json.recognized[7].ToString());
                                }
                                else if ((string)json.recognized[5].ToString() == "VOICE_CHANNEL")
                                {
                                    _client.SendMessageAsync("!ADD_VOICE " + (string)json.recognized[7].ToString());
                                }
                            }
                            else
                            {
                                if ((string)json.recognized[5].ToString() == "TEXT_CHANNEL")
                                {
                                    _client.SendMessageAsync("!ADD_TEXT");
                                }
                                else if ((string)json.subject.ToString() == "VOICE_CHANNEL")
                                {
                                    _client.SendMessageAsync("!ADD_VOICE");
                                }
                            }
                            break;
                        case "REMOVE":
                            if ((string)json.recognized[7].ToString() != null)
                            {
                                if ((string)json.recognized[5].ToString() == "TEXT_CHANNEL")
                                {
                                    _client.SendMessageAsync("!DEL " + (string)json.name);
                                }
                                else if ((string)json.recognized[5].ToString() == "VOICE_CHANNEL")
                                {
                                    _client.SendMessageAsync("!DEL " + (string)json.name);
                                }
                            }
                            else
                            {
                                if ((string)json.recognized[5].ToString() == "TEXT_CHANNEL")
                                {
                                    _client.SendMessageAsync("!DELT");
                                }
                                else if ((string)json.recognized[5].ToString() == "VOICE_CHANNEL")
                                {
                                    _client.SendMessageAsync("!DELV");
                                }
                            }
                            break;
                        case "KICK":
                            if ((string)json.recognized[5].ToString() != null)
                            {
                                _client.SendMessageAsync("!KICK " + (string)json.recognized[5].ToString());
                            }
                            else
                            {
                                _client.SendMessageAsync("Idenfique quem deseja calar");
                            }
                            break;
                        case "BAN":
                            if ((string)json.recognized[5].ToString() != null)
                            {
                                _client.SendMessageAsync("!BAN " + (string)json.recognized[5].ToString());
                            }
                            else
                            {
                                _client.SendMessageAsync("Idenfique quem deseja banir");
                            }
                            break;
                        case "UNBAN":
                            if ((string)json.recognized[5].ToString() != null)
                            {
                                _client.SendMessageAsync("!UNBAN " + (string)json.recognized[5].ToString());
                            }
                            else
                            {
                                _client.SendMessageAsync("Idenfique a quem deseja retirar o ban");
                            }
                            break;
                        case "MUTE":
                            if ((string)json.recognized[5].ToString() != null)
                            {
                                _client.SendMessageAsync("!MUTE " + (string)json.recognized[5].ToString());
                            }
                            else
                            {
                                _client.SendMessageAsync("Idenfique quem deseja calar");
                            }
                            break;
                        case "UNMUTE":
                            if ((string)json.recognized[5].ToString() != null)
                            {
                                _client.SendMessageAsync("!UNMUTE " + (string)json.recognized[5].ToString());
                            }
                            else
                            {
                                _client.SendMessageAsync("Idenfique quem deseja calar");
                            }
                            break;
                        case "INVITE":
                            if ((string)json.recognized[5].ToString() != null)
                            {
                                _client.SendMessageAsync("!INVITE " + (string)json.recognized[5].ToString());
                            }
                            else
                            {
                                _client.SendMessageAsync("Identifique quem deseja convidar");
                            }
                            break;
                        case "TELL":
                            if((string)json.recognized[5].ToString() == "JOKE")
                            {
                                _client.SendMessageAsync("Piada:\n" + (string)json.recognized[7].ToString());
                            }
                            else if ((string)json.recognized[5].ToString() == "NEWS")
                            {
                                string url = "http://feeds.ojogo.pt/OJ-Futebol";
                                XmlReader reader = XmlReader.Create(url);
                                SyndicationFeed feed = SyndicationFeed.Load(reader);
                                reader.Close();
                                String subject = "";
                                int i = 0;
                                foreach (SyndicationItem item in feed.Items)
                                {
                                    subject += item.Title.Text + ".\n";
                                    i++;
                                    if (i == 5)
                                    {
                                        break;
                                    }
                                }
                                t.Speak(subject);
                            }
                            else if ((string)json.recognized[5].ToString() == "WHEATER")
                            {
                                string URL = "http://api.openweathermap.org/data/2.5/weather?q=Aveiro&APPID=6fcebaa15d35d3672004b399373a1279&units=metric";
                                Console.WriteLine(URL);
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                                request.Method = "GET";
                                request.ContentType = "application/json";

                                try
                                {
                                    String temperatura = "";
                                    String info = "";
                                    WebResponse webResponse = request.GetResponse();
                                    using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                                    using (StreamReader responseReader = new StreamReader(webStream))
                                    {
                                        string response = responseReader.ReadToEnd();
                                        dynamic tojson2 = JsonConvert.DeserializeObject(response);

                                        Console.Out.WriteLine((string)tojson2.main.temp.ToString());
                                        temperatura = (string)tojson2.main.temp.ToString();
                                        info = translate((string)tojson2.weather[0].description.ToString());

                                    }

                                    t.Speak("Meteorologia em Aveiro.\n" + info + ".\n " + temperatura + " graus Celcius.");
                                }
                                catch (Exception es)
                                {
                                    Console.Out.WriteLine("-----------------");
                                    Console.Out.WriteLine(es.Message);
                                    t.Speak("Algo de errado aconteceu");

                                }

                            }
                            break;
                        case "ADD_ROLE":
                            if ((string)json.recognized[5].ToString() != null)
                            {
                                if ((string)json.recognized[7].ToString() != null)
                                {
                                    if ((string)json.recognized[7].ToString() == "ADMINISTRATOR")
                                    {
                                        _client.SendMessageAsync("!ADDROLEADMIN " + (string)json.recognized[5].ToString());
                                    }
                                    else
                                    {
                                        _client.SendMessageAsync("!ADDROLEMOD " + (string)json.recognized[5].ToString());
                                    }
                                }
                            }
                            break;
                        case "REMOVE_ROLE":
                            if ((string)json.recognized[5].ToString() != null)
                            {
                                if ((string)json.recognized[7].ToString() != null)
                                {
                                    if ((string)json.recognized[7].ToString() == "ADMINISTRATOR")
                                    {
                                        _client.SendMessageAsync("!DELROLEADMIN " + (string)json.recognized[5].ToString());
                                    }
                                    else
                                    {
                                        _client.SendMessageAsync("!DELROLEMOD " + (string)json.recognized[5].ToString());
                                    }
                                }
                            }
                            break;
                        case "invite":
                            t.Speak("invite");
                            break;
                        default:
                            break;
                    }
                });
            }
            if(json.recognized[1].ToString() == "FUSION")
            {
                switch (json.recognized[3].ToString())
                {
                    case "NEWS":
                        string url = "http://feeds.ojogo.pt/OJ-Futebol";
                        XmlReader reader = XmlReader.Create(url);
                        SyndicationFeed feed = SyndicationFeed.Load(reader);
                        reader.Close();
                        String subject = "";
                        int i = 0;
                        foreach (SyndicationItem item in feed.Items)
                        {
                            subject += item.Title.Text + ".\n";
                            i++;
                            if (i == 5)
                            {
                                break;
                            }

                        }
                        t.Speak(subject);
                        break;
                    case "LEAVE":
                        _client.SendMessageAsync("!MOVEAFKALL");
                        t.Speak("Todos os utilizadores foram retirados da sala.");
                        break;
                    case "JOIN":
                        _client.SendMessageAsync("!UNMOVEAFKALL");
                        t.Speak("Todos os utilizadores foram repostos na sala.");
                        break;
                    case "UMBRELLA":
                        if (json.recognized[5].ToString() == "AVEIRO")
                        {
                            string URL = "http://api.openweathermap.org/data/2.5/weather?q=Aveiro&APPID=6fcebaa15d35d3672004b399373a1279&units=metric";
                            Console.WriteLine(URL);
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                            request.Method = "GET";
                            request.ContentType = "application/json";

                            try
                            {
                                String temperatura = "";
                                String info = "";
                                WebResponse webResponse = request.GetResponse();
                                using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                                using (StreamReader responseReader = new StreamReader(webStream))
                                {
                                    string response = responseReader.ReadToEnd();
                                    dynamic tojson2 = JsonConvert.DeserializeObject(response);

                                    Console.Out.WriteLine((string)tojson2.main.temp.ToString());
                                    temperatura = (string)tojson2.main.temp.ToString();
                                    info = translate((string)tojson2.weather[0].description.ToString());

                                }

                                t.Speak("Meteorologia em Aveiro.\n" + info + ".\n " + temperatura + " graus Celcius.");
                            }
                            catch (Exception es)
                            {
                                Console.Out.WriteLine("-----------------");
                                Console.Out.WriteLine(es.Message);
                                t.Speak("Algo de errado aconteceu");

                            }
                            break;
                        }
                        else
                        {
                            string URL = "http://api.openweathermap.org/data/2.5/weather?q=Porto&APPID=6fcebaa15d35d3672004b399373a1279&units=metric";
                            Console.WriteLine(URL);
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                            request.Method = "GET";
                            request.ContentType = "application/json";

                            try
                            {
                                String temperatura = "";
                                String info = "";
                                WebResponse webResponse = request.GetResponse();
                                using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                                using (StreamReader responseReader = new StreamReader(webStream))
                                {
                                    string response = responseReader.ReadToEnd();
                                    dynamic tojson2 = JsonConvert.DeserializeObject(response);

                                    Console.Out.WriteLine((string)tojson2.main.temp.ToString());
                                    temperatura = (string)tojson2.main.temp.ToString();
                                    info = translate((string)tojson2.weather[0].description.ToString());

                                }

                                t.Speak("Meteorologia em Aveiro.\n" + info + ".\n " + temperatura + " graus Celcius.");
                            }
                            catch (Exception es)
                            {
                                Console.Out.WriteLine("-----------------");
                                Console.Out.WriteLine(es.Message);
                                t.Speak("Algo de errado aconteceu");

                            }
                            break;
                        }
                    case "MUTE":
                        _client.SendMessageAsync("!MUTE " + (string)json.recognized[5].ToString());
                        break;
                    case "UNMUTE":
                        _client.SendMessageAsync("!UNMUTE " + (string)json.recognized[5].ToString());
                        break;
                    default:
                        break;
                }
            }
        }

        private String translate(String s)
        {
            string URL = "https://translate.yandex.net/api/v1.5/tr.json/translate?key=trnsl.1.1.20191114T153016Z.2ebb863454ece652.dc10eb7718df89179800a28b4883d8cbb02fe8ea&text=" + s + "&lang=en-pt";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.ContentType = "application/json";
            String temperatura = "";
            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                    string response = responseReader.ReadToEnd();
                    dynamic tojson2 = JsonConvert.DeserializeObject(response);
                    temperatura = (string)tojson2.text.ToString();
                }
            }
            catch (Exception es)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(es.Message);

            }
            return temperatura;

        }
    }
}
