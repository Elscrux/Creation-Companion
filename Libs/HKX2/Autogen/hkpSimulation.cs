using System.Xml.Linq;
namespace HKX2
{
    // hkpSimulation Signatire: 0x97aba922 size: 64 flags: FLAGS_NOT_SERIALIZABLE

    // m_determinismCheckFrameCounter m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_world m_class: hkpWorld Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_lastProcessingStep m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: LastProcessingStep
    // m_currentTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_currentPsiTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_physicsDeltaTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 44 flags: FLAGS_NONE enum: 
    // m_simulateUntilTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_frameMarkerPsiSnap m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 52 flags: FLAGS_NONE enum: 
    // m_previousStepResult m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    public partial class hkpSimulation : hkReferencedObject, IEquatable<hkpSimulation?>
    {
        public uint m_determinismCheckFrameCounter { set; get; }
        public hkpWorld? m_world { set; get; }
        public byte m_lastProcessingStep { set; get; }
        public float m_currentTime { set; get; }
        public float m_currentPsiTime { set; get; }
        public float m_physicsDeltaTime { set; get; }
        public float m_simulateUntilTime { set; get; }
        public float m_frameMarkerPsiSnap { set; get; }
        public uint m_previousStepResult { set; get; }

        public override uint Signature { set; get; } = 0x97aba922;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_determinismCheckFrameCounter = br.ReadUInt32();
            br.Position += 4;
            m_world = des.ReadClassPointer<hkpWorld>(br);
            m_lastProcessingStep = br.ReadByte();
            br.Position += 3;
            m_currentTime = br.ReadSingle();
            m_currentPsiTime = br.ReadSingle();
            m_physicsDeltaTime = br.ReadSingle();
            m_simulateUntilTime = br.ReadSingle();
            m_frameMarkerPsiSnap = br.ReadSingle();
            m_previousStepResult = br.ReadUInt32();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt32(m_determinismCheckFrameCounter);
            bw.Position += 4;
            s.WriteClassPointer(bw, m_world);
            bw.WriteByte(m_lastProcessingStep);
            bw.Position += 3;
            bw.WriteSingle(m_currentTime);
            bw.WriteSingle(m_currentPsiTime);
            bw.WriteSingle(m_physicsDeltaTime);
            bw.WriteSingle(m_simulateUntilTime);
            bw.WriteSingle(m_frameMarkerPsiSnap);
            bw.WriteUInt32(m_previousStepResult);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_determinismCheckFrameCounter = xd.ReadUInt32(xe, nameof(m_determinismCheckFrameCounter));
            m_world = xd.ReadClassPointer<hkpWorld>(xe, nameof(m_world));
            m_lastProcessingStep = xd.ReadFlag<LastProcessingStep, byte>(xe, nameof(m_lastProcessingStep));
            m_currentTime = xd.ReadSingle(xe, nameof(m_currentTime));
            m_currentPsiTime = xd.ReadSingle(xe, nameof(m_currentPsiTime));
            m_physicsDeltaTime = xd.ReadSingle(xe, nameof(m_physicsDeltaTime));
            m_simulateUntilTime = xd.ReadSingle(xe, nameof(m_simulateUntilTime));
            m_frameMarkerPsiSnap = xd.ReadSingle(xe, nameof(m_frameMarkerPsiSnap));
            m_previousStepResult = xd.ReadUInt32(xe, nameof(m_previousStepResult));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_determinismCheckFrameCounter), m_determinismCheckFrameCounter);
            xs.WriteClassPointer(xe, nameof(m_world), m_world);
            xs.WriteEnum<LastProcessingStep, byte>(xe, nameof(m_lastProcessingStep), m_lastProcessingStep);
            xs.WriteFloat(xe, nameof(m_currentTime), m_currentTime);
            xs.WriteFloat(xe, nameof(m_currentPsiTime), m_currentPsiTime);
            xs.WriteFloat(xe, nameof(m_physicsDeltaTime), m_physicsDeltaTime);
            xs.WriteFloat(xe, nameof(m_simulateUntilTime), m_simulateUntilTime);
            xs.WriteFloat(xe, nameof(m_frameMarkerPsiSnap), m_frameMarkerPsiSnap);
            xs.WriteNumber(xe, nameof(m_previousStepResult), m_previousStepResult);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSimulation);
        }

        public bool Equals(hkpSimulation? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_determinismCheckFrameCounter.Equals(other.m_determinismCheckFrameCounter) &&
                   ((m_world is null && other.m_world is null) || (m_world is not null && other.m_world is not null && m_world.Equals((IHavokObject)other.m_world))) &&
                   m_lastProcessingStep.Equals(other.m_lastProcessingStep) &&
                   m_currentTime.Equals(other.m_currentTime) &&
                   m_currentPsiTime.Equals(other.m_currentPsiTime) &&
                   m_physicsDeltaTime.Equals(other.m_physicsDeltaTime) &&
                   m_simulateUntilTime.Equals(other.m_simulateUntilTime) &&
                   m_frameMarkerPsiSnap.Equals(other.m_frameMarkerPsiSnap) &&
                   m_previousStepResult.Equals(other.m_previousStepResult) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_determinismCheckFrameCounter);
            hashcode.Add(m_world);
            hashcode.Add(m_lastProcessingStep);
            hashcode.Add(m_currentTime);
            hashcode.Add(m_currentPsiTime);
            hashcode.Add(m_physicsDeltaTime);
            hashcode.Add(m_simulateUntilTime);
            hashcode.Add(m_frameMarkerPsiSnap);
            hashcode.Add(m_previousStepResult);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

