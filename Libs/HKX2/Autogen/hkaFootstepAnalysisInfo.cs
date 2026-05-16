using System.Xml.Linq;
namespace HKX2
{
    // hkaFootstepAnalysisInfo Signatire: 0x824faf75 size: 208 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_ARRAY Type.TYPE_CHAR arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_nameStrike m_class:  Type.TYPE_ARRAY Type.TYPE_CHAR arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_nameLift m_class:  Type.TYPE_ARRAY Type.TYPE_CHAR arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_nameLock m_class:  Type.TYPE_ARRAY Type.TYPE_CHAR arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_nameUnlock m_class:  Type.TYPE_ARRAY Type.TYPE_CHAR arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_minPos m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_maxPos m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_minVel m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_maxVel m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_allBonesDown m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_anyBonesDown m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    // m_posTol m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 192 flags: FLAGS_NONE enum: 
    // m_velTol m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 196 flags: FLAGS_NONE enum: 
    // m_duration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 200 flags: FLAGS_NONE enum: 
    public partial class hkaFootstepAnalysisInfo : hkReferencedObject, IEquatable<hkaFootstepAnalysisInfo?>
    {
        public string m_name { set; get; } = "";
        public string m_nameStrike { set; get; } = "";
        public string m_nameLift { set; get; } = "";
        public string m_nameLock { set; get; } = "";
        public string m_nameUnlock { set; get; } = "";
        public IList<float> m_minPos { set; get; } = Array.Empty<float>();
        public IList<float> m_maxPos { set; get; } = Array.Empty<float>();
        public IList<float> m_minVel { set; get; } = Array.Empty<float>();
        public IList<float> m_maxVel { set; get; } = Array.Empty<float>();
        public IList<float> m_allBonesDown { set; get; } = Array.Empty<float>();
        public IList<float> m_anyBonesDown { set; get; } = Array.Empty<float>();
        public float m_posTol { set; get; }
        public float m_velTol { set; get; }
        public float m_duration { set; get; }

        public override uint Signature { set; get; } = 0x824faf75;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = br.ReadASCII();
            m_nameStrike = br.ReadASCII();
            m_nameLift = br.ReadASCII();
            m_nameLock = br.ReadASCII();
            m_nameUnlock = br.ReadASCII();
            m_minPos = des.ReadSingleArray(br);
            m_maxPos = des.ReadSingleArray(br);
            m_minVel = des.ReadSingleArray(br);
            m_maxVel = des.ReadSingleArray(br);
            m_allBonesDown = des.ReadSingleArray(br);
            m_anyBonesDown = des.ReadSingleArray(br);
            m_posTol = br.ReadSingle();
            m_velTol = br.ReadSingle();
            m_duration = br.ReadSingle();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteASCII(m_name, true);
            bw.WriteASCII(m_nameStrike, true);
            bw.WriteASCII(m_nameLift, true);
            bw.WriteASCII(m_nameLock, true);
            bw.WriteASCII(m_nameUnlock, true);
            s.WriteSingleArray(bw, m_minPos);
            s.WriteSingleArray(bw, m_maxPos);
            s.WriteSingleArray(bw, m_minVel);
            s.WriteSingleArray(bw, m_maxVel);
            s.WriteSingleArray(bw, m_allBonesDown);
            s.WriteSingleArray(bw, m_anyBonesDown);
            bw.WriteSingle(m_posTol);
            bw.WriteSingle(m_velTol);
            bw.WriteSingle(m_duration);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_name = xd.ReadString(xe, nameof(m_name));
            m_nameStrike = xd.ReadString(xe, nameof(m_nameStrike));
            m_nameLift = xd.ReadString(xe, nameof(m_nameLift));
            m_nameLock = xd.ReadString(xe, nameof(m_nameLock));
            m_nameUnlock = xd.ReadString(xe, nameof(m_nameUnlock));
            m_minPos = xd.ReadSingleArray(xe, nameof(m_minPos));
            m_maxPos = xd.ReadSingleArray(xe, nameof(m_maxPos));
            m_minVel = xd.ReadSingleArray(xe, nameof(m_minVel));
            m_maxVel = xd.ReadSingleArray(xe, nameof(m_maxVel));
            m_allBonesDown = xd.ReadSingleArray(xe, nameof(m_allBonesDown));
            m_anyBonesDown = xd.ReadSingleArray(xe, nameof(m_anyBonesDown));
            m_posTol = xd.ReadSingle(xe, nameof(m_posTol));
            m_velTol = xd.ReadSingle(xe, nameof(m_velTol));
            m_duration = xd.ReadSingle(xe, nameof(m_duration));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteString(xe, nameof(m_nameStrike), m_nameStrike);
            xs.WriteString(xe, nameof(m_nameLift), m_nameLift);
            xs.WriteString(xe, nameof(m_nameLock), m_nameLock);
            xs.WriteString(xe, nameof(m_nameUnlock), m_nameUnlock);
            xs.WriteFloatArray(xe, nameof(m_minPos), m_minPos);
            xs.WriteFloatArray(xe, nameof(m_maxPos), m_maxPos);
            xs.WriteFloatArray(xe, nameof(m_minVel), m_minVel);
            xs.WriteFloatArray(xe, nameof(m_maxVel), m_maxVel);
            xs.WriteFloatArray(xe, nameof(m_allBonesDown), m_allBonesDown);
            xs.WriteFloatArray(xe, nameof(m_anyBonesDown), m_anyBonesDown);
            xs.WriteFloat(xe, nameof(m_posTol), m_posTol);
            xs.WriteFloat(xe, nameof(m_velTol), m_velTol);
            xs.WriteFloat(xe, nameof(m_duration), m_duration);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaFootstepAnalysisInfo);
        }

        public bool Equals(hkaFootstepAnalysisInfo? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_name.SequenceEqual(other.m_name) &&
                   m_nameStrike.SequenceEqual(other.m_nameStrike) &&
                   m_nameLift.SequenceEqual(other.m_nameLift) &&
                   m_nameLock.SequenceEqual(other.m_nameLock) &&
                   m_nameUnlock.SequenceEqual(other.m_nameUnlock) &&
                   m_minPos.SequenceEqual(other.m_minPos) &&
                   m_maxPos.SequenceEqual(other.m_maxPos) &&
                   m_minVel.SequenceEqual(other.m_minVel) &&
                   m_maxVel.SequenceEqual(other.m_maxVel) &&
                   m_allBonesDown.SequenceEqual(other.m_allBonesDown) &&
                   m_anyBonesDown.SequenceEqual(other.m_anyBonesDown) &&
                   m_posTol.Equals(other.m_posTol) &&
                   m_velTol.Equals(other.m_velTol) &&
                   m_duration.Equals(other.m_duration) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_name.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_nameStrike.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_nameLift.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_nameLock.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_nameUnlock.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_minPos.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_maxPos.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_minVel.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_maxVel.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_allBonesDown.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_anyBonesDown.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_posTol);
            hashcode.Add(m_velTol);
            hashcode.Add(m_duration);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

