using System.Xml.Linq;
namespace HKX2
{
    // hkbVariableBindingSet Signatire: 0x338ad4ff size: 40 flags: FLAGS_NONE

    // m_bindings m_class: hkbVariableBindingSetBinding Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_indexOfBindingToEnable m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_hasOutputBinding m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 36 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbVariableBindingSet : hkReferencedObject, IEquatable<hkbVariableBindingSet?>
    {
        public IList<hkbVariableBindingSetBinding> m_bindings { set; get; } = Array.Empty<hkbVariableBindingSetBinding>();
        public int m_indexOfBindingToEnable { set; get; }
        private bool m_hasOutputBinding { set; get; }

        public override uint Signature { set; get; } = 0x338ad4ff;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bindings = des.ReadClassArray<hkbVariableBindingSetBinding>(br);
            m_indexOfBindingToEnable = br.ReadInt32();
            m_hasOutputBinding = br.ReadBoolean();
            br.Position += 3;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_bindings);
            bw.WriteInt32(m_indexOfBindingToEnable);
            bw.WriteBoolean(m_hasOutputBinding);
            bw.Position += 3;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_bindings = xd.ReadClassArray<hkbVariableBindingSetBinding>(xe, nameof(m_bindings));
            m_indexOfBindingToEnable = xd.ReadInt32(xe, nameof(m_indexOfBindingToEnable));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_bindings), m_bindings);
            xs.WriteNumber(xe, nameof(m_indexOfBindingToEnable), m_indexOfBindingToEnable);
            xs.WriteSerializeIgnored(xe, nameof(m_hasOutputBinding));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbVariableBindingSet);
        }

        public bool Equals(hkbVariableBindingSet? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_bindings.SequenceEqual(other.m_bindings) &&
                   m_indexOfBindingToEnable.Equals(other.m_indexOfBindingToEnable) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_bindings.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_indexOfBindingToEnable);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

