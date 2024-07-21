using WizardSave;
using WizardSave.ObjectSerializers;
using WizardSave.Utils;

namespace SaveSystem
{
    public interface ISavableData
    {
        ISaveableKeyValueStore SaveContainer { get; set; }
        
        string FilePath { get; }
        
        void NewSave(ObjectSerializerMap objectSerializerMap);
        void Save(ObjectSerializerMap objectSerializerMap);
        void Load(ObjectSerializerMap objectSerializerMap);
        void DeleteData();
    }
}