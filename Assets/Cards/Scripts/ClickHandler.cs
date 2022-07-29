using Util;
using RSG;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour, IObserver
{
	[SerializeField]
	GameManager gameManager;

	[SerializeField]
	private Camera uiCamera;

	[SerializeField]
	private Transform heldCardAnchor;

	private Card heldCard;

	private List<int>[] blockedRows = { new List<int>(), new List<int>() };

	private void Awake()
	{
		this.gameManager.Subscribe(this);
	}

	private void Update()
	{
		if(this.gameManager.phase != Phase.Pick)
		{
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			if(this.heldCard != null)
			{
				if (!MiscUtils.RayTraceObject<DropZone>(Camera.main, this.ClickedZone))
				{
					this.SetHeldCard(null);
				}
			}
			else
			{
				MiscUtils.RayTraceObject<Card>(Camera.main, this.ClickedMainCard);
				MiscUtils.RayTraceObject<Card>(this.uiCamera, this.ClickedUiCard);
			}
		}

		if(this.heldCard != null)
		{
			this.heldCard.gameObject.transform.localPosition = new Vector3(Input.mousePosition.x - Screen.width / 2f, Input.mousePosition.y - Screen.height / 2f, -20f); // TODO: for some reason using an anchor in the bottom right causes a weird offset so just use this instead
		}
	}

	public void Notify(IObservable o, string property)
	{
		if(o is GameManager)
		{
			if (property == "phase")
			{
				this.SetHeldCard(null);
			}
		}
	}

	private void SetHeldCard(Card card)
	{
		if(this.heldCard != null)
		{
			Destroy(this.heldCard.gameObject);
		}

		this.heldCard = card;
	}

	private void FindCardRowAndPos(Card card, Army army, out int cardRow, out int cardPos) // TODO: bad performance
	{
		for(int row = 0; row < army.Rows(); ++row)
		{
			int pos = army.GetRow(row).IndexOf(card);
			
			if(pos != -1)
			{
				cardRow = row;
				cardPos = pos;
				return;
			}
		}

		cardRow = -1;
		cardPos = -1;
	}

	private void ClickedMainCard(Card card)
	{
		Side side = card.gameObject.transform.position.x < 0f ? Side.Left : Side.Right;
		Player player = this.gameManager.GetPlayer(side); // TODO: bad
		Army army = player.army;

		this.FindCardRowAndPos(card, army, out int row, out int pos);

		if (this.blockedRows[(int)side].Contains(row))
		{
			return;
		}

		army.RemoveCard(row, pos);

		this.blockedRows[(int) side].Add(row);

		List<IPromise> promises = new List<IPromise>();

		int newPos = 0;
		foreach(Card cardToMove in army.GetRow(row))
		{
			promises.Add(this.Promisify(cardToMove.gameObject.GetComponent<Movable>().MoveTo(army.CalculateCardPosition(row, newPos++), 0.5f, AnimUtils.MakeEase<Vector3>(Vector3.Lerp, 0f, 1f))));
		}

		Promise.All(promises).Then(() => this.blockedRows[(int) side].Remove(row));

		player.gold += card.cost;
	}

	private void ClickedUiCard(Card card)
	{
		Card heldCard = CardRegistry.Instance.GetById(card.id).CreateUi(this.heldCardAnchor);
		this.SetHeldCard(heldCard);
	}

	private void ClickedZone(DropZone zone)
	{
		Player player = this.gameManager.GetPlayer(zone.side);

		if(player.gold < this.heldCard.cost)
		{
			return;
		}

		player.army.AddCard(zone.row, CardRegistry.Instance.GetById(this.heldCard.id).Create());

		player.gold -= this.heldCard.cost;

		this.SetHeldCard(null);
	}
}