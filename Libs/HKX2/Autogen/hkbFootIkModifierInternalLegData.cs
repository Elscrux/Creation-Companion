using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbFootIkModifierInternalLegData Signatire: 0xe5ca3677 size: 32 flags: FLAGS_NONE

    // m_groundPosition m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_footIkSolver m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 16 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbFootIkModifierInternalLegData : IHavokObject, IEquatable<hkbFootIkModifierInternalLegData?>
    {
        public Vector4 m_groundPosition { set; get; }
        private object? m_footIkSolver { set; get; }

        public virtual uint Signature { set; get; } = 0xe5ca3677;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_groundPosition = br.ReadVector4();
            des.ReadEmptyPointer(br);
            br.Position += 8;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_groundPosition);
            s.WriteVoidPointer(bw);
            bw.Position += 8;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_groundPosition = xd.ReadVector4(xe, nameof(m_groundPosition));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_groundPosition), m_groundPosition);
            xs.WriteSerializeIgnored(xe, nameof(m_footIkSolver));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbFootIkModifierInternalLegData);
        }

        public bool Equals(hkbFootIkModifierInternalLegData? other)
        {
            return other is not null &&
                   m_groundPosition.Equals(other.m_groundPosition) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_groundPosition);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

