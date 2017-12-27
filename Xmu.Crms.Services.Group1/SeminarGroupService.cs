using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.Insomnia
{
    public class SeminarGroupService : ISeminarGroupService
    {
        private readonly CrmsContext _db;

        public SeminarGroupService(CrmsContext db) => _db = db;

        public void DeleteSeminarGroupMemberBySeminarGroupId(long seminarGroupId)
        {
            _db.RemoveRange(_db.SeminarGroupMember.Where(s => s.SeminarGroup.Id == seminarGroupId));
            _db.SaveChanges();
        }

        public long InsertSeminarGroupMemberById(long userId, long groupId)
        {
            if (userId < 0 && groupId < 0)
            {
                throw new ArgumentException();
            }

            var group = _db.SeminarGroup.SingleOrDefault(s => s.Id == groupId);
            if (group == null)
            {
                throw new GroupNotFoundException();
            }

            var student = _db.UserInfo.SingleOrDefault(u => u.Id == userId);

            if (student == null)
            {
                throw new UserNotFoundException();
            }

            var isExist = _db.SeminarGroupMember.Include(sg => sg.SeminarGroup).Include(sg => sg.Student)
                .Where(sg => sg.SeminarGroup.Id == groupId && sg.Student.Id == userId);
            if (isExist.Any())
            {
                throw new InvalidOperationException();
            }

            var seminargroup = _db.SeminarGroupMember.Add(new SeminarGroupMember
            {
                SeminarGroup = group,
                Student = student
            });
            _db.SaveChanges();

            return seminargroup.Entity.Id;
        }

        public IList<UserInfo> ListSeminarGroupMemberByGroupId(long groupId)
        {
            if (groupId < 0)
            {
                throw new ArgumentException();
            }

            var group = _db.SeminarGroup.SingleOrDefault(s => s.Id == groupId);
            if (group == null)
            {
                throw new GroupNotFoundException();
            }

            return _db.SeminarGroupMember
                .Include(s => s.Student)
                .Include(s => s.SeminarGroup)
                .Where(s => s.SeminarGroup.Id == groupId)
                .Select(s => s.Student)
                .ToList();
        }

        public IList<SeminarGroup> ListSeminarGroupIdByStudentId(long userId)
        {
            if (userId < 0)
            {
                throw new ArgumentException();
            }

            var user = _db.SeminarGroup.SingleOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new GroupNotFoundException();
            }

            return _db.SeminarGroupMember.Include(s => s.Student).Include(s => s.SeminarGroup)
                .Where(s => s.Student.Id == userId)
                .Select(s => s.SeminarGroup).ToList();
        }

        public long GetSeminarGroupLeaderByGroupId(long groupId)
        {
            if (groupId < 0)
            {
                throw new ArgumentException();
            }

            var group = _db.SeminarGroup.Include(s => s.Leader).SingleOrDefault(s => s.Id == groupId);
            if (group == null)
            {
                throw new GroupNotFoundException();
            }
            if (group.Leader == null) return -1;
            return group.Leader.Id;
        }

        public IList<SeminarGroup> ListSeminarGroupBySeminarId(long seminarId)
        {
            if (seminarId < 0)
            {
                throw new ArgumentException();
            }

            var seminar = _db.Seminar.SingleOrDefault(s => s.Id == seminarId);
            if (seminar == null)
            {
                throw new SeminarNotFoundException();
            }

            return _db.SeminarGroup.Include(s => s.ClassInfo).Where(s => s.Seminar.Id == seminarId).ToList();
        }

        public void DeleteSeminarGroupBySeminarId(long seminarId)
        {
            if (seminarId < 0)
            {
                throw new ArgumentException();
            }

            _db.SeminarGroup.RemoveRange(_db.SeminarGroup.Where(s => s.Seminar.Id == seminarId));
            _db.SaveChanges();
        }

        public long InsertSeminarGroupBySeminarId(long seminarId, long classId, SeminarGroup seminarGroup)
        {
            if (seminarId < 0)
            {
                throw new ArgumentException();
            }

            var seminarinfo = _db.Seminar.Find(seminarId) ?? throw new SeminarNotFoundException();
            var classinfo = _db.ClassInfo.Find(classId) ?? throw new ClassNotFoundException();
            seminarGroup.Seminar = seminarinfo;
            seminarGroup.ClassInfo = classinfo;
            var group = _db.SeminarGroup.Add(seminarGroup);
            _db.SaveChanges();
            return group.Entity.Id;
        }

        public long InsertSeminarGroupMemberByGroupId(long groupId, SeminarGroupMember seminarGroupMember)
        {
            if (groupId < 0)
            {
                throw new ArgumentException();
            }

            var group = _db.SeminarGroup.Find(groupId);
            seminarGroupMember.SeminarGroup = group;
            var member = _db.SeminarGroupMember.Add(seminarGroupMember);
            _db.SaveChanges();
            return member.Entity.Id;
            //throw new NotImplementedException();
        }

        public void DeleteSeminarGroupByGroupId(long seminarGroupId)
        {
            if (seminarGroupId < 0)
            {
                throw new ArgumentException();
            }

            _db.SeminarGroup.RemoveRange(_db.SeminarGroup.Where(s => s.Id == seminarGroupId));
            _db.SaveChanges();
            //throw new NotImplementedException();
        }

        public SeminarGroup GetSeminarGroupByGroupId(long groupId)
        {
            if (groupId < 0)
            {
                throw new ArgumentException();
            }

            var group = _db.SeminarGroup
                .Include(s => s.ClassInfo)
                .SingleOrDefault(sg => sg.Id == groupId);
            if (group == null)
            {
                throw new GroupNotFoundException();
            }

            return group;
        }

        public long GetSeminarGroupLeaderById(long userId, long seminarId)
        {
            if (userId < 0 || seminarId < 0)
            {
                throw new ArgumentException();
            }

            var seminarmember = _db.SeminarGroupMember
                .Include(s => s.Student)
                .Include(s => s.SeminarGroup)
                .ThenInclude(sem => sem.Seminar)
                .Where(s => s.Student.Id == userId)
                .SingleOrDefault(sg => sg.SeminarGroup.Seminar.Id == seminarId);
            if (seminarmember != null)
            {
                return seminarmember.SeminarGroup.Leader.Id;
            }

            return -1;
        }

        public void AutomaticallyGrouping(long seminarId, long classId)
        {
            Seminar seminar = _db.Seminar.SingleOrDefault(s => s.Id == seminarId);
            ClassInfo classInfo = _db.ClassInfo.SingleOrDefault(s => s.Id == classId);
            List<Attendance> attendlist = _db.Attendences.Where(s => s.Seminar.Id == seminarId && s.ClassInfo.Id == classId).Include(s => s.Student).ToList<Attendance>();
            List<Topic> topiclist = _db.Topic.Where(t => t.Seminar.Id == seminarId).ToList<Topic>();
            int group = 0;
            foreach(var t in topiclist)
            {
                group += t.GroupNumberLimit;
            }
            int student = attendlist.Count();
            for(int i=0;i<group;i++)
            {
                SeminarGroup seminarGroup = new SeminarGroup()
                {
                    Seminar = seminar,
                    ClassInfo = classInfo
                };
                _db.SeminarGroup.Add(seminarGroup);
                for(int j=0;j<student;j++)
                {
                    if(j%group==i)
                    {
                        SeminarGroupMember seminarGroupMember = new SeminarGroupMember()
                        {
                            SeminarGroup = seminarGroup,
                            Student = _db.UserInfo.SingleOrDefault(s => s.Id == attendlist[j].Student.Id)
                        };
                        _db.SeminarGroupMember.Add(seminarGroupMember);
                    }
                }
            }
            _db.SaveChanges();
        }

        public void AutomaticallyAllotTopic(long seminarId)
        {
            throw new NotImplementedException();
        }

        public SeminarGroup GetSeminarGroupById(long seminarId, long userId)
        {
            if (userId < 0 || seminarId < 0)
            {
                throw new ArgumentException();
            }

            var seminar = _db.Seminar.Find(seminarId);
            if (seminar == null)
            {
                throw new SeminarNotFoundException();
            }

            var user = _db.UserInfo.Find(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var seminarmember = _db.SeminarGroupMember.Include(s => s.Student).Include(s => s.SeminarGroup)
                .ThenInclude(sem => sem.Seminar).Where(s => s.Student.Id == userId)
                .SingleOrDefault(sg => sg.SeminarGroup.Seminar.Id == seminarId);
            if (seminarmember == null)
            {
                throw new InvalidOperationException();
            }

            return seminarmember.SeminarGroup;
        }

        public IList<SeminarGroup> ListGroupByTopicId(long topicId)
        {
            if (topicId < 0)
            {
                throw new ArgumentException();
            }

            _db.SaveChanges();
            return _db.SeminarGroupTopic.Include(s => s.Topic).Include(s => s.SeminarGroup)
                .Where(s => s.Topic.Id == topicId)
                .Select(sg => sg.SeminarGroup)
                .ToList();
        }

        public string InsertTopicByGroupId(long groupId, long topicId)
        {
            if (groupId < 0 || topicId < 0)
            {
                throw new ArgumentException();
            }

            var group = _db.SeminarGroup.Find(groupId);
            if (group == null)
            {
                throw new GroupNotFoundException();
            }

            var topic = _db.Topic.Find(topicId);
            _db.SeminarGroupTopic.Add(new SeminarGroupTopic
            {
                Topic = topic,
                SeminarGroup = group
            });
            _db.SaveChanges();
            return null;
        }

        public void AssignLeaderById(long groupId, long userId)
        {
            if (groupId < 0 || userId < 0)
            {
                throw new ArgumentException();
            }

            var user = _db.UserInfo.Find(userId) ?? throw new UserNotFoundException();
            var group = _db.SeminarGroup.Include(s => s.Leader).SingleOrDefault(s => s.Id == groupId) ??
                        throw new GroupNotFoundException();
            if (group.Leader != null)
            {
                throw new InvalidOperationException();
            }

            group.Leader = user;
            _db.SaveChanges();
            //throw new NotImplementedException();
        }

        public void ResignLeaderById(long groupId, long userId)
        {
            if (groupId < 0 || userId < 0)
            {
                throw new ArgumentException();
            }

            var user = _db.UserInfo.Find(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var group = _db.SeminarGroup.Include(sg => sg.Leader).SingleOrDefault(s => s.Id == groupId);
            if (group == null)
            {
                throw new GroupNotFoundException();
            }

            if (group.Leader != user)
            {
                throw new InvalidOperationException();
            }

            group.Leader = null;
            _db.SaveChanges();
        }

        public void DeleteTopicByGroupId(long groupId)
        {
            if (groupId < 0)
            {
                throw new ArgumentException();
            }

            var group = _db.SeminarGroup.Find(groupId);
            if (group == null)
            {
                throw new GroupNotFoundException();
            }

            _db.SeminarGroupTopic.RemoveRange(_db.SeminarGroupTopic.Include(s => s.SeminarGroup)
                .Where(s => s.SeminarGroup.Id == groupId));
            _db.SaveChanges();
        }

        public void DeleteSeminarGroupMemberById(long userid,long groupid)
        {
            SeminarGroupMember seminarGroupMember = _db.SeminarGroupMember.Where(s => s.Student.Id == userid && s.SeminarGroup.Id == groupid).SingleOrDefault<SeminarGroupMember>();
            _db.SeminarGroupMember.Remove(seminarGroupMember);
            _db.SaveChanges();
        }
    }
}