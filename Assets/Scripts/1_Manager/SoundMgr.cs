using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundMgr : MonoBehaviour
{
    #region SingleTon

    private static SoundMgr _inst;
    public static SoundMgr GetInst()
    {
        if (_inst != null) return _inst;
        else
        {
            Debug.LogError("_Sound Manager is Already Set");
            return null;
        }
    }
    private void Awake()
    {
        if (_inst == null)
        {
            _inst = this;
            _source = GetComponent<AudioSource>();
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.LogError("Sound Manager Alreadey Set Inst");
        }
    }
    #endregion

    [SerializeField] AudioMixer masterMixer;
    private AudioSource _source;
    public AudioSource _BgmSource;
    public GameObject _BgmSourcePref;

    public AudioClip _btnSound;
    public AudioClip _effectSound;

    [SerializeField] List<AudioClip> effects;

    public Image _settingPannel;
    public Sprite _OnImg;
    public Sprite _offimg;

    public Image smallSettingPannel;
    public Sprite _smallOn;
    public Sprite _smallOff;

    private bool _Switch = true;

    private void Start()
    {
        _BgmSource = Instantiate(_BgmSourcePref).GetComponent<AudioSource>();
        DontDestroyOnLoad(_BgmSource);
    }

    public void OnOffSound()
    {
        if (SceneManager.GetActiveScene().name != GameManager.ClothSceneName)
        {
            _Switch = !_Switch;

            masterMixer.SetFloat("Master", _Switch ? 0 : -80f);

            switch (_Switch)
            {
                case true:
                    smallSettingPannel.sprite = _OnImg;
                    break;
                case false:
                    smallSettingPannel.sprite = _offimg;
                    break;
            }
        }
        else
        {
            _Switch = !_Switch;

            masterMixer.SetFloat("Master", _Switch ? 0 : -80f);

            switch (_Switch)
            {
                case true:
                    _settingPannel.sprite = _OnImg;

                    break;
                case false:
                    _settingPannel.sprite = _offimg;
                    break;
            }
        }
    }

    public void OnClickSoundBtn()
    {
        _source.PlayOneShot(_btnSound);
    }

    public void OnClickSountEffect()
    {
        _source.PlayOneShot(_effectSound);
    }

    public void OnPlayOneShot(string effectName)
    {
        AudioClip audioClip = effects.Find(x => x.name == effectName);
        if (audioClip != null)
        {
            _source.PlayOneShot(audioClip);
        }
    }
}
