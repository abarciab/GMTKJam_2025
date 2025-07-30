using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public enum VolumeType { MASTER, MUSIC, AMBIENT, SFX}

[RequireComponent(typeof(MusicPlayer))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager i;

    [SerializeField] private GameObject _coordinatorPrefab;
    [SerializeField] private AudioMixer _mixer;

    private MusicPlayer _musicPlayer;
    private UnityAction _onCompleteFade;
    private float _startMasterVol;
    private float _fadeTimeLeft;
    private float _fadeTimeMax;

    private readonly Dictionary<VolumeType, float> _volumes = new Dictionary<VolumeType, float>();
    private readonly List<SoundCoordinator> _soundCoordinators = new List<SoundCoordinator>();

    private const string saveFileName = "VolumeSettings.txt";

    public MusicPlayer Music => _musicPlayer;
    public void SetMasterVolume(float vol, bool save = true) => SetVolume(VolumeType.MASTER, vol, save);
    public void SetMusicVolume(float vol) => SetVolume(VolumeType.MUSIC, vol);
    public void SetAmbientVolume(float vol) => SetVolume(VolumeType.AMBIENT, vol);
    public void SetSFXVolume(float vol) => SetVolume(VolumeType.SFX, vol);
    private List<VolumeType> _volTypes => Utils.EnumToList<VolumeType>();
    public AudioMixerGroup GetMixer(SoundType type) => _mixer.FindMatchingGroups(type.ToString())[0];
    public List<float> GetVolumes() => _volumes.Select(x => x.Value).ToList();

    private void Awake()
    {
        if (i) Destroy(i.gameObject);
        i = this;
    }

    private void Start()
    {
        _musicPlayer = GetComponent<MusicPlayer>();
        InitializeVolumes();
    }

    private void Update()
    {
        if (_fadeTimeLeft < 0 || _fadeTimeMax <= 0) return;

        _fadeTimeLeft -= Time.deltaTime;
        float progress = 1 - (_fadeTimeLeft / _fadeTimeMax);
        var masterVol = Mathf.Lerp(_startMasterVol, 0, progress);
        SetMasterVolume(masterVol, false);

        if (_fadeTimeLeft < 0) FinishFadeOut();
    }

    private void OnDestroy()
    {
        if (i == this) i = null;
    }

    private void FinishFadeOut()
    {
        _onCompleteFade?.Invoke();
        _fadeTimeLeft = _fadeTimeMax = 0;
    }

    private void InitializeVolumes()
    {
        _volumes.Clear();
        foreach (VolumeType type in _volTypes) _volumes.Add(type, 0.5f);
        LoadVolumeValuesFromSaveData();
        foreach (VolumeType type in _volTypes) SetVolume(type);
    }

    public void FadeOutMaster(float time, UnityAction OnComplete = null)
    {
        _fadeTimeMax = _fadeTimeLeft = time;
        _onCompleteFade = OnComplete;
        _startMasterVol = _volumes[VolumeType.MASTER];
    }

    public void SetPaused(bool paused)
    {
        foreach (var s in _soundCoordinators) s.SetPaused(paused);
    }

    public void SaveVolume()
    {
        var volumes = new List<string>();
        foreach (var volume in _volumes) volumes.Add(volume.Value.ToString());
        Utils.SaveToFile(saveFileName, string.Join(",", volumes));
    }

    private void LoadVolumeValuesFromSaveData()
    {
        var allText = Utils.ReadFromFile(saveFileName).Split(",");
        if (allText.Length != _volTypes.Count) return;

        for (int i = 0; i < _volTypes.Count; i++) _volumes[_volTypes[i]] = float.Parse(allText[i]);
    }

    private void SetVolume(VolumeType type, float vol = -1, bool save = true)
    {
        if (vol == -1) vol = _volumes[type];
        else _volumes[type] = vol;

        _mixer.SetFloat(type.ToString(), Mathf.Log10(vol) * 20);
        if (save) SaveVolume();
    }

    public void PlaySound(Sound sound, Transform caller, bool restart = true)
    {
        if (caller == null) caller = transform;
        var coordinator = GetExistingCoordinator(caller);
        coordinator.AddNewSound(sound, restart, caller != transform);
    }

    private SoundCoordinator GetExistingCoordinator(Transform caller)
    {
        for (int i = 0; i < _soundCoordinators.Count; i++) {
            var coord = _soundCoordinators[i];
            if (coord == null || coord.transform.parent == null) _soundCoordinators.RemoveAt(i);
        }

        foreach (var coord in _soundCoordinators) {
            if (coord && coord.transform.parent == caller) return coord;
        }
        return AddNewCoordinator(caller);
    }

    private SoundCoordinator AddNewCoordinator(Transform caller)
    {
        var coordObj = Instantiate(_coordinatorPrefab, caller);
        var coord = coordObj.GetComponent<SoundCoordinator>();
        _soundCoordinators.Add(coord);
        return coord;
    }
}
