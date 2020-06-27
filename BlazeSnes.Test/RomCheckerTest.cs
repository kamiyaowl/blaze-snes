using System;

using BlazeSnes.Pages;

using Blazor.FileReader;

using Bunit;
using Bunit.Mocking.JSInterop;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit;

using static Bunit.ComponentParameterFactory;

namespace BlazeSnes.Test {
    public class RomCheckerTest : TestContext {
        [Fact]
        public void PageStartup() {
            // inject services
            using var ctx = new TestContext();
            ctx.Services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);
            var cut = ctx.RenderComponent<RomChecker>();

            // とりあえずページが出てるかみたい
            cut.Find("h1").MarkupMatches("<h1>ROM Checker</h1>");
        }
    }
}