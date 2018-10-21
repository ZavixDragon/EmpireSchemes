using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace PlotGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            var stats = (Stat[]) Enum.GetValues(typeof(Stat));
            var districts = (District[]) Enum.GetValues(typeof(District));
            for(var iteration = 0; iteration < 1000000; iteration++)
            {
                var plots = new List<Plot>();
                for (var i = 0; i < stats.Length - 2; i++)
                for (var ii = i + 1; ii < stats.Length - 1; ii++)
                for (var iii = ii + 1; iii < stats.Length; iii++)
                for (var req1 = 0; req1 < 2; req1++)
                for (var req2 = 0; req2 < 2; req2++)
                for (var req3 = 0; req3 < 2; req3++)
                    plots.Add(new Plot
                    {
                        Req1 = new Requirement {Stat = stats[i], IsPositive = req1 == 0},
                        Req2 = new Requirement {Stat = stats[ii], IsPositive = req2 == 0},
                        Req3 = new Requirement {Stat = stats[iii], IsPositive = req3 == 0}
                    });
                plots = plots.OrderBy(x => random.Next()).ToList();
                foreach (var stat in stats)
                    for (var positive = 0; positive < 2; positive++)
                    for (var value = 1; value < 4; value++)
                    {
                        var applicablePlots = plots.Where(plot => plot.Reqs.All(req => req.Value != value)
                                                                  && plot.Reqs.Any(req => req.Stat == stat
                                                                                          && req.IsPositive ==
                                                                                          (positive == 1)
                                                                                          && req.Value == 0)).ToList();
                        if (applicablePlots.Count() < 8)
                            goto tryTryAgain;
                        applicablePlots.Take(8).ToList().ForEach(plot => plot.Reqs.First(x => x.Stat == stat).Value = value);
                    }
                for (var i = 0; i < districts.Length; i++)
                    plots.Skip(i * 8).Take(8).ToList().ForEach(x => x.District = districts[i]);
                File.WriteAllText(@"C:\git\EmpireSchemes\GeneratorFiles\PlotItems.json", $"{{ \"Items\": [{string.Join(",", plots.Select(x => x.ToJson()))}] }}");
                Console.WriteLine("Success!");
                return;
                tryTryAgain: ;
            }
            Console.WriteLine("Failure");
        }
    }

    public class Plot
    {
        public Requirement Req1 { get; set; }
        public Requirement Req2 { get; set; }
        public Requirement Req3 { get; set; }
        public District District { get; set; }
        public List<Requirement> Reqs => new List<Requirement> { Req1, Req2, Req3 };

        public string ToJson()
        {
            var orderedReqs = Reqs.OrderByDescending(x => x.Value).ToArray();
            return $"{{ \"Req1\": \"{orderedReqs[0].ToNormString()}\", \"Bonus1\": \"{orderedReqs[0].ToBonusString()}\", \"Req2\": \"{orderedReqs[1].ToNormString()}\", \"Bonus2\": \"{orderedReqs[1].ToBonusString()}\", \"Req3\": \"{orderedReqs[2].ToNormString()}\", \"Bonus3\": \"{orderedReqs[2].ToBonusString()}\", \"District\": \"{District}\" }}";
        }
    }

    public class Requirement
    {
        public Stat Stat { get; set; }
        public bool IsPositive { get; set; }
        public int Value { get; set; }

        public string ToNormString()
        {
            var positiveString = IsPositive ? "+" : "-";
            return $"[^{Stat}] {Value}{positiveString}";
            // [^Fear] 3-
        }

        public string ToBonusString()
        {
            var positiveString = IsPositive ? "+" : "-";
            return $"[^{Stat}] {Value + 2}{positiveString}";
            // [^Fear] 5+
        }
    }
    
    public enum Stat
    {
        Fear,
        Information,
        Military,
        Research,
        Wealth
    }

    public enum District
    {
        RoyalGardens,
        MilitaryCompound,
        CommercialDistrict,
        TechCommunity,
        NobleBlock,
        MallPlaza,
        NewsSquare,
        ResidentialArea,
        ManufacturingZone,
        SmugglingCenter
    }
}