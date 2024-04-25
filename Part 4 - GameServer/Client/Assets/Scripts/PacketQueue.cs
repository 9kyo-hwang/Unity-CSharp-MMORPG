using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PacketQueue
{
    public static PacketQueue Instance { get; } = new();
    private Queue<IPacket> _queue = new();
    private object _lock = new();

    public void Push(IPacket packet)
    {
        lock (_lock)
        {
            _queue.Enqueue(packet);
        }
    }

    public IPacket Pop()
    {
        lock (_lock)
        {
            return _queue.Count == 0 ? null : _queue.Dequeue();
        }
    }
}
