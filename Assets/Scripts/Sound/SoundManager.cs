using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.Audio
{
     [Serializable]
     public struct SoundLookup
     {
         public string soundId;
         public AudioClip clip; 
     }
     public class SoundManager : MonoBehaviour
     {
         private static SoundManager _instance;
     
         [SerializeField] private AudioSource audioSourcePrefab;
         [SerializeField] private List<SoundLookup> audioClips;

         [SerializeField] private Transform soundRoot; 
         private readonly Dictionary<string, AudioClip> _soundLookup = new (); 
         
         private void Awake()
         {
             if (_instance == null)
             {
                 _instance = this;
                 DontDestroyOnLoad(gameObject);
                 LoadSounds();
                 return;
             }
             
             Destroy(gameObject);
         }
         
         private void LoadSounds()
         {
             _soundLookup.Clear();
     
             foreach (var clip  in audioClips) {
                 _soundLookup.TryAdd(clip.soundId, clip.clip);
             }
         }
     
         public static void PlaySound(string id)
         {
             bool found = _instance._soundLookup.TryGetValue(id, out var clip);
             if(!found) return;
             
             var source = Instantiate(_instance.audioSourcePrefab);

             if (_instance.soundRoot != null) source.transform.parent = _instance.soundRoot;
             source.clip = clip;
             source.Play();
         }
     
         public static void PlaySound(string id, Vector3 location)
         {
             bool found = _instance._soundLookup.TryGetValue(id, out var clip);
             if(!found) return;
     
             var source = Instantiate(_instance.audioSourcePrefab, location, Quaternion.identity);
             
             if (_instance.soundRoot != null) source.transform.parent = _instance.soundRoot;
             source.clip = clip;
             source.Play();
         }
     }
}