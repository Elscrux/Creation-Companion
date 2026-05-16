using System.Xml.Linq;
namespace HKX2
{
    // hkLocalFrameGroup Signatire: 0xb1a96c2f size: 24 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkLocalFrameGroup : hkReferencedObject, IEquatable<hkLocalFrameGroup?>
    {
        public string m_name { set; get; } = "";

        public override uint Signature { set; get; } = 0xb1a96c2f;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_name);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_name = xd.ReadString(xe, nameof(m_name));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_name), m_name);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkLocalFrameGroup);
        }

        public bool Equals(hkLocalFrameGroup? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_name == other.m_name &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_name);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

