using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class PlayerManager
    {
        public static PlayerManager Instance { get; } = new PlayerManager();
        private object _lock = new object();
        private Dictionary<int, Player> _players = new Dictionary<int, Player>();
        private int _playerId = 1;  // TODO: 아마 이렇게 관리 안하지 않을까 싶음

        public Player Add()
        {
            Player player = new Player();
            lock (_lock)
            {
                player.Info.PlayerId = _playerId;
                _players.Add(_playerId, player);
                _playerId++;
            }

            return player;
        }

        public bool Remove(int playerId)
        {
            lock (_lock)
            {
                return _players.Remove(playerId);
            }
        }

        public Player Find(int playerId)
        {
            lock (_lock)
            {
                return _players.GetValueOrDefault(playerId);
            }
        }
    }
}
