using System.Xml.Linq;
namespace HKX2
{
    // hkpBallAndSocketConstraintDataAtoms Signatire: 0xc73dcaf9 size: 80 flags: FLAGS_NONE

    // m_pivots m_class: hkpSetLocalTranslationsConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_setupStabilization m_class: hkpSetupStabilizationAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_ballSocket m_class: hkpBallSocketConstraintAtom Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpBallAndSocketConstraintDataAtoms : IHavokObject, IEquatable<hkpBallAndSocketConstraintDataAtoms?>
    {
        public hkpSetLocalTranslationsConstraintAtom m_pivots { set; get; } = new();
        public hkpSetupStabilizationAtom m_setupStabilization { set; get; } = new();
        public hkpBallSocketConstraintAtom m_ballSocket { set; get; } = new();

        public virtual uint Signature { set; get; } = 0xc73dcaf9;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pivots.Read(des, br);
            m_setupStabilization.Read(des, br);
            m_ballSocket.Read(des, br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_pivots.Write(s, bw);
            m_setupStabilization.Write(s, bw);
            m_ballSocket.Write(s, bw);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_pivots = xd.ReadClass<hkpSetLocalTranslationsConstraintAtom>(xe, nameof(m_pivots));
            m_setupStabilization = xd.ReadClass<hkpSetupStabilizationAtom>(xe, nameof(m_setupStabilization));
            m_ballSocket = xd.ReadClass<hkpBallSocketConstraintAtom>(xe, nameof(m_ballSocket));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClass<hkpSetLocalTranslationsConstraintAtom>(xe, nameof(m_pivots), m_pivots);
            xs.WriteClass(xe, nameof(m_setupStabilization), m_setupStabilization);
            xs.WriteClass<hkpBallSocketConstraintAtom>(xe, nameof(m_ballSocket), m_ballSocket);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpBallAndSocketConstraintDataAtoms);
        }

        public bool Equals(hkpBallAndSocketConstraintDataAtoms? other)
        {
            return other is not null &&
                   ((m_pivots is null && other.m_pivots is null) || (m_pivots is not null && other.m_pivots is not null && m_pivots.Equals((IHavokObject)other.m_pivots))) &&
                   ((m_setupStabilization is null && other.m_setupStabilization is null) || (m_setupStabilization is not null && other.m_setupStabilization is not null && m_setupStabilization.Equals((IHavokObject)other.m_setupStabilization))) &&
                   ((m_ballSocket is null && other.m_ballSocket is null) || (m_ballSocket is not null && other.m_ballSocket is not null && m_ballSocket.Equals((IHavokObject)other.m_ballSocket))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_pivots);
            hashcode.Add(m_setupStabilization);
            hashcode.Add(m_ballSocket);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

