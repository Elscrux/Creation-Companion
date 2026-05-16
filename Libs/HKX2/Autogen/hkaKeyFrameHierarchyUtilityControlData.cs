using System.Xml.Linq;
namespace HKX2
{
    // hkaKeyFrameHierarchyUtilityControlData Signatire: 0xa3d0ac71 size: 48 flags: FLAGS_NONE

    // m_hierarchyGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_velocityDamping m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_accelerationGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_velocityGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_positionGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_positionMaxLinearVelocity m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 20 flags: FLAGS_NONE enum: 
    // m_positionMaxAngularVelocity m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_snapGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 28 flags: FLAGS_NONE enum: 
    // m_snapMaxLinearVelocity m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_snapMaxAngularVelocity m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_snapMaxLinearDistance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_snapMaxAngularDistance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 44 flags: FLAGS_NONE enum: 
    public partial class hkaKeyFrameHierarchyUtilityControlData : IHavokObject, IEquatable<hkaKeyFrameHierarchyUtilityControlData?>
    {
        public float m_hierarchyGain { set; get; }
        public float m_velocityDamping { set; get; }
        public float m_accelerationGain { set; get; }
        public float m_velocityGain { set; get; }
        public float m_positionGain { set; get; }
        public float m_positionMaxLinearVelocity { set; get; }
        public float m_positionMaxAngularVelocity { set; get; }
        public float m_snapGain { set; get; }
        public float m_snapMaxLinearVelocity { set; get; }
        public float m_snapMaxAngularVelocity { set; get; }
        public float m_snapMaxLinearDistance { set; get; }
        public float m_snapMaxAngularDistance { set; get; }

        public virtual uint Signature { set; get; } = 0xa3d0ac71;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_hierarchyGain = br.ReadSingle();
            m_velocityDamping = br.ReadSingle();
            m_accelerationGain = br.ReadSingle();
            m_velocityGain = br.ReadSingle();
            m_positionGain = br.ReadSingle();
            m_positionMaxLinearVelocity = br.ReadSingle();
            m_positionMaxAngularVelocity = br.ReadSingle();
            m_snapGain = br.ReadSingle();
            m_snapMaxLinearVelocity = br.ReadSingle();
            m_snapMaxAngularVelocity = br.ReadSingle();
            m_snapMaxLinearDistance = br.ReadSingle();
            m_snapMaxAngularDistance = br.ReadSingle();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_hierarchyGain);
            bw.WriteSingle(m_velocityDamping);
            bw.WriteSingle(m_accelerationGain);
            bw.WriteSingle(m_velocityGain);
            bw.WriteSingle(m_positionGain);
            bw.WriteSingle(m_positionMaxLinearVelocity);
            bw.WriteSingle(m_positionMaxAngularVelocity);
            bw.WriteSingle(m_snapGain);
            bw.WriteSingle(m_snapMaxLinearVelocity);
            bw.WriteSingle(m_snapMaxAngularVelocity);
            bw.WriteSingle(m_snapMaxLinearDistance);
            bw.WriteSingle(m_snapMaxAngularDistance);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_hierarchyGain = xd.ReadSingle(xe, nameof(m_hierarchyGain));
            m_velocityDamping = xd.ReadSingle(xe, nameof(m_velocityDamping));
            m_accelerationGain = xd.ReadSingle(xe, nameof(m_accelerationGain));
            m_velocityGain = xd.ReadSingle(xe, nameof(m_velocityGain));
            m_positionGain = xd.ReadSingle(xe, nameof(m_positionGain));
            m_positionMaxLinearVelocity = xd.ReadSingle(xe, nameof(m_positionMaxLinearVelocity));
            m_positionMaxAngularVelocity = xd.ReadSingle(xe, nameof(m_positionMaxAngularVelocity));
            m_snapGain = xd.ReadSingle(xe, nameof(m_snapGain));
            m_snapMaxLinearVelocity = xd.ReadSingle(xe, nameof(m_snapMaxLinearVelocity));
            m_snapMaxAngularVelocity = xd.ReadSingle(xe, nameof(m_snapMaxAngularVelocity));
            m_snapMaxLinearDistance = xd.ReadSingle(xe, nameof(m_snapMaxLinearDistance));
            m_snapMaxAngularDistance = xd.ReadSingle(xe, nameof(m_snapMaxAngularDistance));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_hierarchyGain), m_hierarchyGain);
            xs.WriteFloat(xe, nameof(m_velocityDamping), m_velocityDamping);
            xs.WriteFloat(xe, nameof(m_accelerationGain), m_accelerationGain);
            xs.WriteFloat(xe, nameof(m_velocityGain), m_velocityGain);
            xs.WriteFloat(xe, nameof(m_positionGain), m_positionGain);
            xs.WriteFloat(xe, nameof(m_positionMaxLinearVelocity), m_positionMaxLinearVelocity);
            xs.WriteFloat(xe, nameof(m_positionMaxAngularVelocity), m_positionMaxAngularVelocity);
            xs.WriteFloat(xe, nameof(m_snapGain), m_snapGain);
            xs.WriteFloat(xe, nameof(m_snapMaxLinearVelocity), m_snapMaxLinearVelocity);
            xs.WriteFloat(xe, nameof(m_snapMaxAngularVelocity), m_snapMaxAngularVelocity);
            xs.WriteFloat(xe, nameof(m_snapMaxLinearDistance), m_snapMaxLinearDistance);
            xs.WriteFloat(xe, nameof(m_snapMaxAngularDistance), m_snapMaxAngularDistance);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaKeyFrameHierarchyUtilityControlData);
        }

        public bool Equals(hkaKeyFrameHierarchyUtilityControlData? other)
        {
            return other is not null &&
                   m_hierarchyGain.Equals(other.m_hierarchyGain) &&
                   m_velocityDamping.Equals(other.m_velocityDamping) &&
                   m_accelerationGain.Equals(other.m_accelerationGain) &&
                   m_velocityGain.Equals(other.m_velocityGain) &&
                   m_positionGain.Equals(other.m_positionGain) &&
                   m_positionMaxLinearVelocity.Equals(other.m_positionMaxLinearVelocity) &&
                   m_positionMaxAngularVelocity.Equals(other.m_positionMaxAngularVelocity) &&
                   m_snapGain.Equals(other.m_snapGain) &&
                   m_snapMaxLinearVelocity.Equals(other.m_snapMaxLinearVelocity) &&
                   m_snapMaxAngularVelocity.Equals(other.m_snapMaxAngularVelocity) &&
                   m_snapMaxLinearDistance.Equals(other.m_snapMaxLinearDistance) &&
                   m_snapMaxAngularDistance.Equals(other.m_snapMaxAngularDistance) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_hierarchyGain);
            hashcode.Add(m_velocityDamping);
            hashcode.Add(m_accelerationGain);
            hashcode.Add(m_velocityGain);
            hashcode.Add(m_positionGain);
            hashcode.Add(m_positionMaxLinearVelocity);
            hashcode.Add(m_positionMaxAngularVelocity);
            hashcode.Add(m_snapGain);
            hashcode.Add(m_snapMaxLinearVelocity);
            hashcode.Add(m_snapMaxAngularVelocity);
            hashcode.Add(m_snapMaxLinearDistance);
            hashcode.Add(m_snapMaxAngularDistance);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

