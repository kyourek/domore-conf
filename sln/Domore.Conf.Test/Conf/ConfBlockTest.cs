﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        class Mom { public IList<string> Jobs { get; } = new Collection<string>(); }

        [Test]
        public void Configure_AddsToList() {
            Content = @"
                Mom.Jobs[0] = chef
                Mom.jobs[1] = Nurse
                mom.jobs[2] = accountant";
            var mom = Subject.Configure(new Mom());
            Assert.That(mom.Jobs, Is.EqualTo(new[] { "chef", "Nurse", "accountant" }));
        }

        class NumContainer { public ICollection<double> Nums { get; } = new List<double>(); }

        [Test]
        public void Configure_AddsConvertedValuesToList() {
            Content = @"
                Cont.Nums[0] = 1.23
                Cont.nums[1] = 2.34
                Cont.nums[2] = 3.45";
            var cont = Subject.Configure(new NumContainer(), "cont");
            Assert.That(cont.Nums, Is.EqualTo(new[] { 1.23, 2.34, 3.45 }));
        }

        [Test]
        public void Configure_RespectsLastListedIndexOfList() {
            Content = @"
                Cont.Nums[0] = 1.23
                Cont.nums[1] = 2.34
                Cont.nums[1] = 2.00
                Cont.nums[2] = 3.45";
            var cont = Subject.Configure(new NumContainer(), "cont");
            Assert.That(cont.Nums, Is.EqualTo(new[] { 1.23, 2.00, 3.45 }));
        }

        [Test]
        public void Configure_SetsListItemsToDefault() {
            Content = @"
                Cont.Nums[0] = 1.23
                Cont.nums[1] = 2.34
                Cont.nums[2] = 3.45
                Cont.nums[5] = 5.67";
            var cont = Subject.Configure(new NumContainer(), "cont");
            Assert.That(cont.Nums, Is.EqualTo(new[] { 1.23, 2.34, 3.45, 0.0, 0.0, 5.67 }));
        }

        [Test]
        public void Configure_SetsListItemsToNull() {
            Content = @"
                Mom.Jobs[1] = chef
                Mom.jobs[3] = Nurse
                mom.jobs[7] = accountant";
            var mom = Subject.Configure(new Mom());
            Assert.That(mom.Jobs, Is.EqualTo(new[] { null, "chef", null, "Nurse", null, null, null, "accountant" }));
        }

        class IntContainer { public IDictionary<string, int> Dict { get; } = new Dictionary<string, int>(); }

        [Test]
        public void Configure_SetsDictionaryValues() {
            Content = @"
                cont.Dict[first] = 1
                cont.dict[Third] = 3";
            var cont = Subject.Configure(new IntContainer(), "cont");
            Assert.That(cont.Dict, Is.EqualTo(new Dictionary<string, int> { { "first", 1 }, { "Third", 3 } }));
        }

        class Infant {
            public string Weight { get; set; }
            public int DiaperSize { get; set; }
            public Mom Mom { get; } = new Mom();
        }

        [Test]
        public void Configure_CanSetValuesWithoutKey() {
            Content = @"
                Weight = 12.3 lb
                Diaper size = 1
                Mom.Jobs[0] = chef
                Mom.jobs[1] = Nurse
                mom.jobs[2] = accountant            ";
            var infant = Subject.Configure(new Infant(), "");
            Assert.That(infant.Weight, Is.EqualTo("12.3 lb"));
        }

        [Test]
        public void Configure_SetsSecondValueWithoutKey() {
            Content = @"
                Weight = 12.3 lb
                Diaper size = 1
                Mom.Jobs[0] = chef
                Mom.jobs[1] = Nurse
                mom.jobs[2] = accountant
            ";
            var infant = Subject.Configure(new Infant(), "");
            Assert.That(infant.DiaperSize, Is.EqualTo(1));
        }

        [Test]
        public void Configure_SetsComplexTypeValuesWithoutKey() {
            Content = @"
                Weight = 12.3 lb
                Diaper size = 1
                Mom.Jobs[0] = chef
                Mom.jobs[1] = Nurse
                mom.jobs[2] = accountant
            ";
            var infant = Subject.Configure(new Infant(), "");
            Assert.That(infant.Mom.Jobs[1], Is.EqualTo("Nurse"));
        }

        [Test]
        public void Configure_SetsDeepValues() {
            Content = @"
                kid.Weight = 12.3 lb
                kid.Diaper size = 1
                kid.Mom.Jobs[0] = chef
                kid.Mom.jobs[1] = Nurse
                kid.mom.jobs[2] = accountant
            ";
            var infant = Subject.Configure(new Infant(), "Kid");
            Assert.That(infant.Mom.Jobs[2], Is.EqualTo("accountant"));
        }
    }
}
