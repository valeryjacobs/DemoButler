using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Interop;
using System.IO;
using System.Windows.Threading;
using System.Messaging;
using System.Net;

namespace DemoButler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ButlerFactory _bf;
        List<string> apps = new List<string>();
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        private void ShowWindow()
        {
            // retrieve Notepad main window handle
            IntPtr hWnd = FindWindow("Notepad", "Naamloos - Notepad");
            if (!hWnd.Equals(IntPtr.Zero))
            {
                // SW_SHOWMAXIMIZED to maximize the window
                // SW_SHOWMINIMIZED to minimize the window
                // SW_SHOWNORMAL to make the window be normal size
                ShowWindowAsync(hWnd, SW_SHOWMAXIMIZED);
            }
        }

        MessageQueue _mq;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,100);

           
            if (MessageQueue.Exists(@".\Private$\ScriptQueue"))
                //creates an instance MessageQueue, which points 
                //to the already existing MyQueue
                _mq = new System.Messaging.MessageQueue(@".\Private$\ScriptQueue");
            else
                //creates a new private queue called MyQueue 
                _mq = MessageQueue.Create(@".\Private$\ScriptQueue");


            Initialize();
         
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(_bf!=null)
            CheckMessage();
        }

        public void CheckMessage()
        {
            System.Messaging.Message mes;

            var queueEnum = _mq.GetMessageEnumerator2();

            if (queueEnum.MoveNext())
            {
                mes = _mq.Receive(new TimeSpan(0, 0, 3));
                mes.Formatter = new XmlMessageFormatter(
                  new String[] { "System.String,mscorlib" });

                dispatcherTimer.Stop();
                _bf.HandleMessage(mes.Body.ToString());
                dispatcherTimer.Start();
            }

            //try
            //{
             
            //}
            //catch
            //{

            //}
        }

        private void GetTaskWindows()
        {
            // Get the desktopwindow handle
            int nDeshWndHandle = NativeWin32.GetDesktopWindow();
            // Get the first child window
            int nChildHandle = NativeWin32.GetWindow(nDeshWndHandle, NativeWin32.GW_CHILD);

            while (nChildHandle != 0)
            {
                //If the child window is this (SendKeys) application then ignore it.
                IntPtr windowHandle = new WindowInteropHelper(this).Handle;
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
                            apps.Add(sWinTitle);
                        }
                    }
                }
                // Look for the next child.
                nChildHandle = NativeWin32.GetWindow(nChildHandle, NativeWin32.GW_HWNDNEXT);
            }
        }

        private void Initialize()
        {
            string scriptfile = @"c:\Code\DemoButler\DemoScripts\demo.script";
          //  WebClient webClient = new WebClient();
          //  string result = webClient.DownloadString("https://valeryjacobs.blob.core.windows.net/demoscripts/demo.script");
            _bf = Newtonsoft.Json.JsonConvert.DeserializeObject<ButlerFactory>(File.ReadAllText(scriptfile));
            _bf.PublishActionList();
            //_bf = Newtonsoft.Json.JsonConvert.DeserializeObject<ButlerFactory>(result);

           // _bf = new ButlerFactory();

          //  _bf.Actions.Add(new Action() { Type = "Run", Content = new List<string> { @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Windows Azure\Windows Azure PowerShell.lnk" } , AutoContinue= true, Description = "Hier gaat hij ..."});
            //_bf.Actions.Add(new Action() { Type = "Show", Content = "New-AzureServiceProject scriptr1" });


            //_bf.Actions.Add(new Action() { Type = "ReadInput", Content = "SN1.txt" });
            //.StartApplication(@"C:\Applicaties\Notepad++\notepad++.exe",@"c:\test\test.txt")
            // .StartApplication(@"C:\Applicaties\Notepad++\notepad++.exe")
            //.InputToApp("Hello {ENTER} world ^S ")
            // .StartApplication(@"C:\Windows\System32\cmd.exe")
            //   .StartApplication(@"C:\Program Files (x86)\Microsoft WebMatrix\WebMatrix.exe")
            //   .InputToApp("{TAB}{TAB}")
            //   ;

            this.DataContext = _bf;

          //   File.WriteAllText(@"c:\DemoScripts\ser.txt", Newtonsoft.Json.JsonConvert.SerializeObject(_bf));
            _bf.Init();

            dispatcherTimer.Start();
        }

        private void Init_Click_1(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        private void Previous_Click_1(object sender, RoutedEventArgs e)
        {
            actions.SelectedIndex = _bf.ActionIndex;
            _bf.Previous();
            show.Text = _bf.ShowContent;
        }

        private void Next_Click_1(object sender, RoutedEventArgs e)
        {
            actions.SelectedIndex = _bf.ActionIndex;
            _bf.Next();
            show.Text = _bf.ShowContent;
        }

        private void Skip_Click_1(object sender, RoutedEventArgs e)
        {
            actions.SelectedIndex = _bf.ActionIndex+1;
            _bf.Skip();
            show.Text = _bf.ShowContent;
        }

        private void Restart_Click_1(object sender, RoutedEventArgs e)
        {
            actions.SelectedIndex = 0;
            _bf.Restart();
            show.Text = _bf.ShowContent;
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
           if(_bf !=null)_bf.Close();
        }
    }
}
