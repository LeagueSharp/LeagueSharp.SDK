using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Caching;

namespace LeagueSharp.CommonEx.Core.Signals
{
    /// <summary>
    ///     A signal.
    /// </summary>
    public class Signal : EventArgs
    {
        /// <summary>
        ///     Whether or not to check expirations.
        /// </summary>
        private bool _checkExpiration;

        /// <summary>
        ///     Whether we already called the expire event.
        /// </summary>
        internal bool CalledExpired;

        /// <summary>
        ///     If this returns true, the signal will be raised.
        /// </summary>
        public Func<bool> SignalWaver;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Signal" /> class.
        /// </summary>
        /// <param name="onRaised">The action to execute when this signal is raised</param>
        /// <param name="signalWaver">Waves(activates) this signal on when this returns true.</param>
        public Signal(OnRaisedDelegate onRaised, Func<bool> signalWaver)
            : this(onRaised, signalWaver, ObjectCache.InfiniteAbsoluteExpiration)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Signal" /> class.
        /// </summary>
        /// <param name="onRaised">The on raised.</param>
        /// <param name="signalWaver">The signal waver.</param>
        /// <param name="expiration">The time this signal expires.</param>
        public Signal(OnRaisedDelegate onRaised, Func<bool> signalWaver, DateTimeOffset expiration)
        {
            OnRaised += onRaised;
            SignalWaver = signalWaver;

            Expiration = expiration;

            SignalManager.AddSignal(this);
            Properties = new Dictionary<string, string>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Signal" /> class with an infinite expiration.
        /// </summary>
        public Signal()
        {
            Expiration = ObjectCache.InfiniteAbsoluteExpiration;
            SignalManager.AddSignal(this);
            Properties = new Dictionary<string, string>();
        }

        /// <summary>
        ///     Gets or sets the expiration.
        /// </summary>
        /// <value>
        ///     The expiration.
        /// </value>
        public DateTimeOffset Expiration { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Signal" /> is expired.
        /// </summary>
        /// <value>
        ///     <c>true</c> if expired; otherwise, <c>false</c>.
        /// </value>
        public bool Expired
        {
            get { return DateTime.Now >= Expiration; }
        }

        /// <summary>
        ///     Gets the properties. Useful for attaching things like state, etc.
        /// </summary>
        /// <value>
        ///     The properties.
        /// </value>
        public IDictionary<string, string> Properties { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Signal" /> is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Signal" /> is raised.
        /// </summary>
        /// <value>
        ///     <c>true</c> if raised; otherwise, <c>false</c>.
        /// </value>
        public bool Raised { get; set; }

        /// <summary>
        ///     Gets or sets the last time the signal was signaled.
        /// </summary>
        /// <value>
        ///     The last time the signal was signaled.
        /// </value>
        public DateTimeOffset LastSignaled { get; set; }

        /// <summary>
        ///     Occurs when any signal is raised.
        /// </summary>
        public static event OnSignalRaisedDelegate OnSignalRaised;

        /// <summary>
        ///     Signal raised delegate.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Signal</param>
        public delegate void OnSignalRaisedDelegate(object sender, Signal e);

        /// <summary>
        ///     Occurs when this signal is raied.
        /// </summary>
        public event OnRaisedDelegate OnRaised;

        /// <summary>
        ///     Raised delegate.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">RaisedArgs</param>
        public delegate void OnRaisedDelegate(object sender, RaisedArgs e);

        /// <summary>
        ///     Raised Arguments.
        /// </summary>
        public class RaisedArgs : EventArgs
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="reason">Reason</param>
            public RaisedArgs(string reason)
            {
                Reason = reason;
            }

            /// <summary>
            ///     Raised Reason.
            /// </summary>
            public string Reason;
        }

        /// <summary>
        ///     Occurs when this signal expires.
        /// </summary>
        public event EventHandler OnExpired;

        /// <summary>
        ///     Occurs when <see cref="Enabled" /> is changed.
        /// </summary>
        public event OnEnabledStatusChangedDelegate OnEnabledStatusChanged;

        /// <summary>
        ///     Raised delegate.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">RaisedArgs</param>
        public delegate void OnEnabledStatusChangedDelegate(object sender, EnabledStatusChangedArgs e);

        /// <summary>
        ///     Raised Arguments.
        /// </summary>
        public class EnabledStatusChangedArgs : EventArgs
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="status">Changed Status</param>
            public EnabledStatusChangedArgs(bool status)
            {
                Status = status;
            }

            /// <summary>
            ///     Raised Reason.
            /// </summary>
            public bool Status;
        }

        /// <summary>
        ///     Suspends the expiration checking.
        /// </summary>
        public void SuspendExpirationChecking()
        {
            _checkExpiration = false;
        }

        /// <summary>
        ///     Raises the signal.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="format">The format.</param>
        public void Raise(string reason, params object[] format)
        {
            reason = string.Format(reason, format);

            // Get the caller of the this method.
            var callFrame = new StackFrame(1);
            var declaringType = callFrame.GetMethod().DeclaringType;

            if (declaringType == null)
            {
                return;
            }

            var caller = declaringType.FullName + "." + callFrame.GetMethod().Name;
            TriggerSignal(caller, reason);
        }

        /// <summary>
        ///     Raises the signal.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Raise(Exception exception)
        {
            // Get the caller of the this method.
            var callFrame = new StackFrame(1);
            var declaringType = callFrame.GetMethod().DeclaringType;

            if (declaringType == null)
            {
                return;
            }

            var caller = declaringType.FullName + "." + callFrame.GetMethod().Name;
            TriggerSignal(caller, exception.Message);
        }

        /// <summary>
        ///     Resets the signal.
        /// </summary>
        public void Reset()
        {
            Enabled = true;
            Raised = false;
            CalledExpired = false;
        }

        /// <summary>
        ///     Disables this signal.
        /// </summary>
        public void Disable()
        {
            Enabled = false;

            // Get the caller of the this method.
            var callFrame = new StackFrame(1);
            var declaringType = callFrame.GetMethod().DeclaringType;

            if (declaringType == null)
            {
                return;
            }

            var caller = declaringType.FullName + "." + callFrame.GetMethod().Name;
            TriggerEnabledStatusChanged(caller, false);
        }

        /// <summary>
        ///     Enables this signal.
        /// </summary>
        public void Enable()
        {
            Enabled = true;

            // Get the caller of the this method.
            var callFrame = new StackFrame(1);
            var declaringType = callFrame.GetMethod().DeclaringType;

            if (declaringType == null)
            {
                return;
            }

            var caller = declaringType.FullName + "." + callFrame.GetMethod().Name;
            TriggerEnabledStatusChanged(caller, true);
        }

        /// <summary>
        ///     Triggers the signal.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="reason">The reason.</param>
        internal void TriggerSignal(object sender, string reason)
        {
            if (OnRaised == null || (!Expired && !_checkExpiration) || Raised)
            {
                return;
            }

            Raised = true;
            LastSignaled = DateTimeOffset.Now;
            OnRaised(sender, new RaisedArgs(reason));

            if (OnSignalRaised != null)
            {
                OnSignalRaised(sender, this);
            }
        }

        /// <summary>
        ///     Triggers the enabled status changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="enabled">Signal enabled or not..</param>
        internal void TriggerEnabledStatusChanged(object sender, bool enabled)
        {
            if (OnEnabledStatusChanged != null)
            {
                OnEnabledStatusChanged(sender, new EnabledStatusChangedArgs(enabled));
            }
        }

        /// <summary>
        ///     Triggers the on exipired event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        internal void TriggerOnExipired(object sender)
        {
            if (OnExpired != null)
            {
                OnExpired(sender, new EventArgs());
            }
        }

        /// <summary>
        ///     Arguments for the <see cref="OnSignalRaised" />
        /// </summary>
        public class GlobalSignalRaisedArgs
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="GlobalSignalRaisedArgs" /> class.
            /// </summary>
            /// <param name="reason">The reason.</param>
            /// <param name="signal">The signal.</param>
            internal GlobalSignalRaisedArgs(string reason, Signal signal)
            {
                Reason = reason;
                Signal = signal;
            }

            /// <summary>
            ///     Gets or sets the reason why the signal was raised..
            /// </summary>
            /// <value>
            ///     The reason.
            /// </value>
            public string Reason { get; set; }

            /// <summary>
            ///     Gets or sets the signal that was raised.
            /// </summary>
            /// <value>
            ///     The signal.
            /// </value>
            public Signal Signal { get; set; }
        }
    }
}