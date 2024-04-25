using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerCore
{
	public class PriorityQueue<T> where T : IComparable<T>
	{
		private readonly SortedSet<T> _set = new SortedSet<T>();
		public int Count => _set.Count;
		
		public void Push(T data)
		{
			_set.Add(data);
		}
		
		public void Pop()
		{
			if (_set.Count == 0)
			{
				return;
			}

			_set.Remove(Top());
		}

		public T Top()
		{
			return _set.Count == 0 ? default : _set.First();
		}
	}
}
