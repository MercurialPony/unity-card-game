using System.Collections.Generic;
using RSG;
using Util;
using UnityEngine;

public class SquireCard : Card
{
	private static bool IsValidCard(Card card)
	{
		return card != null && !card.IsDead() && card.id == 4 && (!card.HasWeaponUpgrade() || !card.HasArmorUpgrade()); // TODO dont hardcode id like that
	}

	private IPromise Hop(GameManager gm)
	{
		Vector3 startPos = this.gameObject.transform.position;
		return gm.Promisify(this.gameObject.GetComponent<Movable>().LerpTo(0.5f, (m, t) => m.transform.position = startPos + AnimUtils.ParabolicCurveY(0.05f)(startPos, startPos, t))); // TODO: fix this ugly ass shit
	}

	private IPromise ArmCard(GameManager gm, Card card)
	{
		if(!card.HasWeaponUpgrade())
		{
			card.weapon = new Weapon("knight_sword", 10);
		}
		else if(!card.HasArmorUpgrade())
		{
			card.armor = new Armor("knight_armor", 10);
		}

		return this.Hop(gm);
	}

	public override IPromise DoSpecial(GameManager gm, Side side, int row, int pos)
	{
		Army army = gm.GetPlayer(side).army;
		List<Card> cardRow = army.GetRow(row);

		Card rightCard = null;
		Card leftCard = null;

		if (pos > 0)
		{
			rightCard = cardRow[pos - 1];
		}

		if (pos < cardRow.Count - 1)
		{
			leftCard = cardRow[pos + 1];
		}

		if (IsValidCard(rightCard))
		{
			return this.ArmCard(gm, rightCard);
		}

		if (IsValidCard(leftCard))
		{
			return this.ArmCard(gm, leftCard);
		}

		return Promise.Resolved();
	}
}