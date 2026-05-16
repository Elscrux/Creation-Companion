using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkaDefaultAnimatedReferenceFrame Signatire: 0x6d85e445 size: 80 flags: FLAGS_NONE

    // m_up m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_forward m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_duration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_referenceFrameSamples m_class:  Type.TYPE_ARRAY Type.TYPE_VECTOR4 arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    public partial class hkaDefaultAnimatedReferenceFrame : hkaAnimatedReferenceFrame, IEquatable<hkaDefaultAnimatedReferenceFrame?>
    {
        public Vector4 m_up { set; get; }
        public Vector4 m_forward { set; get; }
        public float m_duration { set; get; }
        public IList<Vector4> m_referenceFrameSamples { set; get; } = Array.Empty<Vector4>();

        public override uint Signature { set; get; } = 0x6d85e445;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_up = br.ReadVector4();
            m_forward = br.ReadVector4();
            m_duration = br.ReadSingle();
            br.Position += 4;
            m_referenceFrameSamples = des.ReadVector4Array(br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_up);
            bw.WriteVector4(m_forward);
            bw.WriteSingle(m_duration);
            bw.Position += 4;
            s.WriteVector4Array(bw, m_referenceFrameSamples);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_up = xd.ReadVector4(xe, nameof(m_up));
            m_forward = xd.ReadVector4(xe, nameof(m_forward));
            m_duration = xd.ReadSingle(xe, nameof(m_duration));
            m_referenceFrameSamples = xd.ReadVector4Array(xe, nameof(m_referenceFrameSamples));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_up), m_up);
            xs.WriteVector4(xe, nameof(m_forward), m_forward);
            xs.WriteFloat(xe, nameof(m_duration), m_duration);
            xs.WriteVector4Array(xe, nameof(m_referenceFrameSamples), m_referenceFrameSamples);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaDefaultAnimatedReferenceFrame);
        }

        public bool Equals(hkaDefaultAnimatedReferenceFrame? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_up.Equals(other.m_up) &&
                   m_forward.Equals(other.m_forward) &&
                   m_duration.Equals(other.m_duration) &&
                   m_referenceFrameSamples.SequenceEqual(other.m_referenceFrameSamples) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_up);
            hashcode.Add(m_forward);
            hashcode.Add(m_duration);
            hashcode.Add(m_referenceFrameSamples.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

