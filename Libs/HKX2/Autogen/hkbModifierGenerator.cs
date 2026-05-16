using System.Xml.Linq;
namespace HKX2
{
    // hkbModifierGenerator Signatire: 0x1f81fae6 size: 88 flags: FLAGS_NONE

    // m_modifier m_class: hkbModifier Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_generator m_class: hkbGenerator Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkbModifierGenerator : hkbGenerator, IEquatable<hkbModifierGenerator?>
    {
        public hkbModifier? m_modifier { set; get; }
        public hkbGenerator? m_generator { set; get; }

        public override uint Signature { set; get; } = 0x1f81fae6;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_modifier = des.ReadClassPointer<hkbModifier>(br);
            m_generator = des.ReadClassPointer<hkbGenerator>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_modifier);
            s.WriteClassPointer(bw, m_generator);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_modifier = xd.ReadClassPointer<hkbModifier>(xe, nameof(m_modifier));
            m_generator = xd.ReadClassPointer<hkbGenerator>(xe, nameof(m_generator));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_modifier), m_modifier);
            xs.WriteClassPointer(xe, nameof(m_generator), m_generator);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbModifierGenerator);
        }

        public bool Equals(hkbModifierGenerator? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_modifier is null && other.m_modifier is null) || (m_modifier is not null && other.m_modifier is not null && m_modifier.Equals((IHavokObject)other.m_modifier))) &&
                   ((m_generator is null && other.m_generator is null) || (m_generator is not null && other.m_generator is not null && m_generator.Equals((IHavokObject)other.m_generator))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_modifier);
            hashcode.Add(m_generator);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

