using System;
using System.Collections.Generic;
using RSG;
using Util;
using UnityEngine;

// Done:
// Prototype
// Bridge
// Factory
// Lazy Init
// Memento
// Command
// Facade
// Observer

// Maybe:
// Singleton (?)
// Iterator (?)
// Object pool (?)

public enum Side
{
	Left,
	Right
}

public enum Phase
{
	Pick,
	Battle,
	End
}

public static class SideUtil
{
	public static readonly Side[] all = { Side.Left, Side.Right };

	public static Side Opposite(this Side side)
	{
		return side == Side.Left ? Side.Right : Side.Left;
	}

	public static Side Random()
	{
		return (Side)UnityEngine.Random.Range(0, 2);
	}
}

public class GameManager : MonoBehaviour, IObservable
{
	public static readonly float CARD_OFFSET = 0.2f;

	public static readonly int MIN_ROWS = 1;
	public static readonly int MAX_ROWS = 5;

	[SerializeField]
	private UiManager uiManager;

	[SerializeField]
	private Movable mainCamera;
	[SerializeField]
	private Transform normalPivot;
	[SerializeField]
	private Transform overviewPivot;

	private List<IObserver> observers = new List<IObserver>();

	public Player[] players { get; private set; } = new Player[] { new Player(Side.Left, 3, -CARD_OFFSET, CARD_OFFSET), new Player(Side.Right, 3, CARD_OFFSET, CARD_OFFSET) };

	private Phase phaseInternal = Phase.Pick;

	public Phase phase
	{
		get => this.phaseInternal;
		set
		{
			Phase oldPhase = this.phaseInternal;
			this.phaseInternal = value;

			if(this.phaseInternal != oldPhase)
			{
				this.Notify("phase");
			}
		}
	}

	private int rowsInternal = 3;

	public int rows
	{
		get => this.rowsInternal;
		set
		{
			if(value < MIN_ROWS || value > MAX_ROWS)
			{
				return;
			}

			int oldRows = this.rowsInternal;
			this.rowsInternal = value;

			foreach(Player player in this.players)
			{
				player.army.Resize(this.rowsInternal);
				player.Reset();
			}

			if(this.rowsInternal != oldRows)
			{
				this.Notify("rows");
			}
		}
	}

	private List<ICommand> turnHistory = new List<ICommand>();
	private int reverseTurnIndex = 0;



	/*
	* 
	* Game state
	* 
	*/



	public void Subscribe(IObserver o)
	{
		this.observers.Add(o);
	}

	public void Unsubscribe(IObserver o)
	{
		this.observers.Remove(o);
	}

	public void Notify(string property)
	{
		foreach(IObserver o in this.observers)
		{
			o.Notify(this, property);
		}
	}

	public Player GetPlayer(Side side)
	{
		return this.players[(int) side];
	}

	public void MakeRandomArmy(Side side)
	{
		this.GetPlayer(side).MakeRandomArmy(CardRegistry.Instance.GetAll());
	}



	/*
	* 
	* UI
	* 
	*/



	private void UpdateHistoryButtons()
	{
		this.uiManager.SetHistoryButtonsBlocked(this.reverseTurnIndex == this.turnHistory.Count - 1, this.reverseTurnIndex == 0);
	}



	/*
	* 
	* Turn history
	* 
	*/



	private void SaveTurn()
	{
		this.turnHistory.Add(new BoardStateCommand(this.players[0].army, this.players[1].army));
	}

	private void SwitchToTurn(int turnIndex)
	{
		if(this.turnHistory.Count <= 1)
		{
			return;
		}

		this.reverseTurnIndex = turnIndex;
		this.turnHistory[this.turnHistory.Count - 1 - this.reverseTurnIndex].Execute();

		this.uiManager.SetTurnBlocked(this.reverseTurnIndex != 0);
		this.UpdateHistoryButtons();
	}

	public void UndoTurn()
	{
		this.SwitchToTurn(this.reverseTurnIndex + 1);
	}

	public void RedoTurn()
	{
		this.SwitchToTurn(this.reverseTurnIndex - 1);
	}



	/*
	* 
	* Camera
	* 
	*/



	private void TransitionCameraTo(Transform transform)
	{
		this.Promisify(this.mainCamera.TransitionTo(transform.position, transform.rotation, 0.8f, AnimUtils.MakeEase<Vector3>(Vector3.Lerp, 0f, 1f), AnimUtils.MakeEase<Quaternion>(Quaternion.Slerp, 0f, 1f)));
	}



