﻿using System;
using System.Collections.Generic;

namespace Domore.Conf.Future.Text.Parsing {
    using Tokens;

    internal class TokenParser {
        public IEnumerable<ConfPair> Parse(char sep, string text) {
            if (null == text) throw new ArgumentNullException(nameof(text));
            var token = new KeyBuilder(sep) as Token;
            for (var i = 0; i < text.Length; i++) {
                if (token == null) {
                    break;
                }
                if (token is Invalid) {
                    for (; i < text.Length; i++) {
                        if (text[i] == sep) {
                            token = new KeyBuilder(sep);
                            break;
                        }
                    }
                }
                if (token is TokenBuilder builder) {
                    token = builder.Add(text, ref i);
                }
                if (token is Complete complete) {
                    yield return complete.Pair();
                    token = new KeyBuilder(sep);
                }
            }
            if (token is ValueBuilder value) {
                yield return new Complete(value.Key, value.String).Pair();
            }
        }
    }
}