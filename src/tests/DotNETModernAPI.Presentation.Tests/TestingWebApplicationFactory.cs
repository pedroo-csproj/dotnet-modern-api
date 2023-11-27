using Microsoft.AspNetCore.Mvc.Testing;

namespace DotNETModernAPI.Presentation.Tests;

public class TestingWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class { }
