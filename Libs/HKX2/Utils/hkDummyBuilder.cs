using System.Reflection;
using System.Reflection.Emit;
namespace HKX2.Utils
{
    public static class hkDummyBuilder
    {
        public static Dictionary<(System.Type, System.Type), System.Type> ClassCache { get; set; } = new();

        private static MethodInfo GetRequiredMethod(System.Type type, string name)
        {
            return type.GetMethod(name) ?? throw new InvalidOperationException($"Required method '{name}' was not found on type '{type}'.");
        }

        public static System.Type CreateDummyType(System.Type targetType, System.Type dummyType)
        {
            if (ClassCache.TryGetValue((targetType, dummyType), out var cachedDummy))
                return cachedDummy;

            AssemblyName aName = new(targetType.Name);
            var ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            var moduleName = aName.Name ?? targetType.Name;
            var mb = ab.DefineDynamicModule(moduleName);
            var tb = mb.DefineType(moduleName, TypeAttributes.Public, dummyType);

            var fbIsDummy = tb.DefineField("m_IsDummy", typeof(bool), FieldAttributes.Public);
            var fbValue = tb.DefineField("m_ConflictValue", typeof(IHavokObject), FieldAttributes.Public);

            // proxy methods
            var targetReadHkx = GetRequiredMethod(targetType, "Read");
            var targetWriteHkx = GetRequiredMethod(targetType, "Write");
            var targetReadXml = GetRequiredMethod(targetType, "ReadXml");
            var targetWriteXml = GetRequiredMethod(targetType, "WriteXml");


            var mReadHkxInfo = GetRequiredMethod(dummyType, "Read");
            var mReadHkxBody = tb.DefineMethod(mReadHkxInfo.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot | MethodAttributes.HideBySig, mReadHkxInfo.ReturnType, mReadHkxInfo.GetParameters().Select(t => t.ParameterType).ToArray());

            var readHkxIl = mReadHkxBody.GetILGenerator();
            readHkxIl.Emit(OpCodes.Nop);
            readHkxIl.Emit(OpCodes.Ldarg_0);
            readHkxIl.Emit(OpCodes.Ldfld, fbValue);
            readHkxIl.Emit(OpCodes.Ldarg_1);
            readHkxIl.Emit(OpCodes.Ldarg_2);
            readHkxIl.Emit(OpCodes.Callvirt, targetReadHkx);
            readHkxIl.Emit(OpCodes.Nop);
            readHkxIl.Emit(OpCodes.Ret);

            var mWriteHkxInfo = GetRequiredMethod(dummyType, "Write");
            var mWriteHkxBody = tb.DefineMethod(mWriteHkxInfo.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot | MethodAttributes.HideBySig, mWriteHkxInfo.ReturnType, mWriteHkxInfo.GetParameters().Select(t => t.ParameterType).ToArray());

            var writeHkxIl = mWriteHkxBody.GetILGenerator();
            writeHkxIl.Emit(OpCodes.Nop);
            writeHkxIl.Emit(OpCodes.Ldarg_0);
            writeHkxIl.Emit(OpCodes.Ldfld, fbValue);
            writeHkxIl.Emit(OpCodes.Ldarg_1);
            writeHkxIl.Emit(OpCodes.Ldarg_2);
            writeHkxIl.Emit(OpCodes.Callvirt, targetWriteHkx);
            writeHkxIl.Emit(OpCodes.Nop);
            writeHkxIl.Emit(OpCodes.Ret);

            var mReadXmlInfo = GetRequiredMethod(dummyType, "ReadXml");
            var mReadXmlBody = tb.DefineMethod(mReadXmlInfo.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot | MethodAttributes.HideBySig, mReadXmlInfo.ReturnType, mReadXmlInfo.GetParameters().Select(t => t.ParameterType).ToArray());

            var readXmlIl = mReadXmlBody.GetILGenerator();
            readXmlIl.Emit(OpCodes.Nop);
            readXmlIl.Emit(OpCodes.Ldarg_0);
            readXmlIl.Emit(OpCodes.Ldfld, fbValue);
            readXmlIl.Emit(OpCodes.Ldarg_1);
            readXmlIl.Emit(OpCodes.Ldarg_2);
            readXmlIl.Emit(OpCodes.Callvirt, targetReadXml);
            readXmlIl.Emit(OpCodes.Nop);
            readXmlIl.Emit(OpCodes.Ret);

            var mWriteXmlInfo = GetRequiredMethod(dummyType, "WriteXml");
            var mWriteXmlBody = tb.DefineMethod(mWriteXmlInfo.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot | MethodAttributes.HideBySig, mWriteXmlInfo.ReturnType, mWriteXmlInfo.GetParameters().Select(t => t.ParameterType).ToArray());

            var writeXmlIl = mWriteXmlBody.GetILGenerator();
            writeXmlIl.Emit(OpCodes.Nop);
            writeXmlIl.Emit(OpCodes.Ldarg_0);
            writeXmlIl.Emit(OpCodes.Ldfld, fbValue);
            writeXmlIl.Emit(OpCodes.Ldarg_1);
            writeXmlIl.Emit(OpCodes.Ldarg_2);
            writeXmlIl.Emit(OpCodes.Callvirt, targetWriteXml);
            writeXmlIl.Emit(OpCodes.Nop);
            writeXmlIl.Emit(OpCodes.Ret);


            var dummy = tb.CreateType() ?? throw new InvalidOperationException($"Failed to create dummy type '{moduleName}'.");
            ClassCache.Add((targetType, dummyType), dummy);
            return dummy;
        }

        public static dynamic CreateDummy<T>(T targetObject, System.Type dummyType) where T : IHavokObject
        {
            var type = CreateDummyType(targetObject.GetType(), dummyType);
            dynamic dummy = Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Failed to create dummy instance of '{type}'.");
            dummy.m_ConflictValue = targetObject;
            dummy.m_IsDummy = true;
            dummy.Signature = targetObject.Signature;
            return dummy;
        }
    }
}
