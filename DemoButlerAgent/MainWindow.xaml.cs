using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SocketIOClient.Eventing;
using SocketIOClient.Messages;
using SocketIOClient;
using System.Diagnostics;
using WebSocket4Net;
using System.Messaging;
using Microsoft.Win32;

namespace DemoButlerAgent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client SocketClient { get; set; }
        MessageQueue _mq;
        MessageQueue _sq;
        public string LastMessageContent { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            if (MessageQueue.Exists(@".\Private$\ActionListQueue"))
                //creates an instance MessageQueue, which points 
                //to the already existing MyQueue
                _mq = new System.Messaging.MessageQueue(@".\Private$\ActionListQueue");
            else
                //creates a new private queue called MyQueue 
                _mq = MessageQueue.Create(@".\Private$\ActionListQueue");


            //SocketClient = new Client("http://localhost:8383/");
            SocketClient = new Client(System.Configuration.ConfigurationManager.AppSettings["proxyaddress"]);
            SocketClient.On("connect", (fn) =>
            {
                Debug.WriteLine("connected");
                CheckActionListMessage();
            });

            SocketClient.On("scriptcommand", (fn) =>
            {
                dynamic message = Newtonsoft.Json.JsonConvert.DeserializeObject(fn.MessageText);
                string m = message.args[0].Value;

                MessageQueue mq;
                if (MessageQueue.Exists(@".\Private$\ScriptQueue"))

                    mq = new System.Messaging.MessageQueue(@".\Private$\ScriptQueue");
                else
                    mq = MessageQueue.Create(@".\Private$\ScriptQueue");

                System.Messaging.Message mm = new System.Messaging.Message();
                mm.Body = m;
                mm.Label = m;
                mq.Send(mm);

                System.Threading.Thread.Sleep(300);
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("DemoButler", true);
                key = key.OpenSubKey("ActionIndex", true);
                
                SocketClient.Emit("statusupdate", key.GetValue("value"));
            });

            SocketClient.Connect();
        }

        public void CheckActionListMessage()
        {
            System.Messaging.Message mes;

            var queueEnum = _mq.GetMessageEnumerator2();

            if (queueEnum.MoveNext())
            {
                mes = _mq.Receive(new TimeSpan(0, 0, 3));
                mes.Formatter = new XmlMessageFormatter(
                  new String[] { "System.String,mscorlib" });

                dynamic actionList = Newtonsoft.Json.JsonConvert.DeserializeObject(mes.Body.ToString());


                LastMessageContent = mes.Body.ToString();
                SocketClient.Emit("actionlist", LastMessageContent);
            }
            else
            {
                if (LastMessageContent != null)
                    SocketClient.Emit("actionlist", LastMessageContent);
            }
        }

        private void reconnect_Click_1(object sender, RoutedEventArgs e)
        {
            CheckActionListMessage();
        }
    }
}
