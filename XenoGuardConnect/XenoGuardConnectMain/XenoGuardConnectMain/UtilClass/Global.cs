using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using XenoGuardConnectMain.Interfaces;

namespace XenoGuardConnect.UtilClass
{
    /// <summary>
    /// Class for holding global variables.
    /// </summary>
    class Global
    {
        public class MessageStructure
        {
            public int id { get; set; }
            public string reqId { get; set; }
            public string Time { get; set; }
            public string Type { get; set; }
            public string Message { get; set; }
            public string Tag { get; set; }
            public bool isChecked { get; set; }
            public bool isLocked { get; set; }

        }

        private static Dictionary<string, string> Errors = new Dictionary<string, string>(){
            {"1", "Field Length Wrong"},
            {"2", "Unknown E-Mail"},
            {"8", "Data Access Failed"},
            {"12", "User Password Wrong"},
            {"13", "Unknown Error"}
        };

        #region Members
        static string ipAddress = "85.215.216.184";
        static int port = 443;
        static string password = "";
        static string serverPassword = "1234";
        static string username = "";
        private static Connection socketConnection;
        private static ObservableCollection<MessageStructure> messages = new ObservableCollection<MessageStructure>() { };
        private static INotificationManager notificationManager;
        #endregion

        /// <summary>
        /// Public static method for storing password to be used in any transaction.
        /// </summary>
        /// <param name="_password">password string</param>
        public static void setPassword(string _password)
        {
            password = _password;
        }

        /// <summary>
        /// Public static method for getting password to be used in any transaction.
        /// </summary>
        public static string getPassword()
        {
            return password;
        }

        /// <summary>
        /// Public static method for storing password to be used in any transaction.
        /// </summary>
        /// <param name="_password">password string</param>
        public static void setServerPassword(string _serverPassword)
        {
            serverPassword = _serverPassword;
        }

        /// <summary>
        /// Public static method for getting password to be used in any transaction.
        /// </summary>
        public static string getServerPassword()
        {
            return serverPassword;
        }

        /// <summary>
        /// Public static method for storing ipAddress to be used in any transaction.
        /// </summary>
        /// <param name="_ipAddress">password string</param>
        public static void setIPAddress(string _ipAddress)
        {
            ipAddress = _ipAddress;
        }

        /// <summary>
        /// Public static method for getting ipAddress to be used in any transaction.
        /// </summary>
        public static string getIPAddress()
        {
            return ipAddress;
        }

        /// <summary>
        /// Public static method for storing port to be used in any transaction.
        /// </summary>
        /// <param name="_port">password string</param>
        public static void setPort(int _port)
        {
            port = _port;
        }

        /// <summary>
        /// Public static method for getting port to be used in any transaction.
        /// </summary>
        public static int getPort()
        {
            return port;
        }

        /// <summary>
        /// Public static method for storing username to be used in any transaction.
        /// </summary>
        /// <param name="_username">username string</param>
        public static void setUsername(string _username)
        {
            username = _username;
        }

        /// <summary>
        /// Public static method for getting username to be used in any transaction.
        /// </summary>
        public static string getUsername()
        {
            return username;
        }
        public static void SetNotificationClickEvent(EventHandler NavigateOnNotify)
        {
            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += NavigateOnNotify;
        }
        public static async void PoolMessageFromSocket()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        socketConnection = Connection.GetInstance(ipAddress, port);
                        while (!socketConnection.isConnected());
                        while (true)
                        {
                            string[] serverResponse = socketConnection.ReadResponse();
                            if (serverResponse[0] == "XENC_PUSH_MESSAGE")
                            {
                                string[] messageResponse = serverResponse[1].Split('|');
                                messages.Add(new MessageStructure() { 
                                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                                    Tag = serverResponse[0], 
                                    Message = messageResponse[1] + " " + messageResponse[0] + " " + messageResponse[2], 
                                    Type = messageResponse[1], 
                                    isLocked=false, 
                                    isChecked = false, 
                                    id = messages.Count,
                                    reqId = messages.Count.ToString()
                                });
                                notificationManager.SendNotification(serverResponse[0], messageResponse[1] + " " + messageResponse[0] + " " + messageResponse[2]);
                            }
                            if (serverResponse[0] == "XENC_REQUEST_MESSAGE")
                            {
                                string[] messageResponse = serverResponse[1].Split('|');
                                string message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" + messageResponse[1] + " " + messageResponse[0] + " " + messageResponse[2];
                                messages.Add(new MessageStructure() { 
                                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                                    Tag = serverResponse[0], 
                                    Message = messageResponse[1] + " " + messageResponse[0] + " " + messageResponse[2], 
                                    Type = messageResponse[1], 
                                    isLocked=false, 
                                    isChecked = false, 
                                    id = messages.Count,
                                    reqId = messageResponse[3]
                                });
                                notificationManager.SendNotification(serverResponse[0], messageResponse[1] + " " + messageResponse[0] + " " + messageResponse[2]);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        username = "";
                        password = "";
                        socketConnection.DisconnectToServer();
                    }
                }
            });
        }

        public static ObservableCollection<MessageStructure> getMessages()
        {
            return messages;
        }
        public static string getErrorResponse(string code)
        {
            return Errors[code];
        }

        public static void removeSelectedMessages()
        {
            for (int i = messages.Count - 1; i >= 0; i--)
            {
                if (messages[i].isChecked)
                {
                    messages.RemoveAt(i);
                }
            }
            
        }
    }
}