using System.Xml.Linq;
namespace HKX2
{
    // BSEventOnDeactivateModifier Signatire: 0x1062d993 size: 96 flags: FLAGS_NONE

    // m_event m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class BSEventOnDeactivateModifier : hkbModifier, IEquatable<BSEventOnDeactivateModifier?>
    {
        public hkbEventProperty m_event { set; get; } = new();

        public override uint Signature { set; get; } = 0x1062d993;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_event.Read(des, br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_event.Write(s, bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_event = xd.ReadClass<hkbEventProperty>(xe, nameof(m_event));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkbEventProperty>(xe, nameof(m_event), m_event);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSEventOnDeactivateModifier);
        }

        public bool Equals(BSEventOnDeactivateModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_event is null && other.m_event is null) || (m_event is not null && other.m_event is not null && m_event.Equals((IHavokObject)other.m_event))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_event);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

