    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mmisharp;
using Microsoft.Speech.Recognition;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Net.Http;
using System.Net;
using System.IO;

namespace speechModality
{
    public class SpeechMod
    {
        private SpeechRecognitionEngine sre;
        private Grammar gr;

        private Grammar grammarToLoad;
        private Grammar grammarLoaded;
        private bool done;

        public event EventHandler<SpeechEventArg> Recognized;
        protected virtual void onRecognized(SpeechEventArg msg)
        {
            EventHandler<SpeechEventArg> handler = Recognized;
            if (handler != null)
            {
                handler(this, msg);
            }
        }

        private LifeCycleEvents lce;
        private MmiCommunication mmic;
        private Tts t;

        public SpeechMod()
        {
            //init LifeCycleEvents..

            lce = new LifeCycleEvents("ASR", "FUSION", "speech-1", "acoustic", "command"); // LifeCycleEvents(string source, string target, string id, string medium, string mode)
            mmic = new MmiCommunication("localhost",9876,"User1", "ASR");  //PORT TO FUSION - uncomment this line to work with fusion later
            //mmic = new MmiCommunication("localhost", 8000, "User1", "ASR"); // MmiCommunication(string IMhost, int portIM, string UserOD, string thisModalityName)

            mmic.Send(lce.NewContextRequest());

            //load pt recognizer
            sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pt-PT"));
            //gr = new Grammar(Environment.CurrentDirectory + "\\ptG.grxml", "rootRule");
            //sre.LoadGrammar(gr);
            grammarLoaded = new Grammar(Environment.CurrentDirectory + "\\ptG.grxml", "rootRule");
            sre.LoadGrammar(grammarLoaded);
            //grammarLoaded = gr;

            sre.SetInputToDefaultAudioDevice();
            sre.RecognizeAsync(RecognizeMode.Multiple);
            sre.SpeechRecognized += Sre_SpeechRecognized;
            sre.SpeechHypothesized += Sre_SpeechHypothesized;

            sre.RecognizerUpdateReached += new EventHandler<RecognizerUpdateReachedEventArgs>(recognizer_RecognizerUpdateReached);

            t = new Tts();
            
            var fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Path = Environment.CurrentDirectory;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void recognizer_RecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
        {
            if (done == false)
            {
                sre.UnloadGrammar(grammarLoaded);
                Console.WriteLine("unloaded");
                sre.LoadGrammar(grammarToLoad);
                Console.WriteLine("loaded");
                done = true;
                grammarLoaded = grammarToLoad;
            }
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            grammarToLoad = new Grammar(Environment.CurrentDirectory + "\\ptG.grxml", "rootRule");
            done = false;
            sre.RequestRecognizerUpdate();
        }

        private void Sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            onRecognized(new SpeechEventArg() { Text = e.Result.Text, Confidence = e.Result.Confidence, Final = false });
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            onRecognized(new SpeechEventArg() { Text = e.Result.Text, Confidence = e.Result.Confidence, Final = true });

            // CHANGED FOR FUSION ---------------------------------------7
            /*
             string json = "{ \"recognized\": [";
            foreach (var resultSemantic in e.Result.Semantics)
            {
                json+= "\""+resultSemantic.Key + "\",\"" + resultSemantic.Value.Value +"\", ";
            }
            json = json.Substring(0, json.Length - 2);
            json += "] }";
            Console.WriteLine(json);
             */
            //SEND
            string json = "{\"recognized\": [";
            foreach (var resultSemantic in e.Result.Semantics)
            {
                foreach (var key in resultSemantic.Value)
                {
                    json += "\"" + key.Key + "\",\"" + key.Value.Value + "\", ";
                }
            }
            json = json.Substring(0, json.Length - 2);
            json += "]}";
            Console.WriteLine(json);
            // END CHANGED FOR FUSION ---------------------------------------
            var exNot = lce.ExtensionNotification(e.Result.Audio.StartTime + "", e.Result.Audio.StartTime.Add(e.Result.Audio.Duration) + "", e.Result.Confidence, json);
            mmic.Send(exNot);
        }

