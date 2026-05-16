using System.Xml.Linq;
namespace HKX2
{
    // hkpBreakableConstraintData Signatire: 0x7d6310c8 size: 72 flags: FLAGS_NONE

    // m_atoms m_class: hkpBridgeAtoms Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_constraintData m_class: hkpConstraintData Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_childRuntimeSize m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_childNumSolverResults m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 58 flags: FLAGS_NONE enum: 
    // m_solverResultLimit m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 60 flags: FLAGS_NONE enum: 
    // m_removeWhenBroken m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_revertBackVelocityOnBreak m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 65 flags: FLAGS_NONE enum: 
    public partial class hkpBreakableConstraintData : hkpConstraintData, IEquatable<hkpBreakableConstraintData?>
    {
        public hkpBridgeAtoms m_atoms { set; get; } = new();
        public hkpConstraintData? m_constraintData { set; get; }
        public ushort m_childRuntimeSize { set; get; }
        public ushort m_childNumSolverResults { set; get; }
        public float m_solverResultLimit { set; get; }
        public bool m_removeWhenBroken { set; get; }
        public bool m_revertBackVelocityOnBreak { set; get; }

        public override uint Signature { set; get; } = 0x7d6310c8;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_atoms.Read(des, br);
            m_constraintData = des.ReadClassPointer<hkpConstraintData>(br);
            m_childRuntimeSize = br.ReadUInt16();
            m_childNumSolverResults = br.ReadUInt16();
            m_solverResultLimit = br.ReadSingle();
            m_removeWhenBroken = br.ReadBoolean();
            m_revertBackVelocityOnBreak = br.ReadBoolean();
            br.Position += 6;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_atoms.Write(s, bw);
            s.WriteClassPointer(bw, m_constraintData);
            bw.WriteUInt16(m_childRuntimeSize);
            bw.WriteUInt16(m_childNumSolverResults);
            bw.WriteSingle(m_solverResultLimit);
            bw.WriteBoolean(m_removeWhenBroken);
            bw.WriteBoolean(m_revertBackVelocityOnBreak);
            bw.Position += 6;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_atoms = xd.ReadClass<hkpBridgeAtoms>(xe, nameof(m_atoms));
            m_constraintData = xd.ReadClassPointer<hkpConstraintData>(xe, nameof(m_constraintData));
            m_childRuntimeSize = xd.ReadUInt16(xe, nameof(m_childRuntimeSize));
            m_childNumSolverResults = xd.ReadUInt16(xe, nameof(m_childNumSolverResults));
            m_solverResultLimit = xd.ReadSingle(xe, nameof(m_solverResultLimit));
            m_removeWhenBroken = xd.ReadBoolean(xe, nameof(m_removeWhenBroken));
            m_revertBackVelocityOnBreak = xd.ReadBoolean(xe, nameof(m_revertBackVelocityOnBreak));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkpBridgeAtoms>(xe, nameof(m_atoms), m_atoms);
            xs.WriteClassPointer(xe, nameof(m_constraintData), m_constraintData);
            xs.WriteNumber(xe, nameof(m_childRuntimeSize), m_childRuntimeSize);
            xs.WriteNumber(xe, nameof(m_childNumSolverResults), m_childNumSolverResults);
            xs.WriteFloat(xe, nameof(m_solverResultLimit), m_solverResultLimit);
            xs.WriteBoolean(xe, nameof(m_removeWhenBroken), m_removeWhenBroken);
            xs.WriteBoolean(xe, nameof(m_revertBackVelocityOnBreak), m_revertBackVelocityOnBreak);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpBreakableConstraintData);
        }

        public bool Equals(hkpBreakableConstraintData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_atoms is null && other.m_atoms is null) || (m_atoms is not null && other.m_atoms is not null && m_atoms.Equals((IHavokObject)other.m_atoms))) &&
                   ((m_constraintData is null && other.m_constraintData is null) || (m_constraintData is not null && other.m_constraintData is not null && m_constraintData.Equals((IHavokObject)other.m_constraintData))) &&
                   m_childRuntimeSize.Equals(other.m_childRuntimeSize) &&
                   m_childNumSolverResults.Equals(other.m_childNumSolverResults) &&
                   m_solverResultLimit.Equals(other.m_solverResultLimit) &&
                   m_removeWhenBroken.Equals(other.m_removeWhenBroken) &&
                   m_revertBackVelocityOnBreak.Equals(other.m_revertBackVelocityOnBreak) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_atoms);
            hashcode.Add(m_constraintData);
            hashcode.Add(m_childRuntimeSize);
            hashcode.Add(m_childNumSolverResults);
            hashcode.Add(m_solverResultLimit);
            hashcode.Add(m_removeWhenBroken);
            hashcode.Add(m_revertBackVelocityOnBreak);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

