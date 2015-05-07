using System;
using System.Runtime.InteropServices;

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Auto GC Pinner.
    /// </summary>
    public class AutoPinner : IDisposable
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="object">Object to be pinned</param>
        public AutoPinner(Object @object)
        {
            GcHandle = GCHandle.Alloc(@object, GCHandleType.Pinned);
        }

        /// <summary>
        ///     Garbage Collector Handle.
        /// </summary>
        private GCHandle GcHandle { get; set; }

        /// <summary>
        ///     Disposal usercall.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     IntPtr convertion operator.
        /// </summary>
        /// <param name="autoPinner"></param>
        /// <returns></returns>
        public static implicit operator IntPtr(AutoPinner autoPinner)
        {
            return autoPinner.GcHandle.AddrOfPinnedObject();
        }

        /// <summary>
        ///     Safe disposal.
        /// </summary>
        /// <param name="value">Value</param>
        private void Dispose(bool value)
        {
            if (value)
            {
                GcHandle.Free();
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        ///     GC Finalization.
        /// </summary>
        ~AutoPinner()
        {
            Dispose(false);
        }
    }
}