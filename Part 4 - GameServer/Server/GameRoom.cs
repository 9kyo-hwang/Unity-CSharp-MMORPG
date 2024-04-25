using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	class GameRoom : IJobQueue
	{
		List<ClientSession> _sessions = new List<ClientSession>();
		JobQueue _jobQueue = new JobQueue();
		List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

		public void Push(Action job)
		{
			_jobQueue.Push(job);
		}

		public void Flush()
		{
			// N ^ 2
			foreach (ClientSession s in _sessions)
				s.Send(_pendingList);

			Console.WriteLine($"Flushed {_pendingList.Count} items");
			_pendingList.Clear();
		}

		public void Broadcast(ArraySegment<byte> segment)
		{
			_pendingList.Add(segment);
		}

		public void Enter(ClientSession session)
		{
			// Player Add
			_sessions.Add(session);
			session.Room = this;
			
			// Send player list to new client
			S_PlayerList players = new S_PlayerList();
			foreach (var s in _sessions)
			{
				players.players.Add(new S_PlayerList.Player
				{
					isSelf = (s == session),
					playerId = s.SessionId,
					posX = s.PosX,
					posY = s.PosY,
					posZ = s.PosZ
				});
			}
			session.Send(players.Write());

			// Broadcast that a new client has entered
			S_BroadcastEnterGame enterGame = new S_BroadcastEnterGame
			{
				playerId = session.SessionId,
				posX = 0,
				posY = 0,
				posZ = 0
			};
			
			Broadcast(enterGame.Write());
		}

		public void Leave(ClientSession session)
		{
			// Exit Player
			_sessions.Remove(session);
			
			// Broadcast exit client
			S_BroadcastLeaveGame leaveGame = new S_BroadcastLeaveGame
			{
				playerId = session.SessionId,
			};
			Broadcast(leaveGame.Write());
		}

		public void Move(ClientSession session, C_Move packet)
		{
			// change position
			session.PosX = packet.posX;
			session.PosY = packet.posY;
			session.PosZ = packet.posZ;

			// broadcast 
			S_BroadcastMove move = new S_BroadcastMove
			{
				playerId = session.SessionId,
				posX = session.PosX,
				posY = session.PosY,
				posZ = session.PosZ
			};
			Broadcast(move.Write());
		}
	}
}
