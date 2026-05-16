using System.Xml.Linq;
namespace HKX2
{
    // hkbModifierWrapper Signatire: 0x3697e044 size: 88 flags: FLAGS_NONE

    // m_modifier m_class: hkbModifier Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkbModifierWrapper : hkbModifier, IEquatable<hkbModifierWrapper?>
    {
        public hkbModifier? m_modifier { set; get; }

        public override uint Signature { set; get; } = 0x3697e044;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_modifier = des.ReadClassPointer<hkbModifier>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_modifier);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_modifier = xd.ReadClassPointer<hkbModifier>(xe, nameof(m_modifier));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_modifier), m_modifier);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbModifierWrapper);
        }

        public bool Equals(hkbModifierWrapper? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_modifier is null && other.m_modifier is null) || (m_modifier is not null && other.m_modifier is not null && m_modifier.Equals((IHavokObject)other.m_modifier))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_modifier);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

