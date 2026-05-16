using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpSerializedDisplayRbTransformsDisplayTransformPair Signatire: 0x94ac5bec size: 80 flags: FLAGS_NONE

    // m_rb m_class: hkpRigidBody Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_localToDisplay m_class:  Type.TYPE_TRANSFORM Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpSerializedDisplayRbTransformsDisplayTransformPair : IHavokObject, IEquatable<hkpSerializedDisplayRbTransformsDisplayTransformPair?>
    {
        public hkpRigidBody? m_rb { set; get; }
        public Matrix4x4 m_localToDisplay { set; get; }

        public virtual uint Signature { set; get; } = 0x94ac5bec;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_rb = des.ReadClassPointer<hkpRigidBody>(br);
            br.Position += 8;
            m_localToDisplay = des.ReadTransform(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointer(bw, m_rb);
            bw.Position += 8;
            s.WriteTransform(bw, m_localToDisplay);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_rb = xd.ReadClassPointer<hkpRigidBody>(xe, nameof(m_rb));
            m_localToDisplay = xd.ReadTransform(xe, nameof(m_localToDisplay));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClassPointer(xe, nameof(m_rb), m_rb);
            xs.WriteTransform(xe, nameof(m_localToDisplay), m_localToDisplay);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSerializedDisplayRbTransformsDisplayTransformPair);
        }

        public bool Equals(hkpSerializedDisplayRbTransformsDisplayTransformPair? other)
        {
            return other is not null &&
                   ((m_rb is null && other.m_rb is null) || (m_rb is not null && other.m_rb is not null && m_rb.Equals((IHavokObject)other.m_rb))) &&
                   m_localToDisplay.Equals(other.m_localToDisplay) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_rb);
            hashcode.Add(m_localToDisplay);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

