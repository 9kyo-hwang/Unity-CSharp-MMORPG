using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using DummyClient;
using ServerCore;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private ServerSession _session = new();
    
    void Start()
    {
        string host = Dns.GetHostName();
        IPHostEntry hostEntry = Dns.GetHostEntry(host);
        IPAddress address = hostEntry.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(address, 7777);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => _session);
    }

    private void Update()
    {
        var packets = PacketQueue.Instance.Clear();
        foreach (var packet in packets)
        {
            PacketManager.Instance.HandlePacket(_session, packet);
        }
    }

    public void Send(ArraySegment<byte> sendBuffer)
    {
        _session.Send(sendBuffer);
    }
}
