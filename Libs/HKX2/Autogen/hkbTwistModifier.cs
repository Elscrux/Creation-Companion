using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbTwistModifier Signatire: 0xb6b76b32 size: 144 flags: FLAGS_NONE

    // m_axisOfRotation m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_twistAngle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_startBoneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    // m_endBoneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 102 flags: FLAGS_NONE enum: 
    // m_setAngleMethod m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 104 flags: FLAGS_NONE enum: SetAngleMethod
    // m_rotationAxisCoordinates m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 105 flags: FLAGS_NONE enum: RotationAxisCoordinates
    // m_isAdditive m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 106 flags: FLAGS_NONE enum: 
    // m_boneChainIndices m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 112 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_parentBoneIndices m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 128 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbTwistModifier : hkbModifier, IEquatable<hkbTwistModifier?>
    {
        public Vector4 m_axisOfRotation { set; get; }
        public float m_twistAngle { set; get; }
        public short m_startBoneIndex { set; get; }
        public short m_endBoneIndex { set; get; }
        public sbyte m_setAngleMethod { set; get; }
        public sbyte m_rotationAxisCoordinates { set; get; }
        public bool m_isAdditive { set; get; }
        public IList<object> m_boneChainIndices { set; get; } = Array.Empty<object>();
        public IList<object> m_parentBoneIndices { set; get; } = Array.Empty<object>();

        public override uint Signature { set; get; } = 0xb6b76b32;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_axisOfRotation = br.ReadVector4();
            m_twistAngle = br.ReadSingle();
            m_startBoneIndex = br.ReadInt16();
            m_endBoneIndex = br.ReadInt16();
            m_setAngleMethod = br.ReadSByte();
            m_rotationAxisCoordinates = br.ReadSByte();
            m_isAdditive = br.ReadBoolean();
            br.Position += 5;
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_axisOfRotation);
            bw.WriteSingle(m_twistAngle);
            bw.WriteInt16(m_startBoneIndex);
            bw.WriteInt16(m_endBoneIndex);
            bw.WriteSByte(m_setAngleMethod);
            bw.WriteSByte(m_rotationAxisCoordinates);
            bw.WriteBoolean(m_isAdditive);
            bw.Position += 5;
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_axisOfRotation = xd.ReadVector4(xe, nameof(m_axisOfRotation));
            m_twistAngle = xd.ReadSingle(xe, nameof(m_twistAngle));
            m_startBoneIndex = xd.ReadInt16(xe, nameof(m_startBoneIndex));
            m_endBoneIndex = xd.ReadInt16(xe, nameof(m_endBoneIndex));
            m_setAngleMethod = xd.ReadFlag<SetAngleMethod, sbyte>(xe, nameof(m_setAngleMethod));
            m_rotationAxisCoordinates = xd.ReadFlag<RotationAxisCoordinates, sbyte>(xe, nameof(m_rotationAxisCoordinates));
            m_isAdditive = xd.ReadBoolean(xe, nameof(m_isAdditive));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_axisOfRotation), m_axisOfRotation);
            xs.WriteFloat(xe, nameof(m_twistAngle), m_twistAngle);
            xs.WriteNumber(xe, nameof(m_startBoneIndex), m_startBoneIndex);
            xs.WriteNumber(xe, nameof(m_endBoneIndex), m_endBoneIndex);
            xs.WriteEnum<SetAngleMethod, sbyte>(xe, nameof(m_setAngleMethod), m_setAngleMethod);
            xs.WriteEnum<RotationAxisCoordinates, sbyte>(xe, nameof(m_rotationAxisCoordinates), m_rotationAxisCoordinates);
            xs.WriteBoolean(xe, nameof(m_isAdditive), m_isAdditive);
            xs.WriteSerializeIgnored(xe, nameof(m_boneChainIndices));
            xs.WriteSerializeIgnored(xe, nameof(m_parentBoneIndices));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbTwistModifier);
        }

        public bool Equals(hkbTwistModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_axisOfRotation.Equals(other.m_axisOfRotation) &&
                   m_twistAngle.Equals(other.m_twistAngle) &&
                   m_startBoneIndex.Equals(other.m_startBoneIndex) &&
                   m_endBoneIndex.Equals(other.m_endBoneIndex) &&
                   m_setAngleMethod.Equals(other.m_setAngleMethod) &&
                   m_rotationAxisCoordinates.Equals(other.m_rotationAxisCoordinates) &&
                   m_isAdditive.Equals(other.m_isAdditive) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_axisOfRotation);
            hashcode.Add(m_twistAngle);
            hashcode.Add(m_startBoneIndex);
            hashcode.Add(m_endBoneIndex);
            hashcode.Add(m_setAngleMethod);
            hashcode.Add(m_rotationAxisCoordinates);
            hashcode.Add(m_isAdditive);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

