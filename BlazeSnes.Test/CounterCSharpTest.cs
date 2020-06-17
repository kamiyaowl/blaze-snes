using System;
using Xunit;
using Bunit;
using Bunit.Mocking.JSInterop;
using static Bunit.ComponentParameterFactory;
using BlazeSnes.Pages;

namespace BlazeSnes.Test
{
    /// <summary>
    /// These tests are written entirely in C#.
    /// Learn more at https://bunit.egilhansen.com/docs/
    /// </summary>
    public class CounterCSharpTests : TestContext
    {
        [Fact]
        public void CounterStartsAtZero()
        {
            // Arrange
            var cut = RenderComponent<Counter>();

            // Assert that content of the paragraph shows counter at zero
            cut.Find("p").MarkupMatches("<p>Current count: 0</p>"); 
        }

        [Fact]
        public void ClickingButtonIncrementsCounter()
        {
            // Arrange
            var cut = RenderComponent<Counter>();

            // Act - click button to increment counter
            cut.Find("button").Click();

            // Assert that the counter was incremented
            cut.Find("p").MarkupMatches("<p>Current count: 1</p>");
        }
    }
}