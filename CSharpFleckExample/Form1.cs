using Fleck;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpFleckExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        WebSocketServer server;
        List<Fleck.IWebSocketConnection> clients = new List<IWebSocketConnection>();

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var server = new WebSocketServer("ws://"+txtServerAddress.Text+":"+txtServerPort.Text);
            server.Start(socket =>
            {
                socket.OnOpen = () => {
                    log("Bağlantı sağlandı. : "+socket.ConnectionInfo.Id);
                    clients.Add(socket);
                    updateListBox();
                };

                socket.OnClose = () => {
                    log("Bağlantı koptu. : " + socket.ConnectionInfo.Id);
                    clients.Remove(socket);
                    updateListBox();
                }; 

                socket.OnMessage = message =>
                {
                    log(message);
                };  
            });

            log("Server başlatıldı.");
            btnStart.Enabled = false; 
        }

        void log(string msg)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { rtxtLog.Text += msg + "\r\n"; });
                return;
            }
            else
            {
                rtxtLog.Text += msg + "\r\n";
            }
        }

       void updateListBox()
       {
            Invoke((MethodInvoker)delegate {
                lbClients.Items.Clear();
                foreach (var item in clients)
                {
                    lbClients.Items.Add(item.ConnectionInfo.Id);
                }
            }); 
       }

        private void btnSend_Click(object sender, EventArgs e)
        {
            foreach (var item in clients)
            {
                item.Send("_SERVER_MESSAGE_:"+txtMessage.Text);
            }
            log("_SERVER_MESSAGE_:" + txtMessage.Text);
            txtMessage.Text = "";
        }
         
    }
}
