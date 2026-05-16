using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbComputeRotationToTargetModifier Signatire: 0x47665f1c size: 192 flags: FLAGS_NONE

    // m_rotationOut m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_targetPosition m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_currentPosition m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_currentRotation m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_localAxisOfRotation m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_localFacingDirection m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_resultIsDelta m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    public partial class hkbComputeRotationToTargetModifier : hkbModifier, IEquatable<hkbComputeRotationToTargetModifier?>
    {
        public Quaternion m_rotationOut { set; get; }
        public Vector4 m_targetPosition { set; get; }
        public Vector4 m_currentPosition { set; get; }
        public Quaternion m_currentRotation { set; get; }
        public Vector4 m_localAxisOfRotation { set; get; }
        public Vector4 m_localFacingDirection { set; get; }
        public bool m_resultIsDelta { set; get; }

        public override uint Signature { set; get; } = 0x47665f1c;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rotationOut = des.ReadQuaternion(br);
            m_targetPosition = br.ReadVector4();
            m_currentPosition = br.ReadVector4();
            m_currentRotation = des.ReadQuaternion(br);
            m_localAxisOfRotation = br.ReadVector4();
            m_localFacingDirection = br.ReadVector4();
            m_resultIsDelta = br.ReadBoolean();
            br.Position += 15;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteQuaternion(bw, m_rotationOut);
            bw.WriteVector4(m_targetPosition);
            bw.WriteVector4(m_currentPosition);
            s.WriteQuaternion(bw, m_currentRotation);
            bw.WriteVector4(m_localAxisOfRotation);
            bw.WriteVector4(m_localFacingDirection);
            bw.WriteBoolean(m_resultIsDelta);
            bw.Position += 15;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_rotationOut = xd.ReadQuaternion(xe, nameof(m_rotationOut));
            m_targetPosition = xd.ReadVector4(xe, nameof(m_targetPosition));
            m_currentPosition = xd.ReadVector4(xe, nameof(m_currentPosition));
            m_currentRotation = xd.ReadQuaternion(xe, nameof(m_currentRotation));
            m_localAxisOfRotation = xd.ReadVector4(xe, nameof(m_localAxisOfRotation));
            m_localFacingDirection = xd.ReadVector4(xe, nameof(m_localFacingDirection));
            m_resultIsDelta = xd.ReadBoolean(xe, nameof(m_resultIsDelta));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteQuaternion(xe, nameof(m_rotationOut), m_rotationOut);
            xs.WriteVector4(xe, nameof(m_targetPosition), m_targetPosition);
            xs.WriteVector4(xe, nameof(m_currentPosition), m_currentPosition);
            xs.WriteQuaternion(xe, nameof(m_currentRotation), m_currentRotation);
            xs.WriteVector4(xe, nameof(m_localAxisOfRotation), m_localAxisOfRotation);
            xs.WriteVector4(xe, nameof(m_localFacingDirection), m_localFacingDirection);
            xs.WriteBoolean(xe, nameof(m_resultIsDelta), m_resultIsDelta);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbComputeRotationToTargetModifier);
        }

        public bool Equals(hkbComputeRotationToTargetModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_rotationOut.Equals(other.m_rotationOut) &&
                   m_targetPosition.Equals(other.m_targetPosition) &&
                   m_currentPosition.Equals(other.m_currentPosition) &&
                   m_currentRotation.Equals(other.m_currentRotation) &&
                   m_localAxisOfRotation.Equals(other.m_localAxisOfRotation) &&
                   m_localFacingDirection.Equals(other.m_localFacingDirection) &&
                   m_resultIsDelta.Equals(other.m_resultIsDelta) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_rotationOut);
            hashcode.Add(m_targetPosition);
            hashcode.Add(m_currentPosition);
            hashcode.Add(m_currentRotation);
            hashcode.Add(m_localAxisOfRotation);
            hashcode.Add(m_localFacingDirection);
            hashcode.Add(m_resultIsDelta);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

