using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbHandIkControlData Signatire: 0xd72b8d17 size: 96 flags: FLAGS_NONE

    // m_targetPosition m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_targetRotation m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_targetNormal m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_targetHandle m_class: hkbHandle Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_transformOnFraction m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_normalOnFraction m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 60 flags: FLAGS_NONE enum: 
    // m_fadeInDuration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_fadeOutDuration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 68 flags: FLAGS_NONE enum: 
    // m_extrapolationTimeStep m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_handleChangeSpeed m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 76 flags: FLAGS_NONE enum: 
    // m_handleChangeMode m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 80 flags: FLAGS_NONE enum: HandleChangeMode
    // m_fixUp m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 81 flags: FLAGS_NONE enum: 
    public partial class hkbHandIkControlData : IHavokObject, IEquatable<hkbHandIkControlData?>
    {
        public Vector4 m_targetPosition { set; get; }
        public Quaternion m_targetRotation { set; get; }
        public Vector4 m_targetNormal { set; get; }
        public hkbHandle? m_targetHandle { set; get; }
        public float m_transformOnFraction { set; get; }
        public float m_normalOnFraction { set; get; }
        public float m_fadeInDuration { set; get; }
        public float m_fadeOutDuration { set; get; }
        public float m_extrapolationTimeStep { set; get; }
        public float m_handleChangeSpeed { set; get; }
        public sbyte m_handleChangeMode { set; get; }
        public bool m_fixUp { set; get; }

        public virtual uint Signature { set; get; } = 0xd72b8d17;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_targetPosition = br.ReadVector4();
            m_targetRotation = des.ReadQuaternion(br);
            m_targetNormal = br.ReadVector4();
            m_targetHandle = des.ReadClassPointer<hkbHandle>(br);
            m_transformOnFraction = br.ReadSingle();
            m_normalOnFraction = br.ReadSingle();
            m_fadeInDuration = br.ReadSingle();
            m_fadeOutDuration = br.ReadSingle();
            m_extrapolationTimeStep = br.ReadSingle();
            m_handleChangeSpeed = br.ReadSingle();
            m_handleChangeMode = br.ReadSByte();
            m_fixUp = br.ReadBoolean();
            br.Position += 14;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_targetPosition);
            s.WriteQuaternion(bw, m_targetRotation);
            bw.WriteVector4(m_targetNormal);
            s.WriteClassPointer(bw, m_targetHandle);
            bw.WriteSingle(m_transformOnFraction);
            bw.WriteSingle(m_normalOnFraction);
            bw.WriteSingle(m_fadeInDuration);
            bw.WriteSingle(m_fadeOutDuration);
            bw.WriteSingle(m_extrapolationTimeStep);
            bw.WriteSingle(m_handleChangeSpeed);
            bw.WriteSByte(m_handleChangeMode);
            bw.WriteBoolean(m_fixUp);
            bw.Position += 14;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_targetPosition = xd.ReadVector4(xe, nameof(m_targetPosition));
            m_targetRotation = xd.ReadQuaternion(xe, nameof(m_targetRotation));
            m_targetNormal = xd.ReadVector4(xe, nameof(m_targetNormal));
            m_targetHandle = xd.ReadClassPointer<hkbHandle>(xe, nameof(m_targetHandle));
            m_transformOnFraction = xd.ReadSingle(xe, nameof(m_transformOnFraction));
            m_normalOnFraction = xd.ReadSingle(xe, nameof(m_normalOnFraction));
            m_fadeInDuration = xd.ReadSingle(xe, nameof(m_fadeInDuration));
            m_fadeOutDuration = xd.ReadSingle(xe, nameof(m_fadeOutDuration));
            m_extrapolationTimeStep = xd.ReadSingle(xe, nameof(m_extrapolationTimeStep));
            m_handleChangeSpeed = xd.ReadSingle(xe, nameof(m_handleChangeSpeed));
            m_handleChangeMode = xd.ReadFlag<HandleChangeMode, sbyte>(xe, nameof(m_handleChangeMode));
            m_fixUp = xd.ReadBoolean(xe, nameof(m_fixUp));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_targetPosition), m_targetPosition);
            xs.WriteQuaternion(xe, nameof(m_targetRotation), m_targetRotation);
            xs.WriteVector4(xe, nameof(m_targetNormal), m_targetNormal);
            xs.WriteClassPointer(xe, nameof(m_targetHandle), m_targetHandle);
            xs.WriteFloat(xe, nameof(m_transformOnFraction), m_transformOnFraction);
            xs.WriteFloat(xe, nameof(m_normalOnFraction), m_normalOnFraction);
            xs.WriteFloat(xe, nameof(m_fadeInDuration), m_fadeInDuration);
            xs.WriteFloat(xe, nameof(m_fadeOutDuration), m_fadeOutDuration);
            xs.WriteFloat(xe, nameof(m_extrapolationTimeStep), m_extrapolationTimeStep);
            xs.WriteFloat(xe, nameof(m_handleChangeSpeed), m_handleChangeSpeed);
            xs.WriteEnum<HandleChangeMode, sbyte>(xe, nameof(m_handleChangeMode), m_handleChangeMode);
            xs.WriteBoolean(xe, nameof(m_fixUp), m_fixUp);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbHandIkControlData);
        }

        public bool Equals(hkbHandIkControlData? other)
        {
            return other is not null &&
                   m_targetPosition.Equals(other.m_targetPosition) &&
                   m_targetRotation.Equals(other.m_targetRotation) &&
                   m_targetNormal.Equals(other.m_targetNormal) &&
                   ((m_targetHandle is null && other.m_targetHandle is null) || (m_targetHandle is not null && other.m_targetHandle is not null && m_targetHandle.Equals((IHavokObject)other.m_targetHandle))) &&
                   m_transformOnFraction.Equals(other.m_transformOnFraction) &&
                   m_normalOnFraction.Equals(other.m_normalOnFraction) &&
                   m_fadeInDuration.Equals(other.m_fadeInDuration) &&
                   m_fadeOutDuration.Equals(other.m_fadeOutDuration) &&
                   m_extrapolationTimeStep.Equals(other.m_extrapolationTimeStep) &&
                   m_handleChangeSpeed.Equals(other.m_handleChangeSpeed) &&
                   m_handleChangeMode.Equals(other.m_handleChangeMode) &&
                   m_fixUp.Equals(other.m_fixUp) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_targetPosition);
            hashcode.Add(m_targetRotation);
            hashcode.Add(m_targetNormal);
            hashcode.Add(m_targetHandle);
            hashcode.Add(m_transformOnFraction);
            hashcode.Add(m_normalOnFraction);
            hashcode.Add(m_fadeInDuration);
            hashcode.Add(m_fadeOutDuration);
            hashcode.Add(m_extrapolationTimeStep);
            hashcode.Add(m_handleChangeSpeed);
            hashcode.Add(m_handleChangeMode);
            hashcode.Add(m_fixUp);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

