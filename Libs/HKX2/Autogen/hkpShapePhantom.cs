using System.Xml.Linq;
namespace HKX2
{
    // hkpShapePhantom Signatire: 0xcb22fbcd size: 416 flags: FLAGS_NONE

    // m_motionState m_class: hkMotionState Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 240 flags: FLAGS_NONE enum: 
    public partial class hkpShapePhantom : hkpPhantom, IEquatable<hkpShapePhantom?>
    {
        public hkMotionState m_motionState { set; get; } = new();

        public override uint Signature { set; get; } = 0xcb22fbcd;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_motionState.Read(des, br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_motionState.Write(s, bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_motionState = xd.ReadClass<hkMotionState>(xe, nameof(m_motionState));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkMotionState>(xe, nameof(m_motionState), m_motionState);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpShapePhantom);
        }

        public bool Equals(hkpShapePhantom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_motionState is null && other.m_motionState is null) || (m_motionState is not null && other.m_motionState is not null && m_motionState.Equals((IHavokObject)other.m_motionState))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_motionState);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

