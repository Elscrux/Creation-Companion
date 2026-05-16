using System.Xml.Linq;
namespace HKX2
{
    // hkpTriggerVolume Signatire: 0xa29a8d1a size: 88 flags: FLAGS_NONE

    // m_overlappingBodies m_class: hkpRigidBody Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_eventQueue m_class: hkpTriggerVolumeEventInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_triggerBody m_class: hkpRigidBody Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_sequenceNumber m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkpTriggerVolume : hkReferencedObject, IEquatable<hkpTriggerVolume?>
    {
        public IList<hkpRigidBody> m_overlappingBodies { set; get; } = Array.Empty<hkpRigidBody>();
        public IList<hkpTriggerVolumeEventInfo> m_eventQueue { set; get; } = Array.Empty<hkpTriggerVolumeEventInfo>();
        public hkpRigidBody? m_triggerBody { set; get; }
        public uint m_sequenceNumber { set; get; }

        public override uint Signature { set; get; } = 0xa29a8d1a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 24;
            m_overlappingBodies = des.ReadClassPointerArray<hkpRigidBody>(br);
            m_eventQueue = des.ReadClassArray<hkpTriggerVolumeEventInfo>(br);
            m_triggerBody = des.ReadClassPointer<hkpRigidBody>(br);
            m_sequenceNumber = br.ReadUInt32();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 24;
            s.WriteClassPointerArray(bw, m_overlappingBodies);
            s.WriteClassArray(bw, m_eventQueue);
            s.WriteClassPointer(bw, m_triggerBody);
            bw.WriteUInt32(m_sequenceNumber);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_overlappingBodies = xd.ReadClassPointerArray<hkpRigidBody>(xe, nameof(m_overlappingBodies));
            m_eventQueue = xd.ReadClassArray<hkpTriggerVolumeEventInfo>(xe, nameof(m_eventQueue));
            m_triggerBody = xd.ReadClassPointer<hkpRigidBody>(xe, nameof(m_triggerBody));
            m_sequenceNumber = xd.ReadUInt32(xe, nameof(m_sequenceNumber));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_overlappingBodies), m_overlappingBodies);
            xs.WriteClassArray(xe, nameof(m_eventQueue), m_eventQueue);
            xs.WriteClassPointer(xe, nameof(m_triggerBody), m_triggerBody);
            xs.WriteNumber(xe, nameof(m_sequenceNumber), m_sequenceNumber);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpTriggerVolume);
        }

        public bool Equals(hkpTriggerVolume? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_overlappingBodies.SequenceEqual(other.m_overlappingBodies) &&
                   m_eventQueue.SequenceEqual(other.m_eventQueue) &&
                   ((m_triggerBody is null && other.m_triggerBody is null) || (m_triggerBody is not null && other.m_triggerBody is not null && m_triggerBody.Equals((IHavokObject)other.m_triggerBody))) &&
                   m_sequenceNumber.Equals(other.m_sequenceNumber) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_overlappingBodies.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_eventQueue.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_triggerBody);
            hashcode.Add(m_sequenceNumber);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

