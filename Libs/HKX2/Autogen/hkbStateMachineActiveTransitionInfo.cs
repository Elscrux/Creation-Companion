using System.Xml.Linq;
namespace HKX2
{
    // hkbStateMachineActiveTransitionInfo Signatire: 0xbb90d54f size: 40 flags: FLAGS_NONE

    // m_transitionEffect m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 0 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_transitionEffectInternalStateInfo m_class: hkbNodeInternalStateInfo Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_transitionInfoReference m_class: hkbStateMachineTransitionInfoReference Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_transitionInfoReferenceForTE m_class: hkbStateMachineTransitionInfoReference Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 22 flags: FLAGS_NONE enum: 
    // m_fromStateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 28 flags: FLAGS_NONE enum: 
    // m_toStateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_isReturnToPreviousState m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    public partial class hkbStateMachineActiveTransitionInfo : IHavokObject, IEquatable<hkbStateMachineActiveTransitionInfo?>
    {
        private object? m_transitionEffect { set; get; }
        public hkbNodeInternalStateInfo? m_transitionEffectInternalStateInfo { set; get; }
        public hkbStateMachineTransitionInfoReference m_transitionInfoReference { set; get; } = new();
        public hkbStateMachineTransitionInfoReference m_transitionInfoReferenceForTE { set; get; } = new();
        public int m_fromStateId { set; get; }
        public int m_toStateId { set; get; }
        public bool m_isReturnToPreviousState { set; get; }

        public virtual uint Signature { set; get; } = 0xbb90d54f;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            des.ReadEmptyPointer(br);
            m_transitionEffectInternalStateInfo = des.ReadClassPointer<hkbNodeInternalStateInfo>(br);
            m_transitionInfoReference.Read(des, br);
            m_transitionInfoReferenceForTE.Read(des, br);
            m_fromStateId = br.ReadInt32();
            m_toStateId = br.ReadInt32();
            m_isReturnToPreviousState = br.ReadBoolean();
            br.Position += 3;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteVoidPointer(bw);
            s.WriteClassPointer(bw, m_transitionEffectInternalStateInfo);
            m_transitionInfoReference.Write(s, bw);
            m_transitionInfoReferenceForTE.Write(s, bw);
            bw.WriteInt32(m_fromStateId);
            bw.WriteInt32(m_toStateId);
            bw.WriteBoolean(m_isReturnToPreviousState);
            bw.Position += 3;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_transitionEffectInternalStateInfo = xd.ReadClassPointer<hkbNodeInternalStateInfo>(xe, nameof(m_transitionEffectInternalStateInfo));
            m_transitionInfoReference = xd.ReadClass<hkbStateMachineTransitionInfoReference>(xe, nameof(m_transitionInfoReference));
            m_transitionInfoReferenceForTE = xd.ReadClass<hkbStateMachineTransitionInfoReference>(xe, nameof(m_transitionInfoReferenceForTE));
            m_fromStateId = xd.ReadInt32(xe, nameof(m_fromStateId));
            m_toStateId = xd.ReadInt32(xe, nameof(m_toStateId));
            m_isReturnToPreviousState = xd.ReadBoolean(xe, nameof(m_isReturnToPreviousState));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteSerializeIgnored(xe, nameof(m_transitionEffect));
            xs.WriteClassPointer(xe, nameof(m_transitionEffectInternalStateInfo), m_transitionEffectInternalStateInfo);
            xs.WriteClass<hkbStateMachineTransitionInfoReference>(xe, nameof(m_transitionInfoReference), m_transitionInfoReference);
            xs.WriteClass(xe, nameof(m_transitionInfoReferenceForTE), m_transitionInfoReferenceForTE);
            xs.WriteNumber(xe, nameof(m_fromStateId), m_fromStateId);
            xs.WriteNumber(xe, nameof(m_toStateId), m_toStateId);
            xs.WriteBoolean(xe, nameof(m_isReturnToPreviousState), m_isReturnToPreviousState);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbStateMachineActiveTransitionInfo);
        }

        public bool Equals(hkbStateMachineActiveTransitionInfo? other)
        {
            return other is not null &&
                   ((m_transitionEffectInternalStateInfo is null && other.m_transitionEffectInternalStateInfo is null) || (m_transitionEffectInternalStateInfo is not null && other.m_transitionEffectInternalStateInfo is not null && m_transitionEffectInternalStateInfo.Equals((IHavokObject)other.m_transitionEffectInternalStateInfo))) &&
                   ((m_transitionInfoReference is null && other.m_transitionInfoReference is null) || (m_transitionInfoReference is not null && other.m_transitionInfoReference is not null && m_transitionInfoReference.Equals((IHavokObject)other.m_transitionInfoReference))) &&
                   ((m_transitionInfoReferenceForTE is null && other.m_transitionInfoReferenceForTE is null) || (m_transitionInfoReferenceForTE is not null && other.m_transitionInfoReferenceForTE is not null && m_transitionInfoReferenceForTE.Equals((IHavokObject)other.m_transitionInfoReferenceForTE))) &&
                   m_fromStateId.Equals(other.m_fromStateId) &&
                   m_toStateId.Equals(other.m_toStateId) &&
                   m_isReturnToPreviousState.Equals(other.m_isReturnToPreviousState) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_transitionEffectInternalStateInfo);
            hashcode.Add(m_transitionInfoReference);
            hashcode.Add(m_transitionInfoReferenceForTE);
            hashcode.Add(m_fromStateId);
            hashcode.Add(m_toStateId);
            hashcode.Add(m_isReturnToPreviousState);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

