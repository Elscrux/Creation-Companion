using System.Xml.Linq;
namespace HKX2
{
    // hkbBlendingTransitionEffect Signatire: 0xfd8584fe size: 144 flags: FLAGS_NONE

    // m_duration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_toGeneratorStartTimeFraction m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_flags m_class:  Type.TYPE_FLAGS Type.TYPE_UINT16 arrSize: 0 offset: 88 flags: FLAGS_NONE enum: FlagBits
    // m_endMode m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 90 flags: FLAGS_NONE enum: EndMode
    // m_blendCurve m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 91 flags: FLAGS_NONE enum: BlendCurve
    // m_fromGenerator m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 96 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_toGenerator m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 104 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_characterPoseAtBeginningOfTransition m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 112 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_timeRemaining m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 128 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_timeInTransition m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 132 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_applySelfTransition m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 136 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_initializeCharacterPose m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 137 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbBlendingTransitionEffect : hkbTransitionEffect, IEquatable<hkbBlendingTransitionEffect?>
    {
        public float m_duration { set; get; }
        public float m_toGeneratorStartTimeFraction { set; get; }
        public ushort m_flags { set; get; }
        public sbyte m_endMode { set; get; }
        public sbyte m_blendCurve { set; get; }
        private object? m_fromGenerator { set; get; }
        private object? m_toGenerator { set; get; }
        public IList<object> m_characterPoseAtBeginningOfTransition { set; get; } = Array.Empty<object>();
        private float m_timeRemaining { set; get; }
        private float m_timeInTransition { set; get; }
        private bool m_applySelfTransition { set; get; }
        private bool m_initializeCharacterPose { set; get; }

        public override uint Signature { set; get; } = 0xfd8584fe;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_duration = br.ReadSingle();
            m_toGeneratorStartTimeFraction = br.ReadSingle();
            m_flags = br.ReadUInt16();
            m_endMode = br.ReadSByte();
            m_blendCurve = br.ReadSByte();
            br.Position += 4;
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyArray(br);
            m_timeRemaining = br.ReadSingle();
            m_timeInTransition = br.ReadSingle();
            m_applySelfTransition = br.ReadBoolean();
            m_initializeCharacterPose = br.ReadBoolean();
            br.Position += 6;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_duration);
            bw.WriteSingle(m_toGeneratorStartTimeFraction);
            bw.WriteUInt16(m_flags);
            bw.WriteSByte(m_endMode);
            bw.WriteSByte(m_blendCurve);
            bw.Position += 4;
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidArray(bw);
            bw.WriteSingle(m_timeRemaining);
            bw.WriteSingle(m_timeInTransition);
            bw.WriteBoolean(m_applySelfTransition);
            bw.WriteBoolean(m_initializeCharacterPose);
            bw.Position += 6;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_duration = xd.ReadSingle(xe, nameof(m_duration));
            m_toGeneratorStartTimeFraction = xd.ReadSingle(xe, nameof(m_toGeneratorStartTimeFraction));
            m_flags = xd.ReadFlag<FlagBits, ushort>(xe, nameof(m_flags));
            m_endMode = xd.ReadFlag<EndMode, sbyte>(xe, nameof(m_endMode));
            m_blendCurve = xd.ReadFlag<BlendCurve, sbyte>(xe, nameof(m_blendCurve));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_duration), m_duration);
            xs.WriteFloat(xe, nameof(m_toGeneratorStartTimeFraction), m_toGeneratorStartTimeFraction);
            xs.WriteFlag<FlagBits, ushort>(xe, nameof(m_flags), m_flags);
            xs.WriteEnum<EndMode, sbyte>(xe, nameof(m_endMode), m_endMode);
            xs.WriteEnum<BlendCurve, sbyte>(xe, nameof(m_blendCurve), m_blendCurve);
            xs.WriteSerializeIgnored(xe, nameof(m_fromGenerator));
            xs.WriteSerializeIgnored(xe, nameof(m_toGenerator));
            xs.WriteSerializeIgnored(xe, nameof(m_characterPoseAtBeginningOfTransition));
            xs.WriteSerializeIgnored(xe, nameof(m_timeRemaining));
            xs.WriteSerializeIgnored(xe, nameof(m_timeInTransition));
            xs.WriteSerializeIgnored(xe, nameof(m_applySelfTransition));
            xs.WriteSerializeIgnored(xe, nameof(m_initializeCharacterPose));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBlendingTransitionEffect);
        }

        public bool Equals(hkbBlendingTransitionEffect? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_duration.Equals(other.m_duration) &&
                   m_toGeneratorStartTimeFraction.Equals(other.m_toGeneratorStartTimeFraction) &&
                   m_flags.Equals(other.m_flags) &&
                   m_endMode.Equals(other.m_endMode) &&
                   m_blendCurve.Equals(other.m_blendCurve) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_duration);
            hashcode.Add(m_toGeneratorStartTimeFraction);
            hashcode.Add(m_flags);
            hashcode.Add(m_endMode);
            hashcode.Add(m_blendCurve);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

