using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketMessage
{
	public ushort Id { get; set; }
	public IMessage Message { get; set; }  // IMessage에서는 프로토콜 Id 정보를 바로 알 수 없기 때문에 별도의 class 정의
}

public class PacketQueue
{
	public static PacketQueue Instance { get; } = new();

    private Queue<PacketMessage> _packetQueue = new();
    private object _lock = new();

	public void Push(ushort id, IMessage packet)
	{
		lock (_lock)
		{
			_packetQueue.Enqueue(new PacketMessage() { Id = id, Message = packet });
		}
	}

	public PacketMessage Pop()
	{
		lock (_lock)
        {
            return _packetQueue.Count == 0 ? null : _packetQueue.Dequeue();
        }
	}

	public List<PacketMessage> PopAll()
	{
		List<PacketMessage> list = new List<PacketMessage>();

		lock (_lock)
		{
			while (_packetQueue.Count > 0)
				list.Add(_packetQueue.Dequeue());
		}

		return list;
	}
}