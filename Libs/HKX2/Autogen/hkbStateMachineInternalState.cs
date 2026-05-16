using System.Xml.Linq;
namespace HKX2
{
    // hkbStateMachineInternalState Signatire: 0xbd1a7502 size: 104 flags: FLAGS_NONE

    // m_activeTransitions m_class: hkbStateMachineActiveTransitionInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_transitionFlags m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_wildcardTransitionFlags m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_delayedTransitions m_class: hkbStateMachineDelayedTransitionInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_timeInState m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_lastLocalTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_currentStateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_previousStateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 92 flags: FLAGS_NONE enum: 
    // m_nextStartStateIndexOverride m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_stateOrTransitionChanged m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    // m_echoNextUpdate m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 101 flags: FLAGS_NONE enum: 
    public partial class hkbStateMachineInternalState : hkReferencedObject, IEquatable<hkbStateMachineInternalState?>
    {
        public IList<hkbStateMachineActiveTransitionInfo> m_activeTransitions { set; get; } = Array.Empty<hkbStateMachineActiveTransitionInfo>();
        public IList<byte> m_transitionFlags { set; get; } = Array.Empty<byte>();
        public IList<byte> m_wildcardTransitionFlags { set; get; } = Array.Empty<byte>();
        public IList<hkbStateMachineDelayedTransitionInfo> m_delayedTransitions { set; get; } = Array.Empty<hkbStateMachineDelayedTransitionInfo>();
        public float m_timeInState { set; get; }
        public float m_lastLocalTime { set; get; }
        public int m_currentStateId { set; get; }
        public int m_previousStateId { set; get; }
        public int m_nextStartStateIndexOverride { set; get; }
        public bool m_stateOrTransitionChanged { set; get; }
        public bool m_echoNextUpdate { set; get; }

        public override uint Signature { set; get; } = 0xbd1a7502;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_activeTransitions = des.ReadClassArray<hkbStateMachineActiveTransitionInfo>(br);
            m_transitionFlags = des.ReadByteArray(br);
            m_wildcardTransitionFlags = des.ReadByteArray(br);
            m_delayedTransitions = des.ReadClassArray<hkbStateMachineDelayedTransitionInfo>(br);
            m_timeInState = br.ReadSingle();
            m_lastLocalTime = br.ReadSingle();
            m_currentStateId = br.ReadInt32();
            m_previousStateId = br.ReadInt32();
            m_nextStartStateIndexOverride = br.ReadInt32();
            m_stateOrTransitionChanged = br.ReadBoolean();
            m_echoNextUpdate = br.ReadBoolean();
            br.Position += 2;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_activeTransitions);
            s.WriteByteArray(bw, m_transitionFlags);
            s.WriteByteArray(bw, m_wildcardTransitionFlags);
            s.WriteClassArray(bw, m_delayedTransitions);
            bw.WriteSingle(m_timeInState);
            bw.WriteSingle(m_lastLocalTime);
            bw.WriteInt32(m_currentStateId);
            bw.WriteInt32(m_previousStateId);
            bw.WriteInt32(m_nextStartStateIndexOverride);
            bw.WriteBoolean(m_stateOrTransitionChanged);
            bw.WriteBoolean(m_echoNextUpdate);
            bw.Position += 2;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_activeTransitions = xd.ReadClassArray<hkbStateMachineActiveTransitionInfo>(xe, nameof(m_activeTransitions));
            m_transitionFlags = xd.ReadByteArray(xe, nameof(m_transitionFlags));
            m_wildcardTransitionFlags = xd.ReadByteArray(xe, nameof(m_wildcardTransitionFlags));
            m_delayedTransitions = xd.ReadClassArray<hkbStateMachineDelayedTransitionInfo>(xe, nameof(m_delayedTransitions));
            m_timeInState = xd.ReadSingle(xe, nameof(m_timeInState));
            m_lastLocalTime = xd.ReadSingle(xe, nameof(m_lastLocalTime));
            m_currentStateId = xd.ReadInt32(xe, nameof(m_currentStateId));
            m_previousStateId = xd.ReadInt32(xe, nameof(m_previousStateId));
            m_nextStartStateIndexOverride = xd.ReadInt32(xe, nameof(m_nextStartStateIndexOverride));
            m_stateOrTransitionChanged = xd.ReadBoolean(xe, nameof(m_stateOrTransitionChanged));
            m_echoNextUpdate = xd.ReadBoolean(xe, nameof(m_echoNextUpdate));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_activeTransitions), m_activeTransitions);
            xs.WriteNumberArray(xe, nameof(m_transitionFlags), m_transitionFlags);
            xs.WriteNumberArray(xe, nameof(m_wildcardTransitionFlags), m_wildcardTransitionFlags);
            xs.WriteClassArray(xe, nameof(m_delayedTransitions), m_delayedTransitions);
            xs.WriteFloat(xe, nameof(m_timeInState), m_timeInState);
            xs.WriteFloat(xe, nameof(m_lastLocalTime), m_lastLocalTime);
            xs.WriteNumber(xe, nameof(m_currentStateId), m_currentStateId);
            xs.WriteNumber(xe, nameof(m_previousStateId), m_previousStateId);
            xs.WriteNumber(xe, nameof(m_nextStartStateIndexOverride), m_nextStartStateIndexOverride);
            xs.WriteBoolean(xe, nameof(m_stateOrTransitionChanged), m_stateOrTransitionChanged);
            xs.WriteBoolean(xe, nameof(m_echoNextUpdate), m_echoNextUpdate);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbStateMachineInternalState);
        }

        public bool Equals(hkbStateMachineInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_activeTransitions.SequenceEqual(other.m_activeTransitions) &&
                   m_transitionFlags.SequenceEqual(other.m_transitionFlags) &&
                   m_wildcardTransitionFlags.SequenceEqual(other.m_wildcardTransitionFlags) &&
                   m_delayedTransitions.SequenceEqual(other.m_delayedTransitions) &&
                   m_timeInState.Equals(other.m_timeInState) &&
                   m_lastLocalTime.Equals(other.m_lastLocalTime) &&
                   m_currentStateId.Equals(other.m_currentStateId) &&
                   m_previousStateId.Equals(other.m_previousStateId) &&
                   m_nextStartStateIndexOverride.Equals(other.m_nextStartStateIndexOverride) &&
                   m_stateOrTransitionChanged.Equals(other.m_stateOrTransitionChanged) &&
                   m_echoNextUpdate.Equals(other.m_echoNextUpdate) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_activeTransitions.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_transitionFlags.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_wildcardTransitionFlags.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_delayedTransitions.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_timeInState);
            hashcode.Add(m_lastLocalTime);
            hashcode.Add(m_currentStateId);
            hashcode.Add(m_previousStateId);
            hashcode.Add(m_nextStartStateIndexOverride);
            hashcode.Add(m_stateOrTransitionChanged);
            hashcode.Add(m_echoNextUpdate);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

