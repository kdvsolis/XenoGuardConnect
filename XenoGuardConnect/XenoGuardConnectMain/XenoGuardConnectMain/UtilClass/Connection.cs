using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace XenoGuardConnect.UtilClass
{
    /// <summary>
    /// This class defines the utilites for handling socket connections.
    /// </summary>
    public class Connection
    {
        #region Members
        private static Connection _instance;

        private IPAddress ipAddress = null;
        private IPEndPoint remoteEP = null;
        private TcpClient clientSocket = null;
        private NetworkStream writer = null;
        private static string targetIP = "";
        private static int port = 0;
        #endregion

        #region Constructors

        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="_targetIP">IP address to be connected</param>
        /// <param name="_port">port to be connected</param>
        private Connection(string _targetIP, int _port)
        {
            targetIP = _targetIP;
            port = _port;
            this.ipAddress = IPAddress.Parse(_targetIP);
            this.remoteEP = new IPEndPoint(ipAddress, _port);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Method for instantiating singleton instance.
        /// </summary>
        /// <param name="targetIP">IP address to be connected</param>
        /// <param name="port">port to be connected</param>
        /// <returns>
        /// Returns Connection
        /// </returns>
        public static Connection GetInstance(string _targetIP = "", int _port = 0)
        {

            targetIP = targetIP == "" ? _targetIP : targetIP;
            port = port == 0 ? _port : port;
            if (_instance == null)
            {
                _instance = new Connection(_targetIP, _port);
            }
            return _instance;
        }
        /// <summary>
        /// Method that lets you connect to the TCP server.
        /// </summary>
        public void ConnectToServer()
        {
            try
            {
                Task t = Task.Run(() =>
                {
                    clientSocket = new TcpClient();
                    clientSocket.Connect(this.remoteEP);
                });
                TimeSpan ts = TimeSpan.FromMilliseconds(10000);
                if (!t.Wait(ts))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method that lets you disconnect to the TCP server.
        /// </summary>
        public void DisconnectToServer()
        {
            try
            {
                if(clientSocket != null)
                {
                    clientSocket.Close();
                    clientSocket = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method that checks your connectivity status.
        /// </summary>
        /// <returns>
        /// Returns boolean
        /// </returns>
        public bool isConnected()
        {
            return clientSocket != null && clientSocket.Connected;
        }

        /// <summary>
        /// Method that lets you send commands according to MycroTools format.
        /// </summary>
        /// <param name="header">header</param>
        /// <param name="label">label</param>
        /// <param name="tag">tag</param>
        /// <param name="serverPassword">password</param>
        /// <param name="command">command to send (e.g. XENC_LOGIN)</param>
        /// <param name="data">payload data to send</param>
        public void SendCommand(int header, string label, string tag, string serverPassword, string command, string data)
        {
            List<byte> payload = new List<byte>();
            byte[] initialPayload = BuildPayload(header, tag, label);
            byte[] mainPayload = BuildPayload(header, tag, serverPassword, command, data);
            payload.AddRange(initialPayload);
            payload.AddRange(WriteUInt32((uint)mainPayload.Length));
            payload.AddRange(mainPayload);
            Console.WriteLine(BitConverter.ToString(payload.ToArray()));
            writer = clientSocket.GetStream();
            writer.Write(payload.ToArray(), 0, payload.ToArray().Length);
        }
        /// <summary>
        /// Method that returns response from TCP socket if available.
        /// </summary>
        /// <returns>
        /// Returns a string array
        /// </returns>
        public string[] ReadResponse()
        {
            string dataFromServer = "Server not reachable";
            int header = 0;
            int lengthHolder = 0;
            int payloadByteLength = 0;
            int currPos = 0;
            string label = "";
            string tag = "ERROR";
            string password = "";
            string command = "";
            string data = "";
            Task t = Task.Run(() =>
            {
                NetworkStream networkStream = clientSocket.GetStream();
                byte[] bytesFrom = new byte[(int)clientSocket.ReceiveBufferSize];
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);

                header = Int32.Parse(BitConverter.ToString(SubArray(bytesFrom, currPos, 4)).Replace("-", string.Empty), System.Globalization.NumberStyles.HexNumber);
                currPos += 4;
                lengthHolder = Int32.Parse(BitConverter.ToString(SubArray(bytesFrom, currPos, 4)).Replace("-", string.Empty), System.Globalization.NumberStyles.HexNumber);
                currPos += 4;
                label = System.Text.Encoding.UTF8.GetString(SubArray(bytesFrom, currPos, lengthHolder));
                currPos += label.Length;
                lengthHolder = Int32.Parse(BitConverter.ToString(SubArray(bytesFrom, currPos, 4)).Replace("-", string.Empty), System.Globalization.NumberStyles.HexNumber);
                currPos += lengthHolder;
                command = System.Text.Encoding.UTF8.GetString(SubArray(bytesFrom, currPos, lengthHolder));
                currPos += command.Length;

                payloadByteLength = Int32.Parse(BitConverter.ToString(SubArray(bytesFrom, currPos, 4)).Replace("-", string.Empty), System.Globalization.NumberStyles.HexNumber);
                currPos += 4;
                header = Int32.Parse(BitConverter.ToString(SubArray(bytesFrom, currPos, 4)).Replace("-", string.Empty), System.Globalization.NumberStyles.HexNumber);
                currPos += 4;
                lengthHolder = Int32.Parse(BitConverter.ToString(SubArray(bytesFrom, currPos, 4)).Replace("-", string.Empty), System.Globalization.NumberStyles.HexNumber);
                currPos += 4;
                label = System.Text.Encoding.UTF8.GetString(SubArray(bytesFrom, currPos, lengthHolder));
                currPos += label.Length;
                lengthHolder = Int32.Parse(BitConverter.ToString(SubArray(bytesFrom, currPos, 4)).Replace("-", string.Empty), System.Globalization.NumberStyles.HexNumber);
                currPos += lengthHolder;
                password = System.Text.Encoding.UTF8.GetString(SubArray(bytesFrom, currPos, lengthHolder));
                currPos += password.Length;
                lengthHolder = Int32.Parse(BitConverter.ToString(SubArray(bytesFrom, currPos, 4)).Replace("-", string.Empty), System.Globalization.NumberStyles.HexNumber);
                currPos += 4;

                tag = System.Text.Encoding.UTF8.GetString(SubArray(bytesFrom, currPos, lengthHolder));
                currPos += tag.Length;

                lengthHolder = Int32.Parse(BitConverter.ToString(SubArray(bytesFrom, currPos, 4)).Replace("-", string.Empty), System.Globalization.NumberStyles.HexNumber);
                currPos += 4;

                data = System.Text.Encoding.UTF8.GetString(SubArray(bytesFrom, currPos, lengthHolder));
                currPos += data.Length;

                dataFromServer = data;
                Console.WriteLine(command);
            });
            TimeSpan ts = TimeSpan.FromMilliseconds(7000);
            if (!t.Wait(ts))
            {
                return new string[] { tag, dataFromServer };
            }
            return new string[] { tag, dataFromServer };
        }

        /// <summary>
        /// Method that converts the payload in general into byte array
        /// </summary>
        /// <param name="header">header</param>
        /// <param name="payloadCommand">payload data to convert</param>
        /// <returns>
        /// Returns an array
        /// </returns>
        public static byte[] BuildPayload(int header, params string[] payloadCommand)
        {
            List<byte> payload = new List<byte>();
            payload.AddRange(WriteInt32(header));
            foreach (string value in payloadCommand)
            {
                payload.AddRange(WriteInt32(value.Length));
                payload.AddRange(StringToByteArray(value));
            }
            return payload.ToArray();
        }

        /// <summary>
        /// Converts a given UTF8 string into an array of bytes.
        /// </summary>
        /// <param name="str">The UTF8 encoded string to convert.
        /// </param>
        /// <returns>
        /// Returns an array
        /// </returns>
        public static byte[] StringToByteArray(string str)
        {
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            return enc.GetBytes(str);
        }

        /// <summary>
        /// Writes an <see cref="int"/> (4 bytes) into the stream.
        /// </summary>
        /// <param name="data">
        /// The data to write.
        /// </param>
        public static byte[] WriteInt32(int data)
        {
            return WriteUInt32((uint)data);
        }

        /// <summary>
        /// Writes an <see cref="uint"/> into the stream.
        /// </summary>
        /// <param name="data">
        /// The data to write.
        /// </param> 
        public static byte[] WriteUInt32(uint data)
        {
            List<byte> uint32Bytes = new List<byte>();
            ushort low = (ushort)(data & 0xFFFF);
            ushort high = (ushort)(data >> 16);
            uint32Bytes.AddRange(WriteUInt16(high));
            uint32Bytes.AddRange(WriteUInt16(low));
            return uint32Bytes.ToArray();
        }

        /// <summary>
        /// Writes a <see cref="short"/> (2 bytes) into the stream.
        /// </summary>
        /// <param name="data">
        /// The data to write.
        /// </param>
        public static byte[] WriteInt16(short data)
        {
            return WriteUInt16((ushort)data);
        }

        /// <summary>
        /// Writes an <see cref="ushort"/> into the stream.
        /// </summary>
        /// <param name="data">
        /// The data to write.
        /// </param> 
        public static byte[] WriteUInt16(ushort data)
        {
            byte low = (byte)(data & 255);
            byte high = (byte)(data >> 8);
            return new byte[] { high, low };
        }

        /// <summary>
        /// For chopping out the sub array of bytes
        /// </summary>
        /// <param name="array">Target byte array </param> 
        /// <param name="offset">Starting point to chop out</param> 
        /// <param name="length">Total length of array you want to chop</param> 
        public static byte[] SubArray(byte[] array, int offset, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }

        #endregion
    }
}