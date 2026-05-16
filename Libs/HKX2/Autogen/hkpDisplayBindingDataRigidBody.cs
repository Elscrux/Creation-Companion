using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpDisplayBindingDataRigidBody Signatire: 0xfe16e2a3 size: 96 flags: FLAGS_NONE

    // m_rigidBody m_class: hkpRigidBody Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_displayObjectPtr m_class: hkReferencedObject Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_rigidBodyFromDisplayObjectTransform m_class:  Type.TYPE_MATRIX4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkpDisplayBindingDataRigidBody : hkReferencedObject, IEquatable<hkpDisplayBindingDataRigidBody?>
    {
        public hkpRigidBody? m_rigidBody { set; get; }
        public hkReferencedObject? m_displayObjectPtr { set; get; }
        public Matrix4x4 m_rigidBodyFromDisplayObjectTransform { set; get; }

        public override uint Signature { set; get; } = 0xfe16e2a3;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rigidBody = des.ReadClassPointer<hkpRigidBody>(br);
            m_displayObjectPtr = des.ReadClassPointer<hkReferencedObject>(br);
            m_rigidBodyFromDisplayObjectTransform = des.ReadMatrix4(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_rigidBody);
            s.WriteClassPointer(bw, m_displayObjectPtr);
            s.WriteMatrix4(bw, m_rigidBodyFromDisplayObjectTransform);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_rigidBody = xd.ReadClassPointer<hkpRigidBody>(xe, nameof(m_rigidBody));
            m_displayObjectPtr = xd.ReadClassPointer<hkReferencedObject>(xe, nameof(m_displayObjectPtr));
            m_rigidBodyFromDisplayObjectTransform = xd.ReadMatrix4(xe, nameof(m_rigidBodyFromDisplayObjectTransform));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_rigidBody), m_rigidBody);
            xs.WriteClassPointer(xe, nameof(m_displayObjectPtr), m_displayObjectPtr);
            xs.WriteMatrix4(xe, nameof(m_rigidBodyFromDisplayObjectTransform), m_rigidBodyFromDisplayObjectTransform);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpDisplayBindingDataRigidBody);
        }

        public bool Equals(hkpDisplayBindingDataRigidBody? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_rigidBody is null && other.m_rigidBody is null) || (m_rigidBody is not null && other.m_rigidBody is not null && m_rigidBody.Equals((IHavokObject)other.m_rigidBody))) &&
                   ((m_displayObjectPtr is null && other.m_displayObjectPtr is null) || (m_displayObjectPtr is not null && other.m_displayObjectPtr is not null && m_displayObjectPtr.Equals((IHavokObject)other.m_displayObjectPtr))) &&
                   m_rigidBodyFromDisplayObjectTransform.Equals(other.m_rigidBodyFromDisplayObjectTransform) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_rigidBody);
            hashcode.Add(m_displayObjectPtr);
            hashcode.Add(m_rigidBodyFromDisplayObjectTransform);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

