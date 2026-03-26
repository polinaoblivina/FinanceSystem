using System;
using FInanceSystem.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FInanceSystem.Domain.Utils;

namespace FInanceSystem.Domain.Entities
{
    public class SystemLog
    {
        public Guid Id { get; private set; }
        public string? Number { get; private set; }
        public Guid UserId { get; private set; }
        public ActionType Action { get; private set; }
        public Guid? TargetId { get; private set; }
        public LogTargetType? TargetType { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string? FromInfo { get; private set; }   
        public string? ToInfo { get; private set; }        
        public decimal? Amount { get; private set; }      

        private SystemLog() { }
        private SystemLog(Guid id, string number, Guid userId, ActionType action, Guid? targetId, LogTargetType? logTargetType, DateTime timestamp, string? fromInfo, string? toInfo, decimal? amount)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid log id", nameof(id));
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Invalid account number", nameof(number));
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user id", nameof(userId));

            Id = id;
            Number = number;
            UserId = userId;
            Action = action;
            TargetId = targetId;
            TargetType = logTargetType;
            Timestamp = timestamp;
            FromInfo = fromInfo;
            ToInfo = toInfo;
            Amount = amount;
        }
        public static SystemLog Create(Guid userId, ActionType action, Guid? targetId = null, LogTargetType? logTargetType = null)
        {
            return new SystemLog(Guid.NewGuid(), FInanceSystem.Domain.Utils.Utils.GenerateLogNumber(), userId, action, targetId, logTargetType, DateTime.UtcNow, null, null, null);
        }
        public static SystemLog CreateTransferLog(Guid userId, ActionType action,Guid targetId, LogTargetType targetType,string fromInfo, string toInfo, decimal amount)
        {
            return new SystemLog(Guid.NewGuid(), FInanceSystem.Domain.Utils.Utils.GenerateLogNumber(),userId, action, targetId, targetType, DateTime.UtcNow, fromInfo, toInfo, amount);
        }
        public static SystemLog CreateRevertLog(Guid adminId, string originalLogNumber, string originalAction)
        {
            return new SystemLog(Guid.NewGuid(), FInanceSystem.Domain.Utils.Utils.GenerateLogNumber(), adminId ,ActionType.Revert, null, null, DateTime.UtcNow, null,$"Reverted action: {originalAction} (log: {originalLogNumber})", null);
        }
    }
}
