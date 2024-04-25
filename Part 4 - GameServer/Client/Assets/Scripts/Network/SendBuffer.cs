using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
	public class SendBufferHelper
	{
		public static ThreadLocal<SendBuffer> CurrentBuffer = new(() => null);

		private static int ChunkSize => 65535;

		public static ArraySegment<byte> Open(int reserveSize)
		{
			CurrentBuffer.Value ??= new SendBuffer(ChunkSize);

			if (CurrentBuffer.Value.FreeSize < reserveSize)
				CurrentBuffer.Value = new SendBuffer(ChunkSize);

			return CurrentBuffer.Value.Open(reserveSize);
		}

		public static ArraySegment<byte> Close(int usedSize)
		{
			return CurrentBuffer.Value.Close(usedSize);
		}
	}

	public class SendBuffer
	{
		// [][][][][][][][][u][]
		byte[] _buffer;
		int _usedSize = 0;

		public int FreeSize => _buffer.Length - _usedSize;

		public SendBuffer(int chunkSize)
		{
			_buffer = new byte[chunkSize];
		}

		public ArraySegment<byte> Open(int reserveSize)
		{
			return reserveSize > FreeSize ? null : new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
		}

		public ArraySegment<byte> Close(int usedSize)
		{
			ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
			_usedSize += usedSize;
			return segment;
		}
	}
}
