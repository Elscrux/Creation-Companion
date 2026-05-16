using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // BSSynchronizedClipGenerator Signatire: 0xd83bea64 size: 304 flags: FLAGS_NONE

    // m_pClipGenerator m_class: hkbGenerator Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_SyncAnimPrefix m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_bSyncClipIgnoreMarkPlacement m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_fGetToMarkTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    // m_fMarkErrorThreshold m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_bLeadCharacter m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 108 flags: FLAGS_NONE enum: 
    // m_bReorientSupportChar m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 109 flags: FLAGS_NONE enum: 
    // m_bApplyMotionFromRoot m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 110 flags: FLAGS_NONE enum: 
    // m_pSyncScene m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 112 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_StartMarkWS m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 128 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_EndMarkWS m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 176 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_StartMarkMS m_class:  Type.TYPE_QSTRANSFORM Type.TYPE_VOID arrSize: 0 offset: 224 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_fCurrentLerp m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 272 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_pLocalSyncBinding m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 280 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_pEventMap m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 288 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_sAnimationBindingIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 296 flags: FLAGS_NONE enum: 
    // m_bAtMark m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 298 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_bAllCharactersInScene m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 299 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_bAllCharactersAtMarks m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 300 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class BSSynchronizedClipGenerator : hkbGenerator, IEquatable<BSSynchronizedClipGenerator?>
    {
        public hkbGenerator? m_pClipGenerator { set; get; }
        public string m_SyncAnimPrefix { set; get; } = "";
        public bool m_bSyncClipIgnoreMarkPlacement { set; get; }
        public float m_fGetToMarkTime { set; get; }
        public float m_fMarkErrorThreshold { set; get; }
        public bool m_bLeadCharacter { set; get; }
        public bool m_bReorientSupportChar { set; get; }
        public bool m_bApplyMotionFromRoot { set; get; }
        private object? m_pSyncScene { set; get; }
        private Matrix4x4 m_StartMarkWS { set; get; }
        private Matrix4x4 m_EndMarkWS { set; get; }
        private Matrix4x4 m_StartMarkMS { set; get; }
        private float m_fCurrentLerp { set; get; }
        private object? m_pLocalSyncBinding { set; get; }
        private object? m_pEventMap { set; get; }
        public short m_sAnimationBindingIndex { set; get; }
        private bool m_bAtMark { set; get; }
        private bool m_bAllCharactersInScene { set; get; }
        private bool m_bAllCharactersAtMarks { set; get; }

        public override uint Signature { set; get; } = 0xd83bea64;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_pClipGenerator = des.ReadClassPointer<hkbGenerator>(br);
            m_SyncAnimPrefix = des.ReadCString(br);
            m_bSyncClipIgnoreMarkPlacement = br.ReadBoolean();
            br.Position += 3;
            m_fGetToMarkTime = br.ReadSingle();
            m_fMarkErrorThreshold = br.ReadSingle();
            m_bLeadCharacter = br.ReadBoolean();
            m_bReorientSupportChar = br.ReadBoolean();
            m_bApplyMotionFromRoot = br.ReadBoolean();
            br.Position += 1;
            des.ReadEmptyPointer(br);
            br.Position += 8;
            m_StartMarkWS = des.ReadQSTransform(br);
            m_EndMarkWS = des.ReadQSTransform(br);
            m_StartMarkMS = des.ReadQSTransform(br);
            m_fCurrentLerp = br.ReadSingle();
            br.Position += 4;
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            m_sAnimationBindingIndex = br.ReadInt16();
            m_bAtMark = br.ReadBoolean();
            m_bAllCharactersInScene = br.ReadBoolean();
            m_bAllCharactersAtMarks = br.ReadBoolean();
            br.Position += 3;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            s.WriteClassPointer(bw, m_pClipGenerator);
            s.WriteCString(bw, m_SyncAnimPrefix);
            bw.WriteBoolean(m_bSyncClipIgnoreMarkPlacement);
            bw.Position += 3;
            bw.WriteSingle(m_fGetToMarkTime);
            bw.WriteSingle(m_fMarkErrorThreshold);
            bw.WriteBoolean(m_bLeadCharacter);
            bw.WriteBoolean(m_bReorientSupportChar);
            bw.WriteBoolean(m_bApplyMotionFromRoot);
            bw.Position += 1;
            s.WriteVoidPointer(bw);
            bw.Position += 8;
            s.WriteQSTransform(bw, m_StartMarkWS);
            s.WriteQSTransform(bw, m_EndMarkWS);
            s.WriteQSTransform(bw, m_StartMarkMS);
            bw.WriteSingle(m_fCurrentLerp);
            bw.Position += 4;
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            bw.WriteInt16(m_sAnimationBindingIndex);
            bw.WriteBoolean(m_bAtMark);
            bw.WriteBoolean(m_bAllCharactersInScene);
            bw.WriteBoolean(m_bAllCharactersAtMarks);
            bw.Position += 3;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_pClipGenerator = xd.ReadClassPointer<hkbGenerator>(xe, nameof(m_pClipGenerator));
            m_SyncAnimPrefix = xd.ReadString(xe, nameof(m_SyncAnimPrefix));
            m_bSyncClipIgnoreMarkPlacement = xd.ReadBoolean(xe, nameof(m_bSyncClipIgnoreMarkPlacement));
            m_fGetToMarkTime = xd.ReadSingle(xe, nameof(m_fGetToMarkTime));
            m_fMarkErrorThreshold = xd.ReadSingle(xe, nameof(m_fMarkErrorThreshold));
            m_bLeadCharacter = xd.ReadBoolean(xe, nameof(m_bLeadCharacter));
            m_bReorientSupportChar = xd.ReadBoolean(xe, nameof(m_bReorientSupportChar));
            m_bApplyMotionFromRoot = xd.ReadBoolean(xe, nameof(m_bApplyMotionFromRoot));
            m_sAnimationBindingIndex = xd.ReadInt16(xe, nameof(m_sAnimationBindingIndex));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_pClipGenerator), m_pClipGenerator);
            xs.WriteString(xe, nameof(m_SyncAnimPrefix), m_SyncAnimPrefix);
            xs.WriteBoolean(xe, nameof(m_bSyncClipIgnoreMarkPlacement), m_bSyncClipIgnoreMarkPlacement);
            xs.WriteFloat(xe, nameof(m_fGetToMarkTime), m_fGetToMarkTime);
            xs.WriteFloat(xe, nameof(m_fMarkErrorThreshold), m_fMarkErrorThreshold);
            xs.WriteBoolean(xe, nameof(m_bLeadCharacter), m_bLeadCharacter);
            xs.WriteBoolean(xe, nameof(m_bReorientSupportChar), m_bReorientSupportChar);
            xs.WriteBoolean(xe, nameof(m_bApplyMotionFromRoot), m_bApplyMotionFromRoot);
            xs.WriteSerializeIgnored(xe, nameof(m_pSyncScene));
            xs.WriteSerializeIgnored(xe, nameof(m_StartMarkWS));
            xs.WriteSerializeIgnored(xe, nameof(m_EndMarkWS));
            xs.WriteSerializeIgnored(xe, nameof(m_StartMarkMS));
            xs.WriteSerializeIgnored(xe, nameof(m_fCurrentLerp));
            xs.WriteSerializeIgnored(xe, nameof(m_pLocalSyncBinding));
            xs.WriteSerializeIgnored(xe, nameof(m_pEventMap));
            xs.WriteNumber(xe, nameof(m_sAnimationBindingIndex), m_sAnimationBindingIndex);
            xs.WriteSerializeIgnored(xe, nameof(m_bAtMark));
            xs.WriteSerializeIgnored(xe, nameof(m_bAllCharactersInScene));
            xs.WriteSerializeIgnored(xe, nameof(m_bAllCharactersAtMarks));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSSynchronizedClipGenerator);
        }

        public bool Equals(BSSynchronizedClipGenerator? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_pClipGenerator is null && other.m_pClipGenerator is null) || (m_pClipGenerator is not null && other.m_pClipGenerator is not null && m_pClipGenerator.Equals((IHavokObject)other.m_pClipGenerator))) &&
                   (m_SyncAnimPrefix is null && other.m_SyncAnimPrefix is null || m_SyncAnimPrefix == other.m_SyncAnimPrefix || m_SyncAnimPrefix is null && other.m_SyncAnimPrefix == "" || m_SyncAnimPrefix == "" && other.m_SyncAnimPrefix is null) &&
                   m_bSyncClipIgnoreMarkPlacement.Equals(other.m_bSyncClipIgnoreMarkPlacement) &&
                   m_fGetToMarkTime.Equals(other.m_fGetToMarkTime) &&
                   m_fMarkErrorThreshold.Equals(other.m_fMarkErrorThreshold) &&
                   m_bLeadCharacter.Equals(other.m_bLeadCharacter) &&
                   m_bReorientSupportChar.Equals(other.m_bReorientSupportChar) &&
                   m_bApplyMotionFromRoot.Equals(other.m_bApplyMotionFromRoot) &&
                   m_sAnimationBindingIndex.Equals(other.m_sAnimationBindingIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_pClipGenerator);
            hashcode.Add(m_SyncAnimPrefix);
            hashcode.Add(m_bSyncClipIgnoreMarkPlacement);
            hashcode.Add(m_fGetToMarkTime);
            hashcode.Add(m_fMarkErrorThreshold);
            hashcode.Add(m_bLeadCharacter);
            hashcode.Add(m_bReorientSupportChar);
            hashcode.Add(m_bApplyMotionFromRoot);
            hashcode.Add(m_sAnimationBindingIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

