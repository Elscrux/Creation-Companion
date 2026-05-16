using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbProxyModifierProxyInfo Signatire: 0x39de637e size: 80 flags: FLAGS_NONE

    // m_dynamicFriction m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_staticFriction m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_keepContactTolerance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_up m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_keepDistance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_contactAngleSensitivity m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_userPlanes m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_maxCharacterSpeedForSolver m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 44 flags: FLAGS_NONE enum: 
    // m_characterStrength m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_characterMass m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 52 flags: FLAGS_NONE enum: 
    // m_maxSlope m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_penetrationRecoverySpeed m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 60 flags: FLAGS_NONE enum: 
    // m_maxCastIterations m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_refreshManifoldInCheckSupport m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 68 flags: FLAGS_NONE enum: 
    public partial class hkbProxyModifierProxyInfo : IHavokObject, IEquatable<hkbProxyModifierProxyInfo?>
    {
        public float m_dynamicFriction { set; get; }
        public float m_staticFriction { set; get; }
        public float m_keepContactTolerance { set; get; }
        public Vector4 m_up { set; get; }
        public float m_keepDistance { set; get; }
        public float m_contactAngleSensitivity { set; get; }
        public uint m_userPlanes { set; get; }
        public float m_maxCharacterSpeedForSolver { set; get; }
        public float m_characterStrength { set; get; }
        public float m_characterMass { set; get; }
        public float m_maxSlope { set; get; }
        public float m_penetrationRecoverySpeed { set; get; }
        public int m_maxCastIterations { set; get; }
        public bool m_refreshManifoldInCheckSupport { set; get; }

        public virtual uint Signature { set; get; } = 0x39de637e;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_dynamicFriction = br.ReadSingle();
            m_staticFriction = br.ReadSingle();
            m_keepContactTolerance = br.ReadSingle();
            br.Position += 4;
            m_up = br.ReadVector4();
            m_keepDistance = br.ReadSingle();
            m_contactAngleSensitivity = br.ReadSingle();
            m_userPlanes = br.ReadUInt32();
            m_maxCharacterSpeedForSolver = br.ReadSingle();
            m_characterStrength = br.ReadSingle();
            m_characterMass = br.ReadSingle();
            m_maxSlope = br.ReadSingle();
            m_penetrationRecoverySpeed = br.ReadSingle();
            m_maxCastIterations = br.ReadInt32();
            m_refreshManifoldInCheckSupport = br.ReadBoolean();
            br.Position += 11;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_dynamicFriction);
            bw.WriteSingle(m_staticFriction);
            bw.WriteSingle(m_keepContactTolerance);
            bw.Position += 4;
            bw.WriteVector4(m_up);
            bw.WriteSingle(m_keepDistance);
            bw.WriteSingle(m_contactAngleSensitivity);
            bw.WriteUInt32(m_userPlanes);
            bw.WriteSingle(m_maxCharacterSpeedForSolver);
            bw.WriteSingle(m_characterStrength);
            bw.WriteSingle(m_characterMass);
            bw.WriteSingle(m_maxSlope);
            bw.WriteSingle(m_penetrationRecoverySpeed);
            bw.WriteInt32(m_maxCastIterations);
            bw.WriteBoolean(m_refreshManifoldInCheckSupport);
            bw.Position += 11;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_dynamicFriction = xd.ReadSingle(xe, nameof(m_dynamicFriction));
            m_staticFriction = xd.ReadSingle(xe, nameof(m_staticFriction));
            m_keepContactTolerance = xd.ReadSingle(xe, nameof(m_keepContactTolerance));
            m_up = xd.ReadVector4(xe, nameof(m_up));
            m_keepDistance = xd.ReadSingle(xe, nameof(m_keepDistance));
            m_contactAngleSensitivity = xd.ReadSingle(xe, nameof(m_contactAngleSensitivity));
            m_userPlanes = xd.ReadUInt32(xe, nameof(m_userPlanes));
            m_maxCharacterSpeedForSolver = xd.ReadSingle(xe, nameof(m_maxCharacterSpeedForSolver));
            m_characterStrength = xd.ReadSingle(xe, nameof(m_characterStrength));
            m_characterMass = xd.ReadSingle(xe, nameof(m_characterMass));
            m_maxSlope = xd.ReadSingle(xe, nameof(m_maxSlope));
            m_penetrationRecoverySpeed = xd.ReadSingle(xe, nameof(m_penetrationRecoverySpeed));
            m_maxCastIterations = xd.ReadInt32(xe, nameof(m_maxCastIterations));
            m_refreshManifoldInCheckSupport = xd.ReadBoolean(xe, nameof(m_refreshManifoldInCheckSupport));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_dynamicFriction), m_dynamicFriction);
            xs.WriteFloat(xe, nameof(m_staticFriction), m_staticFriction);
            xs.WriteFloat(xe, nameof(m_keepContactTolerance), m_keepContactTolerance);
            xs.WriteVector4(xe, nameof(m_up), m_up);
            xs.WriteFloat(xe, nameof(m_keepDistance), m_keepDistance);
            xs.WriteFloat(xe, nameof(m_contactAngleSensitivity), m_contactAngleSensitivity);
            xs.WriteNumber(xe, nameof(m_userPlanes), m_userPlanes);
            xs.WriteFloat(xe, nameof(m_maxCharacterSpeedForSolver), m_maxCharacterSpeedForSolver);
            xs.WriteFloat(xe, nameof(m_characterStrength), m_characterStrength);
            xs.WriteFloat(xe, nameof(m_characterMass), m_characterMass);
            xs.WriteFloat(xe, nameof(m_maxSlope), m_maxSlope);
            xs.WriteFloat(xe, nameof(m_penetrationRecoverySpeed), m_penetrationRecoverySpeed);
            xs.WriteNumber(xe, nameof(m_maxCastIterations), m_maxCastIterations);
            xs.WriteBoolean(xe, nameof(m_refreshManifoldInCheckSupport), m_refreshManifoldInCheckSupport);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbProxyModifierProxyInfo);
        }

        public bool Equals(hkbProxyModifierProxyInfo? other)
        {
            return other is not null &&
                   m_dynamicFriction.Equals(other.m_dynamicFriction) &&
                   m_staticFriction.Equals(other.m_staticFriction) &&
                   m_keepContactTolerance.Equals(other.m_keepContactTolerance) &&
                   m_up.Equals(other.m_up) &&
                   m_keepDistance.Equals(other.m_keepDistance) &&
                   m_contactAngleSensitivity.Equals(other.m_contactAngleSensitivity) &&
                   m_userPlanes.Equals(other.m_userPlanes) &&
                   m_maxCharacterSpeedForSolver.Equals(other.m_maxCharacterSpeedForSolver) &&
                   m_characterStrength.Equals(other.m_characterStrength) &&
                   m_characterMass.Equals(other.m_characterMass) &&
                   m_maxSlope.Equals(other.m_maxSlope) &&
                   m_penetrationRecoverySpeed.Equals(other.m_penetrationRecoverySpeed) &&
                   m_maxCastIterations.Equals(other.m_maxCastIterations) &&
                   m_refreshManifoldInCheckSupport.Equals(other.m_refreshManifoldInCheckSupport) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_dynamicFriction);
            hashcode.Add(m_staticFriction);
            hashcode.Add(m_keepContactTolerance);
            hashcode.Add(m_up);
            hashcode.Add(m_keepDistance);
            hashcode.Add(m_contactAngleSensitivity);
            hashcode.Add(m_userPlanes);
            hashcode.Add(m_maxCharacterSpeedForSolver);
            hashcode.Add(m_characterStrength);
            hashcode.Add(m_characterMass);
            hashcode.Add(m_maxSlope);
            hashcode.Add(m_penetrationRecoverySpeed);
            hashcode.Add(m_maxCastIterations);
            hashcode.Add(m_refreshManifoldInCheckSupport);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

