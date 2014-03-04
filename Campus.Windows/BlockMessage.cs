using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using FISCA.Presentation.Controls;

namespace Campus.Windows
{
    public partial class BlockMessage : BaseForm
    {
        private static ProcessInvoker Method;
        private static ManualResetEvent Wait = new ManualResetEvent(true);
        private static BackgroundWorker Worker = null;
        private static MessageArgs Args;
        private static string Message;
        private static bool WorkerComplete = false;

        public BlockMessage()
        {
            InitializeComponent();
            lblMessage.Text = Message;
        }

        private void BlockMessage_Load(object sender, EventArgs e)
        {
            if (Method != null)
            {
                Args = new MessageArgs();
                CloseTimer.Enabled = true;
                WorkerComplete = false;
                ThreadPool.QueueUserWorkItem(new WaitCallback(Callback));
            }
        }

        private void Callback(object arg)
        {
            if (Method != null) Method(Args);
            WorkerComplete = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Args.Cancel = true;
            Close();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (WorkerComplete)
                Close();
        }

        private static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BlockMessage block = new BlockMessage();
            block.ShowDialog();
            Wait.Set();
        }

        public static void Display(string message, ProcessInvoker method)
        {
            Method = method;
            Message = message;

            if (Worker == null)
            {
                Worker = new BackgroundWorker();
                Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            }

            Wait.Reset();
            Worker.RunWorkerAsync();
            Wait.WaitOne();
        }
    }

    public class MessageArgs
    {
        public MessageArgs()
        {
            Cancel = false;
        }

        public bool Cancel { get; set; }
    }

    public delegate void ProcessInvoker(MessageArgs args);
}
