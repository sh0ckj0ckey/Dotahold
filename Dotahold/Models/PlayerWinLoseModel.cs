namespace Dotahold.Models
{
    public class PlayerWinLoseModel
    {
        public int Win { get; private set; }

        public int Lose { get; private set; }

        public string WinRate { get; private set; }

        public PlayerWinLoseModel(int win, int lose)
        {
            this.Win = win;
            this.Lose = lose;
            this.WinRate = Lose == 0 ? "100%" : $"{(double)win / (win + lose) * 100:F2}%";
        }
    }
}
