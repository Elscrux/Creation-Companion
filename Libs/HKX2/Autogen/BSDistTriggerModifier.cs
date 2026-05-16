using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // BSDistTriggerModifier Signatire: 0xb34d2bbd size: 128 flags: FLAGS_NONE

    // m_targetPosition m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_distance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_distanceTrigger m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    // m_triggerEvent m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    public partial class BSDistTriggerModifier : hkbModifier, IEquatable<BSDistTriggerModifier?>
    {
        public Vector4 m_targetPosition { set; get; }
        public float m_distance { set; get; }
        public float m_distanceTrigger { set; get; }
        public hkbEventProperty m_triggerEvent { set; get; } = new();

        public override uint Signature { set; get; } = 0xb34d2bbd;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_targetPosition = br.ReadVector4();
            m_distance = br.ReadSingle();
            m_distanceTrigger = br.ReadSingle();
            m_triggerEvent.Read(des, br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_targetPosition);
            bw.WriteSingle(m_distance);
            bw.WriteSingle(m_distanceTrigger);
            m_triggerEvent.Write(s, bw);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_targetPosition = xd.ReadVector4(xe, nameof(m_targetPosition));
            m_distance = xd.ReadSingle(xe, nameof(m_distance));
            m_distanceTrigger = xd.ReadSingle(xe, nameof(m_distanceTrigger));
            m_triggerEvent = xd.ReadClass<hkbEventProperty>(xe, nameof(m_triggerEvent));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_targetPosition), m_targetPosition);
            xs.WriteFloat(xe, nameof(m_distance), m_distance);
            xs.WriteFloat(xe, nameof(m_distanceTrigger), m_distanceTrigger);
            xs.WriteClass<hkbEventProperty>(xe, nameof(m_triggerEvent), m_triggerEvent);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSDistTriggerModifier);
        }

        public bool Equals(BSDistTriggerModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_targetPosition.Equals(other.m_targetPosition) &&
                   m_distance.Equals(other.m_distance) &&
                   m_distanceTrigger.Equals(other.m_distanceTrigger) &&
                   ((m_triggerEvent is null && other.m_triggerEvent is null) || (m_triggerEvent is not null && other.m_triggerEvent is not null && m_triggerEvent.Equals((IHavokObject)other.m_triggerEvent))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_targetPosition);
            hashcode.Add(m_distance);
            hashcode.Add(m_distanceTrigger);
            hashcode.Add(m_triggerEvent);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

