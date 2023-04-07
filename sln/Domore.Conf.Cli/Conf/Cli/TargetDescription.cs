﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domore.Conf.Cli {
    internal sealed class TargetDescription {
        private static readonly Dictionary<Type, TargetDescription> Cache = new Dictionary<Type, TargetDescription>();

        private TargetDescription(Type targetType) {
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }

        public Type TargetType { get; }

        public IEnumerable<TargetPropertyDescription> Properties =>
            _Properties ?? (
            _Properties = TargetType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(propertyInfo => new TargetPropertyDescription(propertyInfo))
                .ToList());
        private IEnumerable<TargetPropertyDescription> _Properties;

        public static TargetDescription Describe(Type targetType) {
            if (Cache.TryGetValue(targetType, out var targetDescription) == false) {
                Cache[targetType] = targetDescription = new TargetDescription(targetType);
            }
            return targetDescription;
        }

        public IEnumerable<string> Conf(string cli) {
            var properties = Properties;
            var required = properties
                .Where(p => p.Required)
                .ToList();
            var arguments = properties
                .Where(p => p.ArgumentOrder >= 0)
                .OrderBy(p => p.ArgumentOrder)
                .ToList();
            foreach (var token in Token.Parse(cli)) {
                var key = token.Key?.Trim() ?? "";
                var value = token.Value?.Trim() ?? "";
                if (value == "" && key == "") {
                    continue;
                }
                if (value == "") {
                    if (arguments.Count > 0) {
                        value = key;
                        key = arguments[0].ArgumentName;
                        arguments.RemoveAt(0);
                    }
                    else {
                        throw new CliArgumentNotFoundException();
                    }
                }
                if (required.Count > 0) {
                    required.RemoveAll(p => p.AllNames.Contains(key, StringComparer.OrdinalIgnoreCase));
                }
                yield return key + "=" + value;
            }
            if (required.Count > 0) {
                throw new CliRequiredNotFoundException(required);
            }
        }
    }
}