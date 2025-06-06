using Dotahold.Data.Models;

namespace Dotahold.Models
{
    public class PlayerWinLoseModel
    {
        public int Win { get; private set; }

        public int Lose { get; private set; }

        public string WinRate { get; private set; }

        public PlayerWinLoseModel(DotaPlayerWinLoseModel winLose)
        {
            this.Win = winLose.win;
            this.Lose = winLose.lose;
            this.WinRate = this.Lose == 0 ? "100%" : $"{(double)this.Win / (this.Win + this.Lose) * 100:F2}%";
        }
    }
}
