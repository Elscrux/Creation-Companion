using System.Xml.Linq;
namespace HKX2
{
    // hkpCollidableBoundingVolumeData Signatire: 0xb5f0e6b1 size: 56 flags: FLAGS_NONE

    // m_min m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 3 offset: 0 flags: FLAGS_NONE enum: 
    // m_expansionMin m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 3 offset: 12 flags: FLAGS_NONE enum: 
    // m_expansionShift m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 15 flags: FLAGS_NONE enum: 
    // m_max m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 3 offset: 16 flags: FLAGS_NONE enum: 
    // m_expansionMax m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 3 offset: 28 flags: FLAGS_NONE enum: 
    // m_padding m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 31 flags: FLAGS_NONE enum: 
    // m_numChildShapeAabbs m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 32 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_capacityChildShapeAabbs m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 34 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_childShapeAabbs m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 40 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_childShapeKeys m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 48 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpCollidableBoundingVolumeData : IHavokObject, IEquatable<hkpCollidableBoundingVolumeData?>
    {
        public uint[] m_min = new uint[3];
        public byte[] m_expansionMin = new byte[3];
        public byte m_expansionShift { set; get; }
        public uint[] m_max = new uint[3];
        public byte[] m_expansionMax = new byte[3];
        public byte m_padding { set; get; }
        private ushort m_numChildShapeAabbs { set; get; }
        private ushort m_capacityChildShapeAabbs { set; get; }
        private object? m_childShapeAabbs { set; get; }
        private object? m_childShapeKeys { set; get; }

        public virtual uint Signature { set; get; } = 0xb5f0e6b1;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_min = des.ReadUInt32CStyleArray(br, 3);
            m_expansionMin = des.ReadByteCStyleArray(br, 3);
            m_expansionShift = br.ReadByte();
            m_max = des.ReadUInt32CStyleArray(br, 3);
            m_expansionMax = des.ReadByteCStyleArray(br, 3);
            m_padding = br.ReadByte();
            m_numChildShapeAabbs = br.ReadUInt16();
            m_capacityChildShapeAabbs = br.ReadUInt16();
            br.Position += 4;
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteUInt32CStyleArray(bw, m_min);
            s.WriteByteCStyleArray(bw, m_expansionMin);
            bw.WriteByte(m_expansionShift);
            s.WriteUInt32CStyleArray(bw, m_max);
            s.WriteByteCStyleArray(bw, m_expansionMax);
            bw.WriteByte(m_padding);
            bw.WriteUInt16(m_numChildShapeAabbs);
            bw.WriteUInt16(m_capacityChildShapeAabbs);
            bw.Position += 4;
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_min = xd.ReadUInt32CStyleArray(xe, nameof(m_min), 3);
            m_expansionMin = xd.ReadByteCStyleArray(xe, nameof(m_expansionMin), 3);
            m_expansionShift = xd.ReadByte(xe, nameof(m_expansionShift));
            m_max = xd.ReadUInt32CStyleArray(xe, nameof(m_max), 3);
            m_expansionMax = xd.ReadByteCStyleArray(xe, nameof(m_expansionMax), 3);
            m_padding = xd.ReadByte(xe, nameof(m_padding));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumberArray(xe, nameof(m_min), m_min);
            xs.WriteNumberArray(xe, nameof(m_expansionMin), m_expansionMin);
            xs.WriteNumber(xe, nameof(m_expansionShift), m_expansionShift);
            xs.WriteNumberArray(xe, nameof(m_max), m_max);
            xs.WriteNumberArray(xe, nameof(m_expansionMax), m_expansionMax);
            xs.WriteNumber(xe, nameof(m_padding), m_padding);
            xs.WriteSerializeIgnored(xe, nameof(m_numChildShapeAabbs));
            xs.WriteSerializeIgnored(xe, nameof(m_capacityChildShapeAabbs));
            xs.WriteSerializeIgnored(xe, nameof(m_childShapeAabbs));
            xs.WriteSerializeIgnored(xe, nameof(m_childShapeKeys));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCollidableBoundingVolumeData);
        }

        public bool Equals(hkpCollidableBoundingVolumeData? other)
        {
            return other is not null &&
                   m_min.SequenceEqual(other.m_min) &&
                   m_expansionMin.SequenceEqual(other.m_expansionMin) &&
                   m_expansionShift.Equals(other.m_expansionShift) &&
                   m_max.SequenceEqual(other.m_max) &&
                   m_expansionMax.SequenceEqual(other.m_expansionMax) &&
                   m_padding.Equals(other.m_padding) &&
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
            hashcode.Add(m_padding);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

