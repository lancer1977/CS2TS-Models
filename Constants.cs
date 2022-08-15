namespace InterfacesToTS;

using System;
using System.Collections.Generic;
using System.Reflection;

public static class Constants
{
	public static readonly Type[] NonPrimitivesExcludeList = new Type[4]
	{
		typeof(object),
		typeof(string),
		typeof(decimal),
		typeof(void)
	};

	public static readonly IDictionary<Type, string> ConvertedTypes = new Dictionary<Type, string>
	{
		[typeof(Guid)] = "string",
		[typeof(string)] = "string",
		[typeof(char)] = "string",
		[typeof(byte)] = "number",
		[typeof(sbyte)] = "number",
		[typeof(short)] = "number",
		[typeof(ushort)] = "number",
		[typeof(int)] = "number",
		[typeof(uint)] = "number",
		[typeof(long)] = "number",
		[typeof(ulong)] = "number",
		[typeof(float)] = "number",
		[typeof(double)] = "number",
		[typeof(decimal)] = "number",
		[typeof(bool)] = "boolean",
		[typeof(object)] = "any",
		[typeof(void)] = "void",
		[typeof(DateTime)] = "Date",
        [typeof(Uri)] = "string",
		[typeof(Type)] = "any"
	};


}

