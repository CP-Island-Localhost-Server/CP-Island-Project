using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class TestFixtureSetUpAttribute : NUnitAttribute
	{
	}
}
