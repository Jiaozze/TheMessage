using System.Collections.Generic;
using UniRx;

public class FengyunbianhuanModel
{
    public ReactiveCollection<CardFS> boxCards{ get; private set; }
    public BoolReactiveProperty isTarget { get; private set; }
    public ReactiveCollection<List<CardColorEnum>> chooseCardInfo { get; private set; }


    public FengyunbianhuanModel()
    {
        isTarget = new BoolReactiveProperty(false);
        chooseCardInfo = new ReactiveCollection<List<CardColorEnum>>();
        boxCards = new ReactiveCollection<CardFS>();
    }

}
