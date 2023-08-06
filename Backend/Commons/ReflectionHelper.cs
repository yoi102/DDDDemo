using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Zack.Commons;

namespace Commons;

public static class ReflectionHelper
{
    /// <summary>
    /// 据产品名称获取程序集
    /// </summary>
    /// <param name="productName"></param>
    /// <returns></returns>
    public static IEnumerable<Assembly> GetAssembliesByProductName(string productName)
    {
        var asms = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var asm in asms)
        {
            var asmCompanyAttr = asm.GetCustomAttribute<AssemblyProductAttribute>();
            if (asmCompanyAttr != null && asmCompanyAttr.Product == productName)
            {
                yield return asm;
            }
        }
    }

    //是否是微软等的官方Assembly
    private static bool IsSystemAssembly(Assembly asm)
    {
        var asmCompanyAttr = asm.GetCustomAttribute<AssemblyCompanyAttribute>();
        if (asmCompanyAttr == null)
        {
            return false;
        }
        else
        {
            string companyName = asmCompanyAttr.Company;
            return companyName.Contains("Microsoft");
        }
    }

    private static bool IsSystemAssembly(string asmPath)
    {
        var moduleDef = AsmResolver.DotNet.ModuleDefinition.FromFile(asmPath);
        var assembly = moduleDef.Assembly;
        if (assembly == null)
        {
            return false;
        }
        var asmCompanyAttr = assembly.CustomAttributes.FirstOrDefault(c => c.Constructor?.DeclaringType?.FullName == typeof(AssemblyCompanyAttribute).FullName);
        if (asmCompanyAttr == null)
        {
            return false;
        }
        var companyName = ((AsmResolver.Utf8String?)asmCompanyAttr.Signature?.FixedArguments[0]?.Element)?.Value;
        if (companyName == null)
        {
            return false;
        }
        return companyName.Contains("Microsoft");
    }

    /// <summary>
    /// 判断file这个文件是否是程序集
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private static bool IsManagedAssembly(string file)
    {
        using var fs = File.OpenRead(file);
        using PEReader peReader = new PEReader(fs);
        return peReader.HasMetadata && peReader.GetMetadataReader().IsAssembly;
    }

    private static bool TryLoadAssembly(string asmPath, out Assembly? assembly)
    {
        AssemblyName asmName = AssemblyName.GetAssemblyName(asmPath);
        assembly = null;
        try
        {
            assembly = Assembly.Load(asmName);
        }
        catch (BadImageFormatException ex)
        {
            Debug.WriteLine(ex);
        }
        catch (FileLoadException ex)
        {
            Debug.WriteLine(ex);
        }
        //catch (FileNotFoundException ex)
        //{
        //    Debug.WriteLine(ex);
        //}

        if (assembly == null)
        {
            try
            {
                assembly = Assembly.LoadFile(asmPath);
            }
            catch (BadImageFormatException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (FileLoadException ex)
            {
                Debug.WriteLine(ex);
            }
            //catch (FileNotFoundException ex)
            //{
            //    Debug.WriteLine(ex);
            //}
        }

        return assembly != null;
    }

    private static bool TryLoadAssembly(AssemblyName assemblyName, out Assembly? assembly)
    {
        assembly = null;
        try
        {
            assembly = Assembly.Load(assemblyName);
        }
        catch (BadImageFormatException ex)
        {
            Debug.WriteLine(ex);
        }
        catch (FileLoadException ex)
        {
            Debug.WriteLine(ex);
        }
        catch (FileNotFoundException ex)
        {
            Debug.WriteLine(ex);
        }

        return assembly != null;
    }

    /// <summary>
    /// loop through all assemblies
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Assembly> GetAllReferencedAssemblies(bool skipSystemAssemblies = true)
    {
        Assembly? rootAssembly = Assembly.GetEntryAssembly();
        if (rootAssembly == null)
        {
            rootAssembly = Assembly.GetCallingAssembly();
        }
        var returnAssemblies = new HashSet<Assembly>(new AssemblyEquality());
        var loadedAssemblies = new HashSet<string>();
        var assembliesToCheck = new Queue<Assembly>();
        assembliesToCheck.Enqueue(rootAssembly);
        if (skipSystemAssemblies && IsSystemAssembly(rootAssembly) != false)
        {
            if (IsValid(rootAssembly))
            {
                returnAssemblies.Add(rootAssembly);
            }
        }
        while (assembliesToCheck.Any())
        {
            var assemblyToCheck = assembliesToCheck.Dequeue();
            foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
            {
                if (!loadedAssemblies.Contains(reference.FullName))
                {

                    if (!TryLoadAssembly(reference, out Assembly? assembly))
                    {
                        continue;
                    }
                    if (skipSystemAssemblies && IsSystemAssembly(assembly!))
                    {
                        continue;
                    }
                    assembliesToCheck.Enqueue(assembly!);
                    loadedAssemblies.Add(reference.FullName);
                    if (IsValid(assembly!))
                    {
                        returnAssemblies.Add(assembly!);
                    }
                }
            }
        }
        var asmsInBaseDir = Directory.EnumerateFiles(AppContext.BaseDirectory,
            "*.dll", new EnumerationOptions { RecurseSubdirectories = true });
        foreach (var asmPath in asmsInBaseDir)
        {
            if (!IsManagedAssembly(asmPath))
            {
                continue;
            }
            AssemblyName asmName = AssemblyName.GetAssemblyName(asmPath);
            //如果程序集已经加载过了就不再加载
            if (returnAssemblies.Any(x => AssemblyName.ReferenceMatchesDefinition(x.GetName(), asmName)))
            {
                continue;
            }
            if (skipSystemAssemblies && IsSystemAssembly(asmPath))
            {
                continue;
            }
            if (!TryLoadAssembly(asmPath, out Assembly? asm))
            {
                continue;
            }
            //Assembly assembly = Assembly.Load(asmName);
            if (!IsValid(asm!))
            {
                continue;
            }
            if (skipSystemAssemblies && IsSystemAssembly(asm!))
            {
                continue;
            }
            returnAssemblies.Add(asm!);
        }
        return returnAssemblies.ToArray();
    }

    private static bool IsValid(Assembly asm)
    {
        try
        {
            asm.GetTypes();
            asm.DefinedTypes.ToList();
            return true;
        }
        catch (ReflectionTypeLoadException)
        {
            return false;
        }
    }

    private class AssemblyEquality : EqualityComparer<Assembly>
    {
        public override bool Equals(Assembly? x, Assembly? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return AssemblyName.ReferenceMatchesDefinition(x.GetName(), y.GetName());
        }

        public override int GetHashCode([DisallowNull] Assembly obj)
        {
            return obj.GetName().FullName.GetHashCode();
        }
    }
}