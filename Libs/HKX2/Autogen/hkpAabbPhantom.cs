using System.Xml.Linq;
namespace HKX2
{
    // hkpAabbPhantom Signatire: 0x2c5189dd size: 304 flags: FLAGS_NONE

    // m_aabb m_class: hkAabb Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 240 flags: FLAGS_NONE enum: 
    // m_overlappingCollidables m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 272 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_orderDirty m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 288 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpAabbPhantom : hkpPhantom, IEquatable<hkpAabbPhantom?>
    {
        public hkAabb m_aabb { set; get; } = new();
        public IList<object> m_overlappingCollidables { set; get; } = Array.Empty<object>();
        private bool m_orderDirty { set; get; }

        public override uint Signature { set; get; } = 0x2c5189dd;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_aabb.Read(des, br);
            des.ReadEmptyArray(br);
            m_orderDirty = br.ReadBoolean();
            br.Position += 15;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_aabb.Write(s, bw);
            s.WriteVoidArray(bw);
            bw.WriteBoolean(m_orderDirty);
            bw.Position += 15;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_aabb = xd.ReadClass<hkAabb>(xe, nameof(m_aabb));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkAabb>(xe, nameof(m_aabb), m_aabb);
            xs.WriteSerializeIgnored(xe, nameof(m_overlappingCollidables));
            xs.WriteSerializeIgnored(xe, nameof(m_orderDirty));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpAabbPhantom);
        }

        public bool Equals(hkpAabbPhantom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_aabb is null && other.m_aabb is null) || (m_aabb is not null && other.m_aabb is not null && m_aabb.Equals((IHavokObject)other.m_aabb))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_aabb);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

