using System;
using System.Net;
using Server.Game;
using ServerCore;

namespace Server
{
	class Program
	{
        private static Listener _listener = new Listener();

        private static void FlushRoom()
		{
			JobTimer.Instance.Push(FlushRoom, 250);
		}

        private static void Main(string[] args)
        {
            GameRoomManager.Instance.Add();

			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddress = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddress, 7777);

			_listener.Init(endPoint, () => SessionManager.Instance.Generate());
			Console.WriteLine("Listening...");

			//FlushRoom();
			JobTimer.Instance.Push(FlushRoom);

			while (true)
			{
				JobTimer.Instance.Flush();
			}
		}
	}
}
