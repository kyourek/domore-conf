﻿using System;
using System.Collections.Generic;

namespace Domore.Conf {
    internal class ConfValueConverterCache {
        private readonly Dictionary<Type, ConfValueConverter> Cache = new Dictionary<Type, ConfValueConverter>();

        private static ConfValueConverter Create(Type type) {
            return (ConfValueConverter)Activator.CreateInstance(type);
        }

        public ConfValueConverter Get(Type type) {
            lock (Cache) {
                if (Cache.TryGetValue(type, out var value) == false) {
                    Cache[type] = value = Create(type);
                }
                return value;
            }
        }
    }
}
