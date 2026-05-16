using System.Xml.Linq;
namespace HKX2
{
    // hkbStateMachineTransitionInfoReference Signatire: 0x9810c2d0 size: 6 flags: FLAGS_NONE

    // m_fromStateIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_transitionIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_stateMachineId m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    public partial class hkbStateMachineTransitionInfoReference : IHavokObject, IEquatable<hkbStateMachineTransitionInfoReference?>
    {
        public short m_fromStateIndex { set; get; }
        public short m_transitionIndex { set; get; }
        public short m_stateMachineId { set; get; }

        public virtual uint Signature { set; get; } = 0x9810c2d0;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_fromStateIndex = br.ReadInt16();
            m_transitionIndex = br.ReadInt16();
            m_stateMachineId = br.ReadInt16();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt16(m_fromStateIndex);
            bw.WriteInt16(m_transitionIndex);
            bw.WriteInt16(m_stateMachineId);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_fromStateIndex = xd.ReadInt16(xe, nameof(m_fromStateIndex));
            m_transitionIndex = xd.ReadInt16(xe, nameof(m_transitionIndex));
            m_stateMachineId = xd.ReadInt16(xe, nameof(m_stateMachineId));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_fromStateIndex), m_fromStateIndex);
            xs.WriteNumber(xe, nameof(m_transitionIndex), m_transitionIndex);
            xs.WriteNumber(xe, nameof(m_stateMachineId), m_stateMachineId);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbStateMachineTransitionInfoReference);
        }

        public bool Equals(hkbStateMachineTransitionInfoReference? other)
        {
            return other is not null &&
                   m_fromStateIndex.Equals(other.m_fromStateIndex) &&
                   m_transitionIndex.Equals(other.m_transitionIndex) &&
                   m_stateMachineId.Equals(other.m_stateMachineId) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_fromStateIndex);
            hashcode.Add(m_transitionIndex);
            hashcode.Add(m_stateMachineId);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

