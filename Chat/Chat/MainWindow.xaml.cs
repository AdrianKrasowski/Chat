using System;
using System.Net;
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
using NetworkCommsDotNet;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;

namespace Chat
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Private Fields
        /// <summary>
        /// Dictionary to keep track of which peer messages have already been written to the chat window
        /// </summary>
        Dictionary<ShortGuid, ChatMessage> lastPeerMessageDict = new Dictionary<ShortGuid, ChatMessage>();

        /// <summary>
        /// The maximum number of times a chat message will be relayed
        /// </summary>
        int relayMaximum = 3;

        /// <summary>
        /// An optional encryption key to use should one be required.
        /// This can be changed freely but must obviously be the same
        /// for both sender and receiver.
        /// </summary>
        string encryptionKey = "ljlhjf8uyfln23490jf;m21-=scm20--iflmk;";

        /// <summary>
        /// A local counter used to track the number of messages sent from
        /// this instance.
        /// </summary>
        long messageSendIndex = 0;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Append the provided message to the chatBox text box.
        /// </summary>
        /// <param name="message"></param>
        private void AppendLineToChatBox(string message)
        {
            //To ensure we can successfully append to the text box from any thread
            //we need to wrap the append within an invoke action.
            chatBox.Dispatcher.BeginInvoke(new Action<string>((messageToAdd) =>
            {
                chatBox.AppendText(messageToAdd + "\n");
                chatBox.ScrollToEnd();
            }), new object[] { message });
        }

        /// <summary>
        /// Refresh the messagesFrom text box using the recent message history.
        /// </summary>
        private void RefreshMessagesFromBox()
        {
            //We will perform a lock here to ensure the text box is only
            //updated one thread at  time
            lock (lastPeerMessageDict)
            {
                //Use a linq expression to extract an array of all current users from lastPeerMessageDict
                string[] currentUsers = (from current in lastPeerMessageDict.Values orderby current.SourceName select current.SourceName).ToArray();

                //To ensure we can successfully append to the text box from any thread
                //we need to wrap the append within an invoke action.
                this.messagesFrom.Dispatcher.BeginInvoke(new Action<string[]>((users) =>
                {
                    //First clear the text box
                    messagesFrom.Text = "";

                    //Now write out each username
                    foreach (var username in users)
                        messagesFrom.AppendText(username + "\n");
                }), new object[] { currentUsers });
            }
        }
    }
}
