using System.Xml.Linq;
namespace HKX2
{
    // hkbStateMachineStateInfo Signatire: 0xed7f9d0 size: 120 flags: FLAGS_NONE

    // m_listeners m_class: hkbStateListener Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_enterNotifyEvents m_class: hkbStateMachineEventPropertyArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_exitNotifyEvents m_class: hkbStateMachineEventPropertyArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_transitions m_class: hkbStateMachineTransitionInfoArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_generator m_class: hkbGenerator Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_stateId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_probability m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 108 flags: FLAGS_NONE enum: 
    // m_enable m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    public partial class hkbStateMachineStateInfo : hkbBindable, IEquatable<hkbStateMachineStateInfo?>
    {
        public IList<hkbStateListener> m_listeners { set; get; } = Array.Empty<hkbStateListener>();
        public hkbStateMachineEventPropertyArray? m_enterNotifyEvents { set; get; }
        public hkbStateMachineEventPropertyArray? m_exitNotifyEvents { set; get; }
        public hkbStateMachineTransitionInfoArray? m_transitions { set; get; }
        public hkbGenerator? m_generator { set; get; }
        public string m_name { set; get; } = "";
        public int m_stateId { set; get; }
        public float m_probability { set; get; }
        public bool m_enable { set; get; }

        public override uint Signature { set; get; } = 0xed7f9d0;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_listeners = des.ReadClassPointerArray<hkbStateListener>(br);
            m_enterNotifyEvents = des.ReadClassPointer<hkbStateMachineEventPropertyArray>(br);
            m_exitNotifyEvents = des.ReadClassPointer<hkbStateMachineEventPropertyArray>(br);
            m_transitions = des.ReadClassPointer<hkbStateMachineTransitionInfoArray>(br);
            m_generator = des.ReadClassPointer<hkbGenerator>(br);
            m_name = des.ReadStringPointer(br);
            m_stateId = br.ReadInt32();
            m_probability = br.ReadSingle();
            m_enable = br.ReadBoolean();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_listeners);
            s.WriteClassPointer(bw, m_enterNotifyEvents);
            s.WriteClassPointer(bw, m_exitNotifyEvents);
            s.WriteClassPointer(bw, m_transitions);
            s.WriteClassPointer(bw, m_generator);
            s.WriteStringPointer(bw, m_name);
            bw.WriteInt32(m_stateId);
            bw.WriteSingle(m_probability);
            bw.WriteBoolean(m_enable);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_listeners = xd.ReadClassPointerArray<hkbStateListener>(xe, nameof(m_listeners));
            m_enterNotifyEvents = xd.ReadClassPointer<hkbStateMachineEventPropertyArray>(xe, nameof(m_enterNotifyEvents));
            m_exitNotifyEvents = xd.ReadClassPointer<hkbStateMachineEventPropertyArray>(xe, nameof(m_exitNotifyEvents));
            m_transitions = xd.ReadClassPointer<hkbStateMachineTransitionInfoArray>(xe, nameof(m_transitions));
            m_generator = xd.ReadClassPointer<hkbGenerator>(xe, nameof(m_generator));
            m_name = xd.ReadString(xe, nameof(m_name));
            m_stateId = xd.ReadInt32(xe, nameof(m_stateId));
            m_probability = xd.ReadSingle(xe, nameof(m_probability));
            m_enable = xd.ReadBoolean(xe, nameof(m_enable));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_listeners), m_listeners);
            xs.WriteClassPointer(xe, nameof(m_enterNotifyEvents), m_enterNotifyEvents);
            xs.WriteClassPointer(xe, nameof(m_exitNotifyEvents), m_exitNotifyEvents);
            xs.WriteClassPointer(xe, nameof(m_transitions), m_transitions);
            xs.WriteClassPointer(xe, nameof(m_generator), m_generator);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteNumber(xe, nameof(m_stateId), m_stateId);
            xs.WriteFloat(xe, nameof(m_probability), m_probability);
            xs.WriteBoolean(xe, nameof(m_enable), m_enable);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbStateMachineStateInfo);
        }

        public bool Equals(hkbStateMachineStateInfo? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_listeners.SequenceEqual(other.m_listeners) &&
                   ((m_enterNotifyEvents is null && other.m_enterNotifyEvents is null) || (m_enterNotifyEvents is not null && other.m_enterNotifyEvents is not null && m_enterNotifyEvents.Equals((IHavokObject)other.m_enterNotifyEvents))) &&
                   ((m_exitNotifyEvents is null && other.m_exitNotifyEvents is null) || (m_exitNotifyEvents is not null && other.m_exitNotifyEvents is not null && m_exitNotifyEvents.Equals((IHavokObject)other.m_exitNotifyEvents))) &&
                   ((m_transitions is null && other.m_transitions is null) || (m_transitions is not null && other.m_transitions is not null && m_transitions.Equals((IHavokObject)other.m_transitions))) &&
                   ((m_generator is null && other.m_generator is null) || (m_generator is not null && other.m_generator is not null && m_generator.Equals((IHavokObject)other.m_generator))) &&
                   m_name == other.m_name &&
                   m_stateId.Equals(other.m_stateId) &&
                   m_probability.Equals(other.m_probability) &&
                   m_enable.Equals(other.m_enable) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_listeners.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_enterNotifyEvents);
            hashcode.Add(m_exitNotifyEvents);
            hashcode.Add(m_transitions);
            hashcode.Add(m_generator);
            hashcode.Add(m_name);
            hashcode.Add(m_stateId);
            hashcode.Add(m_probability);
            hashcode.Add(m_enable);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

