using NetArchTest.Rules;
using FluentAssertions; // Agregar esta directiva using

namespace AgenciaViajes.ArchitectureTests.Layer
{
    public sealed class LayerTests : BaseTests
	{
		[Fact]
		public void Domain_Should_Not_Have_Dependencies_On_Others()
		{
			var result = Types.InAssembly(DomainAssembly)
			.Should()
			.NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
			.GetResult();
			result.IsSuccessful.Should().BeTrue();
		}

		[Fact]
		public void DomainLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
		{
			var result = Types.InAssembly(DomainAssembly)
				.Should()
				.NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
				.GetResult();
			result.IsSuccessful.Should().BeTrue();
		}

		[Fact]
		public void InfrastructureLayer_ShouldNotHaveDependencyOn_PresentationLayer()
		{
			var result = Types.InAssembly(InfrastructureAssembly)
				.Should()
				.NotHaveDependencyOn(HostAssembly.GetName().Name)
				.GetResult();
			result.IsSuccessful.Should().BeTrue();
		}
	}
}
