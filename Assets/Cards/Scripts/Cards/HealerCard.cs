using System.Collections.Generic;
using RSG;
using Util;
using UnityEngine;

public class HealerCard : Card
{
	[SerializeField]
	private int healAmount;

	private static bool IsValidCard(Card card)
	{
		return card != null && !card.IsDead() && card.isHealable && !card.IsFullHealth();
	}

	private IPromise Hop(GameManager gm)
	{
		Vector3 startPos = this.gameObject.transform.position;
		return gm.Promisify(this.gameObject.GetComponent<Movable>().LerpTo(0.5f, (m, t) => m.transform.position = startPos + AnimUtils.ParabolicCurveY(0.05f)(startPos, startPos, t))); // TODO: fix this ugly ass shit
	}

	private void HealCard(Card card)
	{
		card.health += this.healAmount;
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

		bool healed = false;

		if (IsValidCard(rightCard))
		{
			healed = true;
			this.HealCard(rightCard);
		}

		if (IsValidCard(leftCard))
		{
			healed = true;
			this.HealCard(leftCard);
		}

		return healed ? this.Hop(gm) : Promise.Resolved();
	}
}