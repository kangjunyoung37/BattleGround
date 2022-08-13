using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ���⸦ ȹ���ϸ� ȹ���� ���⸦ UI�� ���� �����ְ�
/// ���� ��ź���� ��ü ������ �� �ִ� �Ѿ˷��� ���
/// </summary>
public class WeaponUIManager : MonoBehaviour
{
    public Color bulletColor = Color.white;
    public Color emptyBulletColor = Color.black;
    private Color noBulletColor;//�����ϰ� ���� ǥ��


    [SerializeField]
    private Image weaponHUD;
    [SerializeField]
    private GameObject bulletMag;
    [SerializeField]
    private Text totalBulletHUD;

    void Start()
    {
        noBulletColor = new Color(0f, 0f, 0f, 0f);
        if(weaponHUD == null)
        {
            weaponHUD = transform.Find("WeaponHUD/Weapon").GetComponent<Image>();   
        }
        if(bulletMag == null)
        {
            bulletMag = transform.Find("WeaponHUD/Data/Mag").gameObject;
        }
        if(totalBulletHUD == null)
        {
            totalBulletHUD = transform.Find("WeaponHUD/Data/bulletAmount").GetComponent<Text>();
        }
        Toggle(false);
    }
    public void Toggle(bool active)
    {
        weaponHUD.transform.parent.gameObject.SetActive(active);
    }

    public void UpdateWeaponHUD(Sprite weaponSprite,int bulletLeft,int fullMag,int ExtraBullet)
    {
        if(weaponSprite != null && weaponHUD.sprite != weaponSprite)
        {
            weaponHUD.sprite = weaponSprite;
            weaponHUD.type = Image.Type.Filled;
            weaponHUD.fillMethod = Image.FillMethod.Horizontal;

        }
        int bulletCount = 0;
        foreach(Transform bullet in bulletMag.transform)
        {
            //��ź��
            if(bulletCount < bulletLeft)
            {
                bullet.GetComponent<Image>().color = bulletColor;
            }
            //��ġ��ź
            else if(bulletCount >= fullMag)
            {
                bullet.GetComponent<Image>().color = noBulletColor;
                
            }
            //����� ź
            else
            {
                bullet.GetComponent<Image>().color = emptyBulletColor;
            }
            bulletCount++;
        }
        totalBulletHUD.text = bulletLeft + "/" + ExtraBullet;
    }



}
