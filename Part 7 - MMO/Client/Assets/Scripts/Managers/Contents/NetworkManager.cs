using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;

public class NetworkManager
{
    private ServerSession _session = new ServerSession();

	public void Send(ArraySegment<byte> sendBuff)
	{
		_session.Send(sendBuff);
	}

	public void Init()
	{
		// DNS (Domain Name System)
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);
		IPAddress ipAddress = ipHost.AddressList[0];
		IPEndPoint endPoint = new IPEndPoint(ipAddress, 7777);

		Connector connector = new Connector();
        connector.Connect(endPoint, () => _session, 1);
    }

	public void Update()
	{
		List<PacketMessage> list = PacketQueue.Instance.PopAll();
		foreach (PacketMessage packet in list)
		{
			Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            handler?.Invoke(_session, packet.Message);
        }	
	}

}
