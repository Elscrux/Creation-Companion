using System.Numerics;
using System.Xml;
using System.Xml.Linq;
using HKX2.Utils;
namespace HKX2
{
    public class XmlDeserializer
    {
        public XDocument _xdoc = null!;
        private HKXHeader _header = null!;
        // store deserialized
        private Dictionary<string, IHavokObject> _objectsNameMap = null!;
        private Dictionary<string, XElement> _nameXEleMap = null!;

        private bool _ignoreNonFatalError;

        public IHavokObject Deserialize(Stream stream, HKXHeader header, bool ignoreNonFatalError = false)
        {
            _xdoc = XDocument.Load(stream, LoadOptions.SetLineInfo);
            _header = header;
            _objectsNameMap = new Dictionary<string, IHavokObject>();

            _nameXEleMap = new Dictionary<string, XElement>();

            _ignoreNonFatalError = ignoreNonFatalError;

            var hkpack = _xdoc.Element("hkpackfile");
            if (hkpack is null)
                throw new InvalidOperationException("Missing required element 'hkpackfile' in XML document.");
            var hksection = hkpack.Element("hksection");
            if (hksection is null)
                throw new InvalidOperationException("Missing required element 'hksection' under 'hkpackfile' in XML document.");

            // collect nodes
            foreach (var item in hksection.Elements())
            {
                var nameAttr = item.Attribute("name");
                if (nameAttr is null)
                {
                    var ln = ((IXmlLineInfo)item)?.LineNumber ?? -1;
                    throw new InvalidOperationException($"Missing required attribute 'name' on element at line {ln}.");
                }
                var name = nameAttr.Value;
                _nameXEleMap.Add(name, item);
            }

            var testnode = _nameXEleMap.First(kv =>
            {
                var classAttr = kv.Value.Attribute("class");
                if (classAttr is null)
                {
                    var ln = ((IXmlLineInfo)kv.Value)?.LineNumber ?? -1;
                    throw new InvalidOperationException($"Missing required attribute 'class' on element '{kv.Key}' at line {ln}.");
                }
                return classAttr.Value == "hkRootLevelContainer";
            }).Value;

            var rootrefNameAttr = testnode.Attribute("name");
            if (rootrefNameAttr is null)
            {
                var ln = ((IXmlLineInfo)testnode)?.LineNumber ?? -1;
                throw new InvalidOperationException($"Missing required attribute 'name' on root element at line {ln}.");
            }
            var rootrefName = rootrefNameAttr.Value;
            var testobj = ConstructVirtualClass<hkRootLevelContainer>(testnode);
            _objectsNameMap.Add(rootrefName, testobj);

            testobj.ReadXml(this, testnode);

            var hkRootLevelContainer = _objectsNameMap.First(item => item.Value.Signature == 0x2772c11e).Value;

            return hkRootLevelContainer;
        }

        private IHavokObject ConstructVirtualClass<T>(XElement xElement) where T : IHavokObject
        {
            var nameAttr = xElement.Attribute("name");
            if (nameAttr is null)
                throw new InvalidOperationException("Missing required attribute 'name' in XML document.");
            var name = nameAttr.Value;

            if (_objectsNameMap.TryGetValue(name, out var value))
            {
                return value;
            }

            var classAttr = xElement.Attribute("class");
            if (classAttr is null)
                throw new InvalidOperationException("Missing required attribute 'class' in XML document.");
            var hkClassName = classAttr.Value;
            var hkClass = System.Type.GetType($"HKX2.{hkClassName}");
            if (hkClass is null) throw new Exception($"Havok class type '{hkClassName}' not found!");

            var ret = Activator.CreateInstance(hkClass) as IHavokObject;
            if (ret is null) throw new Exception($"Failed to Activator.CreateInstance({hkClass})");

            if (ret is T typedRet)
                return typedRet;

            if (!_ignoreNonFatalError)
                throw new Exception($"Could not convert '{typeof(T)}' to '{ret.GetType()}'. Is source malformed?");

            return hkDummyBuilder.CreateDummy(ret, typeof(T));
        }
        private static XElement? GetPropertyElement(XContainer element, string name)
        {
            if (name.StartsWith("m_"))
            {
                name = name[2..];
            }
            var eles = element.Elements("hkparam").Where(e =>
            {
                var a = e.Attribute("name");
                if (a is null)
                {
                    var ln = ((IXmlLineInfo)e)?.LineNumber ?? -1;
                    throw new InvalidOperationException($"Missing required attribute 'name' on hkparam element at line {ln}.");
                }
                return a.Value == name;
            });
            if (!eles.Any())
            {
                return null;
            }
            return eles.First();
        }
        public T ReadClass<T>(XElement element, string name) where T : IHavokObject, new()
        {
            var ele = GetPropertyElement(element, name)?.Element("hkobject");

            var ret = new T();
            if (ele != null)
            {
                ret.ReadXml(this, ele);
            }

            return ret;
        }

