using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bear
{
	public interface IBearEvent:IHandler
	{
		Type GetEventType();
	}

	public interface IHandler {
		void Handle(object a);
	}

	public interface IBearEventClass : IBearEvent
	{
	
	}

	public interface IBearEventStruct : IBearEvent
	{
	}

	

	
	public abstract class ABearEventClass<A> : IBearEventClass where A : class
	{
		public Type GetEventType()
		{
			return typeof(A);
		}

		protected abstract void Run(object a);

		public void Handle(object a)
		{
			try
			{
				Run(a);
			}
			catch (Exception e)
			{
				//Log.Error(e);
				Debug.LogWarning(e);
			}
		}
	}

	
	public abstract class ABearEvent<A> : IBearEventStruct where A : struct
	{
		public Type GetEventType()
		{
			return typeof(A);
		}

		protected abstract void Run(A a);

        public void Handle(object a)
        {
			try
			{
				if (a is A newA) {
					Run(newA);
				}
				
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}
		}

        public override bool Equals(object obj)
        {
			return obj.GetType().Equals(GetType());
        }

        public override int GetHashCode()
        {
            return base.GetType().GetHashCode();
        }
    }
}