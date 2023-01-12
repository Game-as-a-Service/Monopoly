namespace Shared.Domain
{
    public class DiceSetting
    {
        public DiceSetting(int numberOfDice = 2, int min = 1, int max = 6)
        {
            if (numberOfDice < 1)
            {
                throw new ArgumentException("遊戲至少要有一顆骰子吧");
            }

            if (min > max)
            {
                throw new ArgumentException("骰子大小區間看起來好像不太對喔");
            }

            if (min <= 0)
            {
                throw new ArgumentException("骰子骰不出負數吧, 別太欺人太甚了!");
            }

            NumberOfDice = numberOfDice;
            Max = max;
            Min = min;
        }
        public int Max { get; init; }
        public int Min { get; init; }
        public int NumberOfDice { get; init; }
    }
}
