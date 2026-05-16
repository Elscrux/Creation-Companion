using System.Xml.Linq;
namespace HKX2
{
    // hkpTriSampledHeightFieldBvTreeShape Signatire: 0x58e1e585 size: 80 flags: FLAGS_NONE

    // m_childContainer m_class: hkpSingleShapeContainer Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_childSize m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 56 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_wantAabbRejectionTest m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 60 flags: FLAGS_NONE enum: 
    // m_padding m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 12 offset: 61 flags: FLAGS_NONE enum: 
    public partial class hkpTriSampledHeightFieldBvTreeShape : hkpBvTreeShape, IEquatable<hkpTriSampledHeightFieldBvTreeShape?>
    {
        public hkpSingleShapeContainer m_childContainer { set; get; } = new();
        private int m_childSize { set; get; }
        public bool m_wantAabbRejectionTest { set; get; }
        public byte[] m_padding = new byte[12];

        public override uint Signature { set; get; } = 0x58e1e585;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_childContainer.Read(des, br);
            m_childSize = br.ReadInt32();
            m_wantAabbRejectionTest = br.ReadBoolean();
            m_padding = des.ReadByteCStyleArray(br, 12);
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_childContainer.Write(s, bw);
            bw.WriteInt32(m_childSize);
            bw.WriteBoolean(m_wantAabbRejectionTest);
            s.WriteByteCStyleArray(bw, m_padding);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_childContainer = xd.ReadClass<hkpSingleShapeContainer>(xe, nameof(m_childContainer));
            m_wantAabbRejectionTest = xd.ReadBoolean(xe, nameof(m_wantAabbRejectionTest));
            m_padding = xd.ReadByteCStyleArray(xe, nameof(m_padding), 12);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkpSingleShapeContainer>(xe, nameof(m_childContainer), m_childContainer);
            xs.WriteSerializeIgnored(xe, nameof(m_childSize));
            xs.WriteBoolean(xe, nameof(m_wantAabbRejectionTest), m_wantAabbRejectionTest);
            xs.WriteNumberArray(xe, nameof(m_padding), m_padding);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpTriSampledHeightFieldBvTreeShape);
        }

        public bool Equals(hkpTriSampledHeightFieldBvTreeShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_childContainer is null && other.m_childContainer is null) || (m_childContainer is not null && other.m_childContainer is not null && m_childContainer.Equals((IHavokObject)other.m_childContainer))) &&
                   m_wantAabbRejectionTest.Equals(other.m_wantAabbRejectionTest) &&
                   m_padding.SequenceEqual(other.m_padding) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_childContainer);
            hashcode.Add(m_wantAabbRejectionTest);
            hashcode.Add(m_padding.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

