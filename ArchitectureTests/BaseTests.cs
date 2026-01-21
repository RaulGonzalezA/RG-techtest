using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AgenciaViajes.ArchitectureTests
{
    public abstract class BaseTests
    {
		protected static readonly Assembly DomainAssembly = typeof(AgenciaViajes.Domain.DomainAssemblyMarker).Assembly;
		protected static readonly Assembly ApplicationAssembly = typeof(AgenciaViajes.Application.ApplicationAssemblyMarker).Assembly;
		protected static readonly Assembly InfrastructureAssembly = typeof(AgenciaViajes.Infrastructure.InfrastructureAssemblyMarker).Assembly;
		protected static readonly Assembly HostAssembly = typeof(AgenciaViajes.Host.HostAssemblyMarker).Assembly;
	}
}
