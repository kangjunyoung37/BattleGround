using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//플레이어의 생명력을 담당
//피격시 피격이펙트를 표시하거나, UI업데이트를 한다.
//죽었을경우 모든 동작 스크립트 동작을 멈춘다.
public class PlayerHealth : HealthBase
{
    public float health = 100f;
    public float criticalHealth = 30f;
    public Transform healthHUD;
    public SoundList deathSound;
    public SoundList hitSound;
    public GameObject hurtPrefab;
    public float decayFactor = 0.8f;

    private float totalHeatlh;
    private RectTransform healthBar , placeHolderBar;
    private Text healthLabel;
    private float originalBarScale;
    private bool critical;

    private BlinkHUD criticalHUD;
    private HurtHUD hurtHUD;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        totalHeatlh = health;
        healthBar = healthHUD.Find("HealthBar/Bar").GetComponent<RectTransform>();
        placeHolderBar = healthHUD.Find("HealthBar/Placeholder").GetComponent<RectTransform>();
        healthLabel = healthHUD.Find("HealthBar/Label").GetComponent<Text>();
        originalBarScale = healthBar.sizeDelta.x;
        healthLabel.text = "" + (int)health;

        criticalHUD = healthHUD.Find("Bloodframe").GetComponent<BlinkHUD>();
        hurtHUD = this.gameObject.AddComponent<HurtHUD>();
        hurtHUD.Setup(healthHUD, hurtPrefab, decayFactor, transform);
    }
    private void Update()
    {
        if(placeHolderBar.sizeDelta.x > healthBar.sizeDelta.x)
        {
            placeHolderBar.sizeDelta = Vector2.Lerp(placeHolderBar.sizeDelta, healthBar.sizeDelta, 2f * Time.deltaTime);
        }
    }

    public bool IsFullLife()
    {
        return Mathf.Abs(health - totalHeatlh) < float.Epsilon;
    }
    private void UpdateHeatlhBar()
    {
        healthLabel.text = "" + (int)health;
        float scaleFactor = health / totalHeatlh;
        healthBar.sizeDelta = new Vector2(scaleFactor * originalBarScale, healthBar.sizeDelta.y);
    }
    private void Kill()
    {
        IsDead = true;
        gameObject.layer = KJY.TagAndLayer.GetLayerByName(KJY.TagAndLayer.LayerName.Default);
        gameObject.tag = KJY.TagAndLayer.TagName.Untagged;
        healthHUD.gameObject.SetActive(false);
        healthHUD.parent.Find("WeaponHUD").gameObject.SetActive(false);
        myAnimator.SetBool(KJY.AnimatorKey.Aim, false);
        myAnimator.SetBool(KJY.AnimatorKey.Cover, false);
        myAnimator.SetFloat(KJY.AnimatorKey.Speed, 0f);
        foreach(GenericBehaviour behaviour in GetComponentsInChildren<GenericBehaviour>())
        {
            behaviour.enabled = false;
        }
        SoundManager.Instance.PlayOndShotEffect((int)deathSound, transform.position, 5f);


    }
    public override void TakdDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject origin = null)
    {
        health -= damage;
        UpdateHeatlhBar();
        if(hurtPrefab && healthHUD)
        {
            hurtHUD.DrawHurtUI(origin.transform, origin.GetHashCode());
        }

        if(health <= 0)
        {
            Kill();
        }
        else if(health <= criticalHealth && !critical)
        {
            critical = true;
            criticalHUD.StartBlink();
        }
        SoundManager.Instance.PlayOndShotEffect((int)hitSound, location, 1f);
    }

}
