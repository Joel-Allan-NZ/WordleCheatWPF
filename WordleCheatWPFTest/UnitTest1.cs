using NUnit.Framework;
using System;
using System.Collections.Generic;
using WordleCheat.Logic;

namespace WordleCheatWPFTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Genetic gen = new Genetic();
            //gen.
            gen.FindBest(1000);
            Assert.Pass();
        }

    }
}