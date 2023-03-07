using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Std.Sound
{
    [CreateAssetMenu(fileName = "SoundCollection", menuName = "Scriptable Object/ SoundCollection", order = int.MaxValue)]
    public class SoundCollection : ScriptableObject
    {
        [SerializeField]
        public List<BgmGroupData> bgmGroups;

        [SerializeField]
        public List<SoundGroupData> soundGroups;
    }
}