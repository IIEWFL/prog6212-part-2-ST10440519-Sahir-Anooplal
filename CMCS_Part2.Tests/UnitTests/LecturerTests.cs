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
            var lecturer = new Lecturer();

            // Assert
            Assert.IsTrue(lecturer.CreatedDate > DateTime.MinValue);
            Assert.IsTrue(lecturer.CreatedDate <= DateTime.Now);
        }

        [TestMethod]
        public void Lecturer_ClaimsCollection_Initialized()
        {
            // Arrange & Act
            var lecturer = new Lecturer();

            // Assert
            Assert.IsNotNull(lecturer.Claims);
            Assert.AreEqual(0, lecturer.Claims.Count);
        }

        [TestMethod]
        public void Lecturer_ValidEmail_ValidationPasses()
        {
            // Arrange
            var lecturer = new Lecturer
            {
                Name = "Test Lecturer",
                Email = "valid.email@university.com"
            };

            // Act
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(lecturer);
            var validationResults = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(lecturer, validationContext, validationResults, true);

            // Assert
            Assert.IsTrue(isValid);
        }
    }
}