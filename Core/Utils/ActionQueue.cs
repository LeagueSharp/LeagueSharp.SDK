namespace LeagueSharp.SDK.Utils
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    using LeagueSharp.SDK.Enumerations;

    /// <summary>
    ///     Queues actions.
    /// </summary>
    public class ActionQueue
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="ActionQueue" /> class.
        /// </summary>
        static ActionQueue()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the queues.
        /// </summary>
        /// <value>
        ///     The queues.
        /// </value>
        private static ConcurrentDictionary<string, SynchronizedCollection<Item>> Queues { get; } =
            new ConcurrentDictionary<string, SynchronizedCollection<Item>>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Dequeues the specified item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if the item was successfully dequeued; else <c>false</c>.</returns>
        public static bool Dequeue(Guid id)
        {
            var caller = Assembly.GetCallingAssembly().FullName;

            if (!Queues.ContainsKey(caller))
            {
                return false;
            }

            var queue = Queues[caller];

            return queue.Remove(queue.Find(x => x.Id.Equals(id)));
        }

        /// <summary>
        ///     Enqueues the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A <see cref="Guid" /> used to dequeue items.</returns>
        public static Guid Enqueue(Item item)
        {
            var caller = Assembly.GetCallingAssembly().FullName;
            var guid = Guid.NewGuid();

            item.Id = guid;

            if (!Queues.ContainsKey(caller))
            {
                Queues[caller] = new SynchronizedCollection<Item>();
            }

            Queues[caller].Add(item);

            return guid;
        }

        /// <summary>
        ///     Gets the items.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}" /> of all of the queued items.</returns>
        public static IEnumerable<Item> GetItems()
        {
            return Queues[Assembly.GetCallingAssembly().FullName] ?? new SynchronizedCollection<Item>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game updates.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (var value in Queues.Values)
            {
                foreach (var item in value)
                {
                    try
                    {
                        if (item.Condition())
                        {
                            item.Action();
                        }

                        if (item.RemoveCondition())
                        {
                            value.Remove(item);
                        }
                    }
                    catch (Exception e)
                    {
                        Logging.Write()(LogLevel.Error, "An ActionQueue item threw an exception.\n{0}", e);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// </summary>
        public class Item
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the action.
            /// </summary>
            /// <value>
            ///     The action.
            /// </value>
            public Action Action { get; set; }

            /// <summary>
            ///     Gets or sets the condition. If this returns <c>true</c>, the <see cref="Action" /> will be exectued.
            /// </summary>
            /// <value>
            ///     The condition.
            /// </value>
            public Func<bool> Condition { get; set; }

            /// <summary>
            ///     Gets or sets the remove condition. If this returns <c>true</c>, this item will be removed.
            /// </summary>
            /// <value>
            ///     The remove condition.
            /// </value>
            public Func<bool> RemoveCondition { get; set; }

            #endregion

            #region Properties

            /// <summary>
            ///     Gets or sets the identifier.
            /// </summary>
            /// <value>
            ///     The identifier.
            /// </value>
            internal Guid Id { get; set; }

            #endregion
        }
    }
}