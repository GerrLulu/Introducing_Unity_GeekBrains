using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menu
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private float _minVolume;
        [SerializeField] private float _maxVolume;

        [SerializeField] private Text _value;
        
        [SerializeField] private Toggle _muteToggle;
        
        [SerializeField] private Slider _sliderSoundVolume;
        
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private AudioMixerGroup _mixerGroup;


        private void Start()
        {
            _sliderSoundVolume.onValueChanged.AddListener(ChangeVolume);
            _muteToggle.onValueChanged.AddListener(ToggleMusic);

            _value.text = Mathf.Round(_sliderSoundVolume.value * 100).ToString();
        }

        private void OnDestroy()
        {
            _sliderSoundVolume.onValueChanged.RemoveAllListeners();
            _muteToggle.onValueChanged.RemoveAllListeners();
        }


        public void ToggleMusic(bool enabled)
        {
            if (enabled)
                _mixer.SetFloat(_mixerGroup.name, _minVolume);
            else
                _mixer.SetFloat(_mixerGroup.name, _maxVolume);
        }

        private void ChangeVolume(float volume)
        {
            _mixer.SetFloat(_mixerGroup.name, Mathf.Lerp(_minVolume, _maxVolume, volume));
            _value.text = Mathf.Round(volume * 100).ToString();
        }
    }
}