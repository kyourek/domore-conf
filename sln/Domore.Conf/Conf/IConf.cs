﻿using System;
using System.Collections.Generic;

namespace Domore.Conf {
    public interface IConf {
        T Configure<T>(T target, string key = null);
        IEnumerable<T> Configure<T>(Func<T> factory, string key = null, IEqualityComparer<string> comparer = null);
        IEnumerable<KeyValuePair<string, T>> Configure<T>(Func<string, T> factory, string key = null, IEqualityComparer<string> comparer = null);
    }
}
