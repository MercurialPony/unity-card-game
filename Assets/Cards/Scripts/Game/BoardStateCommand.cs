using System.Collections.Generic;

public class BoardStateCommand : ICommand
{
    private Army leftArmy;
    private Army rightArmy;

    private List<Card.IState>[] leftStates;
    private List<Card.IState>[] rightStates;

    public BoardStateCommand(Army leftArmy, Army rightArmy)
    {
        this.leftArmy = leftArmy;
        this.rightArmy = rightArmy;

        this.leftStates = SaveArmy(leftArmy);
        this.rightStates = SaveArmy(rightArmy);
    }

    private static List<Card.IState>[] SaveArmy(Army army)
    {
        List<Card.IState>[] states = new List<Card.IState>[army.Rows()];

        for(int i = 0; i < states.Length; ++i)
        {
            states[i] = new List<Card.IState>();

            foreach(Card card in army.GetRow(i))
            {
                states[i].Add(card.SaveState());
            }
        }

        return states;
    }

    private static void LoadArmy(Army army, List<Card.IState>[] states)
    {
        army.Clear();

        for(int i = 0; i < states.Length; ++i)
        {
            foreach(Card.IState state in states[i])
            {
                Card card = CardRegistry.Instance.GetById(state.GetId()).Create();
                card.LoadState(state);
                army.AddCard(i, card);
            }
        }
    }

    public void Execute()
    {
        LoadArmy(this.leftArmy, this.leftStates);
        LoadArmy(this.rightArmy, this.rightStates);
    }
}
