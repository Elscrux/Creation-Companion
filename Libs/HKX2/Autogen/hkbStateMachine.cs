using System.Xml.Linq;
namespace HKX2
{
    // hkbStateMachine Signatire: 0x816c1dcb size: 264 flags: FLAGS_NONE

    // m_eventToSendWhenStateOrTransitionChanges m_class: hkbEvent Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_startStateChooser m_class: hkbStateChooser Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_startStateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_returnToPreviousStateEventId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 108 flags: FLAGS_NONE enum: 
    // m_randomTransitionEventId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_transitionToNextHigherStateEventId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 116 flags: FLAGS_NONE enum: 
    // m_transitionToNextLowerStateEventId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 120 flags: FLAGS_NONE enum: 
    // m_syncVariableIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 124 flags: FLAGS_NONE enum: 
    // m_currentStateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 128 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_wrapAroundStateId m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 132 flags: FLAGS_NONE enum: 
    // m_maxSimultaneousTransitions m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 133 flags: FLAGS_NONE enum: 
    // m_startStateMode m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 134 flags: FLAGS_NONE enum: StartStateMode
    // m_selfTransitionMode m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 135 flags: FLAGS_NONE enum: StateMachineSelfTransitionMode
    // m_isActive m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 136 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_states m_class: hkbStateMachineStateInfo Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_wildcardTransitions m_class: hkbStateMachineTransitionInfoArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_stateIdToIndexMap m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 168 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_activeTransitions m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 176 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_transitionFlags m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 192 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_wildcardTransitionFlags m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 208 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_delayedTransitions m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 224 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_timeInState m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 240 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_lastLocalTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 244 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_previousStateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 248 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_nextStartStateIndexOverride m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 252 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_stateOrTransitionChanged m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 256 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_echoNextUpdate m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 257 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_sCurrentStateIndexAndEntered m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 258 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbStateMachine : hkbGenerator, IEquatable<hkbStateMachine?>
    {
        public hkbEvent m_eventToSendWhenStateOrTransitionChanges { set; get; } = new();
        public hkbStateChooser? m_startStateChooser { set; get; }
        public int m_startStateId { set; get; }
        public int m_returnToPreviousStateEventId { set; get; }
        public int m_randomTransitionEventId { set; get; }
        public int m_transitionToNextHigherStateEventId { set; get; }
        public int m_transitionToNextLowerStateEventId { set; get; }
        public int m_syncVariableIndex { set; get; }
        private int m_currentStateId { set; get; }
        public bool m_wrapAroundStateId { set; get; }
        public sbyte m_maxSimultaneousTransitions { set; get; }
        public sbyte m_startStateMode { set; get; }
        public sbyte m_selfTransitionMode { set; get; }
        private bool m_isActive { set; get; }
        public IList<hkbStateMachineStateInfo> m_states { set; get; } = Array.Empty<hkbStateMachineStateInfo>();
        public hkbStateMachineTransitionInfoArray? m_wildcardTransitions { set; get; }
        private object? m_stateIdToIndexMap { set; get; }
        public IList<object> m_activeTransitions { set; get; } = Array.Empty<object>();
        public IList<object> m_transitionFlags { set; get; } = Array.Empty<object>();
        public IList<object> m_wildcardTransitionFlags { set; get; } = Array.Empty<object>();
        public IList<object> m_delayedTransitions { set; get; } = Array.Empty<object>();
        private float m_timeInState { set; get; }
        private float m_lastLocalTime { set; get; }
        private int m_previousStateId { set; get; }
        private int m_nextStartStateIndexOverride { set; get; }
        private bool m_stateOrTransitionChanged { set; get; }
        private bool m_echoNextUpdate { set; get; }
        private ushort m_sCurrentStateIndexAndEntered { set; get; }

        public override uint Signature { set; get; } = 0x816c1dcb;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_eventToSendWhenStateOrTransitionChanges.Read(des, br);
            m_startStateChooser = des.ReadClassPointer<hkbStateChooser>(br);
            m_startStateId = br.ReadInt32();
            m_returnToPreviousStateEventId = br.ReadInt32();
            m_randomTransitionEventId = br.ReadInt32();
            m_transitionToNextHigherStateEventId = br.ReadInt32();
            m_transitionToNextLowerStateEventId = br.ReadInt32();
            m_syncVariableIndex = br.ReadInt32();
            m_currentStateId = br.ReadInt32();
            m_wrapAroundStateId = br.ReadBoolean();
            m_maxSimultaneousTransitions = br.ReadSByte();
            m_startStateMode = br.ReadSByte();
            m_selfTransitionMode = br.ReadSByte();
            m_isActive = br.ReadBoolean();
            br.Position += 7;
            m_states = des.ReadClassPointerArray<hkbStateMachineStateInfo>(br);
            m_wildcardTransitions = des.ReadClassPointer<hkbStateMachineTransitionInfoArray>(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            m_timeInState = br.ReadSingle();
            m_lastLocalTime = br.ReadSingle();
            m_previousStateId = br.ReadInt32();
            m_nextStartStateIndexOverride = br.ReadInt32();
            m_stateOrTransitionChanged = br.ReadBoolean();
            m_echoNextUpdate = br.ReadBoolean();
            m_sCurrentStateIndexAndEntered = br.ReadUInt16();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_eventToSendWhenStateOrTransitionChanges.Write(s, bw);
            s.WriteClassPointer(bw, m_startStateChooser);
            bw.WriteInt32(m_startStateId);
            bw.WriteInt32(m_returnToPreviousStateEventId);
            bw.WriteInt32(m_randomTransitionEventId);
            bw.WriteInt32(m_transitionToNextHigherStateEventId);
            bw.WriteInt32(m_transitionToNextLowerStateEventId);
            bw.WriteInt32(m_syncVariableIndex);
            bw.WriteInt32(m_currentStateId);
            bw.WriteBoolean(m_wrapAroundStateId);
            bw.WriteSByte(m_maxSimultaneousTransitions);
            bw.WriteSByte(m_startStateMode);
            bw.WriteSByte(m_selfTransitionMode);
            bw.WriteBoolean(m_isActive);
            bw.Position += 7;
            s.WriteClassPointerArray(bw, m_states);
            s.WriteClassPointer(bw, m_wildcardTransitions);
            s.WriteVoidPointer(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            bw.WriteSingle(m_timeInState);
            bw.WriteSingle(m_lastLocalTime);
            bw.WriteInt32(m_previousStateId);
            bw.WriteInt32(m_nextStartStateIndexOverride);
            bw.WriteBoolean(m_stateOrTransitionChanged);
            bw.WriteBoolean(m_echoNextUpdate);
            bw.WriteUInt16(m_sCurrentStateIndexAndEntered);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_eventToSendWhenStateOrTransitionChanges = xd.ReadClass<hkbEvent>(xe, nameof(m_eventToSendWhenStateOrTransitionChanges));
            m_startStateChooser = xd.ReadClassPointer<hkbStateChooser>(xe, nameof(m_startStateChooser));
            m_startStateId = xd.ReadInt32(xe, nameof(m_startStateId));
            m_returnToPreviousStateEventId = xd.ReadInt32(xe, nameof(m_returnToPreviousStateEventId));
            m_randomTransitionEventId = xd.ReadInt32(xe, nameof(m_randomTransitionEventId));
            m_transitionToNextHigherStateEventId = xd.ReadInt32(xe, nameof(m_transitionToNextHigherStateEventId));
            m_transitionToNextLowerStateEventId = xd.ReadInt32(xe, nameof(m_transitionToNextLowerStateEventId));
            m_syncVariableIndex = xd.ReadInt32(xe, nameof(m_syncVariableIndex));
            m_wrapAroundStateId = xd.ReadBoolean(xe, nameof(m_wrapAroundStateId));
            m_maxSimultaneousTransitions = xd.ReadSByte(xe, nameof(m_maxSimultaneousTransitions));
            m_startStateMode = xd.ReadFlag<StartStateMode, sbyte>(xe, nameof(m_startStateMode));
            m_selfTransitionMode = xd.ReadFlag<StateMachineSelfTransitionMode, sbyte>(xe, nameof(m_selfTransitionMode));
            m_states = xd.ReadClassPointerArray<hkbStateMachineStateInfo>(xe, nameof(m_states));
            m_wildcardTransitions = xd.ReadClassPointer<hkbStateMachineTransitionInfoArray>(xe, nameof(m_wildcardTransitions));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkbEvent>(xe, nameof(m_eventToSendWhenStateOrTransitionChanges), m_eventToSendWhenStateOrTransitionChanges);
            xs.WriteClassPointer(xe, nameof(m_startStateChooser), m_startStateChooser);
            xs.WriteNumber(xe, nameof(m_startStateId), m_startStateId);
            xs.WriteNumber(xe, nameof(m_returnToPreviousStateEventId), m_returnToPreviousStateEventId);
            xs.WriteNumber(xe, nameof(m_randomTransitionEventId), m_randomTransitionEventId);
            xs.WriteNumber(xe, nameof(m_transitionToNextHigherStateEventId), m_transitionToNextHigherStateEventId);
            xs.WriteNumber(xe, nameof(m_transitionToNextLowerStateEventId), m_transitionToNextLowerStateEventId);
            xs.WriteNumber(xe, nameof(m_syncVariableIndex), m_syncVariableIndex);
            xs.WriteSerializeIgnored(xe, nameof(m_currentStateId));
            xs.WriteBoolean(xe, nameof(m_wrapAroundStateId), m_wrapAroundStateId);
            xs.WriteNumber(xe, nameof(m_maxSimultaneousTransitions), m_maxSimultaneousTransitions);
            xs.WriteEnum<StartStateMode, sbyte>(xe, nameof(m_startStateMode), m_startStateMode);
            xs.WriteEnum<StateMachineSelfTransitionMode, sbyte>(xe, nameof(m_selfTransitionMode), m_selfTransitionMode);
            xs.WriteSerializeIgnored(xe, nameof(m_isActive));
            xs.WriteClassPointerArray(xe, nameof(m_states), m_states);
            xs.WriteClassPointer(xe, nameof(m_wildcardTransitions), m_wildcardTransitions);
            xs.WriteSerializeIgnored(xe, nameof(m_stateIdToIndexMap));
            xs.WriteSerializeIgnored(xe, nameof(m_activeTransitions));
            xs.WriteSerializeIgnored(xe, nameof(m_transitionFlags));
            xs.WriteSerializeIgnored(xe, nameof(m_wildcardTransitionFlags));
            xs.WriteSerializeIgnored(xe, nameof(m_delayedTransitions));
            xs.WriteSerializeIgnored(xe, nameof(m_timeInState));
            xs.WriteSerializeIgnored(xe, nameof(m_lastLocalTime));
            xs.WriteSerializeIgnored(xe, nameof(m_previousStateId));
            xs.WriteSerializeIgnored(xe, nameof(m_nextStartStateIndexOverride));
            xs.WriteSerializeIgnored(xe, nameof(m_stateOrTransitionChanged));
            xs.WriteSerializeIgnored(xe, nameof(m_echoNextUpdate));
            xs.WriteSerializeIgnored(xe, nameof(m_sCurrentStateIndexAndEntered));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbStateMachine);
        }

        public bool Equals(hkbStateMachine? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_eventToSendWhenStateOrTransitionChanges is null && other.m_eventToSendWhenStateOrTransitionChanges is null) || (m_eventToSendWhenStateOrTransitionChanges is not null && other.m_eventToSendWhenStateOrTransitionChanges is not null && m_eventToSendWhenStateOrTransitionChanges.Equals((IHavokObject)other.m_eventToSendWhenStateOrTransitionChanges))) &&
                   ((m_startStateChooser is null && other.m_startStateChooser is null) || (m_startStateChooser is not null && other.m_startStateChooser is not null && m_startStateChooser.Equals((IHavokObject)other.m_startStateChooser))) &&
                   m_startStateId.Equals(other.m_startStateId) &&
                   m_returnToPreviousStateEventId.Equals(other.m_returnToPreviousStateEventId) &&
                   m_randomTransitionEventId.Equals(other.m_randomTransitionEventId) &&
                   m_transitionToNextHigherStateEventId.Equals(other.m_transitionToNextHigherStateEventId) &&
                   m_transitionToNextLowerStateEventId.Equals(other.m_transitionToNextLowerStateEventId) &&
                   m_syncVariableIndex.Equals(other.m_syncVariableIndex) &&
                   m_wrapAroundStateId.Equals(other.m_wrapAroundStateId) &&
                   m_maxSimultaneousTransitions.Equals(other.m_maxSimultaneousTransitions) &&
                   m_startStateMode.Equals(other.m_startStateMode) &&
                   m_selfTransitionMode.Equals(other.m_selfTransitionMode) &&
                   m_states.SequenceEqual(other.m_states) &&
                   ((m_wildcardTransitions is null && other.m_wildcardTransitions is null) || (m_wildcardTransitions is not null && other.m_wildcardTransitions is not null && m_wildcardTransitions.Equals((IHavokObject)other.m_wildcardTransitions))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_eventToSendWhenStateOrTransitionChanges);
            hashcode.Add(m_startStateChooser);
            hashcode.Add(m_startStateId);
            hashcode.Add(m_returnToPreviousStateEventId);
            hashcode.Add(m_randomTransitionEventId);
            hashcode.Add(m_transitionToNextHigherStateEventId);
            hashcode.Add(m_transitionToNextLowerStateEventId);
            hashcode.Add(m_syncVariableIndex);
            hashcode.Add(m_wrapAroundStateId);
            hashcode.Add(m_maxSimultaneousTransitions);
            hashcode.Add(m_startStateMode);
            hashcode.Add(m_selfTransitionMode);
            hashcode.Add(m_states.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_wildcardTransitions);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

