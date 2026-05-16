using System.Xml.Linq;
namespace HKX2
{
    // BSLimbIKModifier Signatire: 0x8ea971e5 size: 120 flags: FLAGS_NONE

    // m_limitAngleDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_currentAngle m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_startBoneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_endBoneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 90 flags: FLAGS_NONE enum: 
    // m_gain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 92 flags: FLAGS_NONE enum: 
    // m_boneRadius m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_castOffset m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    // m_timeStep m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 104 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_pSkeletonMemory m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 112 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class BSLimbIKModifier : hkbModifier, IEquatable<BSLimbIKModifier?>
    {
        public float m_limitAngleDegrees { set; get; }
        private float m_currentAngle { set; get; }
        public short m_startBoneIndex { set; get; }
        public short m_endBoneIndex { set; get; }
        public float m_gain { set; get; }
        public float m_boneRadius { set; get; }
        public float m_castOffset { set; get; }
        private float m_timeStep { set; get; }
        private object? m_pSkeletonMemory { set; get; }

        public override uint Signature { set; get; } = 0x8ea971e5;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_limitAngleDegrees = br.ReadSingle();
            m_currentAngle = br.ReadSingle();
            m_startBoneIndex = br.ReadInt16();
            m_endBoneIndex = br.ReadInt16();
            m_gain = br.ReadSingle();
            m_boneRadius = br.ReadSingle();
            m_castOffset = br.ReadSingle();
            m_timeStep = br.ReadSingle();
            br.Position += 4;
            des.ReadEmptyPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_limitAngleDegrees);
            bw.WriteSingle(m_currentAngle);
            bw.WriteInt16(m_startBoneIndex);
            bw.WriteInt16(m_endBoneIndex);
            bw.WriteSingle(m_gain);
            bw.WriteSingle(m_boneRadius);
            bw.WriteSingle(m_castOffset);
            bw.WriteSingle(m_timeStep);
            bw.Position += 4;
            s.WriteVoidPointer(bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_limitAngleDegrees = xd.ReadSingle(xe, nameof(m_limitAngleDegrees));
            m_startBoneIndex = xd.ReadInt16(xe, nameof(m_startBoneIndex));
            m_endBoneIndex = xd.ReadInt16(xe, nameof(m_endBoneIndex));
            m_gain = xd.ReadSingle(xe, nameof(m_gain));
            m_boneRadius = xd.ReadSingle(xe, nameof(m_boneRadius));
            m_castOffset = xd.ReadSingle(xe, nameof(m_castOffset));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_limitAngleDegrees), m_limitAngleDegrees);
            xs.WriteSerializeIgnored(xe, nameof(m_currentAngle));
            xs.WriteNumber(xe, nameof(m_startBoneIndex), m_startBoneIndex);
            xs.WriteNumber(xe, nameof(m_endBoneIndex), m_endBoneIndex);
            xs.WriteFloat(xe, nameof(m_gain), m_gain);
            xs.WriteFloat(xe, nameof(m_boneRadius), m_boneRadius);
            xs.WriteFloat(xe, nameof(m_castOffset), m_castOffset);
            xs.WriteSerializeIgnored(xe, nameof(m_timeStep));
            xs.WriteSerializeIgnored(xe, nameof(m_pSkeletonMemory));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSLimbIKModifier);
        }

        public bool Equals(BSLimbIKModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_limitAngleDegrees.Equals(other.m_limitAngleDegrees) &&
                   m_startBoneIndex.Equals(other.m_startBoneIndex) &&
                   m_endBoneIndex.Equals(other.m_endBoneIndex) &&
                   m_gain.Equals(other.m_gain) &&
                   m_boneRadius.Equals(other.m_boneRadius) &&
                   m_castOffset.Equals(other.m_castOffset) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_limitAngleDegrees);
            hashcode.Add(m_startBoneIndex);
            hashcode.Add(m_endBoneIndex);
            hashcode.Add(m_gain);
            hashcode.Add(m_boneRadius);
            hashcode.Add(m_castOffset);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

