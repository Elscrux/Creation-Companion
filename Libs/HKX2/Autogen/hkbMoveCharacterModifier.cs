using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbMoveCharacterModifier Signatire: 0x8f7492a0 size: 112 flags: FLAGS_NONE

    // m_offsetPerSecondMS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_timeSinceLastModify m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 96 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbMoveCharacterModifier : hkbModifier, IEquatable<hkbMoveCharacterModifier?>
    {
        public Vector4 m_offsetPerSecondMS { set; get; }
        private float m_timeSinceLastModify { set; get; }

        public override uint Signature { set; get; } = 0x8f7492a0;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_offsetPerSecondMS = br.ReadVector4();
            m_timeSinceLastModify = br.ReadSingle();
            br.Position += 12;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_offsetPerSecondMS);
            bw.WriteSingle(m_timeSinceLastModify);
            bw.Position += 12;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_offsetPerSecondMS = xd.ReadVector4(xe, nameof(m_offsetPerSecondMS));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_offsetPerSecondMS), m_offsetPerSecondMS);
            xs.WriteSerializeIgnored(xe, nameof(m_timeSinceLastModify));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbMoveCharacterModifier);
        }

        public bool Equals(hkbMoveCharacterModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_offsetPerSecondMS.Equals(other.m_offsetPerSecondMS) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_offsetPerSecondMS);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

