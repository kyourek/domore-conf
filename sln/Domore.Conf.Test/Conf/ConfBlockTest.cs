﻿using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Domore.Conf {
    using Providers;

    [TestFixture]
    public class ConfBlockTest {
        object Content;
        IConfBlock _Subject;
        IConfBlock Subject {
            get => _Subject ?? (_Subject = new ConfBlockFactory().CreateConfBlock(Content, new TextContentsProvider()));
            set => _Subject = value;
        }

        [SetUp]
        public void SetUp() {
            Subject = null;
        }

        [Test]
        public void ToString_IsContent() {
            Content = string.Join(Environment.NewLine, new[] { "this is some = good config", "Configuration = good" });
            Assert.AreEqual(Content, Subject.ToString());
        }

        [Test]
        public void ItemCount_ReturnsItemCount() {
            Content = @"
                val1 = hello, world!
                val2 = goodbye, earth..?
                val3 = 3.14
            ";
            Assert.AreEqual(3, Subject.ItemCount());
        }

        [Test]
        public void ItemExists_TrueIfIndexExists() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\nval3 = 3.14";
            Assert.IsTrue(Subject.ItemExists(2));
        }

        [Test]
        public void ItemExists_FalseIfIndexDoesNotExist() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n";
            Assert.IsFalse(Subject.ItemExists(2));
        }

        [Test]
        public void ItemExists_TrueIfKeyExists() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n VAL3 = 3.14";
            Assert.IsTrue(Subject.ItemExists("val 3"));
        }

        [Test]
        public void ItemExists_FalseIfKeyDoesNotExist() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n ";
            Assert.IsFalse(Subject.ItemExists("val 3"));
        }

        [Test]
        public void ItemExists_TrueIfIndexExistsWithOutParam() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\nval3 = 3.14";
            Assert.IsTrue(Subject.ItemExists(2, out _));
        }

        [Test]
        public void ItemExists_FalseIfIndexDoesNotExistWithOutParam() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n";
            Assert.IsFalse(Subject.ItemExists(2, out _));
        }

        [Test]
        public void ItemExists_TrueIfKeyExistsWithOutParam() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n VAL3 = 3.14";
            Assert.IsTrue(Subject.ItemExists("val 3", out _));
        }

        [Test]
        public void ItemExists_FalseIfKeyDoesNotExistWithOutParam() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n ";
            Assert.IsFalse(Subject.ItemExists("val 3", out _));
        }

        [Test]
        public void ItemExists_SetsOutParamIfIndexExists() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\nval3 = 3.14";
            Subject.ItemExists(2, out var item);
            Assert.That(item.OriginalValue, Is.EqualTo("3.14"));
        }

        [Test]
        public void ItemExists_SetsOutParamIfIndexDoesNotExist() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n";
            Subject.ItemExists(2, out var item);
            Assert.That(item, Is.Null);
        }

        [Test]
        public void ItemExists_SetsOutParamIfKeyExists() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n VAL3 = 3.14";
            Subject.ItemExists("val 2", out var item);
            Assert.That(item.OriginalValue, Is.EqualTo("goodbye, earth..?"));
        }

        [Test]
        public void ItemExists_SetsOutParamIfKeyDoesNotExist() {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n ";
            Subject.ItemExists("val 3", out var item);
            Assert.That(item, Is.Null);
        }

        [TestCase(0, "val1")]
        [TestCase(1, "val2")]
        [TestCase(2, "Val 3")]
        public void Item_OriginalKeyIsSet(object key, string expected) {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var item = Subject.Item(key);
            Assert.AreEqual(expected, item.OriginalKey);
        }

        [TestCase(0, "val1")]
        [TestCase(1, "val2")]
        [TestCase(2, "val3")]
        public void Item_NormalizedKeyIsSet(object key, string expected) {
            Content = "val1 = hello, world!\r\n \tVAL  2 = goodbye, earth..?  \n Val 3 = 3.14";
            var item = Subject.Item(key);
            Assert.AreEqual(expected, item.NormalizedKey);
        }

        [TestCase(0, "hello, world!")]
        [TestCase(1, "goodbye, earth..?")]
        [TestCase(2, "3.14")]
        public void Item_OriginalValueIsSet(object key, string expected) {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var item = Subject.Item(key);
            Assert.AreEqual(expected, item.OriginalValue);
        }

        [TestCase("val3")]
        [TestCase("Val 3")]
        [TestCase("VaL \t3")]
        public void Item_GetsItemByStringKey(object key) {
            Content = "val1 = hello, world!\r\nval2 = goodbye, earth..?\n Val 3 = 3.14";
            var item = Subject.Item(key);
            Assert.AreEqual("Val 3", item.OriginalKey);
        }

        [Test]
        public void ItemCount_GetsCountWhenSeparatedBySemicolon() {
            Content = "val1 = hello, world!;val2 = goodbye, earth..?; Val 3 = 3.14  ";
            Assert.AreEqual(3, Subject.ItemCount());
        }

        [Test]
        public void Item_OriginalValueIsSetWhenSeparatedBySemicolon() {
            Content = "val1 = hello, world!;val2 = goodbye, earth..?; Val 3 = 3.14  ";
            Assert.AreEqual("goodbye, earth..?", Subject.Item("val 2").OriginalValue);
        }

        [Test]
        public void ItemCount_NewLinePrecedesSemicolonSeparation() {
            Content = "val1 = hello, world!\n val2 = goodbye, earth..?; Val 3 = 3.14  ";
            Assert.AreEqual(2, Subject.ItemCount());
        }

        [Test]
        public void Item_OriginalValueHasSemicolonWhenSeparatedByNewLine() {
            Content = "val1 = hello, world!\n val2 = goodbye, earth..?; Val 3 = 3.14  ";
            Assert.AreEqual("goodbye, earth..?; Val 3 = 3.14", Subject.Item("val 2").OriginalValue);
        }

        class Man {
            public Dog BestFriend { get; set; }
        }

        [TypeConverter(typeof(DogConfTypeConverter))]
        class Dog {
            public string Color { get; set; }
        }

        class DogConfTypeConverter : ConfTypeConverter {
            public override object ConvertFrom(IConfBlock conf, ITypeDescriptorContext context, CultureInfo culture, object value) =>
                conf.Configure(new Dog(), value.ToString());
        }

        [Test]
        public void Configure_UsesConfTypeConverter() {
            Content = @"
                Penny.color = red
                Man.Best friend = Penny
            ";
            var man = Subject.Configure(new Man());
            Assert.AreEqual("red", man.BestFriend.Color);
        }

        class Kid { public Pet Pet { get; set; } }
        class Pet { }
        class Cat : Pet { }

        [Test]
        public void Configure_CreatesInstanceOfType() {
            Content = @"Kid.Pet = Domore.Conf.ConfBlockTest+Cat, Domore.Conf.Test";
            var kid = Subject.Configure(new Kid());
            Assert.That(kid.Pet, Is.InstanceOf(typeof(Cat)));
        }
    }
}