using System.Xml.Linq;
namespace HKX2
{
    // hkpSimpleShapePhantomCollisionDetail Signatire: 0x98bfa6ce size: 8 flags: FLAGS_NOT_SERIALIZABLE

    // m_collidable m_class: hkpCollidable Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkpSimpleShapePhantomCollisionDetail : IHavokObject, IEquatable<hkpSimpleShapePhantomCollisionDetail?>
    {
        public hkpCollidable? m_collidable { set; get; }

        public virtual uint Signature { set; get; } = 0x98bfa6ce;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_collidable = des.ReadClassPointer<hkpCollidable>(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointer(bw, m_collidable);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_collidable = xd.ReadClassPointer<hkpCollidable>(xe, nameof(m_collidable));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClassPointer(xe, nameof(m_collidable), m_collidable);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSimpleShapePhantomCollisionDetail);
        }

        public bool Equals(hkpSimpleShapePhantomCollisionDetail? other)
        {
            return other is not null &&
                   ((m_collidable is null && other.m_collidable is null) || (m_collidable is not null && other.m_collidable is not null && m_collidable.Equals((IHavokObject)other.m_collidable))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_collidable);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

