using System.Xml.Linq;
namespace HKX2
{
    // hkbStateMachineTransitionInfo Signatire: 0xcdec8025 size: 72 flags: FLAGS_NONE

    // m_triggerInterval m_class: hkbStateMachineTimeInterval Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_initiateInterval m_class: hkbStateMachineTimeInterval Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_transition m_class: hkbTransitionEffect Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_condition m_class: hkbCondition Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_eventId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_toStateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 52 flags: FLAGS_NONE enum: 
    // m_fromNestedStateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_toNestedStateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 60 flags: FLAGS_NONE enum: 
    // m_priority m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_flags m_class:  Type.TYPE_FLAGS Type.TYPE_INT16 arrSize: 0 offset: 66 flags: FLAGS_NONE enum: TransitionFlags
    public partial class hkbStateMachineTransitionInfo : IHavokObject, IEquatable<hkbStateMachineTransitionInfo?>
    {
        public hkbStateMachineTimeInterval m_triggerInterval { set; get; } = new();
        public hkbStateMachineTimeInterval m_initiateInterval { set; get; } = new();
        public hkbTransitionEffect? m_transition { set; get; }
        public hkbCondition? m_condition { set; get; }
        public int m_eventId { set; get; }
        public int m_toStateId { set; get; }
        public int m_fromNestedStateId { set; get; }
        public int m_toNestedStateId { set; get; }
        public short m_priority { set; get; }
        public short m_flags { set; get; }

        public virtual uint Signature { set; get; } = 0xcdec8025;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_triggerInterval.Read(des, br);
            m_initiateInterval.Read(des, br);
            m_transition = des.ReadClassPointer<hkbTransitionEffect>(br);
            m_condition = des.ReadClassPointer<hkbCondition>(br);
            m_eventId = br.ReadInt32();
            m_toStateId = br.ReadInt32();
            m_fromNestedStateId = br.ReadInt32();
            m_toNestedStateId = br.ReadInt32();
            m_priority = br.ReadInt16();
            m_flags = br.ReadInt16();
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_triggerInterval.Write(s, bw);
            m_initiateInterval.Write(s, bw);
            s.WriteClassPointer(bw, m_transition);
            s.WriteClassPointer(bw, m_condition);
            bw.WriteInt32(m_eventId);
            bw.WriteInt32(m_toStateId);
            bw.WriteInt32(m_fromNestedStateId);
            bw.WriteInt32(m_toNestedStateId);
            bw.WriteInt16(m_priority);
            bw.WriteInt16(m_flags);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_triggerInterval = xd.ReadClass<hkbStateMachineTimeInterval>(xe, nameof(m_triggerInterval));
            m_initiateInterval = xd.ReadClass<hkbStateMachineTimeInterval>(xe, nameof(m_initiateInterval));
            m_transition = xd.ReadClassPointer<hkbTransitionEffect>(xe, nameof(m_transition));
            m_condition = xd.ReadClassPointer<hkbCondition>(xe, nameof(m_condition));
            m_eventId = xd.ReadInt32(xe, nameof(m_eventId));
            m_toStateId = xd.ReadInt32(xe, nameof(m_toStateId));
            m_fromNestedStateId = xd.ReadInt32(xe, nameof(m_fromNestedStateId));
            m_toNestedStateId = xd.ReadInt32(xe, nameof(m_toNestedStateId));
            m_priority = xd.ReadInt16(xe, nameof(m_priority));
            m_flags = xd.ReadFlag<TransitionFlags, short>(xe, nameof(m_flags));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkbStateMachineTimeInterval>(xe, nameof(m_triggerInterval), m_triggerInterval);
            xs.WriteClass(xe, nameof(m_initiateInterval), m_initiateInterval);
            xs.WriteClassPointer(xe, nameof(m_transition), m_transition);
            xs.WriteClassPointer(xe, nameof(m_condition), m_condition);
            xs.WriteNumber(xe, nameof(m_eventId), m_eventId);
            xs.WriteNumber(xe, nameof(m_toStateId), m_toStateId);
            xs.WriteNumber(xe, nameof(m_fromNestedStateId), m_fromNestedStateId);
            xs.WriteNumber(xe, nameof(m_toNestedStateId), m_toNestedStateId);
            xs.WriteNumber(xe, nameof(m_priority), m_priority);
            xs.WriteFlag<TransitionFlags, short>(xe, nameof(m_flags), m_flags);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbStateMachineTransitionInfo);
        }

        public bool Equals(hkbStateMachineTransitionInfo? other)
        {
            return other is not null &&
                   ((m_triggerInterval is null && other.m_triggerInterval is null) || (m_triggerInterval is not null && other.m_triggerInterval is not null && m_triggerInterval.Equals((IHavokObject)other.m_triggerInterval))) &&
                   ((m_initiateInterval is null && other.m_initiateInterval is null) || (m_initiateInterval is not null && other.m_initiateInterval is not null && m_initiateInterval.Equals((IHavokObject)other.m_initiateInterval))) &&
                   ((m_transition is null && other.m_transition is null) || (m_transition is not null && other.m_transition is not null && m_transition.Equals((IHavokObject)other.m_transition))) &&
                   ((m_condition is null && other.m_condition is null) || (m_condition is not null && other.m_condition is not null && m_condition.Equals((IHavokObject)other.m_condition))) &&
                   m_eventId.Equals(other.m_eventId) &&
                   m_toStateId.Equals(other.m_toStateId) &&
                   m_fromNestedStateId.Equals(other.m_fromNestedStateId) &&
                   m_toNestedStateId.Equals(other.m_toNestedStateId) &&
                   m_priority.Equals(other.m_priority) &&
                   m_flags.Equals(other.m_flags) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_triggerInterval);
            hashcode.Add(m_initiateInterval);
            hashcode.Add(m_transition);
            hashcode.Add(m_condition);
            hashcode.Add(m_eventId);
            hashcode.Add(m_toStateId);
            hashcode.Add(m_fromNestedStateId);
            hashcode.Add(m_toNestedStateId);
            hashcode.Add(m_priority);
            hashcode.Add(m_flags);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

