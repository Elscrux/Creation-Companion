using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpPulleyConstraintAtom Signatire: 0x94a08848 size: 64 flags: FLAGS_NONE

    // m_fixedPivotAinWorld m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_fixedPivotBinWorld m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_ropeLength m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_leverageOnBodyB m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 52 flags: FLAGS_NONE enum: 
    public partial class hkpPulleyConstraintAtom : hkpConstraintAtom, IEquatable<hkpPulleyConstraintAtom?>
    {
        public Vector4 m_fixedPivotAinWorld { set; get; }
        public Vector4 m_fixedPivotBinWorld { set; get; }
        public float m_ropeLength { set; get; }
        public float m_leverageOnBodyB { set; get; }

        public override uint Signature { set; get; } = 0x94a08848;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 14;
            m_fixedPivotAinWorld = br.ReadVector4();
            m_fixedPivotBinWorld = br.ReadVector4();
            m_ropeLength = br.ReadSingle();
            m_leverageOnBodyB = br.ReadSingle();
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 14;
            bw.WriteVector4(m_fixedPivotAinWorld);
            bw.WriteVector4(m_fixedPivotBinWorld);
            bw.WriteSingle(m_ropeLength);
            bw.WriteSingle(m_leverageOnBodyB);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_fixedPivotAinWorld = xd.ReadVector4(xe, nameof(m_fixedPivotAinWorld));
            m_fixedPivotBinWorld = xd.ReadVector4(xe, nameof(m_fixedPivotBinWorld));
            m_ropeLength = xd.ReadSingle(xe, nameof(m_ropeLength));
            m_leverageOnBodyB = xd.ReadSingle(xe, nameof(m_leverageOnBodyB));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_fixedPivotAinWorld), m_fixedPivotAinWorld);
            xs.WriteVector4(xe, nameof(m_fixedPivotBinWorld), m_fixedPivotBinWorld);
            xs.WriteFloat(xe, nameof(m_ropeLength), m_ropeLength);
            xs.WriteFloat(xe, nameof(m_leverageOnBodyB), m_leverageOnBodyB);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPulleyConstraintAtom);
        }

        public bool Equals(hkpPulleyConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_fixedPivotAinWorld.Equals(other.m_fixedPivotAinWorld) &&
                   m_fixedPivotBinWorld.Equals(other.m_fixedPivotBinWorld) &&
                   m_ropeLength.Equals(other.m_ropeLength) &&
                   m_leverageOnBodyB.Equals(other.m_leverageOnBodyB) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_fixedPivotAinWorld);
            hashcode.Add(m_fixedPivotBinWorld);
            hashcode.Add(m_ropeLength);
            hashcode.Add(m_leverageOnBodyB);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

