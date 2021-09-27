﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Domore.Conf {
    [Guid("884A0522-59A1-4305-92A5-3F53FCFE4E52")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConfBlock {
        [DispId(1)]
        string Content { get; }

        [DispId(2)]
        int ItemCount();

        [DispId(3)]
        bool ItemExists(object key);

        [DispId(4)]
        IConfBlockItem Item(object key);

        [DispId(5)]
        object Configure(object obj, string key = null);

        [ComVisible(false)]
        T Configure<T>(T obj, string key = null);

        [ComVisible(false)]
        IEnumerable<T> Configure<T>(Func<T> factory, string key = null, IEqualityComparer<string> comparer = null);

        [ComVisible(false)]
        IEnumerable<KeyValuePair<string, T>> Configure<T>(Func<string, T> factory, string key = null, IEqualityComparer<string> comparer = null);

        [ComVisible(false)]
        bool ItemExists(object key, out IConfBlockItem item);

        [ComVisible(false)]
        IEnumerable<KeyValuePair<string, string>> Contents { get; }
    }
}
