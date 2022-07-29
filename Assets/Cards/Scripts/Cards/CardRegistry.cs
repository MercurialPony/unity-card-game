using System.Collections.Generic;
using System.Linq;

public class CardRegistry
{
	private static CardRegistry lazyRegistry;

	public static CardRegistry Instance
	{
		get
		{
			if(lazyRegistry == null)
			{
				lazyRegistry = new CardRegistry();
				lazyRegistry.Add(new SquireFactory());
				lazyRegistry.Add(new RangedFactory());
				lazyRegistry.Add(new MageFactory());
				lazyRegistry.Add(new HealerFactory());
				lazyRegistry.Add(new KnightFactory());
				lazyRegistry.Add(new TumbleweedFactory());
			}

			return lazyRegistry;
		}
	}

	private Dictionary<int, ICardFactory> registry;

	public CardRegistry()
	{
		this.registry = new Dictionary<int, ICardFactory>();
	}

	public List<ICardFactory> GetAll()
	{
		return this.registry.Values.ToList();
	}

	public ICardFactory GetById(int id)
	{
		return this.registry[id];
	}

	public void Add(CardFactory factory)
	{
		this.registry.Add(factory.GetId(), factory);
	}
}
