using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton

    public static PacketManager Instance { get; } = new();

    #endregion

	PacketManager()
	{
		Register();
	}

    private Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new();
    private Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new();
		
	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SChat, MakePacket<S_Chat>);
		_handler.Add((ushort)MsgId.SChat, PacketHandler.S_ChatHandler);		
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

        if (_onRecv.TryGetValue(id, out var action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
        if (_handler.TryGetValue(id, out var action))
			action.Invoke(session, pkt);
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
    {
        return _handler.GetValueOrDefault(id);
    }
}