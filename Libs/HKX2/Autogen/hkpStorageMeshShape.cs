using System.Xml.Linq;
namespace HKX2
{
    // hkpStorageMeshShape Signatire: 0xbefd8b39 size: 144 flags: FLAGS_NONE

    // m_storage m_class: hkpStorageMeshShapeSubpartStorage Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    public partial class hkpStorageMeshShape : hkpMeshShape, IEquatable<hkpStorageMeshShape?>
    {
        public IList<hkpStorageMeshShapeSubpartStorage> m_storage { set; get; } = Array.Empty<hkpStorageMeshShapeSubpartStorage>();

        public override uint Signature { set; get; } = 0xbefd8b39;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_storage = des.ReadClassPointerArray<hkpStorageMeshShapeSubpartStorage>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_storage);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_storage = xd.ReadClassPointerArray<hkpStorageMeshShapeSubpartStorage>(xe, nameof(m_storage));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_storage), m_storage);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpStorageMeshShape);
        }

        public bool Equals(hkpStorageMeshShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_storage.SequenceEqual(other.m_storage) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_storage.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

