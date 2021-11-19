using System;

namespace Heracles.Application.Attributes
{
    public class ActivityTitleTextAttribute : Attribute
    {
        public string TitleText { get; }
        public ActivityTitleTextAttribute(string titleText)
        {
            TitleText = titleText;
        }
    }
}
