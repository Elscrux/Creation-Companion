using System.Xml.Linq;
namespace HKX2
{
    // hkAabbUint32 Signatire: 0x11e7c11 size: 32 flags: FLAGS_NONE

    // m_min m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 3 offset: 0 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_expansionMin m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 3 offset: 12 flags: FLAGS_NONE enum: 
    // m_expansionShift m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 15 flags: FLAGS_NONE enum: 
    // m_max m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 3 offset: 16 flags: FLAGS_NONE enum: 
    // m_expansionMax m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 3 offset: 28 flags: FLAGS_NONE enum: 
    // m_shapeKeyByte m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 31 flags: FLAGS_NONE enum: 
    public partial class hkAabbUint32 : IHavokObject, IEquatable<hkAabbUint32?>
    {
        public uint[] m_min = new uint[3];
        public byte[] m_expansionMin = new byte[3];
        public byte m_expansionShift { set; get; }
        public uint[] m_max = new uint[3];
        public byte[] m_expansionMax = new byte[3];
        public byte m_shapeKeyByte { set; get; }

        public virtual uint Signature { set; get; } = 0x11e7c11;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_min = des.ReadUInt32CStyleArray(br, 3);
            m_expansionMin = des.ReadByteCStyleArray(br, 3);
            m_expansionShift = br.ReadByte();
            m_max = des.ReadUInt32CStyleArray(br, 3);
            m_expansionMax = des.ReadByteCStyleArray(br, 3);
            m_shapeKeyByte = br.ReadByte();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteUInt32CStyleArray(bw, m_min);
            s.WriteByteCStyleArray(bw, m_expansionMin);
            bw.WriteByte(m_expansionShift);
            s.WriteUInt32CStyleArray(bw, m_max);
            s.WriteByteCStyleArray(bw, m_expansionMax);
            bw.WriteByte(m_shapeKeyByte);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_min = xd.ReadUInt32CStyleArray(xe, nameof(m_min), 3);
            m_expansionMin = xd.ReadByteCStyleArray(xe, nameof(m_expansionMin), 3);
            m_expansionShift = xd.ReadByte(xe, nameof(m_expansionShift));
            m_max = xd.ReadUInt32CStyleArray(xe, nameof(m_max), 3);
            m_expansionMax = xd.ReadByteCStyleArray(xe, nameof(m_expansionMax), 3);
            m_shapeKeyByte = xd.ReadByte(xe, nameof(m_shapeKeyByte));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumberArray(xe, nameof(m_min), m_min);
            xs.WriteNumberArray(xe, nameof(m_expansionMin), m_expansionMin);
            xs.WriteNumber(xe, nameof(m_expansionShift), m_expansionShift);
            xs.WriteNumberArray(xe, nameof(m_max), m_max);
            xs.WriteNumberArray(xe, nameof(m_expansionMax), m_expansionMax);
            xs.WriteNumber(xe, nameof(m_shapeKeyByte), m_shapeKeyByte);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkAabbUint32);
        }

        public bool Equals(hkAabbUint32? other)
        {
            return other is not null &&
                   m_min.SequenceEqual(other.m_min) &&
                   m_expansionMin.SequenceEqual(other.m_expansionMin) &&
                   m_expansionShift.Equals(other.m_expansionShift) &&
                   m_max.SequenceEqual(other.m_max) &&
                   m_expansionMax.SequenceEqual(other.m_expansionMax) &&
                   m_shapeKeyByte.Equals(other.m_shapeKeyByte) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_min.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_expansionMin.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_expansionShift);
            hashcode.Add(m_max.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_expansionMax.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_shapeKeyByte);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

