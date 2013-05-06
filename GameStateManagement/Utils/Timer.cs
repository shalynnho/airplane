using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Utils
{
	public delegate void TimerDelegate();
	class TimerInstance
	{
		public event TimerDelegate OnTimer;
		public bool bLooping;
		public bool bRemove;
		public float fTotalTime;
		public float fRemainingTime;
		public int iTriggerCount;

		public void Trigger() { OnTimer(); }
	}

	public class Timer
	{
		#region Fields

		private SortedList<string, TimerInstance> m_kTimers = new SortedList<string, TimerInstance>();

		#endregion

		#region Methods

		/// <summary>
		/// Update is called every frame by the owner of this timer class.
		/// It's responsible for updating every currently registered timer.
		/// If any timer has expired, it is triggered, and based on looping or not 
		/// it may either be removed or restarted
		/// Additionally, iTriggerCount for a timer should be incremented every time it triggers.
		/// </summary>
		/// <param name="gameTime">Only ElasedGameTime is used to update all registered timers</param>
		public void Update(GameTime gameTime)
		{
			float fDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			for (int i = 0; i < m_kTimers.Count; i++)
			{
				TimerInstance t = m_kTimers.Values[i];
				t.fRemainingTime -= fDeltaTime;
				if (t.fRemainingTime < 0.0f)
				{
					t.Trigger();
					t.iTriggerCount++;
					if (t.bLooping)
					{
						t.fRemainingTime = t.fTotalTime;
					}
					else
					{
						t.bRemove = true;
					}
				}
			}

			for (int i = m_kTimers.Count - 1; i >= 0; i--)
			{
				if (m_kTimers.Values[i].bRemove)
				{
					m_kTimers.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// AddTimer will add a new timer provided a timer of the same name does not already exist.
		/// </summary>
		/// <param name="sTimerName">Name of timer to be added</param>
		/// <param name="fTimerDuration">Duration timer should last, in seconds</param>
		/// <param name="Callback">Call back delegate which should be called when the timer expires</param>
		/// <param name="bLooping">Whether the timer should loop infinitely, or should fire once and remove itself</param>
		/// <returns>Returns true if the timer was successfully added, false if it wasn't</returns>
		public bool AddTimer(string sTimerName, float fTimerDuration, TimerDelegate Callback, bool bLooping)
		{
			if (!m_kTimers.ContainsKey(sTimerName))
			{
				TimerInstance t = new TimerInstance();
				t.fRemainingTime = fTimerDuration;
				t.fTotalTime = fTimerDuration;
				t.iTriggerCount = 0;
				t.OnTimer += Callback;
				t.bLooping = bLooping;
				t.bRemove = false;
				m_kTimers.Add(sTimerName, t);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// RemoveTimer removes the timer with the specified name
		/// You must support being able to remove one timer from another timer's callback
		/// (But don't worry about removing the same timer from your callback, 'cause that's confusing)
		/// </summary>
		/// <param name="sTimerName">Name of timer to remove</param>
		/// <returns>True if successfully removed, false if not found</returns>
		public bool RemoveTimer(string sTimerName)
		{
			if (m_kTimers.ContainsKey(sTimerName))
			{
				m_kTimers[sTimerName].bRemove = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// GetTriggerCount gets the number of times the specified timer has been triggered
		/// </summary>
		/// <param name="sTimerName">Name of timer to get value for</param>
		/// <returns>iTriggerCount if found, otherwise -1</returns>
		public int GetTriggerCount(string sTimerName)
		{
			if (m_kTimers.ContainsKey(sTimerName))
			{
				return m_kTimers[sTimerName].iTriggerCount;
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// GetRemainingTime gets the remaining time on the specified timer
		/// </summary>
		/// <param name="sTimerName">Name of timer to get value for</param>
		/// <returns>fRemainingTime if found, otherwise -1.0f</returns>
		public float GetRemainingTime(string sTimerName)
		{
			if (m_kTimers.ContainsKey(sTimerName))
			{
				return m_kTimers[sTimerName].fRemainingTime;
			}
			else
			{
				return -1.0f;
			}
		}

		public void RemoveAll()
		{
			m_kTimers.Clear();
		}

		public void ResetTimer(string sTimerName)
		{
			if (m_kTimers.ContainsKey(sTimerName))
			{
				m_kTimers[sTimerName].fRemainingTime = m_kTimers[sTimerName].fTotalTime;
			}
		}
		#endregion
	}
}
