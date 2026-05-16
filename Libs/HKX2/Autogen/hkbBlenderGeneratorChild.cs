using System.Xml.Linq;
namespace HKX2
{
    // hkbBlenderGeneratorChild Signatire: 0xe2b384b0 size: 80 flags: FLAGS_NONE

    // m_generator m_class: hkbGenerator Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_boneWeights m_class: hkbBoneWeightArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_weight m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_worldFromModelWeight m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 68 flags: FLAGS_NONE enum: 
    public partial class hkbBlenderGeneratorChild : hkbBindable, IEquatable<hkbBlenderGeneratorChild?>
    {
        public hkbGenerator? m_generator { set; get; }
        public hkbBoneWeightArray? m_boneWeights { set; get; }
        public float m_weight { set; get; }
        public float m_worldFromModelWeight { set; get; }

        public override uint Signature { set; get; } = 0xe2b384b0;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_generator = des.ReadClassPointer<hkbGenerator>(br);
            m_boneWeights = des.ReadClassPointer<hkbBoneWeightArray>(br);
            m_weight = br.ReadSingle();
            m_worldFromModelWeight = br.ReadSingle();
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_generator);
            s.WriteClassPointer(bw, m_boneWeights);
            bw.WriteSingle(m_weight);
            bw.WriteSingle(m_worldFromModelWeight);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_generator = xd.ReadClassPointer<hkbGenerator>(xe, nameof(m_generator));
            m_boneWeights = xd.ReadClassPointer<hkbBoneWeightArray>(xe, nameof(m_boneWeights));
            m_weight = xd.ReadSingle(xe, nameof(m_weight));
            m_worldFromModelWeight = xd.ReadSingle(xe, nameof(m_worldFromModelWeight));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_generator), m_generator);
            xs.WriteClassPointer(xe, nameof(m_boneWeights), m_boneWeights);
            xs.WriteFloat(xe, nameof(m_weight), m_weight);
            xs.WriteFloat(xe, nameof(m_worldFromModelWeight), m_worldFromModelWeight);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBlenderGeneratorChild);
        }

        public bool Equals(hkbBlenderGeneratorChild? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_generator is null && other.m_generator is null) || (m_generator is not null && other.m_generator is not null && m_generator.Equals((IHavokObject)other.m_generator))) &&
                   ((m_boneWeights is null && other.m_boneWeights is null) || (m_boneWeights is not null && other.m_boneWeights is not null && m_boneWeights.Equals((IHavokObject)other.m_boneWeights))) &&
                   m_weight.Equals(other.m_weight) &&
                   m_worldFromModelWeight.Equals(other.m_worldFromModelWeight) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_generator);
            hashcode.Add(m_boneWeights);
            hashcode.Add(m_weight);
            hashcode.Add(m_worldFromModelWeight);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

