using System.Collections.Generic;
using RSG;
using Util;
using UnityEngine;

public class MageCard : Card
{
	private static bool IsValidCard(Card card)
	{
		return card != null && !card.IsDead() && card.isCloneable;
	}

	private IPromise CloneCard(GameManager gm, Card card, Army army, int row, bool isLeft)
	{
		Card newCard = CardRegistry.Instance.GetById(card.id).Create();
		army.AddCard(row, newCard);

		Vector3 startPos = this.gameObject.transform.position;
		return gm.Promisify(this.gameObject.GetComponent<Movable>().LerpTo(0.5f, (m, t) => m.transform.position = startPos +  AnimUtils.ParabolicCurveY(0.05f)(startPos, startPos, t))); // TODO: fix this ugly ass shit
	}

	public override IPromise DoSpecial(GameManager gm, Side side, int row, int pos)
	{
		Army army = gm.GetPlayer(side).army;
		List<Card> cardRow = army.GetRow(row);

		Card rightCard = null;
		Card leftCard = null;

		if(pos > 0)
		{
			rightCard = cardRow[pos - 1];
		}

		if(pos < cardRow.Count - 1)
		{
			leftCard = cardRow[pos + 1];
		}

		if (IsValidCard(rightCard))
		{
			return this.CloneCard(gm, rightCard, army, row, false);
		}

		if(IsValidCard(leftCard))
		{
			return this.CloneCard(gm, leftCard, army, row, true);
		}

		return Promise.Resolved();
	}
}