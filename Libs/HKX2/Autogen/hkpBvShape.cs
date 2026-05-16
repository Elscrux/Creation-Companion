using System.Xml.Linq;
namespace HKX2
{
    // hkpBvShape Signatire: 0x286eb64c size: 56 flags: FLAGS_NONE

    // m_boundingVolumeShape m_class: hkpShape Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_childShape m_class: hkpSingleShapeContainer Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    public partial class hkpBvShape : hkpShape, IEquatable<hkpBvShape?>
    {
        public hkpShape? m_boundingVolumeShape { set; get; }
        public hkpSingleShapeContainer m_childShape { set; get; } = new();

        public override uint Signature { set; get; } = 0x286eb64c;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_boundingVolumeShape = des.ReadClassPointer<hkpShape>(br);
            m_childShape.Read(des, br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_boundingVolumeShape);
            m_childShape.Write(s, bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_boundingVolumeShape = xd.ReadClassPointer<hkpShape>(xe, nameof(m_boundingVolumeShape));
            m_childShape = xd.ReadClass<hkpSingleShapeContainer>(xe, nameof(m_childShape));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_boundingVolumeShape), m_boundingVolumeShape);
            xs.WriteClass<hkpSingleShapeContainer>(xe, nameof(m_childShape), m_childShape);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpBvShape);
        }

        public bool Equals(hkpBvShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_boundingVolumeShape is null && other.m_boundingVolumeShape is null) || (m_boundingVolumeShape is not null && other.m_boundingVolumeShape is not null && m_boundingVolumeShape.Equals((IHavokObject)other.m_boundingVolumeShape))) &&
                   ((m_childShape is null && other.m_childShape is null) || (m_childShape is not null && other.m_childShape is not null && m_childShape.Equals((IHavokObject)other.m_childShape))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_boundingVolumeShape);
            hashcode.Add(m_childShape);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

