using System.Xml.Linq;
namespace HKX2
{
    // BSiStateTaggingGenerator Signatire: 0xf0826fc1 size: 96 flags: FLAGS_NONE

    // m_pDefaultGenerator m_class: hkbGenerator Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_iStateToSetAs m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_iPriority m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 92 flags: FLAGS_NONE enum: 
    public partial class BSiStateTaggingGenerator : hkbGenerator, IEquatable<BSiStateTaggingGenerator?>
    {
        public hkbGenerator? m_pDefaultGenerator { set; get; }
        public int m_iStateToSetAs { set; get; }
        public int m_iPriority { set; get; }

        public override uint Signature { set; get; } = 0xf0826fc1;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_pDefaultGenerator = des.ReadClassPointer<hkbGenerator>(br);
            m_iStateToSetAs = br.ReadInt32();
            m_iPriority = br.ReadInt32();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            s.WriteClassPointer(bw, m_pDefaultGenerator);
            bw.WriteInt32(m_iStateToSetAs);
            bw.WriteInt32(m_iPriority);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_pDefaultGenerator = xd.ReadClassPointer<hkbGenerator>(xe, nameof(m_pDefaultGenerator));
            m_iStateToSetAs = xd.ReadInt32(xe, nameof(m_iStateToSetAs));
            m_iPriority = xd.ReadInt32(xe, nameof(m_iPriority));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_pDefaultGenerator), m_pDefaultGenerator);
            xs.WriteNumber(xe, nameof(m_iStateToSetAs), m_iStateToSetAs);
            xs.WriteNumber(xe, nameof(m_iPriority), m_iPriority);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSiStateTaggingGenerator);
        }

        public bool Equals(BSiStateTaggingGenerator? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_pDefaultGenerator is null && other.m_pDefaultGenerator is null) || (m_pDefaultGenerator is not null && other.m_pDefaultGenerator is not null && m_pDefaultGenerator.Equals((IHavokObject)other.m_pDefaultGenerator))) &&
                   m_iStateToSetAs.Equals(other.m_iStateToSetAs) &&
                   m_iPriority.Equals(other.m_iPriority) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_pDefaultGenerator);
            hashcode.Add(m_iStateToSetAs);
            hashcode.Add(m_iPriority);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

