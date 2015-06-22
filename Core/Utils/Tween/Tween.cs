namespace LeagueSharp.SDK.Core.Utils.Tween
{
    using System;
    using System.Diagnostics;

    using LeagueSharp.SDK.Core.Enumerations;

    /// <summary>
    ///     Tween Utility, smoothly interpolates between two values over time
    /// </summary>
    /// <typeparam name="T">Type to tween</typeparam>
    public class Tween<T>
    {
        #region Fields

        /// <summary>
        ///     The function that gives a value between the defined starting and ending values
        /// </summary>
        private readonly LerpFunc<T> lerpFunc;

        /// <summary>
        ///     Used for timing how long the tween has been running, to update our elapsed time accurately
        /// </summary>
        private readonly Stopwatch stopwatch;

        /// <summary>
        ///     The last time that we updated the elapsed time
        /// </summary>
        private long lastUpdateTime;

        /// <summary>
        ///     The function that scales our progress (elapsed time/duration)
        /// </summary>
        private ScaleFunc scaleFunc;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Creates a Tween object to handle tweening a given type
        /// </summary>
        /// <param name="lerp">The function that handles lerping the given type</param>
        /// <param name="defaultValue">A value to set <code>Value</code> to as a default</param>
        public Tween(LerpFunc<T> lerp, T defaultValue = default(T))
        {
            this.State = TweenState.Stopped;
            this.Value = defaultValue;
            this.lerpFunc = lerp;
            this.stopwatch = new Stopwatch();
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Delegate for the Started, Paused, Resumed, Stopped, Finished, and Updated events
        /// </summary>
        /// <param name="tween">The tween that called the delegate</param>
        public delegate void ActionHandler(Tween<T> tween);

        /// <summary>
        ///     Standard linear interpolation function of a type, typically following <code>start + (end - start)*scale</code>
        /// </summary>
        /// <typeparam name="TLerp">The type to linear interpolate</typeparam>
        /// <param name="start">The starting value</param>
        /// <param name="end">The ending value</param>
        /// <param name="scale">The percent between start and end (that can go past the range [0, 1])</param>
        /// <returns>The type that is scale percent between the start and end values</returns>
        public delegate TLerp LerpFunc<TLerp>(TLerp start, TLerp end, float scale);

        /// <summary>
        ///     Takes in the progress of the tween and returns the interpolation value that is fed into the lerp function
        /// </summary>
        /// <param name="progress">The progress of the tween in the range [0, 1]</param>
        /// <returns>The value that is fed into the lerp function</returns>
        public delegate float ScaleFunc(float progress);

        #endregion

        #region Public Events

        /// <summary>
        ///     Called when the respective action happens on a tween object
        /// </summary>
        public event ActionHandler Started , Paused , Resumed , Stopped , Finished , Updated;

        #endregion

        #region Public Properties

        /// <summary>
        ///     The duration of the tween (in milliseconds)
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        ///     The current time that has been elapsed (in milliseconds)
        /// </summary>
        public float ElapsedTime { get; private set; }

        /// <summary>
        ///     The ending value of the tween
        /// </summary>
        public T EndValue { get; private set; }

        /// <summary>
        ///     The starting value of the tween
        /// </summary>
        public T StartValue { get; private set; }

        /// <summary>
        ///     The current state of the tween
        /// </summary>
        public TweenState State { get; private set; }

        /// <summary>
        ///     The current value of the tween
        /// </summary>
        public T Value { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Pauses a running tween
        /// </summary>
        /// <param name="callEvent">Whether or not to call the Paused event</param>
        public void Pause(bool callEvent = true)
        {
            if (this.State != TweenState.Running)
            {
                return;
            }

            this.stopwatch.Stop();
            this.State = TweenState.Paused;

            if (callEvent)
            {
                this.InvokeEvent(this.Paused);
            }
        }

        /// <summary>
        ///     Pauses a running tween
        /// </summary>
        /// <param name="callEvent">Whether or not to call the Resumed event</param>
        public void Resume(bool callEvent = true)
        {
            if (this.State != TweenState.Paused)
            {
                return;
            }

            this.stopwatch.Start();
            this.State = TweenState.Running;

            if (callEvent)
            {
                this.InvokeEvent(this.Resumed);
            }
        }

        /// <summary>
        ///     Starts a tween
        /// </summary>
        /// <param name="start">The starting value</param>
        /// <param name="end">The ending value</param>
        /// <param name="duration">The time the tween will run for (in milliseconds)</param>
        /// <param name="scale">The function used to scale elapsed time over the duration</param>
        /// <param name="callEvent">Whether or not to call the Started event</param>
        public void Start(T start, T end, float duration, ScaleFunc scale, bool callEvent = true)
        {
            if (duration <= 0)
            {
                throw new ArgumentException(@"duration must be greater than 0", "duration");
            }
            if (scale == null)
            {
                throw new ArgumentNullException("scale");
            }
            if (this.State != TweenState.Stopped)
            {
                throw new Exception("Tween must be stopped to start tweening");
            }

            this.ElapsedTime = 0f;
            this.Duration = duration;
            this.StartValue = start;
            this.EndValue = end;
            this.scaleFunc = scale;
            this.State = TweenState.Running;

            this.stopwatch.Start();

            Game.OnUpdate += this.OnGameUpdate;

            if (callEvent)
            {
                this.InvokeEvent(this.Started);
            }

            this.UpdateValue();
        }

        /// <summary>
        ///     Stops a running/paused tween
        /// </summary>
        /// <param name="forceComplete">
        ///     Whether or not to force the value and the elapsed time to be equal to their respective
        ///     ending values
        /// </param>
        /// <param name="callEvent">Whether or not to call the Stopped event</param>
        public void Stop(bool forceComplete = true, bool callEvent = true)
        {
            if (this.State == TweenState.Stopped)
            {
                return;
            }

            this.stopwatch.Stop();
            Game.OnUpdate -= this.OnGameUpdate;
            this.State = TweenState.Stopped;

            if (forceComplete)
            {
                this.ElapsedTime = this.Duration;
                this.UpdateValue();
            }

            if (callEvent)
            {
                this.InvokeEvent(this.Stopped);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Updates the Elapsed Time whenever the game updates
        /// </summary>
        /// <param name="args">Game Update Arguments</param>
        private void OnGameUpdate(EventArgs args)
        {
            if (this.State != TweenState.Running)
            {
                return;
            }

            this.ElapsedTime += this.stopwatch.ElapsedMilliseconds - this.lastUpdateTime;
            this.lastUpdateTime = this.stopwatch.ElapsedMilliseconds;

            if (this.ElapsedTime >= this.Duration)
            {
                this.Stop(true, false);
                this.InvokeEvent(this.Updated);
                this.InvokeEvent(this.Finished);
            }
            else
            {
                this.UpdateValue();
                this.InvokeEvent(this.Updated);
            }
        }

        /// <summary>
        ///     Updates the Value by calling the lerp and scale functions with their proper arguments
        /// </summary>
        private void UpdateValue()
        {
            // Everything has a `this` before it o-o
            this.Value = this.lerpFunc(this.StartValue, this.EndValue, this.scaleFunc(this.ElapsedTime / this.Duration));
        }

        #endregion

        /// <summary>
        ///     Invokes an event if it isn't null
        /// </summary>
        /// <param name="event">The event to invoke</param>
        private void InvokeEvent(ActionHandler @event)
        {
            // Since ReSharper blindly assumes this is C# 6:
            // ReSharper disable once UseNullPropagation
            if (@event != null)
            {
                @event.Invoke(this);
            }
        }
    }
}