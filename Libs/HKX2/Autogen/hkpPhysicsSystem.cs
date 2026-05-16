using System.Xml.Linq;
namespace HKX2
{
    // hkpPhysicsSystem Signatire: 0xff724c17 size: 104 flags: FLAGS_NONE

    // m_rigidBodies m_class: hkpRigidBody Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_constraints m_class: hkpConstraintInstance Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_actions m_class: hkpAction Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_phantoms m_class: hkpPhantom Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_userData m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_active m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    public partial class hkpPhysicsSystem : hkReferencedObject, IEquatable<hkpPhysicsSystem?>
    {
        public IList<hkpRigidBody> m_rigidBodies { set; get; } = Array.Empty<hkpRigidBody>();
        public IList<hkpConstraintInstance> m_constraints { set; get; } = Array.Empty<hkpConstraintInstance>();
        public IList<hkpAction> m_actions { set; get; } = Array.Empty<hkpAction>();
        public IList<hkpPhantom> m_phantoms { set; get; } = Array.Empty<hkpPhantom>();
        public string m_name { set; get; } = "";
        public ulong m_userData { set; get; }
        public bool m_active { set; get; }

        public override uint Signature { set; get; } = 0xff724c17;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rigidBodies = des.ReadClassPointerArray<hkpRigidBody>(br);
            m_constraints = des.ReadClassPointerArray<hkpConstraintInstance>(br);
            m_actions = des.ReadClassPointerArray<hkpAction>(br);
            m_phantoms = des.ReadClassPointerArray<hkpPhantom>(br);
            m_name = des.ReadStringPointer(br);
            m_userData = br.ReadUInt64();
            m_active = br.ReadBoolean();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_rigidBodies);
            s.WriteClassPointerArray(bw, m_constraints);
            s.WriteClassPointerArray(bw, m_actions);
            s.WriteClassPointerArray(bw, m_phantoms);
            s.WriteStringPointer(bw, m_name);
            bw.WriteUInt64(m_userData);
            bw.WriteBoolean(m_active);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_rigidBodies = xd.ReadClassPointerArray<hkpRigidBody>(xe, nameof(m_rigidBodies));
            m_constraints = xd.ReadClassPointerArray<hkpConstraintInstance>(xe, nameof(m_constraints));
            m_actions = xd.ReadClassPointerArray<hkpAction>(xe, nameof(m_actions));
            m_phantoms = xd.ReadClassPointerArray<hkpPhantom>(xe, nameof(m_phantoms));
            m_name = xd.ReadString(xe, nameof(m_name));
            m_userData = xd.ReadUInt64(xe, nameof(m_userData));
            m_active = xd.ReadBoolean(xe, nameof(m_active));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_rigidBodies), m_rigidBodies);
            xs.WriteClassPointerArray(xe, nameof(m_constraints), m_constraints);
            xs.WriteClassPointerArray(xe, nameof(m_actions), m_actions);
            xs.WriteClassPointerArray(xe, nameof(m_phantoms), m_phantoms);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteNumber(xe, nameof(m_userData), m_userData);
            xs.WriteBoolean(xe, nameof(m_active), m_active);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPhysicsSystem);
        }

        public bool Equals(hkpPhysicsSystem? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_rigidBodies.SequenceEqual(other.m_rigidBodies) &&
                   m_constraints.SequenceEqual(other.m_constraints) &&
                   m_actions.SequenceEqual(other.m_actions) &&
                   m_phantoms.SequenceEqual(other.m_phantoms) &&
                   m_name == other.m_name &&
                   m_userData.Equals(other.m_userData) &&
                   m_active.Equals(other.m_active) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_rigidBodies.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_constraints.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_actions.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_phantoms.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_name);
            hashcode.Add(m_userData);
            hashcode.Add(m_active);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

