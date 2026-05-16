using System.Xml.Linq;
namespace HKX2
{
    // hkbBindable Signatire: 0x2c1432d7 size: 48 flags: FLAGS_NONE

    // m_variableBindingSet m_class: hkbVariableBindingSet Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_cachedBindables m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 24 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_areBindablesCached m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 40 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbBindable : hkReferencedObject, IEquatable<hkbBindable?>
    {
        public hkbVariableBindingSet? m_variableBindingSet { set; get; }
        public IList<object> m_cachedBindables { set; get; } = Array.Empty<object>();
        private bool m_areBindablesCached { set; get; }

        public override uint Signature { set; get; } = 0x2c1432d7;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_variableBindingSet = des.ReadClassPointer<hkbVariableBindingSet>(br);
            des.ReadEmptyArray(br);
            m_areBindablesCached = br.ReadBoolean();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_variableBindingSet);
            s.WriteVoidArray(bw);
            bw.WriteBoolean(m_areBindablesCached);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_variableBindingSet = xd.ReadClassPointer<hkbVariableBindingSet>(xe, nameof(m_variableBindingSet));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_variableBindingSet), m_variableBindingSet);
            xs.WriteSerializeIgnored(xe, nameof(m_cachedBindables));
            xs.WriteSerializeIgnored(xe, nameof(m_areBindablesCached));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBindable);
        }

        public bool Equals(hkbBindable? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_variableBindingSet is null && other.m_variableBindingSet is null) || (m_variableBindingSet is not null && other.m_variableBindingSet is not null && m_variableBindingSet.Equals((IHavokObject)other.m_variableBindingSet))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_variableBindingSet);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

