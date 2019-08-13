using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelStart : MonoBehaviour
{
    private PlayerControl player;
    private Image image;
    private GlitchEffect glitch;
    private SceneAudioManager audioManager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        image = this.GetComponentInChildren<Image>();
        glitch = Camera.main.GetComponent<GlitchEffect>();
        player.isCutScene = true;
        audioManager = GameObject.Find("SceneAudioManager").GetComponent<SceneAudioManager>();
        StartCoroutine(PlayCutScene());
    }

    IEnumerator PlayCutScene()
    {
        image.DOFade(0, 3);
        audioManager.VolumeChange("BGM", audioManager.sceneClips, 0.8f, 3);
        audioManager.VolumeChange("BGFX", audioManager.sceneClips, 0.8f, 3);
        DOTween.To(() => glitch.intensity, x => glitch.intensity = x, 0, 3).SetEase(Ease.InExpo);
        DOTween.To(() => glitch.colorIntensity, x => glitch.colorIntensity = x, 0, 3).SetEase(Ease.InExpo);
        DOTween.To(() => glitch.flipIntensity, x => glitch.flipIntensity = x, 0, 3).SetEase(Ease.InExpo);
        DOTween.To(() => glitch.flickIntensity, x => glitch.flickIntensity = x, 0, 3).SetEase(Ease.InExpo);
        yield return new WaitForSeconds(3.5f);
        player.isCutScene = false;
        Destroy(this.gameObject);
    }
}
