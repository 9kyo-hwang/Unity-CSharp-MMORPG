using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    private MyPlayer _myPlayer;
    private Dictionary<int, Player> _otherPlayers = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; private set; } = new PlayerManager();

    public void Add(S_PlayerList packet)
    {
        var prefab = Resources.Load("Player");
        foreach (var player in packet.players)
        {
            var instance = Object.Instantiate(prefab) as GameObject;
            if (player.isSelf)
            {
                var myPlayer = instance.AddComponent<MyPlayer>();
                myPlayer.PlayerID = player.playerId;
                myPlayer.transform.position = new Vector3(player.posX, player.posY, player.posZ);
                _myPlayer = myPlayer;
            }
            else
            {
                var otherPlayer = instance.AddComponent<Player>();
                otherPlayer.PlayerID = player.playerId;
                otherPlayer.transform.position = new Vector3(player.posX, player.posY, player.posZ);
                _otherPlayers.Add(player.playerId, otherPlayer);
            }
        }
    }

    // 내가 들어간 경우는 Add에서 처리했음 -> 여기서는 다른 클라이언트만 다룸
    public void Enter(S_BroadcastEnterGame packet)
    {
        if (packet.playerId == _myPlayer.PlayerID)
        {
            return;
        }

        var prefab = Resources.Load("Player");
        var instance = Object.Instantiate(prefab) as GameObject;
        
        var otherPlayer = instance.AddComponent<Player>();
        otherPlayer.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        _otherPlayers.Add(packet.playerId, otherPlayer);
    }

    // 내가 나갈 수도, 타 클라이언트가 나갈 수도 있음.
    public void Leave(S_BroadcastLeaveGame packet)
    {
        if (_myPlayer.PlayerID == packet.playerId)
        {
            Object.Destroy(_myPlayer.gameObject);
            _myPlayer = null;
        }
        else if (_otherPlayers.TryGetValue(packet.playerId, out var player))
        {
            Object.Destroy(player.gameObject);
            _otherPlayers.Remove(player.PlayerID);
        }
    }

    // 내가 움직일 수도, 타 클라이언트가 움직일 수도 있음.
    public void Move(S_BroadcastMove packet)
    {
        // 서버에서 OK 응답이 왔을 때 플레이어가 움직이도록 작성
        if (_myPlayer.PlayerID == packet.playerId)
        {
            _myPlayer.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        }
        else if (_otherPlayers.TryGetValue(packet.playerId, out var player))
        {
            player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        }
    }
}
