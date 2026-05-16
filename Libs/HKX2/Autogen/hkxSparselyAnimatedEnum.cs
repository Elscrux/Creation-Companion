using System.Xml.Linq;
namespace HKX2
{
    // hkxSparselyAnimatedEnum Signatire: 0x68a47b64 size: 56 flags: FLAGS_NONE

    // m_enum m_class: hkxEnum Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkxSparselyAnimatedEnum : hkxSparselyAnimatedInt, IEquatable<hkxSparselyAnimatedEnum?>
    {
        public hkxEnum? m_enum { set; get; }

        public override uint Signature { set; get; } = 0x68a47b64;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_enum = des.ReadClassPointer<hkxEnum>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_enum);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_enum = xd.ReadClassPointer<hkxEnum>(xe, nameof(m_enum));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_enum), m_enum);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxSparselyAnimatedEnum);
        }

        public bool Equals(hkxSparselyAnimatedEnum? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_enum is null && other.m_enum is null) || (m_enum is not null && other.m_enum is not null && m_enum.Equals((IHavokObject)other.m_enum))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_enum);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

