using System.Xml.Linq;
namespace HKX2
{
    // hkpGenericConstraintDataSchemeConstraintInfo Signatire: 0xd6421f19 size: 16 flags: FLAGS_NONE

    // m_maxSizeOfSchema m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_sizeOfSchemas m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_numSolverResults m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_numSolverElemTemps m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    public partial class hkpGenericConstraintDataSchemeConstraintInfo : IHavokObject, IEquatable<hkpGenericConstraintDataSchemeConstraintInfo?>
    {
        public int m_maxSizeOfSchema { set; get; }
        public int m_sizeOfSchemas { set; get; }
        public int m_numSolverResults { set; get; }
        public int m_numSolverElemTemps { set; get; }

        public virtual uint Signature { set; get; } = 0xd6421f19;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_maxSizeOfSchema = br.ReadInt32();
            m_sizeOfSchemas = br.ReadInt32();
            m_numSolverResults = br.ReadInt32();
            m_numSolverElemTemps = br.ReadInt32();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt32(m_maxSizeOfSchema);
            bw.WriteInt32(m_sizeOfSchemas);
            bw.WriteInt32(m_numSolverResults);
            bw.WriteInt32(m_numSolverElemTemps);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_maxSizeOfSchema = xd.ReadInt32(xe, nameof(m_maxSizeOfSchema));
            m_sizeOfSchemas = xd.ReadInt32(xe, nameof(m_sizeOfSchemas));
            m_numSolverResults = xd.ReadInt32(xe, nameof(m_numSolverResults));
            m_numSolverElemTemps = xd.ReadInt32(xe, nameof(m_numSolverElemTemps));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_maxSizeOfSchema), m_maxSizeOfSchema);
            xs.WriteNumber(xe, nameof(m_sizeOfSchemas), m_sizeOfSchemas);
            xs.WriteNumber(xe, nameof(m_numSolverResults), m_numSolverResults);
            xs.WriteNumber(xe, nameof(m_numSolverElemTemps), m_numSolverElemTemps);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpGenericConstraintDataSchemeConstraintInfo);
        }

        public bool Equals(hkpGenericConstraintDataSchemeConstraintInfo? other)
        {
            return other is not null &&
                   m_maxSizeOfSchema.Equals(other.m_maxSizeOfSchema) &&
                   m_sizeOfSchemas.Equals(other.m_sizeOfSchemas) &&
                   m_numSolverResults.Equals(other.m_numSolverResults) &&
                   m_numSolverElemTemps.Equals(other.m_numSolverElemTemps) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_maxSizeOfSchema);
            hashcode.Add(m_sizeOfSchemas);
            hashcode.Add(m_numSolverResults);
            hashcode.Add(m_numSolverElemTemps);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

