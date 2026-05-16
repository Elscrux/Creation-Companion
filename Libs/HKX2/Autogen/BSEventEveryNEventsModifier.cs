using System.Xml.Linq;
namespace HKX2
{
    // BSEventEveryNEventsModifier Signatire: 0x6030970c size: 128 flags: FLAGS_NONE

    // m_eventToCheckFor m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_eventToSend m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_numberOfEventsBeforeSend m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_minimumNumberOfEventsBeforeSend m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 113 flags: FLAGS_NONE enum: 
    // m_randomizeNumberOfEvents m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 114 flags: FLAGS_NONE enum: 
    // m_numberOfEventsSeen m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 116 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_calculatedNumberOfEventsBeforeSend m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 120 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class BSEventEveryNEventsModifier : hkbModifier, IEquatable<BSEventEveryNEventsModifier?>
    {
        public hkbEventProperty m_eventToCheckFor { set; get; } = new();
        public hkbEventProperty m_eventToSend { set; get; } = new();
        public sbyte m_numberOfEventsBeforeSend { set; get; }
        public sbyte m_minimumNumberOfEventsBeforeSend { set; get; }
        public bool m_randomizeNumberOfEvents { set; get; }
        private int m_numberOfEventsSeen { set; get; }
        private sbyte m_calculatedNumberOfEventsBeforeSend { set; get; }

        public override uint Signature { set; get; } = 0x6030970c;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_eventToCheckFor.Read(des, br);
            m_eventToSend.Read(des, br);
            m_numberOfEventsBeforeSend = br.ReadSByte();
            m_minimumNumberOfEventsBeforeSend = br.ReadSByte();
            m_randomizeNumberOfEvents = br.ReadBoolean();
            br.Position += 1;
            m_numberOfEventsSeen = br.ReadInt32();
            m_calculatedNumberOfEventsBeforeSend = br.ReadSByte();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_eventToCheckFor.Write(s, bw);
            m_eventToSend.Write(s, bw);
            bw.WriteSByte(m_numberOfEventsBeforeSend);
            bw.WriteSByte(m_minimumNumberOfEventsBeforeSend);
            bw.WriteBoolean(m_randomizeNumberOfEvents);
            bw.Position += 1;
            bw.WriteInt32(m_numberOfEventsSeen);
            bw.WriteSByte(m_calculatedNumberOfEventsBeforeSend);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_eventToCheckFor = xd.ReadClass<hkbEventProperty>(xe, nameof(m_eventToCheckFor));
            m_eventToSend = xd.ReadClass<hkbEventProperty>(xe, nameof(m_eventToSend));
            m_numberOfEventsBeforeSend = xd.ReadSByte(xe, nameof(m_numberOfEventsBeforeSend));
            m_minimumNumberOfEventsBeforeSend = xd.ReadSByte(xe, nameof(m_minimumNumberOfEventsBeforeSend));
            m_randomizeNumberOfEvents = xd.ReadBoolean(xe, nameof(m_randomizeNumberOfEvents));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkbEventProperty>(xe, nameof(m_eventToCheckFor), m_eventToCheckFor);
            xs.WriteClass(xe, nameof(m_eventToSend), m_eventToSend);
            xs.WriteNumber(xe, nameof(m_numberOfEventsBeforeSend), m_numberOfEventsBeforeSend);
            xs.WriteNumber(xe, nameof(m_minimumNumberOfEventsBeforeSend), m_minimumNumberOfEventsBeforeSend);
            xs.WriteBoolean(xe, nameof(m_randomizeNumberOfEvents), m_randomizeNumberOfEvents);
            xs.WriteSerializeIgnored(xe, nameof(m_numberOfEventsSeen));
            xs.WriteSerializeIgnored(xe, nameof(m_calculatedNumberOfEventsBeforeSend));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSEventEveryNEventsModifier);
        }

        public bool Equals(BSEventEveryNEventsModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_eventToCheckFor is null && other.m_eventToCheckFor is null) || (m_eventToCheckFor is not null && other.m_eventToCheckFor is not null && m_eventToCheckFor.Equals((IHavokObject)other.m_eventToCheckFor))) &&
                   ((m_eventToSend is null && other.m_eventToSend is null) || (m_eventToSend is not null && other.m_eventToSend is not null && m_eventToSend.Equals((IHavokObject)other.m_eventToSend))) &&
                   m_numberOfEventsBeforeSend.Equals(other.m_numberOfEventsBeforeSend) &&
                   m_minimumNumberOfEventsBeforeSend.Equals(other.m_minimumNumberOfEventsBeforeSend) &&
                   m_randomizeNumberOfEvents.Equals(other.m_randomizeNumberOfEvents) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_eventToCheckFor);
            hashcode.Add(m_eventToSend);
            hashcode.Add(m_numberOfEventsBeforeSend);
            hashcode.Add(m_minimumNumberOfEventsBeforeSend);
            hashcode.Add(m_randomizeNumberOfEvents);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

