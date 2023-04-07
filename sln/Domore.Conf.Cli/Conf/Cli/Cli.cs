﻿using System;

namespace Domore.Conf.Cli {
    using Extensions;

    public static class Cli {
        public static T Configure<T>(T target, string line) {
            var description = TargetDescription.Describe(typeof(T));
            var confLines = description.Conf(line);
            var conf = string.Join(Environment.NewLine, confLines);
            return target.ConfFrom(conf, key: "");
        }
    }
}