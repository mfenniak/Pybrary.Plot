using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Pybrary.Plot
{
    public class EventObject
    {
        public event EventHandler OnChanged;
        private int eventsSuspended = 0;
        private bool eventsSuspendedRaise = false;

        public IDisposable SuspendEvents()
        {
            eventsSuspended++;
            return new EventSuspendTicket(this);
        }

        private void ResumeEvents()
        {
            Debug.Assert(eventsSuspended != 0, "ResumeEvents called more than SuspendEvents");
            if (eventsSuspended > 0)
                eventsSuspended--;
            if (eventsSuspended == 0 && eventsSuspendedRaise)
            {
                eventsSuspendedRaise = false;
                raiseEvent();
            }
        }

        protected void raiseEvent()
        {
            if (eventsSuspended != 0)
            {
                eventsSuspendedRaise = true;
                return;
            }

            EventHandler tmp = OnChanged;
            if (tmp != null)
                tmp(this, null);
        }

        private class EventSuspendTicket : IDisposable
        {
            private EventObject o;
            public EventSuspendTicket(EventObject o)
            {
                this.o = o;
            }
            public void Dispose()
            {
                this.o.ResumeEvents();
            }
        }
    }
}