        public IList<T> ReadClassArray<T>(XElement element, string name) where T : IHavokObject, new()
        {
            var eles = GetPropertyElement(element, name);
            if (eles is null)
                return Array.Empty<T>();

            var result = new List<T>();

            var numAttr = eles.Attribute("numelements");
            if (numAttr is null)
            {
                var ln = ((IXmlLineInfo)eles)?.LineNumber ?? -1;
                throw new InvalidOperationException($"Missing required attribute 'numelements' for property '{name}' at line {ln}.");
            }
            if (!int.TryParse(numAttr.Value, out var _))
            {
                throw new Exception($"numelemnets is not vaild number at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");
            }

            foreach (var e in eles.Elements("hkobject"))
            {
                var cls = new T();
                cls.ReadXml(this, e);
                result.Add(cls);
            }
            return result;
        }

        public T[] ReadClassCStyleArray<T>(XElement element, string name, short length) where T : IHavokObject, new()
        {
            var eles = GetPropertyElement(element, name);
            if (eles is null)
                return [];

            var arr = new T[length];

            if (length != eles.Elements("hkobject").Count())
                throw new Exception($"Content's elements mismatch property requierd. at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {arr.Length}");

            foreach (var (value, i) in eles.Elements("hkobject").Select((v, i) => (v, i)))
            {
                var cls = new T();
                cls.ReadXml(this, value);
                arr[i] = cls;
            }
            return arr;
        }

        public T? ReadClassPointer<T>(XElement element, string name) where T : IHavokObject, new()
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return default;

            var refName = ele.Value;
            if (refName == "null")
                return default;

            if (_objectsNameMap.TryGetValue(refName, out var value))
            {
                if (value is T typedValue)
                    return typedValue;

                throw new InvalidOperationException($"Reference symbol '{refName}' resolved to incompatible type '{value.GetType()}', expected '{typeof(T)}'. at Line {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");
            }

            if (!_nameXEleMap.TryGetValue(refName, out var refEle))
                throw new Exception($"Reference symbol '{refName}' not found. Make sure it defined somewhere. at Line {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            var ret = ConstructVirtualClass<T>(refEle);
            if (ret is not T typedRet)
                throw new InvalidOperationException($"Reference symbol '{refName}' resolved to incompatible type '{ret.GetType()}', expected '{typeof(T)}'. at Line {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");
            typedRet.ReadXml(this, refEle);
            _objectsNameMap.Add(refName, typedRet);

            return typedRet;
        }

        public IList<T> ReadClassPointerArray<T>(XElement element, string name) where T : IHavokObject, new()
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return Array.Empty<T>();

            var result = new List<T>();

