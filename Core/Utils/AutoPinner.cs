// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoPinner.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Auto GC Pinner.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Utils
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Auto GC Pinner.
    /// </summary>
    public class AutoPinner : IDisposable
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoPinner" /> class.
        ///     Constructor
        /// </summary>
        /// <param name="object">
        ///     Object to be pinned
        /// </param>
        public AutoPinner(object @object)
        {
            this.GcHandle = GCHandle.Alloc(@object, GCHandleType.Pinned);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="AutoPinner" /> class.
        ///     GC Finalization.
        /// </summary>
        ~AutoPinner()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the Garbage Collector Handle.
        /// </summary>
        private GCHandle GcHandle { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     <c>IntPtr</c> convert operator.
        /// </summary>
        /// <param name="autoPinner"><see cref="AutoPinner" /> handle</param>
        /// <returns></returns>
        public static implicit operator IntPtr(AutoPinner autoPinner)
        {
            return autoPinner.GcHandle.AddrOfPinnedObject();
        }

        /// <summary>
        ///     Disposal user-call.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Safe disposal.
        /// </summary>
        /// <param name="value">The Value</param>
        private void Dispose(bool value)
        {
            if (value)
            {
                this.GcHandle.Free();
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}