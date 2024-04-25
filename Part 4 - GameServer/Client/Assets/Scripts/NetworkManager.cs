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

        StartCoroutine(nameof(CoSendPacket));
    }

    private void Update()
    {
        IPacket packet = PacketQueue.Instance.Pop();
        if (packet != null)
        {
            PacketManager.Instance.HandlePacket(_session, packet);
        }
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            C_Chat chatPacket = new C_Chat
            {
                chat = "Hello, Unity!"
            };
            ArraySegment<byte> segment = chatPacket.Write();
            
            _session.Send(segment);
        }
    }
}
