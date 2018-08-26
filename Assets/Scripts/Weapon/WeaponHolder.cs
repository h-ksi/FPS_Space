using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 武器を持たせるスクリプト
public class WeaponHolder : MonoBehaviour {
	private GameObject m_Weapon = null;
	private string weaponPath = "Weapons/Ak_47";	

	// Use this for initialization
	void Start () {
		EquipWeapon(weaponPath);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// 武器を装備する
	public void EquipWeapon(string path){
		// 既に作られていたらまず削除
		if(m_Weapon != null){
			Destroy(m_Weapon);
			m_Weapon = null;
			Resources.UnloadUnusedAssets();
		}

		m_Weapon = Instantiate(Resources.Load(path)) as GameObject;

		m_Weapon.transform.SetParent(transform, false);
		m_Weapon.transform.localPosition = new Vector3(0.0264f,	0.1912f, -0.1083f);
		m_Weapon.transform.localRotation = Quaternion.Euler(-106.691f, -23.564f, -8.89801f);
		m_Weapon.transform.localScale = new Vector3(7f, 7f, 7f);
	}
}
