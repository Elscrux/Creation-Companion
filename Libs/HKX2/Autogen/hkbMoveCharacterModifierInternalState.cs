using System.Xml.Linq;
namespace HKX2
{
    // hkbMoveCharacterModifierInternalState Signatire: 0x28f67ba0 size: 24 flags: FLAGS_NONE

    // m_timeSinceLastModify m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbMoveCharacterModifierInternalState : hkReferencedObject, IEquatable<hkbMoveCharacterModifierInternalState?>
    {
        public float m_timeSinceLastModify { set; get; }

        public override uint Signature { set; get; } = 0x28f67ba0;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_timeSinceLastModify = br.ReadSingle();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_timeSinceLastModify);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_timeSinceLastModify = xd.ReadSingle(xe, nameof(m_timeSinceLastModify));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_timeSinceLastModify), m_timeSinceLastModify);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbMoveCharacterModifierInternalState);
        }

        public bool Equals(hkbMoveCharacterModifierInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_timeSinceLastModify.Equals(other.m_timeSinceLastModify) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_timeSinceLastModify);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

