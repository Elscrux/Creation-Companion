using System.Xml.Linq;
namespace HKX2
{
    // hkpConstraintChainInstance Signatire: 0x7a490753 size: 136 flags: FLAGS_NONE

    // m_chainedEntities m_class: hkpEntity Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_action m_class: hkpConstraintChainInstanceAction Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    public partial class hkpConstraintChainInstance : hkpConstraintInstance, IEquatable<hkpConstraintChainInstance?>
    {
        public IList<hkpEntity> m_chainedEntities { set; get; } = Array.Empty<hkpEntity>();
        public hkpConstraintChainInstanceAction? m_action { set; get; }

        public override uint Signature { set; get; } = 0x7a490753;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_chainedEntities = des.ReadClassPointerArray<hkpEntity>(br);
            m_action = des.ReadClassPointer<hkpConstraintChainInstanceAction>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_chainedEntities);
            s.WriteClassPointer(bw, m_action);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_chainedEntities = xd.ReadClassPointerArray<hkpEntity>(xe, nameof(m_chainedEntities));
            m_action = xd.ReadClassPointer<hkpConstraintChainInstanceAction>(xe, nameof(m_action));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_chainedEntities), m_chainedEntities);
            xs.WriteClassPointer(xe, nameof(m_action), m_action);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConstraintChainInstance);
        }

        public bool Equals(hkpConstraintChainInstance? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_chainedEntities.SequenceEqual(other.m_chainedEntities) &&
                   ((m_action is null && other.m_action is null) || (m_action is not null && other.m_action is not null && m_action.Equals((IHavokObject)other.m_action))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_chainedEntities.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_action);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

