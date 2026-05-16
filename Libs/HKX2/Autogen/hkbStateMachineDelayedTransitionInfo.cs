using System.Xml.Linq;
namespace HKX2
{
    // hkbStateMachineDelayedTransitionInfo Signatire: 0x26d5499 size: 24 flags: FLAGS_NONE

    // m_delayedTransition m_class: hkbStateMachineProspectiveTransitionInfo Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_timeDelayed m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_isDelayedTransitionReturnToPreviousState m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 20 flags: FLAGS_NONE enum: 
    // m_wasInAbutRangeLastFrame m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 21 flags: FLAGS_NONE enum: 
    public partial class hkbStateMachineDelayedTransitionInfo : IHavokObject, IEquatable<hkbStateMachineDelayedTransitionInfo?>
    {
        public hkbStateMachineProspectiveTransitionInfo m_delayedTransition { set; get; } = new();
        public float m_timeDelayed { set; get; }
        public bool m_isDelayedTransitionReturnToPreviousState { set; get; }
        public bool m_wasInAbutRangeLastFrame { set; get; }

        public virtual uint Signature { set; get; } = 0x26d5499;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_delayedTransition.Read(des, br);
            m_timeDelayed = br.ReadSingle();
            m_isDelayedTransitionReturnToPreviousState = br.ReadBoolean();
            m_wasInAbutRangeLastFrame = br.ReadBoolean();
            br.Position += 2;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_delayedTransition.Write(s, bw);
            bw.WriteSingle(m_timeDelayed);
            bw.WriteBoolean(m_isDelayedTransitionReturnToPreviousState);
            bw.WriteBoolean(m_wasInAbutRangeLastFrame);
            bw.Position += 2;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_delayedTransition = xd.ReadClass<hkbStateMachineProspectiveTransitionInfo>(xe, nameof(m_delayedTransition));
            m_timeDelayed = xd.ReadSingle(xe, nameof(m_timeDelayed));
            m_isDelayedTransitionReturnToPreviousState = xd.ReadBoolean(xe, nameof(m_isDelayedTransitionReturnToPreviousState));
            m_wasInAbutRangeLastFrame = xd.ReadBoolean(xe, nameof(m_wasInAbutRangeLastFrame));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkbStateMachineProspectiveTransitionInfo>(xe, nameof(m_delayedTransition), m_delayedTransition);
            xs.WriteFloat(xe, nameof(m_timeDelayed), m_timeDelayed);
            xs.WriteBoolean(xe, nameof(m_isDelayedTransitionReturnToPreviousState), m_isDelayedTransitionReturnToPreviousState);
            xs.WriteBoolean(xe, nameof(m_wasInAbutRangeLastFrame), m_wasInAbutRangeLastFrame);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbStateMachineDelayedTransitionInfo);
        }

        public bool Equals(hkbStateMachineDelayedTransitionInfo? other)
        {
            return other is not null &&
                   ((m_delayedTransition is null && other.m_delayedTransition is null) || (m_delayedTransition is not null && other.m_delayedTransition is not null && m_delayedTransition.Equals((IHavokObject)other.m_delayedTransition))) &&
                   m_timeDelayed.Equals(other.m_timeDelayed) &&
                   m_isDelayedTransitionReturnToPreviousState.Equals(other.m_isDelayedTransitionReturnToPreviousState) &&
                   m_wasInAbutRangeLastFrame.Equals(other.m_wasInAbutRangeLastFrame) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_delayedTransition);
            hashcode.Add(m_timeDelayed);
            hashcode.Add(m_isDelayedTransitionReturnToPreviousState);
            hashcode.Add(m_wasInAbutRangeLastFrame);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

