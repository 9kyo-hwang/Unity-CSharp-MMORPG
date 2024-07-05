using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class Player
    {
        // proto에서 EnterGame, Spawn 시 필요하도록 설계함.
        // 따라서 플레이어가 PlayerInfo 자체를 들고 있도록 해 오버헤드 줄임
        public PlayerInfo Info { get; set; } = new PlayerInfo();
        public GameRoom Room { get; set; }  // 내가 들어간 방 정보
        public ClientSession Session { get; set; }  // 패킷을 보낼 때 필요한 내 세션 정보
    }
}
