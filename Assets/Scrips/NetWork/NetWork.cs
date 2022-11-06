using UnityEngine;

using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Unity.Threading;

public static class NetWork
{
    public static string EVENT_RECEIVE = "ReceiveMsg";
    public static string EVENT_NNET_ERR = "NetWorkError";
    static Socket socket;
    static Thread thread;
    //static List<ArraySegment<byte>> buffers = new List<ArraySegment<byte>>();
    static byte[] bufferEx = new byte[9999];
    static EasyThread easyThread = Unity.Threading.EasyThread.GetInstance();

    public static void Init(string ipStr = "", Action callback = null)
    {
#if UNITY_EDITOR
        //ipStr = "192.168.124.3";
#endif
        IPAddress ip = IPAddress.Parse(ipStr);
        int port = 9091;

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //for (int i = 0; i < 10; i++)
        {
            try
            {
                socket.Connect(ip, port); //配置服务器IP与端口  
                Debug.Log("连接服务器成功" + port);
                thread = new Thread(StartReceive);
                thread.IsBackground = true;

                //thread.Start(socket);

                easyThread.StartNewThread(thread, socket);
                easyThread.mainRemote.On<int, byte[]>(EVENT_RECEIVE, (id, bodyBuffer) =>
                {
                    ProtoHelper.OnReceiveMsg(id, bodyBuffer);
                });
                easyThread.mainRemote.On(EVENT_NNET_ERR, () =>
                {
                    GameManager.Singleton.OnNetWorkErr();
                });

                //easyThread.childRemote.On<int, byte[]>(EVENT_RECEIVE, (id, bodyBuffer) =>
                //{
                //    ProtoHelper.OnReceiveMsg(id, bodyBuffer);
                //});
                if (callback != null)
                {
                    callback.Invoke();
                }
                //GameManager.Singleton.OnServerConnect();
                //GameManager.Singleton.gameWindow.SetEnterUI(false);
            }
            catch (Exception e)
            {
                //GameManager.Singleton.gameWindow.SetEnterUI(true);
                GameManager.Singleton.OnNetWorkErr();
                Debug.LogError("连接服务器失败！" + e);
            }
        }
    }

    /// <summary>
    /// 开启接收
    /// </summary>
    /// <param name="obj"></param>
    private static void StartReceive(object obj)
    {
        Socket receiveSocket = obj as Socket;
        while (true)
        {
            try
            {
                byte[] buffer = new byte[2048];
                SocketReadWithLength(receiveSocket, buffer, 2);
                int len1 = (buffer[1] << 8) | buffer[0];
                SocketReadWithLength(receiveSocket, buffer, len1);
                int id = (buffer[1] << 8) | buffer[0];
                //string name = Encoding.UTF8.GetString(buffer, 2, len2);
                byte[] body = buffer.Skip(2).Take(len1 - 2).ToArray();
                easyThread.mainRemote.Send(EVENT_RECEIVE, id, body);
            }
            catch (Exception ex)
            {
                easyThread.mainRemote.Send(EVENT_NNET_ERR);
                //GameManager.Singleton.OnNetWorkErr();
                Debug.LogError(ex);
                break;
            }
        }
    }

    public static int SocketReadWithLength(Socket socket, byte[] buffer)
    {
        return SocketReadWithLength(socket, buffer, buffer.Length);
    }

    private static int SocketReadWithLength(Socket socket, byte[] buffer, int length)
    {
        int n = 0;
        while (true)
        {
            int num = socket.Receive(buffer, n, length - n, SocketFlags.None);
            if (num == -1)
            {
                break;
            }
            n += num;
            if (n == buffer.Length)
            {
                break;
            }
            if (n == length)
            {
                break;
            }
        }
        return n;
    }

    public static void Dispose()
    {
        try
        {
            if(socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
                thread.Abort();
                easyThread.StopThread();
                thread = null;
                Debug.Log("关闭与远程服务器的连接!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public static void Send(byte[] buffer)
    {
        if(socket != null)
        {
            socket.Send(buffer);
        }
    }

}
