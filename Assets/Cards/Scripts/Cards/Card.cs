using UnityEngine;
using TMPro;
using RSG;
using Util;

public class Card : MonoBehaviour
{
	public static readonly float BREAK_CHANCE = 0.3f;

	[SerializeField]
	private int baseId;
	[SerializeField]
	private int baseHealth;
	[SerializeField]
	private Weapon baseWeapon;
	[SerializeField]
	private Armor baseArmor;
	[SerializeField]
	private int baseCost;
	[SerializeField]
	private float baseSpecialChance;
	[SerializeField]
	private bool baseIsHealable;
	[SerializeField]
	private bool baseIsCloneable;

	[SerializeField]
	private TextMeshPro healthText;
	[SerializeField]
	private TextMeshPro attackText;
	[SerializeField]
	private TextMeshPro armorText;
	[SerializeField]
	private TextMeshPro costText;

	private int currentHealth;

	public int id { get => this.baseId; }
	public int health
	{
		get => this.currentHealth;
		set
		{
			this.currentHealth = Mathf.Clamp(value, 0, this.baseHealth);

			if(this.IsDead())
			{
				this.gameObject.SetActive(false);
			}
		}
	}
	public Weapon weapon { get; set; }
	public Armor armor { get; set; }
	public int cost { get => this.baseCost; }
	public float specialChance { get => this.baseSpecialChance; }
	public bool isHealable { get => this.baseIsHealable; }
	public bool isCloneable { get => this.baseIsCloneable; }

	private void Awake()
	{
		this.health = this.baseHealth;
		this.weapon = (Weapon) this.baseWeapon.Clone();
		this.armor = (Armor) this.baseArmor.Clone();
	}


	private void Update()
	{
		this.healthText.text = this.health.ToString();
		this.attackText.text = this.weapon.damage.ToString();
		this.armorText.text = this.armor.defense.ToString();
		this.costText.text = this.cost.ToString();
	}
	
	public bool IsDead()
	{
		return this.health <= 0f;
	}

	public bool IsFullHealth()
	{
		return this.health == this.baseHealth;
	}

	public bool HasWeaponUpgrade()
	{
		return this.weapon.name != this.baseWeapon.name;
	}

	public bool HasArmorUpgrade()
	{
		return this.armor.name != this.baseArmor.name;
	}

	public void Attack(Card target)
	{
		this.weapon.Attack(target);
	}

	public void Damage(int damage)
	{
		this.health -= this.armor.AbsorbDamage(damage);

		bool hasWeapon = this.HasWeaponUpgrade();
		bool hasArmor = this.HasArmorUpgrade();

		if((hasWeapon || hasArmor) && MiscUtils.Chance(BREAK_CHANCE))
		{
			if(hasWeapon)
			{
				this.weapon = (Weapon) this.baseWeapon.Clone();
			}

			if(hasArmor)
			{
				this.armor = new Armor("default", 0);
			}
		}
	}

	public IPromise DoSpecialWithChance(GameManager gm, Side side, int row, int pos)
	{
		return MiscUtils.PercentChance(this.specialChance) ? this.DoSpecial(gm, side, row, pos) : Promise.Resolved();
	}

	public virtual IPromise DoSpecial(GameManager gm, Side side, int row, int pos)
	{
		return Promise.Resolved();
	}

	public IState SaveState()
	{
		return new State(this.id, this.health, (Weapon) this.weapon.Clone(), (Armor) this.armor.Clone());
	}

	public void LoadState(IState state)
	{
		this.health = state.GetHealth();
		this.weapon = state.GetWeapon();
		this.armor = state.GetArmor();
	}

	public interface IState
	{
		int GetId();

		int GetHealth();

		Weapon GetWeapon();

		Armor GetArmor();
	}

	private class State : IState
	{
		private int id;
		private int health;
		private Weapon weapon;
		private Armor armor;

		public State(int id, int health, Weapon weapon, Armor armor)
		{
			this.id = id;
			this.health = health;
			this.weapon = weapon;
			this.armor = armor;
		}

		public int GetId()
		{
			return this.id;
		}

		public int GetHealth()
		{
			return this.health;
		}

		public Weapon GetWeapon()
		{
			return this.weapon;
		}

		public Armor GetArmor()
		{
			return this.armor;
		}
	}
}
