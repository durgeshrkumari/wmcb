﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wmcb.model.Data;

namespace wmcb.repo
{
    public class UsersRepo
    {
        public WmcbUser GetUserDetails( string email)
        {
            using (var context = new wmcbContext())
            {
                var user = context.Users.Include("Team").Include("Roles").Include("Roles.Role")
                    .Where(u => u.Email.Equals(email))
                    .Select(s => s);

                return user.FirstOrDefault();
            }
        }

        public WmcbUser getLoggedInUser(string _username, string _password)
        {
            using (var context = new wmcbContext())
            {
                var encodedPwd = Helpers.SHA1.Encode(_password);
                var user = context.Users.Include("Roles").Include("Roles.Role").Include("Team")
                           .Where( u => u.Email.Equals(_username) && u.Password.Equals(encodedPwd))
                           .Select(u => u);
            
                return user.FirstOrDefault();
            }
        }

        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <param name="_username">User name</param>
        /// <param name="_password">User password</param>
        /// <returns>True if user exist and password is correct</returns>
        public bool IsLoginValid(string _username, string _password)
        {
            using (var context = new wmcbContext())
            {
                var encodedPwd = Helpers.SHA1.Encode(_password); 
                var user = context.Users
                    .Where(u => u.Email.Equals(_username) && u.Password.Equals(encodedPwd))
                    .Take(1);
                
                return (user!=null && user.Count()>0);
            }
        }

        public List<Player> GetTeamPlayers(int teamId)
        {
            using (var context = new wmcbContext())
            {
                var teamPlayers = context.Users.Include("Team")
                                                .Where(p => teamId == p.TeamId.Value)
                                                .Select(p => new Player
                                                                    {
                                                                        ID = p.ID,
                                                                        FirstName = p.FirstName,
                                                                        LastName = p.LastName,
                                                                        Team = p.Team,
                                                                        TeamId = p.TeamId,
                                                                        Email = p.Email
                                                                    })
                                                .OrderBy(p => p.LastName)
                                                .ThenBy(p => p.FirstName);

                return teamPlayers.ToList();
            }
        }
    }
}