        /*
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            onRecognized(new SpeechEventArg(){Text = e.Result.Text, Confidence = e.Result.Confidence, Final = true});

            if (e.Result.Confidence < 0.5)
            {
                t.Speak("Desculpe não percebi. Repita por favor.");
            }
            else { 
                string json = "{\n ";
                foreach (var resultSemantic in e.Result.Semantics)
                {
                    foreach(var key in resultSemantic.Value)
                    {
                        json += "\"" + key.Key + "\": " + "\"" + key.Value.Value + "\",\n ";
                    }
                }
                json = json.Substring(0, json.Length - 3);
                json += "\n}";
                Console.WriteLine(json);
                dynamic tojson = JsonConvert.DeserializeObject(json);
                Console.WriteLine(tojson);
                bool wake = false;
                if (tojson.wake != null)
                {
                    wake = true;
                }
                if (wake)
                {
                    if (json.Split(new string[] { "action" }, StringSplitOptions.None).Length > 2)
                    {
                        t.Speak("Utilize só um comando de cada vez.");
                    }
                    else
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            if (tojson.action != null)
                            {
                            // Processamento do comando
                            switch ((string)tojson.action.ToString())
                                {
                                    case "ADD":
                                        if ((string)tojson.name != null)
                                        {
                                            if ((string)tojson.subject.ToString() == "TEXT_CHANNEL")
                                            {
                                                t.Speak("Canal de texto " + (string)tojson.name + " criado");
                                            }
                                            else if ((string)tojson.subject.ToString() == "VOICE_CHANNEL")
                                            {
                                                t.Speak("Canal de voz " + (string)tojson.name + " criado");
                                            }
                                        }
                                        else if ((string)tojson.name == null)
                                        {
                                            if ((string)tojson.subject.ToString() == "TEXT_CHANNEL")
                                            {
                                                t.Speak("Canal de texto criado");
                                            }
                                            else if ((string)tojson.subject.ToString() == "VOICE_CHANNEL")
                                            {
                                                t.Speak("Canal de voz criado");
                                            }
                                        }
                                        break;
                                    case "REMOVE":
                                        if ((string)tojson.name != null)
                                        {
                                            if ((string)tojson.subject.ToString() == "TEXT_CHANNEL")
                                            {
                                                t.Speak("Canal de texto " + (string)tojson.name + " removido");
                                            }
                                            else if ((string)tojson.subject.ToString() == "VOICE_CHANNEL")
                                            {
                                                t.Speak("Canal de voz " + (string)tojson.name + " removido");
                                            }
                                        }
                                        else if ((string)tojson.name == null)
                                        {
                                            if ((string)tojson.subject.ToString() == "TEXT_CHANNEL")
                                            {
                                                t.Speak("Canal de texto removido");
                                            }
                                            else if ((string)tojson.subject.ToString() == "VOICE_CHANNEL")
                                            {
                                                t.Speak("Canal de voz removido");
                                            }
                                        }
                                        break;
                                    case "KICK":
                                        if ((string)tojson.name != null)
                                        {
                                            t.Speak("O " + (string)tojson.name.ToString() + " foi expulso.");
                                        }
                                        else
                                        {
                                            t.Speak("Por favor, identifique quem deseja expulsar.");
                                        }
                                        break;
                                    case "BAN":
                                        if ((string)tojson.name != null)
                                        {
                                            t.Speak("O " + (string)tojson.name.ToString() + " foi banido.");
                                        }
                                        else
                                        {
                                            t.Speak("Por favor, identifique quem deseja banir.");
                                        }
                                        break;
                                    case "UNBAN":
                                        if ((string)tojson.name != null)
                                        {
                                            t.Speak("O ban a " + (string)tojson.name.ToString() + " foi removido.");
                                        }
                                        else
                                        {
                                            t.Speak("Por favor, identifique quem deseja retirar ban.");
                                        }
                                        break;
                                    case "MUTE":
                                        if ((string)tojson.name != null)
                                        {
                                            t.Speak("O " + (string)tojson.name.ToString() + " não pode falar.");
                                        }
                                        else
                                        {
                                            t.Speak("Por favor, identifique quem deseja calar.");
                                        }
                                        break;
                                    case "UNMUTE":
                                        if ((string)tojson.name != null)
                                        {
                                            t.Speak("O " + (string)tojson.name.ToString() + " já pode falar.");
                                        }
                                        else
                                        {
                                            t.Speak("Por favor, identifique quem deseja ouvir");
                                        }
                                        break;
                                    case "INVITE":
                                        if ((string)tojson.name != null)
                                        {
                                            t.Speak("O convite para " + (string)tojson.name.ToString() + " foi enviado.");
                                        }
                                        else
                                        {
                                            t.Speak("Por favor, identifique quem deseja convidar");
                                        }
                                        break;
                                    case "TELL":
                                        if ((string)tojson.subject.ToString() == "JOKE")
                                        {
                                            string[] lines = System.IO.File.ReadAllLines(Environment.CurrentDirectory + "\\piadas.txt");
                                            Random rnd = new Random();
                                            int rand = rnd.Next(0, lines.Length);
                                            t.Speak(lines[rand]);
                                            tojson.val = lines[rand];
                                        }
                                        else if ((string)tojson.subject.ToString() == "NEWS")
                                        {
                                            string url = "http://feeds.jn.pt/JN-Ultimas";
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
                                            tojson.val = subject;
                                        }
                                        else if ((string)tojson.subject.ToString() == "WHEATER")
                                        {
                                            if ((string)tojson.city != null)
                                            {
                                                string URL = "http://api.openweathermap.org/data/2.5/weather?q=" + (string)tojson.city.ToString() + "&APPID=6fcebaa15d35d3672004b399373a1279&units=metric";
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
                                                    tojson.val = "Meteorologia em " + (string)tojson.city.ToString() + ".\n " + info.ToString() + ".\n " + temperatura + " graus Celcius.";
                                                    t.Speak("Meteorologia em " + (string)tojson.city.ToString() + ".\n" + info + ".\n " + temperatura + " graus Celcius.");
                                                }
                                                catch (Exception es)
                                                {
                                                    Console.Out.WriteLine("-----------------");
                                                    Console.Out.WriteLine(es.Message);
                                                    t.Speak("Algo de errado aconteceu");
                                                }
                                            }
                                            else
                                            {
                                                t.Speak("Por favor, identifique uma cidade");
                                            }
                                        }
                                        break;
                                    case "ADD_ROLE":
                                        if ((string)tojson.name != null)
                                        {
                                            if ((string)tojson.role != null)
                                            {
                                                if((string)tojson.role.ToString()== "ADMINISTRATOR")
                                                {
                                                    t.Speak("O " + (string)tojson.name.ToString() + " é agora administrador");
                                                }
                                                else
                                                {
                                                    t.Speak("O " + (string)tojson.name.ToString() + " é agora moderador");
                                                }
                                            }
                                            else
                                            {
                                                t.Speak("Por favor, identifique as permissões a adicionar");
                                            }            
                                        }
                                        else
                                        {
                                            t.Speak("Por favor, tente ser mais especifico");
                                        }
                                        break;
                                    case "REMOVE_ROLE":
                                        if ((string)tojson.name != null)
                                        {
                                            if ((string)tojson.role != null)
                                            {
                                                if ((string)tojson.role.ToString() == "ADMINISTRATOR")
                                                {
                                                    t.Speak("O " + (string)tojson.name.ToString() + " já não é administrador");
                                                }
                                                else
                                                {
                                                    t.Speak("O " + (string)tojson.name.ToString() + " já não é moderador");
                                                }
                                            }
                                            else
                                            {
                                                t.Speak("Por favor, identifique as permições a retirar");
                                            }
                                        }
                                        else
                                        {
                                            t.Speak("Por favor, tente ser mais especifico");
                                        }
                                        break;
                                    default:
                                    // what to do??
                                    break;
                                }
                            }
                            else
                            {
                                t.Speak("Olá sou o Pedro. Em que posso ajudar?");
                            }
                        });

			    // SEND FUSION
			
                string json = "{ \"recognized\": [";
                foreach (var resultSemantic in e.Result.Semantics)
                {
                    json+= "\""+resultSemantic.Key + "\",\"" + resultSemantic.Value.Value +"\", ";
                }
                json = json.Substring(0, json.Length - 2);
                json += "] }";
                Console.WriteLine(json);
                // END SEND FUSION
                var exNot = lce.ExtensionNotification(e.Result.Audio.StartTime + "", e.Result.Audio.StartTime.Add(e.Result.Audio.Duration) + "", e.Result.Confidence, json.ToString());
                //var exNot = lce.ExtensionNotification(e.Result.Audio.StartTime + "", e.Result.Audio.StartTime.Add(e.Result.Audio.Duration) + "", e.Result.Confidence, tojson.ToString());
                        mmic.Send(exNot);
                    }
                }
            }
        }
        */
        private String translate(String s)
        {
            string URL = "https://translate.yandex.net/api/v1.5/tr.json/translate?key=trnsl.1.1.20191114T153016Z.2ebb863454ece652.dc10eb7718df89179800a28b4883d8cbb02fe8ea&text="+s+"&lang=en-pt";
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
