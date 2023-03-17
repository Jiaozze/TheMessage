using System.Collections.Generic;
using UniRx;

public class FengyunbianhuanModel
{
    public ReactiveCollection<CardFS> boxCards{ get; private set; }
    public BoolReactiveProperty isTarget { get; private set; }
    public ReactiveDictionary<int, CardColorEnum> chooseCardInfo { get; private set; }


    public FengyunbianhuanModel(List<CardFS> cards)
    {
        isTarget = new BoolReactiveProperty(false);
        chooseCardInfo = new ReactiveDictionary<int, CardColorEnum>();
        boxCards = new ReactiveCollection<CardFS>();
        foreach (CardFS card in cards)
        {
            boxCards.Add(card);
        }
    }

}
