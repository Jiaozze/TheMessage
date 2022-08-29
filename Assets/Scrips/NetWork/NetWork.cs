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
    static Socket socket;
    static Thread thread;
    //static List<ArraySegment<byte>> buffers = new List<ArraySegment<byte>>();
    static byte[] bufferEx = new byte[9999];
    static EasyThread easyThread = Unity.Threading.EasyThread.GetInstance();

    public static void Init(string ipStr = "")
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
                socket.Connect(ip, port); //���÷�����IP��˿�  
                Debug.Log("���ӷ������ɹ�" + port);
                thread = new Thread(StartReceive);
                thread.IsBackground = true;

                //thread.Start(socket);

                easyThread.StartNewThread(thread, socket);
                easyThread.mainRemote.On<int, byte[]>(EVENT_RECEIVE, (id, bodyBuffer) =>
                {
                    ProtoHelper.OnReceiveMsg(id, bodyBuffer);
                });
                easyThread.childRemote.On<int, byte[]>(EVENT_RECEIVE, (id, bodyBuffer) =>
                {
                    ProtoHelper.OnReceiveMsg(id, bodyBuffer);
                });
                //GameManager.Singleton.gameWindow.SetEnterUI(false);
            }
            catch (Exception e)
            {
                //GameManager.Singleton.gameWindow.SetEnterUI(true);
                //GameManager.Singleton.gameWindow.ShowTip("���ӷ�����ʧ�ܣ�");
                Debug.LogError("���ӷ�����ʧ�ܣ�" + e);
            }
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="obj"></param>
    private static void StartReceive(object obj)
    {
        Socket receiveSocket = obj as Socket;
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024];
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
                thread.Abort();
                easyThread.StopThread();
                thread = null;
                Debug.Log("�ر���Զ�̷�����������!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public static void Send(byte[] buffer)
    {
        socket.Send(buffer);
    }

}
