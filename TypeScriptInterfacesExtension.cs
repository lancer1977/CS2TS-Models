using System.Reflection;
using System.Runtime.CompilerServices;

namespace InterfacesToTS;

public static class TypeScriptInterfacesExtension
{
    private static List<string> FileLocations = new List<string>();

    private static List<Type> EnumList = new List<Type>();

    public static void GenerateTypeScriptInterfaces(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }

        var sourcePath = path + "\\src";
        Directory.CreateDirectory(path);
        File.Copy("package.json",path + "\\package.json");
        File.Copy("tsconfig.json", path + "\\tsconfig.json");
        var typeList = new List<Type>();
        var assemblies = InterfacesToTS.Constants.Assemblies;
        foreach (var item in assemblies)
        {
            typeList.AddRange(item.GetTypes().Where(x=>x.IsInterface));
        }
        typeList.Add(typeof(TimeSpan));

        GenerateTypes(sourcePath, typeList);
        GenerateEnums(sourcePath, EnumList);
        CreateIndex(sourcePath);
    }

    private static void CreateRollup(string path)
    {
        var lines = new List<string>() { "import typescript from 'rollup-plugin-typescript2'", "export default ["};
        foreach (var item in FileLocations)
        {
            var endPath = item.Replace("\\", "/");
            lines.Add("{");
            lines.Add("input: './" + endPath + "',");
            lines.Add("output:");
            lines.Add("{");
            lines.Add("file: './lib/" + endPath.Replace(".ts", ".esm.js") + "',");
            lines.Add("format: 'esm',");
            lines.Add("},");
            lines.Add("plugins: [typescript()],");
            lines.Add("},");
            lines.Add("{");
            lines.Add("input: './" + endPath + "',");
            lines.Add("output:");
            lines.Add("{");
            lines.Add("file: './lib/" + endPath.Replace(".ts", ".js") + "',");
            lines.Add("format: 'cjs',");
            lines.Add("},");
            lines.Add("plugins: [typescript()],");
            lines.Add("},");
        }
        lines.Add("]");
        var fullpath = Path.Combine(path, "index.js");
        File.WriteAllLines(fullpath, lines);
    }

    private static void CreateIndex(string path)
    {
        var lines = new List<string>(); 
        foreach (var item in FileLocations)
        {
            var local = item.Replace("\\", "/");
            var type = local.Substring(item.LastIndexOf("/")+1).Replace(".ts", "");
            lines.Add("export { " + type + " } from '" + local + "'");
         
        }
        var fullpath = Path.Combine(path, "index.js");
        File.WriteAllLines(fullpath, lines);
    }


    private static void GenerateEnums(string path, List<Type> enumList)
    {
        //path += "/Enums";
        foreach (var type in enumList)
        {
            (string, string[]) tsType = ConvertCs2Ts(type);
            var fullPath = Path.Combine(path, tsType.Item1);
            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            FileLocations.Add(fullPath);
            File.WriteAllLines(fullPath, tsType.Item2);
        }
    }

    private static void GenerateTypes(string path, List<Type> types)
    {
        foreach (var type in types)
        {
            (string, string[]) tsType = ConvertCs2Ts(type);
            var fullPath = Path.Combine(path, tsType.Item1);
            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            FileLocations.Add(fullPath);
            File.WriteAllLines(fullPath, tsType.Item2);
        }
    }

    private static Type ReplaceByGenericArgument(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }
        if (!type.IsConstructedGenericType)
        {
            return type;
        }
        var genericArgument = type.GenericTypeArguments.First();
        var isTask = type.GetGenericTypeDefinition() == typeof(Task<>);
        var isEnumerable = typeof(IEnumerable<>).MakeGenericType(genericArgument).IsAssignableFrom(type);
        if (!isTask && !isEnumerable)
        {
            throw new InvalidOperationException();
        }
        if (genericArgument.IsConstructedGenericType)
        {
            return ReplaceByGenericArgument(genericArgument);
        }
        return genericArgument;
    }

    private static (string Name, string[] Lines) ConvertCs2Ts(Type type)
    {
        var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 3);
        defaultInterpolatedStringHandler.AppendFormatted(type.Namespace?.Replace(".", "/"));
        defaultInterpolatedStringHandler.AppendLiteral("/");
        defaultInterpolatedStringHandler.AppendFormatted(type.Name);
        //defaultInterpolatedStringHandler.AppendFormatted(type.IsEnum ? "" : ".Interface");
        defaultInterpolatedStringHandler.AppendLiteral(".ts");
        var filename = defaultInterpolatedStringHandler.ToStringAndClear();
        var types = GetAllNestedTypes(type);
        var lines = new List<string>();
        var array = types;
        foreach (var t in array)
        {
            lines.Add("");
            if (t.IsClass || t.IsInterface)
            {
                ConvertClassOrInterface(lines, t);
                continue;
            }
            if (t.IsEnum)
            {
                ConvertEnum(lines, t);
                continue;
            }

 //structs??
 try
 {
     ConvertClassOrInterface(lines, t);
 }
 catch (Exception ex)
 {
     throw;
 }
           
        }
        return (filename, lines.ToArray());
    }

    private static void ConvertClassOrInterface(IList<string> lines, Type type)
    {
        lines.Add("export interface " + type.Name + " {");
        var headers = new Dictionary<string, TsProp>();
        foreach (var property in from p in type.GetProperties()
                 where p.GetMethod?.IsPublic ?? true
                 select p)
        {
            var propType = property.PropertyType;
            var arrayType = GetArrayOrEnumerableType(propType);
            var nullableType = GetNullableType(propType);
            var typeToUse = nullableType ?? arrayType ?? propType;
            var convertedType = ConvertType(typeToUse);
            var suffix = "";
            suffix = ((arrayType != null) ? "[]" : suffix);
            suffix = ((nullableType != null) ? "|null" : suffix);
            var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 3);
            defaultInterpolatedStringHandler.AppendLiteral("  ");
            defaultInterpolatedStringHandler.AppendFormatted(CamelCaseName(property.Name));
            defaultInterpolatedStringHandler.AppendLiteral(": ");
            defaultInterpolatedStringHandler.AppendFormatted(convertedType.Name);
            defaultInterpolatedStringHandler.AppendFormatted(suffix);
            defaultInterpolatedStringHandler.AppendLiteral(";");
            lines.Add(defaultInterpolatedStringHandler.ToStringAndClear());
            if (convertedType.IsExternal)
            {
                headers.TryAdd(convertedType.Name, convertedType);
            }
        }
        if (headers.Any())
        {
            foreach (var item in headers.Values)
            {
                var head = "import {" + item.Name + "} from \"src/" 
                           + item.ExternalPath.Replace('.', '/') 
                           //+ (item.IsEnum ? "" : ".Interface")
                           + "\"";
                lines.Insert(0, head);
            }
        }
        lines.Add("}");
    }

    private static TsProp ConvertType(Type type)
    {
        if (Constants.ConvertedTypes.ContainsKey(type))
        {
            return new TsProp(Constants.ConvertedTypes[type], isExternal: false, "", isEnum: false);
        }
        if (type.IsEnum)
        {
            EnumList.Add(type);
        }
        if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
        {
            var keyType = type.GenericTypeArguments[0];
            var valueType = type.GenericTypeArguments[1];
            var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
            defaultInterpolatedStringHandler.AppendLiteral("{ [key: ");
            defaultInterpolatedStringHandler.AppendFormatted(ConvertType(keyType));
            defaultInterpolatedStringHandler.AppendLiteral("]: ");
            defaultInterpolatedStringHandler.AppendFormatted(ConvertType(valueType));
            defaultInterpolatedStringHandler.AppendLiteral(" }");
            return new TsProp(defaultInterpolatedStringHandler.ToStringAndClear(), isExternal: true, type.FullName, isEnum: true);
        }
        return new TsProp(type.Name, isExternal: true, type.FullName, !type.IsEnum);
    }

    private static void ConvertEnum(IList<string> lines, Type type)
    {
        var enumValues = type.GetEnumValues().Cast<int>().ToArray();
        var enumNames = type.GetEnumNames();
        lines.Add("export enum " + type.Name + " {");
        for (var i = 0; i < enumValues.Length; i++)
        {
            var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
            defaultInterpolatedStringHandler.AppendLiteral("  ");
            defaultInterpolatedStringHandler.AppendFormatted(enumNames[i]);
            defaultInterpolatedStringHandler.AppendLiteral(" = ");
            defaultInterpolatedStringHandler.AppendFormatted(enumValues[i]);
            defaultInterpolatedStringHandler.AppendLiteral(",");
            lines.Add(defaultInterpolatedStringHandler.ToStringAndClear());
        }
        lines.Add("}");
    }

    private static Type[] GetAllNestedTypes(Type type)
    {
        return new Type[1] { type }.Concat(type.GetNestedTypes().SelectMany(GetAllNestedTypes)).ToArray();
    }

    private static Type GetArrayOrEnumerableType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }
        if (type.IsConstructedGenericType)
        {
            var typeArgument = type.GenericTypeArguments.First();
            if (typeof(IEnumerable<>).MakeGenericType(typeArgument).IsAssignableFrom(type))
            {
                return typeArgument;
            }
        }
        return null;
    }

    private static Type GetNullableType(Type type)
    {
        if (type.IsConstructedGenericType)
        {
            var typeArgument = type.GenericTypeArguments.First();
            if (typeArgument.IsValueType && typeof(Nullable<>).MakeGenericType(typeArgument).IsAssignableFrom(type))
            {
                return typeArgument;
            }
        }
        return null;
    }

    private static string CamelCaseName(string pascalCaseName)
    {
        return pascalCaseName[0].ToString().ToLower() + pascalCaseName.Substring(1);
    }
}