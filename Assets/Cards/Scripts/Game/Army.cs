using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Army : Observable
{
	private int fullSizeInternal = 0;

	public int fullSize
	{
		get => this.fullSizeInternal;
		private set
		{
			int oldFullSize = this.fullSizeInternal;
			this.fullSizeInternal = value;

			if (this.fullSizeInternal != oldFullSize)
			{
				this.Notify("fullSize");
			}
		}
	}
	public float xOffset { get; private set; }
	public float zOffset { get; private set; }

	private List<Card>[] cards;

	public Army(int rows, float xOffset, float zOffset)
	{
		this.Resize(rows);
		this.xOffset = xOffset;
		this.zOffset = zOffset;
	}

	public void Resize(int rows)
	{
		if (this.cards != null)
		{
			this.Clear();
		}

		this.cards = new List<Card>[rows];

		for (int i = 0; i < rows; ++i)
		{
			this.cards[i] = new List<Card>();
		}
	}

	private bool HasRow(int row)
	{
		return row < this.cards.Length;
	}

	public int Rows()
	{
		return this.cards.Length;
	}

	public int Size(int row)
	{
		if(!this.HasRow(row))
		{
			return -1;
		}

		return this.cards[row].Count;
	}

	public Card GetFirst(int row)
	{
		return this.GetCard(row, 0);
	}

	public Card GetCard(int row, int index)
	{
		if(!this.HasRow(row) || index >= this.cards[row].Count)
		{
			return null;
		}

		return this.cards[row][index];
	}

	public List<Card> GetRow(int row) // TODO: own impl
	{
		if(!this.HasRow(row))
		{
			return null;
		}

		return this.cards[row];
	}

	private static Vector3 CalculateCardPosition(int row, int pos, float xOffset, float zOffset)
	{
		return new Vector3((pos + 1) * xOffset, 0f, (row - 1) * zOffset);
	}

	public Vector3 CalculateCardPosition(int row, int pos)
	{
		return CalculateCardPosition(row, pos, this.xOffset, this.zOffset);
	}

	public void AddCard(int row, Card card)
	{
		if(!this.HasRow(row))
		{
			return;
		}

		card.gameObject.transform.position = this.CalculateCardPosition(row, this.Size(row));

		this.cards[row].Add(card);
		++this.fullSize;
	}

	public void RemoveCard(int row, int pos)
	{
		if (!this.HasRow(row))
		{
			return;
		}

		Card card = this.cards[row][pos];
		Object.Destroy(card.gameObject);
		this.cards[row].RemoveAt(pos);
		--this.fullSize;
	}

	public void Clean(int row)
	{
		if(!this.HasRow(row))
		{
			return;
		}

		int removed = 0;

		for(int i = this.cards[row].Count - 1; i >= 0; --i)
		{
			Card card = this.cards[row][i];

			if(card.IsDead())
			{
				this.cards[row].RemoveAt(i);
				Object.Destroy(card.gameObject);
			}

			++removed;
		}

		this.fullSize -= removed;
	}

	public void Clear()
	{
		foreach(List<Card> row in this.cards)
		{
			foreach(Card card in row)
			{
				Object.Destroy(card.gameObject);
			}

			row.Clear();
		}

		this.fullSize = 0;
	}

	public List<Card> GetCardsAlive(int row)
	{
		if(!this.HasRow(row))
		{
			return null;
		}

		return this.cards[row].Where(card => !card.IsDead()).ToList();
	}
}
