using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ClientTcp : MonoBehaviour
{
    public IPEndPoint ServerIPEndPoint { get; set; }
    public Socket Socket { get; set; }
    public const int BufferSize = 1024;
    public byte[] Buffer { get; } = new byte[BufferSize];
    public string result;
    public int Port;

    // ソケット通信の接続
    public void Connect()
    {
        try
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // TCP
            Socket.Connect(ServerIPEndPoint);
            // 非同期で受信を待機
            Socket.BeginReceive(Buffer, 0, BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), Socket);
        }
        catch
        {
            return;
        }
    }

    // ソケット通信接続の切断
    public void OnApplicationQuit()
    {
        Socket.Disconnect(false);
        Socket.Dispose();
    }

    // メッセージの送信(同期処理)
    public void MySend(byte[] data)
    {
        this.Socket.Send(data);
    }

    // 非同期受信のコールバックメソッド(別スレッドで実行される)
    private void ReceiveCallback(IAsyncResult asyncResult)
    {
        var socket = asyncResult.AsyncState as Socket;

        var byteSize = -1;
        try
        {
            // 受信を待機
            byteSize = socket.EndReceive(asyncResult);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return;
        }

        // 受信したデータがある場合、その内容を表示する
        // 再度非同期での受信を開始する
        if (byteSize > 0)
        {
            result = Encoding.UTF8.GetString(this.Buffer, 0, byteSize);
            //Debug.Log(result);
            socket.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, ReceiveCallback, socket);
        }
    }

    // Use this for initialization
    public void Awake()
    {
        var ipadress = System.Net.Dns.GetHostEntry("KyonoMacBook-Pro.local");

#if UNITY_IOS
        this.ServerIPEndPoint = new IPEndPoint(ipadress.AddressList[1], Port);
        //フレームレートを落とす。電池節約のため。
        Application.targetFrameRate = 30;
        //寝ません。
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif

#if UNITY_EDITOR
        this.ServerIPEndPoint = new IPEndPoint(ipadress.AddressList[0], Port);
#endif
        Debug.Log(ipadress.AddressList[0]);
        Debug.Log(ipadress.AddressList[1]);
        Debug.Log(this.ServerIPEndPoint);

        Connect();
    }
}
