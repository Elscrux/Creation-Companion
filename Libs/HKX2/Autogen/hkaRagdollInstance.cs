using System.Xml.Linq;
namespace HKX2
{
    // hkaRagdollInstance Signatire: 0x154948e8 size: 72 flags: FLAGS_NONE

    // m_rigidBodies m_class: hkpRigidBody Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_constraints m_class: hkpConstraintInstance Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_boneToRigidBodyMap m_class:  Type.TYPE_ARRAY Type.TYPE_INT32 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_skeleton m_class: hkaSkeleton Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkaRagdollInstance : hkReferencedObject, IEquatable<hkaRagdollInstance?>
    {
        public IList<hkpRigidBody> m_rigidBodies { set; get; } = Array.Empty<hkpRigidBody>();
        public IList<hkpConstraintInstance> m_constraints { set; get; } = Array.Empty<hkpConstraintInstance>();
        public IList<int> m_boneToRigidBodyMap { set; get; } = Array.Empty<int>();
        public hkaSkeleton? m_skeleton { set; get; }

        public override uint Signature { set; get; } = 0x154948e8;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rigidBodies = des.ReadClassPointerArray<hkpRigidBody>(br);
            m_constraints = des.ReadClassPointerArray<hkpConstraintInstance>(br);
            m_boneToRigidBodyMap = des.ReadInt32Array(br);
            m_skeleton = des.ReadClassPointer<hkaSkeleton>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_rigidBodies);
            s.WriteClassPointerArray(bw, m_constraints);
            s.WriteInt32Array(bw, m_boneToRigidBodyMap);
            s.WriteClassPointer(bw, m_skeleton);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_rigidBodies = xd.ReadClassPointerArray<hkpRigidBody>(xe, nameof(m_rigidBodies));
            m_constraints = xd.ReadClassPointerArray<hkpConstraintInstance>(xe, nameof(m_constraints));
            m_boneToRigidBodyMap = xd.ReadInt32Array(xe, nameof(m_boneToRigidBodyMap));
            m_skeleton = xd.ReadClassPointer<hkaSkeleton>(xe, nameof(m_skeleton));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_rigidBodies), m_rigidBodies);
            xs.WriteClassPointerArray(xe, nameof(m_constraints), m_constraints);
            xs.WriteNumberArray(xe, nameof(m_boneToRigidBodyMap), m_boneToRigidBodyMap);
            xs.WriteClassPointer(xe, nameof(m_skeleton), m_skeleton);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaRagdollInstance);
        }

        public bool Equals(hkaRagdollInstance? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_rigidBodies.SequenceEqual(other.m_rigidBodies) &&
                   m_constraints.SequenceEqual(other.m_constraints) &&
                   m_boneToRigidBodyMap.SequenceEqual(other.m_boneToRigidBodyMap) &&
                   ((m_skeleton is null && other.m_skeleton is null) || (m_skeleton is not null && other.m_skeleton is not null && m_skeleton.Equals((IHavokObject)other.m_skeleton))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_rigidBodies.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_constraints.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_boneToRigidBodyMap.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_skeleton);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

