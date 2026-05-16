using System.Xml.Linq;
namespace HKX2
{
    // hkpStiffSpringChainData Signatire: 0xf170356b size: 80 flags: FLAGS_NONE

    // m_atoms m_class: hkpBridgeAtoms Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_infos m_class: hkpStiffSpringChainDataConstraintInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_tau m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_damping m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 68 flags: FLAGS_NONE enum: 
    // m_cfm m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    public partial class hkpStiffSpringChainData : hkpConstraintChainData, IEquatable<hkpStiffSpringChainData?>
    {
        public hkpBridgeAtoms m_atoms { set; get; } = new();
        public IList<hkpStiffSpringChainDataConstraintInfo> m_infos { set; get; } = Array.Empty<hkpStiffSpringChainDataConstraintInfo>();
        public float m_tau { set; get; }
        public float m_damping { set; get; }
        public float m_cfm { set; get; }

        public override uint Signature { set; get; } = 0xf170356b;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_atoms.Read(des, br);
            m_infos = des.ReadClassArray<hkpStiffSpringChainDataConstraintInfo>(br);
            m_tau = br.ReadSingle();
            m_damping = br.ReadSingle();
            m_cfm = br.ReadSingle();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_atoms.Write(s, bw);
            s.WriteClassArray(bw, m_infos);
            bw.WriteSingle(m_tau);
            bw.WriteSingle(m_damping);
            bw.WriteSingle(m_cfm);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_atoms = xd.ReadClass<hkpBridgeAtoms>(xe, nameof(m_atoms));
            m_infos = xd.ReadClassArray<hkpStiffSpringChainDataConstraintInfo>(xe, nameof(m_infos));
            m_tau = xd.ReadSingle(xe, nameof(m_tau));
            m_damping = xd.ReadSingle(xe, nameof(m_damping));
            m_cfm = xd.ReadSingle(xe, nameof(m_cfm));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkpBridgeAtoms>(xe, nameof(m_atoms), m_atoms);
            xs.WriteClassArray(xe, nameof(m_infos), m_infos);
            xs.WriteFloat(xe, nameof(m_tau), m_tau);
            xs.WriteFloat(xe, nameof(m_damping), m_damping);
            xs.WriteFloat(xe, nameof(m_cfm), m_cfm);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpStiffSpringChainData);
        }

        public bool Equals(hkpStiffSpringChainData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_atoms is null && other.m_atoms is null) || (m_atoms is not null && other.m_atoms is not null && m_atoms.Equals((IHavokObject)other.m_atoms))) &&
                   m_infos.SequenceEqual(other.m_infos) &&
                   m_tau.Equals(other.m_tau) &&
                   m_damping.Equals(other.m_damping) &&
                   m_cfm.Equals(other.m_cfm) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_atoms);
            hashcode.Add(m_infos.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_tau);
            hashcode.Add(m_damping);
            hashcode.Add(m_cfm);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

