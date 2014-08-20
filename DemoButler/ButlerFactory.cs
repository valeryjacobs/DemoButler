using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Interop;
using System.IO;
using System.Windows.Forms;
using WebSocket4Net;
using SocketIOClient;
using WindowsInput;
using System.Windows.Threading;
using System.Messaging;
using Microsoft.Win32;

namespace DemoButler
{
    public class ButlerFactory
    {
        public string V1 { get; set; }
        public string V2 { get; set; }
        public string V3 { get; set; }
        private int _actionIndex;

        public int ActionIndex
        {
            get { return _actionIndex; }
            set {
                
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

                    key.CreateSubKey("DemoButler");

                    key = key.OpenSubKey("DemoButler", true);


                    key.CreateSubKey("ActionIndex");
                    key = key.OpenSubKey("ActionIndex", true);

                    key.SetValue("value", value);
                
                
                _actionIndex = value;
            
            }
        }
        
       
        public List<Action> Actions { get; set; }
        public List<string> Tokens { get; set; }
        public List<int> WindowHandles { get; set; }
        public List<string> Windows { get; set; }
        public List<int> AddedWindows { get; set; }
        public List<string> Encodings { get; set; }
        private string _showContent;

        public bool MyProperty { get; set; }

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        public Client SocketClient { get; set; }

        public string ShowContent
        {
            get { return _showContent; }
            set { _showContent = value; }
        }

        public void HandleMessage(string message)
        {
            switch (message)
            {
                case "next":
                    Next();
                    break;
                case "previous":
                    Previous();
                    break;
                case "init":
                    Init();
                    break;
                case "restart":
                    Restart();
                    break;
                case "skip":
                    Skip();
                    break;
            }
        }

        public void PublishActionList()
        {
            string message = Newtonsoft.Json.JsonConvert.SerializeObject(Actions);

            MessageQueue mq;
            if (MessageQueue.Exists(@".\Private$\ActionListQueue"))

                mq = new System.Messaging.MessageQueue(@".\Private$\ActionListQueue");
            else
                mq = MessageQueue.Create(@".\Private$\ActionListQueue");

            System.Messaging.Message mm = new System.Messaging.Message();
            mm.Body = message;
            mm.Label = "ActionList";
            mq.Send(mm);
        }

        public ButlerFactory()
        {
            MyProperty = true;
            //SocketClient = new Client("http://demobutler.cloudapp.net/");
            //SocketClient = new Client("http://localhost:8383/");
            //SocketClient.On("connect", (fn) =>
            //{
            //    Debug.WriteLine("connected");
            //});

            //SocketClient.On("next", (fn) =>
            //{
            //    Next();
            //});

            //SocketClient.On("news", (fn) =>
            //{
            //    Debug.WriteLine(fn.MessageText);
            //});

            //SocketClient.On("scriptcommand", (fn) =>
            //{
            //    dynamic message = Newtonsoft.Json.JsonConvert.DeserializeObject(fn.MessageText);
            //    string m = message.args[0].Value;

            //    MessageQueue mq;
            //    if (MessageQueue.Exists(@".\Private$\ScriptQueue"))

            //        mq = new System.Messaging.MessageQueue(@".\Private$\ScriptQueue");
            //    else
            //        mq = MessageQueue.Create(@".\Private$\ScriptQueue");

            //    System.Messaging.Message mm = new System.Messaging.Message();
            //    mm.Body = m;
            //    mm.Label = m;
            //    mq.Send(mm);

            //    if (Actions.Count > ActionIndex + 1)
            //        SocketClient.Emit("nextcommand", Actions[ActionIndex + 1].Content.ToString());
            //});

            // SocketClient.Connect();

            Actions = new List<Action>();
            ActionIndex = 0;
            Encodings = new List<string>{
                 "{",
                 "}",
                 "(",
                 ")",
                 "+",
                 "%",
                 "^",
                 "~",
                 "[",
                 "]"};

            //Tokens = new List<string>{ "{{}","{BS}",
            //"{BREAK}",
            //"{CAPSLOCK}",
            //"{DEL}",
            //"{DOWN}",
            //"{END}",
            //"{ENTER}",
            //"{ESC}",
            //"{HELP}",
            //"{HOME}",
            //"{INSERT}",
            //"{LEFT}",
            //"{NUMLOCK}",
            //"{PGDN}",
            //"{PGUP}",
            //"{PRTSC}",
            //"{RIGHT}",
            //"{SCROLLLOCK}",
            //"{SPACE}",
            //"{TAB}",
            //"{UP}",
            //"{F1}",
            //"{F2}",
            //"{F3}",
            //"{F4}",
            //"{F5}",
            //"{F6}",
            //"{F7}",
            //"{F8}",
            //"{F9}",
            //"{F10}",
            //"{F11}",
            //"{F12}",
            //"{F13}",
            //"{F14}",
            //"{F15}",
            //"{F16}",
            //"{ADD}",
            //"{SUBTRACT}",
            //"{MULTIPLY}",

            //"{}}",
            //"{(}",
            //"{)}",
            //"{+}",
            //"{%}",
            //"{^}",
            //"{~}",
            //"{[}",
            //"{]}",
            //"{DIVIDE}"};
        }

