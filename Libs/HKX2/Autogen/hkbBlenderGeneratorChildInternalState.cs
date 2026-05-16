using System.Xml.Linq;
namespace HKX2
{
    // hkbBlenderGeneratorChildInternalState Signatire: 0xff7327c0 size: 2 flags: FLAGS_NONE

    // m_isActive m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_syncNextFrame m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 1 flags: FLAGS_NONE enum: 
    public partial class hkbBlenderGeneratorChildInternalState : IHavokObject, IEquatable<hkbBlenderGeneratorChildInternalState?>
    {
        public bool m_isActive { set; get; }
        public bool m_syncNextFrame { set; get; }

        public virtual uint Signature { set; get; } = 0xff7327c0;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_isActive = br.ReadBoolean();
            m_syncNextFrame = br.ReadBoolean();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteBoolean(m_isActive);
            bw.WriteBoolean(m_syncNextFrame);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_isActive = xd.ReadBoolean(xe, nameof(m_isActive));
            m_syncNextFrame = xd.ReadBoolean(xe, nameof(m_syncNextFrame));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteBoolean(xe, nameof(m_isActive), m_isActive);
            xs.WriteBoolean(xe, nameof(m_syncNextFrame), m_syncNextFrame);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBlenderGeneratorChildInternalState);
        }

        public bool Equals(hkbBlenderGeneratorChildInternalState? other)
        {
            return other is not null &&
                   m_isActive.Equals(other.m_isActive) &&
                   m_syncNextFrame.Equals(other.m_syncNextFrame) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_isActive);
            hashcode.Add(m_syncNextFrame);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

