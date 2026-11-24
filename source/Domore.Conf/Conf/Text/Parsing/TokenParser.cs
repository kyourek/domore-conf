using Domore.Conf.Text.Parsing.Tokens;
using System;
using System.Collections.Generic;

namespace Domore.Conf.Text.Parsing;

internal sealed class TokenParser {
    private static bool StartsComment(string text, int index, string comment) {
        var commentsDisabled = string.IsNullOrWhiteSpace(comment);
        if (commentsDisabled) {
            return false;
        }
        var startsLine = index == 0 || text[index - 1] == '\n';
        if (startsLine != true) {
            return false;
        }
        var withinLine = index < text.Length && text[index] != '\n';
        while (withinLine) {
            var markerMatches = text.Length - index >= comment.Length &&
                                string.CompareOrdinal(text, index, comment, 0, comment.Length) == 0;
            if (markerMatches) {
                return true;
            }
            var whitespace = char.IsWhiteSpace(text[index]);
            if (whitespace != true) {
                return false;
            }
            index++;
            withinLine = index < text.Length && text[index] != '\n';
        }
        return false;
    }

    public static IConfKey Key(string s) {
        if (s is null) {
            throw new ArgumentNullException(nameof(s));
        }
        var sContainsNewLine = s.Contains(
#if NETCOREAPP || NET
            '\n'
#else
            "\n"
#endif
            );
        var sep = sContainsNewLine ? '\n' : ';';
        var key = new KeyBuilder(sep);
        var token = key as Token;
        for (var i = 0; i < s.Length; i++) {
            if (token is null) {
                return key;
            }
            if (token is Invalid) {
                return key;
            }
            if (token is TokenBuilder builder) {
                token = builder.Build(s, ref i);
            }
            if (token is Complete complete) {
                return key;
            }
        }
        return key;
    }

    public IEnumerable<IConfPair> Pairs(string text, bool includeEmptyValues = false, string comment = null) {
        if (text is null) {
            throw new ArgumentNullException(nameof(text));
        }
        var textContainsNewLine = text.Contains(
#if NETCOREAPP || NET
            '\n'
#else
            "\n"
#endif
            );
        var sep = textContainsNewLine ? '\n' : ';';
        var token = new KeyBuilder(sep) as Token;
        for (var i = 0; i < text.Length; i++) {
            var startsComment = token is not MultiLineValueBuilder && StartsComment(text, i, comment);
            if (startsComment) {
                var withinComment = i < text.Length && text[i] != '\n';
                while (withinComment) {
                    i++;
                    withinComment = i < text.Length && text[i] != '\n';
                }
                token = new KeyBuilder(sep);
                continue;
            }
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
                token = builder.Build(text, ref i);
            }
            if (token is Complete complete) {
                if (includeEmptyValues || complete.Value.ToString().Length > 0) {
                    yield return complete;
                }
                token = new KeyBuilder(sep);
            }
        }
        if (token is ValueBuilder value && (includeEmptyValues || value.ToString().Length > 0)) {
            yield return new Complete(value.Key, value);
        }
    }
}
