using System.Xml.Linq;
namespace HKX2
{
    // hkpConvexTransformShapeBase Signatire: 0xfbd72f9 size: 64 flags: FLAGS_NONE

    // m_childShape m_class: hkpSingleShapeContainer Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_childShapeSize m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 56 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpConvexTransformShapeBase : hkpConvexShape, IEquatable<hkpConvexTransformShapeBase?>
    {
        public hkpSingleShapeContainer m_childShape { set; get; } = new();
        private int m_childShapeSize { set; get; }

        public override uint Signature { set; get; } = 0xfbd72f9;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_childShape.Read(des, br);
            m_childShapeSize = br.ReadInt32();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_childShape.Write(s, bw);
            bw.WriteInt32(m_childShapeSize);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_childShape = xd.ReadClass<hkpSingleShapeContainer>(xe, nameof(m_childShape));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkpSingleShapeContainer>(xe, nameof(m_childShape), m_childShape);
            xs.WriteSerializeIgnored(xe, nameof(m_childShapeSize));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConvexTransformShapeBase);
        }

        public bool Equals(hkpConvexTransformShapeBase? other)
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

