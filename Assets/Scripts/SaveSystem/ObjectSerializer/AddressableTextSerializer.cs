using UnityEngine.AddressableAssets;
using WizardSave.ObjectSerializers;

namespace SaveSystem.ObjectSerializer
{
    public class AddressableTextSerializer : ITextSerializer<AssetReference>
    {
        public string SerializeObject(AssetReference obj)
        {
            return obj.AssetGUID;
        }

        public bool TryDeserializeObject(string data, out AssetReference obj)
        {
            try
            {
                obj = new AssetReference(data);
                return true;
            }
            catch
            {
                obj = null;
                return false;
            }
        }
    }
}