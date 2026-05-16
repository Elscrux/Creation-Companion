using System.Xml.Linq;
namespace HKX2
{
    // hkpConstraintChainInstanceAction Signatire: 0xc3971189 size: 56 flags: FLAGS_NONE

    // m_constraintInstance m_class: hkpConstraintChainInstance Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: NOT_OWNED|FLAGS_NONE enum: 
    public partial class hkpConstraintChainInstanceAction : hkpAction, IEquatable<hkpConstraintChainInstanceAction?>
    {
        public hkpConstraintChainInstance? m_constraintInstance { set; get; }

        public override uint Signature { set; get; } = 0xc3971189;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_constraintInstance = des.ReadClassPointer<hkpConstraintChainInstance>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_constraintInstance);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_constraintInstance = xd.ReadClassPointer<hkpConstraintChainInstance>(xe, nameof(m_constraintInstance));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_constraintInstance), m_constraintInstance);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConstraintChainInstanceAction);
        }

        public bool Equals(hkpConstraintChainInstanceAction? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_constraintInstance is null && other.m_constraintInstance is null) || (m_constraintInstance is not null && other.m_constraintInstance is not null && m_constraintInstance.Equals((IHavokObject)other.m_constraintInstance))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_constraintInstance);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

