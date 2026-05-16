using System.Xml.Linq;
namespace HKX2
{
    // hkxEnvironment Signatire: 0x41e1aa5 size: 32 flags: FLAGS_NONE

    // m_variables m_class: hkxEnvironmentVariable Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkxEnvironment : hkReferencedObject, IEquatable<hkxEnvironment?>
    {
        public IList<hkxEnvironmentVariable> m_variables { set; get; } = Array.Empty<hkxEnvironmentVariable>();

        public override uint Signature { set; get; } = 0x41e1aa5;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_variables = des.ReadClassArray<hkxEnvironmentVariable>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_variables);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_variables = xd.ReadClassArray<hkxEnvironmentVariable>(xe, nameof(m_variables));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_variables), m_variables);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxEnvironment);
        }

        public bool Equals(hkxEnvironment? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_variables.SequenceEqual(other.m_variables) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_variables.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

