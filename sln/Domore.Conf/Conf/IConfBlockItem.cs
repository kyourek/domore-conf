﻿using System;
using System.Runtime.InteropServices;

namespace Domore.Conf {
    [Guid("F5BE22A8-89C0-4FD6-924D-156DE83B54BE")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConfBlockItem {
        [DispId(1)]
        string OriginalKey { get; }

        [DispId(2)]
        string NormalizedKey { get; }

        [DispId(3)]
        string OriginalValue { get; }

        [DispId(4)]
        object ConvertValue(Type type);

        [ComVisible(false)]
        T ConvertValue<T>();
    }
}
