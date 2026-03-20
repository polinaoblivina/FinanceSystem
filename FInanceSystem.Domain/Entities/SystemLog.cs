using System;
using FInanceSystem.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Entities
{
    public class SystemLog
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public ActionType Action { get; private set; }
        public Guid? TargetId { get; private set; }
        public LogTargetType? TargetType { get; private set; }
        public DateTime Timestamp { get; private set; }

        private SystemLog() { }
        private SystemLog(Guid id, Guid userId, ActionType action, Guid? targetId, LogTargetType? logTargetType, DateTime timestamp)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid log id", nameof(id));

            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user id", nameof(userId));

            Id = id;
            UserId = userId;
            Action = action;
            TargetId = targetId;
            TargetType = logTargetType;
            Timestamp = timestamp;
        }
        public static SystemLog Create(Guid userId, ActionType action, Guid? targetId = null, LogTargetType? logTargetType = null)
        {
            return new SystemLog(Guid.NewGuid(), userId, action, targetId, logTargetType, DateTime.UtcNow);
        }
    }
}
