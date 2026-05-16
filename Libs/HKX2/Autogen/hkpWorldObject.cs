using System.Xml.Linq;
namespace HKX2
{
    // hkpWorldObject Signatire: 0x49fb6f2e size: 208 flags: FLAGS_NONE

    // m_world m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 16 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_userData m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_collidable m_class: hkpLinkedCollidable Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_multiThreadCheck m_class: hkMultiThreadCheck Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    // m_properties m_class: hkpProperty Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 184 flags: FLAGS_NONE enum: 
    // m_treeData m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 200 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpWorldObject : hkReferencedObject, IEquatable<hkpWorldObject?>
    {
        private object? m_world { set; get; }
        public ulong m_userData { set; get; }
        public hkpLinkedCollidable m_collidable { set; get; } = new();
        public hkMultiThreadCheck m_multiThreadCheck { set; get; } = new();
        public string m_name { set; get; } = "";
        public IList<hkpProperty> m_properties { set; get; } = Array.Empty<hkpProperty>();
        private object? m_treeData { set; get; }

        public override uint Signature { set; get; } = 0x49fb6f2e;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            des.ReadEmptyPointer(br);
            m_userData = br.ReadUInt64();
            m_collidable.Read(des, br);
            m_multiThreadCheck.Read(des, br);
            br.Position += 4;
            m_name = des.ReadStringPointer(br);
            m_properties = des.ReadClassArray<hkpProperty>(br);
            des.ReadEmptyPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteVoidPointer(bw);
            bw.WriteUInt64(m_userData);
            m_collidable.Write(s, bw);
            m_multiThreadCheck.Write(s, bw);
            bw.Position += 4;
            s.WriteStringPointer(bw, m_name);
            s.WriteClassArray(bw, m_properties);
            s.WriteVoidPointer(bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_userData = xd.ReadUInt64(xe, nameof(m_userData));
            m_collidable = xd.ReadClass<hkpLinkedCollidable>(xe, nameof(m_collidable));
            m_multiThreadCheck = xd.ReadClass<hkMultiThreadCheck>(xe, nameof(m_multiThreadCheck));
            m_name = xd.ReadString(xe, nameof(m_name));
            m_properties = xd.ReadClassArray<hkpProperty>(xe, nameof(m_properties));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteSerializeIgnored(xe, nameof(m_world));
            xs.WriteNumber(xe, nameof(m_userData), m_userData);
            xs.WriteClass<hkpLinkedCollidable>(xe, nameof(m_collidable), m_collidable);
            xs.WriteClass(xe, nameof(m_multiThreadCheck), m_multiThreadCheck);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteClassArray(xe, nameof(m_properties), m_properties);
            xs.WriteSerializeIgnored(xe, nameof(m_treeData));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpWorldObject);
        }

        public bool Equals(hkpWorldObject? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_userData.Equals(other.m_userData) &&
                   ((m_collidable is null && other.m_collidable is null) || (m_collidable is not null && other.m_collidable is not null && m_collidable.Equals((IHavokObject)other.m_collidable))) &&
                   ((m_multiThreadCheck is null && other.m_multiThreadCheck is null) || (m_multiThreadCheck is not null && other.m_multiThreadCheck is not null && m_multiThreadCheck.Equals((IHavokObject)other.m_multiThreadCheck))) &&
                   m_name == other.m_name &&
                   m_properties.SequenceEqual(other.m_properties) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_userData);
            hashcode.Add(m_collidable);
            hashcode.Add(m_multiThreadCheck);
            hashcode.Add(m_name);
            hashcode.Add(m_properties.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

