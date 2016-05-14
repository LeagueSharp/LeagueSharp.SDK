// <copyright file="Signal.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDKEx.Signals
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    ///     A signal.
    /// </summary>
    public class Signal : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Signal" /> class.
        /// </summary>
        /// <param name="signalRaised">
        ///     The signal raised
        /// </param>
        /// <param name="signalWaver">
        ///     The signal waver
        /// </param>
        /// <param name="expiration">
        ///     The expiration
        /// </param>
        /// <param name="properties">
        ///     The properties
        /// </param>
        private Signal(
            OnRaisedDelegate signalRaised,
            SignalWaverDelegate signalWaver,
            DateTimeOffset expiration,
            IDictionary<string, object> properties)
        {
            if (signalRaised != null)
            {
                this.OnRaised += signalRaised;
            }

            if (signalWaver != null)
            {
                this.SignalWaver = signalWaver;
            }

            this.Expiration = expiration;
            this.Properties = new Dictionary<string, object>(properties);
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Raised delegate.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Raised Event Data</param>
        public delegate void OnEnabledStatusChangedDelegate(object sender, EnabledStatusChangedArgs e);

        /// <summary>
        ///     Raised delegate.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Raised Event Data</param>
        public delegate void OnRaisedDelegate(object sender, RaisedArgs e);

        /// <summary>
        ///     Signal raised delegate.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Signal</param>
        public delegate void OnSignalRaisedDelegate(object sender, Signal e);

        /// <summary>
        ///     The delegate for <see cref="SignalWaver" />
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>True if the signal should be waved.</returns>
        public delegate bool SignalWaverDelegate(Signal signal);

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when any signal is raised.
        /// </summary>
        public static event OnSignalRaisedDelegate OnSignalRaised;

        /// <summary>
        ///     Occurs when <see cref="Enabled" /> is changed.
        /// </summary>
        public event OnEnabledStatusChangedDelegate OnEnabledStatusChanged;

        /// <summary>
        ///     Occurs when this signal expires.
        /// </summary>
        public event EventHandler OnExpired;

        /// <summary>
        ///     Occurs when this signal is raised.
        /// </summary>
        public event OnRaisedDelegate OnRaised;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Signal" /> is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; private set; }

        /// <summary>
        ///     Gets or sets the expiration.
        /// </summary>
        /// <value>
        ///     The expiration.
        /// </value>
        public DateTimeOffset Expiration { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Signal" /> is expired.
        /// </summary>
        /// <value>
        ///     <c>true</c> if expired; otherwise, <c>false</c>.
        /// </value>
        public bool Expired => DateTimeOffset.Now >= this.Expiration;

        /// <summary>
        ///     Gets or sets the last time the signal was signaled.
        /// </summary>
        /// <value>
        ///     The last time the signal was signaled.
        /// </value>
        public DateTimeOffset LastSignaled { get; set; }

        /// <summary>
        ///     Gets the properties. Useful for attaching things like state, etc.
        /// </summary>
        /// <value>
        ///     The properties.
        /// </value>
        public IDictionary<string, object> Properties { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Signal" /> is raised.
        /// </summary>
        /// <value>
        ///     <c>true</c> if raised; otherwise, <c>false</c>.
        /// </value>
        public bool Raised { get; set; }

        /// <summary>
        ///     Gets or sets the signal delegate.
        /// </summary>
        public SignalWaverDelegate SignalWaver { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether called expired events.
        /// </summary>
        internal bool CalledExpired { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates an instance of the <see cref="Signal" /> class.
        /// </summary>
        /// <param name="onRaised">The delegate to call when this signal is raised.</param>
        /// <param name="signalWaver">The function that returns <c>true</c> when this signal should be waved.</param>
        /// <param name="expiration">The expiration of this signal.</param>
        /// <param name="defaultProperties">A dictionary that contents will be dumped into <see cref="Properties" /></param>
        /// <returns>The <see cref="Signal" /></returns>
        public static Signal Create(
            OnRaisedDelegate onRaised = null,
            SignalWaverDelegate signalWaver = null,
            DateTimeOffset expiration = default(DateTimeOffset),
            IDictionary<string, object> defaultProperties = null)
        {
            if (expiration == default(DateTimeOffset))
            {
                expiration = DateTimeOffset.MaxValue;
            }

            var signal = new Signal(
                onRaised,
                signalWaver,
                expiration,
                defaultProperties ?? new Dictionary<string, object>());

            SignalManager.AddSignal(signal);
            signal.Enabled = true;

            return signal;
        }

        /// <summary>
        ///     Disables this signal.
        /// </summary>
        public void Disable()
        {
            this.Enabled = false;

            // Get the caller of the this method.
            var callFrame = new StackFrame(1);
            var declaringType = callFrame.GetMethod().DeclaringType;

            if (declaringType == null)
            {
                return;
            }

            var caller = declaringType.FullName + "." + callFrame.GetMethod().Name;
            this.TriggerEnabledStatusChanged(caller, false);

            SignalManager.RemoveSignal(this);
        }

        /// <summary>
        ///     Enables this signal.
        /// </summary>
        public void Enable()
        {
            this.Enabled = true;

            // Get the caller of the this method.
            var callFrame = new StackFrame(1);
            var declaringType = callFrame.GetMethod().DeclaringType;

            if (declaringType == null)
            {
                return;
            }

            var caller = declaringType.FullName + "." + callFrame.GetMethod().Name;
            this.TriggerEnabledStatusChanged(caller, true);

            SignalManager.AddSignal(this);
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
            this.TriggerSignal(caller, reason);
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
            this.TriggerSignal(caller, exception.Message);
        }

        /// <summary>
        ///     Resets the signal.
        /// </summary>
        public void Reset()
        {
            this.Enabled = true;
            this.Raised = false;
            this.CalledExpired = false;

            // Add signal back to manager
            SignalManager.AddSignal(this);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Triggers the enabled status changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="enabled">Signal enabled or not..</param>
        internal void TriggerEnabledStatusChanged(object sender, bool enabled)
        {
            this.OnEnabledStatusChanged?.Invoke(sender, new EnabledStatusChangedArgs(enabled));
        }

        /// <summary>
        ///     Triggers the on expired event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        internal void TriggerOnExipired(object sender)
        {
            this.OnExpired?.Invoke(sender, new EventArgs());
        }

        /// <summary>
        ///     Triggers the signal.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="reason">The reason.</param>
        internal void TriggerSignal(object sender, string reason)
        {
            if (this.OnRaised == null || this.Expired || this.Raised)
            {
                return;
            }

            this.Raised = true;
            this.LastSignaled = DateTimeOffset.Now;
            this.OnRaised(sender, new RaisedArgs(reason, this));

            OnSignalRaised?.Invoke(sender, this);
        }

        #endregion

        /// <summary>
        ///     Raised Arguments.
        /// </summary>
        public class EnabledStatusChangedArgs : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="EnabledStatusChangedArgs" /> class.
            /// </summary>
            /// <param name="status">Signal is enabled or not.</param>
            public EnabledStatusChangedArgs(bool status)
            {
                this.Status = status;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets a value indicating whether the status.
            /// </summary>
            public bool Status { get; set; }

            #endregion
        }

        /// <summary>
        ///     Arguments for the <see cref="OnSignalRaised" />
        /// </summary>
        public class GlobalSignalRaisedArgs
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="GlobalSignalRaisedArgs" /> class.
            /// </summary>
            /// <param name="reason">The reason.</param>
            /// <param name="signal">The signal.</param>
            internal GlobalSignalRaisedArgs(string reason, Signal signal)
            {
                this.Reason = reason;
                this.Signal = signal;
            }

            #endregion

            #region Public Properties

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

            #endregion
        }

        /// <summary>
        ///     Raised Arguments.
        /// </summary>
        public class RaisedArgs : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="RaisedArgs" /> class.
            /// </summary>
            /// <param name="reason">
            ///     The Reason
            /// </param>
            /// <param name="signal">
            ///     The Signal
            /// </param>
            public RaisedArgs(string reason, Signal signal)
            {
                this.Reason = reason;
                this.Signal = signal;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the raised reason.
            /// </summary>
            public string Reason { get; set; }

            /// <summary>
            ///     Gets or sets the signal.
            /// </summary>
            public Signal Signal { get; set; }

            #endregion
        }
    }
}