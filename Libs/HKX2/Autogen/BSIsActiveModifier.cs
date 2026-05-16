using System.Xml.Linq;
namespace HKX2
{
    // BSIsActiveModifier Signatire: 0xb0fde45a size: 96 flags: FLAGS_NONE

    // m_bIsActive0 m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_bInvertActive0 m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 81 flags: FLAGS_NONE enum: 
    // m_bIsActive1 m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 82 flags: FLAGS_NONE enum: 
    // m_bInvertActive1 m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 83 flags: FLAGS_NONE enum: 
    // m_bIsActive2 m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_bInvertActive2 m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 85 flags: FLAGS_NONE enum: 
    // m_bIsActive3 m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 86 flags: FLAGS_NONE enum: 
    // m_bInvertActive3 m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 87 flags: FLAGS_NONE enum: 
    // m_bIsActive4 m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_bInvertActive4 m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 89 flags: FLAGS_NONE enum: 
    public partial class BSIsActiveModifier : hkbModifier, IEquatable<BSIsActiveModifier?>
    {
        public bool m_bIsActive0 { set; get; }
        public bool m_bInvertActive0 { set; get; }
        public bool m_bIsActive1 { set; get; }
        public bool m_bInvertActive1 { set; get; }
        public bool m_bIsActive2 { set; get; }
        public bool m_bInvertActive2 { set; get; }
        public bool m_bIsActive3 { set; get; }
        public bool m_bInvertActive3 { set; get; }
        public bool m_bIsActive4 { set; get; }
        public bool m_bInvertActive4 { set; get; }

        public override uint Signature { set; get; } = 0xb0fde45a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bIsActive0 = br.ReadBoolean();
            m_bInvertActive0 = br.ReadBoolean();
            m_bIsActive1 = br.ReadBoolean();
            m_bInvertActive1 = br.ReadBoolean();
            m_bIsActive2 = br.ReadBoolean();
            m_bInvertActive2 = br.ReadBoolean();
            m_bIsActive3 = br.ReadBoolean();
            m_bInvertActive3 = br.ReadBoolean();
            m_bIsActive4 = br.ReadBoolean();
            m_bInvertActive4 = br.ReadBoolean();
            br.Position += 6;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteBoolean(m_bIsActive0);
            bw.WriteBoolean(m_bInvertActive0);
            bw.WriteBoolean(m_bIsActive1);
            bw.WriteBoolean(m_bInvertActive1);
            bw.WriteBoolean(m_bIsActive2);
            bw.WriteBoolean(m_bInvertActive2);
            bw.WriteBoolean(m_bIsActive3);
            bw.WriteBoolean(m_bInvertActive3);
            bw.WriteBoolean(m_bIsActive4);
            bw.WriteBoolean(m_bInvertActive4);
            bw.Position += 6;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_bIsActive0 = xd.ReadBoolean(xe, nameof(m_bIsActive0));
            m_bInvertActive0 = xd.ReadBoolean(xe, nameof(m_bInvertActive0));
            m_bIsActive1 = xd.ReadBoolean(xe, nameof(m_bIsActive1));
            m_bInvertActive1 = xd.ReadBoolean(xe, nameof(m_bInvertActive1));
            m_bIsActive2 = xd.ReadBoolean(xe, nameof(m_bIsActive2));
            m_bInvertActive2 = xd.ReadBoolean(xe, nameof(m_bInvertActive2));
            m_bIsActive3 = xd.ReadBoolean(xe, nameof(m_bIsActive3));
            m_bInvertActive3 = xd.ReadBoolean(xe, nameof(m_bInvertActive3));
            m_bIsActive4 = xd.ReadBoolean(xe, nameof(m_bIsActive4));
            m_bInvertActive4 = xd.ReadBoolean(xe, nameof(m_bInvertActive4));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteBoolean(xe, nameof(m_bIsActive0), m_bIsActive0);
            xs.WriteBoolean(xe, nameof(m_bInvertActive0), m_bInvertActive0);
            xs.WriteBoolean(xe, nameof(m_bIsActive1), m_bIsActive1);
            xs.WriteBoolean(xe, nameof(m_bInvertActive1), m_bInvertActive1);
            xs.WriteBoolean(xe, nameof(m_bIsActive2), m_bIsActive2);
            xs.WriteBoolean(xe, nameof(m_bInvertActive2), m_bInvertActive2);
            xs.WriteBoolean(xe, nameof(m_bIsActive3), m_bIsActive3);
            xs.WriteBoolean(xe, nameof(m_bInvertActive3), m_bInvertActive3);
            xs.WriteBoolean(xe, nameof(m_bIsActive4), m_bIsActive4);
            xs.WriteBoolean(xe, nameof(m_bInvertActive4), m_bInvertActive4);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSIsActiveModifier);
        }

        public bool Equals(BSIsActiveModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_bIsActive0.Equals(other.m_bIsActive0) &&
                   m_bInvertActive0.Equals(other.m_bInvertActive0) &&
                   m_bIsActive1.Equals(other.m_bIsActive1) &&
                   m_bInvertActive1.Equals(other.m_bInvertActive1) &&
                   m_bIsActive2.Equals(other.m_bIsActive2) &&
                   m_bInvertActive2.Equals(other.m_bInvertActive2) &&
                   m_bIsActive3.Equals(other.m_bIsActive3) &&
                   m_bInvertActive3.Equals(other.m_bInvertActive3) &&
                   m_bIsActive4.Equals(other.m_bIsActive4) &&
                   m_bInvertActive4.Equals(other.m_bInvertActive4) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_bIsActive0);
            hashcode.Add(m_bInvertActive0);
            hashcode.Add(m_bIsActive1);
            hashcode.Add(m_bInvertActive1);
            hashcode.Add(m_bIsActive2);
            hashcode.Add(m_bInvertActive2);
            hashcode.Add(m_bIsActive3);
            hashcode.Add(m_bInvertActive3);
            hashcode.Add(m_bIsActive4);
            hashcode.Add(m_bInvertActive4);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

