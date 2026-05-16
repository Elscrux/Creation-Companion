using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkaSkeletonMapperDataChainMapping Signatire: 0xa528f7cf size: 112 flags: FLAGS_NONE

    // m_startBoneA m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_endBoneA m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_startBoneB m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_endBoneB m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 6 flags: FLAGS_NONE enum: 
    // m_startAFromBTransform m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_endAFromBTransform m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkaSkeletonMapperDataChainMapping : IHavokObject, IEquatable<hkaSkeletonMapperDataChainMapping?>
    {
        public short m_startBoneA { set; get; }
        public short m_endBoneA { set; get; }
        public short m_startBoneB { set; get; }
        public short m_endBoneB { set; get; }
        public Matrix4x4 m_startAFromBTransform { set; get; }
        public Matrix4x4 m_endAFromBTransform { set; get; }

        public virtual uint Signature { set; get; } = 0xa528f7cf;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_startBoneA = br.ReadInt16();
            m_endBoneA = br.ReadInt16();
            m_startBoneB = br.ReadInt16();
            m_endBoneB = br.ReadInt16();
            br.Position += 8;
            m_startAFromBTransform = des.ReadQSTransform(br);
            m_endAFromBTransform = des.ReadQSTransform(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt16(m_startBoneA);
            bw.WriteInt16(m_endBoneA);
            bw.WriteInt16(m_startBoneB);
            bw.WriteInt16(m_endBoneB);
            bw.Position += 8;
            s.WriteQSTransform(bw, m_startAFromBTransform);
            s.WriteQSTransform(bw, m_endAFromBTransform);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_startBoneA = xd.ReadInt16(xe, nameof(m_startBoneA));
            m_endBoneA = xd.ReadInt16(xe, nameof(m_endBoneA));
            m_startBoneB = xd.ReadInt16(xe, nameof(m_startBoneB));
            m_endBoneB = xd.ReadInt16(xe, nameof(m_endBoneB));
            m_startAFromBTransform = xd.ReadQSTransform(xe, nameof(m_startAFromBTransform));
            m_endAFromBTransform = xd.ReadQSTransform(xe, nameof(m_endAFromBTransform));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_startBoneA), m_startBoneA);
            xs.WriteNumber(xe, nameof(m_endBoneA), m_endBoneA);
            xs.WriteNumber(xe, nameof(m_startBoneB), m_startBoneB);
            xs.WriteNumber(xe, nameof(m_endBoneB), m_endBoneB);
            xs.WriteQSTransform(xe, nameof(m_startAFromBTransform), m_startAFromBTransform);
            xs.WriteQSTransform(xe, nameof(m_endAFromBTransform), m_endAFromBTransform);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaSkeletonMapperDataChainMapping);
        }

        public bool Equals(hkaSkeletonMapperDataChainMapping? other)
        {
            return other is not null &&
                   m_startBoneA.Equals(other.m_startBoneA) &&
                   m_endBoneA.Equals(other.m_endBoneA) &&
                   m_startBoneB.Equals(other.m_startBoneB) &&
                   m_endBoneB.Equals(other.m_endBoneB) &&
                   m_startAFromBTransform.Equals(other.m_startAFromBTransform) &&
                   m_endAFromBTransform.Equals(other.m_endAFromBTransform) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_startBoneA);
            hashcode.Add(m_endBoneA);
            hashcode.Add(m_startBoneB);
            hashcode.Add(m_endBoneB);
            hashcode.Add(m_startAFromBTransform);
            hashcode.Add(m_endAFromBTransform);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

