using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpMotion Signatire: 0x98aadb4f size: 320 flags: FLAGS_NONE

    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: MotionType
    // m_deactivationIntegrateCounter m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 17 flags: FLAGS_NONE enum: 
    // m_deactivationNumInactiveFrames m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 2 offset: 18 flags: FLAGS_NONE enum: 
    // m_motionState m_class: hkMotionState Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_inertiaAndMassInv m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 208 flags: FLAGS_NONE enum: 
    // m_linearVelocity m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 224 flags: FLAGS_NONE enum: 
    // m_angularVelocity m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 240 flags: FLAGS_NONE enum: 
    // m_deactivationRefPosition m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 2 offset: 256 flags: FLAGS_NONE enum: 
    // m_deactivationRefOrientation m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 2 offset: 288 flags: FLAGS_NONE enum: 
    // m_savedMotion m_class: hkpMaxSizeMotion Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 296 flags: FLAGS_NONE enum: 
    // m_savedQualityTypeIndex m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 304 flags: FLAGS_NONE enum: 
    // m_gravityFactor m_class:  Type.TYPE_HALF Type.TYPE_VOID arrSize: 0 offset: 306 flags: FLAGS_NONE enum: 
    public partial class hkpMotion : hkReferencedObject, IEquatable<hkpMotion?>
    {
        public byte m_type { set; get; }
        public byte m_deactivationIntegrateCounter { set; get; }
        public ushort[] m_deactivationNumInactiveFrames = new ushort[2];
        public hkMotionState m_motionState { set; get; } = new();
        public Vector4 m_inertiaAndMassInv { set; get; }
        public Vector4 m_linearVelocity { set; get; }
        public Vector4 m_angularVelocity { set; get; }
        public Vector4[] m_deactivationRefPosition = new Vector4[2];
        public uint[] m_deactivationRefOrientation = new uint[2];
        public hkpMaxSizeMotion? m_savedMotion { set; get; }
        public ushort m_savedQualityTypeIndex { set; get; }
        public Half m_gravityFactor { set; get; }

        public override uint Signature { set; get; } = 0x98aadb4f;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_type = br.ReadByte();
            m_deactivationIntegrateCounter = br.ReadByte();
            m_deactivationNumInactiveFrames = des.ReadUInt16CStyleArray(br, 2);
            br.Position += 10;
            m_motionState.Read(des, br);
            m_inertiaAndMassInv = br.ReadVector4();
            m_linearVelocity = br.ReadVector4();
            m_angularVelocity = br.ReadVector4();
            m_deactivationRefPosition = des.ReadVector4CStyleArray(br, 2);
            m_deactivationRefOrientation = des.ReadUInt32CStyleArray(br, 2);
            m_savedMotion = des.ReadClassPointer<hkpMaxSizeMotion>(br);
            m_savedQualityTypeIndex = br.ReadUInt16();
            m_gravityFactor = br.ReadHalf();
            br.Position += 12;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteByte(m_type);
            bw.WriteByte(m_deactivationIntegrateCounter);
            s.WriteUInt16CStyleArray(bw, m_deactivationNumInactiveFrames);
            bw.Position += 10;
            m_motionState.Write(s, bw);
            bw.WriteVector4(m_inertiaAndMassInv);
            bw.WriteVector4(m_linearVelocity);
            bw.WriteVector4(m_angularVelocity);
            s.WriteVector4CStyleArray(bw, m_deactivationRefPosition);
            s.WriteUInt32CStyleArray(bw, m_deactivationRefOrientation);
            s.WriteClassPointer(bw, m_savedMotion);
            bw.WriteUInt16(m_savedQualityTypeIndex);
            bw.WriteHalf(m_gravityFactor);
            bw.Position += 12;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_type = xd.ReadFlag<MotionType, byte>(xe, nameof(m_type));
            m_deactivationIntegrateCounter = xd.ReadByte(xe, nameof(m_deactivationIntegrateCounter));
            m_deactivationNumInactiveFrames = xd.ReadUInt16CStyleArray(xe, nameof(m_deactivationNumInactiveFrames), 2);
            m_motionState = xd.ReadClass<hkMotionState>(xe, nameof(m_motionState));
            m_inertiaAndMassInv = xd.ReadVector4(xe, nameof(m_inertiaAndMassInv));
            m_linearVelocity = xd.ReadVector4(xe, nameof(m_linearVelocity));
            m_angularVelocity = xd.ReadVector4(xe, nameof(m_angularVelocity));
            m_deactivationRefPosition = xd.ReadVector4CStyleArray(xe, nameof(m_deactivationRefPosition), 2);
            m_deactivationRefOrientation = xd.ReadUInt32CStyleArray(xe, nameof(m_deactivationRefOrientation), 2);
            m_savedMotion = xd.ReadClassPointer<hkpMaxSizeMotion>(xe, nameof(m_savedMotion));
            m_savedQualityTypeIndex = xd.ReadUInt16(xe, nameof(m_savedQualityTypeIndex));
            m_gravityFactor = xd.ReadHalf(xe, nameof(m_gravityFactor));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteEnum<MotionType, byte>(xe, nameof(m_type), m_type);
            xs.WriteNumber(xe, nameof(m_deactivationIntegrateCounter), m_deactivationIntegrateCounter);
            xs.WriteNumberArray(xe, nameof(m_deactivationNumInactiveFrames), m_deactivationNumInactiveFrames);
            xs.WriteClass<hkMotionState>(xe, nameof(m_motionState), m_motionState);
            xs.WriteVector4(xe, nameof(m_inertiaAndMassInv), m_inertiaAndMassInv);
            xs.WriteVector4(xe, nameof(m_linearVelocity), m_linearVelocity);
            xs.WriteVector4(xe, nameof(m_angularVelocity), m_angularVelocity);
            xs.WriteVector4Array(xe, nameof(m_deactivationRefPosition), m_deactivationRefPosition);
            xs.WriteNumberArray(xe, nameof(m_deactivationRefOrientation), m_deactivationRefOrientation);
            xs.WriteClassPointer(xe, nameof(m_savedMotion), m_savedMotion);
            xs.WriteNumber(xe, nameof(m_savedQualityTypeIndex), m_savedQualityTypeIndex);
            xs.WriteFloat(xe, nameof(m_gravityFactor), m_gravityFactor);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMotion);
        }

        public bool Equals(hkpMotion? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_type.Equals(other.m_type) &&
                   m_deactivationIntegrateCounter.Equals(other.m_deactivationIntegrateCounter) &&
                   m_deactivationNumInactiveFrames.SequenceEqual(other.m_deactivationNumInactiveFrames) &&
                   ((m_motionState is null && other.m_motionState is null) || (m_motionState is not null && other.m_motionState is not null && m_motionState.Equals((IHavokObject)other.m_motionState))) &&
                   m_inertiaAndMassInv.Equals(other.m_inertiaAndMassInv) &&
                   m_linearVelocity.Equals(other.m_linearVelocity) &&
                   m_angularVelocity.Equals(other.m_angularVelocity) &&
                   m_deactivationRefPosition.SequenceEqual(other.m_deactivationRefPosition) &&
                   m_deactivationRefOrientation.SequenceEqual(other.m_deactivationRefOrientation) &&
                   ((m_savedMotion is null && other.m_savedMotion is null) || (m_savedMotion is not null && other.m_savedMotion is not null && m_savedMotion.Equals((IHavokObject)other.m_savedMotion))) &&
                   m_savedQualityTypeIndex.Equals(other.m_savedQualityTypeIndex) &&
                   m_gravityFactor.Equals(other.m_gravityFactor) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_type);
            hashcode.Add(m_deactivationIntegrateCounter);
            hashcode.Add(m_deactivationNumInactiveFrames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_motionState);
            hashcode.Add(m_inertiaAndMassInv);
            hashcode.Add(m_linearVelocity);
            hashcode.Add(m_angularVelocity);
            hashcode.Add(m_deactivationRefPosition.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_deactivationRefOrientation.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_savedMotion);
            hashcode.Add(m_savedQualityTypeIndex);
            hashcode.Add(m_gravityFactor);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

