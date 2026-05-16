using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbVariableValueSet Signatire: 0x27812d8d size: 64 flags: FLAGS_NONE

    // m_wordVariableValues m_class: hkbVariableValue Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_quadVariableValues m_class:  Type.TYPE_ARRAY Type.TYPE_VECTOR4 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_variantVariableValues m_class: hkReferencedObject Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkbVariableValueSet : hkReferencedObject, IEquatable<hkbVariableValueSet?>
    {
        public IList<hkbVariableValue> m_wordVariableValues { set; get; } = Array.Empty<hkbVariableValue>();
        public IList<Vector4> m_quadVariableValues { set; get; } = Array.Empty<Vector4>();
        public IList<hkReferencedObject> m_variantVariableValues { set; get; } = Array.Empty<hkReferencedObject>();

        public override uint Signature { set; get; } = 0x27812d8d;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wordVariableValues = des.ReadClassArray<hkbVariableValue>(br);
            m_quadVariableValues = des.ReadVector4Array(br);
            m_variantVariableValues = des.ReadClassPointerArray<hkReferencedObject>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_wordVariableValues);
            s.WriteVector4Array(bw, m_quadVariableValues);
            s.WriteClassPointerArray(bw, m_variantVariableValues);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_wordVariableValues = xd.ReadClassArray<hkbVariableValue>(xe, nameof(m_wordVariableValues));
            m_quadVariableValues = xd.ReadVector4Array(xe, nameof(m_quadVariableValues));
            m_variantVariableValues = xd.ReadClassPointerArray<hkReferencedObject>(xe, nameof(m_variantVariableValues));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_wordVariableValues), m_wordVariableValues);
            xs.WriteVector4Array(xe, nameof(m_quadVariableValues), m_quadVariableValues);
            xs.WriteClassPointerArray(xe, nameof(m_variantVariableValues), m_variantVariableValues);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbVariableValueSet);
        }

        public bool Equals(hkbVariableValueSet? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_wordVariableValues.SequenceEqual(other.m_wordVariableValues) &&
                   m_quadVariableValues.SequenceEqual(other.m_quadVariableValues) &&
                   m_variantVariableValues.SequenceEqual(other.m_variantVariableValues) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_wordVariableValues.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_quadVariableValues.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_variantVariableValues.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

