using FluentAssertions;
using System.Reflection;
using NetArchTest.Rules;
using AgenciaViajes.Domain.Entities;

namespace AgenciaViajes.ArchitectureTests.Domain
{
	public sealed class DomainTests : BaseTests
	{
		[Fact]
		public void Entities_Should_HavePrivateParameterlessConstructor()
		{
			var entityTypes = Types.InAssembly(DomainAssembly)
				.That()
				.Inherit(typeof(EntityBase))
				.GetTypes();
			var failingTypes = new List<Type>();
			foreach (var entityType in entityTypes)
			{
				var constructors = entityType.GetConstructors(
					BindingFlags.NonPublic |
								BindingFlags.Instance);
				if (!constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0))
				{
					failingTypes.Add(entityType);
				}

				failingTypes.Should().BeEmpty();
			}
		}

		[Fact]
		public void Entities_Should_Have_PrivateSetter()
		{
			var entityTypes = Types.InAssembly(DomainAssembly)
				.That()
				.AreClasses()
				.And()
				.Inherit(typeof(EntityBase))
				.GetTypes();

			foreach (var entityType in entityTypes)
			{
				var properties = entityType.GetProperties();
				foreach (var property in properties)
				{
					if (property.CanWrite)
					{
						property.SetMethod.Should().NotBeNull()
							.And.Match(setMethod => setMethod.IsPrivate || setMethod.IsFamily || setMethod.IsFamilyOrAssembly,
								$"{property.Name} should have a private or protected setter.", property.DeclaringType?.FullName);
					}
				}
			}
		}
	}
}
