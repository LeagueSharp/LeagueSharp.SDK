namespace LeagueSharp.SDK.Core.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    ///     A thread safe implementation of a HashSet.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentHashSet<T> : IDisposable, IEnumerable<T>
    {
        #region Fields

        /// <summary>
        ///     The lock
        /// </summary>
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        ///     The hash set
        /// </summary>
        private readonly HashSet<T> hashSet = new HashSet<T>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the number of items in this <see cref="ConcurrentHashSet{T}" />.
        /// </summary>
        /// <value>
        ///     The the number of items in this <see cref="ConcurrentHashSet{T}" />.
        /// </value>
        public int Count
        {
            get
            {
                try
                {
                    this._lock.EnterReadLock();

                    return this.hashSet.Count;
                }
                finally
                {
                    if (this._lock.IsReadLockHeld)
                    {
                        this._lock.ExitReadLock();
                    }
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the item was added sucesfully, else <c>false</c>.</returns>
        public bool Add(T item)
        {
            try
            {
                this._lock.EnterWriteLock();

                return this.hashSet.Add(item);
            }
            finally
            {
                if (this._lock.IsWriteLockHeld)
                {
                    this._lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            try
            {
                this._lock.EnterWriteLock();

                this.hashSet.Clear();
            }
            finally
            {
                if (this._lock.IsWriteLockHeld)
                {
                    this._lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        ///     Determines whether this <see cref="ConcurrentHashSet{T}" /> contains the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the <see cref="ConcurrentHashSet{T}" /> contains the specified item, else <c>false</c>.</returns>
        public bool Contains(T item)
        {
            try
            {
                this._lock.EnterReadLock();

                return this.hashSet.Contains(item);
            }
            finally
            {
                if (this._lock.IsReadLockHeld)
                {
                    this._lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this._lock?.Dispose();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.hashSet.GetEnumerator();
        }

        /// <summary>
        ///     Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            try
            {
                this._lock.EnterWriteLock();

                return this.hashSet.Remove(item);
            }
            finally
            {
                if (this._lock.IsWriteLockHeld)
                {
                    this._lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        ///     Removes items that matches the predicate.
        /// </summary>
        /// <param name="match">The predicate.</param>
        /// <returns>Number of items removed.</returns>
        public int RemoveWhere(Predicate<T> match)
        {
            try
            {
                this._lock.EnterReadLock();
                this._lock.EnterWriteLock();

                return this.hashSet.RemoveWhere(match);
            }
            finally
            {
                if (this._lock.IsReadLockHeld)
                {
                    this._lock.ExitReadLock();
                }

                if (this._lock.IsWriteLockHeld)
                {
                    this._lock.ExitWriteLock();
                }
            }
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}