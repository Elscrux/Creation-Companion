using System.Xml.Linq;
namespace HKX2
{
    // hkpCdBody Signatire: 0x54a4b841 size: 32 flags: FLAGS_NONE

    // m_shape m_class: hkpShape Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_shapeKey m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_motion m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 16 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_parent m_class: hkpCdBody Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 24 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpCdBody : IHavokObject, IEquatable<hkpCdBody?>
    {
        public hkpShape? m_shape { set; get; }
        public uint m_shapeKey { set; get; }
        private object? m_motion { set; get; }
        private hkpCdBody? m_parent { set; get; }

        public virtual uint Signature { set; get; } = 0x54a4b841;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_shape = des.ReadClassPointer<hkpShape>(br);
            m_shapeKey = br.ReadUInt32();
            br.Position += 4;
            des.ReadEmptyPointer(br);
            m_parent = des.ReadClassPointer<hkpCdBody>(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointer(bw, m_shape);
            bw.WriteUInt32(m_shapeKey);
            bw.Position += 4;
            s.WriteVoidPointer(bw);
            s.WriteClassPointer(bw, m_parent);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_shape = xd.ReadClassPointer<hkpShape>(xe, nameof(m_shape));
            m_shapeKey = xd.ReadUInt32(xe, nameof(m_shapeKey));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClassPointer(xe, nameof(m_shape), m_shape);
            xs.WriteNumber(xe, nameof(m_shapeKey), m_shapeKey);
            xs.WriteSerializeIgnored(xe, nameof(m_motion));
            xs.WriteSerializeIgnored(xe, nameof(m_parent));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCdBody);
        }

        public bool Equals(hkpCdBody? other)
        {
            return other is not null &&
                   ((m_shape is null && other.m_shape is null) || (m_shape is not null && other.m_shape is not null && m_shape.Equals((IHavokObject)other.m_shape))) &&
                   m_shapeKey.Equals(other.m_shapeKey) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_shape);
            hashcode.Add(m_shapeKey);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

