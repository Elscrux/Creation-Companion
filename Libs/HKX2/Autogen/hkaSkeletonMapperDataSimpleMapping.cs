using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkaSkeletonMapperDataSimpleMapping Signatire: 0x3405deca size: 64 flags: FLAGS_NONE

    // m_boneA m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_boneB m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_aFromBTransform m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkaSkeletonMapperDataSimpleMapping : IHavokObject, IEquatable<hkaSkeletonMapperDataSimpleMapping?>
    {
        public short m_boneA { set; get; }
        public short m_boneB { set; get; }
        public Matrix4x4 m_aFromBTransform { set; get; }

        public virtual uint Signature { set; get; } = 0x3405deca;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_boneA = br.ReadInt16();
            m_boneB = br.ReadInt16();
            br.Position += 12;
            m_aFromBTransform = des.ReadQSTransform(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt16(m_boneA);
            bw.WriteInt16(m_boneB);
            bw.Position += 12;
            s.WriteQSTransform(bw, m_aFromBTransform);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_boneA = xd.ReadInt16(xe, nameof(m_boneA));
            m_boneB = xd.ReadInt16(xe, nameof(m_boneB));
            m_aFromBTransform = xd.ReadQSTransform(xe, nameof(m_aFromBTransform));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_boneA), m_boneA);
            xs.WriteNumber(xe, nameof(m_boneB), m_boneB);
            xs.WriteQSTransform(xe, nameof(m_aFromBTransform), m_aFromBTransform);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaSkeletonMapperDataSimpleMapping);
        }

        public bool Equals(hkaSkeletonMapperDataSimpleMapping? other)
        {
            return other is not null &&
                   m_boneA.Equals(other.m_boneA) &&
                   m_boneB.Equals(other.m_boneB) &&
                   m_aFromBTransform.Equals(other.m_aFromBTransform) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_boneA);
            hashcode.Add(m_boneB);
            hashcode.Add(m_aFromBTransform);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

