using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour, IObserver
{
	[SerializeField]
	private GameManager gameManager;

	[SerializeField]
	private GameObject dropZonePrefab;

	[SerializeField]
	private GameObject pickPhase;
	[SerializeField]
	private GameObject battlePhase;
	[SerializeField]
	private GameObject gameOver;

	[SerializeField]
	private Transform cardPanel;

	[SerializeField]
	private Button startButton;
	[SerializeField]
	private Button plusRowButton;
	[SerializeField]
	private Button minusRowButton;

	[SerializeField]
	private Button nextTurnButton;
	[SerializeField]
	private Button undoButton;
	[SerializeField]
	private Button redoButton;

	[SerializeField]
	private TextMeshProUGUI leftGoldText;
	[SerializeField]
	private TextMeshProUGUI rightGoldText;

	[SerializeField]
	private TextMeshProUGUI winnerText;

	private List<GameObject> dropZones = new List<GameObject>();



	private void Awake()
	{
		this.gameManager.Subscribe(this);

		foreach(Player player in this.gameManager.players)
		{
			player.Subscribe(this);
			player.army.Subscribe(this);
			this.SetGoldText(player);
		}

		this.SetUpCardPanel();
		this.SetUpDropZones();
	}

	public void Notify(IObservable o, string property)
	{
		if(o is GameManager gm)
		{
			if (property == "phase")
			{
				this.SetActivePhase(gm);
			}
			else if(property == "rows")
			{
				this.SetRowButtonsBlocked(gm.rows == GameManager.MAX_ROWS, gm.rows == GameManager.MIN_ROWS);
				this.SetUpDropZones();
			}
		}
		else if(o is Player player)
		{
			if (property == "gold")
			{
				this.SetGoldText(player);
			}
		}
		else if(o is Army)
		{
			if (property == "fullSize")
			{
				this.SetStartBlocked(this.gameManager.players[0].army.fullSize <= 0 || this.gameManager.players[1].army.fullSize <= 0);
			}
		}
	}



	/*
	 * 
	 * UI
	 * 
	 */





	public void SetUpCardPanel()
	{
		int i = 0;

		foreach (ICardFactory factory in CardRegistry.Instance.GetAll())
		{
			Card card = factory.CreateUi(this.cardPanel.transform);
			card.gameObject.transform.localPosition = new Vector3((i++) * 80f - 200f, 0, -10f); // TODO: dynamic pos based on card amount
		}
	}

	public void SetUpDropZones()
	{
		foreach(GameObject o in this.dropZones)
		{
			Destroy(o);
		}

		this.dropZones.Clear();

		foreach (Player player in this.gameManager.players)
		{
			for (int row = 0; row < this.gameManager.rows; ++row)
			{
				Vector3 pos = player.army.CalculateCardPosition(row, 1);
				pos.y = -0.1f;
				GameObject o = Instantiate(this.dropZonePrefab, pos, this.dropZonePrefab.transform.rotation);
				this.dropZones.Add(o);
				o.transform.SetParent(this.pickPhase.transform);
				DropZone zone = o.GetComponent<DropZone>();
				zone.side = player.side;
				zone.row = row;
			}
		}
	}

	private void SetGoldText(Player player)
	{
		if(player.side == Side.Right)
		{
			this.rightGoldText.text = player.gold.ToString();
		}
		else
		{
			this.leftGoldText.text = player.gold.ToString();
		}
	}

	private void SetStartBlocked(bool isBlocked)
	{
		this.startButton.interactable = !isBlocked;
	}

	private void SetRowButtonsBlocked(bool plusBlocked, bool minusBlocked)
	{
		this.plusRowButton.interactable = !plusBlocked;
		this.minusRowButton.interactable = !minusBlocked;
	}

	public void SetTurnBlocked(bool isBlocked)
	{
		this.nextTurnButton.interactable = !isBlocked;
	}

	public void SetHistoryButtonsBlocked(bool undoBlocked, bool redoBlocked)
	{
		this.undoButton.interactable = !undoBlocked;
		this.redoButton.interactable = !redoBlocked;
	}

	private void SetActivePhase(GameManager gm)
	{
		this.pickPhase.SetActive(gm.phase == Phase.Pick);
		this.battlePhase.SetActive(gm.phase == Phase.Battle);
		this.gameOver.SetActive(gm.phase == Phase.End);
	}

	public void SetWinner(int leftScore, int rightScore)
	{
		this.winnerText.text = leftScore == rightScore ? "Draw" : leftScore > rightScore ? "Left wins" : "Right wins";
	}



	/*
	 * 
	 * Button callbacks
	 * 
	 */


	public void RandomLeft()
	{
		this.gameManager.MakeRandomArmy(Side.Left);
	}

	public void RandomRight()
	{
		this.gameManager.MakeRandomArmy(Side.Right);
	}

	public void StartGame()
	{
		this.gameManager.StartGame();
	}

	public void NextTurn()
	{
		this.gameManager.NextTurn();
	}

	public void UndoTurn()
	{
		this.gameManager.UndoTurn();
	}

	public void RedoTurn()
	{
		this.gameManager.RedoTurn();
	}

	public void RestartGame()
	{
		this.gameManager.RestartGame();
	}

	public void AddRow()
	{
		++this.gameManager.rows;
	}

	public void RemoveRow()
	{
		--this.gameManager.rows;
	}
}