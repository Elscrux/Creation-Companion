using System.Xml.Linq;
namespace HKX2
{
    // BGSGamebryoSequenceGenerator Signatire: 0xc8df2d77 size: 112 flags: FLAGS_NONE

    // m_pSequence m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_eBlendModeFunction m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 80 flags: FLAGS_NONE enum: BlendModeFunction
    // m_fPercent m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_events m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 88 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_fTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 104 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_bDelayedActivate m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 108 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_bLooping m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 109 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class BGSGamebryoSequenceGenerator : hkbGenerator, IEquatable<BGSGamebryoSequenceGenerator?>
    {
        public string m_pSequence { set; get; } = "";
        public sbyte m_eBlendModeFunction { set; get; }
        public float m_fPercent { set; get; }
        public IList<object> m_events { set; get; } = Array.Empty<object>();
        private float m_fTime { set; get; }
        private bool m_bDelayedActivate { set; get; }
        private bool m_bLooping { set; get; }

        public override uint Signature { set; get; } = 0xc8df2d77;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_pSequence = des.ReadCString(br);
            m_eBlendModeFunction = br.ReadSByte();
            br.Position += 3;
            m_fPercent = br.ReadSingle();
            des.ReadEmptyArray(br);
            m_fTime = br.ReadSingle();
            m_bDelayedActivate = br.ReadBoolean();
            m_bLooping = br.ReadBoolean();
            br.Position += 2;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteCString(bw, m_pSequence);
            bw.WriteSByte(m_eBlendModeFunction);
            bw.Position += 3;
            bw.WriteSingle(m_fPercent);
            s.WriteVoidArray(bw);
            bw.WriteSingle(m_fTime);
            bw.WriteBoolean(m_bDelayedActivate);
            bw.WriteBoolean(m_bLooping);
            bw.Position += 2;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_pSequence = xd.ReadString(xe, nameof(m_pSequence));
            m_eBlendModeFunction = xd.ReadFlag<BlendModeFunction, sbyte>(xe, nameof(m_eBlendModeFunction));
            m_fPercent = xd.ReadSingle(xe, nameof(m_fPercent));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_pSequence), m_pSequence);
            xs.WriteEnum<BlendModeFunction, sbyte>(xe, nameof(m_eBlendModeFunction), m_eBlendModeFunction);
            xs.WriteFloat(xe, nameof(m_fPercent), m_fPercent);
            xs.WriteSerializeIgnored(xe, nameof(m_events));
            xs.WriteSerializeIgnored(xe, nameof(m_fTime));
            xs.WriteSerializeIgnored(xe, nameof(m_bDelayedActivate));
            xs.WriteSerializeIgnored(xe, nameof(m_bLooping));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BGSGamebryoSequenceGenerator);
        }

        public bool Equals(BGSGamebryoSequenceGenerator? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   (m_pSequence is null && other.m_pSequence is null || m_pSequence == other.m_pSequence || m_pSequence is null && other.m_pSequence == "" || m_pSequence == "" && other.m_pSequence is null) &&
                   m_eBlendModeFunction.Equals(other.m_eBlendModeFunction) &&
                   m_fPercent.Equals(other.m_fPercent) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_pSequence);
            hashcode.Add(m_eBlendModeFunction);
            hashcode.Add(m_fPercent);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

