using System.Xml.Linq;
namespace HKX2
{
    // hkpDisplayBindingDataPhysicsSystem Signatire: 0xc8ae86a7 size: 40 flags: FLAGS_NONE

    // m_bindings m_class: hkpDisplayBindingDataRigidBody Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_system m_class: hkpPhysicsSystem Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkpDisplayBindingDataPhysicsSystem : hkReferencedObject, IEquatable<hkpDisplayBindingDataPhysicsSystem?>
    {
        public IList<hkpDisplayBindingDataRigidBody> m_bindings { set; get; } = Array.Empty<hkpDisplayBindingDataRigidBody>();
        public hkpPhysicsSystem? m_system { set; get; }

        public override uint Signature { set; get; } = 0xc8ae86a7;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bindings = des.ReadClassPointerArray<hkpDisplayBindingDataRigidBody>(br);
            m_system = des.ReadClassPointer<hkpPhysicsSystem>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_bindings);
            s.WriteClassPointer(bw, m_system);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_bindings = xd.ReadClassPointerArray<hkpDisplayBindingDataRigidBody>(xe, nameof(m_bindings));
            m_system = xd.ReadClassPointer<hkpPhysicsSystem>(xe, nameof(m_system));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_bindings), m_bindings);
            xs.WriteClassPointer(xe, nameof(m_system), m_system);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpDisplayBindingDataPhysicsSystem);
        }

        public bool Equals(hkpDisplayBindingDataPhysicsSystem? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_bindings.SequenceEqual(other.m_bindings) &&
                   ((m_system is null && other.m_system is null) || (m_system is not null && other.m_system is not null && m_system.Equals((IHavokObject)other.m_system))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_bindings.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_system);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

