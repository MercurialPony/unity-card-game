using UnityEngine;
using Util;

public class CardFactory : ICardFactory
{
	private GameObject prefab;

	private int id;
	private int cost;

	protected CardFactory(string cardName)
	{
		this.prefab = Resources.Load(cardName) as GameObject;

		Card card = prefab.GetComponent<Card>();
		this.id = card.id;
		this.cost = card.cost;
	}

	public int GetId()
	{
		return this.id;
	}

	public int GetCost()
	{
		return this.cost;
	}

	public Card Create()
	{
		return Object.Instantiate(this.prefab).GetComponent<Card>();
	}

	public Card CreateUi(Transform t)
	{
		GameObject o = Object.Instantiate(this.prefab);
		Object.Destroy(o.GetComponent<FaceCamera>());

		o.transform.SetParent(t);
		o.transform.localScale = new Vector3(20f, 20f, 20f);
		o.transform.rotation = Quaternion.identity;

		o.SetLayerRecursively(LayerMask.NameToLayer("UI")); // TODO: cache

		return o.GetComponent<Card>();
	}
}