            var numAttr = ele.Attribute("numelements");
            if (numAttr is null)
            {
                var ln = ((IXmlLineInfo)ele)?.LineNumber ?? -1;
                throw new InvalidOperationException($"Missing required attribute 'numelements' for property '{name}' at line {ln}.");
            }
            if (!int.TryParse(numAttr.Value, out var count))
                throw new Exception($"numelemnets is not vaild number at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            if (count == 0)
                return result;

            var refNames = ele.Value.Split(SplitSpaceList, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var refName in refNames)
            {
                if (_objectsNameMap.TryGetValue(refName, out var value))
                {
                    if (value is T typedValue)
                    {
                        result.Add(typedValue);
                        continue;
                    }

                    throw new InvalidOperationException($"Reference symbol '{refName}' resolved to incompatible type '{value.GetType()}', expected '{typeof(T)}'. at Line {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");
                }

                if (!_nameXEleMap.TryGetValue(refName, out var refEle))
                    throw new Exception($"Reference symbol '{refName}' not found. Make sure it defined somewhere. at Line {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

                var ret = ConstructVirtualClass<T>(refEle);
                if (ret is not T typedRet)
                    throw new InvalidOperationException($"Reference symbol '{refName}' resolved to incompatible type '{ret.GetType()}', expected '{typeof(T)}'. at Line {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");
                typedRet.ReadXml(this, refEle);
                _objectsNameMap.Add(refName, typedRet);

                result.Add(typedRet);
            }
            return result;
        }

        public T?[] ReadClassPointerCStyleArray<T>(XElement element, string name, short length) where T : IHavokObject, new()
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return [];

            var arr = new T?[length];

            var refNames = ele.Value.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (refNames.Length != length)
                throw new Exception($"Content's elements mismatch property requierd. at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {arr.Length}");

            foreach (var (refName, i) in refNames.Select((v, i) => (v, i)))
            {
                if (refName == "null")
                {
                    arr[i] = default;
                    continue;
                }

                if (_objectsNameMap.TryGetValue(refName, out var value))
                {
                    if (value is T typedValue)
                    {
                        arr[i] = typedValue;
                        continue;
                    }

                    throw new InvalidOperationException($"Reference symbol '{refName}' resolved to incompatible type '{value.GetType()}', expected '{typeof(T)}'. at Line {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");
                }

                if (!_nameXEleMap.TryGetValue(refName, out var refEle))
                    throw new Exception($"Reference symbol '{refName}' not found. Make sure it defined somewhere. at Line {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

                var ret = ConstructVirtualClass<T>(refEle);
                if (ret is not T typedRet)
                    throw new InvalidOperationException($"Reference symbol '{refName}' resolved to incompatible type '{ret.GetType()}', expected '{typeof(T)}'. at Line {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");
                typedRet.ReadXml(this, refEle);
                _objectsNameMap.Add(refName, typedRet);

                arr[i] = typedRet;
            }
            return arr;
        }

        public string ReadString(XElement element, string name)
        {
            // if ele exist it is and empty return empty string (stringptr)
            // if ele exist it is and '\u2400' return null (cstring)
            // if not exist it is SERIALIZE_IGNORED flag (null)
            var ele = GetPropertyElement(element, name);
            if (ele is null || ele.Value == "\u2400") return null!;
            return ele.Value.Trim();
        }

        public bool ReadBoolean(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return false;
            return bool.Parse(ele.Value);
        }

        public byte ReadByte(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null) return 0;
            return byte.Parse(ele.Value);
        }

        public sbyte ReadSByte(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null) return 0;
            return sbyte.Parse(ele.Value);
        }

