using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Entity {
	public StatBlock stats;
	public SkillBlock skills;
	
	public List<ResistanceBlock> resistances;
	public List<ChangeOverTimeBlock> changesOverTime;
	
	public float maxHealth {
		get {
			return (stats.endurance + (stats.strength / 0.5f));	
		}
	}
	public float currentHealth;
	public void ChangeHealth (int change, DamageType damageType) {
		if (change < 0)
			change = (int) ((float) change * TotalResistance (damageType)); 
		currentHealth += change;
		currentHealth = Mathf.Clamp (currentHealth, 0.0f, maxHealth);	
	}
	public float maxEnergy {
		get {
			return (stats.resolve + (stats.intelligence / 0.5f));	
		}
	}
	public float currentEnergy;
	public void ChangeEnergy (int change) {
		currentEnergy += change;
		currentEnergy = Mathf.Clamp (currentEnergy, 0.0f, maxEnergy);
	}
	public int maxSpeed {
		get {
			return stats.agility + 5;	
		}
	}
	
	public void Start () {
		currentHealth = maxHealth;
		currentEnergy = maxEnergy;
	}
			
	public float TotalResistance (DamageType damageType) {
		int total = 100;
		foreach (ResistanceBlock resistance in resistances)
			if (resistance.damageTypeResisted == damageType)
				total += resistance.resistanceAmount;
		return Mathf.Clamp (total, 0, int.MaxValue) / 100.0f;
	}
	
	void Update () {
		foreach (ResistanceBlock r in resistances)
			if (!r.isPermenant)
				r.timeRemaining -= Time.deltaTime;
		for (int i = 0; i < resistances.Count; i++)
			if (resistances [i].timeRemaining < 0.0f && !resistances [i].isPermenant) {
				resistances.Remove (resistances [i]);
				i--;
			}
		
		foreach (ChangeOverTimeBlock cot in changesOverTime) {
			if (!cot.isPermenant)
				cot.timeRemaining -= Time.deltaTime;
			cot.currentTickTime -= Time.deltaTime;
			if (cot.currentTickTime < 0.0f) {
				cot.currentTickTime += cot.tickTime;
				ChangeHealth (cot.healthChange, cot.damageType);
				ChangeEnergy (cot.energyChange);
			}
		}
		for (int i = 0; i < changesOverTime.Count; i++)
			if (!changesOverTime [i].isPermenant && changesOverTime [i].timeRemaining < 0.0f) {
				if (changesOverTime [i].particle != null)
					foreach (Transform t in changesOverTime [i].particle.transform) {
						if (t.particleEmitter != null)
							t.particleEmitter.emit = false;
						if (t.light != null)
							t.light.enabled = false;
					}
				Destroy (changesOverTime [i].particle,10.0f);
				changesOverTime.Remove (changesOverTime [i]);
				i--;
			}
	}
	
	public bool HasChangeOverTime (string name) {
		foreach (ChangeOverTimeBlock cot in changesOverTime)
			if (StringUtility.Compare (cot.name, name))
				return true;
		return false;
	}
	
	public bool AddChangeOverTimeBlock (ChangeOverTimeBlock template) {
		if (HasChangeOverTime (template.name))
			return false;
		ChangeOverTimeBlock newCOT = new ChangeOverTimeBlock ();
		newCOT.name = template.name;
		newCOT.healthChange = template.healthChange;
		newCOT.energyChange = template.energyChange;
		newCOT.tickTime = template.tickTime;
		newCOT.currentTickTime = template.currentTickTime;
		newCOT.isPermenant = template.isPermenant;
		newCOT.timeRemaining = template.timeRemaining;
		newCOT.damageType = template.damageType;
		if (template.templateParticle != null) {
			newCOT.particle = (GameObject) Instantiate (template.templateParticle, transform.position, transform.rotation);
			newCOT.particle.transform.parent = transform;
		}
			
		changesOverTime.Add (newCOT);
		return true;
	}
}
[System.Serializable]
public class StatBlock {
	public int strength;
	public int agility;
	public int endurance;
	public int intelligence;
	public int wisdom;
	public int resolve;
}

[System.Serializable]
public class SkillBlock {
	public float software;
	public float hardware;
}

[System.Serializable]
public class ResistanceBlock {
	public DamageType damageTypeResisted;
	public int resistanceAmount;
	public bool isPermenant;
	public float timeRemaining;
}

[System.Serializable]
public class ChangeOverTimeBlock {
	public string name;
	public int healthChange;
	public int energyChange;
	public float tickTime;
	public float currentTickTime;
	public bool isPermenant;
	public float timeRemaining;
	public DamageType damageType;
	public GameObject templateParticle;
	public GameObject particle;
}