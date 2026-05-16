using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // BSDecomposeVectorModifier Signatire: 0x31f6b8b6 size: 112 flags: FLAGS_NONE

    // m_vector m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_x m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_y m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    // m_z m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_w m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 108 flags: FLAGS_NONE enum: 
    public partial class BSDecomposeVectorModifier : hkbModifier, IEquatable<BSDecomposeVectorModifier?>
    {
        public Vector4 m_vector { set; get; }
        public float m_x { set; get; }
        public float m_y { set; get; }
        public float m_z { set; get; }
        public float m_w { set; get; }

        public override uint Signature { set; get; } = 0x31f6b8b6;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_vector = br.ReadVector4();
            m_x = br.ReadSingle();
            m_y = br.ReadSingle();
            m_z = br.ReadSingle();
            m_w = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_vector);
            bw.WriteSingle(m_x);
            bw.WriteSingle(m_y);
            bw.WriteSingle(m_z);
            bw.WriteSingle(m_w);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_vector = xd.ReadVector4(xe, nameof(m_vector));
            m_x = xd.ReadSingle(xe, nameof(m_x));
            m_y = xd.ReadSingle(xe, nameof(m_y));
            m_z = xd.ReadSingle(xe, nameof(m_z));
            m_w = xd.ReadSingle(xe, nameof(m_w));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_vector), m_vector);
            xs.WriteFloat(xe, nameof(m_x), m_x);
            xs.WriteFloat(xe, nameof(m_y), m_y);
            xs.WriteFloat(xe, nameof(m_z), m_z);
            xs.WriteFloat(xe, nameof(m_w), m_w);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSDecomposeVectorModifier);
        }

        public bool Equals(BSDecomposeVectorModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_vector.Equals(other.m_vector) &&
                   m_x.Equals(other.m_x) &&
                   m_y.Equals(other.m_y) &&
                   m_z.Equals(other.m_z) &&
                   m_w.Equals(other.m_w) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_vector);
            hashcode.Add(m_x);
            hashcode.Add(m_y);
            hashcode.Add(m_z);
            hashcode.Add(m_w);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

