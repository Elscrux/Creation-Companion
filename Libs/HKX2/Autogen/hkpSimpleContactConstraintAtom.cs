using System.Xml.Linq;
namespace HKX2
{
    // hkpSimpleContactConstraintAtom Signatire: 0x920df11a size: 48 flags: FLAGS_NONE

    // m_sizeOfAllAtoms m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_numContactPoints m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_numReservedContactPoints m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 6 flags: FLAGS_NONE enum: 
    // m_numUserDatasForBodyA m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_numUserDatasForBodyB m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 9 flags: FLAGS_NONE enum: 
    // m_contactPointPropertiesStriding m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 10 flags: FLAGS_NONE enum: 
    // m_maxNumContactPoints m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 12 flags: FLAGS_NONE enum: 
    // m_info m_class: hkpSimpleContactConstraintDataInfo Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 16 flags: ALIGN_16|FLAGS_NONE enum: 
    public partial class hkpSimpleContactConstraintAtom : hkpConstraintAtom, IEquatable<hkpSimpleContactConstraintAtom?>
    {
        public ushort m_sizeOfAllAtoms { set; get; }
        public ushort m_numContactPoints { set; get; }
        public ushort m_numReservedContactPoints { set; get; }
        public byte m_numUserDatasForBodyA { set; get; }
        public byte m_numUserDatasForBodyB { set; get; }
        public byte m_contactPointPropertiesStriding { set; get; }
        public ushort m_maxNumContactPoints { set; get; }
        public hkpSimpleContactConstraintDataInfo m_info { set; get; } = new();

        public override uint Signature { set; get; } = 0x920df11a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_sizeOfAllAtoms = br.ReadUInt16();
            m_numContactPoints = br.ReadUInt16();
            m_numReservedContactPoints = br.ReadUInt16();
            m_numUserDatasForBodyA = br.ReadByte();
            m_numUserDatasForBodyB = br.ReadByte();
            m_contactPointPropertiesStriding = br.ReadByte();
            br.Position += 1;
            m_maxNumContactPoints = br.ReadUInt16();
            br.Position += 2;
            m_info.Read(des, br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt16(m_sizeOfAllAtoms);
            bw.WriteUInt16(m_numContactPoints);
            bw.WriteUInt16(m_numReservedContactPoints);
            bw.WriteByte(m_numUserDatasForBodyA);
            bw.WriteByte(m_numUserDatasForBodyB);
            bw.WriteByte(m_contactPointPropertiesStriding);
            bw.Position += 1;
            bw.WriteUInt16(m_maxNumContactPoints);
            bw.Position += 2;
            m_info.Write(s, bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_sizeOfAllAtoms = xd.ReadUInt16(xe, nameof(m_sizeOfAllAtoms));
            m_numContactPoints = xd.ReadUInt16(xe, nameof(m_numContactPoints));
            m_numReservedContactPoints = xd.ReadUInt16(xe, nameof(m_numReservedContactPoints));
            m_numUserDatasForBodyA = xd.ReadByte(xe, nameof(m_numUserDatasForBodyA));
            m_numUserDatasForBodyB = xd.ReadByte(xe, nameof(m_numUserDatasForBodyB));
            m_contactPointPropertiesStriding = xd.ReadByte(xe, nameof(m_contactPointPropertiesStriding));
            m_maxNumContactPoints = xd.ReadUInt16(xe, nameof(m_maxNumContactPoints));
            m_info = xd.ReadClass<hkpSimpleContactConstraintDataInfo>(xe, nameof(m_info));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_sizeOfAllAtoms), m_sizeOfAllAtoms);
            xs.WriteNumber(xe, nameof(m_numContactPoints), m_numContactPoints);
            xs.WriteNumber(xe, nameof(m_numReservedContactPoints), m_numReservedContactPoints);
            xs.WriteNumber(xe, nameof(m_numUserDatasForBodyA), m_numUserDatasForBodyA);
            xs.WriteNumber(xe, nameof(m_numUserDatasForBodyB), m_numUserDatasForBodyB);
            xs.WriteNumber(xe, nameof(m_contactPointPropertiesStriding), m_contactPointPropertiesStriding);
            xs.WriteNumber(xe, nameof(m_maxNumContactPoints), m_maxNumContactPoints);
            xs.WriteClass<hkpSimpleContactConstraintDataInfo>(xe, nameof(m_info), m_info);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSimpleContactConstraintAtom);
        }

        public bool Equals(hkpSimpleContactConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_sizeOfAllAtoms.Equals(other.m_sizeOfAllAtoms) &&
                   m_numContactPoints.Equals(other.m_numContactPoints) &&
                   m_numReservedContactPoints.Equals(other.m_numReservedContactPoints) &&
                   m_numUserDatasForBodyA.Equals(other.m_numUserDatasForBodyA) &&
                   m_numUserDatasForBodyB.Equals(other.m_numUserDatasForBodyB) &&
                   m_contactPointPropertiesStriding.Equals(other.m_contactPointPropertiesStriding) &&
                   m_maxNumContactPoints.Equals(other.m_maxNumContactPoints) &&
                   ((m_info is null && other.m_info is null) || (m_info is not null && other.m_info is not null && m_info.Equals((IHavokObject)other.m_info))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_sizeOfAllAtoms);
            hashcode.Add(m_numContactPoints);
            hashcode.Add(m_numReservedContactPoints);
            hashcode.Add(m_numUserDatasForBodyA);
            hashcode.Add(m_numUserDatasForBodyB);
            hashcode.Add(m_contactPointPropertiesStriding);
            hashcode.Add(m_maxNumContactPoints);
            hashcode.Add(m_info);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

