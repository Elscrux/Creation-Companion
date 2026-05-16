using System.Xml.Linq;
namespace HKX2
{
    // hkbStateMachineProspectiveTransitionInfo Signatire: 0x3ab09a2e size: 16 flags: FLAGS_NONE

    // m_transitionInfoReference m_class: hkbStateMachineTransitionInfoReference Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_transitionInfoReferenceForTE m_class: hkbStateMachineTransitionInfoReference Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 6 flags: FLAGS_NONE enum: 
    // m_toStateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    public partial class hkbStateMachineProspectiveTransitionInfo : IHavokObject, IEquatable<hkbStateMachineProspectiveTransitionInfo?>
    {
        public hkbStateMachineTransitionInfoReference m_transitionInfoReference { set; get; } = new();
        public hkbStateMachineTransitionInfoReference m_transitionInfoReferenceForTE { set; get; } = new();
        public int m_toStateId { set; get; }

        public virtual uint Signature { set; get; } = 0x3ab09a2e;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transitionInfoReference.Read(des, br);
            m_transitionInfoReferenceForTE.Read(des, br);
            m_toStateId = br.ReadInt32();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_transitionInfoReference.Write(s, bw);
            m_transitionInfoReferenceForTE.Write(s, bw);
            bw.WriteInt32(m_toStateId);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_transitionInfoReference = xd.ReadClass<hkbStateMachineTransitionInfoReference>(xe, nameof(m_transitionInfoReference));
            m_transitionInfoReferenceForTE = xd.ReadClass<hkbStateMachineTransitionInfoReference>(xe, nameof(m_transitionInfoReferenceForTE));
            m_toStateId = xd.ReadInt32(xe, nameof(m_toStateId));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkbStateMachineTransitionInfoReference>(xe, nameof(m_transitionInfoReference), m_transitionInfoReference);
            xs.WriteClass(xe, nameof(m_transitionInfoReferenceForTE), m_transitionInfoReferenceForTE);
            xs.WriteNumber(xe, nameof(m_toStateId), m_toStateId);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbStateMachineProspectiveTransitionInfo);
        }

        public bool Equals(hkbStateMachineProspectiveTransitionInfo? other)
        {
            return other is not null &&
                   ((m_transitionInfoReference is null && other.m_transitionInfoReference is null) || (m_transitionInfoReference is not null && other.m_transitionInfoReference is not null && m_transitionInfoReference.Equals((IHavokObject)other.m_transitionInfoReference))) &&
                   ((m_transitionInfoReferenceForTE is null && other.m_transitionInfoReferenceForTE is null) || (m_transitionInfoReferenceForTE is not null && other.m_transitionInfoReferenceForTE is not null && m_transitionInfoReferenceForTE.Equals((IHavokObject)other.m_transitionInfoReferenceForTE))) &&
                   m_toStateId.Equals(other.m_toStateId) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_transitionInfoReference);
            hashcode.Add(m_transitionInfoReferenceForTE);
            hashcode.Add(m_toStateId);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

