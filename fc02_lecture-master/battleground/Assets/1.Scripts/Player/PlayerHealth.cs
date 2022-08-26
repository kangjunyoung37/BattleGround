using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//�÷��̾��� �������� ���
//�ǰݽ� �ǰ�����Ʈ�� ǥ���ϰų�, UI������Ʈ�� �Ѵ�.
//�׾������ ��� ���� ��ũ��Ʈ ������ �����.
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

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        totalHeatlh = health;
        healthBar = healthHUD.Find("HealthBar/Bar").GetComponent<RectTransform>();
        placeHolderBar = healthHUD.Find("HealthBar/Placeholder").GetComponent<RectTransform>();
        healthLabel = healthHUD.Find("HealthBar/Label").GetComponent<Text>();
        originalBarScale = healthBar.sizeDelta.x;
        healthLabel.text = "" + (int)health;

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
        gameObject.layer = FC.TagAndLayer.GetLayerByName(FC.TagAndLayer.LayerName.Default);
        gameObject.tag = FC.TagAndLayer.TagName.Untagged;
        healthHUD.gameObject.SetActive(false);
        healthHUD.parent.Find("WeaponHUD").gameObject.SetActive(false);
        myAnimator.SetBool(FC.AnimatorKey.Aim, false);
        myAnimator.SetBool(FC.AnimatorKey.Cover, false);
        myAnimator.SetFloat(FC.AnimatorKey.Speed, 0f);
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
        if(health <= 0)
        {
            Kill();
        }
        else if(health <= criticalHealth && !critical)
        {
            critical = true;
        }
        SoundManager.Instance.PlayOndShotEffect((int)hitSound, location, 1f);
    }

}