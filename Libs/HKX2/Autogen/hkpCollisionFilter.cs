using System.Xml.Linq;
namespace HKX2
{
    // hkpCollisionFilter Signatire: 0x60960336 size: 72 flags: FLAGS_NONE

    // m_prepad m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 2 offset: 48 flags: FLAGS_NONE enum: 
    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_UINT32 arrSize: 0 offset: 56 flags: FLAGS_NONE enum: hkpFilterType
    // m_postpad m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 3 offset: 60 flags: FLAGS_NONE enum: 
    public partial class hkpCollisionFilter : hkReferencedObject, IEquatable<hkpCollisionFilter?>
    {
        public uint[] m_prepad = new uint[2];
        public uint m_type { set; get; }
        public uint[] m_postpad = new uint[3];

        public override uint Signature { set; get; } = 0x60960336;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 32;
            m_prepad = des.ReadUInt32CStyleArray(br, 2);
            m_type = br.ReadUInt32();
            m_postpad = des.ReadUInt32CStyleArray(br, 3);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 32;
            s.WriteUInt32CStyleArray(bw, m_prepad);
            bw.WriteUInt32(m_type);
            s.WriteUInt32CStyleArray(bw, m_postpad);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_prepad = xd.ReadUInt32CStyleArray(xe, nameof(m_prepad), 2);
            m_type = xd.ReadFlag<hkpFilterType, uint>(xe, nameof(m_type));
            m_postpad = xd.ReadUInt32CStyleArray(xe, nameof(m_postpad), 3);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumberArray(xe, nameof(m_prepad), m_prepad);
            xs.WriteEnum<hkpFilterType, uint>(xe, nameof(m_type), m_type);
            xs.WriteNumberArray(xe, nameof(m_postpad), m_postpad);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCollisionFilter);
        }

        public bool Equals(hkpCollisionFilter? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_prepad.SequenceEqual(other.m_prepad) &&
                   m_type.Equals(other.m_type) &&
                   m_postpad.SequenceEqual(other.m_postpad) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_prepad.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_type);
            hashcode.Add(m_postpad.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

