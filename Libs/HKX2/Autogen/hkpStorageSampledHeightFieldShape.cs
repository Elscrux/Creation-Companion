using System.Xml.Linq;
namespace HKX2
{
    // hkpStorageSampledHeightFieldShape Signatire: 0x15ff414b size: 144 flags: FLAGS_NONE

    // m_storage m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_triangleFlip m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    public partial class hkpStorageSampledHeightFieldShape : hkpSampledHeightFieldShape, IEquatable<hkpStorageSampledHeightFieldShape?>
    {
        public IList<float> m_storage { set; get; } = Array.Empty<float>();
        public bool m_triangleFlip { set; get; }

        public override uint Signature { set; get; } = 0x15ff414b;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_storage = des.ReadSingleArray(br);
            m_triangleFlip = br.ReadBoolean();
            br.Position += 15;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteSingleArray(bw, m_storage);
            bw.WriteBoolean(m_triangleFlip);
            bw.Position += 15;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_storage = xd.ReadSingleArray(xe, nameof(m_storage));
            m_triangleFlip = xd.ReadBoolean(xe, nameof(m_triangleFlip));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloatArray(xe, nameof(m_storage), m_storage);
            xs.WriteBoolean(xe, nameof(m_triangleFlip), m_triangleFlip);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpStorageSampledHeightFieldShape);
        }

        public bool Equals(hkpStorageSampledHeightFieldShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_storage.SequenceEqual(other.m_storage) &&
                   m_triangleFlip.Equals(other.m_triangleFlip) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_storage.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_triangleFlip);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