        public short ReadInt16(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null) return 0;
            return short.Parse(ele.Value);
        }

        public ushort ReadUInt16(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null) return 0;
            return ushort.Parse(ele.Value);
        }

        public uint ReadUInt32(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null) return 0;
            return uint.Parse(ele.Value);
        }

        public int ReadInt32(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null) return 0;
            return int.Parse(ele.Value);
        }

        public ulong ReadUInt64(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null) return 0;
            return ulong.Parse(ele.Value);
        }

        public long ReadInt64(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null) return 0;
            return long.Parse(ele.Value);
        }

        public Half ReadHalf(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return new Half();
            return Half.Parse(ele.Value);
        }

        public float ReadSingle(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return new float();
            return float.Parse(ele.Value);
        }

        private static readonly char[] SplitCharList = ['(', ')', ',', ' ', '\n', '\r', '\t'];
        private static IEnumerable<string> Normalize(string str)
        {
            return str.Split(SplitCharList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(x => x == "-1.#IND00" ? "0.0" : x).ToArray();
        }

        public Vector4 ReadVector4(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return new Vector4();

            var vec = Normalize(ele.Value).Select(float.Parse).ToArray();
            return new Vector4(vec[0], vec[1], vec[2], vec[3]);
        }

        public Matrix4x4 ReadMatrix3(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return new Matrix4x4();

            var mat3 = Normalize(ele.Value).Select(float.Parse).ToArray();
            return new Matrix4x4(mat3[0], mat3[1], mat3[2], 0,
                                 mat3[3], mat3[4], mat3[5], 0,
                                 mat3[6], mat3[7], mat3[8], 0,
                                 0, 0, 0, 0);
        }

        public Matrix4x4 ReadMatrix4(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return new Matrix4x4();

            var mat4 = Normalize(ele.Value).Select(float.Parse).ToArray();
            return new Matrix4x4(mat4[0], mat4[1], mat4[2], mat4[3],
                                 mat4[4], mat4[5], mat4[6], mat4[7],
                                 mat4[8], mat4[9], mat4[10], mat4[11],
                                 mat4[12], mat4[13], mat4[14], mat4[15]);
        }

        public Matrix4x4 ReadTransform(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return new Matrix4x4();

            var trans = Normalize(ele.Value).Select(float.Parse).ToArray();
            return new Matrix4x4(trans[0], trans[1], trans[2], 0,
                                 trans[3], trans[4], trans[5], 0,
                                 trans[6], trans[7], trans[8], 0,
                                 trans[9], trans[10], trans[11], 1);
        }

        public Matrix4x4 ReadRotation(XElement element, string name)
        {
            return ReadMatrix3(element, name);
        }

        public Matrix4x4 ReadQSTransform(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele == null)
                return new Matrix4x4();

            var qs = Normalize(ele.Value).Select(float.Parse).ToArray();
            return new Matrix4x4(qs[0], qs[1], qs[2], 0,
                                 qs[3], qs[4], qs[5], qs[6],
                                 qs[7], qs[8], qs[9], 0,
                                 0, 0, 0, 0);
        }

        public Quaternion ReadQuaternion(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele == null) return new Quaternion();
            var quant = Normalize(ele.Value).Select(float.Parse).ToArray();
            return new Quaternion(quant[0], quant[1], quant[2], quant[3]);
        }

        private XElement? ReadBaseArray(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return null;

            var numAttr = ele.Attribute("numelements");
            if (numAttr is null)
            {
                var ln = ((IXmlLineInfo)ele)?.LineNumber ?? -1;
                throw new InvalidOperationException($"Missing required attribute 'numelements' for property '{name}' at line {ln}.");
            }
            if (!int.TryParse(numAttr.Value, out var count))
                throw new Exception($"numelemnets is not vaild number at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            if (count == 0)
                return null;

            return ele;
        }

        public IList<string> ReadStringArray(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<string>();

            return ele.Elements("hkcstring")
                      .Select(ele => ele.Value.Trim())
                      .ToList();
        }
        private static readonly char[] SplitSpaceList = [' ', '\n', '\r', '\t'];
        public IList<bool> ReadBooleanArray(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<bool>();

            return ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(bool.Parse)
                            .ToList();
        }



        public IList<byte> ReadByteArray(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<byte>();

            return ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(byte.Parse)
                            .ToList();
        }

        public IList<sbyte> ReadSByteArray(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<sbyte>();

            return ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(sbyte.Parse)
                            .ToList();
        }

        public IList<ushort> ReadUInt16Array(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<ushort>();

            return ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(ushort.Parse)
                            .ToList();
        }

        public IList<short> ReadInt16Array(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<short>();

            return ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(short.Parse)
                            .ToList();
        }

        public IList<uint> ReadUInt32Array(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<uint>();

            return ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(uint.Parse)
                            .ToList();
        }

        public IList<int> ReadInt32Array(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<int>();

            return ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(int.Parse)
                            .ToList();
        }

        public IList<ulong> ReadUInt64Array(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<ulong>();

            return ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(ulong.Parse)
                            .ToList();
        }

        public IList<long> ReadInt64Array(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<long>();

            return ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(long.Parse)
                            .ToList();
        }
        public IList<Half> ReadHalfArray(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<Half>();

            return ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(Half.Parse)
                            .ToList();
        }

        public IList<float> ReadSingleArray(XElement element, string name)
        {
            var ele = ReadBaseArray(element, name);
            if (ele is null)
                return Array.Empty<float>();

            return ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(float.Parse)
                            .ToList();
        }

        public IList<Vector4> ReadVector4Array(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return Array.Empty<Vector4>();

            var numAttr = ele.Attribute("numelements");
            if (numAttr is null)
            {
                var ln = ((IXmlLineInfo)ele)?.LineNumber ?? -1;
                throw new InvalidOperationException($"Missing required attribute 'numelements' for property '{name}' at line {ln}.");
            }
            if (!int.TryParse(numAttr.Value, out var count))
                throw new Exception($"numelemnets is not vaild number at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            if (count == 0)
                return Array.Empty<Vector4>();

            var vec4Arr = Normalize(ele.Value).Select(float.Parse).Chunk(4);
            if (vec4Arr.Count() != count)
                throw new Exception($"Vector4 element mismatch. at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            return vec4Arr.Select(vec => new Vector4(vec[0], vec[1], vec[2], vec[3]))
                          .ToList();
        }

        public IList<Matrix4x4> ReadMatrix3Array(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return Array.Empty<Matrix4x4>();

            var numAttr = ele.Attribute("numelements");
            if (numAttr is null)
            {
                var ln = ((IXmlLineInfo)ele)?.LineNumber ?? -1;
                throw new InvalidOperationException($"Missing required attribute 'numelements' for property '{name}' at line {ln}.");
            }
            if (!int.TryParse(numAttr.Value, out var count))
                throw new Exception($"numelemnets is not vaild number at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            if (count == 0)
                return Array.Empty<Matrix4x4>();

            var mat3Arr = Normalize(ele.Value).Select(float.Parse).Chunk(9);
            if (mat3Arr.Count() != count)
                throw new Exception($"Matrix3 element mismatch. at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            return mat3Arr.Select(vec => new Matrix4x4(vec[0], vec[1], vec[2], 0,
                                                       vec[3], vec[4], vec[5], 0,
                                                       vec[6], vec[7], vec[8], 0,
                                                       0, 0, 0, 0)).ToList();
        }

        public IList<Matrix4x4> ReadMatrix4Array(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return Array.Empty<Matrix4x4>();

            var numAttr = ele.Attribute("numelements");
            if (numAttr is null)
            {
                var ln = ((IXmlLineInfo)ele)?.LineNumber ?? -1;
                throw new InvalidOperationException($"Missing required attribute 'numelements' for property '{name}' at line {ln}.");
            }
            if (!int.TryParse(numAttr.Value, out var count))
                throw new Exception($"numelemnets is not vaild number at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            if (count == 0)
                return Array.Empty<Matrix4x4>();

            var mat4Arr = Normalize(ele.Value).Select(float.Parse).Chunk(16);
            if (mat4Arr.Count() != count)
                throw new Exception($"Matrix4 element mismatch. at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            return mat4Arr.Select(vec => new Matrix4x4(vec[0], vec[1], vec[2], vec[3],
                                                       vec[4], vec[5], vec[6], vec[7],
                                                       vec[8], vec[9], vec[10], vec[11],
                                                       vec[12], vec[13], vec[14], vec[15])).ToList();
        }

        public IList<Matrix4x4> ReadTransformArray(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return Array.Empty<Matrix4x4>();

            var numAttr = ele.Attribute("numelements");
            if (numAttr is null)
            {
                var ln = ((IXmlLineInfo)ele)?.LineNumber ?? -1;
                throw new InvalidOperationException($"Missing required attribute 'numelements' for property '{name}' at line {ln}.");
            }
            if (!int.TryParse(numAttr.Value, out var count))
                throw new Exception($"numelemnets is not vaild number at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            if (count == 0)
                return Array.Empty<Matrix4x4>();

            var transArr = Normalize(ele.Value).Select(float.Parse).Chunk(12);
            if (transArr.Count() != count)
                throw new Exception($"Transform element mismatch. at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            return transArr.Select(trans => new Matrix4x4(trans[0], trans[1], trans[2], 0,
                                                          trans[3], trans[4], trans[5], 0,
                                                          trans[6], trans[7], trans[8], 0,
                                                          trans[9], trans[10], trans[11], 1)).ToList();
        }

        public IList<Matrix4x4> ReadRotationArray(XElement element, string name)
        {
            return ReadMatrix3Array(element, name);
        }

        public IList<Matrix4x4> ReadQSTransformArray(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele == null)
                return Array.Empty<Matrix4x4>();

            if (!int.TryParse(ele.Attribute("numelements")?.Value, out var count))
                throw new Exception($"numelemnets is not vaild number at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            if (count == 0)
                return Array.Empty<Matrix4x4>();

            var qsArr = Normalize(ele.Value).Select(float.Parse).Chunk(10);
            if (qsArr.Count() != count)
                throw new Exception($"QSTransform element mismatch. at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            return qsArr.Select(qs => new Matrix4x4(qs[0], qs[1], qs[2], 0,
                                                    qs[3], qs[4], qs[5], qs[6],
                                                    qs[7], qs[8], qs[9], 0,
                                                    0, 0, 0, 0)).ToList();
        }

        public IList<Quaternion> ReadQuaternionArray(XElement element, string name)
        {
            var ele = GetPropertyElement(element, name);
            if (ele == null)
                return Array.Empty<Quaternion>();

            if (!int.TryParse(ele.Attribute("numelements")?.Value, out var count))
                throw new Exception($"numelemnets is not vaild number at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            if (count == 0)
                return Array.Empty<Quaternion>();

            var quantArr = Normalize(ele.Value).Select(float.Parse).Chunk(4);
            if (quantArr.Count() != count)
                throw new Exception($"Quaternion element missmatch. at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            return quantArr.Select(quant => new Quaternion(quant[0], quant[1], quant[2], quant[3])).ToList();
        }

        public TValue ReadFlag<TEnum, TValue>(XElement element, string name) where TEnum : Enum where TValue : IBinaryInteger<TValue>
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return TValue.Zero;
            return ele.Value.Split("|", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToFlagValue<TEnum, TValue>();
        }

        public TValue ReadEnum<TEnum, TValue>(XElement element, string name) where TEnum : Enum where TValue : IBinaryInteger<TValue>
        {
            var ele = GetPropertyElement(element, name);
            if (ele is null)
                return (TValue)(IConvertible)0;
            return ele.Value.ToEnumValue<TEnum, TValue>();
        }

        #region C Style Array

        private XElement? ReadBaseCStyleArray(XElement element, string name, short length)
        {
            var ele = GetPropertyElement(element, name);
            return ele;
        }

        public bool[] ReadBooleanCStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new bool[length];

            var eles = ele.Value.Split(SplitSpaceList, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (eles.Length != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {eles.Length}");

            return eles.Select(e => e == "true").ToArray();
        }

        public byte[] ReadByteCStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new byte[length];

            var eles = ele.Value.Split(SplitSpaceList, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (eles.Length != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {eles.Length}");


            return eles.Select(byte.Parse).ToArray();
        }

        public sbyte[] ReadSByteCStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new sbyte[length];

            var eles = ele.Value.Split(SplitSpaceList, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (eles.Length != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {eles.Length}");

            return eles.Select(sbyte.Parse).ToArray();
        }

        public ushort[] ReadUInt16CStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new ushort[length];

            var eles = ele.Value.Split(SplitSpaceList, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (eles.Length != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {eles.Length}");

            return eles.Select(ushort.Parse).ToArray();
        }

        public short[] ReadInt16CStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new short[length];

            var eles = ele.Value.Split(SplitSpaceList, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (eles.Length != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {eles.Length}");

            return eles.Select(short.Parse).ToArray();
        }

        public uint[] ReadUInt32CStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new uint[length];

            var eles = ele.Value.Split(SplitSpaceList, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (eles.Length != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {eles.Length}");


            return eles.Select(uint.Parse).ToArray();
        }

        public int[] ReadInt32CStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new int[length];

            var eles = ele.Value.Split(SplitSpaceList, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (eles.Length != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {eles.Length}");

            return eles.Select(int.Parse).ToArray();
        }

        public ulong[] ReadUInt64CStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new ulong[length];

            var eles = ele.Value.Split(SplitSpaceList, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (eles.Length != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {eles.Length}");


            return eles.Select(ulong.Parse).ToArray();
        }

        public long[] ReadInt64CStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new long[length];

            var eles = ele.Value.Split(SplitSpaceList, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (eles.Length != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {eles.Length}");

            return eles.Select(long.Parse).ToArray();
        }

        public Half[] ReadHalfCStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new Half[length];

            var eles = ele.Value.Split(SplitSpaceList, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (eles.Length != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {eles.Length}");

            return eles.Select(Half.Parse).ToArray();
        }

        public float[] ReadSingleCStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new float[length];

            var eles = ele.Value.Split(SplitSpaceList, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (eles.Length != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {eles.Length}");

            return eles.Select(float.Parse).ToArray();
        }

        public Vector4[] ReadVector4CStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new Vector4[length];

            var vec4arr = Normalize(ele.Value).Select(float.Parse).Chunk(4);
            if (vec4arr.Count() != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}");

            return vec4arr.Select(vec => new Vector4(vec[0], vec[1], vec[2], vec[3]))
                          .ToArray();
        }

        public Matrix4x4[] ReadMatrix3CStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new Matrix4x4[length];

            var arr = Normalize(ele.Value).Select(float.Parse).Chunk(9);
            if (arr.Count() != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {arr.Count()}");

            return arr.Select(vec => new Matrix4x4(vec[0], vec[1], vec[2], 0,
                                                       vec[3], vec[4], vec[5], 0,
                                                       vec[6], vec[7], vec[8], 0,
                                                       0, 0, 0, 0)).ToArray();
        }

        public Matrix4x4[] ReadMatrix4CStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new Matrix4x4[length];

            var arr = Normalize(ele.Value).Select(float.Parse).Chunk(16);
            if (arr.Count() != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {arr.Count()}");

            return arr.Select(vec => new Matrix4x4(vec[0], vec[1], vec[2], vec[3],
                                                   vec[4], vec[5], vec[6], vec[7],
                                                   vec[8], vec[9], vec[10], vec[11],
                                                   vec[12], vec[13], vec[14], vec[15])).ToArray();
        }

        public Matrix4x4[] ReadTransformCStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new Matrix4x4[length];

            var arr = Normalize(ele.Value).Select(float.Parse).Chunk(12);
            if (arr.Count() != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {arr.Count()}");

            return arr.Select(trans => new Matrix4x4(trans[0], trans[1], trans[2], 0,
                                                          trans[3], trans[4], trans[5], 0,
                                                          trans[6], trans[7], trans[8], 0,
                                                          trans[9], trans[10], trans[11], 1)).ToArray();
        }

        public Matrix4x4[] ReadRotationCStyleArray(XElement element, string name, short length)
        {
            return ReadMatrix3CStyleArray(element, name, length);
        }

        public Matrix4x4[] ReadQSTransformCStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new Matrix4x4[length];

            var arr = Normalize(ele.Value).Select(float.Parse).Chunk(10);
            if (arr.Count() != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {arr.Count()}");

            return arr.Select(qs => new Matrix4x4(qs[0], qs[1], qs[2], 0,
                                                    qs[3], qs[4], qs[5], qs[6],
                                                    qs[7], qs[8], qs[9], 0,
                                                    0, 0, 0, 0)).ToArray();
        }

        public Quaternion[] ReadQuaternionCStyleArray(XElement element, string name, short length)
        {
            var ele = ReadBaseCStyleArray(element, name, length);
            if (ele is null)
                return new Quaternion[length];

            var arr = Normalize(ele.Value).Select(float.Parse).Chunk(4);
            if (arr.Count() != length)
                throw new Exception($"Content's elements mismatch property require {length} at Line: {((IXmlLineInfo)element)?.LineNumber ?? -1}, Property: {name}, require: {length} got: {arr.Count()}");

            return arr.Select(quant => new Quaternion(quant[0], quant[1], quant[2], quant[3])).ToArray();
        }


        #endregion
    }
}
