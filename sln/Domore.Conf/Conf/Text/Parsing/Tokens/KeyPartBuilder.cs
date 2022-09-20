﻿using System;
using System.Text;

namespace Domore.Conf.Text.Parsing.Tokens {
    internal sealed class KeyPartBuilder : TokenBuilder, IConfKeyPart {
        protected override string Create() {
            return String.ToString();
        }

        public StringBuilder String { get; } = new StringBuilder();
        public ConfCollection<KeyIndexBuilder> Indices { get; } = new ConfCollection<KeyIndexBuilder>();

        public KeyBuilder Key { get; }

        public KeyPartBuilder(KeyBuilder key) : base((key ?? throw new ArgumentNullException(nameof(key))).Sep) {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Key.Parts.Add(this);
        }

        public override Token Build(string s, ref int i) {
            var c = Next(s, ref i);
            if (c == null) return null;
            if (c == Sep) return new KeyBuilder(Sep);
            switch (c) {
                case '=':
                    return new SingleLineValueBuilder(Key);
                case '[':
                    return new KeyIndexBuilder(this);
                case '.':
                    return new KeyPartBuilder(Key);
                default:
                    String.Append(c);
                    return this;
            }
        }

        IConfCollection<IConfKeyIndex> IConfKeyPart.Indices =>
            Indices;
    }
}