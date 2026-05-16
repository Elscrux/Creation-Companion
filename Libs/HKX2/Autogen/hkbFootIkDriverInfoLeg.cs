using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbFootIkDriverInfoLeg Signatire: 0x224b18d1 size: 96 flags: FLAGS_NONE

    // m_prevAnkleRotLS m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 0 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_kneeAxisLS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_footEndLS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_footPlantedAnkleHeightMS m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_footRaisedAnkleHeightMS m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 52 flags: FLAGS_NONE enum: 
    // m_maxAnkleHeightMS m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_minAnkleHeightMS m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 60 flags: FLAGS_NONE enum: 
    // m_maxKneeAngleDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_minKneeAngleDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 68 flags: FLAGS_NONE enum: 
    // m_maxAnkleAngleDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_hipIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 76 flags: FLAGS_NONE enum: 
    // m_kneeIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 78 flags: FLAGS_NONE enum: 
    // m_ankleIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkbFootIkDriverInfoLeg : IHavokObject, IEquatable<hkbFootIkDriverInfoLeg?>
    {
        private Quaternion m_prevAnkleRotLS { set; get; }
        public Vector4 m_kneeAxisLS { set; get; }
        public Vector4 m_footEndLS { set; get; }
        public float m_footPlantedAnkleHeightMS { set; get; }
        public float m_footRaisedAnkleHeightMS { set; get; }
        public float m_maxAnkleHeightMS { set; get; }
        public float m_minAnkleHeightMS { set; get; }
        public float m_maxKneeAngleDegrees { set; get; }
        public float m_minKneeAngleDegrees { set; get; }
        public float m_maxAnkleAngleDegrees { set; get; }
        public short m_hipIndex { set; get; }
        public short m_kneeIndex { set; get; }
        public short m_ankleIndex { set; get; }

        public virtual uint Signature { set; get; } = 0x224b18d1;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_prevAnkleRotLS = des.ReadQuaternion(br);
            m_kneeAxisLS = br.ReadVector4();
            m_footEndLS = br.ReadVector4();
            m_footPlantedAnkleHeightMS = br.ReadSingle();
            m_footRaisedAnkleHeightMS = br.ReadSingle();
            m_maxAnkleHeightMS = br.ReadSingle();
            m_minAnkleHeightMS = br.ReadSingle();
            m_maxKneeAngleDegrees = br.ReadSingle();
            m_minKneeAngleDegrees = br.ReadSingle();
            m_maxAnkleAngleDegrees = br.ReadSingle();
            m_hipIndex = br.ReadInt16();
            m_kneeIndex = br.ReadInt16();
            m_ankleIndex = br.ReadInt16();
            br.Position += 14;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteQuaternion(bw, m_prevAnkleRotLS);
            bw.WriteVector4(m_kneeAxisLS);
            bw.WriteVector4(m_footEndLS);
            bw.WriteSingle(m_footPlantedAnkleHeightMS);
            bw.WriteSingle(m_footRaisedAnkleHeightMS);
            bw.WriteSingle(m_maxAnkleHeightMS);
            bw.WriteSingle(m_minAnkleHeightMS);
            bw.WriteSingle(m_maxKneeAngleDegrees);
            bw.WriteSingle(m_minKneeAngleDegrees);
            bw.WriteSingle(m_maxAnkleAngleDegrees);
            bw.WriteInt16(m_hipIndex);
            bw.WriteInt16(m_kneeIndex);
            bw.WriteInt16(m_ankleIndex);
            bw.Position += 14;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_kneeAxisLS = xd.ReadVector4(xe, nameof(m_kneeAxisLS));
            m_footEndLS = xd.ReadVector4(xe, nameof(m_footEndLS));
            m_footPlantedAnkleHeightMS = xd.ReadSingle(xe, nameof(m_footPlantedAnkleHeightMS));
            m_footRaisedAnkleHeightMS = xd.ReadSingle(xe, nameof(m_footRaisedAnkleHeightMS));
            m_maxAnkleHeightMS = xd.ReadSingle(xe, nameof(m_maxAnkleHeightMS));
            m_minAnkleHeightMS = xd.ReadSingle(xe, nameof(m_minAnkleHeightMS));
            m_maxKneeAngleDegrees = xd.ReadSingle(xe, nameof(m_maxKneeAngleDegrees));
            m_minKneeAngleDegrees = xd.ReadSingle(xe, nameof(m_minKneeAngleDegrees));
            m_maxAnkleAngleDegrees = xd.ReadSingle(xe, nameof(m_maxAnkleAngleDegrees));
            m_hipIndex = xd.ReadInt16(xe, nameof(m_hipIndex));
            m_kneeIndex = xd.ReadInt16(xe, nameof(m_kneeIndex));
            m_ankleIndex = xd.ReadInt16(xe, nameof(m_ankleIndex));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteSerializeIgnored(xe, nameof(m_prevAnkleRotLS));
            xs.WriteVector4(xe, nameof(m_kneeAxisLS), m_kneeAxisLS);
            xs.WriteVector4(xe, nameof(m_footEndLS), m_footEndLS);
            xs.WriteFloat(xe, nameof(m_footPlantedAnkleHeightMS), m_footPlantedAnkleHeightMS);
            xs.WriteFloat(xe, nameof(m_footRaisedAnkleHeightMS), m_footRaisedAnkleHeightMS);
            xs.WriteFloat(xe, nameof(m_maxAnkleHeightMS), m_maxAnkleHeightMS);
            xs.WriteFloat(xe, nameof(m_minAnkleHeightMS), m_minAnkleHeightMS);
            xs.WriteFloat(xe, nameof(m_maxKneeAngleDegrees), m_maxKneeAngleDegrees);
            xs.WriteFloat(xe, nameof(m_minKneeAngleDegrees), m_minKneeAngleDegrees);
            xs.WriteFloat(xe, nameof(m_maxAnkleAngleDegrees), m_maxAnkleAngleDegrees);
            xs.WriteNumber(xe, nameof(m_hipIndex), m_hipIndex);
            xs.WriteNumber(xe, nameof(m_kneeIndex), m_kneeIndex);
            xs.WriteNumber(xe, nameof(m_ankleIndex), m_ankleIndex);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbFootIkDriverInfoLeg);
        }

        public bool Equals(hkbFootIkDriverInfoLeg? other)
        {
            return other is not null &&
                   m_kneeAxisLS.Equals(other.m_kneeAxisLS) &&
                   m_footEndLS.Equals(other.m_footEndLS) &&
                   m_footPlantedAnkleHeightMS.Equals(other.m_footPlantedAnkleHeightMS) &&
                   m_footRaisedAnkleHeightMS.Equals(other.m_footRaisedAnkleHeightMS) &&
                   m_maxAnkleHeightMS.Equals(other.m_maxAnkleHeightMS) &&
                   m_minAnkleHeightMS.Equals(other.m_minAnkleHeightMS) &&
                   m_maxKneeAngleDegrees.Equals(other.m_maxKneeAngleDegrees) &&
                   m_minKneeAngleDegrees.Equals(other.m_minKneeAngleDegrees) &&
                   m_maxAnkleAngleDegrees.Equals(other.m_maxAnkleAngleDegrees) &&
                   m_hipIndex.Equals(other.m_hipIndex) &&
                   m_kneeIndex.Equals(other.m_kneeIndex) &&
                   m_ankleIndex.Equals(other.m_ankleIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_kneeAxisLS);
            hashcode.Add(m_footEndLS);
            hashcode.Add(m_footPlantedAnkleHeightMS);
            hashcode.Add(m_footRaisedAnkleHeightMS);
            hashcode.Add(m_maxAnkleHeightMS);
            hashcode.Add(m_minAnkleHeightMS);
            hashcode.Add(m_maxKneeAngleDegrees);
            hashcode.Add(m_minKneeAngleDegrees);
            hashcode.Add(m_maxAnkleAngleDegrees);
            hashcode.Add(m_hipIndex);
            hashcode.Add(m_kneeIndex);
            hashcode.Add(m_ankleIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

