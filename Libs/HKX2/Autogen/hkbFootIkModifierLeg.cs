using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbFootIkModifierLeg Signatire: 0x9f3e3a04 size: 160 flags: FLAGS_NONE

    // m_originalAnkleTransformMS m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_prevAnkleRotLS m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 48 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_kneeAxisLS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_footEndLS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_ungroundedEvent m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_footPlantedAnkleHeightMS m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_footRaisedAnkleHeightMS m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 116 flags: FLAGS_NONE enum: 
    // m_maxAnkleHeightMS m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 120 flags: FLAGS_NONE enum: 
    // m_minAnkleHeightMS m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 124 flags: FLAGS_NONE enum: 
    // m_maxKneeAngleDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_minKneeAngleDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 132 flags: FLAGS_NONE enum: 
    // m_verticalError m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 136 flags: FLAGS_NONE enum: 
    // m_maxAnkleAngleDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 140 flags: FLAGS_NONE enum: 
    // m_hipIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_kneeIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 146 flags: FLAGS_NONE enum: 
    // m_ankleIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 148 flags: FLAGS_NONE enum: 
    // m_hitSomething m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 150 flags: FLAGS_NONE enum: 
    // m_isPlantedMS m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 151 flags: FLAGS_NONE enum: 
    // m_isOriginalAnkleTransformMSSet m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 152 flags: FLAGS_NONE enum: 
    public partial class hkbFootIkModifierLeg : IHavokObject, IEquatable<hkbFootIkModifierLeg?>
    {
        public Matrix4x4 m_originalAnkleTransformMS { set; get; }
        private Quaternion m_prevAnkleRotLS { set; get; }
        public Vector4 m_kneeAxisLS { set; get; }
        public Vector4 m_footEndLS { set; get; }
        public hkbEventProperty m_ungroundedEvent { set; get; } = new();
        public float m_footPlantedAnkleHeightMS { set; get; }
        public float m_footRaisedAnkleHeightMS { set; get; }
        public float m_maxAnkleHeightMS { set; get; }
        public float m_minAnkleHeightMS { set; get; }
        public float m_maxKneeAngleDegrees { set; get; }
        public float m_minKneeAngleDegrees { set; get; }
        public float m_verticalError { set; get; }
        public float m_maxAnkleAngleDegrees { set; get; }
        public short m_hipIndex { set; get; }
        public short m_kneeIndex { set; get; }
        public short m_ankleIndex { set; get; }
        public bool m_hitSomething { set; get; }
        public bool m_isPlantedMS { set; get; }
        public bool m_isOriginalAnkleTransformMSSet { set; get; }

        public virtual uint Signature { set; get; } = 0x9f3e3a04;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_originalAnkleTransformMS = des.ReadQSTransform(br);
            m_prevAnkleRotLS = des.ReadQuaternion(br);
            m_kneeAxisLS = br.ReadVector4();
            m_footEndLS = br.ReadVector4();
            m_ungroundedEvent.Read(des, br);
            m_footPlantedAnkleHeightMS = br.ReadSingle();
            m_footRaisedAnkleHeightMS = br.ReadSingle();
            m_maxAnkleHeightMS = br.ReadSingle();
            m_minAnkleHeightMS = br.ReadSingle();
            m_maxKneeAngleDegrees = br.ReadSingle();
            m_minKneeAngleDegrees = br.ReadSingle();
            m_verticalError = br.ReadSingle();
            m_maxAnkleAngleDegrees = br.ReadSingle();
            m_hipIndex = br.ReadInt16();
            m_kneeIndex = br.ReadInt16();
            m_ankleIndex = br.ReadInt16();
            m_hitSomething = br.ReadBoolean();
            m_isPlantedMS = br.ReadBoolean();
            m_isOriginalAnkleTransformMSSet = br.ReadBoolean();
            br.Position += 7;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteQSTransform(bw, m_originalAnkleTransformMS);
            s.WriteQuaternion(bw, m_prevAnkleRotLS);
            bw.WriteVector4(m_kneeAxisLS);
            bw.WriteVector4(m_footEndLS);
            m_ungroundedEvent.Write(s, bw);
            bw.WriteSingle(m_footPlantedAnkleHeightMS);
            bw.WriteSingle(m_footRaisedAnkleHeightMS);
            bw.WriteSingle(m_maxAnkleHeightMS);
            bw.WriteSingle(m_minAnkleHeightMS);
            bw.WriteSingle(m_maxKneeAngleDegrees);
            bw.WriteSingle(m_minKneeAngleDegrees);
            bw.WriteSingle(m_verticalError);
            bw.WriteSingle(m_maxAnkleAngleDegrees);
            bw.WriteInt16(m_hipIndex);
            bw.WriteInt16(m_kneeIndex);
            bw.WriteInt16(m_ankleIndex);
            bw.WriteBoolean(m_hitSomething);
            bw.WriteBoolean(m_isPlantedMS);
            bw.WriteBoolean(m_isOriginalAnkleTransformMSSet);
            bw.Position += 7;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_originalAnkleTransformMS = xd.ReadQSTransform(xe, nameof(m_originalAnkleTransformMS));
            m_kneeAxisLS = xd.ReadVector4(xe, nameof(m_kneeAxisLS));
            m_footEndLS = xd.ReadVector4(xe, nameof(m_footEndLS));
            m_ungroundedEvent = xd.ReadClass<hkbEventProperty>(xe, nameof(m_ungroundedEvent));
            m_footPlantedAnkleHeightMS = xd.ReadSingle(xe, nameof(m_footPlantedAnkleHeightMS));
            m_footRaisedAnkleHeightMS = xd.ReadSingle(xe, nameof(m_footRaisedAnkleHeightMS));
            m_maxAnkleHeightMS = xd.ReadSingle(xe, nameof(m_maxAnkleHeightMS));
            m_minAnkleHeightMS = xd.ReadSingle(xe, nameof(m_minAnkleHeightMS));
            m_maxKneeAngleDegrees = xd.ReadSingle(xe, nameof(m_maxKneeAngleDegrees));
            m_minKneeAngleDegrees = xd.ReadSingle(xe, nameof(m_minKneeAngleDegrees));
            m_verticalError = xd.ReadSingle(xe, nameof(m_verticalError));
            m_maxAnkleAngleDegrees = xd.ReadSingle(xe, nameof(m_maxAnkleAngleDegrees));
            m_hipIndex = xd.ReadInt16(xe, nameof(m_hipIndex));
            m_kneeIndex = xd.ReadInt16(xe, nameof(m_kneeIndex));
            m_ankleIndex = xd.ReadInt16(xe, nameof(m_ankleIndex));
            m_hitSomething = xd.ReadBoolean(xe, nameof(m_hitSomething));
            m_isPlantedMS = xd.ReadBoolean(xe, nameof(m_isPlantedMS));
            m_isOriginalAnkleTransformMSSet = xd.ReadBoolean(xe, nameof(m_isOriginalAnkleTransformMSSet));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteQSTransform(xe, nameof(m_originalAnkleTransformMS), m_originalAnkleTransformMS);
            xs.WriteSerializeIgnored(xe, nameof(m_prevAnkleRotLS));
            xs.WriteVector4(xe, nameof(m_kneeAxisLS), m_kneeAxisLS);
            xs.WriteVector4(xe, nameof(m_footEndLS), m_footEndLS);
            xs.WriteClass<hkbEventProperty>(xe, nameof(m_ungroundedEvent), m_ungroundedEvent);
            xs.WriteFloat(xe, nameof(m_footPlantedAnkleHeightMS), m_footPlantedAnkleHeightMS);
            xs.WriteFloat(xe, nameof(m_footRaisedAnkleHeightMS), m_footRaisedAnkleHeightMS);
            xs.WriteFloat(xe, nameof(m_maxAnkleHeightMS), m_maxAnkleHeightMS);
            xs.WriteFloat(xe, nameof(m_minAnkleHeightMS), m_minAnkleHeightMS);
            xs.WriteFloat(xe, nameof(m_maxKneeAngleDegrees), m_maxKneeAngleDegrees);
            xs.WriteFloat(xe, nameof(m_minKneeAngleDegrees), m_minKneeAngleDegrees);
            xs.WriteFloat(xe, nameof(m_verticalError), m_verticalError);
            xs.WriteFloat(xe, nameof(m_maxAnkleAngleDegrees), m_maxAnkleAngleDegrees);
            xs.WriteNumber(xe, nameof(m_hipIndex), m_hipIndex);
            xs.WriteNumber(xe, nameof(m_kneeIndex), m_kneeIndex);
            xs.WriteNumber(xe, nameof(m_ankleIndex), m_ankleIndex);
            xs.WriteBoolean(xe, nameof(m_hitSomething), m_hitSomething);
            xs.WriteBoolean(xe, nameof(m_isPlantedMS), m_isPlantedMS);
            xs.WriteBoolean(xe, nameof(m_isOriginalAnkleTransformMSSet), m_isOriginalAnkleTransformMSSet);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbFootIkModifierLeg);
        }

        public bool Equals(hkbFootIkModifierLeg? other)
        {
            return other is not null &&
                   m_originalAnkleTransformMS.Equals(other.m_originalAnkleTransformMS) &&
                   m_kneeAxisLS.Equals(other.m_kneeAxisLS) &&
                   m_footEndLS.Equals(other.m_footEndLS) &&
                   ((m_ungroundedEvent is null && other.m_ungroundedEvent is null) || (m_ungroundedEvent is not null && other.m_ungroundedEvent is not null && m_ungroundedEvent.Equals((IHavokObject)other.m_ungroundedEvent))) &&
                   m_footPlantedAnkleHeightMS.Equals(other.m_footPlantedAnkleHeightMS) &&
                   m_footRaisedAnkleHeightMS.Equals(other.m_footRaisedAnkleHeightMS) &&
                   m_maxAnkleHeightMS.Equals(other.m_maxAnkleHeightMS) &&
                   m_minAnkleHeightMS.Equals(other.m_minAnkleHeightMS) &&
                   m_maxKneeAngleDegrees.Equals(other.m_maxKneeAngleDegrees) &&
                   m_minKneeAngleDegrees.Equals(other.m_minKneeAngleDegrees) &&
                   m_verticalError.Equals(other.m_verticalError) &&
                   m_maxAnkleAngleDegrees.Equals(other.m_maxAnkleAngleDegrees) &&
                   m_hipIndex.Equals(other.m_hipIndex) &&
                   m_kneeIndex.Equals(other.m_kneeIndex) &&
                   m_ankleIndex.Equals(other.m_ankleIndex) &&
                   m_hitSomething.Equals(other.m_hitSomething) &&
                   m_isPlantedMS.Equals(other.m_isPlantedMS) &&
                   m_isOriginalAnkleTransformMSSet.Equals(other.m_isOriginalAnkleTransformMSSet) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_originalAnkleTransformMS);
            hashcode.Add(m_kneeAxisLS);
            hashcode.Add(m_footEndLS);
            hashcode.Add(m_ungroundedEvent);
            hashcode.Add(m_footPlantedAnkleHeightMS);
            hashcode.Add(m_footRaisedAnkleHeightMS);
            hashcode.Add(m_maxAnkleHeightMS);
            hashcode.Add(m_minAnkleHeightMS);
            hashcode.Add(m_maxKneeAngleDegrees);
            hashcode.Add(m_minKneeAngleDegrees);
            hashcode.Add(m_verticalError);
            hashcode.Add(m_maxAnkleAngleDegrees);
            hashcode.Add(m_hipIndex);
            hashcode.Add(m_kneeIndex);
            hashcode.Add(m_ankleIndex);
            hashcode.Add(m_hitSomething);
            hashcode.Add(m_isPlantedMS);
            hashcode.Add(m_isOriginalAnkleTransformMSSet);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

