using System;

namespace AntFlowCore.Core.factory;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class AutoParseAttribute: Attribute
{
}