using System.Xml.Linq;
namespace HKX2
{
    // hkbRotateCharacterModifierInternalState Signatire: 0xdc40bf4a size: 24 flags: FLAGS_NONE

    // m_angle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbRotateCharacterModifierInternalState : hkReferencedObject, IEquatable<hkbRotateCharacterModifierInternalState?>
    {
        public float m_angle { set; get; }

        public override uint Signature { set; get; } = 0xdc40bf4a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_angle = br.ReadSingle();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_angle);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_angle = xd.ReadSingle(xe, nameof(m_angle));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_angle), m_angle);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbRotateCharacterModifierInternalState);
        }

        public bool Equals(hkbRotateCharacterModifierInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_angle.Equals(other.m_angle) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_angle);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

