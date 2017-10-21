using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void CreateNumValidPrecisionIsZeroException()
        {
            int precision = 0;
            int scalar = 2;

            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scalar, true));

        }

        [Test]
        public void CreateNumValidWithPrecisionIsNegativeException()
        {
            var precision = -1;
            var scalar = 2;

            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scalar, true));
        }

        [Test]
        public void CreateNumValidWithScalsNegativeException()
        {
            var precision = 1;
            var scalar = -1;

            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scalar, true));
        }

        [Test]
        public void CreateNumValidWithScalIsZeroNotException()
        {
            var precision = 1;
            var scalar = 0;

            Assert.DoesNotThrow(() => new NumberValidator(precision, scalar, true));
        }

        [Test]
        public void CreateNumValidWithScalarEqualPrecisionException()
        {
            var precision = 10;
            var scalar = precision;

            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scalar, true));
        }

        [TestCase(4, 2, "+0.0")]
        [TestCase(4, 2, "-0.0")]
        [TestCase(4, 2, "-00.0")]
        [TestCase(4, 2, "+00.0")]
        [TestCase(4, 2, "0")]
        [TestCase(5, 2, "+00.00")]
        [TestCase(5, 2, "-00.00")]
        public void IsValidNumberInputZeroTrue(int precision, int scalar,
            string differentZero)
        {
            var res = new NumberValidator(precision, scalar, false).
                IsValidNumber(differentZero);

            Assert.IsTrue(res);

        }

        [TestCase(4, 2, "-11.00")]
        public void IsValidNumberInputNegativeNumberFalse(int precision, int scalar,
            string value)
        {

            var res = new NumberValidator(precision, scalar, true).IsValidNumber(value);

            Assert.IsFalse(res);
        }

        [TestCase(5, 2, "11.00")]
        [TestCase(6, 2, "+11.00")]
        public void IsValidNumberInputPositiveTrue(int precision, int scalar,
            string value)
        {
            var res = new NumberValidator(precision, scalar, true).IsValidNumber(value);

            Assert.IsTrue(res);
        }

        [Test]
        public void IsValidNumberInpitZeroWithSignTrueFalse()
        {
            Assert.IsTrue(new NumberValidator(3, 2, true).IsValidNumber("+0.0"));
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.0"));
        }

        [TestCase(5, 3, "+11111.222")]
        [TestCase(5, 3, "-11111.222")]
        [TestCase(4, 3, "21111.222")]
        [TestCase(5, 3, "1111.2233")]
        [TestCase(5, 1, "1111.")]
        public void IsValidNumberWithIncorrectValueLengthFasle(int precision, int scalar,
            string valueWithIncorrectLength)
        {

            var res = new NumberValidator(precision, scalar).
                IsValidNumber(valueWithIncorrectLength);
            Assert.IsFalse(res);
        }


        [TestCase(3, 2, "aa.01")]
        [TestCase(4, 3, " 1.2 ")]
        [TestCase(4, 3, "1 . 2")]
        public void IsValidNumberWhenValueIsLetterFalse(int precision, int scalar,
            string lineWithLetters)
        {
            var res = new NumberValidator(precision, scalar, true).
                IsValidNumber(lineWithLetters);

            Assert.IsFalse(res);
        }

        [Test]
        public void IsValidNumberWhenValueIsNullFalse()
        {
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber(null));
        }

        [Test]
        public void IsValidNumberWhenValueIsEmptyLineFalse()
        {
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber(string.Empty));
        }

        [Test]
        public void IsValidNumberWhenValueContainsTwoDotFalse()
        {
            Assert.IsFalse(new NumberValidator(5, 4, true).IsValidNumber("12.31.12"));
        }

        //[Test]
        //public void Test()
        //{
        //    Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
        //    Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
        //    Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
        //    Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
        //    Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
        //    Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
        //}
    }

    public class NumberValidator
    {
        private readonly Regex numberRegex;
        private readonly bool onlyPositive;
        private readonly int precision;
        private readonly int scale;

        public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
        {
            this.precision = precision;
            this.scale = scale;
            this.onlyPositive = onlyPositive;
            if (precision <= 0)
                throw new ArgumentException("precision must be a positive number");
            if (scale < 0 || scale >= precision)
                throw new ArgumentException("precision must be a non-negative number less or equal than precision");
            numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
        }

        public bool IsValidNumber(string value)
        {
            // Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
            // описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
            // Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
            // целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
            // Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

            if (string.IsNullOrEmpty(value))
                return false;

            var match = numberRegex.Match(value);
            if (!match.Success)
                return false;

            // Знак и целая часть
            var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
            // Дробная часть
            var fracPart = match.Groups[4].Value.Length;

            if (intPart + fracPart > precision || fracPart > scale)
                return false;

            if (onlyPositive && match.Groups[1].Value == "-")
                return false;
            return true;
        }
    }
}