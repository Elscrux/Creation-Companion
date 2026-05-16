using System.Xml.Linq;
namespace HKX2
{
    // hkbTestStateChooser Signatire: 0xc0fcc436 size: 32 flags: FLAGS_NONE

    // m_int m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_real m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 20 flags: FLAGS_NONE enum: 
    // m_string m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    public partial class hkbTestStateChooser : hkbStateChooser, IEquatable<hkbTestStateChooser?>
    {
        public int m_int { set; get; }
        public float m_real { set; get; }
        public string m_string { set; get; } = "";

        public override uint Signature { set; get; } = 0xc0fcc436;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_int = br.ReadInt32();
            m_real = br.ReadSingle();
            m_string = des.ReadStringPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_int);
            bw.WriteSingle(m_real);
            s.WriteStringPointer(bw, m_string);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_int = xd.ReadInt32(xe, nameof(m_int));
            m_real = xd.ReadSingle(xe, nameof(m_real));
            m_string = xd.ReadString(xe, nameof(m_string));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_int), m_int);
            xs.WriteFloat(xe, nameof(m_real), m_real);
            xs.WriteString(xe, nameof(m_string), m_string);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbTestStateChooser);
        }

        public bool Equals(hkbTestStateChooser? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_int.Equals(other.m_int) &&
                   m_real.Equals(other.m_real) &&
                   (m_string is null && other.m_string is null || m_string == other.m_string || m_string is null && other.m_string == "" || m_string == "" && other.m_string is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_int);
            hashcode.Add(m_real);
            hashcode.Add(m_string);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

