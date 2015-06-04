﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wmcb.model.Data;

namespace wmcb.repo
{
    public class StatsRepo
    {
        public List<PlayerStatsDto> GetMatchPlayerStats(int matchId)
        {
            using (var context = new wmcbContext())
            {
                var players = context.PlayerStats
                    .Include("Team")
                    .Include("Player")
                    .Include("Match")
                    .Include("Match.AwayTeam")
                    .Include("Match.HomeTeam")
                    .Where(p => p.MatchId == matchId)
                    .Select(p => new PlayerStatsDto
                    {
                        ID = p.ID,
                        TeamId = p.TeamId,
                        MatchId = p.MatchId,
                        PlayerId = p.PlayerId,
                        Team = p.Team,
                        Match = new MatchDto {
                                                ID = p.Match.ID,
                                                HomeTeamId = p.Match.HomeTeamId,
                                                AwayTeamId = p.Match.AwayTeamId,
                                                IsReviewed = p.Match.IsReviewed,
                                                HomeTeamScore = p.Match.HomeTeamScore,
                                                AwayTeamScore = p.Match.AwayTeamScore
                        },
                        BattingRuns = p.BattingRuns,
                        BallsFaced = p.BallsFaced,
                        HowOut = p.HowOut,
                        BowlerNumber = p.BowlerNumber,
                        Bowler = p.Bowler,
                        Fielder = p.Fielder,
                        OversBowled = p.OversBowled,
                        BowlingRuns = p.BowlingRuns,
                        MaidenOvers = p.MaidenOvers,
                        Wickets = p.Wickets,
                        Player = new Player{
                                            ID = p.Player.ID,
                                            FirstName = p.Player.FirstName,
                                            LastName = p.Player.LastName,
                                            Team = p.Player.Team,
                                            TeamId = p.Player.TeamId,
                                            Email = p.Player.Email
                                            }
                    }).OrderBy(p => p.Player.LastName).ThenBy(p => p.Player.FirstName);
                return players.AsEnumerable().ToList();
            }
        }

        public List<TeamStats> GetMatchTeamStats(int matchId)
        {
            using (var context = new wmcbContext())
            {
                var teamStats = context.TeamStats
                    .Include("Team")
                    .Include("Match")
                    .Where(p => p.MatchId == matchId)
                    .Select(p => p);

                return teamStats.AsEnumerable().ToList();
            }
        }

        public void SetPlayerStats(List<PlayerStats> players)
        {
            using (var context = new wmcbContext())
            {
                players.ForEach(p => {
                        PlayerStats player;
                        using (var getcontext = new wmcbContext())
                        {
                            player = getcontext.PlayerStats.FirstOrDefault(ps => ps.PlayerId == p.PlayerId && ps.MatchId == p.MatchId);
                        }

                        if (player != null)
                        {
                            if (p.IsDeleted)
                                context.Entry(player).State = System.Data.Entity.EntityState.Deleted;
                            else
                            {
                                player.BattingRuns = p.BattingRuns;
                                player.BallsFaced = p.BallsFaced;
                                player.HowOut = p.HowOut;
                                player.Bowler = p.Bowler;
                                player.Fielder = p.Fielder;
                                player.OversBowled = p.OversBowled;
                                player.MaidenOvers = p.MaidenOvers;
                                player.Wickets = p.Wickets;
                                player.BowlingRuns = p.BowlingRuns;

                                context.Entry(player).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                        else
                            context.PlayerStats.Add(p);
                });

                context.SaveChanges();
            }   
        }

        public void SetTeamStats(List<TeamStats> teamStats)
        {
            teamStats.ForEach(ts => {
                TeamStats stat;
                Match match;

                using (var getcontext = new wmcbContext())
                {
                    stat = getcontext.TeamStats.FirstOrDefault(t => ts.TeamId == t.TeamId && ts.MatchId == t.MatchId);
                    match = getcontext.Match.FirstOrDefault(m => m.ID == ts.MatchId);
                }

                using (var context = new wmcbContext())
                {        
                    if (stat != null)
                    {
                        stat.Wides = ts.Wides;
                        stat.NoBalls = ts.NoBalls;
                        stat.PenaltyRuns = ts.PenaltyRuns;
                        stat.Byes = ts.Byes;
                        stat.LegByes = ts.LegByes;
                        stat.TeamScores = ts.TeamScores;
                        context.Entry(stat).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                        context.TeamStats.Add(ts);

                    if (match.HomeTeamId == ts.TeamId)
                    {
                        match.HomeTeamScore = ts.TeamScores;
                        context.Entry(match).State = System.Data.Entity.EntityState.Modified;
                    }
                    else if (match.AwayTeamId == ts.TeamId)
                    {
                        match.AwayTeamScore = ts.TeamScores;
                        context.Entry(match).State = System.Data.Entity.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            });
        }
    }
}
