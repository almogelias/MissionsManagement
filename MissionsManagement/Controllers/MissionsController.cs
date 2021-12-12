using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using MissionsManagement.Models;
using Newtonsoft.Json;

namespace MissionsManagement.Controllers
{
    [Authorize]
    public class MissionsController : ApiController
    {
        // GET api/Missions
        public IHttpActionResult Get()
        {
                                
            var context = new MissionsManagementDBEntitiesEntities();

            /*var missions = context.Missions.Select(x=>new
            {
                x.MissionId,
                x.Mission_UserId,
                x.Title,
                x.Description,
                x.Priority,
                x.Status,
                x.StartingDate,
                x.EndingDate,
                x.ResponsibilityUserId
            }).ToList();*/

            /*foreach (var mission in missions)
            {
                Console.WriteLine(mission);
                var responsibleUser=mission.ResponsibilityUserId;
                var responsibleUserFromTable=context.AspNetUsers.Select(x => new
                {
                    x.Id,
                    x.UserName
                }).Where(f=>f.Id == responsibleUser).FirstOrDefault();
                
                
            }*/
            var missions = context.Missions
            .Join(context.AspNetUsers,
                m => m.ResponsibilityUserId,
                u => u.Id,
                (m, u) => new
                {
                    MissionId = m.MissionId,
                    Mission_UserId = m.Mission_UserId,
                    Title = m.Title,
                    Description = m.Description,
                    Priority = m.Priority,
                    Status = m.Status,
                    StartingDate = m.StartingDate,
                    EndingDate = m.EndingDate,
                    ResponsibilityUserId = m.ResponsibilityUserId,
                    ResponsibilityUsername = u.UserName
                }).ToList();

            string missions_JSON = JsonConvert.SerializeObject(missions);
            var users = context.AspNetUsers.Select(x => new
            {
                x.Id,
                x.UserName
            }).ToList();
            string users_JSON = JsonConvert.SerializeObject(users);

            return Json(new { users = users, missions = missions });
            
        }

        // GET api/Missions?action=string
        public IHttpActionResult Get(string action)
        {
            if (action == "deadlineMissions")
            {
                var context = new MissionsManagementDBEntitiesEntities();
                var date = DateTime.Now.AddDays(-7);
                var missions = context.Missions
                    .Join(context.AspNetUsers,
                        m => m.ResponsibilityUserId,
                        u => u.Id,
                        (m, u) => new
                        {
                            MissionId = m.MissionId,
                            Mission_UserId = m.Mission_UserId,
                            Title = m.Title,
                            Description = m.Description,
                            Priority = m.Priority,
                            Status = m.Status,
                            StartingDate = m.StartingDate,
                            EndingDate = m.EndingDate,
                            ResponsibilityUserId = m.ResponsibilityUserId,
                            ResponsibilityUsername = u.UserName
                        }).Where(e => e.EndingDate > date).ToList();
                return Json(missions);

            }
            
            else
            {
                return Json(new { error = "Something went wrong!"});
            }
        }


