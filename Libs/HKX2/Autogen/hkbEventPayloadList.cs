using System.Xml.Linq;
namespace HKX2
{
    // hkbEventPayloadList Signatire: 0x3d2dbd34 size: 32 flags: FLAGS_NONE

    // m_payloads m_class: hkbEventPayload Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbEventPayloadList : hkbEventPayload, IEquatable<hkbEventPayloadList?>
    {
        public IList<hkbEventPayload> m_payloads { set; get; } = Array.Empty<hkbEventPayload>();

        public override uint Signature { set; get; } = 0x3d2dbd34;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_payloads = des.ReadClassPointerArray<hkbEventPayload>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_payloads);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_payloads = xd.ReadClassPointerArray<hkbEventPayload>(xe, nameof(m_payloads));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_payloads), m_payloads);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbEventPayloadList);
        }

        public bool Equals(hkbEventPayloadList? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_payloads.SequenceEqual(other.m_payloads) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_payloads.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

