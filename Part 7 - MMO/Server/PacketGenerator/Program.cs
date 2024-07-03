
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace PacketGenerator
{
	class Program
	{
        private static string _clientRegister;
        private static string _serverRegister;

        private static void Main(string[] args)
		{
			string file = "../../../Common/protoc-3.20.3-win64/bin/Protocol.proto";
			if (args.Length >= 1)
				file = args[0];

			bool startParsing = false;
			foreach (string line in File.ReadAllLines(file))
			{
                if (!startParsing)
                {
                    startParsing = line.Contains("enum MsgId");
                    continue;
                }

                if (line.Contains("}"))
					break;

				string[] names = line.Trim().Split(" =");
				if (names.Length == 0)
					continue;

				string name = names[0];
				if (name.StartsWith("S_"))  // Server
				{
					string[] words = name.Split("_");
					string msgName = words.Aggregate("", (current, word) => current + FirstCharToUpper(word));
                    string packetName = $"S_{msgName[1..]}";
					_clientRegister += string.Format(PacketFormat.ManagerRegisterFormat, msgName, packetName);  // server에서 보낸 내용은 client가 분석
				}
				else if (name.StartsWith("C_"))  // Client
				{
					string[] words = name.Split("_");
					string msgName = words.Aggregate("", (current, word) => current + FirstCharToUpper(word));
                    string packetName = $"C_{msgName[1..]}";
					_serverRegister += string.Format(PacketFormat.ManagerRegisterFormat, msgName, packetName);  // client에서 보낸 내용은 server가 분석
				}
			}

			string clientManagerText = string.Format(PacketFormat.ManagerFormat, _clientRegister);
			File.WriteAllText("ClientPacketManager.cs", clientManagerText);
			string serverManagerText = string.Format(PacketFormat.ManagerFormat, _serverRegister);
			File.WriteAllText("ServerPacketManager.cs", serverManagerText);
		}

		public static string FirstCharToUpper(string input)
		{
			if (string.IsNullOrEmpty(input))
				return "";
			return input[0].ToString().ToUpper() + input[1..].ToLower();
		}
	}
}
