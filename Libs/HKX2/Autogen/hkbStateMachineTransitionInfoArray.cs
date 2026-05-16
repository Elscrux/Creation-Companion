using System.Xml.Linq;
namespace HKX2
{
    // hkbStateMachineTransitionInfoArray Signatire: 0xe397b11e size: 32 flags: FLAGS_NONE

    // m_transitions m_class: hkbStateMachineTransitionInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbStateMachineTransitionInfoArray : hkReferencedObject, IEquatable<hkbStateMachineTransitionInfoArray?>
    {
        public IList<hkbStateMachineTransitionInfo> m_transitions { set; get; } = Array.Empty<hkbStateMachineTransitionInfo>();

        public override uint Signature { set; get; } = 0xe397b11e;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transitions = des.ReadClassArray<hkbStateMachineTransitionInfo>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_transitions);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_transitions = xd.ReadClassArray<hkbStateMachineTransitionInfo>(xe, nameof(m_transitions));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_transitions), m_transitions);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbStateMachineTransitionInfoArray);
        }

        public bool Equals(hkbStateMachineTransitionInfoArray? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_transitions.SequenceEqual(other.m_transitions) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_transitions.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