        public void Previous()
        {
            if (ActionIndex > 1)
            {
                ActionIndex -= 2;
                Next();
            }
        }

        public void Skip()
        {
            ActionIndex++;
        }

        public void Next()
        {
            try
            {
                //   SocketClient.Emit("scriptevent", "Next");

                if (ActionIndex < 0) return;

                if (ActionIndex > Actions.Count - 1) return;

                if (ActionIndex >= Actions.Count) return;
                switch (Actions[ActionIndex].Type)
                {
                    case "Run":
                        //List<string> actionParms = (List<string>)Actions[ActionIndex].Content;
                        Newtonsoft.Json.Linq.JArray actionParms = (Newtonsoft.Json.Linq.JArray)Actions[ActionIndex].Content;
                        // Run(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Windows Azure\Windows Azure PowerShell.lnk", null);
                        Run(actionParms[0].ToString(), actionParms.Count > 1 ? actionParms[1].ToString() : null);
                        break;
                    case "Input":
                        Input((string)Actions[ActionIndex].Content);
                        break;
                    case "ReadInput":
                        ReadInput((string)Actions[ActionIndex].Content);
                        break;
                    case "Show":
                        Show((string)Actions[ActionIndex].Content);
                        //ActionIndex++;
                        //Next();
                        //return;
                        break;
                    case "Close":
                        Close();
                        break;
                    case "CloseAll":
                        while (AddedWindows.Count > 0)
                        {
                            Close();
                        }
                        break;
                }
                ActionIndex++;

                if (Actions[ActionIndex - 1].AutoContinue)
                    Next();
            }
            catch
            {
                if (AddedWindows != null)
                {
                    Close();
                }
            }
        }

        public void Close()
        {
            if (AddedWindows.Count > 0)
            {
                if (AddedWindows.Last() == int.MinValue)
                {
                    //foreach (System.Diagnostics.Process myProc in System.Diagnostics.Process.GetProcesses())
                    //{
                    //    if (myProc.ProcessName == "chrome")
                    //    {
                    //        myProc.Kill();
                    //    }
                    //}
                    try
                    {
                        System.Windows.Forms.SendKeys.SendWait("^(w)");
                    }
                    catch { }
                }

                try
                {
                    NativeWin32.SendMessage(AddedWindows.Last(), WM_SYSCOMMAND, SC_CLOSE, 0);
                }
                catch { }

                AddedWindows.RemoveAt(AddedWindows.Count - 1);


                if (AddedWindows.Count > 0)
                    NativeWin32.SetForegroundWindow(AddedWindows.Last());
            }
        }

        public void Init()
        {
            V2 = "Demo" + Guid.NewGuid().ToString().Substring(0, 8);
            V1 = "DemoFolder" + Guid.NewGuid().ToString().Substring(0, 8);

            Windows = new List<string>();
            WindowHandles = new List<int>();
            AddedWindows = new List<int>();
        }

        public void Restart()
        {
            if (AddedWindows != null)
            {
                foreach (int wh in AddedWindows)
                {
                    NativeWin32.SendMessage(wh, WM_SYSCOMMAND, SC_CLOSE, 0);
                }
            }

            Init();

            //SocketClient.Emit("scriptevent", "Restart");
            ActionIndex = 0;
        }

        private string Parse(string content)
        {
            if (content == null) return null;

            return content.Replace("[[V1]]", V1).Replace("[[V2]]", V2).Replace("[[V3]]", V3).Replace("[[CLIPBOARD]]", Clipboard.GetText());
        }

        public void Run(string fileName, string argument = "")
        {
            List<int> windowHandles = GetTaskWindows();

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Parse(fileName);
            psi.Arguments = Parse(argument);
            //psi.UseShellExecute = false;
            //psi.UserName = "valeryjacobs@hotmail.com";
            //System.Security.SecureString secString = new System.Security.SecureString();
            //secString.AppendChar('w');
            //secString.AppendChar('e');
            //secString.AppendChar('r');
            //secString.AppendChar('d');
            //secString.AppendChar('e');
            //secString.AppendChar('r');
            //secString.AppendChar('3');
            //secString.AppendChar('2');

            // psi.Password = secString;


            Process proc = new Process();
            proc.StartInfo = psi;
            proc.Start();
            try
            {
                proc.WaitForInputIdle();
            }
            catch
            { }

            List<int> newWindowsHandles = GetTaskWindows();

            foreach (int newwindow in newWindowsHandles)
            {
                if (!windowHandles.Contains(newwindow))
                {
                    AddedWindows.Add(newwindow);
                }
            }

            if (windowHandles.Count == newWindowsHandles.Count)
            {
                //TODO:Browser hack
                AddedWindows.Add(int.MinValue);
            }
        }

