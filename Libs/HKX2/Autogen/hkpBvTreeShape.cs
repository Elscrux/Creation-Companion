using System.Xml.Linq;
namespace HKX2
{
    // hkpBvTreeShape Signatire: 0xa823d623 size: 40 flags: FLAGS_NONE

    // m_bvTreeType m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: BvTreeType
    public partial class hkpBvTreeShape : hkpShape, IEquatable<hkpBvTreeShape?>
    {
        public byte m_bvTreeType { set; get; }

        public override uint Signature { set; get; } = 0xa823d623;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bvTreeType = br.ReadByte();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_bvTreeType);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_bvTreeType = xd.ReadFlag<BvTreeType, byte>(xe, nameof(m_bvTreeType));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteEnum<BvTreeType, byte>(xe, nameof(m_bvTreeType), m_bvTreeType);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpBvTreeShape);
        }

        public bool Equals(hkpBvTreeShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_bvTreeType.Equals(other.m_bvTreeType) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_bvTreeType);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

