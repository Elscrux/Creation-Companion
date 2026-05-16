using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkMotionState Signatire: 0x5797386e size: 176 flags: FLAGS_NONE

    // m_transform m_class:  Type.TYPE_TRANSFORM Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_sweptTransform m_class: hkSweptTransform Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_deltaAngle m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_objectRadius m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_linearDamping m_class:  Type.TYPE_HALF Type.TYPE_VOID arrSize: 0 offset: 164 flags: FLAGS_NONE enum: 
    // m_angularDamping m_class:  Type.TYPE_HALF Type.TYPE_VOID arrSize: 0 offset: 166 flags: FLAGS_NONE enum: 
    // m_timeFactor m_class:  Type.TYPE_HALF Type.TYPE_VOID arrSize: 0 offset: 168 flags: FLAGS_NONE enum: 
    // m_maxLinearVelocity m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 170 flags: FLAGS_NONE enum: 
    // m_maxAngularVelocity m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 171 flags: FLAGS_NONE enum: 
    // m_deactivationClass m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 172 flags: FLAGS_NONE enum: 
    public partial class hkMotionState : IHavokObject, IEquatable<hkMotionState?>
    {
        public Matrix4x4 m_transform { set; get; }
        public hkSweptTransform m_sweptTransform { set; get; } = new();
        public Vector4 m_deltaAngle { set; get; }
        public float m_objectRadius { set; get; }
        public Half m_linearDamping { set; get; }
        public Half m_angularDamping { set; get; }
        public Half m_timeFactor { set; get; }
        public byte m_maxLinearVelocity { set; get; }
        public byte m_maxAngularVelocity { set; get; }
        public byte m_deactivationClass { set; get; }

        public virtual uint Signature { set; get; } = 0x5797386e;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transform = des.ReadTransform(br);
            m_sweptTransform.Read(des, br);
            m_deltaAngle = br.ReadVector4();
            m_objectRadius = br.ReadSingle();
            m_linearDamping = br.ReadHalf();
            m_angularDamping = br.ReadHalf();
            m_timeFactor = br.ReadHalf();
            m_maxLinearVelocity = br.ReadByte();
            m_maxAngularVelocity = br.ReadByte();
            m_deactivationClass = br.ReadByte();
            br.Position += 3;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteTransform(bw, m_transform);
            m_sweptTransform.Write(s, bw);
            bw.WriteVector4(m_deltaAngle);
            bw.WriteSingle(m_objectRadius);
            bw.WriteHalf(m_linearDamping);
            bw.WriteHalf(m_angularDamping);
            bw.WriteHalf(m_timeFactor);
            bw.WriteByte(m_maxLinearVelocity);
            bw.WriteByte(m_maxAngularVelocity);
            bw.WriteByte(m_deactivationClass);
            bw.Position += 3;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_transform = xd.ReadTransform(xe, nameof(m_transform));
            m_sweptTransform = xd.ReadClass<hkSweptTransform>(xe, nameof(m_sweptTransform));
            m_deltaAngle = xd.ReadVector4(xe, nameof(m_deltaAngle));
            m_objectRadius = xd.ReadSingle(xe, nameof(m_objectRadius));
            m_linearDamping = xd.ReadHalf(xe, nameof(m_linearDamping));
            m_angularDamping = xd.ReadHalf(xe, nameof(m_angularDamping));
            m_timeFactor = xd.ReadHalf(xe, nameof(m_timeFactor));
            m_maxLinearVelocity = xd.ReadByte(xe, nameof(m_maxLinearVelocity));
            m_maxAngularVelocity = xd.ReadByte(xe, nameof(m_maxAngularVelocity));
            m_deactivationClass = xd.ReadByte(xe, nameof(m_deactivationClass));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteTransform(xe, nameof(m_transform), m_transform);
            xs.WriteClass<hkSweptTransform>(xe, nameof(m_sweptTransform), m_sweptTransform);
            xs.WriteVector4(xe, nameof(m_deltaAngle), m_deltaAngle);
            xs.WriteFloat(xe, nameof(m_objectRadius), m_objectRadius);
            xs.WriteFloat(xe, nameof(m_linearDamping), m_linearDamping);
            xs.WriteFloat(xe, nameof(m_angularDamping), m_angularDamping);
            xs.WriteFloat(xe, nameof(m_timeFactor), m_timeFactor);
            xs.WriteNumber(xe, nameof(m_maxLinearVelocity), m_maxLinearVelocity);
            xs.WriteNumber(xe, nameof(m_maxAngularVelocity), m_maxAngularVelocity);
            xs.WriteNumber(xe, nameof(m_deactivationClass), m_deactivationClass);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkMotionState);
        }

        public bool Equals(hkMotionState? other)
        {
            return other is not null &&
                   m_transform.Equals(other.m_transform) &&
                   ((m_sweptTransform is null && other.m_sweptTransform is null) || (m_sweptTransform is not null && other.m_sweptTransform is not null && m_sweptTransform.Equals((IHavokObject)other.m_sweptTransform))) &&
                   m_deltaAngle.Equals(other.m_deltaAngle) &&
                   m_objectRadius.Equals(other.m_objectRadius) &&
                   m_linearDamping.Equals(other.m_linearDamping) &&
                   m_angularDamping.Equals(other.m_angularDamping) &&
                   m_timeFactor.Equals(other.m_timeFactor) &&
                   m_maxLinearVelocity.Equals(other.m_maxLinearVelocity) &&
                   m_maxAngularVelocity.Equals(other.m_maxAngularVelocity) &&
                   m_deactivationClass.Equals(other.m_deactivationClass) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_transform);
            hashcode.Add(m_sweptTransform);
            hashcode.Add(m_deltaAngle);
            hashcode.Add(m_objectRadius);
            hashcode.Add(m_linearDamping);
            hashcode.Add(m_angularDamping);
            hashcode.Add(m_timeFactor);
            hashcode.Add(m_maxLinearVelocity);
            hashcode.Add(m_maxAngularVelocity);
            hashcode.Add(m_deactivationClass);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