	/*
	* 
	* Game lifecycle
	* 
	*/



	public void StartGame()
	{
		this.SaveTurn();
		this.UpdateHistoryButtons();
		this.phase = Phase.Battle;
		this.TransitionCameraTo(this.normalPivot);
	}

	public void NextTurn()
	{
		this.CleanUp()
			.Then(() =>
			{
				IPromise[] promises = new IPromise[this.rows];

				for(int row = 0; row < this.rows; ++row)
				{
					int rowCopy = row;
					Side firstSide = SideUtil.Random();

					promises[row] = Promise.Sequence(
						() => this.Wait(UnityEngine.Random.value / 4f),
						() => this.DoAttack(firstSide, rowCopy),
						() => this.DoAttack(firstSide.Opposite(), rowCopy),
						() => this.DoSpecials(firstSide.Opposite(), rowCopy));
				}

				Promise.All(promises)
					.Then(this.EndTurn);
			});
	}

	private IPromise DoAttack(Side side, int row)
	{
		Card firstCard = this.GetPlayer(side).army.GetFirst(row);
		Card firstEnemy = this.GetPlayer(side.Opposite()).army.GetFirst(row);

		if (firstCard == null || firstCard.IsDead() || firstCard.weapon.damage <= 0 || firstEnemy == null || firstEnemy.IsDead())
		{
			return Promise.Resolved();
		}

		return this.DoCardAttackAnim(firstCard.GetComponent<Movable>(), this.GetPlayer(side).army.xOffset * -1f, () => firstCard.Attack(firstEnemy));
	}

	private IPromise DoSpecials(Side firstSide, int row)
	{
		List<Func<IPromise>> specialActions = new List<Func<IPromise>>();

		int maxIndex = Math.Max(this.players[0].army.Size(row), this.players[1].army.Size(row));

		for (int index = 2; index < maxIndex * 2; ++index)
		{
			int newIndex = index / 2;
			Side newSide = (index % 2 == 0) == (firstSide == Side.Left) ? Side.Left : Side.Right;
			Army currentArmy = this.GetPlayer(newSide).army;

			if (newIndex < currentArmy.Size(row))
			{
				Card currentCard = currentArmy.GetCard(row, newIndex);

				specialActions.Add(() =>
				{
					if(currentCard.IsDead())
					{
						return Promise.Resolved();
					}

					return currentCard.DoSpecialWithChance(this, newSide, row, newIndex);
				});
			}
		}

		return Promise.Sequence(specialActions);
	}

	private IPromise CleanUp()
	{
		this.uiManager.SetHistoryButtonsBlocked(true, true);
		this.uiManager.SetTurnBlocked(true);

		List<Func<IPromise>> promises = new List<Func<IPromise>>();

		foreach(Player player in this.players)
		{
			for(int row = 0; row < this.rows; ++row)
			{
				Army army = player.army;
				float offsetMultiplier = 0f;

				foreach (Card card in army.GetRow(row))
				{
					if (card.IsDead())
					{
						offsetMultiplier += 1f;
					}
					else if (offsetMultiplier > 0f)
					{
						// copy values for lambda
						float offset = army.xOffset * -1f * offsetMultiplier;
						Movable mover = card.GetComponent<Movable>();

						promises.Add(() => this.DoCardHopAnim(mover, offset));
					}
				}

				army.Clean(row);
			}
		}

		return Promise.Sequence(promises);
	}

	private void CheckGameOver()
	{
		int leftScore = 0;
		int rightScore = 0;

		for(int row = 0; row < this.rows; ++row)
		{
			bool leftEmpty = this.players[0].army.GetCardsAlive(row).Count <= 0;
			bool rightEmpty = this.players[1].army.GetCardsAlive(row).Count <= 0;

			if(!leftEmpty && !rightEmpty)
			{
				return;
			}

			if(leftEmpty && !rightEmpty)
			{
				++rightScore;
			}

			if(!leftEmpty && rightEmpty)
			{
				++leftScore;
			}
		}

		this.uiManager.SetWinner(leftScore, rightScore);
		this.phase = Phase.End;
	}

	private void EndTurn()
	{
		this.SaveTurn();
		this.uiManager.SetTurnBlocked(false);
		this.UpdateHistoryButtons();
		this.CheckGameOver();
	}

	public void RestartGame()
	{
		foreach (Player player in this.players)
		{
			player.Reset();
		}

		this.turnHistory.Clear();
		this.reverseTurnIndex = 0;

		this.phase = Phase.Pick;
		this.TransitionCameraTo(this.overviewPivot);
	}
}