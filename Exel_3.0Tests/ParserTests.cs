using Microsoft.VisualStudio.TestTools.UnitTesting;
using Exel_3._0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exel_3._0.Tests
{
    [TestClass()]
    public class ParserTests
    {
        [TestMethod()]
        public void EvaluateTest_2plus2()
        {
            Parser parser = new Parser();
            double rezult = parser.Evaluate("2+2");
            double correct = 2 + 2;
            Assert.AreEqual(rezult, correct);
        }
        [TestMethod()]
        public void EvaLuateTest_15_dob_15()
        {
            Parser parser = new Parser();
            double rezult = parser.Evaluate("15*15");
            double correct = 15 * 15;
            Assert.AreEqual(rezult, correct);
        }
        [TestMethod()]
        public void EvaLuateTest_15_minus_15()
        {
            Parser parser = new Parser();
            double rezult = parser.Evaluate("15-15");
            double correct = 15 - 15;
            Assert.AreEqual(rezult, correct);
        }
       
        [TestMethod()]
        public void EvaLuateTest_2plus2dob2()
        {
            Parser parser = new Parser();
            double rezult = parser.Evaluate("2+2*2");
            double correct = 2+2*2;
            Assert.AreEqual(rezult, correct);
        }
    }
}