        public void Show(string content)
        {
            //Clipboard.SetText(Parse(content));
            //prevent bad paste in consoles!!
            Clipboard.Clear();
            ShowContent = content;
        }

        public void ReadInput(string file)
        {
            string content = File.ReadAllText(file);



            Input(content);
        }

        private string Encode(string input)
        {
            if (Encodings.Contains(input))
            {
                return "{" + input + "}";
            }
            //else if (input == "\t")
            //{
            //    return "";
            //}

            return input;
        }

        public void Input(string content)
        {
            //string newContent = "";

            //for (int i = 0; i < content.Length; i++)
            //{
            //    newContent = newContent + Encode(content.Substring(i, 1));
            //}

            content = Parse(content);

            // string space = " ";
            //for (int i = 0; i < lbKeys.Items.Count; i++)
            //{
            //    if (lbKeys.Items[i].Text.ToString() == "{SPACE}")
            //    {
            //        keys += space;
            //    }
            //    else
            //    {
            //        keys += lbKeys.Items[i].Text.ToString();
            //    }
            //}
            int offset = 1;
            int snapOffset;

            System.Random RandNum = new System.Random();


            for (int i = 0; i < content.Length; i += offset)
            {
                if (AddedWindows.Count > 0)
                {
                    NativeWin32.SetForegroundWindow(AddedWindows.Last());
                }
                // Debug.WriteLine(RandNum.Next(7, 15));
                System.Threading.Thread.Sleep(RandNum.Next(8, 60));

                SnapToToken(content, i, out snapOffset);
                offset = snapOffset;

                string keys = content.Substring(i, offset);

                if (offset == 1)
                    keys = Encode(keys);

                // if(NativeWin32.IsWindowVisible(iHandle)==1)
                if (keys.Contains("DOCK"))
                {
                    switch (keys)
                    {
                        case "{DOCKLEFT}":
                            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.LEFT);
                            break;
                        case "{DOCKRIGHT}":
                            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.RIGHT);
                            break;
                    }
                }
                else
                {
                    if (keys == "\r\n")
                    {
                        keys = "{ENTER}";
                    }
                    else if (keys == "\t")
                    {
                        keys = "{TAB}";
                    }

                    try
                    {
                        System.Windows.Forms.SendKeys.SendWait(keys);
                    }
                    catch { }
                }
            }
        }

        private void SnapToToken(string content, int index, out int offset)
        {

            foreach (string token in Tokens)
            {
                if ((content.Length >= index + token.Length) && content.Substring(index, token.Length) == token)
                {
                    offset = token.Length;
                    return;
                }
            }

            offset = 1;
        }

        private List<int> GetTaskWindows()
        {
            List<int> windows = new List<int>();
            Windows.Clear();

            if (Windows == null) Windows = new List<string>();
            // Get the desktopwindow handle
            int nDeshWndHandle = NativeWin32.GetDesktopWindow();
            // Get the first child window
            int nChildHandle = NativeWin32.GetWindow(nDeshWndHandle, NativeWin32.GW_CHILD);

            while (nChildHandle != 0)
            {
                //If the child window is this (SendKeys) application then ignore it.
                IntPtr windowHandle = new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle;
                if (nChildHandle == windowHandle.ToInt32())
                {
                    nChildHandle = NativeWin32.GetWindow(nChildHandle, NativeWin32.GW_HWNDNEXT);
                }

                // Get only visible windows
                if (NativeWin32.IsWindowVisible(nChildHandle) != 0)
                {
                    StringBuilder sbTitle = new StringBuilder(1024);
                    // Read the Title bar text on the windows to put in combobox
                    NativeWin32.GetWindowText(nChildHandle, sbTitle, sbTitle.Capacity);
                    String sWinTitle = sbTitle.ToString();
                    {
                        if (sWinTitle.Length > 0)
                        {
                            Windows.Add(sWinTitle);
                            WindowHandles.Add(nChildHandle);
                            windows.Add(nChildHandle);
                        }
                    }
                }
                // Look for the next child.
                nChildHandle = NativeWin32.GetWindow(nChildHandle, NativeWin32.GW_HWNDNEXT);
            }

            return windows;
        }
    }

    public class Action
    {
        public string Type { get; set; }
        public object Content { get; set; }
        public bool AutoContinue { get; set; }
        public string Description { get; set; }
    }
}
