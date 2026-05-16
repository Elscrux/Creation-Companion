using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbCharacterControllerModifier Signatire: 0xf675d6fb size: 176 flags: FLAGS_NONE

    // m_controlData m_class: hkbCharacterControllerControlData Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_initialVelocity m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_initialVelocityCoordinates m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 128 flags: FLAGS_NONE enum: InitialVelocityCoordinates
    // m_motionMode m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 129 flags: FLAGS_NONE enum: MotionMode
    // m_forceDownwardMomentum m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 130 flags: FLAGS_NONE enum: 
    // m_applyGravity m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 131 flags: FLAGS_NONE enum: 
    // m_setInitialVelocity m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 132 flags: FLAGS_NONE enum: 
    // m_isTouchingGround m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 133 flags: FLAGS_NONE enum: 
    // m_gravity m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 144 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_timestep m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 160 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_isInitialVelocityAdded m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 164 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbCharacterControllerModifier : hkbModifier, IEquatable<hkbCharacterControllerModifier?>
    {
        public hkbCharacterControllerControlData m_controlData { set; get; } = new();
        public Vector4 m_initialVelocity { set; get; }
        public sbyte m_initialVelocityCoordinates { set; get; }
        public sbyte m_motionMode { set; get; }
        public bool m_forceDownwardMomentum { set; get; }
        public bool m_applyGravity { set; get; }
        public bool m_setInitialVelocity { set; get; }
        public bool m_isTouchingGround { set; get; }
        private Vector4 m_gravity { set; get; }
        private float m_timestep { set; get; }
        private bool m_isInitialVelocityAdded { set; get; }

        public override uint Signature { set; get; } = 0xf675d6fb;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_controlData.Read(des, br);
            m_initialVelocity = br.ReadVector4();
            m_initialVelocityCoordinates = br.ReadSByte();
            m_motionMode = br.ReadSByte();
            m_forceDownwardMomentum = br.ReadBoolean();
            m_applyGravity = br.ReadBoolean();
            m_setInitialVelocity = br.ReadBoolean();
            m_isTouchingGround = br.ReadBoolean();
            br.Position += 10;
            m_gravity = br.ReadVector4();
            m_timestep = br.ReadSingle();
            m_isInitialVelocityAdded = br.ReadBoolean();
            br.Position += 11;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_controlData.Write(s, bw);
            bw.WriteVector4(m_initialVelocity);
            bw.WriteSByte(m_initialVelocityCoordinates);
            bw.WriteSByte(m_motionMode);
            bw.WriteBoolean(m_forceDownwardMomentum);
            bw.WriteBoolean(m_applyGravity);
            bw.WriteBoolean(m_setInitialVelocity);
            bw.WriteBoolean(m_isTouchingGround);
            bw.Position += 10;
            bw.WriteVector4(m_gravity);
            bw.WriteSingle(m_timestep);
            bw.WriteBoolean(m_isInitialVelocityAdded);
            bw.Position += 11;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_controlData = xd.ReadClass<hkbCharacterControllerControlData>(xe, nameof(m_controlData));
            m_initialVelocity = xd.ReadVector4(xe, nameof(m_initialVelocity));
            m_initialVelocityCoordinates = xd.ReadFlag<InitialVelocityCoordinates, sbyte>(xe, nameof(m_initialVelocityCoordinates));
            m_motionMode = xd.ReadFlag<MotionMode, sbyte>(xe, nameof(m_motionMode));
            m_forceDownwardMomentum = xd.ReadBoolean(xe, nameof(m_forceDownwardMomentum));
            m_applyGravity = xd.ReadBoolean(xe, nameof(m_applyGravity));
            m_setInitialVelocity = xd.ReadBoolean(xe, nameof(m_setInitialVelocity));
            m_isTouchingGround = xd.ReadBoolean(xe, nameof(m_isTouchingGround));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkbCharacterControllerControlData>(xe, nameof(m_controlData), m_controlData);
            xs.WriteVector4(xe, nameof(m_initialVelocity), m_initialVelocity);
            xs.WriteEnum<InitialVelocityCoordinates, sbyte>(xe, nameof(m_initialVelocityCoordinates), m_initialVelocityCoordinates);
            xs.WriteEnum<MotionMode, sbyte>(xe, nameof(m_motionMode), m_motionMode);
            xs.WriteBoolean(xe, nameof(m_forceDownwardMomentum), m_forceDownwardMomentum);
            xs.WriteBoolean(xe, nameof(m_applyGravity), m_applyGravity);
            xs.WriteBoolean(xe, nameof(m_setInitialVelocity), m_setInitialVelocity);
            xs.WriteBoolean(xe, nameof(m_isTouchingGround), m_isTouchingGround);
            xs.WriteSerializeIgnored(xe, nameof(m_gravity));
            xs.WriteSerializeIgnored(xe, nameof(m_timestep));
            xs.WriteSerializeIgnored(xe, nameof(m_isInitialVelocityAdded));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbCharacterControllerModifier);
        }

        public bool Equals(hkbCharacterControllerModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_controlData is null && other.m_controlData is null) || (m_controlData is not null && other.m_controlData is not null && m_controlData.Equals((IHavokObject)other.m_controlData))) &&
                   m_initialVelocity.Equals(other.m_initialVelocity) &&
                   m_initialVelocityCoordinates.Equals(other.m_initialVelocityCoordinates) &&
                   m_motionMode.Equals(other.m_motionMode) &&
                   m_forceDownwardMomentum.Equals(other.m_forceDownwardMomentum) &&
                   m_applyGravity.Equals(other.m_applyGravity) &&
                   m_setInitialVelocity.Equals(other.m_setInitialVelocity) &&
                   m_isTouchingGround.Equals(other.m_isTouchingGround) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_controlData);
            hashcode.Add(m_initialVelocity);
            hashcode.Add(m_initialVelocityCoordinates);
            hashcode.Add(m_motionMode);
            hashcode.Add(m_forceDownwardMomentum);
            hashcode.Add(m_applyGravity);
            hashcode.Add(m_setInitialVelocity);
            hashcode.Add(m_isTouchingGround);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

