using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameRoom
    {
        private object _lock = new object();
        public int RoomId { get; set; }
        private List<Player> _players = new List<Player>();

        public void Enter(Player newPlayer)
        {
            if (newPlayer == null) return;

            lock (_lock)
            {
                _players.Add(newPlayer);
                newPlayer.Room = this;

                // 새로 들어온 클라이언트에게
                {
                    // Enter 했음을 인지하도록 알림
                    S_EnterGame enterPacket = new S_EnterGame
                    {
                        Player = newPlayer.Info
                    };
                    newPlayer.Session.Send(enterPacket);

                    // 다른 클라이언트는 누가 있는 지 알림
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (var p in _players.Where(p => newPlayer != p))
                    {
                        spawnPacket.Players.Add(p.Info);
                    }
                    newPlayer.Session.Send(spawnPacket);
                }

                // 기존에 있던 클라이언트들에게
                {
                    // 새로운 클라이언트가 들어왔음을 알림
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Players.Add(newPlayer.Info);
                    foreach (var p in _players.Where(p => newPlayer != p))
                    {
                        p.Session.Send(spawnPacket);
                    }
                }
            }
        }

        public void Leave(int playerId)
        {
            lock (_lock)
            {
                // 나가고자 하는 플레이어 찾기
                Player player = _players.Find(p => p.Info.PlayerId == playerId);
                if (player == null) return;

                // 리스트에서 제거 및 해당 플레이어의 Room 정보 초기화
                _players.Remove(player);
                player.Room = null;

                // 나가는 클라이언트에게
                {
                    // 나갔음을 인지하도록 알림
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }

                // 나가지 않은 다른 클라이언트들에게
                {
                    // 나가는 클라이언트가 생겼음을 알림
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.PlayerIds.Add(playerId);
                    foreach (var p in _players.Where(p => p.Info.PlayerId != playerId))
                    {
                        p.Session.Send(despawnPacket);
                    }
                }
            }
        }
    }
}
