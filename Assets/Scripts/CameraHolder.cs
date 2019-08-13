using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class CameraHolder : MonoBehaviour {

    private PlayerControl player;
    public bool isTransitioning, isLocked, smoothLock;
    private bool isShakeing;
    private float duration = 0, seed1 ,seed2;
    public Vector2 corner, corner2;
    [HideInInspector]
    public Vector2 lockPos = Vector2.zero;
    public PlayerPosition pos;
    private GameObject far = null, farR = null, near = null, nearR = null;
    private float farDis, farShift, nearDis, nearShift;
    [SerializeField]
    private EndScene end;
    private GlitchEffect glitch = null;

	// Use this for initialization
	void Start ()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        player.OnDash += StartCamShake;
        isTransitioning = false;
        isShakeing = false;
        isLocked = false;
        smoothLock = false;
        lockPos = Vector2.zero;
        if (pos != null && pos.testMode == true)
        {
            corner = pos.corner;
            corner2 = pos.corner2;
        }
        if(this.transform.Find("Near") != null)
            near = this.transform.Find("Near").gameObject;
        if(this.transform.Find("Near'") != null)
            nearR = this.transform.Find("Near'").gameObject;
        if (near != null && nearR != null)
            nearDis = nearR.transform.position.x - near.transform.position.x;
        nearShift = 0;
        if(this.transform.Find("Far") != null)
            far = this.transform.Find("Far").gameObject;
        if(this.transform.Find("Far'") != null)
            farR = this.transform.Find("Far'").gameObject;
        if (far != null && farR != null)
            farDis = farR.transform.position.x - far.transform.position.x;
        nearShift = 0;
        glitch = this.GetComponent<GlitchEffect>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(isTransitioning == true)
        {
            DOTween.Kill(this.GetComponent<Camera>());
            isTransitioning = false;
        }
        if(player.dead)
        {
            StartCoroutine(CamShake());
            StartCoroutine(Delock());
            if(glitch != null)
            {
                glitch.intensity = 1;
                glitch.flipIntensity = 1;
                //glitch.flickIntensity = 1;
                glitch.colorIntensity = 1;
                DOTween.To(() => glitch.intensity, x => glitch.intensity = x, 0, 0.2f).SetEase(Ease.InExpo);
                DOTween.To(() => glitch.flipIntensity, x => glitch.flipIntensity = x, 0, 0.2f).SetEase(Ease.InExpo);
                DOTween.To(() => glitch.colorIntensity, x => glitch.colorIntensity = x, 0, 0.2f).SetEase(Ease.InExpo);
                DOTween.To(() => glitch.flickIntensity, x => glitch.flickIntensity = x, 0, 0.2f).SetEase(Ease.InExpo);
            }
        }
    }

    private void LateUpdate()
    {
        if (player.complete)
            return;
        Vector3 camPos = this.transform.position;
        if (isLocked == false || (isLocked && new Vector2(camPos.x, camPos.y) != lockPos))
        {
            bool locking = (isLocked && new Vector2(camPos.x, camPos.y) != lockPos);
            Vector2 playerPos = ((locking)? lockPos : new Vector2(player.transform.position.x, player.transform.position.y));
            Vector2 distance = playerPos - new Vector2(camPos.x, camPos.y);
            float followStrengh = ((smoothLock || locking)? Mathf.Lerp(5f, 20f, distance.magnitude / 100) : Mathf.Lerp(5f, 40f, distance.magnitude / 100));
            camPos.x = Mathf.Lerp(camPos.x, playerPos.x, followStrengh * Time.deltaTime);
            if (camPos.x > Mathf.Max(corner.x, corner2.x))
                camPos.x = Mathf.Max(corner.x, corner2.x);
            else if (camPos.x < Mathf.Min(corner.x, corner2.x))
                camPos.x = Mathf.Min(corner.x, corner2.x);
            camPos.y = Mathf.Lerp(camPos.y, playerPos.y, followStrengh * Time.deltaTime);
            if (camPos.y > Mathf.Max(corner.y, corner2.y))
                camPos.y = Mathf.Max(corner.y, corner2.y);
            else if (camPos.y < Mathf.Min(corner.y, corner2.y))
                camPos.y = Mathf.Min(corner.y, corner2.y);
            camPos.z = -10;
            this.transform.position = camPos;
        }
        if (isShakeing)
        {
            int frequency = 8;
            Vector2 pos = this.transform.position;
            pos.x += (Mathf.PerlinNoise(seed1, duration * frequency) - 0.5f) * (0.5f - (1.2f * duration));
            pos.y += (Mathf.PerlinNoise(seed2, duration * frequency) - 0.5f) * (0.5f - (1.2f * duration));
            this.transform.position = new Vector3(pos.x, pos.y, this.transform.position.z);
        }
    }

    void StartCamShake(object sender, EventArgs e)
    {
        StartCoroutine(CamShake());
    }

    IEnumerator CamShake()
    {
        seed1 = UnityEngine.Random.Range(-5, 5);
        seed2 = UnityEngine.Random.Range(-5, 5);
        isShakeing = true;
        while(duration < 0.3f)
        {
            duration += Time.unscaledDeltaTime;
            yield return null;
        }
        isShakeing = false;
        duration = 0;
    }

    IEnumerator Delock()
    {
        yield return new WaitForSeconds(0.7f);
        isLocked = false;
    }

    public IEnumerator MoveParallax(Vector3 endPos, Vector3 oldPos, float time)
    {
        if (far == null || farR == null || near == null || nearR == null)
            yield break;
        float deltaX = endPos.x - oldPos.x;
        float deltaY = endPos.y - oldPos.y;
        float farX = -deltaX / 7.5f, farY = -deltaY / 16.5f, nearX = -deltaX / 5f, nearY = -deltaY / 11f;
        nearShift -= nearX;
        farShift -= farX;
        if (nearShift >= nearDis || nearShift < 0)
        {
            if(nearShift >= 0)
            {
                near.transform.localPosition = new Vector3(nearR.transform.localPosition.x + nearDis, near.transform.localPosition.y, near.transform.localPosition.z);
                nearShift -= nearDis;
            }
            else
            {
                nearR.transform.localPosition = new Vector3(near.transform.localPosition.x - nearDis, nearR.transform.localPosition.y, nearR.transform.localPosition.z);
                nearShift += nearDis;
            }
            GameObject temp = near;
            near = nearR;
            nearR = temp;
        }
        if(farShift >= farDis || farShift < 0)
        {
            if (farShift >= 0)
            {
                far.transform.localPosition = new Vector3(farR.transform.localPosition.x + nearDis, far.transform.localPosition.y, far.transform.localPosition.z);
                farShift -= farDis;
            }
            else
            {
                farR.transform.localPosition = new Vector3(far.transform.localPosition.x - nearDis, farR.transform.localPosition.y, farR.transform.localPosition.z);
                farShift += farDis;
            }
            GameObject temp = far;
            far = farR;
            farR = temp;
        }
        Vector3 farM = new Vector3(far.transform.localPosition.x + farX, far.transform.localPosition.y + farY,10);
        Vector3 farRM = new Vector3(farR.transform.localPosition.x + farX, farR.transform.localPosition.y + farY,10);
        Vector3 nearM = new Vector3(near.transform.localPosition.x + nearX, near.transform.localPosition.y + nearY,10);
        Vector3 nearRM = new Vector3(nearR.transform.localPosition.x + nearX, nearR.transform.localPosition.y + nearY,10);
        far.transform.DOLocalMove(farM, time).SetUpdate(true);
        farR.transform.DOLocalMove(farRM, time).SetUpdate(true);
        near.transform.DOLocalMove(nearM, time).SetUpdate(true);
        nearR.transform.DOLocalMove(nearRM, time).SetUpdate(true);
        yield return new WaitForSeconds(0.75f);
    }

    public IEnumerator MoveParallax(Vector3 endPos, Vector3 oldPos, float time, Ease ease)
    {
        if (far == null || farR == null || near == null || nearR == null)
        {
            StartCoroutine(end.EndCutScene());
            yield break;
        }
        float deltaX = endPos.x - oldPos.x;
        float deltaY = endPos.y - oldPos.y;
        float farX = -deltaX / 7.5f, farY = -deltaY / 16.5f, nearX = -deltaX / 5f, nearY = -deltaY / 11f;
        nearShift -= nearX;
        farShift -= farX;
        if (nearShift >= nearDis || nearShift < 0)
        {
            if (nearShift >= 0)
            {
                near.transform.localPosition = new Vector3(nearR.transform.localPosition.x + nearDis, near.transform.localPosition.y, near.transform.localPosition.z);
                nearShift -= nearDis;
            }
            else
            {
                nearR.transform.localPosition = new Vector3(near.transform.localPosition.x - nearDis, nearR.transform.localPosition.y, nearR.transform.localPosition.z);
                nearShift += nearDis;
            }
            GameObject temp = near;
            near = nearR;
            nearR = temp;
        }
        if (farShift >= farDis || farShift < 0)
        {
            if (farShift >= 0)
            {
                far.transform.localPosition = new Vector3(farR.transform.localPosition.x + nearDis, far.transform.localPosition.y, far.transform.localPosition.z);
                farShift -= farDis;
            }
            else
            {
                farR.transform.localPosition = new Vector3(far.transform.localPosition.x - nearDis, farR.transform.localPosition.y, farR.transform.localPosition.z);
                farShift += farDis;
            }
            GameObject temp = far;
            far = farR;
            farR = temp;
        }
        Vector3 farM = new Vector3(far.transform.localPosition.x + farX, far.transform.localPosition.y + farY, 10);
        Vector3 farRM = new Vector3(farR.transform.localPosition.x + farX, farR.transform.localPosition.y + farY, 10);
        Vector3 nearM = new Vector3(near.transform.localPosition.x + nearX, near.transform.localPosition.y + nearY, 10);
        Vector3 nearRM = new Vector3(nearR.transform.localPosition.x + nearX, nearR.transform.localPosition.y + nearY, 10);
        far.transform.DOLocalMove(farM, time).SetUpdate(true).SetEase(ease);
        farR.transform.DOLocalMove(farRM, time).SetUpdate(true).SetEase(ease);
        near.transform.DOLocalMove(nearM, time).SetUpdate(true).SetEase(ease);
        nearR.transform.DOLocalMove(nearRM, time).SetUpdate(true).SetEase(ease);
        yield return new WaitForSeconds(time - 0.5f);
        StartCoroutine(end.EndCutScene());
    }

    public void EndLevel(float move)
    {
        float endY = this.transform.position.y + move;
        Vector3 oldPos = this.transform.position;
        Vector3 endPos = new Vector3(oldPos.x, endY + 220f, oldPos.z);
        StartCoroutine(MoveParallax(endPos, oldPos, 3.0f, Ease.InOutQuint));
        this.transform.DOMoveY(endY, 3).SetEase(Ease.InOutQuint);
    }
}
