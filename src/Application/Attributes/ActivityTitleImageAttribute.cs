using System;

namespace Heracles.Application.Attributes
{
    public class ActivityTitleImageAttribute : Attribute
    {
        public string ImagePath { get; }
        public ActivityTitleImageAttribute(string imagePath)
        {
            ImagePath = imagePath;
        }
    }
}