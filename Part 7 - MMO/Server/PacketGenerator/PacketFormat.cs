using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
	class PacketFormat
	{
		// {0} 패킷 등록
		public static string ManagerFormat =
@"using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{{
	#region Singleton
	public static PacketManager Instance { get; } = new PacketManager();
	#endregion

	private PacketManager()
	{{
		Register();
	}}

    private Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onReceive = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    private Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
	
	public void Register()
	{{{0}
	}}

	public void OnReceivePacket(PacketSession session, ArraySegment<byte> buffer)
	{{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

        if (_onReceive.TryGetValue(id, out var action))
			action.Invoke(session, buffer, id);
	}}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
		if (_handler.TryGetValue(id, out var action))
			action.Invoke(session, pkt);
	}}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{{
        return _handler.GetValueOrDefault(id);
	}}
}}";

		// {0} MsgId
		// {1} 패킷 이름
		public static string ManagerRegisterFormat =
@"		
		_onReceive.Add((ushort)MsgId.{0}, MakePacket<{1}>);
		_handler.Add((ushort)MsgId.{0}, PacketHandler.{1}Handler);";

	}
}
