using System.Xml.Linq;
namespace HKX2
{
    // hkaSplineCompressedAnimationTrackCompressionParams Signatire: 0x42e878d3 size: 28 flags: FLAGS_NONE

    // m_rotationTolerance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_translationTolerance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_scaleTolerance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_floatingTolerance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_rotationDegree m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_translationDegree m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 18 flags: FLAGS_NONE enum: 
    // m_scaleDegree m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 20 flags: FLAGS_NONE enum: 
    // m_floatingDegree m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 22 flags: FLAGS_NONE enum: 
    // m_rotationQuantizationType m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 24 flags: FLAGS_NONE enum: RotationQuantization
    // m_translationQuantizationType m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 25 flags: FLAGS_NONE enum: ScalarQuantization
    // m_scaleQuantizationType m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 26 flags: FLAGS_NONE enum: ScalarQuantization
    // m_floatQuantizationType m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 27 flags: FLAGS_NONE enum: ScalarQuantization
    public partial class hkaSplineCompressedAnimationTrackCompressionParams : IHavokObject, IEquatable<hkaSplineCompressedAnimationTrackCompressionParams?>
    {
        public float m_rotationTolerance { set; get; }
        public float m_translationTolerance { set; get; }
        public float m_scaleTolerance { set; get; }
        public float m_floatingTolerance { set; get; }
        public ushort m_rotationDegree { set; get; }
        public ushort m_translationDegree { set; get; }
        public ushort m_scaleDegree { set; get; }
        public ushort m_floatingDegree { set; get; }
        public byte m_rotationQuantizationType { set; get; }
        public byte m_translationQuantizationType { set; get; }
        public byte m_scaleQuantizationType { set; get; }
        public byte m_floatQuantizationType { set; get; }

        public virtual uint Signature { set; get; } = 0x42e878d3;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_rotationTolerance = br.ReadSingle();
            m_translationTolerance = br.ReadSingle();
            m_scaleTolerance = br.ReadSingle();
            m_floatingTolerance = br.ReadSingle();
            m_rotationDegree = br.ReadUInt16();
            m_translationDegree = br.ReadUInt16();
            m_scaleDegree = br.ReadUInt16();
            m_floatingDegree = br.ReadUInt16();
            m_rotationQuantizationType = br.ReadByte();
            m_translationQuantizationType = br.ReadByte();
            m_scaleQuantizationType = br.ReadByte();
            m_floatQuantizationType = br.ReadByte();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_rotationTolerance);
            bw.WriteSingle(m_translationTolerance);
            bw.WriteSingle(m_scaleTolerance);
            bw.WriteSingle(m_floatingTolerance);
            bw.WriteUInt16(m_rotationDegree);
            bw.WriteUInt16(m_translationDegree);
            bw.WriteUInt16(m_scaleDegree);
            bw.WriteUInt16(m_floatingDegree);
            bw.WriteByte(m_rotationQuantizationType);
            bw.WriteByte(m_translationQuantizationType);
            bw.WriteByte(m_scaleQuantizationType);
            bw.WriteByte(m_floatQuantizationType);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_rotationTolerance = xd.ReadSingle(xe, nameof(m_rotationTolerance));
            m_translationTolerance = xd.ReadSingle(xe, nameof(m_translationTolerance));
            m_scaleTolerance = xd.ReadSingle(xe, nameof(m_scaleTolerance));
            m_floatingTolerance = xd.ReadSingle(xe, nameof(m_floatingTolerance));
            m_rotationDegree = xd.ReadUInt16(xe, nameof(m_rotationDegree));
            m_translationDegree = xd.ReadUInt16(xe, nameof(m_translationDegree));
            m_scaleDegree = xd.ReadUInt16(xe, nameof(m_scaleDegree));
            m_floatingDegree = xd.ReadUInt16(xe, nameof(m_floatingDegree));
            m_rotationQuantizationType = xd.ReadFlag<RotationQuantization, byte>(xe, nameof(m_rotationQuantizationType));
            m_translationQuantizationType = xd.ReadFlag<ScalarQuantization, byte>(xe, nameof(m_translationQuantizationType));
            m_scaleQuantizationType = xd.ReadFlag<ScalarQuantization, byte>(xe, nameof(m_scaleQuantizationType));
            m_floatQuantizationType = xd.ReadFlag<ScalarQuantization, byte>(xe, nameof(m_floatQuantizationType));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_rotationTolerance), m_rotationTolerance);
            xs.WriteFloat(xe, nameof(m_translationTolerance), m_translationTolerance);
            xs.WriteFloat(xe, nameof(m_scaleTolerance), m_scaleTolerance);
            xs.WriteFloat(xe, nameof(m_floatingTolerance), m_floatingTolerance);
            xs.WriteNumber(xe, nameof(m_rotationDegree), m_rotationDegree);
            xs.WriteNumber(xe, nameof(m_translationDegree), m_translationDegree);
            xs.WriteNumber(xe, nameof(m_scaleDegree), m_scaleDegree);
            xs.WriteNumber(xe, nameof(m_floatingDegree), m_floatingDegree);
            xs.WriteEnum<RotationQuantization, byte>(xe, nameof(m_rotationQuantizationType), m_rotationQuantizationType);
            xs.WriteEnum<ScalarQuantization, byte>(xe, nameof(m_translationQuantizationType), m_translationQuantizationType);
            xs.WriteEnum<ScalarQuantization, byte>(xe, nameof(m_scaleQuantizationType), m_scaleQuantizationType);
            xs.WriteEnum<ScalarQuantization, byte>(xe, nameof(m_floatQuantizationType), m_floatQuantizationType);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaSplineCompressedAnimationTrackCompressionParams);
        }

        public bool Equals(hkaSplineCompressedAnimationTrackCompressionParams? other)
        {
            return other is not null &&
                   m_rotationTolerance.Equals(other.m_rotationTolerance) &&
                   m_translationTolerance.Equals(other.m_translationTolerance) &&
                   m_scaleTolerance.Equals(other.m_scaleTolerance) &&
                   m_floatingTolerance.Equals(other.m_floatingTolerance) &&
                   m_rotationDegree.Equals(other.m_rotationDegree) &&
                   m_translationDegree.Equals(other.m_translationDegree) &&
                   m_scaleDegree.Equals(other.m_scaleDegree) &&
                   m_floatingDegree.Equals(other.m_floatingDegree) &&
                   m_rotationQuantizationType.Equals(other.m_rotationQuantizationType) &&
                   m_translationQuantizationType.Equals(other.m_translationQuantizationType) &&
                   m_scaleQuantizationType.Equals(other.m_scaleQuantizationType) &&
                   m_floatQuantizationType.Equals(other.m_floatQuantizationType) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_rotationTolerance);
            hashcode.Add(m_translationTolerance);
            hashcode.Add(m_scaleTolerance);
            hashcode.Add(m_floatingTolerance);
            hashcode.Add(m_rotationDegree);
            hashcode.Add(m_translationDegree);
            hashcode.Add(m_scaleDegree);
            hashcode.Add(m_floatingDegree);
            hashcode.Add(m_rotationQuantizationType);
            hashcode.Add(m_translationQuantizationType);
            hashcode.Add(m_scaleQuantizationType);
            hashcode.Add(m_floatQuantizationType);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

