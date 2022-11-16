namespace JTuresson.AdventOfCode.AOCClient
{
    public interface IAdventOfCodeCache
    {
        public void AddInput(int year, int day, string input);
        public bool HasInput(int year, int day);
        public string GetInput(int year, int day);
        public void DeleteDay(int year, int day);
        public void DeleteYear(int year);
        public void DeleteAll();

        public void AddDescription(int year, int day, string description);
        public bool HasDescription(int year, int day);
        public string GetDescription(int year, int day);
    }
}