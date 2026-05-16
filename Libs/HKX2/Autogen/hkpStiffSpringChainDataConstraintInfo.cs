using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpStiffSpringChainDataConstraintInfo Signatire: 0xc624a180 size: 48 flags: FLAGS_NONE

    // m_pivotInA m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_pivotInB m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_springLength m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkpStiffSpringChainDataConstraintInfo : IHavokObject, IEquatable<hkpStiffSpringChainDataConstraintInfo?>
    {
        public Vector4 m_pivotInA { set; get; }
        public Vector4 m_pivotInB { set; get; }
        public float m_springLength { set; get; }

        public virtual uint Signature { set; get; } = 0xc624a180;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pivotInA = br.ReadVector4();
            m_pivotInB = br.ReadVector4();
            m_springLength = br.ReadSingle();
            br.Position += 12;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_pivotInA);
            bw.WriteVector4(m_pivotInB);
            bw.WriteSingle(m_springLength);
            bw.Position += 12;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_pivotInA = xd.ReadVector4(xe, nameof(m_pivotInA));
            m_pivotInB = xd.ReadVector4(xe, nameof(m_pivotInB));
            m_springLength = xd.ReadSingle(xe, nameof(m_springLength));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_pivotInA), m_pivotInA);
            xs.WriteVector4(xe, nameof(m_pivotInB), m_pivotInB);
            xs.WriteFloat(xe, nameof(m_springLength), m_springLength);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpStiffSpringChainDataConstraintInfo);
        }

        public bool Equals(hkpStiffSpringChainDataConstraintInfo? other)
        {
            return other is not null &&
                   m_pivotInA.Equals(other.m_pivotInA) &&
                   m_pivotInB.Equals(other.m_pivotInB) &&
                   m_springLength.Equals(other.m_springLength) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_pivotInA);
            hashcode.Add(m_pivotInB);
            hashcode.Add(m_springLength);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

