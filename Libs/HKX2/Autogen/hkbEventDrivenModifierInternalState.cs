using System.Xml.Linq;
namespace HKX2
{
    // hkbEventDrivenModifierInternalState Signatire: 0xd14bf000 size: 24 flags: FLAGS_NONE

    // m_isActive m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbEventDrivenModifierInternalState : hkReferencedObject, IEquatable<hkbEventDrivenModifierInternalState?>
    {
        public bool m_isActive { set; get; }

        public override uint Signature { set; get; } = 0xd14bf000;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isActive = br.ReadBoolean();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteBoolean(m_isActive);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_isActive = xd.ReadBoolean(xe, nameof(m_isActive));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteBoolean(xe, nameof(m_isActive), m_isActive);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbEventDrivenModifierInternalState);
        }

        public bool Equals(hkbEventDrivenModifierInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_isActive.Equals(other.m_isActive) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_isActive);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

