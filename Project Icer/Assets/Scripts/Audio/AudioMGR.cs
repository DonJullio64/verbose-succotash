using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMGR : MonoBehaviour
{
    public static AudioMGR STATIC_AudioMGR;


    #region VARIABLES

    public float MusicFadeTime = 5f;

    public List<AudioSource> List_AudioSources;
    public AudioSource AudioSource_Music;
    public AudioSource AudioSource_VO;
    public AudioSource AudioSource_GlobalSFX;
    public AudioSource AudioSource_NMG;
    public AudioSource AudioSource_FrostThrower;
    public AudioSource AudioSource_Nitronade;
    public AudioSource AudioSource_Ixe;

    #endregion VARIABLES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_INITIALIZATION

    void Awake()
    {
        if (STATIC_AudioMGR == null)
            STATIC_AudioMGR = this;
        else
            Destroy(this);

        foreach (AudioSource audio in GetComponents<AudioSource>())
        {
            List_AudioSources.Add(audio);
        }

        AudioSource_Music = List_AudioSources[0];
        AudioSource_VO = List_AudioSources[1];
        AudioSource_GlobalSFX = List_AudioSources[2];
        AudioSource_NMG = List_AudioSources[3];
        AudioSource_FrostThrower = List_AudioSources[4];
        AudioSource_Nitronade = List_AudioSources[5];
        AudioSource_Ixe = List_AudioSources[6];
    }

    #endregion METHODS_INITIALIZATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_UPDATE

    void Update()
    {

    }

    #endregion METHODS_UPDATE

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_SPECIFICS

    private void SetNewMusic_Immediate(AudioClip newmusic)
    {
        AudioSource_Music.Stop();
        AudioSource_Music.clip = newmusic;
        AudioSource_Music.Play();
    }
    private void SetNewMusic_FadeOutToIn(AudioClip newmusic)
    {
        if (AudioSource_Music.isPlaying)
            StartCoroutine(FadeOutMusic_Coroutine(MusicFadeTime, newmusic));
    }


    private void SetNewVO_Immediate(AudioClip newmusic)
    {
        AudioSource_Music.Stop();
        AudioSource_Music.clip = newmusic;
        AudioSource_Music.Play();
    }


    public void PlayGlobalSFXOneShot(AudioClip clip, float volumescale = 1f)
    {
        AudioSource_GlobalSFX.PlayOneShot(clip, volumescale);
    }


    public void PlayNMG()
    {
        AudioSource_NMG.Play();
    }
    public void PlayFrostThrower()
    {
        AudioSource_FrostThrower.Play();
    }
    public void PlayNitronade()
    {
        AudioSource_Nitronade.Play();
    }
    public void PlayIxe()
    {
        AudioSource_Ixe.Play();
    }

    #endregion METHODS_SPECIFICS

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_EVENTS



    #endregion METHODS_EVENTS

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_COLLISION



    #endregion METHODS_COLLISION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_COROUTINES

    IEnumerator FadeOutMusic_Coroutine(float fadetime, AudioClip newmusic)
    {
        float fadeunit = AudioSource_Music.volume / fadetime;
        while (fadetime > 0)
        {
            AudioSource_Music.volume -= fadeunit;
            fadetime -= Time.deltaTime;
            yield return null;
        }
        //yield return new WaitForSeconds(MusicFadeTime);

        AudioSource_Music.Stop();
        AudioSource_Music.clip = newmusic;
        AudioSource_Music.Play();
    }

    #endregion METHODS_COROUTINES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region GIZMOS

    //OnGizmo - private void OnDrawGizmos() { }

    #endregion GIZMOS
}
