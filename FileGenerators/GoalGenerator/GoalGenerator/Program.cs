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
        
        public static void Main(string[] args)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            retry: ;
            var goals = Enumerable.Range(0, 20).Select(_ => new Goal()).ToList();
            _districts.ForEach(district =>
            {
                goals.Where(goal => goal.Goal1 == null && goal.Goals.All(x => x != district))
                    .OrderBy(_ => random.Next()).Take(2).ToList()
                    .ForEach(goal => goal.Goal1 = district);
                goals.Where(goal => goal.Goal2 == null && goal.Goals.All(x => x != district)).OrderBy(_ => random.Next()).Take(2).ToList().ForEach(goal => goal.Goal2 = district);
                goals.Where(goal => goal.Goal3 == null && goal.Goals.All(x => x != district)).OrderBy(_ => random.Next()).Take(2).ToList().ForEach(goal => goal.Goal3 = district);
                goals.Where(goal => goal.Goal4 == null && goal.Goals.All(x => x != district)).OrderBy(_ => random.Next()).Take(2).ToList().ForEach(goal => goal.Goal4 = district);
                goals.Where(goal => goal.Goal5 == null && goal.Goals.All(x => x != district)).OrderBy(_ => random.Next()).Take(2).ToList().ForEach(goal => goal.Goal5 = district);
                goals.Where(goal => goal.Goal6 == null && goal.Goals.All(x => x != district)).OrderBy(_ => random.Next()).Take(2).ToList().ForEach(goal => goal.Goal6 = district);
                goals.Where(goal => goal.Goal7 == null && goal.Goals.All(x => x != district)).OrderBy(_ => random.Next()).Take(2).ToList().ForEach(goal => goal.Goal7 = district);
                goals.Where(goal => goal.Goal8 == null && goal.Goals.All(x => x != district)).OrderBy(_ => random.Next()).Take(2).ToList().ForEach(goal => goal.Goal8 = district);
                goals.Where(goal => goal.Goal9 == null && goal.Goals.All(x => x != district)).OrderBy(_ => random.Next()).Take(2).ToList().ForEach(goal => goal.Goal9 = district);
                goals.Where(goal => goal.Goal10 == null && goal.Goals.All(x => x != district)).OrderBy(_ => random.Next()).Take(2).ToList().ForEach(goal => goal.Goal10 = district);
            });
            if (goals.Any(goal => goal.Goals.Any(x => x == null)))
                goto retry;
            File.WriteAllText(args[0], $"{{ \"Items\": [{string.Join(",", goals.Select(x => x.ToJson()))}] }}");
        }
    }

    public class Goal
    {
        public string Goal1 { get; set; }
        public string Goal2 { get; set; }
        public string Goal3 { get; set; }
        public string Goal4 { get; set; }
        public string Goal5 { get; set; }
        public string Goal6 { get; set; }
        public string Goal7 { get; set; }
        public string Goal8 { get; set; }
        public string Goal9 { get; set; }
        public string Goal10 { get; set; }
        public List<string> Goals => new List<string> { Goal1, Goal2, Goal3, Goal4, Goal5, Goal6, Goal7, Goal8, Goal9, Goal10 };

        public string ToJson()
            => $"{{ \"Goal1\": \"{Goal1}\", \"Goal2\": \"{Goal2}\", \"Goal3\": \"{Goal3}\", \"Goal4\": \"{Goal4}\", \"Goal5\": \"{Goal5}\", \"Goal6\": \"{Goal6}\", \"Goal7\": \"{Goal7}\", \"Goal8\": \"{Goal8}\", \"Goal9\": \"{Goal9}\", \"Goal10\": \"{Goal10}\" }}";
    }
}