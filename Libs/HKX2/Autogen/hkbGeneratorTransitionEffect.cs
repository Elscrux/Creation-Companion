using System.Xml.Linq;
namespace HKX2
{
    // hkbGeneratorTransitionEffect Signatire: 0x5f771b12 size: 144 flags: FLAGS_NONE

    // m_transitionGenerator m_class: hkbGenerator Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_blendInDuration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_blendOutDuration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 92 flags: FLAGS_NONE enum: 
    // m_syncToGeneratorStartTime m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_fromGenerator m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 104 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_toGenerator m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 112 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_timeInTransition m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 120 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_duration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 124 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_effectiveBlendInDuration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 128 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_effectiveBlendOutDuration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 132 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_toGeneratorState m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 136 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_echoTransitionGenerator m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 137 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_echoToGenerator m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 138 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_justActivated m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 139 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_updateActiveNodes m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 140 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_stage m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 141 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbGeneratorTransitionEffect : hkbTransitionEffect, IEquatable<hkbGeneratorTransitionEffect?>
    {
        public hkbGenerator? m_transitionGenerator { set; get; }
        public float m_blendInDuration { set; get; }
        public float m_blendOutDuration { set; get; }
        public bool m_syncToGeneratorStartTime { set; get; }
        private object? m_fromGenerator { set; get; }
        private object? m_toGenerator { set; get; }
        private float m_timeInTransition { set; get; }
        private float m_duration { set; get; }
        private float m_effectiveBlendInDuration { set; get; }
        private float m_effectiveBlendOutDuration { set; get; }
        private sbyte m_toGeneratorState { set; get; }
        private bool m_echoTransitionGenerator { set; get; }
        private bool m_echoToGenerator { set; get; }
        private bool m_justActivated { set; get; }
        private bool m_updateActiveNodes { set; get; }
        private sbyte m_stage { set; get; }

        public override uint Signature { set; get; } = 0x5f771b12;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transitionGenerator = des.ReadClassPointer<hkbGenerator>(br);
            m_blendInDuration = br.ReadSingle();
            m_blendOutDuration = br.ReadSingle();
            m_syncToGeneratorStartTime = br.ReadBoolean();
            br.Position += 7;
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            m_timeInTransition = br.ReadSingle();
            m_duration = br.ReadSingle();
            m_effectiveBlendInDuration = br.ReadSingle();
            m_effectiveBlendOutDuration = br.ReadSingle();
            m_toGeneratorState = br.ReadSByte();
            m_echoTransitionGenerator = br.ReadBoolean();
            m_echoToGenerator = br.ReadBoolean();
            m_justActivated = br.ReadBoolean();
            m_updateActiveNodes = br.ReadBoolean();
            m_stage = br.ReadSByte();
            br.Position += 2;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_transitionGenerator);
            bw.WriteSingle(m_blendInDuration);
            bw.WriteSingle(m_blendOutDuration);
            bw.WriteBoolean(m_syncToGeneratorStartTime);
            bw.Position += 7;
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            bw.WriteSingle(m_timeInTransition);
            bw.WriteSingle(m_duration);
            bw.WriteSingle(m_effectiveBlendInDuration);
            bw.WriteSingle(m_effectiveBlendOutDuration);
            bw.WriteSByte(m_toGeneratorState);
            bw.WriteBoolean(m_echoTransitionGenerator);
            bw.WriteBoolean(m_echoToGenerator);
            bw.WriteBoolean(m_justActivated);
            bw.WriteBoolean(m_updateActiveNodes);
            bw.WriteSByte(m_stage);
            bw.Position += 2;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_transitionGenerator = xd.ReadClassPointer<hkbGenerator>(xe, nameof(m_transitionGenerator));
            m_blendInDuration = xd.ReadSingle(xe, nameof(m_blendInDuration));
            m_blendOutDuration = xd.ReadSingle(xe, nameof(m_blendOutDuration));
            m_syncToGeneratorStartTime = xd.ReadBoolean(xe, nameof(m_syncToGeneratorStartTime));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_transitionGenerator), m_transitionGenerator);
            xs.WriteFloat(xe, nameof(m_blendInDuration), m_blendInDuration);
            xs.WriteFloat(xe, nameof(m_blendOutDuration), m_blendOutDuration);
            xs.WriteBoolean(xe, nameof(m_syncToGeneratorStartTime), m_syncToGeneratorStartTime);
            xs.WriteSerializeIgnored(xe, nameof(m_fromGenerator));
            xs.WriteSerializeIgnored(xe, nameof(m_toGenerator));
            xs.WriteSerializeIgnored(xe, nameof(m_timeInTransition));
            xs.WriteSerializeIgnored(xe, nameof(m_duration));
            xs.WriteSerializeIgnored(xe, nameof(m_effectiveBlendInDuration));
            xs.WriteSerializeIgnored(xe, nameof(m_effectiveBlendOutDuration));
            xs.WriteSerializeIgnored(xe, nameof(m_toGeneratorState));
            xs.WriteSerializeIgnored(xe, nameof(m_echoTransitionGenerator));
            xs.WriteSerializeIgnored(xe, nameof(m_echoToGenerator));
            xs.WriteSerializeIgnored(xe, nameof(m_justActivated));
            xs.WriteSerializeIgnored(xe, nameof(m_updateActiveNodes));
            xs.WriteSerializeIgnored(xe, nameof(m_stage));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbGeneratorTransitionEffect);
        }

        public bool Equals(hkbGeneratorTransitionEffect? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_transitionGenerator is null && other.m_transitionGenerator is null) || (m_transitionGenerator is not null && other.m_transitionGenerator is not null && m_transitionGenerator.Equals((IHavokObject)other.m_transitionGenerator))) &&
                   m_blendInDuration.Equals(other.m_blendInDuration) &&
                   m_blendOutDuration.Equals(other.m_blendOutDuration) &&
                   m_syncToGeneratorStartTime.Equals(other.m_syncToGeneratorStartTime) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_transitionGenerator);
            hashcode.Add(m_blendInDuration);
            hashcode.Add(m_blendOutDuration);
            hashcode.Add(m_syncToGeneratorStartTime);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

