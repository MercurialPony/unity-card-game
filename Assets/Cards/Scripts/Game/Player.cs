using System.Collections.Generic;
using UnityEngine;

public class Player : Observable
{
	public static readonly int STARTING_GOLD = 30;

	private int goldInternal = STARTING_GOLD;

	public Side side { get; private set; }
	public Army army { get; private set; }
	public int gold
	{
		get => this.goldInternal;
		set
		{
			int oldGold = this.goldInternal;
			this.goldInternal = value;

			if(this.goldInternal != oldGold)
			{
				this.Notify("gold");
			}
		}
	}

	public Player(Side side, int rows, float offsetX, float offsetZ)
	{
		this.side = side;
		this.army = new Army(rows, offsetX, offsetZ);
	}

	public void Reset()
	{
		this.army.Clear();
		this.gold = STARTING_GOLD;
	}

	public void MakeRandomArmy(List<ICardFactory> factories)
	{
		List<ICardFactory> possibleFactories = new List<ICardFactory>(factories);

		int goldCopy = this.gold;
		int row = 0;

		while (goldCopy > 0 && possibleFactories.Count > 0)
		{
			int randomFactoryIndex = Random.Range(0, possibleFactories.Count);
			ICardFactory randomFactory = possibleFactories[randomFactoryIndex];

			int cost = randomFactory.GetCost();

			if (cost > goldCopy)
			{
				possibleFactories.RemoveAt(randomFactoryIndex);
				continue;
			}

			goldCopy -= cost;

			this.army.AddCard(row, randomFactory.Create());

			if(++row >= this.army.Rows())
			{
				row = 0;
			}
		}

		this.gold = goldCopy;
	}
}