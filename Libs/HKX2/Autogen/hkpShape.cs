using System.Xml.Linq;
namespace HKX2
{
    // hkpShape Signatire: 0x666490a1 size: 32 flags: FLAGS_NONE

    // m_userData m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_UINT32 arrSize: 0 offset: 24 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpShape : hkReferencedObject, IEquatable<hkpShape?>
    {
        public ulong m_userData { set; get; }
        private uint m_type { set; get; }

        public override uint Signature { set; get; } = 0x666490a1;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_userData = br.ReadUInt64();
            m_type = br.ReadUInt32();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt64(m_userData);
            bw.WriteUInt32(m_type);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_userData = xd.ReadUInt64(xe, nameof(m_userData));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_userData), m_userData);
            xs.WriteSerializeIgnored(xe, nameof(m_type));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpShape);
        }

        public bool Equals(hkpShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_userData.Equals(other.m_userData) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_userData);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

