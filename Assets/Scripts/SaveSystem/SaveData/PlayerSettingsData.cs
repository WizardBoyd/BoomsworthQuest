using System.IO;
using SaveSystem.Interface;

namespace SaveSystem.SaveData
{
    public class PlayerSettingsData : GameData, IDataSave
    {
        public bool SoundOn { get; set; }
        public bool MusicOn { get; set; }
        public bool SFXOn { get; set; }
        
        public void WriteDataToFile(BinaryWriter writer)
        {
            writer.Write(SoundOn);
            writer.Write(MusicOn);
            writer.Write(SFXOn);
        }

        public void ReadDataFromFile(BinaryReader reader)
        {
             SoundOn = reader.ReadBoolean();
             MusicOn = reader.ReadBoolean();
             SFXOn = reader.ReadBoolean();
        }
    }
}