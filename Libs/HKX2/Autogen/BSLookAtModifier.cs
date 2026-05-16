using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // BSLookAtModifier Signatire: 0xd756fc25 size: 224 flags: FLAGS_NONE

    // m_lookAtTarget m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_bones m_class: BSLookAtModifierBoneData Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_eyeBones m_class: BSLookAtModifierBoneData Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_limitAngleDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 120 flags: FLAGS_NONE enum: 
    // m_limitAngleThresholdDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 124 flags: FLAGS_NONE enum: 
    // m_continueLookOutsideOfLimit m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_onGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 132 flags: FLAGS_NONE enum: 
    // m_offGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 136 flags: FLAGS_NONE enum: 
    // m_useBoneGains m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 140 flags: FLAGS_NONE enum: 
    // m_targetLocation m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_targetOutsideLimits m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_targetOutOfLimitEvent m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 168 flags: FLAGS_NONE enum: 
    // m_lookAtCamera m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 184 flags: FLAGS_NONE enum: 
    // m_lookAtCameraX m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 188 flags: FLAGS_NONE enum: 
    // m_lookAtCameraY m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 192 flags: FLAGS_NONE enum: 
    // m_lookAtCameraZ m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 196 flags: FLAGS_NONE enum: 
    // m_timeStep m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 200 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_ballBonesValid m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 204 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_pSkeletonMemory m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 208 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class BSLookAtModifier : hkbModifier, IEquatable<BSLookAtModifier?>
    {
        public bool m_lookAtTarget { set; get; }
        public IList<BSLookAtModifierBoneData> m_bones { set; get; } = Array.Empty<BSLookAtModifierBoneData>();
        public IList<BSLookAtModifierBoneData> m_eyeBones { set; get; } = Array.Empty<BSLookAtModifierBoneData>();
        public float m_limitAngleDegrees { set; get; }
        public float m_limitAngleThresholdDegrees { set; get; }
        public bool m_continueLookOutsideOfLimit { set; get; }
        public float m_onGain { set; get; }
        public float m_offGain { set; get; }
        public bool m_useBoneGains { set; get; }
        public Vector4 m_targetLocation { set; get; }
        public bool m_targetOutsideLimits { set; get; }
        public hkbEventProperty m_targetOutOfLimitEvent { set; get; } = new();
        public bool m_lookAtCamera { set; get; }
        public float m_lookAtCameraX { set; get; }
        public float m_lookAtCameraY { set; get; }
        public float m_lookAtCameraZ { set; get; }
        private float m_timeStep { set; get; }
        private bool m_ballBonesValid { set; get; }
        private object? m_pSkeletonMemory { set; get; }

        public override uint Signature { set; get; } = 0xd756fc25;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_lookAtTarget = br.ReadBoolean();
            br.Position += 7;
            m_bones = des.ReadClassArray<BSLookAtModifierBoneData>(br);
            m_eyeBones = des.ReadClassArray<BSLookAtModifierBoneData>(br);
            m_limitAngleDegrees = br.ReadSingle();
            m_limitAngleThresholdDegrees = br.ReadSingle();
            m_continueLookOutsideOfLimit = br.ReadBoolean();
            br.Position += 3;
            m_onGain = br.ReadSingle();
            m_offGain = br.ReadSingle();
            m_useBoneGains = br.ReadBoolean();
            br.Position += 3;
            m_targetLocation = br.ReadVector4();
            m_targetOutsideLimits = br.ReadBoolean();
            br.Position += 7;
            m_targetOutOfLimitEvent.Read(des, br);
            m_lookAtCamera = br.ReadBoolean();
            br.Position += 3;
            m_lookAtCameraX = br.ReadSingle();
            m_lookAtCameraY = br.ReadSingle();
            m_lookAtCameraZ = br.ReadSingle();
            m_timeStep = br.ReadSingle();
            m_ballBonesValid = br.ReadBoolean();
            br.Position += 3;
            des.ReadEmptyPointer(br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteBoolean(m_lookAtTarget);
            bw.Position += 7;
            s.WriteClassArray(bw, m_bones);
            s.WriteClassArray(bw, m_eyeBones);
            bw.WriteSingle(m_limitAngleDegrees);
            bw.WriteSingle(m_limitAngleThresholdDegrees);
            bw.WriteBoolean(m_continueLookOutsideOfLimit);
            bw.Position += 3;
            bw.WriteSingle(m_onGain);
            bw.WriteSingle(m_offGain);
            bw.WriteBoolean(m_useBoneGains);
            bw.Position += 3;
            bw.WriteVector4(m_targetLocation);
            bw.WriteBoolean(m_targetOutsideLimits);
            bw.Position += 7;
            m_targetOutOfLimitEvent.Write(s, bw);
            bw.WriteBoolean(m_lookAtCamera);
            bw.Position += 3;
            bw.WriteSingle(m_lookAtCameraX);
            bw.WriteSingle(m_lookAtCameraY);
            bw.WriteSingle(m_lookAtCameraZ);
            bw.WriteSingle(m_timeStep);
            bw.WriteBoolean(m_ballBonesValid);
            bw.Position += 3;
            s.WriteVoidPointer(bw);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_lookAtTarget = xd.ReadBoolean(xe, nameof(m_lookAtTarget));
            m_bones = xd.ReadClassArray<BSLookAtModifierBoneData>(xe, nameof(m_bones));
            m_eyeBones = xd.ReadClassArray<BSLookAtModifierBoneData>(xe, nameof(m_eyeBones));
            m_limitAngleDegrees = xd.ReadSingle(xe, nameof(m_limitAngleDegrees));
            m_limitAngleThresholdDegrees = xd.ReadSingle(xe, nameof(m_limitAngleThresholdDegrees));
            m_continueLookOutsideOfLimit = xd.ReadBoolean(xe, nameof(m_continueLookOutsideOfLimit));
            m_onGain = xd.ReadSingle(xe, nameof(m_onGain));
            m_offGain = xd.ReadSingle(xe, nameof(m_offGain));
            m_useBoneGains = xd.ReadBoolean(xe, nameof(m_useBoneGains));
            m_targetLocation = xd.ReadVector4(xe, nameof(m_targetLocation));
            m_targetOutsideLimits = xd.ReadBoolean(xe, nameof(m_targetOutsideLimits));
            m_targetOutOfLimitEvent = xd.ReadClass<hkbEventProperty>(xe, nameof(m_targetOutOfLimitEvent));
            m_lookAtCamera = xd.ReadBoolean(xe, nameof(m_lookAtCamera));
            m_lookAtCameraX = xd.ReadSingle(xe, nameof(m_lookAtCameraX));
            m_lookAtCameraY = xd.ReadSingle(xe, nameof(m_lookAtCameraY));
            m_lookAtCameraZ = xd.ReadSingle(xe, nameof(m_lookAtCameraZ));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteBoolean(xe, nameof(m_lookAtTarget), m_lookAtTarget);
            xs.WriteClassArray(xe, nameof(m_bones), m_bones);
            xs.WriteClassArray(xe, nameof(m_eyeBones), m_eyeBones);
            xs.WriteFloat(xe, nameof(m_limitAngleDegrees), m_limitAngleDegrees);
            xs.WriteFloat(xe, nameof(m_limitAngleThresholdDegrees), m_limitAngleThresholdDegrees);
            xs.WriteBoolean(xe, nameof(m_continueLookOutsideOfLimit), m_continueLookOutsideOfLimit);
            xs.WriteFloat(xe, nameof(m_onGain), m_onGain);
            xs.WriteFloat(xe, nameof(m_offGain), m_offGain);
            xs.WriteBoolean(xe, nameof(m_useBoneGains), m_useBoneGains);
            xs.WriteVector4(xe, nameof(m_targetLocation), m_targetLocation);
            xs.WriteBoolean(xe, nameof(m_targetOutsideLimits), m_targetOutsideLimits);
            xs.WriteClass<hkbEventProperty>(xe, nameof(m_targetOutOfLimitEvent), m_targetOutOfLimitEvent);
            xs.WriteBoolean(xe, nameof(m_lookAtCamera), m_lookAtCamera);
            xs.WriteFloat(xe, nameof(m_lookAtCameraX), m_lookAtCameraX);
            xs.WriteFloat(xe, nameof(m_lookAtCameraY), m_lookAtCameraY);
            xs.WriteFloat(xe, nameof(m_lookAtCameraZ), m_lookAtCameraZ);
            xs.WriteSerializeIgnored(xe, nameof(m_timeStep));
            xs.WriteSerializeIgnored(xe, nameof(m_ballBonesValid));
            xs.WriteSerializeIgnored(xe, nameof(m_pSkeletonMemory));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSLookAtModifier);
        }

        public bool Equals(BSLookAtModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_lookAtTarget.Equals(other.m_lookAtTarget) &&
                   m_bones.SequenceEqual(other.m_bones) &&
                   m_eyeBones.SequenceEqual(other.m_eyeBones) &&
                   m_limitAngleDegrees.Equals(other.m_limitAngleDegrees) &&
                   m_limitAngleThresholdDegrees.Equals(other.m_limitAngleThresholdDegrees) &&
                   m_continueLookOutsideOfLimit.Equals(other.m_continueLookOutsideOfLimit) &&
                   m_onGain.Equals(other.m_onGain) &&
                   m_offGain.Equals(other.m_offGain) &&
                   m_useBoneGains.Equals(other.m_useBoneGains) &&
                   m_targetLocation.Equals(other.m_targetLocation) &&
                   m_targetOutsideLimits.Equals(other.m_targetOutsideLimits) &&
                   ((m_targetOutOfLimitEvent is null && other.m_targetOutOfLimitEvent is null) || (m_targetOutOfLimitEvent is not null && other.m_targetOutOfLimitEvent is not null && m_targetOutOfLimitEvent.Equals((IHavokObject)other.m_targetOutOfLimitEvent))) &&
                   m_lookAtCamera.Equals(other.m_lookAtCamera) &&
                   m_lookAtCameraX.Equals(other.m_lookAtCameraX) &&
                   m_lookAtCameraY.Equals(other.m_lookAtCameraY) &&
                   m_lookAtCameraZ.Equals(other.m_lookAtCameraZ) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_lookAtTarget);
            hashcode.Add(m_bones.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_eyeBones.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_limitAngleDegrees);
            hashcode.Add(m_limitAngleThresholdDegrees);
            hashcode.Add(m_continueLookOutsideOfLimit);
            hashcode.Add(m_onGain);
            hashcode.Add(m_offGain);
            hashcode.Add(m_useBoneGains);
            hashcode.Add(m_targetLocation);
            hashcode.Add(m_targetOutsideLimits);
            hashcode.Add(m_targetOutOfLimitEvent);
            hashcode.Add(m_lookAtCamera);
            hashcode.Add(m_lookAtCameraX);
            hashcode.Add(m_lookAtCameraY);
            hashcode.Add(m_lookAtCameraZ);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

