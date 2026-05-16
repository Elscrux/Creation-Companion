using System.Xml.Linq;
namespace HKX2
{
    // hkbNamedIntEventPayload Signatire: 0x3c99bda4 size: 32 flags: FLAGS_NONE

    // m_data m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    public partial class hkbNamedIntEventPayload : hkbNamedEventPayload, IEquatable<hkbNamedIntEventPayload?>
    {
        public int m_data { set; get; }

        public override uint Signature { set; get; } = 0x3c99bda4;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_data = br.ReadInt32();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_data);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_data = xd.ReadInt32(xe, nameof(m_data));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_data), m_data);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbNamedIntEventPayload);
        }

        public bool Equals(hkbNamedIntEventPayload? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_data.Equals(other.m_data) &&
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

