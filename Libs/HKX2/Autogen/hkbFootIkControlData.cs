using System.Xml.Linq;
namespace HKX2
{
    // hkbFootIkControlData Signatire: 0xa111b704 size: 48 flags: FLAGS_NONE

    // m_gains m_class: hkbFootIkGains Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: ALIGN_16|FLAGS_NONE enum: 
    public partial class hkbFootIkControlData : IHavokObject, IEquatable<hkbFootIkControlData?>
    {
        public hkbFootIkGains m_gains { set; get; } = new();

        public virtual uint Signature { set; get; } = 0xa111b704;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_gains.Read(des, br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_gains.Write(s, bw);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_gains = xd.ReadClass<hkbFootIkGains>(xe, nameof(m_gains));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkbFootIkGains>(xe, nameof(m_gains), m_gains);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbFootIkControlData);
        }

        public bool Equals(hkbFootIkControlData? other)
        {
            return other is not null &&
                   ((m_gains is null && other.m_gains is null) || (m_gains is not null && other.m_gains is not null && m_gains.Equals((IHavokObject)other.m_gains))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_gains);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

