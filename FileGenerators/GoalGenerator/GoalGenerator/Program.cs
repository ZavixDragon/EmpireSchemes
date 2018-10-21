using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace GoalGenerator
{
    internal class Program
    {
        private static List<string> _districts = new List<string>
        {
            "Royal Gardens",
            "Military Compound",
            "Commercial District",
            "Tech Community",
            "Noble Block",
            "Mall Plaza",
            "News Square",
            "Residential Area",
            "Manufacturing Zone",
            "Smuggling Center"
        };
        
        private static Random _random = new Random(Guid.NewGuid().GetHashCode());
        
        public static void Main(string[] args)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            var iteration = 0;
            retry: ;
            iteration++;
            var goals = Enumerable.Range(0, 20).Select(_ => new Goal()).ToList();
            foreach (var district in _districts)
            {
                if (!SetGoal(goals, district, x => x.Goal1, (x, value) => x.Goal1 = value))
                    goto retry;
                if (!SetGoal(goals, district, x => x.Goal2, (x, value) => x.Goal2 = value))
                    goto retry;
                if (!SetGoal(goals, district, x => x.Goal3, (x, value) => x.Goal3 = value))
                    goto retry;
                if (!SetGoal(goals, district, x => x.Goal4, (x, value) => x.Goal4 = value))
                    goto retry;
                if (!SetGoal(goals, district, x => x.Goal5, (x, value) => x.Goal5 = value))
                    goto retry;
                if (!SetGoal(goals, district, x => x.Goal6, (x, value) => x.Goal6 = value))
                    goto retry;
                if (!SetGoal(goals, district, x => x.Goal7, (x, value) => x.Goal7 = value))
                    goto retry;
                if (!SetGoal(goals, district, x => x.Goal8, (x, value) => x.Goal8 = value))
                    goto retry;
                if (!SetGoal(goals, district, x => x.Goal9, (x, value) => x.Goal9 = value))
                    goto retry;
                if (!SetGoal(goals, district, x => x.Goal10, (x, value) => x.Goal10 = value))
                    goto retry;
            }
            Console.WriteLine(iteration);
            File.WriteAllText(@"C:\git\EmpireSchemes\GeneratorFiles\GoalItems.json", $"{{ \"Items\": [{string.Join(",", goals.Select(x => x.ToJson()))}] }}");
        }

        private static bool SetGoal(List<Goal> goals, string district, Func<Goal, string> getGoal, Action<Goal, string> setGoal)
        {
            var selectedGoal = goals.Where(goal => getGoal(goal) == "" && goal.Goals.All(x => !x.Contains(district))).OrderBy(_ => _random.Next()).ToList();
            if (selectedGoal.Count() < 2)
                return false;
            selectedGoal[0].Goal1 = $"Empire Controls {district}";
            selectedGoal[1].Goal1 = $"Rebel Controls {district}";
            return true;
        }
    }

    public class Goal
    {
        public string Goal1 { get; set; } = "";
        public string Goal2 { get; set; } = "";
        public string Goal3 { get; set; } = "";
        public string Goal4 { get; set; } = "";
        public string Goal5 { get; set; } = "";
        public string Goal6 { get; set; } = "";
        public string Goal7 { get; set; } = "";
        public string Goal8 { get; set; } = "";
        public string Goal9 { get; set; } = "";
        public string Goal10 { get; set; } = "";
        public List<string> Goals => new List<string> { Goal1, Goal2, Goal3, Goal4, Goal5, Goal6, Goal7, Goal8, Goal9, Goal10 };

        public string ToJson()
            => $"{{ \"Goal1\": \"{Goal1}\", \"Goal2\": \"{Goal2}\", \"Goal3\": \"{Goal3}\", \"Goal4\": \"{Goal4}\", \"Goal5\": \"{Goal5}\", \"Goal6\": \"{Goal6}\", \"Goal7\": \"{Goal7}\", \"Goal8\": \"{Goal8}\", \"Goal9\": \"{Goal9}\", \"Goal10\": \"{Goal10}\" }}";
    }
}