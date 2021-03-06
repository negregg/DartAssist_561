using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DartAssistant.Test
{
    [TestClass]
    public class DartTests
    {
        #region BaseValue Property

        /// <summary>
        /// WHEN a Dart is instantiated with valid BaseValue
        /// THEN the BaseValue property will return the value passed in to the constructor
        /// </summary>
        [TestMethod]
        public void BaseValueProperty_ValidBaseValue_ReturnsValueFromConstructor()
        {
            // Arrange
            int expectedBaseValue = 10;

            // Act
            Dart subject = new Dart(expectedBaseValue, SegmentMultiplier.Triple);

            // Assert
            Assert.AreEqual(expectedBaseValue, subject.BaseValue, "BaseValue was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with valid BaseValue but with a SegmentMultiplier.Miss
        /// THEN the BaseValue property will return 0
        /// </summary>
        [TestMethod]
        public void BaseValueProperty_ValidBaseValueWithMiss_Returns0()
        {
            // Arrange
            int expectedBaseValue = 0;

            // Act
            Dart subject = new Dart(10, SegmentMultiplier.Miss);

            // Assert
            Assert.AreEqual(expectedBaseValue, subject.BaseValue, "BaseValue was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is attempted to be instantiated with a BaseValue of -1
        /// THEN an Exception will be thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BaseValueProperty_SetToNegative1_ThrowsAnException()
        {
            // Arrange
            int expectedBaseValue = -1;

            // Act
            Dart subject = new Dart(expectedBaseValue, SegmentMultiplier.Single);

            // Assert
            // Expect Exception - see method attribute.
        }

        /// <summary>
        /// WHEN a Dart is attempted to be instantiated with a BaseValue of 21
        /// THEN an Exception will be thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BaseValueProperty_SetTo21_ThrowsAnException()
        {
            // Arrange
            int expectedBaseValue = 21;

            // Act
            Dart subject = new Dart(expectedBaseValue, SegmentMultiplier.Single);

            // Assert
            // Expect Exception - see method attribute.
        }

        #endregion

        #region Multiplier Property

        /// <summary>
        /// WHEN a Dart is instantiated with valid baseValue and multiplier
        /// THEN the Multiplier property will return the value passed in to the constructor
        /// </summary>
        [TestMethod]
        public void MultiplierProperty_ValidBaseValueAndMultiplier_ReturnsValueFromConstructor()
        {
            // Arrange
            SegmentMultiplier expectedMultiplier = SegmentMultiplier.Triple;

            // Act
            Dart subject = new Dart(5, expectedMultiplier);

            // Assert
            Assert.AreEqual(expectedMultiplier, subject.Multiplier, "Multiplier was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with valid baseValue of 0 and multiplier other than miss
        /// THEN the Multiplier property will return Miss
        /// </summary>
        [TestMethod]
        public void MultiplierProperty_BaseValue0MultiplierNotMiss_ReturnsMiss()
        {
            // Arrange
            SegmentMultiplier expectedMultiplier = SegmentMultiplier.Miss;

            // Act
            Dart subject = new Dart(0, SegmentMultiplier.Single);

            // Assert
            Assert.AreEqual(expectedMultiplier, subject.Multiplier, "Multiplier was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with valid baseValue of 25 and multiplier is Triple
        /// THEN the Multiplier property will return Double
        /// </summary>
        [TestMethod]
        public void MultiplierProperty_BaseValue25MultiplierTriple_ReturnsDouble()
        {
            // Arrange
            SegmentMultiplier expectedMultiplier = SegmentMultiplier.Double;

            // Act
            Dart subject = new Dart(25, SegmentMultiplier.Triple);

            // Assert
            Assert.AreEqual(expectedMultiplier, subject.Multiplier, "Multiplier was not the expected value");
        }

        #endregion

        #region Value Property

        /// <summary>
        /// WHEN a Dart is instantiated with a SegmentMultiplier.Miss
        /// THEN the Value property will always return 0
        /// </summary>
        [TestMethod]
        public void ValueProperty_DartWithMissMultiplier_Returns0()
        {
            // Arrange
            int expectedValue = 0;

            // Act
            Dart subject = new Dart(5, SegmentMultiplier.Miss);

            // Assert
            Assert.AreEqual(expectedValue, subject.Value, "Value was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a SegmentMultiplier.Single
        /// THEN the Value property will always return the BaseValue
        /// </summary>
        [TestMethod]
        public void ValueProperty_DartWithSingleMultiplier_ReturnsBaseValue()
        {
            // Arrange
            int expectedValue = 5;

            // Act
            Dart subject = new Dart(expectedValue, SegmentMultiplier.Single);

            // Assert
            Assert.AreEqual(expectedValue, subject.Value, "Value was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a SegmentMultiplier.Double
        /// THEN the Value property will always return the BaseValue multiplied by two
        /// </summary>
        [TestMethod]
        public void ValueProperty_DartWithDoubleMultiplier_ReturnsBaseValueTimesTwo()
        {
            // Arrange
            int baseValue = 5;
            int expectedValue = baseValue * 2;

            // Act
            Dart subject = new Dart(baseValue, SegmentMultiplier.Double);

            // Assert
            Assert.AreEqual(expectedValue, subject.Value, "Value was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a SegmentMultiplier.Triple
        /// THEN the Value property will always return the BaseValue multiplied by three
        /// </summary>
        [TestMethod]
        public void ValueProperty_DartWithTripleMultiplier_ReturnsBaseValueTimesTwo()
        {
            // Arrange
            int baseValue = 5;
            int expectedValue = baseValue * 3;

            // Act
            Dart subject = new Dart(baseValue, SegmentMultiplier.Triple);

            // Assert
            Assert.AreEqual(expectedValue, subject.Value, "Value was not the expected value");
        }

        #endregion

        #region SegmentName Property

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 1
        /// THEN the SegmentName Property will return a string representation of the BaseValue.
        /// </summary>
        [TestMethod]
        public void SegmentNameProperty_BaseValueOf1_ReturnsBaseValueAsString()
        {
            // Arrange
            int baseValue = 1;

            // Act
            Dart subject = new Dart(baseValue, SegmentMultiplier.Double);

            // Assert
            Assert.AreEqual(baseValue.ToString(), subject.SegmentName, "SegmentName was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 20
        /// THEN the SegmentName Property will return a string representation of the BaseValue.
        /// </summary>
        [TestMethod]
        public void SegmentNameProperty_BaseValueOf20_ReturnsBaseValueAsString()
        {
            // Arrange
            int baseValue = 20;

            // Act
            Dart subject = new Dart(baseValue, SegmentMultiplier.Double);

            // Assert
            Assert.AreEqual(baseValue.ToString(), subject.SegmentName, "SegmentName was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 25
        /// THEN the SegmentName Property will return 'Bull'.
        /// </summary>
        [TestMethod]
        public void SegmentNameProperty_BaseValueOf25_ReturnsBull()
        {
            // Arrange
            int baseValue = 25;

            // Act
            Dart subject = new Dart(baseValue, SegmentMultiplier.Double);

            // Assert
            Assert.AreEqual("Bull", subject.SegmentName, "SegmentName was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 0
        /// THEN the SegmentName Property will return 'Miss'.
        /// </summary>
        [TestMethod]
        public void SegmentNameProperty_BaseValueOf0_ReturnsMiss()
        {
            // Arrange
            int baseValue = 0;

            // Act
            Dart subject = new Dart(baseValue, SegmentMultiplier.Double);

            // Assert
            Assert.AreEqual("Miss", subject.SegmentName, "SegmentName was not the expected value");
        }

        #endregion

        #region Abbreviation

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 0
        /// THEN the Abbreviation property will return "Miss"
        /// </summary>
        [TestMethod]
        public void AbbreviationProperty_BaseValueOf0_ReturnsMiss()
        {
            // Arrange
            int baseValue = 0;

            // Act
            Dart subject = new Dart(baseValue, SegmentMultiplier.Miss);

            // Assert
            Assert.AreEqual("Miss", subject.Abbreviation, "Abbreviation was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 1 and a SegmentMultiplier.Single
        /// THEN the Abbreviation property will return "S1"
        /// </summary>
        [TestMethod]
        public void AbbreviationProperty_BaseValueOf1WithSignle_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(1, SegmentMultiplier.Single);

            // Assert
            Assert.AreEqual("S1", subject.Abbreviation, "Abbreviation was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 1 and a SegmentMultiplier.Double
        /// THEN the Abbreviation property will return "D1"
        /// </summary>
        [TestMethod]
        public void AbbreviationProperty_BaseValueOf1WithDouble_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(1, SegmentMultiplier.Double);

            // Assert
            Assert.AreEqual("D1", subject.Abbreviation, "Abbreviation was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 1 and a SegmentMultiplier.Triple
        /// THEN the Abbreviation property will return "T1"
        /// </summary>
        [TestMethod]
        public void AbbreviationProperty_BaseValueOf1WithTriple_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(1, SegmentMultiplier.Triple);

            // Assert
            Assert.AreEqual("T1", subject.Abbreviation, "Abbreviation was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 20 and a SegmentMultiplier.Triple
        /// THEN the Abbreviation property will return "T20"
        /// </summary>
        [TestMethod]
        public void AbbreviationProperty_BaseValueOf20WithTriple_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(20, SegmentMultiplier.Triple);

            // Assert
            Assert.AreEqual("T20", subject.Abbreviation, "Abbreviation was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 20 and a SegmentMultiplier.Single
        /// THEN the Abbreviation property will return "S20"
        /// </summary>
        [TestMethod]
        public void AbbreviationProperty_BaseValueOf20WithSignle_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(20, SegmentMultiplier.Single);

            // Assert
            Assert.AreEqual("S20", subject.Abbreviation, "Abbreviation was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 20 and a SegmentMultiplier.Double
        /// THEN the Abbreviation property will return "D20"
        /// </summary>
        [TestMethod]
        public void AbbreviationProperty_BaseValueOf20WithDouble_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(20, SegmentMultiplier.Double);

            // Assert
            Assert.AreEqual("D20", subject.Abbreviation, "Abbreviation was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 25 and a SegmentMultiplier.Single
        /// THEN the Abbreviation property will return "SB"
        /// </summary>
        [TestMethod]
        public void AbbreviationProperty_BaseValueOf25WithSignle_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(25, SegmentMultiplier.Single);

            // Assert
            Assert.AreEqual("SB", subject.Abbreviation, "Abbreviation was not the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 25 and a SegmentMultiplier.Double
        /// THEN the Abbreviation property will return "DB"
        /// </summary>
        [TestMethod]
        public void AbbreviationProperty_BaseValueOf25WithDouble_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(25, SegmentMultiplier.Double);

            // Assert
            Assert.AreEqual("DB", subject.Abbreviation, "Abbreviation was not the expected value");
        }

        #endregion

        #region ToString()

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 0
        /// THEN the ToString method will return "Miss"
        /// </summary>
        [TestMethod]
        public void ToString_BaseValueOf0_ReturnsMiss()
        {
            // Arrange

            // Act
            Dart subject = new Dart(0, SegmentMultiplier.Miss);

            // Assert
            Assert.AreEqual("Miss", subject.ToString(), "ToString did not return the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 1 and a SegmentMultiplier.Single
        /// THEN the ToString method will return "Single 1"
        /// </summary>
        [TestMethod]
        public void ToString_BaseValueOf1WithSignle_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(1, SegmentMultiplier.Single);

            // Assert
            Assert.AreEqual("Single 1", subject.ToString(), "ToString did not return the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 1 and a SegmentMultiplier.Double
        /// THEN the ToString method will return "Double 1"
        /// </summary>
        [TestMethod]
        public void ToString_BaseValueOf1WithDouble_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(1, SegmentMultiplier.Double);

            // Assert
            Assert.AreEqual("Double 1", subject.ToString(), "ToString did not return the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 1 and a SegmentMultiplier.Triple
        /// THEN the ToString method will return "Triple 1"
        /// </summary>
        [TestMethod]
        public void ToString_BaseValueOf1WithTriple_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(1, SegmentMultiplier.Triple);

            // Assert
            Assert.AreEqual("Triple 1", subject.ToString(), "ToString did not return the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 20 and a SegmentMultiplier.Triple
        /// THEN the ToString method will return "Triple 20"
        /// </summary>
        [TestMethod]
        public void ToString_BaseValueOf20WithTriple_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(20, SegmentMultiplier.Triple);

            // Assert
            Assert.AreEqual("Triple 20", subject.ToString(), "ToString did not return the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 20 and a SegmentMultiplier.Single
        /// THEN the ToString method will return "Single 20"
        /// </summary>
        [TestMethod]
        public void ToString_BaseValueOf20WithSignle_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(20, SegmentMultiplier.Single);

            // Assert
            Assert.AreEqual("Single 20", subject.ToString(), "ToString did not return the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 20 and a SegmentMultiplier.Double
        /// THEN the ToSTring method will return "Double 20"
        /// </summary>
        [TestMethod]
        public void ToString_BaseValueOf20WithDouble_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(20, SegmentMultiplier.Double);

            // Assert
            Assert.AreEqual("Double 20", subject.ToString(), "ToString did not return the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 25 and a SegmentMultiplier.Single
        /// THEN the ToString method will return "Single Bull"
        /// </summary>
        [TestMethod]
        public void ToString_BaseValueOf25WithSignle_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(25, SegmentMultiplier.Single);

            // Assert
            Assert.AreEqual("Single Bull", subject.ToString(), "ToString did not return the expected value");
        }

        /// <summary>
        /// WHEN a Dart is instantiated with a BaseValue of 25 and a SegmentMultiplier.Double
        /// THEN the ToString method will return "Double Bull"
        /// </summary>
        [TestMethod]
        public void ToString_BaseValueOf25WithDouble_ReturnsExpectedValue()
        {
            // Arrange

            // Act
            Dart subject = new Dart(25, SegmentMultiplier.Double);

            // Assert
            Assert.AreEqual("Double Bull", subject.ToString(), "ToString did not return the expected value");
        }

        #endregion

        #region IsValidScore(int)

        /// <summary>
        /// WHEN a valid score is passed in
        /// THEN true is returned.
        /// </summary>
        [TestMethod]
        public void IsValidScore_ValidScores_ReturnsTrue()
        {
            // Arrange
            List<int> validValues = new List<int>
            { 0, 1, 21, 25, 33, 42, 50, 57, 60 };

            foreach (int item in validValues)
            {
                // Act
                bool result = Dart.IsValidScore(item);

                // Assert
                Assert.IsTrue(result, string.Format("{0} was expected to be valid", item));
            }
        }

        /// <summary>
        /// WHEN an invalid score is passed in
        /// THEN false is returned.
        /// </summary>
        [TestMethod]
        public void IsValidScore_InvalidScores_ReturnsFalse()
        {
            // Arrange
            List<int> invalidValues = new List<int>
            { -1, 23, 29, 35, 43, 52, 59, 61, 100 };

            foreach (int item in invalidValues)
            {
                // Act
                bool result = Dart.IsValidScore(item);

                // Assert
                Assert.IsFalse(result, string.Format("{0} was expected to be invalid", item));
            }
        }

        #endregion

        #region IsValidDouble(int)

        /// <summary>
        /// WHEN valid points are passed in
        /// THEN true is returned.
        /// </summary>
        [TestMethod]
        public void IsValidDouble_ValidScores_ReturnsTrue()
        {
            // Arrange
            List<int> validValues = new List<int>
            { 2, 4, 16, 20, 30, 40, 50 };

            foreach (int val in validValues)
            {
                // Act
                bool result = Dart.IsValidDouble(val);

                // Assert
                Assert.IsTrue(result, string.Format("{0} was expected to be valid", val));
            }
        }

        /// <summary>
        /// WHEN invalid points are passed in
        /// THEN false is returned.
        /// </summary>
        [TestMethod]
        public void IsValidDouble_InvalidScores_ReturnsFalse()
        {
            // Arrange
            List<int> validValues = new List<int>
            { 0, 1, 17, 21, 31, 39, 41, 49, 51 };

            foreach (int val in validValues)
            {
                // Act
                bool result = Dart.IsValidDouble(val);

                // Assert
                Assert.IsFalse(result, string.Format("{0} was expected to be invalid", val));
            }
        }

        #endregion
    }
}
