using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
	struct JobTimerElem : IComparable<JobTimerElem>
	{
		public int ExecTick; // 실행 시간
		public Action Action;

		public int CompareTo(JobTimerElem other)
		{
			return other.ExecTick - ExecTick;
		}
	}

	class JobTimer
	{
        private PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();
        private object _lock = new object();

		public static JobTimer Instance { get; } = new JobTimer();

		public void Push(Action action, int tickAfter = 0)
		{
			JobTimerElem job;
			job.ExecTick = System.Environment.TickCount + tickAfter;
			job.Action = action;

			lock (_lock)
			{
				_pq.Push(job);
			}
		}

		public void Flush()
		{
			while (true)
			{
				int now = System.Environment.TickCount;

				JobTimerElem job;

				lock (_lock)
				{
					if (_pq.Count == 0)
						break;

					job = _pq.Peek();
					if (job.ExecTick > now)
						break;

					_pq.Pop();
				}

				job.Action.Invoke();
			}
		}
	}
}
