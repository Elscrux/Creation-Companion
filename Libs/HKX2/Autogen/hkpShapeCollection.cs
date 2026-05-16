using System.Xml.Linq;
namespace HKX2
{
    // hkpShapeCollection Signatire: 0xe8c3991d size: 48 flags: FLAGS_NONE

    // m_disableWelding m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_collectionType m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 41 flags: FLAGS_NONE enum: CollectionType
    public partial class hkpShapeCollection : hkpShape, IEquatable<hkpShapeCollection?>
    {
        public bool m_disableWelding { set; get; }
        public byte m_collectionType { set; get; }

        public override uint Signature { set; get; } = 0xe8c3991d;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_disableWelding = br.ReadBoolean();
            m_collectionType = br.ReadByte();
            br.Position += 6;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            bw.WriteBoolean(m_disableWelding);
            bw.WriteByte(m_collectionType);
            bw.Position += 6;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_disableWelding = xd.ReadBoolean(xe, nameof(m_disableWelding));
            m_collectionType = xd.ReadFlag<CollectionType, byte>(xe, nameof(m_collectionType));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteBoolean(xe, nameof(m_disableWelding), m_disableWelding);
            xs.WriteEnum<CollectionType, byte>(xe, nameof(m_collectionType), m_collectionType);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpShapeCollection);
        }

        public bool Equals(hkpShapeCollection? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_disableWelding.Equals(other.m_disableWelding) &&
                   m_collectionType.Equals(other.m_collectionType) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_disableWelding);
            hashcode.Add(m_collectionType);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

