using System.Xml.Linq;
namespace HKX2
{
    // hkpSingleShapeContainer Signatire: 0x73aa1d38 size: 16 flags: FLAGS_NONE

    // m_childShape m_class: hkpShape Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkpSingleShapeContainer : hkpShapeContainer, IEquatable<hkpSingleShapeContainer?>
    {
        public hkpShape? m_childShape { set; get; }

        public override uint Signature { set; get; } = 0x73aa1d38;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_childShape = des.ReadClassPointer<hkpShape>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_childShape);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_childShape = xd.ReadClassPointer<hkpShape>(xe, nameof(m_childShape));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_childShape), m_childShape);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSingleShapeContainer);
        }

        public bool Equals(hkpSingleShapeContainer? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_childShape is null && other.m_childShape is null) || (m_childShape is not null && other.m_childShape is not null && m_childShape.Equals((IHavokObject)other.m_childShape))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_childShape);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

