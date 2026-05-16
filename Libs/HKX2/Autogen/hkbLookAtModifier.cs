using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbLookAtModifier Signatire: 0x3d28e066 size: 240 flags: FLAGS_NONE

    // m_targetWS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_headForwardLS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_neckForwardLS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_neckRightLS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_eyePositionHS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_newTargetGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_onGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 164 flags: FLAGS_NONE enum: 
    // m_offGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 168 flags: FLAGS_NONE enum: 
    // m_limitAngleDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 172 flags: FLAGS_NONE enum: 
    // m_limitAngleLeft m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    // m_limitAngleRight m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 180 flags: FLAGS_NONE enum: 
    // m_limitAngleUp m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 184 flags: FLAGS_NONE enum: 
    // m_limitAngleDown m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 188 flags: FLAGS_NONE enum: 
    // m_headIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 192 flags: FLAGS_NONE enum: 
    // m_neckIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 194 flags: FLAGS_NONE enum: 
    // m_isOn m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 196 flags: FLAGS_NONE enum: 
    // m_individualLimitsOn m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 197 flags: FLAGS_NONE enum: 
    // m_isTargetInsideLimitCone m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 198 flags: FLAGS_NONE enum: 
    // m_lookAtLastTargetWS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 208 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_lookAtWeight m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 224 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbLookAtModifier : hkbModifier, IEquatable<hkbLookAtModifier?>
    {
        public Vector4 m_targetWS { set; get; }
        public Vector4 m_headForwardLS { set; get; }
        public Vector4 m_neckForwardLS { set; get; }
        public Vector4 m_neckRightLS { set; get; }
        public Vector4 m_eyePositionHS { set; get; }
        public float m_newTargetGain { set; get; }
        public float m_onGain { set; get; }
        public float m_offGain { set; get; }
        public float m_limitAngleDegrees { set; get; }
        public float m_limitAngleLeft { set; get; }
        public float m_limitAngleRight { set; get; }
        public float m_limitAngleUp { set; get; }
        public float m_limitAngleDown { set; get; }
        public short m_headIndex { set; get; }
        public short m_neckIndex { set; get; }
        public bool m_isOn { set; get; }
        public bool m_individualLimitsOn { set; get; }
        public bool m_isTargetInsideLimitCone { set; get; }
        private Vector4 m_lookAtLastTargetWS { set; get; }
        private float m_lookAtWeight { set; get; }

        public override uint Signature { set; get; } = 0x3d28e066;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_targetWS = br.ReadVector4();
            m_headForwardLS = br.ReadVector4();
            m_neckForwardLS = br.ReadVector4();
            m_neckRightLS = br.ReadVector4();
            m_eyePositionHS = br.ReadVector4();
            m_newTargetGain = br.ReadSingle();
            m_onGain = br.ReadSingle();
            m_offGain = br.ReadSingle();
            m_limitAngleDegrees = br.ReadSingle();
            m_limitAngleLeft = br.ReadSingle();
            m_limitAngleRight = br.ReadSingle();
            m_limitAngleUp = br.ReadSingle();
            m_limitAngleDown = br.ReadSingle();
            m_headIndex = br.ReadInt16();
            m_neckIndex = br.ReadInt16();
            m_isOn = br.ReadBoolean();
            m_individualLimitsOn = br.ReadBoolean();
            m_isTargetInsideLimitCone = br.ReadBoolean();
            br.Position += 9;
            m_lookAtLastTargetWS = br.ReadVector4();
            m_lookAtWeight = br.ReadSingle();
            br.Position += 12;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_targetWS);
            bw.WriteVector4(m_headForwardLS);
            bw.WriteVector4(m_neckForwardLS);
            bw.WriteVector4(m_neckRightLS);
            bw.WriteVector4(m_eyePositionHS);
            bw.WriteSingle(m_newTargetGain);
            bw.WriteSingle(m_onGain);
            bw.WriteSingle(m_offGain);
            bw.WriteSingle(m_limitAngleDegrees);
            bw.WriteSingle(m_limitAngleLeft);
            bw.WriteSingle(m_limitAngleRight);
            bw.WriteSingle(m_limitAngleUp);
            bw.WriteSingle(m_limitAngleDown);
            bw.WriteInt16(m_headIndex);
            bw.WriteInt16(m_neckIndex);
            bw.WriteBoolean(m_isOn);
            bw.WriteBoolean(m_individualLimitsOn);
            bw.WriteBoolean(m_isTargetInsideLimitCone);
            bw.Position += 9;
            bw.WriteVector4(m_lookAtLastTargetWS);
            bw.WriteSingle(m_lookAtWeight);
            bw.Position += 12;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_targetWS = xd.ReadVector4(xe, nameof(m_targetWS));
            m_headForwardLS = xd.ReadVector4(xe, nameof(m_headForwardLS));
            m_neckForwardLS = xd.ReadVector4(xe, nameof(m_neckForwardLS));
            m_neckRightLS = xd.ReadVector4(xe, nameof(m_neckRightLS));
            m_eyePositionHS = xd.ReadVector4(xe, nameof(m_eyePositionHS));
            m_newTargetGain = xd.ReadSingle(xe, nameof(m_newTargetGain));
            m_onGain = xd.ReadSingle(xe, nameof(m_onGain));
            m_offGain = xd.ReadSingle(xe, nameof(m_offGain));
            m_limitAngleDegrees = xd.ReadSingle(xe, nameof(m_limitAngleDegrees));
            m_limitAngleLeft = xd.ReadSingle(xe, nameof(m_limitAngleLeft));
            m_limitAngleRight = xd.ReadSingle(xe, nameof(m_limitAngleRight));
            m_limitAngleUp = xd.ReadSingle(xe, nameof(m_limitAngleUp));
            m_limitAngleDown = xd.ReadSingle(xe, nameof(m_limitAngleDown));
            m_headIndex = xd.ReadInt16(xe, nameof(m_headIndex));
            m_neckIndex = xd.ReadInt16(xe, nameof(m_neckIndex));
            m_isOn = xd.ReadBoolean(xe, nameof(m_isOn));
            m_individualLimitsOn = xd.ReadBoolean(xe, nameof(m_individualLimitsOn));
            m_isTargetInsideLimitCone = xd.ReadBoolean(xe, nameof(m_isTargetInsideLimitCone));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_targetWS), m_targetWS);
            xs.WriteVector4(xe, nameof(m_headForwardLS), m_headForwardLS);
            xs.WriteVector4(xe, nameof(m_neckForwardLS), m_neckForwardLS);
            xs.WriteVector4(xe, nameof(m_neckRightLS), m_neckRightLS);
            xs.WriteVector4(xe, nameof(m_eyePositionHS), m_eyePositionHS);
            xs.WriteFloat(xe, nameof(m_newTargetGain), m_newTargetGain);
            xs.WriteFloat(xe, nameof(m_onGain), m_onGain);
            xs.WriteFloat(xe, nameof(m_offGain), m_offGain);
            xs.WriteFloat(xe, nameof(m_limitAngleDegrees), m_limitAngleDegrees);
            xs.WriteFloat(xe, nameof(m_limitAngleLeft), m_limitAngleLeft);
            xs.WriteFloat(xe, nameof(m_limitAngleRight), m_limitAngleRight);
            xs.WriteFloat(xe, nameof(m_limitAngleUp), m_limitAngleUp);
            xs.WriteFloat(xe, nameof(m_limitAngleDown), m_limitAngleDown);
            xs.WriteNumber(xe, nameof(m_headIndex), m_headIndex);
            xs.WriteNumber(xe, nameof(m_neckIndex), m_neckIndex);
            xs.WriteBoolean(xe, nameof(m_isOn), m_isOn);
            xs.WriteBoolean(xe, nameof(m_individualLimitsOn), m_individualLimitsOn);
            xs.WriteBoolean(xe, nameof(m_isTargetInsideLimitCone), m_isTargetInsideLimitCone);
            xs.WriteSerializeIgnored(xe, nameof(m_lookAtLastTargetWS));
            xs.WriteSerializeIgnored(xe, nameof(m_lookAtWeight));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbLookAtModifier);
        }

        public bool Equals(hkbLookAtModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_targetWS.Equals(other.m_targetWS) &&
                   m_headForwardLS.Equals(other.m_headForwardLS) &&
                   m_neckForwardLS.Equals(other.m_neckForwardLS) &&
                   m_neckRightLS.Equals(other.m_neckRightLS) &&
                   m_eyePositionHS.Equals(other.m_eyePositionHS) &&
                   m_newTargetGain.Equals(other.m_newTargetGain) &&
                   m_onGain.Equals(other.m_onGain) &&
                   m_offGain.Equals(other.m_offGain) &&
                   m_limitAngleDegrees.Equals(other.m_limitAngleDegrees) &&
                   m_limitAngleLeft.Equals(other.m_limitAngleLeft) &&
                   m_limitAngleRight.Equals(other.m_limitAngleRight) &&
                   m_limitAngleUp.Equals(other.m_limitAngleUp) &&
                   m_limitAngleDown.Equals(other.m_limitAngleDown) &&
                   m_headIndex.Equals(other.m_headIndex) &&
                   m_neckIndex.Equals(other.m_neckIndex) &&
                   m_isOn.Equals(other.m_isOn) &&
                   m_individualLimitsOn.Equals(other.m_individualLimitsOn) &&
                   m_isTargetInsideLimitCone.Equals(other.m_isTargetInsideLimitCone) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_targetWS);
            hashcode.Add(m_headForwardLS);
            hashcode.Add(m_neckForwardLS);
            hashcode.Add(m_neckRightLS);
            hashcode.Add(m_eyePositionHS);
            hashcode.Add(m_newTargetGain);
            hashcode.Add(m_onGain);
            hashcode.Add(m_offGain);
            hashcode.Add(m_limitAngleDegrees);
            hashcode.Add(m_limitAngleLeft);
            hashcode.Add(m_limitAngleRight);
            hashcode.Add(m_limitAngleUp);
            hashcode.Add(m_limitAngleDown);
            hashcode.Add(m_headIndex);
            hashcode.Add(m_neckIndex);
            hashcode.Add(m_isOn);
            hashcode.Add(m_individualLimitsOn);
            hashcode.Add(m_isTargetInsideLimitCone);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

