using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlotGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            var stats = ((Stat[]) Enum.GetValues(typeof(Stat))).OrderBy(_ => random.Next()).ToArray();
            var districts = ((District[]) Enum.GetValues(typeof(District))).OrderBy(_ => random.Next()).ToArray();
            var plots = GetPlotCombinations(stats, random);
            plots = WithThirdStat(WithSecondStat(stats, WithFirstStat(stats, plots, random), random));
            for (var i = 0; i < districts.Length; i++)
                plots.Skip(i * 8).Take(8).ToList().ForEach(x => x.District = districts[i]);
            File.WriteAllText(args[0], $"{{ \"Items\": [{string.Join(",", plots.Select(x => x.ToJson()))}] }}");
        }
        
        private static List<Plot> GetPlotCombinations(Stat[] stats, Random random)
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
            return plots;
        }

        private static List<Plot> WithFirstStat(Stat[] stats, List<Plot> plots, Random random)
        {
            retry: ;
            var statTrackers = stats.SelectMany(x => new List<StatTracker>
            {
                new StatTracker {Stat = x, IsPositive = true, Available = 24},
                new StatTracker {Stat = x, IsPositive = false, Available = 24},
            }).ToList();
            var availablePlots = plots.Select(x => x.Clone()).ToList();
            var assignedPlots = new List<Plot>();
            while (statTrackers.Any(x => x.Assigned != 8))
            {
                var statToAssign = statTrackers
                    .Where(x => x.Assigned != 8)
                    .GroupBy(x => x.Priority)
                    .OrderBy(group => @group.Key)
                    .First()
                    .OrderBy(_ => random.Next())
                    .First();
                var viablePlots = availablePlots.Where(plot => plot.HasUnvaluedStat(statToAssign.Stat, statToAssign.IsPositive)).ToList();
                if (viablePlots.Count == 0)
                    goto retry;
                var selectedPlot = viablePlots
                    .GroupBy(plot => plot.Reqs.Sum(req => statTrackers.Get(req.Stat, req.IsPositive).Priority))
                    .OrderByDescending(group => @group.Key)
                    .First()
                    .OrderBy(_ => random.Next())
                    .First();
                availablePlots.Remove(selectedPlot);
                assignedPlots.Add(selectedPlot);
                selectedPlot.Reqs.ForEach(req => statTrackers.Get(req.Stat, req.IsPositive).Available--);
                selectedPlot.GetStat(statToAssign.Stat).Value = 3;
                statToAssign.Assigned++;
            }
            return assignedPlots;
        }

        private static List<Plot> WithSecondStat(Stat[] stats, List<Plot> plots, Random random)
        {
            retry: ;
            var statTrackers = stats.SelectMany(x => new List<StatTracker>
            {
                new StatTracker {Stat = x, IsPositive = true, Available = 16},
                new StatTracker {Stat = x, IsPositive = false, Available = 16},
            }).ToList();
            var availablePlots = plots.Select(x => x.Clone()).ToList();
            var assignedPlots = new List<Plot>();
            while (statTrackers.Any(x => x.Assigned != 8))
            {
                var statToAssign = statTrackers
                    .Where(x => x.Assigned != 8)
                    .GroupBy(x => x.Priority)
                    .OrderBy(group => @group.Key)
                    .First()
                    .OrderBy(_ => random.Next())
                    .First();
                var viablePlots = availablePlots.Where(plot => plot.HasUnvaluedStat(statToAssign.Stat, statToAssign.IsPositive)).ToList();
                if (viablePlots.Count == 0)
                    goto retry;
                var selectedPlot = viablePlots
                    .GroupBy(plot => plot.Reqs.Sum(req => statTrackers.Get(req.Stat, req.IsPositive).Priority))
                    .OrderByDescending(group => @group.Key)
                    .First()
                    .OrderBy(_ => random.Next())
                    .First();
                availablePlots.Remove(selectedPlot);
                assignedPlots.Add(selectedPlot);
                selectedPlot.Reqs.ForEach(req => statTrackers.Get(req.Stat, req.IsPositive).Available--);
                selectedPlot.GetStat(statToAssign.Stat).Value = 2;
                statToAssign.Assigned++;
            }
            if (statTrackers.Any(stat => assignedPlots.Count(plot => plot.HasUnvaluedStat(stat.Stat, stat.IsPositive)) != 8))
                goto retry;
            return assignedPlots;
        }
        
        private static List<Plot> WithThirdStat(List<Plot> plots)
        {
            var finishedPlots = plots.Select(x => x.Clone()).ToList();
            finishedPlots.ForEach(plot => plot.Reqs.First(req => req.Value == 0).Value = 1);
            return finishedPlots;
        }
    }

    public class Plot
    {
        public Requirement Req1 { get; set; }
        public Requirement Req2 { get; set; }
        public Requirement Req3 { get; set; }
        public District District { get; set; }
        public List<Requirement> Reqs => new List<Requirement> { Req1, Req2, Req3 };
        public bool HasUnvaluedStat(Stat stat, bool isPositive) => Reqs.Any(req => req.Stat == stat && req.IsPositive == isPositive && req.Value == 0);
        public Requirement GetStat(Stat stat) => Reqs.First(x => x.Stat == stat);

        public Plot Clone() => new Plot { Req1 = Req1.Clone(), Req2 = Req2.Clone(), Req3 = Req3.Clone(), District = District };

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

        public Requirement Clone() => new Requirement { Stat = Stat, IsPositive = IsPositive, Value = Value };
        
        public string ToNormString()
        {
            var positiveString = IsPositive ? "+" : "-";
            return $"[^{Stat}] {Value}{positiveString}";
        }

        public string ToBonusString()
        {
            var positiveString = IsPositive ? "+" : "-";
            return $"[^{Stat}] {Value + 2}{positiveString}";
        }
    }
    
    public class StatTracker
    {
        public Stat Stat { get; set; }
        public bool IsPositive { get; set; }
        public int Available { get; set; }
        public int Assigned { get; set; }
        public int Priority => Available + Assigned;
    }

    public static class StatTrackerExtensions
    {
        public static StatTracker Get(this List<StatTracker> trackers, Stat stat, bool isPositive)
            => trackers.First(x => x.Stat == stat && x.IsPositive == isPositive);
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