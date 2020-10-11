using System;
using System.IO;
using System.Text;
using Xunit;

namespace QIES.Cli.Shell.Tests
{
    public class InputTests
    {
        [Fact]
        public void TakeInput_Prompt_PromptsWithMessageAndFormattedPrompt()
        {
            // Arrange
            var prompt = " ----- ";
            var message = "This is a message asking for input";
            var stdIn = new StringReader("");
            var stdOut = new StringWriter();

            Console.SetIn(stdIn);
            Console.SetOut(stdOut);
            var input = new Input(prompt);

            // Act
            input.TakeInput(message);

            // Assert
            var expectedOutputBuilder = new StringBuilder();
            expectedOutputBuilder.AppendLine(message);
            expectedOutputBuilder.Append("[ ----- ]  ");

            Assert.Equal(expectedOutputBuilder.ToString(), stdOut.ToString());
        }

        [Fact]
        public void TakeInput_NoInput_ReturnsEmptyString()
        {
            // Arrange
            var stdIn = new StringReader("");
            var stdOut = new StringWriter();

            Console.SetIn(stdIn);
            Console.SetOut(stdOut);
            var input = new Input("");

            // Act
            var value = input.TakeInput("");

            // Assert
            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void TakeInput_SomeInput_ReturnsInputString()
        {
            // Arrange
            var stdIn = new StringReader("this is some input");
            var stdOut = new StringWriter();

            Console.SetIn(stdIn);
            Console.SetOut(stdOut);
            var input = new Input("");

            // Act
            var value = input.TakeInput("");

            // Assert
            Assert.Equal("this is some input", value);
        }

        [Fact]
        public void TakeNumericInput_Numeric_ReturnsInputInteger()
        {
            // Arrange
            var stdIn = new StringReader("1");
            var stdOut = new StringWriter();

            Console.SetIn(stdIn);
            Console.SetOut(stdOut);
            var input = new Input("");

            // Act
            var value = input.TakeNumericInput("");

            // Assert
            Assert.Equal(1, value);
        }

        [Fact]
        public void TakeNumericInput_NonNumeric_ThrowsInvalidDataException()
        {
            // Arrange
            var stdIn = new StringReader("NonNumeric");
            var stdOut = new StringWriter();

            Console.SetIn(stdIn);
            Console.SetOut(stdOut);
            var input = new Input("");

            // Act & Assert
            Assert.Throws<InvalidDataException>(() => input.TakeNumericInput(""));
        }
    }
}
