using System.Xml.Linq;
namespace HKX2
{
    // hkbNamedStringEventPayload Signatire: 0x6caa9113 size: 32 flags: FLAGS_NONE

    // m_data m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    public partial class hkbNamedStringEventPayload : hkbNamedEventPayload, IEquatable<hkbNamedStringEventPayload?>
    {
        public string m_data { set; get; } = "";

        public override uint Signature { set; get; } = 0x6caa9113;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_data = des.ReadStringPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_data);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_data = xd.ReadString(xe, nameof(m_data));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_data), m_data);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbNamedStringEventPayload);
        }

        public bool Equals(hkbNamedStringEventPayload? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_data == other.m_data &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_data);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

