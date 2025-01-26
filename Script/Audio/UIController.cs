using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider musicSlider, sfxSlider;
    public Button musicButton, sfxButton;
    public Sprite sprite1, sprite2, sprite3, sprite4;
    
    private bool isMusicButtonToggled = false;
    private bool isSFXButtonToggled = false;

    void Start()
    {
        // 슬라이더에 이벤트 리스너 추가
        musicSlider.onValueChanged.AddListener(delegate { UpdateMusicButtonImage(); });
        sfxSlider.onValueChanged.AddListener(delegate { UpdateSFXButtonImage(); });

        // 초기 이미지 업데이트
        UpdateMusicButtonImage();
        UpdateSFXButtonImage();

        // 버튼에 이벤트 리스너 추가
        musicButton.onClick.AddListener(ToggleMusicButton);
        sfxButton.onClick.AddListener(ToggleSFXButton);
    }

    public void ToggleSFX()
    {
        AudioManager.instance.ToggleSFX();
    }

    public void SFXVolume()
    {
        AudioManager.instance.SFXVOlume(sfxSlider.value);
    }

    void UpdateMusicButtonImage()
    {
        if (isMusicButtonToggled || musicSlider.value == 0)
        {
            musicButton.image.sprite = sprite4;
        }
        else
        {
            musicButton.image.sprite = GetSpriteForValue(musicSlider.value);
        }
    }

    void UpdateSFXButtonImage()
    {
        if (isSFXButtonToggled || sfxSlider.value == 0)
        {
            sfxButton.image.sprite = sprite4;
        }
        else
        {
            sfxButton.image.sprite = GetSpriteForValue(sfxSlider.value);
        }
    }

    Sprite GetSpriteForValue(float value)
    {
        if (value > 0.66f)
        {
            return sprite1;
        }
        else if (value > 0.33f)
        {
            return sprite2;
        }
        else if (value > 0.01f)
        {
            return sprite3;
        }
        else
        {
            return sprite4;
        }
    }

    void ToggleMusicButton()
    {
        isMusicButtonToggled = !isMusicButtonToggled;
        UpdateMusicButtonImage();
    }

    void ToggleSFXButton()
    {
        isSFXButtonToggled = !isSFXButtonToggled;
        UpdateSFXButtonImage();
    }
}
