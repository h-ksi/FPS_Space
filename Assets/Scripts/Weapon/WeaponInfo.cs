using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfo : MonoBehaviour {
	// <summary>
	// 武器の種類
	// </summary>
	public enum WeaponTypes{
		Ak_47 = 0,
		M4A1 = 1,
		M18_Smoke_Grenade = 2,
		SkorpionVZ = 3,
		UMP_45 = 4,
		maxNum = 5,
	}

	// Resoucesフォルダ以下に入っている武器のファイルパス
	public string[] m_WeaponPath = new string[]{
		"Weapons/Ak-47",
		"Weapons/M4A1_Sopmod",
		"Weapons/Smoke_Grenade",
		"Weapons/Skorpion_VZ",
		"Weapons/UMP-45"	
	};
}
