using CMCS_Part2.Models;

namespace CMCS_Part2.Tests.UnitTests
{
    [TestClass]
    public class LecturerTests
    {
        [TestMethod]
        public void Lecturer_DefaultCreatedDate_IsSet()
        {
            // Arrange & Act
            var lecturer = new Lecturer(); // [1]

            // Assert
            Assert.IsTrue(lecturer.CreatedDate > DateTime.MinValue); // [2]
            Assert.IsTrue(lecturer.CreatedDate <= DateTime.Now); // [2]
        }

        [TestMethod]
        public void Lecturer_ClaimsCollection_Initialized()
        {
            // Arrange & Act
            var lecturer = new Lecturer(); // [1]

            // Assert
            Assert.IsNotNull(lecturer.Claims); // [3]
            Assert.AreEqual(0, lecturer.Claims.Count); // [3]
        }

        [TestMethod]
        public void Lecturer_ValidEmail_ValidationPasses()
        {
            // Arrange
            var lecturer = new Lecturer // [1]
            {
                Name = "Test Lecturer",
                Email = "valid.email@university.com"
            };

            // Act
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(lecturer); // [4]
            var validationResults = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>(); // [4]
            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(lecturer, validationContext, validationResults, true); // [4]

            // Assert
            Assert.IsTrue(isValid); // [5]
        }
    }
}

// [1] Microsoft Docs. “Classes and Objects (C# Programming Guide).” Microsoft Learn, 2024.  
// [2] Microsoft Docs. “DateTime Structure.” Microsoft Learn, 2024.  
// [3] NUnit Documentation. “Asserting on Collections.” NUnit.org, 2024.  
// [4] Microsoft Docs. “Validator.TryValidateObject Method.” Microsoft Learn, 2024.  
// [5] Microsoft Docs. “Assert Class.” Microsoft Learn, 2024.
