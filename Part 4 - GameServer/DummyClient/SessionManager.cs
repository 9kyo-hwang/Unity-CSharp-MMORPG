using System;
using System.Collections.Generic;
using System.Text;

namespace DummyClient
{
	class SessionManager
	{
		public static SessionManager Instance { get; } = new SessionManager();

		List<ServerSession> _sessions = new List<ServerSession>();
		object _lock = new object();
		Random _random = new Random();

		public void SendForEach()
		{
			lock (_lock)
			{
				foreach (ServerSession session in _sessions)
				{
					C_Move movePacket = new C_Move
					{
						posX = _random.Next(-50, 50),
						posY = 0,
						posZ = _random.Next(-50, 50)
					};
					session.Send(movePacket.Write());
				}
			}
		}

		public ServerSession Generate()
		{
			lock (_lock)
			{
				ServerSession session = new ServerSession();
				_sessions.Add(session);
				return session;
			}
		}
	}
}
