using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameRoomManager
    {
        public static GameRoomManager Instance { get; } = new GameRoomManager();
        private object _lock = new object();
        private Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        private int _roomId = 1;

        public GameRoom Add()
        {
            GameRoom room = new GameRoom();
            lock (_lock)
            {
                room.RoomId = _roomId;
                _rooms.Add(_roomId, room);
                _roomId++;
            }

            return room;
        }

        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                return _rooms.Remove(roomId);
            }
        }

        public GameRoom Find(int roomId)
        {
            lock (_lock)
            {
                return _rooms.GetValueOrDefault(roomId);
            }
        }
    }
}
