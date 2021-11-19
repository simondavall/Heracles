using System;
using System.Linq;
using System.Reflection;
using Heracles.Application.Attributes;
using Heracles.Application.Enums;

namespace Heracles.Application.Extensions
{
    public static class ActivityTypeExtensions
    {
        public static string GetImagePath(this ActivityType activityType)
        {
            var memberInfo = GetMemberInfo(activityType);

            var customAttributes = memberInfo.GetCustomAttributes<ActivityTitleImageAttribute>();
            var imagePathAttribute = customAttributes.FirstOrDefault();

            if (imagePathAttribute == null)
            {
                throw new InvalidOperationException($"ActivityType of {activityType} has no ActivityTitleImageAttribute");
            }

            return imagePathAttribute.ImagePath;
        }

        public static string GetTitleText(this ActivityType activityType)
        {
            var memberInfo = GetMemberInfo(activityType);

            var customAttributes = memberInfo.GetCustomAttributes<ActivityTitleTextAttribute>();
            var titleTextAttribute = customAttributes.FirstOrDefault();

            if (titleTextAttribute == null)
            {
                throw new InvalidOperationException($"ActivityType of {activityType} has no ActivityTitleImageAttribute");
            }

            return titleTextAttribute.TitleText;
        }

        private static MemberInfo GetMemberInfo(ActivityType activityType)
        {
            var activityTypeType = typeof(ActivityType);

            var activityTypeName = Enum.GetName(activityTypeType, activityType);
            if (activityTypeName is null)
            {
                throw new ArgumentException($"ActivityType of {activityType} does not exist");
            }

            var memberInfo = activityTypeType.GetMember(activityTypeName);
            if (memberInfo.Length != 1)
            {
                throw new ArgumentException($"ActivityType of {activityType} should only have one memberInfo");
            }

            return memberInfo[0];
        }
    }
}
