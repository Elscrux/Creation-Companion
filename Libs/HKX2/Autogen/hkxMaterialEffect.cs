using System.Xml.Linq;
namespace HKX2
{
    // hkxMaterialEffect Signatire: 0x1d39f925 size: 48 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 24 flags: FLAGS_NONE enum: EffectType
    // m_data m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkxMaterialEffect : hkReferencedObject, IEquatable<hkxMaterialEffect?>
    {
        public string m_name { set; get; } = "";
        public byte m_type { set; get; }
        public IList<byte> m_data { set; get; } = Array.Empty<byte>();

        public override uint Signature { set; get; } = 0x1d39f925;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_type = br.ReadByte();
            br.Position += 7;
            m_data = des.ReadByteArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_name);
            bw.WriteByte(m_type);
            bw.Position += 7;
            s.WriteByteArray(bw, m_data);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_name = xd.ReadString(xe, nameof(m_name));
            m_type = xd.ReadFlag<EffectType, byte>(xe, nameof(m_type));
            m_data = xd.ReadByteArray(xe, nameof(m_data));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteEnum<EffectType, byte>(xe, nameof(m_type), m_type);
            xs.WriteNumberArray(xe, nameof(m_data), m_data);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxMaterialEffect);
        }

        public bool Equals(hkxMaterialEffect? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_name == other.m_name &&
                   m_type.Equals(other.m_type) &&
                   m_data.SequenceEqual(other.m_data) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_name);
            hashcode.Add(m_type);
            hashcode.Add(m_data.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

