﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
	public class Connector
	{
		Func<Session> _sessionFactory;

		public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				// 휴대폰 설정
				Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				_sessionFactory = sessionFactory;

				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += OnConnectCompleted;
				args.RemoteEndPoint = endPoint;
				args.UserToken = socket;

				RegisterConnect(args);
			}
		}

		void RegisterConnect(SocketAsyncEventArgs args)
		{
			if (!(args.UserToken is Socket socket))
				return;

	
			if (socket.ConnectAsync(args) == false)
				OnConnectCompleted(null, args);
		}

		void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError != SocketError.Success)
			{
				Console.WriteLine($"OnConnectCompleted Fail: {args.SocketError}");
				return;
			}

			Session session = _sessionFactory.Invoke();
			session.Start(args.ConnectSocket);
			session.OnConnected(args.RemoteEndPoint);
		}
	}
}
