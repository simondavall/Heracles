using System;
using Heracles.Application.Enums;

namespace Heracles.Web.Models
{
    public class SubNavigationViewModel
    {
        public SubNavigationViewModel()
        {
            InitializeTabSelected();
        }

        public string ActiveSince { get; set; }
        public string Username  { get; set; }
        public string[] TabSelected { get; private set; }

        public void SetSelectedTab(SubNavTab tabSelected)
        {
            InitializeTabSelected();
            TabSelected[(int)tabSelected] = "selected";
        }

        private void InitializeTabSelected()
        {
            TabSelected = new[] { "", "", "" };
        }
    }
}
