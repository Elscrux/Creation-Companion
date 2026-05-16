using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpSampledHeightFieldShape Signatire: 0x11213421 size: 112 flags: FLAGS_NONE

    // m_xRes m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_zRes m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_heightCenter m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_useProjectionBasedHeight m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 44 flags: FLAGS_NONE enum: 
    // m_heightfieldType m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 45 flags: FLAGS_NONE enum: HeightFieldType
    // m_intToFloatScale m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_floatToIntScale m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_floatToIntOffsetFloorCorrected m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_extents m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    public partial class hkpSampledHeightFieldShape : hkpHeightFieldShape, IEquatable<hkpSampledHeightFieldShape?>
    {
        public int m_xRes { set; get; }
        public int m_zRes { set; get; }
        public float m_heightCenter { set; get; }
        public bool m_useProjectionBasedHeight { set; get; }
        public byte m_heightfieldType { set; get; }
        public Vector4 m_intToFloatScale { set; get; }
        public Vector4 m_floatToIntScale { set; get; }
        public Vector4 m_floatToIntOffsetFloorCorrected { set; get; }
        public Vector4 m_extents { set; get; }

        public override uint Signature { set; get; } = 0x11213421;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_xRes = br.ReadInt32();
            m_zRes = br.ReadInt32();
            m_heightCenter = br.ReadSingle();
            m_useProjectionBasedHeight = br.ReadBoolean();
            m_heightfieldType = br.ReadByte();
            br.Position += 2;
            m_intToFloatScale = br.ReadVector4();
            m_floatToIntScale = br.ReadVector4();
            m_floatToIntOffsetFloorCorrected = br.ReadVector4();
            m_extents = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_xRes);
            bw.WriteInt32(m_zRes);
            bw.WriteSingle(m_heightCenter);
            bw.WriteBoolean(m_useProjectionBasedHeight);
            bw.WriteByte(m_heightfieldType);
            bw.Position += 2;
            bw.WriteVector4(m_intToFloatScale);
            bw.WriteVector4(m_floatToIntScale);
            bw.WriteVector4(m_floatToIntOffsetFloorCorrected);
            bw.WriteVector4(m_extents);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_xRes = xd.ReadInt32(xe, nameof(m_xRes));
            m_zRes = xd.ReadInt32(xe, nameof(m_zRes));
            m_heightCenter = xd.ReadSingle(xe, nameof(m_heightCenter));
            m_useProjectionBasedHeight = xd.ReadBoolean(xe, nameof(m_useProjectionBasedHeight));
            m_heightfieldType = xd.ReadFlag<HeightFieldType, byte>(xe, nameof(m_heightfieldType));
            m_intToFloatScale = xd.ReadVector4(xe, nameof(m_intToFloatScale));
            m_floatToIntScale = xd.ReadVector4(xe, nameof(m_floatToIntScale));
            m_floatToIntOffsetFloorCorrected = xd.ReadVector4(xe, nameof(m_floatToIntOffsetFloorCorrected));
            m_extents = xd.ReadVector4(xe, nameof(m_extents));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_xRes), m_xRes);
            xs.WriteNumber(xe, nameof(m_zRes), m_zRes);
            xs.WriteFloat(xe, nameof(m_heightCenter), m_heightCenter);
            xs.WriteBoolean(xe, nameof(m_useProjectionBasedHeight), m_useProjectionBasedHeight);
            xs.WriteEnum<HeightFieldType, byte>(xe, nameof(m_heightfieldType), m_heightfieldType);
            xs.WriteVector4(xe, nameof(m_intToFloatScale), m_intToFloatScale);
            xs.WriteVector4(xe, nameof(m_floatToIntScale), m_floatToIntScale);
            xs.WriteVector4(xe, nameof(m_floatToIntOffsetFloorCorrected), m_floatToIntOffsetFloorCorrected);
            xs.WriteVector4(xe, nameof(m_extents), m_extents);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSampledHeightFieldShape);
        }

        public bool Equals(hkpSampledHeightFieldShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_xRes.Equals(other.m_xRes) &&
                   m_zRes.Equals(other.m_zRes) &&
                   m_heightCenter.Equals(other.m_heightCenter) &&
                   m_useProjectionBasedHeight.Equals(other.m_useProjectionBasedHeight) &&
                   m_heightfieldType.Equals(other.m_heightfieldType) &&
                   m_intToFloatScale.Equals(other.m_intToFloatScale) &&
                   m_floatToIntScale.Equals(other.m_floatToIntScale) &&
                   m_floatToIntOffsetFloorCorrected.Equals(other.m_floatToIntOffsetFloorCorrected) &&
                   m_extents.Equals(other.m_extents) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_xRes);
            hashcode.Add(m_zRes);
            hashcode.Add(m_heightCenter);
            hashcode.Add(m_useProjectionBasedHeight);
            hashcode.Add(m_heightfieldType);
            hashcode.Add(m_intToFloatScale);
            hashcode.Add(m_floatToIntScale);
            hashcode.Add(m_floatToIntOffsetFloorCorrected);
            hashcode.Add(m_extents);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

