using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbSenseHandleModifier Signatire: 0x2a064d99 size: 224 flags: FLAGS_NONE

    // m_handle m_class: hkbHandle Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 80 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_sensorLocalOffset m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_ranges m_class: hkbSenseHandleModifierRange Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_handleOut m_class: hkbHandle Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_handleIn m_class: hkbHandle Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 168 flags: FLAGS_NONE enum: 
    // m_localFrameName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    // m_sensorLocalFrameName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 184 flags: FLAGS_NONE enum: 
    // m_minDistance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 192 flags: FLAGS_NONE enum: 
    // m_maxDistance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 196 flags: FLAGS_NONE enum: 
    // m_distanceOut m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 200 flags: FLAGS_NONE enum: 
    // m_collisionFilterInfo m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 204 flags: FLAGS_NONE enum: 
    // m_sensorRagdollBoneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 208 flags: FLAGS_NONE enum: 
    // m_sensorAnimationBoneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 210 flags: FLAGS_NONE enum: 
    // m_sensingMode m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 212 flags: FLAGS_NONE enum: SensingMode
    // m_extrapolateSensorPosition m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 213 flags: FLAGS_NONE enum: 
    // m_keepFirstSensedHandle m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 214 flags: FLAGS_NONE enum: 
    // m_foundHandleOut m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 215 flags: FLAGS_NONE enum: 
    // m_timeSinceLastModify m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 216 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_rangeIndexForEventToSendNextUpdate m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 220 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbSenseHandleModifier : hkbModifier, IEquatable<hkbSenseHandleModifier?>
    {
        public hkbHandle m_handle { set; get; } = new();
        public Vector4 m_sensorLocalOffset { set; get; }
        public IList<hkbSenseHandleModifierRange> m_ranges { set; get; } = Array.Empty<hkbSenseHandleModifierRange>();
        public hkbHandle? m_handleOut { set; get; }
        public hkbHandle? m_handleIn { set; get; }
        public string m_localFrameName { set; get; } = "";
        public string m_sensorLocalFrameName { set; get; } = "";
        public float m_minDistance { set; get; }
        public float m_maxDistance { set; get; }
        public float m_distanceOut { set; get; }
        public uint m_collisionFilterInfo { set; get; }
        public short m_sensorRagdollBoneIndex { set; get; }
        public short m_sensorAnimationBoneIndex { set; get; }
        public sbyte m_sensingMode { set; get; }
        public bool m_extrapolateSensorPosition { set; get; }
        public bool m_keepFirstSensedHandle { set; get; }
        public bool m_foundHandleOut { set; get; }
        private float m_timeSinceLastModify { set; get; }
        private int m_rangeIndexForEventToSendNextUpdate { set; get; }

        public override uint Signature { set; get; } = 0x2a064d99;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_handle.Read(des, br);
            m_sensorLocalOffset = br.ReadVector4();
            m_ranges = des.ReadClassArray<hkbSenseHandleModifierRange>(br);
            m_handleOut = des.ReadClassPointer<hkbHandle>(br);
            m_handleIn = des.ReadClassPointer<hkbHandle>(br);
            m_localFrameName = des.ReadStringPointer(br);
            m_sensorLocalFrameName = des.ReadStringPointer(br);
            m_minDistance = br.ReadSingle();
            m_maxDistance = br.ReadSingle();
            m_distanceOut = br.ReadSingle();
            m_collisionFilterInfo = br.ReadUInt32();
            m_sensorRagdollBoneIndex = br.ReadInt16();
            m_sensorAnimationBoneIndex = br.ReadInt16();
            m_sensingMode = br.ReadSByte();
            m_extrapolateSensorPosition = br.ReadBoolean();
            m_keepFirstSensedHandle = br.ReadBoolean();
            m_foundHandleOut = br.ReadBoolean();
            m_timeSinceLastModify = br.ReadSingle();
            m_rangeIndexForEventToSendNextUpdate = br.ReadInt32();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_handle.Write(s, bw);
            bw.WriteVector4(m_sensorLocalOffset);
            s.WriteClassArray(bw, m_ranges);
            s.WriteClassPointer(bw, m_handleOut);
            s.WriteClassPointer(bw, m_handleIn);
            s.WriteStringPointer(bw, m_localFrameName);
            s.WriteStringPointer(bw, m_sensorLocalFrameName);
            bw.WriteSingle(m_minDistance);
            bw.WriteSingle(m_maxDistance);
            bw.WriteSingle(m_distanceOut);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.WriteInt16(m_sensorRagdollBoneIndex);
            bw.WriteInt16(m_sensorAnimationBoneIndex);
            bw.WriteSByte(m_sensingMode);
            bw.WriteBoolean(m_extrapolateSensorPosition);
            bw.WriteBoolean(m_keepFirstSensedHandle);
            bw.WriteBoolean(m_foundHandleOut);
            bw.WriteSingle(m_timeSinceLastModify);
            bw.WriteInt32(m_rangeIndexForEventToSendNextUpdate);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_sensorLocalOffset = xd.ReadVector4(xe, nameof(m_sensorLocalOffset));
            m_ranges = xd.ReadClassArray<hkbSenseHandleModifierRange>(xe, nameof(m_ranges));
            m_handleOut = xd.ReadClassPointer<hkbHandle>(xe, nameof(m_handleOut));
            m_handleIn = xd.ReadClassPointer<hkbHandle>(xe, nameof(m_handleIn));
            m_localFrameName = xd.ReadString(xe, nameof(m_localFrameName));
            m_sensorLocalFrameName = xd.ReadString(xe, nameof(m_sensorLocalFrameName));
            m_minDistance = xd.ReadSingle(xe, nameof(m_minDistance));
            m_maxDistance = xd.ReadSingle(xe, nameof(m_maxDistance));
            m_distanceOut = xd.ReadSingle(xe, nameof(m_distanceOut));
            m_collisionFilterInfo = xd.ReadUInt32(xe, nameof(m_collisionFilterInfo));
            m_sensorRagdollBoneIndex = xd.ReadInt16(xe, nameof(m_sensorRagdollBoneIndex));
            m_sensorAnimationBoneIndex = xd.ReadInt16(xe, nameof(m_sensorAnimationBoneIndex));
            m_sensingMode = xd.ReadFlag<SensingMode, sbyte>(xe, nameof(m_sensingMode));
            m_extrapolateSensorPosition = xd.ReadBoolean(xe, nameof(m_extrapolateSensorPosition));
            m_keepFirstSensedHandle = xd.ReadBoolean(xe, nameof(m_keepFirstSensedHandle));
            m_foundHandleOut = xd.ReadBoolean(xe, nameof(m_foundHandleOut));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteSerializeIgnored(xe, nameof(m_handle));
            xs.WriteVector4(xe, nameof(m_sensorLocalOffset), m_sensorLocalOffset);
            xs.WriteClassArray(xe, nameof(m_ranges), m_ranges);
            xs.WriteClassPointer(xe, nameof(m_handleOut), m_handleOut);
            xs.WriteClassPointer(xe, nameof(m_handleIn), m_handleIn);
            xs.WriteString(xe, nameof(m_localFrameName), m_localFrameName);
            xs.WriteString(xe, nameof(m_sensorLocalFrameName), m_sensorLocalFrameName);
            xs.WriteFloat(xe, nameof(m_minDistance), m_minDistance);
            xs.WriteFloat(xe, nameof(m_maxDistance), m_maxDistance);
            xs.WriteFloat(xe, nameof(m_distanceOut), m_distanceOut);
            xs.WriteNumber(xe, nameof(m_collisionFilterInfo), m_collisionFilterInfo);
            xs.WriteNumber(xe, nameof(m_sensorRagdollBoneIndex), m_sensorRagdollBoneIndex);
            xs.WriteNumber(xe, nameof(m_sensorAnimationBoneIndex), m_sensorAnimationBoneIndex);
            xs.WriteEnum<SensingMode, sbyte>(xe, nameof(m_sensingMode), m_sensingMode);
            xs.WriteBoolean(xe, nameof(m_extrapolateSensorPosition), m_extrapolateSensorPosition);
            xs.WriteBoolean(xe, nameof(m_keepFirstSensedHandle), m_keepFirstSensedHandle);
            xs.WriteBoolean(xe, nameof(m_foundHandleOut), m_foundHandleOut);
            xs.WriteSerializeIgnored(xe, nameof(m_timeSinceLastModify));
            xs.WriteSerializeIgnored(xe, nameof(m_rangeIndexForEventToSendNextUpdate));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbSenseHandleModifier);
        }

        public bool Equals(hkbSenseHandleModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_sensorLocalOffset.Equals(other.m_sensorLocalOffset) &&
                   m_ranges.SequenceEqual(other.m_ranges) &&
                   ((m_handleOut is null && other.m_handleOut is null) || (m_handleOut is not null && other.m_handleOut is not null && m_handleOut.Equals((IHavokObject)other.m_handleOut))) &&
                   ((m_handleIn is null && other.m_handleIn is null) || (m_handleIn is not null && other.m_handleIn is not null && m_handleIn.Equals((IHavokObject)other.m_handleIn))) &&
                   (m_localFrameName is null && other.m_localFrameName is null || m_localFrameName == other.m_localFrameName || m_localFrameName is null && other.m_localFrameName == "" || m_localFrameName == "" && other.m_localFrameName is null) &&
                   (m_sensorLocalFrameName is null && other.m_sensorLocalFrameName is null || m_sensorLocalFrameName == other.m_sensorLocalFrameName || m_sensorLocalFrameName is null && other.m_sensorLocalFrameName == "" || m_sensorLocalFrameName == "" && other.m_sensorLocalFrameName is null) &&
                   m_minDistance.Equals(other.m_minDistance) &&
                   m_maxDistance.Equals(other.m_maxDistance) &&
                   m_distanceOut.Equals(other.m_distanceOut) &&
                   m_collisionFilterInfo.Equals(other.m_collisionFilterInfo) &&
                   m_sensorRagdollBoneIndex.Equals(other.m_sensorRagdollBoneIndex) &&
                   m_sensorAnimationBoneIndex.Equals(other.m_sensorAnimationBoneIndex) &&
                   m_sensingMode.Equals(other.m_sensingMode) &&
                   m_extrapolateSensorPosition.Equals(other.m_extrapolateSensorPosition) &&
                   m_keepFirstSensedHandle.Equals(other.m_keepFirstSensedHandle) &&
                   m_foundHandleOut.Equals(other.m_foundHandleOut) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_sensorLocalOffset);
            hashcode.Add(m_ranges.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_handleOut);
            hashcode.Add(m_handleIn);
            hashcode.Add(m_localFrameName);
            hashcode.Add(m_sensorLocalFrameName);
            hashcode.Add(m_minDistance);
            hashcode.Add(m_maxDistance);
            hashcode.Add(m_distanceOut);
            hashcode.Add(m_collisionFilterInfo);
            hashcode.Add(m_sensorRagdollBoneIndex);
            hashcode.Add(m_sensorAnimationBoneIndex);
            hashcode.Add(m_sensingMode);
            hashcode.Add(m_extrapolateSensorPosition);
            hashcode.Add(m_keepFirstSensedHandle);
            hashcode.Add(m_foundHandleOut);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

