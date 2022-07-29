using RSG;
using UnityEngine;
using Util;
using System.Collections.Generic;

public class RangedCard : Card
{
	public static readonly float SPEED = 1f;

	[SerializeField]
	private int projectileDamage;
	[SerializeField]
	private GameObject projectileObject;

	public override IPromise DoSpecial(GameManager gm, Side side, int row, int pos)
	{
		return this.Shoot(gm, side, row);
	}

	private void ResetProjectile()
	{
		this.projectileObject.SetActive(false);
		this.projectileObject.transform.localPosition = Vector3.zero;
	}

	private void OnHit(Card enemyCard)
	{
		enemyCard.Damage(this.projectileDamage);
		this.ResetProjectile();
	}

	private IPromise Shoot(GameManager gm, Side side, int row)
	{
		Player enemyPlayer = gm.GetPlayer(side.Opposite());

		List<Card> cardsAlive = enemyPlayer.army.GetCardsAlive(row);

		if (cardsAlive.Count <= 0)
		{
			return Promise.Resolved();
		}

		Card enemyCard = cardsAlive[Random.Range(0, cardsAlive.Count)];

		this.projectileObject.SetActive(true);

		return gm.Promisify(this.projectileObject.GetComponent<Movable>().MoveToWithSpeed(
			enemyCard.gameObject.transform.position,
			SPEED,
			((Lerp<Vector3>) Vector3.Lerp).Add(AnimUtils.ParabolicCurveY(0.2f))))
			.Then(() => this.OnHit(enemyCard));
	}
}
