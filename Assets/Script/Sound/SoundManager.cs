using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

using Std.Utility;
using Std.Common.Pattern;
using Std.Common.Extension;

namespace Std.Sound
{
    public enum BgmType
    {
        None,
        Ending,
        Custom, // Play explicitly
    } // When play

    public enum SoundType
    {
        //SFX

        UISfx = 2000,

        GameSfx = 3000,

        //Anything...
    }

    public class AudioClipData
    {
        public string name;
        public AudioClip clip;

        public AudioClipData(string name, AudioClip audio)
        {
            this.name = name;
            this.clip = audio;
        }
    }

    [SerializeField]
    public class SoundGroupData
    {
        [SerializeField]
        public SoundType type;

        [SerializeField]
        public List<AudioClip> clips;

        public SoundGroupData(SoundType type, List<AudioClip> audios)
        {
            this.type = type;
            this.clips = audios;
        }

        public override string ToString()
        {
            return type.ToString();
        }
    }

    [SerializeField]
    public class BgmGroupData
    {
        [SerializeField]
        public BgmType type;

        [SerializeField]
        public List<AudioClip> clips;

        public BgmGroupData(BgmType type, List<AudioClip> audios)
        {
            this.type = type;
            this.clips = audios;
        }

        public override string ToString()
        {
            return type.ToString();
        }
    }

    //[RequireComponent((typeof(AudioListener)))]
    [RequireComponent(typeof(AudioSource))]
    [DefaultExecutionOrder(-25)]
    public partial class SoundManager : Singleton<SoundManager>
    {
        private AudioSource bgmSource;
        private AudioSource soundSource;

        public AudioSource BgmSource => bgmSource;
        public AudioSource SoundSource => soundSource;

        private List<AudioClipData> bgms;
        private List<AudioClipData> sounds;

        [SerializeField]
        private SoundCollection soundCollection;

        private ObjectPool soundPool;

        protected override void Init()
        {
            bgmSource = gameObject.GetOrAddComponent<AudioSource>(); //bgm must be unique
            bgmSource.loop = true;

            if (soundSource == null)
            {
                var go = new GameObject();

                go.name = "sound";
                go.transform.parent = transform;
                go.AddComponent<Sound>();

                soundSource = go.GetComponent<AudioSource>();
                soundSource.playOnAwake = false;
            }

            InitSounds();
            InitBgms();
        }

        private void InitSounds()
        {
            sounds = new();

            soundCollection.soundGroups
                .ForEach(group =>
                {
                    var typeName = group.type
                        .ToString();

                    group.clips
                        .ForEach(clip => sounds.Add(new AudioClipData(typeName, clip)));
                });
        }

        private void InitBgms()
        {
            bgms = new();

            soundCollection.bgmGroups
                .ForEach(group =>
                {
                    var typeName = group.type
                        .ToString();

                    group.clips
                        .ForEach(clip => bgms.Add(new AudioClipData(typeName, clip)));
                });
        }
    } //Initialization

    public partial class SoundManager
    {
        private void PlayBGM(BgmType type) => PlayBGM(type == BgmType.None ? default : type.ToString());

        public void PlayBGM(string name = default)
        {
            if (bgms == null || bgms.Count <= 0 || name.Equals(string.Empty))
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Please add a {0} BGM", name);
#endif
                return;
            } // Check validation

            var group = bgms.FindAll(x => x.name == name);
            
            if (group.Count <= 0) return;

            var bgm = group[Random.Range(0, group.Count)];

            StopBGM();

            bgmSource.loop = true;
            bgmSource.clip = bgm.clip;
            bgmSource.Play();
        }

        public void PauseBGM() => bgmSource.Pause();
        public void StopBGM() => bgmSource.Stop();
        public void ResumeBGM() => bgmSource.Play();

        public IUniTaskAsyncEnumerable<float> PlaySoundGroupAsync(SoundType type)
        {
            var name = type.ToString();

            if (sounds == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Please add a " + name + " sound");
#endif
                return null;
            }

            var soundGroup = sounds.FindAll(x => x.name == name);

            return UniTaskAsyncEnumerable.Create<float>(async (writer, _) => // async writer, cancellation token
            {
                foreach (var i in Enumerable.Range(0, soundGroup.Count))
                {
                    var waitLenght = soundGroup[i].clip
                        .length;

                    await writer.YieldAsync(waitLenght);
                    //await UniTask.Delay(i * 1000);//1sec
                    
                    PlaySound(type, i);

                    await UniTask.Delay((int)(waitLenght * 1000));
                }
            });
        }

        public AudioClipData PlaySound(SoundType type, int index = 0, float volume = 1f, int createCount = ObjectPool.createCount)
        {
            var name = type.ToString();

            if (sounds == null)
            {
                Debug.LogWarning("Please add a " + name + " sound");
                return null;
            }

            var soundGroup = sounds.FindAll(x => x.name == name);
            var sound = soundGroup.Count > index ? soundGroup[index] : null;

            if (sound == null || sound.clip == null)
            {
                //Debug.LogWarning("Please add a " + name + " sound");
                return null;
            }

            if (soundPool == null)
            {
                soundPool = new ObjectPool(this.SoundSource.gameObject, gameObject, createCount);
            }

            soundPool.GetObject().GetComponent<Sound>().Play(sound.clip, volume);

            return sound;
        }


    }
}