        // GET api/Missions/?action=string&startingDate=startingDate&endingDate=endingDate
        public IHttpActionResult Get(string action,DateTime startingDate, DateTime endingDate)
        {
            if (action == "getStatistics")
            {
                var context = new MissionsManagementDBEntitiesEntities();
                var missions = context.Missions
                .Join(context.AspNetUsers,
                        m => m.ResponsibilityUserId,
                        u => u.Id,
                        (m, u) => new
                        {
                            MissionId = m.MissionId,
                            Mission_UserId = m.Mission_UserId,
                            Title = m.Title,
                            Description = m.Description,
                            Priority = m.Priority,
                            Status = m.Status,
                            StartingDate = m.StartingDate,
                            EndingDate = m.EndingDate,
                            ResponsibilityUserId = m.ResponsibilityUserId,
                            ResponsibilityUsername = u.UserName
                        }).Where(e => e.StartingDate >= startingDate && e.EndingDate <= endingDate && e.Status == "Done")
                .GroupBy(x => x.ResponsibilityUsername)
                .Select(x => new { userName = x.Key, Count = x.Count() })
                .ToList();

               /* missions = context.Missions
                    .Join(context.AspNetUsers,
                        m => m.ResponsibilityUserId,
                        u => u.Id,
                        (m, u) => new
                        {
                            MissionId = m.MissionId,
                            Mission_UserId = m.Mission_UserId,
                            Title = m.Title,
                            Description = m.Description,
                            Priority = m.Priority,
                            Status = m.Status,
                            StartingDate = m.StartingDate,
                            EndingDate = m.EndingDate,
                            ResponsibilityUserId = m.ResponsibilityUserId,
                            ResponsibilityUsername = u.UserName
                        }).Where(e => e.StartingDate >= startingDate && e.EndingDate<= endingDate && e.Status=="Done").ToList();
*/

                
                return Json(missions);

            }
            
            else
            {
                return Json(new { error = "Something went wrong!" });
            }
        }
        // POST api/Missions
        public HttpResponseMessage Post([FromBody]Mission mission)
        {
            
            mission.Mission_UserId= RequestContext.Principal.Identity.GetUserId(); //add to mission the current UserId

            using (var context = new MissionsManagementDBEntitiesEntities())
            {
                var lastMissionId = 0;
                try
                {
                    lastMissionId = context.Missions.Max(x => x.MissionId); //find the last mission id in table
                }
                catch
                {
                    Console.WriteLine("Adding the first mission. lastMissionId=0");
                }
                mission.MissionId = lastMissionId + 1; //increasing the id by 1
                context.Missions.Add(mission);

                context.SaveChanges();
            }
            Console.WriteLine("test");
            return Request.CreateResponse(HttpStatusCode.OK, "Mission had been submitted!");
        }



        // PUT api/Missions/5
        public IHttpActionResult Put(int id, [FromBody] Mission editedMission)
        {

            editedMission.Mission_UserId = RequestContext.Principal.Identity.GetUserId(); //add to editedMission the current UserId

            var oldMission = new Mission();
            var context = new MissionsManagementDBEntitiesEntities();
            
            oldMission = context.Missions.Select(m => m).Where(f=>f.MissionId== id).FirstOrDefault();
            oldMission.Title = editedMission.Title;
            oldMission.Description = editedMission.Description;
            oldMission.Priority = editedMission.Priority;
            oldMission.Status = editedMission.Status;
            oldMission.StartingDate = editedMission.StartingDate;
            oldMission.EndingDate = editedMission.EndingDate;
            oldMission.ResponsibilityUserId = editedMission.ResponsibilityUserId;


            context.Entry(oldMission).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            //Get the edited row from table into list
            var editedMissionSelected = context.Missions.Select(m => new
            {
                MissionId = m.MissionId,
                Mission_UserId = m.Mission_UserId,
                Title = m.Title,
                Description = m.Description,
                Priority = m.Priority,
                Status = m.Status,
                StartingDate = m.StartingDate,
                EndingDate = m.EndingDate,
                ResponsibilityUserId = m.ResponsibilityUserId,
            }).Where(f => f.MissionId == id).FirstOrDefault();

            

            return Json(editedMissionSelected);
        }

        // DELETE api/Missions/5
        

        public HttpResponseMessage Delete(int missionId)
        {
            using (var context = new MissionsManagementDBEntitiesEntities())
            {
                var mission = context.Missions
                    .Where(s => s.MissionId == missionId)
                    .FirstOrDefault();
                try
                {
                    context.Entry(mission).State = System.Data.Entity.EntityState.Deleted;
                    context.SaveChanges();
                }
                catch
                {
                    if(mission==null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Conflict, "Mission is not exists, missionId:"+ missionId);
                    }
                    
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Mission had been deleted!");
        }
    }
}
