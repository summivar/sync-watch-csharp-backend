namespace FilmRoom.Models
{
    public class State
    {
        public State(bool isPlaying, double time)
        {
            this.IsPlaying = isPlaying;
            this.Time = time;
        }
        public bool IsPlaying { get; set; }
        public double Time { get; set; }
    }
}
