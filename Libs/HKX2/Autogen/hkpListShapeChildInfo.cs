using System.Xml.Linq;
namespace HKX2
{
    // hkpListShapeChildInfo Signatire: 0x80df0f90 size: 32 flags: FLAGS_NONE

    // m_shape m_class: hkpShape Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 0 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_collisionFilterInfo m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_shapeSize m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 12 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_numChildShapes m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 16 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpListShapeChildInfo : IHavokObject, IEquatable<hkpListShapeChildInfo?>
    {
        public hkpShape? m_shape { set; get; }
        public uint m_collisionFilterInfo { set; get; }
        private int m_shapeSize { set; get; }
        private int m_numChildShapes { set; get; }

        public virtual uint Signature { set; get; } = 0x80df0f90;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_shape = des.ReadClassPointer<hkpShape>(br);
            m_collisionFilterInfo = br.ReadUInt32();
            m_shapeSize = br.ReadInt32();
            m_numChildShapes = br.ReadInt32();
            br.Position += 12;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointer(bw, m_shape);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.WriteInt32(m_shapeSize);
            bw.WriteInt32(m_numChildShapes);
            bw.Position += 12;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_shape = xd.ReadClassPointer<hkpShape>(xe, nameof(m_shape));
            m_collisionFilterInfo = xd.ReadUInt32(xe, nameof(m_collisionFilterInfo));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClassPointer(xe, nameof(m_shape), m_shape);
            xs.WriteNumber(xe, nameof(m_collisionFilterInfo), m_collisionFilterInfo);
            xs.WriteSerializeIgnored(xe, nameof(m_shapeSize));
            xs.WriteSerializeIgnored(xe, nameof(m_numChildShapes));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpListShapeChildInfo);
        }

        public bool Equals(hkpListShapeChildInfo? other)
        {
            return other is not null &&
                   ((m_shape is null && other.m_shape is null) || (m_shape is not null && other.m_shape is not null && m_shape.Equals((IHavokObject)other.m_shape))) &&
                   m_collisionFilterInfo.Equals(other.m_collisionFilterInfo) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_shape);
            hashcode.Add(m_collisionFilterInfo);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

